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
        protected PlayerSetEffect(float minimumStrength, DifficultyStrength strengthData, bool prevent) {
            MinimumStrength = minimumStrength;
            EnableStat = !prevent;
	    TimerDuration = new Time(strengthData);
        }

        public float MinimumStrength;
		    public abstract EnchantmentStat statName { get; }
	
        public virtual Time TimerDuration { set; get; };

        public override string Tooltip {
            get {
                if (StrengthData != null && MinimumStrength > StrengthData.Value)
                    return "";

                return $"{(EnableStat ? "Grants" : "Prevents")} {DisplayName} ({TimerDuration} cooldown)"";
            }
	      }
	    
	      public abstract void SetEffect();
        
        public virtual void SetTimer(WEPlayer wePlayer) {
          wePlayer.SetEffectTimer(this);
        }
        public virtual void CheckTimer() {
          Main.LocalPlayer.GetWEPlayer().CheckTimer(this);
        }
        public virtual void TimerEnd() { }
        public virtual void ActivateEffect(WEPlayer wePlayer) {
          SetTimer(wePlayer);
        }
    }
}
