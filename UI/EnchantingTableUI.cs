using KokoLib;
using MagicStorage;
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
using Terraria.ModLoader.Default;
using Terraria.UI;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Configs;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Common.Utility.LogSystem;
using WeaponEnchantments.Content.NPCs;
using WeaponEnchantments.Items;
using WeaponEnchantments.Items.Enchantments;
using WeaponEnchantments.ModLib.KokoLib;
using WeaponEnchantments.Tiles;
using androLib.Common.Utility;
using androLib.UI;
using androLib;
using androLib.Common.Globals;

namespace WeaponEnchantments.UI
{
	public static class EnchantingTableUI
	{
		private static int GetUI_ID(int id) => MasterUIManager.GetUI_ID(id, WE_UI_ID.EnchantingTable_UITypeID);
		public static int DefaultLeft => 560;
		public static int DefaultTop => 550;
		private static int Spacing => 4;
		private static int PanelBorder => 10;
		private static int ButtonBorderY => 0;
		private static int ButtonBorderX => 6;
		public static Color RedButtonColor => new Color(255, 30, 30, UIManager.UIAlpha);
		public static Color RedHoverColor => new Color(255, 70, 70, UIManager.UIAlphaHovered);
		public static Color BackGroundColor => new Color(50, 70, 171, UIManager.UIAlpha);
		public static Color HoverColor => new Color(100, 118, 184, UIManager.UIAlphaHovered);
		public static Color LevelSetColor => new Color(73, 170, 118, UIManager.UIAlphaHovered);
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
		private static bool DisplayDescriptionBlock => MasterUIManager.HoverTime >= 15;
		public static void PostDrawInterface(SpriteBatch spriteBatch) {
			WEPlayer wePlayer = WEPlayer.LocalWEPlayer;
			if (wePlayer.usingEnchantingTable) {
				//If player is too far away, close the enchantment table
				if (!wePlayer.InEnchantingTableInteractionRange() || wePlayer.Player.chest != -1 || !Main.playerInventory) {
					CloseEnchantingTableUI();
					return;
				}
			}
			else {
				return;
			}

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
				}

				if (!wePlayer.infusionConsumeItem.IsAir && itemBeingEnchanted.InfusionAllowed())
					wePlayer.itemBeingEnchanted.TryInfuseItem(wePlayer.infusionConsumeItem);
			}

			//Prevent Trash can and other mouse overides when using enchanting table
			if (!wePlayer.displayEnchantmentLoadoutUI && ItemSlot.ShiftInUse && MasterUIManager.NoUIBeingHovered && (Main.mouseItem.IsAir && !Main.HoverItem.IsAir || Main.cursorOverride == CursorOverrideID.TrashCan)) {
				if (!wePlayer.CheckShiftClickValid(ref Main.HoverItem) || Main.cursorOverride == CursorOverrideID.TrashCan)
					Main.cursorOverride = -1;
			}

			#endregion

			#region Data and Draw

			//Start of UI
			Color mouseColor = MasterUIManager.MouseColor;

			if (wePlayer.displayEnchantmentLoadoutUI) {
				//Loadouts Button Data 1/2
				string loadouts2 = TableTextID.Loadouts.ToString().Lang_WE(L_ID1.TableText);
				TextData loadoutsTextData2 = new(loadouts2);

				//Loadouts Button Data 2/2
				int loadoutsTop2 = wePlayer.enchantingTableUITop + PanelBorder;
				UIButtonData loadoutsData2 = new(GetUI_ID(WE_UI_ID.EnchantingTableLoadoutsButton), wePlayer.enchantingTableUILeft + PanelBorder, loadoutsTop2, loadoutsTextData2, mouseColor, ButtonBorderX, ButtonBorderY, BackGroundColor, HoverColor);

				//Description Block
				TextData loadoutsDescriptionBlockTextData = new(descriptionBlock);
				int loadoutsDescriptionBlockLoadoutsTop = loadoutsTop2 - (descriptionBlock != null ? loadoutsDescriptionBlockTextData.Height + Spacing : 0);
				int loadoutsDescriptionBlockTop = loadoutsTop2 - (descriptionBlock != null ? loadoutsDescriptionBlockTextData.Height + Spacing : 0);
				UITextData loadoutsDescriptionBlockData = new(WE_UI_ID.None, wePlayer.enchantingTableUILeft + PanelBorder, loadoutsDescriptionBlockTop, loadoutsDescriptionBlockTextData, mouseColor);

				//Panel Data
				Point panelTopLeft2 = new(wePlayer.enchantingTableUILeft, wePlayer.enchantingTableUITop - (descriptionBlock != null ? loadoutsDescriptionBlockTextData.Height + Spacing : 0));
				Point panelBottomRight2 = new((descriptionBlock != null ? Math.Max((int)loadoutsData2.BottomRight.X + PanelBorder, wePlayer.enchantingTableUILeft + 2 * PanelBorder + loadoutsDescriptionBlockTextData.Width) : (int)loadoutsData2.BottomRight.X + PanelBorder), (int)loadoutsData2.BottomRight.Y + PanelBorder);
				UIPanelData panel2 = new(GetUI_ID(WE_UI_ID.EnchantingTable), panelTopLeft2, panelBottomRight2, BackGroundColor);

				//Panel Draw
				panel2.Draw(spriteBatch);

				//Description Block Draw
				if (descriptionBlock != null) {
					loadoutsDescriptionBlockData.Draw(spriteBatch);
					descriptionBlock = null;
				}

				//Loadouts Button Draw
				loadoutsData2.Draw(spriteBatch);

				//Loadouts Button Hover
				if (loadoutsData2.MouseHovering()) {
					if (MasterUIManager.LeftMouseClicked) {
						if (wePlayer.displayEnchantmentLoadoutUI) {
							EnchantmentLoadoutUI.Close();
						}
						else {
							EnchantmentLoadoutUI.Open();
						}

						SoundEngine.PlaySound(SoundID.MenuTick);
					}

					if (DisplayDescriptionBlock) {
						SetDescriptionBlock(TableTextID.LoadoutDescription.ToString().Lang_WE(L_ID1.TableText));
					}
				}

				//Panel Drag
				if (panel2.MouseHovering()) {
					panel2.TryStartDraggingUI();
				}

				if (panel2.ShouldDragUI())
					MasterUIManager.DragUI(out wePlayer.enchantingTableUILeft, out wePlayer.enchantingTableUITop);

				return;
			}

			//Item Label Data 1/2
			int itemLabelTop = wePlayer.enchantingTableUITop + Spacing;
			string itemLabel = TableTextID.Item.ToString().Lang_WE(L_ID1.TableText);
			TextData itemLabelTextData = new(itemLabel);
			int largestWidthCenteredOnEnchantingItemSlotCenterX = itemLabelTextData.Width;

			//Description Block Data 1/2
			TextData descriptionBlockTextData = new(descriptionBlock);
			int descriptionBlockTop = itemLabelTop - (descriptionBlock != null ? descriptionBlockTextData.Height + Spacing : 0);

			//Enchanting Item Slot Data 2/2
			int enchantingItemSlotTop = itemLabelTop + itemLabelTextData.Height + Spacing;

			//Loot All Data 1/2
			int lootAllTop = enchantingItemSlotTop + MasterUIManager.ItemSlotSize + Spacing + ButtonBorderY;
			string lootAll = TableTextID.LootAll.ToString().Lang_WE(L_ID1.TableText);
			TextData lootAllTextData = new(lootAll);
			if (lootAllTextData.Width > largestWidthCenteredOnEnchantingItemSlotCenterX)
				largestWidthCenteredOnEnchantingItemSlotCenterX = lootAllTextData.Width;

			//Offer Button Data 1/2
			int offerButtonTop = lootAllTop + lootAllTextData.Height + Spacing + ButtonBorderY * 2;
			string offerButton = TableTextID.Offer.ToString().Lang_WE(L_ID1.TableText);
			TextData offerButtonTextData = new(offerButton);
			if (offerButtonTextData.Width > largestWidthCenteredOnEnchantingItemSlotCenterX)
				largestWidthCenteredOnEnchantingItemSlotCenterX = offerButtonTextData.Width;

			//Left Panel Buttons Data 1/2
			int leftPanelButtonsWidth = largestWidthCenteredOnEnchantingItemSlotCenterX + ButtonBorderX * 2;

			//EnchantingItemSlot Data 1/2
			int enchantingItemSlotCenterX = wePlayer.enchantingTableUILeft + PanelBorder + leftPanelButtonsWidth / 2;
			int enchantingItemSlotLeft = enchantingItemSlotCenterX - MasterUIManager.ItemSlotSize / 2;

			//Left Panel Buttons Data 2/2
			int leftPanelButtonsRightEdge = enchantingItemSlotCenterX + leftPanelButtonsWidth / 2;

			//Description Block Data 2/2
			UITextData descriptionBlockData = new(WE_UI_ID.None, wePlayer.enchantingTableUILeft + PanelBorder, descriptionBlockTop, descriptionBlockTextData, mouseColor);

			//Item Label Data 2/2
			UITextData itemLabelData = new(WE_UI_ID.None, enchantingItemSlotCenterX, itemLabelTop, itemLabel, 1f, mouseColor, true);

