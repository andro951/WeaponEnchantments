using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Common.Utility;

namespace WeaponEnchantments.UI
{
	public static class EnchantingTableUI
	{
		private static int Spacing => 4;
		private static int PanelBorder => 10;
		private static int ButtonBorderY => 2;
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
		public static void PostDrawInterface(SpriteBatch spriteBatch) {
			WEPlayer wePlayer = WEPlayer.LocalWEPlayer;
			if (wePlayer.usingEnchantingTable || true) {
				wePlayer.enchantingTableEnchantments = wePlayer.enchantingTableItem.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem) ? enchantedItem.enchantments: wePlayer.emptyEnchantments;

				//Start of UI
				Color mouseColor = UIManager.MouseColor;

				//Item Label Data 1/2
				int itemLabelTop = wePlayer.enchantingTableUITop + Spacing;
				string itemLabel = TableTextID.Item.ToString().Lang(L_ID1.TableText);
				TextData itemLabelTextData = new(itemLabel);
				int largestWidthCenteredOnEnchantingItemSlotCenterX = itemLabelTextData.Width;

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

				//Syphon Button Data 1/2
				string syphon = TableTextID.Syphon.ToString().Lang(L_ID1.TableText);
				TextData syphonTextData = new(syphon);
				int largestWidthOfRightSideButtons = syphonTextData.Width;

				//Infusion Button Data 1/2
				int infusionButtonTop = enchantingItemSlotTop + syphonTextData.Height + Spacing + ButtonBorderY * 2;
				string infusion = TableTextID.Infusion.ToString().Lang(L_ID1.TableText);
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
					TextData thisLevelsper = new($"{LevelsPerLevelUp[i]}");//, 0.75f);
					levelsPerWidth += thisLevelsper.Width;
					levelsPerTextData[i] = thisLevelsper;
				}

				if (levelsPerWidth > largestWidthOfRightSideButtons)
					largestWidthOfRightSideButtons = levelsPerWidth;

				//Right Panel Buttons Data 2/2
				int rightPanelButtonsWidth = largestWidthOfRightSideButtons + ButtonBorderX * 2;
				int rightPanelButtonsCenterX = rightButtonsLeft + rightPanelButtonsWidth / 2;
				int rightPanelButtonsRightEdge = rightButtonsLeft + rightPanelButtonsWidth;

				//Syphon Button Data 2/2
				UIButtonData syphonData = new(UI_ID.EnchantingTableSyphon, rightButtonsLeft, enchantingItemSlotTop, syphonTextData, mouseColor, (rightPanelButtonsWidth - syphonTextData.Width) / 2, ButtonBorderY, BackGroundColor, HoverColor);

				//Infusion Button Data 2/2
				UIButtonData infusionData = new(UI_ID.EnchantingTableInfusion, rightButtonsLeft, infusionButtonTop, infusionTextData, mouseColor, (rightPanelButtonsWidth - infusionTextData.Width) / 2, ButtonBorderY, BackGroundColor, HoverColor);

				//Level Up Button Data 2/2
				UIButtonData levelUpData = new(UI_ID.EnchantingTableLevelUp, rightButtonsLeft, xpButtonTop, levelUpTextData, mouseColor, (rightPanelButtonsWidth - levelUpTextData.Width) / 2, ButtonBorderY, BackGroundColor, HoverColor);

				//Level Per Level Up Buttons Data 2/2
				int levelsPerTop = xpButtonTop - levelsPerTextData[0].Height + Spacing;
				UITextData[] levelsPerData = new UITextData[LevelsPerLevelUp.Length];
				int currentLevelsPerLeft = rightPanelButtonsCenterX - levelsPerWidth / 2;
				for (int i = 0; i < LevelsPerLevelUp.Length; i++) {
					Color color = wePlayer.levelsPerLevelUp == LevelsPerLevelUp[i] ? LevelSetColor : mouseColor;
					UITextData thisLevelsPer = new(UI_ID.EnchantingTableLevelsPerLevelUp0 + i, currentLevelsPerLeft, levelsPerTop, levelsPerTextData[i], color);
					currentLevelsPerLeft += thisLevelsPer.Width + ButtonBorderX;
					levelsPerData[i] = thisLevelsPer;
				}

