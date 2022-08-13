using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
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
using static WeaponEnchantments.Common.Configs.ConfigValues;

namespace WeaponEnchantments
{
    public class WEModSystem : ModSystem
    {
        public static bool AltDown => Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftAlt) || Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightAlt);
        public static bool ShiftDown => Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift) || Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift);
        internal static UserInterface weModSystemUI;
        internal static UserInterface mouseoverUIInterface;
        internal static UserInterface promptInterface;
        public static bool PromptInterfaceActive => promptInterface?.CurrentState != null;
        public static int[] levelXps = new int[EnchantedItem.MAX_LEVEL];
        private static bool favorited;
        public static int stolenItemToBeCleared = -1;
        public static List<string> updatedPlayerNames;

        private GameTime _lastUpdateUiGameTime;

        public override void OnModLoad() {
            if (!Main.dedServ) {
                weModSystemUI = new UserInterface();
                promptInterface = new UserInterface();
                mouseoverUIInterface = new UserInterface();
            }

            double previous = 0;
            double current;
            int l;
            for (l = 0; l < EnchantedItem.MAX_LEVEL; l++) {
                current = previous * 1.23356622200537 + (l + 1) * 1000;
                previous = current;
                levelXps[l] = (int)current;
            }

            WEMod.playerSwapperModEnabled = ModLoader.HasMod("PlayerSwapper");
            if (WEMod.playerSwapperModEnabled)
                updatedPlayerNames = new List<string>();
        }
        public override void Unload() {
            if (!Main.dedServ) {
                weModSystemUI = null;
                mouseoverUIInterface = null;
                promptInterface = null;
            }
        }
        public override void PostDrawInterface(SpriteBatch spriteBatch) {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            if (wePlayer.usingEnchantingTable) {
                //Disable Left Shift to Quick trash
                if (ItemSlot.Options.DisableLeftShiftTrashCan) {
                    wePlayer.disableLeftShiftTrashCan = ItemSlot.Options.DisableLeftShiftTrashCan;
                    ItemSlot.Options.DisableLeftShiftTrashCan = false;
                }

                Item itemInUI = wePlayer.ItemInUI();
                bool removedItem = false;
                bool addedItem = false;
                bool swappedItem = false;
                //Check if the itemSlot is empty because the item was just taken out and transfer the mods to the global item if so
                if (itemInUI.IsAir) {
                    if (wePlayer.itemInEnchantingTable)//If item WAS in the itemSlot but it is empty now,
                        removedItem = true;//Transfer items to global item and break the link between the global item and enchanting table itemSlots/enchantmentSlots

                    wePlayer.itemInEnchantingTable = false;//The itemSlot's PREVIOUS state is now empty(false)
                }
                else if (!wePlayer.itemInEnchantingTable) {//If itemSlot WAS empty but now has an item in it
                    //Check if itemSlot has item that was just placed there, copy the enchantments to the slots and link the slots to the global item
                    addedItem = true;
                    wePlayer.itemInEnchantingTable = true;//Set PREVIOUS state of itemSlot to having an item in it
                }
                else if (wePlayer.itemBeingEnchanted != itemInUI) {
                    swappedItem = true;
                }

                if (removedItem || swappedItem) {
                    RemoveTableItem(wePlayer);
                }

                if (addedItem || swappedItem) {
                    wePlayer.itemBeingEnchanted = wePlayer.ItemInUI();// Link the item in the table to the player so it can be updated after being taken out.
                    Item itemBeingEnchanted = wePlayer.itemBeingEnchanted;
                    favorited = itemBeingEnchanted.favorited;
                    itemBeingEnchanted.favorited = false;
                    if(itemBeingEnchanted.TryGetEnchantedItem(out EnchantedItem iBEGlobal)) {
                        iBEGlobal.inEnchantingTable = true;
                        iBEGlobal.equippedInArmorSlot = false;
                        wePlayer.previousInfusedItemName = iBEGlobal.infusedItemName;
                    }

                    if (wePlayer.infusionConsumeItem != null && (EnchantedItemStaticMethods.IsWeaponItem(itemBeingEnchanted) || EnchantedItemStaticMethods.IsArmorItem(itemBeingEnchanted))) {
                        wePlayer.itemBeingEnchanted.TryInfuseItem(wePlayer.infusionConsumeItem);
                        wePlayer.enchantingTableUI.infusionButonText.SetText("Finalize");
                    }

					if (wePlayer.ItemInUI().TryGetEnchantedItem(out EnchantedItem iGlobal)) {
                        for (int i = 0; i < EnchantingTable.maxEnchantments; i++) {
                            if (iGlobal.enchantments[i] != null) {//For each enchantment in the global item,
                                wePlayer.EnchantmentUISlot(i).Item = iGlobal.enchantments[i].Clone();//copy enchantments to the enchantmentSlots
                                wePlayer.enchantmentInEnchantingTable[i] = wePlayer.EnchantmentsModItem(i) != null;//Set PREVIOUS state of enchantmentSlot to has an item in it(true)
                                iGlobal.enchantments[i] = wePlayer.EnchantmentUISlot(i).Item;//Force link to enchantmentSlot just in case
                            }

                            iGlobal.enchantments[i] = wePlayer.EnchantmentUISlot(i).Item;//Link global item to the enchantmentSlots
                        }
                    }
                }

                itemInUI = wePlayer.ItemInUI();
                //Check if enchantments are added/removed from enchantmentSlots and re-link global item to enchantmentSlot
                for (int i = 0; i < EnchantingTable.maxEnchantments; i++) {
                    Item tableEnchantment = wePlayer.EnchantmentInUI(i);
                    Item itemEnchantment = new Item();
                    if (itemInUI.TryGetEnchantedItem(out EnchantedItem iGlobal)) {
                        itemEnchantment = iGlobal.enchantments[i];
                    }

                    if (tableEnchantment.IsAir) {
                        if (wePlayer.enchantmentInEnchantingTable[i]) {//if enchantmentSlot HAD an enchantment in it but it was just taken out,
                            //Force global item to re-link to the enchantmentSlot instead of following the enchantment just taken out
                            EnchantedItemStaticMethods.RemoveEnchantment(i);
                            //((Enchantment)itemEnchantment.ModItem).statsSet = false;
                            iGlobal.enchantments[i] = wePlayer.EnchantmentUISlot(i).Item;
                        }

                        wePlayer.enchantmentInEnchantingTable[i] = false;//Set PREVIOUS state of enchantmentSlot to empty(false)
                    }
                    else if (!itemEnchantment.IsAir && itemEnchantment.type != tableEnchantment.type) {
                        //If player swapped enchantments (without removing the previous one in the enchantmentSlot) Force global item to re-link to the enchantmentSlot instead of following the enchantment just taken out
                        EnchantedItemStaticMethods.RemoveEnchantment(i);
                        iGlobal.enchantments[i] = wePlayer.EnchantmentUISlot(i).Item;
                        EnchantedItemStaticMethods.ApplyEnchantment(i);
                    }
                    else if (!wePlayer.enchantmentInEnchantingTable[i]) {
                        //If it WAS empty but isn't now, re-link global item to enchantmentSlot just in case
                        wePlayer.enchantmentInEnchantingTable[i] = true;//Set PREVIOUS state of enchantmentSlot to has an item in it(true)
                        iGlobal.enchantments[i] = wePlayer.EnchantmentUISlot(i).Item;//Force link to enchantmentSlot just in case
                        EnchantedItemStaticMethods.ApplyEnchantment(i);
                    }
                }

                //If player is too far away, close the enchantment table
                if (!wePlayer.Player.IsInInteractionRangeToMultiTileHitbox(wePlayer.Player.chestX, wePlayer.Player.chestY) || wePlayer.Player.chest != -1 || !Main.playerInventory)
                    CloseWeaponEnchantmentUI();

                //Update cursor override
                if (ItemSlot.ShiftInUse) {
                    bool stop = false;
                    bool valid = false;
                    if (Main.mouseItem.IsAir && !Main.HoverItem.IsAir) {
                        for (int j = 0; j < EnchantingTable.maxItems; j++) {
                            if (wePlayer.enchantingTableUI.itemSlotUI[j].contains) {
                                stop = true;
                            }
                        }

                        for (int j = 0; j < EnchantingTable.maxEnchantments && !stop; j++) {
                            if (wePlayer.enchantingTableUI.enchantmentSlotUI[j].contains) {
                                stop = true;
                            }
                        }

                        for (int j = 0; j < EnchantingTable.maxEssenceItems && !stop; j++) {
                            if (wePlayer.enchantingTableUI.essenceSlotUI[j].contains) {
                                stop = true;
                            }
                        }

                        if (!stop)
                            valid = wePlayer.CheckShiftClickValid(ref Main.HoverItem);
                    }

                    if(!stop) {
						if (!valid || Main.cursorOverride == 6) {
                            Main.cursorOverride = -1;
                        }
					}
                }
            }
            
            //Fix for splitting stack of enchanted items in a chest
            if (wePlayer.Player.chest != -1) {
                int chest = wePlayer.Player.chest;
                if (Main.HoverItem.TryGetEnchantedItem()) {
                    Player player = wePlayer.Player;
                    Item[] inventory;
                    switch (chest) {
                        case -2:
                            inventory = player.bank.item;
                            break;
                        case -3:
                            inventory = player.bank2.item;
                            break;
                        case -4:
                            inventory = player.bank3.item;
                            break;
                        case -5:
                            inventory = player.bank4.item;
                            break;
                        default:
                            if (chest > -1) {
                                //Chest
                                inventory = Main.chest[chest].item;
                            }
                            else {
                                inventory = new Item[] { };
                            }

                            break;
                    }

                    for (int i = 0; i < inventory.Length; i++) {
                        ref Item item = ref inventory[i];
                        if (item.TryGetEnchantedItem(out EnchantedItem iGlobal)) {
                            if (item.IsSameEnchantedItem(Main.HoverItem) && item.stack == Main.HoverItem.stack) {
                                int stack = item.stack;
                                if (Main.mouseRight) {
                                    Item mouseItem = Main.mouseItem;
                                    int mouseItemStack = mouseItem.stack;
                                    int maxStack = mouseItem.maxStack;
                                    if (stack + mouseItemStack < maxStack) {
                                        Main.mouseItem.stack += stack;
                                        item = new Item();
                                    }
									else {
                                        Main.mouseItem.stack = maxStack;
                                        item.stack = stack + mouseItemStack - maxStack;
									}
                                }

                                break;
                            }
                        }
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

            //Fargos pirates that steal items
            if(stolenItemToBeCleared != -1 && Main.netMode != NetmodeID.MultiplayerClient) {
                Item itemToClear = Main.item[stolenItemToBeCleared];
                if (itemToClear != null && itemToClear.TryGetEnchantedItem(out EnchantedItem iGlobal)) {
                    iGlobal.lastValueBonus = 0;
                    iGlobal.prefix = -1;
                }

                stolenItemToBeCleared = -1;
            }

            //Player swapper
            if(WEMod.playerSwapperModEnabled && Main.netMode != NetmodeID.Server) {
                string playerName = wePlayer.Player.name;
                if (!updatedPlayerNames.Contains(playerName)) {
                    OldItemManager.ReplaceAllPlayerOldItems(wePlayer.Player);
                    updatedPlayerNames.Add(playerName);
                }
            }
        }
        public static void RemoveTableItem(WEPlayer wePlayer) {
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++) {
                Item enchantmentInUI = wePlayer.EnchantmentInUI(i);
                //For each enchantment in the enchantmentSlots,
                if (enchantmentInUI != null) {
                    if (wePlayer.itemBeingEnchanted.TryGetEnchantedItem(out EnchantedItem iGlobal))
                        iGlobal.enchantments[i] = enchantmentInUI.Clone();//copy enchantments to the global item
                }

                wePlayer.EnchantmentUISlot(i).Item = new Item();//Delete enchantments still in enchantmentSlots(There were transfered to the global item)
                wePlayer.enchantmentInEnchantingTable[i] = false;//The enchantmentSlot's PREVIOUS state is now empty(false)
            }

            if (wePlayer.infusionConsumeItem != null) {
                if (!wePlayer.infusionConsumeItem.IsSameEnchantedItem(wePlayer.itemBeingEnchanted))
                    wePlayer.itemBeingEnchanted.TryInfuseItem(wePlayer.previousInfusedItemName, true);

                wePlayer.enchantingTableUI.infusionButonText.SetText("Cancel");
            }

            if (wePlayer.itemBeingEnchanted.TryGetEnchantedItem(out EnchantedItem iBEGlobal))
                iBEGlobal.inEnchantingTable = false;

            wePlayer.itemBeingEnchanted.favorited = favorited;
            wePlayer.itemBeingEnchanted = wePlayer.enchantingTableUI.itemSlotUI[0].Item;//Stop tracking the item that just left the itemSlot
        }
        public static void CloseWeaponEnchantmentUI(bool noSound = false) {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            Item itemInUI = wePlayer.ItemInUI();
            if(itemInUI != null && !itemInUI.IsAir) {
                //Give item in table back to player
                wePlayer.ItemUISlot().Item = wePlayer.Player.GetItem(Main.myPlayer, itemInUI, GetItemSettings.LootAllSettings);

                //Clear item and enchantments from table
                itemInUI = wePlayer.ItemInUI();
                if (itemInUI.IsAir) {
                    RemoveTableItem(wePlayer);

                    wePlayer.enchantingTable.item[0] = new Item();
                    for(int i = 0; i < EnchantingTable.maxEnchantments; i++) {
                        wePlayer.enchantmentInEnchantingTable[i] = false;
                        wePlayer.enchantingTable.enchantmentItem[i] = new Item();
                        wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item = new Item();
                    }
                }
            }

            wePlayer.itemBeingEnchanted = null;
            wePlayer.itemInEnchantingTable = false;
            wePlayer.usingEnchantingTable = false;
            if(wePlayer.Player.chest == -1) {
                if (!noSound)
                    SoundEngine.PlaySound(SoundID.MenuClose);
            }

            weModSystemUI.SetState(null);
            promptInterface.SetState(null);

            ItemSlot.Options.DisableLeftShiftTrashCan = wePlayer.disableLeftShiftTrashCan;
        }
        public static void OpenWeaponEnchantmentUI(bool noSound = false) {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            wePlayer.usingEnchantingTable = true;
            if(!noSound)
                SoundEngine.PlaySound(SoundID.MenuOpen);

            if (wePlayer.enchantingTableTier > 0) {
                QuickStackEssence();
            }

            UIState state = new UIState();
            state.Append(wePlayer.enchantingTableUI);
            weModSystemUI.SetState(state);
        }
        public static void QuickStackEssence() {
            bool transfered = false;
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            for (int j = 0; j < 50; j++) {
                if (wePlayer.Player.inventory[j].TryGetEnchantmentEssence(out EnchantmentEssence essence)) {
                    int tier = essence.essenceTier;
                    int ammountToTransfer;
                    int startingStack = wePlayer.Player.inventory[j].stack;
                    if (wePlayer.enchantingTable.essenceItem[tier].IsAir) {
                        wePlayer.enchantingTable.essenceItem[tier] = wePlayer.Player.inventory[j].Clone();
                        wePlayer.Player.inventory[j] = new Item();
                        transfered = true;
                    }
                    else {
                        if(wePlayer.enchantingTable.essenceItem[tier].stack < EnchantmentEssence.maxStack) {
                            if (wePlayer.Player.inventory[j].stack + wePlayer.enchantingTable.essenceItem[tier].stack > EnchantmentEssence.maxStack) {
                                ammountToTransfer = EnchantmentEssence.maxStack - wePlayer.enchantingTable.essenceItem[tier].stack;
                            }
                            else {
                                ammountToTransfer = wePlayer.Player.inventory[j].stack;
                            }

                            wePlayer.enchantingTable.essenceItem[tier].stack += ammountToTransfer;
                            wePlayer.Player.inventory[j].stack -= ammountToTransfer;
                            transfered = true;
                        }
                    }

                    if(wePlayer.Player.inventory[j].stack == startingStack)
                        transfered = false;
                }
            }
            if (transfered)
                SoundEngine.PlaySound(SoundID.Grab);
        }
        public static bool AutoCraftEssence() {
            bool crafted = false;
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            for (int i = EnchantingTable.maxEssenceItems - 1; i > 0; i--) {
                if(wePlayer.enchantingTableUI.essenceSlotUI[i].Item.stack < EnchantmentEssence.maxStack) {
                    int ammountToTransfer;
                    if(wePlayer.enchantingTableUI.essenceSlotUI[i].Item.stack == 0 || (EnchantmentEssence.maxStack > wePlayer.enchantingTableUI.essenceSlotUI[i].Item.stack + (wePlayer.enchantingTableUI.essenceSlotUI[i - 1].Item.stack / 4))) {
                        ammountToTransfer = wePlayer.enchantingTableUI.essenceSlotUI[i - 1].Item.stack / 4;
                    }
                    else {
                        ammountToTransfer = EnchantmentEssence.maxStack - wePlayer.enchantingTableUI.essenceSlotUI[i].Item.stack;
                    }

                    if(ammountToTransfer > 0) {
                        wePlayer.enchantingTableUI.essenceSlotUI[i].Item.stack += ammountToTransfer;
                        wePlayer.enchantingTableUI.essenceSlotUI[i - 1].Item.stack -= ammountToTransfer * 4;
                        crafted = true;
                    }
                }
            }

            for (int i = 1; i < EnchantingTable.maxEssenceItems; i++) {
                if (wePlayer.enchantingTableUI.essenceSlotUI[i].Item.stack < EnchantmentEssence.maxStack) {
                    int ammountToTransfer;
                    if (wePlayer.enchantingTableUI.essenceSlotUI[i].Item.stack == 0 || (EnchantmentEssence.maxStack > wePlayer.enchantingTableUI.essenceSlotUI[i].Item.stack + (wePlayer.enchantingTableUI.essenceSlotUI[i - 1].Item.stack / 4))) {
                        ammountToTransfer = wePlayer.enchantingTableUI.essenceSlotUI[i - 1].Item.stack / 4;
                    }
                    else {
                        ammountToTransfer = EnchantmentEssence.maxStack - wePlayer.enchantingTableUI.essenceSlotUI[i].Item.stack;
                    }

                    if (ammountToTransfer > 0) {
                        wePlayer.enchantingTableUI.essenceSlotUI[i].Item.stack += ammountToTransfer;
                        wePlayer.enchantingTableUI.essenceSlotUI[i - 1].Item.stack -= ammountToTransfer * 4;
                        crafted = true;
                    }
                }
            }

            return crafted;
        }
        public override void PreSaveAndQuit() {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            weModSystemUI.SetState(null);
            promptInterface.SetState(null);
            if (wePlayer.usingEnchantingTable) {
                CloseWeaponEnchantmentUI();
                wePlayer.enchantingTableUI.OnDeactivate();
            }
        }
        public override void UpdateUI(GameTime gameTime) {
            _lastUpdateUiGameTime = gameTime;
            if(weModSystemUI?.CurrentState != null) {
                weModSystemUI.Update(gameTime);
            }

            if(PromptInterfaceActive)
                promptInterface.Update(gameTime);
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
            int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Over"));
            if (index != -1) {
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

            index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (index != -1) {
                layers.Insert(index, new LegacyGameInterfaceLayer(
                    "WeaponEnchantments: WeaponEnchantmentsUI",
                    delegate {
                        if (_lastUpdateUiGameTime != null && weModSystemUI?.CurrentState != null) {
                            weModSystemUI.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        }

                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }

            index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (index != -1) {
                layers.Insert(index, new LegacyGameInterfaceLayer(
                    "WeaponEnchantments: PromptUI",
                    delegate {
                        if (_lastUpdateUiGameTime != null && PromptInterfaceActive)
                            promptInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);

                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
        public override void AddRecipeGroups() {
            RecipeGroup group = new RecipeGroup(() => "Any Common Gem", new int[] {
                ItemID.Sapphire, 
                ItemID.Ruby, 
                ItemID.Emerald, 
                ItemID.Topaz, 
                ItemID.Amethyst
            });
            RecipeGroup.RegisterGroup("WeaponEnchantments:CommonGems", group);

            group = new RecipeGroup(() => "Any Rare Gem", new int[] {
                ItemID.Amber, 
                ItemID.Diamond
            });
            RecipeGroup.RegisterGroup("WeaponEnchantments:RareGems", group);

            group = new RecipeGroup(() => "Workbenches", new int[] {
                ItemID.WorkBench, 
                ItemID.BambooWorkbench, 
                ItemID.BlueDungeonWorkBench, 
                ItemID.BoneWorkBench, 
                ItemID.BorealWoodWorkBench, 
                ItemID.CactusWorkBench, 
                ItemID.CrystalWorkbench, 
                ItemID.DynastyWorkBench, 
                ItemID.EbonwoodWorkBench, 
                ItemID.FleshWorkBench, 
                ItemID.FrozenWorkBench, 
                ItemID.GlassWorkBench, 
                ItemID.GoldenWorkbench,
                ItemID.GothicWorkBench, 
                ItemID.GraniteWorkBench, 
                ItemID.GreenDungeonWorkBench, 
                ItemID.HoneyWorkBench, 
                ItemID.LesionWorkbench, 
                ItemID.LihzahrdWorkBench, 
                ItemID.LivingWoodWorkBench, 
                ItemID.MarbleWorkBench, 
                ItemID.MartianWorkBench, 
                ItemID.MeteoriteWorkBench, 
                ItemID.MushroomWorkBench, 
                ItemID.NebulaWorkbench, 
                ItemID.ObsidianWorkBench, 
                ItemID.PalmWoodWorkBench, 
                ItemID.PearlwoodWorkBench, 
                ItemID.PinkDungeonWorkBench, 
                ItemID.PumpkinWorkBench, 
                ItemID.RichMahoganyWorkBench, 
                ItemID.SandstoneWorkbench, 
                ItemID.ShadewoodWorkBench, 
                ItemID.SkywareWorkbench, 
                ItemID.SlimeWorkBench, 
                ItemID.SolarWorkbench, 
                ItemID.SpiderWorkbench, 
                ItemID.SpookyWorkBench, 
                ItemID.StardustWorkbench, 
                ItemID.SteampunkWorkBench, 
                ItemID.VortexWorkbench
            });
            RecipeGroup.RegisterGroup("WeaponEnchantments:Workbenches", group);

            group = new RecipeGroup(() => "Any Aligned Soul", new int[] {
                ItemID.SoulofLight, 
                ItemID.SoulofNight
            });
            RecipeGroup.RegisterGroup("WeaponEnchantments:AlignedSoul", group);
        }
		public override void PostWorldGen() {
            for (int chestIndex = 0; chestIndex < 1000; chestIndex++) {
                Chest chest = Main.chest[chestIndex];
                if(chest == null)
                    continue;

                float chance = ChestSpawnChance;
                int itemsPlaced = 0;
                List<int> itemTypes = new List<int>();

                // If you look at the sprite for Chests by extracting Tiles_21.xnb, you'll see that the 12th chest is the Ice Chest.
                // Since we are counting from 0, this is where 11 comes from. 36 comes from the width of each tile including padding.
                switch (Main.tile[chest.x, chest.y].TileType) {
                    case 21:
                    case 441:
                        switch (Main.tile[chest.x, chest.y].TileFrameX / 36) {
                            case 0://Chest
                                chance *= 0.7f;
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
                                chance *= 2f;
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
                                chance *= 2f;
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
                        switch (Main.tile[chest.x, chest.y].TileFrameX / 36) {
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

                if (chance <= 0f)
                    continue;

                for (int j = 0; j < 40 && itemsPlaced < chance; j++) {
                    if (chest.item[j].type != ItemID.None)
                        continue;

                    int type = itemTypes.GetOneFromList(chance);
                    if(type > 0)
                        chest.item[j] = new Item(type);

                    itemsPlaced++;
                }
            }
        }
    }
}