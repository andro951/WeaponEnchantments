using System.Collections.Generic;
using androLib.Common.Utility;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Configs;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments.Utility
{
	public abstract class OnTickPlayerBuffEnchantment : Enchantment
	{
		public override int StrengthGroup => 19;
		protected abstract int buffID { get; }
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
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<EnchantmentToggle>().OnTick;
		}
	}

	#region Vanilla buffs
	#region Dangersense
	public abstract class DangerSenseEnchantment : OnTickPlayerBuffEnchantment {
		protected override string TypeName => "Dangersense";
		protected override string NamePrefix => "Enchantments/";
		
		protected override int buffID => BuffID.Dangersense;
		public override string Artist => "Zorutan";
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<EnchantmentToggle>().Dangersense;
		}
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
	#endregion
	
	#region Hunter
	public abstract class HunterEnchantment : OnTickPlayerBuffEnchantment
	{
		protected override string TypeName => "Hunter";
		protected override string NamePrefix => "Enchantments/";
		
		protected override int buffID => BuffID.Hunter;
		public override string Artist => "Zorutan";
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<EnchantmentToggle>().Hunter;
		}
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
	#endregion
	
	#region Obsidian Skin
	public abstract class ObsidianSkinEnchantment : OnTickPlayerBuffEnchantment
	{
		protected override string TypeName => "ObsidianSkin";
		protected override string NamePrefix => "Enchantments/";
		
		protected override int buffID => BuffID.ObsidianSkin;
		public override string Artist => "Zorutan";
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<EnchantmentToggle>().ObsidianSkin;
		}
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
	#endregion
	
	#region Spelunker
	public abstract class SpelunkerEnchantment : OnTickPlayerBuffEnchantment
	{
		protected override string TypeName => "Spelunker";
		protected override string NamePrefix => "Enchantments/";
		
		protected override int buffID => BuffID.Spelunker;
		public override string Artist => "Zorutan";
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<EnchantmentToggle>().Spelunker;
		}
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
	#endregion
	
	#region Fishing
	public abstract class FishingEnchantment : OnTickPlayerBuffEnchantment
	{
		protected override string TypeName => "Fishing";
		protected override string NamePrefix => "Enchantments/";
		
		protected override int buffID => BuffID.Fishing;
		public override string Artist => "andro951";
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<EnchantmentToggle>().Fishing;
		}
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
	#endregion
	
	#region Crate
	public abstract class CrateEnchantment : OnTickPlayerBuffEnchantment
	{
		protected override string TypeName => "Enchantments/Crate";
		protected override string NamePrefix => "Enchantments/";
		
		protected override int buffID => BuffID.Crate;
		public override string Artist => "andro951";
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<EnchantmentToggle>().Crate;
		}
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
	#endregion
	
	#region Sonar
	public abstract class SonarEnchantment : OnTickPlayerBuffEnchantment
	{
		protected override string TypeName => "Enchantments/Sonar";
		protected override string NamePrefix => "Enchantments/";
		
		protected override int buffID => BuffID.Sonar;
		public override string Artist => "andro951";
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<EnchantmentToggle>().Sonar;
		}
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
	#endregion
	
	#region Shine
	public abstract class ShineEnchantment : OnTickPlayerBuffEnchantment
	{
		protected override string TypeName => "Enchantments/Shine";
		protected override string NamePrefix => "Enchantments/";
		
		protected override int buffID => BuffID.Shine;
		public override string Artist => "andro951";
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<EnchantmentToggle>().Shine;
		}
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
	#endregion
	#endregion
	
	#region Modded buffs
	#region The Depths
	[JITWhenModsEnabled("TheDepths")]
	public abstract class DepthsCrystalSkinEnchantment : OnTickPlayerBuffEnchantment 
	{
		protected override string TypeName => "CrystalSkin";
		protected override string NamePrefix => "ModSupport/TheDepths/";
		public override string Texture => $"WeaponEnchantments/Items/Sprites/{NamePrefix}{TypeName}/{Name.Replace("Depths"+TypeName+"Enchantment", string.Empty)}";
    
		protected override int buffID => ModContent.Find<ModBuff>("TheDepths/CrystalSkin").Type;
    
		public override string Artist => "Pixus";
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<EnchantmentToggle>().DepthsCrystalSkin && ModLoader.HasMod("TheDepths");
		}
	}

	[Autoload(false)]
	public class DepthsCrystalSkinEnchantmentBasic : DepthsCrystalSkinEnchantment
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
	public class DepthsCrystalSkinEnchantmentCommon : DepthsCrystalSkinEnchantment { }
	[Autoload(false)]
	public class DepthsCrystalSkinEnchantmentRare : DepthsCrystalSkinEnchantment { }
	[Autoload(false)]
	public class DepthsCrystalSkinEnchantmentEpic : DepthsCrystalSkinEnchantment { }
	[Autoload(false)]
	public class DepthsCrystalSkinEnchantmentLegendary : DepthsCrystalSkinEnchantment { }
	#endregion
	#endregion
}
