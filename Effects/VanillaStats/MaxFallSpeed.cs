using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects {
    public class MaxFallSpeed : StatEffect {
        public MaxFallSpeed(StatModifier statModifier) : base(statModifier) { }

        public override EditableStat statName => EditableStat.MaxFallSpeed;
        public override string DisplayName { get; } = "Max Fall Speed";
    }
}
