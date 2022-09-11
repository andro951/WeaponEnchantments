using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using WeaponEnchantments.Common.Configs;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Utility
{
	public abstract class OnTickPlayerBuffEnchantment : Enchantment
	{
		public override int StrengthGroup => 19;
		//public override int EnchantmentValueTierReduction => -2;
		//public override int LowestCraftableTier => 5;
		protected abstract int buffID { get; }
		//public override bool Max1 => true;
		//public override float CapacityCostMultiplier => 1;
		public override float ScalePercent => 0f;
		public override SellCondition SellCondition => SellCondition.AnyTimeRare;
		public override void GetMyStats() {
			Effects = new() {
				new BuffEffect(buffID, BuffStyle.OnTickPlayerBuff, duration: 12 * (uint)WEMod.serverConfig.BuffDuration, chance: EnchantmentStrengthData)
			};

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Weapons, 1f },
				{ EItemType.Armor, 1f },
				{ EItemType.Accessories, 1f },
				{ EItemType.FishingPoles, 1f },
				{ EItemType.Tools, 1f }
			};
		}

		public override string ShortTooltip => $"Passively grants {EnchantmentTypeName.AddSpaces()} for {new Time((uint)EnchantmentStrength * 12 * (uint)WEMod.serverConfig.BuffDuration)} every {ConfigValues.BuffDurationTicks}.";
		public override string ArtModifiedBy => null;
		public override string Designer => "andro951";
	}

	public abstract class DangerSenseEnchantment : OnTickPlayerBuffEnchantment {
		protected override int buffID => BuffID.Dangersense;
		public override string Artist => "Zorutan";
	}
	public class DangerSenseEnchantmentBasic : DangerSenseEnchantment
	{
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
	}
	public class DangerSenseEnchantmentCommon : DangerSenseEnchantment { }
	public class DangerSenseEnchantmentRare : DangerSenseEnchantment { }
	public class DangerSenseEnchantmentEpic : DangerSenseEnchantment { }
	public class DangerSenseEnchantmentLegendary : DangerSenseEnchantment { }
	public abstract class HunterEnchantment : OnTickPlayerBuffEnchantment
	{
		protected override int buffID => BuffID.Hunter;
		public override string Artist => "Zorutan";
	}
	public class HunterEnchantmentBasic : HunterEnchantment
	{
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
	}
	public class HunterEnchantmentCommon : HunterEnchantment { }
	public class HunterEnchantmentRare : HunterEnchantment { }
	public class HunterEnchantmentEpic : HunterEnchantment { }
	public class HunterEnchantmentLegendary : HunterEnchantment { }
	public abstract class ObsidianSkinEnchantment : OnTickPlayerBuffEnchantment
	{
		protected override int buffID => BuffID.ObsidianSkin;
		public override string Artist => "Zorutan";
	}
	public class ObsidianSkinEnchantmentBasic : ObsidianSkinEnchantment
	{
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
	}
	public class ObsidianSkinEnchantmentCommon : ObsidianSkinEnchantment { }
	public class ObsidianSkinEnchantmentRare : ObsidianSkinEnchantment { }
	public class ObsidianSkinEnchantmentEpic : ObsidianSkinEnchantment { }
	public class ObsidianSkinEnchantmentLegendary : ObsidianSkinEnchantment { }
	public abstract class SpelunkerEnchantment : OnTickPlayerBuffEnchantment
	{
		protected override int buffID => BuffID.Spelunker;
		public override string Artist => "Zorutan";
	}
	public class SpelunkerEnchantmentBasic : SpelunkerEnchantment
	{
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
	}
	public class SpelunkerEnchantmentCommon : SpelunkerEnchantment { }
	public class SpelunkerEnchantmentRare : SpelunkerEnchantment { }
	public class SpelunkerEnchantmentEpic : SpelunkerEnchantment { }
	public class SpelunkerEnchantmentLegendary : SpelunkerEnchantment { }

	public abstract class FishingEnchantment : OnTickPlayerBuffEnchantment
	{
		protected override int buffID => BuffID.Fishing;
		public override string Artist => "andro951";
	}
	public class FishingEnchantmentBasic : FishingEnchantment
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
	}
	public class FishingEnchantmentCommon : FishingEnchantment { }
	public class FishingEnchantmentRare : FishingEnchantment { }
	public class FishingEnchantmentEpic : FishingEnchantment { }
	public class FishingEnchantmentLegendary : FishingEnchantment { }
	public abstract class CrateEnchantment : OnTickPlayerBuffEnchantment
	{
		protected override int buffID => BuffID.Crate;
		public override string Artist => "andro951";
	}
	public class CrateEnchantmentBasic : CrateEnchantment
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
	}
	public class CrateEnchantmentCommon : CrateEnchantment { }
	public class CrateEnchantmentRare : CrateEnchantment { }
	public class CrateEnchantmentEpic : CrateEnchantment { }
	public class CrateEnchantmentLegendary : CrateEnchantment { }
	public abstract class SonarEnchantment : OnTickPlayerBuffEnchantment
	{
		protected override int buffID => BuffID.Sonar;
		public override string Artist => "andro951";
	}
	public class SonarEnchantmentBasic : SonarEnchantment
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
	}
	public class SonarEnchantmentCommon : SonarEnchantment { }
	public class SonarEnchantmentRare : SonarEnchantment { }
	public class SonarEnchantmentEpic : SonarEnchantment { }
	public class SonarEnchantmentLegendary : SonarEnchantment { }
}
