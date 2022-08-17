using System.Collections.Generic;
using WeaponEnchantments.Common;

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class AllForOneEnchantment : Enchantment
	{
		public override string CustomTooltip => $"(Item CD equal to {EnchantmentStrength * 0.8f}x use speed)";
		public override int StrengthGroup => 6;
		public override float ScalePercent => 0.8f;
		public override bool Max1 => true;
		public override int RestrictedClass => (int)DamageTypeSpecificID.Summon;
		public override void GetMyStats() {
			AddEStat(EnchantmentTypeName, 0f, EnchantmentStrength);
			AddEStat("Damage", 0f, EnchantmentStrength);
			AddEStat("NPCHitCooldown", 0f, 4f + EnchantmentStrength * 0.4f);
			AddStaticStat("useTime", 0f, 1f + EnchantmentStrength * 0.1f);
			AddStaticStat("useAnimation", 0f, 1f + EnchantmentStrength * 0.1f);
			AddStaticStat("mana", 1.5f + EnchantmentStrength * 0.15f);
			AddStaticStat("P_autoReuse", EnchantmentStrength);
			AddStaticStat("P_autoReuseGlove", EnchantmentStrength);

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Weapon, 1f }
			};
		}

		public override string Artist => "Zorutan";
		public override string Designer => "andro951";
	}
	public class AllForOneEnchantmentBasic : AllForOneEnchantment { }
	public class AllForOneEnchantmentCommon : AllForOneEnchantment { }
	public class AllForOneEnchantmentRare : AllForOneEnchantment { }
	public class AllForOneEnchantmentSuperRare : AllForOneEnchantment { }
	public class AllForOneEnchantmentUltraRare : AllForOneEnchantment { }
}
