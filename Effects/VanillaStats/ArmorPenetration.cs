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
    public class ArmorPenetration : StatEffect, IClassedEffect {
        public ArmorPenetration(StatModifier sm, DamageClass dc = null) : base(sm) {
            damageClass = dc != null ? dc : DamageClass.Generic;
            DisplayName = $"{damageClass.DisplayName} Penetration";
        }

        public DamageClass damageClass { get; set; }
        public override string DisplayName { get; }
        public override EditableStat statName => EditableStat.ArmorPenetration;

        public void PostUpdateMiscEffects(WEPlayer player) {
            player.Player.GetArmorPenetration(damageClass) *= EnchantmentPower;
        }
    }
}
