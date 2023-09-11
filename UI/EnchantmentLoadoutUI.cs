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
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WeaponEnchantments.Items.Enchantments;
using androLib.Common.Utility;
using androLib.Common.Globals;
using androLib.UI;
using androLib;

namespace WeaponEnchantments.UI
{
	public static class EnchantmentLoadoutUI
	{
		public class EnchantmentLoadoutButtonID
		{
			public const int LoadoutSelect = 0;
			public const int All = 1;
			public const int HeldItem = 2;
			public const int Armor = 3;
			public const int Accessories = 4;
			public const int Delete = 5;
			public const int Count = 6;
		}
		private static int GetUI_ID(int id) => MasterUIManager.GetUI_ID(id, WE_UI_ID.EnchantmentLoadout_UITypeID);
		public static int ID => GetUI_ID(WE_UI_ID.EnchantmentLoadoutUI);
		public static int EnchantmentLoadoutUIDefaultLeft => 745;
		public static int EnchantmentLoadoutUIDefaultTop => 290;
		public static Color PanelColor => new Color(26, 2, 56, UIManager.UIAlpha);
		private static int Spacing => 4;
		private static int PanelBorder => 10;
		private static int ButtonBorderY => 0;
		private static int ButtonBorderX => 6;
		public const float buttonScaleMinimum = 0.75f;
		public const float buttonScaleMaximum = 1f;
		private static List<float[]> buttonScale = new();
		public static List<float[]> ButtonScale {
			get {
				if (buttonScale.Count < WEPlayer.LocalWEPlayer.enchantmentLoadouts.Count) {
					for (int i = buttonScale.Count; i < WEPlayer.LocalWEPlayer.enchantmentLoadouts.Count; i++) {
						buttonScale.Add(Enumerable.Repeat(buttonScaleMinimum, buttonNames.Length).ToArray());
					}
				}

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
		public const int MaxLoadouts = 20;
		public static string[] buttonNames = { 
			null, 
			EnchantmentStorageTextID.All.ToString().Lang_WE(L_ID1.EnchantmentStorageText), 
			EnchantmentStorageTextID.HeldItem.ToString().Lang_WE(L_ID1.EnchantmentStorageText), 
			EItemType.Armor.ToString().Lang_WE(L_ID1.Tooltip, L_ID2.ItemType), 
			EItemType.Accessories.ToString().Lang_WE(L_ID1.Tooltip, L_ID2.ItemType),
			EnchantmentStorageTextID.Delete.ToString().Lang_WE(L_ID1.EnchantmentStorageText),
		};
		public static bool useingScrollBar = false;
		public static int availableSlotRow = -1;
		public static int availableSlotIndex = -1;
		public static bool skipAvailableSlotReset = false;
		public static void PostDrawInterface(SpriteBatch spriteBatch) {
			WEPlayer wePlayer = WEPlayer.LocalWEPlayer;
			if (wePlayer.displayEnchantmentLoadoutUI) {

				#region Pre UI

				//Prevent Trash can and other mouse overides when using enchanting table
				if (ItemSlot.ShiftInUse && MasterUIManager.NoUIBeingHovered && (Main.mouseItem.IsAir && !Main.HoverItem.IsAir || Main.cursorOverride == CursorOverrideID.TrashCan)) {
					//if (!wePlayer.CheckShiftClickValid(ref Main.HoverItem) || Main.cursorOverride == CursorOverrideID.TrashCan)
					//	Main.cursorOverride = -1;

					Main.cursorOverride = AvailableSlot(Main.HoverItem) ? CursorOverrideID.InventoryToChest : -1;
				}
				else {
					if (skipAvailableSlotReset) {
						skipAvailableSlotReset = false;
					}
					else {
						availableSlotRow = -1;
						availableSlotIndex = -1;
					}
				}

				#endregion

				#region Data

				Color mouseColor = MasterUIManager.MouseColor;

				//ItemSlots Data 1/2
				List<Item[]> inventory = GetFixLoadout(wePlayer);

				int itemSlotColumns = EnchantingTableUI.MaxEnchantmentSlots;
				int itemSlotRowsDisplayed = 11;//TODO: Calculate 9-11 based on vanilla slot unlocks
				int itemSlotTotalRows = inventory.Count;
				int itemSlotSpaceWidth = MasterUIManager.ItemSlotSize + Spacing;
				int itemSlotSpaceHeight = MasterUIManager.ItemSlotSize + Spacing;
				int itemSlotsWidth = (itemSlotColumns - 1) * itemSlotSpaceWidth + MasterUIManager.ItemSlotSize;
				int itemSlotsHeight = (itemSlotRowsDisplayed - 1) * itemSlotSpaceHeight + MasterUIManager.ItemSlotSize;
				int itemSlotsLeft = wePlayer.EnchantmentLoadoutUILeft + PanelBorder;

				//Name Data
				int nameLeft = itemSlotsLeft;//itemSlotsLeft + (itemSlotsWidth - nameWidth) / 2;
				int nameTop = wePlayer.EnchantmentLoadoutUITop + PanelBorder;
				string name = EnchantmentStorageTextID.EnchantmentLoadouts.ToString().Lang_WE(L_ID1.EnchantmentStorageText);
				UITextData nameData = new(WE_UI_ID.None, nameLeft, nameTop, name, 1f, mouseColor);

				//Panel Data 1/2
				int panelBorderTop = nameData.Height + Spacing + 2;

				//ItemSlots Data 2/2
				int itemSlotsTop = wePlayer.EnchantmentLoadoutUITop + panelBorderTop;

				//Scroll Bar Data 1/2
				useingScrollBar = itemSlotRowsDisplayed != itemSlotTotalRows;
				int scrollBarLeft = itemSlotsLeft + itemSlotsWidth + Spacing;
				int scrollBarWidth = useingScrollBar ? 30 : 0;
				int possiblePanelPositions = itemSlotTotalRows - itemSlotRowsDisplayed;
				if (possiblePanelPositions < 1)
					possiblePanelPositions = 1;

				scrollPanelPosition.Clamp(0, possiblePanelPositions);

				//Add button Data
				int buttonsLeft = scrollBarLeft + scrollBarWidth + Spacing;
				int currentButtonTop = nameTop;
				string addButton = EnchantmentStorageTextID.Add.ToString().Lang_WE(L_ID1.EnchantmentStorageText);
				TextData addButtonTextData = new(addButton);
				UIButtonData addButtonData = new(GetUI_ID(WE_UI_ID.EnchantmentLoadoutAddTextButton), buttonsLeft, currentButtonTop, addButtonTextData, mouseColor, ButtonBorderX, ButtonBorderY, EnchantingTableUI.BackGroundColor, EnchantingTableUI.HoverColor);
				int longestButtonsWidth = addButtonData.Width;

				//Add from Equipped Enchantments button Data
				int addfromEquippedEnchantmentsButtonLeft = buttonsLeft + addButtonData.Width + Spacing;
				string addfromEquippedEnchantmentsButton = EnchantmentStorageTextID.AddFromEquippedEnchantments.ToString().Lang_WE(L_ID1.EnchantmentStorageText);
				TextData addfromEquippedEnchantmentsButtonTextData = new(addfromEquippedEnchantmentsButton);
				UIButtonData addfromEquippedEnchantmentsButtonData = new(GetUI_ID(WE_UI_ID.EnchantmentLoadoutAddFromEquippedEnchantmentsTextButton), addfromEquippedEnchantmentsButtonLeft, currentButtonTop, addfromEquippedEnchantmentsButtonTextData, mouseColor, ButtonBorderX, ButtonBorderY, EnchantingTableUI.BackGroundColor, EnchantingTableUI.HoverColor);

				//UI button cleanup
				currentButtonTop += addButtonData.Height + Spacing;
				longestButtonsWidth += Spacing + addfromEquippedEnchantmentsButtonData.Width;

				//Text buttons Data
				UITextData[,] textButtons = new UITextData[wePlayer.enchantmentLoadouts.Count, buttonNames.Length];
				for (int buttonRow = 0; buttonRow < wePlayer.enchantmentLoadouts.Count; buttonRow++) {
					int currentButtonLeft = buttonsLeft;
					int buttonsWidth = 0;
					for (int buttonIndex = 0; buttonIndex < buttonNames.Length; buttonIndex++) {
						string text = buttonNames[buttonIndex] ?? wePlayer.enchantmentLoadouts.ElementAt(buttonRow).Key;
						float scale = ButtonScale[buttonRow][buttonIndex];
						Color color = buttonIndex == 0 && text == wePlayer.displayedLoadout ? EnchantmentStorage.SelectedTextGray : mouseColor;
						UITextData thisButton = new(GetUI_ID(WE_UI_ID.EnchantmentLoadoutUITextButton), currentButtonLeft, currentButtonTop, text, scale, color, ancorBotomLeft: true);
						textButtons[buttonRow, buttonIndex] = thisButton;
						int xOffset = buttonIndex < buttonNames.Length - 1 ? thisButton.BaseWidth + Spacing : thisButton.Width;
						buttonsWidth += xOffset;
						currentButtonLeft += xOffset;
					}

					longestButtonsWidth = Math.Max(longestButtonsWidth, buttonsWidth);
					currentButtonTop += textButtons[buttonRow, 0].BaseHeight;
				}

				//Panel Data 2/2
				int panelBorderRightOffset = Spacing + longestButtonsWidth + PanelBorder;
				int panelWidth = itemSlotsWidth + Spacing + scrollBarWidth + PanelBorder + panelBorderRightOffset;
				int panelHeight = itemSlotsHeight + PanelBorder + panelBorderTop;
				UIPanelData panel = new(ID, wePlayer.EnchantmentLoadoutUILeft, wePlayer.EnchantmentLoadoutUITop, panelWidth, panelHeight, PanelColor);

				#endregion

				//Panel Draw
				panel.Draw(spriteBatch);

				//ItemSlots Draw
				int startRow = scrollPanelPosition;
				int itemSlotY = itemSlotsTop;
				int endRow = startRow + itemSlotRowsDisplayed;
				for (int rowNum = startRow; rowNum < endRow; rowNum++) {
					Item[] enchantmentSlots = inventory[rowNum];
					int itemSlotX = itemSlotsLeft;
					for (int enchantmentSlotIndex = 0; enchantmentSlotIndex < EnchantingTableUI.MaxEnchantmentSlots; enchantmentSlotIndex++) {
						UIItemSlotData enchantmentSlot = new(GetUI_ID(WE_UI_ID.EnchantmentLoadoutUIItemSlot), itemSlotX, itemSlotY);
						ref Item item = ref enchantmentSlots[enchantmentSlotIndex];
						bool isUtilitySlot = enchantmentSlotIndex == EnchantingTableUI.MaxEnchantmentSlots - 1;
						bool isArmor = rowNum >= 1 && rowNum <= 3;
						EItemType itemType = rowNum switch {
							0 => EItemType.Weapons,
							_ when(isArmor) => EItemType.Armor,
							_ => EItemType.Accessories,
						};

						int armorSlotSpecificID = isArmor ? rowNum - 1 : -1;

						bool canUseSlot =  EnchantingTableUI.UseEnchantmentSlot(itemType, enchantmentSlotIndex);
						if (enchantmentSlot.MouseHovering()) {
							if (ItemSlot.ShiftInUse && !item.IsAir) {
								Main.cursorOverride = CursorOverrideID.TrashCan;
								if (MasterUIManager.LeftMouseClicked) {
									item = new();
									SoundEngine.PlaySound(SoundID.MenuTick);
								}
							}
							else if (MasterUIManager.RightMouseClicked && !item.IsAir) {
								item = new();
								SoundEngine.PlaySound(SoundID.MenuTick);
							}
							else if (Main.mouseItem.IsAir && !item.IsAir) {
								Main.cursorOverride = CursorOverrideID.TrashCan;
								if (MasterUIManager.LeftMouseClicked) {
									item = new();
									SoundEngine.PlaySound(SoundID.MenuTick);
								}
							}
							else {
								if (EnchantingTableUI.ValidItemForLoadoutEnchantmentSlot(Main.mouseItem, itemType, enchantmentSlotIndex, isUtilitySlot, armorSlotSpecificID)) {
									bool canAcceptEnchantment = Main.mouseItem.ModItem is Enchantment enchantment && EnchantingTableUI.CheckUniqueSlot(enchantment, EnchantingTableUI.FindSwapEnchantmentSlot(enchantment, enchantmentSlots), enchantmentSlotIndex) && Main.mouseItem.type != item.type;
									if (canAcceptEnchantment) {
										Main.cursorOverride = CursorOverrideID.CameraDark;
										if (MasterUIManager.LeftMouseClicked) {
											item = Main.mouseItem.Clone();
											item.stack = 1;
											SoundEngine.PlaySound(SoundID.MenuTick);
										}
									}
								}
							}
						}

						bool isAvailableSlot = availableSlotRow == rowNum && availableSlotIndex == enchantmentSlotIndex;
						enchantmentSlot.Draw(spriteBatch, item, canUseSlot ? isAvailableSlot ? ItemSlotContextID.Gold : isUtilitySlot ? ItemSlotContextID.Favorited : ItemSlotContextID.Normal : ItemSlotContextID.Red);
						itemSlotX += itemSlotSpaceWidth;
					}

					itemSlotY += itemSlotSpaceHeight;
				}

				//Name Draw
				nameData.Draw(spriteBatch);

				//Add Button Draw
				addButtonData.Draw(spriteBatch);

				//Add from Equipped Enchantments button Draw
				addfromEquippedEnchantmentsButtonData.Draw(spriteBatch);

				//Text Buttons Draw
				for (int buttonRow = 0; buttonRow < wePlayer.enchantmentLoadouts.Count; buttonRow++) {
					for (int buttonIndex = 0; buttonIndex < buttonNames.Length; buttonIndex++) {
						UITextData textButton = textButtons[buttonRow, buttonIndex];
						textButton.Draw(spriteBatch);
						if (MasterUIManager.MouseHovering(textButton, true)) {
							float temp = buttonScale[buttonRow][buttonIndex];
							buttonScale[buttonRow][buttonIndex] += 0.05f;

							if (buttonScale[buttonRow][buttonIndex] > buttonScaleMaximum)
								buttonScale[buttonRow][buttonIndex] = buttonScaleMaximum;

							if (MasterUIManager.LeftMouseClicked) {
								switch (buttonIndex) {
									case EnchantmentLoadoutButtonID.LoadoutSelect:
										wePlayer.displayedLoadout = textButton.Text;

										break;
									case EnchantmentLoadoutButtonID.All:
									case EnchantmentLoadoutButtonID.HeldItem:
									case EnchantmentLoadoutButtonID.Armor:
									case EnchantmentLoadoutButtonID.Accessories:
										if (TrySwapToLoadout((LoadoutSwapID)(buttonIndex - 1), wePlayer.enchantmentLoadouts.ElementAt(buttonRow).Key))
											SoundEngine.PlaySound(SoundID.Grab);//SoundEngine.PlaySound(SoundID.AbigailUpgrade);

										break;
									case EnchantmentLoadoutButtonID.Delete:
										DeleteLoadout(wePlayer, buttonRow);

										break;
								}

								SoundEngine.PlaySound(SoundID.MenuTick);
							}
						}
						else {
							buttonScale[buttonRow][buttonIndex] -= 0.05f;

							if (buttonScale[buttonRow][buttonIndex] < buttonScaleMinimum)
								buttonScale[buttonRow][buttonIndex] = buttonScaleMinimum;
						}
					}
				}

				//Add button Hover
				if (addButtonData.MouseHovering()) {
					if (MasterUIManager.LeftMouseClicked)
						AddNewBlankLoadout(wePlayer);
				}

				//Add from Equipped Enchantments button Hover
				if (addfromEquippedEnchantmentsButtonData.MouseHovering()) {
					if (MasterUIManager.LeftMouseClicked)
						AddNewLoadoutFromEquippedEnchantments(wePlayer);
				}

				//Scroll Bar
				if (useingScrollBar) {
					//Scroll Bar Data 2/2
					int scrollBarTop = wePlayer.EnchantmentLoadoutUITop + PanelBorder;
					UIPanelData scrollBarData = new(GetUI_ID(WE_UI_ID.EnchantmentLoadoutUIScrollBar), scrollBarLeft, scrollBarTop, scrollBarWidth, panelHeight - PanelBorder * 2, new Color(10, 1, 30, UIManager.UIAlpha));

					//Scroll Panel Data 1/2
					int scrollPanelXOffset = 1;
					int scrollPanelSize = scrollBarWidth - scrollPanelXOffset * 2;
					int scrollPanelMinY = scrollBarData.TopLeft.Y + scrollPanelXOffset;
					int scrollPanelMaxY = scrollBarData.BottomRight.Y - scrollPanelSize - scrollPanelXOffset;

					//Scroll Bar Draw
					scrollBarData.Draw(spriteBatch);

					//Scroll Panel Data 2/2
					bool draggingScrollPanel = MasterUIManager.PanelBeingDragged == GetUI_ID(WE_UI_ID.EnchantmentLoadoutUIScrollPanel);
					if (MasterUIManager.PanelBeingDragged == GetUI_ID(WE_UI_ID.EnchantmentLoadoutUIScrollPanel)) {
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
					UIPanelData scrollPanelData = new(GetUI_ID(WE_UI_ID.EnchantmentLoadoutUIScrollPanel), scrollBarLeft + scrollPanelXOffset, scrollPanelY, scrollPanelSize, scrollPanelSize, mouseColor);
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
				}

				//Panel Hover and Drag
				if (panel.MouseHovering()) {
					panel.TryStartDraggingUI();
				}

				if (panel.ShouldDragUI())
					MasterUIManager.DragUI(out wePlayer.EnchantmentLoadoutUILeft, out wePlayer.EnchantmentLoadoutUITop);

				if (useingScrollBar) {
					int scrollWheelTicks = MasterUIManager.ScrollWheelTicks;
					if (scrollWheelTicks != 0 && UIManager.HoveringEnchantmentLoadoutUI && MasterUIManager.NoPanelBeingDragged) {
						if (scrollPanelPosition > 0 && scrollWheelTicks < 0 || scrollPanelPosition < possiblePanelPositions && scrollWheelTicks > 0) {
							SoundEngine.PlaySound(SoundID.MenuTick);
							scrollPanelPosition += scrollWheelTicks;
						}
					}
				}
			}
		}
		private static List<Item[]> GetFixLoadout(WEPlayer wePlayer) {
			if (wePlayer.displayedLoadout == null)
				wePlayer.displayedLoadout = wePlayer.enchantmentLoadouts.First().Key;

			List<Item[]> loadout = wePlayer.enchantmentLoadouts[wePlayer.displayedLoadout];
			int requiredSize = GetRequiredLoudoutSize(wePlayer);
			if (loadout.Count > requiredSize) {
				for (int i = loadout.Count - 1; i >= requiredSize; i--) {
					Item[] enchantments = loadout[i];
					for (int k = 0; k < enchantments.Length; k++) {
						ref Item enchantment = ref enchantments[k];
						if (!enchantment.IsAir)
							StorageManager.TryReturnItemToPlayer(ref enchantment, wePlayer.Player, true);
					}

					loadout.RemoveAt(i);
				}

				Main.NewText(EnchantmentStorageTextID.LoadoutSizeChanged.ToString().Lang_WE(L_ID1.EnchantmentStorageText));
			}
			else if (loadout.Count < requiredSize) {
				for (int i = loadout.Count; i < requiredSize; i++) {
					loadout.Add(GetEnchantmentRow());
				}
			}

			return loadout;
		}
		private static int enchantmentLoadoutMaxCount = 15;
		public static bool AddNewBlankLoadout(WEPlayer wePlayer) {
			if (wePlayer.enchantmentLoadouts.Count < enchantmentLoadoutMaxCount) {
				int i = 1;
				string loadoutName = GetLoadoutName(i);
				while (wePlayer.enchantmentLoadouts.ContainsKey(loadoutName)) {
					loadoutName = GetLoadoutName(++i);
				}

				wePlayer.enchantmentLoadouts.Add(loadoutName, GetBlankLoadout(wePlayer));
				return true;
			}
			
			return false;
		}
		public static string GetLoadoutName(int num) => $"{EnchantmentStorageTextID.Loadout.ToString().Lang_WE(L_ID1.EnchantmentStorageText)} {num}";
		public static void AddNewLoadoutFromEquippedEnchantments(WEPlayer wePlayer) {
			if (!AddNewBlankLoadout(wePlayer))
				return;

			List<Item[]> loadout = wePlayer.enchantmentLoadouts.Last().Value;
			Item heldItem = wePlayer.Player.HeldItem;
			if (!heldItem.NullOrAir() && heldItem.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedHeldItem)) {
				for (int i = 0; i < enchantedHeldItem.enchantments.Length; i++) {
					Item enchantment = enchantedHeldItem.enchantments[i].Clone();
					if (enchantment.NullOrAir())
						continue;

					loadout[0][i] = enchantment;
				}
			}

			List<Item> armor = wePlayer.Equipment.GetAllArmor().ToList();
			for (int i = 0; i < armor.Count && i - 1 < loadout.Count; i++) {
				Item equipItem = armor[i];
				if (equipItem.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedEquipItem)) {
					for (int k = 0; k < enchantedEquipItem.enchantments.Length; k++) {
						Item enchantment = enchantedEquipItem.enchantments[k].Clone();
						if (enchantment.NullOrAir())
							continue;

						loadout[i + 1][k] = enchantment;
					}
				}
			}
		}
		private static int GetRequiredLoudoutSize(WEPlayer wePlayer) => wePlayer.Equipment.GetAllArmor().Count() + 1;
		private static List<Item[]> GetBlankLoadout(WEPlayer wePlayer) {
			int count = GetRequiredLoudoutSize(wePlayer);
			List<Item[]> loadout = new();
			for (int i = 0; i < count; i++) {
				loadout.Add(GetEnchantmentRow());
			}

			return loadout;
		}
		private static Item[] GetEnchantmentRow() => Enumerable.Repeat(new Item(), EnchantingTableUI.MaxEnchantmentSlots).ToArray();
		public static void Open(bool noSound = false) {
			if (WEPlayer.LocalWEPlayer.enchantmentLoadouts.Count < 1)
				AddNewBlankLoadout(WEPlayer.LocalWEPlayer);

			WEPlayer.LocalWEPlayer.displayEnchantmentLoadoutUI = true;
			Main.playerInventory = true;
			if (!noSound)
				SoundEngine.PlaySound(SoundID.MenuOpen);
		}
		public static void Close(bool noSound = false) {
			WEPlayer wePlayer = WEPlayer.LocalWEPlayer;
			wePlayer.displayEnchantmentLoadoutUI = false;
			if (wePlayer.Player.chest == -1) {
				if (!noSound)
					SoundEngine.PlaySound(SoundID.MenuClose);
			}

			Recipe.FindRecipes(true);
		}
		public enum LoadoutSwapID {
			All,
			HeldItem,
			Armor,
			Accessories
		}
		private static bool TrySwapToLoadout(LoadoutSwapID swapID, string loadoutName) {
			WEPlayer wePlayer = WEPlayer.LocalWEPlayer;

			//Can Swap
			List<Item[]> loadout = wePlayer.enchantmentLoadouts[loadoutName];
			Item[] allArmor = wePlayer.Equipment.GetAllArmor().ToArray();
			bool swapWeapon = swapID == LoadoutSwapID.HeldItem || swapID == LoadoutSwapID.All;
			bool swapArmmor = swapID == LoadoutSwapID.Armor || swapID == LoadoutSwapID.All;
			bool swapAccessories = swapID == LoadoutSwapID.Accessories || swapID == LoadoutSwapID.All;

			//Cost check
			bool canSwapHeldItem = false;
			EnchantedHeldItem enchantedHeldItem = null;
			if (swapWeapon) {
				if (wePlayer.Player.HeldItem.IsAir) {
					if (swapID == LoadoutSwapID.HeldItem) {
						Main.NewText(EnchantmentStorageTextID.NoHeldItem.ToString().Lang_WE(L_ID1.EnchantmentStorageText));

						return false;
					}
				}
				else {
					if (wePlayer.Player.HeldItem.TryGetEnchantedHeldItem(out enchantedHeldItem)) {
						int cost = GetEnchantentsCost(loadout[0]);
						if (enchantedHeldItem.level < cost) {
							Main.NewText(EnchantmentStorageTextID.NotHighEnoughLevel.ToString().Lang_WE(L_ID1.EnchantmentStorageText, new string[] { wePlayer.Player.HeldItem.Name }));

							return false;
						}

						canSwapHeldItem = true;
					}
				}
			}

			bool canSwapAnyArmor = false;
			if (swapArmmor) {
				for (int i = 0; i < 3; i++) {
					Item item = allArmor[i];
					if (item.TryGetEnchantedEquipItem(out EnchantedEquipItem enchantedEquipItem)) {
						int cost = GetEnchantentsCost(loadout[i + 1]);
						if (enchantedEquipItem.level < cost) {
							Main.NewText(EnchantmentStorageTextID.NotHighEnoughLevel.ToString().Lang_WE(L_ID1.EnchantmentStorageText, new string[] { item.Name }));

							return false;
						}

						canSwapAnyArmor = true;
					}
				}

				if (!canSwapAnyArmor) {
					if (swapID == LoadoutSwapID.Armor) {
						Main.NewText(EnchantmentStorageTextID.NoArmor.ToString().Lang_WE(L_ID1.EnchantmentStorageText));

						return false;
					}
				}
			}

			bool canSwapAnyAccessory = false;
			if (swapAccessories) {
				for (int i = 3; i < allArmor.Length; i++) {
					Item item = allArmor[i];
					if (item.TryGetEnchantedEquipItem(out EnchantedEquipItem enchantedEquipItem)) {
						int heldItemCost = GetEnchantentsCost(loadout[i + 1]);
						if (enchantedEquipItem.level < heldItemCost) {
							Main.NewText(EnchantmentStorageTextID.NotHighEnoughLevel.ToString().Lang_WE(L_ID1.EnchantmentStorageText, new string[] { item.Name }));

							return false;
						}

						canSwapAnyAccessory = true;
					}
				}

				if (!canSwapAnyAccessory) {
					if (swapID == LoadoutSwapID.Accessories) {
						Main.NewText(EnchantmentStorageTextID.NoAccessories.ToString().Lang_WE(L_ID1.EnchantmentStorageText));

						return false;
					}
				}
			}

			if (!canSwapHeldItem && !canSwapAnyArmor && !canSwapAnyAccessory) {
				Main.NewText(EnchantmentStorageTextID.NoItems.ToString().Lang_WE(L_ID1.EnchantmentStorageText));

				return false;
			}

			//Storage check
			SortedDictionary<int, int> neededEnchantments = new();
			if (swapWeapon) {
				if (!wePlayer.Player.HeldItem.IsAir) {
					Item[] enchantments = loadout[0];
					for (int i = 0; i < EnchantingTableUI.MaxEnchantmentSlots; i++) {
						int type = enchantments[i].type;
						if (type > ItemID.None)
							neededEnchantments.AddOrCombine(type, 1);
					}
				}
			}

			if (swapArmmor) {
				for (int k = 0; k < 3; k++) {
					if (!allArmor[k].IsAir) {
						Item[] enchantments = loadout[k + 1];
						for (int i = 0; i < EnchantingTableUI.MaxEnchantmentSlots; i++) {
							int type = enchantments[i].type;
							if (type > ItemID.None)
								neededEnchantments.AddOrCombine(type, 1);
						}
					}
				}
			}

			if (swapAccessories) {
				for (int k = 3; k < allArmor.Length; k++) {
					if (!allArmor[k].IsAir) {
						Item[] enchantments = loadout[k + 1];
						for (int i = 0; i < EnchantingTableUI.MaxEnchantmentSlots; i++) {
							int type = enchantments[i].type;
							if (type > ItemID.None)
								neededEnchantments.AddOrCombine(type, 1);
						}
					}
				}
			}

			SortedDictionary<int, int> neededFromStorage = new(neededEnchantments);
			if (wePlayer.vacuumItemsIntoEnchantmentStorage) {
				if (swapWeapon) {
					if (enchantedHeldItem != null) {
						foreach (Item enchantment in enchantedHeldItem.enchantments.All) {
							if (!enchantment.IsAir)
								neededFromStorage.TrySubtractRemove(enchantment.type, 1);
						}
					}
				}

				if (swapArmmor) {
					for (int k = 0; k < 3; k++) {
						if (!allArmor[k].IsAir && allArmor[k].TryGetEnchantedEquipItem(out EnchantedEquipItem enchantedEquipItem)) {
							foreach (Item enchantment in enchantedEquipItem.enchantments.All) {
								if (!enchantment.IsAir)
									neededFromStorage.TrySubtractRemove(enchantment.type, 1);
							}
						}
					}
				}

				if (swapAccessories) {
					for (int k = 3; k < allArmor.Length; k++) {
						if (!allArmor[k].IsAir && allArmor[k].TryGetEnchantedEquipItem(out EnchantedEquipItem enchantedEquipItem)) {
							foreach (Item enchantment in enchantedEquipItem.enchantments.All) {
								if (!enchantment.IsAir)
									neededFromStorage.TrySubtractRemove(enchantment.type, 1);
							}
						}
					}
				}
			}

			if (!EnchantmentStorage.HasEnchantments(wePlayer, neededFromStorage)) {
				string missingEnchantments = neededFromStorage.Select(p => $"{p.Key.CSI().Name} x{p.Value}").JoinList(", ");
				Main.NewText(EnchantmentStorageTextID.NotEnoughEnchantments.ToString().Lang_WE(L_ID1.EnchantmentStorageText, new string[] { missingEnchantments }));

				return false;
			}

			//Swap
			if (swapWeapon) {
				Item item = wePlayer.Player.HeldItem;
				if (enchantedHeldItem != null) {
					if (enchantedHeldItem.enchantments.TryReturnAllEnchantments(wePlayer, true)) {
						enchantedHeldItem.enchantments.ApplyLoadout(loadout[0]);
					}
				}
			}

			if (swapArmmor) {
				for (int i = 0; i < 3; i++) {
					Item item = allArmor[i];
					if (item.TryGetEnchantedEquipItem(out EnchantedEquipItem enchantedEquipItem)) {
						if (enchantedEquipItem.enchantments.TryReturnAllEnchantments(wePlayer, true)) {
							enchantedEquipItem.enchantments.ApplyLoadout(loadout[i + 1]);
						}
					}
				}
			}

			if (swapAccessories) {
				for (int i = 3; i < allArmor.Length; i++) {
					Item item = allArmor[i];
					if (item.TryGetEnchantedEquipItem(out EnchantedEquipItem enchantedEquipItem)) {
						if (enchantedEquipItem.enchantments.TryReturnAllEnchantments(wePlayer, true)) {
							enchantedEquipItem.enchantments.ApplyLoadout(loadout[i + 1]);
						}
					}
				}
			}

			EnchantmentStorage.ConsumeEnchantments(wePlayer, neededEnchantments);
			wePlayer.UpdateEnchantmentEffects();

			return true;
		}
		private static int GetEnchantentsCost(Item[] enchantments) {
			int cost = 0;
			for (int i = 0; i < enchantments.Length; i++) {
				if (enchantments[i].ModItem is Enchantment enchantment) {
					cost += enchantment.GetCapacityCost();
				}
			}

			return cost;
		}
		public static bool AvailableSlot(Item newItem) {
			WEPlayer wePlayer = WEPlayer.LocalWEPlayer;
			List<Item[]> inventory = wePlayer.enchantmentLoadouts[wePlayer.displayedLoadout];
			for (int rowNum = 0; rowNum < inventory.Count; rowNum++) {
				Item[] enchantmentSlots = inventory[rowNum];
				for (int enchantmentSlotIndex = 0; enchantmentSlotIndex < EnchantingTableUI.MaxEnchantmentSlots; enchantmentSlotIndex++) {
					ref Item item = ref enchantmentSlots[enchantmentSlotIndex];

					bool isArmor = rowNum >= 1 && rowNum <= 3;
					EItemType itemType = rowNum switch {
						0 => EItemType.Weapons,
						_ when (isArmor) => EItemType.Armor,
						_ => EItemType.Accessories,
					};

					bool canUseSlot = EnchantingTableUI.UseEnchantmentSlot(itemType, enchantmentSlotIndex);
					if (canUseSlot) {
						bool isUtilitySlot = enchantmentSlotIndex == EnchantingTableUI.MaxEnchantmentSlots - 1;
						int armorSlotSpecificID = isArmor ? rowNum - 1 : -1;
						if (EnchantingTableUI.ValidItemForLoadoutEnchantmentSlot(newItem, itemType, enchantmentSlotIndex, isUtilitySlot, armorSlotSpecificID) && newItem.ModItem is Enchantment enchantment) {
							int uniqueItemSlot = EnchantingTableUI.FindSwapEnchantmentSlot(enchantment, enchantmentSlots);
							bool uniqueSlotNotFound = uniqueItemSlot == -1;
							bool uniqueEnchantmentOnItem = EnchantingTableUI.CheckUniqueSlot(enchantment, uniqueItemSlot, enchantmentSlotIndex);
							bool canAcceptEnchantment = item.IsAir && uniqueSlotNotFound || !uniqueSlotNotFound && uniqueEnchantmentOnItem && newItem.type != item.type;
							if (canAcceptEnchantment) {
								availableSlotRow = rowNum;
								availableSlotIndex = enchantmentSlotIndex;
								skipAvailableSlotReset = true;

								return true;
							}
						}
					}
				}
			}

			availableSlotRow = -1;
			availableSlotIndex = -1;

			return false;
		}
		public static void UpdateAvailableEnchantmentSlot(WEPlayer wePlayer, Item enchantmentItem) {
			Item clone = enchantmentItem.Clone();
			clone.stack = 1;
			wePlayer.enchantmentLoadouts[wePlayer.displayedLoadout][availableSlotRow][availableSlotIndex] = clone;
		}
		public static void ResetAvailableSlot() {
			availableSlotRow = -1;
			availableSlotIndex = -1;
		}
		public static void DeleteLoadout(WEPlayer wePlayer, int loadoutIndex) {
			if (loadoutIndex >= wePlayer.enchantmentLoadouts.Count)
				return;

			wePlayer.enchantmentLoadouts.Remove(wePlayer.enchantmentLoadouts.ElementAt(loadoutIndex).Key);
			if (wePlayer.enchantmentLoadouts.Count < 1)
				AddNewBlankLoadout(wePlayer);

			wePlayer.displayedLoadout = wePlayer.enchantmentLoadouts.First().Key;
		}
	}
}
