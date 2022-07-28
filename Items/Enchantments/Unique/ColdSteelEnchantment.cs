using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common;

namespace WeaponEnchantments.Items.Enchantments.Unique
{
	public abstract class ColdSteelEnchantment : Enchantment
	{
		public override int StrengthGroup => 9;
		public override float ScalePercent => 0.2f / defaultEnchantmentStrengths[StrengthGroup].enchantmentTierStrength[rarity.Length - 1];
		public override DamageClass RestrictedClass => DamageClass.Summon;
		public override DamageClass NewDamageType => DamageClass.SummonMeleeSpeed;
		public override Dictionary<string, float> AllowedList => new Dictionary<string, float>() {
			{ "Weapon", 1f }
		};
		public override void GetMyStats() {
			AddEStat(EnchantmentTypeName, 0f, 1f, 0f, EnchantmentStrength);
			if (EnchantmentTier == 3)
				OnHitBuff.Add(BuffID.CoolWhipPlayerBuff, BuffDuration);

			if (EnchantmentTier == 4)
				Debuff.Add(BuffID.RainbowWhipNPCDebuff, BuffDuration);

			Debuff.Add(BuffID.Frostburn, BuffDuration);
			AddEStat("Damage", 0f, EnchantmentStrength);
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