			//Loot All Data 2/2
			UIButtonData lootAllData = new(GetUI_ID(WE_UI_ID.EnchantingTableLootAll), leftPanelButtonsRightEdge, lootAllTop, lootAllTextData, mouseColor, (leftPanelButtonsWidth - lootAllTextData.Width) / 2, ButtonBorderY, BackGroundColor, HoverColor, true);

			//Offer Button Data 2/2
			UIButtonData offerButtonData = new(GetUI_ID(WE_UI_ID.EnchantingTableOfferButton), leftPanelButtonsRightEdge, offerButtonTop, offerButtonTextData, mouseColor, (leftPanelButtonsWidth - offerButtonTextData.Width) / 2, ButtonBorderY, RedButtonColor, RedHoverColor, true);

			//Panel Middle Data 1/2
			int panelMiddleLeft = leftPanelButtonsRightEdge + Spacing;

			//Enchantments Label Data 1/2
			string enchantmentsLabel = TableTextID.Enchantments.ToString().Lang_WE(L_ID1.TableText);
			TextData enchantmentsLabelTextData = new(enchantmentsLabel);

			//XP button Data 1/2
			string xp = TableTextID.xp.ToString().Lang_WE(L_ID1.TableText);
			TextData xpTextData = new(xp);//, 0.75f);
			int enchantmentSlotsCount = MaxEnchantmentSlots - 1;
			int diff = (xpTextData.Width + ButtonBorderX * 2) - MasterUIManager.ItemSlotSize;
			int widthOfSlotButtonPair = Math.Max(xpTextData.Width, MasterUIManager.ItemSlotSize);
			int enchantmentSlotsWidth = widthOfSlotButtonPair * enchantmentSlotsCount + Spacing * (enchantmentSlotsCount - 1);

			//Panel Middle Data 2/2
			int panelMiddleRight = panelMiddleLeft + Math.Max(enchantmentSlotsWidth, enchantmentsLabelTextData.Width);
			int panelMiddleCenterX = (panelMiddleLeft + panelMiddleRight) / 2;

			//Enchantments Label Data 2/2
			UITextData enchantmentsLabelData = new(WE_UI_ID.None, panelMiddleCenterX, itemLabelTop, enchantmentsLabelTextData, mouseColor, true);

			//XP buttons Data 2/2
			UIButtonData[] xpData = new UIButtonData[MaxEnchantmentSlots];
			int[] middleSlotsLefts = new int[MaxEnchantmentSlots];
			int middleSlotsCurrentLeft = panelMiddleCenterX - enchantmentSlotsWidth / 2;
			int levelUpButtonTop = enchantingItemSlotTop + (MasterUIManager.ItemSlotSize + Spacing) * 2;
			for (int i = 0; i < MaxEnchantmentSlots; i++) {
				UIButtonData thisXpData = new(GetUI_ID(WE_UI_ID.EnchantingTableXPButton0) + i, middleSlotsCurrentLeft - diff / 2, levelUpButtonTop, xpTextData, mouseColor, ButtonBorderX, ButtonBorderY, BackGroundColor, HoverColor);
				xpData[i] = thisXpData;
				middleSlotsLefts[i] = middleSlotsCurrentLeft;
				middleSlotsCurrentLeft += widthOfSlotButtonPair + Spacing;
			}

			//Essence Slots Data
			int essenecSlotsTop = enchantingItemSlotTop + MasterUIManager.ItemSlotSize + Spacing;

			//Utility Label Data 1/2
			string utilityLabel = EnchantmentGeneralTooltipsID.Utility.ToString().Lang_WE(L_ID1.Tooltip, L_ID2.EnchantmentGeneralTooltips);
			TextData utilityLabelTextData = new(utilityLabel);

			//Utility Data
			int projectedUtilityMiddle = panelMiddleRight + Spacing + MasterUIManager.ItemSlotSize / 2;
			int diffEnchantmentsToUtilityLabel = panelMiddleCenterX - projectedUtilityMiddle + (enchantmentsLabelTextData.Width + utilityLabelTextData.Width) / 2;
			int extraSpaceNeeded = diffEnchantmentsToUtilityLabel > 0 ? diffEnchantmentsToUtilityLabel + Spacing * 2 : 0;
			int utilityLeft = panelMiddleRight + Spacing + extraSpaceNeeded;
			int utilityRight = utilityLeft + Math.Max(widthOfSlotButtonPair, utilityLabelTextData.Width);
			int utilityCenterX = (utilityLeft + utilityRight) / 2;

			//Utility Label Data 2/2
			UITextData utilityLabelData = new(WE_UI_ID.None, utilityCenterX, itemLabelTop, utilityLabelTextData, mouseColor, true);

			//Right Panel Buttons Data 1/2
			int rightButtonsLeft = utilityRight + Spacing;

			//Storage Button Data 1/2
			int storageButtonTop = itemLabelTop + 4;
			string storage = TableTextID.Storage.ToString().Lang_WE(L_ID1.TableText);
			TextData storageTextData = new(storage);
			int largestWidthOfRightSideButtons = storageTextData.Width;

			//Siphon Button Data 1/2
			int siphonTop = storageButtonTop + storageTextData.Height + Spacing + ButtonBorderY * 2;
			string siphon = TableTextID.Siphon.ToString().Lang_WE(L_ID1.TableText);
			TextData siphonTextData = new(siphon);
			if (siphonTextData.Width > largestWidthOfRightSideButtons)
				largestWidthOfRightSideButtons = siphonTextData.Width;

			//Infusion Button Data 1/2
			int infusionButtonTop = siphonTop + siphonTextData.Height + Spacing + ButtonBorderY * 2;
			string infusion;
			if (!wePlayer.infusionConsumeItem.IsAir) {
				if (wePlayer.enchantingTableItem.IsAir) {
					infusion = TableTextID.Cancel.ToString().Lang_WE(L_ID1.TableText);
				}
				else {
					infusion = TableTextID.Finalize.ToString().Lang_WE(L_ID1.TableText);
				}
			}
			else {
				infusion = TableTextID.Infusion.ToString().Lang_WE(L_ID1.TableText);
			}

			TextData infusionTextData = new(infusion);
			if (infusionTextData.Width > largestWidthOfRightSideButtons)
				largestWidthOfRightSideButtons = infusionTextData.Width;

			//Level Up Button Data 1/2
			string levelUp = TableTextID.LevelUp.ToString().Lang_WE(L_ID1.TableText);
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

			//Loadouts Button Data 1/2
			int loadoutsTop = storageButtonTop;
			string loadouts = TableTextID.Loadouts.ToString().Lang_WE(L_ID1.TableText);
			TextData loadoutsTextData = new(loadouts);
			int largestWidthOfRightSideButtonsCol2 = loadoutsTextData.Width;

			//Right Panel Buttons Col 2 Data 1/2
			int rightButtonsLeftCol2 = rightPanelButtonsRightEdge + Spacing;

			//Right Panel Buttons Col 2 Data 2/2
			int rightPanelButtonsWidthCol2 = largestWidthOfRightSideButtonsCol2 + ButtonBorderX * 2;
			int rightPanelButtonsCenterXCol2 = rightButtonsLeftCol2 + rightPanelButtonsWidthCol2 / 2;
			int rightPanelButtonsRightEdgeCol2 = rightButtonsLeftCol2 + rightPanelButtonsWidthCol2;

			//Storage Button Data 2/2
			UIButtonData storageData = new(GetUI_ID(WE_UI_ID.EnchantingTableStorageButton), rightButtonsLeft, storageButtonTop, storageTextData, mouseColor, (rightPanelButtonsWidth - storageTextData.Width) / 2, ButtonBorderY, BackGroundColor, HoverColor);

			//Siphon Button Data 2/2
			UIButtonData siphonData = new(GetUI_ID(WE_UI_ID.EnchantingTableSiphon), rightButtonsLeft, siphonTop, siphonTextData, mouseColor, (rightPanelButtonsWidth - siphonTextData.Width) / 2, ButtonBorderY, BackGroundColor, HoverColor);

			//Infusion Button Data 2/2
			UIButtonData infusionData = new(GetUI_ID(WE_UI_ID.EnchantingTableInfusion), rightButtonsLeft, infusionButtonTop, infusionTextData, mouseColor, (rightPanelButtonsWidth - infusionTextData.Width) / 2, ButtonBorderY, BackGroundColor, HoverColor);

			//Level Up Button Data 2/2
			UIButtonData levelUpData = new(GetUI_ID(WE_UI_ID.EnchantingTableLevelUp), rightButtonsLeft, levelUpButtonTop, levelUpTextData, mouseColor, (rightPanelButtonsWidth - levelUpTextData.Width) / 2, ButtonBorderY, BackGroundColor, HoverColor);

