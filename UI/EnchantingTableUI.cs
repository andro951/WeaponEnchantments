using KokoLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Configs;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Common.Utility.LogSystem;
using WeaponEnchantments.Content.NPCs;
using WeaponEnchantments.Items;
using WeaponEnchantments.ModLib.KokoLib;
using WeaponEnchantments.Tiles;

namespace WeaponEnchantments.UI
{
	public static class EnchantingTableUI
	{
		public static int DefaultLeft => (Main.screenWidth - 530) / 2;
		public static int DefaultTop => Main.screenHeight / 2 + 100;
		private static int Spacing => 4;
		private static int PanelBorder => 10;
		private static int ButtonBorderY => 0;
		private static int ButtonBorderX => 6;
		public static Color RedButtonColor => new Color(255, 30, 30, 100);
		public static Color RedHoverColor => new Color(255, 70, 70, 120);
		public static Color BackGroundColor => new Color(50, 70, 171, 100);
		public static Color HoverColor => new Color(100, 118, 184, 120);
		public static Color LevelSetColor => new Color(73, 170, 118, 120);
		//public const int MaxEnchantingItemSlots = 1;
		public const int MaxEnchantmentSlots = 5;
		public const int MaxEssenceSlots = 5;
		public static readonly int[] LevelsPerLevelUp = { 1, 5, 10, EnchantedItem.MAX_Level };
		public static bool DisplayOfferUI = false;
		public const float buttonScaleMinimum = 0.75f;
		public const float buttonScaleMaximum = 1f;
		public static float[] LevelsPerButtonScale = Enumerable.Repeat(buttonScaleMinimum, LevelsPerLevelUp.Length).ToArray();
		private static bool itemBeingEnchantedIsFavorited = false;
		private static string descriptionBlock = null;
		private static bool DisplayDescriptionBlock => UIManager.HoverTime >= 60;
		public static bool pressedLootAll = false;
		public static void PostDrawInterface(SpriteBatch spriteBatch) {
			WEPlayer wePlayer = WEPlayer.LocalWEPlayer;
			if (wePlayer.usingEnchantingTable) {
				
				#region Pre UI
				
				wePlayer.enchantingTableEnchantments = wePlayer.enchantingTableItem.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem) ? enchantedItem.enchantments : wePlayer.emptyEnchantments;
				
				Item itemInUI = wePlayer.enchantingTableItem;
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
				else if (!wePlayer.itemBeingEnchanted.IsSameEnchantedItem(itemInUI)) {
					swappedItem = true;
				}

				if (removedItem || swappedItem)
					RemoveTableItem();

				if (addedItem || swappedItem) {
					wePlayer.itemBeingEnchanted = wePlayer.enchantingTableItem;// Link the item in the table to the player so it can be updated after being taken out.
					Item itemBeingEnchanted = wePlayer.itemBeingEnchanted;
					itemBeingEnchantedIsFavorited = itemBeingEnchanted.favorited;
					itemBeingEnchanted.favorited = false;
					if (itemBeingEnchanted.TryGetEnchantedItemSearchAll(out EnchantedItem iBEGlobal)) {
						iBEGlobal.inEnchantingTable = true;
						wePlayer.previousInfusedItemName = iBEGlobal.infusedItemName;
						if (iBEGlobal is EnchantedEquipItem enchantedEquipItem)
							enchantedEquipItem.equippedInArmorSlot = false;
					}

					if (!wePlayer.infusionConsumeItem.IsAir && itemBeingEnchanted.InfusionAllowed(out bool infusionAllowed)) {
						if (infusionAllowed)
							wePlayer.itemBeingEnchanted.TryInfuseItem(wePlayer.infusionConsumeItem);
					}
				}

				//If player is too far away, close the enchantment table
				if (!wePlayer.Player.IsInInteractionRangeToMultiTileHitbox(wePlayer.Player.chestX, wePlayer.Player.chestY) || wePlayer.Player.chest != -1 || !Main.playerInventory)
					CloseEnchantingTableUI();

				//Prevent Trash can and other mouse overides when using enchanting table
				if (ItemSlot.ShiftInUse && UIManager.NoUIBeingHovered && (Main.mouseItem.IsAir && !Main.HoverItem.IsAir || Main.cursorOverride == CursorOverrideID.TrashCan)) {
					if (!wePlayer.CheckShiftClickValid(ref Main.HoverItem) || Main.cursorOverride == CursorOverrideID.TrashCan)
						Main.cursorOverride = -1;
				}

				#endregion

				#region UI

				//Start of UI
				Color mouseColor = UIManager.MouseColor;

				//Item Label Data 1/2
				int itemLabelTop = wePlayer.enchantingTableUITop + Spacing;
				string itemLabel = TableTextID.Item.ToString().Lang(L_ID1.TableText);
				TextData itemLabelTextData = new(itemLabel);
				int largestWidthCenteredOnEnchantingItemSlotCenterX = itemLabelTextData.Width;

				//Description Block Data 1/2
				TextData descriptionBlockTextData = new(descriptionBlock);
				int descriptionBlockTop = itemLabelTop - (descriptionBlock != null ? descriptionBlockTextData.Height + Spacing : 0);

				//Enchanting Item Slot Data 2/2
				int enchantingItemSlotTop = itemLabelTop + itemLabelTextData.Height + Spacing;

				//Loot All Data 1/2
				int lootAllTop = enchantingItemSlotTop + UIManager.ItemSlotSize + Spacing + ButtonBorderY;
				string lootAll = TableTextID.LootAll.ToString().Lang(L_ID1.TableText);
				TextData lootAllTextData = new(lootAll);
				if (lootAllTextData.Width > largestWidthCenteredOnEnchantingItemSlotCenterX)
					largestWidthCenteredOnEnchantingItemSlotCenterX = lootAllTextData.Width;

				//Offer Button Data 1/2
				int offerButtonTop = lootAllTop + lootAllTextData.Height + Spacing + ButtonBorderY * 2;
				string offerButton = TableTextID.Offer.ToString().Lang(L_ID1.TableText);
				TextData offerButtonTextData = new(offerButton);
				if (offerButtonTextData.Width > largestWidthCenteredOnEnchantingItemSlotCenterX)
					largestWidthCenteredOnEnchantingItemSlotCenterX = offerButtonTextData.Width;

				//Left Panel Buttons Data 1/2
				int leftPanelButtonsWidth = largestWidthCenteredOnEnchantingItemSlotCenterX + ButtonBorderX * 2;

				//EnchantingItemSlot Data 1/2
				int enchantingItemSlotCenterX = wePlayer.enchantingTableUILeft + PanelBorder + leftPanelButtonsWidth / 2;
				int enchantingItemSlotLeft = enchantingItemSlotCenterX - UIManager.ItemSlotSize / 2;

				//Left Panel Buttons Data 2/2
				int leftPanelButtonsRightEdge = enchantingItemSlotCenterX + leftPanelButtonsWidth / 2;

				//Description Block Data 2/2
				UITextData descriptionBlockData = new(UI_ID.None, wePlayer.enchantingTableUILeft + PanelBorder, descriptionBlockTop, descriptionBlockTextData, mouseColor);

				//Item Label Data 2/2
				UITextData itemLabelData = new(UI_ID.None, enchantingItemSlotCenterX, itemLabelTop, itemLabel, 1f, mouseColor, true);

				//Loot All Data 2/2
				UIButtonData lootAllData = new(UI_ID.EnchantingTableLootAll, leftPanelButtonsRightEdge, lootAllTop, lootAllTextData, mouseColor, (leftPanelButtonsWidth - lootAllTextData.Width) / 2, ButtonBorderY, BackGroundColor, HoverColor, true);

				//Offer Button Data 2/2
				UIButtonData offerButtonData = new(UI_ID.EnchantingTableOfferButton, leftPanelButtonsRightEdge, offerButtonTop, offerButtonTextData, mouseColor, (leftPanelButtonsWidth - offerButtonTextData.Width) / 2, ButtonBorderY, RedButtonColor, RedHoverColor, true);

				//Panel Middle Data 1/2
				int panelMiddleLeft = leftPanelButtonsRightEdge + Spacing;

				//Enchantments Label Data 1/2
				string enchantmentsLabel = TableTextID.Enchantments.ToString().Lang(L_ID1.TableText);
				TextData enchantmentsLabelTextData = new(enchantmentsLabel);

				//XP button Data 1/2
				string xp = TableTextID.xp.ToString().Lang(L_ID1.TableText);
				TextData xpTextData = new(xp);//, 0.75f);
				int enchantmentSlotsCount = MaxEnchantmentSlots - 1;
				int diff = (xpTextData.Width + ButtonBorderX * 2) - UIManager.ItemSlotSize;
				int widthOfSlotButtonPair = Math.Max(xpTextData.Width, UIManager.ItemSlotSize);
				int enchantmentSlotsWidth = widthOfSlotButtonPair * enchantmentSlotsCount + Spacing * (enchantmentSlotsCount - 1);

				//Panel Middle Data 2/2
				int panelMiddleRight = panelMiddleLeft + Math.Max(enchantmentSlotsWidth, enchantmentsLabelTextData.Width);
				int panelMiddleCenterX = (panelMiddleLeft + panelMiddleRight) / 2;

				//Enchantments Label Data 2/2
				UITextData enchantmentsLabelData = new(UI_ID.None, panelMiddleCenterX, itemLabelTop, enchantmentsLabelTextData, mouseColor, true);

				//XP buttons Data 2/2
				UIButtonData[] xpData = new UIButtonData[MaxEnchantmentSlots];
				int[] middleSlotsLefts = new int[MaxEnchantmentSlots];
				int middleSlotsCurrentLeft = panelMiddleCenterX - enchantmentSlotsWidth / 2;
				int xpButtonTop = enchantingItemSlotTop + (UIManager.ItemSlotSize + Spacing) * 2;
				for (int i = 0; i < MaxEnchantmentSlots; i++) {
					UIButtonData thisXpData = new(UI_ID.EnchantingTableXPButton0 + i, middleSlotsCurrentLeft - diff / 2, xpButtonTop, xpTextData, mouseColor, ButtonBorderX, ButtonBorderY, BackGroundColor, HoverColor);
					xpData[i] = thisXpData;
					middleSlotsLefts[i] = middleSlotsCurrentLeft;
					middleSlotsCurrentLeft += widthOfSlotButtonPair + Spacing;
				}

				//Essence Slots Data
				int essenecSlotsTop = enchantingItemSlotTop + UIManager.ItemSlotSize + Spacing;

				//Utility Label Data 1/2
				string utilityLabel = EnchantmentGeneralTooltipsID.Utility.ToString().Lang(L_ID1.Tooltip, L_ID2.EnchantmentGeneralTooltips);
				TextData utilityLabelTextData = new(utilityLabel);

				//Utility Data
				int projectedUtilityMiddle = panelMiddleRight + Spacing + UIManager.ItemSlotSize / 2;
				int diffEnchantmentsToUtilityLabel = panelMiddleCenterX - projectedUtilityMiddle + (enchantmentsLabelTextData.Width + utilityLabelTextData.Width) / 2;
				int extraSpaceNeeded = diffEnchantmentsToUtilityLabel > 0 ? diffEnchantmentsToUtilityLabel + Spacing * 2 : 0;
				int utilityLeft = panelMiddleRight + Spacing + extraSpaceNeeded;
				int utilityRight = utilityLeft + Math.Max(widthOfSlotButtonPair, utilityLabelTextData.Width);
				int utilityCenterX = (utilityLeft + utilityRight) / 2;

				//Utility Label Data 2/2
				UITextData utilityLabelData = new(UI_ID.None, utilityCenterX, itemLabelTop, utilityLabelTextData, mouseColor, true);

				//Right Panel Buttons Data 1/2
				int rightButtonsLeft = utilityRight + Spacing;

				//Storage Button Data 1/2
				int storageButtonTop = itemLabelTop + 4;
				string storage = TableTextID.Storage.ToString().Lang(L_ID1.TableText);
				storage = "Storage test";
				TextData storageTextData = new(storage);
				int largestWidthOfRightSideButtons = storageTextData.Width;

				//Syphon Button Data 1/2
				int syphonTop = storageButtonTop + storageTextData.Height + Spacing + ButtonBorderY * 2;
				string syphon = TableTextID.Syphon.ToString().Lang(L_ID1.TableText);
				TextData syphonTextData = new(syphon);
				if (syphonTextData.Width > largestWidthOfRightSideButtons)
					largestWidthOfRightSideButtons = syphonTextData.Width;

				//Infusion Button Data 1/2
				int infusionButtonTop = syphonTop + syphonTextData.Height + Spacing + ButtonBorderY * 2;
				string infusion;
				if (!wePlayer.infusionConsumeItem.IsAir) {
					if (wePlayer.enchantingTableItem.IsAir) {
						infusion = TableTextID.Cancel.ToString().Lang(L_ID1.TableText);
					}
					else {
						infusion = TableTextID.Finalize.ToString().Lang(L_ID1.TableText);
					}
				}
				else {
					infusion = TableTextID.Infusion.ToString().Lang(L_ID1.TableText);
				}

				TextData infusionTextData = new(infusion);
				if (infusionTextData.Width > largestWidthOfRightSideButtons)
					largestWidthOfRightSideButtons = infusionTextData.Width;

				//Level Up Button Data 1/2
				string levelUp = TableTextID.LevelUp.ToString().Lang(L_ID1.TableText);
				TextData levelUpTextData = new(levelUp);
				if (levelUpTextData.Width > largestWidthOfRightSideButtons)
					largestWidthOfRightSideButtons = levelUpTextData.Width;

				//Level Per Level Up Buttons Data 1/2
				TextData[] levelsPerTextData = new TextData[LevelsPerLevelUp.Length];
				int levelsPerWidth = (LevelsPerLevelUp.Length - 1) * ButtonBorderX;
				for (int i = 0; i < LevelsPerLevelUp.Length; i++) {
					TextData thisLevelsper = new($"{LevelsPerLevelUp[i]}", LevelsPerButtonScale[i]);
					levelsPerWidth += thisLevelsper.BaseWidth;
					levelsPerTextData[i] = thisLevelsper;
				}

				if (levelsPerWidth > largestWidthOfRightSideButtons)
					largestWidthOfRightSideButtons = levelsPerWidth;

				//Right Panel Buttons Data 2/2
				int rightPanelButtonsWidth = largestWidthOfRightSideButtons + ButtonBorderX * 2;
				int rightPanelButtonsCenterX = rightButtonsLeft + rightPanelButtonsWidth / 2;
				int rightPanelButtonsRightEdge = rightButtonsLeft + rightPanelButtonsWidth;

				//Storage Button Data 2/2
				UIButtonData storageData = new(UI_ID.EnchantingTableStorageButton, rightButtonsLeft, storageButtonTop, storageTextData, mouseColor, (rightPanelButtonsWidth - storageTextData.Width) / 2, ButtonBorderY, BackGroundColor, HoverColor);

				//Syphon Button Data 2/2
				UIButtonData syphonData = new(UI_ID.EnchantingTableSyphon, rightButtonsLeft, syphonTop, syphonTextData, mouseColor, (rightPanelButtonsWidth - syphonTextData.Width) / 2, ButtonBorderY, BackGroundColor, HoverColor);

				//Infusion Button Data 2/2
				UIButtonData infusionData = new(UI_ID.EnchantingTableInfusion, rightButtonsLeft, infusionButtonTop, infusionTextData, mouseColor, (rightPanelButtonsWidth - infusionTextData.Width) / 2, ButtonBorderY, BackGroundColor, HoverColor);

				//Level Up Button Data 2/2
				UIButtonData levelUpData = new(UI_ID.EnchantingTableLevelUp, rightButtonsLeft, xpButtonTop, levelUpTextData, mouseColor, (rightPanelButtonsWidth - levelUpTextData.Width) / 2, ButtonBorderY, BackGroundColor, HoverColor);

				//Level Per Level Up Buttons Data 2/2
				int levelsPerTop = xpButtonTop - levelsPerTextData[0].BaseHeight + Spacing;
				UITextData[] levelsPerData = new UITextData[LevelsPerLevelUp.Length];
				int currentLevelsPerLeft = rightPanelButtonsCenterX - levelsPerWidth / 2;
				for (int i = 0; i < LevelsPerLevelUp.Length; i++) {
					Color color = wePlayer.levelsPerLevelUp == LevelsPerLevelUp[i] ? LevelSetColor : mouseColor;
					UITextData thisLevelsPer = new(UI_ID.EnchantingTableLevelsPerLevelUp0 + i, currentLevelsPerLeft, levelsPerTop, levelsPerTextData[i], color, ancorBotomLeft: true);
					currentLevelsPerLeft += thisLevelsPer.BaseWidth + ButtonBorderX;
					levelsPerData[i] = thisLevelsPer;
				}

				//Panel Data
				Point panelTopLeft = new(wePlayer.enchantingTableUILeft, wePlayer.enchantingTableUITop - (descriptionBlock != null ? descriptionBlockTextData.Height + Spacing : 0));
				Point panelBottomRight = new((descriptionBlock != null ? Math.Max(rightPanelButtonsRightEdge, leftPanelButtonsRightEdge - leftPanelButtonsWidth + descriptionBlockTextData.Width) : rightPanelButtonsRightEdge) + PanelBorder, xpButtonTop + xpData[0].Height + PanelBorder);
				UIPanelData panel = new(UI_ID.EnchantingTable, panelTopLeft, panelBottomRight, BackGroundColor);

				//Panel Draw
				panel.Draw(spriteBatch);

				//Description Block Draw
				if (descriptionBlock != null) {
					descriptionBlockData.Draw(spriteBatch);
					descriptionBlock = null;
				}

				//Item Label Draw
				itemLabelData.Draw(spriteBatch);

				//Enchanting Item Slot Draw
				UIItemSlotData enchantingItemSlotData = new(UI_ID.EnchantingTableItemSlot, enchantingItemSlotLeft, enchantingItemSlotTop);
				enchantingItemSlotData.Draw(spriteBatch, wePlayer.enchantingTableItem, ItemSlotContextID.Gold);

				//Loot All Button Draw
				lootAllData.Draw(spriteBatch);

				//Offer Button Draw
				offerButtonData.Draw(spriteBatch);

				//Enchantments Label Draw
				enchantmentsLabelData.Draw(spriteBatch);

				//XP buttons Draw
				for (int i = 0; i < MaxEssenceSlots; i++) {
					xpData[i].Draw(spriteBatch);
				}

				//Enchantment Slots Draw
				UIItemSlotData[] enchantmentSlotsData = new UIItemSlotData[MaxEnchantmentSlots];
				for (int i = 0; i < enchantmentSlotsCount; i++) {
					UIItemSlotData enchantmentSlot = new(UI_ID.EnchantingTableEnchantment0 + i, middleSlotsLefts[i], enchantingItemSlotTop);
					enchantmentSlotsData[i] = enchantmentSlot;
					enchantmentSlot.Draw(spriteBatch, wePlayer.enchantingTableEnchantments[i]);
				}

				//Essence Slots Draw
				UIItemSlotData[] essenceSlotsData = new UIItemSlotData[MaxEssenceSlots];
				for (int i = 0; i < MaxEssenceSlots; i++) {
					UIItemSlotData essenceSlot = new(UI_ID.EnchantingTableEssence0 + i, middleSlotsLefts[i], essenecSlotsTop);
					essenceSlotsData[i] = essenceSlot;
					essenceSlot.Draw(spriteBatch, wePlayer.enchantingTableEssence[i], ItemSlotContextID.Purple);
				}

				//Utility Label Draw
				utilityLabelData.Draw(spriteBatch);

				//Utility Slot Draw
				UIItemSlotData utilitySlotData = new(UI_ID.EnchantingTableEssence0 + enchantmentSlotsCount, utilityCenterX - UIManager.ItemSlotSize / 2, enchantingItemSlotTop);
				utilitySlotData.Draw(spriteBatch, wePlayer.enchantingTableEnchantments[enchantmentSlotsCount], ItemSlotContextID.Favorited);
				enchantmentSlotsData[enchantmentSlotsCount] = utilitySlotData;

				//Storage Button Draw
				storageData.Draw(spriteBatch);

				//Syphon Button Draw
				syphonData.Draw(spriteBatch);

				//Infusion Button Draw
				infusionData.Draw(spriteBatch);

				//Level Up Button Draw
				levelUpData.Draw(spriteBatch);

				//Level Per Level Up Buttons Draw
				for (int i = 0; i < LevelsPerLevelUp.Length; i++) {
					levelsPerData[i].Draw(spriteBatch);
				}

				if (DisplayOfferUI) {
					//Yes Data 1/2
					string yes = TableTextID.Yes.ToString().Lang(L_ID1.TableText);
					TextData yesTextData = new(yes);

					//No Data 1/2
					string no = TableTextID.No.ToString().Lang(L_ID1.TableText);
					TextData noTextData = new(no);

					//Yes Data 2/2
					UIButtonData yesData = new(UI_ID.OfferYes, leftPanelButtonsRightEdge, lootAllTop, yesTextData, mouseColor, (leftPanelButtonsWidth - yesTextData.Width) / 2, ButtonBorderY, RedButtonColor, RedHoverColor, true);

					//No Data 2/2
					UIButtonData noData = new(UI_ID.OfferNo, leftPanelButtonsRightEdge, offerButtonTop, noTextData, mouseColor, (leftPanelButtonsWidth - noTextData.Width) / 2, ButtonBorderY, BackGroundColor, HoverColor, true);

					//Offer Warning Data
					string offerWarning = GetOfferWarning();
					TextData offerWarningTextData = new(offerWarning);
					int yOffset = Math.Max(panel.Height - offerWarningTextData.Height, 0) / 2;
					UITextData offerWarningData = new(UI_ID.None, leftPanelButtonsRightEdge + Spacing, itemLabelTop + yOffset, offerWarningTextData, mouseColor);
					/*
					Utils.WordwrapString(text, FontAssets.MouseText.Value, 200, 1, out var lineAmount);
					lineAmount++;
					for (int i = 0; i < lineAmount; i++) {
						ChatManager.DrawColorCodedStringWithShadow(spritebatch, FontAssets.MouseText.Value, text, new Vector2(504f, Main.instance.invBottom + i * 26), color, 0f, Vector2.Zero, Vector2.One, -1f, 1.5f);
					}
					 */

					//Offer Panel Data
					Color offerRed = RedButtonColor;
					offerRed.A = 224;
					Point offerPanelBottomRight = new(Math.Max(panelBottomRight.X, offerWarningData.BottomRight.X + PanelBorder), Math.Max(panelBottomRight.Y, offerWarningData.BottomRight.Y + PanelBorder));
					UIPanelData offerPanel = new(UI_ID.Offer, panelTopLeft, offerPanelBottomRight, offerRed);

					//Offer Panel Draw
					offerPanel.Draw(spriteBatch);

					//Yes Draw
					yesData.Draw(spriteBatch);

					//No Draw
					noData.Draw(spriteBatch);

					//Offer Warning Draw
					offerWarningData.Draw(spriteBatch);

					//Yes Hover
					if (yesData.MouseHovering()) {
						if (UIManager.LeftMouseClicked) {
							Offer();
							DisplayOfferUI = false;
							SoundEngine.PlaySound(SoundID.MenuTick);
						}
					}

					//No Hover
					if (noData.MouseHovering()) {
						if (UIManager.LeftMouseClicked) {
							DisplayOfferUI = false;
							SoundEngine.PlaySound(SoundID.MenuTick);
							SoundEngine.PlaySound(SoundID.MenuClose);
						}
					}

					//Offer Panel Hover
					if (offerPanel.MouseHovering()) {
						offerPanel.TryStartDraggingUI();
					}

					if (offerPanel.ShouldDragUI())
						UIManager.DragUI(out wePlayer.enchantingTableUILeft, out wePlayer.enchantingTableUITop);
				}
				else {
					//Enchanting Item Slot Hover
					if (enchantingItemSlotData.MouseHovering()) {
						bool display = false;
						if (ValidItemForEnchantingSlot(Main.mouseItem)) {
							if (Main.mouseItem.type == PowerBooster.ID) {
								if (UIManager.LeftMouseClicked) {
									if (!wePlayer.enchantingTableItem.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedTableItem) && !enchantedTableItem.PowerBoosterInstalled) {
										if (Main.mouseItem.stack > 1) {
											Main.mouseItem.stack--;
										}
										else {
											Main.mouseItem = new();
										}

										SoundEngine.PlaySound(SoundID.Grab);
										enchantedItem.PowerBoosterInstalled = true;
									}
								}
							}
							else if (Main.mouseItem.type == UltraPowerBooster.ID) {
								if (UIManager.LeftMouseClicked) {
									if (!wePlayer.enchantingTableItem.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedTableItem) && !enchantedTableItem.UltraPowerBoosterInstalled) {
										if (Main.mouseItem.stack > 1) {
											Main.mouseItem.stack--;
										}
										else {
											Main.mouseItem = new();
										}

										SoundEngine.PlaySound(SoundID.Grab);
										enchantedItem.UltraPowerBoosterInstalled = true;
									}
								}
							}
							else {
								display = true;
								enchantingItemSlotData.ClickInteractions(ref wePlayer.enchantingTableItem);
							}
						}
						else {
							display = true;
						}

						if (display && DisplayDescriptionBlock)
							SetDescriptionBlock(TableTextID.weapon0.ToString().Lang(L_ID1.TableText));
					}

