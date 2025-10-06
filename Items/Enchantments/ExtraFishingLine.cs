using androLib.Common.Utility;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Configs;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class ExtraFishingLineEnchantment : Enchantment
	{
		protected override string TypeName => "ExtraFishingLine";
		protected override string NamePrefix => "Enchantments/";
		
		public override int StrengthGroup => 18;
		public override float ScalePercent => 2f/3f;
		public override void GetMyStats() {
			Effects = new() {
				new Multishot(@base: EnchantmentStrengthData)
			};

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.FishingPoles, 1f }
			};
		}
		protected override List<List<EnchantmentEffect>> cursedEffectPossibilities => defaultUtilityCursedEffectPossibilities;
		public override string Artist => "andro951";
		public override string ArtModifiedBy => null;
		public override string Designer => "andro951";
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<EnchantmentToggle>().ExtraFishingLine;
		}
	}
	[Autoload(false)]
	public class ExtraFishingLineEnchantmentBasic : ExtraFishingLineEnchantment
	{
		public override List<DropData> NpcDropTypes => new() {
			new(NPCID.FlyingFish, chance: 0.1f)
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
			new(CrateID.Iron, 0.25f),
			new(CrateID.Jungle, 0.25f),
			new(CrateID.Mirage_OasisHard, 0.25f),
			new(CrateID.Mythril_IronHard, 0.25f),
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
	public class ExtraFishingLineEnchantmentCommon : ExtraFishingLineEnchantment { }
	[Autoload(false)]
	public class ExtraFishingLineEnchantmentRare : ExtraFishingLineEnchantment { }
	[Autoload(false)]
	public class ExtraFishingLineEnchantmentEpic : ExtraFishingLineEnchantment { }
	[Autoload(false)]
	public class ExtraFishingLineEnchantmentLegendary : ExtraFishingLineEnchantment { }
	[Autoload(false)]
	public class ExtraFishingLineEnchantmentCursed : ExtraFishingLineEnchantment { }

}
