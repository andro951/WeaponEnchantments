using System.Collections.Generic;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class ExtraFishingLineEnchantment : Enchantment
	{
		public override string CustomTooltip => "(Chance to produce an extra projectile.  Applies to each projectile created.)";
		public override int StrengthGroup => 8;
		public override void GetMyStats() {
			Effects = new() {
				new Multishot(@base: EnchantmentStrengthData)
			};

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.FishingPoles, 1f }
			};
		}
		public override string Artist => "andro951";
		public override string Designer => "andro951";
	}
	public class ExtraFishingLineEnchantmentBasic : ExtraFishingLineEnchantment { }
	public class ExtraFishingLineEnchantmentCommon : ExtraFishingLineEnchantment { }
	public class ExtraFishingLineEnchantmentRare : ExtraFishingLineEnchantment { }
	public class ExtraFishingLineEnchantmentSuperRare : ExtraFishingLineEnchantment { }
	public class ExtraFishingLineEnchantmentUltraRare : ExtraFishingLineEnchantment { }

}
