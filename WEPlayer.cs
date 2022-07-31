using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;
using Terraria.ModLoader.IO;
using Terraria.UI;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Items;
using WeaponEnchantments.UI;

namespace WeaponEnchantments
{
    public static class PlayerFunctions
    {
        public static void CheckWeapon(this Item newItem, ref Item oldItem, Player player, int slot)
        {
            WEPlayer wePlayer = player.GetModPlayer<WEPlayer>();
            bool checkWeapon = wePlayer.ItemChanged(newItem, oldItem, true);
            if (checkWeapon)
            {
                if(LogMethods.debugging) ($"\\/CheckWeapon({newItem.S()}, {oldItem.S()}, player: {player.S()}, slot: {slot}) ").Log();
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
                //if (Main.netMode < NetmodeID.Server)
                {
                    Item newCheckItem = WEMod.IsWeaponItem(newItem) ? newItem : new Item();
                    Item oldCheckItem = WEMod.IsWeaponItem(oldItem) ? oldItem : new Item();
                    wePlayer.UpdatePotionBuffs(ref newCheckItem, ref oldCheckItem);
                    wePlayer.UpdatePlayerStats(ref newCheckItem, ref oldCheckItem);
                    //if (Main.netMode == NetmodeID.MultiplayerClient) ModContent.GetInstance<WEMod>().SendPacket(WEMod.PacketIDs.TransferGlobalItemFields, newCheckItem, oldCheckItem, true, (byte)slot);
                }
                oldItem = newItem;
                if(LogMethods.debugging) ($"/\\CheckWeapon({newItem.S()}, {oldItem.S()}, player: {player.S()}, slot: {slot}) ").Log();
            }//Check HeldItem
        }
    }
    public class WEPlayer : ModPlayer
    {
    	private string name = "";
        public static bool OldWorldItemsReplaced = false;
        public bool usingEnchantingTable;
        public int enchantingTableTier;
        public int highestTableTierUsed;
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
        public Item trackedHoverItem;
        public Item infusionConsumeItem = null;
        public string previousInfusedItemName = "";
        int hoverItemIndex = 0;
        int hoverItemChest = 0;
        public Item trackedTrashItem = new Item();
        public float enemySpawnBonus = 1f;
        public bool godSlayer = false;
        public bool disableLeftShiftTrashCan = ItemSlot.Options.DisableLeftShiftTrashCan;
        public bool[] vanillaPlayerBuffsWeapon;
        public bool[] vanillaPlayerBuffsArmor;
        public Dictionary<int, int> buffs = new Dictionary<int, int>();
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
            if (LogMethods.debugging) ($"\\/OnEnterWorld({player.S()})").Log();
            InfusionManager.SetUpVanillaWeaponInfusionPowers();
            if (!OldWorldItemsReplaced)
            {
                OldItemManager.ReplaceAllOldItems();
                OldWorldItemsReplaced = true;
            }
            OldItemManager.ReplaceAllPlayerOldItems(player);

            /*foreach(Mod mod in ModLoader.Mods)
            {
                if (ModContent.TryFind<ModItem>("NameOfMod/ItemName", out ModItem modItem))
                {
                    int type = modItem.Type;
                    //use type here
                }
                foreach (Item item in mod)
            }*/
            /*if(Main.netMode == NetmodeID.MultiplayerClient)
            {
                ModPacket packet = ModContent.GetInstance<WEMod>().GetPacket();
                packet.Write(WEMod.PacketIDs.TeleportItemSetting);
                packet.Write(player.name);
                packet.Write(WEMod.clientConfig.teleportEssence);
                packet.Send();
            }*/
            if (LogMethods.debugging) ($"/\\OnEnterWorld({player.S()})").Log();
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
            int armorCount = Player.armor.Length / 2 + Player.GetModPlayer<ModAccessorySlotPlayer>().SlotCount;
            equipArmor = new Item[armorCount];
            equipArmorStatsUpdated = new bool[armorCount];
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
            tag["infusionConsumeItem"] = infusionConsumeItem;
            tag["highestTableTierUsed"] = highestTableTierUsed;
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
            infusionConsumeItem = tag.Get<Item>("infusionConsumeItem");
            if (infusionConsumeItem.IsAir)
                infusionConsumeItem = null;
            highestTableTierUsed = tag.Get<int>("highestTableTierUsed");
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
                //Trash Item
                if (!Player.trashItem.IsAir)
                {
                    if (Player.trashItem.TryGetEnchantedItem(out EnchantedItem tGlobal) && !tGlobal.trashItem)
                    {
                        if (trackedTrashItem.TryGetEnchantedItem(out EnchantedItem trackedTrashGlobal))
                            trackedTrashGlobal.trashItem = false;
                        tGlobal.trashItem = true;
                    }
                }
                else if (trackedTrashItem.TryGetEnchantedItem(out EnchantedItem trackedTrashGlobal)) {
                    trackedTrashGlobal.trashItem = false;
                }

