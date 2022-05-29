using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using WeaponEnchantments.Items;

namespace WeaponEnchantments.Common.Globals
{
    public class EnchantedItem : GlobalItem
    {
        //Start Packet fields
        public int experience;//current experience of a weapon/armor/accessory item
        public Item[] enchantments = new Item[EnchantingTable.maxEnchantments];//Track enchantment items on a weapon/armor/accessory item
        //End Packet fields

        //Vanilla Player buffs
        //public bool[] vanillaPlayerBuffs = new bool[Enum.GetNames(typeof(WEPlayer.VanillaBoolBuffs)).Length];

        public bool[] statsSet = new bool[EnchantingTable.maxEnchantments];
        public Dictionary<string, StatModifier> statMultipliers = new Dictionary<string, StatModifier>();
        public Dictionary<int, int> potionBuffs = new Dictionary<int, int>();
        //Item specific

        public float totalSpeedBonus;
        public float immunityBonus = 0f;
        public int lastUseTimeBonusInt;
        public int lastUseAnimationBonusInt;
        public int lastShootSpeedBonusInt;
        public float lastGenericScaleBonus;
        public int lastArmorPenetrationBonus;

        //Static Non-Item specific
        public float lifeSteal;
        public float lastLifeStealBonus;
        public bool allForOne;
        public float allForOneBonus = 1f;
        public bool oneForAll;
        public float oneForAllBonus = 0f;
        public float enemySpawnBonus = 1f;
        public float godSlayerBonus = 0f;


        public float damageBonus = 0f;
        public int critBonus = 0;
        public int lastDefenceBonus;
        public int lastManaCostBonus;
        public float lastEquipManaCostBonus;
        public float lastAmmoCostBonus;
        public float lastEquipAmmoCostBonus;

        public int lastValueBonus;
        
        
        
        
        
        
        
