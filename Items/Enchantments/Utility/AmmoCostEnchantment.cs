using WeaponEnchantments.Common;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments.Utility
{
	public abstract class AmmoCostEnchantment : Enchantment
	{
		public override void GetMyStats() {
			//AddEStat(EnchantmentTypeName, 0f, 1f, 0f, -EnchantmentStrength);
			Effects = new() {
				new AmmoCost(@base: EnchantmentStrengthData)
			};
		}

		public override string Artist => "Zorutan";
		public override string Designer => "andro951";
	}
	public class AmmoCostEnchantmentBasic : AmmoCostEnchantment { }
	public class AmmoCostEnchantmentCommon : AmmoCostEnchantment { }
	public class AmmoCostEnchantmentRare : AmmoCostEnchantment { }
	public class AmmoCostEnchantmentSuperRare : AmmoCostEnchantment { }
	public class AmmoCostEnchantmentUltraRare : AmmoCostEnchantment { }
}
