using System.Collections.Generic;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class MaxMinionsEnchantment : Enchantment
	{
		public override int StrengthGroup => 10;
		public override float ScalePercent => 0.6f;
		public override void GetMyStats() {
			Effects = new() {
				new MaxMinions(@base: EnchantmentStrengthData)
			};

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Armor, 1f },
				{ EItemType.Accessory, 1f }
			};
		}

		public override string Artist => "andro951";
		public override string Designer => "andro951";
	}
	public class MaxMinionsEnchantmentBasic : MaxMinionsEnchantment { }
	public class MaxMinionsEnchantmentCommon : MaxMinionsEnchantment { }
	public class MaxMinionsEnchantmentRare : MaxMinionsEnchantment { }
	public class MaxMinionsEnchantmentSuperRare : MaxMinionsEnchantment { }
	public class MaxMinionsEnchantmentUltraRare : MaxMinionsEnchantment { }

}