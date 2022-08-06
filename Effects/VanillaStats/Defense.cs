using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects {
    public class DefenseEffect : StatEffect {
        public DefenseEffect(StatModifier statModifier) : base(statModifier) { }

        public override EditableStat statName => EditableStat.Defense;
        public override string DisplayName => "Defense";
    }
}
