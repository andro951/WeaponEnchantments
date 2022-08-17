using WeaponEnchantments.Common;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class DamageEnchantment : Enchantment
	{
		public override int LowestCraftableTier => 0;
		public override void GetMyStats() {
			Effects = new() {
				new DamageAfterDefenses(EnchantmentStrength)
			};
		}

		public override string Artist => "Zorutan";
		public override string Designer => "andro951";
	}
	public class DamageEnchantmentBasic : DamageEnchantment { }
	public class DamageEnchantmentCommon : DamageEnchantment { }
	public class DamageEnchantmentRare : DamageEnchantment { }
	public class DamageEnchantmentSuperRare : DamageEnchantment { }
	public class DamageEnchantmentUltraRare : DamageEnchantment { }

}
