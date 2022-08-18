using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects {
    public class WingTime : StatEffect, IPassiveEffect, IVanillaStat {
        public WingTime(bool infinity, float additive = 0f, float multiplicative = 1f, float flat = 0f, float @base = 0f) : base(additive, multiplicative, flat, @base) {
            Infinity = infinity;
        }

        public override EnchantmentStat statType => EnchantmentStat.WingTime;
        public override string DisplayName { get; } = "Wing Time";
        private bool Infinity;

        public void PostUpdateMiscEffects(WEPlayer player) {
            if (Infinity && player.Player.wingTime != 0f) { // Exactly how soaring insignia does it
                player.Player.wingTime = player.Player.wingTimeMax;
            }
        }
    }
}
