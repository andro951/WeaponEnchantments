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

namespace WeaponEnchantments.UI
{
	public static class EnchantmentStorage
	{
		public class EnchantmentStorageButtonID
		{
			public const int LootAll = 0;
			public const int DepositAll = 1;
			public const int QuickStack = 2;
			public const int Restock = 3;
			public const int Sort = 4;
			public const int RenameChest = 5;
			public const int RenameChestCancel = 6;
			public const int ToggleVacuum = 7;
			public static readonly int Count = 7;
		}
		public static int ID => UI_ID.EnchantmentStorage;
		public static int enchantmentStorageUIDefaultX => Main.screenWidth / 2;
		public static int enchantmentStorageUIDefaultY => 300;
		private static int spacing => 4;
		private static int panelBorder => 10;
		public const float buttonScaleMinimum = 0.75f;
		public const float buttonScaleMaximum = 1f;
		public static float[] ButtonScale = new float[EnchantmentStorageButtonID.Count];
		public static void PostDrawInterface(SpriteBatch spriteBatch) {
			WEPlayer wePlayer = WEPlayer.LocalWEPlayer;
			if (wePlayer.displayEnchantmentStorage || true) {
				//ItemSlots Measurements
				int itemSlotColumns = 10;
				int itemSlotRows = 4;//wePlayer.enchantmentStorageItems.Length / columns
				int itemSlotSpaceWidth = UIManager.ItemSlotSize + spacing;
				int itemSlotSpaceHeight = UIManager.ItemSlotSize + spacing;
				int itemSlotsWidth = (itemSlotColumns - 1) * itemSlotSpaceWidth + UIManager.ItemSlotSize;
				int itemSlotsHeight = (itemSlotRows - 1) * itemSlotSpaceHeight + UIManager.ItemSlotSize;
				int itemSlotsLeft = wePlayer.enchantmentStorageUILeft + panelBorder;
				int itemSlotsTop = wePlayer.enchantmentStorageUITop + panelBorder;

				//Name Data
				string name = EnchantmentStorageTextID.EnchantmentStorage.ToString().Lang(L_ID1.EnchantmentStorageText);
				name = "Enchantment Storage Name Test";
				int textsLeft = itemSlotsLeft + itemSlotsWidth + spacing;
				Vector2 vector = FontAssets.MouseText.Value.MeasureString(name);
				int nameX = (int)vector.X;
				int nameY = (int)vector.Y;

				//Text buttons Data


				//Panel Data
				int panelBorderRight = spacing + nameX + panelBorder;
				int panelWidth = itemSlotsWidth + panelBorder + panelBorderRight;
				int panelHeight = itemSlotsHeight + panelBorder * 2;
				UIPanelData panel = new(ID, wePlayer.enchantmentStorageUILeft, wePlayer.enchantmentStorageUITop, panelWidth, panelHeight);

				//Panel Draw
				UIManager.DrawUIPanel(spriteBatch, panel, new Color(26, 2, 56, 100));

				//ItemSlots Draw
				int itemSlotY = itemSlotsTop;
				int itemSlotIndex = 0;
				int startRow = 0;
				int endRow = startRow + 4;
				for (int row = startRow; row < endRow; row++) {
					int itemSlotX = itemSlotsLeft;
					for (int column = 0; column < itemSlotColumns; column++) {
						ref Item item = ref wePlayer.enchantmentStorageItems[itemSlotIndex];
						if (UIManager.MouseHoveringItemSlot(itemSlotX, itemSlotY, UI_ID.EnchantmentStorageItemSlot1 + itemSlotIndex)) {
							if (Main.mouseItem.NullOrAir() || Main.mouseItem?.ModItem is WEModItem weModItem && weModItem is not EnchantmentEssence)
								UIManager.ItemSlotClickInteractions(ref item);
						}

						UIManager.DrawItemSlot(spriteBatch, ref item, itemSlotX, itemSlotY);
						itemSlotX += itemSlotSpaceWidth;
						itemSlotIndex++;
					}

					itemSlotY += itemSlotSpaceHeight;
				}

				//Name
				Color color = Color.White * (1f - (255f - (float)(int)Main.mouseTextColor) / 255f * 0.5f);
				color.A = byte.MaxValue;
				ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, name, new Vector2(textsLeft, itemSlotsTop), color, 0f, Vector2.Zero, Vector2.One, -1f, 1.5f);


				//Buttons


				//Panel Hover and Drag
				if (UIManager.MouseHovering(panel)) {
					UIManager.TryStartDraggingUI(panel);
				}

				if (UIManager.ShouldDragUI(panel))
					UIManager.DragUI(out wePlayer.enchantmentStorageUILeft, out wePlayer.enchantmentStorageUITop);
			}
		}
	}
}
