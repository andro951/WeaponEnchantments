using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponEnchantments.Common;

namespace WeaponEnchantments.Items.Enchantments.Utility
{
	public abstract class AmmoCostEnchantment : Enchantment
	{
		public override List<EStat> EStats => new List<EStat>() {
			new EStat(EnchantmentTypeName, 0f, 1f, 0f, -EnchantmentStrength)
		};
	}

	public class AmmoCostEnchantmentBasic : AmmoCostEnchantment { }public class AmmoCostEnchantmentCommon : AmmoCostEnchantment { }public class AmmoCostEnchantmentRare : AmmoCostEnchantment { }public class AmmoCostEnchantmentSuperRare : AmmoCostEnchantment { }public class AmmoCostEnchantmentUltraRare : AmmoCostEnchantment { }
}
