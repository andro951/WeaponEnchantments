using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects {
    public class AutoReuse : BoolEffect {
        public AutoReuse(bool prevent = false) {
            EnableStat = !prevent;
		}
        public override EditableStat statName => EditableStat.AutoReuse;
        public override string DisplayName { get; } = "Auto Reuse";
    }
}
