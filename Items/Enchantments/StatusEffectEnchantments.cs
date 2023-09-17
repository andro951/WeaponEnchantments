using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using WeaponEnchantments.Effects;
using WeaponEnchantments.Common.Utility;
using androLib.Items;
using androLib.Common.Utility;
using Terraria.ModLoader;

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
		
		public override void GetMyStats() {
			Effects = new() {
				new BuffEffect(StatusEffect,BuffStyle.OnHitEnemyDebuff, BuffDuration),
				new DamageAfterDefenses(multiplicative: EnchantmentStrengthData)
			};

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Weapons, 1f }
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
		public override string ArtModifiedBy => null;
		public override string Designer => "Princess of Evil";
	}

	public abstract class OnFireEnchantment : StatusEffectEnchantment {
		public override short StatusEffect => BuffID.OnFire;
		public override Tuple<int, int> CraftingIngredient => new Tuple<int, int>(ItemID.Gel, 33);
	}
	[Autoload(false)]
	public class OnFireEnchantmentBasic : OnFireEnchantment { }
	[Autoload(false)]
	public class OnFireEnchantmentCommon : OnFireEnchantment { }
	[Autoload(false)]
	public class OnFireEnchantmentRare : OnFireEnchantment { }
	[Autoload(false)]
	public class OnFireEnchantmentEpic : OnFireEnchantment { }
	[Autoload(false)]
	public class OnFireEnchantmentLegendary : OnFireEnchantment { }

	public abstract class PoisonEnchantment : StatusEffectEnchantment
	{
		public override short StatusEffect => BuffID.Poisoned;
		public override Tuple<int, int> CraftingIngredient => new Tuple<int, int>(ItemID.DartTrap, 1);
		public override string Artist => "Princess of Evil";
		public override string ArtModifiedBy => "andro951";
		public override string Designer => "andro951";
	}
	[Autoload(false)]
	public class PoisonEnchantmentBasic : PoisonEnchantment
	{
		public override SellCondition SellCondition => SellCondition.AnyTime;
	}
	[Autoload(false)]
	public class PoisonEnchantmentCommon : PoisonEnchantment { }
	[Autoload(false)]
	public class PoisonEnchantmentRare : PoisonEnchantment { }
	[Autoload(false)]
	public class PoisonEnchantmentEpic : PoisonEnchantment { }
	[Autoload(false)]
	public class PoisonEnchantmentLegendary : PoisonEnchantment { }

	public abstract class FrostburnEnchantment : StatusEffectEnchantment
	{
		public override short StatusEffect => BuffID.Frostburn;
		public override Tuple<int, int> CraftingIngredient => new Tuple<int, int>(ItemID.IceBlock, 33);
	}
	[Autoload(false)]
	public class FrostburnEnchantmentBasic : FrostburnEnchantment { }
	[Autoload(false)]
	public class FrostburnEnchantmentCommon : FrostburnEnchantment { }
	[Autoload(false)]
	public class FrostburnEnchantmentRare : FrostburnEnchantment { }
	[Autoload(false)]
	public class FrostburnEnchantmentEpic : FrostburnEnchantment { }
	[Autoload(false)]
	public class FrostburnEnchantmentLegendary : FrostburnEnchantment { }


	public abstract class CursedInfernoEnchantment : StatusEffectEnchantment
	{
		public override short StatusEffect => BuffID.CursedInferno;
		public override Tuple<int, int> CraftingIngredient => new Tuple<int, int>(ItemID.CursedFlame, 3);
	}
	[Autoload(false)]
	public class CursedInfernoEnchantmentBasic : CursedInfernoEnchantment
	{
		public override SellCondition SellCondition => SellCondition.HardMode;
	}
	[Autoload(false)]
	public class CursedInfernoEnchantmentCommon : CursedInfernoEnchantment { }
	[Autoload(false)]
	public class CursedInfernoEnchantmentRare : CursedInfernoEnchantment { }
	[Autoload(false)]
	public class CursedInfernoEnchantmentEpic : CursedInfernoEnchantment { }
	[Autoload(false)]
	public class CursedInfernoEnchantmentLegendary : CursedInfernoEnchantment { }

	public abstract class IchorEnchantment : StatusEffectEnchantment
	{
		public override short StatusEffect => BuffID.Ichor;
		public override Tuple<int, int> CraftingIngredient => new Tuple<int, int>(ItemID.Ichor, 3);
	}
	[Autoload(false)]
	public class IchorEnchantmentBasic : IchorEnchantment
	{
		public override SellCondition SellCondition => SellCondition.HardMode;
	}
	[Autoload(false)]
	public class IchorEnchantmentCommon : IchorEnchantment { }
	[Autoload(false)]
	public class IchorEnchantmentRare : IchorEnchantment { }
	[Autoload(false)]
	public class IchorEnchantmentEpic : IchorEnchantment { }
	[Autoload(false)]
	public class IchorEnchantmentLegendary : IchorEnchantment { }

	public abstract class VenomEnchantment : StatusEffectEnchantment
	{
		public override short StatusEffect => BuffID.Venom;
		public override Tuple<int, int> CraftingIngredient => new Tuple<int, int>(ItemID.VialofVenom, 3);
	}
	[Autoload(false)]
	public class VenomEnchantmentBasic : VenomEnchantment
	{
		public override SellCondition SellCondition => SellCondition.HardMode;
	}
	[Autoload(false)]
	public class VenomEnchantmentCommon : VenomEnchantment { }
	[Autoload(false)]
	public class VenomEnchantmentRare : VenomEnchantment { }
	[Autoload(false)]
	public class VenomEnchantmentEpic : VenomEnchantment { }
	[Autoload(false)]
	public class VenomEnchantmentLegendary : VenomEnchantment { }


	public abstract class DaybreakEnchantment : StatusEffectEnchantment
	{
		public override float CapacityCostMultiplier => 2;

		public override short StatusEffect => BuffID.Daybreak;
		public override Tuple<int, int> CraftingIngredient => new Tuple<int, int>(ItemID.FragmentSolar, 6);
	}
	[Autoload(false)]
	public class DaybreakEnchantmentBasic : DaybreakEnchantment
	{
		public override SellCondition SellCondition => SellCondition.PostSolarTower;
	}
	[Autoload(false)]
	public class DaybreakEnchantmentCommon : DaybreakEnchantment { }
	[Autoload(false)]
	public class DaybreakEnchantmentRare : DaybreakEnchantment { }
	[Autoload(false)]
	public class DaybreakEnchantmentEpic : DaybreakEnchantment { }
	[Autoload(false)]
	public class DaybreakEnchantmentLegendary : DaybreakEnchantment { }

	public abstract class ShadowflameEnchantment : StatusEffectEnchantment
	{
		public override short StatusEffect => BuffID.ShadowFlame;
		public override int LowestCraftableTier => 1;
		public override string Artist => "Princess of Evil";
		public override string ArtModifiedBy => "andro951";
		public override string Designer => "andro951";
	}
	[Autoload(false)]
	public class ShadowflameEnchantmentBasic : ShadowflameEnchantment
	{
		public override SellCondition SellCondition => SellCondition.HardMode;
		public override List<DropData> NpcDropTypes => new() {
			new(NPCID.GoblinSummoner, chance: 0.5f)
		};

	}
	[Autoload(false)]
	public class ShadowflameEnchantmentCommon : ShadowflameEnchantment { }
	[Autoload(false)]
	public class ShadowflameEnchantmentRare : ShadowflameEnchantment { }
	[Autoload(false)]
	public class ShadowflameEnchantmentEpic : ShadowflameEnchantment { }
	[Autoload(false)]
	public class ShadowflameEnchantmentLegendary : ShadowflameEnchantment { }
}