					//Loot All Hover
					if (lootAllData.MouseHovering()) {
						if (UIManager.LeftMouseClicked) {
							LootAll();
							SoundEngine.PlaySound(SoundID.MenuTick);
						}
					}

					//Offer Button Hover
					if (offerButtonData.MouseHovering()) {
						if (UIManager.LeftMouseClicked) {
							SoundEngine.PlaySound(SoundID.MenuTick);
							if (!wePlayer.enchantingTableItem.IsAir) {
								DisplayOfferUI = true;
								SoundEngine.PlaySound(SoundID.MenuOpen);
							}
						}
					}

					//XP buttons Hover
					for (int i = 0; i < xpData.Length; i++) {
						if (xpData[i].MouseHovering()) {
							if (UIManager.LeftMouseClicked) {
								ConvertEssenceToXP(i);
								SoundEngine.PlaySound(SoundID.MenuTick);
							}
						}
					}

					//Enchantment Slots Hover
					for (int i = 0; i < MaxEnchantmentSlots; i++) {
						UIItemSlotData enchantmentSlot = enchantmentSlotsData[i];
						if (enchantmentSlot.MouseHovering()) {
							Item enchantmentItem = wePlayer.enchantingTableEnchantments[i];
							bool isUtilitySlot = i == enchantmentSlotsCount;
							bool display = false;
							if (ValidItemForEnchantmentSlot(Main.mouseItem, i, i == enchantmentSlotsCount)) {
								if (wePlayer.displayEnchantmentStorage && ItemSlot.ShiftInUse && UIManager.LeftMouseClicked && EnchantmentStorage.CanBeStored(enchantmentItem) && EnchantmentStorage.RoomInStorage(enchantmentItem)) {
									EnchantmentStorage.DepositAll(ref enchantmentItem);
								}
								else if (Main.mouseItem.ModItem is Enchantment enchantment) {
									if (CheckUniqueSlot(enchantment, FindSwapEnchantmentSlot(enchantment, wePlayer.enchantingTableItem), i)) {
										if (Main.mouseItem.type != wePlayer.enchantingTableEnchantments[i].type) {
											if (Main.mouseItem.stack > 1) {
												if (Main.mouseLeft && Main.mouseLeftRelease) {
													enchantmentItem = wePlayer.Player.GetItem(Main.myPlayer, enchantmentItem, GetItemSettings.LootAllSettings);
													if (enchantmentItem.IsAir) {
														Main.mouseItem.stack--;
														wePlayer.enchantingTableEnchantments[i] = Main.mouseItem.Clone();
														wePlayer.enchantingTableEnchantments[i].stack = 1;
														SoundEngine.PlaySound(SoundID.Grab);
													}
												}
											}
											else {
												display = true;
												enchantmentSlot.ClickInteractions(wePlayer.enchantingTableEnchantments, i);
											}
										}
									}
								}
								else {
									display = true;
									enchantmentSlot.ClickInteractions(wePlayer.enchantingTableEnchantments, i);
								}
							}
							else {
								display = true;
							}

							if (display && DisplayDescriptionBlock) {
								if (isUtilitySlot) {
									SetDescriptionBlock(TableTextID.utility0.ToString().Lang(L_ID1.TableText));
								}
								else {
									SetDescriptionBlock(TableTextID.enchantment0.ToString().Lang(L_ID1.TableText), TableTextID.enchantment4.ToString().Lang(L_ID1.TableText, new object[] { EnchantingTableItem.IDs[i].CSI().Name }));
								}
							}
						}
					}

