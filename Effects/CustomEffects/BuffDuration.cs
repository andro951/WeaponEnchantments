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
	public class BuffDuration : StatEffect, IPassiveEffect
    {
        private static float carryover;
        public BuffDuration(DifficultyStrength additive = null, DifficultyStrength multiplicative = null, DifficultyStrength flat = null, DifficultyStrength @base = null) : base(additive, multiplicative, flat, @base) {

        }
        public BuffDuration(EStatModifier eStatModifier) : base(eStatModifier) { }
        public override EnchantmentEffect Clone() {
            return new BuffDuration(EStatModifier.Clone());
        }

		public void PostUpdateMiscEffects(WEPlayer wePlayer) {
            carryover = 1f - (1f / EStatModifier.ApplyTo(1f)) + carryover;
            Player player = wePlayer.Player;
            if (carryover >= 1f) {
                carryover %= 1f;
                int maxBuffs = Player.MaxBuffs;
                for(int i = 0; i < maxBuffs; i++) {
                    if (player.buffTime[i] > 0 && !Main.debuff[player.buffType[i]])
                        player.buffTime[i]++;
				}
			}
		}

		public override EnchantmentStat statName => EnchantmentStat.BuffDuration;
	}
}
