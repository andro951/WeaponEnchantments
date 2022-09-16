using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using static WeaponEnchantments.Common.Globals.EnchantedWeapon;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects {
    public class DamageReduction : StatEffect, INonVanillaStat {
        public DamageReduction(DifficultyStrength additive = null, DifficultyStrength multiplicative = null, DifficultyStrength flat = null, DifficultyStrength @base = null) : base(additive, multiplicative, flat, @base, combineModeID: CombineModeID.MultiplicativePartOf1) {

		}
		public DamageReduction(EStatModifier eStatModifier) : base(eStatModifier) { }
		public override EnchantmentEffect Clone() {
			return new DamageReduction(EStatModifier.Clone());
		}

		public override EnchantmentStat statName => EnchantmentStat.DamageReduction;
		public override string Tooltip => $"{EStatModifier.PercentMult100Tooltip} {DisplayName}";
	}
}