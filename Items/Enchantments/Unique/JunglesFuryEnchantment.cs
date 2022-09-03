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
		public override int RestrictedClass => (int)DamageClassID.Summon;
		public override SellCondition SellCondition => SellCondition.PostPlantera;
		public override void GetMyStats() {
			Effects = new() {
				new DamageAfterDefenses(multiplicative: EnchantmentStrengthData),
				new DamageClassSwap(DamageClass.SummonMeleeSpeed),
				new MinionAttackTarget(),
				new BuffEffect(EnchantmentTier >= 2 ? BuffID.Venom : BuffID.Poisoned, BuffStyle.OnHitEnemyDebuff, BuffDuration)
			};

			if (EnchantmentTier >= 3) {
				Effects.Add(new BuffEffect(BuffID.SwordWhipPlayerBuff, BuffStyle.OnHitPlayerBuff, BuffDuration));
				Effects.Add(new BuffEffect(BuffID.SwordWhipNPCDebuff, BuffStyle.OnHitEnemyDebuff, BuffDuration));
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
	public class JunglesFuryEnchantmentBasic : JunglesFuryEnchantment
	{
		public override List<WeightedPair> NpcDropTypes => new() {
			new(NPCID.Plantera)
		};
	}
	public class JunglesFuryEnchantmentCommon : JunglesFuryEnchantment { }
	public class JunglesFuryEnchantmentRare : JunglesFuryEnchantment { }
	public class JunglesFuryEnchantmentEpic : JunglesFuryEnchantment { }
	public class JunglesFuryEnchantmentLegendary : JunglesFuryEnchantment { }
}
