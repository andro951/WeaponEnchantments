using WeaponEnchantments.Effects;
using static WeaponEnchantments.Common.Configs.ConfigValues;

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class SpeedEnchantment : Enchantment
	{
		public override int StrengthGroup => 12;
		public override void GetMyStats() {
			Effects = new() {
				new AttackSpeed(EnchantmentStrengthData)
			};

			if(EnchantmentStrength >= 0.1f)
				Effects.Add(new AutoReuse());
		}

		public override string Artist => "Zorutan";
		public override string Designer => "andro951";
	}
	public class SpeedEnchantmentBasic : SpeedEnchantment { }
	public class SpeedEnchantmentCommon : SpeedEnchantment { }
	public class SpeedEnchantmentRare : SpeedEnchantment { }
	public class SpeedEnchantmentSuperRare : SpeedEnchantment { }
	public class SpeedEnchantmentUltraRare : SpeedEnchantment { }

}
