using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using WeaponEnchantments.Common.Globals;

namespace WeaponEnchantments.Effects
{
	public interface IAddDynamicEffects
	{
		public void AddDynamicEffects(List<EnchantmentEffect> effects, EnchantedItem enchantedItem);
		public EnchantedItem EnchantedItem { get; set; }
	}
}
