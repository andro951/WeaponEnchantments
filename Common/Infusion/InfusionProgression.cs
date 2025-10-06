using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Common.Utility;
using System.Collections.Immutable;
using System.Collections;
using static WeaponEnchantments.Common.Globals.EnchantedItemStaticMethods;
using static WeaponEnchantments.Common.InfusionProgression;
using Mono.Cecil;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;
using Terraria.GameContent.ItemDropRules;
using static Humanizer.On;
using Ionic.Zlib;
using WeaponEnchantments.UI;
using androLib.Common.Utility;
using androLib.Common.Globals;
using WeaponEnchantments.Items;
using androLib;
using VacuumOreBag.Items;

namespace WeaponEnchantments.Common
{
	public enum ItemSourceType {
		Error = -1,
		Craft,
		NPCDrop,
		BossDrop,
		Gathering,
		Shop
	}
	public enum InfusionOffset {
		Error = -1,
		Consumable,
		EasyObtain,
		None,
		CraftFromWeapon
	}
	public enum ProgressionGroupID {
		None = -1,
		#region Vanilla
		ForestPreHardMode,
		Presents,
		Desert,
		CopperOre,
		TinOre,
		MerchantShop,
		GiantTree,
		NormalChest,
		ForestPreHardModeNight,
		IronOre,
		LeadOre,
		Beach,
		Snow,
		ForestPreHardModeRare,
		DemolitionistShop,
		Underground,
		Ocean,
		Amethyst,
		SilverOre,
		TungstenOre,
		Topaz,
		Sapphire,
		UndergroundSnow,
		Emerald,
		GraveYard,
		DeepOcean,
		ForestPreHardModeVeryRare,
		Jungle,
		Ruby,
		GoldOre,
		PlatinumOre,
		Mushroom,
		ObsidianOre,
		PreHardmodeUncommonShops,
		Evil,
		TownNPCDrops,
		GoldChest,
		Extractinator,
		TorchGod,
		KingSlime,
		Diamond,
		UndergroundDesert,
		Sky,
		Fishing,
		UndergroundJungle,
		DemonicOrbOrCrimsonHeart,
		ArmsDealerShop,
		Granite,
		Marble,
		HolidayWeapons,
		Eye,
		DemoniteOre,
		CrimtaneOre,
		PostEyeEasy,
		BloodMoon,
		PostGoblinArmyEasy,
		GoblinArmy,
		SwordShrine,
		Hell,
		BloodMoonFishing,
		OldOneArmyT1,
		EaterBrain,
		MeteoriteOre,
		HellstoneOre,
		PostBeeEasy,
		Bee,
		PostSkeletronEasy,
		Skeletron,
		Dungeon,
		ShadowChest,
		Deer,
		DungeonGuardian,
		HardMode,
		HardmodeShopItems,
		HardModeUnderground,
		Wall,
		Hallow,
		HardModeFishing,
		HardModeNight,
		CobaltOre,
		PalladiumOre,
		Wyvern,
		HardModeBloodMoon,
		MushroomHardMode,
		UndergroundHallow,
		UndergroundEvil,
		MythrilOre,
		OrichalcumOre,
		PostFrostLegeonEasy,
		FrostLegeon,
		HardModeBloodMoonFishing,
		AdamantiteOre,
		TitaniumOre,
		HardModeFishingRare,
		HardModeRare,
		HardModeBloodMoonFishingRare,
		GoblinArmyHardMode,
		BigMimics,
		PostPiratesEasy,
		Pirates,
		Eclipse,
		QueenSlime,
		Destroyer,
		OldOneArmyT2,
		PostMechanicalBoss,
		AnyHardmodeTreasureBagRare,
		SkeletronPrime,
		Twins,
		PostAllMechanicalBosses,
		ChlorophyteOre,
		PostPlanteraEasy,
		Plantera,
		DungeonPostPlantera,
		DungeonPostPlanteraRare,
		BiomeChests,
		EclipsePostPlantera,
		EclipsePostPlanteraRare,
		PostFrostMoonEasy,
		PumpkinMoon,
		FrostMoon,
		OldOneArmyT3,
		PostGolemEasy,
		Betsey,
		Golem,
		MartianInvasion,
		MartianSaucer,
		DukeFishron,
		EmpressNight,
		Empress,
		LunaticCultist,
		LunarInvasion,
		PostMoonLordEasy,
		MoonLord,
		#endregion

		#region Calamity
		ArsenalLabs,
		SulfurousSea,
		DesertScourge,
		SunkenSea,
		GiantClam,
		SeaKingShop,
		PrismShard,
		AcidRainT1,
		Crabulon,
		AerialiteOre,
		HiveMind,
		Perforators,
		AbyssT1,
		SlimeGod,
		AstraulInfection,
		HardModeSunkenSea,
		GiantClamHardMode,
		InfernalSuevite,
		HardModeSulphurousSea,
		PostCryogenEasy,
		Cryogen,
		PostAquaticScourgeEasy,
		AquaticScourge,
		Voidstone,
		AcidRainT2,
		CragmawMire,
		BrimstoneElemental,
		PostCalamitasCloneEasy,
		CalamitasClone,
		GreatSandShark,
		Leviathan,
		PostAstrumAuresEasy,
		AstrumAures,
		PostAstrumAures,
		PlaguebringerGoliath,
		Ravager,
		PostAstrumDeusEasy,
		AstrumDeus,
		PostMoonLord,
		ProfanedGuardians,
		DragonFolly,
		PostProvidenceEasy,
		Providence,
		UelibloomBar,
		CeaselessVoid,
		StormWeaver,
		Signus,
		PostPolterghastEasy,
		Polterghast,
		AcidRainT3,
		PostOldDukeEasy,
		OldDuke,
		DevouererOfGods,
		AscendentSpirit,
		PostYharonEasy,
		Yharon,
		ExoMechs,
		SupremeCalamitas,
		AdultEidolon,
		BossRush,
		#endregion

		#region Stars Above
		PostKingSlimeEasy,
		PostEaterBrainEasy,
		StellaglyphT2,
		PostDeerEasy,
		Vagrant,
		PostQueenSlimeEasy,
		Nalhaun,
		Penthesilea,
		PostPumpkinMoonEasy,
		PostMartianSaucerEasy,
		PostEmpressEasy,
		PostDukeFishronEasy,
		PostLunaticCultistEasy,
		Arbitration,
		WarriorOfLight,
		FirstStarfarer,
		#endregion

		#region Thorium
		Opal,
		Aquamarine,
		TrackerContractT1,
		PatchWerk,
		ScarletChest,
		EnchantedPickaxeShrine,
		ThoriumUnobtainableItems,
		CorpseBloom,
		AquaticDepths,
		SpatialMemoriam,
		PostForgottenOneEasy,
		ForgottenOne,
		LunaticCultistThorium,
		PostBoreanStriderEasy,
		BoreanStrider,
		DarkMageThorium,
		OgreThorium,
		Primordials,
		GraniteEnergyStorm,
		PostFallenBeholderEasy,
		FallenBeholder,
		BuriedChampion,
		PostBuriedChampionEasy,
		Lich,
		AquaticDepthsPostPlantera,
		FlyingDutchmanThorium,
		MartianSaucerThorium,
		PostQueenJellyfishEasy,
		QueenJellyfish,
		StarScouter,
		AquaticDepthsHardMode,
		PostGrandThunderBirdEasy,
		GrandThunderBird,
		LifeQuartz,
		ThoriumOre,
		Viscount,
		DestinyWeaverUnobtainable,
		#endregion

		#region Fargos Souls
		TrojanSquirrel,
		AbomBoss,
		DeviBoss,
		BanishedBaron,
		CosmosChampion,
		MutantBoss,
		LieFlight,
		FargosUnobtainableItems,
		Energizers,
		PostAttraidies,
		#endregion

		#region Red Cloud
		Leon,
		Slogra,
		Gaibon,
		Gwyn,
		WyvernMage,
		EarthFiendLich,
		FireFiendMarilith,
		TheRage,
		TheSorrow,
		TheHunter,
		HellkiteDragon,
		Attraidies,
		Blight,
		Witchking,
		DarkCloud,
		SeathTheScaleless,
		Artorias,
		JungleWyvern,
		SerrisX,
		Death,
		BrokenOkiku,
		WaterFiendKraken,
		AbysmalOolacileSorcerer,
		#endregion

		#region Aequus
		/*Crabson,
		CrabsonEasyAfter,
		Glimmer,
		UltraStarite,
		DemonSiege,
		DemonSiegeEasyAfter,
		OmegaStariteEasyAfter,
		OmegaStarite,
		GaleStreams,
		RedSprite,
		SpaceSquid,
		DustDevil,
		AequusUnobtainable,
		AequusUndergroundOcean,
		RockMan,
		AequusWorkInProgress,*/
		#endregion

		#region Secret of the Shadows
		Glowmoth,
		PutridPinkyPhase2,
		PharaohsCurse,
		TheAdvisorHead,
		Polaris,
		Lux,
		SubspaceSerpentHead,
		OtherworldyConstruct,
		NatureConstruct,
		EarthenConstruct,
		PermafrostConstruct,
		TidalConstruct,
		EvilConstruct,
		InfernoConstruct,
		ChaosConstruct
		#endregion
	}
	public struct ItemSource {
		public ItemSource(int resultItem, ItemSourceType itemSourceType, int sourceItem) {
			ResultItemID = resultItem;
			ItemSourceType = itemSourceType;
			SourceID = sourceItem;
		}
		public ItemSource(int resultItem, ItemSourceType itemSourceType, int sourceItem, int ingredientCount) {
			ResultItemID = resultItem;
			ItemSourceType = itemSourceType;
			SourceID = sourceItem;
			IngredientCount = ingredientCount;
		}

		public ItemSourceType ItemSourceType;
		public int ResultItemID;
		public int SourceID;
		public int InfusionPowerOverride = -1;
		public int IngredientCount = -1;
		public bool Modded => SourceID >= ItemID.Count;
		public bool LockedByBoss = false;
		public bool TryGetIngredientItem(out Item ingredientItem) {
			ingredientItem = null;
			bool tryGetIngredientItem = 0 < SourceID && SourceID < ItemLoader.ItemCount; 
			if (tryGetIngredientItem)
				ingredientItem = SourceID.CSI();

			return tryGetIngredientItem;
		}
		public Item ResultItem => ResultItemID.CSI();

		public override string ToString() {
			switch (ItemSourceType) {
				case ItemSourceType.Craft:
					if (TryGetIngredientItem(out Item ingredientItem))
						return $"{ResultItem.S()} from {ItemSourceType}, {ingredientItem.S()}";// ({ingredientItemStack})";

					break;
				default:
					return $"Type Not Set Up";
			}

			return $"Failed {ItemSourceType}";
		}
	}
	public struct InfusionPowerSource {
		public InfusionPowerSource(ItemSource ItemSource, int BaseInfusionPower = -1) {
			itemSource = ItemSource;
			infusionOffset = GetInfusionPowerOffset(ItemSource);
			baseInfusionPower = BaseInfusionPower;
		}

		public int InfusionPower {
			get {
				int infusionPower = BaseInfusionPower + infusionPowerOffset;
				switch (itemSource.ItemSourceType) {
					case ItemSourceType.Craft:
						return infusionPower > 0 ? infusionPower : 0;
					default:
						return infusionPower > 0 ? infusionPower : 0;
				}
			}
		}

		private InfusionOffset infusionOffset;
		public ItemSource itemSource;
		public int SourceID => itemSource.SourceID;
		public int ResultItemID => itemSource.ResultItemID;
		public Item ResultItem => itemSource.ResultItem;
		private int baseInfusionPower;
		private int BaseInfusionPower => baseInfusionPower >= 0 ? baseInfusionPower : ItemInfusionPowers[SourceID];
		private int infusionPowerOffset {
			get {
				switch (infusionOffset) {
					case InfusionOffset.Consumable:
						return -10;
					//case InfusionOffset.EasyObtain:
					//	return -5;
					case InfusionOffset.CraftFromWeapon:
						return 5;
					default:
						return 0;
				}
			}
		}

		private static InfusionOffset GetInfusionPowerOffset(ItemSource itemSource) {
			if (itemSource.TryGetIngredientItem(out Item sourceItem)) {
				if (itemSource.ResultItem.consumable) {
					return InfusionOffset.Consumable;
				}
				//else if (itemSource.LockedByBoss) {
				//	return InfusionOffset.EasyObtain;
				//}
				else if (sourceItem.IsWeaponItem() && sourceItem.type != itemSource.ResultItemID && itemSource.IngredientCount > 2) {
					return InfusionOffset.CraftFromWeapon;
				}
				else {
					return InfusionOffset.None;
				}
			}

			return InfusionOffset.Error;
		}

		public override string ToString() {
			return $"{itemSource}, InfusionPower: {InfusionPower}";
		}
	}
	public class ProgressionGroup {
		public ProgressionGroup(ProgressionGroupID id, int InfusionPower, ProgressionGroupID parent = ProgressionGroupID.None, 
				IEnumerable<int> itemTypes = null, IEnumerable<string> itemNames = null, IEnumerable<int> npcTypes = null, 
				IEnumerable<string> npcNames = null, IEnumerable<ChestID> chests = null, IEnumerable<CrateID> crates = null, IEnumerable<int> lootItemTypes = null,
				IEnumerable<string> lootItemNames = null, IEnumerable<int> ignoredItemTypes = null) {
			parentID = parent;
			ID = id;
			infusionPower = InfusionPower;
			BossNetIDs = GetBossType(id);
			NpcTypes = new(BossNetIDs);
			if (ignoredItemTypes != null)
				addedItems = new(ignoredItemTypes);

			if (TryGetLootBagFromBoss(out SortedSet<int> lootBags))
				AddLootItems(lootBags);

			if (itemTypes != null)
				AddItems(itemTypes);

			if (itemNames != null)
				AddItems(itemNames);

			if (npcTypes != null)
				AddNPCs(npcTypes);

			if (npcNames != null)
				AddNPCs(npcNames);

			if (chests != null)
				Add(chests);

			if (crates != null)
				Add(crates);

			if (lootItemTypes != null)
				AddLootItems(lootItemTypes);

			if (lootItemNames != null)
				AddLootItems(lootItemNames);
		}

