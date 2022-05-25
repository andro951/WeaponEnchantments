using System.Collections.Generic;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using WeaponEnchantments.UI;
using WeaponEnchantments.Items;
using WeaponEnchantments.Common.Globals;
using static WeaponEnchantments.Items.Enchantments;
using System;
using Terraria.UI;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.DataStructures;
using WeaponEnchantments.Common;

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
        public Item trashItem = new Item();
        public bool spelunker = false;
        public bool dangerSense = false;
        public bool hunter = false;
        public float enemySpawnBonus = 1f;
        public bool godSlayer = false;
        public bool stickyFavorited = true;
        /*
        public override void Load()
        {
            //IL.Terraria.Player.ItemCheck_MeleeHitNPCs += HookItemCheck_MeleeHitNPCs;
        }
        public static void HookItemCheck_MeleeHitNPCs(ILContext il)
        {
            var c = new ILCursor(il);

            if (!c.TryGotoNext(MoveType.After,
                i => i.MatchCall(out _),
                i => i.MatchLdcI4(1),
                i => i.MatchLdcI4(101),
                i => i.MatchCallvirt(out _),
                i => i.MatchLdloc(7),
                i => i.MatchBgt(out _),
                i => i.MatchLdcI4(1)
            )) { throw new Exception("Failed to find instructions HookItemCheck_MeleeHitNPCs"); }
            c.Emit(OpCodes.Pop);
            c.Emit(OpCodes.Ldc_I4_0);
        }*/
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
                Item enchantingTableSlotItem = null;
                for (int j = 0; j < EnchantingTable.maxItems; j++)
                {
                    if (enchantingTableUI.itemSlotUI[j].contains)
                    {
                        stop = true;
                        enchantingTableSlotItem = enchantingTableUI.itemSlotUI[j].Item;
                    }
                }//Check itemSlot(s)
                for (int j = 0; j < EnchantingTable.maxEnchantments && !stop; j++)
                {
                    if (enchantingTableUI.enchantmentSlotUI[j].contains)
                    {
                        stop = true;
                        enchantingTableSlotItem = enchantingTableUI.enchantmentSlotUI[j].Item;
                    }
                }//Check enchantmentSlots
                for (int j = 0; j < EnchantingTable.maxEssenceItems && !stop; j++)
                {
                    if (enchantingTableUI.essenceSlotUI[j].contains)
                    {
                        stop = true;
                        enchantingTableSlotItem = enchantingTableUI.essenceSlotUI[j].Item;
                    }
                }//Check essenceSlots
                if (stop)
                {
                    bool itemWillBeTrashed = true;
                    for(int i = 49; i >= 0 && itemWillBeTrashed; i--)
                    {
                        if (Player.inventory[i].IsAir || (Player.inventory[i].type == enchantingTableSlotItem.type && Player.inventory[i].stack < Player.inventory[i].maxStack))
                        {
                            itemWillBeTrashed = false;
                        }
                    }
                    if (itemWillBeTrashed)
                    {
                        return true;
                    }
                }//Prevent Trashing item TODO: Edit this if you ever make ammo bags enchantable
                if (!stop)
                {
                    CheckShiftClickValid(ref inventory[slot], true);
                    return true;
                }//Move Item
            }
            return false;
        }
        public void CheckShiftClickValid(ref Item item, bool moveItem = false)
        {
            bool valid = false;
            if (Main.mouseItem.IsAir)
            {
                if (!Player.trashItem.IsAir)
                {
                    if (!Player.trashItem.GetGlobalItem<EnchantedItem>().trashItem)
                    {
                        if (!trashItem.IsAir)
                        {
                            trashItem.GetGlobalItem<EnchantedItem>().trashItem = false;
                        }
                        Player.trashItem.GetGlobalItem<EnchantedItem>().trashItem = true;
                    }
                }
                else
                {
                    if (!trashItem.IsAir)
                    {
                        trashItem.GetGlobalItem<EnchantedItem>().trashItem = false;
                    }
                }
                bool hoveringOverTrash = false;
                if (!item.IsAir)
                {
                    if(item.GetGlobalItem<EnchantedItem>().trashItem)
                        hoveringOverTrash = true;
                }
                if (!hoveringOverTrash)
                {
                    if (item.type == PowerBooster.ID && !enchantingTableUI.itemSlotUI[0].Item.IsAir && !enchantingTableUI.itemSlotUI[0].Item.GetGlobalItem<EnchantedItem>().powerBoosterInstalled)
                    {
                        if (moveItem)
                        {
                            enchantingTableUI.itemSlotUI[0].Item.GetGlobalItem<EnchantedItem>().powerBoosterInstalled = true;
                            if (item.stack > 1)
                            {
                                item.stack--;
                            }
                            else
                            {
                                item = new Item();
                            }
                            SoundEngine.PlaySound(SoundID.Grab);
                        }
                        valid = true;
                    }//Check Power Booster
                    else
                    {
                        for (int i = 0; i < EnchantingTable.maxItems; i++)
                        {
                            if (enchantingTableUI.itemSlotUI[i].Valid(item))
                            {
                                if (!item.IsAir)
                                {
                                    bool doNotSwap = false;
                                    if(item.TryGetGlobalItem(out EnchantedItem iGlobal))
                                    {
                                        Item tableItem = enchantingTableUI.itemSlotUI[0].Item;
                                        if (iGlobal.equip && !tableItem.IsAir)
                                        {
                                            if(item.accessory && !tableItem.accessory || item.headSlot > 0 && tableItem.headSlot == -1 || item.bodySlot > 0 && tableItem.bodySlot == -1 || item.legSlot > 0 && tableItem.legSlot == -1)
                                            {
                                                doNotSwap = true;
                                            }
                                        }
                                    }
                                    if (!doNotSwap)
                                    {
                                        if (moveItem)
                                        {
                                            enchantingTableUI.itemSlotUI[i].Item = item.Clone();
                                            item = itemInEnchantingTable ? itemBeingEnchanted : new Item();
                                            SoundEngine.PlaySound(SoundID.Grab);
                                        }
                                        valid = true;
                                        break;
                                    }
                                }
                            }
                        }//Check/Move item
                        if (!valid)
                        {
                            if (item.ModItem is Enchantments enchantment)
                            {
                                int uniqueItemSlot = WEUIItemSlot.FindSwapEnchantmentSlot(enchantment, enchantingTableUI.itemSlotUI[0].Item);
                                for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
                                {
                                    if (enchantingTableUI.enchantmentSlotUI[i].Valid(item))
                                    {
                                        if (!item.IsAir)
                                        {
                                            if (enchantingTableUI.enchantmentSlotUI[i].Item.IsAir && uniqueItemSlot == -1)
                                            {
                                                if (moveItem)
                                                {
                                                    int s = i;
                                                    if (enchantment.Utility && enchantingTableUI.enchantmentSlotUI[4].Item.IsAir)
                                                    {
                                                        s = 4;
                                                    }
                                                    enchantingTableUI.enchantmentSlotUI[s].Item = item.Clone();
                                                    enchantingTableUI.enchantmentSlotUI[s].Item.stack = 1;
                                                    if (item.stack > 1)
                                                    {
                                                        item.stack--;
                                                    }
                                                    else
                                                    {
                                                        item = new Item();
                                                    }
                                                    SoundEngine.PlaySound(SoundID.Grab);
                                                }
                                                valid = true;
                                                break;
                                            }//Empty slot or not a unique enchantment
                                            else if (uniqueItemSlot != -1 && enchantingTableUI.enchantmentSlotUI[i].CheckUniqueSlot(enchantment, uniqueItemSlot) && item.type != enchantingTableUI.enchantmentSlotUI[i].Item.type)
                                            {
                                                if (moveItem)
                                                {
                                                    Item returnItem = enchantingTableUI.enchantmentSlotUI[i].Item.Clone();
                                                    enchantingTableUI.enchantmentSlotUI[i].Item = item.Clone();
                                                    enchantingTableUI.enchantmentSlotUI[i].Item.stack = 1;
                                                    if (!returnItem.IsAir)
                                                    {
                                                        if (item.stack > 1)
                                                        {
                                                            Player.QuickSpawnItem(Player.GetSource_Misc("PlayerDropItemCheck"), returnItem);
                                                            item.stack--;
                                                        }
                                                        else
                                                        {
                                                            item = returnItem;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        item = new Item();
                                                    }
                                                    SoundEngine.PlaySound(SoundID.Grab);
                                                }
                                                valid = true;
                                                break;
                                            }//Check unique can swap
                                        }
                                    }
                                }
                            }
                        }//Check/Move Enchantment
                        if (!valid)
                        {
                            for (int i = 0; i < EnchantingTable.maxEssenceItems; i++)
                            {
                                if (enchantingTableUI.essenceSlotUI[i].Valid(item))
                                {
                                    if (!item.IsAir)
                                    {
                                        bool canTransfer = false;
                                        if (enchantingTableUI.essenceSlotUI[i].Item.IsAir)
                                        {
                                            if (moveItem)
                                            {
                                                enchantingTableUI.essenceSlotUI[i].Item = item.Clone();
                                                item = new Item();
                                            }
                                            canTransfer = true;
                                        }//essence slot empty
                                        else
                                        {
                                            if (enchantingTableUI.essenceSlotUI[i].Item.stack < EnchantmentEssence.maxStack)
                                            {
                                                if (moveItem)
                                                {
                                                    int ammountToTransfer;
                                                    if (item.stack + enchantingTableUI.essenceSlotUI[i].Item.stack > EnchantmentEssence.maxStack)
                                                    {
                                                        ammountToTransfer = EnchantmentEssence.maxStack - enchantingTableUI.essenceSlotUI[i].Item.stack;
                                                        item.stack -= ammountToTransfer;
                                                    }
                                                    else
                                                    {
                                                        ammountToTransfer = item.stack;
                                                        item.stack = 0;
                                                    }
                                                    enchantingTableUI.essenceSlotUI[i].Item.stack += ammountToTransfer;
                                                }
                                                canTransfer = true;
                                            }
                                        }//Essence slot not empty
                                        if (canTransfer)
                                        {
                                            if (moveItem)
                                                SoundEngine.PlaySound(SoundID.Grab);
                                            valid = true;
                                            break;
                                        }//Common to all essence transfer
                                    }
                                }
                            }
                        }//Check/Move Essence
                    }
                }
                if (!valid && moveItem)
                {
                    Main.mouseItem = item.Clone();
                    item = new Item();
                }//Pick up item
                else if (valid && !moveItem && !hoveringOverTrash)
                {
                    Main.cursorOverride = 9;
                }
            }
            else if(item.IsAir && moveItem)
            { 
                item = Main.mouseItem.Clone();
                Main.mouseItem = new Item();
            }//Put item down
        }
        public void StoreLastFocusRecipe()
        {
            lastFocusRecipe = Main.availableRecipe[Main.focusRecipe];
        }
        public override void PostUpdate()
        {
            bool check = false;
            bool skipHeldItemCheck = false;
            int i = 0;
            if (Main.mouseItem.IsAir)
            {
                if (Player.HeldItem.TryGetGlobalItem(out EnchantedItem hiGlobal))
                {
                    if (hiGlobal.heldItem)
                    {
                        skipHeldItemCheck = true;
                    }
                }
                else if (Player.HeldItem.IsAir && heldItem.IsAir)
                {
                    skipHeldItemCheck = true;
                }
                if (!skipHeldItemCheck)
                {
                    //EnchantedItem hiGlobal;
                    if (Player.HeldItem.type != ItemID.None)
                    {
                        hiGlobal = Player.HeldItem.GetGlobalItem<EnchantedItem>();
                        hiGlobal.heldItem = true;
                        if (spelunker != hiGlobal.spelunker || dangerSense != hiGlobal.dangerSense || hunter != hiGlobal.hunter)
                            check = true;
                        enemySpawnBonus *= hiGlobal.enemySpawnBonus;
                        if (hiGlobal.GetLevelsAvailable() < 0)
                        {
                            for (int k = EnchantingTable.maxEnchantments - 1; k >= 0 && hiGlobal.GetLevelsAvailable() < 0; k--)
                            {
                                if (!hiGlobal.enchantments[k].IsAir)
                                {
                                    hiGlobal.enchantments[i] = Player.GetItem(Main.myPlayer, hiGlobal.enchantments[i], GetItemSettings.LootAllSettings);
                                }
                                if (!hiGlobal.enchantments[k].IsAir)
                                {
                                    Player.QuickSpawnItem(Player.GetSource_Misc("PlayerDropItemCheck"), hiGlobal.enchantments[k]);
                                    hiGlobal.enchantments[k] = new Item();
                                }
                            }
                            Main.NewText("Your " + Player.HeldItem.Name + "' level is too low to use that many enchantments.");
                        }//Check too many enchantments on helditem
                    }
                    else
                    {
                        check = true;
                    }
                    if (!heldItem.IsAir)
                    {
                        enemySpawnBonus /= heldItem.GetGlobalItem<EnchantedItem>().enemySpawnBonus;
                        heldItem.GetGlobalItem<EnchantedItem>().heldItem = false;
                    }
                    heldItem = Player.HeldItem;
                }//Check HeldItem
            }
            else
            {
                if(Main.mouseItem.TryGetGlobalItem(out EnchantedItem miGlobal))
                {
                    if (miGlobal.GetLevelsAvailable() < 0)
                    {
                        for (int k = EnchantingTable.maxEnchantments - 1; k >= 0 && miGlobal.GetLevelsAvailable() < 0; k--)
                        {
                            if (!miGlobal.enchantments[k].IsAir)
                            {
                                miGlobal.enchantments[i] = Player.GetItem(Main.myPlayer, miGlobal.enchantments[i], GetItemSettings.LootAllSettings);
                            }
                            if (!miGlobal.enchantments[k].IsAir)
                            {
                                Player.QuickSpawnItem(Player.GetSource_Misc("PlayerDropItemCheck"), miGlobal.enchantments[k]);
                                miGlobal.enchantments[k] = new Item();
                            }
                        }
                        Main.NewText("Your " + Main.mouseItem.Name + "' level is too low to use that many enchantments.");
                    }
                }//Check too many enchantments on mouseItem
            }
            if (!check)
            {
                foreach (Item armor in Player.armor)
                {
                    if(i < 10)
                    {
                        if (!armor.vanity && !armor.IsAir)
                        {
                            if (equiptArmor[i].IsAir)
                            {
                                check = true;
                                break;
                            }
                            else if (!armor.GetGlobalItem<EnchantedItem>().equip)
                            {
                                check = true;
                                break;
                            }
                            else if (!equiptArmor[i].IsSameEnchantedItem(armor))
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
                    }
                    i++;
                }
            }//Check Armor
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
                int j = 0;
                foreach (Item armor in Player.armor)
                {
                    if (j < 10)
                    {
                        if (!armor.vanity && !armor.IsAir)
                        {
                            for (i = 0; i < EnchantingTable.maxEnchantments; i++)
                            {
                                if (!armor.GetGlobalItem<EnchantedItem>().enchantments[i].IsAir)
                                {
                                    if(i > 1 && i < 4 || i > 0 && armor.accessory)
                                    {
                                        armor.GetGlobalItem<EnchantedItem>().enchantments[i] = Player.GetItem(Main.myPlayer, armor.GetGlobalItem<EnchantedItem>().enchantments[i], GetItemSettings.LootAllSettings);
                                        if (!armor.GetGlobalItem<EnchantedItem>().enchantments[i].IsAir)
                                        {
                                            Player.QuickSpawnItem(Player.GetSource_Misc("PlayerDropItemCheck"), armor.GetGlobalItem<EnchantedItem>().enchantments[i]);
                                            armor.GetGlobalItem<EnchantedItem>().enchantments[i] = new Item();
                                            if (armor.accessory)
                                            {
                                                Main.NewText("Accessories can only equip an enchantment in the first slot");
                                            }
                                            else
                                            {
                                                Main.NewText("Armor can only equip enchantments in the first 2 slots and the utility slot");
                                            }
                                        }
                                    }
                                    else
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
                                                enemySpawnBonus *= 1 + str;
                                                break;
                                            case EnchantmentTypeID.Peace:
                                                enemySpawnBonus /= 1 + str;
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                        if (!equiptArmor[j].IsAir)
                        {
                            equiptArmor[j].GetGlobalItem<EnchantedItem>().equip = false;
                        }
                        if (!armor.IsAir)
                        {
                            armor.GetGlobalItem<EnchantedItem>().equip = true;
                            equiptArmor[j] = armor.Clone();
                        }
                        else
                        {
                            equiptArmor[j].TurnToAir();
                        }
                    }
                    j++;
                }
                itemScale += itemScaleBonus / 4;
                manaCost += manaCostBonus / 4;
                ammoCost += ammoCostBonus / 4;
                lifeSteal += lifeStealBonus / 4;
                float heldItemEnemySpawnBonus = Player.HeldItem.IsAir ? 0f : Player.HeldItem.GetGlobalItem<EnchantedItem>().enemySpawnBonus;
                this.enemySpawnBonus = enemySpawnBonus + heldItemEnemySpawnBonus;
            }//Update bonuses
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
    }
}
