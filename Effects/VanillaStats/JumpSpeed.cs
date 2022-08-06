using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects {
    public class JumpSpeed : StatEffect {
        public JumpSpeed(StatModifier statModifier) : base(statModifier) { }

        public override EditableStat statName => EditableStat.JumpSpeedBoost;
        public override string DisplayName { get; } = "Jump Speed";
    }
}
