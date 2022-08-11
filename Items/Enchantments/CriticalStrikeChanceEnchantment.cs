

using Terraria.ModLoader;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class CriticalStrikeChanceEnchantment : Enchantment
	{
		public override void GetMyStats() {
			Effects = new EnchantmentEffect[] {
				new CriticalStrikeChance(@base: EnchantmentStrength * 100f),
			};
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
