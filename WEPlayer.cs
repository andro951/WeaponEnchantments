using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI.Chat;
using WeaponEnchantments.UI.WeaponEnchantmentUI;
using WeaponEnchantments.Common;
 

namespace WeaponEnchantments
{ 
    public class WEPlayer : ModPlayer
    {
        public bool usingEnchantingTable;
        public int enchantingTableTier;
        public EnchantingTable enchantingTable = new EnchantingTable();
        public WeaponEnchantmentUI enchantingTableUI = new WeaponEnchantmentUI();

        /*
        public void RefreshModItems()
        {
            RefreshItemArray(enchantingTable.item);
            RefreshItemArray(enchantingTable.enchantmentItem);
            RefreshItemArray(enchantingTable.essenceItem);
        }
        */

        public override void Initialize()
        {

        }
        public override void SaveData(TagCompound tag)
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            for (int i = 0; i < EnchantingTable.maxItems; i++)
            {
                if (!wePlayer.enchantingTable.item[i].IsAir)
                {
                    tag["enchantingTableItem" + i.ToString()] = wePlayer.enchantingTable.item[i];
                }
            }
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
            {
                if (!wePlayer.enchantingTable.enchantmentItem[i].IsAir)
                {
                    tag["enchantingTableEnchantmentItem" + i.ToString()] = wePlayer.enchantingTable.enchantmentItem[i];
                }
            }
            for (int i = 0; i < EnchantingTable.maxEssenceItems; i++)
            {
                if (!wePlayer.enchantingTable.essenceItem[i].IsAir)
                {
                    tag["enchantingTableEssenceItem" + i.ToString()] = wePlayer.enchantingTable.essenceItem[i];
                }
            }
        }
        public override void LoadData(TagCompound tag)
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            for (int i = 0; i < EnchantingTable.maxItems; i++)
            {
                if (tag.Get<Item>("enchantingTableItem" + i.ToString()).IsAir)
                {
                    wePlayer.enchantingTable.item[i] = new Item();
                }
                else
                { 
                    wePlayer.enchantingTable.item[i] = tag.Get<Item>("enchantingTableItem" + i.ToString());
                }
                //wePlayer.enchantingTableUI.itemSlotUI[i].Item.stack = 0;
            }
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
            {
                if (tag.Get<Item>("enchantingTableEnchantmentItem" + i.ToString()).IsAir)
                {
                    wePlayer.enchantingTable.enchantmentItem[i] = new Item();
                }
                else
                {
                    wePlayer.enchantingTable.enchantmentItem[i] = tag.Get<Item>("enchantingTableEnchantmentItem" + i.ToString());
                }
            }
            for (int i = 0; i < EnchantingTable.maxEssenceItems; i++)
            {
                if (tag.Get<Item>("enchantingTableEssenceItem" + i.ToString()).IsAir)
                {
                    wePlayer.enchantingTable.essenceItem[i] = new Item();
                }
                else
                {
                    wePlayer.enchantingTable.essenceItem[i] = tag.Get<Item>("enchantingTableEssenceItem" + i.ToString());
                }
            }
        }
        private void RefreshItemArray(Item[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if(!array[i].IsAir)
                    array[i].Refresh();//Find what this does
            }
        }
    }
}
