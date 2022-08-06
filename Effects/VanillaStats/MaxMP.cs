using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects {
    public class MaxMP : StatEffect {
        public MaxMP(StatModifier statModifier) : base(statModifier) { }

        public override EditableStat statName => EditableStat.MaxMP;
        public override string DisplayName { get; } = "Max Mana";
    }
}
