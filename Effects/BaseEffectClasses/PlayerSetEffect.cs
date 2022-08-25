using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;

namespace WeaponEnchantments.Effects {
    public abstract class PlayerSetEffect : EnchantmentEffect, IEnchantmentStat {
        protected PlayerSetEffect(float minimumStrength, DifficultyStrength strengthData, bool prevent) {
            MinimumStrength = minimumStrength;
            EnableStat = !prevent;
            StrengthData = strengthData;
        }

        public bool EnableStat { get; protected set; }
        public DifficultyStrength StrengthData;
        public float MinimumStrength;
		public virtual EnchantmentStat statName { get; }

        public override string Tooltip {
            get {
                if (StrengthData != null && MinimumStrength > StrengthData.Value)
                    return "";

                return $"{DisplayName}{": " + (EnableStat ? "Enabled" : "Prevented")}";
            }
	}
	    
	public abstract void SetEffect(Player player);
    }
}
