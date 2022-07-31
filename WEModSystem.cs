using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Items;
using WeaponEnchantments.Items.Enchantments;
using WeaponEnchantments.Items.Enchantments.Unique;
using WeaponEnchantments.Items.Enchantments.Utility;
using WeaponEnchantments.UI;
using static WeaponEnchantments.UI.WeaponEnchantmentUI;

namespace WeaponEnchantments
{
    public class WEModSystem : ModSystem
    {
        internal static UserInterface weModSystemUI;
        internal static UserInterface mouseoverUIInterface;
        internal static UserInterface promptInterface;
        private GameTime _lastUpdateUiGameTime;
        private static bool needsToQuickStack = false;
        private static bool tryNextTick = false;
        private static bool firstDraw = true;
        private static bool secondDraw = true;
        private static bool transfered = false;
        public static bool playerInventoryUpdated = false;
        public static bool enchantingTableInventoryUpdated = false;
        public static int previousChest = -1;
        public static int[] levelXps = new int[EnchantedItem.MAX_LEVEL];
        private static int[] essenceStack = new int[EnchantedItem.MAX_LEVEL];
        private static bool favorited;
        public static int stolenItemToBeCleared = -1;
        public static bool playerSwapperModEnabled = false;
        public static List<string> updatedPlayerNames;



        #region Properties (static)

