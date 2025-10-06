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
using System.Collections;
using Humanizer;
using WeaponEnchantments.Common.Utility.LogSystem;
using androLib.Common.Utility;
using androLib.UI;
using androLib.Common.Globals;
using androLib;
using Terraria.WorldBuilding;
using WeaponEnchantments.Content.NPCs;

namespace WeaponEnchantments.UI
{
	public static class EnchantmentStorage
	{
		private static int GetUI_ID(int id) => MasterUIManager.GetUI_ID(id, WE_UI_ID.EnchantmentStorage_UITypeID);
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
			public const int QuickCrafting = 10;
			public const int Count = 11;
		}
		public static int ID => GetUI_ID(WE_UI_ID.EnchantmentStorage);
		public static int SearchID => GetUI_ID(WE_UI_ID.EnchantmentStorageSearch);
		public static int EnchantmentStorageUIDefaultLeft => 600;
		public static int EnchantmentStorageUIDefaultTop => 5;
		public static int EnchantmentStorageUIDefaultLeftWitchUI => 235;
		public static int EnchantmentStorageUIDefaultTopWitchUI => 455;
		public static Color PanelColor => new Color(26, 2, 56, UIManager.UIAlpha);
		public static Color SelectedTextGray => BagUI.SelectedTextGray;
		public static Color VacuumPurple => BagUI.VacuumPurple;
		private static int Spacing => 4;
		private static int PanelBorder => 10;
		public const float buttonScaleMinimum = 0.75f;
		public const float buttonScaleMaximum = 1f;
		public static float[] ButtonScale = Enumerable.Repeat(buttonScaleMinimum, EnchantmentStorageButtonID.Count).ToArray();
		private static bool markingTrash = false;
		private static int glowTime = 0;
		private static float glowHue = 0f;
		public static bool crafting = false;
		public static SortedDictionary<int, int> uncraftedExtraItems = new();
		public static List<Item> uncraftedItems = new();
		private static int scrollPanelY = int.MinValue;
		private static int scrollPanelPosition = 0;
		public static bool managingTrash = false;
		public static bool managingOfferdItems = false;
		public static bool quickCrafting = false;
		private static Item[] quickCraftInventory = null;
		private static SortedDictionary<int, int> availableEnchantmentRecipes = new();
		private static SortedDictionary<int, int> quickCraftItemCounts = new();
		public static bool recipiesUpdated = true;
		private static Item[] allEnchantments = null;
		private static Item[] AllEnchantments {
			get {
				if (allEnchantments == null)
					allEnchantments = ContentSamples.ItemsByType.Select(p => p.Value).GetSortedEnchantments().ToArray();

				return allEnchantments;
			}
		}
		private static Item[] allTrashableItems = null;
		private static Item[] itemsMarkedTrash = null;
		private static Item[] AllTrashableItems {
			get {
				WEPlayer wePlayer = WEPlayer.LocalWEPlayer;
				bool updateMarkedTrashItems = false;
				if (itemsMarkedTrash == null) {
					updateMarkedTrashItems = true;
				}
				else {
					if (itemsMarkedTrash.Length != wePlayer.trashEnchantmentsFullNames.Count) {
						updateMarkedTrashItems = true;
					}
					else {
						for (int i = 0; i < itemsMarkedTrash.Length; i++) {
							if (!wePlayer.trashEnchantmentsFullNames.Contains(itemsMarkedTrash[i].type.GetItemIDOrName())) {
								updateMarkedTrashItems = true;
								break;
							}
						}
					}
				}
				
				// TODO - fix and uncomment this
				/*if (updateMarkedTrashItems)
					itemsMarkedTrash = wePlayer.trashEnchantmentsFullNames.Select(n => n.Replace("\"", "").CSI_FromItemIDOrName()).ToArray();*/

				if (allTrashableItems == null || updateMarkedTrashItems) {
					if (itemsMarkedTrash.Length == 0) {
						allTrashableItems = (Item[])AllEnchantments.Clone();
					}
					else {
						int emptyspacers = 10;
						int rowSize = itemsMarkedTrash.Length % 10;
						if (rowSize > 0)
							emptyspacers += 10 - rowSize;

						allTrashableItems = itemsMarkedTrash.Concat(Enumerable.Repeat<Item>(null, emptyspacers)).Concat(AllEnchantments).ToArray();
					}
				}

				return allTrashableItems;
			}
		}
		private static Item[] allOfferableItems = null;
		private static Item[] autoOfferItems = new Item[0];
		private static Item[] AllOfferableItems {
			get {
				if (allOfferableItems == null) {
					allOfferableItems = ContentSamples.ItemsByType.Select(p => p.Value).Where(i => i.IsEnchantable()).Select(i => new Item(i.type)).ToArray();
				}

				if (MasterUIManager.UsingTypingBar(SearchID))
					return allOfferableItems;

				WEPlayer wePlayer = WEPlayer.LocalWEPlayer;
				if (autoOfferItems.Length != wePlayer.allOfferedItems.Count) {
					autoOfferItems = ContentSamples.ItemsByType.Select(p => p.Value).Where(i => i.IsEnchantable() && wePlayer.allOfferedItems.Contains(i.type.GetItemIDOrName())).Select(i => new Item(i.type)).ToArray();
				}

				if (autoOfferItems.Length == 0)
					return allOfferableItems;

				int emptyspacers = 10;
				int rowSize = autoOfferItems.Length % 10;
				if (rowSize > 0)
					emptyspacers += 10 - rowSize;

				return autoOfferItems.Concat(Enumerable.Repeat<Item>(null, emptyspacers)).Concat(allOfferableItems).ToArray();
			}
		}
		private static DrawnUIData drawnUIData;
		public static bool DisplayStorage => WEPlayer.LocalWEPlayer.displayEnchantmentStorage || Witch.DisplayingUI;
		public class DrawnUIData {

		}
		public static void PostDrawInterface(SpriteBatch spriteBatch) {
			WEPlayer wePlayer = WEPlayer.LocalWEPlayer;
			if (!DisplayStorage)
				return;

			#region Data

			drawnUIData = new();

			Color mouseColor = MasterUIManager.MouseColor;
			if (glowTime > 0) {
				glowTime--;
				if (glowTime <= 0)
					glowHue = 0f;
			}

			//ItemSlots Data 1/2
			Item[] inventory;
			if (managingTrash) {
				inventory = AllTrashableItems;
			}
			else if (quickCrafting) {
				UpdateQuickCraftInventory(wePlayer);
				inventory = quickCraftInventory;
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
			int itemSlotSpaceWidth = MasterUIManager.ItemSlotSize + Spacing;
			int itemSlotSpaceHeight = MasterUIManager.ItemSlotSize + Spacing;
			int itemSlotsWidth = (itemSlotColumns - 1) * itemSlotSpaceWidth + MasterUIManager.ItemSlotSize;
			int itemSlotsHeight = (itemSlotRowsDisplayed - 1) * itemSlotSpaceHeight + MasterUIManager.ItemSlotSize;
			int itemSlotsLeft = wePlayer.EnchantmentStorageUILeft + PanelBorder;

			//Name Data
			int nameLeft = itemSlotsLeft;//itemSlotsLeft + (itemSlotsWidth - nameWidth) / 2;
			int nameTop = wePlayer.EnchantmentStorageUITop + PanelBorder;
			string name = EnchantmentStorageTextID.EnchantmentStorage.ToString().Lang_WE(L_ID1.EnchantmentStorageText);
			UITextData nameData = new(WE_UI_ID.None, nameLeft, nameTop, name, 1f, mouseColor);

			//Panel Data 1/2
			int panelBorderTop = nameData.Height + Spacing + 2;

			//Search Bar Data
			int searchBarMinWidth = 100;
			TextData searchBarTextData = new(MasterUIManager.DisplayedSearchBarString(SearchID));
			UIButtonData searchBarData = new(SearchID, nameData.BottomRight.X + Spacing * 2, nameTop - 6, searchBarTextData, mouseColor, Math.Max(6, (searchBarMinWidth - searchBarTextData.Width) / 2), 0, PanelColor, new Color(50, 4, 110, UIManager.UIAlpha));

			//ItemSlots Data 2/2
			int itemSlotsTop = wePlayer.EnchantmentStorageUITop + panelBorderTop;

			//Scroll Bar Data 1/2
			int scrollBarLeft = itemSlotsLeft + itemSlotsWidth + Spacing;
			int scrollBarWidth = 30;

			//Text buttons Data
			int buttonsLeft = scrollBarLeft + scrollBarWidth + Spacing;
			int currentButtonTop = nameTop;
			UITextData[] textButtons = new UITextData[EnchantmentStorageButtonID.Count];
			int longestButtonNameWidth = 0;
			for (int buttonIndex = 0; buttonIndex < EnchantmentStorageButtonID.Count; buttonIndex++) {
				string text = buttonIndex >= (int)EnchantmentStorageTextID.ToggleMarkTrash ? ((EnchantmentStorageTextID)buttonIndex).ToString().Lang_WE(L_ID1.EnchantmentStorageText) : ((StorageTextID)buttonIndex).ToString().Lang(AndroMod.ModName, L_ID1.StorageText);
				float scale = ButtonScale[buttonIndex];
				Color color;
				if (buttonIndex == EnchantmentStorageButtonID.ToggleVacuum && wePlayer.vacuumItemsIntoEnchantmentStorage) {
					color = VacuumPurple;
				}
				else if (buttonIndex == EnchantmentStorageButtonID.ToggleMarkTrash && markingTrash || buttonIndex == EnchantmentStorageButtonID.ManageTrash && managingTrash || buttonIndex == EnchantmentStorageButtonID.ManageOfferedItems && managingOfferdItems || buttonIndex == EnchantmentStorageButtonID.QuickCrafting && quickCrafting) {
					color = SelectedTextGray;
				}
				else {
					color = mouseColor;
				}

				UITextData thisButton = new(GetUI_ID(WE_UI_ID.EnchantmentStorageLootAll) + buttonIndex, buttonsLeft, currentButtonTop, text, scale, color, ancorBotomLeft: true);
				textButtons[buttonIndex] = thisButton;
				longestButtonNameWidth = Math.Max(longestButtonNameWidth, thisButton.Width);
				currentButtonTop += (int)(thisButton.BaseHeight * 0.88f);
			}

			//Panel Data 2/2
			int panelBorderRightOffset = Spacing + longestButtonNameWidth + PanelBorder;
			int panelWidth = itemSlotsWidth + Spacing + scrollBarWidth + PanelBorder + panelBorderRightOffset;
			int panelHeight = itemSlotsHeight + PanelBorder + panelBorderTop;
			UIPanelData panel = new(ID, wePlayer.EnchantmentStorageUILeft, wePlayer.EnchantmentStorageUITop, panelWidth, panelHeight, PanelColor);

			//Scroll Bar Data 2/2
			int scrollBarTop = wePlayer.EnchantmentStorageUITop + PanelBorder;
			UIPanelData scrollBarData = new(GetUI_ID(WE_UI_ID.EnchantmentStorageScrollBar), scrollBarLeft, scrollBarTop, scrollBarWidth, panelHeight - PanelBorder * 2, new Color(10, 1, 30, UIManager.UIAlpha));

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
			bool UsingSearchBar = MasterUIManager.UsingTypingBar(SearchID);
			//int inventoryIndexStart = !UsingSearchBar ? startRow * itemSlotColumns : 0;
			int inventoryIndexStart = startRow * itemSlotColumns;
			int slotsToDisplay = itemSlotRowsDisplayed * itemSlotColumns;
			int slotNum = 0;
			int itemSlotX = itemSlotsLeft;
			int itemSlotY = itemSlotsTop;
			bool quickCraftingFoundAll = false;
			for (int inventoryIndex = inventoryIndexStart; inventoryIndex < inventory.Length && slotNum < slotsToDisplay; inventoryIndex++) {
				if (inventoryIndex >= inventory.Length)
					break;

				ref Item item = ref inventory[inventoryIndex];
				if (!UsingSearchBar || item != null && item.Name.ToLower().Contains(MasterUIManager.TypingBarString.ToLower())) {
					UIItemSlotData slotData = new(GetUI_ID(WE_UI_ID.EnchantmentStorageItemSlot), itemSlotX, itemSlotY);
					string modFullName = item?.type.GetItemIDOrName();
					if (managingTrash) {
						if (item != null) {//null filters are added to inventory to keep the items in the same horizontal slot.
							bool isTrash = wePlayer.trashEnchantmentsFullNames.Contains(modFullName);
							if (slotData.MouseHovering()) {
								ItemSlot.MouseHover(inventory, 0, slot: inventoryIndex);
								Main.cursorOverride = isTrash ? CursorOverrideID.CameraLight : CursorOverrideID.TrashCan;
								if (MasterUIManager.LeftMouseClicked) {
									if (isTrash) {
										wePlayer.trashEnchantmentsFullNames.Remove(modFullName);
									}
									else {
										wePlayer.trashEnchantmentsFullNames.Add(modFullName);
									}

									SoundEngine.PlaySound(SoundID.MenuTick);
								}
							}

							if (isTrash) {
								slotData.Draw(spriteBatch, item, ItemSlotContextID.MarkedTrash, glowHue, glowTime);
							}
							else {
								slotData.Draw(spriteBatch, item, ItemSlotContextID.Normal, glowHue, glowTime);
							}
						}
					}
					else if (managingOfferdItems) {
						if (item != null) {//null fillers are added to inventory to keep the items in the same horizontal slot.
							bool isOffered = wePlayer.allOfferedItems.Contains(modFullName);
							if (slotData.MouseHovering()) {
								ItemSlot.MouseHover(inventory, 0, slot: inventoryIndex);
								Main.cursorOverride = isOffered ? CursorOverrideID.CameraLight : CursorOverrideID.TrashCan;
								if (MasterUIManager.LeftMouseClicked) {
									if (isOffered) {
										wePlayer.allOfferedItems.Remove(modFullName);
									}
									else {
										wePlayer.allOfferedItems.Add(modFullName);
									}

									SoundEngine.PlaySound(SoundID.MenuTick);
								}
							}

							if (isOffered) {
								slotData.Draw(spriteBatch, item, ItemSlotContextID.MarkedTrash, glowHue, glowTime);
							}
							else {
								slotData.Draw(spriteBatch, item, ItemSlotContextID.Normal, glowHue, glowTime);
							}
						}
					}
					else if (quickCrafting) {
						int recipeNum = -1;
						if (!quickCraftingFoundAll) {
							if (!availableEnchantmentRecipes.TryGetValue(item.type, out recipeNum)) {
								recipeNum = -1;
								quickCraftingFoundAll = true;
							}
						}

						if (slotData.MouseHovering()) {
							ItemSlot.MouseHover(inventory, 0, slot: inventoryIndex);
							Main.cursorOverride = CursorOverrideID.BackInventory;
							if (MasterUIManager.LeftMouseClicked) {
								if (recipeNum != -1 && recipeNum.TryCraftItem(out Item crafted, true, true)) {
									DepositAll(ref crafted);
									SoundEngine.PlaySound(SoundID.Tink);
								}
								else {
									SoundEngine.PlaySound(SoundID.MenuTick);
								}
							}
						}

						int stack = quickCraftItemCounts[item.type];
						if (recipeNum != -1) {
							slotData.Draw(spriteBatch, item, ItemSlotContextID.GoldFavorited, glowHue, glowTime, stack);
						}
						else {
							slotData.Draw(spriteBatch, item, ItemSlotContextID.Normal, glowHue, glowTime, stack);
						}
					}
					else {
						bool isTrash = wePlayer.trashEnchantmentsFullNames.Contains(modFullName);
						if (slotData.MouseHovering()) {
							bool normalClickInteractions = true;
							if (WEModSystem.FavoriteKeyDown) {
								normalClickInteractions = false;
								Main.cursorOverride = CursorOverrideID.FavoriteStar;
								if (MasterUIManager.LeftMouseClicked) {
									item.favorited = !item.favorited;
									SoundEngine.PlaySound(SoundID.MenuTick);
									if (item.TryGetGlobalItem(out VacuumToStorageItem vacummItem2))
										vacummItem2.favorited = item.favorited;
								}
							}
							else {
								if (markingTrash) {
									if (!item.IsAir) {
										normalClickInteractions = false;
										if (item.ModItem is Enchantment) {
											Main.cursorOverride = isTrash ? CursorOverrideID.CameraLight : CursorOverrideID.TrashCan;
											if (MasterUIManager.LeftMouseClicked) {
												if (isTrash) {
													wePlayer.trashEnchantmentsFullNames.Remove(modFullName);
												}
												else {
													wePlayer.trashEnchantmentsFullNames.Add(modFullName);
												}

												SoundEngine.PlaySound(SoundID.MenuTick);
											}
										}
									}
								}
								else {
									if (ItemSlot.ShiftInUse) {
										normalClickInteractions = false;
										if (wePlayer.displayEnchantmentLoadoutUI) {
											if (item.ModItem is Enchantment) {
												if (EnchantmentLoadoutUI.AvailableSlot(item)) {
													Main.cursorOverride = CursorOverrideID.InventoryToChest;
													if (MasterUIManager.LeftMouseClicked)
														EnchantmentLoadoutUI.UpdateAvailableEnchantmentSlot(wePlayer, item);
												}
											}
										}
										else if (wePlayer.usingEnchantingTable) {
											if (wePlayer.CheckShiftClickValid(ref item)) {
												if (MasterUIManager.LeftMouseClicked)
													wePlayer.CheckShiftClickValid(ref item, true);
											}
											else {
												if (Main.mouseItem.IsAir) {
													if (!item.IsAir) {
														if (MasterUIManager.LeftMouseClicked) {
															MasterUIManager.SwapMouseItem(ref item);
														}
													}
												}
												else {
													if (CanBeStored(Main.mouseItem)) {
														if (MasterUIManager.LeftMouseClicked) {
															MasterUIManager.SwapMouseItem(ref item);
														}
													}
												}
											}
										}
										else if (Witch.DisplayingUI) {
											if (wePlayer.CheckShiftClickValid_WitchUI(ref item)) {
												if (MasterUIManager.LeftMouseClicked)
													wePlayer.CheckShiftClickValid_WitchUI(ref item, true);
											}
											else {
												if (Main.mouseItem.IsAir) {
													if (!item.IsAir) {
														if (MasterUIManager.LeftMouseClicked) {
															MasterUIManager.SwapMouseItem(ref item);
														}
													}
												}
												else {
													if (CanBeStored(Main.mouseItem)) {
														if (MasterUIManager.LeftMouseClicked) {
															MasterUIManager.SwapMouseItem(ref item);
														}
													}
												}
											}
										}
									}
									else {
										if (!Main.mouseItem.IsAir) {
											if (!CanBeStored(Main.mouseItem))
												normalClickInteractions = false;
										}
									}
								}
							}

							if (normalClickInteractions)
								slotData.ClickInteractions(ref item);
						}

						if (!item.IsAir && !item.favorited && item.TryGetGlobalItem(out VacuumToStorageItem vacuumItem) && vacuumItem.favorited)
							item.favorited = true;

						if (Witch.cursesUI && item.ModItem is Enchantment enchantment && enchantment.CanBeCursed || Witch.rerollUI && item.ModItem is Enchantment enchantment1 && enchantment1.IsRerollable) {
							slotData.Draw(spriteBatch, item, ItemSlotContextID.OliveGreen, glowHue, glowTime);
						}
						else {
							if (isTrash && !item.favorited) {
								slotData.Draw(spriteBatch, item, ItemSlotContextID.MarkedTrash, glowHue, glowTime);
							}
							else {
								slotData.Draw(spriteBatch, item, ItemSlotContextID.Normal, glowHue, glowTime);
							}
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
				if (MasterUIManager.LeftMouseClicked)
					MasterUIManager.ClickTypingBar(SearchID);
			}

			if (MasterUIManager.TypingOnBar(SearchID)) {
				if (MasterUIManager.LeftMouseClicked && !mouseHoveringSearchBar || MasterUIManager.ShouldStopTypingOnBar()) {
					MasterUIManager.StopTypingOnBar();
				}
				else {
					PlayerInput.WritingText = true;
					Main.instance.HandleIME();
					MasterUIManager.TypingBarString = Main.GetInputText(MasterUIManager.TypingBarString);
				}
			}

			//Text Buttons Draw
			for (int buttonIndex = 0; buttonIndex < EnchantmentStorageButtonID.Count; buttonIndex++) {
				UITextData textButton = textButtons[buttonIndex];
				textButton.Draw(spriteBatch);
				if (MasterUIManager.MouseHovering(textButton, true)) {
					ButtonScale[buttonIndex] += 0.05f;

					if (ButtonScale[buttonIndex] > buttonScaleMaximum)
						ButtonScale[buttonIndex] = buttonScaleMaximum;

					if (MasterUIManager.LeftMouseClicked) {
						MasterUIManager.TryResetTypingBar(GetUI_ID(WE_UI_ID.EnchantmentStorageSearch));
						if (managingTrash && buttonIndex != EnchantmentStorageButtonID.ManageTrash)
							managingTrash = false;

						if (managingOfferdItems && buttonIndex != EnchantmentStorageButtonID.ManageOfferedItems)
							managingOfferdItems = false;

						if (quickCrafting && buttonIndex != EnchantmentStorageButtonID.QuickCrafting)
							quickCrafting = false;

						switch (buttonIndex) {
							case EnchantmentStorageButtonID.LootAll:
								LootAll();
								break;
							case EnchantmentStorageButtonID.DepositAll:
								DepositAll(Main.LocalPlayer.inventory.TakePlayerInventory40());
								break;
							case EnchantmentStorageButtonID.QuickStack:
								QuickStack(Main.LocalPlayer.inventory.TakePlayerInventory40(), Main.LocalPlayer);
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
							case EnchantmentStorageButtonID.QuickCrafting:
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
			bool draggingScrollPanel = MasterUIManager.PanelBeingDragged == GetUI_ID(WE_UI_ID.EnchantmentStorageScrollPanel);
			if (MasterUIManager.PanelBeingDragged == GetUI_ID(WE_UI_ID.EnchantmentStorageScrollPanel)) {
				scrollPanelY.Clamp(scrollPanelMinY, scrollPanelMaxY);
			}
			else {
				int scrollPanelRange = scrollPanelMaxY - scrollPanelMinY;
				int offset = scrollPanelPosition * scrollPanelRange / possiblePanelPositions;
				scrollPanelY = offset + scrollPanelMinY;
			}

			//Scroll Bar Hover
			if (scrollBarData.MouseHovering()) {
				if (MasterUIManager.LeftMouseClicked) {
					MasterUIManager.UIBeingHovered = UI_ID.None;
					scrollPanelY = Main.mouseY - scrollPanelSize / 2;
					scrollPanelY.Clamp(scrollPanelMinY, scrollPanelMaxY);
				}
			}

			//Scroll Panel Hover and Drag
			UIPanelData scrollPanelData = new(GetUI_ID(WE_UI_ID.EnchantmentStorageScrollPanel), scrollBarLeft + scrollPanelXOffset, scrollPanelY, scrollPanelSize, scrollPanelSize, mouseColor);
			if (scrollPanelData.MouseHovering()) {
				scrollPanelData.TryStartDraggingUI();
			}

			if (scrollPanelData.ShouldDragUI()) {
				MasterUIManager.DragUI(out _, out scrollPanelY);
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
				MasterUIManager.DragUI(out wePlayer.EnchantmentStorageUILeft, out wePlayer.EnchantmentStorageUITop);

			int scrollWheelTicks = MasterUIManager.ScrollWheelTicks;
			if (scrollWheelTicks != 0 && UIManager.HoveringEnchantmentStorage && MasterUIManager.NoPanelBeingDragged) {
				if (scrollPanelPosition > 0 && scrollWheelTicks < 0 || scrollPanelPosition < possiblePanelPositions && scrollWheelTicks > 0) {
					SoundEngine.PlaySound(SoundID.MenuTick);
					scrollPanelPosition += scrollWheelTicks;
				}
			}
		}

		private static void UpdateQuickCraftInventory(WEPlayer wePlayer) {
			if (recipiesUpdated) {
				recipiesUpdated = false;
				availableEnchantmentRecipes = new();
				quickCraftItemCounts = new();
				for (int i = Main.availableRecipe.Length - 1; i >= 0; i--) {
					int recipeNum = Main.availableRecipe[i];
					Recipe r = Main.recipe[recipeNum];
					if (r.createItem.IsAir || availableEnchantmentRecipes.ContainsKey(r.createItem.type))
						continue;

					if (r.createItem.ModItem is Enchantment)
						availableEnchantmentRecipes.Add(r.createItem.type, recipeNum);
				}

				IEnumerable<Item> storageItems = wePlayer.enchantmentStorageItems.GetSortedEnchantments();
				foreach (Item storageItem in storageItems) {
					quickCraftItemCounts.AddOrCombine(storageItem.type, storageItem.stack);
				}

				IEnumerable<Item> allOtherCraftableEnchantments = AllEnchantments.Where(all => availableEnchantmentRecipes.ContainsKey(all.type) || storageItems.Select(i => i.type).Contains(all.type));
				foreach (Item otherCraftableEnchantment in allOtherCraftableEnchantments) {
					quickCraftItemCounts.TryAdd(otherCraftableEnchantment.type, 0);
				}

				quickCraftInventory = allOtherCraftableEnchantments.OrderByDescending(i => availableEnchantmentRecipes.ContainsKey(i.type)).ToArray();
			}
		}

		public static bool CanBeStored(Item item) {
			if (item?.ModItem is WEModItem weModItem)
				return weModItem.CanBeStoredInEnchantmentStorage;

			return false;
		}
		public static bool RoomInStorage(Item item, Player player = null) {
			if (item.NullOrAir())
				return false;

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
		public static bool CanAutoOfferItem(Item item, Player player) => !item.NullOrAir() && item.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem) && !enchantedItem.Modified && player.GetWEPlayer().allOfferedItems.Contains(item.type.GetItemIDOrName());
		public static bool TryAutoOfferItem(ref Item item, Player player) {
			if (CanAutoOfferItem(item, player)) {
				EnchantingTableUI.OfferItem(ref item);
				item.TurnToAir();
				return true;
			}

			return false;
		}
		public static bool CanVacuumItem(Item item, Player player) => !item.NullOrAir() && WEPlayer.LocalWEPlayer.highestTableTierUsed >= 0 && WEPlayer.LocalWEPlayer.vacuumItemsIntoEnchantmentStorage && CanBeStored(item) && (RoomInStorage(item) || CanBeTrashed(item));
		public static bool CanBeTrashed(Item item) => WEPlayer.LocalWEPlayer.trashEnchantmentsFullNames.Contains(item.type.GetItemIDOrName());
		public static bool TryVacuumItem(ref Item item, Player player) {
			if (CanVacuumItem(item, player))
				return DepositAll(ref item);

			return false;
		}
		public static bool DepositAll(ref Item item) => DepositAll(new Item[] { item });
		public static bool DepositAll(Item[] inv) {
			bool transferedAnyItem = Restock(inv, false);

			int storageIndex = 0;
			List<Item> acceptedItemsForUncraftTrashCheck = new();
			for (int i = 0; i < inv.Length; i++) {
				ref Item item = ref inv[i];
				if (!item.favorited && CanBeStored(item)) {
					while (storageIndex < WEPlayer.LocalWEPlayer.enchantmentStorageItems.Length && WEPlayer.LocalWEPlayer.enchantmentStorageItems[storageIndex].type > ItemID.None) {
						storageIndex++;
					}

					if (storageIndex < WEPlayer.LocalWEPlayer.enchantmentStorageItems.Length) {
						WEPlayer.LocalWEPlayer.enchantmentStorageItems[storageIndex] = item.Clone();
						acceptedItemsForUncraftTrashCheck.Add(WEPlayer.LocalWEPlayer.enchantmentStorageItems[storageIndex]);
						item.TurnToAir();
						transferedAnyItem = true;
					}
					else {
						break;
					}
				}
			}

			UncraftTrash(acceptedItemsForUncraftTrashCheck.ToArray());

			if (transferedAnyItem) {
				SoundEngine.PlaySound(SoundID.Grab);
				Recipe.FindRecipes(true);
			}

			return transferedAnyItem;
		}
		public static bool Contains(Item item, Player player) => !item.NullOrAir() && player.TryGetWEPlayer(out WEPlayer wePlayer) && wePlayer.enchantmentStorageItems.Select(i => i.type).Contains(item.type);
		public static bool QuickStack(ref Item item, Player player) {
			if (Contains(item, player))
				return TryVacuumItem(ref item, player);

			return false;
		}
		public static void QuickStack(Item[] inv, Player player) {
			for (int i = 0; i < inv.Length; i++) {
				ref Item item = ref inv[i];
				QuickStack(ref item, player);
			}
		}
		public static bool Restock(ref Item item) => Restock(new Item[] { item });
		public static bool Restock(Item[] inv, bool playSound = true) {
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
							if (ItemLoader.TryStackItems(storageItem, item, out int transfered)) {
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
			MasterUIManager.SortItems(ref WEPlayer.LocalWEPlayer.enchantmentStorageItems, false);
			IEnumerable<Item> containments = WEPlayer.LocalWEPlayer.enchantmentStorageItems.Where(i => i.ModItem is ContainmentItem).OrderBy(i => i.type);
			IEnumerable<Item> powerBoosters = WEPlayer.LocalWEPlayer.enchantmentStorageItems.Where(i => i.ModItem is PowerBooster or UltraPowerBooster).OrderBy(i => i.type);
			IEnumerable<Item> enchantments = WEPlayer.LocalWEPlayer.enchantmentStorageItems.GetSortedEnchantments();
			IEnumerable<Item> goodEnchantments = enchantments.Where(i => i.favorited || !WEPlayer.LocalWEPlayer.trashEnchantmentsFullNames.Contains(i.type.GetItemIDOrName()));
			IEnumerable<Item> trashEnchantments = enchantments.Where(i => !i.favorited && WEPlayer.LocalWEPlayer.trashEnchantmentsFullNames.Contains(i.type.GetItemIDOrName()));
			IEnumerable<Item> otherItems = WEPlayer.LocalWEPlayer.enchantmentStorageItems.Where(i => !i.NullOrAir() && (i.ModItem == null || i.ModItem is not (Enchantment or ContainmentItem or PowerBooster or UltraPowerBooster)));
			IEnumerable<Item> airItems = WEPlayer.LocalWEPlayer.enchantmentStorageItems.Where(i => i.NullOrAir());
			WEPlayer.LocalWEPlayer.enchantmentStorageItems = containments.Concat(powerBoosters).Concat(otherItems).Concat(goodEnchantments).Concat(trashEnchantments).Concat(airItems).ToArray();
			glowTime = 300;
			glowHue = 0.5f;

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
			WEPlayer.LocalWEPlayer.vacuumItemsIntoEnchantmentStorage = !WEPlayer.LocalWEPlayer.vacuumItemsIntoEnchantmentStorage;
		}
		private static void ToggleMarkTrash() {
			markingTrash = !markingTrash;
		}
		public static void UncraftTrash(Item item) => UncraftTrash(new Item[] { item });
		private static void UncraftTrash(Item[] inv) {
			crafting = true;
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
							if (recipeNum.TryCraftItem(out Item crafted, true))
								uncraftedExtraItems.AddOrCombine(crafted.type, crafted.stack);
						}
					}
				}
			}

			QuickSpawnAllCraftedItems();

			Recipe.FindRecipes();
			crafting = false;
		}
		private static void RevertAllToBasic() {
			crafting = true;
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
					foreach (int slot in itemType.Value) {
						int stack = WEPlayer.LocalWEPlayer.enchantmentStorageItems[slot].stack;
						for (int i = 0; i < stack; i++) {
							if (recipeNum.TryCraftItem(out Item crafted, true)) {
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
			crafting = false;
		}
		public static bool TryCraftItem(this int recipeNum, out Item crafted, bool ignoreTileAndEnvironmentRequirements = false, bool reverseOrder = false) {
			crafted = null;
			Recipe.FindRecipes();
			if (!reverseOrder) {
				for (int availableRecipeIndex = 0; availableRecipeIndex < Main.numAvailableRecipes; availableRecipeIndex++) {
					int availableRecipeNum = Main.availableRecipe[availableRecipeIndex];
					if (recipeNum == availableRecipeNum) {
						crafted = recipeNum.CraftItem();

						return true;
					}
				}
			}
			else {
				for (int availableRecipeIndex = Main.numAvailableRecipes - 1; availableRecipeIndex >= 0; availableRecipeIndex--) {
					int availableRecipeNum = Main.availableRecipe[availableRecipeIndex];
					if (recipeNum == availableRecipeNum) {
						crafted = recipeNum.CraftItem();

						return true;
					}
				}
			}

			if (ignoreTileAndEnvironmentRequirements) {
				Recipe recipe = Main.recipe[recipeNum];
				if (Recipe.CollectedEnoughItemsToCraftRecipeNew(recipe) && RecipeLoader.RecipeAvailable(recipe)) {
					crafted = recipeNum.CraftItem();
					return true;
				}
			}

			return false;
		}
		public static Item CraftItem(this int recipeNum) {
			Recipe r = Main.recipe[recipeNum];
			Item crafted = r.createItem.Clone();
			crafted.Prefix(-1);
			r.Create();
			RecipeLoader.OnCraft(crafted, r, new());
			Recipe.FindRecipes(true);

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
		public static bool HasEnchantments(WEPlayer wePlayer, IDictionary<int, int> items) {
			if (items.Count < 1)
				return true;

			for (int i = 0; i < wePlayer.enchantmentStorageItems.Length; i++) {
				Item item = wePlayer.enchantmentStorageItems[i];
				if (item.ModItem is not Enchantment)
					continue;

				if (items.TryGetValue(item.type, out int stack)) {
					int transferAmmount = Math.Min(stack, item.stack);
					stack -= transferAmmount;
					if (stack <= 0) {
						items.Remove(item.type);
						if (items.Count < 1)
							return true;
					}
					else {
						items[item.type] = stack;
					}
				}
			}

			return false;
		}
		public static void ConsumeEnchantments(WEPlayer wePlayer, IDictionary<int, int> items) {
			if (items.Count < 1)
				return;

			for (int i = wePlayer.enchantmentStorageItems.Length - 1; i >= 0; i--) {
				ref Item item = ref wePlayer.enchantmentStorageItems[i];
				if (item.ModItem is not Enchantment)
					continue;

				int type = item.type;
				if (items.TryGetValue(type, out int stack)) {
					int transferAmmount = Math.Min(stack, item.stack);
					//ref Item item = ref wePlayer.enchantmentStorageItems[itemInfo.Key];

					stack -= transferAmmount;
					items[item.type] = stack;
					item.stack -= transferAmmount;
					if (item.stack < 1)
						item.TurnToAir();

					if (stack <= 0) {
						items.Remove(type);
						if (items.Count < 1)
							return;
					}
				}
			}
		}

		internal static void FindRecipes(On_Recipe.orig_FindRecipes orig, bool canDelayCheck) {
			orig(canDelayCheck);

			if (!canDelayCheck)
				recipiesUpdated = true;
		}
	}
}