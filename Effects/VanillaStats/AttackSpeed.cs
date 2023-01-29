using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects {
    public class AttackSpeed : ClassedStatEffect, IVanillaStat {
        public AttackSpeed(DifficultyStrength additive = null, DifficultyStrength multiplicative = null, DifficultyStrength flat = null, DifficultyStrength @base = null, DamageClass dc = null) : base(additive, multiplicative, flat, @base, dc) {

		}
		public AttackSpeed(EStatModifier eStatModifier, DamageClass dc) : base(eStatModifier, dc) { }
		public override EnchantmentEffect Clone() {
			return new AttackSpeed(EStatModifier.Clone(), damageClass);
		}

		public override EnchantmentStat statName => EnchantmentStat.AttackSpeed;
		public override IEnumerable<object> TooltipArgs => new object[] { base.Tooltip };
		public override string Tooltip => StandardTooltip;
	}
}
