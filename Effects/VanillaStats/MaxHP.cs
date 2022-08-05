using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace WeaponEnchantments.Effects {
    public class MaxHP : StatEffect {
        public MaxHP(StatModifier statModifier) : base(statModifier) { }

        public override bool isVanilla { get; } = true;
        public override string statName { get; } = "statLifeMax2";
        public override string DisplayName { get; } = "Max Life";
    }
}
