using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using WeaponEnchantments.Common.Utility;

namespace WeaponEnchantments.Effects {
    public interface ICanUseItem {
        public bool CanUseItem(Item item, Player player);
	}
}
