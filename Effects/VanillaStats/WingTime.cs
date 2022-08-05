using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;

namespace WeaponEnchantments.Effects {
    public class WingTime : StatEffect, IPassiveEffect {
        public WingTime(StatModifier sm, bool infinity = false) : base(sm) {
            Infinity = infinity;
        }

        public override string DisplayName { get; } = "Wing Time";
        public override bool isVanilla { get; } = true;
        public override string statName { get; } = "wingTimeMax";
        private bool Infinity;

        protected override string modifierToString() {
            if (Infinity) {
                return "Permanent";
            }
            string final = "";
            float ad = statModifier.Flat;
            if (ad > 0) {
                final += $"{s(ad)}{new Time((int)ad, Time.Magnitude.Frames)}";
            }
            float mp = statModifier.Multiplicative + statModifier.Additive - 2;
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
