

namespace WeaponEnchantments.Items.Enchantments.Utility
{
	public abstract class ShootSpeedEnchantment : Enchantment
	{
		public override int StrengthGroup => 10;
		public override void GetMyStats() {
			CheckStaticStatByName();
		}
	}
	public class ShootSpeedEnchantmentBasic : ShootSpeedEnchantment { }
	public class ShootSpeedEnchantmentCommon : ShootSpeedEnchantment { }
	public class ShootSpeedEnchantmentRare : ShootSpeedEnchantment { }
	public class ShootSpeedEnchantmentSuperRare : ShootSpeedEnchantment { }
	public class ShootSpeedEnchantmentUltraRare : ShootSpeedEnchantment { }

}
