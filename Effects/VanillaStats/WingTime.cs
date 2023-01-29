using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects {
    public class WingTime : StatEffect, IPassiveEffect, IVanillaStat {
        public WingTime(bool infinity, DifficultyStrength additive = null, DifficultyStrength multiplicative = null, DifficultyStrength flat = null, DifficultyStrength @base = null) : base(additive, multiplicative, flat, @base) {
            Infinity = infinity;
        }
        public WingTime(bool infinity, EStatModifier eStatModifier) : base(eStatModifier) {
            Infinity = infinity;
        }
        public override EnchantmentEffect Clone() {
            return new WingTime(Infinity, EStatModifier.Clone());
        }

        public override EnchantmentStat statName => EnchantmentStat.WingTime;
        public override string DisplayName { get; } = "Wing Time";
        private bool Infinity;

        public void PostUpdateMiscEffects(WEPlayer player) {
            if (Infinity && player.Player.wingTime != 0f) { // Exactly how soaring insignia does it
                player.Player.wingTime = player.Player.wingTimeMax;
            }
        }
    }
}