                bool hoveringOverTrash = false;
                if (!item.IsAir)
                {
                    if(item.TryGetEnchantedItem(out EnchantedItem iGlobal) && iGlobal.trashItem)
                        hoveringOverTrash = true;
                }
                if (!hoveringOverTrash)
                {
                    Item tableItem = enchantingTableUI.itemSlotUI[0].Item;
                    if (item.type == PowerBooster.ID && !enchantingTableUI.itemSlotUI[0].Item.IsAir && !enchantingTableUI.itemSlotUI[0].Item.GetEnchantedItem().PowerBoosterInstalled)
                    {
                        if (moveItem)
                        {
                            enchantingTableUI.itemSlotUI[0].Item.GetEnchantedItem().PowerBoosterInstalled = true;
                            if (item.stack > 1)
                                item.stack--;
                            else
                                item = new Item();
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
                                        if (iGlobal.equippedInArmorSlot && !tableItem.IsAir)
                                            if(WEMod.IsAccessoryItem(item) && !WEMod.IsArmorItem(item) && (WEMod.IsAccessoryItem(tableItem) || WEMod.IsArmorItem(tableItem)) || item.headSlot > -1 && tableItem.headSlot == -1 || item.bodySlot > -1 && tableItem.bodySlot == -1 || item.legSlot > -1 && tableItem.legSlot == -1)
                                                doNotSwap = true;//Fix for Armor Modifiers & Reforging setting item.accessory to true to allow reforging armor
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
                            if (item.ModItem is Enchantment enchantment)
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
        public override void PostUpdate() {
            //if (Main.netMode < NetmodeID.Server)
            {
                string n;
				if (!Main.HoverItem.IsAir) {
                    if(Main.HoverItem.ModItem != null) {
                        n = Main.HoverItem.ModItem.Name;
                    }
				}

                int vanillaArmorLength = Player.armor.Length / 2;
                var loader = LoaderManager.Get<AccessorySlotLoader>();
                for (int j = 0; j < equipArmor.Length; j++) {
                    Item armor;
                    if (j < vanillaArmorLength)
                        armor = Player.armor[j];
                    else {
                        int num = j - vanillaArmorLength;
                        if (loader.ModdedIsAValidEquipmentSlotForIteration(num, Player))
                            armor = loader.Get(num).FunctionalItem;
                        else
                            armor = new Item();
                    }
                    if (!armor.vanity) {
                        equipArmorStatsUpdated[j] = !ItemChanged(armor, equipArmor[j]);
                    }
                }//Check if armor changed
                /*for (int k = 0; k < Player.GetModPlayer<ModAccessorySlotPlayer>().SlotCount; k++)
                {
                    if (loader.ModdedIsAValidEquipmentSlotForIteration(k, Player))
                    {
                        Item accessory = loader.Get(k).FunctionalItem;
                        if (!accessory.vanity)
                        {
                            modAccessoryStatsUpdated[k] = !ItemChanged(accessory, modAccessorys[k]);
                        };
                    }
                }*/
                for (int j = 0; j < equipArmor.Length; j++) {
                    Item armor;
                    if (j < vanillaArmorLength)
                        armor = Player.armor[j];
                    else {
                        int num = j - vanillaArmorLength;
                        if (loader.ModdedIsAValidEquipmentSlotForIteration(num, Player))
                            armor = loader.Get(num).FunctionalItem;
                        else
                            armor = new Item();
                    }
                    bool armorStatsUpdated = equipArmorStatsUpdated[j];
                    if (!armorStatsUpdated) {
                        if (!armor.vanity && !armor.IsAir) {
                            for (int i = 0; i < EnchantingTable.maxEnchantments; i++) {
                                if (!armor.GetEnchantedItem().enchantments[i].IsAir) {
                                    if (i > 1 && i < 4 || i > 0 && !WEMod.IsArmorItem(armor)) {
                                        armor.GetEnchantedItem().enchantments[i] = Player.GetItem(Main.myPlayer, armor.GetEnchantedItem().enchantments[i], GetItemSettings.LootAllSettings);
                                        if (!armor.GetEnchantedItem().enchantments[i].IsAir) {
                                            Player.QuickSpawnItem(Player.GetSource_Misc("PlayerDropItemCheck"), armor.GetEnchantedItem().enchantments[i]);
                                            armor.GetEnchantedItem().enchantments[i] = new Item();
                                            if (WEMod.IsArmorItem(armor)) {
                                                Main.NewText("Armor can only equip enchantments in the first 2 slots and the utility slot");
                                            }
                                            else {
                                                Main.NewText("Accessories can only equip an enchantment in the first slot");
                                            }
                                        }
                                    }//Move to Removewhileuntil below
                                }
                            }//Pop off excess
                        }
                        //if (Main.netMode < NetmodeID.Server)
                        {
                            UpdatePotionBuffs(ref armor, ref equipArmor[j]);
                            UpdatePlayerStats(ref armor, ref equipArmor[j]);
                            //if (Main.netMode == NetmodeID.MultiplayerClient) ModContent.GetInstance<WEMod>().SendPacket(WEMod.PacketIDs.TransferGlobalItemFields, armor, equipArmor[j], true, (byte)j);
                        }
                        if (!equipArmor[j].IsAir) {
                            Item temp = equipArmor[j];
                            temp.GetEnchantedItem().equippedInArmorSlot = false;
                        }
                        if (!armor.IsAir) {
                            armor.GetEnchantedItem().equippedInArmorSlot = true;
                        }
                        equipArmor[j] = armor;
                    }
                }


                if (Main.mouseItem.IsAir)
                {
                    Player.HeldItem.CheckWeapon(ref trackedWeapon, Player, 0);
                }
                else if (WEMod.IsEnchantable(Main.mouseItem))
                {
                    Main.mouseItem.CheckWeapon(ref trackedWeapon, Player, 1);
                }//Check too many enchantments on mouseItem
                //(Main.HoverItem.Name + " Main.HoverItem != null: " + (Main.HoverItem != null) + " && WEMod.IsWeaponItem(Main.HoverItem): " + WEMod.IsWeaponItem(Main.HoverItem) + " && !Main.HoverItem.G().trackedWeapon: " + (Main.HoverItem != null && WEMod.IsWeaponItem(Main.HoverItem) && !Main.HoverItem.G().trackedWeapon) + " && !Main.HoverItem.G().hoverItem: " + (Main.HoverItem != null && WEMod.IsWeaponItem(Main.HoverItem) && !Main.HoverItem.G().trackedWeapon && !Main.HoverItem.G().hoverItem)).LogT();
                if (Main.HoverItem != null && WEMod.IsWeaponItem(Main.HoverItem) && !Main.HoverItem.GetEnchantedItem().trackedWeapon && !Main.HoverItem.GetEnchantedItem().hoverItem)
                {
                    if(LogMethods.debugging) ($"\\/Start hoverItem check").Log();
                    Item newItem = null;
                    if (usingEnchantingTable && EnchantedItemStaticMethods.IsSameEnchantedItem(enchantingTableUI.itemSlotUI[0].Item, Main.HoverItem))
                        newItem = enchantingTableUI.itemSlotUI[0].Item;
                    if (newItem != null && EnchantedItemStaticMethods.IsSameEnchantedItem(Player.inventory[hoverItemIndex], Main.HoverItem))
                        newItem = Player.inventory[hoverItemIndex];
                    if (newItem != null && Player.chest != -1)
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
                        if (EnchantedItemStaticMethods.IsSameEnchantedItem(inventory[hoverItemIndex], Main.HoverItem))
                            newItem = inventory[hoverItemIndex];
                    }
                    if (newItem == null)
                    {
                        for (int i = 0; i < Player.inventory.Length; i++)
                        {
                            if (WEMod.IsWeaponItem(Player.inventory[i]))
                            {
                                if (EnchantedItemStaticMethods.IsSameEnchantedItem(Player.inventory[i], Main.HoverItem))
                                {
                                    hoverItemIndex = i;
                                    newItem = Player.inventory[i];
                                    break;
                                }
                            }
                        }
                    }
                    if (Player.chest != -1 && newItem == null)
                    {
                        Item[] inventory = null;
                        switch (Player.chest)
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
                        for (int i = 0; i < inventory.Length; i++)
                        {
                            Item chestItem = inventory[i];
                            if (WEMod.IsWeaponItem(chestItem))
                            {
                                if (EnchantedItemStaticMethods.IsSameEnchantedItem(chestItem, Main.HoverItem))
                                {
                                    hoverItemIndex = i;
                                    newItem = chestItem;
                                    hoverItemChest = Player.chest;
                                    break;
                                }
                            }
                        }
                    }
                    if(LogMethods.debugging) ($"newItem: " + newItem.S()).Log();
                    bool checkWeapon = ItemChanged(newItem, trackedHoverItem, true);
                    if(LogMethods.debugging) ($"checkWeapon: " + ItemChanged(newItem, trackedHoverItem, true)).Log();
                    if (checkWeapon)
                    {
                        if (newItem != null && WEMod.IsEnchantable(newItem))
                            newItem.GetEnchantedItem().hoverItem = true;
                        if (trackedHoverItem != null && WEMod.IsEnchantable(trackedHoverItem))
                        {
                            trackedHoverItem.GetEnchantedItem().hoverItem = false;
                        }
                        trackedHoverItem = newItem;
                        UpdateItemStats(ref newItem);
                    }//Check HeldItem
                    if(LogMethods.debugging) ($"/\\End hoverItem check Item: " + (newItem != null ? newItem.Name : "null ")).Log();
                }
                else
                {
                    if (trackedHoverItem.TryGetEnchantedItem(out EnchantedItem trackedHoverItemEI) && (Main.HoverItem.TryGetEnchantedItem(out EnchantedItem hoverItemEI) && (hoverItemEI != null && hoverItemEI.hoverItem == false) || (Main.HoverItem == null || Main.HoverItem.IsAir)))
                    {
                        if(LogMethods.debugging) ($"remove hoverItem: {trackedHoverItem.S()}").Log();
                        trackedHoverItemEI.hoverItem = false;
                        trackedHoverItem = null;
                    }
                    /*if (hoverItem != null && !hoverItem.IsAir && (!WEMod.IsWeaponItem(Main.HoverItem) || !Main.HoverItem.G().trackedWeapon && !Main.HoverItem.G().hoverItem))
                    {
                        ("remove hoverItem: " + hoverItem.Name).Log();
                        hoverItem.G().hoverItem = false;
                        hoverItem = null;
                    }*/
                }
                
            }
            foreach (int key in buffs.Keys)
            {
                Player.AddBuff(key, 1);
            }
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
                if(previous == null)
                {
                    return true;
                }
                if (previous.IsAir)
                {
                    return true;
                }
                else if (WEMod.IsEnchantable(current) && (weapon && !current.GetEnchantedItem().trackedWeapon || !weapon && !current.GetEnchantedItem().equippedInArmorSlot))
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
            if(LogMethods.debugging) ($"\\/UpdatePotionBuffs(" + newItem.S() + ", " + oldItem.S() + ")").Log();
            UpdatePotionBuff(ref newItem);
            UpdatePotionBuff(ref oldItem, true);
            if(LogMethods.debugging) ($"/\\UpdatePotionBuffs(" + newItem.S() + ", " + oldItem.S() + ")").Log();
        }
        private void UpdatePotionBuff(ref Item item, bool remove = false)
        {
            if(LogMethods.debugging) ($"\\/UpdatePotionBuff(" + item.S() + ", remove: " + remove + ")").Log();
            if (WEMod.IsEnchantable(item))
            {
                EnchantedItem iGlobal = item.GetEnchantedItem();
                WEPlayer wePlayer = Player.GetModPlayer<WEPlayer>();
                if(LogMethods.debugging) ($"potionBuffs.Count: {buffs.Count}, iGlobal.potionBuffs.Count: {iGlobal.buffs.Count}").Log();
                foreach (int key in iGlobal.buffs.Keys)
                {
                    if(LogMethods.debugging) ($"player: {buffs.S(key)}, item: {iGlobal.buffs.S(key)}").Log();
                    if (wePlayer.buffs.ContainsKey(key))
                        wePlayer.buffs[key] += iGlobal.buffs[key] * (remove ? -1 : 1);
                    else
                        wePlayer.buffs.Add(key, iGlobal.buffs[key]);
                    if (remove && wePlayer.buffs[key] < 1)
                        wePlayer.buffs.Remove(key);
                    if(LogMethods.debugging) ($"player: {buffs.S(key)}, item: {iGlobal.buffs.S(key)}").Log();
                }
            }
            if(LogMethods.debugging) ($"/\\UpdatePotionBuff(" + item.S() + ", remove: " + remove + ")").Log();
        }
        public static StatModifier CombineStatModifier(StatModifier baseStatModifier, StatModifier newStatModifier, bool remove)
        {
            if(LogMethods.debugging) ($"\\/CombineStatModifier(baseStatModifier: " + baseStatModifier.S() + ", newStatModifier: " + newStatModifier.S() + ", remove: " + remove + ") StatModifier").Log();
            StatModifier finalModifier;
            if (remove)
            {
                finalModifier = new StatModifier(baseStatModifier.Additive / newStatModifier.Additive, baseStatModifier.Multiplicative / newStatModifier.Multiplicative, baseStatModifier.Flat - newStatModifier.Flat, baseStatModifier.Base - newStatModifier.Base);
            }
            else
            {
                finalModifier = new StatModifier(baseStatModifier.Additive * newStatModifier.Additive, baseStatModifier.Multiplicative * newStatModifier.Multiplicative, baseStatModifier.Flat + newStatModifier.Flat, baseStatModifier.Base + newStatModifier.Base);
            }
            if(LogMethods.debugging) ($"/\\CombineStatModifier(baseStatModifier: " + baseStatModifier.S() + ", newStatModifier: " + newStatModifier.S() + ", remove: " + remove + ") return " + finalModifier.S()).Log();
            return finalModifier;
        }
        public static StatModifier InverseCombineWith(StatModifier baseStatModifier, StatModifier newStatModifier, bool remove)
        {
            if(LogMethods.debugging) ($"\\/InverseCombineWith(baseStatModifier: " + baseStatModifier.S() + ", newStatModifier: " + newStatModifier.S() + ", remove: " + remove + ") StatModifier").Log();
            StatModifier newInvertedStatModifier;
            if (remove)
                newInvertedStatModifier = new StatModifier(2f - newStatModifier.Additive, 1f/newStatModifier.Multiplicative, -newStatModifier.Flat, -newStatModifier.Base);
            else
                newInvertedStatModifier = newStatModifier;
            if(LogMethods.debugging) ($"newInvertedStatModifier: " + newInvertedStatModifier.S()).Log();
            StatModifier finalModifier = baseStatModifier.CombineWith(newInvertedStatModifier);
            if(LogMethods.debugging) ($"/\\InverseCombineWith(baseStatModifier: " + baseStatModifier.S() + ", newStatModifier: " + newStatModifier.S() + ", remove: " + remove + ") return " + finalModifier.S()).Log();
            return finalModifier;
        }
        public static void TryRemoveStat(ref Dictionary<string, StatModifier> dictionary, string key)
        {
            if(LogMethods.debugging) ($"\\/TryRemoveStat( dictionary, key: " + key + ") dictionary: " + dictionary.S(key)).Log();
            if (dictionary.ContainsKey(key))
            {
                if((float)Math.Abs(Math.Abs(Math.Round(dictionary[key].Additive, 4)) - 1f) < 1E-4 && (float)Math.Abs(Math.Abs(Math.Round(dictionary[key].Multiplicative, 4)) - 1f) < 1E-4 && Math.Abs(Math.Round(dictionary[key].Flat, 4)) < 1E-4 && Math.Abs(Math.Round(dictionary[key].Base, 4)) < 1E-4)
                {
                    dictionary.Remove(key);
                    //(key + " removed").Log();
                }
            }
            if(LogMethods.debugging) ($"/\\TryRemoveStat( dictionary, key: " + key + ") dictionary: " + dictionary.S(key)).Log();
        }
        public void UpdatePlayerStats(ref Item newItem, ref Item oldItem)
        {
            if(LogMethods.debugging) ($"\\/UpdatePlayerStats(" + newItem.S() + ", " + oldItem.S() + ")").Log();
            if (WEMod.IsEnchantable(newItem))
                UpdatePlayerDictionaries(newItem);
            if (WEMod.IsEnchantable(oldItem))
                UpdatePlayerDictionaries(oldItem, true);
            if (WEMod.IsEnchantable(newItem))
                UpdateItemStats(ref newItem);
            if (!WEMod.IsWeaponItem(newItem))
            {
                Item weapon = null;
                if (WEMod.IsEnchantable(Player.HeldItem) && Player.HeldItem.GetEnchantedItem().trackedWeapon)
                    weapon = Player.HeldItem;
                else if (WEMod.IsEnchantable(Main.mouseItem) && Main.mouseItem.GetEnchantedItem().trackedWeapon)
                    weapon = Main.mouseItem;
                if (weapon != null)
                    UpdateItemStats(ref weapon);
            }
            UpdatePlayerStat();
            if(LogMethods.debugging) ($"/\\UpdatePlayerStats(" + newItem.S() + ", " + oldItem.S() + ")").Log();
        }
        private void UpdatePlayerStat()
        {
            if(LogMethods.debugging) ($"\\/UpdatePlayerStat()").Log();
            /*foreach (string key in item.G().appliedStatModifiers.Keys)
                {
                    if (!combinedStatModifiers.ContainsKey(key))
                        combinedStatModifiers.Add(key, StatModifier.Default);
                } //May need something like this here too if stats arent being removed when removing armor*/
            foreach (string key in statModifiers.Keys)
            {
                string statName = key.RemoveInvert();
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
                            //(statName.ToString() + ": " + field.GetValue(Player)).LogT();
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
                            //(statName.ToString() + ": " + field.GetValue(Player)).LogT();
                        }//field
                        else if (property != null)
                        {
                            //(statName.ToString() + ": " + property.GetValue(Player)).LogT();
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
                            //(statName.ToString() + ": " + property.GetValue(Player)).LogT();
                        }//property
                    }
                }
            }
            if(LogMethods.debugging) ($"/\\UpdatePlayerStat()").Log();
        }
        private void UpdatePlayerDictionaries(Item item, bool remove = false)
        {
            if(LogMethods.debugging) ($"\\/UpdatePlayerDictionaries(" + item.S() + ", remove: " + remove + ") statModifiers.Count: " + item.GetEnchantedItem().statModifiers.Count).Log();
            foreach (string key in item.GetEnchantedItem().statModifiers.Keys)
            {
                string statName = key.RemoveInvert();
                if (!WEMod.IsWeaponItem(item) || item.GetType().GetField(statName) == null && item.GetType().GetProperty(statName) == null)
                {
                    if(!statModifiers.ContainsKey(key))
                        statModifiers.Add(key, StatModifier.Default);
                    //statModifiers[key].S().Log();
                    statModifiers[key] = InverseCombineWith(statModifiers[key], item.GetEnchantedItem().statModifiers[key], remove);
                    //statModifiers[key].S().Log();
                    TryRemoveStat(ref statModifiers, key);
                }
            }
            if (!WEMod.IsWeaponItem(item))
            {
                foreach (string key in item.GetEnchantedItem().eStats.Keys)
                {
                    if (!eStats.ContainsKey(key))
                        eStats.Add(key, StatModifier.Default);
                    eStats[key] = InverseCombineWith(eStats[key], item.GetEnchantedItem().eStats[key], remove);
                    TryRemoveStat(ref eStats, key);
                }
            }
            if(LogMethods.debugging) ($"/\\UpdatePlayerDictionaries(" + item.S() + ", remove: " + remove + ")").Log();
        }
        public void UpdateItemStats(ref Item item)
        {
            if (WEMod.IsEnchantable(item))
            {
                if(LogMethods.debugging) ($"\\/UpdateItemStats(" + item.S() + ")").Log();

                int trackedPrefix = item.GetEnchantedItem().prefix;
                if (trackedPrefix != item.prefix)
                {
                    item.GetEnchantedItem().appliedStatModifiers.Clear();
                    item.GetEnchantedItem().appliedEStats.Clear();
                    item.GetEnchantedItem().prefix = item.prefix;
                    int damageType = item.GetEnchantedItem().damageType;
                    if (damageType > -1)
                        item.UpdateDamageType(damageType);
                }
                int infusedArmorSlot = item.GetEnchantedItem().infusedArmorSlot;
                int armorSlot = item.GetInfusionArmorSlot(false, true);
                if (infusedArmorSlot != -1 && armorSlot != infusedArmorSlot)
                    item.UpdateArmorSlot(item.GetEnchantedItem().infusedArmorSlot);
                Dictionary<string, StatModifier> combinedStatModifiers = new Dictionary<string, StatModifier>();
                foreach (string itemKey in item.GetEnchantedItem().statModifiers.Keys)
                {
                    string riItemKey = itemKey.RemoveInvert().RemovePrevent();
                    if (item.GetType().GetField(riItemKey) != null || item.GetType().GetProperty(riItemKey) != null)
                    {
                        StatModifier riStatModifier = itemKey.ContainsInvert() ? CombineStatModifier(StatModifier.Default, item.GetEnchantedItem().statModifiers[itemKey], true) : item.GetEnchantedItem().statModifiers[itemKey];
                        if(combinedStatModifiers.ContainsKey(riItemKey))
                            combinedStatModifiers[riItemKey] = combinedStatModifiers[riItemKey].CombineWith(riStatModifier); 
                        else
                            combinedStatModifiers.Add(riItemKey, riStatModifier);
                        if (LogMethods.debugging) ($"combinedStatModifiers.Add(itemKey: " + itemKey + ", " + item.GetEnchantedItem().statModifiers.S(itemKey) + ")").Log();
                    }
                }//Populate itemStatModifiers
                if (WEMod.IsWeaponItem(item))
                {
                    foreach (string playerKey in statModifiers.Keys)
                    {
                        string riPlayerKey = playerKey.RemoveInvert().RemovePrevent();
                        if (item.GetType().GetField(riPlayerKey) != null || item.GetType().GetProperty(riPlayerKey) != null)
                        {
                            StatModifier riStatModifier = playerKey.ContainsInvert() ? CombineStatModifier(StatModifier.Default, statModifiers[playerKey], true) : statModifiers[playerKey];
                            if (combinedStatModifiers.ContainsKey(riPlayerKey))
                                combinedStatModifiers[riPlayerKey] = combinedStatModifiers[riPlayerKey].CombineWith(riStatModifier);
                            else
                                combinedStatModifiers.Add(riPlayerKey, riStatModifier);
                            if(LogMethods.debugging) ($"combinedStatModifiers.Add(playerKey: " + playerKey + ", " + item.GetEnchantedItem().statModifiers.S(playerKey) + ")").Log();
                        }
                    }
                }//Populate playerStatModifiers if item is a weapon
                foreach (string key in item.GetEnchantedItem().appliedStatModifiers.Keys)
                {
                    if (!combinedStatModifiers.ContainsKey(key))
                        combinedStatModifiers.Add(key, StatModifier.Default);
                }
                foreach (string key in combinedStatModifiers.Keys)
                {
                    bool statsNeedUpdate = true;
                    if (item.GetEnchantedItem().appliedStatModifiers.ContainsKey(key))
                        statsNeedUpdate = combinedStatModifiers[key] != item.GetEnchantedItem().appliedStatModifiers[key];
                    if(LogMethods.debugging) ($"statsNeedUpdate: " + statsNeedUpdate + " combinedStatModifiers[" + key + "]: " + combinedStatModifiers.S(key) + " != item.G().appliedStatModifiers[" + key + "]: " + item.GetEnchantedItem().appliedStatModifiers.S(key)).Log();
                    if (statsNeedUpdate)
                    {
                        FieldInfo field = item.GetType().GetField(key);
                        PropertyInfo property = item.GetType().GetProperty(key);
                        if (item.GetType().GetField(key) != null || item.GetType().GetProperty(key) != null)
                        {
                            if (!item.GetEnchantedItem().appliedStatModifiers.ContainsKey(key))
                                item.GetEnchantedItem().appliedStatModifiers.Add(key, StatModifier.Default);
                            StatModifier lastAppliedStatModifier = item.GetEnchantedItem().appliedStatModifiers[key];
                            item.GetEnchantedItem().appliedStatModifiers[key] = combinedStatModifiers[key];
                            StatModifier staticStat = CombineStatModifier(combinedStatModifiers[key], lastAppliedStatModifier, true);
                            if (field != null)
                            {
                                //(statName.ToString() + ": " + field.GetValue(item)).Log();
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
                                    staticStat.RoundCheck(ref finalValue, (int)field.GetValue(item), item.GetEnchantedItem().appliedStatModifiers[key], (int)field.GetValue(ContentSamples.ItemsByType[item.type]));
                                    field.SetValue(item, (int)Math.Round(finalValue + 5E-6));
                                }//int (field)
                                if (fieldType == typeof(bool))
                                {
                                    bool baseValue = (bool)field.GetValue(ContentSamples.ItemsByType[item.type]);
                                    bool finalValue = combinedStatModifiers[key].Additive > 1.001f;
                                    bool containtPrevent = item.GetEnchantedItem().statModifiers.ContainsKey("P_" + key) && item.GetEnchantedItem().statModifiers["P_" + key].Additive > 1.001f || statModifiers.ContainsKey("P_" + key) && statModifiers["P_" + key].Additive > 1.001f;
                                    bool setValue = !containtPrevent && (baseValue || finalValue);
                                    field.SetValue(item, setValue);
                                }//bool (field)
                                //(statName.ToString() + ": " + field.GetValue(item)).Log();
                            }//field
                            else if (property != null)
                            {
                                //(statName.ToString() + property.GetValue(item)).Log();
                                Type propertyType = property.PropertyType;
                                if (propertyType == typeof(float))
                                {
                                    float finalValue = staticStat.ApplyTo((float)property.GetValue(item));
                                    property.SetValue(item, finalValue);
                                }//float (property)
                                if (propertyType == typeof(int))
                                {
                                    //int valueInt = (int)property.GetValue(item);
                                    float finalValue = staticStat.ApplyTo((float)(int)property.GetValue(item));
                                    //Item contentSampleItem = ContentSamples.ItemsByType[item.type].Clone();
                                    staticStat.RoundCheck(ref finalValue, (int)property.GetValue(item), item.GetEnchantedItem().appliedStatModifiers[key], (int)property.GetValue(ContentSamples.ItemsByType[item.type]));
                                    property.SetValue(item, (int)Math.Round(finalValue + 5E-6));
                                }//int (property)
                                if (propertyType == typeof(bool))
                                {
                                    bool baseValue = (bool)property.GetValue(ContentSamples.ItemsByType[item.type]);
                                    bool finalValue = combinedStatModifiers[key].Additive != 1f;
                                    property.SetValue(item, !combinedStatModifiers.ContainsKey("P_" + key) && (baseValue || finalValue));
                                }//bool (property)
                                //(statName.ToString() + property.GetValue(item)).Log();
                            }//property
                            //TryRemoveStat(ref item.G().appliedStatModifiers, key);
                        }
                    }
                }
                Dictionary<string, StatModifier> combinedEStats = new Dictionary<string, StatModifier>();
                foreach (string itemKey in item.GetEnchantedItem().eStats.Keys)
                {
                    string riItemKey = itemKey.RemoveInvert();
                    StatModifier riEStat = itemKey.ContainsInvert() ? CombineStatModifier(StatModifier.Default, item.GetEnchantedItem().eStats[itemKey], true) : item.GetEnchantedItem().eStats[itemKey];
                    if(combinedEStats.ContainsKey(riItemKey))
                        combinedEStats[riItemKey] = combinedEStats[riItemKey].CombineWith(riEStat);
                    else
                        combinedEStats.Add(riItemKey, riEStat);
                    if (LogMethods.debugging) ($"combinedEStats.Add(itemKey: " + itemKey + ", " + item.GetEnchantedItem().eStats.S(itemKey) + ")").Log();
                }//Populate itemeStats
                if (WEMod.IsWeaponItem(item))
                {
                    foreach (string playerKey in eStats.Keys)
                    {
                        string riPlayerKey = playerKey.RemoveInvert();
                        StatModifier riEStat = playerKey.ContainsInvert() ? CombineStatModifier(StatModifier.Default, eStats[playerKey], true) : eStats[playerKey];
                        if (combinedEStats.ContainsKey(riPlayerKey))
                            combinedEStats[riPlayerKey] = combinedEStats[riPlayerKey].CombineWith(riEStat);
                        else
                            combinedEStats.Add(riPlayerKey, riEStat);
                        if (LogMethods.debugging) ($"combinedEStats.Add(riPlayerKey: " + riPlayerKey + ", " + item.GetEnchantedItem().eStats.S(playerKey) + ")").Log();
                    }
                }//Populate playereStats if item is a weapon
                foreach (string key in item.GetEnchantedItem().appliedEStats.Keys)
                {
                    if (!combinedEStats.ContainsKey(key))
                        combinedEStats.Add(key, StatModifier.Default);
                }
                foreach (string key in combinedEStats.Keys)
                {
                    bool statsNeedUpdate = true;
                    if (item.GetEnchantedItem().appliedEStats.ContainsKey(key))
                        statsNeedUpdate = combinedEStats[key] != item.GetEnchantedItem().appliedEStats[key];
                    if (statsNeedUpdate)
                    {
                        if (!item.GetEnchantedItem().appliedEStats.ContainsKey(key))
                            item.GetEnchantedItem().appliedEStats.Add(key, combinedEStats[key]);
                        else
                            item.GetEnchantedItem().appliedEStats[key] = combinedEStats[key];
                        //TryRemoveStat(ref item.G().appliedEStats, key);
                    }
                }

                foreach (string key in item.GetEnchantedItem().statModifiers.Keys)
                    TryRemoveStat(ref item.GetEnchantedItem().statModifiers, key);

                foreach (string key in item.GetEnchantedItem().appliedStatModifiers.Keys)
                    TryRemoveStat(ref item.GetEnchantedItem().appliedStatModifiers, key);
                
                foreach(string key in item.GetEnchantedItem().eStats.Keys)
                    TryRemoveStat(ref item.GetEnchantedItem().eStats, key);

                foreach(string key in item.GetEnchantedItem().appliedEStats.Keys)
                    TryRemoveStat(ref item.GetEnchantedItem().appliedEStats, key);

                if (LogMethods.debugging) ($"/\\UpdateItemStats(" + item.S() + ")").Log();
            }
        }
        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if(Player.difficulty == 1 || Player.difficulty == 2)
            {
                for (int i = 0; i < Player.inventory.Length; i++)
                {
                    if (WEMod.IsEnchantable(Player.inventory[i]))
                    {
                        Player.inventory[i].GetEnchantedItem().appliedStatModifiers.Clear();
                    }
                }
                for (int i = 0; i < Player.armor.Length; i++)
                {
                    if (WEMod.IsEnchantable(Player.armor[i]))
                    {
                        Player.armor[i].GetEnchantedItem().appliedStatModifiers.Clear();
                    }
                }
            }
        }
        public override void ResetEffects()
        {
            int temp1 = Player.maxMinions;
            bool updatePlayerStat = false;
            foreach (string key in statModifiers.Keys)
            {
                string name = key.RemoveInvert().RemovePrevent();
                if (Player.GetType().GetField(name) != null)
                {
                    appliedStatModifiers.Remove(key);
                    updatePlayerStat = true;
                }
            }
            if (updatePlayerStat)
                UpdatePlayerStat();
            bool autoReuseGlove = Player.autoReuseGlove;
        }
        public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath)
        {
            List<Item> items = new List<Item>();
            if (WEMod.serverConfig.DCUStart)
            {
                Item item = new Item(ItemID.DrillContainmentUnit);
                items.Add(item);
            }
            return items;
        }
	}
}
