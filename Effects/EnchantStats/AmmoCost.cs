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

namespace WeaponEnchantments.Effects
{
	public class AmmoCost : StatEffect, INonVanillaStat
    {
        public AmmoCost(float additive = 0f, float multiplicative = 1f, float flat = 0f, float @base = 0f) : base(additive, multiplicative, flat, @base) {
            DisplayName = $"Ammo Cost Reduction";
        }

        public override string DisplayName { get; }
        public override EnchantmentStat statType => EnchantmentStat.AmmoCost;
	}
}
