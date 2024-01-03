using androLib.UI;
using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;
using Terraria.UI.Gamepad;
using WeaponEnchantments.Common.Configs;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Content.NPCs;
using static System.Net.Mime.MediaTypeNames;
using androLib;
using WeaponEnchantments.Items;
using androLib.Common.Utility;

namespace WeaponEnchantments.UI
{
	public static class UIManager {
		public static byte UIAlpha = byte.MaxValue;
		public static byte UIAlphaHovered = byte.MaxValue;

		public static bool HoveringWitchReroll => MasterUIManager.HoveringMyUIType(WE_UI_ID.Witch_UITypeID);
		public static bool HoveringEnchantmentStorage => MasterUIManager.HoveringMyUIType(WE_UI_ID.EnchantmentStorage_UITypeID);
		public static bool HoveringOreBag => MasterUIManager.HoveringMyUIType(WE_UI_ID.OreBag_UITypeID);
		public static bool HoveringEnchantmentLoadoutUI => MasterUIManager.HoveringMyUIType(WE_UI_ID.EnchantmentLoadout_UITypeID);
		public static bool HoveringEnchantingTable => MasterUIManager.HoveringMyUIType(WE_UI_ID.EnchantingTable_UITypeID);

		public static void RegisterWithMaster() {
			WE_UI_ID.Witch_UITypeID = MasterUIManager.RegisterUI_ID();
			//WE_UI_ID.OfferUI_ID = MasterUIManager.RegisterUI_ID(true);//Is this one needed?
			WE_UI_ID.EnchantingTable_UITypeID = MasterUIManager.RegisterUI_ID();
			WE_UI_ID.EnchantmentStorage_UITypeID = MasterUIManager.RegisterUI_ID();
			WE_UI_ID.EnchantmentLoadout_UITypeID = MasterUIManager.RegisterUI_ID();

			MasterUIManager.IsDisplayingUI.Add(() => Witch.rerollUI);
			MasterUIManager.IsDisplayingUI.Add(() => WEPlayer.LocalWEPlayer.usingEnchantingTable);
			MasterUIManager.IsDisplayingUI.Add(() => WEPlayer.LocalWEPlayer.displayEnchantmentStorage);
			MasterUIManager.IsDisplayingUI.Add(() => WEPlayer.LocalWEPlayer.displayEnchantmentLoadoutUI);

			MasterUIManager.ShouldPreventTrashingItem.Add(() => WEPlayer.LocalWEPlayer.usingEnchantingTable);
			MasterUIManager.ShouldPreventTrashingItem.Add(() => WEPlayer.LocalWEPlayer.displayOreBagUI);

			MasterUIManager.DrawAllInterfaces += EnchantmentLoadoutUI.PostDrawInterface;
			MasterUIManager.DrawAllInterfaces += EnchantingTableUI.PostDrawInterface;
			MasterUIManager.DrawAllInterfaces += EnchantmentStorage.PostDrawInterface;
			MasterUIManager.DrawAllInterfaces += WitchRerollUI.PostDrawInterface;

			//MasterUIManager.UpdateInterfaces += EnchantmentLoadoutUI.UpdateInterface;
			//MasterUIManager.UpdateInterfaces += EnchantingTableUI.UpdateInterface;
			//MasterUIManager.UpdateInterfaces += EnchantmentStorage.UpdateInterface;
			MasterUIManager.UpdateInterfaces += WitchRerollUI.UpdateInterface;

			MasterUIManager.ShouldPreventRecipeScrolling.Add(() => MasterUIManager.HoveringMyUIType(WE_UI_ID.EnchantmentStorage_UITypeID));
			MasterUIManager.ShouldPreventRecipeScrolling.Add(() => MasterUIManager.HoveringMyUIType(WE_UI_ID.EnchantmentLoadout_UITypeID) && EnchantmentLoadoutUI.useingScrollBar);

			StorageManager.CanVacuumItemHandler.Add(EnchantmentStorage.CanVacuumItem);
			StorageManager.CanVacuumItemHandler.Add(EnchantingTableUI.CanVacuumItem);
			StorageManager.CanVacuumItemHandler.Add(EnchantmentStorage.CanAutoOfferItem);

			StorageManager.TryVacuumItemHandler.Add((Item item, Player player) => EnchantmentStorage.TryVacuumItem(ref item, player));
			StorageManager.TryVacuumItemHandler.Add((Item item, Player player) => EnchantingTableUI.TryVacuumItem(ref item, player));
			StorageManager.TryVacuumItemHandler.Add((Item item, Player player) => EnchantmentStorage.TryAutoOfferItem(ref item, player));

			StorageManager.TryRestockItemHandler.Add((Item item) => EnchantmentStorage.Restock(ref item));
			StorageManager.TryRestockItemHandler.Add((Item item) => EnchantingTableUI.Restock(ref item));

			StorageManager.TryQuickStackItemHandler.Add((Item item, Player player) => EnchantmentStorage.QuickStack(ref item, player));
			StorageManager.TryQuickStackItemHandler.Add((Item item, Player player) => EnchantingTableUI.QuickStack(ref item, player));

			StorageManager.OnOpenMagicStorageCloseAllStorageUIEvent += () => {
				if (WEPlayer.LocalWEPlayer.usingEnchantingTable)
					EnchantingTableUI.CloseEnchantingTableUI(true);
			};
			StorageManager.OnOpenMagicStorageCloseAllStorageUIEvent += () => {
				if (WEPlayer.LocalWEPlayer.displayEnchantmentLoadoutUI)
					EnchantmentLoadoutUI.Close(true);
			};

			StorageManager.MagicStorageItemsHandler.Add(() => WEPlayer.LocalWEPlayer.enchantmentStorageItems);
			StorageManager.MagicStorageItemsHandler.Add(() => WEPlayer.LocalWEPlayer.enchantingTableEssence);

			if (AndroMod.vacuumBagsEnabled)
				AndroMod.vacuumBagsMod.Call("RegisterAllBagsWithAndroLib");
		}
		public static void OnUpdateUIAlpha() {
			UIAlpha = androLib.Common.Configs.ConfigValues.UIAlpha;
			UIAlphaHovered = (byte)Math.Min(UIAlpha + 20, byte.MaxValue);
		}
		public static void ItemSlotClickInteractions(EnchantmentsArray enchantmentsArray, int index, int context) {
			Item enchantmentItem = enchantmentsArray[index];
			if (enchantmentItem.NullOrAir() && Main.mouseItem.NullOrAir())
				return;

			WEPlayer.LocalWEPlayer.TryHandleEnchantmentRemoval(index, enchantmentsArray, returnFunc: (Item itemToReturn, Player player, bool quickSpawnAllowed) => {
				MasterUIManager.ItemSlotClickInteractions(ref itemToReturn, context);
				return new ItemAndBool(ref itemToReturn);
			});
		}
		public static void SwapMouseItem(EnchantmentsArray enchantmentsArray, int index) {
			WEPlayer.LocalWEPlayer.TryHandleEnchantmentRemoval(index, enchantmentsArray, returnFunc: (Item itemToReturn, Player player, bool quickSpawnAllowed) => {
				MasterUIManager.SwapMouseItem(ref itemToReturn);
				return new ItemAndBool(ref itemToReturn);
			});
		}
	}
	public struct ItemAndBool {
		public ItemAndBool(ref Item item, bool result = true) {
			Item = item;
			Result = result;
		}

