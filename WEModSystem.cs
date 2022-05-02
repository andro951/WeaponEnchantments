using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Tile_Entities;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Items;
using WeaponEnchantments.Tiles;
using WeaponEnchantments.UI;

namespace WeaponEnchantments
{
    public class WEModSystem : ModSystem
    {
        internal static UserInterface weModSystemUI;
        internal static UserInterface mouseoverUIInterface;
        private GameTime _lastUpdateUiGameTime;
        private static Item[] itemSlots = new Item[EnchantingTable.maxItems];
        private static Item[] enchantmentSlots = new Item[EnchantingTable.maxEnchantments];
        private static Item[] essenceSlots = new Item[EnchantingTable.maxEssenceItems];
        //private bool slotsLinked = false;
        private static bool needsToQuickStack = false;
        private static bool tryNextTick = false;
        private static bool firstDraw = true;
        private static bool secondDraw = true;
        private static bool transfered = false;
        public static bool playerInventoryUpdated = false;
        public static bool enchantingTableInventoryUpdated = false;
        public static int previousChest = -1;
        //private static bool firstCheck = true;

        public override void OnModLoad()
        {
            if (!Main.dedServ)
            {
                weModSystemUI = new UserInterface();
                mouseoverUIInterface = new UserInterface();
            }
        }//PR
        public override void Unload()
        {
            if (!Main.dedServ)
            {
                weModSystemUI = null;
                mouseoverUIInterface = null;
            }
        }//PR
        public override void PostUpdatePlayers()
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();

