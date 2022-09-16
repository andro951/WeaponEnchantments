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
	public class OneForAll : StatEffect, INonVanillaStat
    {
        public OneForAll(DifficultyStrength additive = null, DifficultyStrength multiplicative = null, DifficultyStrength flat = null, DifficultyStrength @base = null) : base(additive, multiplicative, flat, @base) {

        }
        public OneForAll(EStatModifier eStatModifier) : base(eStatModifier) { }
        public override EnchantmentEffect Clone() {
            return new OneForAll(EStatModifier.Clone());
        }

        public override string Tooltip => $"{EStatModifier.PercentMult100Tooltip} {DisplayName} (Hitting an enemy will damage all nearby enemies)\n(WARNING - Destroys your projectiles upon hitting an enemy)";
		public override EnchantmentStat statName => EnchantmentStat.OneForAll;
	}
}
