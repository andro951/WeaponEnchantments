using System;
using System.Collections.Generic;
using androLib;
using Terraria;
using Terraria.ID;
using WeaponEnchantments.Effects;
using WeaponEnchantments.Common.Utility;
using androLib.Common.Utility;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Configs;

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class StatusEffectEnchantment : Enchantment
	{
		public override int StrengthGroup => 13;
		public override float ScalePercent => 0.1f;
		public override int LowestCraftableTier => 0;
		public override float CapacityCostMultiplier => CapacityCostUtility;
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

	#region Vanilla status effects
	#region On Fire!
	public abstract class OnFireEnchantment : StatusEffectEnchantment {
		protected override string TypeName => "OnFire";
		protected override string NamePrefix => "Enchantments/";
		
		public override short StatusEffect => BuffID.OnFire;
		public override Tuple<int, int> CraftingIngredient => new Tuple<int, int>(ItemID.Gel, 33);
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<EnchantmentToggle>().OnFire;
		}
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
	[Autoload(false)]
	public class OnFireEnchantmentCursed : OnFireEnchantment { }
	#endregion

	#region Poison
	public abstract class PoisonEnchantment : StatusEffectEnchantment
	{
		protected override string TypeName => "Poison";
		protected override string NamePrefix => "Enchantments/";

		public override short StatusEffect => BuffID.Poisoned;
		public override Tuple<int, int> CraftingIngredient => new Tuple<int, int>(ItemID.DartTrap, 1);
		public override string Artist => "Princess of Evil";
		public override string ArtModifiedBy => "andro951";
		public override string Designer => "andro951";
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<EnchantmentToggle>().Poison;
		}
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
	[Autoload(false)]
	public class PoisonEnchantmentCursed : PoisonEnchantment { }
	#endregion

	#region Frostburn
	public abstract class FrostburnEnchantment : StatusEffectEnchantment
	{
		protected override string TypeName => "Frostburn";
		protected override string NamePrefix => "Enchantments/";
		
		public override short StatusEffect => BuffID.Frostburn;
		public override Tuple<int, int> CraftingIngredient => new Tuple<int, int>(ItemID.IceBlock, 33);
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<EnchantmentToggle>().Frostburn;
		}
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
	[Autoload(false)]
	public class FrostburnEnchantmentCursed : FrostburnEnchantment { }
	#endregion

	#region Cursed Inferno
	public abstract class CursedInfernoEnchantment : StatusEffectEnchantment
	{
		protected override string TypeName => "CursedInferno";
		protected override string NamePrefix => "Enchantments/";
        		
		public override short StatusEffect => BuffID.CursedInferno;
		public override Tuple<int, int> CraftingIngredient => new Tuple<int, int>(ItemID.CursedFlame, 3);
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<EnchantmentToggle>().CursedInferno;
		}
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
	[Autoload(false)]
	public class CursedInfernoEnchantmentCursed : CursedInfernoEnchantment { }
	#endregion
	
	#region Ichor
	public abstract class IchorEnchantment : StatusEffectEnchantment
	{
		protected override string TypeName => "Ichor";
		protected override string NamePrefix => "Enchantments/";
        		
		public override short StatusEffect => BuffID.Ichor;
		public override Tuple<int, int> CraftingIngredient => new Tuple<int, int>(ItemID.Ichor, 3);
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<EnchantmentToggle>().Ichor;
		}
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
	[Autoload(false)]
	public class IchorEnchantmentCursed : IchorEnchantment { }
	#endregion
	
	#region Venom
	public abstract class VenomEnchantment : StatusEffectEnchantment
	{
		protected override string TypeName => "Venom";
		protected override string NamePrefix => "Enchantments/";
		
		public override short StatusEffect => BuffID.Venom;
		public override Tuple<int, int> CraftingIngredient => new Tuple<int, int>(ItemID.VialofVenom, 3);
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<EnchantmentToggle>().Venom;
		}
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
	[Autoload(false)]
	public class VenomEnchantmentCursed : VenomEnchantment { }
	#endregion

	#region Daybroken
	public abstract class DaybreakEnchantment : StatusEffectEnchantment
	{
		protected override string TypeName => "Daybreak";
		protected override string NamePrefix => "Enchantments/";
		
		public override float CapacityCostMultiplier => CapacityCostNormal;

		public override short StatusEffect => BuffID.Daybreak;
		public override Tuple<int, int> CraftingIngredient => new Tuple<int, int>(ItemID.FragmentSolar, 6);
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<EnchantmentToggle>().Daybreak;
		}
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
	[Autoload(false)]
	public class DaybreakEnchantmentCursed : DaybreakEnchantment { }
	#endregion
	
	#region Shadowflame
	public abstract class ShadowflameEnchantment : StatusEffectEnchantment
	{
		protected override string TypeName => "Shadowflame";
		protected override string NamePrefix => "Enchantments/";
		
		public override short StatusEffect => BuffID.ShadowFlame;
		public override int LowestCraftableTier => 1;
		public override string Artist => "Princess of Evil";
		public override string ArtModifiedBy => "andro951";
		public override string Designer => "andro951";
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<EnchantmentToggle>().Shadowflame;
		}
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
	[Autoload(false)]
	public class ShadowflameEnchantmentCursed : ShadowflameEnchantment { }
	#endregion
	#endregion
	
	#region Modded status effects
	#region Thorium mod
	#region Elemental Decay
	[JITWhenModsEnabled("ThoriumMod")]
	public abstract class ThoriumElementalDecayEnchantment : StatusEffectEnchantment
	{
		protected override string TypeName => "ElementalDecay";
		protected override string NamePrefix => "ModSupport/Thorium/";
		public override string Texture => $"WeaponEnchantments/Items/Sprites/{NamePrefix}{TypeName}/{Name.Replace("Thorium"+TypeName+"Enchantment", string.Empty)}";

		public override float CapacityCostMultiplier => CapacityCostNormal;
		public override short StatusEffect => (short)ModContent.Find<ModBuff>("ThoriumMod/MagickStaffDebuff").Type;
		public override int LowestCraftableTier => 1;
		public override List<int> IngredientEnchantments =>
		[
			ItemID.Ruby,
			ItemID.Amber,
			ItemID.Topaz,
			ItemID.Emerald,
			(short)ModContent.Find<ModItem>("ThoriumMod/Aquamarine").Type,
			ItemID.Sapphire,
			ItemID.Amethyst,
			(short)ModContent.Find<ModItem>("ThoriumMod/Opal").Type,
			ItemID.Diamond
		];
		
		public override string Artist => "Pixus";
		public override string ArtModifiedBy => null;
		public override string Designer => "Pixus";
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<EnchantmentToggle>().ThoriumElementalDecay && AndroMod.thoriumEnabled;
		}
	}
	[Autoload(false)]
	public class ThoriumElementalDecayEnchantmentBasic : ThoriumElementalDecayEnchantment
	{
		public override SellCondition SellCondition => SellCondition.PostEaterOfWorldsOrBrainOfCthulhu;
	}
	[Autoload(false)]
	public class ThoriumElementalDecayEnchantmentCommon : ThoriumElementalDecayEnchantment { }
	[Autoload(false)]
	public class ThoriumElementalDecayEnchantmentRare : ThoriumElementalDecayEnchantment { }
	[Autoload(false)]
	public class ThoriumElementalDecayEnchantmentEpic : ThoriumElementalDecayEnchantment { }
	[Autoload(false)]
	public class ThoriumElementalDecayEnchantmentLegendary : ThoriumElementalDecayEnchantment { }
	[Autoload(false)]
	public class ThoriumElementalDecayEnchantmentCursed : ThoriumElementalDecayEnchantment { }
	#endregion
	#endregion
	#endregion
}
