using WeaponEnchantments.Effects;
using static WeaponEnchantments.Common.Configs.ConfigValues;

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class SpeedEnchantment : Enchantment
	{
		public override EnchantmentEffect[] Effects => new EnchantmentEffect[] {
			new AttackSpeed(EnchantmentStrength)
		};

		public override int StrengthGroup => 12;

		public override string Artist => "Zorutan";
		public override string Designer => "andro951";
	}
	public class SpeedEnchantmentBasic : SpeedEnchantment { }
	public class SpeedEnchantmentCommon : SpeedEnchantment { }
	public class SpeedEnchantmentRare : SpeedEnchantment { }
	public class SpeedEnchantmentSuperRare : SpeedEnchantment { }
	public class SpeedEnchantmentUltraRare : SpeedEnchantment { }

}
