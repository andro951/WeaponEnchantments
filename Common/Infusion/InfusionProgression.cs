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
		None = -1,
		Forest,
		Desert,
		GiantTree,
		Beach,
		Snow,
		Ocean,
		UndergroundSnow,
		DeepOcean,
		Jungle,
		Evil,
		KingSlime,
		UndergroundDesert,
		UndergroundJungle,
		Granite,
		Marble,
		Eye,
		BloodMoon,
		GoblinArmy,
		EaterBrain,
		OldOneArmyT1,
		Bee,
		Skeletron,
		Dungeon,
		ShadowChest,
		Deer,
		Wall,
		HardModeNight,
		FrostLegeon,
		Pirates,
		Eclipse,
		QueenSlime,
		Destroyer,
		OldOneArmyT2,
		SkeletronPrime,
		Twins,
		Plantera,
		PumpkinMoon,
		FrostMoon,
		Betsey,
		Golem,
		OldOneArmyT3,
		MartianInvasion,
		MartianSaucer,
		DukeFishron,
		EmpressNight,
		Empress,
		LunaticCultist,
		LunarInvasion,
		PostMoonLordEasy,
		MoonLord
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
		public ProgressionGroup(ProgressionGroupID id, int InfusionPower, ProgressionGroupID parent = ProgressionGroupID.None, 
				IEnumerable<int> itemTypes = null, IEnumerable<string> itemNames = null, IEnumerable<int> npcTypes = null, 
				IEnumerable<string> npcNames = null, IEnumerable<ChestID> chests = null, IEnumerable<CrateID> crates = null) {
			parentID = parent;
			ID = id;
			infusionPower = InfusionPower;
			bossNetIDs = GetBossType(id);
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
		}

		public ProgressionGroup Parent => parentID > ProgressionGroupID.None ? progressionGroups[parentID] : null;
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
		}//TODO: Give +5 if from an npc when converting this to ItemSource or InfusionSource
		SortedSet<int> bossNetIDs;
		public SortedSet<int> ItemTypes { get; private set; } = new();
		//public SortedSet<string> ItemNames { get; private set; } = new();
		public SortedSet<int> NpcTypes { get; private set; } = new();
		//public SortedSet<string> NpcNames { get; private set; } = new();
		public SortedSet<ChestID> Chests { get; private set; } = new();
		public SortedSet<CrateID> Crates { get; private set; } = new();
		public void AddItems(IEnumerable<int> newItems) => ItemTypes.UnionWith(newItems);
		//public void AddItems(IEnumerable<string> newItems) => ItemNames.UnionWith(newItems);
		public void AddItems(IEnumerable<string> newItems) {
			SortedSet<string> newItemsSet = new SortedSet<string>(newItems);
			foreach (int itemType in WeaponsList) {
				string itemName = itemType.CSI().Name;
				if (newItemsSet.Contains(itemName)) {
					ItemTypes.Add(itemType);
					newItemsSet.Remove(itemName);
				}

				if (newItemsSet.Count < 1)
					break;
			}

			if (newItemsSet.Count > 0)
				$"Couldn't find Items with the names: {newItemsSet.JoinList(", ")}".LogSimple();
		}
		public void AddNPCs(IEnumerable<int> newNPCs) => NpcTypes.UnionWith(newNPCs);
		//public void AddNPCs(IEnumerable<string> newNPCs) => NpcNames.UnionWith(newNPCs);
		public void AddNPCs(IEnumerable<string> newNPCs) {
			SortedSet<string> newNpcsSet = new SortedSet<string>(newNPCs);
			foreach (int itemType in WeaponsList) {
				string itemName = itemType.CSI().Name;
				if (newNpcsSet.Contains(itemName)) {
					NpcTypes.Add(itemType);
					newNpcsSet.Remove(itemName);
				}

				if (newNpcsSet.Count < 1)
					break;
			}

			if (newNpcsSet.Count > 0)
				$"Couldn't find Npcs with the names: {newNpcsSet.JoinList(", ")}".LogSimple();
		}
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
		/*
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
		*/
	}
	public static class InfusionProgression {
		private static bool guessingInfusionPowers => false;
		public const int VANILLA_RECIPE_COUNT = 2691;
		private static bool finishedSetup = false;
		public static SortedDictionary<int, InfusionPowerSource> WeaponSources { get; private set; } = new();//Not cleared
		public static SortedDictionary<InfusionPowerSource, int> SourceInfusionPowers { get; private set; } = new();//Not cleared
		private static SortedDictionary<int, HashSet<HashSet<int>>> allWeaponRecipies = new();
		public static SortedSet<int> WeaponsList { get; private set; } = new();
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
		public static SortedSet<int> NPCsThatAreSetup { get; private set; } = new();
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

		#region SetupTempDictionaries

		private static void SetupTempDictionaries() {
			SetupWeaponsList();
			SetupReverseCraftableIngredients();
			GetAllCraftingResources();
			SetupItemsFromNPCs();
			SetupItemsFromLootItems();
			SetupMinedOreInfusionPowers();
			//CheckNPCSpawns();
			SetupProgressionGroups();
			PopulateNPCsThatAreSetup();
		}
		private static void SetupWeaponsList() {
			//string allWeapons = $"\nAll Items:";
			for (int i = 0; i < ItemLoader.ItemCount; i++) {
				Item sampleItem = i.CSI();
				//allWeapons += $"\n{sampleItem.S()}";
				if (IsWeaponItem(sampleItem))
					WeaponsList.Add(i);
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
			foreach (int weaponType in WeaponsList) {
				if (TryGetAllCraftingIngredientTypes(weaponType, out HashSet<HashSet<int>> ingredients))
					allWeaponRecipies.Add(weaponType, ingredients);
			}

			foreach (int ingredient in allWeaponRecipies.Select(p => p.Value).SelectMany(t => t).SelectMany(t => t)) {
				weaponCraftingIngredients.Add(ingredient);
			}

			if (guessingInfusionPowers) $"\n{allWeaponRecipies.Select(weapon => $"{weapon.Key.CSI().S()}:{weapon.Value.Select(ingredient => $" {ingredient.Select(i => i.CSI().Name).JoinList(" or ")}").JoinList(", ")}").JoinList("\n")}".LogSimple();
		}
		private static void SetupItemsFromNPCs() {
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
						Item item = itemType.CSI();
						if (WeaponsList.Contains(itemType)) {
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
						if (WeaponsList.Contains(itemType)) {
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
		/*
		private static void CheckNPCSpawns() {
			foreach (KeyValuePair<ProgressionGroupID, ProgressionGroup> progressionGroup in progressionGroups) {
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

				progressionGroups[progressionGroup.Key].AddNPCs(npcs);
			}
		}
		*/
		private static void SetupProgressionGroups() {
			AddProgressionGroup(new(ProgressionGroupID.Forest, 0,
				npcTypes: new SortedSet<int>() {
					NPCID.BlueSlime
				}));
			AddProgressionGroup(new(ProgressionGroupID.Desert, 10));//, Underground: 80));
			AddProgressionGroup(new(ProgressionGroupID.GiantTree, 20));
			AddProgressionGroup(new(ProgressionGroupID.Beach, 30));
			AddProgressionGroup(new(ProgressionGroupID.Snow, 35));//, Underground: 30));
			AddProgressionGroup(new(ProgressionGroupID.Ocean, 50));//, Underground: 20));
			AddProgressionGroup(new(ProgressionGroupID.UndergroundSnow, 65));
			AddProgressionGroup(new(ProgressionGroupID.DeepOcean, 70));
			AddProgressionGroup(new(ProgressionGroupID.Jungle, 75));//, Underground: 25));
			AddProgressionGroup(new(ProgressionGroupID.Evil, 80));
			AddProgressionGroup(new(ProgressionGroupID.KingSlime, 85));
			AddProgressionGroup(new(ProgressionGroupID.UndergroundDesert, 90));
			AddProgressionGroup(new(ProgressionGroupID.UndergroundJungle, 100));
			AddProgressionGroup(new(ProgressionGroupID.Granite, 105));
			AddProgressionGroup(new(ProgressionGroupID.Marble, 105));
			AddProgressionGroup(new(ProgressionGroupID.Eye, 120));
			AddProgressionGroup(new(ProgressionGroupID.BloodMoon, 150));
			AddProgressionGroup(new(ProgressionGroupID.GoblinArmy, 180));
			AddProgressionGroup(new(ProgressionGroupID.EaterBrain, 200));
			AddProgressionGroup(new(ProgressionGroupID.OldOneArmyT1, -10, ProgressionGroupID.EaterBrain));
			AddProgressionGroup(new(ProgressionGroupID.Bee, 250));
			AddProgressionGroup(new(ProgressionGroupID.Skeletron, 300));
			AddProgressionGroup(new(ProgressionGroupID.Dungeon, 320));
			AddProgressionGroup(new(ProgressionGroupID.ShadowChest, 350));
			AddProgressionGroup(new(ProgressionGroupID.Deer, 380));
			AddProgressionGroup(new(ProgressionGroupID.Wall, 420));
			AddProgressionGroup(new(ProgressionGroupID.HardModeNight, 430));
			AddProgressionGroup(new(ProgressionGroupID.FrostLegeon, 450));
			AddProgressionGroup(new(ProgressionGroupID.Pirates, 545));
			AddProgressionGroup(new(ProgressionGroupID.Eclipse, 560));
			AddProgressionGroup(new(ProgressionGroupID.QueenSlime, 575));
			AddProgressionGroup(new(ProgressionGroupID.OldOneArmyT2, -10, ProgressionGroupID.Destroyer));
			AddProgressionGroup(new(ProgressionGroupID.Destroyer, 605));
			AddProgressionGroup(new(ProgressionGroupID.SkeletronPrime, 615));
			AddProgressionGroup(new(ProgressionGroupID.Twins, 630));
			AddProgressionGroup(new(ProgressionGroupID.Plantera, 725));
			AddProgressionGroup(new(ProgressionGroupID.PumpkinMoon, 820));
			AddProgressionGroup(new(ProgressionGroupID.FrostMoon, 820));
			AddProgressionGroup(new(ProgressionGroupID.OldOneArmyT3, -10, ProgressionGroupID.Golem));
			AddProgressionGroup(new(ProgressionGroupID.Betsey, -5, ProgressionGroupID.Golem));
			AddProgressionGroup(new(ProgressionGroupID.Golem, 845));
			AddProgressionGroup(new(ProgressionGroupID.MartianInvasion, 860));
			AddProgressionGroup(new(ProgressionGroupID.MartianSaucer, 880));
			AddProgressionGroup(new(ProgressionGroupID.EmpressNight, 910));
			AddProgressionGroup(new(ProgressionGroupID.DukeFishron, 940));
			AddProgressionGroup(new(ProgressionGroupID.Empress, 970));
			AddProgressionGroup(new(ProgressionGroupID.LunaticCultist, 975));
			AddProgressionGroup(new(ProgressionGroupID.LunarInvasion, 1005));
			AddProgressionGroup(new(ProgressionGroupID.PostMoonLordEasy, -50, ProgressionGroupID.MoonLord));
			AddProgressionGroup(new(ProgressionGroupID.MoonLord, 1100));
			//AddProgressionGroup(new(ProgressionGroupID., ));

		}
		private static void PopulateNPCsThatAreSetup() {
			foreach (ProgressionGroup progressionGroup in progressionGroups.Values) {
				foreach (int npcType in progressionGroup.NpcTypes) {
					if (!NPCsThatAreSetup.Contains(npcType)) {
						NPCsThatAreSetup.Add(npcType);
					}
					else {
						$"{npcType.CSNPC().S()} already exists in Progression Group: {progressionGroup.ID}".LogSimple();
					}
				}
			}
		}

		//Supporting Functions
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
		private static bool IsReverseCraftable(this Recipe recipe) {
			int createItemType = recipe.createItem.type;
			string temp = $"createItem {recipe.createItem.S()}: {recipe.requiredItem.Select(i => i.S()).JoinList(", ")}";
			if (reverseCraftableIngredients.Keys.Contains(createItemType)) {
				if (reverseCraftableIngredients[createItemType].Where(ingredientType => recipe.requiredItem.Select(item => item.type).Contains(ingredientType)).Any())
					return true;
			}

			return false;
		}
		private static void AddProgressionGroup(ProgressionGroup progressionGroup) => progressionGroups.Add(progressionGroup.ID, progressionGroup);
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


		#endregion

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
		private static void ClearTempDictionaries() {
			progressionGroups.Clear();
			WeaponsList.Clear();
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
