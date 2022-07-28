using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common;

namespace WeaponEnchantments.Items.Enchantments.Unique
{
	public abstract class HellsWrathEnchantment : Enchantment
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
			Debuff.Add(BuffID.FlameWhipEnemyDebuff, BuffDuration);
			if (EnchantmentTier == 4)
				Debuff.Add(BuffID.RainbowWhipNPCDebuff, BuffDuration);

			Debuff.Add(EnchantmentTier == 3 ? BuffID.OnFire3 : BuffID.OnFire, BuffDuration);
			AddEStat("Damage", 0f, EnchantmentStrength);
		}

		public override string Artist => "Zorutan";
		public override string Designer => "andro951";
	}
	public class HellsWrathEnchantmentBasic : HellsWrathEnchantment { }
	public class HellsWrathEnchantmentCommon : HellsWrathEnchantment { }
	public class HellsWrathEnchantmentRare : HellsWrathEnchantment { }
	public class HellsWrathEnchantmentSuperRare : HellsWrathEnchantment { }
	public class HellsWrathEnchantmentUltraRare : HellsWrathEnchantment { }
}
