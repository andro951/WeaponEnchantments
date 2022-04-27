/*
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.UI;
using Terraria.UI.Chat;

namespace WeaponEnchantments
{
    internal class MouseoverUI : UIState
    {
		internal static string drawString = string.Empty;
		internal static Color drawColor = Color.White;

		public override void Update(GameTime gameTime)//*
		{
			WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
			if (Main.hoverItemName != string.Empty || wePlayer.Player.mouseInterface || Main.mouseText) return;
			base.Update(gameTime);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)//*
		{
			WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
			if (Main.hoverItemName != string.Empty || drawString == string.Empty || wePlayer.Player.mouseInterface || Main.mouseText) return;
			base.DrawSelf(spriteBatch);

			wePlayer.Player.cursorItemIconEnabled = false;
			Vector2 mousePos = new Vector2(Main.mouseX, Main.mouseY);
			mousePos.X += 10;
			mousePos.Y += 10;
			if (Main.ThickMouse)
			{
				mousePos.X += 6;
				mousePos.Y += 6;
			}

			DynamicSpriteFont value = FontAssets.MouseText.Value;
			Vector2 vector = value.MeasureString(drawString);

			if (mousePos.X + vector.X + 4f > Main.screenWidth)
			{
				mousePos.X = (int)(Main.screenWidth - vector.X - 4f);
			}
			if (mousePos.Y + vector.Y + 4f > Main.screenHeight)
			{
				mousePos.Y = (int)(Main.screenHeight - vector.Y - 4f);
			}
			ChatManager.DrawColorCodedStringWithShadow(spriteBatch, value, drawString, mousePos, drawColor, 0, Vector2.Zero, Vector2.One);//Try getting rid of this
		}
	}
}*/