using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using WeaponEnchantments;
using WeaponEnchantments.UI;
using WeaponEnchantments.UI.WeaponEnchantmentUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.Default;
using Terraria.UI;
using Terraria.UI.Chat;
using Terraria.UI.Gamepad;
using WeaponEnchantments.Items;
using WeaponEnchantments.Common.GlobalItems;
using Terraria.ModLoader.IO;

namespace WeaponEnchantments.Common.GlobalItems
{
    public class EnchantedItem : GlobalItem
    {
        public Item[] enchantments = new Item[EnchantingTable.maxEnchantments];
        //public Enchantments[] enchantments = new Enchantments[EnchantingTable.maxEnchantments];
        public int experience;//Make sure to add tags for this.  Somehow needs to deal with summoned weapons too and add to tooltip.  Base xp values off enemy value
        public EnchantedItem()
        {
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++) 
            {
                enchantments[i] = new Item();
                //enchantments[i] = new Enchantments();
            }
        }
        public override bool InstancePerEntity => true;

        public override GlobalItem Clone(Item item, Item itemClone)
        {
            EnchantedItem clone = (EnchantedItem)base.Clone(item, itemClone);
            clone.enchantments = (Item[])enchantments.Clone();
            //clone.enchantments = (Enchantments[])enchantments.Clone();
            for (int i = 0; i < enchantments.Length; i++)
            {
                clone.enchantments[i] = enchantments[i].Clone();
                //clone.enchantments[i] = (Enchantments)enchantments[i].Clone();
            }
            return clone;
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
                        //enchantments[i] = tag.Get<Enchantments>("enchantments" + i.ToString()).Clone();
                        //item.damage = (int)(1.1f * item.damage);
                    }
                    else
                    {
                        enchantments[i] = new Item();
                        //enchantments[i] = new Enchantments();
                    }
                }
            }
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
                        //if (!enchantments[i].Item.IsAir)
                        {
                            tag["enchantments" + i.ToString()] = enchantments[i].Clone();
                        }
                    }
                }
            }
        }
        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage, ref float flat)
        {
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
            {
                if (!enchantments[i].IsAir && ((Enchantments)enchantments[i].ModItem).enchantmentType == 0)
                {
                    damage += ((Enchantments)enchantments[i].ModItem).enchantmentStrength * damage.Multiplicative;
                }
            }
        }
        public override void ModifyWeaponCrit(Item item, Player player, ref int crit)
        {
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
            {
                if (!enchantments[i].IsAir && ((Enchantments)enchantments[i].ModItem).enchantmentType == 1)
                {
                    crit += (int)(((Enchantments)enchantments[i].ModItem).enchantmentStrength * 100);
                }
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            bool enchantmentsToolTipAdded = false;
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
            {
                if (!enchantments[i].IsAir)
                {
                    if (!enchantmentsToolTipAdded)
                    {
                        tooltips.Add(new TooltipLine(Mod, "enchantmentsToolTip", "Enchantments:") { overrideColor = Color.Violet});
                        enchantmentsToolTipAdded = true;
                    }
                    //tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "+" + ((int)(((Enchantments)enchantments[i].ModItem).enchantmentStrength * 100)).ToString() + " % " + ((Enchantments)enchantments[i].ModItem).enchantmentType));
                    tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "+" + ((int)(((Enchantments)enchantments[i].ModItem).enchantmentStrength * 100)).ToString() + "% " + ((Enchantments)enchantments[i].ModItem).enchantmentTypeName)
                    {
                        overrideColor = Enchantments.rarityColors[((Enchantments)enchantments[i].ModItem).enchantmentSize]
                    });
                }
            }
        }

        //If it doesn't have a weapon modifier, make it's weapon modifier Enchanted just so it will show the stat changes
    }
}
