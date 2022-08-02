using static WeaponEnchantments.Common.Configs.ConfigValues;

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class SpeedEnchantment : Enchantment
	{
		public override void GetMyStats() {
			AddEStat("I_NPCHitCooldown", EnchantmentStrength);
			AddStaticStat("I_useTime", EnchantmentStrength);
			AddStaticStat("I_useAnimation", EnchantmentStrength);
			if (EnchantmentStrength >= SpeedEnchantmentAutoReuseSetpoint)
				AddStaticStat("autoReuse", EnchantmentStrength);
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