					//Essence Slots Hover
					for (int i = 0; i < MaxEssenceSlots; i++) {
						UIItemSlotData essenceSlot = essenceSlotsData[i];
						if (essenceSlot.MouseHovering()) {
							if (WEModSystem.FavoriteKeyDown) {
								Main.cursorOverride = CursorOverrideID.FavoriteStar;
								if (UIManager.LeftMouseClicked) {
									wePlayer.enchantingTableEssence[i].favorited = !wePlayer.enchantingTableEssence[i].favorited;
									SoundEngine.PlaySound(SoundID.MenuTick);
								}
							}
							else if (ValidItemForEssenceSlot(Main.mouseItem, i)) {
								essenceSlot.ClickInteractions(ref wePlayer.enchantingTableEssence[i]);
							}

							if (DisplayDescriptionBlock)
								SetDescriptionBlock(TableTextID.essence0.ToString().Lang(L_ID1.TableText, new object[] { EnchantmentEssence.IDs[i].CSI().Name }));
						}
					}

					//Storage Hover
					if (storageData.MouseHovering()) {
						if (UIManager.LeftMouseClicked) {
							wePlayer.displayEnchantmentStorage = !wePlayer.displayEnchantmentStorage;
							if (!wePlayer.displayEnchantmentStorage)
								UIManager.SearchBarString = "";

							SoundEngine.PlaySound(SoundID.MenuTick);
							if (wePlayer.displayEnchantmentStorage) {
								SoundEngine.PlaySound(SoundID.MenuOpen);
							}
							else {
								SoundEngine.PlaySound(SoundID.MenuClose);
							}
						}
					}

