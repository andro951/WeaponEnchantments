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
        public AutoReuse(bool prevent = false) : base(prevent) { }
        public override EnchantmentStat statName => EnchantmentStat.AutoReuse;
        public override string DisplayName { get; } = "Auto Reuse";
    }
}
