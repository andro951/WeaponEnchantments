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
		public InfinitePenetration(float minimumStrength = 0f, DifficultyStrength strengthData = null, bool prevent = false) : base(minimumStrength, strengthData, prevent) {

		}
		public override EnchantmentEffect Clone() {
			return new InfinitePenetration(MinimumStrength, StrengthData.Clone(), !EnableStat);
		}

		public override EnchantmentStat statName => EnchantmentStat.InfinitePenetration;
	}
}
