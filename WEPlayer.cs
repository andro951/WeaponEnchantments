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
using System;
using WeaponEnchantments.Common;
using System.Collections.Generic;

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
        public float enemySpawnBonus = 1f;
        public bool godSlayer = false;
        public bool stickyFavorited = true;
        public bool[] vanillaPlayerBuffsWeapon;
        public bool[] vanillaPlayerBuffsArmor;
        public EStat[] staticStats;
        public Dictionary<int, int> potionBuffs = new Dictionary<int, int>();

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
            equiptArmor = new Item[Player.armor.Length];
            heldItem = new Item();
            confirmationUI = new ConfirmationUI();
            inventoryItemRecord = new Item[102];
            for (int i = 0; i < equiptArmor.Length; i++)
            {
                equiptArmor[i] = new Item();
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
            bool checkArmor = false;
            if (Main.mouseItem.IsAir)
            {
                bool checkWeapon = ItemChanged(Player.HeldItem, heldItem, true);
                if (checkWeapon)
                {
                    if (!Player.HeldItem.IsAir && WEMod.IsWeaponItem(Player.HeldItem) && Player.HeldItem.TryGetGlobalItem(out EnchantedItem hiGlobal))
                    {
                        hiGlobal = Player.HeldItem.GetGlobalItem<EnchantedItem>();
                        hiGlobal.heldItem = true;
                        //SetFalseVanillaBoolBuffs(ref vanillaPlayerBuffsWeapon, hiGlobal);
                        /*for (int k = 0; k < EnchantingTable.maxEnchantments; k++)
                        {
                            AllForOneEnchantmentBasic enchantment = ((AllForOneEnchantmentBasic)hiGlobal.enchantments[k].ModItem);
                            if (!hiGlobal.enchantments[k].IsAir)
                            {
                                CheckPotionBuffs(enchantment, vanillaPlayerBuffsWeapon);
                            }
                        }//vanillaBuffs = enchantments*/
                        enemySpawnBonus *= hiGlobal.enemySpawnBonus;
                        Player.HeldItem.RemoveUntilPositive();
                    }
                    /*else
                        SetFalseVanillaBoolBuffs(ref vanillaPlayerBuffsWeapon);*/
                    if (!heldItem.IsAir && WEMod.IsWeaponItem(heldItem))
                    {
                        hiGlobal = heldItem.GetGlobalItem<EnchantedItem>();
                        //check = !check ? CheckWeaponBuffs(hiGlobal) : check;
                        //SetFalseVanillaBoolBuffs(ref vanillaPlayerBuffsWeapon, hiGlobal, true);
                        enemySpawnBonus /= heldItem.GetGlobalItem<EnchantedItem>().enemySpawnBonus;
                        hiGlobal.heldItem = false;
                    }
                    CheckUpdateEnchantmentsOnItem(Player.HeldItem);
                    UpdatePotionBuffs(Player.HeldItem, heldItem);
                    heldItem = Player.HeldItem;
                }//Check HeldItem
            }
            else if(WEMod.IsEnchantable(Main.mouseItem))
            {
                Main.mouseItem.RemoveUntilPositive();
                CheckUpdateEnchantmentsOnItem(Main.mouseItem);
            }//Check too many enchantments on mouseItem
            if (!checkArmor)
            {
                for(int j = 0; j < Player.armor.Length; j++)
                {
                    Item armor = Player.armor[j];
                    if (j < 10)
                    {
                        if (!armor.vanity)
                        {
                            checkArmor = ItemChanged(armor, equiptArmor[j]);
                            if (checkArmor)
                                break;
                        }
                    }
                }
            }//Check if armor changed 
            if (checkArmor)
            {
                //SetFalseVanillaBoolBuffs(ref vanillaPlayerBuffsArmor);
                itemScale = 0f;
                manaCost = 0f;
                ammoCost = 0f;
                lifeSteal = 0f;
                enemySpawnBonus = 1f;
                float itemScaleBonus = 0f;
                float manaCostBonus = 0f;
                float ammoCostBonus = 0f;
                float lifeStealBonus = 0f;
                for(int j = 0; j < Player.armor.Length; j++)
                {
                    Item armor = Player.armor[j];
                    if (j < 10)
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
                                    }//Pop off excess
                                    else
                                    {
                                        armor.RemoveUntilPositive();
                                        AllForOneEnchantmentBasic enchantment = ((AllForOneEnchantmentBasic)armor.GetGlobalItem<EnchantedItem>().enchantments[i].ModItem);
                                        float str = enchantment.EnchantmentStrength;
                                        switch ((EnchantmentTypeID)enchantment.EnchantmentType)
                                        {
                                            case EnchantmentTypeID.Scale:
                                                itemScaleBonus += str;
                                                break;
                                            case EnchantmentTypeID.Mana:
                                                manaCostBonus += str;
                                                break;
                                            case EnchantmentTypeID.AmmoCost:
                                                ammoCostBonus += str;
                                                break;
                                            case EnchantmentTypeID.LifeSteal:
                                                lifeStealBonus += str;
                                                break;
                                            case EnchantmentTypeID.War:
                                                enemySpawnBonus *= 1 + str;
                                                break;
                                            case EnchantmentTypeID.Peace:
                                                enemySpawnBonus /= 1 + str;
                                                break;
                                        }//switch(enchantment)
                                    }
                                }
                            }
                            CheckUpdateEnchantmentsOnItem(armor);
                            UpdatePotionBuffs(armor, equiptArmor[j]);
                        }
                        if (!equiptArmor[j].IsAir)
                        {
                            Item temp = equiptArmor[j];
                            temp.GetGlobalItem<EnchantedItem>().equip = false;
                        }
                        if (!armor.IsAir)
                        {
                            armor.GetGlobalItem<EnchantedItem>().equip = true;
                        }
                        equiptArmor[j] = armor;
                    }
                }
                itemScale += itemScaleBonus / 4;
                manaCost += manaCostBonus / 4;
                ammoCost += ammoCostBonus / 4;
                lifeSteal += lifeStealBonus / 4;
                float heldItemEnemySpawnBonus = Player.HeldItem.IsAir ? 1f : Player.HeldItem.GetGlobalItem<EnchantedItem>().enemySpawnBonus;
                enemySpawnBonus *= heldItemEnemySpawnBonus;
            }//Update bonuses
            foreach(int key in potionBuffs.Keys)
            {
                Player.AddBuff(key, 1);
            }
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
                    SoundEngine.PlaySound(SoundID.MaxMana);
                }
            }
        }
        private bool ItemChanged(Item current, Item previous, bool weapon = false)
        {
            if (!current.IsAir)
            {
                EnchantedItem cGlobal = current.GetGlobalItem<EnchantedItem>();
                if (previous.IsAir)
                {
                    return true;
                }
                else if (WEMod.IsEnchantable(current) && (weapon && !cGlobal.heldItem || !weapon && !cGlobal.equip))
                {
                    return true;
                }
            }
            else if (!previous.IsAir)
            {
                return true;
            }
            return false;
        }
        /*private void SetFalseVanillaBoolBuffs(ref bool[] vanillaBuffsBool, EnchantedItem hiGlobal = null, bool globalOnly = false)
        {
            if (!globalOnly)
            {
                for (int k = 0; k < vanillaPlayerBuffsWeapon.Length; k++)
                {
                    vanillaBuffsBool[k] = false;
                }//vanillaBuffs = false
            }
            if (hiGlobal != null)
            {
                for (int k = 0; k < vanillaPlayerBuffsWeapon.Length; k++)
                {
                    hiGlobal.vanillaPlayerBuffs[k] = false;
                }//vanillaBuffs = false
            }
        }*/
        private void UpdatePotionBuffs(Item newItem, Item oldItem)
        {
            UpdatePotionBuff(newItem);
            UpdatePotionBuff(oldItem, true);
            /*int l = 0;
            foreach (VanillaBoolBuffs boolBuff in (VanillaBoolBuffs[])Enum.GetValues(typeof(VanillaBoolBuffs)))
            {
                if (enchantment.EnchantmentTypeName == boolBuff.ToString())
                {
                    vanillaBuffsBool[l] = true;
                }
                l++;
            }*/
        }
        private static void UpdatePotionBuff(Item item, bool remove = false)
        {
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
        }
        public void CheckUpdateEnchantmentsOnItem(Item item) 
        {
            if (WEMod.IsEnchantable(item))
            {
                for (int k = 0; k < EnchantingTable.maxEnchantments; k++)
                {
                    if (!item.GetGlobalItem<EnchantedItem>().statsSet[k])
                    {
                        item.UpdateEnchantment((AllForOneEnchantmentBasic)item.GetGlobalItem<EnchantedItem>().enchantments[k].ModItem);
                    }
                }
            }
        }
    }
}
