using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class MaxMinionsEnchantment : Enchantment
	{
		public override int StrengthGroup => 10;
		public override float ScalePercent => 0.6f;
		public override SellCondition SellCondition => SellCondition.AnyTimeRare;
		public override void GetMyStats() {
			Effects = new() {
				new MaxMinions(@base: EnchantmentStrengthData)
			};

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Armor, 1f },
				{ EItemType.Accessories, 1f }
			};
		}

		public override string ShortTooltip => GetShortTooltip(sign: true, percent: false, multiply100: false);
		public override string Artist => "𝐍𝐢𝐱𝐲♱";
		public override string Designer => "andro951";
	}
	public class MaxMinionsEnchantmentBasic : MaxMinionsEnchantment
	{
		public override List<WeightedPair> NpcDropTypes => new() {
			new(NPCID.QueenBee)
		};
		public override List<WeightedPair> NpcAIDrops => new() {
			new(NPCAIStyleID.Spider)
		};
		public override List<WeightedPair> CrateDrops => new() {
			new(CrateID.Oasis, 0.5f),
			new(CrateID.Mirage_OasisHard, 0.5f),
			new(CrateID.Jungle, 0.5f),
			new(CrateID.Bramble_JungleHard, 0.5f)
		};
	}
	public class MaxMinionsEnchantmentCommon : MaxMinionsEnchantment { }
	public class MaxMinionsEnchantmentRare : MaxMinionsEnchantment { }
	public class MaxMinionsEnchantmentSuperRare : MaxMinionsEnchantment { }
	public class MaxMinionsEnchantmentUltraRare : MaxMinionsEnchantment { }

}