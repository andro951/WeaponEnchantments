using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace WeaponEnchantments.Effects {
    public class FlatHPRegen : StatEffect {
        public FlatHPRegen(StatModifier statModifier) : base(statModifier) { }

        public override bool isVanilla => true;
        public override string statName => "lifeRegen";
        public override string DisplayName => "Life Regeneration";
    }
}
