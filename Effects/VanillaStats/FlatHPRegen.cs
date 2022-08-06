using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects {
    public class FlatHPRegen : StatEffect {
        public FlatHPRegen(StatModifier statModifier) : base(statModifier) { }

        public override EditableStat statName => EditableStat.LifeRegen;
        public override string DisplayName => "Life Regeneration";
    }
}
