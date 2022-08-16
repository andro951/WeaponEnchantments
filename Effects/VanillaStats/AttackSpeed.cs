using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects {
    public class AttackSpeed : StatEffect, IClassedEffect, IVanillaStat {
        public AttackSpeed(float additive = 0f, float multiplicative = 1f, float flat = 0f, float @base = 0f, DamageClass dc = null) : base(additive, multiplicative, flat, @base) {
            damageClass = dc ?? DamageClass.Generic;
            DisplayName = $"{(damageClass.Type > DamageClass.Generic.Type ? damageClass.DisplayName : "")} Speed";
        }

        public DamageClass damageClass { get; set; }
        public override string DisplayName { get; }
        public override PlayerStat statName => PlayerStat.AttackSpeed;
    }
}