

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class CriticalStrikeChanceEnchantment : Enchantment
	{
		//public override bool? ShowPercentSignInTooltip => false;
		public override bool? MultiplyBy100InTooltip => false;
		public override void GetMyStats() {
			CheckStaticStatByName();
		}

		public override string Artist => "Zorutan";
		public override string Designer => "andro951";
	}
	public class CriticalStrikeChanceEnchantmentBasic : CriticalStrikeChanceEnchantment { }
	public class CriticalStrikeChanceEnchantmentCommon : CriticalStrikeChanceEnchantment { }
	public class CriticalStrikeChanceEnchantmentRare : CriticalStrikeChanceEnchantment { }
	public class CriticalStrikeChanceEnchantmentSuperRare : CriticalStrikeChanceEnchantment { }
	public class CriticalStrikeChanceEnchantmentUltraRare : CriticalStrikeChanceEnchantment { }
}
