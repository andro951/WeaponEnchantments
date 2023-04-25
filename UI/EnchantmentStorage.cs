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
	public static class EnchantmentStorage
	{
		public class EnchantmentStorageButtonID
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
			public const int Count = 10;
		}
		public static int ID => UI_ID.EnchantmentStorage;
		public static int SearchID => UI_ID.EnchantmentStorageSearch;
		public static int EnchantmentStorageUIDefaultLeft => 680;
		public static int EnchantmentStorageUIDefaultTop => 90;
		public static Color PanelColor => new Color(26, 2, 56, 100);
		private static int Spacing => 4;
		private static int PanelBorder => 10;
		public const float buttonScaleMinimum = 0.75f;
		public const float buttonScaleMaximum = 1f;
		public static float[] ButtonScale = Enumerable.Repeat(buttonScaleMinimum, EnchantmentStorageButtonID.Count).ToArray();
		private static bool markingTrash = false;
		private static int glowTime = 0;
		private static float glowHue = 0f;
		public static bool uncrafting = false;
		public static SortedDictionary<int, int> uncraftedExtraItems = new();
		public static List<Item> uncraftedItems = new();
		private static int scrollPanelY = int.MinValue;
		private static int scrollPanelPosition = 0;
		public static bool managingTrash = false;
		public static bool managingOfferdItems = false;
		private static Item[] allEnchantments = null;
		private static Item[] AllEnchantments {
			get {
				if (allEnchantments == null) {
					allEnchantments = ContentSamples.ItemsByType.Select(p => p.Value).Where(i => i.ModItem is Enchantment).Select(i => new Item(i.type)).ToArray();
				}

				return allEnchantments;
			}
		}
		private static Item[] allOfferableItems = null;
		private static Item[] AllOfferableItems {
			get {
				if (allOfferableItems == null) {
					allOfferableItems = ContentSamples.ItemsByType.Select(p => p.Value).Where(i => EnchantedItemStaticMethods.IsEnchantable(i)).Select(i => new Item(i.type)).ToArray();
				}

				return allOfferableItems;
			}
		}
		public static void PostDrawInterface(SpriteBatch spriteBatch) {
			WEPlayer wePlayer = WEPlayer.LocalWEPlayer;
			if (wePlayer.displayEnchantmentStorage) {

				#region Data

				Color mouseColor = UIManager.MouseColor;
				if (glowTime > 0) {
					glowTime--;
					if (glowTime <= 0)
						glowHue = 0f;
				}

				//ItemSlots Data 1/2
				Item[] inventory;
				if (managingTrash) {
					inventory = AllEnchantments;
				}
				else if (managingOfferdItems) {
					inventory = AllOfferableItems;
				}
				else {
					inventory = wePlayer.enchantmentStorageItems;
				}

				int itemSlotColumns = 10;
				int itemSlotRowsDisplayed = 5;
				int itemSlotTotalRows = inventory.Length.CeilingDivide(itemSlotColumns);
				int itemSlotSpaceWidth = UIManager.ItemSlotSize + Spacing;
				int itemSlotSpaceHeight = UIManager.ItemSlotSize + Spacing;
				int itemSlotsWidth = (itemSlotColumns - 1) * itemSlotSpaceWidth + UIManager.ItemSlotSize;
				int itemSlotsHeight = (itemSlotRowsDisplayed - 1) * itemSlotSpaceHeight + UIManager.ItemSlotSize;
				int itemSlotsLeft = wePlayer.enchantmentStorageUILeft + PanelBorder;

				//Name Data
				int nameLeft = itemSlotsLeft;//itemSlotsLeft + (itemSlotsWidth - nameWidth) / 2;
				int nameTop = wePlayer.enchantmentStorageUITop + PanelBorder;
				string name = EnchantmentStorageTextID.EnchantmentStorage.ToString().Lang(L_ID1.EnchantmentStorageText);
				UITextData nameData = new(UI_ID.None, nameLeft, nameTop, name, 1f, mouseColor);

				//Panel Data 1/2
				int panelBorderTop = nameData.Height + Spacing + 2;

				//Search Bar Data
				int searchBarMinWidth = 100;
				TextData searchBarTextData = new(UIManager.DisplayedSearchBarString(SearchID));
				UIButtonData searchBarData = new(SearchID, nameData.BottomRight.X + Spacing * 2, nameTop - 6, searchBarTextData, mouseColor, Math.Max(6, (searchBarMinWidth - searchBarTextData.Width) / 2), 0, PanelColor, new Color(50, 4, 110, 100));

				//ItemSlots Data 2/2
				int itemSlotsTop = wePlayer.enchantmentStorageUITop + panelBorderTop;

				//Scroll Bar Data 1/2
				int scrollBarLeft = itemSlotsLeft + itemSlotsWidth + Spacing;
				int scrollBarWidth = 30;

				//Text buttons Data
				int buttonsLeft = scrollBarLeft + scrollBarWidth + Spacing;
				int currentButtonTop = nameTop;
				UITextData[] textButtons = new UITextData[EnchantmentStorageButtonID.Count];
				int longestButtonNameWidth = 0;
				for (int buttonIndex = 0; buttonIndex < EnchantmentStorageButtonID.Count; buttonIndex++) {
					string text = ((EnchantmentStorageTextID)buttonIndex).ToString().Lang(L_ID1.EnchantmentStorageText);
					float scale = ButtonScale[buttonIndex];
					Color color;
					if (buttonIndex == EnchantmentStorageButtonID.ToggleVacuum && wePlayer.vacuumItemsIntoEnchantmentStorage) {
						color = new(162, 22, 255);
					}
					else if (buttonIndex == EnchantmentStorageButtonID.ToggleMarkTrash && markingTrash || buttonIndex == EnchantmentStorageButtonID.ManageTrash && managingTrash || buttonIndex == EnchantmentStorageButtonID.ManageOfferedItems && managingOfferdItems) {
						color = new(100, 100, 100);
					}
					else {
						color = mouseColor;
					}

					UITextData thisButton = new(UI_ID.EnchantmentStorageLootAll + buttonIndex, buttonsLeft, currentButtonTop, text, scale, color, ancorBotomLeft: true);
					textButtons[buttonIndex] = thisButton;
					longestButtonNameWidth = Math.Max(longestButtonNameWidth, thisButton.Width);
					currentButtonTop += (int)(thisButton.BaseHeight * 0.95f);
				}

				//Panel Data 2/2
				int panelBorderRightOffset = Spacing + longestButtonNameWidth + PanelBorder;
				int panelWidth = itemSlotsWidth + Spacing + scrollBarWidth + PanelBorder + panelBorderRightOffset;
				int panelHeight = itemSlotsHeight + PanelBorder + panelBorderTop;
				UIPanelData panel = new(ID, wePlayer.enchantmentStorageUILeft, wePlayer.enchantmentStorageUITop, panelWidth, panelHeight, PanelColor);

				//Scroll Bar Data 2/2
				int scrollBarTop = wePlayer.enchantmentStorageUITop + PanelBorder;
				UIPanelData scrollBarData = new(UI_ID.EnchantmentStorageScrollBar, scrollBarLeft, scrollBarTop, scrollBarWidth, panelHeight - PanelBorder * 2, new Color(10, 1, 30, 100));

				//Scroll Panel Data 1/2
				int scrollPanelXOffset = 1;
				int scrollPanelSize = scrollBarWidth - scrollPanelXOffset * 2;
				int scrollPanelMinY = scrollBarData.TopLeft.Y + scrollPanelXOffset;
				int scrollPanelMaxY = scrollBarData.BottomRight.Y - scrollPanelSize - scrollPanelXOffset;
				int possiblePanelPositions = itemSlotTotalRows - itemSlotRowsDisplayed;
				scrollPanelPosition.Clamp(0, possiblePanelPositions);

				#endregion

				//Panel Draw
				panel.Draw(spriteBatch);

				//Scroll Bar Draw
				scrollBarData.Draw(spriteBatch);

				//ItemSlots Draw
				int startRow = scrollPanelPosition;
				int endRow = startRow + itemSlotRowsDisplayed;
				bool UsingSearchBar = UIManager.UsingSearchBar(SearchID);
				int inventoryIndexStart = !UsingSearchBar ? startRow * itemSlotColumns : 0;
				int slotsToDisplay = itemSlotRowsDisplayed * itemSlotColumns;
				int slotNum = 0;
				int itemSlotX = itemSlotsLeft;
				int itemSlotY = itemSlotsTop;
				for (int inventoryIndex = inventoryIndexStart; inventoryIndex < inventory.Length && slotNum < slotsToDisplay; inventoryIndex++) {
					if (inventoryIndex >= inventory.Length)
						break;

					ref Item item = ref inventory[inventoryIndex];
					if (!UsingSearchBar || item.Name.ToLower().Contains(UIManager.SearchBarString.ToLower())) {
						UIItemSlotData slotData = new(UI_ID.EnchantmentStorageItemSlot, itemSlotX, itemSlotY);
						string modFullName = item.type.GetItemIDOrName();
						if (managingTrash) {
							if (slotData.MouseHovering()) {
								ItemSlot.MouseHover(inventory, 0, slot: inventoryIndex);
								Main.cursorOverride = CursorOverrideID.TrashCan;
								if (UIManager.LeftMouseClicked) {
									if (wePlayer.trashEnchantmentsFullNames.Contains(modFullName)) {
										wePlayer.trashEnchantmentsFullNames.Remove(modFullName);
									}
									else {
										wePlayer.trashEnchantmentsFullNames.Add(modFullName);
									}

									SoundEngine.PlaySound(SoundID.MenuTick);
								}
							}

							if (wePlayer.trashEnchantmentsFullNames.Contains(modFullName)) {
								slotData.Draw(spriteBatch, item, ItemSlotContextID.MarkedTrash, glowHue, glowTime);
							}
							else {
								slotData.Draw(spriteBatch, item, ItemSlotContextID.Normal, glowHue, glowTime);
							}
						}
						else if (managingOfferdItems) {
							if (slotData.MouseHovering()) {
								ItemSlot.MouseHover(inventory, 0, slot: inventoryIndex);
								Main.cursorOverride = CursorOverrideID.TrashCan;
								if (UIManager.LeftMouseClicked) {
									if (wePlayer.allOfferedItems.Contains(modFullName)) {
										wePlayer.allOfferedItems.Remove(modFullName);
									}
									else {
										wePlayer.allOfferedItems.Add(modFullName);
									}

									SoundEngine.PlaySound(SoundID.MenuTick);
								}
							}

							if (wePlayer.allOfferedItems.Contains(modFullName)) {
								slotData.Draw(spriteBatch, item, ItemSlotContextID.MarkedTrash, glowHue, glowTime);
							}
							else {
								slotData.Draw(spriteBatch, item, ItemSlotContextID.Normal, glowHue, glowTime);
							}
						}
						else {
							if (slotData.MouseHovering()) {
								if (WEModSystem.FavoriteKeyDown) {
									Main.cursorOverride = CursorOverrideID.FavoriteStar;
									if (UIManager.LeftMouseClicked) {
										item.favorited = !item.favorited;
										SoundEngine.PlaySound(SoundID.MenuTick);
										if (item.TryGetGlobalItem(out VacuumToStorageItems vacummItem2))
											vacummItem2.favorited = item.favorited;
									}
								}
								else if (Main.mouseItem.NullOrAir() || CanBeStored(Main.mouseItem)) {
									if (markingTrash && Main.mouseItem.NullOrAir() && item.ModItem is Enchantment) {
										Main.cursorOverride = CursorOverrideID.TrashCan;
										if (UIManager.LeftMouseClicked) {
											if (wePlayer.trashEnchantmentsFullNames.Contains(modFullName)) {
												wePlayer.trashEnchantmentsFullNames.Remove(modFullName);
											}
											else {
												wePlayer.trashEnchantmentsFullNames.Add(modFullName);
											}

											SoundEngine.PlaySound(SoundID.MenuTick);
										}
										else {
											slotData.ClickInteractions(ref item);
										}
									}
									else if (wePlayer.usingEnchantingTable && ItemSlot.ShiftInUse) {
										if (wePlayer.CheckShiftClickValid(ref item)) {
											if (UIManager.LeftMouseClicked)
												wePlayer.CheckShiftClickValid(ref item, true);
										}
										else {
											if (UIManager.LeftMouseClicked && Main.mouseItem.IsAir) {
												Main.mouseItem = item.Clone();
												item = new();
											}
											else {
												slotData.ClickInteractions(ref item);
											}

											Main.cursorOverride = -1;
										}
									}
									else {
										slotData.ClickInteractions(ref item);
									}
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
				for (int buttonIndex = 0; buttonIndex < EnchantmentStorageButtonID.Count; buttonIndex++) {
					UITextData textButton = textButtons[buttonIndex];
					textButton.Draw(spriteBatch);
					if (UIManager.MouseHovering(textButton, true)) {
						ButtonScale[buttonIndex] += 0.05f;

						if (ButtonScale[buttonIndex] > buttonScaleMaximum)
							ButtonScale[buttonIndex] = buttonScaleMaximum;

						if (UIManager.LeftMouseClicked) {
							if (managingTrash && buttonIndex != EnchantmentStorageButtonID.ManageTrash)
								managingTrash = false;

							if (managingOfferdItems && buttonIndex != EnchantmentStorageButtonID.ManageOfferedItems)
								managingOfferdItems = false;

							switch (buttonIndex) {
								case EnchantmentStorageButtonID.LootAll:
									LootAll();
									break;
								case EnchantmentStorageButtonID.DepositAll:
									DepositAll(Main.LocalPlayer.inventory);
									break;
								case EnchantmentStorageButtonID.QuickStack:
									QuickStack(Main.LocalPlayer.inventory);
									break;
								case EnchantmentStorageButtonID.Sort:
									Sort();
									break;
								case EnchantmentStorageButtonID.ToggleVacuum:
									ToggleVacuum();
									break;
								case EnchantmentStorageButtonID.ToggleMarkTrash:
									ToggleMarkTrash();
									break;
								case EnchantmentStorageButtonID.UncraftAllTrash:
									UncraftTrash(wePlayer.enchantmentStorageItems);
									break;
								case EnchantmentStorageButtonID.RevertAllToBasic:
									RevertAllToBasic();
									break;
								case EnchantmentStorageButtonID.ManageTrash:
									managingTrash = !managingTrash;
									break;
								case EnchantmentStorageButtonID.ManageOfferedItems:
									managingOfferdItems = !managingOfferdItems;
									break;
								//DODO: Add Select button like Mark Trash
								//DODO: Add Quick craft all to tier.  Needs tier buttons, Basic, Common....
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
				bool draggingScrollPanel = UIManager.PanelBeingDragged == UI_ID.EnchantmentStorageScrollPanel;
				if (UIManager.PanelBeingDragged == UI_ID.EnchantmentStorageScrollPanel) {
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
				UIPanelData scrollPanelData = new(UI_ID.EnchantmentStorageScrollPanel, scrollBarLeft + scrollPanelXOffset, scrollPanelY, scrollPanelSize, scrollPanelSize, mouseColor);
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
					UIManager.DragUI(out wePlayer.enchantmentStorageUILeft, out wePlayer.enchantmentStorageUITop);

				int scrollWheelTicks = UIManager.ScrollWheelTicks;
				if (scrollWheelTicks != 0 && UIManager.HoveringEnchantmentStorage && UIManager.NoPanelBeingDragged) {
					if (scrollPanelPosition > 0 && scrollWheelTicks < 0 || scrollPanelPosition < possiblePanelPositions && scrollWheelTicks > 0) {
						SoundEngine.PlaySound(SoundID.MenuTick);
						scrollPanelPosition += scrollWheelTicks;
					}
				}
			}
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

			Item[] inv = player.GetWEPlayer().enchantmentStorageItems;
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
			for (int i = 0; i < WEPlayer.LocalWEPlayer.enchantmentStorageItems.Length; i++) {
				if (WEPlayer.LocalWEPlayer.enchantmentStorageItems[i].type > ItemID.None) {
					WEPlayer.LocalWEPlayer.enchantmentStorageItems[i] = Main.LocalPlayer.GetItem(Main.myPlayer, WEPlayer.LocalWEPlayer.enchantmentStorageItems[i], GetItemSettings.LootAllSettings);
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
					while (storageIndex < WEPlayer.LocalWEPlayer.enchantmentStorageItems.Length && WEPlayer.LocalWEPlayer.enchantmentStorageItems[storageIndex].type > ItemID.None) {
						storageIndex++;
					}

					if (storageIndex < WEPlayer.LocalWEPlayer.enchantmentStorageItems.Length) {
						WEPlayer.LocalWEPlayer.enchantmentStorageItems[storageIndex] = item.Clone();
						item.TurnToAir();
						transferedAnyItem = true;
					}
					else {
						break;
					}
				}
			}

			if (transferedAnyItem)
				SoundEngine.PlaySound(SoundID.Grab);

			return transferedAnyItem;
		}
		public static bool QuickStack(ref Item item) => QuickStack(new Item[] { item });
		public static bool QuickStack(Item[] inv, bool playSound = true) {
			bool transferedAnyItem = false;
			SortedDictionary<int, List<int>> nonAirItemsInStorage = new();
			for (int i = 0; i < WEPlayer.LocalWEPlayer.enchantmentStorageItems.Length; i++) {
				int type = WEPlayer.LocalWEPlayer.enchantmentStorageItems[i].type;
				if (type > ItemID.None)
					nonAirItemsInStorage.AddOrCombine(type, i);
			}

			for (int i = 0; i < inv.Length; i++) {
				ref Item item = ref inv[i];
				if (!item.favorited && CanBeStored(item) && nonAirItemsInStorage.TryGetValue(item.type, out List<int> storageIndexes)) {
					foreach (int storageIndex in storageIndexes) {
						ref Item storageItem = ref WEPlayer.LocalWEPlayer.enchantmentStorageItems[storageIndex];
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

			if (playSound && transferedAnyItem)
				SoundEngine.PlaySound(SoundID.Grab);

			return transferedAnyItem;
		}
		private static void Sort() {
			MethodInfo sort = typeof(ItemSorting).GetMethod("Sort", BindingFlags.NonPublic | BindingFlags.Static);
			sort.Invoke(null, new object[] { WEPlayer.LocalWEPlayer.enchantmentStorageItems, new int[0] });
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

			IEnumerable<Item> containments = WEPlayer.LocalWEPlayer.enchantmentStorageItems.Where(i => i.ModItem is ContainmentItem).OrderBy(i => i.type);
			IEnumerable<Item> powerBoosters = WEPlayer.LocalWEPlayer.enchantmentStorageItems.Where(i => i.ModItem is PowerBooster or UltraPowerBooster).OrderBy(i => i.type);
			IEnumerable<Item> enchantments = WEPlayer.LocalWEPlayer.enchantmentStorageItems.Where(i => i.ModItem is Enchantment)
				.GroupBy(i => ((Enchantment)i.ModItem).EnchantmentTypeName).OrderBy(g => g.Key).Select(l => l.ToList().OrderBy(i => ((Enchantment)i.ModItem).EnchantmentTier)).SelectMany(l => l);
			IEnumerable<Item> goodEnchantments = enchantments.Where(i => i.favorited || !WEPlayer.LocalWEPlayer.trashEnchantmentsFullNames.Contains(i.type.GetItemIDOrName()));
			IEnumerable<Item> trashEnchantments = enchantments.Where(i => !i.favorited && WEPlayer.LocalWEPlayer.trashEnchantmentsFullNames.Contains(i.type.GetItemIDOrName()));
			IEnumerable<Item> otherItems = WEPlayer.LocalWEPlayer.enchantmentStorageItems.Where(i => i.ModItem == null || i.ModItem is not (Enchantment or ContainmentItem or PowerBooster or UltraPowerBooster));
			WEPlayer.LocalWEPlayer.enchantmentStorageItems = containments.Concat(powerBoosters).Concat(goodEnchantments).Concat(trashEnchantments).Concat(otherItems).ToArray();
			glowTime = 300;
			glowHue = 0.5f;
		}
		private static void ToggleVacuum() {
			WEPlayer.LocalWEPlayer.vacuumItemsIntoEnchantmentStorage = !WEPlayer.LocalWEPlayer.vacuumItemsIntoEnchantmentStorage;
		}
		private static void ToggleMarkTrash() {
			markingTrash = !markingTrash;
		}
		public static void UncraftTrash(Item item, bool force = true) => UncraftTrash(new Item[] { item }, force);
		private static void UncraftTrash(Item[] inv, bool force = false) {
			uncrafting = true;
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
							
							if (shouldCraft) {
								Item crafted = r.createItem.Clone();
								crafted.Prefix(-1);
								r.Create();
								RecipeLoader.OnCraft(crafted, r, new() { new Item() });
								uncraftedExtraItems.AddOrCombine(crafted.type, crafted.stack);
							}
						}
					}
				}
			}

			QuickSpawnAllUncraftedItems();

			uncrafting = false;
			Recipe.FindRecipes();
		}
		private static void RevertAllToBasic() {
			uncrafting = true;
			Recipe.FindRecipes();
			uncraftedExtraItems.Clear();
			uncraftedItems.Clear();
			Dictionary<int, HashSet<int>> storageIndexes = new();
			for (int i = 0; i < WEPlayer.LocalWEPlayer.enchantmentStorageItems.Length; i++) {
				ref Item item = ref WEPlayer.LocalWEPlayer.enchantmentStorageItems[i];
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
					Recipe r = Main.recipe[recipeNum];
					foreach (int slot in itemType.Value) {
						int stack = WEPlayer.LocalWEPlayer.enchantmentStorageItems[slot].stack;
						for (int i = 0; i < stack; i++) {
							Recipe.FindRecipes();
							for (int availableRecipeIndex = 0; availableRecipeIndex < Main.numAvailableRecipes; availableRecipeIndex++) {
								int availableRecipeNum = Main.availableRecipe[availableRecipeIndex];
								if (recipeNum == availableRecipeNum) {
									Item crafted = r.createItem.Clone();
									crafted.Prefix(-1);
									r.Create();
									RecipeLoader.OnCraft(crafted, r, new() { new Item() });
									uncraftedItems.Add(crafted);
									break;
								}
							}
						}
					}
				}
			}

			QuickSpawnAllUncraftedItems();

			uncrafting = false;
			Recipe.FindRecipes();
		}//TODO: Issue with Time Enchantment.  Causes reroll
		private static void QuickSpawnAllUncraftedItems() {
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
}
