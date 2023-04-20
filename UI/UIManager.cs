using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace WeaponEnchantments.UI
{
	public static class UIManager
	{
		public static bool NoPanelBeingDragged => PanelBeingDragged == UI_ID.None;
		private static int mouseOffsetX = 0;
		private static int mouseOffsetY = 0;
		public static bool lastMouseLeft = false;
		public static bool LeftMouseClicked => Main.mouseLeft && !lastMouseLeft;
		public static readonly Asset<Texture2D>[] uiTextures = { Main.Assets.Request<Texture2D>("Images/UI/PanelBackground"), Main.Assets.Request<Texture2D>("Images/UI/PanelBorder") };
		public static readonly int ItemSlotSize = 44;
		public static readonly int ItemSlotDrawContext = 5;
		public static readonly int ItemSlotInteractContext = 30;
		private static int PanelBeingDragged = UI_ID.None;
		private static int UIBeingHovered = UI_ID.None;
		public static void PostDrawInterface(SpriteBatch spriteBatch) {
			UIBeingHovered = UI_ID.None;
			float savedInventoryScale = Main.inventoryScale;
			Main.inventoryScale = 0.86f;

			EnchantmentStorage.PostDrawInterface(spriteBatch);
			WitchRerollUI.PoseDrawInterface(spriteBatch);

			Main.inventoryScale = savedInventoryScale;
			lastMouseLeft = Main.mouseLeft;
		}
		/// <summary>
		/// Only returns true for the first panel reached to prevent clicking multiple panels.
		/// </summary>
		/// <param name="panel"></param>
		/// <returns></returns>
		public static bool MouseHovering(UIPanelData panel) => MouseHovering(panel.IsMouseHovering, panel.ID);
		public static bool MouseHovering(bool hovering, int ID) {
			if (UIBeingHovered == UI_ID.None && hovering) {
				UIBeingHovered = ID;
				Main.LocalPlayer.mouseInterface = true;
				return true;
			}

			return false;
		}
		public static void TryStartDraggingUI(UIPanelData panel) {
			if (PanelBeingDragged == UI_ID.None) {
				if (LeftMouseClicked)
					StartDraggingUI(panel);
			}
		}
		public static void StartDraggingUI(UIPanel panel, int UI_ID) {
			StartDraggingUI((int)panel.Left.Pixels, (int)panel.Top.Pixels, UI_ID);
		}
		public static void StartDraggingUI(int panelX, int panelY, int UI_ID) {
			PanelBeingDragged = UI_ID;
			mouseOffsetX = panelX - Main.mouseX;
			mouseOffsetY = panelY - Main.mouseY;
		}
		public static void StartDraggingUI(UIPanelData panel) {
			//Main.NewText($"{DateTime.Now}");
			PanelBeingDragged = panel.ID;
			Point topLeft = panel.TopLeft;
			mouseOffsetX = topLeft.X - Main.mouseX;
			mouseOffsetY = topLeft.Y - Main.mouseY;
		}
		public static bool ShouldDragUI(UIPanelData panel) => ShouldDragUI(panel.ID);
		public static bool ShouldDragUI(int ID) {
			if (PanelBeingDragged == ID) {
				if (!Main.mouseLeft) {
					PanelBeingDragged = UI_ID.None;
				}
				else {
					return true;
				}
			}

			return false;
		}
		public static void DragUI(out int panelX, out int panelY) {
			panelX = Main.mouseX + mouseOffsetX;
			panelY = Main.mouseY + mouseOffsetY;
		}
		public static void DragUI(UIPanel panel) {
			DragUI(out int panelX, out int panelY);
			panel.Left.Pixels = panelX;
			panel.Top.Pixels = panelY;
		}
		public static void CheckOutOfBoundsRestoreDefaultPosition(ref int uiX, ref int uiY, int defaultX, int defaultY) {
			if (uiX <= 10 || uiX >= Main.screenWidth - 10 || uiY <= 10 || uiY >= Main.screenHeight - 10) {
				uiX = defaultX;
				uiY = defaultY;
			}
		}
		public static void DrawUIPanel(SpriteBatch spriteBatch, UIPanelData panel, Color panelColor) {
			Point panelTopLeft = panel.TopLeft;
			Point panelBottomRight = panel.BottomRight;
			int _barSize = 4;
			int cornerSize = 12;
			panelBottomRight = new(panelBottomRight.X - cornerSize, panelBottomRight.Y - cornerSize);
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
		public static void DrawItemSlot(SpriteBatch spriteBatch, ref Item item, int itemSlotX, int itemSlotY) {
			ItemSlot.Draw(spriteBatch, ref item, ItemSlotDrawContext, new Vector2(itemSlotX, itemSlotY));
		}
		public static bool MouseHoveringItemSlot(int itemSlotX, int itemSlotY, int ID) {
			if (UIBeingHovered == UI_ID.None && Main.mouseX >= itemSlotX && Main.mouseX <= itemSlotX + ItemSlotSize && Main.mouseY >= itemSlotY && Main.mouseY <= itemSlotY + ItemSlotSize && !PlayerInput.IgnoreMouseInterface) {
				Main.LocalPlayer.mouseInterface = true;
				UIBeingHovered = ID;
				return true;
			}

			return false;
		}
		public static void ItemSlotClickInteractions(ref Item item) {
			ItemSlot.LeftClick(ref item, ItemSlotInteractContext);
			if (Main.mouseLeftRelease && Main.mouseLeft)
				Recipe.FindRecipes();

			ItemSlot.RightClick(ref item, ItemSlotInteractContext);
			ItemSlot.MouseHover(ref item, ItemSlotInteractContext);
		}
	}
	public struct UIPanelData {
		public Point TopLeft;
		public Point BottomRight;
		public int ID;
		public UIPanelData(int id, int left, int top, int width, int height) {
			ID = id;
			TopLeft = new Point(left, top);
			BottomRight = new Point(left + width, top + height);
		}
		public bool IsMouseHovering => Main.mouseX >= TopLeft.X && Main.mouseX <= BottomRight.X && Main.mouseY >= TopLeft.Y && Main.mouseY <= BottomRight.Y && !PlayerInput.IgnoreMouseInterface;
		public Point Center => new((TopLeft.X + BottomRight.X) / 2, (TopLeft.Y + BottomRight.Y) / 2);
	}
	public struct UITextData {
		public Point TopLeft;
		public Point BottomRight;
		public int ID;
		public UITextData(int id, int left, int top, string text, int textSize) {
			ID = id;
			TopLeft = new Point(left, top);
			//BottomRight = new Point(left + width, top + height);//Look at chest UI code for text buttons.
		}
		public bool IsMouseHovering => Main.mouseX >= TopLeft.X && Main.mouseX <= BottomRight.X && Main.mouseY >= TopLeft.Y && Main.mouseY <= BottomRight.Y && !PlayerInput.IgnoreMouseInterface;
	}
	public static class UI_ID {
		public const int None = 0;
		public const int EnchantingTable = 1;
		public const int Offer = 2;
		public const int WitchReroll = 3;
		public const int EnchantmentStorage = 4;
		public const int WitchRerollItemSlot = 5;

		public const int EnchantmentStorageItemSlot1 = 1000;
		public const int EnchantmentStorageItemSlotLast = EnchantmentStorageItemSlot1 + 99;
	}
}
