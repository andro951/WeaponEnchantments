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

namespace WeaponEnchantments.UI
{
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
		public static int EnchantmentLoadoutUIDefaultLeft => 1070;
		public static int EnchantmentLoadoutUIDefaultTop => 290;
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
		public static string[] buttonNames = { EnchantmentStorageTextID.All.ToString().Lang(L_ID1.EnchantmentStorageText), EnchantmentStorageTextID.HeldItem.ToString().Lang(L_ID1.EnchantmentStorageText), EItemType.Armor.ToString().Lang(L_ID1.Tooltip, L_ID2.ItemType), EItemType.Accessories.ToString().Lang(L_ID1.Tooltip, L_ID2.ItemType) };
		public static bool useingScrollBar = false;
		public static void PostDrawInterface(SpriteBatch spriteBatch) {
			WEPlayer wePlayer = WEPlayer.LocalWEPlayer;
			if (wePlayer.displayEnchantmentLoadoutUI) {

				#region Data

				Color mouseColor = UIManager.MouseColor;
				//ItemSlots Data 1/2
				List<Item[]> inventory = GetFixLoadout(wePlayer);

				int itemSlotColumns = EnchantingTableUI.MaxEnchantmentSlots;
				int itemSlotRowsDisplayed = 11;//TODO: Calculate 9-11 based on vanilla slot unlocks
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
				useingScrollBar = itemSlotRowsDisplayed != itemSlotTotalRows;
				int scrollBarLeft = itemSlotsLeft + itemSlotsWidth + Spacing;
				int scrollBarWidth = useingScrollBar ? 30 : 0;
				int possiblePanelPositions = itemSlotTotalRows - itemSlotRowsDisplayed;
				if (possiblePanelPositions < 1)
					possiblePanelPositions = 1;

				scrollPanelPosition.Clamp(0, possiblePanelPositions);

				//Text buttons Data
				int buttonsLeft = scrollBarLeft + scrollBarWidth + Spacing;
				int currentButtonTop = nameTop;
				UITextData[,] textButtons = new UITextData[wePlayer.enchantmentLoadouts.Count, buttonNames.Length];
				int totalButtonWidths = 0;
				for (int buttonRow = 0; buttonRow < wePlayer.enchantmentLoadouts.Count; buttonRow++) {
					int currentButtonLeft = buttonsLeft;
					for (int buttonIndex = 0; buttonIndex < buttonNames.Length; buttonIndex++) {
						string text = buttonNames[buttonIndex];
						float scale = ButtonScale[buttonRow, buttonIndex];
						Color color = mouseColor;
						UITextData thisButton = new(UI_ID.EnchantmentLoadoutUITextButton, currentButtonLeft, currentButtonTop, text, scale, color, ancorBotomLeft: true);
						textButtons[buttonRow, buttonIndex] = thisButton;
						int xOffset = buttonIndex < buttonNames.Length - 1 ? thisButton.BaseWidth + Spacing : thisButton.Width;
						totalButtonWidths += xOffset;
						currentButtonLeft += xOffset;
					}

					currentButtonTop += textButtons[buttonRow, 0].BaseHeight;
				}

				//Panel Data 2/2
				int panelBorderRightOffset = Spacing + totalButtonWidths + PanelBorder;
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
						UIItemSlotData enchantmentSlot = new(UI_ID.EnchantmentLoadoutUIItemSlot, itemSlotX, itemSlotY);
						ref Item item = ref enchantmentSlots[enchantmentSlotIndex];
						bool isUtilitySlot = enchantmentSlotIndex == EnchantingTableUI.MaxEnchantmentSlots - 1;
						bool isArmor = rowNum >= 1 && rowNum <= 3;
						EItemType itemType = rowNum switch {
							0 => EItemType.Weapons,
							_ when(isArmor) => EItemType.Armor,
							_ => EItemType.Accessories,
						};

						int armorSlotSpecificID = isArmor ? rowNum - 1 : -1;

						bool canUseSlot =  EnchantingTableUI.UseEnchantmentSlot(itemType, enchantmentSlotIndex);//Change to item type
						if (enchantmentSlot.MouseHovering()) {
							bool clear = false;
							if (ItemSlot.ShiftInUse) {
								Main.cursorOverride = CursorOverrideID.TrashCan;
								if (UIManager.LeftMouseClicked) {
									item = new();
									SoundEngine.PlaySound(SoundID.MenuTick);
								}
							}
							else if (UIManager.RightMouseClicked) {
								item = new();
								SoundEngine.PlaySound(SoundID.MenuTick);
							}
							else if (Main.mouseItem.IsAir) {
								Main.cursorOverride = CursorOverrideID.TrashCan;
								if (UIManager.LeftMouseClicked) {
									item = new();
									SoundEngine.PlaySound(SoundID.MenuTick);
								}
							}
							else {
								if (EnchantingTableUI.ValidItemForLoadoutEnchantmentSlot(Main.mouseItem, itemType, enchantmentSlotIndex, isUtilitySlot, armorSlotSpecificID)) {
									bool canAcceptEnchantment = Main.mouseItem.ModItem is Enchantment enchantment && EnchantingTableUI.CheckUniqueSlot(enchantment, EnchantingTableUI.FindSwapEnchantmentSlot(enchantment, enchantmentSlots), enchantmentSlotIndex) && Main.mouseItem.type != item.type;
									if (canAcceptEnchantment) {
										Main.cursorOverride = CursorOverrideID.CameraDark;
										if (UIManager.LeftMouseClicked) {
											item = Main.mouseItem.Clone();
											SoundEngine.PlaySound(SoundID.MenuTick);
										}
									}
								}
							}
						}

						enchantmentSlot.Draw(spriteBatch, item, canUseSlot ? isUtilitySlot ? ItemSlotContextID.Favorited : ItemSlotContextID.Normal : ItemSlotContextID.Red);
						itemSlotX += itemSlotSpaceWidth;
					}

					itemSlotY += itemSlotSpaceHeight;
				}

