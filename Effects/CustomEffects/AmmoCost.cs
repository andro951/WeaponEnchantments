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
	public class AmmoCost : StatEffect, INonVanillaStat
    {
        public AmmoCost(DifficultyStrength additive = null, DifficultyStrength multiplicative = null, DifficultyStrength flat = null, DifficultyStrength @base = null) : base(additive, multiplicative, flat, @base) {

		}
		public AmmoCost(EStatModifier eStatModifier) : base(eStatModifier) { }
		public override EnchantmentEffect Clone() {
			return new AmmoCost(EStatModifier.Clone());
		}
		/*
		public override string DisplayName { 
			get {
				if (EffectStrength >= 0f) {
					return "Chance To Not Consume Ammo";//1
				}
				else {
					return "Increased Ammo Cost";//2
				}
			} 
		}
		*/
		public override int DisplayNameNum => EffectStrength >= 0f ? 1 : 2;//1 is Chance not to consume.  2 is Increased Ammo cost
		public override EnchantmentStat statName => EnchantmentStat.AmmoCost;
		public override string Tooltip => ModifierToString();
	
		private string ModifierToString() {
			float strength = EffectStrength;
			if (strength < 0f)
				strength *= -1f;
			
			return $"{strength.Percent() + "%" + DisplayName} (Also Saves Bait When Fishing)";
		}
	}
}
