using Terraria.ModLoader;
using static WeaponEnchantments.Common.Globals.EnchantedWeapon;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects {
    public class LifeSteal : StatEffect, INonVanillaStat {
        public LifeSteal(float additive = 0f, float multiplicative = 1f, float flat = 0f, float @base = 0f) : base(additive, multiplicative, flat, @base) { }

        public override EnchantmentStat statType => EnchantmentStat.LifeSteal;
    }
}
