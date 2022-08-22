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
        protected PlayerSetTimerEffect(float minimumStrength, DifficultyStrength strengthData, bool prevent) : base(minimumStrength, strengthData, prevent) {
            MinimumStrength = minimumStrength;
            EnableStat = !prevent;
	        TimerDuration = new Time(strengthData);
        }

        //public float MinimumStrength;
		public override EnchantmentStat statName { get; }
	
        public virtual Time TimerDuration { set; get; }

        public override string Tooltip {
            get {
                if (StrengthData != null && MinimumStrength > StrengthData.Value)
                    return "";

                return $"{(EnableStat ? "Grants" : "Prevents")} {DisplayName} ({TimerDuration} cooldown)";
            }
	    }



		//public abstract void SetEffect();

		public virtual void SetTimer(WEPlayer wePlayer) {
            wePlayer.SetEffectTimer(this);
        }
        public virtual bool CheckTimer() {
            return Main.LocalPlayer.GetWEPlayer().CheckTimer(this);
        }
        public virtual void TimerEnd() { }
        public virtual void ActivateEffect(WEPlayer wePlayer) {
          SetTimer(wePlayer);
        }

		public void SetTimer() {
			throw new NotImplementedException();
		}

		void IUseTimer.CheckTimer() {
			throw new NotImplementedException();
		}

		public void ActivateEffect() {
			throw new NotImplementedException();
		}
	}
}
