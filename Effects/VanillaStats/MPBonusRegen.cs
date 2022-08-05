using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects {
    /// <summary>
    /// This is for constant mana regen
    /// </summary>
    public class MPBonusRegen : StatEffect {
        public MPBonusRegen(StatModifier statModifier) : base(statModifier) { }

        public override EditableStat statName => EditableStat.BonusManaRegen;
        public override string DisplayName { get; } = "Bonus Mana Regeneration";
    }
}
