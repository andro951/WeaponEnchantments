using System.Collections.Generic;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class OneForAllEnchantment : Enchantment
	{
		public override string CustomTooltip => "(Hitting an enemy will damage all nearby enemies)\n(WARNING - Destroys your projectiles upon hitting an enemy)";
		public override int StrengthGroup => 10;
		public override float ScalePercent => 0.8f;
		public override bool Max1 => true;
		public override void GetMyStats() {
			Effects = new() {
				new OneForAll(@base: EnchantmentStrengthData),
				new AttackSpeed(multiplicative: EnchantmentStrengthData * (0.4f / 3f) + (2f / 3f)),
				new NPCHitCooldown(multiplicative: EnchantmentStrengthData * -0.2f + 1.5f)
			};
			//AddEStat(EnchantmentTypeName, 0f, 1f, 0f, EnchantmentStrength);
			//AddEStat("NPCHitCooldown", 0f, 1.5f - EnchantmentStrength * 0.2f);
			//AddStaticStat("useTime", 0f, 1.5f - EnchantmentStrength * 0.2f);
			//AddStaticStat("useAnimation", 0f, 1.5f - EnchantmentStrength * 0.2f);

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Weapons, 1f }
			};
		}

		public override string Artist => "Zorutan";
		public override string Designer => "andro951";
	}
	public class OneForAllEnchantmentBasic : OneForAllEnchantment { }
	public class OneForAllEnchantmentCommon : OneForAllEnchantment { }
	public class OneForAllEnchantmentRare : OneForAllEnchantment { }
	public class OneForAllEnchantmentSuperRare : OneForAllEnchantment { }
	public class OneForAllEnchantmentUltraRare : OneForAllEnchantment { }

}
