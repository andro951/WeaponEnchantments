using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace WeaponEnchantments.Effects {
    /// <summary>
    /// This is for maximum, after charging, mana regen
    /// </summary>
    public class FlatMPRegen : StatEffect {
        public FlatMPRegen(StatModifier statModifier) : base(statModifier) { }

        public override bool isVanilla { get; } = true;
        public override string statName { get; } = "manaRegen";
        public override string DisplayName { get; } = "Mana Regeneration";
    }
}
