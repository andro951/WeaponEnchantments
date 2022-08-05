using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects {
    /// <summary>
    /// This is for maximum, after charging, mana regen
    /// </summary>
    public class FlatMPRegen : StatEffect {
        public FlatMPRegen(StatModifier statModifier) : base(statModifier) { }

        public override EditableStat statName => EditableStat.ManaRegen;
        public override string DisplayName { get; } = "Mana Regeneration";
    }
}
