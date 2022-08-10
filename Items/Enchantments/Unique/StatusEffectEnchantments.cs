using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using static WeaponEnchantments.Common.EnchantingRarity;

namespace WeaponEnchantments.Items.Enchantments.Unique
{
	public abstract class StatusEffectEnchantment : Enchantment
	{
		public override int StrengthGroup => 9;
		public override float ScalePercent => 0.2f / defaultEnchantmentStrengths[StrengthGroup].enchantmentTierStrength[tierNames.Length - 1];
		public override Dictionary<EItemType, float> AllowedList => new Dictionary<EItemType, float>() {
			{ EItemType.Weapon, 1f }
		};
		public override int LowestCraftableTier => 0;
		public override float CapacityCostMultiplier => 1;

		public virtual int StatusEffect => BuffID.OnFire;

		public override string Texture => $"WeaponEnchantments/Items/Sprites/StatusEffects/{Name}";
		
		public override void GetMyStats() {
			Debuff.Add(StatusEffect, BuffDuration);
			AddEStat("Damage", 0f, EnchantmentStrength);
		}

		public abstract Recipe AddToRecipe(Recipe recipe);

		// Dumbest way to do this, but it works.
		public override void AddRecipes() {
			for (int i = EnchantmentTier; i < tierNames.Length; i++) {
				//Lowest Craftable Tier
				if (EnchantmentTier < LowestCraftableTier)
					continue;

				Recipe recipe;

				for (int j = LowestCraftableTier; j <= EnchantmentTier; j++) {
					recipe = AddToRecipe(CreateRecipe());
					
					//Essence
					for (int k = j; k <= EnchantmentTier; k++) {
						int essenceNumber = Utility ? 5 : 10;
						recipe.AddIngredient(Mod, "EnchantmentEssence" + tierNames[k], essenceNumber);
					}

					//Enchantment
					if (j > 0) {
						recipe.AddIngredient(Mod, EnchantmentTypeName + "Enchantment" + tierNames[j - 1], 1);
					}
						
					//Containment
					if (EnchantmentTier < 3) {
						recipe.AddIngredient(Mod, ContainmentItem.sizes[EnchantmentTier] + "Containment", 1);
					}
					else if (j < 3) {
						recipe.AddIngredient(Mod, ContainmentItem.sizes[2] + "Containment", 1);
					}

					//Gems
					if (EnchantmentTier == 3) {
						recipe.AddRecipeGroup("WeaponEnchantments:CommonGems", 2);
					}
					if (EnchantmentTier == 4) {
						recipe.AddRecipeGroup("WeaponEnchantments:RareGems");
					}

					//Enchanting Table
					recipe.AddTile(Mod, EnchantingTableItem.enchantingTableNames[i] + "EnchantingTable");

					recipe.Register();
				}
			}
		}

		public override string Artist => "Princess of Evil";
		public override string Designer => "Princess of Evil";
	}


	public abstract class OnFireEnchantment : StatusEffectEnchantment
	{
		public override int StatusEffect => BuffID.OnFire;
		public override Recipe AddToRecipe(Recipe recipe)
		{
			recipe.AddIngredient(ItemID.Gel, 33);
			return recipe;
		}
	}
	public class OnFireEnchantmentBasic : OnFireEnchantment { }
	public class OnFireEnchantmentCommon : OnFireEnchantment { }
	public class OnFireEnchantmentRare : OnFireEnchantment { }
	public class OnFireEnchantmentSuperRare : OnFireEnchantment { }
	public class OnFireEnchantmentUltraRare : OnFireEnchantment { }	

	public abstract class FrostburnEnchantment : StatusEffectEnchantment
	{
		public override int StatusEffect => BuffID.Frostburn;
		public override Recipe AddToRecipe(Recipe recipe)
		{
			recipe.AddIngredient(ItemID.IceBlock, 33);
			return recipe;
		}
	}
	public class FrostburnEnchantmentBasic : FrostburnEnchantment { }
	public class FrostburnEnchantmentCommon : FrostburnEnchantment { }
	public class FrostburnEnchantmentRare : FrostburnEnchantment { }
	public class FrostburnEnchantmentSuperRare : FrostburnEnchantment { }
	public class FrostburnEnchantmentUltraRare : FrostburnEnchantment { }


	public abstract class CursedInfernoEnchantment : StatusEffectEnchantment
	{
		public override int StatusEffect => BuffID.CursedInferno;
		public override Recipe AddToRecipe(Recipe recipe)
		{
			recipe.AddIngredient(ItemID.CursedFlames, 3);
			return recipe;
		}
	}
	public class CursedInfernoEnchantmentBasic : CursedInfernoEnchantment { }
	public class CursedInfernoEnchantmentCommon : CursedInfernoEnchantment { }
	public class CursedInfernoEnchantmentRare : CursedInfernoEnchantment { }
	public class CursedInfernoEnchantmentSuperRare : CursedInfernoEnchantment { }
	public class CursedInfernoEnchantmentUltraRare : CursedInfernoEnchantment { }

	public abstract class IchorEnchantment : StatusEffectEnchantment
	{
		public override int StatusEffect => BuffID.Ichor;
		public override Recipe AddToRecipe(Recipe recipe)
		{
			recipe.AddIngredient(ItemID.Ichor, 3);
			return recipe;
		}
	}
	public class IchorEnchantmentBasic : IchorEnchantment { }
	public class IchorEnchantmentCommon : IchorEnchantment { }
	public class IchorEnchantmentRare : IchorEnchantment { }
	public class IchorEnchantmentSuperRare : IchorEnchantment { }
	public class IchorEnchantmentUltraRare : IchorEnchantment { }

	public abstract class VenomEnchantment : StatusEffectEnchantment
	{
		public override int StatusEffect => BuffID.Venom;
		public override Recipe AddToRecipe(Recipe recipe)
		{
			recipe.AddIngredient(ItemID.VialofVenom, 3);
			return recipe;
		}
	}
	public class VenomEnchantmentBasic : VenomEnchantment { }
	public class VenomEnchantmentCommon : VenomEnchantment { }
	public class VenomEnchantmentRare : VenomEnchantment { }
	public class VenomEnchantmentSuperRare : VenomEnchantment { }
	public class VenomEnchantmentUltraRare : VenomEnchantment { }


	public abstract class DaybreakEnchantment : StatusEffectEnchantment
	{
		public override float CapacityCostMultiplier => 2;

		public override int StatusEffect => BuffID.Daybreak;
		public override Recipe AddToRecipe(Recipe recipe)
		{
			recipe.AddIngredient(ItemID.FragmentSolar, 6);
			return recipe;
		}
	}
	public class DaybreakEnchantmentBasic : DaybreakEnchantment { }
	public class DaybreakEnchantmentCommon : DaybreakEnchantment { }
	public class DaybreakEnchantmentRare : DaybreakEnchantment { }
	public class DaybreakEnchantmentSuperRare : DaybreakEnchantment { }
	public class DaybreakEnchantmentUltraRare : DaybreakEnchantment { }
}
