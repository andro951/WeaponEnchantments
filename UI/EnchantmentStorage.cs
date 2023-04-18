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

namespace WeaponEnchantments.UI
{
	public class EnchantmentStorage
	{
		private static bool movingEnchantmentStorageUI = false;
		public static bool hoveringEnchantmentStorageUI = false;
		public static int enchantmentStorageUIDefaultX => Main.screenWidth / 2;
		public static int enchantmentStorageUIDefaultY => 300;
		public static void PoseDrawInterface(SpriteBatch spriteBatch, WEPlayer wePlayer) {
			if (wePlayer.displayEnchantmentStorage) {
				int columns = 10;
				int rows = 4;//wePlayer.enchantmentStorageItems.Length / columns
				int itemSlotSpaceing = (4f * Main.inventoryScale).RoundNearest(2);
				int itemSlotTextureHeight = (TextureAssets.InventoryBack.Height() * Main.inventoryScale).RoundNearest(2);
				int itemSlotTextureWidth = (TextureAssets.InventoryBack.Width() * Main.inventoryScale).RoundNearest(2);
				int itemSlotSpaceHeight = itemSlotTextureHeight + itemSlotSpaceing;
				int itemSlotSpaceWidth = itemSlotTextureWidth + itemSlotSpaceing;
				int itemSlotsHeight = (rows - 1) * itemSlotSpaceHeight + itemSlotTextureHeight;
				int itemSlotsWidth = (columns - 1) * itemSlotSpaceWidth + itemSlotTextureWidth;
				int itemSlotsTopY = wePlayer.enchantmentStorageUILocationY - itemSlotsHeight / 2;
				int itemSlotsLeftX = wePlayer.enchantmentStorageUILocationX - itemSlotsWidth / 2;
				int itemSlotY = itemSlotsTopY;
				bool notHoveringOverAnItemSlot = true;
				int itemSlotIndex = 0;
				int startRow = 0;
				int endRow = startRow + 4;

				//UIPannel
				int panelBorderTop = 10;
				int panelBorderBottom = 10;
				int panelBorderLeft = 10;
				int panelBorderRight = 100;
				int panelHeight = itemSlotsHeight + panelBorderTop + panelBorderBottom;
				int panelWidth = itemSlotsWidth + panelBorderLeft + panelBorderRight;
				int cornerSize = 12;
				Point panelTopLeft = new Point(wePlayer.enchantmentStorageUILocationX - panelBorderLeft - itemSlotsWidth / 2, wePlayer.enchantmentStorageUILocationY - panelBorderTop - itemSlotsHeight / 2);
				Point panelBottomRight = new Point(panelTopLeft.X + panelWidth - cornerSize, panelTopLeft.Y + panelHeight - cornerSize);

				UIManager.DrawUIPanel(spriteBatch, cornerSize, panelTopLeft, panelBottomRight, new Color(26, 2, 56));

				//ItemSlots
				for (int row = startRow; row < endRow; row++) {
					int itemSlotX = itemSlotsLeftX;
					for (int column = 0; column < columns; column++) {
						ref Item item = ref wePlayer.enchantmentStorageItems[itemSlotIndex];
						if (notHoveringOverAnItemSlot) {
							if (Main.mouseX >= itemSlotX && Main.mouseX <= itemSlotX + itemSlotTextureWidth && Main.mouseY >= itemSlotY && Main.mouseY <= itemSlotY + itemSlotTextureHeight && !PlayerInput.IgnoreMouseInterface) {
								Main.LocalPlayer.mouseInterface = true;
								Main.craftingHide = true;
								if (Main.mouseItem.NullOrAir() || Main.mouseItem?.ModItem is WEModItem weModItem && weModItem is not EnchantmentEssence) {
									ItemSlot.LeftClick(ref item, 30);
									if (Main.mouseLeftRelease && Main.mouseLeft)
										Recipe.FindRecipes();

									ItemSlot.RightClick(ref item, 30);
									ItemSlot.MouseHover(ref item, 30);
								}

								notHoveringOverAnItemSlot = false;
							}
						}

						ItemSlot.Draw(spriteBatch, ref item, 5, new Vector2(itemSlotX, itemSlotY));
						itemSlotX += itemSlotSpaceWidth;
						itemSlotIndex++;
					}

					itemSlotY += itemSlotSpaceHeight;
				}

				//Right click storage button to recenter it.

				if (Main.mouseX >= panelTopLeft.X && Main.mouseX <= panelBottomRight.X && Main.mouseY >= panelTopLeft.Y && Main.mouseY <= panelBottomRight.Y && !PlayerInput.IgnoreMouseInterface) {
					hoveringEnchantmentStorageUI = true;
					if (notHoveringOverAnItemSlot && !movingEnchantmentStorageUI && Main.mouseLeft && !UIManager.lastMouseLeft) {
						movingEnchantmentStorageUI = true;
						UIManager.mouseOffsetX = wePlayer.enchantmentStorageUILocationX - Main.mouseX;
						UIManager.mouseOffsetY = wePlayer.enchantmentStorageUILocationY - Main.mouseY;
					}
				}
				else {
					hoveringEnchantmentStorageUI = false;
				}

				if (movingEnchantmentStorageUI) {
					if (!Main.mouseLeft) {
						movingEnchantmentStorageUI = false;
					}
					else {
						wePlayer.enchantmentStorageUILocationY = Main.mouseY + UIManager.mouseOffsetY;
						wePlayer.enchantmentStorageUILocationX = Main.mouseX + UIManager.mouseOffsetX;
					}
				}

			}
		}
	}
}
