using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponEnchantments.Common.Globals;

namespace WeaponEnchantments.Items
{
	public interface IStoreAppliedItem
	{
		EnchantedItem EnchantedItem { get; set; }
	}
}
