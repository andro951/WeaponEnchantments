using WeaponEnchantments.Common;

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class SpeedEnchantment : Enchantment
	{
		public override void GetMyStats() {
			AddEStat("I_NPCHitCooldown", EnchantmentStrength);
			AddStaticStat("I_useTime", EnchantmentStrength);
			AddStaticStat("I_useAnimation", EnchantmentStrength);
			if (EnchantmentStrength >= 0.1f)
				AddStaticStat("autoReuse", EnchantmentStrength);
		}
	}
	public class SpeedEnchantmentBasic : SpeedEnchantment { }
	public class SpeedEnchantmentCommon : SpeedEnchantment { }
	public class SpeedEnchantmentRare : SpeedEnchantment { }
	public class SpeedEnchantmentSuperRare : SpeedEnchantment { }
	public class SpeedEnchantmentUltraRare : SpeedEnchantment { }

}
