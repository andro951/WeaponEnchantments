using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.UI;
using Terraria.UI.Chat;
using Terraria.UI.Gamepad;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Content.NPCs;
using WeaponEnchantments.Items;
using androLib.Common.Utility;
using androLib.Common.Globals;
using androLib.UI;

namespace WeaponEnchantments.UI
{
	public class DrawnUIData {
		public int cost;
		public bool hovering;
	}
	public class WitchRerollUI
	{
		private static int GetUI_ID(int id) => MasterUIManager.GetUI_ID(id, WE_UI_ID.Witch_UITypeID);

		private static bool talkingToWitch = false;
		private static DrawnUIData drawnUIData = new();
		public static void PostDrawInterface(SpriteBatch spriteBatch) {
			//Witch Re-roll ItemSlot
			if (Witch.rerollUI) {
				int talkNPC = Main.LocalPlayer.talkNPC;
				talkingToWitch = talkNPC >= 0 && Main.npc[talkNPC].ModFullName() == "WeaponEnchantments/Witch";
				if (!talkingToWitch) {
					//Not talking to the Witch
					Witch.rerollUI = false;
					if (Witch.rerollItem.type > 0) {
						Witch.rerollItem.position = Main.LocalPlayer.Center;
						Item item2 = Main.LocalPlayer.GetItem(Main.myPlayer, Witch.rerollItem, GetItemSettings.GetItemInDropItemCheck);
						if (item2.stack > 0)
							Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_DropAsItem("Drop from Witch Re-Roll UI"), item2, item2.stack);

						Witch.rerollItem = new Item();
						Recipe.FindRecipes();
					}
				}
				else {
					//Talking to the Witch
					drawnUIData = new();

					#region Determine Text

					if (Witch.mouseRerollEnchantment) {
						if (Witch.rerollScale < 1f) {
							Witch.rerollScale += 0.02f;
						}
					}
					else if (Witch.rerollScale > 1f) {
						Witch.rerollScale -= 0.02f;
					}

					int num56 = 50;
					int num57 = 270;
					string text = Lang.inter[46].Value + ": ";
					if (Witch.rerollItem.type > 0) {
						int cost = 100000;
						if (Main.LocalPlayer.discountAvailable)
							cost *= (int)((double)cost * 0.8);

						cost = (int)((double)cost * Main.LocalPlayer.currentShoppingSettings.PriceAdjustment);

						string text2 = "";
						int num59 = 0;
						int num60 = 0;
						int num61 = 0;
						int num62 = 0;
						int num63 = cost;
						if (num63 < 1)
							num63 = 1;

						if (num63 >= 1000000) {
							num59 = num63 / 1000000;
							num63 -= num59 * 1000000;
						}

						if (num63 >= 10000) {
							num60 = num63 / 10000;
							num63 -= num60 * 10000;
						}

						if (num63 >= 100) {
							num61 = num63 / 100;
							num63 -= num61 * 100;
						}

						if (num63 >= 1)
							num62 = num63;

						if (num59 > 0)
							text2 = text2 + "[c/" + Colors.AlphaDarken(Colors.CoinPlatinum).Hex3() + ":" + num59 + " " + Lang.inter[15].Value + "] ";

						if (num60 > 0)
							text2 = text2 + "[c/" + Colors.AlphaDarken(Colors.CoinGold).Hex3() + ":" + num60 + " " + Lang.inter[16].Value + "] ";

						if (num61 > 0)
							text2 = text2 + "[c/" + Colors.AlphaDarken(Colors.CoinSilver).Hex3() + ":" + num61 + " " + Lang.inter[17].Value + "] ";

						if (num62 > 0)
							text2 = text2 + "[c/" + Colors.AlphaDarken(Colors.CoinCopper).Hex3() + ":" + num62 + " " + Lang.inter[18].Value + "] ";

						ItemSlot.DrawSavings(spriteBatch, num56 + 130, Main.instance.invBottom, horizontal: true);
						ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, text2, new Vector2((float)(num56 + 50) + FontAssets.MouseText.Value.MeasureString(text).X, num57), Microsoft.Xna.Framework.Color.White, 0f, Vector2.Zero, Vector2.One);

						int num64 = num56 + 70;
						int num65 = num57 + 40;
						bool mouseHovering = MasterUIManager.NoUIBeingHovered && Main.mouseX > num64 - 15 && Main.mouseX < num64 + 15 && Main.mouseY > num65 - 15 && Main.mouseY < num65 + 15 && !PlayerInput.IgnoreMouseInterface;
						Texture2D value4 = TextureAssets.Reforge[0].Value;
						if (mouseHovering) {
							MasterUIManager.UIBeingHovered = GetUI_ID(WE_UI_ID.WitchReroll);
							value4 = TextureAssets.Reforge[1].Value;
						}

						spriteBatch.Draw(value4, new Vector2(num64, num65), null, Color.White, 0f, value4.Size() / 2f, Witch.rerollScale, SpriteEffects.None, 0f);
						UILinkPointNavigator.SetPosition(304, new Vector2(num64, num65) + value4.Size() / 4f);

						drawnUIData.hovering = mouseHovering;
						drawnUIData.cost = cost;
					}
					else {
						text = "Place an enchantment here to re-roll";
					}

					#endregion

					if (Witch.rerollItem?.ModItem is Enchantment enchantment) {
						text += "\n";
						IEnumerable<string> tooltips = enchantment.GetEffectsTooltips().Select(t => t.Item1);
						foreach (string tooltip in tooltips) {
							text += $"\n{tooltip}";
						}
					}

					ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, text, new Vector2(num56 + 50, num57), new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor), 0f, Vector2.Zero, Vector2.One);
					if (MasterUIManager.MouseHoveringItemSlot(num56, num57, GetUI_ID(WE_UI_ID.WitchReroll))) {
						if (Main.mouseItem.NullOrAir() || Main.mouseItem?.ModItem is IRerollableEnchantment rerollableEnchantment) {
							MasterUIManager.ItemSlotClickInteractions(ref Witch.rerollItem);
						}
					}