					//Syphon Hover
					if (syphonData.MouseHovering()) {
						if (UIManager.LeftMouseClicked) {
							Syphon();
							SoundEngine.PlaySound(SoundID.MenuTick);
						}
					}

					//Infusion Hover
					if (infusionData.MouseHovering()) {
						if (UIManager.LeftMouseClicked) {
							Infusion();
							SoundEngine.PlaySound(SoundID.MenuTick);
						}
					}

					//Level Up Hover
					if (levelUpData.MouseHovering()) {
						if (UIManager.LeftMouseClicked) {
							LevelUp();
							SoundEngine.PlaySound(SoundID.MenuTick);
						}
					}

					//Level Per Level Up Buttons Hover
					for (int i = 0; i < LevelsPerLevelUp.Length; i++) {
						if (levelsPerData[i].MouseHovering()) {
							if (UIManager.LeftMouseClicked) {
								wePlayer.levelsPerLevelUp = LevelsPerLevelUp[i];
								SoundEngine.PlaySound(SoundID.MenuTick);
							}

							LevelsPerButtonScale[i] += 0.05f;
							if (LevelsPerButtonScale[i] > buttonScaleMaximum)
								LevelsPerButtonScale[i] = buttonScaleMaximum;
						}
						else {
							LevelsPerButtonScale[i] -= 0.05f;
							if (LevelsPerButtonScale[i] < buttonScaleMinimum)
								LevelsPerButtonScale[i] = buttonScaleMinimum;
						}
					}

