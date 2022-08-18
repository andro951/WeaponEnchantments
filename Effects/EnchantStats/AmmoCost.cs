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

        public override string DisplayName => EffectStrength >= 0f ? "Ammo Cost Reduction" : "Increased Ammo Cost";
        public override EnchantmentStat statType => EnchantmentStat.AmmoCost;
	public override string Tooltip => ;
	
	private string modifierToString() {
		float strength = EffectStrength;
		if (strength < 0f)
			strength *= -1f;
			
		return $"{strengh}% {DisplayName}";
	}
    }
}
