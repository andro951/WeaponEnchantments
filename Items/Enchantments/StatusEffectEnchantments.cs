using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using WeaponEnchantments.Effects;
using WeaponEnchantments.Common.Utility;

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class StatusEffectEnchantment : Enchantment
	{
		public override int StrengthGroup => 13;
		public override float ScalePercent => 0.1f;
		public override int LowestCraftableTier => 0;
		public override float CapacityCostMultiplier => 1;
        public override bool Max1 => true;
		public abstract short StatusEffect { get; }
		public virtual Tuple<int, int> CraftingIngredient { get; } = null;

		public override string Texture => $"WeaponEnchantments/Items/Sprites/StatusEffects/{Name}";
		
		public override void GetMyStats() {
			Effects = new() {
				new OnHitTargetDebuffEffect(StatusEffect, new Time(BuffDuration)),
				new DamageAfterDefenses(multiplicative: EnchantmentStrengthData)
			};

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Weapon, 1f }
			};
		}

		protected override void EditTier0Recipies(Recipe recipe) {
			if (CraftingIngredient != null) {
				int type = CraftingIngredient.Item1;
				int stack = CraftingIngredient.Item2;
				recipe.AddIngredient(type, stack);
			}
		}

		public override string Artist => "Princess of Evil";
		public override string Designer => "Princess of Evil";
	}

	//DODO: Split into seperate files
	public abstract class OnFireEnchantment : StatusEffectEnchantment {
		public override short StatusEffect => BuffID.OnFire;
		public override Tuple<int, int> CraftingIngredient => new Tuple<int, int>(ItemID.Gel, 33);
	}
	public class OnFireEnchantmentBasic : OnFireEnchantment { }
	public class OnFireEnchantmentCommon : OnFireEnchantment { }
	public class OnFireEnchantmentRare : OnFireEnchantment { }
	public class OnFireEnchantmentSuperRare : OnFireEnchantment { }
	public class OnFireEnchantmentUltraRare : OnFireEnchantment { }	

	public abstract class FrostburnEnchantment : StatusEffectEnchantment
	{
		public override short StatusEffect => BuffID.Frostburn;
		public override Tuple<int, int> CraftingIngredient => new Tuple<int, int>(ItemID.IceBlock, 33);
	}
	public class FrostburnEnchantmentBasic : FrostburnEnchantment { }
	public class FrostburnEnchantmentCommon : FrostburnEnchantment { }
	public class FrostburnEnchantmentRare : FrostburnEnchantment { }
	public class FrostburnEnchantmentSuperRare : FrostburnEnchantment { }
	public class FrostburnEnchantmentUltraRare : FrostburnEnchantment { }


	public abstract class CursedInfernoEnchantment : StatusEffectEnchantment
	{
		public override short StatusEffect => BuffID.CursedInferno;
		public override Tuple<int, int> CraftingIngredient => new Tuple<int, int>(ItemID.CursedFlame, 3);
	}
	public class CursedInfernoEnchantmentBasic : CursedInfernoEnchantment { }
	public class CursedInfernoEnchantmentCommon : CursedInfernoEnchantment { }
	public class CursedInfernoEnchantmentRare : CursedInfernoEnchantment { }
	public class CursedInfernoEnchantmentSuperRare : CursedInfernoEnchantment { }
	public class CursedInfernoEnchantmentUltraRare : CursedInfernoEnchantment { }

	public abstract class IchorEnchantment : StatusEffectEnchantment
	{
		public override short StatusEffect => BuffID.Ichor;
		public override Tuple<int, int> CraftingIngredient => new Tuple<int, int>(ItemID.Ichor, 3);
	}
	public class IchorEnchantmentBasic : IchorEnchantment { }
	public class IchorEnchantmentCommon : IchorEnchantment { }
	public class IchorEnchantmentRare : IchorEnchantment { }
	public class IchorEnchantmentSuperRare : IchorEnchantment { }
	public class IchorEnchantmentUltraRare : IchorEnchantment { }

	public abstract class VenomEnchantment : StatusEffectEnchantment
	{
		public override short StatusEffect => BuffID.Venom;
		public override Tuple<int, int> CraftingIngredient => new Tuple<int, int>(ItemID.VialofVenom, 3);
	}
	public class VenomEnchantmentBasic : VenomEnchantment { }
	public class VenomEnchantmentCommon : VenomEnchantment { }
	public class VenomEnchantmentRare : VenomEnchantment { }
	public class VenomEnchantmentSuperRare : VenomEnchantment { }
	public class VenomEnchantmentUltraRare : VenomEnchantment { }


	public abstract class DaybreakEnchantment : StatusEffectEnchantment
	{
		public override float CapacityCostMultiplier => 2;

		public override short StatusEffect => BuffID.Daybreak;
		public override Tuple<int, int> CraftingIngredient => new Tuple<int, int>(ItemID.FragmentSolar, 6);
	}
	public class DaybreakEnchantmentBasic : DaybreakEnchantment { }
	public class DaybreakEnchantmentCommon : DaybreakEnchantment { }
	public class DaybreakEnchantmentRare : DaybreakEnchantment { }
	public class DaybreakEnchantmentSuperRare : DaybreakEnchantment { }
	public class DaybreakEnchantmentUltraRare : DaybreakEnchantment { }
}