					MasterUIManager.DrawItemSlot(spriteBatch, Witch.rerollItem, num56, num57);
				}
			}
			else {
				talkingToWitch = false;
			}
		}
		public static void UpdateInterface() {
			if (!talkingToWitch)
				return;

			if (Witch.rerollItem.type > 0) {
				bool mouseHovering = drawnUIData.hovering;
				int cost = drawnUIData.cost;

				//Mouse hovering?
				if (mouseHovering) {
					Main.hoverItemName = "Re-roll";//Lang.inter[19].Value;
					if (!Witch.mouseRerollEnchantment)
						SoundEngine.PlaySound(SoundID.MenuTick);

					Witch.mouseRerollEnchantment = true;
					Main.LocalPlayer.mouseInterface = true;

					if (Main.mouseLeftRelease && Main.mouseLeft && Main.LocalPlayer.CanAfford(cost) && Witch.rerollItem?.ModItem is IRerollableEnchantment rerollableEnchantment) {
						Main.LocalPlayer.BuyItem(cost);
						rerollableEnchantment.Reroll();
						Witch.rerollItem.position.X = Main.LocalPlayer.position.X + (float)(Main.LocalPlayer.width / 2) - (float)(Witch.rerollItem.width / 2);
						Witch.rerollItem.position.Y = Main.LocalPlayer.position.Y + (float)(Main.LocalPlayer.height / 2) - (float)(Witch.rerollItem.height / 2);

						//Todo Popup text for this
						//PopupText.NewText(PopupTextContext.ItemReforge, Witch.rerollItem, Witch.rerollItem.stack, noStack: true);
						SoundEngine.PlaySound(SoundID.Item37);
					}
				}
				else {
					Witch.mouseRerollEnchantment = false;
				}
			}
		}
	}
}
