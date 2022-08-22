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
    public class VanillaDash : PlayerSetTimerEffect {
        protected VanillaDash(int dashType, DifficultyStrength strengthData, float minimumStrength = 0f, bool prevent = false) : base(minimumStrength, strengthData, prevent) {
            _dashType = dashType;
        }
        private int _dashType;
        public DifficultyStrength StrengthData;
        public float MinimumStrength;
		public EnchantmentStat statName { get; }
        public virtual Time TimerDuration { set; get; }

        public override string DisplayName {
            get {
                switch (_dashType) {
                    case 0:
                        return "";
                    case 1:
                        return "";
                    case 2:
                        return "";
                    case 3:
                        return "";
                    case 4:
                        return "";
                    case 5:
                        return "";
                    case 6:
                        return "";
                    case 7:
                        return "";
                    case 8:
                        return "";
                    case 9:
                        return "";
                    default:
                        return "";
                }
            }
        }
	    
	    public void SetEffect(WEPlayer wePlayer) {
            if (CheckTimer())
            wePlayer.Player.dashType = _dashType;
        }
    }
}
