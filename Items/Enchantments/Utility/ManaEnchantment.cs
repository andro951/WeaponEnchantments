using WeaponEnchantments.Common;

namespace WeaponEnchantments.Items.Enchantments.Utility
{
	public abstract class ManaEnchantment : Enchantment
	{
		public override void GetMyStats() {
			AddStaticStat(EnchantmentTypeName.ToFieldName(), -EnchantmentStrength);
		}
	}
	public class ManaEnchantmentBasic : ManaEnchantment { }
	public class ManaEnchantmentCommon : ManaEnchantment { }
	public class ManaEnchantmentRare : ManaEnchantment { }
	public class ManaEnchantmentSuperRare : ManaEnchantment { }
	public class ManaEnchantmentUltraRare : ManaEnchantment { }

}
