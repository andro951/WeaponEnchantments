using WeaponEnchantments.Effects;
using static WeaponEnchantments.Common.Configs.ConfigValues;

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class AttackSpeedEnchantment : Enchantment
	{
		public override void GetMyStats() {
			Effects = new() {
				new AttackSpeed(EnchantmentStrengthData),
				new AutoReuse(AttackSpeedEnchantmentAutoReuseSetpoint, EnchantmentStrengthData)
			};
		}

		public override string ShortTooltip => GetShortTooltip(sign: true);
		public override string Artist => "Zorutan";
		public override string Designer => "andro951";
	}
	public class AttackSpeedEnchantmentBasic : AttackSpeedEnchantment { }
	public class AttackSpeedEnchantmentCommon : AttackSpeedEnchantment { }
	public class AttackSpeedEnchantmentRare : AttackSpeedEnchantment { }
	public class AttackSpeedEnchantmentSuperRare : AttackSpeedEnchantment { }
	public class AttackSpeedEnchantmentUltraRare : AttackSpeedEnchantment { }

}
