using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.UI;
using Terraria;
using WeaponEnchantments.Items;
using WeaponEnchantments.Common.Utility;
using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using static System.Net.Mime.MediaTypeNames;
using Terraria.UI.Chat;
using Microsoft.CodeAnalysis;
using Terraria.ID;
using Terraria.Audio;
using WeaponEnchantments.Common;
using System.Reflection;
using WeaponEnchantments.Effects;
using WeaponEnchantments.Common.Configs;
using WeaponEnchantments.Common.Globals;
using Microsoft.Xna.Framework.Input;
using Terraria.GameContent.UI.States;
using Terraria.Localization;

namespace WeaponEnchantments.UI
{
	/*
	public static class EnchantmentLoadoutUI
	{
		public class EnchantmentLoadoutUIButtonID
		{
			public const int LootAll = 0;
			public const int DepositAll = 1;
			public const int QuickStack = 2;
			public const int Sort = 3;
			public const int ToggleVacuum = 4;
			public const int ToggleMarkTrash = 5;
			public const int UncraftAllTrash = 6;
			public const int RevertAllToBasic = 7;
			public const int ManageTrash = 8;
			public const int ManageOfferedItems = 9;
			public const int QuickCrafting = 10;
			public const int Count = 11;
		}
		public static int ID => UI_ID.EnchantmentLoadoutUI;
		public static int EnchantmentLoadoutUIDefaultLeft => 680;
		public static int EnchantmentLoadoutUIDefaultTop => 90;
		public static Color PanelColor => new Color(26, 2, 56, 100);
		private static int Spacing => 4;
		private static int PanelBorder => 10;
		public const float buttonScaleMinimum = 0.75f;
		public const float buttonScaleMaximum = 1f;
		private static float[,] buttonScale = null;
		public static float[,] ButtonScale {
			get {
				if (buttonScale == null)
					buttonScale = FillArray(buttonScaleMinimum, WEPlayer.LocalWEPlayer.enchantmentLoadouts.Count, buttonNames.Length);

				return buttonScale;
			}
		}
		public static T[,] FillArray<T>(T value, int xLen, int yLen) {
			T[,] array = new T[xLen, yLen];
			for (int x = 0; x < xLen; x++) {
				for (int y = 0; y < yLen; y++) {
					array[x, y] = value;
				}
			}

			return array;
		}
		private static int scrollPanelY = int.MinValue;
		private static int scrollPanelPosition = 0;
		private static int displayedLoadout = 0;
		public const int MaxLoadouts = 20;
		public static string[] buttonNames = { EnchantmentStorageTextID.Edit.ToString().Lang(L_ID1.EnchantmentStorageText), EnchantmentStorageTextID.Weapon.ToString().Lang(L_ID1.EnchantmentStorageText), EItemType.Armor.ToString().Lang(L_ID1.Tooltip, L_ID2.ItemType), EItemType.Accessories.ToString().Lang(L_ID1.Tooltip, L_ID2.ItemType) };
		public static void PostDrawInterface(SpriteBatch spriteBatch) {
			WEPlayer wePlayer = WEPlayer.LocalWEPlayer;
			if (wePlayer.displayEnchantmentLoadoutUI) {

				#region Data

				Color mouseColor = UIManager.MouseColor;
				//ItemSlots Data 1/2
				SortedDictionary<int, int> availableEnchantmentRecipes = new();
				SortedDictionary<int, int> quickCraftItemCounts = new();
				List<Item[]> inventory = wePlayer.enchantmentLoadouts[displayedLoadout];

				int itemSlotColumns = EnchantingTableUI.MaxEnchantmentSlots;
				int itemSlotRowsDisplayed = 10;
				int itemSlotTotalRows = inventory.Count;
				int itemSlotSpaceWidth = UIManager.ItemSlotSize + Spacing;
				int itemSlotSpaceHeight = UIManager.ItemSlotSize + Spacing;
				int itemSlotsWidth = (itemSlotColumns - 1) * itemSlotSpaceWidth + UIManager.ItemSlotSize;
				int itemSlotsHeight = (itemSlotRowsDisplayed - 1) * itemSlotSpaceHeight + UIManager.ItemSlotSize;
				int itemSlotsLeft = wePlayer.EnchantmentLoadoutUILeft + PanelBorder;

				//Name Data
				int nameLeft = itemSlotsLeft;//itemSlotsLeft + (itemSlotsWidth - nameWidth) / 2;
				int nameTop = wePlayer.EnchantmentLoadoutUITop + PanelBorder;
				string name = EnchantmentStorageTextID.EnchantmentLoadouts.ToString().Lang(L_ID1.EnchantmentStorageText);
				UITextData nameData = new(UI_ID.None, nameLeft, nameTop, name, 1f, mouseColor);

				//Panel Data 1/2
				int panelBorderTop = nameData.Height + Spacing + 2;

				//ItemSlots Data 2/2
				int itemSlotsTop = wePlayer.EnchantmentLoadoutUITop + panelBorderTop;

				//Scroll Bar Data 1/2
				int scrollBarLeft = itemSlotsLeft + itemSlotsWidth + Spacing;
				int scrollBarWidth = 30;

				//Text buttons Data
				int buttonsLeft = scrollBarLeft + scrollBarWidth + Spacing;
				int currentButtonTop = nameTop;
				UITextData[,] textButtons = new UITextData[wePlayer.enchantmentLoadouts.Count, buttonNames.Length];
				int longestButtonNameWidth = 0;
				for (int buttonRow = 0; buttonRow < wePlayer.enchantmentLoadouts.Count; buttonRow++) {
					for (int buttonIndex = 0; buttonIndex < buttonNames.Length; buttonIndex++) {
						string text = buttonNames[buttonIndex];
						float scale = ButtonScale[buttonRow, buttonIndex];
						Color color = mouseColor;
						UITextData thisButton = new(UI_ID.EnchantmentLoadoutUITextButton, buttonsLeft, currentButtonTop, text, scale, color, ancorBotomLeft: true);
						textButtons[buttonRow, buttonIndex] = thisButton;
						longestButtonNameWidth = Math.Max(longestButtonNameWidth, thisButton.Width);
						currentButtonTop += (int)(thisButton.BaseHeight);
					}
				}

				//Panel Data 2/2
				int panelBorderRightOffset = Spacing + longestButtonNameWidth + PanelBorder;
				int panelWidth = itemSlotsWidth + Spacing + scrollBarWidth + PanelBorder + panelBorderRightOffset;
				int panelHeight = itemSlotsHeight + PanelBorder + panelBorderTop;
				UIPanelData panel = new(ID, wePlayer.EnchantmentLoadoutUILeft, wePlayer.EnchantmentLoadoutUITop, panelWidth, panelHeight, PanelColor);

				//Scroll Bar Data 2/2
				int scrollBarTop = wePlayer.EnchantmentLoadoutUITop + PanelBorder;
				UIPanelData scrollBarData = new(UI_ID.EnchantmentLoadoutUIScrollBar, scrollBarLeft, scrollBarTop, scrollBarWidth, panelHeight - PanelBorder * 2, new Color(10, 1, 30, 100));

				//Scroll Panel Data 1/2
				int scrollPanelXOffset = 1;
				int scrollPanelSize = scrollBarWidth - scrollPanelXOffset * 2;
				int scrollPanelMinY = scrollBarData.TopLeft.Y + scrollPanelXOffset;
				int scrollPanelMaxY = scrollBarData.BottomRight.Y - scrollPanelSize - scrollPanelXOffset;
				int possiblePanelPositions = itemSlotTotalRows - itemSlotRowsDisplayed;
				if (possiblePanelPositions < 1)
					possiblePanelPositions = 1;

				scrollPanelPosition.Clamp(0, possiblePanelPositions);

				#endregion

				//Panel Draw
				panel.Draw(spriteBatch);

				//Scroll Bar Draw
				scrollBarData.Draw(spriteBatch);

				//ItemSlots Draw
				int startRow = scrollPanelPosition;
				int inventoryIndexStart = startRow * itemSlotColumns;
				int slotsToDisplay = itemSlotRowsDisplayed * itemSlotColumns;
				int slotNum = 0;
				int itemSlotX = itemSlotsLeft;
				int itemSlotY = itemSlotsTop;
				bool quickCraftingFoundAll = false;
				for (int rowNum = startRow; rowNum < itemSlotTotalRows; rowNum++) {
					Item[] enchantmentSlots = inventory[rowNum];
					for (int enchantmentSlotIndex = 0; enchantmentSlotIndex < EnchantingTableUI.MaxEnchantmentSlots; enchantmentSlotIndex++) {
						ref Item item = ref enchantmentSlots[enchantmentSlotIndex];
						UIItemSlotData slotData = new(UI_ID.EnchantmentLoadoutUIItemSlot, itemSlotX, itemSlotY);
						string modFullName = item.type.GetItemIDOrName();
						if (slotData.MouseHovering()) {
							if (wePlayer.vacuumItemsIntoEnchantmentStorage && ItemSlot.ShiftInUse && EnchantmentStorage.CanBeStored(item)) {
								if (UIManager.LeftMouseClicked) {
									PopupText.NewText(PopupTextContext.RegularItemPickup, item, item.stack);
									EnchantmentStorage.DepositAll(ref item);
									SoundEngine.PlaySound(SoundID.Grab);
								}

								Main.cursorOverride = CursorOverrideID.InventoryToChest;
							}
							else if (Main.mouseItem.IsAir || Main.mouseItem.ModItem is Enchantment) {
								slotData.ClickInteractions(ref item);
							}
						}

						if (!item.IsAir && !item.favorited && item.TryGetGlobalItem(out VacuumToStorageItems vacummItem) && vacummItem.favorited)
							item.favorited = true;

						if (wePlayer.trashEnchantmentsFullNames.Contains(modFullName) && !item.favorited) {
							slotData.Draw(spriteBatch, item, ItemSlotContextID.MarkedTrash, glowHue, glowTime);
						}
						else {
							slotData.Draw(spriteBatch, item, ItemSlotContextID.Normal, glowHue, glowTime);
						}

						slotNum++;
						if (slotNum % itemSlotColumns == 0) {
							itemSlotX = itemSlotsLeft;
							itemSlotY += itemSlotSpaceHeight;
						}
						else {
							itemSlotX += itemSlotSpaceWidth;
						}
					}
				}

				//Name Draw
				nameData.Draw(spriteBatch);

				//Search Bar Draw
				searchBarData.Draw(spriteBatch);

				bool mouseHoveringSearchBar = searchBarData.MouseHovering();
				if (mouseHoveringSearchBar) {
					if (UIManager.LeftMouseClicked)
						UIManager.ClickSearchBar(SearchID);
				}

				if (UIManager.TypingOnSearchBar(SearchID)) {
					if (UIManager.LeftMouseClicked && !mouseHoveringSearchBar || Main.mouseRight || !Main.playerInventory) {
						UIManager.StopTypingOnSearchBar();
					}
					else {
						PlayerInput.WritingText = true;
						Main.instance.HandleIME();
						UIManager.SearchBarString = Main.GetInputText(UIManager.SearchBarString);
					}
				}

				//Text Buttons Draw
				for (int buttonIndex = 0; buttonIndex < EnchantmentLoadoutUIButtonID.Count; buttonIndex++) {
					UITextData textButton = textButtons[buttonIndex];
					textButton.Draw(spriteBatch);
					if (UIManager.MouseHovering(textButton, true)) {
						ButtonScale[buttonIndex] += 0.05f;

						if (ButtonScale[buttonIndex] > buttonScaleMaximum)
							ButtonScale[buttonIndex] = buttonScaleMaximum;

						if (UIManager.LeftMouseClicked) {
							if (managingTrash && buttonIndex != EnchantmentLoadoutUIButtonID.ManageTrash)
								managingTrash = false;

							if (managingOfferdItems && buttonIndex != EnchantmentLoadoutUIButtonID.ManageOfferedItems)
								managingOfferdItems = false;

							if (quickCrafting && buttonIndex != EnchantmentLoadoutUIButtonID.QuickCrafting)
								quickCrafting = false;

							switch (buttonIndex) {
								case EnchantmentLoadoutUIButtonID.LootAll:
									LootAll();
									break;
								case EnchantmentLoadoutUIButtonID.DepositAll:
									DepositAll(Main.LocalPlayer.inventory);
									break;
								case EnchantmentLoadoutUIButtonID.QuickStack:
									QuickStack(Main.LocalPlayer.inventory);
									break;
								case EnchantmentLoadoutUIButtonID.Sort:
									Sort();
									break;
								case EnchantmentLoadoutUIButtonID.ToggleVacuum:
									ToggleVacuum();
									break;
								case EnchantmentLoadoutUIButtonID.ToggleMarkTrash:
									ToggleMarkTrash();
									break;
								case EnchantmentLoadoutUIButtonID.UncraftAllTrash:
									UncraftTrash(wePlayer.EnchantmentLoadoutUIItems);
									break;
								case EnchantmentLoadoutUIButtonID.RevertAllToBasic:
									RevertAllToBasic();
									break;
								case EnchantmentLoadoutUIButtonID.ManageTrash:
									managingTrash = !managingTrash;
									break;
								case EnchantmentLoadoutUIButtonID.ManageOfferedItems:
									managingOfferdItems = !managingOfferdItems;
									break;
								case EnchantmentLoadoutUIButtonID.QuickCrafting:
									quickCrafting = !quickCrafting;
									break;
							}

							SoundEngine.PlaySound(SoundID.MenuTick);
						}
					}
					else {
						ButtonScale[buttonIndex] -= 0.05f;

						if (ButtonScale[buttonIndex] < buttonScaleMinimum)
							ButtonScale[buttonIndex] = buttonScaleMinimum;
					}
				}

				//Scroll Panel Data 2/2
				bool draggingScrollPanel = UIManager.PanelBeingDragged == UI_ID.EnchantmentLoadoutUIScrollPanel;
				if (UIManager.PanelBeingDragged == UI_ID.EnchantmentLoadoutUIScrollPanel) {
					scrollPanelY.Clamp(scrollPanelMinY, scrollPanelMaxY);
				}
				else {
					int scrollPanelRange = scrollPanelMaxY - scrollPanelMinY;
					int offset = scrollPanelPosition * scrollPanelRange / possiblePanelPositions;
					scrollPanelY = offset + scrollPanelMinY;
				}

				//Scroll Bar Hover
				if (scrollBarData.MouseHovering()) {
					if (UIManager.LeftMouseClicked) {
						UIManager.UIBeingHovered = UI_ID.None;
						scrollPanelY = Main.mouseY - scrollPanelSize / 2;
						scrollPanelY.Clamp(scrollPanelMinY, scrollPanelMaxY);
					}
				}

				//Scroll Panel Hover and Drag
				UIPanelData scrollPanelData = new(UI_ID.EnchantmentLoadoutUIScrollPanel, scrollBarLeft + scrollPanelXOffset, scrollPanelY, scrollPanelSize, scrollPanelSize, mouseColor);
				if (scrollPanelData.MouseHovering()) {
					scrollPanelData.TryStartDraggingUI();
				}

				if (scrollPanelData.ShouldDragUI()) {
					UIManager.DragUI(out _, out scrollPanelY);
				}
				else if (draggingScrollPanel) {
					int scrollPanelRange = scrollPanelMaxY - scrollPanelMinY;
					scrollPanelPosition = ((scrollPanelY - scrollPanelMinY) * possiblePanelPositions).RoundDivide(scrollPanelRange);
				}

				scrollPanelData.Draw(spriteBatch);

				//Panel Hover and Drag
				if (panel.MouseHovering()) {
					panel.TryStartDraggingUI();
				}

				if (panel.ShouldDragUI())
					UIManager.DragUI(out wePlayer.EnchantmentLoadoutUILeft, out wePlayer.EnchantmentLoadoutUITop);

				int scrollWheelTicks = UIManager.ScrollWheelTicks;
				if (scrollWheelTicks != 0 && UIManager.HoveringEnchantmentLoadoutUI && UIManager.NoPanelBeingDragged) {
					if (scrollPanelPosition > 0 && scrollWheelTicks < 0 || scrollPanelPosition < possiblePanelPositions && scrollWheelTicks > 0) {
						SoundEngine.PlaySound(SoundID.MenuTick);
						scrollPanelPosition += scrollWheelTicks;
					}
				}
			}
		}
		public static List<List<Item>> GetBlankLoadout() {
			int count = WEPlayer.LocalWEPlayer.Equipment.GetAllArmor().Count() + 1;
			return Enumerable.Repeat(Enumerable.Repeat(new Item(), EnchantingTableUI.MaxEnchantmentSlots).ToList(), count).ToList();
		}
		public static bool CanBeStored(Item item) {
			if (item?.ModItem is WEModItem weModItem)
				return weModItem.CanBeStoredInEnchantmentStroage;

			return false;
		}
		public static bool RoomInStorage(Item item, Player player = null) {
			if (Main.netMode == NetmodeID.Server)
				return false;

			if (player == null)
				player = Main.LocalPlayer;

			if (player.whoAmI != Main.myPlayer)
				return false;

			Item[] inv = player.GetWEPlayer().EnchantmentLoadoutUIItems;
			int stack = item.stack;
			for (int i = 0; i < inv.Length; i++) {
				Item invItem = inv[i];
				if (invItem.IsAir) {
					return true;
				}
				else if (invItem.type == item.type) {
					stack -= invItem.maxStack - invItem.stack;
					if (stack <= 0)
						return true;
				}
			}

			return false;
		}
		private static void LootAll() {
			for (int i = 0; i < WEPlayer.LocalWEPlayer.EnchantmentLoadoutUIItems.Length; i++) {
				if (WEPlayer.LocalWEPlayer.EnchantmentLoadoutUIItems[i].type > ItemID.None) {
					WEPlayer.LocalWEPlayer.EnchantmentLoadoutUIItems[i] = Main.LocalPlayer.GetItem(Main.myPlayer, WEPlayer.LocalWEPlayer.EnchantmentLoadoutUIItems[i], GetItemSettings.LootAllSettings);
				}
			}
		}
		public static bool DepositAll(ref Item item) => DepositAll(new Item[] { item });
		public static bool DepositAll(Item[] inv) {
			bool transferedAnyItem = QuickStack(inv, false);
			int storageIndex = 0;
			for (int i = 0; i < inv.Length; i++) {
				ref Item item = ref inv[i];
				if (!item.favorited && CanBeStored(item)) {
					while (storageIndex < WEPlayer.LocalWEPlayer.EnchantmentLoadoutUIItems.Length && WEPlayer.LocalWEPlayer.EnchantmentLoadoutUIItems[storageIndex].type > ItemID.None) {
						storageIndex++;
					}

					if (storageIndex < WEPlayer.LocalWEPlayer.EnchantmentLoadoutUIItems.Length) {
						WEPlayer.LocalWEPlayer.EnchantmentLoadoutUIItems[storageIndex] = item.Clone();
						item.TurnToAir();
						transferedAnyItem = true;
					}
					else {
						break;
					}
				}
			}

			if (transferedAnyItem) {
				SoundEngine.PlaySound(SoundID.Grab);
				Recipe.FindRecipes(true);
			}

			return transferedAnyItem;
		}
		public static bool QuickStack(ref Item item) => QuickStack(new Item[] { item });
		public static bool QuickStack(Item[] inv, bool playSound = true) {
			bool transferedAnyItem = false;
			SortedDictionary<int, List<int>> nonAirItemsInStorage = new();
			for (int i = 0; i < WEPlayer.LocalWEPlayer.EnchantmentLoadoutUIItems.Length; i++) {
				int type = WEPlayer.LocalWEPlayer.EnchantmentLoadoutUIItems[i].type;
				if (type > ItemID.None)
					nonAirItemsInStorage.AddOrCombine(type, i);
			}

			for (int i = 0; i < inv.Length; i++) {
				ref Item item = ref inv[i];
				if (!item.favorited && CanBeStored(item) && nonAirItemsInStorage.TryGetValue(item.type, out List<int> storageIndexes)) {
					foreach (int storageIndex in storageIndexes) {
						ref Item storageItem = ref WEPlayer.LocalWEPlayer.EnchantmentLoadoutUIItems[storageIndex];
						if (storageItem.stack < item.maxStack) {
							if (ItemLoader.CanStack(storageItem, item)) {
								int amountToTransfer = Math.Min(item.stack, storageItem.maxStack - storageItem.stack);
								storageItem.stack += amountToTransfer;
								item.stack -= amountToTransfer;
								transferedAnyItem = true;
								if (item.stack < 1) {
									item.TurnToAir();
									break;
								}
							}
						}
					}
				}
			}

			if (playSound && transferedAnyItem) {
				SoundEngine.PlaySound(SoundID.Grab);
				Recipe.FindRecipes(true);
			}

			return transferedAnyItem;
		}
		private static void Sort() {
			MethodInfo sort = typeof(ItemSorting).GetMethod("Sort", BindingFlags.NonPublic | BindingFlags.Static);
			sort.Invoke(null, new object[] { WEPlayer.LocalWEPlayer.EnchantmentLoadoutUIItems, new int[0] });
			Type itemSlotType = typeof(ItemSlot);
			int[] inventoryGlowTime = (int[])itemSlotType.GetField("inventoryGlowTime", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
			for (int i = 0; i < inventoryGlowTime.Length; i++) {
				inventoryGlowTime[i] = 0;
			}

			if (Main.LocalPlayer.chest != -1) {
				int[] inventoryGlowTimeChest = (int[])itemSlotType.GetField("inventoryGlowTimeChest", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
				for (int i = 0; i < inventoryGlowTimeChest.Length; i++) {
					inventoryGlowTimeChest[i] = 0;
				}
			}

			IEnumerable<Item> containments = WEPlayer.LocalWEPlayer.EnchantmentLoadoutUIItems.Where(i => i.ModItem is ContainmentItem).OrderBy(i => i.type);
			IEnumerable<Item> powerBoosters = WEPlayer.LocalWEPlayer.EnchantmentLoadoutUIItems.Where(i => i.ModItem is PowerBooster or UltraPowerBooster).OrderBy(i => i.type);
			IEnumerable<Item> enchantments = WEPlayer.LocalWEPlayer.EnchantmentLoadoutUIItems.GetSortedEnchantments();
			IEnumerable<Item> goodEnchantments = enchantments.Where(i => i.favorited || !WEPlayer.LocalWEPlayer.trashEnchantmentsFullNames.Contains(i.type.GetItemIDOrName()));
			IEnumerable<Item> trashEnchantments = enchantments.Where(i => !i.favorited && WEPlayer.LocalWEPlayer.trashEnchantmentsFullNames.Contains(i.type.GetItemIDOrName()));
			IEnumerable<Item> otherItems = WEPlayer.LocalWEPlayer.EnchantmentLoadoutUIItems.Where(i => i.ModItem == null || i.ModItem is not (Enchantment or ContainmentItem or PowerBooster or UltraPowerBooster));
			WEPlayer.LocalWEPlayer.EnchantmentLoadoutUIItems = containments.Concat(powerBoosters).Concat(goodEnchantments).Concat(trashEnchantments).Concat(otherItems).ToArray();
			glowTime = 300;
			glowHue = 0.5f;
		}
		public static IEnumerable<Item> GetSortedEnchantments(this IEnumerable<Item> items) {
			return items
				.Where(i => i.ModItem is Enchantment)
				.GroupBy(i => ((Enchantment)i.ModItem).EnchantmentTypeName)
				.OrderBy(g => g.Key)
				.Select(l => l.ToList().OrderBy(i => ((Enchantment)i.ModItem).EnchantmentTier))
				.SelectMany(l => l);
		}
		private static void ToggleVacuum() {
			WEPlayer.LocalWEPlayer.vacuumItemsIntoEnchantmentLoadoutUI = !WEPlayer.LocalWEPlayer.vacuumItemsIntoEnchantmentLoadoutUI;
		}
		private static void ToggleMarkTrash() {
			markingTrash = !markingTrash;
		}
		public static void UncraftTrash(Item item, bool force = true) => UncraftTrash(new Item[] { item }, force);
		private static void UncraftTrash(Item[] inv, bool force = false) {
			Recipe.FindRecipes();
			uncraftedExtraItems.Clear();
			Dictionary<int, HashSet<int>> storageIndexes = new();
			for (int i = 0; i < inv.Length; i++) {
				ref Item item = ref inv[i];
				if (!item.favorited && WEPlayer.LocalWEPlayer.trashEnchantmentsFullNames.Contains(item.type.GetItemIDOrName()))
					storageIndexes.AddOrCombine(item.type, i);
			}

			int enchantmentEssence = ModContent.ItemType<EnchantmentEssenceBasic>();
			Dictionary<int, int> recipeNumbers = new();
			for (int i = 0; i < Main.recipe.Length; i++) {
				Recipe r = Main.recipe[i];
				if (r.createItem.type == enchantmentEssence && r.requiredItem.Count > 0 && r.requiredItem.First().type is int type && storageIndexes.ContainsKey(type) && !recipeNumbers.ContainsKey(type))
					recipeNumbers.Add(type, i);
			}

			foreach (KeyValuePair<int, HashSet<int>> itemType in storageIndexes) {
				int type = itemType.Key;
				if (recipeNumbers.TryGetValue(type, out int recipeNum)) {
					Recipe r = Main.recipe[recipeNum];
					foreach (int slot in itemType.Value) {
						int stack = inv[slot].stack;
						for (int i = 0; i < stack; i++) {
							Recipe.FindRecipes();
							bool shouldCraft = force;
							if (!shouldCraft) {
								for (int availableRecipeIndex = 0; availableRecipeIndex < Main.numAvailableRecipes; availableRecipeIndex++) {
									int availableRecipeNum = Main.availableRecipe[availableRecipeIndex];
									if (recipeNum == availableRecipeNum) {
										shouldCraft = true;
										break;
									}
								}
							}

							if (shouldCraft && recipeNum.TryCraftItem(out Item crafted))
								uncraftedExtraItems.AddOrCombine(crafted.type, crafted.stack);
						}
					}
				}
			}

			QuickSpawnAllCraftedItems();

			Recipe.FindRecipes();
		}
		private static void RevertAllToBasic() {
			Recipe.FindRecipes();
			uncraftedExtraItems.Clear();
			uncraftedItems.Clear();
			Dictionary<int, HashSet<int>> storageIndexes = new();
			for (int i = 0; i < WEPlayer.LocalWEPlayer.EnchantmentLoadoutUIItems.Length; i++) {
				ref Item item = ref WEPlayer.LocalWEPlayer.EnchantmentLoadoutUIItems[i];
				if (item.ModItem is Enchantment enchantment && enchantment.EnchantmentTier > 0)
					storageIndexes.AddOrCombine(item.type, i);
			}

			Dictionary<int, int> recipeNumbers = new();
			for (int i = 0; i < Main.recipe.Length; i++) {
				Recipe r = Main.recipe[i];
				if (r.createItem.ModItem is Enchantment enchantment && enchantment.EnchantmentTier == 0 && r.requiredItem.Count > 0 && r.requiredItem.First().type is int type && storageIndexes.ContainsKey(type) && !recipeNumbers.ContainsKey(type))
					recipeNumbers.Add(type, i);
			}

			foreach (KeyValuePair<int, HashSet<int>> itemType in storageIndexes) {
				int type = itemType.Key;
				if (recipeNumbers.TryGetValue(type, out int recipeNum)) {
					foreach (int slot in itemType.Value) {
						int stack = WEPlayer.LocalWEPlayer.EnchantmentLoadoutUIItems[slot].stack;
						for (int i = 0; i < stack; i++) {
							if (recipeNum.TryCraftItem(out Item crafted)) {
								uncraftedItems.Add(crafted);
							}
							else {
								break;
							}
						}
					}
				}
			}

			QuickSpawnAllCraftedItems();

			Recipe.FindRecipes();
		}
		public static bool TryCraftItem(this int recipeNum, out Item crafted) {
			crafted = null;
			Recipe.FindRecipes();
			for (int availableRecipeIndex = 0; availableRecipeIndex < Main.numAvailableRecipes; availableRecipeIndex++) {
				int availableRecipeNum = Main.availableRecipe[availableRecipeIndex];
				if (recipeNum == availableRecipeNum) {
					crafted = recipeNum.CraftItem();

					return true;
				}
			}

			return false;
		}
		public static Item CraftItem(this int recipeNum) {
			crafting = true;
			Recipe r = Main.recipe[recipeNum];
			Item crafted = r.createItem.Clone();
			crafted.Prefix(-1);
			r.Create();
			List<Item> ConsumedItems = (List<Item>)typeof(RecipeLoader).GetField("ConsumedItems", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
			RecipeLoader.OnCraft(crafted, r, ConsumedItems);
			Recipe.FindRecipes(true);
			crafting = false;

			return crafted;
		}
		private static void QuickSpawnAllCraftedItems() {
			if (uncraftedExtraItems.Count > 0) {
				SoundEngine.PlaySound(SoundID.Grab);
				foreach (KeyValuePair<int, int> item in uncraftedExtraItems) {
					Item sample = ContentSamples.ItemsByType[item.Key];
					int stack = item.Value;
					while (stack > 0) {
						int amountToSpawn = Math.Min(sample.maxStack, stack);
						Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("Crafting"), item.Key, amountToSpawn);
						stack -= amountToSpawn;
					}
				}

				foreach (Item item in uncraftedItems) {
					Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("Crafting"), item, item.stack);
				}

				uncraftedExtraItems.Clear();
				uncraftedItems.Clear();
			}
		}
	}
	*/
}
