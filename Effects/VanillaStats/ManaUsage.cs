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
	public class ManaUsage : StatEffect, IVanillaStat
    {
        public ManaUsage(DifficultyStrength additive = null, DifficultyStrength multiplicative = null, DifficultyStrength flat = null, DifficultyStrength @base = null) : base(additive, multiplicative, flat, @base) {

		}
		public ManaUsage(EStatModifier eStatModifier) : base(eStatModifier) { }
		public override EnchantmentEffect Clone() {
			return new ManaUsage(EStatModifier.Clone());
		}

        public override EnchantmentStat statName => EnchantmentStat.ManaUsage;
		public override int DisplayNameNum => EffectStrength - 1f <= 0f ? 1 : 2;//1 is Reduced Mana Usage.  2 is Increased Mana Usage
		public override string TooltipValue => (strength < 0f ? strength * -1 : strength).PercentString();
		private float strength => EffectStrength - 1f < 0f ? EffectStrength - 1f : EffectStrength;
	}
}
