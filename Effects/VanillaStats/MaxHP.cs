using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects {
    public class MaxHP : StatEffect {
        public MaxHP(StatModifier statModifier) : base(statModifier) { }

        public override EditableStat statName => EditableStat.MaxHP;
        public override string DisplayName { get; } = "Max Life";
    }
}
