using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using WeaponEnchantments.UI;
using WeaponEnchantments.Items;
using WeaponEnchantments.Common.Globals;
using static WeaponEnchantments.Items.Enchantments;

namespace WeaponEnchantments
{ 
    public class WEPlayer : ModPlayer
    {
        public bool usingEnchantingTable;
        public int enchantingTableTier;
        public bool itemInEnchantingTable;
        public bool[] enchantmentInEnchantingTable = new bool[EnchantingTable.maxEnchantments];
        public Item itemBeingEnchanted;
        public EnchantingTable enchantingTable;
        public WeaponEnchantmentUI enchantingTableUI;
        public ConfirmationUI confirmationUI;
        public Item[] inventoryItemRecord;
        public int lastFocusRecipeListNum = -1;
        public int lastFocusRecipe = -1;
        public float itemScale = 0f;
        public float manaCost = 0f;
        public float ammoCost = 0f;
        public float lifeSteal = 0f;
        public float lifeStealRollover = 0f;
        public bool allForOneCooldown = false;
        public int allForOneTimer = 0;
        public Item[] equiptArmor;
        public Item heldItem;
        public bool spelunker = false;
        public bool dangerSense = false;
        public bool hunter = false;
        public float enemySpawnBonus = 1f;
        public bool godSlayer = false;

