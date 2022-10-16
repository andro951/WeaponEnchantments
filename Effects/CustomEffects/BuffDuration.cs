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
	public class BuffDuration : EnchantmentEffect, IPassiveEffect
    {
        private static float carryover;
        DifficultyStrength _additiveBonus;
        public BuffDuration(DifficultyStrength additiveBonus = null) {
            _additiveBonus = additiveBonus;
        }
        public override EnchantmentEffect Clone() {
            return new BuffDuration(_additiveBonus.Clone());
        }

		public void PostUpdateMiscEffects(WEPlayer wePlayer) {
            carryover = 1f - (1f / (1f + _additiveBonus.Value)) + carryover;
            Player player = wePlayer.Player;
            if (carryover >= 1f) {
                carryover %= 1f;
                for(int i = 0; i < player.buffType.Length; i++) {
                    if (player.buffTime[i] > 0 && !Main.debuff[player.buffType[i]])
                        player.buffTime[i]++;
				}
			}
		}

		public override IEnumerable<object> DisplayNameArgs => new object[] { TooltipValue };
		public override string TooltipValue => _additiveBonus.Value.PercentString();
		public override string Tooltip => StandardTooltip;
	}
}
