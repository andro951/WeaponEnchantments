using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Content.NPCs;
using static System.Net.Mime.MediaTypeNames;

namespace WeaponEnchantments.UI
{
	public static class UIManager
	{
		public static bool DisplayingAnyUI => WEPlayer.LocalWEPlayer.displayEnchantmentStorage || WEPlayer.LocalWEPlayer.usingEnchantingTable || Witch.rerollUI || WEPlayer.LocalWEPlayer.displayOreBagUI;
		public static bool NoPanelBeingDragged => PanelBeingDragged == UI_ID.None;
		public static bool NoUIBeingHovered => UIBeingHovered == UI_ID.None;
		public static bool HoveringWitchReroll => UI_ID.WitchReroll <= UIBeingHovered && UIBeingHovered < UI_ID.WitchRerollEnd;
		public static bool HoveringEnchantmentStorage => UI_ID.EnchantmentStorage <= UIBeingHovered && UIBeingHovered < UI_ID.EnchantmentStorageEnd;
		public static bool HoveringOreBag => UI_ID.OreBag <= UIBeingHovered && UIBeingHovered < UI_ID.OreBagEnd;
		public static bool HoveringEnchantingTable => UI_ID.EnchantingTable <= UIBeingHovered && UIBeingHovered < UI_ID.EnchantingTableEnd;
		private static int mouseOffsetX = 0;
		private static int mouseOffsetY = 0;
		public static bool lastMouseLeft = false;
		public static bool LeftMouseClicked => Main.mouseLeft && !lastMouseLeft;
		public static bool LeftMouseDown = false;
		public static Color MouseColor {
			get {
				Color mouseColor = Color.White * (1f - (255f - (float)(int)Main.mouseTextColor) / 255f * 0.5f);
				mouseColor.A = byte.MaxValue;
				return mouseColor;
			}
		}
		public static readonly Asset<Texture2D>[] uiTextures = { Main.Assets.Request<Texture2D>("Images/UI/PanelBackground"), Main.Assets.Request<Texture2D>("Images/UI/PanelBorder") };
		public static readonly int ItemSlotSize = 44;
		public const int ItemSlotInteractContext = ItemSlot.Context.BankItem;
		public static int PanelBeingDragged { get; private set; } = UI_ID.None;
		public static int UIBeingHovered = UI_ID.None;
		public static int LastUIBeingHovered { get; private set; } = UI_ID.None;
		public static int HoverTime = 0;
		public static int ScrollWheel = 0;
		public static int LastScrollWheel = 0;
		public static int ScrollWheelTicks => (LastScrollWheel - ScrollWheel) / 120;
		public static int FocusRecipe = Main.focusRecipe;
		public static int LastFocusRecipe = Main.focusRecipe;
		public static int SearchBarTimer = 0;
		public static bool ShouldShowSearchBarHeartbeat => SearchBarTimer % 60 >= 30;
		public static string SearchBarString = "";
		public static int SearchBarInUse = UI_ID.None;
		public static bool TypingOnAnySearchBar = false;
		public static void PostDrawInterface(SpriteBatch spriteBatch) {
			WEPlayer wePlayer = WEPlayer.LocalWEPlayer;
			if (wePlayer.disableLeftShiftTrashCan) {
				ItemSlot.Options.DisableLeftShiftTrashCan = true;
				wePlayer.disableLeftShiftTrashCan = false;
			}

			if (!DisplayingAnyUI)
				return;

			if (NoPanelBeingDragged) {
				if (!NoUIBeingHovered && UIBeingHovered == LastUIBeingHovered) {
					HoverTime++;
				}
				else {
					HoverTime = 0;
				}

				LastUIBeingHovered = UIBeingHovered;
				UIBeingHovered = UI_ID.None;
			}

			if (TypingOnAnySearchBar) {
				SearchBarTimer++;
			}
			else {
				SearchBarTimer = 0;
			}

			LastScrollWheel = ScrollWheel;
			ScrollWheel = (int)typeof(Mouse).GetField("INTERNAL_MouseWheel", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
			LastFocusRecipe = FocusRecipe;
			FocusRecipe = Main.focusRecipe;
			float savedInventoryScale = Main.inventoryScale;
			Main.inventoryScale = 0.86f;
			bool preventTrashingItem = wePlayer.usingEnchantingTable || wePlayer.displayOreBagUI && OreBagUI.CanBeStored(Main.HoverItem);
			if (preventTrashingItem) {
				//Disable Left Shift to Quick trash
				if (ItemSlot.Options.DisableLeftShiftTrashCan) {
					wePlayer.disableLeftShiftTrashCan = ItemSlot.Options.DisableLeftShiftTrashCan;
					ItemSlot.Options.DisableLeftShiftTrashCan = false;
				}
			}

			EnchantingTableUI.PostDrawInterface(spriteBatch);
			EnchantmentStorage.PostDrawInterface(spriteBatch);
			WitchRerollUI.PostDrawInterface(spriteBatch);
			OreBagUI.PostDrawInterface(spriteBatch);

			Main.inventoryScale = savedInventoryScale;
			lastMouseLeft = Main.mouseLeft;
		}
		public static void PostUpdateEverything() {
			if (Main.focusRecipe != FocusRecipe && (HoveringEnchantmentStorage || HoveringOreBag))
				Main.focusRecipe = FocusRecipe;
		}
		public static string DisplayedSearchBarString(int SearchBarID) {
			if (!UsingSearchBar(SearchBarID))
				return EnchantmentStorageTextID.Search.ToString().Lang(L_ID1.EnchantmentStorageText);

			return $"{(SearchBarString.Length > 15 ? SearchBarString.Substring(SearchBarString.Length - 15) : SearchBarString)}{Main.chatText}{(ShouldShowSearchBarHeartbeat ? "|" : SearchBarString != "" ? "" : " ")}";
		}
		public static bool UsingSearchBar(int ID) => SearchBarInUse == ID;
		public static bool TypingOnSearchBar(int ID) => UsingSearchBar(ID) && TypingOnAnySearchBar;
		public static void TryResetSearch(int ID) {
			if (SearchBarInUse == ID)
				ResetSearch();
		}
		public static void ResetSearch() {
			SearchBarString = "";
			SearchBarInUse = UI_ID.None;
			TypingOnAnySearchBar = false;
		}
		public static void StopTypingOnSearchBar() {
			TypingOnAnySearchBar = false;
			if (SearchBarString == "")
				SearchBarInUse = UI_ID.None;
		}
		public static void ClickSearchBar(int ID) {
			if (UsingSearchBar(ID)) {
				if (TypingOnAnySearchBar) {
					StopTypingOnSearchBar();
				}
				else {
					TypingOnAnySearchBar = true;
				}
			}
			else {
				if (SearchBarInUse != UI_ID.None)
					TryResetSearch(SearchBarInUse);

				StartTypingOnSearchBar(ID);
			}
		}
		public static void StartTypingOnSearchBar(int ID) {
			TypingOnAnySearchBar = true;
			SearchBarInUse = ID;
		}
		public static bool MouseHovering(UIPanel panel, int ID, bool playSound = false) {
			if (NoUIBeingHovered && panel.IsMouseHovering) {
				SetMouseHovering(ID, playSound);
				return true;
			}

			return false;
		}
		public static bool MouseHovering(UIPanelData panel, bool playSound = false) {
			if (NoUIBeingHovered && panel.IsMouseHovering) {
				SetMouseHovering(panel.ID, playSound);
				return true;
			}

			return false;
		}
		public static bool MouseHovering(UITextData text, bool playSound = false) {
			if (NoUIBeingHovered && text.IsMouseHovering) {
				SetMouseHovering(text.ID, playSound);
				return true;
			}

			return false;
		}
		public static bool MouseHovering(UIButtonData button, bool playSound = false) {
			if (NoUIBeingHovered && button.IsMouseHovering) {
				SetMouseHovering(button.ID, playSound);
				return true;
			}

			return false;
		}
		private static void SetMouseHovering(int ID, bool playSound = false) {
			UIBeingHovered = ID;
			Main.LocalPlayer.mouseInterface = true;
			if (playSound && LastUIBeingHovered != ID)
				SoundEngine.PlaySound(SoundID.MenuTick);
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
		public static void DrawItemSlot(SpriteBatch spriteBatch, Item item, UIItemSlotData slot, int context = ItemSlotContextID.Normal, float hue = 0f, int glowTime = 0, int stack = int.MinValue) {
			DrawItemSlot(spriteBatch, item, slot.TopLeft.X, slot.TopLeft.Y, context, hue, glowTime, stack);
		}
		public static void DrawItemSlot(SpriteBatch spriteBatch, Item item, int itemSlotX, int itemSlotY, int context = ItemSlotContextID.Normal, float hue = 0f, int glowTime = 0, int stack = int.MinValue) {
			//ItemSlot.Draw(spriteBatch, ref item, context, new Vector2(itemSlotX, itemSlotY));
			if (stack == int.MinValue)
				stack = item.stack;

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
				Main.instance.LoadItem(item.type);
				Texture2D value7 = TextureAssets.Item[item.type].Value;
				Rectangle rectangle2 = (Main.itemAnimations[item.type] == null) ? value7.Frame() : Main.itemAnimations[item.type].GetFrame(value7);
				Color currentColor = color;
				float scale3 = 1f;
				ItemSlot.GetItemLight(ref currentColor, ref scale3, item);
				float num8 = 1f;
				if (rectangle2.Width > 32 || rectangle2.Height > 32)
					num8 = ((rectangle2.Width <= rectangle2.Height) ? (32f / (float)rectangle2.Height) : (32f / (float)rectangle2.Width));

				num8 *= inventoryScale;
				Vector2 position2 = position + vector / 2f - rectangle2.Size() * num8 / 2f;
				Vector2 origin = rectangle2.Size() * (scale3 / 2f - 0.5f);

				if (!ItemLoader.PreDrawInInventory(item, spriteBatch, position2, rectangle2, item.GetAlpha(currentColor), item.GetColor(color), origin, num8 * scale3))
					goto SkipVanillaItemDraw;

				spriteBatch.Draw(value7, position2, rectangle2, item.GetAlpha(currentColor), 0f, origin, num8 * scale3, SpriteEffects.None, 0f);
				if (item.color != Color.Transparent) {
					Color newColor = color;
					if (context == 13)
						newColor.A = byte.MaxValue;

					// Extra context.

					spriteBatch.Draw(value7, position2, rectangle2, item.GetColor(newColor), 0f, origin, num8 * scale3, SpriteEffects.None, 0f);
				}

				SkipVanillaItemDraw:
				ItemLoader.PostDrawInInventory(item, spriteBatch, position2, rectangle2, item.GetAlpha(currentColor), item.GetColor(color), origin, num8 * scale3);

				//Draw Stack
				if (item.stack > 1 || stack != item.stack)
					ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, stack.ToString(), position + new Vector2(10f, 26f) * inventoryScale, color, 0f, Vector2.Zero, new Vector2(inventoryScale), -1f, inventoryScale);
			}
		}
		public static bool MouseHoveringItemSlot(int itemSlotX, int itemSlotY, int ID) {
			if (NoUIBeingHovered && Main.mouseX >= itemSlotX && Main.mouseX <= itemSlotX + ItemSlotSize && Main.mouseY >= itemSlotY && Main.mouseY <= itemSlotY + ItemSlotSize && !PlayerInput.IgnoreMouseInterface) {
				Main.LocalPlayer.mouseInterface = true;
				UIBeingHovered = ID;
				return true;
			}

			return false;
		}
		public static void ItemSlotClickInteractions(ref Item item, int context = ItemSlotInteractContext) {
			ItemSlot.Handle(ref item, context);
			/*
			ItemSlot.LeftClick(ref item, context);
			if (Main.mouseLeftRelease && Main.mouseLeft)
				Recipe.FindRecipes();

			ItemSlot.RightClick(ref item, context);
			ItemSlot.MouseHover(ref item, context);
			*/
		}
		public static void ItemSlotClickInteractions(EnchantmentsArray enchantmentsArray, int index, int context) {
			Item enchantmentItem = enchantmentsArray[index];
			ItemSlotClickInteractions(ref enchantmentItem, context);
			if (!enchantmentItem.IsSameEnchantment(enchantmentsArray[index]))
				enchantmentsArray[index] = enchantmentItem;
		}
	}
	public struct UIPanelData {
		public Point TopLeft;
		public Point BottomRight;
		public int Width => BottomRight.X - TopLeft.X;
		public int Height => BottomRight.Y - TopLeft.Y;
		public int ID;
		public Color Color;
		public UIPanelData(int id, int left, int top, int width, int height, Color color) {
			ID = id;
			TopLeft = new Point(left, top);
			BottomRight = new Point(left + width, top + height);
			Color = color;
		}
		public UIPanelData(int id, Point topLeft, Point bottomRight, Color color) {
			ID = id;
			TopLeft = topLeft;
			BottomRight = bottomRight;
			Color = color;
		}
		public bool IsMouseHovering => Main.mouseX >= TopLeft.X && Main.mouseX <= BottomRight.X && Main.mouseY >= TopLeft.Y && Main.mouseY <= BottomRight.Y && !PlayerInput.IgnoreMouseInterface;
		public Point Center => new((TopLeft.X + BottomRight.X) / 2, (TopLeft.Y + BottomRight.Y) / 2);
		public void Draw(SpriteBatch spriteBatch) => UIManager.DrawUIPanel(spriteBatch, this, Color);
		public bool MouseHovering() => UIManager.MouseHovering(this);
		public void TryStartDraggingUI() => UIManager.TryStartDraggingUI(this);
		public bool ShouldDragUI() => UIManager.ShouldDragUI(this);
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
			Vector2 baseSize = text != null ? FontAssets.MouseText.Value.MeasureString(Text) : Vector2.Zero;
			BaseTextSize = baseSize;
			Vector2 size = baseSize * scale;
			TextSize = size;
			int heightOffset = ancorBotomLeft ? (int)baseSize.Y / 2 : 0;
			TopLeft = new Point(left, top + heightOffset);
			BottomRight = new Point(left + (int)size.X, top + (int)size.Y + heightOffset);
		}
		public UITextData(int id, int left, int top, TextData textData, Color color, bool center = false, bool ancorBotomLeft = false) {
			ID = id;
			Text = textData.Text;
			Scale = textData.Scale;
			Center = center;
			Color = color;
			AncorBotomLeft = ancorBotomLeft;
			Vector2 baseSize = textData.Text != null ? FontAssets.MouseText.Value.MeasureString(Text) : Vector2.Zero;
			BaseTextSize = baseSize;
			Vector2 size = baseSize * textData.Scale;
			TextSize = size;
			int heightOffset = ancorBotomLeft ? (int)baseSize.Y / 2 : 0;
			TopLeft = new Point(left, top + heightOffset);
			BottomRight = new Point(left + (int)size.X, top + (int)size.Y + heightOffset);
		}
		public bool IsMouseHovering => Main.mouseX >= TopLeft.X && Main.mouseX <= BottomRight.X && Main.mouseY >= TopLeft.Y - Position.Y && Main.mouseY <= BottomRight.Y - Position.Y && !PlayerInput.IgnoreMouseInterface;
		public bool MouseHovering() => UIManager.MouseHovering(this, true);
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
		public int BaseWidth => (int)BaseTextSize.X;
		public int BaseHeight => (int)BaseTextSize.Y;
		public Vector2 BaseTextSize;
		public Vector2 TextSize;
		public TextData(string text, float scale = 1f) {
			Text = text;
			Scale = scale;
			BaseTextSize = text != null ? FontAssets.MouseText.Value.MeasureString(text) : Vector2.Zero;
			TextSize = BaseTextSize * Scale;
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
		Vector2 Position => new(0, -2);// TextSize / 2;
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
		private bool? isMouseHovering = null;
		public bool MouseHovering() => UIManager.MouseHovering(this, true);
		public void Draw(SpriteBatch spriteBatch) {
			UIManager.DrawUIPanel(spriteBatch, TopLeft, BottomRight, IsMouseHovering ? HoverColor : PanelColor);
			ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, Text, TopLeft + Borders, Color, 0f, Position, new Vector2(Scale), -1f, 1.5f);
		}
	}
	public struct UIItemSlotData {
		public Point TopLeft;
		public Point BottomRight;
		public int ID;
		public UIItemSlotData(int id, int left, int top) {
			ID = id;
			TopLeft = new Point(left, top);
			BottomRight = new Point(left + UIManager.ItemSlotSize, top + UIManager.ItemSlotSize);
		}
		public UIItemSlotData(int id, Point topLeft) {
			ID = id;
			TopLeft = topLeft;
			BottomRight = new Point(TopLeft.X + UIManager.ItemSlotSize, TopLeft.Y + UIManager.ItemSlotSize);
		}
		public bool IsMouseHovering => Main.mouseX >= TopLeft.X && Main.mouseX <= BottomRight.X && Main.mouseY >= TopLeft.Y && Main.mouseY <= BottomRight.Y && !PlayerInput.IgnoreMouseInterface;
		public Point Center => new((TopLeft.X + BottomRight.X) / 2, (TopLeft.Y + BottomRight.Y) / 2);
		public void Draw(SpriteBatch spriteBatch, Item item, int context = ItemSlotContextID.Normal, float hue = 0f, int glowTime = 0, int stack = int.MinValue) {
			UIManager.DrawItemSlot(spriteBatch, item, this, context, hue, glowTime, stack);
		}
		public bool MouseHovering() => UIManager.MouseHoveringItemSlot(TopLeft.X, TopLeft.Y, ID);
		public void ClickInteractions(ref Item item, int context = UIManager.ItemSlotInteractContext) => UIManager.ItemSlotClickInteractions(ref item, context);
		public void ClickInteractions(EnchantmentsArray enchantmentsArray, int index, int context = UIManager.ItemSlotInteractContext) => UIManager.ItemSlotClickInteractions(enchantmentsArray, index, context);
	}
	public static class UI_ID {
		public const int None = -1;

		public const int WitchReroll = 0;
		public const int WitchRerollEnd = Offer;

		public const int Offer = 100;
		public const int OfferYes = 101;
		public const int OfferNo = 102;
		public const int OfferEnd = EnchantmentStorage;

		public const int EnchantmentStorage = 1000;
		public const int EnchantmentStorageScrollBar = 1001;
		public const int EnchantmentStorageScrollPanel = 1002;
		public const int EnchantmentStorageSearch = 1003;
		public const int EnchantmentStorageLootAll = 1100;
		public const int EnchantmentStorageDepositAll = 1101;
		public const int EnchantmentStorageQuickStack = 1102;
		public const int EnchantmentStorageSort = 1103;
		public const int EnchantmentStorageToggleVacuum = 1104;
		public const int EnchantmentStorageToggleMarkTrash = 1105;
		public const int EnchantmentStorageUncraftAllTrash = 1106;
		public const int EnchantmentStorageRevertAllToBasic = 1107;
		public const int EnchantmentStorageManageTrash = 1108;
		public const int EnchantmentStorageManageOfferedItems = 1109;
		public const int EnchantmentstorageQuickCrafting = 1110;
		public const int EnchantmentStorageItemSlot = 1200;
		public const int EnchantmentStorageEnd = EnchantingTable;

		public const int EnchantingTable = 2000;
		public const int EnchantingTableLootAll = 2001;
		public const int EnchantingTableOfferButton = 2002;
		public const int EnchantingTableSyphon = 2003;
		public const int EnchantingTableInfusion = 2004;
		public const int EnchantingTableLevelUp = 2005;
		public const int EnchantingTableItemSlot = 2006;
		public const int EnchantingTableStorageButton = 2007;
		public const int EnchantingTableEnchantment0 = 2200;
		public const int EnchantingTableEnchantmentLast = EnchantingTableEnchantment0 + EnchantingTableUI.MaxEnchantmentSlots - 1;
		public const int EnchantingTableEssence0 = 2300;
		public const int EnchantingTableEssenceLast = EnchantingTableEssence0 + EnchantingTableUI.MaxEssenceSlots - 1;
		public const int EnchantingTableLevelsPerLevelUp0 = 2404;
		public const int EnchantingTableLevelsPerLevelUpLast = 2407;
		public const int EnchantingTableXPButton0 = 2500;
		public const int EnchantingTableXPButtonLast = EnchantingTableXPButton0 + EnchantingTableUI.MaxEssenceSlots - 1;
		public const int EnchantingTableEnd = OreBag;

		public const int OreBag = 3000;
		public const int OreBagScrollBar = 3001;
		public const int OreBagScrollPanel = 3002;
		public const int OreBagSearch = 3003;
		public const int OreBagLootAll = 3100;
		public const int OreBagDepositAll = 3101;
		public const int OreBagQuickStack = 3102;
		public const int OreBagSort = 3103;
		public const int OreBagToggleVacuum = 3104;
		public const int OreBagItemSlot = 3200;
		public const int OreBagEnd = 4000;
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
