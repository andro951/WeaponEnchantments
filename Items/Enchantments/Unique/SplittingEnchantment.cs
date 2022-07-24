

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class SplittingEnchantment : Enchantment
	{
		public override string CustomTooltip => "(Chance to produce an extra projectile)";
		public override int StrengthGroup => 8;
		public override int DamageClassSpecific => (int)DamageTypeSpecificID.Ranged;
	}
	public class SplittingEnchantmentBasic : SplittingEnchantment { }
	public class SplittingEnchantmentCommon : SplittingEnchantment { }
	public class SplittingEnchantmentRare : SplittingEnchantment { }
	public class SplittingEnchantmentSuperRare : SplittingEnchantment { }
	public class SplittingEnchantmentUltraRare : SplittingEnchantment { }

}
