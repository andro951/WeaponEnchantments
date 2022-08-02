

namespace WeaponEnchantments.Items.Enchantments.Utility
{
	public abstract class ScaleEnchantment : Enchantment
	{
		public override int StrengthGroup => 10;
		public override string MyDisplayName => "Size";
		public override void GetMyStats() {
			CheckStaticStatByName();
			AddStaticStat("whipRangeMultiplier", EnchantmentStrength);
		}

		public override string Artist => "Zorutan";
		public override string Designer => "andro951";
	}
	public class ScaleEnchantmentBasic : ScaleEnchantment { }
	public class ScaleEnchantmentCommon : ScaleEnchantment { }
	public class ScaleEnchantmentRare : ScaleEnchantment { }
	public class ScaleEnchantmentSuperRare : ScaleEnchantment { }
	public class ScaleEnchantmentUltraRare : ScaleEnchantment { }

}
