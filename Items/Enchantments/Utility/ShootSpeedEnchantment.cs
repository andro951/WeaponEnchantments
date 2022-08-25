

using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments.Utility
{
	public abstract class ProjectileVelocityEnchantment : Enchantment
	{
		public override int StrengthGroup => 10;
		public override void GetMyStats() {
			Effects = new() {
				new ProjectileVelocity(additive: EnchantmentStrengthData)
			};
		}

		public override string ShortTooltip => GetShortTooltip(sign: true);
		public override string Artist => "Sir Bumpleton";
		public override string Designer => "Sir Bumpleton";
	}
	public class ProjectileVelocityEnchantmentBasic : ProjectileVelocityEnchantment { }
	public class ProjectileVelocityEnchantmentCommon : ProjectileVelocityEnchantment { }
	public class ProjectileVelocityEnchantmentRare : ProjectileVelocityEnchantment { }
	public class ProjectileVelocityEnchantmentSuperRare : ProjectileVelocityEnchantment { }
	public class ProjectileVelocityEnchantmentUltraRare : ProjectileVelocityEnchantment { }
}
