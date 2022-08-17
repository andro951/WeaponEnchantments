using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects.EnchantStats
{
	public class DamageAfterDefenses : StatEffect, IClassedEffect, IModifyHitDamage, INonVanillaStat
    {
        public DamageAfterDefenses(float additive = 0f, float multiplicative = 1f, float flat = 0f, float @base = 0f, DamageClass dc = null) : base(additive, multiplicative, flat, @base) {
            damageClass = dc != null ? dc : DamageClass.Generic;
            DisplayName = $"{damageClass.DisplayName} Damage (Bonus is applied after defenses. Not visible in weapon tooltip)";
        }

        public DamageClass damageClass { get; set; }
        public override string DisplayName { get; }
        public override EnchantmentStat statType => EnchantmentStat.DamageAfterDefenses;

		public void ModifyHitDamage(ref float damageMultiplier, Item item, NPC target, ref int damage, ref float knockback, ref bool crit, int hitDirection, Projectile projectile) {
            EStatModifier.ApplyTo(ref damageMultiplier);
		}
	}
}
