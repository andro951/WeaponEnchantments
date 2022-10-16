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

namespace WeaponEnchantments.Effects
{
	public class EnemySpawnRate: StatEffect, INonVanillaStat
    {
        public EnemySpawnRate(DifficultyStrength additive = null, DifficultyStrength multiplicative = null, DifficultyStrength flat = null, DifficultyStrength @base = null) : base(additive, multiplicative, flat, @base) {

		}
		public EnemySpawnRate(EStatModifier eStatModifier) : base(eStatModifier) { }
		public override EnchantmentEffect Clone() {
			return new EnemySpawnRate(EStatModifier.Clone());
		}

		public override EnchantmentStat statName => EnchantmentStat.EnemySpawnRate;
		public override string Tooltip => EffectStrength > 1f ? StandardTooltip : base.Tooltip;
		public override IEnumerable<object> TooltipArgs => new object[] { base.Tooltip };
	}
}
