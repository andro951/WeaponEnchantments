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
	public class FishingEnemySpawnChance : StatEffect, INonVanillaStat
    {
        public FishingEnemySpawnChance(DifficultyStrength additive = null, DifficultyStrength multiplicative = null, DifficultyStrength flat = null, DifficultyStrength @base = null) : base(additive, multiplicative, flat, @base) {

		}
		public FishingEnemySpawnChance(EStatModifier eStatModifier) : base(eStatModifier) { }
		public override EnchantmentEffect Clone() {
			return new FishingEnemySpawnChance(EStatModifier.Clone());
		}

		//public override string Tooltip => $"{EStatModifier.PercentMult100Tooltip} {DisplayName} (Reduced by 5x during the day.  Affected by Chum Caster.  Can also spawn Duke Fishron)";
		public override IEnumerable<object> TooltipArgs => new object[] { $"{EStatModifier.PercentMult100Tooltip} {DisplayName}" };
		public override string Tooltip => StandardTooltip;
		public override EnchantmentStat statName => EnchantmentStat.FishingEnemySpawnChance;
	}
}