			//Level Per Level Up Buttons Data 2/2
			int levelsPerTop = levelUpButtonTop - levelsPerTextData[0].BaseHeight + Spacing;
			UITextData[] levelsPerData = new UITextData[LevelsPerLevelUp.Length];
			int currentLevelsPerLeft = rightPanelButtonsCenterX - levelsPerWidth / 2;
			for (int i = 0; i < LevelsPerLevelUp.Length; i++) {
				Color color = wePlayer.levelsPerLevelUp == LevelsPerLevelUp[i] ? LevelSetColor : mouseColor;
				UITextData thisLevelsPer = new(GetUI_ID(WE_UI_ID.EnchantingTableLevelsPerLevelUp0) + i, currentLevelsPerLeft, levelsPerTop, levelsPerTextData[i], color, ancorBotomLeft: true);
				currentLevelsPerLeft += thisLevelsPer.BaseWidth + ButtonBorderX;
				levelsPerData[i] = thisLevelsPer;
			}

			//Loadouts Button Data 2/2
			UIButtonData loadoutsData = new(GetUI_ID(WE_UI_ID.EnchantingTableLoadoutsButton), rightButtonsLeftCol2, loadoutsTop, loadoutsTextData, mouseColor, (rightPanelButtonsWidthCol2 - loadoutsTextData.Width) / 2, ButtonBorderY, BackGroundColor, HoverColor);

