using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects {
    public class FlatGroundSpeed : StatEffect {
        public FlatGroundSpeed(StatModifier statModifier) : base(statModifier) { }

        public override EditableStat statName => EditableStat.MoveSpeed;
        public override string DisplayName => "Ground Speed";
    }
}
