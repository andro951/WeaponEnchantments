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
using androLib.Common.Utility;
using androLib.Common.Globals;

namespace WeaponEnchantments.Effects {
    public class VanillaDash : PlayerSetEffect {
        public VanillaDash(DashID_WE dashType, DifficultyStrength timerData, float minimumStrength = 0f, bool prevent = false) : base(minimumStrength, timerData, prevent) {
            _dashType = dashType;
        }
		public override EnchantmentEffect Clone() {
            return new VanillaDash(_dashType, StrengthData.Clone(), MinimumStrength, !EnableStat);
		}

		private DashID_WE _dashType;
		public override string TooltipValue => _dashType.ToString().AddSpaces();
		public override int DisplayNameNum => (int)_dashType;

		public override void SetEffect(Player player) {
            int playerDashType = player.dashType;
            switch (_dashType) {
                case DashID_WE.NinjaTabiDash when playerDashType < 3:
                case DashID_WE.EyeOfCthulhuShieldDash when playerDashType == 0:
                case DashID_WE.SolarDash:
                case DashID_WE.CrystalNinjaDash when playerDashType < 3:
                    player.dashType = (int)_dashType;
                    break;
			}
        }
	}
}