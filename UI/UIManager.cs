using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;
using Terraria.UI.Gamepad;

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
		public const int ItemSlotDrawContext = 5;
		public static readonly int ItemSlotInteractContext = 30;
		private static int PanelBeingDragged = UI_ID.None;
		private static int UIBeingHovered = UI_ID.None;
		public static int LastUIBeingHovered { get; private set; } = UI_ID.None;
		public static void PostDrawInterface(SpriteBatch spriteBatch) {
			LastUIBeingHovered = UIBeingHovered;
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
		public static bool MouseHovering(UIPanelData panel, bool playSound = false) => MouseHovering(panel.IsMouseHovering, panel.ID, playSound);
		public static bool MouseHovering(UITextData text, bool playSound = false) => MouseHovering(text.IsMouseHovering, text.ID, playSound);
		public static bool MouseHovering(bool hovering, int ID, bool playSound = false) {
			if (UIBeingHovered == UI_ID.None && hovering) {
				UIBeingHovered = ID;
				Main.LocalPlayer.mouseInterface = true;
				if (playSound && LastUIBeingHovered != ID)
					SoundEngine.PlaySound(SoundID.MenuTick);

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
		public static void DrawItemSlot(SpriteBatch spriteBatch, ref Item item, int itemSlotX, int itemSlotY, int context = ItemSlotDrawContext, float hue = 0f, int glowTime = 0) {
			//ItemSlot.Draw(spriteBatch, ref item, context, new Vector2(itemSlotX, itemSlotY));
			Player player = Main.LocalPlayer;
			float inventoryScale = Main.inventoryScale;
			Color color = Color.White;
			Vector2 position = new(itemSlotX, itemSlotY);

			Texture2D value = TextureAssets.InventoryBack.Value;
			Color color2 = Main.inventoryBack;
			bool flag2 = false;

			//Glow
			if (hue != 0f && !item.favorited && !item.IsAir) {
				float num5 = Main.invAlpha / 255f;
				Color value2 = new Color(63, 65, 151, 255) * num5;
				Color value3 = Main.hslToRgb(hue, 1f, 0.5f) * num5;
				float num6 = (float)glowTime / 300f;
				num6 *= num6;
				color2 = Color.Lerp(value2, value3, num6 / 2f);
				value = TextureAssets.InventoryBack13.Value;
			}

			//Favorited
			if (item.favorited) {
				value = TextureAssets.InventoryBack10.Value;
			}

			//Draw ItemSlot
			if (!flag2)
				spriteBatch.Draw(value, position, null, color2, 0f, default(Vector2), inventoryScale, SpriteEffects.None, 0f);

			Vector2 vector = value.Size() * inventoryScale;
			if (item.type > 0 && item.stack > 0) {
				//Trash Can
				if (context == 6) {
					Texture2D value11 = TextureAssets.Trash.Value;
					Vector2 position4 = position + value.Size() * inventoryScale / 2f - value11.Size() * inventoryScale / 2f * 1.5f;
					spriteBatch.Draw(value11, position4, null, new Color(100, 100, 100, 100), 0f, default(Vector2), inventoryScale * 1.5f, SpriteEffects.None, 0f);
				}

				//Draw Item
				ItemSlot.DrawItemIcon(item, context, spriteBatch, position + vector / 2f, inventoryScale, 32f, color);

				//Draw Stack
				if (item.stack > 1)
					ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, item.stack.ToString(), position + new Vector2(10f, 26f) * inventoryScale, color, 0f, Vector2.Zero, new Vector2(inventoryScale), -1f, inventoryScale);
			}
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
		public int ID;
		public string Text;
		public float Scale;
		public Color Color;
		public bool Center;
		public bool AncorBotomLeft;
		Vector2 BaseTextSize;
		Vector2 TextSize;
		Vector2 Position => AncorBotomLeft ? new(0, BaseTextSize.Y / 2) : Vector2.Zero;
		public int Width => (int)TextSize.X;
		public int Height => (int)TextSize.Y;
		public int BaseWidth => (int)BaseTextSize.X;
		public int BaseHeight => (int)BaseTextSize.Y;
		public Point TopLeft;
		public Point BottomRight;
		public UITextData(int id, int left, int top, string text, float scale, Color color, bool center = false, bool ancorBotomLeft = false) {
			ID = id;
			Text = text;
			Scale = scale;
			Center = center;
			Color = color;
			AncorBotomLeft = ancorBotomLeft;
			BaseTextSize = FontAssets.MouseText.Value.MeasureString(text);
			TextSize = BaseTextSize * scale;
			int heightOffset = AncorBotomLeft ? (int)BaseTextSize.Y / 2 : 0;
			TopLeft = new Point(left, top + heightOffset);
			BottomRight = new Point(left + Width, top + Height + heightOffset);
		}
		public bool IsMouseHovering => Main.mouseX >= TopLeft.X && Main.mouseX <= BottomRight.X && Main.mouseY >= TopLeft.Y - Position.Y && Main.mouseY <= BottomRight.Y - Position.Y && !PlayerInput.IgnoreMouseInterface;
		public void Draw(SpriteBatch spriteBatch) {
			int left = Center ? TopLeft.X - Width / 2: TopLeft.X;
			ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, Text, new Vector2(left, TopLeft.Y), Color, 0f, Position, new Vector2(Scale), -1f, 1.5f);
		}
	}
	public static class UI_ID {
		public const int None = 0;
		public const int EnchantingTable = 1;
		public const int Offer = 2;
		public const int WitchReroll = 3;
		public const int EnchantmentStorage = 4;
		public const int EnchantmentStorageLootAll = 5;
		public const int EnchantmentStorageDepositAll = 6;
		public const int EnchantmentStorageQuickStack = 7;
		public const int EnchantmentStorageSort = 8;
		public const int EnchantmentStorageToggleVacuum = 9;

		public const int EnchantmentStorageItemSlot1 = 1000;
		public const int EnchantmentStorageItemSlotLast = EnchantmentStorageItemSlot1 + 99;
	}
}