		private static SortedSet<int> addedItems = new();
		public static void ClearSetupData() {
			addedItems.Clear();
		}
		public ProgressionGroup Parent => parentID > ProgressionGroupID.None ? ProgressionGroups[parentID] : null;
		private ProgressionGroupID parentID;
		public ProgressionGroupID ID { get; private set; }
		int infusionPower;
		public int InfusionPower {
			get {
				if (Parent != null) {
					return Parent.InfusionPower + infusionPower;
				}
				else {
					return infusionPower;
				}
			}
		}
		public SortedSet<int> BossNetIDs { get; private set; }
		public SortedSet<int> ItemTypes { get; private set; } = new();
		public SortedSet<int> NpcTypes { get; private set; } = new();
		public SortedSet<ChestID> Chests { get; private set; } = new();
		public SortedSet<CrateID> Crates { get; private set; } = new();
		public SortedSet<int> LootItemTypes { get; private set; } = new();
		public void AddItems(IEnumerable<int> newItems) {
			IEnumerable<int> newItemsNoMatch = newItems.Where(t => !addedItems.Contains(t));
			ItemTypes.UnionWith(newItemsNoMatch);
			addedItems.UnionWith(newItemsNoMatch);
		}
		public void AddLootItems(IEnumerable<int> newLootItems) {
			IEnumerable<int> newItemsNoMatch = newLootItems.Where(t => !addedItems.Contains(t));
			LootItemTypes.UnionWith(newItemsNoMatch);
			addedItems.UnionWith(newItemsNoMatch);
		}
		public void AddItems(IEnumerable<string> newItems) {
			SortedSet<string> newItemsSet = new SortedSet<string>(newItems);
			IEnumerable<int> weaponAndIngredientList = WeaponsList.Concat(WeaponCraftingIngredients);
			foreach (int itemType in weaponAndIngredientList) {
				string itemName = itemType.CSI().ModFullName();
				if (newItemsSet.Contains(itemName)) {
					if (!addedItems.Contains(itemType)) {
						ItemTypes.Add(itemType);
						addedItems.Add(itemType);
					}

					newItemsSet.Remove(itemName);
				}

				if (newItemsSet.Count < 1)
					break;
			}

			if (newItemsSet.Count > 0) {
				for (int itemType = 0; itemType < ItemLoader.ItemCount; itemType++) {
					string itemName = itemType.CSI().ModFullName();
					if (newItemsSet.Contains(itemName)) {
						if (!addedItems.Contains(itemType)) {
							ItemTypes.Add(itemType);
							addedItems.Add(itemType);
						}

						newItemsSet.Remove(itemName);
					}

					if (newItemsSet.Count < 1)
						break;
				}
			}

			if (newItemsSet.Count > 0)
				$"{GameMessageTextID.CouldntFindItemsInWeaponsList.ToString().Lang_WE(L_ID1.GameMessages)} {newItemsSet.JoinList(", ")}".LogSimple();
		}
		public void AddLootItems(IEnumerable<string> newItems) {
			SortedSet<string> newItemsSet = new SortedSet<string>(newItems);
			if (newItemsSet.Count > 0) {
				for (int itemType = 0; itemType < ItemLoader.ItemCount; itemType++) {
					string itemName = itemType.CSI().ModFullName();
					if (newItemsSet.Contains(itemName)) {
						if (!addedItems.Contains(itemType)) {
							LootItemTypes.Add(itemType);
							addedItems.Add(itemType);
						}

						newItemsSet.Remove(itemName);
					}

					if (newItemsSet.Count < 1)
						break;
				}
			}

			if (newItemsSet.Count > 0)
				$"{GameMessageTextID.CouldntFindItemsInWeaponsList.ToString().Lang_WE(L_ID1.GameMessages)} {newItemsSet.JoinList(", ")}".LogSimple();
		}
		public void AddNPCs(IEnumerable<int> newNPCs) => NpcTypes.UnionWith(newNPCs);
		public void AddNPCs(IEnumerable<string> newNPCs) {
			SortedSet<string> newNpcsSet = new SortedSet<string>(newNPCs);
			SortedSet<string> notFoundSet = new SortedSet<string>(newNpcsSet);
			foreach (int netID in NPCsThatDropWeaponsOrIngredients.Keys) {
				string npcName = netID.CSNPC().ModFullName();
				if (newNpcsSet.Contains(npcName)) {
					NpcTypes.Add(netID);
					notFoundSet.Remove(npcName);
				}
			}

			if (Debugger.IsAttached && notFoundSet.Count > 0)
				$"{GameMessageTextID.CouldntFindNPCsInIngredientsList.ToString().Lang_WE(L_ID1.GameMessages)} {newNpcsSet.JoinList(", ")}".LogSimple();
		}
		public void Add(IEnumerable<ChestID> newChests) => Chests.UnionWith(newChests);
		public void Add(IEnumerable<CrateID> newCrates) => Crates.UnionWith(newCrates);
		public void Add(IEnumerable<Item> newItems) => AddItems(newItems.Select(i => i.type));
		public void Add(IEnumerable<NPC> newNPCs) => AddNPCs(newNPCs.Select(npc => npc.netID));
		public static SortedSet<int> GetBossType(ProgressionGroupID id) {
			int npcid = NPCID.NegativeIDCount;
			switch (id) {
				case ProgressionGroupID.TorchGod:
					npcid = NPCID.TorchGod;
					break;
				case ProgressionGroupID.KingSlime:
					npcid = NPCID.KingSlime;
					break;
				case ProgressionGroupID.Eye:
					npcid = NPCID.EyeofCthulhu;
					break;
				case ProgressionGroupID.EaterBrain:
					return new() { NPCID.EaterofWorldsHead, NPCID.BrainofCthulhu };
				case ProgressionGroupID.Bee:
					npcid = NPCID.QueenBee;
					break;
				case ProgressionGroupID.Skeletron:
					npcid = NPCID.SkeletronHead;
					break;
				case ProgressionGroupID.Deer:
					npcid = NPCID.Deerclops;
					break;
				case ProgressionGroupID.Wall:
					npcid = NPCID.WallofFlesh;
					break;
				case ProgressionGroupID.QueenSlime:
					npcid = NPCID.QueenSlimeBoss;
					break;
				case ProgressionGroupID.Destroyer:
					npcid = NPCID.TheDestroyer;
					break;
				case ProgressionGroupID.SkeletronPrime:
					npcid = NPCID.SkeletronPrime;
					break;
				case ProgressionGroupID.Twins:
					return new() { NPCID.Retinazer, NPCID.Spazmatism };
				case ProgressionGroupID.Plantera:
					npcid = NPCID.Plantera;
					break;
				case ProgressionGroupID.Betsey:
					npcid = NPCID.DD2Betsy;
					break;
				case ProgressionGroupID.Golem:
					npcid = NPCID.Golem;
					break;
				//case ProgressionGroupID.MartianSaucer:
				//	npcid = NPCID.MartianSaucerCore;
				//	break;
				case ProgressionGroupID.DukeFishron:
					npcid = NPCID.DukeFishron;
					break;
				case ProgressionGroupID.EmpressNight:
					npcid = NPCID.HallowBoss;
					break;
				//case ProgressionGroupID.LunaticCultist:
				//	npcid = NPCID.CultistBoss;
				//	break;
				case ProgressionGroupID.MoonLord:
					npcid = NPCID.MoonLordCore;
					break;
				case >= ProgressionGroupID.DesertScourge:
					string bossName = null;
					switch (id) {
						//Calamity
						case ProgressionGroupID.AquaticScourge:
							bossName = "CalamityMod/AquaticScourgeHead";
							break;
						case ProgressionGroupID.AstrumAures:
							bossName = "CalamityMod/AstrumAureus";
							break;
						case ProgressionGroupID.AstrumDeus:
							bossName = "CalamityMod/AstrumDeusHead";
							break;
						case ProgressionGroupID.BrimstoneElemental:
							bossName = "CalamityMod/BrimstoneElemental";
							break;
						case ProgressionGroupID.CalamitasClone:
							bossName = "CalamityMod/CalamitasClone";
							break;
						case ProgressionGroupID.SupremeCalamitas:
							bossName = "CalamityMod/SupremeCalamitas";
							break;
						case ProgressionGroupID.CeaselessVoid:
							bossName = "CalamityMod/CeaselessVoid";
							break;
						case ProgressionGroupID.Crabulon:
							bossName = "CalamityMod/Crabulon";
							break;
						case ProgressionGroupID.Cryogen:
							bossName = "CalamityMod/Cryogen";
							break;
						case ProgressionGroupID.DesertScourge:
							bossName = "CalamityMod/DesertScourgeHead";
							break;
						case ProgressionGroupID.DevouererOfGods:
							bossName = "CalamityMod/DevourerofGodsHead";
							break;
						case ProgressionGroupID.ExoMechs:
							bossName = "CalamityMod/AresBody";
							break;
						case ProgressionGroupID.DragonFolly:
							bossName = "CalamityMod/Bumblefuck";
							break;
						case ProgressionGroupID.HiveMind:
							bossName = "CalamityMod/HiveMind";
							break;
						case ProgressionGroupID.Leviathan:
							bossName = "CalamityMod/Leviathan";
							break;
						case ProgressionGroupID.OldDuke:
							bossName = "CalamityMod/OldDuke";
							break;
						case ProgressionGroupID.Perforators:
							bossName = "CalamityMod/PerforatorHive";
							break;
						case ProgressionGroupID.PlaguebringerGoliath:
							bossName = "CalamityMod/PlaguebringerGoliath";
							break;
						case ProgressionGroupID.Polterghast:
							bossName = "CalamityMod/Polterghast";
							break;
						case ProgressionGroupID.Providence:
							bossName = "CalamityMod/Providence";
							break;
						case ProgressionGroupID.Ravager:
							bossName = "CalamityMod/RavagerBody";
							break;
						case ProgressionGroupID.Signus:
							bossName = "CalamityMod/Signus";
							break;
						case ProgressionGroupID.SlimeGod:
							bossName = "CalamityMod/SlimeGodCore";
							break;
						case ProgressionGroupID.StormWeaver:
							bossName = "CalamityMod/StormWeaverHead";
							break;
						case ProgressionGroupID.Yharon:
							bossName = "CalamityMod/Yharon";
							break;

						//Stars Above
						case ProgressionGroupID.Vagrant:
							bossName = "StarsAbove/VagrantBoss";
							break;
						case ProgressionGroupID.Nalhaun:
							bossName = "Nalhaun, The Burnished King";
							break;
						case ProgressionGroupID.Penthesilea:
							bossName = "StarsAbove/Penthesilea";
							break;
						case ProgressionGroupID.Arbitration:
							bossName = "StarsAbove/Arbitration";
							break;
						case ProgressionGroupID.WarriorOfLight:
							bossName = "StarsAbove/WarriorOfLightBossFinalPhase";
							break;

						//ThoriumMod
						case ProgressionGroupID.ForgottenOne:
							bossName = "ThoriumMod/ForgottenOne";
							break;
						case ProgressionGroupID.LunaticCultistThorium:
							bossName = "NPCID.CultistBoss";
							break;
						case ProgressionGroupID.BoreanStrider:
							bossName = "ThoriumMod/BoreanStrider";
							break;
						case ProgressionGroupID.DarkMageThorium:
							bossName = "NPCID.DD2DarkMageT1";
							break;
						case ProgressionGroupID.OgreThorium:
							bossName = "NPCID.DD2OgreT2";
							break;
						case ProgressionGroupID.Primordials:
							bossName = "ThoriumMod/RealityBreaker";
							break;
						case ProgressionGroupID.GraniteEnergyStorm:
							bossName = "ThoriumMod/GraniteEnergyStorm";
							break;
						case ProgressionGroupID.FallenBeholder:
							bossName = "ThoriumMod/FallenBeholder";
							break;
						case ProgressionGroupID.BuriedChampion:
							bossName = "ThoriumMod/BuriedChampion";
							break;
						case ProgressionGroupID.Lich:
							bossName = "ThoriumMod/Lich";
							break;
						case ProgressionGroupID.FlyingDutchmanThorium:
							bossName = "Flying Dutchman";
							break;
						case ProgressionGroupID.MartianSaucerThorium:
							bossName = "Martian Saucer";
							break;
						case ProgressionGroupID.QueenJellyfish:
							bossName = "ThoriumMod/QueenJellyfish";
							break;
						case ProgressionGroupID.StarScouter:
							bossName = "ThoriumMod/StarScouter";
							break;
						case ProgressionGroupID.GrandThunderBird:
							bossName = "ThoriumMod/TheGrandThunderBird";
							break;
						case ProgressionGroupID.Viscount:
							bossName = "ThoriumMod/Viscount";
							break;

						//Fargos Souls
						case ProgressionGroupID.TrojanSquirrel:
							bossName = "FargowiltasSouls/TrojanSquirrel";
							break;
						case ProgressionGroupID.AbomBoss:
							bossName = "FargowiltasSouls/AbomBoss";
							break;
						case ProgressionGroupID.DeviBoss:
							bossName = "FargowiltasSouls/DeviBoss";
							break;
						case ProgressionGroupID.BanishedBaron:
							bossName = "FargowiltasSouls/BanishedBaron";
							break;
						case ProgressionGroupID.CosmosChampion:
							bossName = "FargowiltasSouls/CosmosChampion";
							break;
						case ProgressionGroupID.MutantBoss:
							bossName = "FargowiltasSouls/MutantBoss";
							break;
						case ProgressionGroupID.LieFlight:
							bossName = "FargowiltasSouls/LifeChallenger";
							break;

						//Red Cloud
						case ProgressionGroupID.Leon:
							bossName = "tsorcRevamp/LeonhardPhase1";
							break;
						case ProgressionGroupID.Slogra:
							bossName = "tsorcRevamp/Slogra";
							break;
						case ProgressionGroupID.Gaibon:
							bossName = "tsorcRevamp/Gaibon";
							break;
						case ProgressionGroupID.JungleWyvern:
							bossName = "tsorcRevamp/JungleWyvernHead";
							break;
						case ProgressionGroupID.TheRage:
							bossName = "tsorcRevamp/TheRage";
							break;
						case ProgressionGroupID.TheSorrow:
							bossName = "tsorcRevamp/TheSorrow";
							break;
						case ProgressionGroupID.WyvernMage:
							bossName = "tsorcRevamp/WyvernMage";
							break;
						case ProgressionGroupID.TheHunter:
							bossName = "tsorcRevamp/TheHunter";
							break;
						case ProgressionGroupID.SerrisX:
							bossName = "tsorcRevamp/SerrisX";
							break;
						case ProgressionGroupID.Death:
							bossName = "tsorcRevamp/Death";
							break;
						case ProgressionGroupID.BrokenOkiku:
							bossName = "tsorcRevamp/BrokenOkiku";
							break;
						case ProgressionGroupID.Attraidies:
							bossName = "tsorcRevamp/Attraidies";
							break;
						case ProgressionGroupID.HellkiteDragon:
							bossName = "tsorcRevamp/HellkiteDragonHead";
							break;
						case ProgressionGroupID.WaterFiendKraken:
							bossName = "tsorcRevamp/WaterFiendKraken";
							break;
						case ProgressionGroupID.EarthFiendLich:
							bossName = "tsorcRevamp/EarthFiendLich";
							break;
						case ProgressionGroupID.Witchking:
							bossName = "tsorcRevamp/Witchking";
							break;
						case ProgressionGroupID.Artorias:
							bossName = "tsorcRevamp/Artorias";
							break;
						case ProgressionGroupID.SeathTheScaleless:
							bossName = "tsorcRevamp/SeathTheScalelessHead";
							break;
						case ProgressionGroupID.AbysmalOolacileSorcerer:
							bossName = "tsorcRevamp/AbysmalOolacileSorcerer";
							break;
						case ProgressionGroupID.FireFiendMarilith:
							bossName = "tsorcRevamp/FireFiendMarilith";
							break;
						case ProgressionGroupID.DarkCloud:
							bossName = "tsorcRevamp/DarkCloud";
							break;
						case ProgressionGroupID.Blight:
							bossName = "tsorcRevamp/Blight";
							break;
						case ProgressionGroupID.Gwyn:
							bossName = "tsorcRevamp/Gwyn";
							break;

						//Aequus
						/*case ProgressionGroupID.Crabson:
							bossName = "Aequus/CrabsonOld";
							break;
						case ProgressionGroupID.DustDevil:
							bossName = "Aequus/DustDevil";
							break;
						case ProgressionGroupID.OmegaStarite:
							bossName = "Aequus/OmegaStarite";
							break;*/

						//SOTS
						case ProgressionGroupID.Glowmoth:
							bossName = "SOTS/Glowmoth";
							break;
						case ProgressionGroupID.PutridPinkyPhase2:
							bossName = "SOTS/PutridPinkyPhase2";
							break;
						case ProgressionGroupID.PharaohsCurse:
							bossName = "SOTS/PharaohsCurse";
							break;
						case ProgressionGroupID.TheAdvisorHead:
							bossName = "SOTS/TheAdvisorHead";
							break;
						case ProgressionGroupID.Polaris:
							bossName = "SOTS/Polaris";
							break;
						case ProgressionGroupID.Lux:
							bossName = "SOTS/Lux";
							break;
						case ProgressionGroupID.SubspaceSerpentHead:
							bossName = "SOTS/SubspaceSerpentHead";
							break;

							/*
						case ProgressionGroupID.:
							bossName = "";
							break;
							*/
					}

					if (bossName != null && GlobalBossBags.BossTypes.TryGetValue(bossName, out int value))
						npcid = value;

					break;
			}

			if (npcid > NPCID.NegativeIDCount)
				return new() { npcid };

			return new();
		}
		public bool TryGetBossType(out SortedSet<int> type) {
			type = null;
			if (BossNetIDs.Count <= 0)
				return false;

			type = BossNetIDs;
			return true;
		}
		public bool TryGetLootBagFromBoss(out SortedSet<int> lootItemTypes) {
			lootItemTypes = new();

			if (!TryGetBossType(out SortedSet<int> netIds))
				return false;

			if (WEGlobalNPC.AllItemDropsFromNpcs == null)
				return false;

			foreach (int netId in netIds) {
				int bossBagType = -1;
				if (GlobalBossBags.BossBagNPCs.ContainsKey(netId)) {
					bossBagType = GlobalBossBags.BossBagNPCs[netId];
					lootItemTypes.Add(bossBagType);
				}
			}

			if (Debugger.IsAttached && lootItemTypes.Count < 1)
				$"{GameMessageTextID.FailedFindBossBag.ToString().Lang_WE(L_ID1.GameMessages)} {netIds.Select(n => n.CSNPC().S()).JoinList(", ")}".LogSimple();

			return true;
		}
		public SortedSet<int> GetLootItemDrops() {
			SortedSet<int> itemTypes = new();
			if (LootItemTypes.Count < 1)
				return itemTypes;

			foreach (int bossBagType in LootItemTypes) {
				foreach (KeyValuePair<int, SortedSet<int>> weaponsFromLootItem in WeaponsFromLootItems) {
					if (weaponsFromLootItem.Value.Contains(bossBagType) && !addedItems.Contains(weaponsFromLootItem.Key)) {
						itemTypes.Add(weaponsFromLootItem.Key);
						addedItems.Add(weaponsFromLootItem.Key);
					}
				}

				foreach (KeyValuePair<int, SortedSet<int>> ingredientsFromLootItem in IngredientsFromLootItems) {
					if (ingredientsFromLootItem.Value.Contains(bossBagType) && !addedItems.Contains(ingredientsFromLootItem.Key)) {
						itemTypes.Add(ingredientsFromLootItem.Key);
						addedItems.Add(ingredientsFromLootItem.Key);
					}
				}
			}

			if (Debugger.IsAttached && itemTypes.Count < 1)
				$"{GameMessageTextID.FailedFindItemDropsForGroup.ToString().Lang_WE(L_ID1.GameMessages)} {ID}: {LootItemTypes.Select(i => i.CSI().S()).JoinList(", ")}".LogSimple();

			return itemTypes;
		}
		public override string ToString() {
			return $"{ID}";
		}
	}
	public static class InfusionProgression {
		private static bool finishedSetup = false;
		private static bool finishedRecipeSetup = false;
		public static SortedDictionary<int, InfusionPowerSource> WeaponInfusionPowers { get; private set; } = new();//Not cleared
		private static SortedDictionary<int, HashSet<HashSet<int>>> allWeaponRecipies = new();
		public static SortedSet<int> WeaponsList { get; private set; } = new();
		public static SortedSet<int> WeaponCraftingIngredients { get; private set; } = new();
		public static SortedDictionary<int, HashSet<HashSet<int>>> allExpandedRecepies = new();
		private static SortedSet<int> reverseCraftableRecipes = new();
		private static SortedDictionary<int, HashSet<int>> shimmerRecipes = new();
		private static SortedDictionary<int, int> oreInfusionPowers = null;
		public static SortedDictionary<int, int> OreInfusionPowers {
			get {
				if (oreInfusionPowers == null) {
					IEnumerable<ProgressionGroup> oreGroups = ProgressionGroups.Where(g => g.Key <= ProgressionGroupID.MoonLord && $"{g.Key}".EndsWith("Ore")).Select(g => g.Value);
					oreInfusionPowers = new(oreGroups.ToDictionary(g => g.ItemTypes.First(), g => g.InfusionPower));
				}

				return oreInfusionPowers;
			}
		}
		public static SortedDictionary<int, SortedSet<int>> NPCsThatDropWeaponsOrIngredients { get; private set; } = new();
		public static SortedDictionary<int, SortedSet<int>> WeaponsFromNPCs { get; private set; } = new();
		public static SortedDictionary<int, SortedSet<int>> IngredientsFromNPCs { get; private set; } = new();
		public static SortedDictionary<int, SortedSet<int>> WeaponsFromLootItems { get; private set; } = new();
		public static SortedDictionary<int, SortedSet<int>> IngredientsFromLootItems { get; private set; } = new();
		public static SortedSet<int> LootItemTypes { get; private set; } = new();
		public static SortedDictionary<ProgressionGroupID, ProgressionGroup> ProgressionGroups {
			get {
				if (progressionGroups == null)
					SetupProgressionGroups();

				return progressionGroups;
			}
		}
		public static SortedDictionary<ProgressionGroupID, ProgressionGroup> progressionGroups = null;
		private static SortedDictionary<int, (int pickPower, float value)> infusionPowerTiles = null;
		public static SortedDictionary<int, (int pickPower, float value)> InfusionPowerTiles {
			get {
				if (infusionPowerTiles == null)
					SetupInfusionPowerTiles();

				return infusionPowerTiles;
			}
		}
		public static SortedDictionary<int, int> ItemInfusionPowers { get; private set; } = new();//Not Cleared
		public static void PostSetupRecipes() {
			if (finishedSetup)
				return;

			SetupDictionaries();

			PopulateInfusionPowerSources();

			PrintMissingInfusionPowers();

			if (Debugger.IsAttached)
				InfusionProgressionTests.RunTests();

			if (Debugger.IsAttached)
				UpdateAndPrintString();

			ClearSetupData();
		}

		#region SetupDictionaries

