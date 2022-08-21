using System.Collections.Generic;
using WeaponEnchantments.Common.Utility;
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
				{ EItemType.Accessories, 1f }
			};
		}

		public override string ShortTooltip => GetShortTooltip(sign: true, percent: false, multiply100: false);
		public override string Artist => "𝕾𝖔𝖚𝖙𝖍𝕸𝖆𝖓𝖊♱";
		public override string Designer => "andro951";
	}
	public class MaxMinionsEnchantmentBasic : MaxMinionsEnchantment { }
	public class MaxMinionsEnchantmentCommon : MaxMinionsEnchantment { }
	public class MaxMinionsEnchantmentRare : MaxMinionsEnchantment { }
	public class MaxMinionsEnchantmentSuperRare : MaxMinionsEnchantment { }
	public class MaxMinionsEnchantmentUltraRare : MaxMinionsEnchantment { }

}