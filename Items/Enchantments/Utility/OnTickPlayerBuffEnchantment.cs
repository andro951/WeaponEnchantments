using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using WeaponEnchantments.Common.Configs;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using androLib.Common.Utility;
using Terraria.ModLoader;

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
		public override SellCondition SellCondition => EnchantmentTier == 0 ? SellCondition.AnyTimeRare : SellCondition.Never;
		public override void GetMyStats() {
			Effects = new() {
				new BuffEffect(buffID, BuffStyle.OnTickPlayerBuff, duration: (uint)(EnchantmentStrength * (12 * WEMod.serverConfig.BuffDuration)))
			};

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Weapons, 1f },
				{ EItemType.Armor, 1f },
				{ EItemType.Accessories, 1f },
				{ EItemType.FishingPoles, 1f },
				{ EItemType.Tools, 1f }
			};
		}

		public override string ShortTooltip => $"{BuffStyle.OnTickPlayerBuff}".Lang_WE(L_ID1.Tooltip, L_ID2.EnchantmentShortTooltip, new object[] { GetLocalizationTypeName(), (new Time((uint)(EnchantmentStrength * 12 * WEMod.serverConfig.BuffDuration))).ToString(), ConfigValues.BuffDurationTicks.ToString() });
		public override string ArtModifiedBy => null;
		public override string Designer => "andro951";
	}

	public abstract class DangerSenseEnchantment : OnTickPlayerBuffEnchantment {
		protected override int buffID => BuffID.Dangersense;
		public override string Artist => "Zorutan";
	}
	[Autoload(false)]
	public class DangerSenseEnchantmentBasic : DangerSenseEnchantment
	{
		public override List<DropData> NpcAIDrops => new() {
			new(NPCAIStyleID.Mimic)
		};
		public override List<DropData> ChestDrops => new() {
			new(ChestID.Gold),
			new(ChestID.Gold_DeadMans)
		};
		public override List<DropData> CrateDrops => new() {
			new(CrateID.Iron, 0.25f),
			new(CrateID.Mythril_IronHard, 0.25f)
		};
	}
	[Autoload(false)]
	public class DangerSenseEnchantmentCommon : DangerSenseEnchantment { }
	[Autoload(false)]
	public class DangerSenseEnchantmentRare : DangerSenseEnchantment { }
	[Autoload(false)]
	public class DangerSenseEnchantmentEpic : DangerSenseEnchantment { }
	[Autoload(false)]
	public class DangerSenseEnchantmentLegendary : DangerSenseEnchantment { }
	public abstract class HunterEnchantment : OnTickPlayerBuffEnchantment
	{
		protected override int buffID => BuffID.Hunter;
		public override string Artist => "Zorutan";
	}
	[Autoload(false)]
	public class HunterEnchantmentBasic : HunterEnchantment
	{
		public override List<DropData> NpcAIDrops => new() {
			new(NPCAIStyleID.Mimic)
		};
		public override List<DropData> ChestDrops => new() {
			new(ChestID.Gold),
			new(ChestID.Gold_DeadMans)
		};
		public override List<DropData> CrateDrops => new() {
			new(CrateID.Iron, 0.25f),
			new(CrateID.Mythril_IronHard, 0.25f)
		};
	}
	[Autoload(false)]
	public class HunterEnchantmentCommon : HunterEnchantment { }
	[Autoload(false)]
	public class HunterEnchantmentRare : HunterEnchantment { }
	[Autoload(false)]
	public class HunterEnchantmentEpic : HunterEnchantment { }
	[Autoload(false)]
	public class HunterEnchantmentLegendary : HunterEnchantment { }
	public abstract class ObsidianSkinEnchantment : OnTickPlayerBuffEnchantment
	{
		protected override int buffID => BuffID.ObsidianSkin;
		public override string Artist => "Zorutan";
	}
	[Autoload(false)]
	public class ObsidianSkinEnchantmentBasic : ObsidianSkinEnchantment
	{
		public override List<DropData> NpcAIDrops => new() {
			new(NPCAIStyleID.Mimic)
		};
		public override List<DropData> ChestDrops => new() {
			new(ChestID.Gold),
			new(ChestID.Gold_DeadMans)
		};
		public override List<DropData> CrateDrops => new() {
			new(CrateID.Iron, 0.25f),
			new(CrateID.Mythril_IronHard, 0.25f)
		};
	}
	[Autoload(false)]
	public class ObsidianSkinEnchantmentCommon : ObsidianSkinEnchantment { }
	[Autoload(false)]
	public class ObsidianSkinEnchantmentRare : ObsidianSkinEnchantment { }
	[Autoload(false)]
	public class ObsidianSkinEnchantmentEpic : ObsidianSkinEnchantment { }
	[Autoload(false)]
	public class ObsidianSkinEnchantmentLegendary : ObsidianSkinEnchantment { }
	public abstract class SpelunkerEnchantment : OnTickPlayerBuffEnchantment
	{
		protected override int buffID => BuffID.Spelunker;
		public override string Artist => "Zorutan";
	}
	[Autoload(false)]
	public class SpelunkerEnchantmentBasic : SpelunkerEnchantment
	{
		public override List<DropData> NpcAIDrops => new() {
			new(NPCAIStyleID.Mimic)
		};
		public override List<DropData> ChestDrops => new() {
			new(ChestID.Gold),
			new(ChestID.Gold_DeadMans)
		};
		public override List<DropData> CrateDrops => new() {
			new(CrateID.Iron, 0.25f),
			new(CrateID.Mythril_IronHard, 0.25f)
		};
	}
	[Autoload(false)]
	public class SpelunkerEnchantmentCommon : SpelunkerEnchantment { }
	[Autoload(false)]
	public class SpelunkerEnchantmentRare : SpelunkerEnchantment { }
	[Autoload(false)]
	public class SpelunkerEnchantmentEpic : SpelunkerEnchantment { }
	[Autoload(false)]
	public class SpelunkerEnchantmentLegendary : SpelunkerEnchantment { }

	public abstract class FishingEnchantment : OnTickPlayerBuffEnchantment
	{
		protected override int buffID => BuffID.Fishing;
		public override string Artist => "andro951";
	}
	[Autoload(false)]
	public class FishingEnchantmentBasic : FishingEnchantment
	{
		public override List<DropData> NpcAIDrops => new() {
			new(NPCAIStyleID.Mimic, 0.5f)
		};
		public override List<DropData> ChestDrops => new() {
			new(ChestID.Gold, 0.5f),
			new(ChestID.Gold_DeadMans, 0.5f),
			new(ChestID.Water, 0.5f)
		};
		public override List<DropData> CrateDrops => new() {
			new(CrateID.Azure_SkyHard, 0.25f),
			new(CrateID.Boreal_FrozenHard, 0.25f),
			new(CrateID.Bramble_JungleHard, 0.25f),
			new(CrateID.Corrupt, 0.25f),
			new(CrateID.Crimson, 0.25f),
			new(CrateID.Defiled_CorruptHard, 0.25f),
			new(CrateID.Divine_HallowedHard, 0.25f),
			new(CrateID.Dungeon, 0.25f),
			new(CrateID.Frozen, 0.25f),
			new(CrateID.Golden_LockBox, 0.25f),
			new(CrateID.Hallowed, 0.25f),
			new(CrateID.Hellstone_ObsidianHard, 0.25f),
			new(CrateID.Hematic_CrimsonHard, 0.25f),
			new(CrateID.Iron, 0.5f),
			new(CrateID.Jungle, 0.25f),
			new(CrateID.Mirage_OasisHard, 0.25f),
			new(CrateID.Mythril_IronHard, 0.5f),
			new(CrateID.Oasis, 0.25f),
			new(CrateID.Obsidian, 0.25f),
			new(CrateID.Obsidian_LockBox, 0.25f),
			new(CrateID.Ocean, 0.25f),
			new(CrateID.Seaside_OceanHard, 0.25f),
			new(CrateID.Sky, 0.25f),
			new(CrateID.Stockade_DungeonHard, 0.25f),
			new(CrateID.Golden, chance: 0.025f),
			new(CrateID.Titanium_GoldenHard, chance: 0.025f),
		};
	}
	[Autoload(false)]
	public class FishingEnchantmentCommon : FishingEnchantment { }
	[Autoload(false)]
	public class FishingEnchantmentRare : FishingEnchantment { }
	[Autoload(false)]
	public class FishingEnchantmentEpic : FishingEnchantment { }
	[Autoload(false)]
	public class FishingEnchantmentLegendary : FishingEnchantment { }
	public abstract class CrateEnchantment : OnTickPlayerBuffEnchantment
	{
		protected override int buffID => BuffID.Crate;
		public override string Artist => "andro951";
	}
	[Autoload(false)]
	public class CrateEnchantmentBasic : CrateEnchantment
	{
		public override List<DropData> NpcAIDrops => new() {
			new(NPCAIStyleID.Mimic, 0.5f)
		};
		public override List<DropData> ChestDrops => new() {
			new(ChestID.Gold, 0.5f),
			new(ChestID.Gold_DeadMans, 0.5f),
			new(ChestID.Water, 0.5f)
		};
		public override List<DropData> CrateDrops => new() {
			new(CrateID.Azure_SkyHard, 0.25f),
			new(CrateID.Boreal_FrozenHard, 0.25f),
			new(CrateID.Bramble_JungleHard, 0.25f),
			new(CrateID.Corrupt, 0.25f),
			new(CrateID.Crimson, 0.25f),
			new(CrateID.Defiled_CorruptHard, 0.25f),
			new(CrateID.Divine_HallowedHard, 0.25f),
			new(CrateID.Dungeon, 0.25f),
			new(CrateID.Frozen, 0.25f),
			new(CrateID.Golden_LockBox, 0.25f),
			new(CrateID.Hallowed, 0.25f),
			new(CrateID.Hellstone_ObsidianHard, 0.25f),
			new(CrateID.Hematic_CrimsonHard, 0.25f),
			new(CrateID.Iron, 0.5f),
			new(CrateID.Jungle, 0.25f),
			new(CrateID.Mirage_OasisHard, 0.25f),
			new(CrateID.Mythril_IronHard, 0.5f),
			new(CrateID.Oasis, 0.25f),
			new(CrateID.Obsidian, 0.25f),
			new(CrateID.Obsidian_LockBox, 0.25f),
			new(CrateID.Ocean, 0.25f),
			new(CrateID.Seaside_OceanHard, 0.25f),
			new(CrateID.Sky, 0.25f),
			new(CrateID.Stockade_DungeonHard, 0.25f),
			new(CrateID.Golden, chance: 0.025f),
			new(CrateID.Titanium_GoldenHard, chance: 0.025f),
		};
	}
	[Autoload(false)]
	public class CrateEnchantmentCommon : CrateEnchantment { }
	[Autoload(false)]
	public class CrateEnchantmentRare : CrateEnchantment { }
	[Autoload(false)]
	public class CrateEnchantmentEpic : CrateEnchantment { }
	[Autoload(false)]
	public class CrateEnchantmentLegendary : CrateEnchantment { }
	public abstract class SonarEnchantment : OnTickPlayerBuffEnchantment
	{
		protected override int buffID => BuffID.Sonar;
		public override string Artist => "andro951";
	}
	[Autoload(false)]
	public class SonarEnchantmentBasic : SonarEnchantment
	{
		public override List<DropData> NpcAIDrops => new() {
			new(NPCAIStyleID.Mimic, 0.5f)
		};
		public override List<DropData> ChestDrops => new() {
			new(ChestID.Gold, 0.5f),
			new(ChestID.Gold_DeadMans, 0.5f),
			new(ChestID.Water, 0.5f)
		};
		public override List<DropData> CrateDrops => new() {
			new(CrateID.Azure_SkyHard, 0.25f),
			new(CrateID.Boreal_FrozenHard, 0.25f),
			new(CrateID.Bramble_JungleHard, 0.25f),
			new(CrateID.Corrupt, 0.25f),
			new(CrateID.Crimson, 0.25f),
			new(CrateID.Defiled_CorruptHard, 0.25f),
			new(CrateID.Divine_HallowedHard, 0.25f),
			new(CrateID.Dungeon, 0.25f),
			new(CrateID.Frozen, 0.25f),
			new(CrateID.Golden_LockBox, 0.25f),
			new(CrateID.Hallowed, 0.25f),
			new(CrateID.Hellstone_ObsidianHard, 0.25f),
			new(CrateID.Hematic_CrimsonHard, 0.25f),
			new(CrateID.Iron, 0.5f),
			new(CrateID.Jungle, 0.25f),
			new(CrateID.Mirage_OasisHard, 0.25f),
			new(CrateID.Mythril_IronHard, 0.5f),
			new(CrateID.Oasis, 0.25f),
			new(CrateID.Obsidian, 0.25f),
			new(CrateID.Obsidian_LockBox, 0.25f),
			new(CrateID.Ocean, 0.25f),
			new(CrateID.Seaside_OceanHard, 0.25f),
			new(CrateID.Sky, 0.25f),
			new(CrateID.Stockade_DungeonHard, 0.25f),
			new(CrateID.Golden, chance: 0.025f),
			new(CrateID.Titanium_GoldenHard, chance: 0.025f),
		};
	}
	[Autoload(false)]
	public class SonarEnchantmentCommon : SonarEnchantment { }
	[Autoload(false)]
	public class SonarEnchantmentRare : SonarEnchantment { }
	[Autoload(false)]
	public class SonarEnchantmentEpic : SonarEnchantment { }
	[Autoload(false)]
	public class SonarEnchantmentLegendary : SonarEnchantment { }
	public abstract class ShineEnchantment : OnTickPlayerBuffEnchantment
	{
		protected override int buffID => BuffID.Shine;
		public override string Artist => "andro951";
	}
	[Autoload(false)]
	public class ShineEnchantmentBasic : ShineEnchantment
	{
		public override List<DropData> NpcDropTypes => new() {
			new(NPCID.UndeadMiner, chance: 1f)
		};
		public override List<DropData> ChestDrops => new() {
			new(ChestID.Chest_Normal)
		};
		public override List<DropData> CrateDrops => new() {
			new(CrateID.Wooden, 0.25f),
			new(CrateID.Pearlwood_WoodenHard, 0.25f)
		};
	}
	[Autoload(false)]
	public class ShineEnchantmentCommon : ShineEnchantment { }
	[Autoload(false)]
	public class ShineEnchantmentRare : ShineEnchantment { }
	[Autoload(false)]
	public class ShineEnchantmentEpic : ShineEnchantment { }
	[Autoload(false)]
	public class ShineEnchantmentLegendary : ShineEnchantment { }
}