				//Name Draw
				nameData.Draw(spriteBatch);

				//Text Buttons Draw
				for (int buttonRow = 0; buttonRow < wePlayer.enchantmentLoadouts.Count; buttonRow++) {
					for (int buttonIndex = 0; buttonIndex < buttonNames.Length; buttonIndex++) {
						UITextData textButton = textButtons[buttonRow, buttonIndex];
						textButton.Draw(spriteBatch);
						if (UIManager.MouseHovering(textButton, true)) {
							ButtonScale[buttonRow, buttonIndex] += 0.05f;

							if (ButtonScale[buttonRow, buttonIndex] > buttonScaleMaximum)
								ButtonScale[buttonRow, buttonIndex] = buttonScaleMaximum;

							if (UIManager.LeftMouseClicked) {
								TrySwapToLoadout((LoadoutSwapID)buttonIndex, buttonRow);

								SoundEngine.PlaySound(SoundID.MenuTick);
							}
						}
						else {
							ButtonScale[buttonRow, buttonIndex] -= 0.05f;

							if (ButtonScale[buttonRow, buttonIndex] < buttonScaleMinimum)
								ButtonScale[buttonRow, buttonIndex] = buttonScaleMinimum;
						}
					}
				}

				//Scroll Bar
				if (useingScrollBar) {
					//Scroll Bar Data 2/2
					int scrollBarTop = wePlayer.EnchantmentLoadoutUITop + PanelBorder;
					UIPanelData scrollBarData = new(UI_ID.EnchantmentLoadoutUIScrollBar, scrollBarLeft, scrollBarTop, scrollBarWidth, panelHeight - PanelBorder * 2, new Color(10, 1, 30, 100));

					//Scroll Panel Data 1/2
					int scrollPanelXOffset = 1;
					int scrollPanelSize = scrollBarWidth - scrollPanelXOffset * 2;
					int scrollPanelMinY = scrollBarData.TopLeft.Y + scrollPanelXOffset;
					int scrollPanelMaxY = scrollBarData.BottomRight.Y - scrollPanelSize - scrollPanelXOffset;

					//Scroll Bar Draw
					scrollBarData.Draw(spriteBatch);

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
				}

