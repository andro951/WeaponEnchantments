using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using WeaponEnchantments.Common.Utility;

namespace WeaponEnchantments.Effects {
    public interface IEnchantmentStat {
        public EnchantmentStat statName { get; }
    }
}
