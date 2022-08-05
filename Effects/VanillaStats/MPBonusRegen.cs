using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace WeaponEnchantments.Effects {
    /// <summary>
    /// This is for constant mana regen
    /// </summary>
    public class MPBonusRegen : StatEffect {
        public MPBonusRegen(StatModifier statModifier) : base(statModifier) { }

        public override bool isVanilla { get; } = true;
        public override string statName { get; } = "manaRegenBonus";
        public override string DisplayName { get; } = "Bonus Mana Regeneration";
    }
}
