using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using WeaponEnchantments.Items;
using Terraria.ModLoader.IO;
using static WeaponEnchantments.Items.Enchantments;
using Terraria.ID;
using System;

namespace WeaponEnchantments.Common.Globals
{
    public class EnchantedItem : GlobalItem
    {
        public Item[] enchantments = new Item[EnchantingTable.maxEnchantments];//Track enchantment items on a weapon/armor/accessory item
        
        public int experience;//current experience of a weapon/armor/accessory item
        public int baseDamage;
        public float origionalScale;
        public int origionalValue;
        public int level;
        public bool powerBoosterInstalled;
        public const int maxLevel = 22;
        //public bool trackingProjectileThroughCloning;
        //public Projectile trackedProjectile;
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
            /*clone.enchantments = (Item[])enchantments.Clone();
            for (int i = 0; i < enchantments.Length; i++)
            {
               clone.enchantments[i] = enchantments[i].Clone();
            }*/
            /*if(trackingProjectileThroughCloning)
            {
                trackedProjectile.GetGlobalProjectile<ProjectileEnchantedItem>().sourceItem = itemClone;
            }*/
            return clone;
        }
        public void UpdateLevel()//Optimize this into get next level?
        {
            int previous1 = 1;
            int previous2 = 0;
            int current;
            int total = 0;
            int l;
            for(l = 0; l <= maxLevel; l++)
            {
                current = previous1 + previous2;
                total += current;
                previous2 = previous1;
                previous1 = current;
                if(experience < total)
                {
                    level = l;
                    break;
                }
            }
            level = powerBoosterInstalled ? l + 10 : l;
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
            if (baseDamage == 0)
            {
                baseDamage = ContentSamples.ItemsByType[item.type].damage;
            }
            float modifier = 0f;
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
            {
                if (!enchantments[i].IsAir && ((Enchantments)enchantments[i].ModItem).enchantmentType == EnchantmentTypeIDs.Damage)
                {
                    modifier += ((Enchantments)enchantments[i].ModItem).enchantmentStrength * damage.Multiplicative;
                }
            }
            if (modifier > 0 && baseDamage * (1 + modifier) * damage.Multiplicative - baseDamage < 1)
            {
                damage += 1/baseDamage/damage.Multiplicative;
            }
            else
            {
                damage += modifier * damage.Multiplicative;
            }
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
            if (origionalScale == 0)
            {
                origionalScale = item.scale;
                origionalValue = item.value;
            }
            float scale = 0f;
            int value = 0;
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
            {
                if (!enchantments[i].IsAir && ((Enchantments)enchantments[i].ModItem).enchantmentType == EnchantmentTypeIDs.Size)
                {
                    knockback += (int)(((Enchantments)enchantments[i].ModItem).enchantmentStrength * 100);
                    scale += ((Enchantments)enchantments[i].ModItem).enchantmentStrength / 2;//Only do 50% of enchantmentStrength to size
                }
                value += enchantments[i].value;
            }
            item.scale = origionalScale * (1 + scale);//Update item size
            item.value = origionalValue + value;//Update items value based on enchantments installed
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            bool enchantmentsToolTipAdded = false;
            if (experience > 0)
            {
                UpdateLevel();
                tooltips.Add(new TooltipLine(Mod, "level", "Level: " + level.ToString() + " Points available: " + GetLevelsAvailable().ToString()) { OverrideColor = Color.LightGreen });
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
                    if (((Enchantments)enchantments[i].ModItem).enchantmentType == EnchantmentTypeIDs.Size)
                    {
                        tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "+" + ((int)(((Enchantments)enchantments[i].ModItem).enchantmentStrength * 50)).ToString() + "% " + ((Enchantments)enchantments[i].ModItem).enchantmentTypeName)
                        {
                            OverrideColor = Enchantments.rarityColors[((Enchantments)enchantments[i].ModItem).enchantmentSize]
                        });
                        tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString() + "-2", "+" + ((int)(((Enchantments)enchantments[i].ModItem).enchantmentStrength * 100)).ToString() + "% Knockback")
                        {
                            OverrideColor = Enchantments.rarityColors[((Enchantments)enchantments[i].ModItem).enchantmentSize]
                        });
                    }//Special tooltip if size enchantment installed
                    else
                    {
                        tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "+" + ((int)(((Enchantments)enchantments[i].ModItem).enchantmentStrength * 100)).ToString() + "% " + ((Enchantments)enchantments[i].ModItem).enchantmentTypeName)
                        {
                            OverrideColor = Enchantments.rarityColors[((Enchantments)enchantments[i].ModItem).enchantmentSize]
                        });
                    }//Default tooltip
                }
            }//Edit Tooltips
        }
        public override void OnHitNPC(Item item, Player player, NPC target, int damage, float knockBack, bool crit)
        {
            if (target.life <= 0) 
            {
                KillNPC(item, target);
            }
        }
        public void KillNPC(Item item, NPC target)
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            if (target.type != NPCID.TargetDummy && !target.SpawnedFromStatue && !target.friendly && !target.townNPC)
            {
                float xp;
                int xpInt;
                if (target.value > target.lifeMax)
                {
                    xp = 0.2f * (target.value - target.lifeMax) + target.lifeMax;
                }
                else
                {
                    xp = 0.2f * (target.lifeMax - target.value) + target.value;
                }
                xpInt = (int)xp;
                Main.NewText(wePlayer.Player.name + " recieved " + xpInt.ToString() + " xp from killing " + target.FullName + ".");
                GainXP(item, xpInt);
            }
        }
        public void GainXP(Item item, int xpInt)
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            int currentLevel = level;
            experience += xpInt;
            UpdateLevel();
            if (level > currentLevel)
            {
                Main.NewText(wePlayer.Player.name + "'s " + item.Name + " reached level " + level.ToString() + ".");
            }
        }
    }
}
