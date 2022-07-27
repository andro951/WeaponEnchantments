﻿using System.Collections.Generic;

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class MaxMinionsEnchantment : Enchantment
	{
		public override bool? ShowPercentSignInTooltip => false;
		public override bool? MultiplyBy100InTooltip => false;
		public override int StrengthGroup => 10;
		public override float ScalePercent => 0.6f;
		public override Dictionary<string, float> AllowedList => new Dictionary<string, float>() {
			{ "Armor", 1f },
			{ "Accessory", 1f }
		};
		public override void GetMyStats() {
			CheckStaticStatByName();
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