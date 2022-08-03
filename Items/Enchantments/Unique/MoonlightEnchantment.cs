using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Common;

namespace WeaponEnchantments.Items.Enchantments.Unique
{
	public abstract class MoonlightEnchantment : Enchantment
	{
		public override int StrengthGroup => 9;
		public override float ScalePercent => 0.2f / defaultEnchantmentStrengths[StrengthGroup].enchantmentTierStrength[rarity.Length - 1];
		public override int RestrictedClass => (int)DamageTypeSpecificID.Summon;
		public override int NewDamageType => (int)DamageTypeSpecificID.SummonMeleeSpeed;
		public override Dictionary<EItemType, float> AllowedList => new Dictionary<EItemType, float>() {
			{ EItemType.Weapon, 1f }
		};
		public override void GetMyStats() {
			AddEStat(EnchantmentTypeName, 0f, 1f, 0f, EnchantmentStrength);
			OnHitBuff.Add(BuffID.ScytheWhipPlayerBuff, BuffDuration);
			if (EnchantmentTier == 3)
				Debuff.Add(BuffID.ScytheWhipEnemyDebuff, BuffDuration);

			if (EnchantmentTier == 4)
				Debuff.Add(BuffID.RainbowWhipNPCDebuff, BuffDuration);

			AddEStat("Damage", 0f, EnchantmentStrength);
		}

		public override string Artist => "Zorutan";
		public override string Designer => "andro951";
	}
	public class MoonlightEnchantmentBasic : MoonlightEnchantment { }
	public class MoonlightEnchantmentCommon : MoonlightEnchantment { }
	public class MoonlightEnchantmentRare : MoonlightEnchantment { }
	public class MoonlightEnchantmentSuperRare : MoonlightEnchantment { }
	public class MoonlightEnchantmentUltraRare : MoonlightEnchantment { }
}
