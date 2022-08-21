using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments.Utility
{
	public abstract class ReducedManaUsageEnchantment : Enchantment
	{
		public override void GetMyStats() {
			Effects = new() {
				new ManaUsage(@base: EnchantmentStrengthData * -1f)
			};
		}

		public override string Artist => "Zorutan";
		public override string Designer => "andro951";
	}
	public class ReducedManaUsageEnchantmentBasic : ReducedManaUsageEnchantment { }
	public class ReducedManaUsageEnchantmentCommon : ReducedManaUsageEnchantment { }
	public class ReducedManaUsageEnchantmentRare : ReducedManaUsageEnchantment { }
	public class ReducedManaUsageEnchantmentSuperRare : ReducedManaUsageEnchantment { }
	public class ReducedManaUsageEnchantmentUltraRare : ReducedManaUsageEnchantment { }

}
