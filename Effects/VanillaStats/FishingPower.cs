using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using static WeaponEnchantments.Common.Globals.EnchantedWeapon;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects {
    public class FishingPower : StatEffect, IVanillaStat {
        public FishingPower(DifficultyStrength additive = null, DifficultyStrength multiplicative = null, DifficultyStrength flat = null, DifficultyStrength @base = null) : base(additive, multiplicative, flat, @base) {
            
        }

        public override string Tooltip => $"{EStatModifier.SignTooltip} {DisplayName}";
        public override EnchantmentStat statName => EnchantmentStat.FishingPower;
    }
}