		private static void SetupDictionaries() {
			SetupWeaponsList();
			SetupReverseCraftableIngredients();
			GetAllCraftingResources();
			SetupShimmerRecipes();
			SetupItemsFromNPCs();
			SetupItemsFromLootItems();
			PopulateItemInfusionPowers();
			GuessMinedOreInfusionPowers();
		}
		private static void SetupWeaponsList() {
			for (int i = 0; i < ItemLoader.ItemCount; i++) {
				Item sampleItem = i.CSI();
				if (sampleItem.IsWeaponItem())
					WeaponsList.Add(i);
			}

			//if (Debugger.IsAttached) $"\nweaponsList:\n{weaponsList.Select(type => $"{type.CSI().S()}").JoinList("\n")}".LogSimple();
		}
		private static void SetupReverseCraftableIngredients() {
			SortedSet<int> originalWeaponIngredients = new();
			SortedDictionary<int, (int, HashSet<int>, HashSet<int>)> allRecipes = new();
			for (int i = 0; i < Main.recipe.Length; i++) {
				Recipe r = Main.recipe[i];

				if (r.createItem.NullOrAir())
					continue;

				HashSet<int> ingredients = r.requiredItem.Select(i => i.type).Where(t => t > 0).ToHashSet();
				HashSet<int> tiles = r.requiredTile.Select(t => GenericGlobalTile.GetDroppedItem(t)).Where(t => t > 0).ToHashSet();
				//if (Debugger.IsAttached) $"{ingredients.StringList(i => i.CSI().S(), $"createItem: {r.createItem.S()}")}".LogSimple();

				allRecipes.Add(i, (r.createItem.type, ingredients, tiles));
				if (r.createItem.IsWeaponItem())
					originalWeaponIngredients.UnionWith(ingredients);
			}

			SortedDictionary<int, SortedSet<int>> recipeNumbersByCraftedItem = new();
			foreach (KeyValuePair<int, (int createItemType, HashSet<int> ingredients, HashSet<int> tiles)> recipe in allRecipes) {
				recipeNumbersByCraftedItem.AddOrCombine(recipe.Value.createItemType, recipe.Key);
			}

			//if (Debugger.IsAttached) $"{reicpeNumbersByCraftedItem.Select(p => p.Value.StringList(n => $"{n}", $"{p.Key.CSI().S()}")).S("reicpeNumbersByRequiredItems")}".LogSimple();

			SortedSet<int> weaponIngredients = new(originalWeaponIngredients);
			foreach (KeyValuePair<int, (int createItemType, HashSet<int> ingredients, HashSet<int> tiles)> recipe in allRecipes) {
				int createItemType = recipe.Value.createItemType;
				int recipeNum = recipe.Key;
				HashSet<int> requiredItemTypes = recipe.Value.ingredients;
				HashSet<int> requiredTileTypes = recipe.Value.tiles;

				//Item createItem = createItemType.CSI();
				//string requiredItemTypesString = requiredItemTypes.StringList(i => i.CSI().S());
				bool added = reverseCraftableRecipes.Contains(recipeNum);
				IEnumerable<KeyValuePair<int, (int createItemType, HashSet<int> ingredients, HashSet<int> tiles)>> otherRecipes = recipeNumbersByCraftedItem.Where(p => requiredItemTypes.Contains(p.Key)).Select(p => p.Value.Select(n => new KeyValuePair<int, (int, HashSet<int>, HashSet<int>)>(n, allRecipes[n]))).SelectMany(p => p);
				foreach (KeyValuePair<int, (int createItemType, HashSet<int> ingredients, HashSet<int> tiles)> otherRecipe in otherRecipes) {
					int otherCreateItemType = otherRecipe.Value.createItemType;
					int otherRecipeNum = otherRecipe.Key;
					HashSet<int> otherRequiredItemTypes = otherRecipe.Value.ingredients;
					//HashSet<int> otherRequiredTileTypes = otherRecipe.Value.tiles;
					//Item otherCreateItem = otherCreateItemType.CSI();
					//string otherRequiredItemTypesString = otherRequiredItemTypes.StringList(i => i.CSI().S());

					if (!otherRequiredItemTypes.Contains(createItemType))
						continue;

					bool modRecipe = recipe.Key >= AndroMod.VanillaRecipeCount;
					bool otherModRecipe = otherRecipe.Key >= AndroMod.VanillaRecipeCount;
					bool isWeapon = WeaponsList.Contains(createItemType);
					bool otherIsWeapon = WeaponsList.Contains(otherCreateItemType);
					bool reverse = isWeapon ? modRecipe ? otherIsWeapon && createItemType <= otherCreateItemType /* && recipeNumbersByCraftedItem[createItemType].Count <= recipeNumbersByCraftedItem[otherCreateItemType].Count */: !otherModRecipe : modRecipe || !otherModRecipe;
					bool otherReverse = otherIsWeapon ? otherModRecipe ? isWeapon && otherCreateItemType <= createItemType /* recipeNumbersByCraftedItem[otherCreateItemType].Count <= recipeNumbersByCraftedItem[createItemType].Count */: !modRecipe : otherModRecipe || !modRecipe;

					if (reverse && !added) {
						reverseCraftableRecipes.Add(recipeNum);
						added = true;
					}

					if (otherReverse)
						reverseCraftableRecipes.Add(otherRecipeNum);
				}

				if (requiredTileTypes.Contains(createItemType) && !added)
					reverseCraftableRecipes.Add(recipeNum);
			}

			foreach (KeyValuePair<int, (int createItemType, HashSet<int> ingredients, HashSet<int> tiles)> recipe in allRecipes) {
				int createItemType = recipe.Value.createItemType;
				int recipeNum = recipe.Key;
				HashSet<int> requiredItemTypes = recipe.Value.ingredients;
				//HashSet<int> requiredTileTypes = recipe.Value.tiles;
				
				if (reverseCraftableRecipes.Contains(recipeNum))
					continue;

				//Item createItem = createItemType.CSI();
				//string requiredItemTypesString = requiredItemTypes.StringList(i => i.CSI().S());
				IEnumerable<KeyValuePair<int, (int createItemType, HashSet<int> ingredients, HashSet<int> tiles)>> otherRecipes = recipeNumbersByCraftedItem.Where(p => requiredItemTypes.Contains(p.Key)).Select(p => p.Value.Select(n => new KeyValuePair<int, (int, HashSet<int>, HashSet<int>)>(n, allRecipes[n]))).SelectMany(p => p);
				foreach (KeyValuePair<int, (int createItemType, HashSet<int> ingredients, HashSet<int> tiles)> otherRecipe in otherRecipes) {
					//int otherCreateItemType = otherRecipe.Value.createItemType;
					int otherRecipeNum = otherRecipe.Key;
					//HashSet<int> otherRequiredItemTypes = otherRecipe.Value.ingredients;
					HashSet<int> otherRequiredTileTypes = otherRecipe.Value.tiles;
					//Item otherCreateItem = otherCreateItemType.CSI();
					//string otherRequiredItemTypesString = otherRequiredItemTypes.StringList(i => i.CSI().S());

					if (reverseCraftableRecipes.Contains(otherRecipeNum))
						continue;

					if (otherRequiredTileTypes.Contains(createItemType))
						reverseCraftableRecipes.Add(recipeNum);
				}
			}

			//if (Debugger.IsAttached) allRecipes.Select(pair => $"{pair.Key}: {Main.recipe[pair.Key].createItem.S()}, {Main.recipe[pair.Key].requiredItem.StringList(i => i.S())}, {Main.recipe[pair.Key].requiredTile.Select(t => $"{WEGlobalTile.GetDroppedItem(t).CSI().S()}-{t.GetTileIDOrName()}").JoinList(", ")}").S("allRecipes").LogSimple();
			//if (Debugger.IsAttached) reverseCraftableRecipes.Select(i =>  $"{i}: {Main.recipe[i].createItem.S()}, {Main.recipe[i].requiredItem.StringList(i => i.S())}").S("reverseCraftableRecipes").LogSimple();
		}
		private static void GetAllCraftingResources() {
			foreach (int weaponType in WeaponsList) {
				//string temp = weaponType.CSI().ModFullName();
				if (TryGetAllCraftingIngredientTypes(weaponType, out HashSet<HashSet<int>> ingredients))
					allWeaponRecipies.Add(weaponType, ingredients);
			}

			finishedRecipeSetup = true;
			foreach (int ingredient in allWeaponRecipies.Select(p => p.Value).SelectMany(t => t).SelectMany(t => t)) {
				WeaponCraftingIngredients.Add(ingredient);
			}

			//if (Debugger.IsAttached) $"\n{allWeaponRecipies.Select(weapon => $"{weapon.Key.CSI().S()}:{weapon.Value.Select(ingredient => $" {ingredient.Select(i => i.CSI().Name).JoinList(" or ")}").JoinList(", ")}").JoinList("\n")}".LogSimple();
		}
		private static void SetupShimmerRecipes() {
			for (int i = 1; i < ItemLoader.ItemCount; i++) {
				int shimmerItemType = ItemID.Sets.ShimmerTransformToItem[i];
				if (shimmerItemType > 0) {
					if (WeaponsList.Contains(shimmerItemType)) {
						shimmerRecipes.AddOrCombine(shimmerItemType, i);
						//$"{i.CSI().ModFullName()} ->Shimmer-> {shimmerItemType.CSI().ModFullName()}".LogSimple();
					}
				}
			}
		}
		private static void SetupItemsFromNPCs() {
			//SortedDictionary<string, SortedSet<int>> conditionItems = new();
			foreach (KeyValuePair<int, NPC> npcPair in ContentSamples.NpcsByNetId) {
				int netID = npcPair.Key;
				NPC npc = npcPair.Value;
				List<IItemDropRule> dropRules = Main.ItemDropsDB.GetRulesForNPCID(netID, false).ToList();
				foreach (IItemDropRule dropRule in dropRules) {
					List<DropRateInfo> dropRates = new();
					DropRateInfoChainFeed dropRateInfoChainFeed = new(1f);
					dropRule.ReportDroprates(dropRates, dropRateInfoChainFeed);
					foreach (DropRateInfo dropRate in dropRates) {
						int itemType = dropRate.itemId;
						//Item item = itemType.CSI();
						bool inWeaponsList = WeaponsList.Contains(itemType);
						bool inInhredientList = WeaponCraftingIngredients.Contains(itemType);
						if (inWeaponsList) {
							WeaponsFromNPCs.AddOrCombine(itemType, netID);
						}
						else if (inInhredientList) {
							IngredientsFromNPCs.AddOrCombine(itemType, netID);
						}

						//if (Debugger.IsAttached && itemType.CSI().ModItem != null && (inWeaponsList || inInhredientList)) {
						//	if (dropRate.conditions != null) {
						//		foreach (IItemDropRuleCondition condition in dropRate.conditions) {
						//			var temp2 = condition.GetConditionDescription();
						//			var temp3 = condition.GetType().Name;
						//			var temp4 = condition.ToString();
						//		}

						//		List<string> ignored = new() {
						//		    "",
						//			" ",
						//			null
						//	    };
						//		IEnumerable<string> conditions = dropRate.conditions.Select(c => c.GetConditionDescription()).Where(c => !ignored.Contains(c));
						//		if (conditions.Count() > 0) {
						//			string conditionString = conditions.JoinList(", ");
						//			conditionItems.AddOrCombine(conditionString, itemType);
						//		}
						//	}
						//}
					}
				}

				int banner = Item.NPCtoBanner(npc.BannerID());
				string npcName = npc.ModFullName();
				if (banner > 0) {
					int bannerItemType = Item.BannerToItem(banner);
					if (WeaponCraftingIngredients.Contains(bannerItemType))
						IngredientsFromNPCs.AddOrCombine(bannerItemType, netID);
				}
			}

			//if (Debugger.IsAttached) $"{conditionItems.DictionaryOfItemLists("ConditionItems", (s) => s)}".LogSimple();

			foreach (KeyValuePair<int, SortedSet<int>> weapon in WeaponsFromNPCs) {
				foreach (int netID in weapon.Value) {
					//Item item = weapon.Key.CSI();
					//NPC npc = netID.CSNPC();
					NPCsThatDropWeaponsOrIngredients.AddOrCombine(netID, weapon.Key);
				}
			}

			foreach (KeyValuePair<int, SortedSet<int>> ingredient in IngredientsFromNPCs) {
				foreach (int netID in ingredient.Value) {
					//Item item = ingredient.Key.CSI();
					//NPC npc = netID.CSNPC();
					NPCsThatDropWeaponsOrIngredients.AddOrCombine(netID, ingredient.Key);
				}
			}

			//if (Debugger.IsAttached) $"{NPCsThatDropWeaponsOrIngredients.Select(p => p.Value.StringList(i => i.CSI().S(), p.Key.CSNPC().S())).S("NPCsThatDropWeaponsOrIngredients")}".LogSimple();
		}
		private static void SetupItemsFromLootItems() {
			foreach (KeyValuePair<int, Item> lootItemPair in ContentSamples.ItemsByType) {
				int type = lootItemPair.Key;
				foreach (IItemDropRule dropRule in Main.ItemDropsDB.GetRulesForItemID(type)) {
					List<DropRateInfo> dropRates = new();
					DropRateInfoChainFeed dropRateInfoChainFeed = new(1f);
					dropRule.ReportDroprates(dropRates, dropRateInfoChainFeed);
					foreach (DropRateInfo dropRate in dropRates) {
						int itemType = dropRate.itemId;
						if (WeaponsList.Contains(itemType)) {
							if (WeaponsFromLootItems.ContainsKey(itemType)) {
								WeaponsFromLootItems[itemType].Add(type);
							}
							else {
								WeaponsFromLootItems.Add(itemType, new() { type });
							}
						}
						else if (WeaponCraftingIngredients.Contains(itemType)) {
							if (IngredientsFromLootItems.ContainsKey(itemType)) {
								IngredientsFromLootItems[itemType].Add(type);
							}
							else {
								IngredientsFromLootItems.Add(itemType, new() { type });
							}
						}
					}
				}
			}

			foreach (int lootItemType in WeaponsFromLootItems.Values.SelectMany(s => s)) {
				LootItemTypes.Add(lootItemType);
			}

			foreach (int lootItemType in IngredientsFromLootItems.Values.SelectMany(s => s)) {
				LootItemTypes.Add(lootItemType);
			}

			//if (Debugger.IsAttached) $"\nWeaponsFromLootItems:\n{WeaponsFromLootItems.OrderBy(p => p.Key.CSI().GetWeaponInfusionPower()).Select(p => $"{p.Key.CSI().S()} from {p.Value.Select(i => i.CSI().S()).JoinList(", ")}").S()}".LogSimple();
			//if (Debugger.IsAttached) $"\nIngredientsFromLootItems:\n{IngredientsFromLootItems.OrderBy(p => p.Key.CSI().GetWeaponInfusionPower()).Select(p => $"{p.Key.CSI().S()} from {p.Value.Select(i => i.CSI().S()).JoinList(", ")}").S()}".LogSimple();
		}
		private static void SetupProgressionGroups() {
			progressionGroups = new();

			#region Vanilla Groups

			AddProgressionGroup(new(ProgressionGroupID.ForestPreHardMode, 0,
				itemTypes: new SortedSet<int>() {
					ItemID.Wood,
					ItemID.StoneBlock,
					ItemID.DirtBlock,
					ItemID.Mushroom,
					ItemID.Acorn,
					ItemID.ClayBlock,
					ItemID.Tombstone,
					ItemID.Count,//April Fools Joke
					ItemID.Sunflower,
					ItemID.Daybloom,
					ItemID.Stinkbug,
					ItemID.YellowMarigold,
					ItemID.BlueBerries
				},
				npcTypes: new SortedSet<int>() {
					NPCID.BlueSlime,
					NPCID.WindyBalloon
				}));
			AddProgressionGroup(new(ProgressionGroupID.Presents, 10,
				lootItemTypes: new SortedSet<int>() {
					ItemID.GoodieBag,
					ItemID.Present
				}));
			AddProgressionGroup(new(ProgressionGroupID.Desert, 10,
				itemTypes: new SortedSet<int>() {
					ItemID.Cactus,
					ItemID.SandBlock,
					ItemID.PinkPricklyPear
				},
				npcTypes: new SortedSet<int>() {
					NPCID.Vulture,
					NPCID.Antlion
				}));
			AddProgressionGroup(new(ProgressionGroupID.TinOre, 10,
				itemTypes: new SortedSet<int>() {
					ItemID.TinOre
				}));
			AddProgressionGroup(new(ProgressionGroupID.CopperOre, 10,
				itemTypes: new SortedSet<int>() {
					ItemID.CopperOre,
					ItemID.CopperCoin
				}));
			AddProgressionGroup(new(ProgressionGroupID.MerchantShop, 10,
				itemTypes: new SortedSet<int>() {
					ItemID.Shuriken,
					ItemID.Sickle,
					ItemID.ThrowingKnife,
					ItemID.Cardinal,
					ItemID.Squirrel
				}));
			AddProgressionGroup(new(ProgressionGroupID.ForestPreHardModeNight, 20,
				itemTypes: new SortedSet<int>() {
					ItemID.FallenStar
				},
				npcTypes: new SortedSet<int>() {
					NPCID.Zombie,
					NPCID.DemonEye
				}));
			AddProgressionGroup(new(ProgressionGroupID.GiantTree, 20,
				itemTypes: new SortedSet<int>() {
					ItemID.BabyBirdStaff,
					ItemID.LivingLoom
				}));
			AddProgressionGroup(new(ProgressionGroupID.NormalChest, 20,
				itemTypes: new SortedSet<int>() {
					ItemID.Spear,
					ItemID.Blowpipe,
					ItemID.WoodenBoomerang,
					ItemID.Umbrella,
					ItemID.WandofSparking
				}));
			AddProgressionGroup(new(ProgressionGroupID.IronOre, 20,
				itemTypes: new SortedSet<int>() {
					ItemID.IronOre,
					ItemID.WaterBucket
				}));
			AddProgressionGroup(new(ProgressionGroupID.LeadOre, 20,
				itemTypes: new SortedSet<int>() {
					ItemID.LeadOre
				}));
			AddProgressionGroup(new(ProgressionGroupID.Beach, 30,
				itemTypes: new SortedSet<int>() {
					ItemID.PalmWood,
					ItemID.Banana
				}));
			AddProgressionGroup(new(ProgressionGroupID.Snow, 35,
				itemTypes: new SortedSet<int>() {
					ItemID.BorealWood,
					ItemID.Shiverthorn,
					ItemID.SnowBlock,
					ItemID.IceBlock
				}));
			AddProgressionGroup(new(ProgressionGroupID.ForestPreHardModeRare, 40,
				itemTypes: new SortedSet<int>() {
					ItemID.MagicalPumpkinSeed,
					ItemID.EucaluptusSap
				},
				npcTypes: new SortedSet<int>() {
					NPCID.GoblinScout,
				}));
			AddProgressionGroup(new(ProgressionGroupID.Underground, 45,
				itemTypes: new SortedSet<int>() {
					ItemID.Cobweb,
					ItemID.LavaBucket,
					ItemID.GreenMushroom
				},
				npcTypes: new SortedSet<int>() {
					NPCID.UndeadMiner,
					NPCID.CaveBat,
					NPCID.PantlessSkeleton,
					NPCID.Salamander6,
					NPCID.BlueJellyfish
				}));
			AddProgressionGroup(new(ProgressionGroupID.Ocean, 50,
				itemTypes: new SortedSet<int>() {
					ItemID.SharkBait
				},
				npcTypes: new SortedSet<int>() {
					NPCID.SeaSnail,
					NPCID.Crab,
					NPCID.PinkJellyfish
				}));
			AddProgressionGroup(new(ProgressionGroupID.Amethyst, 50,
				itemTypes: new SortedSet<int>() {
					ItemID.Amethyst
				}));
			AddProgressionGroup(new(ProgressionGroupID.DemolitionistShop, 50,
				itemTypes: new SortedSet<int>() {
					ItemID.Grenade,
					ItemID.Dynamite
				}));
			AddProgressionGroup(new(ProgressionGroupID.Topaz, 55,
				itemTypes: new SortedSet<int>() {
					ItemID.Topaz
				}));
			AddProgressionGroup(new(ProgressionGroupID.SilverOre, 55,
				itemTypes: new SortedSet<int>() {
					ItemID.SilverOre
				}));
			AddProgressionGroup(new(ProgressionGroupID.TungstenOre, 55,
				itemTypes: new SortedSet<int>() {
					ItemID.TungstenOre
				}));
			AddProgressionGroup(new(ProgressionGroupID.Sapphire, 60,
				itemTypes: new SortedSet<int>() {
					ItemID.Sapphire
				}));
			AddProgressionGroup(new(ProgressionGroupID.UndergroundSnow, 65,
				itemTypes: new SortedSet<int>() {
					ItemID.IceBoomerang,
					ItemID.IceBlade,
					ItemID.SnowballCannon,
					ItemID.IceMachine,
					ItemID.Fish
				},
				npcTypes: new SortedSet<int>() {
					NPCID.SnowFlinx
				}));
			AddProgressionGroup(new(ProgressionGroupID.Emerald, 65,
				itemTypes: new SortedSet<int>() {
					ItemID.Emerald
				}));
			AddProgressionGroup(new(ProgressionGroupID.GraveYard, 65,
				itemTypes: new SortedSet<int>() {
					ItemID.AbigailsFlower
				}));
			AddProgressionGroup(new(ProgressionGroupID.DeepOcean, 70,
				itemTypes: new SortedSet<int>() {
					ItemID.BreathingReed,
					ItemID.Trident,
					ItemID.Starfish,
					ItemID.Coral,
					ItemID.Seashell
				},
				npcTypes: new SortedSet<int>() {
					NPCID.Shark,
					NPCID.Squid
				}));
			AddProgressionGroup(new(ProgressionGroupID.ForestPreHardModeVeryRare, 70,
				itemTypes: new SortedSet<int>() {
					ItemID.GoldButterfly
				},
				npcTypes: new SortedSet<int>() {
					NPCID.Pinky
				}));
			AddProgressionGroup(new(ProgressionGroupID.Jungle, 75,
				itemTypes: new SortedSet<int>() {
					ItemID.Moonglow
				}));
			AddProgressionGroup(new(ProgressionGroupID.Ruby, 75,
				itemTypes: new SortedSet<int>() {
					ItemID.Ruby
				}));
			AddProgressionGroup(new(ProgressionGroupID.Mushroom, 75,
				npcTypes: new SortedSet<int>() {
					NPCID.SporeBat,
					NPCID.SporeSkeleton
				}));
			AddProgressionGroup(new(ProgressionGroupID.GoldOre, 75,
				itemTypes: new SortedSet<int>() {
					ItemID.GoldOre
				}));
			AddProgressionGroup(new(ProgressionGroupID.PlatinumOre, 75,
				itemTypes: new SortedSet<int>() {
					ItemID.PlatinumOre
				}));
			AddProgressionGroup(new(ProgressionGroupID.TorchGod, 75));
			AddProgressionGroup(new(ProgressionGroupID.PostKingSlimeEasy, -10, ProgressionGroupID.KingSlime));//75
			AddProgressionGroup(new(ProgressionGroupID.TownNPCDrops, 80,
				npcTypes: new SortedSet<int>() {
					NPCID.PartyGirl,
					NPCID.DyeTrader,
					NPCID.Painter,
					NPCID.Stylist,
					NPCID.DD2Bartender,
					NPCID.Guide
				}));
			AddProgressionGroup(new(ProgressionGroupID.Evil, 80,
				itemTypes: new SortedSet<int>() {
					ItemID.EbonsandBlock,
					ItemID.CrimsandBlock,
					ItemID.VileMushroom,
					ItemID.ViciousMushroom,
					ItemID.Ebonwood,
					ItemID.Shadewood,
					ItemID.Deathweed,
					ItemID.EbonstoneBlock,
					ItemID.CrimstoneBlock,
					ItemID.PurpleIceBlock,
					ItemID.RedIceBlock,
					ItemID.PinkIceBlock
				},
				npcTypes: new SortedSet<int>() {
					NPCID.EaterofSouls,
					NPCID.Crimera,
					NPCID.DevourerHead
				}));
			AddProgressionGroup(new(ProgressionGroupID.GoldChest, 80,
				itemTypes: new SortedSet<int>() {
					ItemID.Mace,
					ItemID.FlareGun,
					ItemID.EnchantedBoomerang,
					ItemID.Extractinator
				}));
			AddProgressionGroup(new(ProgressionGroupID.Extractinator, 80,
				itemTypes: new SortedSet<int>() {
					ItemID.AmberMosquito
				}));
			AddProgressionGroup(new(ProgressionGroupID.ObsidianOre, 80,
				itemTypes: new SortedSet<int>() {
					ItemID.Obsidian
				}));
			AddProgressionGroup(new(ProgressionGroupID.PreHardmodeUncommonShops, 80,
				itemTypes: new SortedSet<int>() {
					ItemID.ConfettiGun,//Party Girl
					ItemID.BlandWhip,//Bestiary Girl
					ItemID.Confetti,
					ItemID.Katana,//Traveling Merchant
					ItemID.CompanionCube,//Traveling Merchant
					ItemID.BambooLeaf,//Traveling Merchant
					ItemID.TigerSkin,//Traveling Merchant
					ItemID.BirdieRattle,//Traveling Merchant
					ItemID.ExoticEasternChewToy,//Traveling Merchant
					ItemID.BedazzledNectar,//Traveling Merchant
					ItemID.CelestialWand,//Traveling Merchant
					ItemID.Paintbrush,
					ItemID.VanityTreeSakuraSeed,
					ItemID.DyeVat,
					ItemID.DynastyWood,
					ItemID.DD2ElderCrystal,//Bartender
					ItemID.DD2ElderCrystalStand,//Bartender
					ItemID.LightningCarrot,
					ItemID.LawnMower//Golfer
				}));
			AddProgressionGroup(new(ProgressionGroupID.KingSlime, 85));
			AddProgressionGroup(new(ProgressionGroupID.Diamond, 85,
				itemTypes: new SortedSet<int>() {
					ItemID.Diamond,
					ItemID.LifeCrystal
				}));
			AddProgressionGroup(new(ProgressionGroupID.Sky, 85,
				itemTypes: new SortedSet<int>() {
					ItemID.Starfury,
					ItemID.SkyMill,
					ItemID.Cloud
				},
				npcTypes: new SortedSet<int>() {
					NPCID.Harpy
				}));
			AddProgressionGroup(new(ProgressionGroupID.UndergroundDesert, 90,
				itemTypes: new SortedSet<int>() {
					ItemID.Amber,
					ItemID.ThunderSpear,
					ItemID.ThunderStaff,
					ItemID.HardenedSand,
					ItemID.SandstorminaBottle,
				},
				npcTypes: new SortedSet<int>() {
					NPCID.WalkingAntlion,
					NPCID.TombCrawlerHead
				}));
			AddProgressionGroup(new(ProgressionGroupID.Fishing, 95,
				itemTypes: new SortedSet<int>() {
					ItemID.Swordfish,
					ItemID.PurpleClubberfish,
					ItemID.FrostDaggerfish,
					ItemID.Rockfish,
					ItemID.ReaverShark,
					ItemID.SawtoothShark,
					ItemID.ZephyrFish,
					ItemID.GoldenFishingRod,
					ItemID.Shrimp,
					ItemID.SpecularFish
				},
				lootItemTypes: new SortedSet<int>() {
					ItemID.WoodenCrate,
					ItemID.IronCrate,
					ItemID.CorruptFishingCrate,
					ItemID.CrimsonFishingCrate,
					ItemID.DungeonFishingCrate,
					ItemID.GoldenCrate,
					ItemID.FrozenCrate,
					ItemID.OceanCrate,
					ItemID.OasisCrate,
					ItemID.JungleFishingCrate,
					ItemID.DungeonFishingCrate,
					ItemID.FloatingIslandFishingCrate,
					ItemID.HallowedFishingCrate,
					ItemID.Oyster
				}));
			AddProgressionGroup(new(ProgressionGroupID.UndergroundJungle, 100,
				itemTypes: new SortedSet<int>() {
					ItemID.JungleSpores,
					ItemID.Boomstick,
					ItemID.StaffofRegrowth,
					ItemID.RichMahogany,
					ItemID.Hive,
					ItemID.HoneyBlock,
					ItemID.JungleRose,
					ItemID.NaturesGift,
					ItemID.FeralClaws,
					ItemID.HoneyBucket,
					ItemID.HoneyDispenser,
					ItemID.Seaweed
				},
				npcTypes: new SortedSet<int>() {
					NPCID.Hornet,
					NPCID.ManEater
				}));
			AddProgressionGroup(new(ProgressionGroupID.DemonicOrbOrCrimsonHeart, 100,
				itemTypes: new SortedSet<int>() {
					ItemID.TheUndertaker,
					ItemID.CrimsonRod,
					ItemID.TheRottedFork,
					ItemID.Musket,
					ItemID.Vilethorn,
					ItemID.BallOHurt
				}));//From WorldGen.CheckOrb()
			AddProgressionGroup(new(ProgressionGroupID.ArmsDealerShop, 0, ProgressionGroupID.DemonicOrbOrCrimsonHeart,
				itemTypes: new SortedSet<int>() {
					ItemID.Minishark,
					ItemID.FlintlockPistol,
					ItemID.IllegalGunParts
				}));
			AddProgressionGroup(new(ProgressionGroupID.Granite, 105,
				itemTypes: new SortedSet<int>() {
					ItemID.Granite
				},
				npcTypes: new SortedSet<int>() {
					NPCID.GraniteGolem
				}));
			AddProgressionGroup(new(ProgressionGroupID.Marble, 105,
				itemTypes: new SortedSet<int>() {
					ItemID.Marble
				},
				npcTypes: new SortedSet<int>() {
					NPCID.GreekSkeleton
				}));
			AddProgressionGroup(new(ProgressionGroupID.HolidayWeapons, 110,
				itemTypes: new SortedSet<int>() {
					ItemID.BloodyMachete,
					ItemID.BladedGlove,
					ItemID.FruitcakeChakram,
					ItemID.CandyCaneSword,
					ItemID.CnadyCanePickaxe,
					ItemID.RedRyder
				}));
			AddProgressionGroup(new(ProgressionGroupID.PostEyeEasy, -10, ProgressionGroupID.Eye,
				itemTypes: new SortedSet<int>() {
					ItemID.Revolver,//Traveling Merchant
					ItemID.ZapinatorGray,//Traveling Merchant
					ItemID.Code1,//Traveling Merchant
					ItemID.PurificationPowder,//Dryad
					ItemID.PumpkinSeed,//Dryad
					ItemID.FlowerPacketWhite,//Dryad
					ItemID.Pumpkin
				}));//110
			AddProgressionGroup(new(ProgressionGroupID.DemoniteOre, 0, ProgressionGroupID.Eye,
				itemTypes: new SortedSet<int>() {
					ItemID.DemoniteOre
				}));//120
			AddProgressionGroup(new(ProgressionGroupID.CrimtaneOre, 0, ProgressionGroupID.Eye,
				itemTypes: new SortedSet<int>() {
					ItemID.CrimtaneOre
				}));//120
			AddProgressionGroup(new(ProgressionGroupID.Eye, 120));
			AddProgressionGroup(new(ProgressionGroupID.BloodMoon, 150,
				npcTypes: new SortedSet<int>() {
					NPCID.TheGroom,
					NPCID.BloodZombie
				}));
			AddProgressionGroup(new(ProgressionGroupID.PostGoblinArmyEasy, -10, ProgressionGroupID.GoblinArmy,
				itemTypes: new SortedSet<int>() {
					ItemID.Ruler,
					ItemID.TinkerersWorkshop
				}));//170
			AddProgressionGroup(new(ProgressionGroupID.GoblinArmy, 180,
				npcTypes: new SortedSet<int>() {
					NPCID.GoblinPeon,
					NPCID.GoblinSorcerer
				}));
			AddProgressionGroup(new(ProgressionGroupID.SwordShrine, 180,
				itemTypes: new SortedSet<int>() {
					ItemID.EnchantedSword,
					ItemID.Terragrim
				}));
			AddProgressionGroup(new(ProgressionGroupID.BloodMoonFishing, 40, ProgressionGroupID.BloodMoon,
				npcTypes: new SortedSet<int>() {
					NPCID.ZombieMerman,
					NPCID.EyeballFlyingFish
				}));//190
			AddProgressionGroup(new(ProgressionGroupID.Hell, 190,
				itemTypes: new SortedSet<int>() {
					ItemID.Fireblossom,
					ItemID.AshWood,
					ItemID.Obsidifish
				},
				npcTypes: new SortedSet<int>() {
					NPCID.Demon,
					NPCID.VoodooDemon,
					NPCID.FireImp,
					NPCID.BoneSerpentHead
				},
				lootItemTypes: new SortedSet<int>() {
					ItemID.LavaCrate
				}));
			AddProgressionGroup(new(ProgressionGroupID.OldOneArmyT1, -10, ProgressionGroupID.EaterBrain,
				itemTypes: new SortedSet<int>() {
					ItemID.DD2FlameburstTowerT1Popper,
					ItemID.DD2BallistraTowerT1Popper,
					ItemID.DD2ExplosiveTrapT1Popper,
					ItemID.DD2LightningAuraT1Popper,
				},
				npcTypes: new SortedSet<int>() {
					NPCID.DD2DarkMageT1
				}));//190
			AddProgressionGroup(new(ProgressionGroupID.PostEaterBrainEasy, -10, ProgressionGroupID.EaterBrain));//190
			AddProgressionGroup(new(ProgressionGroupID.EaterBrain, 200));
			AddProgressionGroup(new(ProgressionGroupID.MeteoriteOre, 210,
				itemTypes: new SortedSet<int>() {
					ItemID.Meteorite
				}));
			AddProgressionGroup(new(ProgressionGroupID.HellstoneOre, 20, ProgressionGroupID.EaterBrain,
				itemTypes: new SortedSet<int>() {
					ItemID.Hellstone,
					ItemID.Hellforge
				}));//220
			AddProgressionGroup(new(ProgressionGroupID.PostBeeEasy, -10, ProgressionGroupID.Bee,
				itemTypes: new SortedSet<int>() {
					ItemID.Blowgun,
					ItemID.ImbuingStation
				}));//240
			AddProgressionGroup(new(ProgressionGroupID.Bee, 250));
			AddProgressionGroup(new(ProgressionGroupID.PostSkeletronEasy, -10, ProgressionGroupID.Skeletron,
				itemTypes: new SortedSet<int>() {
					ItemID.Book,
					ItemID.WaterBolt,
					ItemID.Cascade,
					ItemID.Spike,
					ItemID.QuadBarrelShotgun,
					ItemID.TragicUmbrella,
					ItemID.Wire,
					ItemID.Lever,
					ItemID.MechanicalLens,
					ItemID.Wrench,//Mechanic
					ItemID.PlacePainting//Clothier
				},
				npcTypes: new SortedSet<int>() {
					NPCID.AngryBones,
					NPCID.DarkCaster,
					NPCID.Mechanic
				}));//290
			AddProgressionGroup(new(ProgressionGroupID.Skeletron, 300));
			AddProgressionGroup(new(ProgressionGroupID.Dungeon, 320,
				itemTypes: new SortedSet<int>() {
					ItemID.BlueMoon,
					ItemID.MagicMissile,
					ItemID.Muramasa,
					ItemID.Handgun,
					ItemID.Valor,
					ItemID.AquaScepter,
					ItemID.AlchemyTable
				},
				npcTypes: new SortedSet<int>() {
					//NPCID.DungeonSlime,
					NPCID.CursedSkull
				},
				lootItemTypes: new SortedSet<int>() {
					ItemID.LockBox
				}));
			AddProgressionGroup(new(ProgressionGroupID.ShadowChest, 350,
				itemTypes: new SortedSet<int>() {
					ItemID.Sunfury,
					ItemID.FlowerofFire,
					ItemID.Flamelash,
					ItemID.DarkLance,
					ItemID.HellwingBow,
					ItemID.TreasureMagnet,
					ItemID.BoneWelder,
					ItemID.HellCake,
					ItemID.OrnateShadowKey
				},
				lootItemTypes: new SortedSet<int>() {
					ItemID.ObsidianLockbox
				}));
			AddProgressionGroup(new(ProgressionGroupID.Deer, 380));
			AddProgressionGroup(new(ProgressionGroupID.DungeonGuardian, 390,
				npcTypes: new SortedSet<int>() {
					NPCID.DungeonGuardian
				}));
			AddProgressionGroup(new(ProgressionGroupID.HardMode, 400,
				itemTypes: new SortedSet<int>() {
					ItemID.Amarok,
					ItemID.HelFire
				},
				npcTypes: new SortedSet<int>() {
					NPCID.AngryNimbus,
					NPCID.TaxCollector,
					NPCID.PossessedArmor
				},
				lootItemTypes: new SortedSet<int>() {
					ItemID.WoodenCrateHard,
					ItemID.IronCrateHard,
					ItemID.GoldenCrateHard,
					ItemID.CorruptFishingCrateHard,
					ItemID.CrimsonFishingCrateHard,
					ItemID.DungeonFishingCrateHard,
					ItemID.FloatingIslandFishingCrateHard,
					ItemID.FrozenCrateHard,
					ItemID.HallowedFishingCrateHard,
					ItemID.JungleFishingCrateHard,
					ItemID.OasisCrateHard,
					ItemID.OceanCrateHard
				}));
			AddProgressionGroup(new(ProgressionGroupID.HardmodeShopItems, 400,
				itemTypes: new SortedSet<int>() {
					ItemID.JoustingLance,//BestiaryGirl
					ItemID.HallowedSeeds,//Dryad
					ItemID.Shotgun,//Arms Dealer
					ItemID.Harp,//Wizard
					ItemID.Bell,//Wizard
					ItemID.IceRod,//Wizard
					ItemID.CrystalBall,//Wizard
					ItemID.MusicBox,//Wizard
					ItemID.SpellTome,//Wizard
					ItemID.EmptyDropper,//Wizard
					ItemID.ZapinatorOrange,//Traveling Merchant
					ItemID.Gatligator,//Traveling Merchant
					ItemID.BouncingShield,//Traveling Merchant
					ItemID.Gradient,//Skeleton Merchant
					ItemID.FormatC,//Skeleton Merchant
					ItemID.ExplosivePowder,//Demolitionist
					ItemID.CowboyHat,//Clothier
					ItemID.GoldDust,//Merchant
					ItemID.TikiTotem,//Witch Doctor
					ItemID.BallOfFuseWire//Zoologist
				}));
			AddProgressionGroup(new(ProgressionGroupID.HardModeUnderground, 10, ProgressionGroupID.HardMode,
				itemTypes: new SortedSet<int>() {
					ItemID.LivingFireBlock
				},
				npcTypes: new SortedSet<int>() {
					NPCID.SkeletonArcher,
					NPCID.ArmoredSkeleton,
					NPCID.BlackRecluse,
					NPCID.DesertGhoul,
					NPCID.MossHornet,
					NPCID.DesertScorpionWalk,
					NPCID.GiantTortoise,
					NPCID.Derpling,
					NPCID.ToxicSludge
				},
				lootItemTypes: new SortedSet<int>() {
					ItemID.LavaCrateHard
				}));//410
			AddProgressionGroup(new(ProgressionGroupID.Wall, 420));
			AddProgressionGroup(new(ProgressionGroupID.Hallow, 420,
				itemTypes: new SortedSet<int>() {
					ItemID.Pearlwood,
					ItemID.PearlstoneBlock,
				},
				npcTypes: new SortedSet<int>() {
					NPCID.Pixie,
					NPCID.Unicorn
				}));
			AddProgressionGroup(new(ProgressionGroupID.HardModeFishing, 420,
				itemTypes: new SortedSet<int>() {
					ItemID.Anchor,
					ItemID.ObsidianSwordfish,
					ItemID.ScalyTruffle
				}));
			AddProgressionGroup(new(ProgressionGroupID.HardModeNight, 430));
			AddProgressionGroup(new(ProgressionGroupID.CobaltOre, 430,
				itemTypes: new SortedSet<int>() {
					ItemID.CobaltOre
				}));
			AddProgressionGroup(new(ProgressionGroupID.PalladiumOre, 430,
				itemTypes: new SortedSet<int>() {
					ItemID.PalladiumOre
				}));
			AddProgressionGroup(new(ProgressionGroupID.Wyvern, 430,
				npcTypes: new SortedSet<int>() {
					NPCID.WyvernHead
				}));
			AddProgressionGroup(new(ProgressionGroupID.MushroomHardMode, 435,
				itemTypes: new SortedSet<int>() {
					ItemID.GlowingMushroom
				}));
			AddProgressionGroup(new(ProgressionGroupID.HardModeBloodMoon, 10, ProgressionGroupID.HardModeNight,
				itemTypes: new SortedSet<int>() {
					ItemID.KOCannon,
					ItemID.SlapHand,
					ItemID.FullMoonSqueakyToy
				},
				npcTypes: new SortedSet<int>() {
					NPCID.Clown
				}));//440
			AddProgressionGroup(new(ProgressionGroupID.UndergroundHallow, 440,
				itemTypes: new SortedSet<int>() {
					ItemID.SoulofLight,
					ItemID.CrystalShard
				},
				npcTypes: new SortedSet<int>() {
					NPCID.PigronHallow,
					NPCID.LightMummy
				}));
			AddProgressionGroup(new(ProgressionGroupID.UndergroundEvil, 440,
				itemTypes: new SortedSet<int>() {
					ItemID.SoulofNight
				},
				npcTypes: new SortedSet<int>() {
					NPCID.DarkMummy,
					NPCID.Clinger,
					NPCID.IchorSticker
				}));
			AddProgressionGroup(new(ProgressionGroupID.MythrilOre, 440,
				itemTypes: new SortedSet<int>() {
					ItemID.MythrilOre
				}));
			AddProgressionGroup(new(ProgressionGroupID.OrichalcumOre, 440,
				itemTypes: new SortedSet<int>() {
					ItemID.OrichalcumOre
				}));
			AddProgressionGroup(new(ProgressionGroupID.HardModeBloodMoonFishing, 10, ProgressionGroupID.HardModeBloodMoon,
				npcTypes: new SortedSet<int>() {
					NPCID.GoblinShark,
					NPCID.BloodEelHead
				}));//450
			AddProgressionGroup(new(ProgressionGroupID.FrostLegeon, 450,
				npcTypes: new SortedSet<int>() {
					NPCID.SnowBalla,
					NPCID.MisterStabby,
					NPCID.SnowmanGangsta
				}));
			AddProgressionGroup(new(ProgressionGroupID.PostFrostLegeonEasy, -10, ProgressionGroupID.FrostLegeon,
				itemTypes: new SortedSet<int>() {
					ItemID.PaintingColdSnap,
					ItemID.PaintingTheSeason
				}));
			AddProgressionGroup(new(ProgressionGroupID.AdamantiteOre, 450,
				itemTypes: new SortedSet<int>() {
					ItemID.AdamantiteOre
				}));
			AddProgressionGroup(new(ProgressionGroupID.TitaniumOre, 450,
				itemTypes: new SortedSet<int>() {
					ItemID.TitaniumOre
				}));
			AddProgressionGroup(new(ProgressionGroupID.HardModeRare, 460,
				npcTypes: new SortedSet<int>() {
					NPCID.Mimic,
					NPCID.IceMimic,
					NPCID.IceElemental,
					NPCID.Medusa,
					NPCID.AngryTrapper,
					NPCID.DesertDjinn,
					NPCID.IceGolem,
					NPCID.SandElemental,
					NPCID.FlyingSnake
				}));
			AddProgressionGroup(new(ProgressionGroupID.HardModeFishingRare, 460,
				itemTypes: new SortedSet<int>() {
					ItemID.CrystalSerpent,
					ItemID.Toxikarp,
					ItemID.Bladetongue
				}));
			AddProgressionGroup(new(ProgressionGroupID.HardModeBloodMoonFishingRare, 30, ProgressionGroupID.HardModeBloodMoonFishing,
				npcTypes: new SortedSet<int>() {
					NPCID.BloodNautilus
				}));
			AddProgressionGroup(new(ProgressionGroupID.GoblinArmyHardMode, 490,
				npcTypes: new SortedSet<int>() {
					NPCID.GoblinSummoner
				}));
			AddProgressionGroup(new(ProgressionGroupID.BigMimics, 500,
				npcTypes: new SortedSet<int>() {
					NPCID.BigMimicJungle,
					NPCID.BigMimicCorruption,
					NPCID.BigMimicCrimson,
					NPCID.BigMimicHallow
				}));
			AddProgressionGroup(new(ProgressionGroupID.PostPiratesEasy, -10, ProgressionGroupID.Pirates,
				itemTypes: new SortedSet<int>() {
					ItemID.Cannonball,
					ItemID.ConfettiCannon,
					ItemID.ParrotCracker
				}));
			AddProgressionGroup(new(ProgressionGroupID.Pirates, 545,
				npcTypes: new SortedSet<int>() {
					NPCID.PirateDeckhand,
					NPCID.PirateDeadeye,
					NPCID.PirateCaptain,
					NPCID.PirateShip
				}));
			AddProgressionGroup(new(ProgressionGroupID.Eclipse, 560,
				npcTypes: new SortedSet<int>() {
					NPCID.Reaper,
					NPCID.Frankenstein,
					NPCID.SwampThing,
					NPCID.Eyezor
				}));
			AddProgressionGroup(new(ProgressionGroupID.QueenSlime, 575));
			AddProgressionGroup(new(ProgressionGroupID.OldOneArmyT2, -10, ProgressionGroupID.Destroyer,
				itemTypes: new SortedSet<int>() {
					ItemID.DD2FlameburstTowerT2Popper,
					ItemID.DD2BallistraTowerT2Popper,
					ItemID.DD2ExplosiveTrapT2Popper,
					ItemID.DD2LightningAuraT2Popper,
				},
				npcTypes: new SortedSet<int>() {
					NPCID.DD2OgreT2,
					NPCID.DD2OgreT3
				}));//595
			AddProgressionGroup(new(ProgressionGroupID.PostMechanicalBoss, -10, ProgressionGroupID.Destroyer,
				itemTypes: new SortedSet<int>() {
					ItemID.Yelets,
					ItemID.LifeFruit,
					ItemID.MushroomSpear,//Truffle
					ItemID.Hammush,//Truffle
					ItemID.StrangeGlowingMushroom,//Truffle
					ItemID.Cog,//SteamPunker
					ItemID.BlendOMatic,//SteamPunker
					ItemID.FleshCloningVaat,//SteamPunker
					ItemID.SteampunkBoiler,//SteamPunker
					ItemID.LesionStation,//SteamPunker
					ItemID.Code2//Traveling Merchant
				},
				npcTypes: new SortedSet<int>() {
					NPCID.RedDevil,
					NPCID.Moth
				}));//595
			AddProgressionGroup(new(ProgressionGroupID.AnyHardmodeTreasureBagRare, 600,
				itemTypes: new SortedSet<int>() {
					ItemID.Arkhalis,
					ItemID.ValkyrieYoyo,
					ItemID.RedsYoyo
				}));
			AddProgressionGroup(new(ProgressionGroupID.Destroyer, 605));
			AddProgressionGroup(new(ProgressionGroupID.SkeletronPrime, 615));
			AddProgressionGroup(new(ProgressionGroupID.Twins, 630));
			AddProgressionGroup(new(ProgressionGroupID.PostAllMechanicalBosses, 640,
				itemTypes: new SortedSet<int>() {
					ItemID.PulseBow
				}));
			AddProgressionGroup(new(ProgressionGroupID.ChlorophyteOre, 650,
				itemTypes: new SortedSet<int>() {
					ItemID.ChlorophyteOre
				}));
			AddProgressionGroup(new(ProgressionGroupID.PostPlanteraEasy, -10, ProgressionGroupID.Plantera,
				itemTypes: new SortedSet<int>() {
					ItemID.Autohammer,
					ItemID.Nanites,//Cyborg
					ItemID.ProximityMineLauncher,//Cyborg
					ItemID.VialofVenom,
					ItemID.RocketIV,
					ItemID.LihzahrdFurnace,
					ItemID.BerniePetItem,//Princess
					ItemID.MudBud
				},
				npcTypes: new SortedSet<int>() {
					NPCID.Princess
				}));//715
			AddProgressionGroup(new(ProgressionGroupID.Plantera, 725));
			AddProgressionGroup(new(ProgressionGroupID.DungeonPostPlantera, 750,
				itemTypes: new SortedSet<int>() {
					ItemID.Kraken
				},
				npcTypes: new SortedSet<int>() {
					NPCID.DungeonSpirit,
					NPCID.HellArmoredBones
				}));
			AddProgressionGroup(new(ProgressionGroupID.DungeonPostPlanteraRare, 775,
				npcTypes: new SortedSet<int>() {
					NPCID.BlueArmoredBones,
					NPCID.TacticalSkeleton,
					NPCID.Necromancer,
					NPCID.RaggedCaster,
					NPCID.SkeletonCommando,
					NPCID.SkeletonSniper,
					NPCID.Paladin,
					NPCID.GiantCursedSkull,
					NPCID.DiabolistRed,
					NPCID.BoneLee
				}));
			AddProgressionGroup(new(ProgressionGroupID.BiomeChests, 800,
				itemTypes: new SortedSet<int>() {
					ItemID.PiranhaGun,
					ItemID.ScourgeoftheCorruptor,
					ItemID.VampireKnives,
					ItemID.RainbowGun,
					ItemID.StaffoftheFrostHydra,
					ItemID.StormTigerStaff
				}));
			AddProgressionGroup(new(ProgressionGroupID.EclipsePostPlantera, 800,
				npcTypes: new SortedSet<int>() {
					NPCID.Butcher,
					NPCID.DrManFly,
					NPCID.Psycho,
					NPCID.Nailhead,
					NPCID.DeadlySphere
				}));
			AddProgressionGroup(new(ProgressionGroupID.EclipsePostPlanteraRare, 10, ProgressionGroupID.EclipsePostPlantera,
				npcTypes: new SortedSet<int>() {
					NPCID.Mothron
				}));
			AddProgressionGroup(new(ProgressionGroupID.PostFrostMoonEasy, -10, ProgressionGroupID.FrostMoon));//815
			AddProgressionGroup(new(ProgressionGroupID.PumpkinMoon, 820,
				npcTypes: new SortedSet<int>() {
					NPCID.SantaNK1,
					NPCID.IceQueen
				}));
			AddProgressionGroup(new(ProgressionGroupID.FrostMoon, 820,
				npcTypes: new SortedSet<int>() {
					NPCID.Everscream,
					NPCID.Pumpking,
					NPCID.MourningWood,
					NPCID.ElfCopter
				}));
			AddProgressionGroup(new(ProgressionGroupID.OldOneArmyT3, -10, ProgressionGroupID.Golem,
				itemTypes: new SortedSet<int>() {
					ItemID.DD2FlameburstTowerT3Popper,
					ItemID.DD2BallistraTowerT3Popper,
					ItemID.DD2ExplosiveTrapT3Popper,
					ItemID.DD2LightningAuraT3Popper,
				}));//835
			AddProgressionGroup(new(ProgressionGroupID.PostGolemEasy, -10, ProgressionGroupID.Golem,
				itemTypes: new SortedSet<int>() {
					ItemID.FireworksLauncher
				}));//835
			AddProgressionGroup(new(ProgressionGroupID.Betsey, -5, ProgressionGroupID.Golem));//840
			AddProgressionGroup(new(ProgressionGroupID.Golem, 845));
			AddProgressionGroup(new(ProgressionGroupID.MartianInvasion, 860,
				npcTypes: new SortedSet<int>() {
					NPCID.MartianOfficer,
					NPCID.MartianWalker,
					NPCID.MartianEngineer
				}));
			AddProgressionGroup(new(ProgressionGroupID.PostMartianSaucerEasy, -10, ProgressionGroupID.MartianSaucer,
				itemTypes: new SortedSet<int>() {
					ItemID.PaintingTheTruthIsUpThere
				}));//870
			AddProgressionGroup(new(ProgressionGroupID.MartianSaucer, 880,
				npcTypes: new SortedSet<int>() {
					NPCID.MartianSaucerCore
				}));
			AddProgressionGroup(new(ProgressionGroupID.EmpressNight, 910));
			AddProgressionGroup(new(ProgressionGroupID.DukeFishron, 940));
			AddProgressionGroup(new(ProgressionGroupID.Empress, 60, ProgressionGroupID.EmpressNight,
				itemTypes: new SortedSet<int>() {
					ItemID.EmpressBlade
				}));//970
			AddProgressionGroup(new(ProgressionGroupID.PostLunaticCultistEasy, -10, ProgressionGroupID.LunaticCultist));
			AddProgressionGroup(new(ProgressionGroupID.LunaticCultist, 975,
				npcTypes: new SortedSet<int>() {
					NPCID.CultistBoss
				}));
			AddProgressionGroup(new(ProgressionGroupID.LunarInvasion, 1005,
				itemTypes: new SortedSet<int>() {
					ItemID.FragmentNebula,
					ItemID.FragmentSolar,
					ItemID.FragmentStardust,
					ItemID.FragmentVortex
				},
				npcTypes: new SortedSet<int>() {
					NPCID.StardustWormHead,
					NPCID.LunarTowerVortex
				}));
			AddProgressionGroup(new(ProgressionGroupID.PostMoonLordEasy, -50, ProgressionGroupID.MoonLord,
				itemTypes: new SortedSet<int>() {
					ItemID.LunarOre
				}));//1050
			AddProgressionGroup(new(ProgressionGroupID.MoonLord, 1100));

			#endregion

			if (AndroMod.calamityEnabled) {
				progressionGroups[ProgressionGroupID.Beach].AddNPCs(
					new SortedSet<string>() {
						"CalamityMod/SeaUrchin",
						"CalamityMod/MorayEel"
					});//30
				progressionGroups[ProgressionGroupID.ForestPreHardModeRare].AddNPCs(
					new SortedSet<string>() {
						"CalamityMod/WulfrumAmplifier"
					});//40
				progressionGroups[ProgressionGroupID.Evil].AddNPCs(
					new SortedSet<string>() {
						"CalamityMod/EbonianBlightSlime"
					});//80
				progressionGroups[ProgressionGroupID.UndergroundDesert].AddNPCs(
					new SortedSet<string>() {
						"CalamityMod/Stormlion"
					});//90
				progressionGroups[ProgressionGroupID.Fishing].AddItems(
					new SortedSet<string>() {
						"CalamityMod/Spadefish",
						"CalamityMod/SparklingEmpress"
					});//90
				progressionGroups[ProgressionGroupID.Hell].AddNPCs(
					new SortedSet<string>() {
						"CalamityMod/Scryllar",
						"CalamityMod/ScryllarRage"
					});//190
				progressionGroups[ProgressionGroupID.Hell].AddItems(
					new SortedSet<string>() {
						"CalamityMod/DragoonDrizzlefish"
					});//190
				progressionGroups[ProgressionGroupID.PostSkeletronEasy].AddItems(
					new SortedSet<string>() {
						"CalamityMod/Cinquedea",
						"CalamityMod/Glaive",
						"CalamityMod/Kylie"
					});//290
				progressionGroups[ProgressionGroupID.HardmodeShopItems].AddItems(
					new SortedSet<string>() {
						"CalamityMod/P90",
						"CalamityMod/SlickCane",
						"CalamityMod/SpitefulCandle"
					});//400
				progressionGroups[ProgressionGroupID.HardMode].AddItems(
					new SortedSet<string>() {
						"CalamityMod/EssenceofHavoc",
						"CalamityMod/EssenceofEleum",
						"CalamityMod/EssenceofSunlight",
						"CalamityMod/ClothiersWrath"
					});//400
				progressionGroups[ProgressionGroupID.HardModeUnderground].AddNPCs(
					new SortedSet<string>() {
						"CalamityMod/IceClasper"
					});//410
				progressionGroups[ProgressionGroupID.HardModeUnderground].AddItems(
					new SortedSet<string>() {
						"CalamityMod/EvilSmasher"
					});//410
				progressionGroups[ProgressionGroupID.PostMechanicalBoss].AddItems(
					new SortedSet<string>() {
						"CalamityMod/DeepcoreGK2"
					});//595
				progressionGroups[ProgressionGroupID.HardModeRare].AddNPCs(
					new SortedSet<string>() {
						"CalamityMod/Horse",
						"CalamityMod/ThiccWaifu"
					});//460
				progressionGroups[ProgressionGroupID.PostPlanteraEasy].AddItems(
					new SortedSet<string>() {
						"CalamityMod/MantisClaws",
						"CalamityMod/PerennialOre",
						"CalamityMod/MonkeyDarts"
					});//715
				progressionGroups[ProgressionGroupID.PostGolemEasy].AddItems(
					new SortedSet<string>() {
						"CalamityMod/ScoriaOre"
					});//835
				progressionGroups[ProgressionGroupID.PostMoonLordEasy].AddItems(
					new SortedSet<string>() {
						"CalamityMod/CelestialReaper",
						"CalamityMod/Phantoplasm"
					});//1050
				AddProgressionGroup(new(ProgressionGroupID.ArsenalLabs, 70,
					itemNames: new SortedSet<string>() {
						"CalamityMod/DubiousPlating",
						"CalamityMod/MysteriousCircuitry",
						"CalamityMod/PlasmaDriveCore",
						"CalamityMod/SuspiciousScrap"
					}));
				AddProgressionGroup(new(ProgressionGroupID.SulfurousSea, 80,
					itemNames: new SortedSet<string>() {
						"CalamityMod/Acidwood",
						"CalamityMod/SulphurousSand"
					}));
				AddProgressionGroup(new(ProgressionGroupID.DesertScourge, 87));
				AddProgressionGroup(new(ProgressionGroupID.SunkenSea, 92,
					itemNames: new SortedSet<string>() {
						"CalamityMod/Navystone"
					}));
				AddProgressionGroup(new(ProgressionGroupID.GiantClam, 97));
				AddProgressionGroup(new(ProgressionGroupID.SeaKingShop, 105,
					itemNames: new SortedSet<string>() {
						"CalamityMod/Shellshooter",
						"CalamityMod/SnapClam",
						"CalamityMod/SandDollar",
						"CalamityMod/Waywasher",
						"CalamityMod/CoralCannon",
						"CalamityMod/UrchinFlail",
						"CalamityMod/AmidiasTrident",
						"CalamityMod/EnchantedConch",
						"CalamityMod/PolypLauncher"
					}));
				AddProgressionGroup(new(ProgressionGroupID.PrismShard, 120,
					itemNames: new SortedSet<string>() {
						"CalamityMod/PrismShard"
					}));
				AddProgressionGroup(new(ProgressionGroupID.AcidRainT1, 125,
					npcNames: new SortedSet<string>() {
						"CalamityMod/NuclearToad",
						"CalamityMod/AcidEel",
						"CalamityMod/Skyfin"
					}));
				AddProgressionGroup(new(ProgressionGroupID.Crabulon, 150));
				AddProgressionGroup(new(ProgressionGroupID.AerialiteOre, -10, ProgressionGroupID.HiveMind,
					itemNames: new SortedSet<string>() {
						"CalamityMod/AerialiteOre"
					}));//215
				AddProgressionGroup(new(ProgressionGroupID.HiveMind, 225,
					ignoredItemTypes: new SortedSet<int>() {
						ItemID.CursedFlame
					}));
				AddProgressionGroup(new(ProgressionGroupID.Perforators, 225,
					ignoredItemTypes: new SortedSet<int>() {
						ItemID.Ichor
					}));
				AddProgressionGroup(new(ProgressionGroupID.AbyssT1, 260,
					itemNames: new SortedSet<string>() {
						"CalamityMod/PlantyMush",
						"CalamityMod/BlackAnurian",
						"CalamityMod/BallOFugu",
						"CalamityMod/Archerfish",
						"CalamityMod/Lionfish",
						"CalamityMod/HerringStaff"
					},
					npcNames: new SortedSet<string>() {
						"CalamityMod/BoxJellyfish"
					}));
				AddProgressionGroup(new(ProgressionGroupID.SlimeGod, 390));
				AddProgressionGroup(new(ProgressionGroupID.AstraulInfection, 420,
					itemNames: new SortedSet<string>() {
						"CalamityMod/AstralGrassSeeds",
						"CalamityMod/PolarisParrotfish",
						"CalamityMod/GacruxianMollusk",
						"CalamityMod/GloriousEnd",
						"CalamityMod/Stardust"
					}));
				AddProgressionGroup(new(ProgressionGroupID.HardModeSunkenSea, 420,
					itemNames: new SortedSet<string>() {
						"CalamityMod/MolluskHusk"
					},
					npcNames: new SortedSet<string>() {
						"CalamityMod/SeaSerpent1",
						"CalamityMod/BlindedAngler"
					}));
				AddProgressionGroup(new(ProgressionGroupID.GiantClamHardMode, 425,
					itemNames: new SortedSet<string>() {
						"CalamityMod/Poseidon",
						"CalamityMod/ClamCrusher",
						"CalamityMod/ClamorRifle",
						"CalamityMod/ShellfishStaff"
					}));
				AddProgressionGroup(new(ProgressionGroupID.InfernalSuevite, 450,
					itemNames: new SortedSet<string>() {
						"CalamityMod/InfernalSuevite"
					}));
				AddProgressionGroup(new(ProgressionGroupID.HardModeSulphurousSea, 480));
				AddProgressionGroup(new(ProgressionGroupID.Voidstone, 480,
					itemNames: new SortedSet<string>() {
						"CalamityMod/Voidstone"
					}));
				AddProgressionGroup(new(ProgressionGroupID.PostCryogenEasy, -10, ProgressionGroupID.Cryogen,
					itemNames: new SortedSet<string>() {
						"CalamityMod/CryonicOre",
						"CalamityMod/FrostbiteBlaster",
						"CalamityMod/IcicleTrident",
						"CalamityMod/IceStar"
					}));
				AddProgressionGroup(new(ProgressionGroupID.Cryogen, 590));
				AddProgressionGroup(new(ProgressionGroupID.PostAquaticScourgeEasy, -10, ProgressionGroupID.AquaticScourge,
					npcNames: new SortedSet<string>() {
						"CalamityMod/BelchingCoral"
					}));
				AddProgressionGroup(new(ProgressionGroupID.AquaticScourge, 600));
				AddProgressionGroup(new(ProgressionGroupID.AcidRainT2, 0, ProgressionGroupID.AquaticScourge,
					itemNames: new SortedSet<string>() {
						"CalamityMod/SlitheringEels",
						"CalamityMod/SkyfinBombers"
					},
					npcNames: new SortedSet<string>() {
						"CalamityMod/SulphurousSkater",
						"CalamityMod/FlakCrab",
						"CalamityMod/Orthocera"
					}));
				AddProgressionGroup(new(ProgressionGroupID.CragmawMire, 610,
					npcNames: new SortedSet<string>() {
						"CalamityMod/CragmawMire"
					}));
				AddProgressionGroup(new(ProgressionGroupID.BrimstoneElemental, 625));
				progressionGroups[ProgressionGroupID.PostAllMechanicalBosses].AddItems(
					new SortedSet<string>() {
						"CalamityMod/ArcticBearPaw",
						"CalamityMod/Cryophobia",
						"CalamityMod/CryogenicStaff"
					});//640
				progressionGroups[ProgressionGroupID.PostAllMechanicalBosses].AddItems(
					new SortedSet<string>() {
						"CalamityMod/FrostyFlare"
					});//640
				AddProgressionGroup(new(ProgressionGroupID.PostCalamitasCloneEasy, -10, ProgressionGroupID.CalamitasClone,
					itemNames: new SortedSet<string>() {
						"CalamityMod/DepthCells",
						"CalamityMod/Lumenyl",
						"CalamityMod/DeepWounder"
					}));//690
				AddProgressionGroup(new(ProgressionGroupID.CalamitasClone, 700,
					npcNames: new SortedSet<string>() {
						"CalamityMod/Cataclysm",
						"CalamityMod/Catastrophe",
						"CalamityMod/CalamitasClone"
					}));
				AddProgressionGroup(new(ProgressionGroupID.GreatSandShark, 750,
					npcNames: new SortedSet<string>() {
						"CalamityMod/GreatSandShark"
					}));
				progressionGroups[ProgressionGroupID.PostFrostMoonEasy].AddItems(
					new SortedSet<string>() {
						"CalamityMod/AbsoluteZero",
						"CalamityMod/EternalBlizzard",
						"CalamityMod/WintersFury"
					});//640
				AddProgressionGroup(new(ProgressionGroupID.Leviathan, 835));
				AddProgressionGroup(new(ProgressionGroupID.PostAstrumAuresEasy, -10, ProgressionGroupID.AstrumAures,
					itemNames: new SortedSet<string>() {
						"CalamityMod/AegisBlade",
						"CalamityMod/TitanArm",
						"CalamityMod/AstralScythe",
						"CalamityMod/AstralachneaStaff",
						"CalamityMod/StellarCannon",
						"CalamityMod/HivePod",
						"CalamityMod/AbandonedSlimeStaff",
						"CalamityMod/StellarKnife"
					}));
				AddProgressionGroup(new(ProgressionGroupID.AstrumAures, 840));
				AddProgressionGroup(new(ProgressionGroupID.PostAstrumAures, 10, ProgressionGroupID.AstrumAures,
					itemNames: new SortedSet<string>() {
						"CalamityMod/HeavenfallenStardisk"
					}));
				AddProgressionGroup(new(ProgressionGroupID.PlaguebringerGoliath, 900));
				AddProgressionGroup(new(ProgressionGroupID.Ravager, 970));
				AddProgressionGroup(new(ProgressionGroupID.PostAstrumDeusEasy, -10, ProgressionGroupID.AstrumDeus,
					itemNames: new SortedSet<string>() {
						"CalamityMod/AstralOre"
					}));
				AddProgressionGroup(new(ProgressionGroupID.AstrumDeus, 1010));
				AddProgressionGroup(new(ProgressionGroupID.PostMoonLord, 1105,
					itemNames: new SortedSet<string>() {
						"CalamityMod/ExodiumCluster",
						"CalamityMod/UnholyEssence"
					},
					npcNames: new SortedSet<string>() {
						"CalamityMod/ImpiousImmolator"
					}));
				AddProgressionGroup(new(ProgressionGroupID.ProfanedGuardians, 1110,
					npcNames: new SortedSet<string>() {
						"CalamityMod/ProfanedGuardianCommander"
					}));
				AddProgressionGroup(new(ProgressionGroupID.DragonFolly, 1130));
				AddProgressionGroup(new(ProgressionGroupID.PostProvidenceEasy, -10, ProgressionGroupID.Providence,
					itemNames: new SortedSet<string>() {
						"CalamityMod/Bloodstone",
						"CalamityMod/GuidelightofOblivion"
					}));
				AddProgressionGroup(new(ProgressionGroupID.Providence, 1145));
				AddProgressionGroup(new(ProgressionGroupID.UelibloomBar, 15, ProgressionGroupID.Providence,
					itemNames: new SortedSet<string>() {
						"CalamityMod/UelibloomOre"
					}));
				AddProgressionGroup(new(ProgressionGroupID.CeaselessVoid, 1180));
				AddProgressionGroup(new(ProgressionGroupID.StormWeaver, 1200));
				AddProgressionGroup(new(ProgressionGroupID.Signus, 1215));
				AddProgressionGroup(new(ProgressionGroupID.PostPolterghastEasy, -10, ProgressionGroupID.Polterghast,
					itemNames: new SortedSet<string>() {
						"CalamityMod/EidolicWail",
						"CalamityMod/EidolonStaff",
						"CalamityMod/SoulEdge",
						"CalamityMod/ReaperTooth",
						"CalamityMod/Valediction",
						"CalamityMod/DeepSeaDumbbell",
						"CalamityMod/CalamarisLament",
						"CalamityMod/LionHeart"
					}));
				AddProgressionGroup(new(ProgressionGroupID.Polterghast, 1235));
				AddProgressionGroup(new(ProgressionGroupID.AcidRainT3, 15, ProgressionGroupID.Polterghast,
					npcNames: new SortedSet<string>() {
						"CalamityMod/NuclearTerror",
						"CalamityMod/Mauler"
					}));
				AddProgressionGroup(new(ProgressionGroupID.PostOldDukeEasy, -10, ProgressionGroupID.OldDuke,
					itemNames: new SortedSet<string>() {
						//"Bloodworm"
					}));
				AddProgressionGroup(new(ProgressionGroupID.OldDuke, 1275));
				AddProgressionGroup(new(ProgressionGroupID.DevouererOfGods, 1305));
				/*
				AddProgressionGroup(new(ProgressionGroupID.AscendentSpirit, 1500,
					itemNames: new SortedSet<string>() {
						"CalamityMod/AscendantSpiritEssence"
					}));
				*/
				AddProgressionGroup(new(ProgressionGroupID.PostYharonEasy, -10, ProgressionGroupID.Yharon,
					itemNames: new SortedSet<string>() {
						"CalamityMod/AuricOre"
					}));
				AddProgressionGroup(new(ProgressionGroupID.Yharon, 1360,
					itemNames: new SortedSet<string>() {
						"CalamityMod/Murasama"
					}));
				AddProgressionGroup(new(ProgressionGroupID.ExoMechs, 1390));
				AddProgressionGroup(new(ProgressionGroupID.SupremeCalamitas, 1450,
					itemNames: new SortedSet<string>() {
						"CalamityMod/GruesomeEminence",
						"CalamityMod/Rancor",
						"CalamityMod/CindersOfLament",
						"CalamityMod/Metastasis"
					},
					npcNames: new SortedSet<string>() {
						"CalamityMod/SupremeCalamitas"
					}));
				AddProgressionGroup(new(ProgressionGroupID.AdultEidolon, 1450,
					npcNames: new SortedSet<string>() {
						"CalamityMod/PrimordialWyrmHead"
					}));
				AddProgressionGroup(new(ProgressionGroupID.BossRush, 1450,
					itemNames: new SortedSet<string>() {
						"CalamityMod/Rock"
					}));
			}

			if (AndroMod.starsAboveEnabled) {
				/*Not used to make weapons
				progressionGroups[ProgressionGroupID.GraveYard].AddItems(
					new SortedSet<string>() {
						"StarsAbove/EssenceOfFarewells",
						"StarsAbove/EssenceOfOffseeing"
					});//65
				*/
				progressionGroups[ProgressionGroupID.PostKingSlimeEasy].AddItems(
					new SortedSet<string>() {
						"StarsAbove/EssenceOfStyle",
						"StarsAbove/EssenceOfTheAegis"
					});//75
				progressionGroups[ProgressionGroupID.PostEyeEasy].AddItems(
					new SortedSet<string>() {
						"StarsAbove/EssenceOfTheDarkMoon",
						"StarsAbove/EssenceOfTheGardener"
					});//110
				AddProgressionGroup(new(ProgressionGroupID.StellaglyphT2, -10, ProgressionGroupID.Eye,
					itemNames: new SortedSet<string>() {
						"StarsAbove/EssenceOfNature"
					}));//110
				progressionGroups[ProgressionGroupID.PostEaterBrainEasy].AddItems(
					new SortedSet<string>() {
						"StarsAbove/EssenceOfAsh",
						"StarsAbove/EssenceOfOuterGods",
						"StarsAbove/EssenceOfTheAnomaly",
						"StarsAbove/EssenceOfTheSoldier"
					});//190
				progressionGroups[ProgressionGroupID.Hell].AddItems(
					new SortedSet<string>() {
						"StarsAbove/EssenceOfDespair"
					});//190
				progressionGroups[ProgressionGroupID.PostBeeEasy].AddItems(
					new SortedSet<string>() {
						"StarsAbove/EssenceOfBitterfrost",
						"StarsAbove/EssenceOfFingers"
					});//240
				progressionGroups[ProgressionGroupID.PostSkeletronEasy].AddItems(
					new SortedSet<string>() {
						"StarsAbove/EssenceOfMisery",
						"StarsAbove/EssenceOfTheFreeshooter",
						"StarsAbove/EssenceOfTheOcean",
						"StarsAbove/EssenceOfTheSharpshooter",
						"StarsAbove/EssenceOfTheUnderworldGoddess",
						"StarsAbove/EssenceOfTheAutomaton",
						"StarsAbove/EssenceOfThePegasus",
						"StarsAbove/EssenceOfTheHallownest"
					});//290
				AddProgressionGroup(new(ProgressionGroupID.PostDeerEasy, -10, ProgressionGroupID.Deer,
					itemNames: new SortedSet<string>() {
						"StarsAbove/EssenceOfVampirism"
					}));//370
				progressionGroups[ProgressionGroupID.HardMode].AddItems(
					new SortedSet<string>() {
						"StarsAbove/EssenceOfBlasting",
						"StarsAbove/EssenceOfPerfection",
						"StarsAbove/EssenceOfSilverAsh",
						"StarsAbove/EssenceOfTheObservatory",
						"StarsAbove/EssenceOfTheVoid"
					});//400
				progressionGroups[ProgressionGroupID.Hallow].AddItems(
					new SortedSet<string>() {
						"StarsAbove/EssenceOfGold"
					});//420
				AddProgressionGroup(new(ProgressionGroupID.Vagrant, 500,
					itemNames: new SortedSet<string>() {
						"StarsAbove/EssenceOfIzanagi",
						"StarsAbove/EssenceOfTheHawkmoon",
						"StarsAbove/EssenceOfTheWatch",
						"StarsAbove/EssenceOfSpinning"
					}));
				progressionGroups[ProgressionGroupID.PostPiratesEasy].AddItems(
					new SortedSet<string>() {
						"StarsAbove/EssenceOfPiracy"
					});//535
				AddProgressionGroup(new(ProgressionGroupID.PostQueenSlimeEasy, -10, ProgressionGroupID.QueenSlime,
					itemNames: new SortedSet<string>() {
						"StarsAbove/EssenceOfAbsoluteChaos",
						"StarsAbove/EssenceOfTheHunt",
						"StarsAbove/EssenceOfChemtech",
						"StarsAbove/EssenceOfStaticShock"
					}));//565
				progressionGroups[ProgressionGroupID.PostMechanicalBoss].AddItems(
					new SortedSet<string>() {
						"StarsAbove/EssenceOfDeathsApprentice",
						"StarsAbove/EssenceOfTheAerialAce",
						"StarsAbove/EssenceOfTheBionis",
						"StarsAbove/EssenceOfTheBull",
						"StarsAbove/EssenceOfButterflies"
					});//595
				progressionGroups[ProgressionGroupID.PostAllMechanicalBosses].AddItems(
					new SortedSet<string>() {
						"StarsAbove/EssenceOfTheDragonslayer",
						"StarsAbove/EssenceOfTheGunlance",
						"StarsAbove/EssenceOfTheRenegade"
					});//640
				AddProgressionGroup(new(ProgressionGroupID.Nalhaun, 660,
					itemNames: new SortedSet<string>() {
						"StarsAbove/EssenceOfTheHollowheart",
						"StarsAbove/EssenceOfTheHunt",
						"StarsAbove/EssenceOfThePhantom",
						"StarsAbove/EssenceOfLightning"
					}));
				progressionGroups[ProgressionGroupID.PostPlanteraEasy].AddItems(
					new SortedSet<string>() {
						"StarsAbove/EssenceOfFoxfire",
						"StarsAbove/EssenceOfTheFallen",
						"StarsAbove/EssenceOfTheSwarm",
						"StarsAbove/EssenceOfDreams"
					});//715
				AddProgressionGroup(new(ProgressionGroupID.Penthesilea, 810,
					itemNames: new SortedSet<string>() {
						"StarsAbove/EssenceOfTime",
						"StarsAbove/EssenceOfEuthymia",
						"StarsAbove/EssenceOfInk"
					}));
				progressionGroups[ProgressionGroupID.PostFrostMoonEasy].AddItems(
					new SortedSet<string>() {
						"StarsAbove/EssenceOfDuality",
						"StarsAbove/EssenceOfIRyS"
					});//810
				AddProgressionGroup(new(ProgressionGroupID.PostPumpkinMoonEasy, -10, ProgressionGroupID.PumpkinMoon,
					itemNames: new SortedSet<string>() {
						"StarsAbove/EssenceOfLifethirsting"
					}));//810
				progressionGroups[ProgressionGroupID.PostGolemEasy].AddItems(
					new SortedSet<string>() {
						"StarsAbove/EssenceOfBlood",
						"StarsAbove/EssenceOfExplosions",
						"StarsAbove/EssenceOfSilence",
						"StarsAbove/EssenceOfTheHarbinger",
						"StarsAbove/EssenceOfTheMoonlitAdepti"
					});//835
				progressionGroups[ProgressionGroupID.PostMartianSaucerEasy].AddItems(
					new SortedSet<string>() {
						"StarsAbove/EssenceOfTheBehemothTyphoon"
					});//870
				AddProgressionGroup(new(ProgressionGroupID.PostEmpressEasy, -10, ProgressionGroupID.EmpressNight,
					itemNames: new SortedSet<string>() {
						"StarsAbove/EssenceOfSakura",
						"StarsAbove/EssenceOfTechnology"
					}));//900
				AddProgressionGroup(new(ProgressionGroupID.PostDukeFishronEasy, -10, ProgressionGroupID.DukeFishron,
					itemNames: new SortedSet<string>() {
						"StarsAbove/EssenceOfSin",
						"StarsAbove/EssenceOfAlpha",
						"StarsAbove/EssenceOfKinetics",
						"StarsAbove/EssenceOfMania"
					}));//930
				progressionGroups[ProgressionGroupID.PostLunaticCultistEasy].AddItems(
					new SortedSet<string>() {
						"StarsAbove/EssenceOfDrivingThunder",
						"StarsAbove/EssenceOfQuantum",
						"StarsAbove/EssenceOfTheAscendant",
						"StarsAbove/EssenceOfTheTimeless",
						"StarsAbove/EssenceOfTheUnyieldingEarth",
						"StarsAbove/EssenceOfTwinStars"
					});//965
				AddProgressionGroup(new(ProgressionGroupID.Arbitration, 1010,
					itemNames: new SortedSet<string>() {
						"StarsAbove/EssenceOfLiberation",
						"StarsAbove/EssenceOfRadiance",
						"StarsAbove/EssenceOfAzakana"
					}));
				progressionGroups[ProgressionGroupID.PostMoonLordEasy].AddItems(
					new SortedSet<string>() {
						"StarsAbove/EssenceOfAdagium",
						"StarsAbove/EssenceOfDestiny",
						"StarsAbove/EssenceOfEternity",
						"StarsAbove/EssenceOfLunarDominion",
						"StarsAbove/EssenceOfSouls",
						"StarsAbove/EssenceOfStarsong",
						"StarsAbove/EssenceOfTheChimera",
						"StarsAbove/EssenceOfTheCosmos"
					});//1050
				AddProgressionGroup(new(ProgressionGroupID.WarriorOfLight, 1130,
					itemNames: new SortedSet<string>() {
						"StarsAbove/EssenceOfSurpassingLimits",
						"StarsAbove/EssenceOfTheAbyss",
						"StarsAbove/EssenceOfTheBeginningAndEnd",
						"StarsAbove/EssenceOfTheOverwhelmingBlaze",
						"StarsAbove/EssenceOfTheTreasury",
						"StarsAbove/EssenceOfBalance",
						"StarsAbove/EssenceOfAuthority"
					}
					//,
					//npcNames: new SortedSet<string>() {
					//	"StarsAbove/WarriorOfLight"
					//}//Might be needed at some point.  Warror of light has 2 phases.  The final phase is used in the boss switch way above.
					));
				AddProgressionGroup(new(ProgressionGroupID.FirstStarfarer, 1160,
					itemNames: new SortedSet<string>() {
						"StarsAbove/EssenceOfLuminance",
						"StarsAbove/EssenceOfTheFuture",
						"StarsAbove/EssenceOfSurya"
					}));
				AddProgressionGroup(new(ProgressionGroupID.SpatialMemoriam, 1200,
					itemNames: new SortedSet<string>() {
						"StarsAbove/SpatialMemoriam"
					}));
			}

			if (AndroMod.thoriumEnabled) {
				progressionGroups[ProgressionGroupID.ForestPreHardModeNight].AddItems(
					new SortedSet<string>() {
						"ThoriumMod/ArcaneDust"
					});//10
				progressionGroups[ProgressionGroupID.Presents].AddItems(
					new SortedSet<string>() {
						"ThoriumMod/FamilyHeirloom"
					});//10
				progressionGroups[ProgressionGroupID.MerchantShop].AddItems(
					new SortedSet<string>() {
						"ThoriumMod/BaseballBat"
					});//10
				progressionGroups[ProgressionGroupID.NormalChest].AddItems(
					new SortedSet<string>() {
						"ThoriumMod/Flute",
						"ThoriumMod/RecoveryWand"
					});//20
				progressionGroups[ProgressionGroupID.Underground].AddNPCs(
					new SortedSet<string>() {
						"ThoriumMod/BabySpider"
					});//45
				progressionGroups[ProgressionGroupID.Underground].AddItems(
					new SortedSet<string>() {
						"ThoriumMod/SmoothCoal"
					});//45
				AddProgressionGroup(new(ProgressionGroupID.Opal, 53,
					itemNames: new SortedSet<string>() {
						"ThoriumMod/Opal"
					}));
				AddProgressionGroup(new(ProgressionGroupID.Aquamarine, 58,
					itemNames: new SortedSet<string>() {
						"ThoriumMod/Aquamarine"
					}));
				AddProgressionGroup(new(ProgressionGroupID.PostGrandThunderBirdEasy, -10, ProgressionGroupID.GrandThunderBird,
					itemNames: new SortedSet<string>() {
						"ThoriumMod/EighthPlagueStaff",
						"ThoriumMod/Scorpain",
						"ThoriumMod/TechniqueBlankScroll"
					}));//70
				AddProgressionGroup(new(ProgressionGroupID.LifeQuartz, 75,
					itemNames: new SortedSet<string>() {
						"ThoriumMod/LifeQuartz"
					}));
				progressionGroups[ProgressionGroupID.PostKingSlimeEasy].AddItems(
					new SortedSet<string>() {
						//Cook
					});//75
				AddProgressionGroup(new(ProgressionGroupID.GrandThunderBird, 80));
				progressionGroups[ProgressionGroupID.TownNPCDrops].AddNPCs(
					new SortedSet<string>() {
						"ThoriumMod/Cobbler"
					});//80
				progressionGroups[ProgressionGroupID.GoldChest].AddItems(
					new SortedSet<string>() {
						"ThoriumMod/EnchantedStaff",
						"ThoriumMod/EnchantedCane",
						"ThoriumMod/Webgun"
					});//80
				progressionGroups[ProgressionGroupID.PreHardmodeUncommonShops].AddItems(
					new SortedSet<string>() {
						"ThoriumMod/Trapper"
					});//80
				progressionGroups[ProgressionGroupID.Fishing].AddItems(
					new SortedSet<string>() {
						"ThoriumMod/RottenCod",
						"ThoriumMod/BrainCoral",
						"ThoriumMod/RivetingTadpole",
						"ThoriumMod/Heartstriker",
						"ThoriumMod/SpittingFish"
					});//95
				progressionGroups[ProgressionGroupID.UndergroundJungle].AddItems(
					new SortedSet<string>() {
						"ThoriumMod/Petal",
						"ThoriumMod/LivingLeaf",
						"ThoriumMod/TheDigester",
						"ThoriumMod/ForestOcarina"
					});//100
				AddProgressionGroup(new(ProgressionGroupID.ThoriumOre, 105,
					itemNames: new SortedSet<string>() {
						"ThoriumMod/ThoriumOre"
					}));
				progressionGroups[ProgressionGroupID.PostEyeEasy].AddNPCs(
					new SortedSet<string>() {
						"ThoriumMod/Blacksmith"
					});//110
				progressionGroups[ProgressionGroupID.PostEyeEasy].AddItems(
					new SortedSet<string>() {
						"ThoriumMod/WarForger",
						"ThoriumMod/ArcaneArmorFabricator",
						"ThoriumMod/SteelPickaxe",
						"ThoriumMod/SteelAxe",
						"ThoriumMod/SteelHammer",
						"ThoriumMod/SteelBlade",
						"ThoriumMod/SteelBattleAxe",
						"ThoriumMod/SteelBow",
						"ThoriumMod/Fork",
						"ThoriumMod/Spoon",
						"ThoriumMod/Knife",
						"ThoriumMod/SpudBomber"
					});//110
				AddProgressionGroup(new(ProgressionGroupID.ScarletChest, 110,
					itemNames: new SortedSet<string>() {
						"ThoriumMod/EnchantedPickaxe"
					}));
				AddProgressionGroup(new(ProgressionGroupID.EnchantedPickaxeShrine, 110,
					itemNames: new SortedSet<string>() {
						"ThoriumMod/DeepStaff"
					}));
				progressionGroups[ProgressionGroupID.BloodMoon].AddItems(
					new SortedSet<string>() {
						"ThoriumMod/UnholyShards"
					});//150
				progressionGroups[ProgressionGroupID.BloodMoon].AddNPCs(
					new SortedSet<string>() {
						"ThoriumMod/GraveLimb"
					});//150
				AddProgressionGroup(new(ProgressionGroupID.TrackerContractT1, 10, ProgressionGroupID.Eye,
					itemNames: new SortedSet<string>() {
						"ThoriumMod/TrackersSkinningBlade",
						"ThoriumMod/RosySlimeStaff",
						//"ThoriumMod/GrimPedestal",
						"ThoriumMod/Whip",
						"ThoriumMod/TotemCaller"
					}));//130
				AddProgressionGroup(new(ProgressionGroupID.PatchWerk, 10, ProgressionGroupID.BloodMoon,
					itemNames: new SortedSet<string>() {
						"ThoriumMod/LeechBolt",
						"ThoriumMod/BentZombieArm",
						"ThoriumMod/ViciousMockery"
					},
					npcNames: new SortedSet<string>() {
						"ThoriumMod/PatchWerk"
					}));//160
				progressionGroups[ProgressionGroupID.GoblinArmy].AddNPCs(
					new SortedSet<string>() {
						"ThoriumMod/GoblinTrapper"
					});//180
				AddProgressionGroup(new(ProgressionGroupID.ThoriumUnobtainableItems, 180,
					itemNames: new SortedSet<string>() {
						"ThoriumMod/ArtificersExtractor",
						"ThoriumMod/StonePurple",
						"ThoriumMod/TesterProjectile",
						"ThoriumMod/BasicPickaxe",
						"ThoriumMod/ArcaneSpike",
						"ThoriumMod/DyingRealityWhisper"
					}));
				AddProgressionGroup(new(ProgressionGroupID.AquaticDepths, 180,
					itemNames: new SortedSet<string>() {
						"ThoriumMod/Aquaite",
						"ThoriumMod/SteelDrum",
						"ThoriumMod/MagicConch"
					},
					npcNames: new SortedSet<string>() {
						"ThoriumMod/ManofWar",
						"ThoriumMod/Hammerhead",
						"ThoriumMod/GigaClam"
					}));
				AddProgressionGroup(new(ProgressionGroupID.PostQueenJellyfishEasy, -10, ProgressionGroupID.QueenJellyfish,
					itemNames: new SortedSet<string>() {

					}));//180
				progressionGroups[ProgressionGroupID.BloodMoonFishing].AddItems(
					new SortedSet<string>() {
						"ThoriumMod/VampirePickaxe"
					});//190
				AddProgressionGroup(new(ProgressionGroupID.QueenJellyfish, 190));
				progressionGroups[ProgressionGroupID.PostEaterBrainEasy].AddItems(
					new SortedSet<string>() {
						"ThoriumMod/JarOMayo",
						"ThoriumMod/LargePopcorn",
						"ThoriumMod/ChiLantern",
						"ThoriumMod/DivineLotus",
						"ThoriumMod/SamsaraLotus"
					});//190
				progressionGroups[ProgressionGroupID.PostEaterBrainEasy].AddNPCs(
					new SortedSet<string>() {
						"ThoriumMod/WindElemental"
					});//190
				AddProgressionGroup(new(ProgressionGroupID.DarkMageThorium, 0, ProgressionGroupID.OldOneArmyT1));//190
				AddProgressionGroup(new(ProgressionGroupID.Viscount, 210));
				progressionGroups[ProgressionGroupID.MeteoriteOre].AddNPCs(
					new SortedSet<string>() {
						"ThoriumMod/UFO"
					});//210
				progressionGroups[ProgressionGroupID.HellstoneOre].AddItems(
					new SortedSet<string>() {
						"ThoriumMod/MagicThorHammer",
						"ThoriumMod/RangedThorHammer"
					});//220
				progressionGroups[ProgressionGroupID.PostBeeEasy].AddItems(
					new SortedSet<string>() {
						"ThoriumMod/MantisCane"
					});//240
				AddProgressionGroup(new(ProgressionGroupID.CorpseBloom, -10, ProgressionGroupID.Skeletron,
					itemNames: new SortedSet<string>() {
						"ThoriumMod/LifeDisperser"
					},
					npcNames: new SortedSet<string>() {
						"ThoriumMod/CorpseBloom"
					}));//290
				progressionGroups[ProgressionGroupID.PostSkeletronEasy].AddItems(
					new SortedSet<string>() {
						"ThoriumMod/WhirlpoolSaber",
						"ThoriumMod/Eelrod",
						"ThoriumMod/MarineLauncher",
						"ThoriumMod/WackWrench",
						"ThoriumMod/YarnBall"
					});//290
				progressionGroups[ProgressionGroupID.PostSkeletronEasy].AddItems(
					new SortedSet<string>() {
						"ThoriumMod/Microphone",
						"ThoriumMod/Subwoofer"
					});//320
				progressionGroups[ProgressionGroupID.Dungeon].AddItems(
					new SortedSet<string>() {
						"ThoriumMod/StreamSting",
						"ThoriumMod/HighTide",
						"ThoriumMod/NaiadShiv",
						"ThoriumMod/BoneReaper",
						"ThoriumMod/StrongestLink"
					});//320
				progressionGroups[ProgressionGroupID.Dungeon].AddNPCs(
					new SortedSet<string>() {
						"ThoriumMod/RagingMinotaur"
					});//320
				progressionGroups[ProgressionGroupID.ShadowChest].AddItems(
					new SortedSet<string>() {
						"ThoriumMod/Nocturne",
						"ThoriumMod/LightsLament",
						"ThoriumMod/EternalNight"
					});//350
				AddProgressionGroup(new(ProgressionGroupID.PostBuriedChampionEasy, -10, ProgressionGroupID.BuriedChampion,
					itemNames: new SortedSet<string>() {
						"ThoriumMod/SentinelWand",
						"ThoriumMod/RedeemerStaff"
					}));//375
				AddProgressionGroup(new(ProgressionGroupID.GraniteEnergyStorm, 385));
				AddProgressionGroup(new(ProgressionGroupID.BuriedChampion, 385));
				AddProgressionGroup(new(ProgressionGroupID.StarScouter, 395));
				progressionGroups[ProgressionGroupID.HardMode].AddNPCs(
					new SortedSet<string>() {
						"ThoriumMod/VileFloater",
						"ThoriumMod/BlizzardBat",
						"ThoriumMod/FrostBurntFlayer",
						"ThoriumMod/ChilledSpitter",
						"ThoriumMod/Coldling",
						"ThoriumMod/MartianScout"
					});//400
				progressionGroups[ProgressionGroupID.HardmodeShopItems].AddItems(
					new SortedSet<string>() {
						"ThoriumMod/DurasteelDrill",
						"ThoriumMod/DurasteelChainsaw",
						"ThoriumMod/DurasteelJackhammer",
						"ThoriumMod/DurasteelBlade",
						"ThoriumMod/DurasteelJavelin",
						"ThoriumMod/DurasteelRepeater",
						"ThoriumMod/DurasteelThrowingSpear",
						"ThoriumMod/SteelThrowingAxe",
						"ThoriumMod/Kunai",
						"ThoriumMod/BenignBalloon",
						"ThoriumMod/SacrificialDagger",
						"ThoriumMod/GrimFlayer",
						"ThoriumMod/HealingRain",
						"ThoriumMod/Chum",
						"ThoriumMod/MagicCard",
						"ThoriumMod/HexWand",
						"ThoriumMod/StaffofMycelium",
						"ThoriumMod/StaffofOvergrowth",
						"ThoriumMod/FlanPlatter",
						"ThoriumMod/ScholarsHarp",
						"ThoriumMod/TranquilLyre",
						"ThoriumMod/HeavenlyCloudScepter"
					});//400
				progressionGroups[ProgressionGroupID.HardModeUnderground].AddItems(
					new SortedSet<string>() {
						"ThoriumMod/SoulofPlight",
						"ThoriumMod/PharaohsBreath"
					});//410
				progressionGroups[ProgressionGroupID.HardModeUnderground].AddNPCs(
					new SortedSet<string>() {
						"ThoriumMod/BoneFlayer",
						"ThoriumMod/MoltenMortar"
					});//410
				AddProgressionGroup(new(ProgressionGroupID.AquaticDepthsHardMode, 420,
					npcNames: new SortedSet<string>() {
						"ThoriumMod/VoltEelHead"
					}));
				progressionGroups[ProgressionGroupID.HardModeNight].AddNPCs(
					new SortedSet<string>() {
						"ThoriumMod/Lycan"
					});//430
				progressionGroups[ProgressionGroupID.HardModeBloodMoon].AddNPCs(
					new SortedSet<string>() {
						"ThoriumMod/BloodyWarg",
						"ThoriumMod/BloodMage"
					});//440
				progressionGroups[ProgressionGroupID.HardModeBloodMoon].AddItems(
					new SortedSet<string>() {
						"ThoriumMod/BloodDrinker",
						"ThoriumMod/RifleSpear",
						"ThoriumMod/BloodFeasterStaff"
					});//440
				progressionGroups[ProgressionGroupID.UndergroundHallow].AddNPCs(
					new SortedSet<string>() {
						"ThoriumMod/Spectrumite"
					});//440
				AddProgressionGroup(new(ProgressionGroupID.PostBoreanStriderEasy, -10, ProgressionGroupID.BoreanStrider,
					npcNames: new SortedSet<string>() {
						"ThoriumMod/Tarantula"
					}));//435
				progressionGroups[ProgressionGroupID.UndergroundEvil].AddItems(
					new SortedSet<string>() {
						"ThoriumMod/CursedHammer"
					});//440
				AddProgressionGroup(new(ProgressionGroupID.BoreanStrider, 445));
				progressionGroups[ProgressionGroupID.FrostLegeon].AddNPCs(
					new SortedSet<string>() {
						"ThoriumMod/SnowSinga"
					});//450
				progressionGroups[ProgressionGroupID.HardModeRare].AddItems(
					new SortedSet<string>() {
						"ThoriumMod/IcyGaze"
					});//460
				progressionGroups[ProgressionGroupID.HardModeRare].AddNPCs(
					new SortedSet<string>() {
						"ThoriumMod/LihzardMimic",
						"ThoriumMod/UnderworldPot1"
					});//460
				progressionGroups[ProgressionGroupID.GoblinArmyHardMode].AddItems(
					new SortedSet<string>() {
						"ThoriumMod/ShadowPurgeCaltrop"
					});//490
				progressionGroups[ProgressionGroupID.BigMimics].AddNPCs(
					new SortedSet<string>() {
						"ThoriumMod/SubmergedMimic",
						"ThoriumMod/MyceliumMimic",
						"ThoriumMod/HellBringerMimic"
					});//500
				progressionGroups[ProgressionGroupID.PostPiratesEasy].AddItems(
					new SortedSet<string>() {
						"ThoriumMod/MidasGavel"
					});//535
				progressionGroups[ProgressionGroupID.Pirates].AddNPCs(
					new SortedSet<string>() {
						"ThoriumMod/SeaShantySinger"
					});//545
				AddProgressionGroup(new(ProgressionGroupID.PostFallenBeholderEasy, -10, ProgressionGroupID.FallenBeholder,
					itemNames: new SortedSet<string>() {
						"ThoriumMod/LodeStoneChunk",
						"ThoriumMod/ValadiumChunk",
						"ThoriumMod/VoidHeart",
						"ThoriumMod/Recuperate",
						"ThoriumMod/AngelStaff",
						"ThoriumMod/DevilStaff",
						"ThoriumMod/TheSeaMine",
						"ThoriumMod/KineticKnife",
						"ThoriumMod/ArmorBane",
						"ThoriumMod/BulletStorm",
						"ThoriumMod/Scalper",
						"ThoriumMod/Executioner",
						"ThoriumMod/Rapier",
						"ThoriumMod/Kazoo",
						"ThoriumMod/Thunderstruck",
						"ThoriumMod/LilCherubsWand",
						"ThoriumMod/LilDevilsWand"
					}));//550
				AddProgressionGroup(new(ProgressionGroupID.FlyingDutchmanThorium, 10, ProgressionGroupID.Pirates));//555
				progressionGroups[ProgressionGroupID.Eclipse].AddNPCs(
					new SortedSet<string>() {
						"ThoriumMod/LeFantome"
					});//560
				AddProgressionGroup(new(ProgressionGroupID.FallenBeholder, 560));
				AddProgressionGroup(new(ProgressionGroupID.OgreThorium, 0, ProgressionGroupID.OldOneArmyT2));//595
				progressionGroups[ProgressionGroupID.PostMechanicalBoss].AddItems(
					new SortedSet<string>() {
						"ThoriumMod/SteamBattery",
						"ThoriumMod/BaritoneSaxophone",
						"ThoriumMod/OnemanQuartet",
						"ThoriumMod/LifePulseStaff",
						"ThoriumMod/AeonStaff",
						"ThoriumMod/Teslanator",
						"ThoriumMod/SteamgunnerController",
						"ThoriumMod/SteamFlail"
					});//595
				progressionGroups[ProgressionGroupID.PostAllMechanicalBosses].AddNPCs(
					new SortedSet<string>() {
						"ThoriumMod/ScissorStalker"
					});//640
				AddProgressionGroup(new(ProgressionGroupID.Lich, 660));
				AddProgressionGroup(new(ProgressionGroupID.AquaticDepthsPostPlantera, -10, ProgressionGroupID.Plantera,
					npcNames: new SortedSet<string>() {
						"ThoriumMod/AquaticHallucination"
					}));//715
				progressionGroups[ProgressionGroupID.PostPlanteraEasy].AddItems(
					new SortedSet<string>() {
						"ThoriumMod/SolarPebble",
						"ThoriumMod/CorrodlingStaff",
						"ThoriumMod/IllumiteChunk",
						"ThoriumMod/BudBomb",
						"ThoriumMod/DreamMegaphone",
						"ThoriumMod/TheBopper",
						"ThoriumMod/LethalInjection",
						"ThoriumMod/SupersonicBomber",
						"ThoriumMod/PhaseLauncher",
						"ThoriumMod/PLG",
						"ThoriumMod/MicroLauncher"
					});//715
				progressionGroups[ProgressionGroupID.DungeonPostPlanteraRare].AddItems(
					new SortedSet<string>() {
						"ThoriumMod/TheBlackBlade",
						"ThoriumMod/TheBlackBow",
						"ThoriumMod/TheBlackStaff",
						"ThoriumMod/TheBlackCane",
						"ThoriumMod/TheBlackDagger",
						"ThoriumMod/TheBlackScythe",
						"ThoriumMod/TheBlackOtamatone"
					});//775
				progressionGroups[ProgressionGroupID.BiomeChests].AddItems(
					new SortedSet<string>() {
						"ThoriumMod/Fishbone",
						"ThoriumMod/PharaohsSlab",
						"ThoriumMod/PhoenixStaff"
					});//800
				progressionGroups[ProgressionGroupID.EclipsePostPlantera].AddItems(
					new SortedSet<string>() {
						"ThoriumMod/GodKiller"
					});//800
				progressionGroups[ProgressionGroupID.PostGolemEasy].AddItems(
					new SortedSet<string>() {
						"ThoriumMod/WyvernSlayer"
					});//830
				AddProgressionGroup(new(ProgressionGroupID.MartianSaucerThorium, 0, ProgressionGroupID.MartianSaucer));//880
				AddProgressionGroup(new(ProgressionGroupID.PostForgottenOneEasy, -10, ProgressionGroupID.ForgottenOne,
					itemNames: new SortedSet<string>() {
						"ThoriumMod/TheWhirlpool",
						"ThoriumMod/HydroPump"
					}));//890
				AddProgressionGroup(new(ProgressionGroupID.ForgottenOne, 900));
				AddProgressionGroup(new(ProgressionGroupID.LunaticCultistThorium, 0, ProgressionGroupID.LunaticCultist));//975
				AddProgressionGroup(new(ProgressionGroupID.Primordials, 1200,
					npcNames: new SortedSet<string>() {
						"ThoriumMod/Aquaius"
					}, lootItemNames: new SortedSet<string>() {
						"ThoriumMod/ThePrimordialsTreasureBag"
					}));
				AddProgressionGroup(new(ProgressionGroupID.DestinyWeaverUnobtainable, 1450,
					itemNames: new SortedSet<string>() {
						"ThoriumMod/DestinyWeaver"
					}));
			}

			if (AndroMod.magicStorageEnabled) {
				progressionGroups[ProgressionGroupID.Evil].AddItems(
					new SortedSet<string>() {
						"MagicStorage/DemonAltar"
					});//80
			}

			if (AndroMod.fargosEnabled) {
				progressionGroups[ProgressionGroupID.MerchantShop].AddItems(
					new SortedSet<string>() {
						"Fargowiltas/LumberJaxe"
					});//10
			}

			if (AndroMod.fargosSoulsEnabled) {
				progressionGroups[ProgressionGroupID.PostMartianSaucerEasy].AddItems(
					new SortedSet<string>() {
						"Fargowiltas/MartianMemoryStick"
					});//870
				AddProgressionGroup(new(ProgressionGroupID.Energizers, 1000,
					itemTypes:
						ContentSamples.ItemsByType.Where(p => p.Value.type > ItemID.Count && p.Value.ModItem?.Mod.Name == "Fargowiltas" && p.Value.Name.Contains("Energizer")).Select(p => p.Value.type)
					));
				AddProgressionGroup(new(ProgressionGroupID.TrojanSquirrel, 70));
				AddProgressionGroup(new(ProgressionGroupID.FargosUnobtainableItems, 200,
					itemNames: new SortedSet<string>() {
						"FargowiltasSouls/SpiritLongbow",
						"FargowiltasSouls/PrismaRegalia"
					}));
				AddProgressionGroup(new(ProgressionGroupID.DeviBoss, 395));
				AddProgressionGroup(new(ProgressionGroupID.BanishedBaron, 610));
				AddProgressionGroup(new(ProgressionGroupID.LieFlight, 650));
				AddProgressionGroup(new(ProgressionGroupID.CosmosChampion, 1115));
				AddProgressionGroup(new(ProgressionGroupID.AbomBoss, 1235,
					itemNames: new SortedSet<string>() {
						"FargowiltasSouls/StaffOfUnleashedOcean",
						"FargowiltasSouls/BrokenHilt"
					}));
				AddProgressionGroup(new(ProgressionGroupID.MutantBoss, 1450,
					itemNames: new SortedSet<string>() {
						"FargowiltasSouls/BrokenBlade",
						"FargowiltasSouls/PhantasmalEnergy",
						"FargowiltasSouls/EternalEnergy"
					}));
			}
			
			if (WEMod.avaliRaceEnabled) {
				progressionGroups[ProgressionGroupID.ForestPreHardMode].AddItems(
					new SortedSet<string>() {
						"AvaliRace/CentralPrinterAvali",
						"AvaliRace/PilotPistolAvali"
					});//0
			}

			if (WEMod.dbtEnabled) {
				progressionGroups[ProgressionGroupID.ForestPreHardMode].AddItems(
					new SortedSet<string>() {
						"DBZMODPORT/StableKiCrystal"
					});//0
				progressionGroups[ProgressionGroupID.MerchantShop].AddItems(
					new SortedSet<string>() {
						"DBZMODPORT/ScrapMetal"
					});//10
				progressionGroups[ProgressionGroupID.PostEaterBrainEasy].AddItems(
					new SortedSet<string>() {
						"DBZMODPORT/CalmKiCrystal",
						"DBZMODPORT/EarthenShard"
					});//190
				progressionGroups[ProgressionGroupID.PostBeeEasy].AddItems(
					new SortedSet<string>() {
						"DBZMODPORT/AstralEssentia"
					});//240
				progressionGroups[ProgressionGroupID.PostSkeletronEasy].AddItems(
					new SortedSet<string>() {
						"DBZMODPORT/BlightedFang",
						"DBZMODPORT/SkeletalEssence",
						"DBZMODPORT/PridefulKiCrystal"
					});//290
				progressionGroups[ProgressionGroupID.HardModeUnderground].AddItems(
					new SortedSet<string>() {
						"DBZMODPORT/AngerKiCrystal"
					});//410
				progressionGroups[ProgressionGroupID.PostAllMechanicalBosses].AddItems(
					new SortedSet<string>() {
						"DBZMODPORT/SoulofEntity"
					});//640
				progressionGroups[ProgressionGroupID.PostPlanteraEasy].AddItems(
					new SortedSet<string>() {
						"DBZMODPORT/PureKiCrystal"
					});//715
				progressionGroups[ProgressionGroupID.PostGolemEasy].AddItems(
					new SortedSet<string>() {
						"DBZMODPORT/DemonicSoul"
					});//835
				progressionGroups[ProgressionGroupID.PostLunaticCultistEasy].AddItems(
					new SortedSet<string>() {
						"DBZMODPORT/CandyLaser"
					});//965
				progressionGroups[ProgressionGroupID.LunarInvasion].AddItems(
					new SortedSet<string>() {
						"DBZMODPORT/RadiantFragment"
					});//1005
			}

			if (WEMod.bountifulGoodieBagsEnabled) {
				progressionGroups[ProgressionGroupID.HolidayWeapons].AddItems(
					new SortedSet<string>() {
						"BountifulGoodieBags/CandyCornCarver",
						"BountifulGoodieBags/CandyCornDrill",
						"BountifulGoodieBags/CobwebChakram",
						"BountifulGoodieBags/DextroseKunai",
						"BountifulGoodieBags/SWModel15"
					});//110
			}

			if (WEMod.amuletOfManyMinionsEnabled) {
				progressionGroups[ProgressionGroupID.NormalChest].AddItems(
					new SortedSet<string>() {
						"AmuletOfManyMinions/TumbleSheepMinionItem"
					});//20
				progressionGroups[ProgressionGroupID.Ocean].AddItems(
					new SortedSet<string>() {
						"AmuletOfManyMinions/FishBowlMinionItem"
					});//50
				progressionGroups[ProgressionGroupID.GoldChest].AddItems(
					new SortedSet<string>() {
						"AmuletOfManyMinions/RatsMinionItem"
					});//80
				progressionGroups[ProgressionGroupID.PreHardmodeUncommonShops].AddItems(
					new SortedSet<string>() {
						"AmuletOfManyMinions/BalloonBuddyMinionItem"
					});//80
				progressionGroups[ProgressionGroupID.Sky].AddItems(
					new SortedSet<string>() {
						"AmuletOfManyMinions/SkywareSquireMinionItem"
					});//85
				progressionGroups[ProgressionGroupID.UndergroundJungle].AddItems(
					new SortedSet<string>() {
						"AmuletOfManyMinions/BalloonMonkeyMinionItem"
					});//100
				progressionGroups[ProgressionGroupID.PostEyeEasy].AddItems(
					new SortedSet<string>() {
						"AmuletOfManyMinions/MysticPaintbrushMinionItem",
						"AmuletOfManyMinions/GuideSquireMinionItem"
					});//110
				progressionGroups[ProgressionGroupID.Dungeon].AddItems(
					new SortedSet<string>() {
						"AmuletOfManyMinions/ExciteSkullMinionItem"
					});//320
				progressionGroups[ProgressionGroupID.ShadowChest].AddItems(
					new SortedSet<string>() {
						"AmuletOfManyMinions/DemonSquireMinionItem"
					});//350
				progressionGroups[ProgressionGroupID.HardModeBloodMoon].AddItems(
					new SortedSet<string>() {
						"AmuletOfManyMinions/SlimepireMinionItem"
					});//440
				progressionGroups[ProgressionGroupID.PostMartianSaucerEasy].AddItems(
					new SortedSet<string>() {
						"AmuletOfManyMinions/GoblinTechnomancerMinionItem"
					});//870
			}

			if (WEMod.redCloudEnabled) {
				progressionGroups[ProgressionGroupID.ForestPreHardModeRare].AddNPCs(
					new SortedSet<string>() {
						"tsorcRevamp/Dunlending",
						"tsorcRevamp/UndeadCaster",
						"tsorcRevamp/FirebombHollow",
						"tsorcRevamp/LothricSpearKnight",
						"tsorcRevamp/MindflayerServant",
						"tsorcRevamp/AttraidiesManifestation",
						"tsorcRevamp/AbandonedStump"
					});//40
				AddProgressionGroup(new(ProgressionGroupID.Leon, 130));
				progressionGroups[ProgressionGroupID.Hell].AddNPCs(
					new SortedSet<string>() {
						"tsorcRevamp/FireLurker"
					});//190
				AddProgressionGroup(new(ProgressionGroupID.Gaibon, 215));
				AddProgressionGroup(new(ProgressionGroupID.Slogra, 0, ProgressionGroupID.Gaibon));//215
				progressionGroups[ProgressionGroupID.Dungeon].AddNPCs(
					new SortedSet<string>() {
						"tsorcRevamp/GhostoftheForgottenKnight",
						"tsorcRevamp/BlackKnight"
					});//320
				AddProgressionGroup(new(ProgressionGroupID.JungleWyvern, 330));
				progressionGroups[ProgressionGroupID.HardMode].AddNPCs(
					new SortedSet<string>() {
						"tsorcRevamp/DarkElfMage",
						"tsorcRevamp/QuaraHydromancer",
						"tsorcRevamp/Assassin",
						"tsorcRevamp/Archdeacon",
						"tsorcRevamp/CrazedDemonSpirit"
					});//400
				AddProgressionGroup(new(ProgressionGroupID.TheRage, 445));
				AddProgressionGroup(new(ProgressionGroupID.TheSorrow, 585));
				AddProgressionGroup(new(ProgressionGroupID.WyvernMage, 595));
				AddProgressionGroup(new(ProgressionGroupID.TheHunter, 600));
				AddProgressionGroup(new(ProgressionGroupID.SerrisX, 610));
				AddProgressionGroup(new(ProgressionGroupID.Death, 620));
				AddProgressionGroup(new(ProgressionGroupID.Attraidies, 1160));
				AddProgressionGroup(new(ProgressionGroupID.PostAttraidies, 20, ProgressionGroupID.Attraidies,
					npcNames: new SortedSet<string>() {
						"tsorcRevamp/CrystalKnight",
						"tsorcRevamp/RedKnight",
						"tsorcRevamp/KnightOfGwyn",
						"tsorcRevamp/Tetsujin",
						"tsorcRevamp/BarrowWightNemesis",
						"tsorcRevamp/AncientDemonOfTheAbyss",
						"tsorcRevamp/GhostOfTheDarkmoonKnight",
						"tsorcRevamp/LothricBlackKnight",
						"tsorcRevamp/CorruptedElemental"
					}));//1180
				AddProgressionGroup(new(ProgressionGroupID.HellkiteDragon, 1240));
				AddProgressionGroup(new(ProgressionGroupID.WaterFiendKraken, 1330));
				AddProgressionGroup(new(ProgressionGroupID.EarthFiendLich, 1375));
				AddProgressionGroup(new(ProgressionGroupID.Witchking, 1455));
				AddProgressionGroup(new(ProgressionGroupID.Artorias, 1460));
				AddProgressionGroup(new(ProgressionGroupID.SeathTheScaleless, 1465));
				AddProgressionGroup(new(ProgressionGroupID.AbysmalOolacileSorcerer, 1470));
				AddProgressionGroup(new(ProgressionGroupID.FireFiendMarilith, 1475));
				AddProgressionGroup(new(ProgressionGroupID.DarkCloud, 1480));
				AddProgressionGroup(new(ProgressionGroupID.Blight, 1490));
				AddProgressionGroup(new(ProgressionGroupID.Gwyn, 1500));
			}

			/*if (WEMod.aequusEnabled) {
				progressionGroups[ProgressionGroupID.ForestPreHardModeNight].AddNPCs(
					new SortedSet<string>() {
						"Aequus/Starite"
					});//20
				progressionGroups[ProgressionGroupID.UndergroundSnow].AddItems(
					new SortedSet<string>() {
						"Aequus/CrystalDagger"
					});//65
				progressionGroups[ProgressionGroupID.TownNPCDrops].AddNPCs(
					new SortedSet<string>() {
						"Aequus/Carpenter"
					});//80
				progressionGroups[ProgressionGroupID.Sky].AddNPCs(
					new SortedSet<string>() {
						"Aequus/Meteor"
					});//85
				AddProgressionGroup(new(ProgressionGroupID.RockMan, 90,
					itemNames: new SortedSet<string>() {
						"Aequus/RockMan"
					}));
				progressionGroups[ProgressionGroupID.Fishing].AddItems(
					new SortedSet<string>() {
						"Aequus/PearlShardWhite"
					});//95
				AddProgressionGroup(new(ProgressionGroupID.AequusUndergroundOcean, 110,
					itemNames: new SortedSet<string>() {
						"Aequus/LiquidGun"
					},
					lootItemNames: new SortedSet<string>() {
						"Aequus/EnvelopeUndergroundOcean"
					}));
				AddProgressionGroup(new(ProgressionGroupID.AequusUnobtainable, 110,
					itemNames: new SortedSet<string>() {
						"Aequus/EnthrallingScepter",
						"Aequus/SilkHammer",
						"Aequus/SilkPickaxe",
						"Aequus/Narrizuul",
						"Aequus/ThrashBag"
					},
					lootItemNames: new SortedSet<string>() {
						"Aequus/EnvelopeGlimmer"
					}));
				AddProgressionGroup(new(ProgressionGroupID.CrabsonEasyAfter, -10, ProgressionGroupID.Crabson,
					itemNames: new SortedSet<string>() {
						"Aequus/Mallet",
						"Aequus/RecyclingMachine"
					}));//120
				AddProgressionGroup(new(ProgressionGroupID.Crabson, 130,
					itemNames: new SortedSet<string>() {
						"Aequus/Crabax"
					}));
				progressionGroups[ProgressionGroupID.BloodMoon].AddNPCs(
					new SortedSet<string>() {
						"Aequus/BloodMimic"
					});//150
				progressionGroups[ProgressionGroupID.BloodMoon].AddItems(
					new SortedSet<string>() {
						"Aequus/Meathook",
						"Aequus/Wabbajack"
					});//150
				progressionGroups[ProgressionGroupID.Dungeon].AddItems(
					new SortedSet<string>() {
						"Aequus/Valari",
						"Aequus/DungeonCandle",
						"Aequus/Revenant"
					});//320
				AddProgressionGroup(new(ProgressionGroupID.DemonSiegeEasyAfter, -10, ProgressionGroupID.DemonSiege,
					itemNames: new SortedSet<string>() {
						"Aequus/OccultistCandle"
					}));//370
				AddProgressionGroup(new(ProgressionGroupID.DemonSiege, 380,
					npcNames: new SortedSet<string>() {
						"Aequus/Cindera",
						"Aequus/Magmabubble",
						"Aequus/TrapperImp"
					}));
				AddProgressionGroup(new(ProgressionGroupID.OmegaStariteEasyAfter, -10, ProgressionGroupID.OmegaStarite,
					itemNames: new SortedSet<string>() {
						"Aequus/PhysicistSentry"
					}));//385
				AddProgressionGroup(new(ProgressionGroupID.OmegaStarite, 395));
				progressionGroups[ProgressionGroupID.HardMode].AddItems(
					new SortedSet<string>() {
						"Aequus/Umystick",
						"Aequus/PixieCandle"
					});//400
				AddProgressionGroup(new(ProgressionGroupID.GaleStreams, 570,
					npcNames: new SortedSet<string>() {
						"Aequus/StreamingBalloon",
						"Aequus/Vraine"
					}));
				AddProgressionGroup(new(ProgressionGroupID.RedSprite, 580,
					npcNames: new SortedSet<string>() {
						"Aequus/RedSprite"
					}));
				AddProgressionGroup(new(ProgressionGroupID.SpaceSquid, 580,
					npcNames: new SortedSet<string>() {
						"Aequus/SpaceSquid"
					}));
				AddProgressionGroup(new(ProgressionGroupID.DustDevil, 590));
				progressionGroups[ProgressionGroupID.DungeonPostPlantera].AddNPCs(
					new SortedSet<string>() {
						"Aequus/Heckto"
					});//750
				AddProgressionGroup(new(ProgressionGroupID.AequusWorkInProgress, 0,
					itemNames: new SortedSet<string>() {
						"Aequus/ZombieSceptre",
						"Aequus/CrimsonSceptre",
						"Aequus/BattleAxe",
						"Aequus/RecordBreaker",
						"Aequus/BloodMoonCandle",
						"Aequus/CorruptionCandle",
						"Aequus/CrimsonCandle",
						"Aequus/LihzahrdKusariyari",
						"Aequus/Dynaknife",
						"Aequus/SickBeat",
						"Aequus/Osiris",
						"Aequus/Insurgency",
						"Aequus/DragonsBreath"
					}));
			}*/

			if (WEMod.clickerClassEnabled) {
				progressionGroups[ProgressionGroupID.ForestPreHardModeNight].AddItems(
					new SortedSet<string>() {
						"ClickerClass/DreamClicker"
					});//20
				progressionGroups[ProgressionGroupID.Underground].AddItems(
					new SortedSet<string>() {
						"ClickerClass/HeartyClicker"
					});//45
				progressionGroups[ProgressionGroupID.PreHardmodeUncommonShops].AddItems(
					new SortedSet<string>() {
						"ClickerClass/ConfettiClicker",
						"ClickerClass/CandleClicker",
						"ClickerClass/CritterClicker"
					});//80
				progressionGroups[ProgressionGroupID.GoldChest].AddItems(
					new SortedSet<string>() {
						"ClickerClass/FaultyClicker"
					});//80
				progressionGroups[ProgressionGroupID.UndergroundDesert].AddItems(
					new SortedSet<string>() {
						"ClickerClass/PharaohsClicker"
					});//90
				progressionGroups[ProgressionGroupID.PostSkeletronEasy].AddItems(
					new SortedSet<string>() {
						"ClickerClass/MagnetClicker"
					});//290
				progressionGroups[ProgressionGroupID.PostMoonLordEasy].AddItems(
					new SortedSet<string>() {
						"ClickerClass/QuintessenceClicker"
					});//1050
			}

			if (WEMod.secretsOfTheShadowsEnabled) {
				progressionGroups[ProgressionGroupID.ForestPreHardModeRare].AddNPCs(
					new SortedSet<string>() {
						"SOTS/BasicTreasureSlime"
					});//40
				progressionGroups[ProgressionGroupID.Underground].AddNPCs(
					new SortedSet<string>() {
						"SOTS/GoldenTreasureSlime"
					});//45
				progressionGroups[ProgressionGroupID.Mushroom].AddNPCs(
					new SortedSet<string>() {
						"SOTS/SittingMushroom"
					});//75
				progressionGroups[ProgressionGroupID.UndergroundDesert].AddNPCs(
					new SortedSet<string>() {
						"SOTS/PyramidTreasureSlime"
					});//90
				progressionGroups[ProgressionGroupID.Sky].AddNPCs(
					new SortedSet<string>() {
						"SOTS/TwilightScouter"
					});//95
				progressionGroups[ProgressionGroupID.Fishing].AddLootItems(
					new SortedSet<string>() {
						"SOTS/PyramidCrate"
					});//95
				AddProgressionGroup(new(ProgressionGroupID.OtherworldyConstruct, 100,
					npcNames: new SortedSet<string>() {
						"SOTS/OtherworldlyConstructHead",
						"SOTS/OtherworldlySpirit"
					}));
				AddProgressionGroup(new(ProgressionGroupID.NatureConstruct, 105,
					npcNames: new SortedSet<string>() {
						"SOTS/NatureConstruct",
						"SOTS/NatureSpirit"
					}));
				AddProgressionGroup(new(ProgressionGroupID.EarthenConstruct, 110,
					npcNames: new SortedSet<string>() {
						"SOTS/EarthenConstruct",
						"SOTS/EarthenSpirit"
					}));
				AddProgressionGroup(new(ProgressionGroupID.PermafrostConstruct, 115,
					npcNames: new SortedSet<string>() {
						"SOTS/PermafrostConstruct",
						"SOTS/PermafrostSpirit"
					}));
				AddProgressionGroup(new(ProgressionGroupID.TidalConstruct, 120,
					npcNames: new SortedSet<string>() {
						"SOTS/TidalConstruct",
						"SOTS/TidalSpirit"
					}));
				AddProgressionGroup(new(ProgressionGroupID.Glowmoth, 125));
				progressionGroups[ProgressionGroupID.Hell].AddNPCs(
					new SortedSet<string>() {
						"SOTS/LesserWisp"
					});//190
				AddProgressionGroup(new(ProgressionGroupID.PutridPinkyPhase2, 260));
				AddProgressionGroup(new(ProgressionGroupID.PharaohsCurse, 275));
				progressionGroups[ProgressionGroupID.PostSkeletronEasy].AddNPCs(
					new SortedSet<string>() {
						"SOTS/CorruptionTreasureSlime"
					});//290
				AddProgressionGroup(new(ProgressionGroupID.TheAdvisorHead, 395));
				AddProgressionGroup(new(ProgressionGroupID.EvilConstruct, 560,
					npcNames: new SortedSet<string>() {
						"SOTS/EvilConstruct",
						"SOTS/EvilSpirit"
					}));
				AddProgressionGroup(new(ProgressionGroupID.ChaosConstruct, 580,
					npcNames: new SortedSet<string>() {
						"SOTS/ChaosConstruct",
						"SOTS/ChaosSpirit"
					}));
				AddProgressionGroup(new(ProgressionGroupID.InfernoConstruct, 620,
					npcNames: new SortedSet<string>() {
						"SOTS/InfernoConstruct",
						"SOTS/InfernoSpirit"
					}));
				AddProgressionGroup(new(ProgressionGroupID.Polaris, 610));
				AddProgressionGroup(new(ProgressionGroupID.Lux, 905));
				AddProgressionGroup(new(ProgressionGroupID.SubspaceSerpentHead, 1090));
			}
		}
		private static void PopulateItemInfusionPowers() {
			IEnumerable<ProgressionGroup> progressionGroupsEnum = ProgressionGroups.Values.OrderBy(g => g.InfusionPower);
			foreach (ProgressionGroup progressionGroup in progressionGroupsEnum) {
				int infusionPower = progressionGroup.InfusionPower;
				foreach (int itemType in progressionGroup.ItemTypes) {
					if (!ItemInfusionPowers.ContainsKey(itemType)) {
						ItemInfusionPowers.Add(itemType, infusionPower);
					}
					else if (Debugger.IsAttached) $"ItemInfusionPowers already contains item: {itemType.CSI().S()}, {progressionGroup.ID}".LogSimple();
				}
			}

			foreach (ProgressionGroup progressionGroup in progressionGroupsEnum) {
				int infusionPower = progressionGroup.InfusionPower;
				foreach (int netID in progressionGroup.NpcTypes) {
					if (NPCsThatDropWeaponsOrIngredients.ContainsKey(netID)) {
						SortedSet<int> itemTypes = NPCsThatDropWeaponsOrIngredients[netID];
						bool added = false;
						NPC npc = netID.CSNPC();
						foreach (int itemType in itemTypes) {
							//Item item = itemType.CSI();
							if (!ItemInfusionPowers.ContainsKey(itemType)) {
								ItemInfusionPowers.Add(itemType, infusionPower);
								added = true;
							}
							//else if (Debugger.IsAttached) $"ItemInfusionPowers already contains {itemType.CSI().S()}. Skipped drop from {npc.S()}".LogSimple();
						}

						if (Debugger.IsAttached && !added && netID > NPCID.Count && !progressionGroup.BossNetIDs.Contains(netID)) $"{GameMessageTextID.NPCInProgressionGroupNoUniqueEnchantments.ToString().Lang_WE(L_ID1.GameMessages)}  {netID.CSNPC().S()}, {progressionGroup.ID}".LogSimple();
					}
					else if (Debugger.IsAttached && netID > NPCID.Count) $"{GameMessageTextID.NPCInProgressionGroupNotInDropList.ToString().Lang_WE(L_ID1.GameMessages)}  {netID.CSNPC().S()}, {progressionGroup.ID}".LogSimple();
				}
			}

			foreach (ProgressionGroup progressionGroup in progressionGroupsEnum) {
				int infusionPower = progressionGroup.InfusionPower;
				SortedSet<int> lootItemDrops = progressionGroup.GetLootItemDrops();
				foreach (int itemType in lootItemDrops) {
					if (!ItemInfusionPowers.ContainsKey(itemType)) {
						ItemInfusionPowers.Add(itemType, infusionPower);
					}
					//else if (Debugger.IsAttached) $"ItemInfusionPowers already contains item from boss bag: {itemType.CSI().S()}, {progressionGroup.ID}".LogSimple();
				}
			}

			//if (Debugger.IsAttached) $"{ItemInfusionPowers.OrderBy(p => p.Value).StringList((p) => $"{p.Key.CSI().S()}: {p.Value}", "ItemInfusionPowers")}".LogSimple();
		}
		private static void GuessMinedOreInfusionPowers() {
			foreach (int tileType in OreBag.ModOreTileTypes) {
				int itemType = GenericGlobalTile.GetDroppedItem(tileType, ignoreError: true);
				if (itemType <= 0)
					continue;

				if (ItemInfusionPowers.ContainsKey(itemType))
					continue;

				if (!WeaponCraftingIngredients.Contains(itemType))
					continue;

				Item item = itemType.CSI();
				int requiredPickaxePower = GenericGlobalTile.GetRequiredPickaxePower(tileType, true);
				float value = item.value;
				int infusionPower = GuessOreInfusionPower(requiredPickaxePower, value);
				ItemInfusionPowers.Add(itemType, infusionPower);
				$"{GameMessageTextID.OreInfusionPowerNotSetup.ToString().Lang_WE(L_ID1.GameMessages, new object[] { item.S() })} {infusionPower}".LogNT(ChatMessagesIDs.OreInfusionPowerNotSetup);
			}

			//if (Debugger.IsAttached) $"\nOreInfusionPowers\n{OreInfusionPowers.Select(i => $"{i.Key.CSI().S()}: {i.Value}").JoinList("\n")}".LogSimple();
		}

