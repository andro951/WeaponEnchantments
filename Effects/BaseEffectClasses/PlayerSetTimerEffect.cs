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
    public abstract class PlayerSetTimerEffect : PlayerSetEffect, IUseTimer {
        protected PlayerSetTimerEffect(float minimumStrength, DifficultyStrength timerData, bool prevent) : base(minimumStrength, timerData, prevent) {
            MinimumStrength = minimumStrength;
            EnableStat = !prevent;
	        TimerDuration = new Time(timerData);
        }

        public EnchantmentStat TimerStatName => statName;
        public virtual Time TimerDuration { set; get; }

        public override string Tooltip {
            get {
                if (StrengthData != null && MinimumStrength > StrengthData.Value)
                    return "";

                return $"{(EnableStat ? "Grants" : "Prevents")} {DisplayName} ({TimerDuration} cooldown)";//Not set up for Localization yet
            }
	    }

		public override string TooltipKey => EnableStat ? "Grants" : "Prevents";
		public override string TooltipValue => TooltipKey;
	}
}