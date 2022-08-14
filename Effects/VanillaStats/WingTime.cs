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
    public class WingTime : StatEffect, IPassiveEffect {
        public WingTime(bool infinity, float additive = 0f, float multiplicative = 1f, float flat = 0f, float @base = 0f) : base(additive, multiplicative, flat, @base) {
            Infinity = infinity;
        }

        public override PlayerStat statName => PlayerStat.WingTime;
        public override string DisplayName { get; } = "Wing Time";
        private bool Infinity;

        protected override string modifierToString() {
            if (Infinity) {
                return "Permanent";
            }
            string final = "";
            float ad = EStatModifier.Flat;
            if (ad > 0) {
                final += $"{s(ad)}{new Time((int)ad, Time.Magnitude.Frames)}";
            }
            float mp = EStatModifier.Multiplicative + EStatModifier.Additive - 2;
            if (mp > 0) {
                if (final != "") final += ' ';
                final += $"{s(mp)}{mp.Percent()}";
            }
            return final;
        }

        public void PostUpdateMiscEffects(WEPlayer player) {
            if (Infinity && player.Player.wingTime != 0f) { // Exactly how soaring insignia does it
                player.Player.wingTime = player.Player.wingTimeMax;
            }
        }
    }
}
