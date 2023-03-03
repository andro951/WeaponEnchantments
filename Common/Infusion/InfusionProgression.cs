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
using IL.Terraria.DataStructures;
using Mono.Cecil;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;
using Terraria.GameContent.ItemDropRules;
using static Humanizer.On;

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
		Forest = 0,
		Desert = 10,
		GiantTree = 20,
		Beach = 30,
		Snow = 35,
		Ocean = 50,
		//UndergroundSnow = 65,
		//DeepOcean = 70,
		Jungle = 75,
		Evil = 80,
		KingSlime = 85,
		//UndergroundDesert = 90,
		//UndergroundJungle = 100,
		Granite = 105,
		Marble = 110,
		Eye = 120,
		BloodMoon = 150,
		GoblinArmy = 180,
		EaterBrain = 200,
		//OldOne army T1 post EaterBrain
		Bee = 250,
		Skeletron = 300,
		Dungeon = 320,
		ShadowChest = 350,
		Deer = 380,
		Wall = 420,
		FrostLegeon = 450,
		Pirates = 545,
		Eclipse = 560,
		QueenSlime = 575,
		Destroyer = 605,
		//OldOne army T2 post Destroyer
		SkeletronPrime = 615,
		Twins = 630,
		Plantera = 725,
		PumpkinMoon = 820,
		FrostMoon = 825,
		Betsey = 840,
		Golem = 845,
		//OldOne army T3 Easy post Golem - 10
		MartianInvasion = 860,
		MartianSaucer = 880,
		DukeFishron = 940,
		//EmpressNight = 910,
		Empress = 970,//Night = -60
		LunaticCultist = 975,
		LunarInvasion = 1005,
		MoonLord = 1100,//Easy obtain -50
	}
	public struct ItemSource {
		public ItemSource(int resultItem, ItemSourceType itemSourceType, int ingredientItem) {
			ResultItemID = resultItem;
			ItemSourceType = itemSourceType;
			SourceID = ingredientItem;
			//ingredientItemStack = ingredientItem.stack;
		}
		/*
		public static bool operator ==(ItemSource left, ItemSource right) {
			return 
				left.ItemSourceType == right.ItemSourceType && 
				left.SourceID == right.SourceID && 
				left.ingredientItemStack == right.ingredientItemStack &&
				left.LockedByBoss == right.LockedByBoss;
		}
		public static bool operator !=(ItemSource left, ItemSource right) {
			return !(left == right);
		}
		*/

		public ItemSourceType ItemSourceType;
		public int ResultItemID;
		public int SourceID;
		//private int ingredientItemStack;
		public string ModdedSourceName => Modded ? SourceID.CSI().Name : null;
		public bool Modded => SourceID >= ItemID.Count;
		public bool LockedByBoss = false;
		public bool TryGetIngredientItem(out Item ingredientItem) {
			ingredientItem = null;
			bool tryGetIngredientItem = ItemSourceType == ItemSourceType.Craft;// && ingredientItemStack > 0;
			if (tryGetIngredientItem)
				ingredientItem = SourceID.CSI();// new(SourceID, ingredientItemStack);

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
		public InfusionPowerSource(ItemSource itemSource) {
			/*
			switch (itemSource.ItemSourceType) {
				case ItemSourceType.Craft:
					if (itemSource.TryGetIngredientItem(out Item sourceItem)) {
						
					}

					break;
			}
			*/
			this.itemSource = itemSource;
			infusionOffset = GetInfusionPowerOffset(itemSource);
		}

		public int InfusionPower {
			get {
				switch (itemSource.ItemSourceType) {
					case ItemSourceType.Craft:
						int infusionPower = baseCraftingInfusionPower + infusionPowerOffset;
						return infusionPower > 0 ? infusionPower : 0;
					default:
						return -1;
				}
			}
		}


		private InfusionOffset infusionOffset;
		private ItemSource itemSource;
		public string ModdedSourceName => itemSource.ModdedSourceName;
		public int SourceID => itemSource.SourceID;
		public int ResultItemID => itemSource.ResultItemID;
		public Item ResultItem => itemSource.ResultItem;
		private int baseCraftingInfusionPower => itemSource.Modded ? ModdedCraftingItemSourceInfusionPowers[ModdedSourceName] : VanillaCraftingItemSourceInfusionPowers[SourceID];
		private int infusionPowerOffset {
			get {
				switch (infusionOffset) {
					case InfusionOffset.Consumable:
						return -10;
					case InfusionOffset.EasyObtain:
						return -5;
					case InfusionOffset.CraftFromWeapon:
						return 5;
					default:
						return 0;
				}
			}
		}

		private static InfusionOffset GetInfusionPowerOffset(ItemSource itemSource) {
			switch (itemSource.ItemSourceType) {
				case ItemSourceType.Craft:
					if (itemSource.TryGetIngredientItem(out Item sourceItem)) {
						if (itemSource.ResultItem.consumable) {
							return InfusionOffset.Consumable;
						}
						else if (itemSource.LockedByBoss) {
							return InfusionOffset.EasyObtain;
						}
						else if (IsWeaponItem(sourceItem)) {
							return InfusionOffset.CraftFromWeapon;
						}
						else {
							return InfusionOffset.None;
						}
					}

					break;
			}

			return InfusionOffset.Error;
		}

		public override string ToString() {
			return $"{itemSource}, InfusionPower: {InfusionPower}";
		}
	}
	public class ProgressionGroup {
		public ProgressionGroup(ProgressionGroupID id, int InfusionPower, ProgressionGroup parent = null, IEnumerable<int> itemTypes = null, 
				IEnumerable<int> npcTypes = null, IEnumerable<ChestID> chests = null, IEnumerable<CrateID> crates = null) {
			Parent = parent;
			ID = id;
			infusionPower = InfusionPower;
			bossNetIDs = GetBossType(id);
			if (itemTypes != null)
				AddItems(itemTypes);

			if (npcTypes != null)
				AddNPCs(npcTypes);

			if (chests != null)
				Add(chests);

			if (crates != null)
				Add(crates);
		}

		public ProgressionGroup Parent { get; private set; }
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
		SortedSet<int> bossNetIDs;
		public SortedSet<int> ItemTypes { get; private set; } = new();
		public SortedSet<string> ItemNames { get; private set; } = new();
		public SortedSet<int> NpcTypes { get; private set; } = new();
		public SortedSet<string> NpcNames { get; private set; } = new();
		public SortedSet<ChestID> Chests { get; private set; } = new();
		public SortedSet<CrateID> Crates { get; private set; } = new();
		public void AddItems(IEnumerable<int> newItems) => ItemTypes.UnionWith(newItems);
		public void AddItems(IEnumerable<string> newItems) => ItemNames.UnionWith(newItems);
		public void AddNPCs(IEnumerable<int> newNPCs) => NpcTypes.UnionWith(newNPCs);
		public void AddNPCs(IEnumerable<string> newNPCs) => NpcNames.UnionWith(newNPCs);
		public void Add(IEnumerable<ChestID> newChests) => Chests.UnionWith(newChests);
		public void Add(IEnumerable<CrateID> newCrates) => Crates.UnionWith(newCrates);
		public void Add(IEnumerable<Item> newItems) => AddItems(newItems.Select(i => i.type));
		public void Add(IEnumerable<NPC> newNPCs) => AddNPCs(newNPCs.Select(npc => npc.netID));
		public static SortedSet<int> GetBossType(ProgressionGroupID id) {
			int npcid = NPCID.NegativeIDCount;
			switch (id) {
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
					npcid = NPCID.Skeleton;
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
				case ProgressionGroupID.MartianSaucer:
					npcid = NPCID.MartianSaucer;
					break;
				case ProgressionGroupID.DukeFishron:
					npcid = NPCID.DukeFishron;
					break;
				case ProgressionGroupID.Empress:
					npcid = NPCID.HallowBoss;
					break;
				case ProgressionGroupID.LunaticCultist:
					npcid = NPCID.CultistBoss;
					break;
				case ProgressionGroupID.MoonLord:
					npcid = NPCID.MoonLordCore;
					break;
			}

			if (npcid > NPCID.NegativeIDCount)
				return new() { npcid };

			return new();
		}
		public bool TryGetBossType(out SortedSet<int> type) {
			type = null;
			if (bossNetIDs.Count <= 0)
				return false;

			type = bossNetIDs;
			return true;
		}
		public bool TryGetSpawnConditions(out NPCSpawnInfo nPCSpawnInfo) {
			nPCSpawnInfo = new();
			switch (ID) {
				case ProgressionGroupID.Forest:
					nPCSpawnInfo = new();
					break;
				case ProgressionGroupID.Desert:
					nPCSpawnInfo = new();
					break;
				case ProgressionGroupID.GiantTree:
					nPCSpawnInfo = new();
					break;
				case ProgressionGroupID.Beach:
					nPCSpawnInfo = new();
					break;
				case ProgressionGroupID.Snow:
					nPCSpawnInfo = new();
					break;
				case ProgressionGroupID.Ocean:
					nPCSpawnInfo = new();
					break;
				case ProgressionGroupID.Jungle:
					nPCSpawnInfo = new();
					break;
				case ProgressionGroupID.Evil:
					nPCSpawnInfo = new();
					break;
				case ProgressionGroupID.Granite:
					nPCSpawnInfo = new();
					break;
				case ProgressionGroupID.Marble:
					nPCSpawnInfo = new();
					break;
				case ProgressionGroupID.BloodMoon:
					nPCSpawnInfo = new();
					break;
				case ProgressionGroupID.GoblinArmy:
					nPCSpawnInfo = new();
					break;
				case ProgressionGroupID.Dungeon:
					nPCSpawnInfo = new();
					break;
				case ProgressionGroupID.FrostLegeon:
					nPCSpawnInfo = new();
					break;
				case ProgressionGroupID.Pirates:
					nPCSpawnInfo = new();
					break;
				case ProgressionGroupID.Eclipse:
					nPCSpawnInfo = new();
					break;
				case ProgressionGroupID.PumpkinMoon:
					nPCSpawnInfo = new();
					break;
				case ProgressionGroupID.FrostMoon:
					nPCSpawnInfo = new();
					break;
				case ProgressionGroupID.MartianInvasion:
					nPCSpawnInfo = new();
					break;
				case ProgressionGroupID.LunarInvasion:
					nPCSpawnInfo = new();
					break;
				default:
					return false;
			}

			return true;
		}
	}
	public static class InfusionProgression {
		private static bool guessingInfusionPowers => false;
		public const int VANILLA_RECIPE_COUNT = 2691;
		private static bool finishedSetup = false;
		public static SortedDictionary<int, InfusionPowerSource> WeaponSources { get; private set; } = new();//Not cleared
		public static SortedDictionary<InfusionPowerSource, int> SourceInfusionPowers { get; private set; } = new();//Not cleared
		private static SortedDictionary<int, HashSet<HashSet<int>>> allWeaponRecipies = new();
		private static SortedSet<int> weaponsList = new();
		private static SortedSet<int> weaponCraftingIngredients = new();
		public static SortedDictionary<int, HashSet<HashSet<int>>> allExpandedRecepies = new();
		private static SortedDictionary<int, HashSet<int>> reverseCraftableIngredients = new();
		public static SortedDictionary<int, int> OreInfusionPowers = new() {
			{ ItemID.CopperOre, 10 },
			{ ItemID.TinOre, 10 },
			{ ItemID.IronOre, 20 },
			{ ItemID.LeadOre, 20 },
			{ ItemID.SilverOre, 55 },
			{ ItemID.TungstenOre, 55 },
			{ ItemID.GoldOre, 75 },
			{ ItemID.PlatinumOre, 75 },
			{ ItemID.Obsidian, 80 },
			{ ItemID.DemoniteOre, 120 },
			{ ItemID.CrimtaneOre, 120 },
			{ ItemID.Meteorite, 210 },//Needs to be post EaterOfWorlds/BrainOfCthulhu instead?
			{ ItemID.Hellstone, 220 },
			{ ItemID.CobaltOre, 430 },
			{ ItemID.PalladiumOre, 430 },
			{ ItemID.MythrilOre, 440 },
			{ ItemID.OrichalcumOre, 440 },
			{ ItemID.AdamantiteOre, 450 },
			{ ItemID.TitaniumOre, 450 },
			{ ItemID.ChlorophyteOre, 653 }
		};
		public static SortedDictionary<string, int> ModOreInfusionPowers = new() {

		};
		public static SortedSet<int> NPCsThatDropWeaponsOrIngredients { get; private set; } = new();//Not cleared
		public static SortedDictionary<int, int> VanillaCraftingItemSourceInfusionPowers = new() {
			//{ ItemID.Wood, 0 },
			//{ ItemID.StoneBlock, 0 },
			{ ItemID.Gel, 5 },//Slime
			//{ ItemID.FallenStar, 5 },//Night
			{ ItemID.Cobweb, 8 },//Cavern
			//{ ItemID.Cactus, 10 },
			//{ ItemID.SandBlock, 10 },
			//{ ItemID.PalmWood, 30 },
			//{ ItemID.Shiverthorn, 30 },
			//{ ItemID.BorealWood, 30 },
			{ ItemID.Grenade, 40 },
			{ ItemID.Amethyst, 50 },
			{ ItemID.Topaz, 55 },
			{ ItemID.Sapphire, 60 },
			{ ItemID.FlinxFur, 65 },
			{ ItemID.Emerald, 65 },
			{ ItemID.PinkGel, 70 },
			{ ItemID.Ruby, 75 },
			{ ItemID.Mace, 80 },
			//{ ItemID.EbonsandBlock, 80 },
			//{ ItemID.CrimsandBlock, 80 },
			//{ ItemID.VileMushroom, 80 },
			//{ ItemID.ViciousMushroom, 80 },
			//{ ItemID.Ebonwood, 80 },
			//{ ItemID.Shadewood, 80 },
			//{ ItemID.RichMahogany, 80 },
			{ ItemID.LifeCrystal, 85 },
			{ ItemID.Diamond, 85 },
			//{ ItemID.Amber, 90 },
			{ ItemID.FossilOre, 95 },//Drop
			{ ItemID.AntlionMandible, 95 },//Drop
			//{ ItemID.JungleSpores, 100 },
			{ ItemID.Stinger, 105 },//Drop
			{ ItemID.Vine, 105 },//Drop
			{ ItemID.CorruptSeeds, 120 },
			{ ItemID.HallowedSeeds, 120 },
			{ ItemID.CrimsonSeeds, 120 },
			{ ItemID.IllegalGunParts, 160 },
			{ ItemID.Minishark, 190 },
			{ ItemID.TissueSample, 200 },//Needs to be EaterOfWorlds/BrainOfCthulhu instead
			{ ItemID.ShadowScale, 200 },//Needs to be EaterOfWorlds/BrainOfCthulhu instead
			{ ItemID.Hellforge, 205 },//Needs to be post EaterOfWorlds/BrainOfCthulhu instead
			{ ItemID.Bone, 200 },//Post Skeletron
			{ ItemID.BeeWax, 243 },
			{ ItemID.PixieDust, 400 },
			{ ItemID.Pearlwood, 400 },
			{ ItemID.CrystalShard, 420 },
			{ ItemID.SoulofNight, 420 },
			{ ItemID.SpiderFang, 420 },
			{ ItemID.AncientBattleArmorMaterial, 420 },
			{ ItemID.DjinnLamp, 420 },
			{ ItemID.SoulofLight, 440 },
			{ ItemID.DarkShard, 453 },
			{ ItemID.LightShard, 453 },
			{ ItemID.Book, 460 },
			{ ItemID.CursedFlame, 460 },
			{ ItemID.SpellTome, 460 },
			{ ItemID.Ichor, 460 },
			{ ItemID.FrostCore, 460 },
			{ ItemID.Shotgun, 467 },
			{ ItemID.HallowedBar, 598 },
			{ ItemID.SoulofMight, 603 },
			{ ItemID.SharkFin, 606 },
			{ ItemID.SoulofFright, 616 },
			{ ItemID.Lens, 628 },
			{ ItemID.BlackLens, 628 },
			{ ItemID.Harp, 628 },
			{ ItemID.UnicornHorn, 628 },
			{ ItemID.SoulofSight, 628 },
			{ ItemID.GlowingMushroom, 678 },
			{ ItemID.Autohammer, 678 },
			{ ItemID.BrokenHeroSword, 725 },
			{ ItemID.Ectoplasm, 734 },
			{ ItemID.FragmentVortex, 1007 },
			{ ItemID.FragmentNebula, 1007 },
			{ ItemID.FragmentSolar, 1007 },
			{ ItemID.FragmentStardust, 1007 },
			{ ItemID.LunarCraftingStation, 1007 },
			{ ItemID.LunarOre, 1020 },
			{ ItemID.Meowmere, 1100 },
			{ ItemID.StarWrath, 1100 }
		};//Not cleared
		public static SortedDictionary<string, int> ModdedCraftingItemSourceInfusionPowers = new() {
			{ "Wulfrum Metal Scrap", 40 },
			{ "Energy Core", 43 },
			{ "Desert Feather", 63 },
			{ "Sea Remains", 87 },
			{ "Pearl Shard", 97 },
			{ "Stormlion Mandible", 97 },
			{ "Navystone", 97 },
			{ "Sea Prism", 97 },
			{ "Astral Monolith", 120 },
			{ "Astral Sand", 120 },
			{ "Sulphuric Scale", 125 },
			{ "Acidwood", 125 },
			{ "Blood Sample", 215 },
			{ "Rotten Matter", 215 },
			{ "Aerialite Bar", 230 },
			{ "Dubious Plating", 235 },
			{ "Mysterious Circuitry", 235 },
			{ "Purified Gel", 280 },
			{ "Essence of Eleum", 420 },
			{ "Essence of Havoc", 420 },
			{ "Essence of Sunlight", 420 },
			{ "Mollusk Husk", 420 },
			{ "Stardust", 420 },
			{ "Prototype Plasma Drive Core", 440 },
			{ "Suspicious Scrap", 440 },
			{ "Unholy Core", 450 },
			{ "Cryonic Bar", 585 },
			{ "Corroded Fossil", 600 },
			{ "Hoarfrost Bow", 600 },
			{ "Solar Veil", 615 },
			{ "Ashes of Calamity", 700 },
			{ "Depth Cells", 710 },
			{ "Lumenyl", 710 },
			{ "Smooth Voidstone", 710 },
			{ "Sulphurous Sand", 710 },
			{ "Voidstone", 710 },
			{ "Living Shard", 725 },
			{ "Perennial Bar", 725 },
			{ "Core of Eleum", 750 },
			{ "Core of Havoc", 750 },
			{ "Core of Sunlight", 750 },
			{ "Grand Scale", 775 },
			{ "Core of Calamity", 860 },
			{ "Infected Armor Plating", 860 },
			{ "Life Alloy", 860 },
			{ "Scoria Bar", 860 },
			{ "Plague Cell Canister", 880 },
			{ "Planty Mush", 885 },
			{ "Meld Construct", 995 },
			{ "Galactica Singularity", 1005 },
			{ "Astral Bar", 1010 },
			{ "Auric Bar", 1100 },
			{ "Bloodstone Core", 1105 },
			{ "Demonic Bone Ash", 1105 },
			{ "Effulgent Feather", 1150 },
			{ "Divine Geode", 1180 },
			{ "Unholy Essence", 1180 },
			{ "Uelibloom Bar", 1200 },
			{ "Aureus Cell", 1210 },
			{ "Dark Plasma", 1240 },
			{ "Armored Shell", 1270 },
			{ "Phantoplasm", 1270 },
			{ "Twisting Nether", 1300 },
			{ "Reaper Tooth", 1330 },
			{ "Ruinous Soul", 1330 },
			{ "Exodium Cluster", 1330 },
			{ "Cosmilite Bar", 1400 },
			{ "Darksun Fragment", 1450 },
			{ "Endothermic Energy", 1450 },
			{ "Nightmare Fuel", 1450 },
			{ "The Obliterator", 1450 },
			{ "Eradicator", 1450 },
			{ "Ascendant Spirit Essence", 1500 },
			{ "Yharon Soul Fragment", 1550 },
			{ "Miracle Matter", 1600 },
			{ "Shadowspec Bar", 1700 },
			{ "Abombination", 1700 },
			{ "Heresy", 1700 }
		};//Not cleared
		public static SortedDictionary<int, SortedSet<int>> WeaponsFromNPCs { get; private set; } = new();//Not cleared
		public static SortedDictionary<int, SortedSet<int>> IngredientsFromNPCs { get; private set; } = new();//Not cleared
		public static SortedDictionary<int, SortedSet<int>> WeaponsFromLootItems { get; private set; } = new();//Not cleared
		public static SortedDictionary<int, SortedSet<int>> IngredientsFromLootItems { get; private set; } = new();//Not cleared
		public static SortedDictionary<ProgressionGroupID, ProgressionGroup> progressionGroups = new();
		public static SortedDictionary<ProgressionGroupID, SortedSet<int>> gatherItemBiome = new() {
			{ ProgressionGroupID.Forest, new() { ItemID.Wood, ItemID.Acorn, ItemID.StoneBlock } },
			{ ProgressionGroupID.Desert, new() { ItemID.Cactus, ItemID.SandBlock, ItemID.Amber } },
			{ ProgressionGroupID.Beach, new() { ItemID.PalmWood } },
			{ ProgressionGroupID.Snow, new() { ItemID.Shiverthorn, ItemID.BorealWood } },
			{ ProgressionGroupID.Jungle, new() { ItemID.RichMahogany, ItemID.JungleSpores } },
			{ ProgressionGroupID.Evil, new() { ItemID.Ebonwood, ItemID.Shadewood, ItemID.EbonsandBlock,
				ItemID.CrimsandBlock, ItemID.ViciousMushroom, ItemID.VileMushroom } },
			//{ ProgressionGroupID., new() { ItemID. } },
			//{ ProgressionGroupID., new() { ItemID. } },
			//{ ProgressionGroupID., new() { ItemID. } }
		};
		public static SortedDictionary<ProgressionGroupID, SortedSet<int>> undergroundItems = new() {
			{ ProgressionGroupID.Desert, new() { ItemID.Amber } },
			{ ProgressionGroupID.Jungle, new() { ItemID.JungleSpores } },
			/*
			{ ProgressionGroupID., new() { ItemID. } },
			{ ProgressionGroupID., new() { ItemID. } },
			{ ProgressionGroupID., new() { ItemID. } },
			{ ProgressionGroupID., new() { ItemID. } },
			{ ProgressionGroupID., new() { ItemID. } },
			{ ProgressionGroupID., new() { ItemID. } },
			{ ProgressionGroupID., new() { ItemID. } },
			{ ProgressionGroupID., new() { ItemID. } }
			*/
		};
		public static SortedDictionary<ProgressionGroupID, SortedSet<int>> nightItems = new() {
			{ ProgressionGroupID.Forest, new() { ItemID.FallenStar } }
		};
		private static SortedDictionary<int, (int pickPower, float value)> infusionPowerTiles = null;
		public static SortedDictionary<int, (int pickPower, float value)> InfusionPowerTiles {
			get {
				if (infusionPowerTiles == null)
					SetupInfusionPowerTiles();

				return infusionPowerTiles;
			}
		}
		public static SortedDictionary<ProgressionGroupID, SortedSet<int>> NPCGroups = new() {//TODO: Set up keybind to print all enemies spawned since last press.  Only display NPCID.name seperated by commas.
			/*
			{ ProgressionGroupID.Forest, new() {  } },
			{ ProgressionGroupID.Desert, new() { NPCID. } },
			{ ProgressionGroupID.Beach, new() { NPCID. } },
			{ ProgressionGroupID.Snow, new() { NPCID. } },
			{ ProgressionGroupID.Ocean, new() { NPCID. } },
			{ ProgressionGroupID.Jungle, new() { NPCID. } },
			{ ProgressionGroupID.Evil, new() { NPCID. } },
			{ ProgressionGroupID.Granite, new() { NPCID. } },
			{ ProgressionGroupID.Marble, new() { NPCID. } },
			{ ProgressionGroupID.BloodMoon, new() { NPCID. } },
			{ ProgressionGroupID.GoblinArmy, new() { NPCID. } },
			{ ProgressionGroupID.Dungeon, new() { NPCID. } },
			{ ProgressionGroupID.ShadowChest, new() { NPCID. } },
			{ ProgressionGroupID.FrostLegeon, new() { NPCID. } },
			{ ProgressionGroupID.Pirates, new() { NPCID. } },
			{ ProgressionGroupID.Eclipse, new() { NPCID. } },
			{ ProgressionGroupID.PumpkinMoon, new() { NPCID. } },
			{ ProgressionGroupID.FrostMoon, new() { NPCID. } },
			{ ProgressionGroupID.MartianInvasion, new() { NPCID. } },
			{ ProgressionGroupID.LunarInvasion, new() { NPCID. } }
			*/
		};
		private static SortedDictionary<int, ((int[], bool[]), (int[], bool[]))> NPCSpawnRulesTypes = new() {
			{ NPCID.BigRainZombie, ((new int[]{ 0, 3774, 259, 41, 3690, 244 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3837, 264, 712, 3911, 269 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.BigHeadacheSkeleton, ((new int[]{ 0, 2486, 758, 1, 2461, 713 }, new bool[]{ false, false, false, false, false, false, false, false, true, false, false, true }), (new int[]{ 0, 2486, 758, 1, 2461, 713 }, new bool[]{ false, false, false, false, false, false, false, false, true, false, false, true })) },
			{ NPCID.SmallSkeleton, ((new int[]{ 0, 1887, 575, 0, 1965, 555 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2110, 753, 63, 2190, 792 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.BigFemaleZombie, ((new int[]{ 0, 2015, 286, 2, 1939, 284 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2267, 288, 2, 2191, 297 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.SmallFemaleZombie, ((new int[]{ 0, 3049, 254, 147, 3128, 285 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 3049, 254, 147, 3128, 285 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ NPCID.DemonEye2, ((new int[]{ 0, 901, 216, 1, 979, 251 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 4111, 389, 727, 4179, 431 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.PurpleEye2, ((new int[]{ 0, 1341, 285, 2, 1256, 285 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2266, 426, 311, 2324, 384 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.GreenEye2, ((new int[]{ 0, 2148, 284, 2, 2073, 284 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3485, 403, 181, 3562, 431 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.DialatedEye2, ((new int[]{ 0, 2266, 267, 1, 2240, 290 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3782, 389, 59, 3812, 431 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.SleepyEye2, ((new int[]{ 0, 1719, 211, 2, 1786, 248 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2582, 310, 728, 2576, 304 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.BigTwiggyZombie, ((new int[]{ 0, 748, 234, 0, 814, 243 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2067, 288, 53, 2084, 327 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.SmallTwiggyZombie, ((new int[]{ 0, 2033, 284, 0, 1961, 286 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2427, 302, 53, 2497, 295 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.BigSwampZombie, ((new int[]{ 0, 2044, 261, 2, 1964, 270 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 4111, 280, 712, 4179, 303 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.SmallSwampZombie, ((new int[]{ 0, 1874, 282, 2, 1946, 284 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2215, 302, 2, 2150, 294 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.BigSlimedZombie, ((new int[]{ 0, 1923, 284, 0, 1838, 285 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2138, 289, 53, 2208, 303 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.SmallSlimedZombie, ((new int[]{ 0, 2089, 286, 0, 2011, 286 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2089, 286, 0, 2011, 286 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.SmallPincushionZombie, ((new int[]{ 0, 901, 211, 53, 979, 248 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 4111, 302, 728, 4179, 312 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.BigBaldZombie, ((new int[]{ 0, 2089, 282, 0, 2010, 285 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2358, 286, 2, 2280, 287 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.SmallBaldZombie, ((new int[]{ 0, 2241, 286, 2, 2175, 285 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2241, 286, 2, 2175, 285 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.BigZombie, ((new int[]{ 0, 2095, 285, 2, 2020, 283 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2241, 286, 2, 2175, 285 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.HeavySkeleton, ((new int[]{ 0, 934, 714, 0, 877, 738 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3018, 1046, 368, 3087, 1040 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.ShortBones, ((new int[]{ 0, 3443, 459, 48, 3508, 457 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3443, 665, 182, 3522, 645 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.BigEater, ((new int[]{ 0, 1073, 223, 23, 1098, 236 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3018, 497, 168, 3095, 537 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.LittleEater, ((new int[]{ 0, 1054, 192, 0, 985, 147 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3020, 431, 152, 3068, 407 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.JungleSlime, ((new int[]{ 0, 391, 162, 0, 465, 113 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 1366, 312, 384, 1439, 333 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.YellowSlime, ((new int[]{ 0, 3732, 344, 0, 3725, 383 }, new bool[]{ false, false, false, false, false, false, false, false, true, false, false, false }), (new int[]{ 0, 3732, 344, 0, 3725, 383 }, new bool[]{ false, false, false, false, false, false, false, false, true, false, false, false })) },
			{ NPCID.RedSlime, ((new int[]{ 0, 1268, 288, 25, 1303, 260 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 1441, 371, 25, 1465, 410 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.PurpleSlime, ((new int[]{ 0, 1960, 282, 2, 1887, 286 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 4015, 775, 682, 4035, 728 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.BlackSlime, ((new int[]{ 0, 1883, 414, 0, 1801, 389 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3304, 966, 181, 3273, 987 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.BabySlime, ((new int[]{ 0, 2266, 431, 0, 2229, 437 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3304, 874, 831, 3253, 915 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.GreenSlime, ((new int[]{ 0, 1054, 182, 0, 1134, 135 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 4015, 775, 727, 4035, 728 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Slimeling, ((new int[]{ 0, 1127, 234, 0, 1049, 236 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3153, 864, 321, 3233, 864 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.BlueSlime, ((new int[]{ 0, 53, 79, 0, 42, 119 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 4015, 1068, 831, 4035, 1086 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.DemonEye, ((new int[]{ 0, 45, 160, 0, 117, 116 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 4111, 614, 740, 4179, 573 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Zombie, ((new int[]{ 0, 167, 167, 0, 236, 204 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3635, 446, 740, 3709, 486 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.EaterofSouls, ((new int[]{ 0, 1112, 179, 0, 1028, 137 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3316, 451, 163, 3252, 436 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.DevourerHead, ((new int[]{ 0, 1201, 228, 1, 1134, 236 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3277, 1049, 368, 3223, 1078 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.DevourerBody, ((new int[]{ 0, 1201, 228, 1, 1134, 236 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3277, 1049, 368, 3206, 1078 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.DevourerTail, ((new int[]{ 0, 1201, 228, 1, 1134, 236 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3277, 1049, 368, 3206, 1078 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.GiantWormHead, ((new int[]{ 0, 71, 299, 0, 147, 341 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3583, 1041, 397, 3665, 1010 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.GiantWormBody, ((new int[]{ 0, 71, 299, 0, 147, 341 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3583, 1041, 397, 3665, 1010 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.GiantWormTail, ((new int[]{ 0, 71, 299, 0, 147, 341 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3583, 1041, 397, 3665, 1010 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.MotherSlime, ((new int[]{ 0, 158, 382, 0, 92, 424 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 1051, 734, 3815, 1010 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Skeleton, ((new int[]{ 0, 631, 412, 0, 547, 425 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3443, 1047, 368, 3521, 1010 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.MeteorHead, ((new int[]{ 0, 2196, 235, 0, 2122, 262 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2713, 303, 734, 2700, 343 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.FireImp, ((new int[]{ 0, 44, 971, 0, 28, 1001 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3443, 1084, 829, 3483, 1105 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.BurningSphere, ((new int[]{ 0, 63, 286, 0, 37, 285 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3281, 1084, 829, 3248, 1106 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.GoblinPeon, ((new int[]{ 0, 1745, 252, 0, 1663, 257 }, new bool[]{ false, false, true, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2931, 309, 161, 3009, 351 }, new bool[]{ false, false, true, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.GoblinThief, ((new int[]{ 0, 1745, 248, 0, 1660, 256 }, new bool[]{ false, false, true, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2943, 309, 167, 3013, 351 }, new bool[]{ false, false, true, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.GoblinWarrior, ((new int[]{ 0, 1745, 243, 0, 1661, 257 }, new bool[]{ false, false, true, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2967, 309, 167, 3019, 351 }, new bool[]{ false, false, true, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.GoblinSorcerer, ((new int[]{ 0, 1745, 245, 0, 1662, 257 }, new bool[]{ false, false, true, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2956, 309, 167, 3021, 351 }, new bool[]{ false, false, true, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.ChaosBall, ((new int[]{ 0, 58, 198, 0, 50, 210 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 4151, 311, 746, 4175, 347 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.AngryBones, ((new int[]{ 0, 3443, 441, 7, 3365, 457 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 975, 182, 3813, 971 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.DarkCaster, ((new int[]{ 0, 3443, 432, 1, 3369, 396 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 980, 41, 3812, 971 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.WaterSphere, ((new int[]{ 0, 3443, 401, 0, 3473, 359 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 961, 481, 3813, 964 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.CursedSkull, ((new int[]{ 0, 3583, 496, 41, 3512, 533 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 958, 41, 3813, 966 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.SkeletronHand, ((new int[]{ 0, 3628, 270, 2, 3555, 287 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3628, 270, 2, 3555, 287 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.BoneSerpentHead, ((new int[]{ 0, 109, 990, 19, 85, 1016 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3225, 1084, 829, 3287, 1086 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.BoneSerpentBody, ((new int[]{ 0, 109, 990, 19, 85, 1016 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3225, 1084, 829, 3287, 1086 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.BoneSerpentTail, ((new int[]{ 0, 109, 1008, 19, 85, 1016 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3225, 1079, 76, 3287, 1086 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Hornet, ((new int[]{ 0, 271, 355, 60, 342, 385 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 1242, 1043, 60, 1162, 1008 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.ManEater, ((new int[]{ 0, 272, 360, 60, 346, 386 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 1257, 1049, 60, 1182, 1010 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.UndeadMiner, ((new int[]{ 0, 213, 399, 0, 137, 422 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3583, 1053, 830, 3523, 1010 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Tim, ((new int[]{ 0, 159, 785, 0, 116, 811 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3443, 1050, 367, 3522, 1008 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Bunny, ((new int[]{ 0, 1289, 238, 0, 1209, 262 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3631, 352, 397, 3589, 359 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Harpy, ((new int[]{ 0, 41, 44, 0, 3, 3 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3765, 380, 746, 3832, 416 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.CaveBat, ((new int[]{ 0, 123, 385, 0, 117, 424 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 1057, 1345, 3815, 1010 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.JungleBat, ((new int[]{ 0, 148, 167, 1, 127, 204 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 1322, 375, 384, 1308, 353 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.DoctorBones, ((new int[]{ 0, 635, 250, 60, 623, 288 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 1219, 292, 60, 1146, 306 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.TheGroom, ((new int[]{ 0, 2095, 242, 2, 2023, 256 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3628, 299, 712, 3707, 312 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Goldfish, ((new int[]{ 0, 138, 187, 0, 69, 214 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 3732, 961, 481, 3812, 925 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ NPCID.Snatcher, ((new int[]{ 0, 321, 161, 0, 383, 112 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 1289, 312, 60, 1354, 352 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.CorruptGoldfish, ((new int[]{ 0, 1155, 179, 0, 1079, 139 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3306, 959, 368, 3382, 998 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Piranha, ((new int[]{ 0, 54, 179, 0, 132, 209 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 954, 734, 3814, 951 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.LavaSlime, ((new int[]{ 0, 41, 928, 0, 28, 893 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3304, 1084, 829, 3376, 1108 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Hellbat, ((new int[]{ 0, 41, 865, 0, 28, 865 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3443, 1084, 829, 3490, 1116 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Vulture, ((new int[]{ 0, 164, 162, 1, 231, 204 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2997, 376, 60, 2921, 384 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Demon, ((new int[]{ 0, 48, 984, 1, 29, 981 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3304, 1084, 829, 3288, 1108 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.BlueJellyfish, ((new int[]{ 0, 50, 267, 0, 51, 254 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 1002, 821, 3814, 1043 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.PinkJellyfish, ((new int[]{ 0, 41, 166, 53, 3, 204 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 432, 380, 60, 405, 380 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Shark, ((new int[]{ 0, 41, 177, 53, 5, 208 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 310, 375, 60, 373, 380 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ NPCID.VoodooDemon, ((new int[]{ 0, 120, 977, 1, 40, 1012 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3234, 1081, 829, 3243, 1101 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Crab, ((new int[]{ 0, 41, 142, 0, 7, 103 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3018, 574, 163, 3095, 610 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Antlion, ((new int[]{ 0, 164, 163, 1, 231, 204 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2938, 542, 396, 2878, 553 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.SpikeBall, ((new int[]{ 0, 3583, 494, 41, 3541, 501 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 978, 41, 3813, 971 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.DungeonSlime, ((new int[]{ 0, 3583, 525, 41, 3514, 539 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 928, 41, 3811, 966 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.BlazingWheel, ((new int[]{ 0, 3583, 526, 41, 3517, 491 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 927, 41, 3801, 969 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.GoblinScout, ((new int[]{ 0, 163, 209, 1, 246, 211 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3602, 353, 384, 3674, 315 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Bird, ((new int[]{ 0, 1198, 230, 2, 1178, 261 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3605, 376, 147, 3592, 336 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Pixie, ((new int[]{ 0, 1233, 155, 0, 1166, 108 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 1811, 313, 117, 1888, 346 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.ArmoredSkeleton, ((new int[]{ 0, 72, 345, 0, 9, 324 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 1053, 831, 3815, 1040 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Mummy, ((new int[]{ 0, 147, 143, 0, 82, 98 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3583, 707, 746, 3602, 694 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.DarkMummy, ((new int[]{ 0, 2625, 707, 0, 2594, 743 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2726, 1021, 112, 2753, 982 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.LightMummy, ((new int[]{ 0, 1674, 309, 116, 1683, 346 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 1872, 1023, 116, 1939, 977 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.CorruptSlime, ((new int[]{ 0, 1150, 220, 0, 1160, 236 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3304, 1054, 368, 3229, 1010 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Wraith, ((new int[]{ 0, 177, 163, 0, 251, 204 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3759, 516, 728, 3830, 554 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.CursedHammer, ((new int[]{ 0, 2625, 450, 25, 2629, 441 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3018, 1051, 163, 2961, 1006 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.EnchantedSword, ((new int[]{ 0, 1858, 1033, 117, 1937, 989 }, new bool[]{ false, false, false, false, false, false, false, false, true, false, false, false }), (new int[]{ 0, 1858, 1033, 117, 1937, 989 }, new bool[]{ false, false, false, false, false, false, false, false, true, false, false, false })) },
			{ NPCID.Mimic, ((new int[]{ 0, 78, 244, 0, 34, 265 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 1079, 829, 3815, 1098 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Unicorn, ((new int[]{ 0, 1306, 185, 0, 1231, 141 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2029, 312, 117, 2089, 341 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.WyvernHead, ((new int[]{ 0, 51, 46, 0, 15, 18 }, new bool[]{ false, false, false, false, false, false, false, false, false, true, false, false }), (new int[]{ 0, 3741, 199, 0, 3692, 151 }, new bool[]{ false, false, false, false, false, false, false, false, false, true, false, false })) },
			{ NPCID.WyvernLegs, ((new int[]{ 0, 51, 46, 0, 15, 18 }, new bool[]{ false, false, false, false, false, false, false, false, false, true, false, false }), (new int[]{ 0, 3741, 199, 0, 3692, 151 }, new bool[]{ false, false, false, false, false, false, false, false, false, true, false, false })) },
			{ NPCID.WyvernBody, ((new int[]{ 0, 51, 46, 0, 15, 18 }, new bool[]{ false, false, false, false, false, false, false, false, false, true, false, false }), (new int[]{ 0, 3741, 199, 0, 3692, 151 }, new bool[]{ false, false, false, false, false, false, false, false, false, true, false, false })) },
			{ NPCID.WyvernBody2, ((new int[]{ 0, 51, 46, 0, 15, 18 }, new bool[]{ false, false, false, false, false, false, false, false, false, true, false, false }), (new int[]{ 0, 3741, 199, 0, 3692, 151 }, new bool[]{ false, false, false, false, false, false, false, false, false, true, false, false })) },
			{ NPCID.WyvernBody3, ((new int[]{ 0, 51, 46, 0, 15, 18 }, new bool[]{ false, false, false, false, false, false, false, false, false, true, false, false }), (new int[]{ 0, 3741, 199, 0, 3692, 151 }, new bool[]{ false, false, false, false, false, false, false, false, false, true, false, false })) },
			{ NPCID.WyvernTail, ((new int[]{ 0, 51, 46, 0, 15, 18 }, new bool[]{ false, false, false, false, false, false, false, false, false, true, false, false }), (new int[]{ 0, 3741, 199, 0, 3692, 151 }, new bool[]{ false, false, false, false, false, false, false, false, false, true, false, false })) },
			{ NPCID.GiantBat, ((new int[]{ 0, 41, 333, 0, 10, 311 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 1059, 1345, 3815, 1061 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Corruptor, ((new int[]{ 0, 1066, 192, 0, 1094, 147 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3304, 1050, 367, 3232, 1071 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.DiggerHead, ((new int[]{ 0, 1088, 322, 0, 1135, 344 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 1046, 830, 3815, 1006 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.DiggerBody, ((new int[]{ 0, 1088, 322, 0, 1135, 344 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 1046, 830, 3815, 1006 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.DiggerTail, ((new int[]{ 0, 1088, 398, 0, 1135, 403 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 1046, 830, 3815, 1006 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.SeekerHead, ((new int[]{ 0, 1203, 249, 0, 1132, 261 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3293, 1066, 368, 3355, 1099 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.SeekerBody, ((new int[]{ 0, 1203, 249, 0, 1132, 261 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3293, 1066, 368, 3355, 1099 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.SeekerTail, ((new int[]{ 0, 1203, 266, 0, 1132, 261 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3293, 1066, 368, 3355, 1099 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Clinger, ((new int[]{ 0, 2486, 341, 1, 2535, 337 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3153, 1049, 397, 3219, 1008 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.AnglerFish, ((new int[]{ 0, 210, 142, 0, 127, 95 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 981, 734, 3815, 951 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.GreenJellyfish, ((new int[]{ 0, 1674, 298, 0, 1674, 338 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 992, 368, 3815, 951 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.BoundGoblin, ((new int[]{ 0, 2105, 385, 0, 2038, 422 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 1018, 831, 3804, 987 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.BoundWizard, ((new int[]{ 0, 584, 384, 0, 645, 422 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 1035, 381, 3808, 988 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.SkeletonArcher, ((new int[]{ 0, 48, 345, 0, 115, 324 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 1059, 830, 3815, 1071 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.GoblinArcher, ((new int[]{ 0, 1745, 244, 0, 1661, 255 }, new bool[]{ false, false, true, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2948, 309, 161, 3025, 351 }, new bool[]{ false, false, true, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.VileSpit, ((new int[]{ 0, 1161, 148, 0, 1149, 101 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3018, 895, 162, 3083, 936 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.WallofFleshEye, ((new int[]{ 0, 2987, 1081, 76, 2945, 1041 }, new bool[]{ false, false, false, false, false, false, false, false, true, false, false, false }), (new int[]{ 0, 2987, 1081, 76, 2945, 1041 }, new bool[]{ false, false, false, false, false, false, false, false, true, false, false, false })) },
			{ NPCID.TheHungry, ((new int[]{ 0, 2987, 1081, 76, 2945, 1041 }, new bool[]{ false, false, false, false, false, false, false, false, true, false, false, false }), (new int[]{ 0, 2987, 1081, 76, 2945, 1041 }, new bool[]{ false, false, false, false, false, false, false, false, true, false, false, false })) },
			{ NPCID.TheHungryII, ((new int[]{ 0, 2987, 1081, 76, 2945, 1041 }, new bool[]{ false, false, false, false, false, false, false, false, true, false, false, false }), (new int[]{ 0, 2987, 1081, 76, 2945, 1041 }, new bool[]{ false, false, false, false, false, false, false, false, true, false, false, false })) },
			{ NPCID.ChaosElemental, ((new int[]{ 0, 1859, 1011, 117, 1857, 971 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2008, 1050, 117, 2001, 1005 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Slimer, ((new int[]{ 0, 1148, 225, 1, 1086, 248 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3176, 1049, 163, 3232, 1009 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Gastropod, ((new int[]{ 0, 1265, 196, 0, 1230, 150 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 1788, 310, 117, 1807, 346 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.BoundMechanic, ((new int[]{ 0, 3583, 524, 41, 3648, 511 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 975, 41, 3811, 962 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.PrimeCannon, ((new int[]{ 0, 2060, 286, 2, 2127, 286 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2060, 286, 2, 2127, 286 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.PrimeSaw, ((new int[]{ 0, 2060, 286, 2, 2127, 286 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2060, 286, 2, 2127, 286 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.PrimeVice, ((new int[]{ 0, 2060, 286, 2, 2127, 286 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2060, 286, 2, 2127, 286 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.PrimeLaser, ((new int[]{ 0, 2060, 286, 2, 2127, 286 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2060, 286, 2, 2127, 286 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.BaldZombie, ((new int[]{ 0, 143, 162, 0, 117, 204 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3633, 375, 746, 3704, 336 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.WanderingEye, ((new int[]{ 0, 217, 187, 0, 298, 206 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3637, 388, 728, 3705, 429 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.IlluminantBat, ((new int[]{ 0, 1675, 847, 0, 1697, 835 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2105, 1050, 117, 2184, 1041 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.IlluminantSlime, ((new int[]{ 0, 1748, 835, 0, 1681, 819 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2219, 1057, 367, 2250, 1059 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Probe, ((new int[]{ 0, 2028, 286, 2, 2101, 286 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2028, 286, 2, 2101, 286 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.PossessedArmor, ((new int[]{ 0, 200, 169, 0, 261, 204 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 4111, 689, 746, 4179, 730 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.ToxicSludge, ((new int[]{ 0, 989, 254, 0, 984, 223 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 504, 746, 3813, 539 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.SnowmanGangsta, ((new int[]{ 0, 901, 246, 0, 979, 255 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2977, 298, 727, 3043, 328 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.MisterStabby, ((new int[]{ 0, 901, 245, 0, 979, 255 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2979, 300, 727, 3038, 328 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.SnowBalla, ((new int[]{ 0, 901, 245, 0, 979, 255 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3001, 292, 727, 3083, 326 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.IceSlime, ((new int[]{ 0, 2714, 179, 0, 2658, 139 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3306, 483, 726, 3387, 521 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Penguin, ((new int[]{ 0, 2081, 234, 147, 2093, 261 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3374, 375, 161, 3306, 375 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.PenguinBlack, ((new int[]{ 0, 2816, 233, 147, 2897, 261 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3361, 364, 161, 3308, 375 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.IceBat, ((new int[]{ 0, 2880, 334, 1, 2809, 291 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3443, 772, 163, 3384, 760 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Lavabat, ((new int[]{ 0, 41, 881, 0, 29, 874 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3324, 1073, 829, 3376, 1106 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.GiantFlyingFox, ((new int[]{ 0, 400, 195, 60, 407, 204 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 1257, 311, 384, 1190, 312 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.GiantTortoise, ((new int[]{ 0, 170, 144, 0, 97, 99 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 1317, 1049, 384, 1308, 1006 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.IceTortoise, ((new int[]{ 0, 2714, 393, 0, 2789, 361 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3443, 804, 1345, 3524, 826 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Wolf, ((new int[]{ 0, 2015, 179, 0, 1937, 139 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3481, 421, 161, 3489, 462 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.RedDevil, ((new int[]{ 0, 41, 881, 0, 30, 886 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3304, 1073, 829, 3277, 1108 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Arapaima, ((new int[]{ 0, 204, 141, 0, 131, 96 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 1309, 316, 384, 1369, 356 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.VampireBat, ((new int[]{ 0, 548, 212, 0, 580, 248 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2226, 297, 117, 2246, 313 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Vampire, ((new int[]{ 0, 211, 187, 0, 287, 205 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3616, 316, 746, 3671, 337 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.ZombieEskimo, ((new int[]{ 0, 2015, 230, 53, 2093, 261 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3397, 311, 161, 3315, 328 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Frankenstein, ((new int[]{ 0, 175, 182, 0, 137, 204 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3622, 313, 746, 3682, 337 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.BlackRecluse, ((new int[]{ 0, 2105, 435, 0, 2021, 436 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3583, 995, 821, 3663, 984 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.WallCreeper, ((new int[]{ 0, 2105, 432, 0, 2020, 434 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3583, 1009, 481, 3666, 979 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.WallCreeperWall, ((new int[]{ 0, 2405, 524, 0, 2320, 558 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3304, 984, 181, 3275, 1022 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.SwampThing, ((new int[]{ 0, 190, 187, 0, 188, 204 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3620, 316, 746, 3685, 336 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.UndeadViking, ((new int[]{ 0, 2880, 380, 147, 2807, 364 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3443, 806, 368, 3383, 845 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.IceElemental, ((new int[]{ 0, 2015, 169, 0, 2093, 139 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3476, 726, 192, 3471, 729 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.PigronCorruption, ((new int[]{ 0, 2880, 378, 1, 2810, 388 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3153, 823, 368, 3233, 862 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.RuneWizard, ((new int[]{ 0, 1243, 825, 1, 1188, 825 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3304, 1045, 167, 3373, 1007 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.AngryTrapper, ((new int[]{ 0, 288, 144, 0, 336, 101 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 1366, 1045, 384, 1439, 1005 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.MossHornet, ((new int[]{ 0, 786, 300, 60, 745, 341 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 1210, 1049, 60, 1161, 1010 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Derpling, ((new int[]{ 0, 201, 122, 0, 131, 79 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 1309, 312, 384, 1381, 345 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.SpikedIceSlime, ((new int[]{ 0, 2713, 399, 0, 2634, 411 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3443, 802, 831, 3409, 810 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.SnowFlinx, ((new int[]{ 0, 2713, 388, 0, 2634, 411 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3443, 813, 831, 3520, 842 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.PincushionZombie, ((new int[]{ 0, 165, 163, 0, 238, 204 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3758, 393, 746, 3836, 401 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.SlimedZombie, ((new int[]{ 0, 148, 181, 0, 176, 204 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 4111, 395, 746, 4179, 432 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.SwampZombie, ((new int[]{ 0, 148, 190, 0, 169, 204 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3760, 375, 728, 3829, 337 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.TwiggyZombie, ((new int[]{ 0, 100, 162, 0, 175, 204 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3742, 390, 740, 3815, 393 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.CataractEye, ((new int[]{ 0, 85, 184, 0, 74, 205 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3837, 424, 727, 3911, 383 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.SleepyEye, ((new int[]{ 0, 149, 188, 0, 177, 204 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3639, 375, 740, 3709, 336 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.DialatedEye, ((new int[]{ 0, 117, 167, 0, 169, 206 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3632, 389, 727, 3709, 431 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.GreenEye, ((new int[]{ 0, 79, 163, 0, 66, 204 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3632, 313, 727, 3701, 337 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.PurpleEye, ((new int[]{ 0, 222, 194, 0, 288, 205 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3862, 389, 727, 3911, 431 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.LostGirl, ((new int[]{ 0, 1088, 429, 0, 1144, 469 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 1048, 368, 3659, 1007 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.ArmoredViking, ((new int[]{ 0, 2713, 389, 0, 2637, 361 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3443, 922, 831, 3522, 962 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.FemaleZombie, ((new int[]{ 0, 83, 168, 0, 66, 204 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3837, 448, 728, 3911, 483 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.HeadacheSkeleton, ((new int[]{ 0, 140, 390, 0, 117, 425 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 1047, 367, 3662, 1010 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.MisassembledSkeleton, ((new int[]{ 0, 183, 391, 0, 250, 427 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 1043, 481, 3802, 1010 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.PantlessSkeleton, ((new int[]{ 0, 1088, 385, 0, 1086, 427 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 1049, 1343, 3784, 1033 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.SpikedJungleSlime, ((new int[]{ 0, 273, 359, 60, 344, 383 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 1252, 1050, 60, 1230, 1005 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Moth, ((new int[]{ 0, 985, 1042, 60, 966, 996 }, new bool[]{ false, false, false, false, false, false, false, false, true, false, false, false }), (new int[]{ 0, 985, 1042, 60, 966, 996 }, new bool[]{ false, false, false, false, false, false, false, false, true, false, false, false })) },
			{ NPCID.IcyMerman, ((new int[]{ 0, 2713, 336, 0, 2633, 362 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3443, 891, 1345, 3524, 907 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.PirateDeckhand, ((new int[]{ 0, 1012, 96, 0, 978, 60 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3901, 306, 679, 3929, 327 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.PirateCorsair, ((new int[]{ 0, 1022, 98, 0, 1087, 60 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3901, 305, 679, 3929, 327 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.PirateDeadeye, ((new int[]{ 0, 1020, 69, 0, 984, 60 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3901, 305, 679, 3929, 327 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.PirateCrossbower, ((new int[]{ 0, 1925, 264, 0, 1840, 284 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3901, 303, 679, 3929, 313 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.PirateCaptain, ((new int[]{ 0, 1063, 69, 2, 981, 64 }, new bool[]{ false, false, true, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2192, 302, 183, 2121, 304 }, new bool[]{ false, false, true, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.CochinealBeetle, ((new int[]{ 0, 186, 404, 0, 122, 428 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 1054, 762, 3813, 1010 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.CyanBeetle, ((new int[]{ 0, 2880, 406, 1, 2801, 436 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3304, 776, 368, 3385, 813 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.LacBeetle, ((new int[]{ 0, 272, 303, 60, 346, 340 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 1232, 1044, 60, 1161, 1001 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.SeaSnail, ((new int[]{ 0, 41, 173, 53, 11, 209 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 427, 375, 53, 346, 379 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ NPCID.Squid, ((new int[]{ 0, 41, 179, 53, 13, 208 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 309, 375, 53, 334, 380 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ NPCID.ZombieRaincoat, ((new int[]{ 0, 1951, 240, 0, 1906, 229 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3762, 288, 712, 3828, 328 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.FlyingFish, ((new int[]{ 0, 74, 86, 0, 64, 44 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3609, 382, 740, 3677, 419 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.UmbrellaSlime, ((new int[]{ 0, 225, 141, 0, 308, 106 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3478, 404, 821, 3522, 445 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.HornetFatty, ((new int[]{ 0, 287, 356, 60, 348, 388 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 843, 1042, 60, 805, 1002 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.HornetHoney, ((new int[]{ 0, 309, 353, 60, 348, 391 }, new bool[]{ false, false, false, false, false, false, false, false, true, false, false, false }), (new int[]{ 0, 309, 353, 60, 348, 391 }, new bool[]{ false, false, false, false, false, false, false, false, true, false, false, false })) },
			{ NPCID.HornetLeafy, ((new int[]{ 0, 309, 360, 60, 342, 398 }, new bool[]{ false, false, false, false, false, false, false, false, true, false, false, false }), (new int[]{ 0, 309, 360, 60, 342, 398 }, new bool[]{ false, false, false, false, false, false, false, false, true, false, false, false })) },
			{ NPCID.HornetSpikey, ((new int[]{ 0, 275, 356, 60, 344, 383 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 857, 1038, 60, 775, 994 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.HornetStingy, ((new int[]{ 0, 278, 356, 60, 357, 383 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 287, 361, 60, 360, 384 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.JungleCreeper, ((new int[]{ 0, 702, 311, 60, 746, 349 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 1024, 1049, 60, 1064, 1008 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.BlackRecluseWall, ((new int[]{ 0, 2266, 454, 0, 2187, 425 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3443, 987, 368, 3383, 1029 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.IceGolem, ((new int[]{ 0, 3091, 282, 147, 3015, 275 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3091, 282, 147, 3015, 275 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.RainbowSlime, ((new int[]{ 0, 1343, 224, 109, 1424, 247 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 1679, 310, 117, 1638, 274 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.AngryNimbus, ((new int[]{ 0, 79, 97, 0, 105, 56 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3444, 311, 740, 3511, 327 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Eyezor, ((new int[]{ 0, 761, 224, 0, 692, 247 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3606, 313, 384, 3551, 327 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Parrot, ((new int[]{ 0, 1004, 95, 0, 1085, 70 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3901, 305, 679, 3929, 327 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Reaper, ((new int[]{ 0, 209, 182, 0, 242, 204 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3640, 313, 746, 3605, 337 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.FungoFish, ((new int[]{ 0, 2486, 627, 70, 2455, 585 }, new bool[]{ false, false, false, false, false, false, false, false, true, false, false, true }), (new int[]{ 0, 3304, 810, 70, 3303, 848 }, new bool[]{ false, false, false, false, false, false, false, false, true, false, false, true })) },
			{ NPCID.AnomuraFungus, ((new int[]{ 0, 2405, 565, 1, 2436, 572 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 977, 182, 3737, 943 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.MushiLadybug, ((new int[]{ 0, 2405, 479, 0, 2330, 518 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 913, 70, 3728, 874 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.FungiBulb, ((new int[]{ 0, 2405, 613, 70, 2456, 574 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3443, 837, 70, 3359, 874 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.GiantFungiBulb, ((new int[]{ 0, 2405, 522, 70, 2484, 561 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 891, 70, 3724, 874 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.FungiSpore, ((new int[]{ 0, 2405, 424, 1, 2336, 426 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 996, 734, 3810, 998 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.AngryBonesBig, ((new int[]{ 0, 3443, 459, 30, 3506, 457 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 961, 182, 3811, 971 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.AngryBonesBigMuscle, ((new int[]{ 0, 3443, 457, 1, 3362, 457 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 974, 182, 3814, 971 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.AngryBonesBigHelmet, ((new int[]{ 0, 3443, 462, 1, 3358, 440 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 978, 161, 3811, 969 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.BirdBlue, ((new int[]{ 0, 1530, 231, 2, 1607, 261 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3628, 348, 147, 3555, 327 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.BirdRed, ((new int[]{ 0, 1136, 227, 2, 1177, 261 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3562, 343, 147, 3589, 320 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Squirrel, ((new int[]{ 0, 1143, 246, 2, 1213, 267 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3596, 314, 191, 3525, 312 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Mouse, ((new int[]{ 0, 54, 302, 0, 131, 338 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 1036, 1343, 3813, 989 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.WebbedStylist, ((new int[]{ 0, 2105, 447, 0, 2038, 481 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3583, 972, 381, 3605, 951 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Firefly, ((new int[]{ 0, 1696, 247, 2, 1619, 261 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3512, 312, 2, 3450, 315 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Butterfly, ((new int[]{ 0, 1993, 281, 2, 1926, 282 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2543, 290, 2, 2476, 302 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Worm, ((new int[]{ 0, 60, 287, 0, 45, 286 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 1034, 1343, 3814, 1073 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Snail, ((new int[]{ 0, 62, 298, 0, 110, 338 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 848, 831, 3815, 810 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.GlowingSnail, ((new int[]{ 0, 2405, 608, 70, 2482, 581 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3443, 914, 70, 3359, 892 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Frog, ((new int[]{ 0, 134, 181, 0, 106, 204 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 1279, 378, 495, 1325, 336 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Duck, ((new int[]{ 0, 264, 179, 1, 343, 214 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 3492, 300, 161, 3433, 333 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ NPCID.DuckWhite, ((new int[]{ 0, 441, 216, 1, 363, 234 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 3500, 319, 161, 3429, 335 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ NPCID.ScorpionBlack, ((new int[]{ 0, 1665, 235, 53, 1711, 254 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2987, 331, 53, 2916, 364 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Scorpion, ((new int[]{ 0, 1674, 238, 53, 1744, 255 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2988, 320, 53, 2915, 310 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Angler, ((new int[]{ 0, 3812, 258, 712, 3889, 272 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 3812, 258, 712, 3889, 272 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ NPCID.TruffleWorm, ((new int[]{ 0, 2405, 520, 70, 2442, 561 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 944, 70, 3735, 903 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Grasshopper, ((new int[]{ 0, 1380, 241, 0, 1452, 247 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 483, 734, 3653, 518 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.ArmedZombie, ((new int[]{ 0, 87, 172, 0, 66, 204 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 444, 728, 3813, 430 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.ArmedZombieEskimo, ((new int[]{ 0, 2042, 232, 147, 2093, 261 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3391, 305, 161, 3313, 327 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.ArmedZombiePincussion, ((new int[]{ 0, 41, 165, 0, 118, 204 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3632, 361, 740, 3708, 369 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.ArmedZombieSlimed, ((new int[]{ 0, 148, 182, 0, 118, 204 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3619, 375, 746, 3694, 336 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.ArmedZombieSwamp, ((new int[]{ 0, 228, 189, 0, 257, 204 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3901, 384, 740, 3929, 411 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.ArmedZombieTwiggy, ((new int[]{ 0, 202, 164, 0, 272, 204 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3628, 368, 728, 3698, 334 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.ArmedZombieCenx, ((new int[]{ 0, 198, 162, 0, 249, 204 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 4111, 362, 728, 4179, 335 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.GoldBird, ((new int[]{ 0, 2027, 286, 2, 1999, 286 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2081, 286, 2, 2099, 286 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.GoldBunny, ((new int[]{ 0, 1773, 305, 2, 1694, 310 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 1773, 305, 2, 1694, 310 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.GoldFrog, ((new int[]{ 0, 1024, 312, 60, 946, 287 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 1024, 312, 60, 957, 294 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.GoldMouse, ((new int[]{ 0, 2266, 918, 1, 2183, 899 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2486, 1027, 111, 2558, 984 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.GoldWorm, ((new int[]{ 0, 2433, 303, 1, 2392, 338 }, new bool[]{ false, false, false, false, false, false, false, false, true, false, false, false }), (new int[]{ 0, 2486, 757, 1, 2408, 753 }, new bool[]{ false, false, false, false, false, false, false, false, true, false, false, false })) },
			{ NPCID.BoneThrowingSkeleton, ((new int[]{ 0, 1342, 387, 0, 1303, 426 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3443, 1035, 368, 3496, 1008 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.BoneThrowingSkeleton4, ((new int[]{ 0, 225, 395, 0, 297, 434 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 1053, 831, 3659, 1010 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.SkeletonMerchant, ((new int[]{ 0, 182, 403, 0, 132, 430 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 1052, 762, 3806, 1010 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.CreatureFromTheDeep, ((new int[]{ 0, 166, 182, 0, 94, 203 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3830, 315, 746, 3757, 337 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Fritz, ((new int[]{ 0, 114, 185, 0, 67, 203 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3614, 315, 746, 3680, 336 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.ThePossessed, ((new int[]{ 0, 189, 185, 0, 183, 204 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3834, 315, 728, 3913, 337 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.GoblinSummoner, ((new int[]{ 0, 1745, 273, 0, 1676, 275 }, new bool[]{ false, false, true, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2941, 308, 147, 3014, 309 }, new bool[]{ false, false, true, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.BigMimicCorruption, ((new int[]{ 0, 2486, 366, 1, 2419, 358 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3153, 1059, 1345, 3234, 1099 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.BigMimicHallow, ((new int[]{ 0, 1761, 1019, 19, 1777, 1052 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2046, 1050, 76, 2118, 1083 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Medusa, ((new int[]{ 0, 1651, 770, 0, 1688, 791 }, new bool[]{ false, false, false, false, true, false, false, false, false, false, false, false }), (new int[]{ 0, 2880, 1035, 367, 2946, 1006 }, new bool[]{ false, false, false, false, true, false, false, false, false, false, false, false })) },
			{ NPCID.GreekSkeleton, ((new int[]{ 0, 1719, 725, 0, 1691, 743 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2880, 1069, 367, 2951, 1054 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.GraniteGolem, ((new int[]{ 0, 2714, 600, 0, 2651, 572 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 977, 481, 3813, 1000 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.GraniteFlyer, ((new int[]{ 0, 2880, 588, 0, 2803, 630 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 940, 481, 3763, 974 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Grubby, ((new int[]{ 0, 460, 195, 1, 378, 208 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2987, 1081, 76, 2945, 1041 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Sluggy, ((new int[]{ 0, 519, 195, 53, 492, 226 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 1957, 303, 60, 1880, 340 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Buggy, ((new int[]{ 0, 437, 199, 60, 492, 226 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 1221, 291, 60, 1148, 306 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.BloodZombie, ((new int[]{ 0, 41, 157, 0, 68, 113 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3837, 380, 712, 3911, 387 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Drippler, ((new int[]{ 0, 79, 163, 0, 1, 204 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3735, 378, 712, 3709, 337 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.PirateShip, ((new int[]{ 0, 1938, 267, 0, 2015, 285 }, new bool[]{ false, false, true, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2196, 296, 2, 2115, 286 }, new bool[]{ false, false, true, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.PirateShipCannon, ((new int[]{ 0, 1938, 267, 0, 2015, 285 }, new bool[]{ false, false, true, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2196, 296, 2, 2115, 286 }, new bool[]{ false, false, true, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.GiantShelly, ((new int[]{ 0, 210, 400, 0, 170, 442 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 1048, 1370, 3659, 1010 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.GiantShelly2, ((new int[]{ 0, 248, 404, 0, 170, 434 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 1045, 830, 3814, 1010 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Salamander5, ((new int[]{ 0, 1362, 409, 0, 1311, 424 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3304, 1003, 368, 3262, 1008 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Salamander8, ((new int[]{ 0, 140, 394, 0, 129, 424 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3583, 1047, 762, 3661, 1008 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Salamander9, ((new int[]{ 0, 1075, 412, 0, 1130, 429 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3443, 1045, 182, 3508, 1007 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.GiantWalkingAntlion, ((new int[]{ 0, 1827, 445, 396, 1756, 412 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 1827, 544, 484, 1906, 546 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.GiantFlyingAntlion, ((new int[]{ 0, 1827, 448, 396, 1767, 488 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 1827, 546, 484, 1896, 586 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.DuneSplicerHead, ((new int[]{ 0, 1674, 230, 53, 1734, 255 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2833, 312, 53, 2815, 312 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.DuneSplicerBody, ((new int[]{ 0, 1674, 230, 53, 1734, 255 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2833, 312, 53, 2815, 312 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.DuneSplicerTail, ((new int[]{ 0, 1674, 230, 53, 1734, 255 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2833, 312, 53, 2815, 312 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.TombCrawlerHead, ((new int[]{ 0, 1827, 487, 396, 1765, 526 }, new bool[]{ false, false, false, false, false, false, false, false, true, false, false, false }), (new int[]{ 0, 1827, 487, 396, 1765, 526 }, new bool[]{ false, false, false, false, false, false, false, false, true, false, false, false })) },
			{ NPCID.TombCrawlerBody, ((new int[]{ 0, 1827, 487, 396, 1765, 526 }, new bool[]{ false, false, false, false, false, false, false, false, true, false, false, false }), (new int[]{ 0, 1827, 487, 396, 1765, 526 }, new bool[]{ false, false, false, false, false, false, false, false, true, false, false, false })) },
			{ NPCID.TombCrawlerTail, ((new int[]{ 0, 1827, 487, 396, 1765, 526 }, new bool[]{ false, false, false, false, false, false, false, false, true, false, false, false }), (new int[]{ 0, 1827, 487, 396, 1765, 526 }, new bool[]{ false, false, false, false, false, false, false, false, true, false, false, false })) },
			{ NPCID.DemonTaxCollector, ((new int[]{ 0, 93, 976, 1, 71, 1011 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3304, 1072, 829, 3287, 1089 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.TheBride, ((new int[]{ 0, 970, 235, 0, 1045, 264 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3578, 310, 147, 3500, 321 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.SandSlime, ((new int[]{ 0, 1827, 450, 396, 1747, 411 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 1827, 533, 484, 1904, 566 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.SquirrelRed, ((new int[]{ 0, 1560, 244, 2, 1636, 269 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3605, 348, 191, 3586, 315 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.SandElemental, ((new int[]{ 0, 1674, 230, 53, 1751, 266 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2818, 310, 53, 2805, 307 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.SandShark, ((new int[]{ 0, 1583, 185, 0, 1507, 144 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3038, 313, 746, 3112, 313 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.SandsharkHallow, ((new int[]{ 0, 1674, 308, 116, 1684, 346 }, new bool[]{ false, false, false, false, false, false, false, false, true, false, false, true }), (new int[]{ 0, 1684, 311, 116, 1686, 349 }, new bool[]{ false, false, false, false, false, false, false, false, true, false, false, true })) },
			{ NPCID.Tumbleweed, ((new int[]{ 0, 1640, 165, 0, 1583, 121 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2922, 477, 831, 2962, 508 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.DD2EterniaCrystal, ((new int[]{ 0, 2017, 286, 0, 2088, 286 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2019, 286, 2, 2090, 286 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.DD2LanePortal, ((new int[]{ 0, 2017, 286, 0, 2088, 286 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2019, 286, 2, 2090, 286 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.DD2GoblinT1, ((new int[]{ 0, 1944, 262, 0, 1869, 284 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2105, 289, 167, 2166, 328 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.DD2GoblinT2, ((new int[]{ 0, 1951, 264, 0, 1879, 284 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2086, 289, 53, 2162, 327 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.DD2GoblinBomberT1, ((new int[]{ 0, 1944, 262, 0, 1882, 284 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2090, 289, 167, 2152, 328 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.DD2GoblinBomberT2, ((new int[]{ 0, 1958, 264, 0, 1881, 285 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2083, 286, 53, 2162, 327 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.DD2WyvernT1, ((new int[]{ 0, 1952, 270, 0, 1878, 284 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2074, 286, 53, 2154, 327 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.DD2WyvernT2, ((new int[]{ 0, 1959, 274, 0, 1882, 285 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2087, 286, 53, 2141, 327 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.DD2JavelinstT1, ((new int[]{ 0, 1956, 263, 0, 1873, 284 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2105, 286, 167, 2154, 328 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.DD2JavelinstT2, ((new int[]{ 0, 1956, 263, 0, 1887, 285 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2059, 286, 53, 2109, 306 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.DD2DarkMageT1, ((new int[]{ 0, 1957, 286, 0, 1908, 304 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 1990, 286, 2, 2014, 325 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.DD2SkeletonT1, ((new int[]{ 0, 2036, 286, 0, 1970, 286 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2036, 286, 0, 1970, 286 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.DD2WitherBeastT2, ((new int[]{ 0, 1969, 286, 0, 1899, 284 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2074, 286, 53, 2155, 313 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.DD2DrakinT2, ((new int[]{ 0, 1951, 267, 0, 1871, 284 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2074, 289, 53, 2142, 327 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.DD2KoboldWalkerT2, ((new int[]{ 0, 1953, 263, 0, 1876, 284 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2087, 289, 53, 2142, 327 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.DD2KoboldFlyerT2, ((new int[]{ 0, 1967, 286, 0, 1899, 285 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2073, 286, 53, 2127, 306 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.DD2OgreT2, ((new int[]{ 0, 1961, 286, 0, 1884, 286 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 1976, 286, 53, 2036, 306 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.BartenderUnconscious, ((new int[]{ 0, 79, 132, 0, 106, 141 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 1069, 829, 3798, 1086 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.WalkingAntlion, ((new int[]{ 0, 1571, 196, 0, 1487, 150 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2914, 549, 746, 2933, 586 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.FlyingAntlion, ((new int[]{ 0, 1674, 230, 2, 1684, 257 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2907, 548, 746, 2839, 589 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.LarvaeAntlion, ((new int[]{ 0, 1827, 448, 396, 1755, 488 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 1827, 493, 484, 1785, 523 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.FairyCritterPink, ((new int[]{ 0, 309, 353, 0, 382, 382 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 891, 162, 3810, 886 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.FairyCritterGreen, ((new int[]{ 0, 1443, 369, 1, 1469, 409 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3443, 897, 223, 3490, 879 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.FairyCritterBlue, ((new int[]{ 0, 2063, 267, 1, 1993, 286 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3443, 817, 223, 3360, 826 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.GolferRescue, ((new int[]{ 0, 1827, 443, 396, 1894, 471 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 1827, 443, 396, 1894, 471 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.TorchZombie, ((new int[]{ 0, 280, 162, 0, 254, 204 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 4111, 374, 746, 4179, 337 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.ArmedTorchZombie, ((new int[]{ 0, 197, 168, 0, 258, 204 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3837, 369, 712, 3911, 337 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.GoldGoldfish, ((new int[]{ 0, 3304, 511, 161, 3228, 523 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 3304, 511, 161, 3228, 523 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ NPCID.BlueDragonfly, ((new int[]{ 0, 1782, 304, 2, 1709, 310 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 1782, 304, 2, 1709, 310 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.RedDragonfly, ((new int[]{ 0, 1782, 304, 2, 1709, 310 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 1782, 304, 2, 1709, 310 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Seagull, ((new int[]{ 0, 52, 163, 53, 111, 204 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 396, 264, 53, 342, 244 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.LadyBug, ((new int[]{ 0, 1638, 269, 2, 1695, 282 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3522, 286, 2, 3593, 310 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Pupfish, ((new int[]{ 0, 1004, 229, 53, 1079, 259 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 2893, 311, 53, 2843, 335 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ NPCID.Grebe, ((new int[]{ 0, 1143, 227, 53, 1078, 260 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 2893, 304, 53, 2813, 319 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ NPCID.WaterStrider, ((new int[]{ 0, 309, 199, 53, 368, 212 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 2880, 305, 60, 2808, 268 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ NPCID.Dolphin, ((new int[]{ 0, 41, 183, 53, 112, 210 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 286, 269, 53, 337, 243 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ NPCID.Turtle, ((new int[]{ 0, 283, 200, 2, 365, 235 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 3517, 291, 2, 3434, 323 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ NPCID.TurtleJungle, ((new int[]{ 0, 296, 185, 60, 368, 212 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 1145, 303, 60, 1081, 334 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ NPCID.Gnome, ((new int[]{ 0, 176, 249, 0, 108, 285 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 384, 191, 3782, 353 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.SeaTurtle, ((new int[]{ 0, 41, 180, 53, 110, 208 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 414, 261, 53, 337, 244 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ NPCID.Seahorse, ((new int[]{ 0, 68, 201, 53, 109, 211 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 263, 258, 53, 338, 245 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ NPCID.IceMimic, ((new int[]{ 0, 2880, 385, 0, 2797, 426 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3443, 734, 824, 3510, 721 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.RockGolem, ((new int[]{ 0, 241, 394, 0, 258, 430 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 1044, 182, 3809, 1004 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.SporeBat, ((new int[]{ 0, 2405, 463, 1, 2448, 502 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 916, 734, 3728, 874 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.SporeSkeleton, ((new int[]{ 0, 2470, 476, 1, 2445, 462 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2643, 690, 734, 2699, 696 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.HellButterfly, ((new int[]{ 0, 41, 975, 1, 30, 1011 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3247, 1084, 829, 3273, 1106 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.Lavafly, ((new int[]{ 0, 42, 975, 19, 70, 1013 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3191, 1070, 829, 3238, 1097 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.MagmaSnail, ((new int[]{ 0, 78, 974, 1, 29, 1011 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3304, 1084, 829, 3296, 1102 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.QueenSlimeMinionBlue, ((new int[]{ 0, 1571, 267, 117, 1487, 254 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 1571, 267, 117, 1487, 254 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.QueenSlimeMinionPink, ((new int[]{ 0, 1571, 267, 117, 1487, 254 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 1571, 267, 117, 1487, 254 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.QueenSlimeMinionPurple, ((new int[]{ 0, 1571, 267, 117, 1487, 254 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 1571, 267, 117, 1487, 254 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.EmpressButterfly, ((new int[]{ 0, 1744, 292, 109, 1676, 309 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 1744, 292, 109, 1676, 309 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.PirateGhost, ((new int[]{ 0, 997, 69, 0, 1059, 24 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2161, 285, 179, 2241, 286 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ NPCID.ChaosBallTim, ((new int[]{ 0, 1530, 703, 0, 1446, 685 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3443, 1063, 182, 3521, 1052 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) }
		};
		private static SortedDictionary<string, ((int[], bool[]), (int[], bool[]))> ModNPCSpawnRulesNames = new() {
			{ "Bloatfish", ((new int[]{ 0, 3971, 747, 682, 3941, 707 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 4068, 905, 1295, 4107, 924 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ "Bobbit Worm", ((new int[]{ 0, 3959, 748, 682, 3939, 707 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 4071, 909, 1295, 4109, 924 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ "Bobbit Worm", ((new int[]{ 0, 3959, 748, 682, 3939, 707 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 4071, 909, 1295, 4109, 924 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ "Box Jellyfish", ((new int[]{ 0, 41, 176, 53, 5, 208 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 4098, 412, 1350, 4120, 448 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ "Chaotic Puffer", ((new int[]{ 0, 3937, 597, 675, 3935, 568 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 4070, 743, 718, 4109, 766 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ "Colossal Squid", ((new int[]{ 0, 3952, 602, 675, 3953, 571 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 4071, 918, 1295, 4106, 914 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ "Cuttlefish", ((new int[]{ 0, 3945, 418, 675, 3937, 377 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 4104, 592, 714, 4146, 634 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ "Devil Fish", ((new int[]{ 0, 3938, 550, 675, 3943, 570 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 4072, 847, 1350, 4106, 885 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ "Devil Fish", ((new int[]{ 0, 3937, 607, 675, 3935, 568 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 4068, 750, 718, 4111, 767 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ "Eidolon Wyrm", ((new int[]{ 0, 3977, 746, 682, 3939, 707 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 4064, 906, 718, 4104, 924 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ "Eidolon Wyrm", ((new int[]{ 0, 3977, 746, 682, 3939, 707 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 4064, 906, 718, 4104, 924 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ "Eidolon Wyrm", ((new int[]{ 0, 3977, 746, 682, 3939, 707 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 4064, 906, 718, 4104, 924 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ "Eidolon Wyrm", ((new int[]{ 0, 3977, 746, 682, 3939, 707 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 4064, 906, 718, 4080, 924 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ "Giant Squid", ((new int[]{ 0, 3937, 548, 675, 3932, 514 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 4078, 801, 1350, 4113, 839 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ "Gulper Eel", ((new int[]{ 0, 3947, 595, 675, 3933, 588 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 4062, 908, 1295, 4088, 924 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ "Gulper Eel", ((new int[]{ 0, 3947, 595, 675, 3933, 588 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 4062, 908, 1295, 4088, 924 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ "Gulper Eel", ((new int[]{ 0, 3947, 595, 675, 3933, 588 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 4062, 908, 1295, 4088, 924 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ "Gulper Eel", ((new int[]{ 0, 3947, 595, 675, 3933, 588 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 4062, 908, 1295, 4088, 924 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ "Laserfish", ((new int[]{ 0, 3937, 324, 675, 3933, 336 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 4099, 750, 1350, 4132, 767 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ "Luminous Corvina", ((new int[]{ 0, 3945, 387, 675, 3935, 356 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 4101, 604, 1350, 4123, 642 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ "Mirage Jelly", ((new int[]{ 0, 3940, 493, 675, 3937, 535 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 4078, 739, 1350, 4105, 767 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ "Moray Eel", ((new int[]{ 0, 41, 170, 53, 10, 204 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 4050, 412, 675, 4069, 450 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Oarfish", ((new int[]{ 0, 3943, 420, 675, 3948, 377 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 4081, 726, 718, 4116, 765 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ "Oarfish", ((new int[]{ 0, 3943, 420, 675, 3948, 377 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 4081, 726, 718, 4116, 765 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ "Oarfish", ((new int[]{ 0, 3943, 420, 675, 3948, 377 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 4081, 726, 718, 4116, 765 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ "Reaper Shark", ((new int[]{ 0, 3955, 753, 682, 3951, 714 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 4059, 918, 718, 4073, 924 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ "Toxic Minnow", ((new int[]{ 0, 3942, 301, 675, 3934, 336 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 4105, 607, 1350, 4141, 634 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ "Viperfish", ((new int[]{ 0, 3937, 319, 675, 3935, 319 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 4101, 779, 1350, 4127, 821 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ "Acid Eel", ((new int[]{ 0, 3671, 218, 675, 3722, 259 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 4157, 396, 830, 4198, 435 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Baby Flak Crab", ((new int[]{ 0, 3627, 44, 0, 3548, 23 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 4157, 393, 829, 4198, 434 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Cragmaw Mire", ((new int[]{ 0, 3815, 256, 675, 3876, 268 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 4066, 383, 713, 4120, 420 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ "Flak Crab", ((new int[]{ 0, 3633, 160, 0, 3613, 111 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 4135, 498, 831, 4198, 483 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Irradiated Slime", ((new int[]{ 0, 3910, 244, 679, 3838, 269 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 4104, 307, 712, 4180, 325 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ "Nuclear Toad", ((new int[]{ 0, 3640, 227, 2, 3577, 241 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 4157, 441, 714, 4197, 483 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Orthocera", ((new int[]{ 0, 3736, 226, 0, 3786, 267 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 4121, 457, 829, 4192, 492 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ "Radiator", ((new int[]{ 0, 3671, 218, 41, 3636, 242 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 4157, 522, 830, 4192, 481 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Skyfin", ((new int[]{ 0, 3337, 223, 2, 3390, 250 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 4157, 396, 831, 4197, 435 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Sulphurous Skater", ((new int[]{ 0, 3570, 229, 1, 3562, 239 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 4121, 388, 714, 4194, 426 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Trilobite", ((new int[]{ 0, 3747, 228, 53, 3783, 267 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 4123, 528, 831, 4188, 546 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Aquatic Scourge", ((new int[]{ 0, 3761, 252, 679, 3828, 267 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 4157, 264, 713, 4163, 306 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ "Aquatic Scourge", ((new int[]{ 0, 3761, 252, 679, 3828, 267 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 4157, 306, 713, 4163, 306 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ "Aquatic Scourge", ((new int[]{ 0, 3761, 252, 679, 3828, 268 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 4157, 306, 713, 4163, 306 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ "Aquatic Scourge", ((new int[]{ 0, 3761, 252, 679, 3828, 267 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 4157, 264, 713, 4163, 306 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ "Aries", ((new int[]{ 0, 79, 165, 0, 105, 116 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2921, 481, 1343, 2885, 505 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Astralachnea", ((new int[]{ 0, 2405, 339, 0, 2320, 302 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2714, 717, 1343, 2796, 696 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Astralachnea", ((new int[]{ 0, 2405, 297, 0, 2411, 271 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2714, 701, 746, 2779, 743 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Astral Probe", ((new int[]{ 0, 2317, 154, 0, 2240, 108 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2837, 1023, 1343, 2900, 1065 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Seeker Spit", ((new int[]{ 0, 2264, 93, 0, 2185, 71 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2851, 422, 1343, 2932, 434 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Astral Slime", ((new int[]{ 0, 2210, 165, 0, 2131, 116 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2797, 481, 746, 2859, 505 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Atlas", ((new int[]{ 0, 2316, 228, 0, 2250, 239 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2722, 339, 1343, 2795, 378 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Big Sightseer", ((new int[]{ 0, 2266, 150, 0, 2185, 107 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2829, 465, 1343, 2859, 507 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Fusion Feeder", ((new int[]{ 0, 2344, 161, 0, 2261, 117 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2833, 1005, 1343, 2905, 966 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Hadarian", ((new int[]{ 0, 2344, 165, 0, 2261, 116 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2797, 530, 1343, 2855, 550 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Hive", ((new int[]{ 0, 2405, 224, 0, 2322, 262 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2714, 708, 746, 2778, 664 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Hiveling", ((new int[]{ 0, 2265, 286, 1, 2184, 265 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2714, 665, 746, 2785, 621 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Mantis", ((new int[]{ 0, 2127, 160, 0, 2184, 118 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2811, 455, 1343, 2859, 465 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Nova", ((new int[]{ 0, 2120, 45, 0, 2195, 16 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2829, 463, 1343, 2905, 500 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Small Sightseer", ((new int[]{ 0, 2266, 46, 0, 2195, 70 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2809, 481, 1343, 2855, 520 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Stellar Culex", ((new int[]{ 0, 2266, 224, 0, 2183, 257 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2714, 717, 1343, 2796, 743 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Cataclysm", ((new int[]{ 0, 2042, 286, 2, 1957, 286 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2042, 286, 2, 1957, 286 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Catastrophe", ((new int[]{ 0, 2042, 286, 2, 1957, 286 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2042, 286, 2, 1957, 286 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Soul Seeker", ((new int[]{ 0, 2042, 286, 2, 1957, 286 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2042, 286, 2, 1957, 286 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Calamity Eye", ((new int[]{ 0, 3249, 879, 0, 3187, 863 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 4157, 1112, 774, 4188, 1111 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Charred Slime", ((new int[]{ 0, 3304, 965, 0, 3221, 970 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 1099, 774, 3810, 1111 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Cultist Assassin", ((new int[]{ 0, 3263, 501, 0, 3187, 504 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 4154, 1106, 774, 4180, 1111 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Despair Stone", ((new int[]{ 0, 3018, 942, 0, 2939, 897 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 4157, 1112, 774, 4180, 1111 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Heat Spirit", ((new int[]{ 0, 3021, 612, 0, 2956, 643 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 4157, 1112, 774, 4194, 1111 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Scryllar", ((new int[]{ 0, 3153, 837, 0, 3071, 804 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 4155, 1112, 774, 4182, 1111 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Scryllar", ((new int[]{ 0, 3018, 945, 0, 2937, 943 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 4090, 1112, 774, 4101, 1111 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Soul Slurper", ((new int[]{ 0, 2917, 907, 0, 2927, 865 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 4157, 1112, 774, 4105, 1111 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Cryogen's Shield", ((new int[]{ 0, 3038, 272, 147, 3112, 301 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 3038, 272, 147, 3112, 301 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ "Androomba", ((new int[]{ 0, 359, 92, 0, 329, 51 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3153, 1064, 1370, 3223, 1071 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Nanodroid", ((new int[]{ 0, 344, 85, 0, 268, 43 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3153, 1067, 1373, 3236, 1074 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Nanodroid", ((new int[]{ 0, 343, 74, 0, 269, 48 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3153, 1068, 1373, 3235, 1076 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Repair Unit", ((new int[]{ 0, 297, 74, 0, 226, 51 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3153, 1067, 1373, 3236, 1074 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Hive Blob", ((new int[]{ 0, 1367, 245, 23, 1299, 247 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3018, 820, 368, 3090, 827 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Hive Blob", ((new int[]{ 0, 1314, 248, 23, 1384, 255 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3018, 787, 368, 3025, 827 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Hive Tumor", ((new int[]{ 0, 1162, 221, 23, 1225, 247 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3304, 1004, 163, 3229, 978 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "???", ((new int[]{ 0, 116, 174, 53, 95, 208 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 315, 344, 53, 233, 361 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ "Aero Slime", ((new int[]{ 0, 41, 44, 0, 5, 4 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 4157, 385, 740, 4199, 347 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Aurora Spirit", ((new int[]{ 0, 2702, 64, 0, 2636, 73 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3308, 334, 746, 3382, 354 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Cnidrion", ((new int[]{ 0, 1674, 231, 0, 1695, 254 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 2928, 309, 161, 3001, 341 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Cosmic Elemental", ((new int[]{ 0, 283, 381, 0, 252, 422 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 1047, 830, 3813, 1006 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Amber Crawler", ((new int[]{ 0, 1827, 504, 396, 1751, 520 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 1827, 530, 484, 1891, 524 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Amethyst Crawler", ((new int[]{ 0, 132, 297, 0, 47, 338 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3981, 468, 1343, 4054, 421 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Diamond Crawler", ((new int[]{ 0, 183, 343, 0, 155, 337 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 1049, 1343, 3815, 1009 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Emerald Crawler", ((new int[]{ 0, 184, 345, 0, 122, 341 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 1053, 1343, 3813, 1010 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Ruby Crawler", ((new int[]{ 0, 121, 383, 0, 119, 423 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 4047, 1052, 831, 4023, 1009 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Sapphire Crawler", ((new int[]{ 0, 309, 301, 0, 390, 336 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 520, 1343, 3813, 550 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Topaz Crawler", ((new int[]{ 0, 85, 302, 0, 51, 338 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3955, 463, 746, 4031, 482 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Cryon", ((new int[]{ 0, 2713, 167, 0, 2629, 123 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3443, 864, 1345, 3522, 900 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Cryo Slime", ((new int[]{ 0, 2880, 398, 0, 2799, 364 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3443, 864, 830, 3525, 900 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Ebonian Blight Slime", ((new int[]{ 0, 1097, 203, 0, 1162, 236 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3312, 1053, 367, 3229, 1065 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Eidolist", ((new int[]{ 0, 3943, 603, 682, 3949, 643 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 4060, 896, 1295, 4097, 921 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ "Fearless Goldfish Warrior", ((new int[]{ 0, 190, 172, 0, 268, 139 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3402, 306, 728, 3481, 333 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Frogfish", ((new int[]{ 0, 41, 170, 53, 5, 204 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 378, 381, 60, 445, 380 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Earth Elemental", ((new int[]{ 0, 1169, 426, 0, 1132, 436 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3732, 1047, 734, 3729, 1009 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Ice Clasper", ((new int[]{ 0, 2874, 244, 0, 2797, 257 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3326, 786, 397, 3389, 811 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Mantis Shrimp", ((new int[]{ 0, 41, 173, 53, 42, 208 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 319, 277, 53, 235, 317 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ "Overloaded Soldier", ((new int[]{ 0, 1206, 300, 0, 1206, 340 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 4050, 530, 746, 4128, 550 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Piggy", ((new int[]{ 0, 2137, 284, 0, 2084, 324 }, new bool[]{ false, false, false, false, false, false, true, true, true, false, false, false }), (new int[]{ 0, 2137, 284, 0, 2084, 324 }, new bool[]{ false, false, false, false, false, false, true, true, true, false, false, false })) },
			{ "Rimehound", ((new int[]{ 0, 2713, 172, 0, 2634, 139 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3443, 811, 726, 3521, 826 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Rotdog", ((new int[]{ 0, 41, 93, 0, 106, 49 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 4111, 421, 746, 4179, 455 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Sea Urchin", ((new int[]{ 0, 41, 167, 53, 3, 205 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 386, 381, 60, 455, 380 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Shockstorm Shuttle", ((new int[]{ 0, 41, 44, 0, 12, 5 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 4157, 257, 740, 4183, 298 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Stormlion", ((new int[]{ 0, 1827, 321, 53, 1742, 356 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 1827, 549, 484, 1908, 571 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Sunskater", ((new int[]{ 0, 51, 44, 0, 20, 5 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 4157, 214, 740, 4175, 242 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Cloud Elemental", ((new int[]{ 0, 185, 44, 0, 134, 7 }, new bool[]{ false, false, false, false, false, false, false, false, false, true, false, false }), (new int[]{ 0, 4154, 197, 0, 4164, 151 }, new bool[]{ false, false, false, false, false, false, false, false, false, true, false, false })) },
			{ "Wulfrum Amplifier", ((new int[]{ 0, 45, 188, 0, 1, 205 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3616, 1047, 746, 3676, 1041 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Wulfrum Drone", ((new int[]{ 0, 123, 72, 0, 116, 85 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3602, 382, 740, 3680, 337 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Wulfrum Gyrator", ((new int[]{ 0, 45, 142, 0, 1, 96 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 4157, 1047, 746, 4082, 1041 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Wulfrum Hovercraft", ((new int[]{ 0, 99, 51, 0, 65, 56 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 4137, 1047, 746, 4142, 1041 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Wulfrum Rover", ((new int[]{ 0, 45, 179, 0, 68, 139 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 3631, 424, 746, 3706, 388 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Crimulan Slime God", ((new int[]{ 0, 1526, 258, 25, 1444, 259 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 1526, 258, 25, 1444, 259 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Ebonian Slime God", ((new int[]{ 0, 1526, 258, 25, 1444, 259 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 1526, 258, 25, 1444, 259 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Crimulan Slime God", ((new int[]{ 0, 1526, 258, 25, 1444, 259 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 1526, 258, 25, 1444, 259 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Ebonian Slime God", ((new int[]{ 0, 1526, 258, 25, 1444, 259 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 1526, 258, 25, 1444, 259 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Anthozoan Crab", ((new int[]{ 0, 3382, 44, 0, 3306, 5 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 4157, 523, 831, 4198, 558 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Aquatic Urchin", ((new int[]{ 0, 3697, 224, 1, 3617, 250 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 4157, 407, 1350, 4198, 444 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Belching Coral", ((new int[]{ 0, 3463, 49, 0, 3493, 12 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 4157, 396, 829, 4197, 434 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Gnasher", ((new int[]{ 0, 3435, 44, 0, 3352, 4 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 4157, 528, 830, 4198, 567 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Microbial Cluster", ((new int[]{ 0, 3649, 226, 0, 3727, 266 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 4157, 396, 831, 4198, 434 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ "Sulflounder", ((new int[]{ 0, 3716, 231, 0, 3724, 266 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 4157, 413, 831, 4198, 434 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Toxicatfish", ((new int[]{ 0, 3649, 160, 0, 3652, 111 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 4157, 407, 831, 4198, 443 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Trasher", ((new int[]{ 0, 3423, 44, 0, 3352, 4 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 4157, 395, 830, 4194, 435 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Baby Ghost Bell", ((new int[]{ 0, 1760, 609, 0, 1677, 624 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 1964, 807, 1373, 2046, 842 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ "Clam", ((new int[]{ 0, 1760, 577, 0, 1677, 617 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 1964, 796, 1373, 1994, 831 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Eutrophic Ray", ((new int[]{ 0, 1760, 606, 0, 1679, 638 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 1964, 796, 1373, 2046, 837 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Ghost Bell", ((new int[]{ 0, 1760, 603, 0, 1679, 633 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 1964, 807, 1373, 1974, 842 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Giant Clam", ((new int[]{ 0, 1827, 669, 1370, 1758, 668 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 1827, 669, 1370, 1758, 668 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ "Prism-Back", ((new int[]{ 0, 1760, 612, 0, 1677, 620 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 1964, 808, 1373, 1969, 842 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Sea Floaty", ((new int[]{ 0, 1760, 574, 0, 1678, 616 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false }), (new int[]{ 0, 1917, 795, 1373, 1923, 835 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, false })) },
			{ "Sea Minnow", ((new int[]{ 0, 1760, 600, 0, 1676, 628 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 1937, 807, 1376, 1955, 843 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) },
			{ "Archmage", ((new int[]{ 0, 3038, 272, 147, 3112, 301 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true }), (new int[]{ 0, 3038, 272, 147, 3112, 301 }, new bool[]{ false, false, false, false, false, false, false, false, false, false, false, true })) }
		};
		public static void PostSetupContent() {
			if (finishedSetup)
				return;

			//Get all dictionaries
			SetupTempDictionaries();

			PopulateCraftingWeaponSources();
			//TODO: Include required tile in crafted item recipes.  Set to crafted from incredients for the tile item
			//TODO: add function to check for item with same name as tile
			//TODO: Include other requiremenets for crafting like with water/honey/lava etc
			//TODO: Allow modded recipies for vanilla weapons to affect their infusion power.   (example slime staff)

			//Fill WeaponSources
			if (guessingInfusionPowers)
				GuessInfusionPowers();

			if (Debugger.IsAttached)
				InfusionProgressionTests.RunTests();

			//Clear all other dictionaries
			ClearTempDictionaries();
		}
		private static void SetupTempDictionaries() {
			SetupWeaponsList();
			SetupReverseCraftableIngredients();
			GetAllCraftingResources();
			SetupItemsFromNPCs();
			SetupItemsFromLootItems();
			SetupProgressionGroups();
		}
		private static void SetupProgressionGroups() {
			SetupMinedOreInfusionPowers();
			CheckNPCSpawns();
			AddProgressionGroup(new(ProgressionGroupID.Forest, 5, npcTypes: new SortedSet<int>() { NPCID.BlueSlime }));
			AddProgressionGroup(new(ProgressionGroupID.Desert, 10));//, Underground: 80));
			AddProgressionGroup(new(ProgressionGroupID.Snow, 35));//, Underground: 30));
			AddProgressionGroup(new(ProgressionGroupID.Ocean, 50));//, Underground: 20));
			AddProgressionGroup(new(ProgressionGroupID.Jungle, 75));//, Underground: 25));
			AddProgressionGroup(new(ProgressionGroupID.Empress, 970));//, Night: -60));

			/*
			foreach (var id in Enum.GetValues(typeof(ProgressionGroupID)).Cast<ProgressionGroupID>()) {
				if (!progressionGroups.ContainsKey(id))
					AddProgressionGroup(new(id));
			}
			*/
		}
		private static void SetupInfusionPowerTiles() {
			infusionPowerTiles = new();
			for (int tileType = 0; tileType < TileID.Count; tileType++) {
				int itemType = WEGlobalTile.GetDroppedItem(tileType, ignoreError: true);
				if (itemType <= 0)
					continue;

				if (OreInfusionPowers.ContainsKey(itemType)) {
					int infusionPower = OreInfusionPowers[itemType];
					if (infusionPowerTiles.ContainsKey(infusionPower))
						continue;

					int requiredPickaxePower = WEGlobalTile.GetRequiredPickaxePower(tileType, true);
					Item item = itemType.CSI();
					infusionPowerTiles.Add(infusionPower, (requiredPickaxePower, item.value));
				}
			}

			//$"\ninfusionPowerTiles:\n{infusionPowerTiles.Select(i => $"infusionPower: {i.Key}, pickPower: {i.Value.pickPower}, value: {i.Value.value}").JoinList("\n")}".LogSimple();
		}
		private static void SetupMinedOreInfusionPowers() {
			for (int itemType = ItemID.Count; itemType < ItemLoader.ItemCount; itemType++) {
				Item item = itemType.CSI();
				string itemName = item.Name;
				if (ModOreInfusionPowers.ContainsKey(itemName))
					OreInfusionPowers.Add(itemType, ModOreInfusionPowers[itemName]);
			}

			SortedDictionary<int, (int tile, Item item)> infusionPowerTiles = new();
			for (int tileType = TileID.Count; tileType < TileLoader.TileCount; tileType++) {
				int itemType = WEGlobalTile.GetDroppedItem(tileType, ignoreError: true);
				if (itemType <= 0)
					continue;

				if (OreInfusionPowers.ContainsKey(itemType))
					continue;

				Item item = itemType.CSI();
				ModTile modTile = TileLoader.GetTile(tileType);
				if (itemType > 0 && modTile != null) {
					bool ore = TileID.Sets.Ore[tileType];
					int requiredPickaxePower = WEGlobalTile.GetRequiredPickaxePower(tileType, true);
					float mineResist = modTile.MineResist;
					float value = item.value;
					if (ore || ((requiredPickaxePower > 0 || mineResist > 1) && value > 0)) {//Try getting rid of mineResist.
						if (!weaponCraftingIngredients.Contains(itemType))
							continue;

						int infusionPower = GetOreInfusionPower(requiredPickaxePower, value);
						OreInfusionPowers.Add(itemType, infusionPower);
						$"Ore {item.S()} infusion power not set up. Guessed infusion power: {infusionPower}".LogNT(ChatMessagesIDs.OreInfusionPowerNotSetup);
					}
				}
			}

			//$"\nOreInfusionPowers\n{OreInfusionPowers.Select(i => $"{i.Key.CSI().S()}: {i.Value}").JoinList("\n")}".LogSimple();

			foreach (KeyValuePair<int, int> pair in OreInfusionPowers) {
				VanillaCraftingItemSourceInfusionPowers.Add(pair.Key, pair.Value);
			}
		}
		private static void CheckNPCSpawns() {
			foreach (var pGroup in progressionGroups) {
				if (!pGroup.Value.TryGetSpawnConditions(out NPCSpawnInfo spawnInfo))
					continue;

				SortedSet<int> npcs = new();
				for (int netID = NPCID.Count; netID < NPCLoader.NPCCount; netID++) {
					ModNPC modNPC = netID.CSNPC().ModNPC;
					if (modNPC != null) {
						float weight = modNPC.SpawnChance(spawnInfo);
						if (weight > 0f)
							npcs.Add(netID);
					}
				}

				progressionGroups[pGroup.Key].AddNPCs(npcs);
			}
		}
		public static int GetOreInfusionPower(int requiredPickaxePower, float value) {
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
		private static void AddProgressionGroup(ProgressionGroup progressionGroup) => progressionGroups.Add(progressionGroup.ID, progressionGroup);
		private static void GuessInfusionPowers() {
			//Guess crafting source infusion powers
			SortedDictionary<int, int> guessedSourceInfusionPowers = new();
			for (int weaponType = 0; weaponType < ItemLoader.ItemCount; weaponType++) {
				if (!allWeaponRecipies.ContainsKey(weaponType))
					continue;

				Item sampleWeapon = weaponType.CSI();
				//For each weapon
				if (sampleWeapon.TryGetEnchantedItem(out EnchantedWeapon enchantedWeapon)) {
					int infusionPowerWeapon = sampleWeapon.GetWeaponInfusionPower();
					foreach (HashSet<int> ingredientTypes in allWeaponRecipies[weaponType]) {
						foreach (int ingredientType in ingredientTypes) {
							//Check if weapon's infusion power lowers the infusion power of the ingredient
							Item ingredientSampleItem = ingredientType.CSI();
							//If already an entry in VanillaCraftingItemSourceInfusionPowers or ModdedCraftingItemSourceInfusionPowers, use it instead of guessing
							if (VanillaCraftingItemSourceInfusionPowers.ContainsKey(ingredientType)) {
								if (!guessedSourceInfusionPowers.ContainsKey(ingredientType)) {
									guessedSourceInfusionPowers.Add(ingredientType, VanillaCraftingItemSourceInfusionPowers[ingredientType]);
									$"{ingredientSampleItem.S()} = {VanillaCraftingItemSourceInfusionPowers[ingredientType]} IP from {sampleWeapon.S()} (already existed)".LogSimple();
								}

								continue;
							}
							else if (ModdedCraftingItemSourceInfusionPowers.ContainsKey(ingredientSampleItem.Name)) {
								if (!guessedSourceInfusionPowers.ContainsKey(ingredientType)) {

									guessedSourceInfusionPowers.Add(ingredientType, ModdedCraftingItemSourceInfusionPowers[ingredientSampleItem.Name]);
									$"{ingredientSampleItem.S()} = {ModdedCraftingItemSourceInfusionPowers[ingredientSampleItem.Name]} IP from {sampleWeapon.S()} (already existed)".LogSimple();
								}

								continue;
							}

							int infusionPower = infusionPowerWeapon;
							if (ingredientSampleItem.TryGetEnchantedItem(out EnchantedWeapon enchantedWeaponIngredient))
								infusionPower = ingredientSampleItem.GetWeaponInfusionPower();

							if (ingredientType < ItemID.Count && weaponType >= ItemID.Count)//Don't allow modded weapon recipies to affect vanilla ingredients
								continue;

							if (guessedSourceInfusionPowers.ContainsKey(ingredientType)) {
								int currentInfusionPower = guessedSourceInfusionPowers[ingredientType];
								if (currentInfusionPower > infusionPower) {
									guessedSourceInfusionPowers[ingredientType] = infusionPower;
									$"{ingredientSampleItem.S()} = {infusionPower} IP from {sampleWeapon.S()}".LogSimple();
								}
							}
							else {
								guessedSourceInfusionPowers.Add(ingredientType, infusionPower);
								$"{ingredientSampleItem.S()} = {infusionPower} IP from {sampleWeapon.S()}".LogSimple();
							}
						}
					}
				}
			}

			//Remove non-limmiting ingredients
			foreach (KeyValuePair<int, int> sourceItem in new SortedDictionary<int, int>(guessedSourceInfusionPowers)) {
				int key = sourceItem.Key;
				string sourceItemName = key.CSI().Name;//temp
				int infusionPower = sourceItem.Value;
				bool isLimmitingItem = false;
				foreach (int weaponKey in allWeaponRecipies.Keys) {
					bool isLimmitingItemForThisItem = true;
					string weaponName = weaponKey.CSI().Name;//temp
					if (!allWeaponRecipies[weaponKey].SelectMany(t => t).Contains(key))
						continue;

					foreach (HashSet<int> ingredientTypes in allWeaponRecipies[weaponKey]) {
						foreach (int ingredientType in ingredientTypes) {
							if (guessedSourceInfusionPowers.ContainsKey(ingredientType)) {
								int ingredientInfusionPower = guessedSourceInfusionPowers[ingredientType];
								//string ingredientName = ingredientType.CSI().Name;//temp

								if (infusionPower < ingredientInfusionPower) {// || infusionPower == ingredientInfusionPower && IsWeaponItem(key.CSI())) {//Works but not needed?
									isLimmitingItemForThisItem = false;
									break;
								}
							}
						}
					}

					if (isLimmitingItemForThisItem) {
						isLimmitingItem = true;
						break;
					}
				}

				if (!isLimmitingItem)
					guessedSourceInfusionPowers.Remove(key);
			}

			//Print guessedSourceInfusionPowers as VanillaCraftingItemSourceInfusionPowers and ModdedCraftingItemSourceInfusionPowers
			SortedDictionary<int, int> guessedVanillaCraftingItemSourceInfusionPowers = new();
			SortedDictionary<int, int> guessedModdedCraftingItemSourceInfusionPowers = new();
			foreach (KeyValuePair<int, int> sourceItem in guessedSourceInfusionPowers) {
				int soureItemType = sourceItem.Key;
				int infusionPower = sourceItem.Value;// - (values.stack <= 1 ? 10 : 0);
				if (soureItemType < ItemID.Count) {
					guessedVanillaCraftingItemSourceInfusionPowers.Add(soureItemType, infusionPower);
				}
				else {
					guessedModdedCraftingItemSourceInfusionPowers.Add(soureItemType, infusionPower);
				}
			}

			string txt = "\npublic static SortedDictionary<int, int> VanillaCraftingItemSourceInfusionPowers = new() {";
			txt += guessedVanillaCraftingItemSourceInfusionPowers
				.OrderBy(sourcItem => sourcItem.Value)
				.Select(sourceItem => ItemID.Search.TryGetName(sourceItem.Key, out string name) ?
					$"\n\t{"{"} ItemID.{name}, {sourceItem.Value} {"}"}" :
					$"Failed to find a name for item: {sourceItem.Key.CSI().S()}")
				.JoinList(",");
			txt += "\n};//Not cleared\npublic static SortedDictionary<string, int> ModdedCraftingItemSourceInfusionPowers = new() {";
			txt += guessedModdedCraftingItemSourceInfusionPowers
				.OrderBy(sourcItem => sourcItem.Value)
				.Select(sourceItem => $"\n\t{"{"} \"{sourceItem.Key.CSI().Name}\", {sourceItem.Value} {"}"}").JoinList(",");
			txt += "\n};//Not cleared";
			txt.LogSimple();
		}
		private static void SetupWeaponsList() {
			//string allWeapons = $"\nAll Items:";
			for (int i = 0; i < ItemLoader.ItemCount; i++) {
				Item sampleItem = i.CSI();
				//allWeapons += $"\n{sampleItem.S()}";
				if (IsWeaponItem(sampleItem))
					weaponsList.Add(i);
			}

			//allWeapons.LogSimple();
			//$"\nweaponsList:\n{weaponsList.Select(type => $"{type.CSI().S()}").JoinList("\n")}".LogSimple();
		}
		private static void SetupReverseCraftableIngredients() {
			SortedDictionary<int, HashSet<int>> allRecipes = new();
			foreach (Recipe r in Main.recipe) {
				allRecipes.AddOrCombine(r.createItem.type, r.requiredItem.Select(i => i.type).ToHashSet());
			}

			foreach (int createItemType in allRecipes.Keys) {
				foreach (int ingredient in allRecipes[createItemType]) {
					if (allRecipes.ContainsKey(ingredient) && allRecipes[ingredient].Contains(createItemType))
						reverseCraftableIngredients.AddOrCombine(createItemType, ingredient);
				}
			}

			//$"\nreverseCraftableIngredients:\n{reverseCraftableIngredients.Select(pair => $"{pair.Key.CSI().S()}: {pair.Value.Select(t => t.CSI().S()).JoinList(", ")}").JoinList("\n")}".LogSimple();
		}
		private static void GetAllCraftingResources() {
			foreach (int weaponType in weaponsList) {
				if (TryGetAllCraftingIngredientTypes(weaponType, out HashSet<HashSet<int>> ingredients))
					allWeaponRecipies.Add(weaponType, ingredients);
			}

			foreach (int ingredient in allWeaponRecipies.Select(p => p.Value).SelectMany(t => t).SelectMany(t => t)) {
				weaponCraftingIngredients.Add(ingredient);
			}

			if (guessingInfusionPowers) $"\n{allWeaponRecipies.Select(weapon => $"{weapon.Key.CSI().S()}:{weapon.Value.Select(ingredient => $" {ingredient.Select(i => i.CSI().Name).JoinList(" or ")}").JoinList(", ")}").JoinList("\n")}".LogSimple();
		}
		private static bool TryGetAllCraftingIngredientTypes(int createItemType, out HashSet<HashSet<int>> ingredients) {
			//Item createItem = createItemType.CSI();
			//$"TryGetAllCraftingIngredientTypes({createItem.S()})".LogSimple();
			HashSet<HashSet<int>> resultIngredients = new();
			if (finishedSetup || !allExpandedRecepies.ContainsKey(createItemType)) {
				//IEnumerable<Recipe> recipies = Main.recipe.Where(r => r.createItem.type == createItemType);//Swap to this after all testing
				IEnumerable<Recipe> recipies = Main.recipe.Where((r, index) => r.createItem.type == createItemType && (r.createItem.type > ItemID.Count || index <= VANILLA_RECIPE_COUNT));
				HashSet<HashSet<HashSet<int>>> requiredItemTypeLists = new();
				foreach (Recipe recipe in recipies) {
					if (recipe.IsReverseCraftable())
						continue;

					HashSet<HashSet<int>> requiredItemTypes = new();
					foreach (Item ingredientItem in recipe.requiredItem) {
						int ingredientType = ingredientItem.type;
						if (TryGetAllCraftingIngredientTypes(ingredientType, out HashSet<HashSet<int>> ingredientTypes)) {
							//string ingredientTypesNames = ingredientTypes.Select(set => set.Select(t => t.CSI().S()).JoinList(" or ")).JoinList(", ");//Temp
							requiredItemTypes.CombineHashSet(ingredientTypes);
						}
						else {
							requiredItemTypes.TryAdd(new() { ingredientType });
						}
					}

					if (requiredItemTypes.Count <= 0)
						continue;

					foreach (Item requiredTileItem in recipe.requiredTile.Select(tile => WEGlobalTile.GetDroppedItem(tile)).Where(type => type > 0).Select(type => type.CSI())) {
						int requiredTileItemType = requiredTileItem.type;
						bool skip = WEMod.magicStorageEnabled && requiredTileItem.Name == "Demon Altar";
						if (skip)
							continue;

						if (TryGetAllCraftingIngredientTypes(requiredTileItemType, out HashSet<HashSet<int>> tileIngredientTypes)) {
							//string ingredientTypesNames = tileIngredientTypes.Select(set => set.Select(t => t.CSI().S()).JoinList(" or ")).JoinList(", ");//Temp
							requiredItemTypes.CombineHashSet(tileIngredientTypes);
						}
						else {
							requiredItemTypes.TryAdd(new() { requiredTileItemType });
						}
					}

					requiredItemTypeLists.Add(requiredItemTypes);
				}

				resultIngredients = resultIngredients.CombineIngredientLists(requiredItemTypeLists);
				if (!finishedSetup)
					allExpandedRecepies.Add(createItemType, resultIngredients);
			}

			if (finishedSetup) {
				ingredients = resultIngredients;
			}
			else {
				ingredients = allExpandedRecepies[createItemType];
			}

			//string ingredientTypesNames2 = $"{createItem.S()}: {ingredients.Select(set => set.Select(t => t.CSI().S()).JoinList(" or ")).JoinList(", ")}";
			//ingredientTypesNames2.LogSimple();
			return ingredients.Count > 0;
		}
		private static void SetupItemsFromNPCs() {
			foreach (KeyValuePair<int, NPC> npcPair in ContentSamples.NpcsByNetId) {
				int netID = npcPair.Key;
				NPC npc = npcPair.Value;
				foreach (IItemDropRule dropRule in Main.ItemDropsDB.GetRulesForNPCID(netID)) {
					List<DropRateInfo> dropRates = new();
					DropRateInfoChainFeed dropRateInfoChainFeed = new(1f);
					dropRule.ReportDroprates(dropRates, dropRateInfoChainFeed);
					foreach (DropRateInfo dropRate in dropRates) {
						int itemType = dropRate.itemId;
						if (weaponsList.Contains(itemType)) {
							if (WeaponsFromNPCs.ContainsKey(itemType)) {
								WeaponsFromNPCs[itemType].Add(netID);
							}
							else {
								WeaponsFromNPCs.Add(itemType, new() { netID });
							}
						}
						else if (weaponCraftingIngredients.Contains(itemType)) {
							if (IngredientsFromNPCs.ContainsKey(itemType)) {
								IngredientsFromNPCs[itemType].Add(netID);
							}
							else {
								IngredientsFromNPCs.Add(itemType, new() { netID });
							}
						}
					}
				}
			}

			foreach (KeyValuePair<int, SortedSet<int>> weapon in WeaponsFromNPCs) {
				foreach (int netID in weapon.Value) {
					NPCsThatDropWeaponsOrIngredients.Add(netID);
				}
			}

			foreach (KeyValuePair<int, SortedSet<int>> ingredient in IngredientsFromNPCs) {
				foreach (int netID in ingredient.Value) {
					NPCsThatDropWeaponsOrIngredients.Add(netID);
				}
			}
		}
		private static void SetupItemsFromLootItems() {
			foreach (KeyValuePair<int, Item> lootItemPair in ContentSamples.ItemsByType) {
				int type = lootItemPair.Key;
				Item item = lootItemPair.Value;
				foreach (IItemDropRule dropRule in Main.ItemDropsDB.GetRulesForItemID(type)) {
					List<DropRateInfo> dropRates = new();
					DropRateInfoChainFeed dropRateInfoChainFeed = new(1f);
					dropRule.ReportDroprates(dropRates, dropRateInfoChainFeed);
					foreach (DropRateInfo dropRate in dropRates) {
						int itemType = dropRate.itemId;
						if (weaponsList.Contains(itemType)) {
							if (WeaponsFromLootItems.ContainsKey(itemType)) {
								WeaponsFromLootItems[itemType].Add(type);
							}
							else {
								WeaponsFromLootItems.Add(itemType, new() { type });
							}
						}
						else if (weaponCraftingIngredients.Contains(itemType)) {
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
		private static void PopulateCraftingWeaponSources() {
			foreach (int weaponType in allWeaponRecipies.Keys) {
				InfusionPowerSource highestInfusionPowerSource = new();
				int infusionPower = -1;
				foreach (HashSet<int> ingredientTypes in allWeaponRecipies[weaponType]) {
					foreach (int ingredientType in ingredientTypes) {
						bool found = false;
						ItemSource itemSource = new(weaponType, ItemSourceType.Craft, ingredientType);
						InfusionPowerSource infusionPowerSource = new(itemSource);
						if (ingredientType < ItemID.Count) {
							if (VanillaCraftingItemSourceInfusionPowers.ContainsKey(ingredientType))
								found = true;
						}
						else {
							Item sampleIngredientItem = ingredientType.CSI();
							if (ModdedCraftingItemSourceInfusionPowers.ContainsKey(sampleIngredientItem.Name))
								found = true;
						}

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
					WeaponSources.Add(weaponType, highestInfusionPowerSource);
			}
		}
		private static void ClearTempDictionaries() {
			progressionGroups.Clear();
			weaponsList.Clear();
			weaponCraftingIngredients.Clear();
			allWeaponRecipies.Clear();
			allExpandedRecepies.Clear();
			infusionPowerTiles = null;
			finishedSetup = true;
		}
		public static bool TryGetBaseInfusionPower(Item item, out int baseInfusionPower) {
			int weaponType = item.type;
			if (false && WeaponSources.ContainsKey(weaponType)) {
				baseInfusionPower = WeaponSources[weaponType].InfusionPower;

				return true;
			}

			baseInfusionPower = 0;
			return false;
		}
		private static bool IsReverseCraftable(this Recipe recipe) {
			int createItemType = recipe.createItem.type;
			string temp = $"createItem {recipe.createItem.S()}: {recipe.requiredItem.Select(i => i.S()).JoinList(", ")}";
			if (reverseCraftableIngredients.Keys.Contains(createItemType)) {
				if (reverseCraftableIngredients[createItemType].Where(ingredientType => recipe.requiredItem.Select(item => item.type).Contains(ingredientType)).Any())
					return true;
			}

			return false;
		}
		public static (int[], bool[]) EncodeNPCSpawnInfo(this NPCSpawnInfo info) {
			int[] spawnInfoCodeInts = new int[6];
			bool[] spawnInfoCodeBools = new bool[12];
			spawnInfoCodeInts[0] = info.Player.whoAmI;
			spawnInfoCodeInts[1] = info.PlayerFloorX;
			spawnInfoCodeInts[2] = info.PlayerFloorY;
			spawnInfoCodeInts[3] = info.SpawnTileType;
			spawnInfoCodeInts[4] = info.SpawnTileX;
			spawnInfoCodeInts[5] = info.SpawnTileY;
			spawnInfoCodeBools[0] = info.DesertCave;
			spawnInfoCodeBools[1] = info.Granite;
			spawnInfoCodeBools[2] = info.Invasion;
			spawnInfoCodeBools[3] = info.Lihzahrd;
			spawnInfoCodeBools[4] = info.Marble;
			spawnInfoCodeBools[5] = info.PlanteraDefeated;
			spawnInfoCodeBools[6] = info.PlayerInTown;
			spawnInfoCodeBools[7] = info.PlayerSafe;
			spawnInfoCodeBools[8] = info.SafeRangeX;
			spawnInfoCodeBools[9] = info.Sky;
			spawnInfoCodeBools[10] = info.SpiderCave;
			spawnInfoCodeBools[11] = info.Water;

			return (spawnInfoCodeInts, spawnInfoCodeBools);
		}
		public static NPCSpawnInfo DecodeNPCSpawnInfo(this (int[], bool[]) spawnInfoCode) {
			NPCSpawnInfo info = new NPCSpawnInfo();
			int[] ints = spawnInfoCode.Item1;
			bool[] bools = spawnInfoCode.Item2;
			info.DesertCave = bools[0];
			info.Granite = bools[1];
			info.Invasion = bools[2];
			info.Lihzahrd = bools[3];
			info.Marble = bools[4];
			info.PlanteraDefeated = bools[5];
			info.PlayerInTown = bools[6];
			info.PlayerSafe = bools[7];
			info.SafeRangeX = bools[8];
			info.Sky = bools[9];
			info.SpiderCave = bools[10];
			info.Water = bools[11];
			info.Player = Main.player[ints[0]] ?? Main.LocalPlayer;
			info.PlayerFloorX = ints[1];
			info.PlayerFloorY = ints[2];
			info.SpawnTileType = ints[3];
			info.SpawnTileX = ints[4];
			info.SpawnTileY = ints[5];

			return info;
		}
	}
	public static class InfusionProgressionTests {
		private static bool shouldRunTests = true;
		public static void RunTests() {
			if (!shouldRunTests)
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
				int result = GetOreInfusionPower(pair.pickPower, pair.value);
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