			//Panel Data
			Point panelTopLeft = new(wePlayer.enchantingTableUILeft, wePlayer.enchantingTableUITop - (descriptionBlock != null ? descriptionBlockTextData.Height + Spacing : 0));
			Point panelBottomRight = new((descriptionBlock != null ? Math.Max(rightPanelButtonsRightEdgeCol2, leftPanelButtonsRightEdge - leftPanelButtonsWidth + descriptionBlockTextData.Width) : rightPanelButtonsRightEdgeCol2) + PanelBorder, levelUpButtonTop + xpData[0].Height + PanelBorder);
			UIPanelData panel = new(GetUI_ID(WE_UI_ID.EnchantingTable), panelTopLeft, panelBottomRight, BackGroundColor);

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
			UIItemSlotData enchantingItemSlotData = new(GetUI_ID(WE_UI_ID.EnchantingTableItemSlot), enchantingItemSlotLeft, enchantingItemSlotTop);
			enchantingItemSlotData.Draw(spriteBatch, wePlayer.enchantingTableItem, ItemSlotContextID.GoldFavorited);

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
				bool canUseSlot = UseEnchantmentSlot(WEPlayer.LocalWEPlayer.enchantingTableItem, i);
				UIItemSlotData enchantmentSlot = new(GetUI_ID(WE_UI_ID.EnchantingTableEnchantment0) + i, middleSlotsLefts[i], enchantingItemSlotTop);
				enchantmentSlotsData[i] = enchantmentSlot;
				enchantmentSlot.Draw(spriteBatch, wePlayer.enchantingTableEnchantments[i], canUseSlot ? ItemSlotContextID.Normal : ItemSlotContextID.Red);
			}

			//Essence Slots Draw
			UIItemSlotData[] essenceSlotsData = new UIItemSlotData[MaxEssenceSlots];
			for (int i = 0; i < MaxEssenceSlots; i++) {
				UIItemSlotData essenceSlot = new(GetUI_ID(WE_UI_ID.EnchantingTableEssence0) + i, middleSlotsLefts[i], essenecSlotsTop);
				essenceSlotsData[i] = essenceSlot;
				essenceSlot.Draw(spriteBatch, wePlayer.enchantingTableEssence[i], ItemSlotContextID.Purple);
			}

			//Utility Label Draw
			utilityLabelData.Draw(spriteBatch);

			//Utility Slot Draw
			bool canUseUtilitySlot = UseEnchantmentSlot(WEPlayer.LocalWEPlayer.enchantingTableItem, MaxEnchantmentSlots - 1);
			UIItemSlotData utilitySlotData = new(GetUI_ID(WE_UI_ID.EnchantingTableEssence0) + enchantmentSlotsCount, utilityCenterX - MasterUIManager.ItemSlotSize / 2, enchantingItemSlotTop);
			utilitySlotData.Draw(spriteBatch, wePlayer.enchantingTableEnchantments[enchantmentSlotsCount], canUseUtilitySlot ? ItemSlotContextID.Normal : ItemSlotContextID.Red);
			enchantmentSlotsData[enchantmentSlotsCount] = utilitySlotData;

			//Storage Button Draw
			storageData.Draw(spriteBatch);

			//Siphon Button Draw
			siphonData.Draw(spriteBatch);

			//Infusion Button Draw
			infusionData.Draw(spriteBatch);

			//Level Up Button Draw
			levelUpData.Draw(spriteBatch);

			//Level Per Level Up Buttons Draw
			for (int i = 0; i < LevelsPerLevelUp.Length; i++) {
				levelsPerData[i].Draw(spriteBatch);
			}

			//Loadouts Button Draw
			loadoutsData.Draw(spriteBatch);

			#endregion

			if (DisplayOfferUI) {
				//Yes Data 1/2
				string yes = TableTextID.Yes.ToString().Lang_WE(L_ID1.TableText);
				TextData yesTextData = new(yes);

				//No Data 1/2
				string no = TableTextID.No.ToString().Lang_WE(L_ID1.TableText);
				TextData noTextData = new(no);

				//Yes Data 2/2
				UIButtonData yesData = new(GetUI_ID(WE_UI_ID.OfferYes), leftPanelButtonsRightEdge, lootAllTop, yesTextData, mouseColor, (leftPanelButtonsWidth - yesTextData.Width) / 2, ButtonBorderY, RedButtonColor, RedHoverColor, true);

				//No Data 2/2
				UIButtonData noData = new(GetUI_ID(WE_UI_ID.OfferNo), leftPanelButtonsRightEdge, offerButtonTop, noTextData, mouseColor, (leftPanelButtonsWidth - noTextData.Width) / 2, ButtonBorderY, BackGroundColor, HoverColor, true);

				//Offer Warning Data
				string offerWarning = GetOfferWarning();
				TextData offerWarningTextData = new(offerWarning);
				UITextData offerWarningData = new(WE_UI_ID.None, leftPanelButtonsRightEdge + Spacing, itemLabelTop + Spacing, offerWarningTextData, mouseColor);

				//Auto Trash Offered Data
				string autoTrashOffered = TableTextID.ToggleAutoTrashOfferedItems.ToString().Lang_WE(L_ID1.TableText);
				TextData autoTrashOfferedTextData = new(autoTrashOffered);
				Color autoTrashOfferedColor = wePlayer.autoTrashOfferedItems ? EnchantmentStorage.VacuumPurple : mouseColor;
				UIButtonData autoTrashOfferedData = new(GetUI_ID(WE_UI_ID.ToggleAutoTrashOfferedItems), offerWarningData.TopLeft.X, offerWarningData.BottomRight.Y + Spacing, autoTrashOfferedTextData, autoTrashOfferedColor, ButtonBorderX, ButtonBorderY, RedButtonColor, RedHoverColor);

				//Offer Panel Data
				Color offerRed = RedButtonColor;
				offerRed.A = 224;
				Point offerPanelBottomRight = new(Math.Max(panelBottomRight.X, offerWarningData.BottomRight.X + PanelBorder), Math.Max(panelBottomRight.Y, (int)autoTrashOfferedData.BottomRight.Y + PanelBorder));
				UIPanelData offerPanel = new(GetUI_ID(WE_UI_ID.Offer), panelTopLeft, offerPanelBottomRight, offerRed);

				//Offer Panel Draw
				offerPanel.Draw(spriteBatch);

				//Yes Draw
				yesData.Draw(spriteBatch);

				//No Draw
				noData.Draw(spriteBatch);

				//Offer Warning Draw
				offerWarningData.Draw(spriteBatch);

				//Offer Panel Draw
				autoTrashOfferedData.Draw(spriteBatch);

				//Yes Hover
				if (yesData.MouseHovering()) {
					if (MasterUIManager.LeftMouseClicked) {
						Offer();
						DisplayOfferUI = false;
						SoundEngine.PlaySound(SoundID.MenuTick);
					}
				}

				//No Hover
				if (noData.MouseHovering()) {
					if (MasterUIManager.LeftMouseClicked) {
						DisplayOfferUI = false;
						SoundEngine.PlaySound(SoundID.MenuTick);
						SoundEngine.PlaySound(SoundID.MenuClose);
					}
				}

				//Offer Panel Hover
				if (autoTrashOfferedData.MouseHovering()) {
					if (MasterUIManager.LeftMouseClicked) {
						wePlayer.autoTrashOfferedItems = !wePlayer.autoTrashOfferedItems;
						SoundEngine.PlaySound(SoundID.MenuTick);
					}
				}

				//Offer Panel Hover
				if (offerPanel.MouseHovering()) {
					offerPanel.TryStartDraggingUI();
				}

				if (offerPanel.ShouldDragUI())
					MasterUIManager.DragUI(out wePlayer.enchantingTableUILeft, out wePlayer.enchantingTableUITop);
			}
			else {
				//Enchanting Item Slot Hover
				if (enchantingItemSlotData.MouseHovering()) {
					ref Item item = ref wePlayer.enchantingTableItem;
					bool display = Main.mouseItem.IsAir && item.IsAir;
					bool normalClickInteractions = true;
					if (Main.mouseItem.IsAir) {
						if (!item.IsAir) {
							if (ItemSlot.ShiftInUse) {
								if (wePlayer.Player.ItemWillBeTrashedFromShiftClick(item)) {
									normalClickInteractions = false;
									if (MasterUIManager.LeftMouseClicked)
										MasterUIManager.SwapMouseItem(ref item);
								}
							}
						}
					}
					else {
						if (ValidItemForEnchantingSlot(Main.mouseItem)) {
							if (ItemSlot.ShiftInUse) {
								if (wePlayer.Player.ItemWillBeTrashedFromShiftClick(item) || item.IsAir) {
									normalClickInteractions = false;
									if (MasterUIManager.LeftMouseClicked) {
										if (item.IsAir)
											MasterUIManager.SwapMouseItem(ref item);
									}
								}
							}
							else {
								Main.cursorOverride = CursorOverrideID.CameraDark;
								if (Main.mouseItem.type == PowerBooster.ID) {
									normalClickInteractions = false;
									if (MasterUIManager.LeftMouseClicked) {
										if (item.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedTableItem) && !enchantedTableItem.PowerBoosterInstalled) {
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
									normalClickInteractions = false;
									if (MasterUIManager.LeftMouseClicked) {
										if (item.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedTableItem) && !enchantedTableItem.UltraPowerBoosterInstalled) {
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
							}
						}
						else {
							normalClickInteractions = false;
						}
					}

					if (display && DisplayDescriptionBlock)
						SetGenericDescriptionBlock(TableTextID.weapon0.ToString().Lang_WE(L_ID1.TableText));

					if (normalClickInteractions)
						enchantingItemSlotData.ClickInteractions(ref wePlayer.enchantingTableItem);
				}

				//Loot All Hover
				if (lootAllData.MouseHovering()) {
					if (MasterUIManager.LeftMouseClicked) {
						LootAll();
						SoundEngine.PlaySound(SoundID.MenuTick);
					}

					if (DisplayDescriptionBlock) {
						string itemName = wePlayer.enchantingTableItem.IsAir ? TableTextID.Item.ToString().Lang_WE(L_ID1.TableText) : wePlayer.enchantingTableItem.Name;
						SetDescriptionBlock(TableTextID.LootAllDescription.ToString().Lang_WE(L_ID1.TableText, new object[] { itemName }));
					}
				}

				//Offer Button Hover
				if (offerButtonData.MouseHovering()) {
					if (MasterUIManager.LeftMouseClicked) {
						SoundEngine.PlaySound(SoundID.MenuTick);
						if (!wePlayer.enchantingTableItem.IsAir) {
							DisplayOfferUI = true;
							SoundEngine.PlaySound(SoundID.MenuOpen);
						}
					}

					if (DisplayDescriptionBlock) {
						string itemName = wePlayer.enchantingTableItem.IsAir ? TableTextID.Item.ToString().Lang_WE(L_ID1.TableText) : wePlayer.enchantingTableItem.Name;
						SetDescriptionBlock(TableTextID.OfferDescription.ToString().Lang_WE(L_ID1.TableText, new object[] { itemName }));
					}
				}

				//XP buttons Hover
				for (int i = 0; i < xpData.Length; i++) {
					if (xpData[i].MouseHovering()) {
						if (MasterUIManager.LeftMouseClicked) {
							ConvertEssenceToXP(i);
							SoundEngine.PlaySound(SoundID.MenuTick);
						}

						if (DisplayDescriptionBlock) {
							string essenceName = EnchantmentEssence.IDs[i].CSI().Name;
							float xpPerEssence = EnchantmentEssence.xpPerEssence[i];
							string itemName = wePlayer.enchantingTableItem.IsAir ? TableTextID.Item.ToString().Lang_WE(L_ID1.TableText) : wePlayer.enchantingTableItem.Name;
							SetDescriptionBlock(TableTextID.XPButtonDescription.ToString().Lang_WE(L_ID1.TableText, new object[] { essenceName, xpPerEssence, itemName }));
						}
					}
				}

				//Enchantment Slots Hover
				for (int i = 0; i < MaxEnchantmentSlots; i++) {
					UIItemSlotData enchantmentSlot = enchantmentSlotsData[i];
					if (enchantmentSlot.MouseHovering()) {
						HandleEnchantmentSlot(enchantmentSlot, wePlayer, i);
					}
				}

				//Essence Slots Hover
				for (int i = 0; i < MaxEssenceSlots; i++) {
					UIItemSlotData essenceSlot = essenceSlotsData[i];
					if (essenceSlot.MouseHovering()) {
						ref Item item = ref wePlayer.enchantingTableEssence[i];
						bool display = Main.mouseItem.IsAir && item.IsAir;
						bool normalClickInteractions = true;
						if (WEModSystem.FavoriteKeyDown) {
							normalClickInteractions = false;
							Main.cursorOverride = CursorOverrideID.FavoriteStar;
							if (MasterUIManager.LeftMouseClicked) {
								item.favorited = !item.favorited;
								SoundEngine.PlaySound(SoundID.MenuTick);
							}
						}
						else if (Main.mouseItem.IsAir) {
							if (!item.IsAir) {
								if (ItemSlot.ShiftInUse) {
									if (wePlayer.Player.ItemWillBeTrashedFromShiftClick(item)) {
										normalClickInteractions = false;
										if (MasterUIManager.LeftMouseClicked)
											MasterUIManager.SwapMouseItem(ref item);
									}
								}
							}
						}
						else {
							if (ValidItemForEssenceSlot(Main.mouseItem, i)) {
								if (ItemSlot.ShiftInUse) {
									if (wePlayer.Player.ItemWillBeTrashedFromShiftClick(item) || item.IsAir) {
										normalClickInteractions = false;
										if (MasterUIManager.LeftMouseClicked) {
											if (item.IsAir)
												MasterUIManager.SwapMouseItem(ref item);
										}
									}
								}
								else {
									Main.cursorOverride = CursorOverrideID.CameraDark;
								}
							}
							else {
								normalClickInteractions = false;
							}
						}

						if (display && DisplayDescriptionBlock)
							SetGenericDescriptionBlock(TableTextID.essence0.ToString().Lang_WE(L_ID1.TableText, new object[] { EnchantmentEssence.IDs[i].CSI().Name }));

						if (normalClickInteractions)
							essenceSlot.ClickInteractions(ref item);
					}
				}

				//Storage Hover
				if (storageData.MouseHovering()) {
					if (MasterUIManager.LeftMouseClicked) {
						wePlayer.displayEnchantmentStorage = !wePlayer.displayEnchantmentStorage;
						if (!wePlayer.displayEnchantmentStorage)
							MasterUIManager.TypingBarString = "";

						SoundEngine.PlaySound(SoundID.MenuTick);
						if (wePlayer.displayEnchantmentStorage) {
							SoundEngine.PlaySound(SoundID.MenuOpen);
						}
						else {
							SoundEngine.PlaySound(SoundID.MenuClose);
						}
					}

					if (DisplayDescriptionBlock) {
						SetDescriptionBlock(TableTextID.StorageDescription.ToString().Lang_WE(L_ID1.TableText));
					}
				}

				//Siphon Hover
				if (siphonData.MouseHovering()) {
					if (MasterUIManager.LeftMouseClicked) {
						Siphon(ref wePlayer.enchantingTableItem);
						SoundEngine.PlaySound(SoundID.MenuTick);
					}

					if (DisplayDescriptionBlock) {
						int siphonXPCost = !wePlayer.enchantingTableItem.NullOrAir() && enchantedItem != null ? GetSiphonXPCost(wePlayer.enchantingTableItem, enchantedItem, out _) : 0;
						string itemName = wePlayer.enchantingTableItem.IsAir ? TableTextID.Item.ToString().Lang_WE(L_ID1.TableText) : wePlayer.enchantingTableItem.Name;
						SetDescriptionBlock(TableTextID.SiphonDescription.ToString().Lang_WE(L_ID1.TableText, new object[] { siphonXPCost, itemName }));
					}
				}

				//Infusion Hover
				if (infusionData.MouseHovering()) {
					if (MasterUIManager.LeftMouseClicked) {
						Infusion();
						MasterUIManager.HoverTime = 0;
						SoundEngine.PlaySound(SoundID.MenuTick);
					}

					if (DisplayDescriptionBlock) {
						if (!wePlayer.infusionConsumeItem.IsAir) {
							string infusiedItemName = wePlayer.infusionConsumeItem.Name;
							if (wePlayer.enchantingTableItem.IsAir) {
								//Cancel
								SetDescriptionBlock(TableTextID.InfusionCancelDescription.ToString().Lang_WE(L_ID1.TableText, new object[] { infusiedItemName }));
							}
							else {
								//Finalize
								if (CanDoInfusion(out string errorMessage)) {
									string itemName = wePlayer.enchantingTableItem.Name;
									string consumeItemName = wePlayer.infusionConsumeItem.Name;
									if (wePlayer.enchantingTableItem.TryGetEnchantedWeapon(out _)) {
										SetDescriptionBlock(TableTextID.InfusionFinalizeDescriptionWeapon.ToString().Lang_WE(L_ID1.TableText, new object[] { consumeItemName, itemName }));
									}
									else if (wePlayer.enchantingTableItem.TryGetEnchantedArmor(out _)) {
										SetDescriptionBlock(TableTextID.InfusionFinalizeDescriptionArmor.ToString().Lang_WE(L_ID1.TableText, new object[] { consumeItemName, itemName }));
									}
								}
								else {
									SetDescriptionBlock(errorMessage);
								}
							}
						}
						else {
							//Infusion
							string infusionDamageMultiplierPercent = (ConfigValues.InfusionDamageMultiplier - 1f).PercentString();
							SetDescriptionBlock(TableTextID.InfusionDescription.ToString().Lang_WE(L_ID1.TableText, new object[] { infusionDamageMultiplierPercent, ConfigValues.InfusionDamageMultiplier }));
						}
					}
				}

				//Level Up Hover
				if (levelUpData.MouseHovering()) {
					if (MasterUIManager.LeftMouseClicked) {
						LevelUp();
						SoundEngine.PlaySound(SoundID.MenuTick);
					}

					if (DisplayDescriptionBlock) {
						string itemName = wePlayer.enchantingTableItem.IsAir ? TableTextID.Item.ToString().Lang_WE(L_ID1.TableText) : wePlayer.enchantingTableItem.Name;
						int currentLevel = 0;
						int currentXP = 0;
						if (enchantedItem != null) {
							currentLevel = enchantedItem.levelBeforeBooster;
							currentXP = enchantedItem.Experience;
						}

						CalculateXPAvailable(out long xpAvailable, out long _);
						CalculateXPRequired(currentLevel, currentXP, out int _, out int xpNeeded);
						SetDescriptionBlock(TableTextID.LevelUpDescription.ToString().Lang_WE(L_ID1.TableText, new object[] { itemName, currentLevel, Math.Min(currentLevel + wePlayer.levelsPerLevelUp, 40), xpNeeded, xpAvailable }));
					}
				}

				//Level Per Level Up Buttons Hover
				for (int i = 0; i < LevelsPerLevelUp.Length; i++) {
					if (levelsPerData[i].MouseHovering()) {
						if (MasterUIManager.LeftMouseClicked) {
							wePlayer.levelsPerLevelUp = LevelsPerLevelUp[i];
							SoundEngine.PlaySound(SoundID.MenuTick);
						}

						LevelsPerButtonScale[i] += 0.05f;
						if (LevelsPerButtonScale[i] > buttonScaleMaximum)
							LevelsPerButtonScale[i] = buttonScaleMaximum;

						if (DisplayDescriptionBlock) {
							SetDescriptionBlock(TableTextID.LevelUpNumberDescription.ToString().Lang_WE(L_ID1.TableText, new object[] { LevelsPerLevelUp[i] }));
						}
					}
					else {
						LevelsPerButtonScale[i] -= 0.05f;
						if (LevelsPerButtonScale[i] < buttonScaleMinimum)
							LevelsPerButtonScale[i] = buttonScaleMinimum;
					}
				}

				//Loadouts Button Hover
				if (loadoutsData.MouseHovering()) {
					if (MasterUIManager.LeftMouseClicked) {
						if (wePlayer.displayEnchantmentLoadoutUI) {
							EnchantmentLoadoutUI.Close();
						}
						else {
							EnchantmentLoadoutUI.Open();
						}

						SoundEngine.PlaySound(SoundID.MenuTick);
					}

					if (DisplayDescriptionBlock) {
						SetDescriptionBlock(TableTextID.LoadoutDescription.ToString().Lang_WE(L_ID1.TableText));
					}
				}

				//Panel Drag
				if (panel.MouseHovering()) {
					panel.TryStartDraggingUI();
				}

				if (panel.ShouldDragUI())
					MasterUIManager.DragUI(out wePlayer.enchantingTableUILeft, out wePlayer.enchantingTableUITop);
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

			if (WEPlayer.LocalWEPlayer.openLoadoutsWhenOpeningTable)
				EnchantmentLoadoutUI.Open(true);
		}
		public static void CloseEnchantingTableUI(bool noSound = false) {
			WEPlayer wePlayer = WEPlayer.LocalWEPlayer;
			MasterUIManager.TryResetTypingBar(GetUI_ID(WE_UI_ID.EnchantmentStorageSearch));
			wePlayer.openStorageWhenOpeningTable = wePlayer.displayEnchantmentStorage;
			wePlayer.openLoadoutsWhenOpeningTable = wePlayer.displayEnchantmentLoadoutUI;
			wePlayer.displayEnchantmentStorage = false;
			EnchantmentLoadoutUI.Close(true);
			Item itemInUI = wePlayer.enchantingTableItem;
			if (!itemInUI.IsAir) {
				//Give item in table back to player
				wePlayer.enchantingTableItem = wePlayer.Player.GetItem(Main.myPlayer, itemInUI, GetItemSettings.LootAllSettings);

				//Clear item and enchantments from table
				if (wePlayer.enchantingTableItem.IsAir)
					RemoveTableItem();
			}

			wePlayer.itemBeingEnchanted = wePlayer.enchantingTableItem;
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

			return item.IsEnchantable();
		}
		public static bool ValidItemForEnchantingTableEnchantmentSlot(Item item, int slot, bool utility) {
			if (item.IsAir)
				return true;
			
			WEPlayer wePlayer = WEPlayer.LocalWEPlayer;
			wePlayer.enchantingTableItem.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem);
			if (enchantedItem == null)
				return false;

			if (!ValidItemForEnchantmentSlot(item, enchantedItem.ItemType, slot, utility))
				return false;

			if (item.ModItem is not Enchantment newEnchantment || !EnchantmentAllowedOnItem(wePlayer.enchantingTableItem, newEnchantment))
				return false;

			int currentEnchantmentLevelCost = 0;
			if (wePlayer.enchantingTableEnchantments[slot].ModItem is Enchantment enchantment)
				currentEnchantmentLevelCost = enchantment.GetCapacityCost();
			
			int levelsAvailable = enchantedItem.GetLevelsAvailable();
			int newEnchantmentCost = newEnchantment.GetCapacityCost();

			return levelsAvailable >= newEnchantmentCost - currentEnchantmentLevelCost;
		}
		private static bool ValidItemForEnchantmentSlot(Item item, EItemType itemType, int slot, bool utility) {
			bool useEnchantmentSlot = UseEnchantmentSlot(itemType, slot, utility);
			if (!useEnchantmentSlot)
				return false;

			bool isEnchantmentItem = IsValidEnchantmentForSlot(item, utility);
			if (!isEnchantmentItem)
				return false;

			return true;
		}
		public static bool ValidItemForLoadoutEnchantmentSlot(Item item, EItemType itemType, int slot, bool utility, int armorSlotSpecificID = -1) {
			if (!ValidItemForEnchantmentSlot(item, itemType, slot, utility))
				return false;

			if (item.ModItem is not Enchantment newEnchantment || !CheckArmorSlotSpecific(newEnchantment, armorSlotSpecificID))
				return false;

			if (!ConfigValues.RemoveEnchantmentRestrictions) {
				bool allowed = newEnchantment.AllowedList.ContainsKey(itemType);
				if (!allowed && itemType == EItemType.Weapons)
					allowed = newEnchantment.AllowedList.ContainsKey(EItemType.Tools) || newEnchantment.AllowedList.ContainsKey(EItemType.FishingPoles);

				if (!allowed)
					return false;
			}

			return true;
		}
		public static int GetSlotTier(this int slot) => slot < 1 ? slot : slot == MaxEnchantmentSlots - 1 ? 1 : slot - 1;
		public static bool ValidItemForEssenceSlot(Item item, int slot) {
			if (item.IsAir)
				return true;
			
			return item.ModItem is EnchantmentEssence essence && essence.EssenceTier == slot;
		}
		public static bool UseEnchantmentSlot(Item item, int slotTier, bool useHighestTableTier = false) {
			EItemType itemType = item.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem) ? enchantedItem.ItemType : EItemType.None;
			return UseEnchantmentSlot(itemType, slotTier, useHighestTableTier);
		}
		public static bool UseEnchantmentSlot(EItemType itemType, int slotTier, bool useHighestTableTier = false) {
			WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();

			bool utilitySlot = slotTier == MaxEnchantmentSlots - 1;
			if (slotTier > (useHighestTableTier ? wePlayer.highestTableTierUsed : wePlayer.enchantingTableTier) && !utilitySlot)
				return false;

			return SlotAllowedByConfig(itemType, slotTier);
		}
		public static bool SlotAllowedByConfig(EItemType itemType, int slot) {
			int configSlots = ConfigSlotsNum(itemType);

			if (configSlots <= 0)
				return false;

			if (configSlots == 1)
				return slot == 0;

			bool utilitySlot = slot == MaxEnchantmentSlots - 1;

			return utilitySlot || slot <= configSlots - 2;
		}
		public static int ConfigSlotsNum(EItemType itemType) => itemType == EItemType.None ? ConfigValues.EnchantmentSlotsOnItems.Max() : ConfigValues.EnchantmentSlotsOnItems[(int)itemType - 1];
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

			if (item.IsEnchantable()) {
				int damageType = item.type.CSI().DamageType.Type;

				int damageClassSpecific = Enchantment.GetDamageClass(damageType);

				if (newEnchantment.DamageClassSpecific != 0 && damageClassSpecific != newEnchantment.DamageClassSpecific)
					return false;

				if (newEnchantment.IsClassRestricted(damageClassSpecific))
					return false;
			}

			if (!CheckAllowedList(item, newEnchantment))
				return false;

			int slotArmorID = item.headSlot != -1 ? (int)ArmorSlotSpecificID.Head : item.bodySlot != -1 ? (int)ArmorSlotSpecificID.Body : item.legSlot != -1 ? (int)ArmorSlotSpecificID.Legs : -1;
			if (!CheckArmorSlotSpecific(newEnchantment, slotArmorID))
				return false;

			return true;
		}
		public static bool CheckArmorSlotSpecific(Enchantment newEnchantment, int slotArmorID) {
			return newEnchantment.ArmorSlotSpecific == -1 || newEnchantment.ArmorSlotSpecific == slotArmorID;
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

			if (item.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem)) {
				int configSlots = ConfigSlotsNum(enchantedItem.ItemType);
				if (configSlots == 1)
					return 0;

				for (int i = 0; i < configSlots; i++) {
					if (IsSwapEnchantmentSlot(enchantement, enchantedItem.enchantments[i]))
						return i;
				}
			}

			return -1;
		}
		public static int FindSwapEnchantmentSlot(Enchantment enchantement, Item[] equippedEnchantments) {
			for (int i = 0; i < MaxEnchantmentSlots; i++) {
				if (IsSwapEnchantmentSlot(enchantement, equippedEnchantments[i]))
					return i;
			}

			return -1;
		}
		private static bool IsSwapEnchantmentSlot(Enchantment enchantment, Item other) =>
			other.ModItem is Enchantment appliedEnchantment && (enchantment.Unique && appliedEnchantment.Unique || enchantment.Max1 && enchantment.EnchantmentTypeName == appliedEnchantment.EnchantmentTypeName);

		#endregion

		#region UI Methods
		private static void SetGenericDescriptionBlock(string firstLine, string lastLine = null) {
			List<string> lines = new() { firstLine };
			for (int j = 1; j <= 3; j++) {
				lines.Add($"general{j}".Lang_WE(L_ID1.TableText));
			}

			if (lastLine != null)
				lines.Add(lastLine);

			lines.PadStrings();
			descriptionBlock = lines.JoinList("\n");
		}
		private static void SetDescriptionBlock(string text) {
			descriptionBlock = text;
		}
		public static void HandleEnchantmentSlot(UIItemSlotData enchantmentSlot, WEPlayer wePlayer, int slotNum) {
			Item item = wePlayer.enchantingTableEnchantments[slotNum];
			bool display = Main.mouseItem.IsAir && item.IsAir || !UseEnchantmentSlot(item, slotNum);
			bool isUtilitySlot = slotNum == MaxEnchantmentSlots - 1;
			bool normalClickInteractions = true;
			if (Main.mouseItem.IsAir) {
				if (!item.IsAir) {
					if (ItemSlot.ShiftInUse) {
						if (wePlayer.Player.ItemWillBeTrashedFromShiftClick(item)) {
							normalClickInteractions = false;
							StorageManager.TryUpdateMouseOverrideForDeposit(item);
							if (MasterUIManager.LeftMouseClicked) {
								if (!wePlayer.TryReturnEnchantmentFromTableToPlayer(slotNum))
									UIManager.SwapMouseItem(wePlayer.enchantingTableEnchantments, slotNum);
							}
						}
						else {
							if (MasterUIManager.LeftMouseClicked) {
								normalClickInteractions = false;
								wePlayer.TryReturnEnchantmentFromTableToPlayer(slotNum);
							}
						}
					}
				}
			}
			else {
				if (ValidItemForEnchantingTableEnchantmentSlot(Main.mouseItem, slotNum, isUtilitySlot)) {
					if (ItemSlot.ShiftInUse) {
						if (wePlayer.Player.ItemWillBeTrashedFromShiftClick(item) || item.IsAir) {
							normalClickInteractions = false;
							StorageManager.TryUpdateMouseOverrideForDeposit(item);
							if (MasterUIManager.LeftMouseClicked) {
								if (item.IsAir) {
									UIManager.SwapMouseItem(wePlayer.enchantingTableEnchantments, slotNum);
								}
								else {
									wePlayer.TryReturnEnchantmentFromTableToPlayer(slotNum);
								}
							}
						}
						else {
							if (MasterUIManager.LeftMouseClicked) {
								normalClickInteractions = false;
								wePlayer.TryReturnEnchantmentFromTableToPlayer(slotNum);
							}
						}
					}
					else {
						bool canSwap = Main.mouseItem.ModItem is Enchantment enchantment && CheckUniqueSlot(enchantment, FindSwapEnchantmentSlot(enchantment, wePlayer.enchantingTableItem), slotNum) && Main.mouseItem.type != wePlayer.enchantingTableEnchantments[slotNum].type;
						if (canSwap)
							Main.cursorOverride = CursorOverrideID.CameraDark;
						
						if (MasterUIManager.LeftMouseClicked) {
							if (canSwap) {
								if (Main.mouseItem.stack > 1) {
									normalClickInteractions = false;
									if (!item.IsAir)
										wePlayer.TryReturnEnchantmentFromTableToPlayer(slotNum, true);

									Main.mouseItem.stack--;
									Item mouseItemClone = Main.mouseItem.Clone();
									mouseItemClone.stack = 1;
									wePlayer.enchantingTableEnchantments[slotNum] = mouseItemClone;
									SoundEngine.PlaySound(SoundID.Grab);
								}
							}
							else {
								normalClickInteractions = false;
							}
						}
					}
				}
				else {
					normalClickInteractions = false;
				}
			}

			if (display && DisplayDescriptionBlock) {
				if (isUtilitySlot) {
					SetGenericDescriptionBlock(TableTextID.utility0.ToString().Lang_WE(L_ID1.TableText));
				}
				else {
					SetGenericDescriptionBlock(TableTextID.enchantment0.ToString().Lang_WE(L_ID1.TableText), TableTextID.enchantment4.ToString().Lang_WE(L_ID1.TableText, new object[] { EnchantingTableItem.IDs[slotNum].CSI().Name }));
				}
			}

			if (normalClickInteractions) {
				UIManager.ItemSlotClickInteractions(wePlayer.enchantingTableEnchantments, slotNum, MasterUIManager.ItemSlotInteractContext);
			}
		}

		#endregion

		#region Button Methods

		private static void LootAll() {
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
		}
		private static bool LootAllEnchantments(ref Item item, bool quickSpawnIfNeeded = false) {
			WEPlayer wePlayer = WEPlayer.LocalWEPlayer;
			if (!item.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem))
				return false;

			return enchantedItem.enchantments.TryReturnAllEnchantments(wePlayer, quickSpawnIfNeeded);
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
				float infusionPower = Math.Min((float)enchantedWeapon.GetInfusionPower(ref item), 1100f);
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

				GetEssence(tier, numberEssenceRecieved);
			}

			return xpInitial - xpNotConsumed;
		}
		public static bool CanVacuumItem(Item item, Player player) => !item.NullOrAir() && player.GetWEPlayer().highestTableTierUsed >= 0 && WEMod.clientConfig.teleportEssence && CanBeStored(item) && RoomInStorage(item);
		public static bool CanBeStored(Item item) => !item.NullOrAir() && item.ModItem != null && item.ModItem is EnchantmentEssence;
		public static bool RoomInStorage(Item item, Player player = null) {
			if (item.NullOrAir())
				return false;

			if (Main.netMode == NetmodeID.Server)
				return false;

			if (player == null)
				player = Main.LocalPlayer;

			if (player.whoAmI != Main.myPlayer)
				return false;

			EnchantmentEssence essence = (EnchantmentEssence)item.ModItem;
			Item[] essenceSlots = WEPlayer.LocalWEPlayer.enchantingTableEssence;
			if (essenceSlots == null)
				return false;

			int tier = essence.EssenceTier;
			if (essenceSlots[tier] == null)
				return false;

			int tableStack = essenceSlots[tier].stack;
			if (tableStack == 0 || tableStack < essenceSlots[tier].maxStack)
				return true;

			return false;
		}
		public static bool TryVacuumItem(ref Item item, Player player) {
			if (CanVacuumItem(item, player))
				return DepositAll(ref item);

			return false;
		}
		public static bool DepositAll(ref Item item) => DepositAll(new Item[] { item });
		public static bool DepositAll(Item[] inv) {
			bool transferedAnyItem = Restock(inv, false);

			for (int i = 0; i < inv.Length; i++) {
				ref Item item = ref inv[i];
				if (item.NullOrAir() || item.favorited || item.ModItem == null || item.ModItem is not EnchantmentEssence essence)
					continue;

				Item[] essenceSlots = WEPlayer.LocalWEPlayer.enchantingTableEssence;
				int storageIndex = essence.EssenceTier;
				if (essenceSlots[storageIndex] == null)
					essenceSlots[storageIndex] = new();

				if (essenceSlots[storageIndex].stack > 0) {
					if (Restock(ref item))
						transferedAnyItem = true;
				}
				else {
					essenceSlots[storageIndex] = item.Clone();
					item.SetDefaults();
					transferedAnyItem = true;
				}
			}

			if (transferedAnyItem) {
				SoundEngine.PlaySound(SoundID.Grab);
				Recipe.FindRecipes(true);
			}

			return transferedAnyItem;
		}
		public static bool Contains(Item item, Player player) => !item.NullOrAir() && player.TryGetWEPlayer(out WEPlayer wePlayer) && wePlayer.enchantingTableEssence.Select(i => i.type).Contains(item.type);
		public static bool QuickStack(ref Item item, Player player) {
			if (Contains(item, player))
				return TryVacuumItem(ref item, player);

			return false;
		}
		public static bool Restock(ref Item item) => Restock(new Item[] { item });
		public static bool Restock(Item[] inv, bool playSound = true) {
			bool transferedAnyItem = false;
			SortedDictionary<int, List<int>> nonAirItemsInStorage = new();
			for (int i = 0; i < WEPlayer.LocalWEPlayer.enchantingTableEssence.Length; i++) {
				int type = WEPlayer.LocalWEPlayer.enchantingTableEssence[i].type;
				if (type > ItemID.None)
					nonAirItemsInStorage.AddOrCombine(type, i);
			}

			for (int i = 0; i < inv.Length; i++) {
				ref Item item = ref inv[i];
				if (!item.favorited && CanBeStored(item) && nonAirItemsInStorage.TryGetValue(item.type, out List<int> storageIndexes)) {
					foreach (int storageIndex in storageIndexes) {
						ref Item storageItem = ref WEPlayer.LocalWEPlayer.enchantingTableEssence[storageIndex];
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
		public static int GetEssence(int tier, int stack, bool canQuckSpawn = true, WEPlayer wePlayer = null) {
			if (Main.netMode == NetmodeID.Server)
				return stack;

			if (stack < 1 || tier < 0 || tier >= MaxEssenceSlots)
				return stack;

			if (wePlayer == null)
				wePlayer = WEPlayer.LocalWEPlayer;

			int maxEssenceStack = EnchantmentEssence.MAX_STACK;
			int remainingStack;
			if (wePlayer.enchantingTableEssence[tier].NullOrAir()) {
				int numberTransfered = stack;
				if (stack > maxEssenceStack)
					numberTransfered = maxEssenceStack;

				wePlayer.enchantingTableEssence[tier] = new(EnchantmentEssence.IDs[tier], numberTransfered);

				remainingStack = stack - numberTransfered;
			}
			else {
				Item essence = wePlayer.enchantingTableEssence[tier];
				int currentStack = essence.stack;
				int totalStack = ModMath.AddCheckOverflow(stack, currentStack, out long remainder);
				if (totalStack > maxEssenceStack)
					totalStack = maxEssenceStack;

				essence.stack = totalStack;
				int numberTransfered = totalStack - currentStack;

				remainingStack = stack - numberTransfered + (int)remainder;
			}

			if (!canQuckSpawn)
				return remainingStack;

			int maxStack = EnchantmentEssence.IDs[tier].CSI().maxStack;
			while (remainingStack > 0) {
				int stackToQuickSpawn = remainingStack > maxStack ? maxStack : remainingStack;
				remainingStack -= stackToQuickSpawn;
				Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), EnchantmentEssence.IDs[tier], stackToQuickSpawn);
			}

			return remainingStack;
		}
		public static void Siphon(ref Item item, bool force = false) {
			if (item.IsAir)
				return;

			if (!item.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem))
				return;

			int smallestXpPerEssence = (int)EnchantmentEssence.xpPerEssence[0];
			int xpCost = GetSiphonXPCost(item, enchantedItem, out bool useOldSystem);
			if (useOldSystem && !force) {
				//Original System
				int maxLevelXP = WEModSystem.levelXps[EnchantedItem.MAX_Level - 1];
				int  minimumXPToSiphon = maxLevelXP + smallestXpPerEssence;
				if (enchantedItem.Experience < minimumXPToSiphon) {
					Main.NewText(GameMessageTextID.MinSiphonXP.ToString().Lang_WE(L_ID1.GameMessages, new object[] { minimumXPToSiphon }));
				}
				else {
					int xp = enchantedItem.Experience - maxLevelXP;
					enchantedItem.Experience -= ConvertXPToEssence(xp, item: item);
				}
			}
			else {
				int minimumXPToSiphon = xpCost + smallestXpPerEssence;
				if (!force && enchantedItem.Experience < minimumXPToSiphon) {
					Main.NewText(GameMessageTextID.MinSiphonXP.ToString().Lang_WE(L_ID1.GameMessages, new object[] { minimumXPToSiphon }));
				}
				else {
					if (force) {
						if (enchantedItem.Experience - xpCost < 0) {
							enchantedItem.Experience = 0;
						}
						else {
							enchantedItem.Experience -= xpCost;
						}
					}
					else {
						enchantedItem.Experience -= xpCost;
					}
					
					ReturnAllModifications(ref item, true);
				}
			}
		}
		private static int GetSiphonXPCost(Item item, EnchantedItem enchantedItem, out bool useOldSystem) {
			useOldSystem = ConfigValues.SiphonExperienceCost > 0.991f;
			if (useOldSystem)
				return 0;

			int xpCost = (int)((float)item.value * 4f / EnchantmentEssence.valuePerXP);
			int configCost = (int)(ConfigValues.SiphonExperienceCost * enchantedItem.Experience);
			if (configCost < xpCost)
				xpCost = configCost;

			return xpCost;
		}
		private static bool CanDoInfusion(out string errorMessage, bool allowLogErrors = false) {
			errorMessage = "";
			WEPlayer wePlayer = WEPlayer.LocalWEPlayer;
			Item tableItem = wePlayer.enchantingTableItem;
			if (tableItem.IsEnchantable()) {
				if (!tableItem.InfusionAllowed())
					return false;

				if (allowLogErrors && wePlayer.infusionConsumeItem == null) {
					GameMessageTextID.InfusionConsumeItemWasNull.ToString().Lang_WE(L_ID1.GameMessages, new object[] { tableItem.S(), (tableItem?.ModItem is UnloadedItem unloadedTableItem ? $", {unloadedTableItem.ItemName}" : ""), wePlayer.infusionConsumeItem.S(), (wePlayer.infusionConsumeItem?.ModItem is UnloadedItem unloadedConsumeItem ? $", {unloadedConsumeItem.ItemName}" : "") }).LogNT(ChatMessagesIDs.AlwaysShowInfusionError);
				}

				if (wePlayer.infusionConsumeItem.IsAir) {
					bool canConsume = false;

					//Prevent specific items from being consumed for infusion.
					switch (tableItem.ModFullName()) {
						case "CalamityMod/Murasama":
							errorMessage = GameMessageTextID.MurasamaNoInfusion.ToString().Lang_WE(L_ID1.GameMessages);
							break;
						default:
							canConsume = true;
							break;
					}

					if (!canConsume)
						return false;

					//Store item for infusion
					if (tableItem.stack > 1) {
						return true;
					}
					else {
						if (wePlayer.enchantingTableItem.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem) && enchantedItem.favorited) {
							errorMessage = GameMessageTextID.FavoritedItemsCantBeConsumedForInfusion.ToString().Lang_WE(L_ID1.GameMessages);
							return false;
						}

						return true;
					}
				}
				else {
					bool canInfuse = true;

					//Prevent specific items from being upgraded with infusion.
					if (tableItem.ModFullName().Contains("PrimaryZenith")) {
						canInfuse = false;
						errorMessage = GameMessageTextID.ResistsYourAttemptToEmpower.ToString().Lang_WE(L_ID1.GameMessages, new object[] { tableItem.Name });
					}

					if (!canInfuse)
						return false;

					//Check armor slots match if armor
					if (tableItem.IsArmorItem() && wePlayer.infusionConsumeItem.IsArmorItem()) {
						int tableItemSlotIndex = tableItem.GetSlotIndex();
						int consumeItemSlotIndex = wePlayer.infusionConsumeItem.GetSlotIndex();
						if (tableItemSlotIndex != consumeItemSlotIndex) {
							errorMessage = GameMessageTextID.CantInfusionArmorDifferentTypes.ToString().Lang_WE(L_ID1.GameMessages);
							return false;
						}
					}

					if (!(tableItem.IsWeaponItem() && wePlayer.infusionConsumeItem.IsWeaponItem() || tableItem.IsArmorItem() && wePlayer.infusionConsumeItem.IsArmorItem())) {
						errorMessage = GameMessageTextID.InfusionOnlyPossibleSameType.ToString().Lang_WE(L_ID1.GameMessages);
						return false;
					}

					//Infuse (Finalize)
					return true;
				}
			}
			else if (!wePlayer.infusionConsumeItem.IsAir) {
				return true;
			}
			else if (!tableItem.IsAir) {
				if (allowLogErrors)
					GameMessageTextID.NotEnchantableAndNotAirInfusionItem.ToString().Lang_WE(L_ID1.GameMessages, new object[] { tableItem.S(), (tableItem?.ModItem is UnloadedItem unloadedTableItem ? $", {unloadedTableItem.ItemName}" : ""), wePlayer.infusionConsumeItem.S(), (wePlayer.infusionConsumeItem?.ModItem is UnloadedItem unloadedConsumeItem ? $", {unloadedConsumeItem.ItemName}" : "") }).LogNT(ChatMessagesIDs.AlwaysShowInfusionError);

				return false;
			}

			return false;
		}
		private static void Infusion() {
			if (!CanDoInfusion(out string errorMessage, true)) {
				Main.NewText(errorMessage);
				return;
			}

			WEPlayer wePlayer = WEPlayer.LocalWEPlayer;
			Item tableItem = wePlayer.enchantingTableItem;
			if (tableItem.IsEnchantable()) {
				if (wePlayer.infusionConsumeItem.IsAir) {
					//Store item for infusion
					if (tableItem.stack > 1) {
						wePlayer.enchantingTableItem.stack -= 1;
						wePlayer.infusionConsumeItem = new Item(tableItem.type);
					}
					else {
						wePlayer.infusionConsumeItem = tableItem.Clone();
						wePlayer.enchantingTableItem = new Item();
					}
				}
				else {
					//Infuse (Finalize)
					if (wePlayer.enchantingTableItem.TryInfuseItem(wePlayer.infusionConsumeItem, false, true)) {
						OfferItem(ref wePlayer.infusionConsumeItem, true, true, false);
						wePlayer.infusionConsumeItem = new();
					}
					else {
						GameMessageTextID.TryInfuseFailed.ToString().Lang_WE(L_ID1.GameMessages, new object[] { tableItem.S(), (tableItem?.ModItem is UnloadedItem unloadedTableItem ? $", {unloadedTableItem.ItemName}" : ""), wePlayer.infusionConsumeItem.S(), (wePlayer.infusionConsumeItem?.ModItem is UnloadedItem unloadedConsumeItem ? $", {unloadedConsumeItem.ItemName}" : "") }).LogNT(ChatMessagesIDs.AlwaysShowInfusionError);
					}
				}
			}
			else if (!wePlayer.infusionConsumeItem.IsAir) {
				//Return infusion item to table
				wePlayer.enchantingTableItem = wePlayer.infusionConsumeItem.Clone();
				wePlayer.infusionConsumeItem = new();
			}
		}
		private static void CalculateXPAvailable(out long xpAvailable, out long nonFavoriteXpAvailable) {
			WEPlayer wePlayer = WEPlayer.LocalWEPlayer;

			xpAvailable = 0;
			nonFavoriteXpAvailable = 0;

			//xpAvailable
			for (int i = EnchantingTableUI.MaxEnchantmentSlots - 1; i >= 0; i--) {
				long xpToAdd = ModMath.MultiplyCheckOverflow((long)EnchantmentEssence.xpPerEssence[i], (long)wePlayer.enchantingTableEssence[i].stack);
				xpAvailable.AddCheckOverflow(xpToAdd);
				if (!wePlayer.enchantingTableEssence[i].favorited)
					nonFavoriteXpAvailable.AddCheckOverflow(xpToAdd);
			}
		}
		private static void CalculateXPRequired(EnchantedItem enchantedItem, out int targetLevelXPIndex, out int xpNeeded) {
			CalculateXPRequired(enchantedItem.levelBeforeBooster, enchantedItem.Experience, out targetLevelXPIndex, out xpNeeded);
		}
		private static void CalculateXPRequired(int currentLevel, int currentExperience, out int targetLevelXPIndex, out int xpNeeded) {
			WEPlayer wePlayer = WEPlayer.LocalWEPlayer;

			//xpNeeded
			targetLevelXPIndex = currentLevel + wePlayer.levelsPerLevelUp - 1;
			targetLevelXPIndex.Clamp(max: EnchantedItem.MAX_Level - 1);
			xpNeeded = Math.Max(WEModSystem.levelXps[targetLevelXPIndex] - currentExperience, 0);
		}
		private static void LevelUp() {
			WEPlayer wePlayer = WEPlayer.LocalWEPlayer;
			Item item = wePlayer.enchantingTableItem;
			if (!item.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem))
				return;

			if (enchantedItem.levelBeforeBooster == EnchantedItem.MAX_Level) {
				Main.NewText(GameMessageTextID.AlreadyMaxLevel.ToString().Lang_WE(L_ID1.GameMessages, new object[] { item.Name }));
				return;
			}

			CalculateXPAvailable(out long xpAvailable, out long nonFavoriteXpAvailable);
			CalculateXPRequired(enchantedItem, out int targetLevelXPIndex, out int xpNeeded);
			bool enoughWithoutFavorite = nonFavoriteXpAvailable >= xpNeeded;

			if (xpAvailable < xpNeeded) {
				Main.NewText(GameMessageTextID.NotEnoughEssence.ToString().Lang_WE(L_ID1.GameMessages, new object[] { xpNeeded, targetLevelXPIndex + 1, xpAvailable }));
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
						//Check essence available below me
						int xpAvailableBelowThis = 0;
						for (int j = i - 1; j >= 0; j--) {
							if (!wePlayer.enchantingTableEssence[j].favorited || !enoughWithoutFavorite) {
								int xpPerEssenceLowerTier = (int)EnchantmentEssence.xpPerEssence[j];
								xpAvailableBelowThis.AddCheckOverflow(xpPerEssenceLowerTier * wePlayer.enchantingTableEssence[j].stack);
							}
						}

						if (xpAvailableBelowThis < xpNeeded - xpPerEssence * numberEssenceTransfered)
							numberEssenceTransfered++;
					}
				}

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
				warning = GameMessageTextID.NonEnchantableItemInTable.ToString().Lang_WE(L_ID1.GameMessages, new object[] { wePlayer.enchantingTableItem.S() });// "Non-Enchantable item detected in table: {}.\n" + $"WARNING, DO NOT PRESS CONFIRM.\n" + $"Please report this issue to andro951(Weapon Enchantments)";
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
					oreAndEssencePercent = TableTextID.ExchangeEssence.ToString().Lang_WE(L_ID1.TableText);
				}
				else if (percentEss == 0f) {
					//oreAndEssencePercent = $"In exchange for ores?";
					oreAndEssencePercent = TableTextID.ExchangeOres.ToString().Lang_WE(L_ID1.TableText);
				}
				else {
					//oreAndEssencePercent = $"In exchange for ores({(1f - percentEss).PercentString()}) and essence({percentEss.PercentString()})?";
					oreAndEssencePercent = TableTextID.ExchangeEssenceAndOres.ToString().Lang_WE(L_ID1.TableText, new object[] { (1f - percentEss).PercentString(), percentEss.PercentString() });
				}

				object[] args = new object[] { enchantedItem.level.ToString(), wePlayer.enchantingTableItem.Name, oreAndEssencePercent, percentEss < 1f ? $"{oreString}\n" : "" };
				warning = TableTextID.AreYouSure.ToString().Lang_WE(L_ID1.TableText, args);
			}

			return warning;
		}
		private static void Offer() {
			int type = OfferItem(ref WEPlayer.LocalWEPlayer.enchantingTableItem, nonTableItem: false);
			if (type == 0)
				return;

			WEPlayer wePlayer = WEPlayer.LocalWEPlayer;
			if (wePlayer.autoTrashOfferedItems)
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

							if (wePlayer.autoTrashOfferedItems)
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
		public static void ReturnAllModifications(ref Item item, bool shouldGiveBackInfusiedItem = true) {
			if (!item.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem))
				return;

			LootAllEnchantments(ref item, true);

			//Power Booster
			if (enchantedItem.PowerBoosterInstalled) {
				StorageManager.GiveNewItemToPlayer(ModContent.ItemType<PowerBooster>(), Main.LocalPlayer);
				enchantedItem.PowerBoosterInstalled = false;
			}

			//Ultra Power Booster
			if (enchantedItem.UltraPowerBoosterInstalled) {
				StorageManager.GiveNewItemToPlayer(ModContent.ItemType<UltraPowerBooster>(), Main.LocalPlayer);
				enchantedItem.UltraPowerBoosterInstalled = false;
			}

			//Xp -> Essence
			int xp = enchantedItem.Experience;
			enchantedItem.Experience -= ConvertXPToEssence(xp, true, item);

			//Infused Item
			if (shouldGiveBackInfusiedItem && enchantedItem.infusedItemName != "") {
				if (InfusionManager.TryFindItem(enchantedItem.infusedItemName, out Item foundItem)) {
					StorageManager.TryReturnItemToPlayer(ref foundItem, Main.LocalPlayer, true);
				}
				else {
					Main.LocalPlayer.SpawnCoins(enchantedItem.InfusionValueAdded / 5);
				}

				enchantedItem.ResetInfusion(item);
			}
		}
		public static int OfferItem(ref Item item, bool noOre = false, bool nonTableItem = true, bool returnInfusedItem = true) {
			int type = item.type;
			if (!item.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem))
				return -1;

			//Enchantments
			if (!nonTableItem && !LootAllEnchantments(ref item))
				return -1;

			float value = item.value;

			ReturnAllModifications(ref item, returnInfusedItem);

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
					ConvertXPToEssence(essenceValue, true);
			}

			item.TurnToAir();
			SoundEngine.PlaySound(SoundID.Grab);

			return type;
		}

		#endregion
	}
}
