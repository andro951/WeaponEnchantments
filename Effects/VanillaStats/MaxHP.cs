using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects {
    public class MaxHP : StatEffect {
        public MaxHP(float additive = 0f, float multiplicative = 1f, float flat = 0f, float @base = 0f) : base(additive, multiplicative, flat, @base) { }

        public override PlayerStat statName => PlayerStat.MaxHP;
        public override string DisplayName { get; } = "Max Life";
    }
}