				//Panel Hover and Drag
				if (panel.MouseHovering()) {
					panel.TryStartDraggingUI();
				}

				if (panel.ShouldDragUI())
					UIManager.DragUI(out wePlayer.EnchantmentLoadoutUILeft, out wePlayer.EnchantmentLoadoutUITop);

				if (useingScrollBar) {
					int scrollWheelTicks = UIManager.ScrollWheelTicks;
					if (scrollWheelTicks != 0 && UIManager.HoveringEnchantmentLoadoutUI && UIManager.NoPanelBeingDragged) {
						if (scrollPanelPosition > 0 && scrollWheelTicks < 0 || scrollPanelPosition < possiblePanelPositions && scrollWheelTicks > 0) {
							SoundEngine.PlaySound(SoundID.MenuTick);
							scrollPanelPosition += scrollWheelTicks;
						}
					}
				}
			}
		}
		private static List<Item[]> GetFixLoadout(WEPlayer wePlayer) {
			List<Item[]> loadout = wePlayer.enchantmentLoadouts[displayedLoadout];
			int requiredSize = GetRequiredLoudoutSize(wePlayer);
			if (loadout.Count > requiredSize) {
				for (int i = loadout.Count - 1; i >= requiredSize; i--) {
					Item[] enchantments = loadout[i];
					for (int k = 0; k < enchantments.Length; k++) {
						ref Item enchantment = ref enchantments[k];
						if (!enchantment.IsAir)
							wePlayer.TryReturnItemToPlayer(ref enchantment, true);
					}

					loadout.RemoveAt(i);
				}

				Main.NewText(EnchantmentStorageTextID.LoadoutSizeChanged.ToString().Lang(L_ID1.EnchantmentStorageText));
			}
			else if (loadout.Count < requiredSize) {
				for (int i = loadout.Count; i < requiredSize; i++) {
					loadout.Add(GetEnchantmentRow());
				}
			}

			return loadout;
		}
		public static void AddNewBlankLoadout(WEPlayer wePlayer) {
			wePlayer.enchantmentLoadouts.Add(GetBlankLoadout(wePlayer));
		}
		private static int GetRequiredLoudoutSize(WEPlayer wePlayer) => wePlayer.Equipment.GetAllArmor().Count() + 1;
		private static List<Item[]> GetBlankLoadout(WEPlayer wePlayer) {
			int count = GetRequiredLoudoutSize(wePlayer);
			return Enumerable.Repeat(GetEnchantmentRow(), count).ToList();
		}
		private static Item[] GetEnchantmentRow() => Enumerable.Repeat(new Item(), EnchantingTableUI.MaxEnchantmentSlots).ToArray();
		public static void Open(bool noSound = false) {
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
		private static bool TrySwapToLoadout(LoadoutSwapID swapID, int loadoutNum) {
			WEPlayer wePlayer = WEPlayer.LocalWEPlayer;

			//Can Swap
			bool swapWeapon = swapID == LoadoutSwapID.HeldItem || swapID == LoadoutSwapID.All;
			bool swapArmmor = swapID == LoadoutSwapID.Armor || swapID == LoadoutSwapID.All;
			bool swapAccessories = swapID == LoadoutSwapID.Accessories || swapID == LoadoutSwapID.All;
			if (swapWeapon) {
				if (wePlayer.Player.HeldItem.IsAir) {
					Main.NewText(EnchantmentStorageTextID.NoHeldItem.ToString().Lang(L_ID1.EnchantmentStorageText));
					return false;
				}
			}

			if (swapArmmor) {

			}

			if (swapAccessories) {

			}

			//Swap
			if (swapWeapon) {

			}

			if (swapArmmor) {

			}

			if (swapAccessories) {

			}

			return true;
		}
	}
}