				//Panel Data
				Point panelTopLeft = new(wePlayer.enchantingTableUILeft, wePlayer.enchantingTableUITop);
				Point panelBottomRight = new(rightPanelButtonsRightEdge + PanelBorder, xpButtonTop + xpData[0].Height + PanelBorder);
				UIPanelData panel = new(UI_ID.EnchantingTable, panelTopLeft, panelBottomRight, BackGroundColor);

				//Panel Draw
				panel.Draw(spriteBatch);

				//Item Label Draw
				itemLabelData.Draw(spriteBatch);

				//Enchanting Item Slot Draw
				UIItemSlotData enchantingItemSlotData = new(UI_ID.EnchantingTableItemSlot, enchantingItemSlotLeft, enchantingItemSlotTop);
				if (enchantingItemSlotData.MouseHovering()) {
					enchantingItemSlotData.ClickInteractions(ref wePlayer.enchantingTableItem);
				}

				enchantingItemSlotData.Draw(spriteBatch, wePlayer.enchantingTableItem, ItemSlotContextID.Gold);

				//Loot All Button Draw
				if (lootAllData.MouseHovering()) {
					if (UIManager.LeftMouseClicked)
						LootAll();
				}

				lootAllData.Draw(spriteBatch);

				//Offer Button Draw
				offerButtonData.Draw(spriteBatch);

				//Enchantments Label Draw
				enchantmentsLabelData.Draw(spriteBatch);

				//XP buttons Draw
				for (int i = 0; i < MaxEssenceSlots; i++) {
					xpData[i].Draw(spriteBatch);
				}

				//EnchantmentSlots Draw
				for (int i = 0; i < enchantmentSlotsCount; i++) {
					UIItemSlotData enchantmentSlot = new(UI_ID.EnchantingTableEnchantment0 + i, middleSlotsLefts[i], enchantingItemSlotTop);
					if (enchantmentSlot.MouseHovering())
						enchantmentSlot.ClickInteractions(wePlayer.enchantingTableEnchantments, i);

					enchantmentSlot.Draw(spriteBatch, wePlayer.enchantingTableEnchantments[i]);
				}

				//EssenceSlots Draw
				for (int i = 0; i < MaxEssenceSlots; i++) {
					UIItemSlotData essenceSlot = new(UI_ID.EnchantingTableEssence0 + i, middleSlotsLefts[i], essenecSlotsTop);
					if (essenceSlot.MouseHovering())
						essenceSlot.ClickInteractions(ref wePlayer.enchantingTableEssence[i]);

					essenceSlot.Draw(spriteBatch, wePlayer.enchantingTableEssence[i], ItemSlotContextID.Purple);
				}

				//Utility Label Draw
				utilityLabelData.Draw(spriteBatch);

				//Utility Slot Draw
				UIItemSlotData utilitySlotData = new(UI_ID.EnchantingTableEssence0 + enchantmentSlotsCount, utilityCenterX - UIManager.ItemSlotSize / 2, enchantingItemSlotTop);
				if (utilitySlotData.MouseHovering())
					utilitySlotData.ClickInteractions(wePlayer.enchantingTableEnchantments, enchantmentSlotsCount);

				utilitySlotData.Draw(spriteBatch, wePlayer.enchantingTableEnchantments[enchantmentSlotsCount], ItemSlotContextID.Favorited);

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

				if (panel.MouseHovering()) {
					panel.TryStartDraggingUI();
				}

				if (panel.ShouldDragUI())
					UIManager.DragUI(out wePlayer.enchantingTableUILeft, out wePlayer.enchantingTableUITop);
			}
		}
		private static void LootAll() {
			WEPlayer wePlayer = WEPlayer.LocalWEPlayer;
			Player player = wePlayer.Player;
			GetItemSettings lootSettings = GetItemSettings.LootAllSettings;
			for (int i = 0; i < MaxEnchantmentSlots; i++) {
				Item enchantmentItem = wePlayer.enchantingTableEnchantments[i];
				if (!enchantmentItem.IsAir) {
					enchantmentItem.position = player.Center;
					enchantmentItem = player.GetItem(Main.myPlayer, enchantmentItem, lootSettings);
					wePlayer.enchantingTableEnchantments[i] = enchantmentItem;
					if (!enchantmentItem.IsAir)
						return;
				}
			}

			ref Item item = ref wePlayer.enchantingTableItem;
			if (!item.IsAir) {
				item.position = player.Center;
				item = player.GetItem(Main.myPlayer, item, lootSettings);
			}
		}
	}
}
