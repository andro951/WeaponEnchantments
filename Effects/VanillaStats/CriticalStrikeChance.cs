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
    public class CriticalStrikeChance : StatEffect, IClassedEffect {
        public CriticalStrikeChance(float additive = 0f, float multiplicative = 1f, float flat = 0f, float @base = 0f, DamageClass dc = null) : base(additive, multiplicative, flat * 100f, @base * 100f) {
            damageClass = dc != null ? dc : DamageClass.Generic;
            DisplayName = $"{damageClass.DisplayName} Critical Strike Chance";
        }

        public DamageClass damageClass { get; set; }
        public override string DisplayName { get; }
        public override PlayerStat statName => PlayerStat.CriticalStrikeChance;
    }
}