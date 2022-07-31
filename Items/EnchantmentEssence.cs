using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;

namespace WeaponEnchantments.Items
{
	public abstract class EnchantmentEssence : ModItem
	{
		public int essenceRarity = -1;
		public static string[] rarityNames = new string[5] { "Basic", "Common", "Rare", "SuperRare", "UltraRare" };
		public static int[] IDs = new int[rarityNames.Length];
		public const int maxStack = 9999;
		public static float[] values = new float[rarityNames.Length];
		public static float[] xpPerEssence = new float[rarityNames.Length];
		public static float valuePerXP;
		public override string Texture => (GetType().Namespace + ".Sprites." + Name + (WEMod.clientConfig.UseAlternateEnchantmentEssenceTextures ? "Alt" : "")).Replace('.', '/');

		public virtual string Artist { private set; get; } = "Kiroto";
		public virtual string Designer { private set; get; } = "andro951";
		private int entitySize = 20;

		public Color glowColor => ((EssenceColorAttribute)Attribute.GetCustomAttribute(GetType(), typeof(EssenceColorAttribute))).color;

		public abstract int itemRarity { get; }
		public abstract int glowBrightness { get; }
		public abstract int animationFrames { get; }

		public override void SetStaticDefaults()
		{
			Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, animationFrames));
			ItemID.Sets.AnimatesAsSoul[Item.type] = true;
			ItemID.Sets.ItemIconPulse[Item.type] = true;
			ItemID.Sets.ItemNoGravity[Item.type] = true;

			for (int i = 0; i < rarityNames.Length; i++)
			{
				values[i] = (float)(25 * Math.Pow(8, i));
				xpPerEssence[i] = (float)(400 * Math.Pow(4, i));
			}
			valuePerXP = (values[rarityNames.Length - 1] / xpPerEssence[rarityNames.Length - 1]);
			GetDefaults();
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
			Tooltip.SetDefault(Enchantment.displayRarity[essenceRarity].AddSpaces() + " material for crafting and upgrading enchantments.\nCan be converted to " + xpPerEssence[essenceRarity] + " experience in an enchanting table.");
			if (!WEMod.clientConfig.UseOldRarityNames)
			{
				DisplayName.SetDefault(StringManipulation.AddSpaces(Name.Substring(0, Name.IndexOf(rarityNames[essenceRarity])) + Enchantment.displayRarity[essenceRarity]));
			}
			if (LogModSystem.printListOfContributors)
			{
				LogModSystem.UpdateContributorsList(this);
				WEMod.clientConfig.UseAlternateEnchantmentEssenceTextures = !WEMod.clientConfig.UseAlternateEnchantmentEssenceTextures;
				LogModSystem.UpdateContributorsList(this);
				WEMod.clientConfig.UseAlternateEnchantmentEssenceTextures = !WEMod.clientConfig.UseAlternateEnchantmentEssenceTextures;
			}
		}

		public override void PostUpdate()
		{
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

		private void GetDefaults()
		{
			for (int i = 0; i < rarityNames.Length; i++)
			{
				if (rarityNames[i] == Name.Substring(Name.IndexOf("Essence") + 7))
				{
					essenceRarity = i;
					break;
				}
			}
		}
		public override void SetDefaults()
		{
			GetDefaults();
			Item.value = (int)values[essenceRarity];
			Item.maxStack = maxStack;
			Item.width = entitySize;
			Item.height = entitySize;
			Item.rare = itemRarity;
		}

		public override void AddRecipes()
		{
			for (int i = 0; i < rarityNames.Length; i++)
			{
				if (essenceRarity > -1)
				{
					//Dont sell basic/common/rare with NPC!!!
					Recipe recipe = CreateRecipe();
					if (essenceRarity > 0)
					{
						recipe.AddIngredient(Mod, "EnchantmentEssence" + rarityNames[essenceRarity - 1], 8 - i);
						recipe.AddTile(Mod, WoodEnchantingTable.enchantingTableNames[i] + "EnchantingTable"); //Put this inside if(essenceRarity >0) when not testing
						recipe.Register(); //Put this inside if(essenceRarity >0) when not testing
					}


					if (essenceRarity < rarityNames.Length - 1)
					{
						recipe = CreateRecipe();
						recipe.AddIngredient(Mod, "EnchantmentEssence" + rarityNames[essenceRarity + 1], 1);
						recipe.createItem.stack = 2 + i / 2;
						recipe.AddTile(Mod, WoodEnchantingTable.enchantingTableNames[i] + "EnchantingTable");
						recipe.Register();
					}
					IDs[essenceRarity] = this.Type;
				}
			}
		}
	}

    [EssenceColor(0x2E, 0x7F, 0x4C, 0x3C, 0xA4, 0x62)]
	public class EnchantmentEssenceBasic : EnchantmentEssence
	{		
		public override int itemRarity => ModContent.RarityType<EnchantingRarityBasic>();
		public override int animationFrames => 8;
		public override int glowBrightness => 0x80;
	}
	[EssenceColor(0x1F, 0xD4, 0xDA, 0x3A, 0x4C, 0xBF)]
	public class EnchantmentEssenceCommon : EnchantmentEssence
	{		
		public override int itemRarity => ModContent.RarityType<EnchantingRarityCommon>();
		public override int animationFrames => 8;
		public override int glowBrightness => 0x84;

	}
	[EssenceColor(0x67, 0x26, 0xA1, 0x81, 0x30, 0xC9)]
	public class EnchantmentEssenceRare : EnchantmentEssence
	{		
		public override int itemRarity => ModContent.RarityType<EnchantingRarityRare>();
		public override int animationFrames => 6;
		public override int glowBrightness => 0x87;
	}
	[EssenceColor(0xF9, 0x00, 0x23, 0xCE, 0x2B, 0x42)]
	public class EnchantmentEssenceSuperRare : EnchantmentEssence
	{
		public override int itemRarity => ModContent.RarityType<EnchantingRaritySuperRare>();
		public override int animationFrames => 10;
		public override int glowBrightness => 0x89;
	}
	[EssenceColor(0xD7, 0x54, 0x09, 0xEF, 0x5D, 0x0A)]
	public class EnchantmentEssenceUltraRare : EnchantmentEssence
	{
		public override int itemRarity => ModContent.RarityType<EnchantingRarityUltraRare>();
		public override int animationFrames => 16;
		public override int glowBrightness => 0x8a;
	}
}