        public override void Initialize()
        {
            enchantingTable = new EnchantingTable();
            enchantingTableUI = new WeaponEnchantmentUI();
            equiptArmor = new Item[Player.armor.Length];
            heldItem = new Item();
            confirmationUI = new ConfirmationUI();
            inventoryItemRecord = new Item[102];
            for (int i = 0; i < equiptArmor.Length; i++)
            {
                equiptArmor[i] = new Item();
            }
        }
        public override void SaveData(TagCompound tag)
        {
            /*
            string name = Player.name;
            if(enchantingTable.item != null)
            {
                tag.Add("enchantingTableItems", enchantingTable.item);
            }
            if(enchantingTable.essenceItem != null)
            {
                tag.Add("enchantingTableEssenceItem", enchantingTable.essenceItem);
            }
            */
            //tag["enchantingTableItems"] = enchantingTable.item;
            //tag["enchantingTableEssenceItem"] = enchantingTable.essenceItem;
            
            for (int i = 0; i < EnchantingTable.maxItems; i++)
            {
                //if (!enchantingTable.item[i].IsAir)
                {
                    string name = Player.name;
                    int tempInt = enchantingTable.item[i].stack;
                    tag["enchantingTableItem" + i.ToString()] = enchantingTable.item[i];
                }
            }
            /*if (enchantingTableUI?.itemSlotUI[0]?.Item != null)
            {
                //if (enchantingTableUI.itemSlotUI[0].Item.IsAir)
                {
                    for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
                    {
                        if (!enchantingTable.enchantmentItem[i].IsAir)
                        {
                            tag["enchantingTableEnchantmentItem" + i.ToString()] = enchantingTable.enchantmentItem[i];
                        }
                    }
                }//enchantments in the enchantmentSlots are saved by global items.  This is just in case enchantment items are left in after Offering items.
            }*///Not used
            for (int i = 0; i < EnchantingTable.maxEssenceItems; i++)
            {
                string name = Player.name;
                int tempInt = enchantingTable.essenceItem[i].stack;
                tag["enchantingTableEssenceItem" + i.ToString()] = enchantingTable.essenceItem[i];
            }
        }
        public override void LoadData(TagCompound tag)
        {
            /*
            string name = Player.name;
            if (tag.Get<Item[]>("enchantingTableItems") != null)
                enchantingTable.item = tag.Get<List<Item>>("enchantingTableItems");
            if (tag.Get<Item[]>("enchantingTableEssenceItems") != null)
                enchantingTable.essenceItem = tag.Get<List<Item>>("enchantingTableEssenceItems");
            */
            
            for (int i = 0; i < EnchantingTable.maxItems; i++)
            {
                if (tag.Get<Item>("enchantingTableItem" + i.ToString()).IsAir)
                {
                    enchantingTable.item[i] = new Item();
                }
                else
                {
                    enchantingTable.item[i] = tag.Get<Item>("enchantingTableItem" + i.ToString());
                }
                string name = Player.name;
                int tempInt = enchantingTable.item[i].stack;
            }
            /*
            if (enchantingTable.item[0].IsAir)//enchantments in the enchantmentSlots are loaded by global items.  This is just in case enchantment items are left in after Offering items.
            {
                for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
                {
                    if (tag.Get<Item>("enchantingTableEnchantmentItem" + i.ToString()).IsAir)
                    {
                        enchantingTable.enchantmentItem[i] = new Item();
                    }
                    else
                    {
                        enchantingTable.enchantmentItem[i] = tag.Get<Item>("enchantingTableEnchantmentItem" + i.ToString());
                    }
                }
            }
            *///Not used
            
            for (int i = 0; i < EnchantingTable.maxEssenceItems; i++)
            {
                if (tag.Get<Item>("enchantingTableEssenceItem" + i.ToString()).IsAir)
                {
                    enchantingTable.essenceItem[i] = new Item();
                }
                else
                {
                    enchantingTable.essenceItem[i] = tag.Get<Item>("enchantingTableEssenceItem" + i.ToString());
                }
                string name = Player.name;
                int tempInt = enchantingTable.essenceItem[i].stack;
            }
        }
        public override bool ShiftClickSlot(Item[] inventory, int context, int slot)
        {
            if (usingEnchantingTable)
            {
                bool stop = false;
                for (int j = 0; j < EnchantingTable.maxItems; j++)
                {
                    if (enchantingTableUI.itemSlotUI[j].contains)
                    {
                        stop = true;
                    }
                }
                for (int j = 0; j < EnchantingTable.maxEnchantments && !stop; j++)
                {
                    if (enchantingTableUI.enchantmentSlotUI[j].contains)
                    {
                        stop = true;
                    }
                }
                for (int j = 0; j < EnchantingTable.maxEssenceItems && !stop; j++)
                {
                    if (enchantingTableUI.essenceSlotUI[j].contains)
                    {
                        stop = true;
                    }
                }
                if (stop)
                {
                    bool itemWillBeTrashed = true;
                    for(int i = 49; i >= 0 && itemWillBeTrashed; i--)
                    {
                        if (Player.inventory[i].IsAir)
                        {
                            itemWillBeTrashed = false;
                        }
                    }
                    if (itemWillBeTrashed)
                    {
                        return true;
                    }
                }//TODO: Edit this if you ever make ammo bags enchantable
                if (!stop)
                {
                    if (Main.mouseItem.IsAir)
                    {
                        bool valid = false;
                        if (inventory[slot].type == PowerBooster.ID && !enchantingTableUI.itemSlotUI[0].Item.IsAir && !enchantingTableUI.itemSlotUI[0].Item.GetGlobalItem<EnchantedItem>().powerBoosterInstalled)
                        {
                            enchantingTableUI.itemSlotUI[0].Item.GetGlobalItem<EnchantedItem>().powerBoosterInstalled = true;
                            if (inventory[slot].stack > 1)
                            {
                                inventory[slot].stack--;
                            }
                            else
                            {
                                inventory[slot] = new Item();
                            }
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
                                        inventory[slot] = itemInEnchantingTable ? itemBeingEnchanted : new Item();
                                        SoundEngine.PlaySound(SoundID.Grab);
                                        valid = true;
                                        break;
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
                                            int s = i;
                                            if (((Enchantments)inventory[slot].ModItem).Utility && enchantingTableUI.enchantmentSlotUI[4].Item.IsAir)
                                            {
                                                s = 4;
                                            }
                                            enchantingTableUI.enchantmentSlotUI[s].Item = inventory[slot].Clone();
                                            enchantingTableUI.enchantmentSlotUI[s].Item.stack = 1;
                                            if (inventory[slot].stack > 1)
                                            {
                                                inventory[slot].stack--;
                                            }
                                            else
                                            {
                                                inventory[slot] = new Item();
                                            }
                                            SoundEngine.PlaySound(SoundID.Grab);
                                            valid = true;
                                            break;
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
                                                        inventory[slot].stack = 0;
                                                    }
                                                    enchantingTableUI.essenceSlotUI[i].Item.stack += ammountToTransfer;
                                                    transfered = true;
                                                }
                                            }
                                            if (transfered)
                                            {
                                                SoundEngine.PlaySound(SoundID.Grab);
                                                valid = true;
                                                break;
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
        public void StoreLastFocusRecipe()
        {
            lastFocusRecipe = Main.availableRecipe[Main.focusRecipe];
        }
        public override void PostUpdate()
        {
            bool check = false;
            int i = 0;
            if(Player.HeldItem != heldItem)
            {
                EnchantedItem hiGlobal;
                if (Player.HeldItem.type != ItemID.None)
                {
                    hiGlobal = Player.HeldItem.GetGlobalItem<EnchantedItem>();
                    if (spelunker != hiGlobal.spelunker || dangerSense != hiGlobal.dangerSense || hunter != hiGlobal.hunter)
                        check = true;
                    enemySpawnBonus *= hiGlobal.enemySpawnBonus;
                }
                else
                {
                    check = true;
                }
                if(!heldItem.IsAir)
                    enemySpawnBonus /= heldItem.GetGlobalItem<EnchantedItem>().enemySpawnBonus;
                heldItem = Player.HeldItem;
            }
            if (!check)
            {
                foreach (Item armor in Player.armor)
                {
                    if (!armor.vanity && !armor.IsAir)
                    {
                        if (equiptArmor[i].IsAir)
                        {
                            check = true;
                            break;
                        }
                        else if (!armor.GetGlobalItem<EnchantedItem>().equip || equiptArmor[i] != armor)
                        {
                            check = true;
                            break;
                        }
                    }
                    else if (armor.IsAir && !equiptArmor[i].IsAir)
                    {
                        check = true;
                        break;
                    }
                    i++;
                }
            }
            if (check)
            {
                if (Player.HeldItem.type != ItemID.None)
                {
                    EnchantedItem hiGlobal = Player.HeldItem.GetGlobalItem<EnchantedItem>();
                    spelunker = hiGlobal.spelunker;
                    dangerSense = hiGlobal.dangerSense;
                    hunter = hiGlobal.hunter;
                }
                else
                {
                    spelunker = false;
                    dangerSense = false;
                    hunter = false;
                }
                itemScale = 0f;
                manaCost = 0f;
                ammoCost = 0f;
                lifeSteal = 0f;
                enemySpawnBonus = 1f;
                float itemScaleBonus = 0f;
                float manaCostBonus = 0f;
                float ammoCostBonus = 0f;
                float lifeStealBonus = 0f;
                foreach (Item armor in Player.armor)
                {
                    if (!armor.vanity && !armor.IsAir)
                    {
                        for (i = 0; i < EnchantingTable.maxEnchantments; i++)
                        {
                            if (!armor.GetGlobalItem<EnchantedItem>().enchantments[i].IsAir)
                            {
                                float str = ((Enchantments)armor.GetGlobalItem<EnchantedItem>().enchantments[i].ModItem).EnchantmentStrength;
                                switch ((EnchantmentTypeID)((Enchantments)armor.GetGlobalItem<EnchantedItem>().enchantments[i].ModItem).EnchantmentType)
                                {
                                    case EnchantmentTypeID.Size:
                                        itemScaleBonus += str;
                                        break;
                                    case EnchantmentTypeID.ManaCost:
                                        manaCostBonus += str;
                                        break;
                                    case EnchantmentTypeID.AmmoCost:
                                        ammoCostBonus += str;
                                        break;
                                    case EnchantmentTypeID.LifeSteal:
                                        lifeStealBonus += str;
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
                                        enemySpawnBonus *= str;
                                        break;
                                    case EnchantmentTypeID.Peace:
                                        enemySpawnBonus /= str;
                                        break;
                                }
                            }
                        }
                        if (!equiptArmor[i].IsAir) { equiptArmor[i].GetGlobalItem<EnchantedItem>().equip = false; }
                        armor.GetGlobalItem<EnchantedItem>().equip = true;
                        equiptArmor[i] = armor;
                    }
                }
                itemScale += itemScaleBonus / 4;
                manaCost += manaCostBonus / 4;
                ammoCost += ammoCostBonus / 4;
                lifeSteal += lifeStealBonus / 4;
                float heldItemEnemySpawnBonus = Player.HeldItem.IsAir ? 0f : Player.HeldItem.GetGlobalItem<EnchantedItem>().enemySpawnBonus;
                this.enemySpawnBonus = enemySpawnBonus + heldItemEnemySpawnBonus;
            }
            if (spelunker) { Player.AddBuff(9, 1); }
            if(dangerSense) { Player.AddBuff(111, 1); }
            if (hunter) { Player.AddBuff(17, 1); }
            if (allForOneTimer > 0)
            {
                allForOneTimer--;
                if (allForOneTimer == 0)
                {
                    SoundEngine.PlaySound(SoundID.MaxMana);
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
        }//My UI
    }
}