		public Item Item;
		public bool Result;
	}
	public static class WE_UI_ID {
		public const int None = UI_ID.None;

		public static int Witch_UITypeID;//Set by MasterUIManager

		public const int WitchReroll = 0;


		//public static int OfferUI_ID;//Set by MasterUIManager//Is this one needed?

		public const int Offer = 1;
		public const int OfferYes = 2;
		public const int OfferNo = 3;
		public const int ToggleAutoTrashOfferedItems = 4;


		public static int EnchantmentStorage_UITypeID;//Set by MasterUIManager

		public const int EnchantmentStorage = 0;
		public const int EnchantmentStorageScrollBar = 5;
		public const int EnchantmentStorageScrollPanel = 6;
		public const int EnchantmentStorageSearch = 7;
		public const int EnchantmentStorageLootAll = 100;
		public const int EnchantmentStorageDepositAll = 101;
		public const int EnchantmentStorageQuickStack = 102;
		public const int EnchantmentStorageSort = 103;
		public const int EnchantmentStorageToggleVacuum = 104;
		public const int EnchantmentStorageToggleMarkTrash = 105;
		public const int EnchantmentStorageUncraftAllTrash = 106;
		public const int EnchantmentStorageRevertAllToBasic = 107;
		public const int EnchantmentStorageManageTrash = 108;
		public const int EnchantmentStorageManageOfferedItems = 109;
		public const int EnchantmentstorageQuickCrafting = 110;
		public const int EnchantmentStorageItemSlot = 200;


		public static int EnchantingTable_UITypeID;//Set by MasterUIManager

		public const int EnchantingTable = 0;
		public const int EnchantingTableLootAll = 1;
		public const int EnchantingTableOfferButton = 2;
		public const int EnchantingTableSiphon = 3;
		public const int EnchantingTableInfusion = 4;
		public const int EnchantingTableLevelUp = 5;
		public const int EnchantingTableItemSlot = 6;
		public const int EnchantingTableStorageButton = 7;
		public const int EnchantingTableLoadoutsButton = 8;
		public const int EnchantingTableEnchantment0 = 200;
		public const int EnchantingTableEnchantmentLast = EnchantingTableEnchantment0 + EnchantingTableUI.MaxEnchantmentSlots - 1;
		public const int EnchantingTableEssence0 = 300;
		public const int EnchantingTableEssenceLast = EnchantingTableEssence0 + EnchantingTableUI.MaxEssenceSlots - 1;
		public const int EnchantingTableLevelsPerLevelUp0 = 404;
		public const int EnchantingTableLevelsPerLevelUpLast = 407;
		public const int EnchantingTableXPButton0 = 500;
		public const int EnchantingTableXPButtonLast = EnchantingTableXPButton0 + EnchantingTableUI.MaxEssenceSlots - 1;

		public static int OreBag_UITypeID;//Set by MasterUIManager

		public const int OreBag = 0;
		public const int OreBagScrollBar = 1;
		public const int OreBagScrollPanel = 2;
		public const int OreBagSearch = 3;
		public const int OreBagLootAll = 100;
		public const int OreBagDepositAll = 101;
		public const int OreBagQuickStack = 102;
		public const int OreBagSort = 103;
		public const int OreBagToggleVacuum = 104;
		public const int OreBagItemSlot = 200;


		public static int EnchantmentLoadout_UITypeID;//Set by MasterUIManager

		public const int EnchantmentLoadoutUI = 0;
		public const int EnchantmentLoadoutUIScrollBar = 1;
		public const int EnchantmentLoadoutUIScrollPanel = 2;
		public const int EnchantmentLoadoutUITextButton = 3;
		public const int EnchantmentLoadoutAddTextButton = 4;
		public const int EnchantmentLoadoutAddFromEquippedEnchantmentsTextButton = 5;
		public const int EnchantmentLoadoutUIItemSlot = 200;
	}
}