            if (wePlayer.usingEnchantingTable)
            {
                if (wePlayer.enchantingTableUI.itemSlotUI[0].Item.IsAir)
                {
                    if (wePlayer.itemInEnchantingTable)//If item WAS in the itemSlot but it is empty now,
                    {
                        for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
                        {
                            if (wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item != null)//For each enchantment in the enchantmentSlots,
                            {
                                wePlayer.itemBeingEnchanted.GetGlobalItem<EnchantedItem>().enchantments[i] = wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item.Clone();//copy enchantments to the global item
                            }
                            wePlayer.enchantmentInEnchantingTable[i] = false;//The enchantmentSlot's PREVIOUS state is now empty(false)
                            wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item = new Item();//Delete enchantments still in enchantmentSlots(There were transfered to the global item)
                        }
                        wePlayer.itemBeingEnchanted.GetGlobalItem<EnchantedItem>().inEnchantingTable = false;
                        wePlayer.itemBeingEnchanted = wePlayer.enchantingTableUI.itemSlotUI[0].Item;//Stop tracking the item that just left the itemSlot
                    }//Transfer items to global item and break the link between the global item and enchanting table itemSlots/enchantmentSlots
                    wePlayer.itemInEnchantingTable = false;//The itemSlot's PREVIOUS state is now empty(false)
                }//Check if the itemSlot is empty because the item was just taken out and transfer the mods to the global item if so
                else if(!wePlayer.itemInEnchantingTable)//If itemSlot WAS empty but now has an item in it
                {
                    wePlayer.itemBeingEnchanted = wePlayer.enchantingTableUI.itemSlotUI[0].Item;// Link the item in the table to the player so it can be updated after being taken out.
                    wePlayer.itemBeingEnchanted.GetGlobalItem<EnchantedItem>().inEnchantingTable = true;
                    for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
                    {
                        if (wePlayer.enchantingTableUI.itemSlotUI[0].Item.GetGlobalItem<EnchantedItem>().enchantments[i] != null)//For each enchantment in the global item,
                        {
                            wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item = wePlayer.enchantingTableUI.itemSlotUI[0].Item.GetGlobalItem<EnchantedItem>().enchantments[i].Clone();//copy enchantments to the enchantmentSlots
                        }
                        //else
                        {
                            wePlayer.enchantingTableUI.itemSlotUI[0].Item.GetGlobalItem<EnchantedItem>().enchantments[i] = wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item;//Link global item to the enchantmentSlots
                        }
                    }
                    wePlayer.itemInEnchantingTable = true;//Set PREVIOUS state of itemSlot to having an item in it
                }//Check if itemSlot has item that was just placed there, copy the enchantments to the slots and link the slots to the global item
                for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
                {
                    if (wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item.IsAir)
                    {
                        if (wePlayer.enchantmentInEnchantingTable[i])//if enchantmentSlot HAD an enchantment in it but it was just taken out,
                        {
                            //Force global item to re-link to the enchantmentSlot instead of following the enchantment just taken out
                            wePlayer.enchantingTableUI.itemSlotUI[0].Item.GetGlobalItem<EnchantedItem>().enchantments[i] = wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item;
                        }
                        wePlayer.enchantmentInEnchantingTable[i] = false;//Set PREVIOUS state of enchantmentSlot to empty(false)
                    }
                    else if(wePlayer.enchantingTableUI.itemSlotUI[0].Item.GetGlobalItem<EnchantedItem>().enchantments[i] != wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item)
                    {
                        wePlayer.enchantingTableUI.itemSlotUI[0].Item.GetGlobalItem<EnchantedItem>().enchantments[i] = wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item;
                    }//If player swapped enchantments (without removing the previous one in the enchantmentSlot) Force global item to re-link to the enchantmentSlot instead of following the enchantment just taken out
                    else if (!wePlayer.enchantmentInEnchantingTable[i])
                    {
                        wePlayer.enchantmentInEnchantingTable[i] = true;//Set PREVIOUS state of enchantmentSlot to has an item in it(true)
                        wePlayer.enchantingTableUI.itemSlotUI[0].Item.GetGlobalItem<EnchantedItem>().enchantments[i] = wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item;//Force link to enchantmentSlot just in case
                    }//If it WAS empty but isn't now, re-link global item to enchantmentSlot just in case
                }//Check if enchantments are added/removed from enchantmentSlots and re-link global item to enchantmentSlot
                if (!wePlayer.Player.IsInInteractionRangeToMultiTileHitbox(wePlayer.Player.chestX, wePlayer.Player.chestY) || wePlayer.Player.chest != -1 || !Main.playerInventory)
                {
                    CloseWeaponEnchantmentUI();
                }//If player is too far away, close the enchantment table
                //wePlayer.CustomFindRecipeis();
                if(Main.mouseItem.type == Main.recipe[wePlayer.lastFocusRecipeType].createItem.type)
                {
                    if ((wePlayer.inventoryItemRecord[91].type == 0 && wePlayer.inventoryItemRecord[91].type != Main.mouseItem.type) || wePlayer.inventoryItemRecord[91].type == Main.mouseItem.type && wePlayer.inventoryItemRecord[91].stack != Main.mouseItem.stack)
                    {bool pickedUp = false;
                        for(int i = 0; i < 50; i++)
                        {
                            if(wePlayer.inventoryItemRecord[i].type == Main.recipe[wePlayer.lastFocusRecipeType].createItem.type)
                            {
                                if (wePlayer.Player.inventory[i].stack < wePlayer.inventoryItemRecord[i].stack)
                                {
                                    pickedUp = true;
                                }
                            }
                        }
                        for(int i = 92; i < 92 + EnchantingTable.maxEnchantments + EnchantingTable.maxEssenceItems; i++)
                        {
                            if(i < 92 + EnchantingTable.maxEnchantments)
                            {
                                if (wePlayer.enchantingTableUI.enchantmentSlotUI[i - 92].Item.stack < wePlayer.inventoryItemRecord[i].stack)
                                {
                                    pickedUp = true;
                                }
                            }
                            else
                            {
                                if (wePlayer.enchantingTableUI.essenceSlotUI[i - EnchantingTable.maxEnchantments - 92].Item.stack < wePlayer.inventoryItemRecord[i].stack)
                                {
                                    pickedUp = true;
                                }
                            }
                        }
                        if (!pickedUp)
                        {
                            foreach(Item requiredItem in Main.recipe[wePlayer.lastFocusRecipeType].requiredItem)
                            {
                                if(requiredItem.ModItem is EnchantmentEssence)
                                {
                                    int ammountConsumedFromInventory = 0;
                                    for (int j = 0; j < 50; j++)
                                    {
                                        if (wePlayer.inventoryItemRecord[j].type == requiredItem.type)
                                        {
                                            ammountConsumedFromInventory += wePlayer.inventoryItemRecord[j].stack;
                                        }
                                    }
                                    wePlayer.enchantingTableUI.essenceSlotUI[requiredItem.type - EnchantmentEssence.IDs[0]].Item.stack -= requiredItem.stack - ammountConsumedFromInventory;
                                }
                            }
                        }
                    }
                }
                wePlayer.CustomFindRecipeis();
            }//If enchanting table is open, check item(s) and enchantments in it every tick
            if (Main.playerInventory)
            {
                for (int i = 0; i < 50; i++)
                {
                    if (wePlayer.Player.inventory?[i] != null)//try get rid of
                    {
                        if (!wePlayer.Player.inventory[i].IsAir)//try get rid of
                        {
                            wePlayer.inventoryItemRecord[i] = wePlayer.Player.inventory[i].Clone();
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
                    Item[] inventory = Main.chest[wePlayer.Player.chest].item;
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
                    for(int i = 0; i < EnchantingTable.maxEnchantments; i++)
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
        }//If enchanting table is open, check item(s) and enchantments in it every tick
        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            if (wePlayer.usingEnchantingTable)
            {
                if (ItemSlot.ShiftInUse)
                {
                    if (Main.mouseItem.IsAir)
                    {
                        for (int x = 0; x < 10; x++)
                        {
                            for (int y = 0; y < 5; y++)
                            {
                                int xTotal = (int)(17f + (x * 47.6f));
                                int yTotal = (int)(17f + (y * 47.6f));
                                int i = x + y * 10;
                                if (Main.mouseX >= xTotal && Main.mouseX <= xTotal + 44.2f && Main.mouseY >= yTotal && Main.mouseY <= yTotal + 44.2f && !PlayerInput.IgnoreMouseInterface)
                                {
                                    if (!wePlayer.Player.inventory[i].IsAir)
                                    {
                                        if (wePlayer.Player.inventory[i].type == PowerBooster.ID && !wePlayer.enchantingTableUI.itemSlotUI[0].Item.IsAir && !wePlayer.enchantingTableUI.itemSlotUI[0].Item.GetGlobalItem<EnchantedItem>().powerBoosterInstalled)
                                        {
                                            Main.cursorOverride = 9;
                                        }
                                        else
                                        {
                                            for (int j = 0; j < EnchantingTable.maxItems; j++)
                                            {
                                                if (wePlayer.enchantingTableUI.itemSlotUI[j].Valid(wePlayer.Player.inventory[i]))
                                                {
                                                    Main.cursorOverride = 9;
                                                    break;
                                                }
                                            }
                                            if (Main.cursorOverride != 9)
                                            {
                                                for (int j = 0; j < EnchantingTable.maxEnchantments; j++)
                                                {
                                                    if (wePlayer.enchantingTableUI.enchantmentSlotUI[j].Valid(wePlayer.Player.inventory[i]))
                                                    {
                                                        Main.cursorOverride = 9;
                                                    }
                                                }
                                            }
                                            if (Main.cursorOverride != 9)
                                            {
                                                for (int j = 0; j < EnchantingTable.maxEssenceItems; j++)
                                                {
                                                    if (wePlayer.enchantingTableUI.essenceSlotUI[j].Valid(wePlayer.Player.inventory[i]))
                                                    {
                                                        if (wePlayer.enchantingTableUI.essenceSlotUI[j].Item.IsAir)
                                                        {
                                                            Main.cursorOverride = 9;
                                                        }
                                                        else
                                                        {
                                                            if (wePlayer.enchantingTableUI.essenceSlotUI[j].Item.stack < EnchantmentEssence.maxStack)
                                                            {
                                                                Main.cursorOverride = 9;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        if (Main.cursorOverride != 9)
                                        {
                                            Main.cursorOverride = -1;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if(Main.cursorOverride == 6)
                    {
                        Main.cursorOverride = -1;
                    }
                }
            }
            /*if (Main.playerInventory)
            {
                if (!playerInventoryUpdated)
                {
                    for (int i = 0; i < 50; i++)
                    {
                        if (wePlayer.Player.inventory?[i] != null)
                        {
                            if (!wePlayer.Player.inventory[i].IsAir)
                            {
                                wePlayer.inventoryItemRecord[i] = wePlayer.Player.inventory[i].Clone();
                            }
                            else
                            {
                                wePlayer.inventoryItemRecord[i] = new Item();
                            }
                        }
                        else
                        {
                            wePlayer.inventoryItemRecord[i] = new Item();
                        }
                    }
                    playerInventoryUpdated = true;
                }
                if (wePlayer.Player.chest != previousChest && wePlayer.Player.chest != -1)
                {
                    Item[] inventory = Main.chest[wePlayer.Player.chest].item;
                    for (int i = 0; i < 40; i++)
                    {
                        if (inventory?[i] != null)
                        {
                            if (!inventory[i].IsAir)
                            {
                                wePlayer.inventoryItemRecord[i + 50] = inventory[i].Clone();
                            }
                            else
                            {
                                wePlayer.inventoryItemRecord[i + 50] = new Item();
                            }
                        }
                        else
                        {
                            wePlayer.inventoryItemRecord[i + 50] = new Item();
                        }
                    }
                    previousChest = wePlayer.Player.chest;
                    enchantingTableInventoryUpdated = false;
                    playerInventoryUpdated = false;
                }
                else if (previousChest == -6)
                {
                    for (int i = 0; i < 40; i++)
                    {
                        wePlayer.inventoryItemRecord[i + 50] = new Item();
                    }
                    previousChest = -1;
                }
                if (wePlayer.usingEnchantingTable)
                {
                    if (!enchantingTableInventoryUpdated)
                    {
                        if (wePlayer.enchantingTableUI?.itemSlotUI?[0]?.Item != null)
                        {
                            if (!wePlayer.enchantingTableUI.itemSlotUI[0].Item.IsAir)
                            {
                                wePlayer.inventoryItemRecord[90] = wePlayer.enchantingTableUI.itemSlotUI[0].Item.Clone();
                            }
                            else
                            {
                                wePlayer.inventoryItemRecord[90] = new Item();
                            }
                        }
                        else
                        {
                            wePlayer.inventoryItemRecord[90] = new Item();
                        }
                        enchantingTableInventoryUpdated = true;
                        playerInventoryUpdated = false;
                    }
                }
                else
                {
                    enchantingTableInventoryUpdated = false;
                    playerInventoryUpdated = false;
                }
            }
            else
            {
                playerInventoryUpdated = false;
                enchantingTableInventoryUpdated = false;
                previousChest = -6;
            }*/
        }
        internal static void CloseWeaponEnchantmentUI()//Check on tick if too far or wePlayer.Player.chest != wePlayer.chest
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            wePlayer.usingEnchantingTable = false;//Stop checking enchantingTable slots
            if(wePlayer.Player.chest == -1)
            {
                SoundEngine.PlaySound(SoundID.MenuClose);
            }
            wePlayer.enchantingTable.Close();
            wePlayer.enchantingTableUI.OnDeactivate();//Store items left in enchanting table to player
            if (WeaponEnchantmentUI.PR)
            {
                weModSystemUI.SetState(null);
            }//PR
        }
        internal static void OpenWeaponEnchantmentUI()
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            wePlayer.usingEnchantingTable = true;
            SoundEngine.PlaySound(SoundID.MenuOpen);

            if (WeaponEnchantmentUI.PR)
            {
                UIState state = new UIState();
                state.Append(wePlayer.enchantingTableUI);
                weModSystemUI.SetState(state);
            }

            wePlayer.enchantingTable.Open();
            wePlayer.enchantingTableUI.OnActivate();
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
                                        wePlayer.Player.inventory[j].stack -= ammountToTransfer;
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
            weModSystemUI.SetState(null);
        }
        public override void UpdateUI(GameTime gameTime)//*
        {
            _lastUpdateUiGameTime = gameTime;
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            //if (wePlayer.usingEnchantingTable)
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
                            crafted = AutoCraftEssence();
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
                    "WeaponEnchantments: ",//++
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
        }
    }
}
