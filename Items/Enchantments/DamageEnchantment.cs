using WeaponEnchantments.Common;

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class DamageEnchantment : Enchantment
	{
		public override int LowestCraftableTier => 0;
		public override void GetMyStats() {
			AddEStat(EnchantmentTypeName, EnchantmentStrength);
		}
	}
	public class DamageEnchantmentBasic : DamageEnchantment { }
	public class DamageEnchantmentCommon : DamageEnchantment { }
	public class DamageEnchantmentRare : DamageEnchantment { }
	public class DamageEnchantmentSuperRare : DamageEnchantment { }
	public class DamageEnchantmentUltraRare : DamageEnchantment { }

}
