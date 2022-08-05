using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace WeaponEnchantments.Effects {
    public class MaxFallSpeed : StatEffect {
        public MaxFallSpeed(StatModifier statModifier) : base(statModifier) { }

        public override bool isVanilla { get; } = true;
        public override string statName { get; } = "maxFallSpeed";
        public override string DisplayName { get; } = "Jump Speed";
    }
}
