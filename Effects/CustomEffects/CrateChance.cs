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
	public class CrateChance : StatEffect, INonVanillaStat
    {
        public CrateChance(DifficultyStrength additive = null, DifficultyStrength multiplicative = null, DifficultyStrength flat = null, DifficultyStrength @base = null) : base(additive, multiplicative, flat, @base) {

		}
		public CrateChance(EStatModifier eStatModifier) : base(eStatModifier) { }
		public override EnchantmentEffect Clone() {
			return new CrateChance(EStatModifier.Clone());
		}

		public override string Tooltip => $"{EStatModifier.SignPercentMult100Tooltip} {DisplayName}";
		public override EnchantmentStat statName => EnchantmentStat.CrateChance;
	}
}
