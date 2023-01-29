using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using static WeaponEnchantments.Common.Globals.EnchantedWeapon;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects {
    public class FishingPower : StatEffect, IVanillaStat {
        public FishingPower(DifficultyStrength additive = null, DifficultyStrength multiplicative = null, DifficultyStrength flat = null, DifficultyStrength @base = null) : base(additive, multiplicative, flat, @base) {

        }
        public FishingPower(EStatModifier eStatModifier) : base(eStatModifier) { }
        public override EnchantmentEffect Clone() {
            return new FishingPower(EStatModifier.Clone());
        }


        public override string TooltipValue => EStatModifier.SignTooltip;
        public override EnchantmentStat statName => EnchantmentStat.FishingPower;
    }
}
