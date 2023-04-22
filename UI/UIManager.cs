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
using static System.Net.Mime.MediaTypeNames;

namespace WeaponEnchantments.UI
{
	public static class UIManager
	{
		public static bool NoPanelBeingDragged => PanelBeingDragged == UI_ID.None;
		private static int mouseOffsetX = 0;
		private static int mouseOffsetY = 0;
		public static bool lastMouseLeft = false;
		public static bool LeftMouseClicked => Main.mouseLeft && !lastMouseLeft;
		public static Color MouseColor {
			get {
				Color mouseColor = Color.White * (1f - (255f - (float)(int)Main.mouseTextColor) / 255f * 0.5f);
				mouseColor.A = byte.MaxValue;
				return mouseColor;
			}
		}
		public static readonly Asset<Texture2D>[] uiTextures = { Main.Assets.Request<Texture2D>("Images/UI/PanelBackground"), Main.Assets.Request<Texture2D>("Images/UI/PanelBorder") };
		public static readonly int ItemSlotSize = 44;
		public static readonly int ItemSlotInteractContext = 30;
		private static int PanelBeingDragged = UI_ID.None;
		private static int UIBeingHovered = UI_ID.None;
		public static int LastUIBeingHovered { get; private set; } = UI_ID.None;
		public static void PostDrawInterface(SpriteBatch spriteBatch) {
			LastUIBeingHovered = UIBeingHovered;
			UIBeingHovered = UI_ID.None;
			float savedInventoryScale = Main.inventoryScale;
			Main.inventoryScale = 0.86f;

			EnchantingTableUI.PostDrawInterface(spriteBatch);
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
			DrawUIPanel(spriteBatch, panel.TopLeft.X, panel.TopLeft.Y, panel.BottomRight.X, panel.BottomRight.Y, panelColor);
		}
		public static void DrawUIPanel(SpriteBatch spriteBatch, Point panelTopLeft, Point panelBottomRight, Color panelColor) {
			DrawUIPanel(spriteBatch, panelTopLeft.X, panelTopLeft.Y, panelBottomRight.X, panelBottomRight.Y, panelColor);
		}
		public static void DrawUIPanel(SpriteBatch spriteBatch, Vector2 panelTopLeft, Vector2 panelBottomRight, Color panelColor) {
			DrawUIPanel(spriteBatch, (int)panelTopLeft.X, (int)panelTopLeft.Y, (int)panelBottomRight.X, (int)panelBottomRight.Y, panelColor);
		}
		public static void DrawUIPanel(SpriteBatch spriteBatch, int Left, int Top, int Right, int Bottom, Color panelColor) {
			int _barSize = 4;
			int cornerSize = 12;
			Right -= cornerSize;
			Bottom -= cornerSize;
			int width = Right - Left - cornerSize;
			int height = Bottom - Top - cornerSize;
			Color[] colors = { panelColor, Color.Black };
			for (int i = 0; i < uiTextures.Length; i++) {
				Texture2D texture = uiTextures[i].Value;
				Color color = colors[i];
				spriteBatch.Draw(texture, new Rectangle(Left, Top, cornerSize, cornerSize), new Rectangle(0, 0, cornerSize, cornerSize), color);
				spriteBatch.Draw(texture, new Rectangle(Right, Top, cornerSize, cornerSize), new Rectangle(cornerSize + _barSize, 0, cornerSize, cornerSize), color);
				spriteBatch.Draw(texture, new Rectangle(Left, Bottom, cornerSize, cornerSize), new Rectangle(0, cornerSize + _barSize, cornerSize, cornerSize), color);
				spriteBatch.Draw(texture, new Rectangle(Right, Bottom, cornerSize, cornerSize), new Rectangle(cornerSize + _barSize, cornerSize + _barSize, cornerSize, cornerSize), color);
				spriteBatch.Draw(texture, new Rectangle(Left + cornerSize, Top, width, cornerSize), new Rectangle(cornerSize, 0, _barSize, cornerSize), color);
				spriteBatch.Draw(texture, new Rectangle(Left + cornerSize, Bottom, width, cornerSize), new Rectangle(cornerSize, cornerSize + _barSize, _barSize, cornerSize), color);
				spriteBatch.Draw(texture, new Rectangle(Left, Top + cornerSize, cornerSize, height), new Rectangle(0, cornerSize, cornerSize, _barSize), color);
				spriteBatch.Draw(texture, new Rectangle(Right, Top + cornerSize, cornerSize, height), new Rectangle(cornerSize + _barSize, cornerSize, cornerSize, _barSize), color);
				spriteBatch.Draw(texture, new Rectangle(Left + cornerSize, Top + cornerSize, width, height), new Rectangle(cornerSize, cornerSize, _barSize, _barSize), color);
			}
		}
		public static void DrawItemSlot(SpriteBatch spriteBatch, ref Item item, int itemSlotX, int itemSlotY, int context = ItemSlotContextID.Normal, float hue = 0f, int glowTime = 0) {
			//ItemSlot.Draw(spriteBatch, ref item, context, new Vector2(itemSlotX, itemSlotY));
			Player player = Main.LocalPlayer;
			float inventoryScale = Main.inventoryScale;
			Color color = Color.White;
			Vector2 position = new(itemSlotX, itemSlotY);

			Texture2D texture;
			Color color2 = Main.inventoryBack;

			switch (context) {
				case ItemSlotContextID.Purple when !item.favorited:
					texture = TextureAssets.InventoryBack4.Value;//Purple
					break;
				case ItemSlotContextID.Purple when item.favorited:
					texture = (Texture2D)ModContent.Request<Texture2D>("WeaponEnchantments/UI/Sprites/Inventory_Back4(Favorited)");
					break;
				case ItemSlotContextID.Favorited:
					texture = TextureAssets.InventoryBack5.Value;
					break;
				case 6:
					texture = TextureAssets.InventoryBack6.Value;
					break;
				case 7:
					texture = TextureAssets.InventoryBack7.Value;
					break;
				case 8:
					texture = TextureAssets.InventoryBack8.Value;
					break;
				case 9:
					texture = TextureAssets.InventoryBack9.Value;
					break;
				case ItemSlotContextID.Ten:
					texture = TextureAssets.InventoryBack10.Value;
					break;
				case 11:
					texture = TextureAssets.InventoryBack11.Value;
					break;
				case 12:
					texture = TextureAssets.InventoryBack12.Value;
					break;
				case 13:
					texture = TextureAssets.InventoryBack13.Value;
					break;
				case 14:
					texture = TextureAssets.InventoryBack14.Value;
					break;
				case 15:
					texture = TextureAssets.InventoryBack15.Value;
					break;
				case 16:
					texture = TextureAssets.InventoryBack16.Value;
					break;
				case ItemSlotContextID.Gold:
					texture = TextureAssets.InventoryBack17.Value;
					break;
				case 18:
					texture = TextureAssets.InventoryBack18.Value;
					break;
				default:
					texture = item.favorited ? TextureAssets.InventoryBack5.Value : TextureAssets.InventoryBack.Value;
					break;
			}

			//Glow
			if (hue != 0f && !item.favorited && !item.IsAir) {
				float num5 = Main.invAlpha / 255f;
				Color value2 = new Color(63, 65, 151, 255) * num5;
				Color value3 = Main.hslToRgb(hue, 1f, 0.5f) * num5;
				float num6 = (float)glowTime / 300f;
				num6 *= num6;
				color2 = Color.Lerp(value2, value3, num6 / 2f);
				texture = TextureAssets.InventoryBack13.Value;
			}

			//Favorited
			if (item.favorited || context == ItemSlotContextID.Favorited) {
				texture = TextureAssets.InventoryBack10.Value;
			}

			//Draw ItemSlot
			spriteBatch.Draw(texture, position, null, color2, 0f, default(Vector2), inventoryScale, SpriteEffects.None, 0f);

			Vector2 vector = texture.Size() * inventoryScale;
			if (item.type > ItemID.None && item.stack > 0) {
				//Trash Can
				if (context == ItemSlotContextID.MarkedTrash) {
					Texture2D value11 = TextureAssets.Trash.Value;
					Vector2 position4 = position + texture.Size() * inventoryScale / 2f - value11.Size() * inventoryScale / 2f * 1.5f;
					spriteBatch.Draw(value11, position4, null, new Color(100, 100, 100, 100), 0f, default(Vector2), inventoryScale * 1.5f, SpriteEffects.None, 0f);
				}

				//Draw Item
				ItemSlot.DrawItemIcon(item, 5, spriteBatch, position + vector / 2f, inventoryScale, 32f, color);

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
		public UIPanelData(int id, Point topLeft, Point bottomRight) {
			ID = id;
			TopLeft = topLeft;
			BottomRight = bottomRight;
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
			TextSize = BaseTextSize * Scale;
			int heightOffset = AncorBotomLeft ? (int)BaseTextSize.Y / 2 : 0;
			TopLeft = new Point(left, top + heightOffset);
			BottomRight = new Point(left + Width, top + Height + heightOffset);
		}
		public UITextData(int id, int left, int top, TextData textData, Color color, bool center = false, bool ancorBotomLeft = false) {
			ID = id;
			Text = textData.Text;
			Scale = textData.Scale;
			Center = center;
			Color = color;
			AncorBotomLeft = ancorBotomLeft;
			BaseTextSize = FontAssets.MouseText.Value.MeasureString(Text);
			TextSize = BaseTextSize * Scale;
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
	public struct TextData {
		public string Text;
		public float Scale;
		public int Width => (int)TextSize.X;
		public int Height => (int)TextSize.Y;
		public Vector2 TextSize;
		public TextData(string text, float scale = 1f) {
			Text = text;
			Scale = scale;
			TextSize = FontAssets.MouseText.Value.MeasureString(text) * scale;
		}
	}
	public struct UIButtonData
	{
		public int ID;
		public string Text;
		public float Scale;
		public Color Color;
		Vector2 TextSize;
		Vector2 Borders;
		Vector2 Position => Vector2.Zero;// TextSize / 2;
		public int Width;
		public int Height;
		public Vector2 TopLeft;
		public Vector2 BottomRight;
		public Color PanelColor;
		public Color HoverColor;
		public UIButtonData(int id, int X, int top, string text, float scale, Color color, int borderWidth, int borderHeight, Color panelColor, Color hoverColor, bool fromRight = false) {
			ID = id;
			Text = text;
			Scale = scale;
			Color = color;
			TextSize = FontAssets.MouseText.Value.MeasureString(text) * scale;
			Borders = new Vector2(borderWidth, borderHeight);
			Width = (int)TextSize.X + borderWidth * 2;
			Height = (int)TextSize.Y + borderHeight * 2;
			TopLeft = new Vector2(X - (fromRight ? Width : 0), top);
			BottomRight = new Vector2(X + (fromRight ? 0 : Width), top + Height);
			PanelColor = panelColor;
			HoverColor = hoverColor;
		}
		public UIButtonData(int id, int X, int top, TextData textData, Color color, int borderWidth, int borderHeight, Color panelColor, Color hoverColor, bool fromRight = false) {
			ID = id;
			Text = textData.Text;
			Scale = textData.Scale;
			Color = color;
			TextSize = textData.TextSize;
			Borders = new Vector2(borderWidth, borderHeight * 2);
			Width = (int)TextSize.X + borderWidth * 2;
			Height = (int)TextSize.Y + borderHeight * 2;
			TopLeft = new Vector2(X - (fromRight ? Width : 0), top);
			BottomRight = new Vector2(X + (fromRight ? 0 : Width), top + Height);
			PanelColor = panelColor;
			HoverColor = hoverColor;
		}
		public bool IsMouseHovering {
			get {
				if (isMouseHovering == null)
					isMouseHovering = Main.mouseX >= TopLeft.X && Main.mouseX <= BottomRight.X && Main.mouseY >= TopLeft.Y - Position.Y && Main.mouseY <= BottomRight.Y - Position.Y && !PlayerInput.IgnoreMouseInterface;

				return (bool)isMouseHovering;
			}
		}
		private bool? isMouseHovering;
		public void Draw(SpriteBatch spriteBatch) {
			UIManager.DrawUIPanel(spriteBatch, TopLeft, BottomRight, IsMouseHovering ? HoverColor : PanelColor);
			ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, Text, TopLeft + Borders, Color, 0f, Position, new Vector2(Scale), -1f, 1.5f);
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
		public const int EnchantingTableLootAll = 10;
		public const int EnchantingTableOfferButton = 11;
		public const int EnchantingTableSyphon = 12;
		public const int EnchantingTableInfusion = 13;
		public const int EnchantingTableLevelUp = 14;

		public const int EnchantingTableLevelsPerLevelUp0 = 800;
		public const int EnchantingTableLevelsPerLevelUpLast = 803;

		public const int EnchantingTableXPButton0 = 900;
		public const int EnchantingTableXPButtonLast = 904;

		public const int EnchantmentStorageItemSlot1 = 1000;
		public const int EnchantmentStorageItemSlotLast = EnchantmentStorageItemSlot1 + 99;
	}
	public static class ItemSlotContextID
	{
		public const int MarkedTrash = -1;
		public const int Normal = 0;
		public const int Purple = 4;
		public const int Favorited = 5;
		public const int Ten = 10;
		public const int Gold = 17;
	}
}
