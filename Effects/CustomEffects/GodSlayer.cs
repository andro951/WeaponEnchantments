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
	public class GodSlayer: StatEffect, INonVanillaStat
    {
        public GodSlayer(DifficultyStrength additive = null, DifficultyStrength multiplicative = null, DifficultyStrength flat = null, DifficultyStrength @base = null) : base(additive, multiplicative, flat, @base) {

        }

		public override string Tooltip => $"{EStatModifier.PercentMult100Tooltip} {DisplayName} (Bonus true damage based on enemy max hp)\n(Bonus damage not affected by LifeSteal)";
		public override EnchantmentStat statName => EnchantmentStat.GodSlayer;
	}
}
