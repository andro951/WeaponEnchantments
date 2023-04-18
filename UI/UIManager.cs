using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace WeaponEnchantments.UI
{
	public static class UIManager
	{
		public static bool HoveringOverUI => WeaponEnchantmentUI.preventItemUse || EnchantmentStorage.hoveringEnchantmentStorageUI;
		public static int mouseOffsetX = 0;
		public static int mouseOffsetY = 0;
		public static bool lastMouseLeft = false;
		public static readonly Asset<Texture2D>[] uiTextures = { Main.Assets.Request<Texture2D>("Images/UI/PanelBackground"), Main.Assets.Request<Texture2D>("Images/UI/PanelBorder") };
		public static void PostDrawInterface(SpriteBatch spriteBatch, WEPlayer wePlayer) {
			float savedInventoryScale = Main.inventoryScale;
			Main.inventoryScale = 0.86f;

			EnchantmentStorage.PoseDrawInterface(spriteBatch, wePlayer);

			Main.inventoryScale = savedInventoryScale;
			lastMouseLeft = Main.mouseLeft;
		}
		public static void DrawUIPanel(SpriteBatch spriteBatch, int cornerSize, Point panelTopLeft, Point panelBottomRight, Color panelColor) {
			int _barSize = 4;
			int width = panelBottomRight.X - panelTopLeft.X - cornerSize;
			int height = panelBottomRight.Y - panelTopLeft.Y - cornerSize;
			Color[] colors = { panelColor, Color.Black };
			for (int i = 0; i < uiTextures.Length; i++) {
				Texture2D texture = uiTextures[i].Value;
				Color color = colors[i];
				spriteBatch.Draw(texture, new Rectangle(panelTopLeft.X, panelTopLeft.Y, cornerSize, cornerSize), new Rectangle(0, 0, cornerSize, cornerSize), color);
				spriteBatch.Draw(texture, new Rectangle(panelBottomRight.X, panelTopLeft.Y, cornerSize, cornerSize), new Rectangle(cornerSize + _barSize, 0, cornerSize, cornerSize), color);
				spriteBatch.Draw(texture, new Rectangle(panelTopLeft.X, panelBottomRight.Y, cornerSize, cornerSize), new Rectangle(0, cornerSize + _barSize, cornerSize, cornerSize), color);
				spriteBatch.Draw(texture, new Rectangle(panelBottomRight.X, panelBottomRight.Y, cornerSize, cornerSize), new Rectangle(cornerSize + _barSize, cornerSize + _barSize, cornerSize, cornerSize), color);
				spriteBatch.Draw(texture, new Rectangle(panelTopLeft.X + cornerSize, panelTopLeft.Y, width, cornerSize), new Rectangle(cornerSize, 0, _barSize, cornerSize), color);
				spriteBatch.Draw(texture, new Rectangle(panelTopLeft.X + cornerSize, panelBottomRight.Y, width, cornerSize), new Rectangle(cornerSize, cornerSize + _barSize, _barSize, cornerSize), color);
				spriteBatch.Draw(texture, new Rectangle(panelTopLeft.X, panelTopLeft.Y + cornerSize, cornerSize, height), new Rectangle(0, cornerSize, cornerSize, _barSize), color);
				spriteBatch.Draw(texture, new Rectangle(panelBottomRight.X, panelTopLeft.Y + cornerSize, cornerSize, height), new Rectangle(cornerSize + _barSize, cornerSize, cornerSize, _barSize), color);
				spriteBatch.Draw(texture, new Rectangle(panelTopLeft.X + cornerSize, panelTopLeft.Y + cornerSize, width, height), new Rectangle(cornerSize, cornerSize, _barSize, _barSize), color);
			}
		}
	}
}
