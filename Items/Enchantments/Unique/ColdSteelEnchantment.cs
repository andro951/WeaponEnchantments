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
		public override int RestrictedClass => (int)DamageClassID.Summon;
		public override SellCondition SellCondition => SellCondition.HardMode;
		public override void GetMyStats() {
			Effects = new() {
				new DamageAfterDefenses(multiplicative: EnchantmentStrengthData),
				new DamageClassSwap(DamageClass.SummonMeleeSpeed),
				new MinionAttackTarget(),
				new BuffEffect(BuffID.Frostburn, BuffStyle.OnHitEnemyDebuff, BuffDuration)
			};

			if (EnchantmentTier >= 3) {
				Effects.Add(new BuffEffect(BuffID.CoolWhipPlayerBuff, BuffStyle.OnHitPlayerBuff, BuffDuration));
				Effects.Add(new OnHitSpawnProjectile(ProjectileID.CoolWhipProj, 10));
			}

			if (EnchantmentTier == 4)
				Effects.Add(new BuffEffect(BuffID.RainbowWhipNPCDebuff, BuffStyle.OnHitEnemyDebuff, BuffDuration));

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Weapons, 1f }
			};
		}

		public override string Artist => "Zorutan";
		public override string Designer => "andro951";
	}
	public class ColdSteelEnchantmentBasic : ColdSteelEnchantment
	{
		public override List<WeightedPair> NpcDropTypes => new() {
			new(NPCID.SkeletronPrime)
		};
	}
	public class ColdSteelEnchantmentCommon : ColdSteelEnchantment { }
	public class ColdSteelEnchantmentRare : ColdSteelEnchantment { }
	public class ColdSteelEnchantmentSuperRare : ColdSteelEnchantment { }
	public class ColdSteelEnchantmentUltraRare : ColdSteelEnchantment { }
}
