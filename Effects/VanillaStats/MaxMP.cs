using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace WeaponEnchantments.Effects {
    public class MaxMP : StatEffect {
        public MaxMP(StatModifier statModifier) : base(statModifier) { }

        public override bool isVanilla { get; } = true;
        public override string statName { get; } = "statManaMax2";
        public override string DisplayName { get; } = "Max Mana";
    }
}