		#region Supporting Functions
		private static int recursionCounter = 0;
		private static SortedSet<int> createItemTypesAlreadyBeingProcessed = new();
		private static bool TryGetAllCraftingIngredientTypes(int createItemType, out HashSet<HashSet<int>> ingredients) {
			//if (Debugger.IsAttached) $"\\/TryGetAllCraftingIngredientTypes({createItemType.CSI().S()})".LogSimple();
			bool first = recursionCounter == 0;
			recursionCounter++;
			if (recursionCounter > 110) {
				ingredients = null;
				return false;
			}

			HashSet<HashSet<int>> resultIngredients = new();
			if (!createItemTypesAlreadyBeingProcessed.Contains(createItemType) && (finishedRecipeSetup || !allExpandedRecepies.ContainsKey(createItemType))) {
				createItemTypesAlreadyBeingProcessed.Add(createItemType);
				//IEnumerable<Recipe> recipies = Main.recipe.Where((r, index) => r.createItem.type == createItemType);//TODO: troubleshoot, Goes infinite with Calamity.  
				IEnumerable<int> recipeNumbers = Main.recipe.Select((r, index) => index).Where(index => Main.recipe[index].createItem.type == createItemType && (Main.recipe[index].createItem.type > ItemID.Count || index <= AndroMod.VanillaRecipeCount));
				HashSet<HashSet<HashSet<int>>> requiredItemTypeLists = new();
				foreach (int recipeNum in recipeNumbers) {
					Recipe recipe = Main.recipe[recipeNum];
					if (reverseCraftableRecipes.Contains(recipeNum))
						continue;

					HashSet<HashSet<int>> requiredItemTypes = new();
					foreach (Item ingredientItem in recipe.requiredItem) {
						int ingredientType = ingredientItem.type;
						if (recursionCounter > 100)
							$"|ingredient {ingredientType.CSI().S()}| {recipe.requiredItem.StringList(i => i.S(), $"{recipeNum} {recipe.createItem.S()}:")}, {recipe.requiredTile.Select(tile => GenericGlobalTile.GetDroppedItem(tile)).Where(type => type > 0).StringList(i => i.CSI().S(), "tiles")}".LogSimple();

						if (TryGetAllCraftingIngredientTypes(ingredientType, out HashSet<HashSet<int>> ingredientTypes)) {
							requiredItemTypes.CombineHashSet(ingredientTypes);
						}
						else {
							requiredItemTypes.TryAdd(new() { ingredientType });
						}
					}

					if (requiredItemTypes.Count <= 0)
						continue;

					foreach (Item requiredTileItem in recipe.requiredTile.Select(tile => GenericGlobalTile.GetDroppedItem(tile)).Where(type => type > 0).Select(type => type.CSI())) {
						int requiredTileItemType = requiredTileItem.type;
						if (recursionCounter > 100)
							$"|tile {requiredTileItemType.CSI().S()}| {recipe.requiredItem.StringList(i => i.S(), $"{recipeNum} {recipe.createItem.S()}:")}, {recipe.requiredTile.Select(tile => GenericGlobalTile.GetDroppedItem(tile)).Where(type => type > 0).StringList(i => i.CSI().S(), "tiles")}".LogSimple();

						if (TryGetAllCraftingIngredientTypes(requiredTileItemType, out HashSet<HashSet<int>> tileIngredientTypes)) {
							requiredItemTypes.CombineHashSet(tileIngredientTypes);
						}
						else {
							requiredItemTypes.TryAdd(new() { requiredTileItemType });
						}
					}

					requiredItemTypeLists.Add(requiredItemTypes);
				}

				createItemTypesAlreadyBeingProcessed.Remove(createItemType);
				resultIngredients = resultIngredients.CombineIngredientLists(requiredItemTypeLists);
				if (!finishedRecipeSetup)
					allExpandedRecepies.Add(createItemType, resultIngredients);
			}

			if (finishedRecipeSetup) {
				ingredients = resultIngredients;
			}
			else {
				ingredients = allExpandedRecepies.TryGetValue(createItemType, out HashSet<HashSet<int>> value) ? value : new();
			}

			//if (Debugger.IsAttached) $"/\\{createItemType.CSI().S()}: {ingredients.Select(set => set.Select(t => t.CSI().S()).JoinList(" or ")).JoinList(", ")}".LogSimple();

			if (first) {
				recursionCounter = 0;
			}
			else {
				recursionCounter--;
			}
			
			return ingredients.Count > 0;
		}
		public static HashSet<HashSet<int>> CombineIngredientLists(this HashSet<HashSet<int>> ingredientsArg, HashSet<HashSet<HashSet<int>>> requiredItemTypeLists) {
			HashSet<HashSet<int>> ingredients = ingredientsArg;
			foreach (HashSet<HashSet<int>> requiredItemTypes in requiredItemTypeLists) {
				if (ingredients.Count <= 0) {
					ingredients = requiredItemTypes;
					continue;
				}

				HashSet<HashSet<int>> failedMatches = new();
				HashSet<int> failedMatch = new();
				foreach (HashSet<int> requiredItemType in requiredItemTypes) {
					bool contains = ingredients.ContainsHashSet(requiredItemType);

					if (contains) {
						continue;
					}
					else {
						failedMatches.Add(requiredItemType);
						failedMatch = requiredItemType;
					}
				}

				if (failedMatches.Count == 1 && requiredItemTypes.Count == ingredients.Count) {
					foreach (HashSet<int> ingredientType in ingredients) {
						if (!requiredItemTypes.Contains(ingredientType)) {
							ingredients.Remove(ingredientType);
							ingredients.Add(ingredientType.Concat(failedMatch).ToHashSet());
							break;
						}
					}
				}
				else {
					ingredients.CombineHashSet(failedMatches);
				}
			}

			return ingredients;
		}
		private static void AddProgressionGroup(ProgressionGroup progressionGroup) => progressionGroups.Add(progressionGroup.ID, progressionGroup);
		public static int GuessOreInfusionPower(int requiredPickaxePower, float value) {
			if (value < 0)
				value = 0;

			int i = 1;
			int count = InfusionPowerTiles.Count;
			KeyValuePair<int, (int pickPower, float value)> lastTileData = new();
			foreach (KeyValuePair<int, (int pickPower, float value)> tileData in InfusionPowerTiles) {
				int pickPower = tileData.Value.pickPower;
				if (pickPower > 0 && pickPower >= requiredPickaxePower) {
					//linear interpolate from last and current with pickPower only
					int lastPickPower = lastTileData.Value.pickPower;
					int lastInfusionPower = lastTileData.Key;
					int currentInfusionPower = tileData.Key;
					float percent = (requiredPickaxePower - lastPickPower) / (float)(pickPower - lastPickPower);
					int infusionPower = lastInfusionPower + (int)Math.Round(percent * (currentInfusionPower - lastInfusionPower));
					return infusionPower;
				}
				else if (requiredPickaxePower <= 0) {
					float currentValue = tileData.Value.value;
					if (currentValue > value || pickPower > 0) {
						int currentInfusionPower = tileData.Key;
						if (i == 1) {
							//Determine below copper using value only
							int infusionPower = (int)Math.Round(currentInfusionPower * (value / currentValue));
							return infusionPower;
						}
						else {
							//linear interpolate from last and current with value only
							float lastValue = lastTileData.Value.value;
							int lastInfusionPower = lastTileData.Key;
							float percent = (value - lastValue) / (currentValue - lastValue);
							int infusionPower = lastInfusionPower + (int)Math.Round(percent * (currentInfusionPower - lastInfusionPower));
							return infusionPower;
						}
					}
				}
				else if (i == count) {
					//Determine above using pickPower with tileData
					//Linear project from last to current to new
					int lastPickPower = lastTileData.Value.pickPower;
					int lastInfusionPower = lastTileData.Key;
					int currentInfusionPower = tileData.Key;
					float infusionPowerPerPickPower = (currentInfusionPower - lastInfusionPower) / (float)(pickPower - lastPickPower);
					int infusionPower = currentInfusionPower + (int)Math.Round(infusionPowerPerPickPower * (requiredPickaxePower - pickPower));
					return infusionPower;
				}

				lastTileData = tileData;
				i++;
			}

			return -1;
		}
		private static void SetupInfusionPowerTiles() {
			infusionPowerTiles = new();
			for (int tileType = 0; tileType < TileID.Count; tileType++) {
				int itemType = GenericGlobalTile.GetDroppedItem(tileType, ignoreError: true);
				if (itemType <= 0)
					continue;

				if (OreInfusionPowers.ContainsKey(itemType)) {
					int infusionPower = OreInfusionPowers[itemType];
					if (infusionPowerTiles.ContainsKey(infusionPower))
						continue;

					int requiredPickaxePower = GenericGlobalTile.GetRequiredPickaxePower(tileType, true);
					Item item = itemType.CSI();
					infusionPowerTiles.Add(infusionPower, (requiredPickaxePower, item.value));
				}
			}

			//if (Debugger.IsAttached) $"\ninfusionPowerTiles:\n{infusionPowerTiles.Select(i => $"infusionPower: {i.Key}, pickPower: {i.Value.pickPower}, value: {i.Value.value}").JoinList("\n")}".LogSimple();
		}
		public static void ResetAndSetupProgressionGroups() {
			progressionGroups.Clear();
			progressionGroups = null;
			ItemInfusionPowers.Clear();
			PopulateItemInfusionPowers();
		}

