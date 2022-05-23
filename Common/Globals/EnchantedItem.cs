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
        public float damageBonus = 0f;
        public float totalSpeedBonus;
        public float immunityBonus = 0f;
        public int critBonus = 0;
        public int lastUseTimeBonusInt;
        public int lastUseAnimationBonusInt;
        public int lastShootSpeedBonusInt;
        public int lastReuseDelayBonus;
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
        public float allForOneBonus = 1f;
        public bool oneForAll;
        public float oneForAllBonus = 0f;
        public bool spelunker = false;
        public bool dangerSense = false;
        public bool hunter = false;
        public float enemySpawnBonus = 0f;
        public float godSlayerBonus = 0f;
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
                Enchantments enchantment = ((Enchantments)enchantments[i].ModItem);
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
                        case EnchantmentTypeID.Critical:
                            criticalBonus += str;
                            break;
                        case EnchantmentTypeID.Size:
                            knockbackBonus += str;
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
            player.GetKnockback(DamageClass.Generic) += knockbackBonus / 2; 
            player.GetArmorPenetration(DamageClass.Generic) += armorPenetrationBonus / 4;
            item.defense += defenceBonus - lastDefenceBonus;
            lastDefenceBonus = defenceBonus;
        }
        public override void UpdateInventory(Item item, Player player)
        {
            if(experience > 0 || powerBoosterInstalled)
            {
                int value = 0;
                for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
                {
                    value += enchantments[i].value;
                }
                int powerBoosterValue = powerBoosterInstalled ? ModContent.GetModItem(ModContent.ItemType<PowerBooster>()).Item.value : 0;
                int npcTalking = player.talkNPC != -1 ? Main.npc[player.talkNPC].type : -1;
                int valueToAdd = npcTalking != NPCID.GoblinTinkerer ? value + 16 * experience + powerBoosterValue : 0;
                item.value += valueToAdd - lastValueBonus;//Update items value based on enchantments installed
                lastValueBonus = valueToAdd;
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
                Enchantments enchantment = ((Enchantments)enchantments[i].ModItem);
                if (!enchantments[i].IsAir) 
                { 
                    total += enchantment.GetLevelCost();
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
            float speedModifier = 0f;
            int defenceBonus = 0;
            float manaCostBonus = 0f;
            float lifeStealBonus = 0f;
            float armorPenetrationBonus = 0f;
            float allForOneSpeedMultiplier = 1f;
            float allForOneImmunityBonus = 1f;
            float oneForAllSpeedMultiplier = 1f;
            float enemySpawnBonusLocal = 1f;
            float allForOneManaBonus = 0f;
            damageBonus = 0f;
            immunityBonus = 0f;
            allForOneBonus = 1f;
            oneForAllBonus = 0f;
            godSlayerBonus = 0f;
            allForOne = false;
            oneForAll = false;
            spelunker = false;
            dangerSense = false;
            hunter = false;
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
            {
                Enchantments enchantment = ((Enchantments)enchantments[i].ModItem);
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
                        case EnchantmentTypeID.ManaCost:
                            manaCostBonus += str;
                            break;
                        case EnchantmentTypeID.LifeSteal:
                            lifeStealBonus += str;
                            break;
                        case EnchantmentTypeID.ArmorPenetration:
                            armorPenetrationBonus += str;
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
                        case EnchantmentTypeID.Spelunker:
                            spelunker = true;
                            break;
                        case EnchantmentTypeID.DangerSense:
                            dangerSense = true;
                            break;
                        case EnchantmentTypeID.Hunter:
                            hunter = true;
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
            item.scale += wePlayer.itemScale - lastGenericScaleBonus;
            lastGenericScaleBonus = wePlayer.itemScale;
            int mana = (int)Math.Round((float)ContentSamples.ItemsByType[item.type].mana * (manaCostBonus + wePlayer.manaCost - (allForOneManaBonus * 0.4f)));
            item.mana -= mana - lastManaCostBonus;
            lastManaCostBonus = mana;
            lifeSteal = lifeStealBonus;
            item.ArmorPenetration += (int)armorPenetrationBonus - lastArmorPenetrationBonus;
            lastArmorPenetrationBonus = (int)armorPenetrationBonus;
            enemySpawnBonus = enemySpawnBonusLocal;
        }
        public override void ModifyWeaponCrit(Item item, Player player, ref float crit)
        {
            critBonus = 0;
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
            {
                Enchantments enchantment = ((Enchantments)enchantments[i].ModItem);
                if (!enchantments[i].IsAir && (EnchantmentTypeID)enchantment.EnchantmentType == EnchantmentTypeID.Critical)
                {
                    critBonus += (int)(enchantment.EnchantmentStrength * 100);
                }
            }
            crit += critBonus;
        }
        public override void ModifyWeaponKnockback(Item item, Player player, ref StatModifier knockback)
        {
            float scale = 0f;
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
            {
                Enchantments enchantment = ((Enchantments)enchantments[i].ModItem);
                if (!enchantments[i].IsAir && (EnchantmentTypeID)enchantment.EnchantmentType == EnchantmentTypeID.Size)
                {
                    float temp = knockback.Additive;
                    float temp1 = knockback.Flat;
                    float temp2 = knockback.Base;
                    float temp3 = knockback.Multiplicative;
                    knockback += enchantment.EnchantmentStrength;
                    float temp4 = knockback.Multiplicative;
                    float temp5 = knockback.Additive;
                    scale += enchantment.EnchantmentStrength / 2;//Only do 50% of enchantmentStrength to size
                }
            }
            item.scale += scale - lastSizeBonus;//Update item size
            lastSizeBonus = scale;
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
                tooltips.Add(new TooltipLine(Mod, "experience", "Experience: " + experience.ToString()) { OverrideColor = Color.White });
            }
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
            {
                if (!enchantments[i].IsAir)
                {
                    Enchantments enchantment = ((Enchantments)enchantments[i].ModItem);
                    if (!enchantmentsToolTipAdded)
                    {
                        tooltips.Add(new TooltipLine(Mod, "enchantmentsToolTip", "Enchantments:") { OverrideColor = Color.Violet});
                        enchantmentsToolTipAdded = true;
                    }//Enchantmenst: tooltip
                    if(WEMod.IsWeaponItem(item))
                    {
                        switch ((EnchantmentTypeID)enchantment.EnchantmentType)
                        {
                            case EnchantmentTypeID.Size:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "+" + (enchantment.EnchantmentStrength * 50).ToString() + "% " + enchantment.EnchantmentTypeName + ", +" + (enchantment.EnchantmentStrength * 100).ToString() + "% Knockback")
                                {
                                    OverrideColor = Enchantments.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                            case EnchantmentTypeID.Defence:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "+" + (enchantment.EnchantmentStrength / 2).ToString() + " " + enchantment.EnchantmentTypeName)
                                {
                                    OverrideColor = Enchantments.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                            case EnchantmentTypeID.ArmorPenetration:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "+" + enchantment.EnchantmentStrength.ToString() + " Armor Penetration")
                                {
                                    OverrideColor = Enchantments.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                            case EnchantmentTypeID.ManaCost:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "-" + (enchantment.EnchantmentStrength * 100).ToString() + "% Mana Cost")
                                {
                                    OverrideColor = Enchantments.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                            case EnchantmentTypeID.AmmoCost:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "-" + (enchantment.EnchantmentStrength * 100).ToString() + "% Chance to consume ammo")
                                {
                                    OverrideColor = Enchantments.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                            case EnchantmentTypeID.LifeSteal:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), (enchantment.EnchantmentStrength * 100).ToString() + "% Life Steal")
                                {
                                    OverrideColor = Enchantments.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                            case EnchantmentTypeID.OneForAll:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), (enchantment.EnchantmentStrength * 100).ToString() + " % damage to nearby enemies, -" + (30f * enchantment.EnchantmentStrength).ToString() + "% base attack speed")
                                {
                                    OverrideColor = Enchantments.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                            case EnchantmentTypeID.AllForOne:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), enchantment.EnchantmentStrength + "x Damage, item CD equal to " + enchantment.EnchantmentStrength * 0.8f + "x use speed\n(WARNING - DESTROYS PROJECTILES ON HIT)")
                                {
                                    OverrideColor = Enchantments.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                            case EnchantmentTypeID.Spelunker:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "Spelunker buff")
                                {
                                    OverrideColor = Enchantments.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                            case EnchantmentTypeID.DangerSense:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "Danger Sense buff")
                                {
                                    OverrideColor = Enchantments.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                            case EnchantmentTypeID.Hunter:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "Hunter buff")
                                {
                                    OverrideColor = Enchantments.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                            case EnchantmentTypeID.War:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), (enchantment.EnchantmentStrength + 1f).ToString() + "x enemy spawn rate and max enemies")
                                {
                                    OverrideColor = Enchantments.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                            case EnchantmentTypeID.Peace:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), (1f / (enchantment.EnchantmentStrength + 1f)).ToString() + "x enemy spawn rate and max enemies")
                                {
                                    OverrideColor = Enchantments.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                            case EnchantmentTypeID.GodSlayer:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), (enchantment.EnchantmentStrength * 100).ToString() + "% God Slayer Bonus")
                                {
                                    OverrideColor = Enchantments.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                            default:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "+" + (enchantment.EnchantmentStrength * 100).ToString() + "% " + enchantment.EnchantmentTypeName)
                                {
                                    OverrideColor = Enchantments.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                        }
                    }
                    else
                    {
                        switch ((EnchantmentTypeID)enchantment.EnchantmentType)
                        {
                            case EnchantmentTypeID.Size:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "+" + ((enchantment.EnchantmentStrength * 50 / 2)).ToString() + "% " + enchantment.EnchantmentTypeName + ", +" + ((enchantment.EnchantmentStrength * 100 / 2)).ToString() + "% Knockback")
                                {
                                    OverrideColor = Enchantments.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                            case EnchantmentTypeID.Defence:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "+" + enchantment.EnchantmentStrength.ToString() + " " + enchantment.EnchantmentTypeName)
                                {
                                    OverrideColor = Enchantments.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                            case EnchantmentTypeID.ArmorPenetration:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "+" + (enchantment.EnchantmentStrength / 4).ToString() + " Armor Penetration")
                                {
                                    OverrideColor = Enchantments.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                            case EnchantmentTypeID.ManaCost:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "-" + (enchantment.EnchantmentStrength * 100 / 4).ToString() + "% Mana Cost")
                                {
                                    OverrideColor = Enchantments.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                            case EnchantmentTypeID.AmmoCost:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "-" + (enchantment.EnchantmentStrength * 100 / 4).ToString() + "% Chance to consume ammo")
                                {
                                    OverrideColor = Enchantments.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                            case EnchantmentTypeID.LifeSteal:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), (enchantment.EnchantmentStrength * 100 / 4).ToString() + "% Life Steal")
                                {
                                    OverrideColor = Enchantments.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                            case EnchantmentTypeID.Spelunker:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "Spelunker buff")
                                {
                                    OverrideColor = Enchantments.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                            case EnchantmentTypeID.DangerSense:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "Danger Sense buff")
                                {
                                    OverrideColor = Enchantments.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                            case EnchantmentTypeID.Hunter:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "Hunter buff")
                                {
                                    OverrideColor = Enchantments.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                            case EnchantmentTypeID.War:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), (enchantment.EnchantmentStrength + 1f).ToString() + "x enemy spawn rate and max enemies")
                                {
                                    OverrideColor = Enchantments.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                            case EnchantmentTypeID.Peace:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), (1f / (enchantment.EnchantmentStrength + 1f)).ToString() + "x enemy spawn rate and max enemies")
                                {
                                    OverrideColor = Enchantments.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                            default:
                                tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "+" + ((enchantment.EnchantmentStrength * 100 / 4)).ToString() + "% " + enchantment.EnchantmentTypeName)
                                {
                                    OverrideColor = Enchantments.rarityColors[enchantment.EnchantmentSize]
                                });
                                break;
                        }
                    }
                }
            }//Edit Tooltips
        }
        public void DamageNPC(Item item, NPC target, int damage)
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            target.GetGlobalNPC<WEGlobalNPC>().xpCalculated = true;
            if (target.type != NPCID.TargetDummy && !target.friendly && !target.townNPC)
            {
                int xpInt;

                float multiplier;
                float effDamage;
                float xp;
                multiplier = (1f + ((float)((target.noGravity ? 2f : 0f) + (target.noTileCollide ? 2f : 0f)) + 2f * (1f - target.knockBackResist)) / 10f + (float)target.defDamage / 40f) / (target.boss ? 2f : 1f);
                effDamage = (float)item.damage * (1f + (float)item.crit / 100f);
                damage = target.life < 0 ? damage + target.life : damage;
                if (target.value > 0 || !target.SpawnedFromStatue && target.lifeMax > 5)
                {
                    if (effDamage - (float)target.defDefense / 2 > 1)
                    {
                        xp = (float)damage * multiplier * effDamage / (effDamage - (float)target.defDefense / 2);
                    }
                    else
                    {
                        xp = (float)damage * multiplier * effDamage;
                    }
                    if (item.accessory)
                    {
                        xp /= 2;
                    }
                    xpInt = (int)Math.Round(xp);
                    xpInt = xpInt > 1 ? xpInt : 1;
                    if (!item.consumable)
                    {
                        //ModContent.GetInstance<WEMod>().Logger.Info(wePlayer.Player.name + " recieved " + xpInt.ToString() + " xp from hitting " + target.FullName + ".");
                        //Main.NewText(wePlayer.Player.name + " recieved " + xpInt.ToString() + " xp from killing " + target.FullName + ".");
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
                Enchantments enchantment = ((Enchantments)enchantments[i].ModItem);
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
                        ;
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
                    if (Main.rand.NextFloat() < value / 500000f)
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
            DamageNPC(item, target, damage);
        }
    }
}
