using Terraria.ModLoader;
using static WeaponEnchantments.Common.Globals.EnchantedWeapon;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects {
    public class LifeSteal : StatEffect, INonVanillaStat {
        public LifeSteal(float additive = 0f, float multiplicative = 1f, float flat = 0f, float @base = 0f) : base((byte)WeaponStat.LifeSteal, additive, multiplicative, flat, @base) { }

        public override PlayerStat statName => PlayerStat.LifeSteal;
        public override string DisplayName { get; } = "Life Steal";

        protected override string modifierToString() {
            return EStatModifier.SmartTooltip;
        }
    }
}