					//Panel Drag
					if (panel.MouseHovering()) {
						panel.TryStartDraggingUI();
					}

					if (panel.ShouldDragUI())
						UIManager.DragUI(out wePlayer.enchantingTableUILeft, out wePlayer.enchantingTableUITop);
				}

				#endregion
			}
		}

		#region Pre UI Methods

		public static void RemoveTableItem() {
			WEPlayer wePlayer = WEPlayer.LocalWEPlayer;
			if (!wePlayer.infusionConsumeItem.IsAir) {
				if (!wePlayer.infusionConsumeItem.IsSameEnchantedItem(wePlayer.itemBeingEnchanted))
					wePlayer.itemBeingEnchanted.TryInfuseItem(wePlayer.previousInfusedItemName, true);
			}

			if (wePlayer.itemBeingEnchanted.TryGetEnchantedItemSearchAll(out EnchantedItem iBEGlobal))
				iBEGlobal.inEnchantingTable = false;

			wePlayer.itemBeingEnchanted.favorited = itemBeingEnchantedIsFavorited;
			wePlayer.itemBeingEnchanted = wePlayer.enchantingTableItem;//Stop tracking the item that just left the itemSlot
		}
		public static void OpenEnchantingTableUI(bool noSound = false) {
			WEPlayer.LocalWEPlayer.usingEnchantingTable = true;
			Main.playerInventory = true;
			if (!noSound)
				SoundEngine.PlaySound(SoundID.MenuOpen);

			if (WEPlayer.LocalWEPlayer.openStorageWhenOpeningTable)
				WEPlayer.LocalWEPlayer.displayEnchantmentStorage = true;
		}
		public static void CloseEnchantingTableUI(bool noSound = false) {
			WEPlayer wePlayer = WEPlayer.LocalWEPlayer;
			UIManager.TryResetSearch(UI_ID.EnchantmentStorageSearch);
			wePlayer.openStorageWhenOpeningTable = wePlayer.displayEnchantmentStorage;
			wePlayer.displayEnchantmentStorage = false;
			Item itemInUI = wePlayer.enchantingTableItem;
			if (!itemInUI.IsAir) {
				//Give item in table back to player
				wePlayer.enchantingTableItem = wePlayer.Player.GetItem(Main.myPlayer, itemInUI, GetItemSettings.LootAllSettings);

				//Clear item and enchantments from table
				if (wePlayer.enchantingTableItem.IsAir)
					RemoveTableItem();
			}

			wePlayer.itemBeingEnchanted = null;
			wePlayer.itemInEnchantingTable = false;
			wePlayer.usingEnchantingTable = false;
			if (wePlayer.Player.chest == -1) {
				if (!noSound)
					SoundEngine.PlaySound(SoundID.MenuClose);
			}

			Recipe.FindRecipes(true);
		}
		public static bool ValidItemForEnchantingSlot(Item item) {
			if (item.IsAir)
				return true;

			WEPlayer.LocalWEPlayer.enchantingTableItem.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem);
			if (enchantedItem != null) {
				if (item.type == PowerBooster.ID && !enchantedItem.PowerBoosterInstalled)
					return true;

				if (item.type == UltraPowerBooster.ID && !enchantedItem.UltraPowerBoosterInstalled)
					return true;
			}

			return EnchantedItemStaticMethods.IsEnchantable(item);
		}
		public static bool ValidItemForEnchantmentSlot(Item item, int slot, bool utility) {
			if (item.IsAir)
				return true;
			
			WEPlayer wePlayer = WEPlayer.LocalWEPlayer;
			wePlayer.enchantingTableItem.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem);
			if (enchantedItem == null)
				return false;

			bool useEnchantmentSlot = UseEnchantmentSlot(WEPlayer.LocalWEPlayer.enchantingTableItem, slot, utility);
			if (!useEnchantmentSlot)
				return false;

			bool isEnchantmentItem = IsValidEnchantmentForSlot(item, utility);
			if (!isEnchantmentItem)
				return false;

			Enchantment newEnchantment = ((Enchantment)item.ModItem);
			if (!EnchantmentAllowedOnItem(wePlayer.enchantingTableItem, newEnchantment))
				return false;

			int currentEnchantmentLevelCost = 0;
			if (wePlayer.enchantingTableEnchantments[slot].ModItem is Enchantment enchantment)
				currentEnchantmentLevelCost = enchantment.GetCapacityCost();

			int levelsAvailable = enchantedItem.GetLevelsAvailable();
			int newEnchantmentCost = newEnchantment.GetCapacityCost();

			return levelsAvailable >= newEnchantmentCost - currentEnchantmentLevelCost;
		}
		public static bool ValidItemForEssenceSlot(Item item, int slot) {
			if (item.IsAir)
				return true;
			
			return item.ModItem is EnchantmentEssence essence && essence.EssenceTier == slot;
		}
		public static bool UseEnchantmentSlot(Item item, int slot, bool utilitySlot = false, bool useHighestTableTier = false) {
			WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();

			if (slot > (useHighestTableTier ? wePlayer.highestTableTierUsed : wePlayer.enchantingTableTier) && !utilitySlot)
				return false;

			return SlotAllowedByConfig(item, slot);
		}
		public static bool SlotAllowedByConfig(Item item, int slot) {
			int configSlots;

			if (item.NullOrAir()) {
				configSlots = ConfigValues.EnchantmentSlotsOnItems.Max();
			}
			else if (item.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem)) {
				configSlots = ConfigValues.EnchantmentSlotsOnItems[(int)enchantedItem.ItemType - 1];
			}
			else {
				configSlots = 0;
			}

			if (configSlots <= 0)
				return false;

			if (configSlots == 1)
				return slot == 0;

			int maxIndex = MaxEnchantmentSlots - 1;

			return slot == maxIndex || slot <= configSlots - 2;
		}
		public static bool IsValidEnchantmentForSlot(Item item, bool utility) {
			if (item.ModItem is Enchantment enchantment) {
				if (utility) {
					return enchantment.Utility || ConfigValues.RemoveEnchantmentRestrictions;
				}
				else {
					return true;
				}
			}
			else {
				return false;
			}
		}
		public static bool EnchantmentAllowedOnItem(Item item, Enchantment newEnchantment) {
			if (ConfigValues.RemoveEnchantmentRestrictions)
				return true;

			if (EnchantedItemStaticMethods.IsEnchantable(item)) {
				int damageType = item.type.CSI().DamageType.Type;

				int damageClassSpecific = Enchantment.GetDamageClass(damageType);

				if (newEnchantment.DamageClassSpecific != 0 && damageClassSpecific != newEnchantment.DamageClassSpecific)
					return false;

				if (newEnchantment.RestrictedClass.Contains(damageClassSpecific))
					return false;
			}

			if (!CheckAllowedList(item, newEnchantment))
				return false;

			if (newEnchantment.ArmorSlotSpecific > -1) {
				int slot = -1;
				switch (newEnchantment.ArmorSlotSpecific) {
					case (int)ArmorSlotSpecificID.Head:
						slot = item.headSlot;
						break;
					case (int)ArmorSlotSpecificID.Body:
						slot = item.bodySlot;
						break;
					case (int)ArmorSlotSpecificID.Legs:
						slot = item.legSlot;
						break;
				}

				if (slot == -1)
					return false;
			}

			return true;
		}
		public static bool CheckAllowedList(Item item, Enchantment enchantment) {
			if (ConfigValues.RemoveEnchantmentRestrictions)
				return true;

			if (item.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem)) {
				bool allowedOnItem = enchantment.AllowedList.ContainsKey(enchantedItem.ItemType);

				return allowedOnItem;
			}

			return false;
		}
		public static bool CheckUniqueSlot(Enchantment enchantment, int swapEnchantmentSlot, int slot) {
			return ConfigValues.RemoveEnchantmentRestrictions || ((!enchantment.Unique && !enchantment.Max1) || swapEnchantmentSlot == -1 || swapEnchantmentSlot == slot);
		}
		public static int FindSwapEnchantmentSlot(Enchantment enchantement, Item item) {
			if (ConfigValues.RemoveEnchantmentRestrictions)
				return -1;

			for (int i = 0; i < MaxEnchantmentSlots; i++) {
				if (item.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem)) {
					if (enchantedItem.enchantments[i].ModItem is Enchantment appliedEnchantment && (enchantement.Unique && appliedEnchantment.Unique || enchantement.Max1 && enchantement.EnchantmentTypeName == appliedEnchantment.EnchantmentTypeName))
						return i;
				}
			}

			return -1;
		}

		#endregion

		#region UI Methods

		private static void SetDescriptionBlock(string firstLine, string lastLine = null) {
			List<string> lines = new() { firstLine };
			for (int j = 1; j <= 3; j++) {
				lines.Add($"general{j}".Lang(L_ID1.TableText));
			}

			if (lastLine != null)
				lines.Add(lastLine);

			lines.PadStrings();
			descriptionBlock = lines.JoinList("\n");
		}

		#endregion

		#region Button Methods

		private static void LootAll() {
			pressedLootAll = true;
			WEPlayer wePlayer = WEPlayer.LocalWEPlayer;
			if (!LootAllEnchantments(ref wePlayer.enchantingTableItem))
				return;

			Player player = wePlayer.Player;
			GetItemSettings lootSettings = GetItemSettings.LootAllSettings;
			ref Item item = ref wePlayer.enchantingTableItem;
			if (!item.IsAir) {
				item.position = player.Center;
				item = player.GetItem(Main.myPlayer, item, lootSettings);
			}

			pressedLootAll = false;
		}
		private static bool LootAllEnchantments(ref Item item, bool quickSpawnIfNeeded = false) {
			WEPlayer wePlayer = WEPlayer.LocalWEPlayer;
			Player player = wePlayer.Player;
			GetItemSettings lootSettings = GetItemSettings.LootAllSettings;
			bool quickSpawn = false;
			if (!item.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem))
				return false;

			for (int i = 0; i < MaxEnchantmentSlots; i++) {
				Item enchantmentItem = enchantedItem.enchantments[i];
				if (!enchantmentItem.IsAir) {
					if (wePlayer.displayEnchantmentStorage && EnchantmentStorage.CanBeStored(enchantmentItem) && EnchantmentStorage.RoomInStorage(enchantmentItem)) {
						EnchantmentStorage.DepositAll(ref enchantmentItem);
					}
					else if (!quickSpawn) {
						enchantmentItem.position = player.Center;
						enchantmentItem = player.GetItem(Main.myPlayer, enchantmentItem, lootSettings);
						enchantedItem.enchantments[i] = enchantmentItem;
						if (!enchantmentItem.IsAir) {
							if (quickSpawnIfNeeded) {
								quickSpawn = true;
							}
							else {
								return false;
							}
						}
					}

					if (quickSpawn) {
						Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), enchantmentItem, enchantmentItem.stack);
						enchantedItem.enchantments[i] = new();
					}
				}
			}

			return true;
		}
		private static void ConvertEssenceToXP(int tier) {
			WEPlayer wePlayer = WEPlayer.LocalWEPlayer;
			Item essence = wePlayer.enchantingTableEssence[tier];
			Item item = wePlayer.enchantingTableItem;
			if (essence.IsAir || item.IsAir)
				return;

			if (!item.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem))
				return;

			if (enchantedItem.Experience < int.MaxValue) {
				essence.stack--;
				int xp = (int)EnchantmentEssence.xpPerEssence[tier];
				enchantedItem.GainXP(item, xp);
				SoundEngine.PlaySound(SoundID.MenuTick);
			}
			else {
				Main.NewText($"You cannot gain any more experience on your {item.S()}.");
			}
		}
		public static int ConvertXPToEssence(int xp, bool consumeAll = false, Item item = null) {
			if (xp <= 0)
				return 0;

			WEPlayer wePlayer = WEPlayer.LocalWEPlayer;
			//Apply table tier reduction
			int xpCounter;
			if (WEMod.serverConfig.ReduceOfferEfficiencyByTableTier && wePlayer.highestTableTierUsed < 4) {
				//Tier 3 or lower table
				float essenceReduction = 0.6f + 0.1f * wePlayer.highestTableTierUsed;
				xpCounter = (int)Math.Round(xp * essenceReduction);
			}
			else {
				//Tier 4 table
				xpCounter = xp;
			}

			if (item?.TryGetEnchantedItem(out EnchantedWeapon enchantedWeapon) == true && WEMod.serverConfig.ReduceOfferEfficiencyByBaseInfusionPower) {
				float infusionPower = Math.Min((float)enchantedWeapon.InfusionPower, 1100f);
				xpCounter = (int)Math.Round((float)xpCounter * (1f - 0.2f * (infusionPower / 1100f)));
			}

			int xpInitial = xpCounter;
			int xpNotConsumed = 0;
			int numberEssenceRecieved;
			for (int tier = EnchantingTableUI.MaxEssenceSlots - 1; tier >= 0; tier--) {
				if (wePlayer.highestTableTierUsed < tier)
					continue;

				int xpPerEssence = (int)EnchantmentEssence.xpPerEssence[tier];
				if (tier > 0) {
					numberEssenceRecieved = xpCounter / xpPerEssence * 4 / 5;
				}
				else {
					numberEssenceRecieved = xpCounter / xpPerEssence;
				}

				xpCounter -= (int)EnchantmentEssence.xpPerEssence[tier] * numberEssenceRecieved;
				if (tier == 0 && xpCounter > 0) {
					if (consumeAll) {
						numberEssenceRecieved++;
					}
					else {
						xpNotConsumed = xpCounter;
					}
					xpCounter = 0;
				}

				//Get or spawn essence
				int maxStack = EnchantmentEssence.IDs[tier].CSI().maxStack;
				while (numberEssenceRecieved > 0) {
					int stack = numberEssenceRecieved > maxStack ? maxStack : numberEssenceRecieved;
					numberEssenceRecieved -= stack;
					Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), EnchantmentEssence.IDs[tier], stack);
				}
			}

			return xpInitial - xpNotConsumed;
		}
		private static void Syphon() {
			WEPlayer wePlayer = WEPlayer.LocalWEPlayer;
			Item item = wePlayer.enchantingTableItem;

			if (item.IsAir)
				return;

			if (!item.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem))
				return;

			int maxLevelXP = WEModSystem.levelXps[EnchantedItem.MAX_Level - 1];
			int smallestXpPerEssence = (int)EnchantmentEssence.xpPerEssence[0];
			int minimumXPToSyphon = maxLevelXP + smallestXpPerEssence;
			if (enchantedItem.Experience < minimumXPToSyphon) {
				Main.NewText($"You can only Syphon an item if it is max level and over {minimumXPToSyphon} experience.");
			}
			else {
				int xp = enchantedItem.Experience - maxLevelXP;
				enchantedItem.Experience -= ConvertXPToEssence(xp, item: item);
			}
		}
		private static void Infusion() {
			WEPlayer wePlayer = WEPlayer.LocalWEPlayer;
			Item tableItem = wePlayer.enchantingTableItem;
			if (EnchantedItemStaticMethods.IsEnchantable(tableItem)) {
				tableItem.InfusionAllowed(out bool infusionAllowed);
				if (!infusionAllowed)
					return;

				if (wePlayer.infusionConsumeItem.IsAir) {

					bool canConsume = false;

					//Prevent specific items from being consumed for infusion.
					switch (tableItem.Name) {
						case "Murasama":
							Main.NewText("Murasama cannot be consumed for infusion until a check for the Yharon, Dragon of Rebirth being defeated can be added.");
							break;
						default:
							canConsume = true;
							break;
					}

					if (!canConsume)
						return;

					//Store item for infusion
					if (tableItem.stack > 1) {
						wePlayer.enchantingTableItem.stack -= 1;
						wePlayer.infusionConsumeItem = new Item(tableItem.type);
					}
					else {
						if (wePlayer.enchantingTableItem.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem) && enchantedItem.favorited) {
							Main.NewText("Favorited items cannot be consumed for infusion.");
							return;
						}

						wePlayer.infusionConsumeItem = tableItem.Clone();
						wePlayer.enchantingTableItem = new Item();
					}
				}
				else {
					bool canInfuse = false;

					//Prevent specific items from being upgraded with infusion.
					switch (tableItem.Name) {
						case "Primary Zenith":
							Main.NewText($"{tableItem.Name} resisted your attempt to empower it.");
							break;
						default:
							canInfuse = true;
							break;
					}

					if (!canInfuse)
						return;

					//Infuse (Finalize)
					if (wePlayer.enchantingTableItem.TryInfuseItem(wePlayer.infusionConsumeItem, false, true)) {
						OfferItem(ref wePlayer.infusionConsumeItem, true, true);//Come Back to here
						wePlayer.infusionConsumeItem = new();
					}
				}
			}
			else if (!wePlayer.infusionConsumeItem.IsAir) {
				//Return infusion item to table
				wePlayer.enchantingTableItem = wePlayer.infusionConsumeItem.Clone();
				wePlayer.infusionConsumeItem = new();
			}
		}
		private static void LevelUp() {
			WEPlayer wePlayer = WEPlayer.LocalWEPlayer;
			Item item = wePlayer.enchantingTableItem;
			if (!item.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem))
				return;

			int xpAvailable = 0;
			int nonFavoriteXpAvailable = 0;
			if (enchantedItem.levelBeforeBooster == EnchantedItem.MAX_Level) {
				Main.NewText("Your " + item.Name + " is already max level.");
				return;
			}

			//xpAvailable
			for (int i = EnchantingTableUI.MaxEnchantmentSlots - 1; i >= 0; i--) {
				int xpToAdd = WEMath.MultiplyCheckOverflow((int)EnchantmentEssence.xpPerEssence[i], wePlayer.enchantingTableEssence[i].stack);
				xpAvailable.AddCheckOverflow(xpToAdd);
				if (!wePlayer.enchantingTableEssence[i].favorited)
					nonFavoriteXpAvailable.AddCheckOverflow(xpToAdd);
			}

			//xpNeeded
			int targetLevel = enchantedItem.levelBeforeBooster + wePlayer.levelsPerLevelUp - 1;
			targetLevel.Clamp(max: EnchantedItem.MAX_Level - 1);
			int xpNeeded = WEModSystem.levelXps[targetLevel] - enchantedItem.Experience;
			bool enoughWithoutFavorite = nonFavoriteXpAvailable >= xpNeeded;
			if (xpAvailable < xpNeeded) {
				Main.NewText("Not Enough Essence. You need " + xpNeeded + " experience for level " + (targetLevel + 1).ToString() + " you only have " + xpAvailable + " available.");
				return;
			}

			//Consume xp and convert to essence
			int totalXPToGain = 0;
			for (int i = EnchantingTableUI.MaxEnchantmentSlots - 1; i >= 0; i--) {
				Item essenceItem = wePlayer.enchantingTableEssence[i];
				bool allowUsingThisEssence = !essenceItem.favorited || !enoughWithoutFavorite;
				int stack = essenceItem.stack;
				int xpPerEssence = (int)EnchantmentEssence.xpPerEssence[i];
				int numberEssenceNeeded = xpNeeded / xpPerEssence;
				int numberEssenceTransfered = 0;
				if (allowUsingThisEssence) {
					if (numberEssenceNeeded > stack) {
						numberEssenceTransfered = stack;
					}
					else {
						numberEssenceTransfered = numberEssenceNeeded;
					}
				}

				//Check essence available below me
				int xpAvailableBelowThis = 0;
				for (int j = i - 1; j >= 0; j--) {
					if (!wePlayer.enchantingTableEssence[j].favorited || !enoughWithoutFavorite) {
						int xpPerEssenceLowerTier = (int)EnchantmentEssence.xpPerEssence[j];
						xpAvailableBelowThis += xpPerEssenceLowerTier * wePlayer.enchantingTableEssence[j].stack;
					}
				}

				if (allowUsingThisEssence && xpAvailableBelowThis < xpNeeded - xpPerEssence * numberEssenceTransfered)
					numberEssenceTransfered++;

				if (numberEssenceTransfered > 0) {
					int xpTransfered = xpPerEssence * numberEssenceTransfered;
					xpNeeded -= xpTransfered;
					essenceItem.stack -= numberEssenceTransfered;
					totalXPToGain += xpTransfered;
				}
			}

			if (totalXPToGain > 0)
				enchantedItem.GainXP(item, totalXPToGain);
		}
		private static string GetOfferWarning() {
			WEPlayer wePlayer = WEPlayer.LocalWEPlayer;
			string warning;
			if (!wePlayer.enchantingTableItem.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem)) {
				warning = $"Non-Enchantable item detected in table.\n" +
					$"WARNING, DO NOT PRESS CONFIRM.\n" +
					$"Please report this issue to andro951(Weapon Enchantments)";
			}
			else {
				int oresEnd = !WEMod.serverConfig.AllowHighTierOres || !Main.hardMode ? 3 : 8;
				bool canGetChlorophyte = NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3;
				if (canGetChlorophyte)
					oresEnd++;

				string oreString = $"({WorldDataManager.GetOreNamesList(1, oresEnd)})";
				float percentEss = ConfigValues.PercentOfferEssence;
				string oreAndEssencePercent;
				if (percentEss == 1f) {
					//oreAndEssencePercent = $"In exchange for essence?";
					oreAndEssencePercent = TableTextID.ExchangeEssence.ToString().Lang(L_ID1.TableText);
				}
				else if (percentEss == 0f) {
					//oreAndEssencePercent = $"In exchange for ores?";
					oreAndEssencePercent = TableTextID.ExchangeOres.ToString().Lang(L_ID1.TableText);
				}
				else {
					//oreAndEssencePercent = $"In exchange for ores({(1f - percentEss).PercentString()}) and essence({percentEss.PercentString()})?";
					oreAndEssencePercent = TableTextID.ExchangeEssenceAndOres.ToString().Lang(L_ID1.TableText, new object[] { (1f - percentEss).PercentString(), percentEss.PercentString() });
				}

				object[] args = new object[] { enchantedItem.level.ToString(), wePlayer.enchantingTableItem.Name, oreAndEssencePercent, percentEss < 1f ? $"{oreString}\n" : "" };
				warning = TableTextID.AreYouSure.ToString().Lang(L_ID1.TableText, args);
			}

			return warning;
		}
		private static void Offer() {
			int type = OfferItem(ref WEPlayer.LocalWEPlayer.enchantingTableItem, nonTableItem: false);
			if (type == 0)
				return;

			WEPlayer.LocalWEPlayer.allOfferedItems.Add(type.GetItemIDOrName());

			if (!WEMod.clientConfig.OfferAll)
				return;

			Player player = Main.LocalPlayer;

			//Offer every non-Modified item with the same type in the player's inventory.
			for (int i = 0; i < player.inventory.Length; i++) {
				if (player.inventory[i].favorited)
					continue;

				if (!player.inventory[i].TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem))
					continue;

				//Offer the inventory item
				if (player.inventory[i].type == type && !enchantedItem.Modified)
					OfferItem(ref player.inventory[i]);
			}

			if (FindEnchantingTable(player, out Point tablePoint)) {
				if (FindChestsInRange(tablePoint.X, tablePoint.Y, out List<int> chests, xRangeLeft: 2, xRangeRight: 2, yRangeUp: -1, yRangeDown: 1, exactPoints: true)) {
					SortedDictionary<int, SortedSet<short>> offeredChestItemSlots = new();
					foreach (int chestNum in chests) {
						int chestLength = Main.chest[chestNum].item.Length;
						for (int i = 0; i < chestLength; i++) {
							Item item = Main.chest[chestNum].item[i];
							if (item.favorited)
								continue;

							if (!item.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem))
								continue;

							if (enchantedItem.Modified)
								continue;

							WEPlayer.LocalWEPlayer.allOfferedItems.Add(item.type.GetItemIDOrName());
							if (OfferItem(ref Main.chest[chestNum].item[i]) > -1 && Main.netMode == NetmodeID.MultiplayerClient)
								offeredChestItemSlots.AddOrCombine(chestNum, (short)i);
						}
					}

					if (Main.netMode == NetmodeID.MultiplayerClient)
						Net<INetMethods>.Proxy.NetOfferChestItems(offeredChestItemSlots);
				}
			}
		}
		public static void OfferChestItems(SortedDictionary<int, SortedSet<short>> chestItems) {
			if (Main.netMode != NetmodeID.Server)
				return;

			foreach (KeyValuePair<int, SortedSet<short>> chest in chestItems) {
				foreach (int itemIndex in chest.Value) {
					Main.chest[chest.Key].item[itemIndex] = new();
				}
			}
		}
		public static bool FindEnchantingTable(Player player, out Point table) {
			table = new();
			Point clicked = player.GetWEPlayer().enchantingTableLocation;
			if (clicked.X == -1 && clicked.Y == -1)
				return false;

			int tileType = Main.tile[clicked.X - 1, clicked.Y].TileType;
			if (EnchantingTableTile.TableTypes.Contains(tileType)) {
				table = new(clicked.X - 1, clicked.Y);
			}
			else {
				table = new(clicked.X, clicked.Y);
			}

			return true;
		}
		public static bool FindChestsInRange(int xNum, int yNum, out List<int> chests, int xRangeRight = 0, int yRangeUp = 0, int xRangeLeft = int.MinValue, int yRangeDown = 0, bool exactPoints = false) {
			chests = new();
			Point low = new Point(xNum - xRangeLeft, yNum - yRangeDown);
			Point high = new Point(xNum + xRangeRight, yNum + yRangeUp);

			if (exactPoints) {
				CheckAddChest(low.X, low.Y, chests);
				CheckAddChest(high.X, high.Y, chests);
				return chests.Count > 0;
			}

			for (int x = low.X; x <= high.X; x++) {
				for (int y = low.Y; y <= high.Y; y++) {
					CheckAddChest(x, y, chests);
				}
			}

			return chests.Count > 0;
		}
		private static void CheckAddChest(int x, int y, List<int> chests) {
			if (Main.tileContainer[Main.tile[x, y].TileType]) {
				int chestNum = Chest.FindChest(x, y);
				if (chestNum != -1) {
					chests.Add(chestNum);
				}
			}
		}
		public static int OfferItem(ref Item item, bool noOre = false, bool nonTableItem = true) {
			WEPlayer wePlayer = WEPlayer.LocalWEPlayer;

			int type = item.type;
			if (!item.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem))
				return -1;

			//Enchantments
			if (!nonTableItem && !LootAllEnchantments(ref item))
				return -1;

			bool quickSpawn = false;
			LootAllEnchantments(ref item, true);

			//Power Booster
			if (enchantedItem.PowerBoosterInstalled)
				Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), ModContent.ItemType<PowerBooster>());

			//Ultra Power Booster
			if (enchantedItem.UltraPowerBoosterInstalled)
				Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), ModContent.ItemType<UltraPowerBooster>());

			int xp = enchantedItem.Experience;
			float value = (item.value - enchantedItem.lastValueBonus) * item.stack;

			//Xp -> Essence
			if (WEMod.magicStorageEnabled) $"OfferItem(item: {item}, noOre: {noOre.S()}, nonTableItem: {nonTableItem.S()})".Log();
			ConvertXPToEssence(xp, true, item);

			//Item value -> ores/essence
			if (!noOre) {
				int essenceValue = (int)(value * ConfigValues.PercentOfferEssence);
				int valueConvertedToOre = (int)Math.Round(value - (float)essenceValue);

				//Ores
				if (valueConvertedToOre > 0) {
					int[] ores = { ItemID.ChlorophyteOre, WorldDataManager.AdamantiteOre, WorldDataManager.MythrilOre, WorldDataManager.CobaltOre, WorldDataManager.GoldOre, WorldDataManager.SilverOre, WorldDataManager.IronOre };

					int indexStart;
					int indexOfGold = ores.Length - 3;
					bool canRecieveHighTierOres = WEMod.serverConfig.AllowHighTierOres && Main.hardMode;
					if (!canRecieveHighTierOres) {
						//Start at Gold
						indexStart = indexOfGold;
					}
					else {
						bool canGetChlorophyte = NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3;
						if (!canGetChlorophyte) {
							//Start at Adamantite
							indexStart = 1;
						}
						else {
							//Start at Chlorophyte
							indexStart = 0;
						}
					}

					for (int i = indexStart; i < ores.Length; i++) {
						int orevalue = ContentSamples.ItemsByType[ores[i]].value;
						int stack;
						int oreType = ores[i];
						if (oreType > ItemID.IronOre) {
							float oreWeightingMultiplier = i >= indexOfGold ? 0.8f : 0.2f;

							//Convert a portion of the remaining value into ore
							stack = (int)Math.Round(valueConvertedToOre * oreWeightingMultiplier / orevalue);
						}
						else {
							//Convert the rest of the remaining value to iron ore
							stack = (int)((float)valueConvertedToOre / (float)orevalue);
						}

						valueConvertedToOre -= stack * orevalue;

						//Round up for iron
						if (oreType == ItemID.IronOre)
							stack++;

						//Spawn ores
						if (stack > 0)
							Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), ores[i], stack);
					}
				}

				//Essence
				if (essenceValue > 0)
					ConvertXPToEssence(essenceValue, true, item);
			}

			item = new Item();
			SoundEngine.PlaySound(SoundID.Grab);

			return type;
		}

		#endregion

	}
}
