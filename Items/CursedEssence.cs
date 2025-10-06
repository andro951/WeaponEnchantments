using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Configs;
using WeaponEnchantments.Common.Utility;
using static androLib.Common.EnchantingRarity;
using androLib.Common.Utility;
using androLib.Items;
using WeaponEnchantments.Content.NPCs;
using WeaponEnchantments.Debuffs;

namespace WeaponEnchantments.Items
{
	[Autoload(false)]
	public class CursedEssence : WEModItem, ISoldByNPC {
		private int entitySize = 20;
		public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
		public Color glowColor = TierColors[Enchantment.CursedTier];
		public virtual float SellPriceModifier => (float)Math.Pow(2, tierNames.Length - Enchantment.CursedTier) * 10f;
		public override List<WikiTypeID> WikiItemTypes => new() { WikiTypeID.CursedEssence, WikiTypeID.CraftingMaterial };
		public override Type GroupingType => typeof(EnchantmentEssence);
		public override int CreativeItemSacrifice => 25;
		public override bool CanBeStoredInEnchantmentStorage => true;

		public override string Artist => "andro951";
		public override string Designer => "andro951";

		public Func<int> SoldByNPCNetID => ModContent.NPCType<Witch>;
		public virtual SellCondition SellCondition => SellCondition.Always;

		public const int TicksPerFrame = 17;
		public const int AnimationFrames = 16;
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();
			int type = Item.type;
			Main.RegisterItemAnimation(type, new DrawAnimationVertical(TicksPerFrame, AnimationFrames));
			ItemID.Sets.AnimatesAsSoul[type] = true;
			ItemID.Sets.ItemIconPulse[type] = true;
			ItemID.Sets.ItemNoGravity[type] = true;
			EnchantmentEssence.values[Enchantment.CursedTier] = (float)(EnchantmentEssence.valueMult * Math.Pow(EnchantmentEssence.valuePower, Enchantment.CursedTier))/10000f;
		}
		public override void ModifyTooltips(List<TooltipLine> tooltips) {
			int cursedEssenceCount = CurseAttractionNPC.PlayerCursedEssence[Main.myPlayer];
			CurseAttractionNPC.GetSpawnRateAndMaxSpawnsMultipliers(Main.LocalPlayer, out double spawnRateMult, out double maxSpawnsMult);
			double cursedSpawnChance = CurseAttractionNPC.GetCursedSpawnChance(Main.LocalPlayer.position);
			tooltips.Add(new(Mod, "cursedEssenceTooltip", Name.Lang_WE(L_ID1.Tooltip, L_ID2.ItemTooltip, new object[] { cursedEssenceCount, ((float)spawnRateMult).S(2), ((float)maxSpawnsMult).S(2), ((float)cursedSpawnChance).PercentString() })));
        }
		public override void PostUpdate() {
			float intensity = 0.5f;
			Lighting.AddLight(Item.Center, glowColor.ToVector3() * intensity * Main.essScale);
		}
		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI) {
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
					texture.Height / AnimationFrames
				) * 0.5f,
				scale,
				SpriteEffects.None,
				0f
			);
		}
		public override void SetDefaults() {
			Item.value = (int)EnchantmentEssence.values[Enchantment.CursedTier];
			Item.maxStack = EnchantmentEssence.MAX_STACK;
			Item.width = entitySize;
			Item.height = entitySize;
			Item.rare = GetRarityFromTier(Enchantment.CursedTier);
		}

		public override void AddRecipes() {
			Recipe recipe = CreateRecipe();
			recipe.createItem = new(ModContent.ItemType<EnchantmentEssenceEpic>());
			recipe.AddIngredient(Type);
			int num = 20;
			recipe.AddIngredient(ItemID.PurificationPowder, num);
			recipe.AddTile(Mod, EnchantingTableItem.enchantingTableNames[3] + "EnchantingTable");
			recipe.Register();
		}
	}
}
