using WeaponEnchantments.Common;

namespace WeaponEnchantments.Items.Enchantments.Utility
{
	public abstract class AmmoCostEnchantment : Enchantment
	{
		public override void GetMyStats() {
			AddEStat(EnchantmentTypeName, 0f, 1f, 0f, -EnchantmentStrength);
		}
	}
	public class AmmoCostEnchantmentBasic : AmmoCostEnchantment { }
	public class AmmoCostEnchantmentCommon : AmmoCostEnchantment { }
	public class AmmoCostEnchantmentRare : AmmoCostEnchantment { }
	public class AmmoCostEnchantmentSuperRare : AmmoCostEnchantment { }
	public class AmmoCostEnchantmentUltraRare : AmmoCostEnchantment { }
}
