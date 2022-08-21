using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments.Utility
{
	public abstract class ManaUsageEnchantment : Enchantment
	{
		public override void GetMyStats() {
			Effects = new() {
				new ManaUsage(@base: EnchantmentStrengthData * -1f)
			};
		}

		public override string ShortTooltip => GetShortTooltip(sign: true);
		public override string Artist => "Zorutan";
		public override string Designer => "andro951";
	}
	public class ManaUsageEnchantmentBasic : ManaUsageEnchantment { }
	public class ManaUsageEnchantmentCommon : ManaUsageEnchantment { }
	public class ManaUsageEnchantmentRare : ManaUsageEnchantment { }
	public class ManaUsageEnchantmentSuperRare : ManaUsageEnchantment { }
	public class ManaUsageEnchantmentUltraRare : ManaUsageEnchantment { }

}
