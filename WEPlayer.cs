using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI.Chat;
using WeaponEnchantments.Common;
using WeaponEnchantments.UI;
using WeaponEnchantments.Items;
using WeaponEnchantments.Common.Globals;

namespace WeaponEnchantments
{ 
    public class WEPlayer : ModPlayer
    {
        public bool usingEnchantingTable;
        public int enchantingTableTier;
        public bool itemInEnchantingTable;
        public bool[] enchantmentInEnchantingTable = new bool[EnchantingTable.maxEnchantments];
        public Item itemBeingEnchanted;
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
            if (wePlayer?.enchantingTableUI?.itemSlotUI[0]?.Item != null)
            {
                if (wePlayer.enchantingTableUI.itemSlotUI[0].Item.IsAir)
                {
                    for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
                    {
                        if (!wePlayer.enchantingTable.enchantmentItem[i].IsAir)
                        {
                            tag["enchantingTableEnchantmentItem" + i.ToString()] = wePlayer.enchantingTable.enchantmentItem[i];
                        }
                    }
                }//enchantments in the enchantmentSlots are saved by global items.  This is just in case enchantment items are left in after Offering items.
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
            }
            if (wePlayer.enchantingTableUI.itemSlotUI[0].Item.IsAir)//enchantments in the enchantmentSlots are loaded by global items.  This is just in case enchantment items are left in after Offering items.
            {
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
        public override bool ShiftClickSlot(Item[] inventory, int context, int slot)
        {
            if (usingEnchantingTable)
            {
                if(inventory == Player.inventory)
                {
                    if (Main.mouseItem.IsAir)
                    {
                        bool valid = false;
                        if (inventory[slot].type == PowerBooster.ID && !enchantingTableUI.itemSlotUI[0].Item.IsAir && !enchantingTableUI.itemSlotUI[0].Item.GetGlobalItem<EnchantedItem>().powerBoosterInstalled)
                        {
                            inventory[slot] = new Item();
                            enchantingTableUI.itemSlotUI[0].Item.GetGlobalItem<EnchantedItem>().powerBoosterInstalled = true;
                            SoundEngine.PlaySound(SoundID.Grab);
                            valid = true;
                        }//If using a PowerBooster, destroy the booster and update the global item.
                        else
                        {
                            for (int i = 0; i < EnchantingTable.maxItems; i++)
                            {
                                if (enchantingTableUI.itemSlotUI[i].Valid(inventory[slot]))
                                {
                                    if (!inventory[slot].IsAir)
                                    {
                                        enchantingTableUI.itemSlotUI[i].Item = inventory[slot].Clone();
                                        inventory[slot] = new Item();
                                        SoundEngine.PlaySound(SoundID.Grab);
                                        valid = true;
                                    }
                                }
                            }
                            if (!valid)
                            {
                                for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
                                {
                                    if (enchantingTableUI.enchantmentSlotUI[i].Valid(inventory[slot]))
                                    {
                                        if (!inventory[slot].IsAir && enchantingTableUI.enchantmentSlotUI[i].Item.IsAir)
                                        {
                                            if (((Enchantments)inventory[slot].ModItem).utility && enchantingTableUI.enchantmentSlotUI[4].Item.IsAir)
                                            {
                                                enchantingTableUI.enchantmentSlotUI[4].Item = inventory[slot].Clone();
                                                inventory[slot] = new Item();
                                            }
                                            else
                                            {
                                                enchantingTableUI.enchantmentSlotUI[i].Item = inventory[slot].Clone();
                                                inventory[slot] = new Item();
                                            }
                                            SoundEngine.PlaySound(SoundID.Grab);
                                            valid = true;
                                        }
                                    }
                                }
                            }
                            if (!valid)
                            {
                                for (int i = 0; i < EnchantingTable.maxEssenceItems; i++)
                                {
                                    if (enchantingTableUI.essenceSlotUI[i].Valid(inventory[slot]))
                                    {
                                        if (!inventory[slot].IsAir)
                                        {
                                            bool transfered = false;
                                            if (enchantingTableUI.essenceSlotUI[i].Item.IsAir)
                                            {
                                                enchantingTableUI.essenceSlotUI[i].Item = inventory[slot].Clone();
                                                inventory[slot] = new Item();
                                                transfered = true;
                                            }
                                            else
                                            {
                                                if (enchantingTableUI.essenceSlotUI[i].Item.stack < EnchantmentEssence.maxStack)
                                                {
                                                    int ammountToTransfer;
                                                    if (inventory[slot].stack + enchantingTableUI.essenceSlotUI[i].Item.stack > EnchantmentEssence.maxStack)
                                                    {
                                                        ammountToTransfer = EnchantmentEssence.maxStack - enchantingTableUI.essenceSlotUI[i].Item.stack;
                                                        inventory[slot].stack -= ammountToTransfer;
                                                    }
                                                    else
                                                    {
                                                        ammountToTransfer = inventory[slot].stack;
                                                    }
                                                    enchantingTableUI.essenceSlotUI[i].Item.stack += ammountToTransfer;
                                                    transfered = true;
                                                }
                                            }
                                            if (transfered)
                                            {
                                                SoundEngine.PlaySound(SoundID.Grab);
                                                valid = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (!valid)
                        {
                            Main.mouseItem = inventory[slot].Clone();
                            inventory[slot] = new Item();
                        }
                    }
                    return true;
                }
            }
            return false;
        }
        private void RefreshItemArray(Item[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if(!array[i].IsAir)
                    array[i].Refresh();//Find what this does
            }
        }//My UI
    }
}
