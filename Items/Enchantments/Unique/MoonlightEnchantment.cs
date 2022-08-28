using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using static WeaponEnchantments.Common.EnchantingRarity;

namespace WeaponEnchantments.Items.Enchantments.Unique
{
	public abstract class MoonlightEnchantment : Enchantment
	{
		public override int StrengthGroup => 9;
		public override float ScalePercent => 0.2f / defaultEnchantmentStrengths[StrengthGroup].enchantmentTierStrength[tierNames.Length - 1];
		public override int RestrictedClass => (int)DamageTypeSpecificID.Summon;
		public override void GetMyStats() {
			Effects = new() {
				new DamageAfterDefenses(multiplicative: EnchantmentStrengthData),
				new DamageClassChange(DamageClass.SummonMeleeSpeed),
				new MinionAttackTarget()
			};

			if (EnchantmentTier >= 2)
				Effects.Add(new BuffEffect(BuffID.ScytheWhipPlayerBuff, BuffStyle.OnHitPlayerBuff, BuffDuration));

			if (EnchantmentTier >= 3)
				Effects.Add(new BuffEffect(BuffID.ScytheWhipEnemyDebuff, BuffStyle.OnHitEnemyDebuff, BuffDuration));

			if (EnchantmentTier == 4)
				Effects.Add(new BuffEffect(BuffID.RainbowWhipNPCDebuff, BuffStyle.OnHitEnemyDebuff, BuffDuration));

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Weapons, 1f }
			};
		}

		public override string Artist => "Zorutan";
		public override string Designer => "andro951";
	}
	public class MoonlightEnchantmentBasic : MoonlightEnchantment
	{
		public override List<WeightedPair> NpcDropTypes => new() {
			new(NPCID.CultistBoss)
		};
	}
	public class MoonlightEnchantmentCommon : MoonlightEnchantment { }
	public class MoonlightEnchantmentRare : MoonlightEnchantment { }
	public class MoonlightEnchantmentSuperRare : MoonlightEnchantment { }
	public class MoonlightEnchantmentUltraRare : MoonlightEnchantment { }
}
