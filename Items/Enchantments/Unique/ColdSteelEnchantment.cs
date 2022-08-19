using System.Collections.Generic;
using Terraria.ID;
using static WeaponEnchantments.Common.EnchantingRarity;
using WeaponEnchantments.Effects;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;

namespace WeaponEnchantments.Items.Enchantments.Unique
{
	public abstract class ColdSteelEnchantment : Enchantment
	{
		public override int StrengthGroup => 9;
		public override float ScalePercent => 0.2f / defaultEnchantmentStrengths[StrengthGroup].enchantmentTierStrength[tierNames.Length - 1];
		public override int RestrictedClass => (int)DamageTypeSpecificID.Summon;
		public override void GetMyStats() {
			Effects = new() {
				new DamageClassChange(DamageClass.SummonMeleeSpeed),
				new MinionAttackTarget(),
				new OnHitTargetDebuffEffect(BuffID.Frostburn, BuffDuration)
			};

			if (EnchantmentTier >= 3)
				Effects.Add(new OnHitPlayerBuffEffect(BuffID.CoolWhipPlayerBuff, BuffDuration));

			if (EnchantmentTier == 4)
				Effects.Add(new OnHitTargetDebuffEffect(BuffID.RainbowWhipNPCDebuff, BuffDuration));

			//AddEStat(EnchantmentTypeName, 0f, 1f, 0f, EnchantmentStrength);
			//Debuff.Add(BuffID.Frostburn, BuffDuration);
			//AddEStat("Damage", 0f, EnchantmentStrength);
			//if (EnchantmentTier == 3)
			//	OnHitBuff.Add(BuffID.CoolWhipPlayerBuff, BuffDuration);

			//if (EnchantmentTier == 4)
			//	Debuff.Add(BuffID.RainbowWhipNPCDebuff, BuffDuration);


			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Weapons, 1f }
			};
		}

		public override string Artist => "Zorutan";
		public override string Designer => "andro951";
	}
	public class ColdSteelEnchantmentBasic : ColdSteelEnchantment { }
	public class ColdSteelEnchantmentCommon : ColdSteelEnchantment { }
	public class ColdSteelEnchantmentRare : ColdSteelEnchantment { }
	public class ColdSteelEnchantmentSuperRare : ColdSteelEnchantment { }
	public class ColdSteelEnchantmentUltraRare : ColdSteelEnchantment { }
}
