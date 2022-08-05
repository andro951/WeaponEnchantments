using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace WeaponEnchantments.Effects {
    public class DefenseEffect : StatEffect {
        public DefenseEffect(StatModifier statModifier) : base(statModifier) { }

        public override bool isVanilla => true;
        public override string statName => "statDefense";
        public override string DisplayName => "Defense";
    }
}
