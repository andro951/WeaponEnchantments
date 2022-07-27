using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
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
		public static string[] rarity = new string[5] { "Basic", "Common", "Rare", "SuperRare", "UltraRare" };
		public static int[] IDs = new int[rarity.Length];
		public const int maxStack = 9999;
		public static float[] values = new float[rarity.Length];
		public static float[] xpPerEssence = new float[rarity.Length];
		public static float valuePerXP;
		public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');

		public virtual string Artist { private set; get; } = "Zorutan";
		public virtual string Designer { private set; get; } = "andro951";

		public abstract Color glowColor { get; }		// The color of the essence's glow. Alpha is brightness (Exponentially brighter with each point).
		public abstract int animationFrames { get; }	// The amount of frames of the essence animation
		public abstract int entitySize { get; }         // The entity's hitbox size in the world

		public override void SetStaticDefaults()
        {
			Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, animationFrames));
			ItemID.Sets.AnimatesAsSoul[Item.type] = true; // Makes the item have an animation while in world (not held.).
			ItemID.Sets.ItemIconPulse[Item.type] = true; // The item pulses while in the player's inventory
			ItemID.Sets.ItemNoGravity[Item.type] = true; // Makes the item have no gravity

			for (int i = 0; i < rarity.Length; i++)
            {
				values[i] = (float)(25 * Math.Pow(8, i));
				xpPerEssence[i] = (float)(400 * Math.Pow(4, i));
			}
			valuePerXP = (values[rarity.Length - 1] / xpPerEssence[rarity.Length - 1]);
			GetDefaults();
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
			Tooltip.SetDefault(Enchantment.displayRarity[essenceRarity].AddSpaces() + " material for crafting and upgrading enchantments.\nCan be converted to " + xpPerEssence[essenceRarity] + " experience in an enchanting table.");
			if (!WEMod.clientConfig.UseOldRarityNames)
				DisplayName.SetDefault(UtilityMethods.AddSpaces(Name.Substring(0, Name.IndexOf(rarity[essenceRarity])) + Enchantment.displayRarity[essenceRarity]));

			LogUtilities.UpdateContributorsList(this);
		}

		public override void PostUpdate()
		{
			// Makes this item glow when thrown out of inventory
			float glowIntensity = glowColor.A / 255f;												// Turn the alpha of the color into it's brightness
			Lighting.AddLight(Item.Center, glowColor.ToVector3() * glowIntensity * Main.essScale);	
		}

		private void GetDefaults()
        {
			for (int i = 0; i < rarity.Length; i++)
			{
				if (rarity[i] == Name.Substring(Name.IndexOf("Essence") + 7))
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
		}

		public override void AddRecipes()
		{
			for (int i = 0; i < rarity.Length; i++)
			{
				if (essenceRarity > -1)
				{
					//Dont sell basic/common/rare with NPC!!!
					Recipe recipe = CreateRecipe();
					if (essenceRarity > 0)
					{
						recipe.AddIngredient(Mod, "EnchantmentEssence" + rarity[essenceRarity - 1], 8 - i);
						recipe.AddTile(Mod, EnchantingTable.enchantingTableNames[i] + "EnchantingTable"); //Put this inside if(essenceRarity >0) when not testing
						recipe.Register(); //Put this inside if(essenceRarity >0) when not testing
					}
					

					if (essenceRarity < rarity.Length - 1)
					{
						recipe = CreateRecipe();
						recipe.AddIngredient(Mod, "EnchantmentEssence" + rarity[essenceRarity + 1], 1);
						recipe.createItem.stack = 2 + i / 2;
						recipe.AddTile(Mod, EnchantingTable.enchantingTableNames[i] + "EnchantingTable");
						recipe.Register();
					}
					IDs[essenceRarity] = this.Type;
				}
			}
		}
    }
	public class EnchantmentEssenceBasic: EnchantmentEssence {
		public override int animationFrames => 8;
		public override Color glowColor => Color.FromNonPremultiplied(0x18, 0xA9, 0x37, 0x56);	// #18A937
		public override int entitySize => 12;
    }
	public class EnchantmentEssenceCommon : EnchantmentEssence {
		public override int animationFrames => 8;
		public override Color glowColor => Color.FromNonPremultiplied(0x18, 0x44, 0xA9, 0x5E);	// #1844A9
		public override int entitySize => 16;
	}
	public class EnchantmentEssenceRare : EnchantmentEssence {
		public override int animationFrames => 4;
		public override Color glowColor => Color.FromNonPremultiplied(0xA9, 0x18, 0x88, 0x66);	// #A91888
		public override int entitySize => 20;
	}
	public class EnchantmentEssenceSuperRare : EnchantmentEssence {
		public override int animationFrames => 8;
		public override Color glowColor => Color.FromNonPremultiplied(0xA9, 0x21, 0x18, 0x6E);   // #A92118
		public override int entitySize => 24;
	}
	public class EnchantmentEssenceUltraRare : EnchantmentEssence {
		public override int animationFrames => 4;
		public override Color glowColor => Color.FromNonPremultiplied(0xA9, 0x68, 0x18, 0x76);	// #A96818
		public override int entitySize => 28;
	}
}
