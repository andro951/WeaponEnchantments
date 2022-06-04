using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Items;
using WeaponEnchantments.UI;

namespace WeaponEnchantments
{
    public static class PlayerFunctions
    {
        public static void CheckWeapon(this Item newItem, ref Item oldItem)
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            bool checkWeapon = wePlayer.ItemChanged(newItem, oldItem, true);
            if (checkWeapon)
            {
                ("\\/CheckWeapon(" + (newItem != null ? newItem.Name : "null ") + ", " + (oldItem != null ? oldItem.Name : "null ") + ") -after if(checkWeapon)").Log();
                if (!newItem.IsAir && newItem.TryGetGlobalItem(out EnchantedItem newGlobal))
                {
                    newGlobal.trackedWeapon = true;
                    //newItem.RemoveUntilPositive();
                    //CheckUpdateEnchantmentsOnItem(newItem);
                }
                if (!oldItem.IsAir && oldItem.TryGetGlobalItem(out EnchantedItem oldGlobal))
                {
                    oldGlobal.trackedWeapon = false;
                }
                wePlayer.UpdatePotionBuffs(ref newItem, ref oldItem);
                wePlayer.UpdatePlayerStats(ref newItem, ref oldItem);
                oldItem = newItem;
                ("/\\CheckWeapon(" + (newItem != null ? newItem.Name : "null ") + ", " + (oldItem != null ? oldItem.Name : "null ") + ") -after if(checkWeapon)").Log();
            }//Check HeldItem
        }
    }
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
        public Item[] equipArmor;
        public bool[] equipArmorStatsUpdated;
        public Item trackedWeapon;
        public Item hoverItem;
        int hoverItemIndex = 0;
        int hoverItemChest = 0;
        public Item trashItem = new Item();
        public float enemySpawnBonus = 1f;
        public bool godSlayer = false;
        public bool stickyFavorited = true;
        public bool[] vanillaPlayerBuffsWeapon;
        public bool[] vanillaPlayerBuffsArmor;
        public Dictionary<int, int> potionBuffs = new Dictionary<int, int>();
        public Dictionary<string, StatModifier> statModifiers = new Dictionary<string, StatModifier>();
        public Dictionary<string, StatModifier> appliedStatModifiers = new Dictionary<string, StatModifier>();
        public Dictionary<string, StatModifier> eStats = new Dictionary<string, StatModifier>();
        //public Dictionary<string, int> boolPreventedFields = new Dictionary<string, int>();
        //public Dictionary<string, int> boolFields = new Dictionary<string, int>();
        //public Dictionary<string, StaticStatStruct> staticStats = new Dictionary<string, StaticStatStruct>();

        /*public enum VanillaBoolBuffs : int
        {
            Spelunker = 9,
            Hunter = 17,
            DangerSense = 111
        }
        public enum StaticStats : int
        {

        }*/
        public override void Load()
        {
            IL.Terraria.Player.ItemCheck_MeleeHitNPCs += HookItemCheck_MeleeHitNPCs;
        }
        public override void OnEnterWorld(Player player)
        {
            OldItemManager.ReplaceAllOldItems();
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
        }
        public override void Initialize()
        {
            enchantingTable = new EnchantingTable();
            enchantingTableUI = new WeaponEnchantmentUI();
            equipArmor = new Item[Player.armor.Length];
            equipArmorStatsUpdated = new bool[Player.armor.Length];
            trackedWeapon = new Item();
            confirmationUI = new ConfirmationUI();
            inventoryItemRecord = new Item[102];
            for (int i = 0; i < equipArmor.Length; i++)
            {
                equipArmor[i] = new Item();
            }
            //vanillaPlayerBuffsWeapon = new bool[Enum.GetNames(typeof(VanillaBoolBuffs)).Length];
            //vanillaPlayerBuffsArmor = new bool[vanillaPlayerBuffsWeapon.Length];
            /*staticStats = new EStat[Enum.GetNames(typeof(StaticStats)).Length];
            for(int i = 0; i < staticStats.Length; i++)
            {
                staticStats[i] = new EStat();
            }*/
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
                    Item tableItem = enchantingTableUI.itemSlotUI[0].Item;
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
                                        if (iGlobal.equip && !tableItem.IsAir)
                                        {
                                            if(WEMod.IsAccessoryItem(item) && !WEMod.IsArmorItem(item) && (WEMod.IsAccessoryItem(tableItem) || WEMod.IsArmorItem(tableItem)) || item.headSlot > -1 && tableItem.headSlot == -1 || item.bodySlot > -1 && tableItem.bodySlot == -1 || item.legSlot > -1 && tableItem.legSlot == -1)
                                            {
                                                doNotSwap = true;
                                            }//Fix for Armor Modifiers & Reforging setting item.accessory to true to allow reforging armor
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
                            if (item.ModItem is AllForOneEnchantmentBasic enchantment)
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
                                                    if (enchantment.Utility && enchantingTableUI.enchantmentSlotUI[4].Item.IsAir && (WEMod.IsWeaponItem(tableItem) || WEMod.IsArmorItem(tableItem)))
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
                                            if (enchantingTableUI.essenceSlotUI[i].Item.stack < EnchantmentEssenceBasic.maxStack)
                                            {
                                                if (moveItem)
                                                {
                                                    int ammountToTransfer;
                                                    if (item.stack + enchantingTableUI.essenceSlotUI[i].Item.stack > EnchantmentEssenceBasic.maxStack)
                                                    {
                                                        ammountToTransfer = EnchantmentEssenceBasic.maxStack - enchantingTableUI.essenceSlotUI[i].Item.stack;
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
            if (Main.mouseItem.IsAir)
            {
                /*bool checkWeapon = ItemChanged(Player.HeldItem, heldItem, true);
                if (checkWeapon)
                {
                    if (!Player.HeldItem.IsAir && WEMod.IsWeaponItem(Player.HeldItem) && Player.HeldItem.TryGetGlobalItem(out EnchantedItem hiGlobal))
                    {
                        hiGlobal = Player.HeldItem.GetGlobalItem<EnchantedItem>();
                        hiGlobal.heldItem = true;
                        Player.HeldItem.RemoveUntilPositive();
                        CheckUpdateEnchantmentsOnItem(Player.HeldItem);
                    }
                    if (!heldItem.IsAir && WEMod.IsWeaponItem(heldItem))
                    {
                        hiGlobal = heldItem.GetGlobalItem<EnchantedItem>();
                        hiGlobal.heldItem = false;
                    }
                    UpdatePotionBuffs(Player.HeldItem, heldItem);
                    UpdatePlayerStats(Player.HeldItem, heldItem);
                    heldItem = Player.HeldItem;
                }//Check HeldItem*/
                Player.HeldItem.CheckWeapon(ref trackedWeapon);
            }
            else if(WEMod.IsEnchantable(Main.mouseItem))
            {
                Main.mouseItem.CheckWeapon(ref trackedWeapon);
                //Main.mouseItem.RemoveUntilPositive();
            }//Check too many enchantments on mouseItem
            //(Main.HoverItem.Name + " Main.HoverItem != null: " + (Main.HoverItem != null) + " && WEMod.IsWeaponItem(Main.HoverItem): " + WEMod.IsWeaponItem(Main.HoverItem) + " && !Main.HoverItem.G().trackedWeapon: " + (Main.HoverItem != null && WEMod.IsWeaponItem(Main.HoverItem) && !Main.HoverItem.G().trackedWeapon) + " && !Main.HoverItem.G().hoverItem: " + (Main.HoverItem != null && WEMod.IsWeaponItem(Main.HoverItem) && !Main.HoverItem.G().trackedWeapon && !Main.HoverItem.G().hoverItem)).LogT();
            if (Main.HoverItem != null && WEMod.IsWeaponItem(Main.HoverItem) && !Main.HoverItem.G().trackedWeapon && !Main.HoverItem.G().hoverItem)
            {
                ("\\/Start hoverItem check").Log();
                Item newItem = null;
                if(usingEnchantingTable && UtilityMethods.IsSameEnchantedItem(enchantingTableUI.itemSlotUI[0].Item, Main.HoverItem))
                    newItem = enchantingTableUI.itemSlotUI[0].Item;
                if (newItem != null && UtilityMethods.IsSameEnchantedItem(Player.inventory[hoverItemIndex], Main.HoverItem))
                    newItem = Player.inventory[hoverItemIndex];
                if(newItem != null && Player.chest != -1)
                {
                    Item[] inventory = null;
                    switch (hoverItemChest)
                    {
                        case > -1:
                            inventory = Main.chest[hoverItemChest].item;
                            break;
                        case -2:
                            inventory = Player.bank.item;
                            break;
                        case -3:
                            inventory = Player.bank2.item;
                            break;
                        case -4:
                            inventory = Player.bank3.item;
                            break;
                        case -5:
                            inventory = Player.bank4.item;
                            break;
                    }
                    if (UtilityMethods.IsSameEnchantedItem(inventory[hoverItemIndex], Main.HoverItem))
                        newItem = inventory[hoverItemIndex];
                }
                if (newItem == null)
                {
                    for (int i = 0; i < Player.inventory.Length; i++)
                    {
                        if (WEMod.IsWeaponItem(Player.inventory[i]))
                        {
                            if (UtilityMethods.IsSameEnchantedItem(Player.inventory[i], Main.HoverItem))
                            {
                                //Player.inventory[i].G().hoverItem = true;
                                hoverItemIndex = i;
                                newItem = Player.inventory[i];
                                break;
                            }
                        }
                    }
                }
                if(Player.chest != -1 && newItem == null)
                {
                    Item[] inventory = null;
                    switch(Player.chest)
                    {
                        case > -1:
                            inventory = Main.chest[Player.chest].item;
                            break;
                        case -2:
                            inventory = Player.bank.item;
                            break;
                        case -3:
                            inventory = Player.bank2.item;
                            break;
                        case -4:
                            inventory = Player.bank3.item;
                            break;
                        case -5:
                            inventory = Player.bank4.item;
                            break;
                    }
                    for(int i = 0; i < inventory.Length; i++)
                    {
                        Item chestItem = inventory[i];
                        if (WEMod.IsWeaponItem(chestItem))
                        {
                            if (UtilityMethods.IsSameEnchantedItem(chestItem, Main.HoverItem))
                            {
                                //chestItem.G().hoverItem = true;
                                hoverItemIndex = i;
                                newItem = chestItem;
                                hoverItemChest = Player.chest;
                                break;
                            }
                        }
                    }
                }
                ("newItem: " + newItem.S()).Log();
                bool checkWeapon = ItemChanged(newItem, hoverItem, true);
                ("checkWeapon: " + ItemChanged(newItem, hoverItem, true)).Log();
                if (checkWeapon)
                {
                    if (newItem != null && !newItem.IsAir)
                        newItem.G().hoverItem = true;
                    //newItem.RemoveUntilPositive();
                    //CheckUpdateEnchantmentsOnItem(newItem);
                    if (hoverItem != null && !hoverItem.IsAir)
                    {
                        hoverItem.G().hoverItem = false;
                    }
                    hoverItem = newItem;
                    UpdateItemStats(ref newItem);
                }//Check HeldItem
                ("/\\End hoverItem check Item: " + (newItem != null ? newItem.Name : "null ")).Log();
            }
            else
            {
                if (hoverItem != null && !hoverItem.IsAir && (!WEMod.IsWeaponItem(Main.HoverItem) || !Main.HoverItem.G().trackedWeapon && !Main.HoverItem.G().hoverItem))
                {
                    ("remove hoverItem: " + hoverItem.Name).Log();
                    hoverItem.G().hoverItem = false;
                    hoverItem = null;
                }
            }
            for (int j = 0; j < Player.armor.Length; j++)
            {
                Item armor = Player.armor[j];
                if (j < 10)
                {
                    if (!armor.vanity)
                    {
                        equipArmorStatsUpdated[j] = !ItemChanged(armor, equipArmor[j]);
                    }
                }
            }//Check if armor changed 
            //SetFalseVanillaBoolBuffs(ref vanillaPlayerBuffsArmor);
            /*itemScale = 0f;
            manaCost = 0f;
            ammoCost = 0f;
            lifeSteal = 0f;
            enemySpawnBonus = 1f;
            float itemScaleBonus = 0f;
            float manaCostBonus = 0f;
            float ammoCostBonus = 0f;
            float lifeStealBonus = 0f;*/
            for(int j = 0; j < Player.armor.Length; j++)
            {
                Item armor = Player.armor[j];
                if (j < 10 && !equipArmorStatsUpdated[j])
                {
                    if (!armor.vanity && !armor.IsAir)
                    {
                        for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
                        {
                            if (!armor.GetGlobalItem<EnchantedItem>().enchantments[i].IsAir)
                            {
                                if(i > 1 && i < 4 || i > 0 && !WEMod.IsArmorItem(armor))
                                {
                                    armor.GetGlobalItem<EnchantedItem>().enchantments[i] = Main.LocalPlayer.GetItem(Main.myPlayer, armor.GetGlobalItem<EnchantedItem>().enchantments[i], GetItemSettings.LootAllSettings);
                                    if (!armor.GetGlobalItem<EnchantedItem>().enchantments[i].IsAir)
                                    {
                                        Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), armor.GetGlobalItem<EnchantedItem>().enchantments[i]);
                                        armor.GetGlobalItem<EnchantedItem>().enchantments[i] = new Item();
                                        if (WEMod.IsArmorItem(armor))
                                        {
                                            Main.NewText("Armor can only equip enchantments in the first 2 slots and the utility slot");
                                        }
                                        else
                                        {
                                            Main.NewText("Accessories can only equip an enchantment in the first slot");
                                        }
                                    }
                                }//Move to Removewhileuntil below
                            }
                        }//Pop off excess
                        //armor.RemoveUntilPositive();
                        //CheckUpdateEnchantmentsOnItem(armor);
                    }
                    //UpdateStats(armor, equipArmor[j]);
                    UpdatePotionBuffs(ref armor, ref equipArmor[j]);
                    UpdatePlayerStats(ref armor, ref equipArmor[j]);
                    if (!equipArmor[j].IsAir)
                    {
                        Item temp = equipArmor[j];
                        temp.GetGlobalItem<EnchantedItem>().equip = false;
                    }
                    if (!armor.IsAir)
                    {
                        armor.GetGlobalItem<EnchantedItem>().equip = true;
                    }
                    equipArmor[j] = armor;
                }
            }
            /*itemScale += itemScaleBonus / 4;
            manaCost += manaCostBonus / 4;
            ammoCost += ammoCostBonus / 4;
            lifeSteal += lifeStealBonus / 4;
            float heldItemEnemySpawnBonus = Player.HeldItem.IsAir ? 1f : Player.HeldItem.GetGlobalItem<EnchantedItem>().enemySpawnBonus;
            enemySpawnBonus *= heldItemEnemySpawnBonus;*/
            foreach(int key in potionBuffs.Keys)
            {
                Player.AddBuff(key, 1);
            }
            bool updatePlayerStat = false;
            foreach(string key in statModifiers.Keys)
            {
                if(Player.GetType().GetField(key)!= null)
                {
                    appliedStatModifiers.Remove(key);
                    updatePlayerStat = true;
                }
            }
            if(updatePlayerStat)
                UpdatePlayerStat();
            /*for (int k = 0; k < Enum.GetNames(typeof(VanillaBoolBuffs)).Length; k++)
            {
                if (vanillaPlayerBuffsWeapon[k] || vanillaPlayerBuffsArmor[k])
                {
                    VanillaBoolBuffs[] vanillaBoolBuffs = (VanillaBoolBuffs[])Enum.GetValues(typeof(VanillaBoolBuffs));
                    int buff = (int)vanillaBoolBuffs[k];
                    Player.AddBuff(buff, 1);
                }
            }*/
            if (allForOneTimer > 0)
            {
                allForOneTimer--;
                if (allForOneTimer == 0)
                {
                    SoundEngine.PlaySound(SoundID.Unlock);
                }
            }
        }
        public bool ItemChanged(Item current, Item previous, bool weapon = false)
        {
            if (current != null && !current.IsAir)
            {
                EnchantedItem cGlobal = current.GetGlobalItem<EnchantedItem>();
                if(previous == null)
                {
                    return true;
                }
                if (previous.IsAir)
                {
                    return true;
                }
                else if (WEMod.IsEnchantable(current) && (weapon && !cGlobal.trackedWeapon || !weapon && !cGlobal.equip))
                {
                    return true;
                }
            }
            else if ( previous != null && !previous.IsAir)
            {
                return true;
            }
            return false;
        }
        public void UpdatePotionBuffs(ref Item newItem, ref Item oldItem)
        {
            ("\\/UpdatePotionBuffs(" + newItem.S() + ", " + oldItem.S() + ")").Log();
            UpdatePotionBuff(ref newItem);
            UpdatePotionBuff(ref oldItem, true);
            ("/\\UpdatePotionBuffs(" + newItem.S() + ", " + oldItem.S() + ")").Log();
        }
        private static void UpdatePotionBuff(ref Item item, bool remove = false)
        {
            ("\\/UpdatePotionBuff(" + item.S() + ", remove: " + remove + ")").Log();
            if (WEMod.IsEnchantable(item))
            {
                EnchantedItem iGlobal = item.GetGlobalItem<EnchantedItem>();
                WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
                foreach (int key in iGlobal.potionBuffs.Keys)
                {
                    if (wePlayer.potionBuffs.ContainsKey(key))
                        wePlayer.potionBuffs[key] += iGlobal.potionBuffs[key] * (remove ? -1 : 1);
                    else
                        wePlayer.potionBuffs.Add(key, iGlobal.potionBuffs[key]);
                    if (remove && wePlayer.potionBuffs[key] < 1)
                        wePlayer.potionBuffs.Remove(key);
                }
            }
            ("/\\UpdatePotionBuff(" + item.S() + ", remove: " + remove + ")").Log();
        }
        public static StatModifier CombineStatModifier(StatModifier baseStatModifier, StatModifier newStatModifier, bool remove)
        {
            ("\\/CombineStatModifier(baseStatModifier: " + baseStatModifier.S() + ", newStatModifier: " + newStatModifier.S() + ", remove: " + remove + ") StatModifier").Log();
            StatModifier finalModifier;
            if (remove)
            {
                finalModifier = new StatModifier(baseStatModifier.Additive / newStatModifier.Additive, baseStatModifier.Multiplicative / newStatModifier.Multiplicative, baseStatModifier.Flat - newStatModifier.Flat, baseStatModifier.Base - newStatModifier.Base);
            }
            else
            {
                finalModifier = new StatModifier(baseStatModifier.Additive * newStatModifier.Additive, baseStatModifier.Multiplicative * newStatModifier.Multiplicative, baseStatModifier.Flat + newStatModifier.Flat, baseStatModifier.Base + newStatModifier.Base);
            }
            ("/\\CombineStatModifier(baseStatModifier: " + baseStatModifier.S() + ", newStatModifier: " + newStatModifier.S() + ", remove: " + remove + ") return " + finalModifier.S()).Log();
            return finalModifier;
        }
        public static void TryRemoveStat(ref Dictionary<string, StatModifier> dictionary, string key)
        {
            ("\\/TryRemoveStat( dictionary, key: " + key + ") dictionary: " + dictionary.S(key)).Log();
            if (dictionary.ContainsKey(key))
            {
                if((float)Math.Abs(Math.Abs(Math.Round(dictionary[key].Additive, 4)) - 1f) < 1E-4 && (float)Math.Abs(Math.Abs(Math.Round(dictionary[key].Multiplicative, 4)) - 1f) < 1E-4 && Math.Abs(Math.Round(dictionary[key].Flat, 4)) < 1E-4 && Math.Abs(Math.Round(dictionary[key].Base, 4)) < 1E-4)
                {
                    dictionary.Remove(key);
                    (key + " removed").Log();
                }
            }
            ("/\\TryRemoveStat( dictionary, key: " + key + ") dictionary: " + dictionary.S(key)).Log();
        }
        public void UpdatePlayerStats(ref Item newItem, ref Item oldItem)
        {
            ("\\/UpdatePlayerStats(" + newItem.S() + ", " + oldItem.S() + ")").Log();
            if (WEMod.IsEnchantable(newItem))
                UpdatePlayerDictionaries(newItem);
            if (WEMod.IsEnchantable(oldItem))
                UpdatePlayerDictionaries(oldItem, true);
            if (WEMod.IsEnchantable(newItem))
                UpdateItemStats(ref newItem);
            if (!WEMod.IsWeaponItem(newItem))
            {
                Item weapon = null;
                if (!Player.HeldItem.IsAir && Player.HeldItem.G().trackedWeapon)
                    weapon = Player.HeldItem;
                else if (!Main.mouseItem.IsAir && Main.mouseItem.G().trackedWeapon)
                    weapon = Main.mouseItem;
                if (weapon != null)
                    UpdateItemStats(ref weapon);
            }
            UpdatePlayerStat();
            ("/\\UpdatePlayerStats(" + newItem.S() + ", " + oldItem.S() + ")").Log();
        }
        private void UpdatePlayerStat()
        {
            ("\\/UpdatePlayerStat()").LogT();
            foreach (string key in statModifiers.Keys)
            {
                string statName = key.RI();
                bool statsNeedUpdate = true;
                if(appliedStatModifiers.ContainsKey(key))
                    statsNeedUpdate = statModifiers[key] != appliedStatModifiers[key];
                if (statsNeedUpdate)
                {
                    FieldInfo field = Player.GetType().GetField(statName);
                    PropertyInfo property = Player.GetType().GetProperty(statName);
                    if(Player.GetType().GetField(statName) != null || Player.GetType().GetProperty(statName) != null)
                    {
                        if (!appliedStatModifiers.ContainsKey(key))
                            appliedStatModifiers.Add(key, StatModifier.Default);
                        StatModifier lastAppliedStatModifier = appliedStatModifiers[key];
                        appliedStatModifiers[key] = statModifiers[key];
                        StatModifier staticStat = CombineStatModifier(statModifiers[key], lastAppliedStatModifier, true);
                        if (field != null)
                        {
                            (statName.ToString() + ": " + field.GetValue(Player)).LogT();
                            Type fieldType = field.FieldType;
                            if (fieldType == typeof(float))
                            {
                                float finalValue = staticStat.ApplyTo((float)field.GetValue(Player));
                                field.SetValue(Player, finalValue);
                            }//float (field)
                            if (fieldType == typeof(int))
                            {
                                //int valueInt = (int)field.GetValue(Player);
                                float finalValue = staticStat.ApplyTo((float)(int)field.GetValue(Player));
                                field.SetValue(Player, (int)Math.Round(finalValue + 5E-6));
                            }//int (field)
                            if (fieldType == typeof(bool))
                            {
                                bool baseValue = (bool)field.GetValue(new Player());
                                bool finalValue = statModifiers[key].Additive != 1f;
                                field.SetValue(Player, !statModifiers.ContainsKey("P_" + key) && (baseValue || finalValue));
                            }//bool (field)
                            (statName.ToString() + ": " + field.GetValue(Player)).LogT();
                        }//field
                        else if (property != null)
                        {
                            (statName.ToString() + ": " + property.GetValue(Player)).LogT();
                            Type propertyType = property.PropertyType;
                            if (propertyType == typeof(float))
                            {
                                float finalValue = staticStat.ApplyTo((float)property.GetValue(Player));
                                property.SetValue(Player, finalValue);
                            }//float (property)
                            if (propertyType == typeof(int))
                            {
                                //int valueInt = (int)property.GetValue(Player);
                                float finalValue = staticStat.ApplyTo((float)(int)property.GetValue(Player));
                                property.SetValue(Player, (int)Math.Round(finalValue + 5E-6));
                            }//int (property)
                            if (propertyType == typeof(bool))
                            {
                                bool baseValue = (bool)property.GetValue(new Player());
                                bool finalValue = statModifiers[key].Additive != 1f;
                                property.SetValue(Player, !statModifiers.ContainsKey("P_" + key) && (baseValue || finalValue));
                            }//bool (property)
                            (statName.ToString() + ": " + property.GetValue(Player)).LogT();
                        }//property
                    }
                }
            }
            ("/\\UpdatePlayerStat()").LogT();
        }
        private void UpdatePlayerDictionaries(Item item, bool remove = false)
        {
            ("\\/UpdatePlayerDictionaries(" + item.S() + ", remove: " + remove + ") statModifiers.Count: " + item.GetGlobalItem<EnchantedItem>().statModifiers.Count).Log();
            foreach (string key in item.G().statModifiers.Keys)
            {
                string statName = key.RI();
                if (!WEMod.IsWeaponItem(item) || item.GetType().GetField(statName) == null && item.GetType().GetProperty(statName) == null)
                {
                    if(!statModifiers.ContainsKey(key))
                        statModifiers.Add(key, StatModifier.Default);
                    statModifiers[key].S().Log();
                    statModifiers[key] = CombineStatModifier(statModifiers[key], item.G().statModifiers[key], remove);
                    statModifiers[key].S().Log();
                    TryRemoveStat(ref statModifiers, key);
                }
            }
            foreach(string key in item.G().eStats.Keys)
            {
                if (!eStats.ContainsKey(key))
                    eStats.Add(key, StatModifier.Default);
                eStats[key] = CombineStatModifier(eStats[key], item.G().eStats[key], remove);
                TryRemoveStat(ref eStats, key);
            }
            ("/\\UpdatePlayerDictionaries(" + item.S() + ", remove: " + remove + ")").Log();
        }
        public void UpdateItemStats(ref Item item)
        {
            if (WEMod.IsEnchantable(item))
            {
                ("\\/UpdateItemStats(" + item.S() + ")").Log();
                if (item.G().prefix != item.prefix)
                {
                    item.G().appliedStatModifiers.Clear();
                    item.G().prefix = item.prefix;
                }
                Dictionary<string, StatModifier> combinedStatModifiers = new Dictionary<string, StatModifier>();
                foreach (string itemKey in item.G().statModifiers.Keys)
                {
                    if (item.GetType().GetField(itemKey.RI()) != null)
                    {
                        combinedStatModifiers.Add(itemKey, item.G().statModifiers[itemKey]);
                        ("combinedStatModifiers.Add(itemKey: " + itemKey + ", " + item.G().statModifiers.S(itemKey) + ")").Log();
                    }
                }//Populate itemStatModifiers
                if (WEMod.IsWeaponItem(item))
                {
                    foreach (string playerKey in statModifiers.Keys)
                    {
                        if (item.GetType().GetField(playerKey.RI()) != null)
                        {
                            if (combinedStatModifiers.ContainsKey(playerKey))
                            {
                                combinedStatModifiers[playerKey] = combinedStatModifiers[playerKey].CombineWith(statModifiers[playerKey]);
                            }
                            else
                            {
                                combinedStatModifiers.Add(playerKey, statModifiers[playerKey]);
                            }
                            ("combinedStatModifiers.Add(playerKey: " + playerKey + ", " + item.G().statModifiers.S(playerKey) + ")").Log();
                        }
                    }
                }//Populate playerStatModifiers if item is a weapon
                foreach (string key in combinedStatModifiers.Keys)
                {
                    string statName = key.RI();
                    bool statsNeedUpdate = true;
                    if (item.G().appliedStatModifiers.ContainsKey(key))
                        statsNeedUpdate = combinedStatModifiers[key] != item.G().appliedStatModifiers[key];
                    ("statsNeedUpdate: " + statsNeedUpdate + " combinedStatModifiers[" + key + "]: " + combinedStatModifiers.S(key) + " != item.G().appliedStatModifiers[" + key + "]: " + item.G().appliedStatModifiers.S(key)).Log();
                    if (statsNeedUpdate)
                    {
                        if (key.CI())
                            combinedStatModifiers[key] = CombineStatModifier(StatModifier.Default, combinedStatModifiers[key], true);
                        FieldInfo field = item.GetType().GetField(statName);
                        PropertyInfo property = item.GetType().GetProperty(statName);
                        if (item.GetType().GetField(statName) != null || item.GetType().GetProperty(statName) != null)
                        {
                            if (!item.G().appliedStatModifiers.ContainsKey(key))
                                item.G().appliedStatModifiers.Add(key, StatModifier.Default);
                            StatModifier lastAppliedStatModifier = item.G().appliedStatModifiers[key];
                            item.G().appliedStatModifiers[key] = combinedStatModifiers[key];
                            StatModifier staticStat = CombineStatModifier(combinedStatModifiers[key], lastAppliedStatModifier, true);
                            if (field != null)
                            {
                                (statName.ToString() + ": " + field.GetValue(item)).Log();
                                Type fieldType = field.FieldType;
                                if (fieldType == typeof(float))
                                {
                                    float finalValue = staticStat.ApplyTo((float)field.GetValue(item));
                                    field.SetValue(item, finalValue);
                                }//float (field)
                                if (fieldType == typeof(int))
                                {
                                    //int valueInt = (int)field.GetValue(item);
                                    float finalValue = staticStat.ApplyTo((float)(int)field.GetValue(item));
                                    //Item contentSampleItem = new Item(item.type);
                                    staticStat.RoundCheck(ref finalValue, (int)field.GetValue(item), item.G().appliedStatModifiers[key], (int)field.GetValue(ContentSamples.ItemsByType[item.type]));
                                    field.SetValue(item, (int)Math.Round(finalValue + 5E-6));
                                }//int (field)
                                if (fieldType == typeof(bool))
                                {
                                    bool baseValue = (bool)field.GetValue(ContentSamples.ItemsByType[item.type]);
                                    bool finalValue = combinedStatModifiers[key].Additive != 1f;
                                    field.SetValue(item, !combinedStatModifiers.ContainsKey("P_" + key) && (baseValue || finalValue));
                                }//bool (field)
                                (statName.ToString() + ": " + field.GetValue(item)).Log();
                            }//field
                            else if (property != null)
                            {
                                (statName.ToString() + property.GetValue(item)).Log();
                                Type propertyType = property.PropertyType;
                                if (propertyType == typeof(float))
                                {
                                    float finalValue = staticStat.ApplyTo((float)property.GetValue(item));
                                    property.SetValue(item, finalValue);
                                }//float (property)
                                if (propertyType == typeof(int))
                                {
                                    //int valueInt = (int)property.GetValue(item);
                                    float finalValue = (float)(int)property.GetValue(item);
                                    //Item contentSampleItem = ContentSamples.ItemsByType[item.type].Clone();
                                    staticStat.RoundCheck(ref finalValue, (int)property.GetValue(item), item.G().appliedStatModifiers[key], (int)property.GetValue(ContentSamples.ItemsByType[item.type]));
                                    property.SetValue(item, (int)Math.Round(finalValue + 5E-6));
                                }//int (property)
                                if (propertyType == typeof(bool))
                                {
                                    bool baseValue = (bool)property.GetValue(ContentSamples.ItemsByType[item.type]);
                                    bool finalValue = combinedStatModifiers[key].Additive != 1f;
                                    property.SetValue(item, !combinedStatModifiers.ContainsKey("P_" + key) && (baseValue || finalValue));
                                }//bool (property)
                                (statName.ToString() + property.GetValue(item)).Log();
                            }//property
                            TryRemoveStat(ref item.G().appliedStatModifiers, key);
                        }
                    }
                }
            ("/\\UpdateItemStats(" + item.S() + ")").Log();
            }
        }
    }
}
