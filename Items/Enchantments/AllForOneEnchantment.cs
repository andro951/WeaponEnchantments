using System;
using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class AllForOneEnchantment : Enchantment
	{
		public override int StrengthGroup => 6;
		public override float ScalePercent => 0.8f;
		public override bool Max1 => true;
		public override int RestrictedClass => (int)DamageClassID.Summon;
		public override SellCondition SellCondition => SellCondition.AnyTimeRare;
		public override void GetMyStats() {
			Effects = new() {
				new AllForOne(EnchantmentStrengthData * 0.4f + 4f),
				new DamageAfterDefenses(multiplicative: EnchantmentStrengthData),
				new NPCHitCooldown(multiplicative: EnchantmentStrengthData * 0.4f + 4f),
				new AttackSpeed(multiplicative: (EnchantmentStrengthData * 0.1f + 1f).Invert()),
				new ManaUsage(@base: EnchantmentStrengthData * 0.15f + 1.5f),
				new AutoReuse(prevent: true)
			};

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Weapons, 1f }
			};
		}

		public override string ShortTooltip => $"{Math.Round(EnchantmentStrength * AllowedListMultiplier, 3)}x {EnchantmentTypeName.AddSpaces()}";
		public override string Artist => "Zorutan";
		public override string Designer => "andro951";
	}
	public class AllForOneEnchantmentBasic : AllForOneEnchantment
	{
		public override List<WeightedPair> NpcDropTypes => new() {
			new(NPCID.Mothron)
		};
		public override SortedDictionary<ChestID, float> ChestDrops => new() {
			{ ChestID.Gold_Locked, 1f },
			{ ChestID.Lihzahrd, 1f }
		};
		public override List<WeightedPair> CrateDrops => new() {
			new(CrateID.Golden_LockBox, 0.45f)
		};
	}
	public class AllForOneEnchantmentCommon : AllForOneEnchantment { }
	public class AllForOneEnchantmentRare : AllForOneEnchantment { }
	public class AllForOneEnchantmentEpic : AllForOneEnchantment { }
	public class AllForOneEnchantmentLegendary : AllForOneEnchantment { }
}
