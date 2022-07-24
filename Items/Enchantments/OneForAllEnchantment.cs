using System.Collections.Generic;
using WeaponEnchantments.Common;

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class OneForAllEnchantment : Enchantment
	{
		public override string CustomTooltip => "(Hitting an enemy will damage all nearby enemies)\n(WARNING - Destroys your projectiles upon hitting an enemy)";
		public override int StrengthGroup => 10;
		public override float ScalePercent => 0.8f;
		public override bool Max1 => true;
		public override Dictionary<string, float> AllowedList => new Dictionary<string, float>() {
			{ "Weapon", 1f }
		};
		public override void GetMyStats() {
			AddEStat(EnchantmentTypeName, 0f, 1f, 0f, EnchantmentStrength);
			AddEStat("NPCHitCooldown", 0f, 1.5f - EnchantmentStrength * 0.2f);
			AddStaticStat("useTime", 0f, 1.5f - EnchantmentStrength * 0.2f);
			AddStaticStat("useAnimation", 0f, 1.5f - EnchantmentStrength * 0.2f);
		}
	}
	public class OneForAllEnchantmentBasic : OneForAllEnchantment { }
	public class OneForAllEnchantmentCommon : OneForAllEnchantment { }
	public class OneForAllEnchantmentRare : OneForAllEnchantment { }
	public class OneForAllEnchantmentSuperRare : OneForAllEnchantment { }
	public class OneForAllEnchantmentUltraRare : OneForAllEnchantment { }

}
