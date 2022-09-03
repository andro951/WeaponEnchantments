using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Utility
{
	public abstract class OnTickPlayerBuffEnchantment : Enchantment
	{
		public override int EnchantmentValueTierReduction => -2;
		public override int LowestCraftableTier => 5;
		protected abstract int buffID { get; }
		public override bool Max1 => true;
		public override float CapacityCostMultiplier => 1;
		public override SellCondition SellCondition => SellCondition.AnyTimeRare;
		public override void GetMyStats() {
			Effects = new() {
				new BuffEffect(buffID, BuffStyle.OnTickPlayerBuff)
			};

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Weapons, 1f },
				{ EItemType.Armor, 1f },
				{ EItemType.Accessories, 1f },
				{ EItemType.FishingPoles, 1f },
				{ EItemType.Tools, 1f }
			};
		}
		public override List<WeightedPair> NpcAIDrops => new() {
			new(NPCAIStyleID.Mimic)
		};
		public override SortedDictionary<ChestID, float> ChestDrops => new() {
			{ ChestID.Gold, 1f },
			{ ChestID.Gold_DeadMans, 1f }
		};
		public override List<WeightedPair> CrateDrops => new() {
			new(CrateID.Iron, 0.5f),
			new(CrateID.Iron, 0.5f)
		};

		public override string ShortTooltip => $"Passively grants {EnchantmentTypeName.AddSpaces()}";
		public override string Artist => "Zorutan";
		public override string Designer => "andro951";
	}

	public class DangerSenseEnchantmentLegendary : OnTickPlayerBuffEnchantment
	{
		protected override int buffID => BuffID.Dangersense;
	}
	public class HunterEnchantmentLegendary : OnTickPlayerBuffEnchantment
	{
		protected override int buffID => BuffID.Hunter;
	}
	public class ObsidianSkinEnchantmentLegendary : OnTickPlayerBuffEnchantment
	{
		protected override int buffID => BuffID.ObsidianSkin;
	}
	public class SpelunkerEnchantmentLegendary : OnTickPlayerBuffEnchantment
	{
		protected override int buffID => BuffID.Spelunker;
	}
	public class FishingEnchantmentLegendary : OnTickPlayerBuffEnchantment
	{
		protected override int buffID => BuffID.Fishing;
		public override List<WeightedPair> NpcAIDrops => new() {
			new(NPCAIStyleID.Mimic, 0.5f)
		};
		public override SortedDictionary<ChestID, float> ChestDrops => new() {
			{ ChestID.Gold, 0.5f },
			{ ChestID.Gold_DeadMans, 0.5f }
		};
		public override List<WeightedPair> CrateDrops => new() {
			new(CrateID.Iron, 1f),
			new(CrateID.Iron, 1f)
		};
		public override string Artist => "andro951";
	}
	public class CrateEnchantmentLegendary : OnTickPlayerBuffEnchantment
	{
		protected override int buffID => BuffID.Crate;
		public override List<WeightedPair> NpcAIDrops => new() {
			new(NPCAIStyleID.Mimic, 0.5f)
		};
		public override SortedDictionary<ChestID, float> ChestDrops => new() {
			{ ChestID.Gold, 0.5f },
			{ ChestID.Gold_DeadMans, 0.5f }
		};
		public override List<WeightedPair> CrateDrops => new() {
			new(CrateID.Iron, 1f),
			new(CrateID.Iron, 1f)
		};
		public override string Artist => "andro951";
	}
	public class SonarEnchantmentLegendary : OnTickPlayerBuffEnchantment
	{
		protected override int buffID => BuffID.Sonar;
		public override List<WeightedPair> NpcAIDrops => new() {
			new(NPCAIStyleID.Mimic, 0.5f)
		};
		public override SortedDictionary<ChestID, float> ChestDrops => new() {
			{ ChestID.Gold, 0.5f },
			{ ChestID.Gold_DeadMans, 0.5f }
		};
		public override List<WeightedPair> CrateDrops => new() {
			new(CrateID.Iron, 1f),
			new(CrateID.Iron, 1f)
		};
		public override string Artist => "andro951";
	}
}
