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
    public class ArmorPenetration : StatEffect, IClassedEffect, IVanillaStat {
        public ArmorPenetration(float additive = 0f, float multiplicative = 1f, float flat = 0f, float @base = 0f, DamageClass dc = null) : base(additive, multiplicative, flat, @base) {
            damageClass = dc != null ? dc : DamageClass.Generic;
        }
        
        public DamageClass damageClass { get; set; }
        public override EnchantmentStat statType => EnchantmentStat.ArmorPenetration;
        public override DisplayName { get; } = $"{damageClass.S()} Armor Penetration";
    }
}