        public static bool AltDown => Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftAlt) || Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightAlt);

        #endregion

        public override void OnModLoad()
        {
            if (!Main.dedServ)
            {
                weModSystemUI = new UserInterface();
                promptInterface = new UserInterface();
                mouseoverUIInterface = new UserInterface();
            }
            double previous = 0;
            double current;
            int l;
            for (l = 0; l < EnchantedItem.MAX_LEVEL; l++)
            {
                current = previous * 1.23356622200537 + (l + 1) * 1000;
                previous = current;
                levelXps[l] = (int)current;
            }
            playerSwapperModEnabled = ModLoader.HasMod("PlayerSwapper");
            if (playerSwapperModEnabled)
                updatedPlayerNames = new List<string>();
        }//PR
        public override void Unload()
        {
            if (!Main.dedServ)
            {
                weModSystemUI = null;
                mouseoverUIInterface = null;
                promptInterface = null;
            }
        }//PR
        private static void ApplyEnchantment(int i)
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            Item item = wePlayer.enchantingTableUI.itemSlotUI[0].Item;
            item.ApplyEnchantment(i);
        }
        public static void RemoveEnchantment(int i)
        {
            if(LogMethods.debugging) ($"\\/RemoveEnchantment(i: " + i + ")").Log();
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            Item item = wePlayer.enchantingTableUI.itemSlotUI[0].Item;
            if (!item.IsAir)
            {
                EnchantedItem iGlobal = item.GetEnchantedItem();
                Enchantment enchantment = (Enchantment)(iGlobal.enchantments[i].ModItem);
                item.UpdateEnchantment(ref enchantment, i, true);
                wePlayer.UpdateItemStats(ref item);
            }
            if(LogMethods.debugging) ($"/\\RemoveEnchantment(i: " + i + ")").Log();
        }
        public override void PreUpdateItems()
        {
            /*
            if (!Main.HoverItem.IsAir)
            {
                if(Main.mouseRight && Main.mouseRightRelease)
                {
                    if(Main.HoverItem.G().experience > 0 || Main.HoverItem.G().powerBoosterInstalled)
                    {
                        if (Main.HoverItem.stack > 1)
                        {
                            Main.mouseItem = Main.HoverItem.Clone();
                            Main.HoverItem = new Item();
                        }
                    }
                }
            }
            */
            /*if (Main.mouseLeft)
                if (Main.mouseLeft && !Main.mouseItem.IsAir && Main.mouseItem.maxStack > 1 && WEMod.IsEnchantable(Main.mouseItem) && Main.mouseItem.type == Main.HoverItem.type)
                {
                    Item hoverItem = Main.HoverItem;
                    int maxStack = Main.mouseItem.maxStack;
                    if (Main.mouseItem.stack + hoverItem.stack <= maxStack)
                    {
                        List<Item> list = new List<Item>();
                        list.Add(hoverItem);
						Main.mouseItem.CombineEnchantedItems(ref list);
                    }
                }*/
        }
        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            if (wePlayer.usingEnchantingTable)
            {
                if (ItemSlot.Options.DisableLeftShiftTrashCan)
                {
                    wePlayer.disableLeftShiftTrashCan = ItemSlot.Options.DisableLeftShiftTrashCan;
                    ItemSlot.Options.DisableLeftShiftTrashCan = false;
                }
                bool removedItem = false;
                bool addedItem = false;
                bool swappedItem = false;
                if (wePlayer.enchantingTableUI.itemSlotUI[0].Item.IsAir)
                {
                    if (wePlayer.itemInEnchantingTable)//If item WAS in the itemSlot but it is empty now,
                    {
                        removedItem = true;
                    }//Transfer items to global item and break the link between the global item and enchanting table itemSlots/enchantmentSlots
                    wePlayer.itemInEnchantingTable = false;//The itemSlot's PREVIOUS state is now empty(false)
                }//Check if the itemSlot is empty because the item was just taken out and transfer the mods to the global item if so
                else if (!wePlayer.itemInEnchantingTable)//If itemSlot WAS empty but now has an item in it
                {
                    addedItem = true;
                    wePlayer.itemInEnchantingTable = true;//Set PREVIOUS state of itemSlot to having an item in it
                }//Check if itemSlot has item that was just placed there, copy the enchantments to the slots and link the slots to the global item
                else if (wePlayer.itemBeingEnchanted != wePlayer.enchantingTableUI.itemSlotUI[0].Item)
                {
                    swappedItem = true;
                }
                if (removedItem || swappedItem)
                {
                    for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
                    {
                        if (wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item != null)//For each enchantment in the enchantmentSlots,
                        //if(wePlayer.itemBeingEnchanted != null && !wePlayer.itemBeingEnchanted.IsAir)
                        {
                            wePlayer.itemBeingEnchanted.GetEnchantedItem().enchantments[i] = wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item.Clone();//copy enchantments to the global item
                        }
                        wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item = new Item();//Delete enchantments still in enchantmentSlots(There were transfered to the global item)
                        wePlayer.enchantmentInEnchantingTable[i] = false;//The enchantmentSlot's PREVIOUS state is now empty(false)
                    }
                    if (wePlayer.infusionConsumeItem != null)
                    {
                        if(!wePlayer.infusionConsumeItem.IsSameEnchantedItem(wePlayer.itemBeingEnchanted))
                            wePlayer.itemBeingEnchanted.TryInfuseItem(wePlayer.previousInfusedItemName, true);
                        wePlayer.enchantingTableUI.infusionButonText.SetText("Cancel");
                    }
                    wePlayer.itemBeingEnchanted.GetEnchantedItem().inEnchantingTable = false;
                    wePlayer.itemBeingEnchanted.favorited = favorited;
                    wePlayer.itemBeingEnchanted = wePlayer.enchantingTableUI.itemSlotUI[0].Item;//Stop tracking the item that just left the itemSlot
                    //TryToggleAutoPauseOff();
                }
                if (addedItem || swappedItem)
                {
                    wePlayer.itemBeingEnchanted = wePlayer.enchantingTableUI.itemSlotUI[0].Item;// Link the item in the table to the player so it can be updated after being taken out.
                    favorited = wePlayer.itemBeingEnchanted.favorited;
                    wePlayer.itemBeingEnchanted.favorited = false;
                    wePlayer.itemBeingEnchanted.GetEnchantedItem().inEnchantingTable = true;
                    wePlayer.itemBeingEnchanted.value -= wePlayer.itemBeingEnchanted.GetEnchantedItem().lastValueBonus;
                    wePlayer.itemBeingEnchanted.GetEnchantedItem().lastValueBonus = 0;
                    wePlayer.previousInfusedItemName = wePlayer.itemBeingEnchanted.GetEnchantedItem().infusedItemName;
                    if (wePlayer.infusionConsumeItem != null && (WEMod.IsWeaponItem(wePlayer.itemBeingEnchanted) || WEMod.IsArmorItem(wePlayer.itemBeingEnchanted)))
                    {
                        wePlayer.itemBeingEnchanted.TryInfuseItem(wePlayer.infusionConsumeItem);
                        wePlayer.enchantingTableUI.infusionButonText.SetText("Finalize");
                    }
                    for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
                    {
                        if (wePlayer.enchantingTableUI.itemSlotUI[0].Item.GetEnchantedItem().enchantments[i] != null)//For each enchantment in the global item,
                        {
                            wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item = wePlayer.enchantingTableUI.itemSlotUI[0].Item.GetEnchantedItem().enchantments[i].Clone();//copy enchantments to the enchantmentSlots
                            wePlayer.enchantmentInEnchantingTable[i] = wePlayer.EnchantmentsModItem(i) != null;//Set PREVIOUS state of enchantmentSlot to has an item in it(true)
                            wePlayer.enchantingTableUI.itemSlotUI[0].Item.GetEnchantedItem().enchantments[i] = wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item;//Force link to enchantmentSlot just in case
                        }
                        wePlayer.enchantingTableUI.itemSlotUI[0].Item.GetEnchantedItem().enchantments[i] = wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item;//Link global item to the enchantmentSlots
                    }
                    //TryToggleAutoPauseOff();
                }
                for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
                {
                    Item tableEnchantment = wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item;
                    Item itemEnchantment = new Item();
                    if (wePlayer.enchantingTableUI.itemSlotUI[0].Item.TryGetGlobalItem(out EnchantedItem ieGlobal))
                    {
                        itemEnchantment = ieGlobal.enchantments[i];
                    }
                    if (wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item.IsAir)
                    {
                        if (wePlayer.enchantmentInEnchantingTable[i])//if enchantmentSlot HAD an enchantment in it but it was just taken out,
                        {
                            //Force global item to re-link to the enchantmentSlot instead of following the enchantment just taken out
                            RemoveEnchantment(i);
                            //((Enchantment)itemEnchantment.ModItem).statsSet = false;
                            wePlayer.enchantingTableUI.itemSlotUI[0].Item.GetEnchantedItem().enchantments[i] = wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item;
                        }
                        wePlayer.enchantmentInEnchantingTable[i] = false;//Set PREVIOUS state of enchantmentSlot to empty(false)
                    }
                    else if (!itemEnchantment.IsAir && itemEnchantment != tableEnchantment)
                    {
                        RemoveEnchantment(i);
                        wePlayer.enchantingTableUI.itemSlotUI[0].Item.GetEnchantedItem().enchantments[i] = wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item;
                        ApplyEnchantment(i);
                    }//If player swapped enchantments (without removing the previous one in the enchantmentSlot) Force global item to re-link to the enchantmentSlot instead of following the enchantment just taken out
                    else if (!wePlayer.enchantmentInEnchantingTable[i])
                    {
                        wePlayer.enchantmentInEnchantingTable[i] = true;//Set PREVIOUS state of enchantmentSlot to has an item in it(true)
                        wePlayer.enchantingTableUI.itemSlotUI[0].Item.GetEnchantedItem().enchantments[i] = wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item;//Force link to enchantmentSlot just in case
                        //if (!wePlayer.enchantingTableUI.itemSlotUI[0].Item.G().statsSet[i])
                        //if(!wePlayer.I().E(i).statsSet)
                        ApplyEnchantment(i);
                    }//If it WAS empty but isn't now, re-link global item to enchantmentSlot just in case
                }//Check if enchantments are added/removed from enchantmentSlots and re-link global item to enchantmentSlot
                if (!wePlayer.Player.IsInInteractionRangeToMultiTileHitbox(wePlayer.Player.chestX, wePlayer.Player.chestY) || wePlayer.Player.chest != -1 || !Main.playerInventory)
                {
                    CloseWeaponEnchantmentUI();
                }//If player is too far away, close the enchantment table
                /*for (int i = 0; i < EnchantingTable.maxEssenceItems; i++)
                {
                    bool findRecipes = false;
                    if (essenceStack[i] != wePlayer.enchantingTableUI.essenceSlotUI[i].Item.stack)
                    {
                        findRecipes = true;
                    }
                    essenceStack[i] = wePlayer.enchantingTableUI.essenceSlotUI[i].Item.stack;
                    //if (findRecipes)
                    //Recipe.FindRecipes();
                }*/
            }//If enchanting table is open, check item(s) and enchantments in it every tick
            wePlayer.StoreLastFocusRecipe();
            if (Main.playerInventory)
            {
                for (int i = 0; i < 50; i++)
                {
                    if (wePlayer.Player.inventory?[i] != null)//try get rid of
                    {
                        if (!wePlayer.Player.inventory[i].IsAir)//try get rid of
                        {
                            wePlayer.inventoryItemRecord[i] = wePlayer.Player.inventory[i].Clone();
                            if(wePlayer.Player.inventory[i].TryGetGlobalItem(out EnchantedItem iGlobal))
                            {
                                if (iGlobal.needsUpdateOldItems)
                                    OldItemManager.ReplaceOldItem(ref wePlayer.Player.inventory[i], wePlayer.Player, i);
                            }
                        }
                        else
                        {
                            wePlayer.inventoryItemRecord[i] = new Item();//try get rid of
                        }
                    }
                    else//try get rid of
                    {
                        wePlayer.inventoryItemRecord[i] = new Item();//try get rid of
                    }
                }
                //if (wePlayer.Player.chest != previousChest && wePlayer.Player.chest != -1)
                if (wePlayer.Player.chest != -1)
                {
                    Item[] inventory;
                    switch (wePlayer.Player.chest)
                    {
                        case -2:
                            inventory = wePlayer.Player.bank.item;
                            break;
                        case -3:
                            inventory = wePlayer.Player.bank2.item;
                            break;
                        case -4:
                            inventory = wePlayer.Player.bank3.item;
                            break;
                        case -5:
                            inventory = wePlayer.Player.bank4.item;
                            break;
                        default:
                            inventory = Main.chest[wePlayer.Player.chest].item;
                            break;
                    }
                    for (int i = 0; i < 40; i++)
                    {
                        if (inventory?[i] != null)//try get rid of
                        {
                            if (!inventory[i].IsAir)//try get rid of
                            {
                                wePlayer.inventoryItemRecord[i + 50] = inventory[i].Clone();
                            }
                            else
                            {
                                wePlayer.inventoryItemRecord[i + 50] = new Item();//try get rid of
                            }
                        }
                        else
                        {
                            wePlayer.inventoryItemRecord[i + 50] = new Item();//try get rid of
                        }
                    }
                }
                if (wePlayer.usingEnchantingTable)
                {
                    if (wePlayer.enchantingTableUI?.itemSlotUI?[0]?.Item != null)//Try get rid of this
                    {
                        if (!wePlayer.enchantingTableUI.itemSlotUI[0].Item.IsAir)//Try get rid of this
                        {
                            wePlayer.inventoryItemRecord[90] = wePlayer.enchantingTableUI.itemSlotUI[0].Item.Clone();
                        }
                        else
                        {
                            wePlayer.inventoryItemRecord[90] = new Item();//Try get rid of this
                        }
                    }
                    else
                    {
                        wePlayer.inventoryItemRecord[90] = new Item();//Try get rid of this
                    }
                    for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
                    {
                        if (wePlayer.enchantingTableUI?.enchantmentSlotUI?[i]?.Item != null)//Try get rid of this
                        {
                            if (!wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item.IsAir)//Try get rid of this
                            {
                                wePlayer.inventoryItemRecord[92 + i] = wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item.Clone();
                            }
                            else
                            {
                                wePlayer.inventoryItemRecord[92 + i] = new Item();//Try get rid of this
                            }
                        }
                        else
                        {
                            wePlayer.inventoryItemRecord[92 + i] = new Item();//Try get rid of this
                        }
                    }
                    for (int i = 0; i < EnchantingTable.maxEssenceItems; i++)
                    {
                        if (wePlayer.enchantingTableUI?.essenceSlotUI?[i]?.Item != null)//Try get rid of this
                        {
                            if (!wePlayer.enchantingTableUI.essenceSlotUI[i].Item.IsAir)//Try get rid of this
                            {
                                wePlayer.inventoryItemRecord[97 + i] = wePlayer.enchantingTableUI.essenceSlotUI[i].Item.Clone();
                            }
                            else
                            {
                                wePlayer.inventoryItemRecord[97 + i] = new Item();//Try get rid of this
                            }
                        }
                        else
                        {
                            wePlayer.inventoryItemRecord[97 + i] = new Item();//Try get rid of this
                        }
                    }
                }
                else
                {
                    wePlayer.inventoryItemRecord[90] = new Item();
                    for (int i = 0; i < EnchantingTable.maxEnchantments + EnchantingTable.maxEssenceItems; i++)
                    {
                        wePlayer.inventoryItemRecord[92 + i] = new Item();
                    }
                }
                wePlayer.inventoryItemRecord[91] = Main.mouseItem.Clone();
            }
            
            if (wePlayer.usingEnchantingTable)
            {
                if (ItemSlot.ShiftInUse)
                {
                    bool stop = false;
                    if (Main.mouseItem.IsAir && !Main.HoverItem.IsAir)
                    {
                        for (int j = 0; j < EnchantingTable.maxItems && Main.cursorOverride != 9; j++)
                        {
                            if (wePlayer.enchantingTableUI.itemSlotUI[j].contains)
                            {
                                stop = true;
                            }
                        }
                        for (int j = 0; j < EnchantingTable.maxEnchantments && Main.cursorOverride != 9 && !stop; j++)
                        {
                            if (wePlayer.enchantingTableUI.enchantmentSlotUI[j].contains)
                            {
                                stop = true;
                            }
                        }
                        for (int j = 0; j < EnchantingTable.maxEssenceItems && Main.cursorOverride != 9 && !stop; j++)
                        {
                            if (wePlayer.enchantingTableUI.essenceSlotUI[j].contains)
                            {
                                stop = true;
                            }
                        }
                        if(!stop)
                            wePlayer.CheckShiftClickValid(ref Main.HoverItem);
                    }
                    if (Main.cursorOverride != 9 && !stop || Main.cursorOverride == 6)
                    {
                        Main.cursorOverride = -1;
                    }
                }
            }

            //Calamity Reforge
            if(EnchantedItem.calamityReforged) {
                if(Main.reforgeItem.TryGetEnchantedItem()) {
                    //Calamity only
                    EnchantedItem.ReforgeItem(ref Main.reforgeItem, wePlayer.Player, true);
                }
				else {
                    //Calamity and AutoReforge
                    EnchantedItem.ReforgeItem(ref EnchantedItem.calamityAndAutoReforgePostReforgeItem, wePlayer.Player, true);
                }
            }

            if(stolenItemToBeCleared != -1 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Item itemToClear = Main.item[stolenItemToBeCleared];
                if (itemToClear != null && itemToClear.TryGetGlobalItem(out EnchantedItem iGlobal))
                {
                    iGlobal.lastValueBonus = 0;
                    iGlobal.prefix = -1;
                }
                stolenItemToBeCleared = -1;
            }
            if(playerSwapperModEnabled && Main.netMode != NetmodeID.Server)
            {
                string playerName = wePlayer.Player.name;
                if (!updatedPlayerNames.Contains(playerName))
				{
                    OldItemManager.ReplaceAllPlayerOldItems(wePlayer.Player);
                    updatedPlayerNames.Add(playerName);
                }
            }
        }
        internal static void CloseWeaponEnchantmentUI(bool noSound = false)//Check on tick if too far or wePlayer.Player.chest != wePlayer.chest
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            if(wePlayer.enchantingTableUI?.itemSlotUI[0]?.Item != null && !wePlayer.enchantingTableUI.itemSlotUI[0].Item.IsAir)
            {
                wePlayer.enchantingTableUI.itemSlotUI[0].Item = wePlayer.Player.GetItem(Main.myPlayer, wePlayer.enchantingTableUI.itemSlotUI[0].Item, GetItemSettings.LootAllSettings);
                if (wePlayer.enchantingTableUI.itemSlotUI[0].Item.IsAir)
                {
                    wePlayer.enchantingTable.item[0] = new Item();
                    for(int i = 0; i < EnchantingTable.maxEnchantments; i++)
                    {
                        wePlayer.enchantmentInEnchantingTable[i] = false;
                        wePlayer.enchantingTable.enchantmentItem[i] = new Item();
                        wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item = new Item();
                    }
                }
            }
            wePlayer.itemBeingEnchanted = null;
            wePlayer.itemInEnchantingTable = false;
            wePlayer.usingEnchantingTable = false;//Stop checking enchantingTable slots
            if(wePlayer.Player.chest == -1)
            {
                if (!noSound) SoundEngine.PlaySound(SoundID.MenuClose);
            }
            wePlayer.enchantingTable.Close();
            //wePlayer.enchantingTableUI.OnDeactivate();//Store items left in enchanting table to player
            if (WeaponEnchantmentUI.PR)
            {
                weModSystemUI.SetState(null);
                promptInterface.SetState(null);
            }//PR
            ItemSlot.Options.DisableLeftShiftTrashCan = wePlayer.disableLeftShiftTrashCan;
        }
        internal static void OpenWeaponEnchantmentUI(bool noSound = false)
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            wePlayer.usingEnchantingTable = true;
            if(!noSound) SoundEngine.PlaySound(SoundID.MenuOpen);

            if (WeaponEnchantmentUI.PR)
            {
                UIState state = new UIState();
                state.Append(wePlayer.enchantingTableUI);
                weModSystemUI.SetState(state);
            }

            wePlayer.enchantingTable.Open();
            //wePlayer.enchantingTableUI.OnActivate();
            if(wePlayer.enchantingTableTier > 0)
            {
                needsToQuickStack = true;
            }
        }
        public static bool QuickStackEssence()
        {
            bool transfered = false;
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            for (int j = 0; j < 50; j++) 
            {
                if (WEMod.IsEssenceItem(wePlayer.Player.inventory[j])) 
                {
                    for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
                    {
                        if(((EnchantmentEssence)wePlayer.Player.inventory[j].ModItem).essenceRarity == wePlayer.enchantingTableUI.essenceSlotUI[i]._slotTier)
                        {
                            int ammountToTransfer = 0;
                            int startingStack = wePlayer.Player.inventory[j].stack;
                            if (wePlayer.enchantingTableUI.essenceSlotUI[i].Item.IsAir)
                            {
                                ammountToTransfer = wePlayer.Player.inventory[j].stack;
                                wePlayer.enchantingTableUI.essenceSlotUI[i].Item = wePlayer.Player.inventory[j].Clone();
                                wePlayer.Player.inventory[j] = new Item();
                                transfered = true;
                            }
                            else
                            {
                                if(wePlayer.enchantingTableUI.essenceSlotUI[i].Item.stack < EnchantmentEssence.maxStack)
                                {
                                    if (wePlayer.Player.inventory[j].stack + wePlayer.enchantingTableUI.essenceSlotUI[i].Item.stack > EnchantmentEssence.maxStack)
                                    {
                                        ammountToTransfer = EnchantmentEssence.maxStack - wePlayer.enchantingTableUI.essenceSlotUI[i].Item.stack;
                                    }
                                    else
                                    {
                                        ammountToTransfer = wePlayer.Player.inventory[j].stack;
                                    }
                                    wePlayer.enchantingTableUI.essenceSlotUI[i].Item.stack += ammountToTransfer;
                                    wePlayer.Player.inventory[j].stack -= ammountToTransfer;
                                    transfered = true;
                                }
                            }
                            if(wePlayer.Player.inventory[j].stack == startingStack)
                            {
                                transfered = false;
                            }
                            break;
                        }
                    }
                }
            }
            if (transfered)
            {
                SoundEngine.PlaySound(SoundID.Grab);
            }
            return transfered;
        }
        public static bool AutoCraftEssence()
        {
            bool crafted = false;
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            for (int i = EnchantingTable.maxEssenceItems - 1; i > 0; i--)
            {
                if(wePlayer.enchantingTableUI.essenceSlotUI[i].Item.stack < EnchantmentEssence.maxStack)
                {
                    int ammountToTransfer;
                    if(wePlayer.enchantingTableUI.essenceSlotUI[i].Item.stack == 0 || (EnchantmentEssence.maxStack > wePlayer.enchantingTableUI.essenceSlotUI[i].Item.stack + (wePlayer.enchantingTableUI.essenceSlotUI[i - 1].Item.stack / 4)))
                    {
                        ammountToTransfer = wePlayer.enchantingTableUI.essenceSlotUI[i - 1].Item.stack / 4;
                    }
                    else
                    {
                        ammountToTransfer = EnchantmentEssence.maxStack - wePlayer.enchantingTableUI.essenceSlotUI[i].Item.stack;
                    }
                    if(ammountToTransfer > 0)
                    {
                        wePlayer.enchantingTableUI.essenceSlotUI[i].Item.stack += ammountToTransfer;
                        wePlayer.enchantingTableUI.essenceSlotUI[i - 1].Item.stack -= ammountToTransfer * 4;
                        crafted = true;
                    }
                }
            }
            for (int i = 1; i < EnchantingTable.maxEssenceItems; i++)
            {
                if (wePlayer.enchantingTableUI.essenceSlotUI[i].Item.stack < EnchantmentEssence.maxStack)
                {
                    int ammountToTransfer;
                    if (wePlayer.enchantingTableUI.essenceSlotUI[i].Item.stack == 0 || (EnchantmentEssence.maxStack > wePlayer.enchantingTableUI.essenceSlotUI[i].Item.stack + (wePlayer.enchantingTableUI.essenceSlotUI[i - 1].Item.stack / 4)))
                    {
                        ammountToTransfer = wePlayer.enchantingTableUI.essenceSlotUI[i - 1].Item.stack / 4;
                    }
                    else
                    {
                        ammountToTransfer = EnchantmentEssence.maxStack - wePlayer.enchantingTableUI.essenceSlotUI[i].Item.stack;
                    }
                    if (ammountToTransfer > 0)
                    {
                        wePlayer.enchantingTableUI.essenceSlotUI[i].Item.stack += ammountToTransfer;
                        wePlayer.enchantingTableUI.essenceSlotUI[i - 1].Item.stack -= ammountToTransfer * 4;
                        crafted = true;
                    }
                }
            }
            return crafted;
        }
        public override void PreSaveAndQuit()//*
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            weModSystemUI.SetState(null);
            promptInterface.SetState(null);
            if (wePlayer.usingEnchantingTable)
            {
                CloseWeaponEnchantmentUI();
                wePlayer.enchantingTableUI.OnDeactivate();
            }
        }
        public override void UpdateUI(GameTime gameTime)//*
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            //Update UI
            _lastUpdateUiGameTime = gameTime;
            if(weModSystemUI?.CurrentState != null)
            {
                weModSystemUI.Update(gameTime);
                if (firstDraw) 
                { 
                    firstDraw = false;
                } 
                else if(secondDraw) 
                { 
                    secondDraw = false;
                    if (wePlayer.enchantingTableTier > 0)
                    {
                        needsToQuickStack = true;
                    }
                }
                else if (tryNextTick && !secondDraw)
                {
                    if (Main.playerInventory)
                    {
                        bool crafted;
                        if (wePlayer.enchantingTableTier == EnchantingTable.maxTier)
                        {
                            crafted = false;//AutoCraftEssence();
                            if (!transfered && crafted)
                            {
                                SoundEngine.PlaySound(SoundID.Grab);
                            }
                        }
                        tryNextTick = false;
                    }
                }
                else if (needsToQuickStack)
                {
                    needsToQuickStack = false;
                    tryNextTick = true;
                    transfered = QuickStackEssence();
                }
                //wePlayer.enchantingTableUI.DrawSelf(Main.spriteBatch);
            }
            //mouseoverUI.Update(gameTime);
            if(promptInterface?.CurrentState != null)
            {
                promptInterface.Update(gameTime);
            }
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)//*
        {
            int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Over"));
            if (index != -1)
            {
                layers.Insert
                (
                    ++index, 
                    new LegacyGameInterfaceLayer
                    (
                        "WeaponEnchantments: Mouse Over", 
                        delegate 
                        { 
                            if (_lastUpdateUiGameTime != null && mouseoverUIInterface?.CurrentState != null) 
                            { 
                                mouseoverUIInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                            } return true; 
                        }, 
                        InterfaceScaleType.UI
                     )
                );
            }
            index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));//++
            if (index != -1)//++
            {
                layers.Insert(index, new LegacyGameInterfaceLayer(//++
                    "WeaponEnchantments: WeaponEnchantmentsUI",//++
                    delegate//++
                    {
                        if (_lastUpdateUiGameTime != null && weModSystemUI?.CurrentState != null)//++
                        {
                            weModSystemUI.Draw(Main.spriteBatch, _lastUpdateUiGameTime);//++
                        }
                        return true;//++
                    },
                    InterfaceScaleType.UI)//++
                );
            }
            index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));//++
            if (index != -1)//++
            {
                layers.Insert(index, new LegacyGameInterfaceLayer(//++
                    "WeaponEnchantments: PromptUI",//++
                    delegate//++
                    {
                        if (_lastUpdateUiGameTime != null && promptInterface?.CurrentState != null)//++
                        {
                            promptInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        }
                        return true;//++
                    },
                    InterfaceScaleType.UI)//++
                );
            }
        }
        public override void AddRecipeGroups() {
            RecipeGroup group = new RecipeGroup(() => "Any Common Gem", new int[]
            {
                ItemID.Sapphire, ItemID.Ruby, ItemID.Emerald, ItemID.Topaz, ItemID.Amethyst
            });
            RecipeGroup.RegisterGroup("WeaponEnchantments:CommonGems", group);
            group = new RecipeGroup(() => "Any Rare Gem", new int[]
            {
                ItemID.Amber, ItemID.Diamond
            });
            RecipeGroup.RegisterGroup("WeaponEnchantments:RareGems", group);
            group = new RecipeGroup(() => "Workbenches", new int[]
            {
                ItemID.WorkBench, ItemID.BambooWorkbench, ItemID.BlueDungeonWorkBench, ItemID.BoneWorkBench, ItemID.BorealWoodWorkBench, ItemID.CactusWorkBench, ItemID.CrystalWorkbench, ItemID.DynastyWorkBench, ItemID.EbonwoodWorkBench, ItemID.FleshWorkBench, ItemID.FrozenWorkBench, ItemID.GlassWorkBench, ItemID.GoldenWorkbench, ItemID.GothicWorkBench, ItemID.GraniteWorkBench, ItemID.GreenDungeonWorkBench, ItemID.HoneyWorkBench, ItemID.LesionWorkbench, ItemID.LihzahrdWorkBench, ItemID.LivingWoodWorkBench, ItemID.MarbleWorkBench, ItemID.MartianWorkBench, ItemID.MeteoriteWorkBench, ItemID.MushroomWorkBench, ItemID.NebulaWorkbench, ItemID.ObsidianWorkBench, ItemID.PalmWoodWorkBench, ItemID.PearlwoodWorkBench, ItemID.PinkDungeonWorkBench, ItemID.PumpkinWorkBench, ItemID.RichMahoganyWorkBench, ItemID.SandstoneWorkbench, ItemID.ShadewoodWorkBench, ItemID.SkywareWorkbench, ItemID.SlimeWorkBench, ItemID.SolarWorkbench, ItemID.SpiderWorkbench, ItemID.SpookyWorkBench, ItemID.StardustWorkbench, ItemID.SteampunkWorkBench, ItemID.VortexWorkbench
            });
            RecipeGroup.RegisterGroup("WeaponEnchantments:Workbenches", group);
            group = new RecipeGroup(() => "Any Aligned Soul", new int[]
            {
                ItemID.SoulofLight, ItemID.SoulofNight
            });
            RecipeGroup.RegisterGroup("WeaponEnchantments:AlignedSoul", group);
        }
		public override void PostWorldGen()
        {
            for (int chestIndex = 0; chestIndex < 1000; chestIndex++)
            {
                Chest chest = Main.chest[chestIndex];
                if(chest != null)
                {
                    float chance = 0.5f;
                    int itemsPlaced = 0;
                    List<int> itemTypes = new List<int>();
                    switch (Main.tile[chest.x, chest.y].TileType)
                    {
                        case 21:
                        case 441:
                            switch (Main.tile[chest.x, chest.y].TileFrameX / 36)
                            {
                                case 0://Chest
                                    chance = 0.35f;
                                    itemTypes.Add(ModContent.ItemType<StatDefenseEnchantmentBasic>());
                                    itemTypes.Add(ModContent.ItemType<DamageEnchantmentBasic>());
                                    itemTypes.Add(ModContent.ItemType<CriticalStrikeChanceEnchantmentBasic>());
                                    itemTypes.Add(ModContent.ItemType<ManaEnchantmentBasic>());
                                    itemTypes.Add(ModContent.ItemType<ScaleEnchantmentBasic>());
                                    itemTypes.Add(ModContent.ItemType<AmmoCostEnchantmentBasic>());
                                    itemTypes.Add(ModContent.ItemType<SpeedEnchantmentBasic>());
                                    itemTypes.Add(ModContent.ItemType<PeaceEnchantmentBasic>());
                                    break;
                                case 1://Gold Chest
                                    itemTypes.Add(ModContent.ItemType<CriticalStrikeChanceEnchantmentBasic>());
                                    itemTypes.Add(ModContent.ItemType<SpelunkerEnchantmentUltraRare>());
                                    itemTypes.Add(ModContent.ItemType<DangerSenseEnchantmentUltraRare>());
                                    itemTypes.Add(ModContent.ItemType<HunterEnchantmentUltraRare>());
                                    itemTypes.Add(ModContent.ItemType<ObsidianSkinEnchantmentUltraRare>());
                                    itemTypes.Add(ModContent.ItemType<SpeedEnchantmentBasic>());
                                    break;
                                case 2://Gold Chest (Locked)
                                    itemTypes.Add(ModContent.ItemType<AllForOneEnchantmentBasic>());
                                    itemTypes.Add(ModContent.ItemType<OneForAllEnchantmentBasic>());
                                    break;
                                case 3://Shadow Chest
                                case 4://Shadow Chest (Locked)
                                    chance = 1f;
                                    itemTypes.Add(ModContent.ItemType<ArmorPenetrationEnchantmentBasic>());
                                    itemTypes.Add(ModContent.ItemType<LifeStealEnchantmentBasic>());
                                    itemTypes.Add(ModContent.ItemType<WarEnchantmentBasic>());
                                    break;
                                case 8://Rich Mahogany Chest (Jungle)
                                    itemTypes.Add(ModContent.ItemType<CriticalStrikeChanceEnchantmentBasic>());
                                    break;
                                case 10://Ivy Chest (Jungle)
                                    itemTypes.Add(ModContent.ItemType<CriticalStrikeChanceEnchantmentBasic>());
                                    break;
                                case 11://Frozen Chest
                                    itemTypes.Add(ModContent.ItemType<ManaEnchantmentBasic>());
                                    break;
                                case 12://Living Wood Chest
                                    itemTypes.Add(ModContent.ItemType<ScaleEnchantmentBasic>());
                                    break;
                                case 13://Skyware Chest
                                    itemTypes.Add(ModContent.ItemType<SpeedEnchantmentBasic>());
                                    break;
                                case 15://Web Covered Chest
                                    itemTypes.Add(ModContent.ItemType<AmmoCostEnchantmentBasic>());
                                    break;
                                case 16://Lihzahrd Chest
                                    chance = 1f;
                                    itemTypes.Add(ModContent.ItemType<ArmorPenetrationEnchantmentBasic>());
                                    itemTypes.Add(ModContent.ItemType<LifeStealEnchantmentBasic>());
                                    itemTypes.Add(ModContent.ItemType<AllForOneEnchantmentBasic>());
                                    itemTypes.Add(ModContent.ItemType<OneForAllEnchantmentBasic>());
                                    break;
                                case 17://Water Chest
                                    itemTypes.Add(ModContent.ItemType<ManaEnchantmentBasic>());
                                    break;
                                case 23://Jungle Chest
                                        chance = 1f;
                                        //itemTypes.Add(ModContent.ItemType<Enchantment>());
                                    break;
                                case 24://Corruption Chest
                                        chance = 1f;
                                        //itemTypes.Add(ModContent.ItemType<Enchantment>());
                                    break;
                                case 25://Crimson Chest
                                        chance = 1f;
                                        //itemTypes.Add(ModContent.ItemType<Enchantment>());
                                    break;
                                case 26://Hallowed Chest
                                        chance = 1f;
                                        //itemTypes.Add(ModContent.ItemType<Enchantment>());
                                    break;
                                case 27://Ice Chest
                                        chance = 1f;
                                        //itemTypes.Add(ModContent.ItemType<Enchantment>());
                                    break;
                                case 32://Mushroom Chest
                                    itemTypes.Add(ModContent.ItemType<AmmoCostEnchantmentBasic>());
                                    break;
                                case 40://Granite Chest
                                    itemTypes.Add(ModContent.ItemType<SpeedEnchantmentBasic>());
                                    break;
                                case 41://Marble Chest
                                    itemTypes.Add(ModContent.ItemType<AmmoCostEnchantmentBasic>());
                                    break;
                            }
                            break;
                        case 467:
                        case 468:
                            switch (Main.tile[chest.x, chest.y].TileFrameX / 36)
                            {
                                case 4://Gold Dead man's chest
                                    itemTypes.Add(ModContent.ItemType<CriticalStrikeChanceEnchantmentBasic>());
                                    itemTypes.Add(ModContent.ItemType<SpelunkerEnchantmentUltraRare>());
                                    itemTypes.Add(ModContent.ItemType<DangerSenseEnchantmentUltraRare>());
                                    itemTypes.Add(ModContent.ItemType<HunterEnchantmentUltraRare>());
                                    itemTypes.Add(ModContent.ItemType<ObsidianSkinEnchantmentUltraRare>());
                                    itemTypes.Add(ModContent.ItemType<SpeedEnchantmentBasic>());
                                    break;
                                case 10://SandStone Chest
                                    itemTypes.Add(ModContent.ItemType<AmmoCostEnchantmentBasic>());
                                    break;
                                case 13://Desert Chest
                                        chance = 1f;
                                        //itemTypes.Add(ModContent.ItemType<Enchantment>());
                                    break;
                            }
                            break;
                        default:
                            chance = 0f;
                            break;
                    }
                    // If you look at the sprite for Chests by extracting Tiles_21.xnb, you'll see that the 12th chest is the Ice Chest. Since we are counting from 0, this is where 11 comes from. 36 comes from the width of each tile including padding. 
                    if (chest != null)//Make sure the chest exists
                    {
                        for (int j = 0; j < 40 && itemsPlaced < chance; j++)//for each slot in the chest(40), try to place an item.  itemsPlaced < chance is if you want to place more than 1 by setting chance to something greater than 1f.
                        {
                            if (chest.item[j].type == ItemID.None)//If the itemslot you're currently looking at in the chest is empty(ItemID.None), try spawning an item there.
                            {
                                if (itemTypes.Count > 1)//itemTypes is set in the switch statemts eariler.  It's a list of possible items to spawn.
                                {
                                    float randFloat = Main.rand.NextFloat();//Get a random float number between 0f and 1f.
                                    for (int i = 0; i < itemTypes.Count; i++)//This part distributes the drop chance between all the items in itemTypes evenly
                                    {
                                        //Example, Gold Dead man's chest (just above this section) has 6 items in itemTypes and a chance of 0.5f (50%).  
                                        //Lets say randFloat is 0.3f;
                                        //iterating through the loop: starting with i = 0:

                                        //randFloat: 0.5, i: 0, itemTypes.Count: 6, chance: 0.5
                                        //0.3 >= 0 / 6 * 0.5 && 0.3 < (0 + 1) / 6 * 0.5   (simplify)   0.3 >= 0 && 0.3 < 0.083333.  This statement is false, so is skipped

                                        //randFloat: 0.5, i: 1, itemTypes.Count: 6, chance: 0.5
                                        //0.3 >= 1 / 6 * 0.5 && 0.3 < (1 + 1) / 6 * 0.5   (simplify)   0.3 >= 0.083333 && 0.3 < 0.1666667.  This statement is false, so is skipped

                                        //randFloat: 0.5, i: 1, itemTypes.Count: 6, chance: 0.5
                                        //0.3 >= 2 / 6 * 0.5 && 0.3 < (2 + 1) / 6 * 0.5   (simplify)   0.3 >= 0.1666667 && 0.3 < 0.25.  This statement is false, so is skipped

                                        //randFloat: 0.5, i: 1, itemTypes.Count: 6, chance: 0.5
                                        //0.3 >= 3 / 6 * 0.5 && 0.3 < (3 + 1) / 6 * 0.5   (simplify)   0.3 >= 0.25 && 0.3 < 0.333333  This statement is true, so the if statement executes.
                                        if (randFloat >= (float)i / (float)itemTypes.Count * chance && randFloat < ((float)i + 1f) / (float)itemTypes.Count * chance)
                                        {
                                            //The item in the empty slot becomes the itemTypes[3] item in this case it would be "HunterEnchantmentUltraRare".
                                            chest.item[j].SetDefaults(itemTypes[i]);
                                            break;
                                        }
                                    }
                                }
                                else if(itemTypes.Count == 1)
                                {
                                    //If there is only 1 possible drop, there is no need for the above calculation.  Just compair the chance to the random float:
                                    //we'll say it was 0.3 again.  and chance is 0.5 again.  0.3 is < 0.5, so it will spawn the item.
                                    if (Main.rand.NextFloat() < chance)
                                    {
                                        chest.item[j].SetDefaults(itemTypes[0]);
                                    }
                                    
                                }
                                itemsPlaced++;//This will stop stop spawning from happening if your chance is < 1 becasue of "&& itemsPlaced < chance" in the for loop. 
                            }
                        }
                    }
                }
            }
        }
    }
}