		#endregion

		#endregion

		private static void PopulateInfusionPowerSources() {
			int mechBossHighestSoul = Math.Max(Math.Max(
				ProgressionGroups[ProgressionGroupID.SkeletronPrime].InfusionPower, 
				ProgressionGroups[ProgressionGroupID.Destroyer].InfusionPower), 
				ProgressionGroups[ProgressionGroupID.Twins].InfusionPower
			) + 20;
			SortedDictionary<int, SortedSet<int>> OverridenInfusionPowerList = new() {
				{ mechBossHighestSoul, new SortedSet<int>() { ItemID.SoulofFright, ItemID.SoulofMight, ItemID.SoulofSight } }
			};

			//if (Debugger.IsAttached) ItemInfusionPowers.Select(p => p.Key.CSI().S()).JoinList(", ").LogSimple();
			foreach (KeyValuePair<int, int> pair in ItemInfusionPowers) {
				int itemType = pair.Key;
				Item item = itemType.CSI();
				if (item.IsWeaponItem())
					continue;

				if (TryGetAllCraftingIngredientTypes(itemType, out HashSet<HashSet<int>> ingredients)) {
					SortedSet<int> ingredientTypes = new(ingredients.Select(i => i.First()));
					//string temp = ingredientTypes.Select(t => t.CSI().S()).JoinList(", ");
					if (ingredientTypes.Count > 1) {
						int overridenItem = ItemInfusionPowers[itemType];
						if (!OverridenInfusionPowerList.ContainsKey(overridenItem))
							OverridenInfusionPowerList.Add(overridenItem, ingredientTypes);
					}
				}
			}

			foreach (int weaponType in WeaponsList) {
				if (allWeaponRecipies.ContainsKey(weaponType)) {
					InfusionPowerSource highestInfusionPowerSource = new();
					int infusionPower = -1;
					HashSet<HashSet<int>> ingredientTypeLists = allWeaponRecipies[weaponType];

					foreach (HashSet<int> ingredientTypes in ingredientTypeLists) {
						foreach (int ingredientType in ingredientTypes) {
							bool found = false;
							//string temp = ingredientTypeLists.StringList(l => l.StringList(i => $"{i}"));
							ItemSource itemSource = new(weaponType, ItemSourceType.Craft, ingredientType, ingredientTypeLists.Count);
							InfusionPowerSource infusionPowerSource = new(itemSource);
							if (ItemInfusionPowers.ContainsKey(ingredientType))
								found = true;

							if (found) {
								int newInfusionPower = infusionPowerSource.InfusionPower;
								if (newInfusionPower > infusionPower) {
									infusionPower = newInfusionPower;
									highestInfusionPowerSource = infusionPowerSource;
								}
							}
						}
					}

					if (infusionPower >= 0)
						WeaponInfusionPowers.Add(weaponType, highestInfusionPowerSource);
				}
				else if (ItemInfusionPowers.ContainsKey(weaponType)) {
					ItemSource itemSource = new(weaponType, ItemSourceType.NPCDrop, weaponType);
					InfusionPowerSource infusionPowerSource = new(itemSource);
					WeaponInfusionPowers.Add(weaponType, infusionPowerSource);
				}
			}

			SortedDictionary<int, InfusionPowerSource> weaponInfusionPowersWithRecipies = new(WeaponInfusionPowers);
			foreach (KeyValuePair<int, InfusionPowerSource> pair in weaponInfusionPowersWithRecipies.Where(p => allWeaponRecipies.ContainsKey(p.Key))) {
				int weaponType = pair.Key;
				HashSet<HashSet<int>> ingredientTypeLists = allWeaponRecipies[weaponType];
				SortedSet<int> firstTypeList = new(ingredientTypeLists.Select(i => i.First()));
				int infusionPower = -1;
				foreach (KeyValuePair<int, SortedSet<int>> overridenList in OverridenInfusionPowerList) {
					string ingredientTypesString = firstTypeList.Count > 0 ? firstTypeList.Select(t => t.CSI().S()).JoinList(", ") : "";
					string overridenListString = overridenList.Value.Count > 0 ? overridenList.Value.Select(t => t.CSI().S()).JoinList(", ") : "";
					if (overridenList.Value.IsSubsetOf(firstTypeList)) {
						infusionPower = overridenList.Key;
						if (infusionPower > pair.Value.InfusionPower)
							WeaponInfusionPowers[pair.Key] = new(WeaponInfusionPowers[pair.Key].itemSource, infusionPower);
						break;
					}
				}
			}

			//Shimmer recipes
			foreach (KeyValuePair<int, HashSet<int>> p in shimmerRecipes) {
				int weaponType = p.Key;
				if (!WeaponInfusionPowers.ContainsKey(weaponType)) {
					//Item temp = weaponType.CSI();
					InfusionPowerSource lowestSource = new();
					int lowestInfusionPower = -1;
					foreach (int ingredient in p.Value) {
						if (WeaponInfusionPowers.TryGetValue(ingredient, out InfusionPowerSource ingredientSource)) {
							int ingredientInfusionPower = ingredientSource.InfusionPower;
							if (lowestInfusionPower < 0 || ingredientInfusionPower < lowestInfusionPower) {
								lowestSource = ingredientSource;
								lowestInfusionPower = ingredientInfusionPower;
							}
						}
					}

					if (lowestInfusionPower >= 0) {
						if (!ItemInfusionPowers.ContainsKey(lowestSource.ResultItemID)) {
							ItemInfusionPowers.Add(lowestSource.ResultItemID, lowestSource.InfusionPower);
						}

						ItemSource itemSource = new(weaponType, ItemSourceType.Craft, lowestSource.ResultItemID);
						InfusionPowerSource infusionPowerSource = new(itemSource);
						WeaponInfusionPowers.Add(weaponType, infusionPowerSource);
						//$"{lowestSource.ResultItemID.CSI().ModFullName()} ->Shimmer-> {weaponType.CSI().ModFullName()} ({lowestSource.InfusionPower})".LogSimple();
					}
				}
			}

			//if (Debugger.IsAttached) $"\nItemInfusionPowers:\n{ItemInfusionPowers.OrderBy(p => p.Value).Select(p => $"{p.Key.CSI().S()}: {p.Value}").S()}".LogSimple();
		}
		private static void PrintMissingInfusionPowers() {
			if (Debugger.IsAttached) {
				IEnumerable<KeyValuePair<int, SortedSet<int>>> weaponsFromNPCs = WeaponsFromNPCs.Where(w => !ItemInfusionPowers.ContainsKey(w.Key) && !WeaponInfusionPowers.ContainsKey(w.Key));
				if (weaponsFromNPCs.Any())
					$"{weaponsFromNPCs.OrderBy(w => w.Key.CSI().GetWeaponInfusionPower()).Select(w => $"{w.Key.CSI().S()}: {w.Value.Select(n => n.CSNPC().S()).JoinList(", ")}").S(GameMessageTextID.ItemsFromNPCsNotIncluded.ToString().Lang_WE(L_ID1.GameMessages))}".LogNT(ChatMessagesIDs.AlwaysShowItemInfusionPowersNotSetup);

				IEnumerable<KeyValuePair<int, SortedSet<int>>> ingredientsFromNPCs = IngredientsFromNPCs.Where(w => !ItemInfusionPowers.ContainsKey(w.Key) && !WeaponInfusionPowers.ContainsKey(w.Key));
				if (ingredientsFromNPCs.Any())
					$"{ingredientsFromNPCs.OrderBy(w => w.Key.CSI().GetWeaponInfusionPower()).Select(w => $"{w.Key.CSI().S()}: {w.Value.Select(n => n.CSNPC().S()).JoinList(", ")}").S(GameMessageTextID.ItemsFromNPCIngredientsNotIncluded.ToString().Lang_WE(L_ID1.GameMessages))}".LogNT(ChatMessagesIDs.AlwaysShowItemInfusionPowersNotSetup);

				IEnumerable<KeyValuePair<int, SortedSet<int>>> weaponsFromLootItems = WeaponsFromLootItems.Where(w => !ItemInfusionPowers.ContainsKey(w.Key) && !WeaponInfusionPowers.ContainsKey(w.Key));
				if (weaponsFromLootItems.Any())
					$"{weaponsFromLootItems.OrderBy(w => w.Key.CSI().GetWeaponInfusionPower()).Select(w => $"{w.Key.CSI().S()}: {w.Value.Select(n => n.CSI().S()).JoinList(", ")}").S(GameMessageTextID.ItemsFromLootItemsNotIncluded.ToString().Lang_WE(L_ID1.GameMessages))}".LogNT(ChatMessagesIDs.AlwaysShowItemInfusionPowersNotSetup);

				IEnumerable<KeyValuePair<int, SortedSet<int>>> ingredientsFromLootItems = IngredientsFromLootItems.Where(w => !ItemInfusionPowers.ContainsKey(w.Key) && !WeaponInfusionPowers.ContainsKey(w.Key));
				if (ingredientsFromLootItems.Any())
					$"{ingredientsFromLootItems.OrderBy(w => w.Key.CSI().GetWeaponInfusionPower()).Select(w => $"{w.Key.CSI().S()}: {w.Value.Select(n => n.CSI().S()).JoinList(", ")}").S(GameMessageTextID.IngredientsFromLootItemsNotincluded.ToString().Lang_WE(L_ID1.GameMessages))}".LogNT(ChatMessagesIDs.AlwaysShowItemInfusionPowersNotSetup);
			}

			IEnumerable<int> weaponsNotSetup = WeaponsList.Where(t => !WeaponInfusionPowers.ContainsKey(t));
			if (weaponsNotSetup.Any())
				$"{weaponsNotSetup.OrderBy(t => t.CSI().GetWeaponInfusionPower()).Select(t => $"{t.CSI().S()}").S(GameMessageTextID.WeaponInfusionPowersNotSetup.ToString().Lang_WE(L_ID1.GameMessages))}".LogNT(ChatMessagesIDs.AlwaysShowItemInfusionPowersNotSetup);

			IEnumerable<int> ingredientsNotSetup = WeaponCraftingIngredients.Where(t => !ItemInfusionPowers.ContainsKey(t) && !WeaponInfusionPowers.ContainsKey(t));
			if (ingredientsNotSetup.Any())
				$"{ingredientsNotSetup.OrderBy(t => t.CSI().GetWeaponInfusionPower()).Select(t => $"{t.CSI().S()}").S(GameMessageTextID.IngredientInfusionPowersNotSetup.ToString().Lang_WE(L_ID1.GameMessages))}".LogNT(ChatMessagesIDs.AlwaysShowItemInfusionPowersNotSetup);

			IEnumerable<int> shimmerIngredientsNotSetup = shimmerRecipes.Where(p => !ItemInfusionPowers.ContainsKey(p.Key) && !WeaponInfusionPowers.ContainsKey(p.Key)).Select(p => p.Value).SelectMany(t => t);
			if (shimmerIngredientsNotSetup.Any())
				$"{shimmerIngredientsNotSetup.OrderBy(t => t.CSI().GetWeaponInfusionPower()).Select(t => $"{t.CSI().S()}").S(GameMessageTextID.ShimmerIngredientInfusionPowersNotSetup.ToString().Lang_WE(L_ID1.GameMessages))}".LogNT(ChatMessagesIDs.AlwaysShowItemInfusionPowersNotSetup);
		}
		private static void UpdateAndPrintString() {
			if (!Debugger.IsAttached)
				return;

			string stringToConvert = "";
			bool any = stringToConvert != "";
			bool npcs = any && true;
			bool items = any && false;
			if (npcs) {
				for (int netID = NPCID.Count; netID < NPCLoader.NPCCount; netID++) {
					NPC npc = netID.CSNPC();
					string fullName = npc.FullName();
					if (fullName == "" || fullName == " ")
						continue;

					string newName = npc.ModFullName();
					stringToConvert = stringToConvert.Replace(fullName, newName);
				}
			}

			if (items) {
				for (int itemType = ItemID.Count + 1; itemType < ItemLoader.ItemCount; itemType++) {
					Item item = itemType.CSI();
					string fullName = item.Name;
					if (fullName == "" || fullName == " ")
						continue;

					string newName = item.ModFullName();
					stringToConvert = stringToConvert.Replace(fullName, newName);
				}
			}

			if (npcs || items)
				$"stringToConvert:\n{stringToConvert}".LogSimple();
		}
		private static void ClearSetupData() {
			progressionGroups.Clear();
			progressionGroups = null;
			WeaponsList.Clear();
			WeaponCraftingIngredients.Clear();
			allWeaponRecipies.Clear();
			allExpandedRecepies.Clear();
			shimmerRecipes.Clear();
			reverseCraftableRecipes.Clear();
			LootItemTypes.Clear();
			ProgressionGroup.ClearSetupData();
			infusionPowerTiles = null;
			oreInfusionPowers = null;
			NPCsThatDropWeaponsOrIngredients.Clear();
			WeaponsFromNPCs.Clear();
			IngredientsFromNPCs.Clear();
			WeaponsFromLootItems.Clear();
			IngredientsFromLootItems.Clear();
			finishedSetup = true;
		}
		public static bool TryGetBaseInfusionPower(Item item, out int baseInfusionPower) {
			int weaponType = item.type;
			if (WeaponInfusionPowers.ContainsKey(weaponType)) {
			//if (weaponType < ItemID.Count && WeaponInfusionPowers.ContainsKey(weaponType)) {
				baseInfusionPower = WeaponInfusionPowers[weaponType].InfusionPower;

				return true;
			}

			baseInfusionPower = 0;
			return false;
		}
	}
	public static class InfusionProgressionTests {
		private static bool shouldRunTests = true;
		public static void RunTests() {
			if (!Debugger.IsAttached || !shouldRunTests)
				return;

			//Test_GetOreInfusionPower();
		}
		static List<(int pickPower, float value, int expectedResult)> exampleOreTiles = new() {
			{ (-100, -1000000f * 5f, 0) },
			{ (-100, 0f, 0) },
			{ (0, -1000000f * 5f, 0) },
			{ (0, 0f, 0) },
			{ (25, 1000000f * 5f, OreInfusionPowers[ItemID.GoldOre] + (int)Math.Round(25f/55f * (OreInfusionPowers[ItemID.DemoniteOre] - OreInfusionPowers[ItemID.GoldOre]))) },
			{ (60, -1000000f * 5f, OreInfusionPowers[ItemID.Meteorite]) },
			{ (300, -1000000f * 5f, OreInfusionPowers[ItemID.ChlorophyteOre] + (int)Math.Round(100f/50f * (OreInfusionPowers[ItemID.ChlorophyteOre] - OreInfusionPowers[ItemID.AdamantiteOre]))) },
			{ (0, 25f * 5f, (int)Math.Round(0.5f * OreInfusionPowers[ItemID.CopperOre])) },
			{ (0, 75f * 5f, OreInfusionPowers[ItemID.CopperOre] +(int)Math.Round(0.5f * (OreInfusionPowers[ItemID.IronOre] - OreInfusionPowers[ItemID.CopperOre]))) },
			{ (0, 100f * 5f, OreInfusionPowers[ItemID.IronOre]) },
			{ (0, 110f * 5f, OreInfusionPowers[ItemID.IronOre] +(int)Math.Round(0.2f * (OreInfusionPowers[ItemID.SilverOre] - OreInfusionPowers[ItemID.IronOre]))) },
			{ (0, 285f * 5f, OreInfusionPowers[ItemID.SilverOre] +(int)Math.Round(0.9f * (OreInfusionPowers[ItemID.GoldOre] - OreInfusionPowers[ItemID.SilverOre]))) },
			{ (0, 300f * 5f, OreInfusionPowers[ItemID.GoldOre]) },
			{ (0, 10000000f * 5f, OreInfusionPowers[ItemID.GoldOre]) },
			{ (200, 1000000f * 5f, OreInfusionPowers[ItemID.ChlorophyteOre]) }
		};
		private static void Test_GetOreInfusionPower() {
			int i = 1;
			foreach ((int pickPower, float value, int expectedResult) pair in exampleOreTiles) {
				int result = GuessOreInfusionPower(pair.pickPower, pair.value);
				int expectedResult = pair.expectedResult;
				if (result == expectedResult) {
					$"Test_GetOreInfusionPower {i} Successful, pickPower: {pair.pickPower}, value: {pair.value}, expectedInfusionPower: {expectedResult}".LogSimple();
				}
				else {
					$"Test_GetOreInfusionPower {i} Failed, pickPower: {pair.pickPower}, value: {pair.value}, expectedInfusionPower: {expectedResult}, infusionPower: {result}".LogSimple();
				}

				i++;
			}
		}
	}
}
