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
	public class Multishot: StatEffect, INonVanillaStat
    {
        public Multishot(DifficultyStrength additive = null, DifficultyStrength multiplicative = null, DifficultyStrength flat = null, DifficultyStrength @base = null) : base(additive, multiplicative, flat, @base) {

        }
        public Multishot(EStatModifier eStatModifier) : base(eStatModifier) { }
        public override EnchantmentEffect Clone() {
            return new Multishot(EStatModifier.Clone());
        }

        public override string Tooltip => $"{EStatModifier.PercentMult100Tooltip} {DisplayName}";
		public override EnchantmentStat statName => EnchantmentStat.Multishot;
	}
}
