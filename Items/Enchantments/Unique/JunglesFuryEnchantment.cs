using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using static WeaponEnchantments.Common.EnchantingRarity;

namespace WeaponEnchantments.Items.Enchantments.Unique
{
	public abstract class JunglesFuryEnchantment : Enchantment
	{
		public override int StrengthGroup => 9;
		public override float ScalePercent => 0.2f / defaultEnchantmentStrengths[StrengthGroup].enchantmentTierStrength[tierNames.Length - 1];
		public override int RestrictedClass => (int)DamageTypeSpecificID.Summon;
		public override void GetMyStats() {
			Effects = new() {
				new DamageAfterDefenses(multiplicative: EnchantmentStrengthData),
				new DamageClassChange(DamageClass.SummonMeleeSpeed),
				new MinionAttackTarget(),
				new OnHitTargetDebuffEffect(EnchantmentTier >= 2 ? BuffID.Venom : BuffID.Poisoned, BuffDuration)
			};

			if (EnchantmentTier >= 3) {
				Effects.Add(new OnHitPlayerBuffEffect(BuffID.SwordWhipPlayerBuff, BuffDuration));
				Effects.Add(new OnHitTargetDebuffEffect(BuffID.SwordWhipNPCDebuff, BuffDuration));
			}

			if (EnchantmentTier == 4)
				Effects.Add(new OnHitTargetDebuffEffect(BuffID.RainbowWhipNPCDebuff, BuffDuration));

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Weapons, 1f }
			};
		}

		public override string Artist => "Zorutan";
		public override string Designer => "andro951";
	}
	public class JunglesFuryEnchantmentBasic : JunglesFuryEnchantment { }
	public class JunglesFuryEnchantmentCommon : JunglesFuryEnchantment { }
	public class JunglesFuryEnchantmentRare : JunglesFuryEnchantment { }
	public class JunglesFuryEnchantmentSuperRare : JunglesFuryEnchantment { }
	public class JunglesFuryEnchantmentUltraRare : JunglesFuryEnchantment { }
}
