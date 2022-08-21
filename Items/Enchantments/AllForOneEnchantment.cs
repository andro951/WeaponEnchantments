using System;
using System.Collections.Generic;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

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
			Effects = new() {
				new DamageAfterDefenses(multiplicative: EnchantmentStrengthData),
				new NPCHitCooldown(multiplicative: EnchantmentStrengthData * 0.4f + 4f),
				new AttackSpeed(multiplicative: EnchantmentStrengthData * -0.05f + 1f),
				new ManaUsage(@base: EnchantmentStrengthData * 0.15f + 1.5f),
				new AutoReuse(prevent: true)
			};
			AddEStat(EnchantmentTypeName, 0f, EnchantmentStrength);
			//AddEStat("Damage", 0f, EnchantmentStrength);
			//AddEStat("NPCHitCooldown", 0f, 4f + EnchantmentStrength * 0.4f);
			AddStaticStat("useTime", 0f, 1f + EnchantmentStrength * 0.1f);
			AddStaticStat("useAnimation", 0f, 1f + EnchantmentStrength * 0.1f);
			//AddStaticStat("mana", 1.5f + EnchantmentStrength * 0.15f);
			//AddStaticStat("P_autoReuse", EnchantmentStrength);
			//AddStaticStat("P_autoReuseGlove", EnchantmentStrength);

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Weapons, 1f }
			};
		}

		public override string ShortTooltip => $"{Math.Round(EnchantmentStrength * AllowedListMultiplier, 3)}x {EnchantmentTypeName.AddSpaces()}";
		public override string Artist => "Zorutan";
		public override string Designer => "andro951";
	}
	public class AllForOneEnchantmentBasic : AllForOneEnchantment { }
	public class AllForOneEnchantmentCommon : AllForOneEnchantment { }
	public class AllForOneEnchantmentRare : AllForOneEnchantment { }
	public class AllForOneEnchantmentSuperRare : AllForOneEnchantment { }
	public class AllForOneEnchantmentUltraRare : AllForOneEnchantment { }
}