        public int levelBeforeBooster;
        public int level;
        public bool powerBoosterInstalled;//Tracks if Power Booster is installed on item +10 levels to spend on enchantments (Does not affect experience)
        public bool inEnchantingTable;
        public bool equip = false;
        public bool heldItem = false;
        public bool favorited = false;
        public bool trashItem = false;
        public const int maxLevel = 40;
        public EnchantedItem()
        {
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++) 
            {
                enchantments[i] = new Item();
            }
        }//Constructor
        public override bool InstancePerEntity => true;
        public override GlobalItem Clone(Item item, Item itemClone)
        {
            EnchantedItem clone = (EnchantedItem)base.Clone(item, itemClone);
            clone.enchantments = (Item[])enchantments.Clone();
            for (int i = 0; i < enchantments.Length; i++)
            {
               clone.enchantments[i] = enchantments[i].Clone();
            }//fixes enchantments being applied to all of an item instead of just the instance
            return clone;
        }
        public static class PacketIDs
        {
            public const byte TransferGlobalItemFields = 0;
        }
        public override void NetSend(Item item, BinaryWriter writer)
        {
            if (!item.IsAir)
            {
                if (WEMod.IsEnchantable(item))
                {
                    writer.Write(PacketIDs.TransferGlobalItemFields);
                    writer.Write(experience);
                    writer.Write(powerBoosterInstalled);
                    for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
                    {
                        writer.Write((short)enchantments[i].type);
                    }
                }
            }
        }
        public override void NetReceive(Item item, BinaryReader reader)
        {
            if (!item.IsAir)
            {
                if (WEMod.IsEnchantable(item))
                {
                    byte type = reader.ReadByte();
                    switch (type)
                    {
                        case PacketIDs.TransferGlobalItemFields:
                            experience = reader.ReadInt32();
                            powerBoosterInstalled = reader.ReadBoolean();
                            for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
                            {
                                enchantments[i] = new Item(reader.ReadUInt16());
                            }
                            break;
                        default:
                            ModContent.GetInstance<WEMod>().Logger.Debug("*NOT RECOGNIZED*\ncase: " + type + "\n*NOT RECOGNIZED*");
                            break;
                    }
                }
            }
        }
        public override void UpdateEquip(Item item, Player player)
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            float damageModifier = 0f;
            float speedModifier = 0f;
            int defenceBonus = 0;
            float criticalBonus = 0f;
            float armorPenetrationBonus = 0f;
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
            {
                AllForOneEnchantmentBasic enchantment = ((AllForOneEnchantmentBasic)enchantments[i].ModItem);
                if (!enchantments[i].IsAir)
                {
                    float str = enchantment.EnchantmentStrength;
                    switch ((EnchantmentTypeID)enchantment.EnchantmentType)
                    {
                        case EnchantmentTypeID.Damage:
                            damageModifier += str;
                            break;
                        case EnchantmentTypeID.Speed:
                            speedModifier += str;
                            break;
                        case EnchantmentTypeID.Defence:
                            defenceBonus += (int)Math.Round(str);
                            break;
                        case EnchantmentTypeID.CriticalStrikeChance:
                            criticalBonus += str;
                            break;
                        case EnchantmentTypeID.ArmorPenetration:
                            armorPenetrationBonus += str;
                            break;
                    }
                }
            }
            player.GetDamage(DamageClass.Generic) += damageModifier / 4;
            player.GetAttackSpeed(DamageClass.Generic) += speedModifier / 4;
            player.GetCritChance(DamageClass.Generic) += criticalBonus * 25;
            player.GetArmorPenetration(DamageClass.Generic) += armorPenetrationBonus / 4;
            item.defense += defenceBonus - lastDefenceBonus;
            lastDefenceBonus = defenceBonus;
        }
        public override void UpdateInventory(Item item, Player player)
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            if(experience > 0 || powerBoosterInstalled)
            {
                int value = 0;
                for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
                {
                    value += enchantments[i].value;
                }
                int powerBoosterValue = powerBoosterInstalled ? ModContent.GetModItem(ModContent.ItemType<PowerBooster>()).Item.value : 0;
                int npcTalking = player.talkNPC != -1 ? Main.npc[player.talkNPC].type : -1;
                int valueToAdd = npcTalking != NPCID.GoblinTinkerer ? value + (int)(EnchantmentEssenceBasic.valuePerXP * experience) + powerBoosterValue : 0;
                item.value += valueToAdd - lastValueBonus;//Update items value based on enchantments installed
                lastValueBonus = valueToAdd;
            }
            equip = false;
            if (wePlayer.stickyFavorited)
            {
                if (item.favorited)
                {
                    if (!favorited && Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftAlt) || Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightAlt))
                    {
                        favorited = true;
                    }
                }//Sticky Favorited
                else
                {
                    if (favorited)
                    {
                        if (!(Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftAlt) || Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightAlt)))
                        {
                            item.favorited = true;
                        }
                        else
                        {
                            favorited = false;
                        }
                    }
                }//Sticky Favorited
            }
        }
        public void UpdateLevel()
        {
            int l;
            for(l = 0; l < maxLevel; l++)
            {
                if(experience < WEModSystem.levelXps[l])
                {
                    level = l + 1;
                    break;
                }
            }
            if (l == maxLevel)
            {
                levelBeforeBooster = maxLevel;
                level = powerBoosterInstalled ? maxLevel + 10 : maxLevel;
            }
            else
            {
                levelBeforeBooster = l;
                level = powerBoosterInstalled ? l + 10 : l;
            }
        }
        public int GetLevelsAvailable()
        {
            UpdateLevel();
            int total = 0;
            for (int i = 0; i < enchantments.Length; i++)
            {
                AllForOneEnchantmentBasic enchantment = ((AllForOneEnchantmentBasic)enchantments[i].ModItem);
                if (!enchantments[i].IsAir) 
                { 
                    total += enchantment.GetLevelCost();
                }
            }
            return level - total;
        }
        public override void LoadData(Item item, TagCompound tag)
        {
            experience = tag.Get<int>("experience");//Load experience tag
            powerBoosterInstalled = tag.Get<bool>("powerBooster");//Load status of powerBoosterInstalled
            UpdateLevel();
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
            {
                if (tag.Get<Item>("enchantments" + i.ToString()) != null)
                {
                    if (!tag.Get<Item>("enchantments" + i.ToString()).IsAir)
                    {
                        enchantments[i] = tag.Get<Item>("enchantments" + i.ToString()).Clone();
                        OldItemManager.ReplaceOldItem(ref enchantments[i]);
                    }
                    else
                    {
                        enchantments[i] = new Item();
                    }
                }
            }//Load enchantment item tags
        }
        public override void SaveData(Item item, TagCompound tag)
        {
            if (enchantments != null)
            {
                for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
                {
                    if (enchantments[i] != null)
                    {
                        if (!enchantments[i].IsAir)
                        {
                            tag["enchantments" + i.ToString()] = enchantments[i].Clone();
                        }
                    }
                }//Save enchantment item tags
            }
            tag["experience"] = experience;//Save experience tag
            tag["powerBooster"] = powerBoosterInstalled;//save status of powerBoosterInstalled
        }
        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            float speedModifier = 0f;
            int defenceBonus = 0;
            float lifeStealBonus = 0f;
            float allForOneSpeedMultiplier = 1f;
            float allForOneImmunityBonus = 1f;
            float oneForAllSpeedMultiplier = 1f;
            float enemySpawnBonusLocal = 1f;
            float allForOneManaBonus = 0f;
            float manaCostBonus = 0f;
            damageBonus = 0f;
            immunityBonus = 0f;
            allForOneBonus = 1f;
            oneForAllBonus = 0f;
            godSlayerBonus = 0f;
            allForOne = false;
            oneForAll = false;
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
            {
                AllForOneEnchantmentBasic enchantment = ((AllForOneEnchantmentBasic)enchantments[i].ModItem);
                if (!enchantments[i].IsAir)
                {
                    float str = enchantment.EnchantmentStrength;
                    switch ((EnchantmentTypeID)enchantment.EnchantmentType)
                    {
                        case EnchantmentTypeID.Damage:
                            damageBonus += str * damage.Multiplicative;
                            break;
                        case EnchantmentTypeID.Speed:
                            speedModifier += str;
                            break;
                        case EnchantmentTypeID.Defence:
                            defenceBonus += (int)Math.Round(str);
                            break;
                        case EnchantmentTypeID.Mana:
                            manaCostBonus += str;
                            break;
                        case EnchantmentTypeID.LifeSteal:
                            lifeStealBonus += str;
                            break;
                        case EnchantmentTypeID.AllForOne:
                            allForOneBonus = str;
                            allForOneSpeedMultiplier = str * 0.2f;
                            allForOneImmunityBonus = str * 0.8f;
                            allForOneManaBonus = str * 0.4f;
                            allForOne = true;
                            break;
                        case EnchantmentTypeID.OneForAll:
                            oneForAllBonus = str;
                            oneForAllSpeedMultiplier = 1f - 0.3f * str;
                            oneForAll = true;
                            break;
                        case EnchantmentTypeID.War:
                            enemySpawnBonusLocal *= 1f + str;
                            break;
                        case EnchantmentTypeID.Peace:
                            enemySpawnBonusLocal /= 1f + str;
                            break;
                        case EnchantmentTypeID.GodSlayer:
                            godSlayerBonus += str;
                            break;
                        case EnchantmentTypeID.Scale:

                            break;
                    }
                }
            }
            if (damageBonus > 0 && ContentSamples.ItemsByType[item.type].damage * (1 + damageBonus) * damage.Multiplicative - ContentSamples.ItemsByType[item.type].damage < 1)
            {
                damageBonus = 1 / (ContentSamples.ItemsByType[item.type].damage);
            }
            damage += damageBonus;
            if (allForOne) 
            { 
                damage.Base = (damage.Base + ContentSamples.ItemsByType[item.type].damage) * allForOneBonus;
            }
            if (player.HeldItem == item)
            {
                player.statDefense += (int)Math.Round((float)defenceBonus / 2);
            }
            immunityBonus = (allForOneImmunityBonus / ((1f + speedModifier) * oneForAllSpeedMultiplier) - 1f);
            if (!item.channel)
            {
                totalSpeedBonus = (allForOneSpeedMultiplier / ((1f + speedModifier) * oneForAllSpeedMultiplier) - 1f);
                item.useTime += (int)Math.Round((float)ContentSamples.ItemsByType[item.type].useTime * totalSpeedBonus) - lastUseTimeBonusInt;
                lastUseTimeBonusInt = (int)Math.Round((float)ContentSamples.ItemsByType[item.type].useTime * totalSpeedBonus);
                item.useAnimation += (int)Math.Round((float)ContentSamples.ItemsByType[item.type].useAnimation * totalSpeedBonus) - lastUseAnimationBonusInt;
                lastUseAnimationBonusInt = (int)Math.Round((float)ContentSamples.ItemsByType[item.type].useAnimation * totalSpeedBonus);
            }
            //item.scale += sizeBonus + wePlayer.itemScale - lastGenericScaleBonus;
            //lastGenericScaleBonus = sizeBonus + wePlayer.itemScale;
            int mana = (int)Math.Round((float)ContentSamples.ItemsByType[item.type].mana * (manaCostBonus + wePlayer.manaCost - (allForOneManaBonus * 0.4f)));
            item.mana -= mana - lastManaCostBonus;
            lastManaCostBonus = mana;
            lifeSteal = lifeStealBonus;
            enemySpawnBonus = enemySpawnBonusLocal;
        }
        public override void ModifyWeaponCrit(Item item, Player player, ref float crit)
        {
            critBonus = 0;
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
            {
                AllForOneEnchantmentBasic enchantment = ((AllForOneEnchantmentBasic)enchantments[i].ModItem);
                if (!enchantments[i].IsAir && (EnchantmentTypeID)enchantment.EnchantmentType == EnchantmentTypeID.CriticalStrikeChance)
                {
                    critBonus += (int)(enchantment.EnchantmentStrength * 100);
                }
            }
            crit += critBonus;
        }
        public override void ModifyWeaponKnockback(Item item, Player player, ref StatModifier knockback)
        {
            /*for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
            {
                AllForOneEnchantmentBasic enchantment = ((AllForOneEnchantmentBasic)enchantments[i].ModItem);
                if (!enchantments[i].IsAir && (EnchantmentTypeID)enchantment.EnchantmentType == EnchantmentTypeID.Knockback)
                {
                    knockback += enchantment.EnchantmentStrength;
                }
            }*/
        }
        public override void ModifyManaCost(Item item, Player player, ref float reduce, ref float mult)
        {
            
        }
        public override void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            bool enchantmentsToolTipAdded = false;
            bool enchantemntInstalled = false;
            UpdateLevel();
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
            {
                if (!enchantments[i].IsAir)
                {
                    enchantemntInstalled = true;
                    break;
                }
            }
            if (experience > 0 || powerBoosterInstalled || inEnchantingTable || enchantemntInstalled)
            {
                if (powerBoosterInstalled)
                {
                    tooltips.Add(new TooltipLine(Mod, "level", "Level: " + levelBeforeBooster.ToString() + " Points available: " + GetLevelsAvailable().ToString() + " (Booster Installed)") { OverrideColor = Color.LightGreen });
                }
                else
                {
                    tooltips.Add(new TooltipLine(Mod, "level", "Level: " + levelBeforeBooster.ToString() + " Points available: " + GetLevelsAvailable().ToString()) { OverrideColor = Color.LightGreen });
                }
                string levelString = level < maxLevel ? " (" + (WEModSystem.levelXps[levelBeforeBooster] - experience).ToString() + " to next level)" : " (Max Level)";
                tooltips.Add(new TooltipLine(Mod, "experience", "Experience: " + experience.ToString() + levelString) { OverrideColor = Color.White });
            }
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
            {
                if (!enchantments[i].IsAir)
                {
                    AllForOneEnchantmentBasic enchantment = ((AllForOneEnchantmentBasic)enchantments[i].ModItem);
                    if (!enchantmentsToolTipAdded)
                    {
                        tooltips.Add(new TooltipLine(Mod, "enchantmentsToolTip", "Enchantments:") { OverrideColor = Color.Violet});
                        enchantmentsToolTipAdded = true;
                    }//Enchantmenst: tooltip
                    if(WEMod.IsWeaponItem(item))
                    {
                        switch ((EnchantmentTypeID)enchantment.EnchantmentType)
                        {
                            case EnchantmentTypeID.Defence:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "+" + (enchantment.EnchantmentStrength / 2).ToString() + " " + enchantment.MyDisplayName)
                                {
                                    OverrideColor = AllForOneEnchantmentBasic.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                            case EnchantmentTypeID.ArmorPenetration:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "+" + enchantment.EnchantmentStrength.ToString() + " Armor Penetration")
                                {
                                    OverrideColor = AllForOneEnchantmentBasic.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                            case EnchantmentTypeID.Mana:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "-" + (enchantment.EnchantmentStrength * 100).ToString() + "% Mana Cost")
                                {
                                    OverrideColor = AllForOneEnchantmentBasic.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                            case EnchantmentTypeID.AmmoCost:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "-" + (enchantment.EnchantmentStrength * 100).ToString() + "% Chance to consume ammo")
                                {
                                    OverrideColor = AllForOneEnchantmentBasic.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                            case EnchantmentTypeID.LifeSteal:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), (enchantment.EnchantmentStrength * 100).ToString() + "% Life Steal")
                                {
                                    OverrideColor = AllForOneEnchantmentBasic.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                            case EnchantmentTypeID.OneForAll:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), (enchantment.EnchantmentStrength * 100).ToString() + " % damage to nearby enemies, -" + (30f * enchantment.EnchantmentStrength).ToString() + "% base attack speed")
                                {
                                    OverrideColor = AllForOneEnchantmentBasic.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                            case EnchantmentTypeID.AllForOne:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), enchantment.EnchantmentStrength + "x Damage, item CD equal to " + enchantment.EnchantmentStrength * 0.8f + "x use speed")
                                {
                                    OverrideColor = AllForOneEnchantmentBasic.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                            case EnchantmentTypeID.Spelunker:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "Spelunker buff")
                                {
                                    OverrideColor = AllForOneEnchantmentBasic.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                            case EnchantmentTypeID.DangerSense:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "Danger Sense buff")
                                {
                                    OverrideColor = AllForOneEnchantmentBasic.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                            case EnchantmentTypeID.Hunter:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "Hunter buff")
                                {
                                    OverrideColor = AllForOneEnchantmentBasic.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                            case EnchantmentTypeID.War:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), (enchantment.EnchantmentStrength + 1f).ToString() + "x enemy spawn rate and max enemies")
                                {
                                    OverrideColor = AllForOneEnchantmentBasic.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                            case EnchantmentTypeID.Peace:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), (1f / (enchantment.EnchantmentStrength + 1f)).ToString() + "x enemy spawn rate and max enemies")
                                {
                                    OverrideColor = AllForOneEnchantmentBasic.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                            case EnchantmentTypeID.GodSlayer:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), (enchantment.EnchantmentStrength * 100).ToString() + "% God Slayer Bonus")
                                {
                                    OverrideColor = AllForOneEnchantmentBasic.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                            default:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "+" + (enchantment.EnchantmentStrength * 100).ToString() + "% " + enchantment.MyDisplayName)
                                {
                                    OverrideColor = AllForOneEnchantmentBasic.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                        }
                    }
                    else
                    {
                        switch ((EnchantmentTypeID)enchantment.EnchantmentType)
                        {
                            case EnchantmentTypeID.Defence:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "+" + enchantment.EnchantmentStrength.ToString() + " " + enchantment.MyDisplayName)
                                {
                                    OverrideColor = AllForOneEnchantmentBasic.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                            case EnchantmentTypeID.ArmorPenetration:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "+" + (enchantment.EnchantmentStrength / 4).ToString() + " Armor Penetration")
                                {
                                    OverrideColor = AllForOneEnchantmentBasic.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                            case EnchantmentTypeID.Mana:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "-" + (enchantment.EnchantmentStrength * 100 / 4).ToString() + "% Mana Cost")
                                {
                                    OverrideColor = AllForOneEnchantmentBasic.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                            case EnchantmentTypeID.AmmoCost:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "-" + (enchantment.EnchantmentStrength * 100 / 4).ToString() + "% Chance to consume ammo")
                                {
                                    OverrideColor = AllForOneEnchantmentBasic.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                            case EnchantmentTypeID.LifeSteal:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), (enchantment.EnchantmentStrength * 100 / 4).ToString() + "% Life Steal")
                                {
                                    OverrideColor = AllForOneEnchantmentBasic.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                            case EnchantmentTypeID.Spelunker:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "Spelunker buff")
                                {
                                    OverrideColor = AllForOneEnchantmentBasic.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                            case EnchantmentTypeID.DangerSense:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "Danger Sense buff")
                                {
                                    OverrideColor = AllForOneEnchantmentBasic.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                            case EnchantmentTypeID.Hunter:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "Hunter buff")
                                {
                                    OverrideColor = AllForOneEnchantmentBasic.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                            case EnchantmentTypeID.War:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), (enchantment.EnchantmentStrength + 1f).ToString() + "x enemy spawn rate and max enemies")
                                {
                                    OverrideColor = AllForOneEnchantmentBasic.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                            case EnchantmentTypeID.Peace:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), (1f / (enchantment.EnchantmentStrength + 1f)).ToString() + "x enemy spawn rate and max enemies")
                                {
                                    OverrideColor = AllForOneEnchantmentBasic.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                            default:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "+" + ((enchantment.EnchantmentStrength * 100 / 4)).ToString() + "% " + enchantment.MyDisplayName)
                                {
                                    OverrideColor = AllForOneEnchantmentBasic.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                        }
                    }
                }
            }//Edit Tooltips
        }
        public void DamageNPC(Item item, Player player, NPC target, int damage, bool crit, bool melee = false)
        {
            target.GetGlobalNPC<WEGlobalNPC>().xpCalculated = true;
            float value;
            switch (Main.netMode)
            {
                case 1:
                    value = ContentSamples.NpcsByNetId[target.type].value;
                    break;
                default:
                    value = target.value;
                    break;
            }
            if (target.type != NPCID.TargetDummy && !target.friendly && !target.townNPC && (value > 0 || !target.SpawnedFromStatue && target.lifeMax > 5))
            {
                int xpInt;
                int xpDamage;
                float multiplier;
                float effDamage;
                float effDamageDenom;
                float xp;
                multiplier = (1f + ((float)((target.noGravity ? 2f : 0f) + (target.noTileCollide ? 2f : 0f)) + 2f * (1f - target.knockBackResist)) / 10f) / (target.boss ? 4f : 1f);
                effDamage = (float)item.damage * (1f + (float)player.GetWeaponCrit(item) / 100f);
                float actualDefence = target.defense / 2f - target.checkArmorPenetration(player.GetWeaponArmorPenetration(item));
                float actualDamage = melee ? damage : damage - actualDefence;
                actualDamage = crit && !melee ? actualDamage * 2 : actualDamage;
                xpDamage = target.life < 0 ? (int)actualDamage + target.life : (int)actualDamage;
                if(xpDamage > 0)
                {
                    effDamageDenom = effDamage - actualDefence;
                    if (effDamageDenom > 1)
                        xp = (float)xpDamage * multiplier * effDamage / effDamageDenom;
                    else
                        xp = (float)xpDamage * multiplier * effDamage;
                    xp /= UtilityMethods.GetReductionFactor((int)target.lifeMax);
                    xpInt = (int)Math.Round(xp);
                    xpInt = xpInt > 1 ? xpInt : 1;
                    if (!item.consumable)
                    {
                        //ModContent.GetInstance<WEMod>().Logger.Info(wePlayer.Player.name + " recieved " + xpInt.ToString() + " xp from hitting " + target.FullName + ".");
                        //Main.NewText(wePlayer.Player.name + " recieved " + xpInt.ToString() + " xp from killing " + target.FullName + ".");
                        GainXP(item, xpInt);
                    }
                    AllArmorGainXp(xpInt);
                }
            }
        }
        public static void AllArmorGainXp(int xp)
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            int xpInt;
            int i = 0;
            foreach (Item armor in wePlayer.Player.armor)
            {
                if (i < 10)
                {
                    if (!armor.vanity && !armor.IsAir)
                    {
                        if (armor.GetGlobalItem<EnchantedItem>().levelBeforeBooster < maxLevel)
                        {
                            if (WEMod.IsArmorItem(armor))
                            {
                                xpInt = (int)Math.Round(xp / 2f);
                                xpInt = xpInt > 0 ? xpInt : 1;
                                armor.GetGlobalItem<EnchantedItem>().GainXP(armor, xpInt);
                            }
                            else
                            {
                                xpInt = (int)Math.Round(xp / 4f);
                                xpInt = xpInt > 0 ? xpInt : 1;
                                armor.GetGlobalItem<EnchantedItem>().GainXP(armor, xpInt);
                            }
                            //wePlayer.equiptArmor[i].GetGlobalItem<EnchantedItem>().GainXP(wePlayer.equiptArmor[i], xpInt);
                        }
                    }
                }
                i++;
            }
        }
        public void GainXP(Item item, int xpInt)
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            int currentLevel = levelBeforeBooster;
            experience += xpInt;
            if (levelBeforeBooster < maxLevel)
            {
                UpdateLevel();
                if (levelBeforeBooster > currentLevel && wePlayer.usingEnchantingTable)
                {
                    if(levelBeforeBooster == 40)
                    {
                        SoundEngine.PlaySound(SoundID.Unlock);
                        //ModContent.GetInstance<WEMod>().Logger.Info("Congratulations!  " + wePlayer.Player.name + "'s " + item.Name + " reached the maximum level, " + levelBeforeBooster.ToString() + " (" + WEModSystem.levelXps[levelBeforeBooster - 1] + " xp).");
                        Main.NewText("Congratulations!  " + wePlayer.Player.name + "'s " + item.Name + " reached the maximum level, " + levelBeforeBooster.ToString() + " (" + WEModSystem.levelXps[levelBeforeBooster - 1] + " xp).");
                    }
                    else
                    {
                        //ModContent.GetInstance<WEMod>().Logger.Info(wePlayer.Player.name + "'s " + item.Name + " reached level " + levelBeforeBooster.ToString() + " (" + WEModSystem.levelXps[levelBeforeBooster - 1] + " xp).");
                        Main.NewText(wePlayer.Player.name + "'s " + item.Name + " reached level " + levelBeforeBooster.ToString() + " (" + WEModSystem.levelXps[levelBeforeBooster - 1] + " xp).");
                    }
                }
            }
        }
        public override bool CanConsumeAmmo(Item weapon, Player player)
        {
            float ammoCostBonus = 0f;
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
            {
                AllForOneEnchantmentBasic enchantment = ((AllForOneEnchantmentBasic)enchantments[i].ModItem);
                if (!enchantments[i].IsAir)
                {
                    float str = enchantment.EnchantmentStrength;
                    switch ((EnchantmentTypeID)enchantment.EnchantmentType)
                    {
                        case EnchantmentTypeID.AmmoCost:
                            ammoCostBonus += str;
                            break;
                    }
                }
            }
            return Main.rand.NextFloat() >= ammoCostBonus;
        }
        public override bool? UseItem(Item item, Player player)
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            if (allForOne)
            {
                wePlayer.allForOneCooldown = true;
                wePlayer.allForOneTimer = (int)((float)item.useTime * allForOneBonus * 0.4f);
            }
            return null;
        }
        public override bool CanUseItem(Item item, Player player)
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            return allForOne ? (wePlayer.allForOneTimer <= 0 ? true : false) : true;
        }
        public override void OpenVanillaBag(string context, Player player, int arg)
        {
            if(context == "bossBag")
            {
                IEntitySource src = player.GetSource_OpenItem(arg);
                NPC npc = GetNPCFromBossBagType(arg);
                if(npc != null)
                {
                    WEGlobalNPC.GetEssenceDropList(npc, out float[] essenceValues, out float[] dropRate, out int baseID, out float hp, out float total);
                    for (int i = 0; i < essenceValues.Length; ++i)
                    {
                        if (dropRate[i] > 0)
                        {
                            switch (npc.type)
                            {
                                case NPCID.EaterofWorldsHead:
                                    dropRate[i] *= 50f;
                                    break;
                                default:

                                    break;
                            }
                            int stack = Main.rand.NextBool() ? (int)Math.Round(dropRate[i]) : (int)Math.Round(dropRate[i] + 1f);
                            player.QuickSpawnItem(src, baseID + i, stack);
                        }
                    }
                    float value = npc.value;
                    switch (npc.type)
                    {
                        case NPCID.EaterofWorldsHead:
                            value *= 50f;
                            break;
                        default:

                            break;
                    }
                    if (Main.rand.NextFloat() < value / 500000f)
                    {
                        player.QuickSpawnItem(src, ModContent.ItemType<SuperiorContainment>());
                    }
                    if (Main.rand.NextFloat() < value / 1000000f)
                    {
                        player.QuickSpawnItem(src, ModContent.ItemType<PowerBooster>());
                    }
                    float chance = WEGlobalNPC.GetDropChance(arg);
                    List<int> itemTypes = WEGlobalNPC.GetDropItems(arg, true);
                    if (itemTypes.Count > 1)
                    {
                        float randFloat = Main.rand.NextFloat();
                        for (int i = 0; i < itemTypes.Count; i++)
                        {
                            if (randFloat >= (float)i / (float)itemTypes.Count * chance && randFloat < ((float)i + 1f) / (float)itemTypes.Count * chance)
                            {
                                player.QuickSpawnItem(src, itemTypes[i]);
                                break;
                            }
                        }
                    }
                    else if (itemTypes.Count > 0)
                    {
                        if (Main.rand.NextFloat() < chance)
                        {
                            player.QuickSpawnItem(src, itemTypes[0]);
                        }
                    }
                }
            }
        }
        public static NPC GetNPCFromBossBagType(int bossBagType)
        {
            int npcID;
            switch (bossBagType)
            {
                case ItemID.KingSlimeBossBag:
                    npcID = NPCID.KingSlime;
                    break;
                case ItemID.EyeOfCthulhuBossBag:
                    npcID = NPCID.EyeofCthulhu;
                    break;
                case ItemID.EaterOfWorldsBossBag:
                    npcID = NPCID.EaterofWorldsHead;
                    break;
                case ItemID.BrainOfCthulhuBossBag:
                    npcID = NPCID.BrainofCthulhu;
                    break;
                case ItemID.QueenBeeBossBag:
                    npcID = NPCID.QueenBee;
                    break;
                case ItemID.SkeletronBossBag:
                    npcID = NPCID.SkeletronHead;
                    break;
                case ItemID.DeerclopsBossBag:
                    npcID = NPCID.Deerclops;
                    break;
                case ItemID.WallOfFleshBossBag:
                    npcID = NPCID.WallofFlesh;
                    break;
                case ItemID.QueenSlimeBossBag:
                    npcID = NPCID.QueenSlimeBoss;
                    break;
                case ItemID.TwinsBossBag:
                    npcID = NPCID.Retinazer;
                    break;
                case ItemID.DestroyerBossBag:
                    npcID = NPCID.TheDestroyer;
                    break;
                case ItemID.SkeletronPrimeBossBag:
                    npcID = NPCID.SkeletronPrime;
                    break;
                case ItemID.PlanteraBossBag:
                    npcID = NPCID.Plantera;
                    break;
                case ItemID.GolemBossBag:
                    npcID = NPCID.Golem;
                    break;
                case ItemID.FishronBossBag:
                    npcID = NPCID.DukeFishron;
                    break;
                case ItemID.FairyQueenBossBag:
                    npcID = NPCID.HallowBoss;
                    break;
                case ItemID.CultistBossBag:
                    npcID = NPCID.CultistBoss;
                    break;
                case ItemID.MoonLordBossBag:
                    npcID = NPCID.MoonLordCore;
                    break;
                case ItemID.BossBagDarkMage:
                    npcID = NPCID.DD2DarkMageT1;
                    break;
                case ItemID.BossBagOgre:
                    npcID = NPCID.DD2OgreT2;
                    break;
                case ItemID.BossBagBetsy:
                    npcID = NPCID.DD2Betsy;
                    break;
                default:
                    npcID = -1000;
                    break;
            }
            if(npcID != -1000)
            {
                NPC tempNpc = (NPC)ContentSamples.NpcsByNetId[npcID].Clone();
                return tempNpc;
            }
            return null;
        }
        public override bool CanRightClick(Item item)
        {
            if (item.stack > 1)
            {
                if (experience > 0 || powerBoosterInstalled)
                {
                    if (Main.mouseItem.IsAir)
                    {
                        return true;
                    }
                }
            }//Prevent splitting stack of enchantable items with maxstack > 1
            return false;
        }
        public override void RightClick(Item item, Player player)
        {
            if (item.stack > 1)
            {
                if (experience > 0 || powerBoosterInstalled)
                {
                    if (Main.mouseItem.IsAir)
                    {
                        item.stack++;
                    }
                }
            }//Prevent splitting stack of enchantable items with maxstack > 1
        }
        public override void OnHitNPC(Item item, Player player, NPC target, int damage, float knockBack, bool crit)
        {
            DamageNPC(item, player, target, damage, crit, true);
        }
    }
}
