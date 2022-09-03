using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;
using static WeaponEnchantments.Common.EnchantingRarity;

namespace WeaponEnchantments.Items
{
	public abstract class EnchantmentEssence : ModItem
	{
		public virtual int EssenceTier {
			get {
				if (essenceTier == -1) {
					essenceTier = GetTierNumberFromName(Name);
				}

				return essenceTier;
			}
		}
		private int essenceTier = -1;

		public const int maxStack = 9999;
		public static int[] IDs = new int[tierNames.Length];
		public static float[] values = new float[tierNames.Length];
		public static float[] xpPerEssence = new float[tierNames.Length];
		public static float valuePerXP;

		private int entitySize = 20;
		int glowBrightness;
		public override string Texture => (GetType().Namespace + ".Sprites." + Name + (WEMod.clientConfig.UseAlternateEnchantmentEssenceTextures ? "Alt" : "")).Replace('.', '/');
		public Color glowColor => TierColors[EssenceTier];
		public abstract int animationFrames { get; }

		public virtual string Artist { private set; get; } = "Kiroto";
		public virtual string Designer { private set; get; } = "andro951";

		public override void SetStaticDefaults() {
			int type = Item.type;
			Main.RegisterItemAnimation(type, new DrawAnimationVertical(5, animationFrames));
			ItemID.Sets.AnimatesAsSoul[type] = true;
			ItemID.Sets.ItemIconPulse[type] = true;
			ItemID.Sets.ItemNoGravity[type] = true;

			//Values and xp per essence
			for (int i = 0; i < tierNames.Length; i++) {
				values[i] = (float)(25 * Math.Pow(8, i));
				xpPerEssence[i] = (float)(400 * Math.Pow(4, i));
			}

			//Value per xp
			if(EssenceTier == 4)
				valuePerXP = values[tierNames.Length - 1] / xpPerEssence[tierNames.Length - 1];

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;

			//Tooltip
			Tooltip.SetDefault(tierNames[EssenceTier].AddSpaces() + " material for crafting and upgrading enchantments.\nCan be converted to " + xpPerEssence[EssenceTier] + " experience in an enchanting table.");
			//if (!WEMod.clientConfig.UseOldTierNames)
			//	DisplayName.SetDefault(StringManipulation.AddSpaces(Name.Substring(0, Name.IndexOf(tierNames[essenceTier])) + displayTierNames[essenceTier]));

			//Log contributors for both normal and alternate spritesheets
			if (LogModSystem.printListOfContributors) {
				LogModSystem.UpdateContributorsList(this);
				WEMod.clientConfig.UseAlternateEnchantmentEssenceTextures = !WEMod.clientConfig.UseAlternateEnchantmentEssenceTextures;
				LogModSystem.UpdateContributorsList(this);
				WEMod.clientConfig.UseAlternateEnchantmentEssenceTextures = !WEMod.clientConfig.UseAlternateEnchantmentEssenceTextures;
			}
		}

		public override void PostUpdate() {
			// Turn the alpha of the color into it's brightness (0-1)
			float intensity = glowBrightness / 255f;
			Lighting.AddLight(Item.Center, glowColor.ToVector3() * intensity * Main.essScale);
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			// Add glow mask
			Texture2D texture = TextureAssets.Item[Item.type].Value;

			// Calculate the animation's current frame
			int currentFrame = Main.itemFrameCounter[whoAmI];
			Rectangle frame = Main.itemAnimations[Item.type] is not null ? Main.itemAnimations[Item.type].GetFrame(texture, currentFrame) : texture.Frame();

			// Draw over the sprite
			spriteBatch.Draw
			(
				texture,
				new Vector2
				(
					Item.position.X - Main.screenPosition.X + Item.width * 0.5f,
					Item.position.Y - Main.screenPosition.Y + Item.height * 0.5f
				),
				frame,
				Color.White,
				rotation,
				new Vector2
				(
					texture.Width,
					texture.Height / animationFrames
				) * 0.5f,
				scale,
				SpriteEffects.None,
				0f
			);
		}
		public override void SetDefaults() {
			Item.value = (int)values[EssenceTier];
			Item.maxStack = maxStack;
			Item.width = entitySize;
			Item.height = entitySize;
			Item.rare = GetRarityFromTier(EssenceTier);

			glowBrightness = 128 + (int)((9f - EssenceTier) / 2f * EssenceTier);//Calculus useful for something =D
		}

		public override void AddRecipes() {
			for (int i = 0; i < tierNames.Length; i++) {
				if (EssenceTier > -1) {
					Recipe recipe = CreateRecipe();
					if (EssenceTier > 0) {
						recipe.AddIngredient(Mod, "EnchantmentEssence" + tierNames[EssenceTier - 1], 8 - i);
						recipe.AddTile(Mod, EnchantingTableItem.enchantingTableNames[i] + "EnchantingTable");
						recipe.Register();
					}

					if (EssenceTier < tierNames.Length - 1) {
						recipe = CreateRecipe();
						recipe.AddIngredient(Mod, "EnchantmentEssence" + tierNames[EssenceTier + 1], 1);
						recipe.createItem.stack = 2 + i / 2;
						recipe.AddTile(Mod, EnchantingTableItem.enchantingTableNames[i] + "EnchantingTable");
						recipe.Register();
					}

					IDs[EssenceTier] = Type;
				}
			}
		}
	}
	public class EnchantmentEssenceBasic : EnchantmentEssence
	{
		public override int animationFrames => 8;
	}
	public class EnchantmentEssenceCommon : EnchantmentEssence
	{
		public override int animationFrames => 8;

	}
	public class EnchantmentEssenceRare : EnchantmentEssence
	{
		public override int animationFrames => 6;
	}
	public class EnchantmentEssenceEpic : EnchantmentEssence
	{
		public override int animationFrames => 10;
	}
	public class EnchantmentEssenceLegendary : EnchantmentEssence
	{
		public override int animationFrames => 16;
	}
}
