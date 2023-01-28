using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;

namespace WeaponEnchantments.Effects
{
	public class InfinitePenetration : BoolEffect
	{
		public InfinitePenetration(bool prevent = false) : base(prevent) {

		}
		public override EnchantmentEffect Clone() {
			return new InfinitePenetration(!EnableStat);
		}

		public override EnchantmentStat statName => EnchantmentStat.InfinitePenetration;
	}
}
