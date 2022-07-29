using WeaponEnchantments.Common.Utility;

namespace WeaponEnchantments.Items.Enchantments.Utility
{
	public abstract class ManaEnchantment : Enchantment
	{
		public override void GetMyStats() {
			AddStaticStat(EnchantmentTypeName.ToFieldName(), -EnchantmentStrength);
		}

		public override string Artist => "Zorutan";
		public override string Designer => "andro951";
	}
	public class ManaEnchantmentBasic : ManaEnchantment { }
	public class ManaEnchantmentCommon : ManaEnchantment { }
	public class ManaEnchantmentRare : ManaEnchantment { }
	public class ManaEnchantmentSuperRare : ManaEnchantment { }
	public class ManaEnchantmentUltraRare : ManaEnchantment { }

}
