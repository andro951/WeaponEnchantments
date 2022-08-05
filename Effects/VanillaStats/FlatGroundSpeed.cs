using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace WeaponEnchantments.Effects {
    public class FlatGroundSpeed : StatEffect {
        public FlatGroundSpeed(StatModifier statModifier) : base(statModifier) { }

        public override bool isVanilla => true;
        public override string statName => "moveSpeed ";
        public override string DisplayName => "Ground Speed";
    }
}
