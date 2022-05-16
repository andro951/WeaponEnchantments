using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using WeaponEnchantments.Items;
using Terraria.ModLoader.IO;
using static WeaponEnchantments.Items.Enchantments;
using Terraria.ID;
using System;
using Terraria.Audio;
using Terraria.DataStructures;
using static WeaponEnchantments.Items.Containment;

namespace WeaponEnchantments.Common.Globals
{
    public class EnchantedItem : GlobalItem
    {
        public Item[] enchantments = new Item[EnchantingTable.maxEnchantments];//Track enchantment items on a weapon/armor/accessory item
        
        public int experience;//current experience of a weapon/armor/accessory item
        //public float lastSpeedBonus;
        public int lastUseTimeBonusInt;
        public int lastUseAnimationBonusInt;
        public float lastSizeBonus;
        public int lastValueBonus;
        public int lastDefenceBonus;
        public int lastManaCostBonus;
        public float lastEquipManaCostBonus;
        public float lastAmmoCostBonus;
        public float lastEquipAmmoCostBonus;
        public float lastScaleBonus;
        public float lastGenericScaleBonus;
        public float lifeSteal;
        public float lastLifeStealBonus;
        public int lastArmorPenetrationBonus;
        public bool allForOne;
        public bool oneForAll;
        public bool spelunker = false;
        public bool equip;
        public bool heldItem = false;
        public int levelBeforeBooster;
        public int level;
        public bool powerBoosterInstalled;//Tracks if Power Booster is installed on item +10 levels to spend on enchantments (Does not affect experience)
        public bool inEnchantingTable;
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
        public override void UpdateEquip(Item item, Player player)
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            float damageModifier = 0f;
            float speedModifier = 0f;
            int defenceBonus = 0;
            float criticalBonus = 0f;
            float knockbackBonus = 0f;
            float armorPenetrationBonus = 0f;
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
            {
                if (!enchantments[i].IsAir)
                {
                    float str = ((Enchantments)enchantments[i].ModItem).enchantmentStrength;
                    switch (((Enchantments)enchantments[i].ModItem).enchantmentType)
                    {
                        case EnchantmentTypeIDs.Damage:
                            damageModifier += str;
                            break;
                        case EnchantmentTypeIDs.Speed:
                            speedModifier += str;
                            break;
                        case EnchantmentTypeIDs.Defence:
                            defenceBonus += (int)Math.Round(str);
                            break;
                        case EnchantmentTypeIDs.Critical:
                            criticalBonus += str;
                            break;
                        case EnchantmentTypeIDs.Size:
                            knockbackBonus += str;
                            break;
                        case EnchantmentTypeIDs.ArmorPenetration:
                            armorPenetrationBonus += str;
                            break;
                    }
                }
            }
            player.GetDamage(DamageClass.Generic) += damageModifier / 4;
            player.GetAttackSpeed(DamageClass.Generic) += speedModifier / 4;
            player.GetCritChance(DamageClass.Generic) += criticalBonus * 25;
            player.GetKnockback(DamageClass.Generic) += knockbackBonus / 2;
            player.GetArmorPenetration(DamageClass.Generic) += armorPenetrationBonus / 4;
            item.defense += defenceBonus - lastDefenceBonus;
            lastDefenceBonus = defenceBonus;
        }
        public override void UpdateInventory(Item item, Player player)
        {
            int value = 0;
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
            {
                value += enchantments[i].value;
            }
            int powerBoosterValue = powerBoosterInstalled ? ModContent.GetModItem(ModContent.ItemType<PowerBooster>()).Item.value : 0;
            item.value += value + 16 * experience + powerBoosterValue - lastValueBonus;//Update items value based on enchantments installed
            lastValueBonus = value + 16 * experience + powerBoosterValue;
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
                if (!enchantments[i].IsAir) 
                { 
                    total += ((Enchantments)enchantments[i].ModItem).GetLevelCost();
                }
            }
            return level - total;
        }
        public override void LoadData(Item item, TagCompound tag)
        {
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
            {
                if (tag.Get<Item>("enchantments" + i.ToString()) != null)
                {
                    if (!tag.Get<Item>("enchantments" + i.ToString()).IsAir)
                    {
                        enchantments[i] = tag.Get<Item>("enchantments" + i.ToString()).Clone();
                    }
                    else
                    {
                        enchantments[i] = new Item();
                    }
                }
            }//Load enchantment item tags
            experience = tag.Get<int>("experience");//Load experience tag
            powerBoosterInstalled = tag.Get<bool>("powerBooster");//Load status of powerBoosterInstalled
            UpdateLevel();
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
            float modifier = 0f;
            float speedModifier = 0f;
            int defenceBonus = 0;
            float manaCostBonus = 0f;
            float lifeStealBonus = 0f;
            float armorPenetrationBonus = 0f;
            allForOne = false;
            oneForAll = false;
            spelunker = false;
            float oneForAllBonus = 1f;
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
            {
                if (!enchantments[i].IsAir)
                {
                    float str = ((Enchantments)enchantments[i].ModItem).enchantmentStrength;
                    switch (((Enchantments)enchantments[i].ModItem).enchantmentType)
                    {
                        case EnchantmentTypeIDs.Damage:
                            modifier += str * damage.Multiplicative;
                            break;
                        case EnchantmentTypeIDs.Speed:
                            speedModifier += str;
                            break;
                        case EnchantmentTypeIDs.Defence:
                            defenceBonus += (int)Math.Round(str);
                            break;
                        case EnchantmentTypeIDs.ManaCost:
                            manaCostBonus += str;
                            break;
                        case EnchantmentTypeIDs.LifeSteal:
                            lifeStealBonus += str;
                            break;
                        case EnchantmentTypeIDs.ArmorPenetration:
                            armorPenetrationBonus += str;
                            break;
                        case EnchantmentTypeIDs.AllForOne:
                            allForOne = true;
                            break;
                        case EnchantmentTypeIDs.OneForAll:
                            oneForAllBonus = 0.7f;
                            oneForAll = true;
                            break;
                        case EnchantmentTypeIDs.Spelunker:
                            spelunker = true;
                            break;
                    }
                }
            }
            if (modifier > 0 && ContentSamples.ItemsByType[item.type].damage * (1 + modifier) * damage.Multiplicative - ContentSamples.ItemsByType[item.type].damage < 1)
            {
                damage.Flat += 1;
            }
            else
            {
                damage += modifier * damage.Multiplicative;
            }
            if (allForOne) { damage.Base = (damage.Base + ContentSamples.ItemsByType[item.type].damage) * 10; }
            //damage = allForOne ? damage * 10 : damage;
            if (player.HeldItem == item)
            {
                player.statDefense += (int)Math.Round((float)defenceBonus / 2);
            }
            item.useTime += (int)Math.Round((float)ContentSamples.ItemsByType[item.type].useTime * (1f / (1f + speedModifier) / oneForAllBonus - 1f)) - lastUseTimeBonusInt;
            lastUseTimeBonusInt = (int)Math.Round((float)ContentSamples.ItemsByType[item.type].useTime * (1f / (1f + speedModifier) / oneForAllBonus - 1f));
            item.useAnimation += (int)Math.Round((float)ContentSamples.ItemsByType[item.type].useAnimation * (1f / (1f + speedModifier) / oneForAllBonus - 1f)) - lastUseAnimationBonusInt;
            lastUseAnimationBonusInt = (int)Math.Round((float)ContentSamples.ItemsByType[item.type].useAnimation * (1f / (1f + speedModifier) / oneForAllBonus - 1f));
            item.scale += wePlayer.itemScale - lastGenericScaleBonus;
            lastGenericScaleBonus = wePlayer.itemScale;
            item.mana -= (int)Math.Round((float)ContentSamples.ItemsByType[item.type].mana * (manaCostBonus + wePlayer.manaCost)) - lastManaCostBonus;
            lastManaCostBonus = (int)Math.Round((float)ContentSamples.ItemsByType[item.type].mana * (manaCostBonus + wePlayer.manaCost));
            lifeSteal = lifeStealBonus;
            item.ArmorPenetration += (int)armorPenetrationBonus - lastArmorPenetrationBonus;
            lastArmorPenetrationBonus = (int)armorPenetrationBonus;
        }
        public override void ModifyWeaponCrit(Item item, Player player, ref float crit)
        {
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
            {
                if (!enchantments[i].IsAir && ((Enchantments)enchantments[i].ModItem).enchantmentType == EnchantmentTypeIDs.Critical)
                {
                    crit += (int)(((Enchantments)enchantments[i].ModItem).enchantmentStrength * 100);
                }
            }
        }
        public override void ModifyWeaponKnockback(Item item, Player player, ref StatModifier knockback)
        {
            float scale = 0f;
            //int value = 0;
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
            {
                if (!enchantments[i].IsAir && ((Enchantments)enchantments[i].ModItem).enchantmentType == EnchantmentTypeIDs.Size)
                {
                    knockback += (int)(((Enchantments)enchantments[i].ModItem).enchantmentStrength * 100);
                    scale += ((Enchantments)enchantments[i].ModItem).enchantmentStrength / 2;//Only do 50% of enchantmentStrength to size
                }
                //value += enchantments[i].value;
            }
            item.scale += scale - lastSizeBonus;//Update item size
            lastSizeBonus = scale;
            //item.value += value + 16 * experience - lastValueBonus;//Update items value based on enchantments installed
            //lastValueBonus = value + 16 * experience;
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
            }
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
            {
                if (!enchantments[i].IsAir)
                {
                    if (!enchantmentsToolTipAdded)
                    {
                        tooltips.Add(new TooltipLine(Mod, "enchantmentsToolTip", "Enchantments:") { OverrideColor = Color.Violet});
                        enchantmentsToolTipAdded = true;
                    }//Enchantmenst: tooltip
                    if(WEMod.IsWeaponItem(item))
                    {
                        switch (((Enchantments)enchantments[i].ModItem).enchantmentType)
                        {
                            case EnchantmentTypeIDs.Size:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "+" + ((((Enchantments)enchantments[i].ModItem).enchantmentStrength * 50)).ToString() + "% " + ((Enchantments)enchantments[i].ModItem).enchantmentTypeName)
                                {
                                    OverrideColor = Enchantments.rarityColors[((Enchantments)enchantments[i].ModItem).enchantmentSize]
                                });
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString() + "-2", "+" + ((((Enchantments)enchantments[i].ModItem).enchantmentStrength * 100)).ToString() + "% Knockback")
                                {
                                    OverrideColor = Enchantments.rarityColors[((Enchantments)enchantments[i].ModItem).enchantmentSize]
                                });
                                break;
                            case EnchantmentTypeIDs.Defence:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "+" + (((Enchantments)enchantments[i].ModItem).enchantmentStrength / 2).ToString() + " " + ((Enchantments)enchantments[i].ModItem).enchantmentTypeName)
                                {
                                    OverrideColor = Enchantments.rarityColors[((Enchantments)enchantments[i].ModItem).enchantmentSize]
                                });
                                break;
                            case EnchantmentTypeIDs.ArmorPenetration:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "+" + ((Enchantments)enchantments[i].ModItem).enchantmentStrength.ToString() + " Armor Penetration")
                                {
                                    OverrideColor = Enchantments.rarityColors[((Enchantments)enchantments[i].ModItem).enchantmentSize]
                                });
                                break;
                            case EnchantmentTypeIDs.ManaCost:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "-" + ((((Enchantments)enchantments[i].ModItem).enchantmentStrength * 100)).ToString() + "% Mana Cost")
                                {
                                    OverrideColor = Enchantments.rarityColors[((Enchantments)enchantments[i].ModItem).enchantmentSize]
                                });
                                break;
                            case EnchantmentTypeIDs.AmmoCost:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "-" + ((((Enchantments)enchantments[i].ModItem).enchantmentStrength * 100)).ToString() + "% Chance to consume ammo")
                                {
                                    OverrideColor = Enchantments.rarityColors[((Enchantments)enchantments[i].ModItem).enchantmentSize]
                                });
                                break;
                            case EnchantmentTypeIDs.LifeSteal:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), (((Enchantments)enchantments[i].ModItem).enchantmentStrength * 100).ToString() + "% Life Steal")
                                {
                                    OverrideColor = Enchantments.rarityColors[((Enchantments)enchantments[i].ModItem).enchantmentSize]
                                });
                                break;
                            case EnchantmentTypeIDs.AllForOne:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "10x Damage, item CD equal to 8x use speed")
                                {
                                    OverrideColor = Enchantments.rarityColors[((Enchantments)enchantments[i].ModItem).enchantmentSize]
                                });
                                break;
                            case EnchantmentTypeIDs.OneForAll:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "Hiting an enemy will damage all nearby enemies, 0.7x attack speed\n(WARNING - DESTROYS PROJECTILES ON HIT)")
                                {
                                    OverrideColor = Enchantments.rarityColors[((Enchantments)enchantments[i].ModItem).enchantmentSize]
                                });
                                break;
                            case EnchantmentTypeIDs.Spelunker:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "Spelunker buff")
                                {
                                    OverrideColor = Enchantments.rarityColors[((Enchantments)enchantments[i].ModItem).enchantmentSize]
                                });
                                break;
                            default:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "+" + ((((Enchantments)enchantments[i].ModItem).enchantmentStrength * 100)).ToString() + "% " + ((Enchantments)enchantments[i].ModItem).enchantmentTypeName)
                                {
                                    OverrideColor = Enchantments.rarityColors[((Enchantments)enchantments[i].ModItem).enchantmentSize]
                                });
                                break;
                        }
                    }
                    else
                    {
                        switch (((Enchantments)enchantments[i].ModItem).enchantmentType)
                        {
                            case EnchantmentTypeIDs.Size:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "+" + ((((Enchantments)enchantments[i].ModItem).enchantmentStrength * 50 / 2)).ToString() + "% " + ((Enchantments)enchantments[i].ModItem).enchantmentTypeName)
                                {
                                    OverrideColor = Enchantments.rarityColors[((Enchantments)enchantments[i].ModItem).enchantmentSize]
                                });
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString() + "-2", "+" + ((((Enchantments)enchantments[i].ModItem).enchantmentStrength * 100 / 2)).ToString() + "% Knockback")
                                {
                                    OverrideColor = Enchantments.rarityColors[((Enchantments)enchantments[i].ModItem).enchantmentSize]
                                });
                                break;
                            case EnchantmentTypeIDs.Defence:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "+" + ((Enchantments)enchantments[i].ModItem).enchantmentStrength.ToString() + " " + ((Enchantments)enchantments[i].ModItem).enchantmentTypeName)
                                {
                                    OverrideColor = Enchantments.rarityColors[((Enchantments)enchantments[i].ModItem).enchantmentSize]
                                });
                                break;
                            case EnchantmentTypeIDs.ArmorPenetration:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "+" + (((Enchantments)enchantments[i].ModItem).enchantmentStrength / 4).ToString() + " Armor Penetration")
                                {
                                    OverrideColor = Enchantments.rarityColors[((Enchantments)enchantments[i].ModItem).enchantmentSize]
                                });
                                break;
                            case EnchantmentTypeIDs.ManaCost:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "-" + (((Enchantments)enchantments[i].ModItem).enchantmentStrength * 100 / 4).ToString() + "% Mana Cost")
                                {
                                    OverrideColor = Enchantments.rarityColors[((Enchantments)enchantments[i].ModItem).enchantmentSize]
                                });
                                break;
                            case EnchantmentTypeIDs.AmmoCost:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "-" + (((Enchantments)enchantments[i].ModItem).enchantmentStrength * 100 / 4).ToString() + "% Chance to consume ammo")
                                {
                                    OverrideColor = Enchantments.rarityColors[((Enchantments)enchantments[i].ModItem).enchantmentSize]
                                });
                                break;
                            case EnchantmentTypeIDs.LifeSteal:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), (((Enchantments)enchantments[i].ModItem).enchantmentStrength * 100 / 4).ToString() + "% Life Steal")
                                {
                                    OverrideColor = Enchantments.rarityColors[((Enchantments)enchantments[i].ModItem).enchantmentSize]
                                });
                                break;
                            case EnchantmentTypeIDs.Spelunker:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "Spelunker buff")
                                {
                                    OverrideColor = Enchantments.rarityColors[((Enchantments)enchantments[i].ModItem).enchantmentSize]
                                });
                                break;
                            default:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "+" + ((((Enchantments)enchantments[i].ModItem).enchantmentStrength * 100 / 4)).ToString() + "% " + ((Enchantments)enchantments[i].ModItem).enchantmentTypeName)
                                {
                                    OverrideColor = Enchantments.rarityColors[((Enchantments)enchantments[i].ModItem).enchantmentSize]
                                });
                                break;
                        }
                    }
                }
            }//Edit Tooltips
        }
        public void KillNPC(Item item, NPC target)
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            target.GetGlobalNPC<WEGlobalNPC>().xpCalculated = true;
            if (target.type != NPCID.TargetDummy && !target.SpawnedFromStatue && !target.friendly && !target.townNPC)
            {
                int xpInt;
                float multiplier = (1f + ((float)((target.noGravity ? 2f : 0f) + (target.noTileCollide ? 2f : 0f)) + 2f * (1f - target.knockBackResist)) / 10f + (float)target.defDamage / 40f) / (target.boss ? 2f : 1f);
                float effDamage = (float)item.damage * (1f + (float)item.crit / 100f);
                float xp;
                if(target.value > 0)
                {
                    if (effDamage - (float)target.defDefense / 2 > 1)
                    {
                        xp = (float)target.lifeMax * multiplier * effDamage / (effDamage - (float)target.defDefense / 2);
                    }
                    else
                    {
                        xp = (float)target.lifeMax * multiplier * effDamage;
                    }
                    if (item.accessory)
                    {
                        xp /= 2;
                    }
                    xpInt = (int)Math.Round(xp);
                    if (!item.consumable)
                    {
                        Main.NewText(wePlayer.Player.name + " recieved " + xpInt.ToString() + " xp from killing " + target.FullName + ".");
                        GainXP(item, xpInt);
                    }
                    foreach (Item armor in wePlayer.Player.armor)
                    {
                        if (!armor.vanity && !armor.IsAir)
                        {
                            if (armor.GetGlobalItem<EnchantedItem>().levelBeforeBooster < maxLevel)
                            {
                                if (armor.accessory)
                                {
                                    armor.GetGlobalItem<EnchantedItem>().GainXP(armor, xpInt / 2);
                                }
                                else
                                {
                                    armor.GetGlobalItem<EnchantedItem>().GainXP(armor, xpInt);
                                }
                            }
                        }
                    }
                }
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
                if (levelBeforeBooster > currentLevel && item.damage > 0)
                {
                    if(levelBeforeBooster == 40)
                    {
                        SoundEngine.PlaySound(SoundID.Unlock);
                        Main.NewText("Congradulations!  " + wePlayer.Player.name + "'s " + item.Name + " reached the maximum level, " + levelBeforeBooster.ToString() + " (" + WEModSystem.levelXps[levelBeforeBooster - 1] + " xp).");
                    }
                    else
                    {
                        Main.NewText(wePlayer.Player.name + "'s " + item.Name + " reached level " + levelBeforeBooster.ToString() + " (" + WEModSystem.levelXps[levelBeforeBooster - 1] + " xp).");
                    }
                }
            }
        }
        public override void OnCreate(Item item, ItemCreationContext context)
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            for (int j = 0; j < Main.recipe.Length; j++)
            {
                if(Main.recipe[j].createItem.type == item.type)
                {
                    foreach(Item requiredItem in Main.recipe[j].requiredItem)
                    {
                        if(requiredItem.damage > 0)
                        {
                            for(int i = 0; i < 91; i++)
                            {
                                if(wePlayer.inventoryItemRecord[i].type == requiredItem.type)
                                {
                                    experience += wePlayer.inventoryItemRecord[i].GetGlobalItem<EnchantedItem>().experience;
                                    if (wePlayer.inventoryItemRecord[i].GetGlobalItem<EnchantedItem>().powerBoosterInstalled)
                                    {
                                        powerBoosterInstalled = true;
                                    }
                                    WEModSystem.playerInventoryUpdated = false;
                                    WEModSystem.enchantingTableInventoryUpdated = false;
                                    WEModSystem.previousChest = -1;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        public override bool CanConsumeAmmo(Item weapon, Player player)
        {
            float ammoCostBonus = 0f;
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
            {
                if (!enchantments[i].IsAir)
                {
                    float str = ((Enchantments)enchantments[i].ModItem).enchantmentStrength;
                    switch (((Enchantments)enchantments[i].ModItem).enchantmentType)
                    {
                        case EnchantmentTypeIDs.AmmoCost:
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
                wePlayer.allForOneTimer = item.useTime * 8;
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
                WEGlobalNPC.GetEssenceDropList(npc, out float[] essenceValues, out float[] dropRate, out int baseID, out float hp, out float total);
                for (int i = 0; i < essenceValues.Length; ++i)
                {;
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
                player.QuickSpawnItem(src, ModContent.ItemType<ContainmentFragment>(), (int)((1f + Main.rand.NextFloat()) * value / 10000f));
                if (Main.rand.NextFloat() <  value / 500000f)
                {
                    player.QuickSpawnItem(src, ModContent.ItemType<SuperiorContainment>());
                }
                if (Main.rand.NextFloat() < value / 1000000f)
                {
                    player.QuickSpawnItem(src, ModContent.ItemType<PowerBooster>());
                }
                float chance = WEGlobalNPC.GedDropChance(arg);
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
                else if(itemTypes.Count > 0)
                {
                    if (Main.rand.NextFloat() < chance)
                    {
                        player.QuickSpawnItem(src, itemTypes[0]);
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
    }
}
