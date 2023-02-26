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
		Priates = 545,
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
		public ProgressionGroup(ProgressionGroupID id, int Night = 0, int Underground = 0, int EasyObtainAfter = -5, int ObtainAfter = 5) {
			ID = id;
			infusionPower = (int)ID;
			night = Night;
			underground = Underground;
			easyObtainAfter = EasyObtainAfter;
			obtainAfter = ObtainAfter;
		}

		public ProgressionGroupID ID { get; private set; }
		int infusionPower;
		int night;
		int underground;
		int easyObtainAfter;
		int obtainAfter;
		SortedSet<int> itemTypes = new();
		SortedSet<int> npcTypes = new();
		SortedSet<ChestID> chests = new();
		SortedSet<CrateID> crates = new();
		public void AddItems(IEnumerable<int> newItems) => itemTypes.UnionWith(newItems);
		public void AddNPCs(IEnumerable<int> newNPCs) => npcTypes.UnionWith(newNPCs);
		public void Add(IEnumerable<ChestID> newChests) => chests.UnionWith(newChests);
		public void Add(IEnumerable<CrateID> newCrates) => crates.UnionWith(newCrates);
		public void Add(IEnumerable<Item> newItems) => AddItems(newItems.Select(i => i.type));
		public void Add(IEnumerable<NPC> newNPCs) => AddNPCs(newNPCs.Select(npc => npc.netID));
	}
	public static class InfusionProgression
	{
		private static bool guessingInfusionPowers = true;
		public const int VANILLA_RECIPE_COUNT = 2691;
		private static bool finishedSetup = false;
		public static SortedDictionary<int, InfusionPowerSource> WeaponSources { get; private set; } = new();//Not cleared
		public static SortedDictionary<InfusionPowerSource, int> SourceInfusionPowers { get; private set; } = new();//Not cleared
		private static SortedDictionary<int, HashSet<HashSet<int>>> allWeaponRecipies = new();
		private static SortedSet<int> weaponsList = new();
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
			{ ItemID.Meteorite, 205 },//Needs to be post EaterOfWorlds/BrainOfCthulhu instead?
			{ ItemID.Hellstone, 306 },
			{ ItemID.CobaltOre, 420 },
			{ ItemID.MythrilOre, 420 },
			{ ItemID.PalladiumOre, 430 },
			{ ItemID.OrichalcumOre, 430 },
			{ ItemID.AdamantiteOre, 440 },
			{ ItemID.TitaniumOre, 440 },
			{ ItemID.ChlorophyteOre, 653 }
		};
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
			SetupProgressionGroups();
			SetupWeaponsList();
			SetupReverseCraftableIngredients();
			GetAllCraftingResources();
		}
		private static void SetupProgressionGroups() {
			SetupMinedOreInfusionPowers();
			AddProgressionGroup(new(ProgressionGroupID.Forest, Night: 5));
			AddProgressionGroup(new(ProgressionGroupID.Desert, Underground: 80));
			AddProgressionGroup(new(ProgressionGroupID.Snow, Underground: 30));
			AddProgressionGroup(new(ProgressionGroupID.Ocean, Underground: 20));
			AddProgressionGroup(new(ProgressionGroupID.Jungle, Underground: 25));
			AddProgressionGroup(new(ProgressionGroupID.Empress, Night: -60));

			foreach (var id in Enum.GetValues(typeof(ProgressionGroupID)).Cast<ProgressionGroupID>()) {
				if (!progressionGroups.ContainsKey(id))
					AddProgressionGroup(new(id));
			}
		}
		private static void SetupInfusionPowerTiles() {
			infusionPowerTiles = new();
			for (int tileType = 0; tileType < TileID.Count; tileType++) {
				int itemType = WEGlobalTile.GetDroppedItem(tileType);
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

			$"\ninfusionPowerTiles:\n{infusionPowerTiles.Select(i => $"infusionPower: {i.Key}, pickPower: {i.Value.pickPower}, value: {i.Value.value}").JoinList("\n")}".LogSimple();
		}
		private static void SetupMinedOreInfusionPowers() {
			SortedSet<int> oreInfusionPowerSet = new(OreInfusionPowers.Select(p => p.Value));
			//SortedDictionary<int, (int tile, Item item)> infusionPowerTiles = new(); 
			for (int tileType = TileID.Count; tileType < TileLoader.TileCount; tileType++) {
				int itemType = WEGlobalTile.GetDroppedItem(tileType);
				if (itemType <= 0)
					continue;

				Item item = itemType.CSI();
				ModTile modTile = TileLoader.GetTile(tileType);
				if (itemType > 0 && modTile != null) {
					bool ore = TileID.Sets.Ore[tileType];
					int requiredPickaxePower = WEGlobalTile.GetRequiredPickaxePower(tileType, true);
					float mineResist = modTile.MineResist;
					float value = item.value;
					if (ore || ((requiredPickaxePower > 0 || mineResist > 0) && value > 0)) {
						//int infusionPower = GetOreInfusionPower(requiredPickaxePower, value);
					}
				}
			}
			foreach (KeyValuePair<int, int> pair in OreInfusionPowers) {

			}

			foreach (KeyValuePair<int, int> pair in OreInfusionPowers) {
				VanillaCraftingItemSourceInfusionPowers.Add(pair.Key, pair.Value);
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
		private static void AddProgressionGroup(ProgressionGroup progressionGroup) {
			progressionGroups.Add(progressionGroup.ID, progressionGroup);
		}
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
			//reverseCraftableIngredients
			SortedDictionary<int, HashSet<int>> allRecipes = new();
			foreach (Recipe r in Main.recipe) {
				allRecipes.AddOrCombine(r.createItem.type, r.requiredItem.Select(i => i.type).ToHashSet());
			}
			/*
			for (int i = 0; i < ItemLoader.ItemCount; i++) {
				HashSet<int> ingredients = Main.recipe.Select(r => r.requiredItem.Select(i => i.type)).SelectMany(t => t).ToHashSet();
				if (ingredients.Any())
					allRecipes.Add(i, ingredients);
			}
			*/

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
