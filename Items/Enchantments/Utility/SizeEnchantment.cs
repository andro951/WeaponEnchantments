using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments.Utility
{
	public abstract class SizeEnchantment : Enchantment
	{
		public override int StrengthGroup => 10;
		public override void GetMyStats() {
			Effects = new() {
				new Size(EnchantmentStrengthData),
				new WhipRange(EnchantmentStrengthData)
			};
		}

		public override string ShortTooltip => GetShortTooltip(sign: true);
		public override string Artist => "Zorutan";
		public override string Designer => "andro951";
	}
	public class SizeEnchantmentBasic : SizeEnchantment { }
	public class SizeEnchantmentCommon : SizeEnchantment { }
	public class SizeEnchantmentRare : SizeEnchantment { }
	public class SizeEnchantmentSuperRare : SizeEnchantment { }
	public class SizeEnchantmentUltraRare : SizeEnchantment { }

}
