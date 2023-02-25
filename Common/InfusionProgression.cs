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
	public static class InfusionProgression
	{
		private static bool guessingInfusionPowers = true;
		public const int VANILLA_RECIPE_COUNT = 2691;
		private static bool finishedSetup = false;
		public static SortedDictionary<int, InfusionPowerSource> WeaponSources { get; private set; } = new();//Not cleared
		public static SortedDictionary<InfusionPowerSource, int> SourceInfusionPowers { get; private set; } = new();//Not cleared
		private static SortedDictionary<int, HashSet<HashSet<int>>> allWeaponRecipies = new();
		private static HashSet<int> weaponsList = new();
		private static HashSet<int> craftingStations = new();
		public static HashSet<int> weaponIngredients = new();
		private static SortedDictionary<int, HashSet<int>> requiredTileIngredients = new();
		private static SortedDictionary<int, HashSet<int>> reverseCraftableIngredients = new();
		public static SortedDictionary<int, int> VanillaCraftingItemSourceInfusionPowers = new() {
			{ ItemID.Wood, 0 },
			{ ItemID.Acorn, 0 },
			{ ItemID.Ebonwood, 0 },
			{ ItemID.RichMahogany, 0 },
			{ ItemID.Shadewood, 0 },
			{ ItemID.BorealWood, 0 },
			{ ItemID.PalmWood, 0 },
			{ ItemID.Gel, 5 },
			{ ItemID.Cobweb, 8 },
			{ ItemID.CopperBar, 10 },
			{ ItemID.RedBrick, 10 },
			{ ItemID.Cactus, 10 },
			{ ItemID.TinBar, 10 },
			{ ItemID.IronBar, 20 },
			{ ItemID.Bomb, 20 },
			{ ItemID.WaterBucket, 20 },
			{ ItemID.LeadBar, 20 },
			{ ItemID.VilePowder, 25 },
			{ ItemID.Silk, 25 },
			{ ItemID.SandstoneBrick, 25 },
			{ ItemID.IceBlock, 25 },
			{ ItemID.ViciousPowder, 25 },
			{ ItemID.Shiverthorn, 30 },
			{ ItemID.Amethyst, 50 },
			{ ItemID.SilverBar, 55 },
			{ ItemID.Topaz, 55 },
			{ ItemID.TungstenBar, 55 },
			{ ItemID.Sapphire, 60 },
			{ ItemID.JungleRose, 60 },
			{ ItemID.Emerald, 65 },
			{ ItemID.PinkGel, 70 },
			{ ItemID.GoldBar, 75 },
			{ ItemID.Ruby, 75 },
			{ ItemID.PlatinumBar, 75 },
			{ ItemID.LifeCrystal, 85 },
			{ ItemID.Diamond, 85 },
			{ ItemID.FlinxFur, 95 },
			{ ItemID.Stinger, 96 },
			{ ItemID.JungleSpores, 96 },
			{ ItemID.FossilOre, 100 },
			{ ItemID.Ale, 101 },
			{ ItemID.Vine, 118 },
			{ ItemID.DemoniteBar, 120 },
			{ ItemID.CorruptSeeds, 120 },
			{ ItemID.BottledWater, 120 },
			{ ItemID.HallowedSeeds, 120 },
			{ ItemID.EbonsandBlock, 120 },
			{ ItemID.PixieDust, 120 },
			{ ItemID.CrimsandBlock, 120 },
			{ ItemID.CrimtaneBar, 120 },
			{ ItemID.CrimsonSeeds, 120 },
			{ ItemID.MeteoriteBar, 122 },
			{ ItemID.Amber, 129 },
			{ ItemID.FallenStar, 157 },
			{ ItemID.AntlionMandible, 159 },
			{ ItemID.IllegalGunParts, 159 },
			{ ItemID.TissueSample, 160 },
			{ ItemID.ShadowScale, 162 },
			{ ItemID.BonePlatform, 200 },
			{ ItemID.BoneBlockWall, 200 },
			{ ItemID.Minishark, 222 },
			{ ItemID.BeeWax, 243 },
			{ ItemID.HellstoneBar, 306 },
			{ ItemID.Pearlwood, 400 },
			{ ItemID.CobaltBar, 418 },
			{ ItemID.AdamantiteBar, 420 },
			{ ItemID.CrystalShard, 420 },
			{ ItemID.SoulofNight, 420 },
			{ ItemID.SpiderFang, 420 },
			{ ItemID.AncientBattleArmorMaterial, 420 },
			{ ItemID.DjinnLamp, 420 },
			{ ItemID.PalladiumBar, 424 },
			{ ItemID.MythrilBar, 427 },
			{ ItemID.OrichalcumBar, 433 },
			{ ItemID.SoulofLight, 440 },
			{ ItemID.TitaniumBar, 442 },
			{ ItemID.DarkShard, 453 },
			{ ItemID.LightShard, 453 },
			{ ItemID.CursedFlame, 460 },
			{ ItemID.SpellTome, 460 },
			{ ItemID.Ichor, 460 },
			{ ItemID.FrostCore, 460 },
			{ ItemID.Shotgun, 467 },
			{ ItemID.HallowedBar, 598 },
			{ ItemID.SoulofMight, 603 },
			{ ItemID.SharkFin, 606 },
			{ ItemID.SoulofFright, 616 },
			{ ItemID.BlackLens, 628 },
			{ ItemID.Harp, 628 },
			{ ItemID.UnicornHorn, 628 },
			{ ItemID.SoulofSight, 628 },
			{ ItemID.ChlorophyteBar, 653 },
			{ ItemID.ShroomiteBar, 678 },
			{ ItemID.BrokenHeroSword, 725 },
			{ ItemID.SpectreBar, 734 },
			{ ItemID.FragmentVortex, 1007 },
			{ ItemID.FragmentNebula, 1007 },
			{ ItemID.FragmentSolar, 1007 },
			{ ItemID.FragmentStardust, 1007 },
			{ ItemID.LunarBar, 1020 },
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

			//Clear all other dictionaries
			ClearTempDictionaries();
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
					int infusionPowerWeapon = enchantedWeapon.GetWeaponInfusionPower();
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
								infusionPower = enchantedWeaponIngredient.GetWeaponInfusionPower();

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
		private static void SetupTempDictionaries() {
			SetupWeaponsList();
			SetupReverseCraftableIngredients();
			//GetAllRequiredTileIngredients();
			GetAllWeaponIngredients();
			GetAllCraftingResources();
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
		private static bool CraftedFromAnyInList(this int ingredientType, IEnumerable<int> createdItemTypes) {
			foreach (int ingredient in Main.recipe.Where(r => createdItemTypes.Contains(r.createItem.type)).Select(r => r.requiredItem.Select(i => i.type)).SelectMany(t => t)) {
				if (ingredient == ingredientType)
					return true;
			}

			return false;
		}
		private static void GetAllRequiredTileIngredients() {//TODO: Continue to check required tile ingredients like TryGetAllCraftingIngredientTypes.  Maybe just merge the 2
			/*
			//requiredTileIngredients
			SortedDictionary<int, int> tileTypeToItemType = new();
			foreach (int tileType in Main.recipe.Select(recipe => recipe.requiredTile).SelectMany(tiles => tiles).ToHashSet()) {
				int itemType = WEGlobalTile.GetDroppedItem(tileType);
				if (itemType <= 0) {
					if (tileType > TileID.Count) {
						if (TryGetModTileName(tileType, out string modTileName) && TryGetModTileItemType(modTileName, out int modTileItemType)) {
							itemType = modTileItemType;
						}
						else {
							$"Failed to find find modded tile name for tile: {tileType}, modTileName: {modTileName}".LogSimple();
						}
					}
					else {
						$"Failed to find find vanilla tile type: {tileType}".LogSimple();
					}
				}

				if (itemType > 0)
					tileTypeToItemType.Add(tileType, itemType);
			}
			*/
			HashSet<int> requiredTileTypes = Main.recipe.Select(recipe => recipe.requiredTile).SelectMany(tiles => tiles).ToHashSet();

			foreach (int tileType in requiredTileTypes) {
				int itemType = WEGlobalTile.GetDroppedItem(tileType);
				if (itemType <= 0)
					continue;

				Item temp = itemType.CSI();//Temp
										   //int createItemType in Main.recipe.Select(r => r.requiredTile).SelectMany(t => t).ToHashSet().Where(type => tileTypeToItemType.ContainsKey(type))
				IEnumerable<Recipe> recipies = Main.recipe.Where(r => r.createItem.type == itemType);
				if (recipies.Any()) {
					HashSet<int> ingredients = recipies.First().requiredItem.Select(i => i.type).ToHashSet();
					string temp2 = ingredients.Select(type => $"{type.CSI().S()}").JoinList(", ");
					requiredTileIngredients.Add(tileType, ingredients);
				}
			}

			//$"\n{requiredTileIngredients.Select(pair => $"{(TileLoader.GetTile(pair.Key) != null ? TileLoader.GetTile(pair.Key).Name : WEGlobalTile.GetDroppedItem(pair.Key).CSI().S())}: {pair.Value.Select(type => type.CSI().S()).JoinList(", ")}").JoinList("\n")}".LogSimple();
		}
		private static void GetAllWeaponIngredients() {
			foreach (Recipe recipe in Main.recipe.Where((r, index) => weaponsList.Contains(r.createItem.type) && (r.createItem.type > ItemID.Count || index <= VANILLA_RECIPE_COUNT))) {
				foreach (int type in recipe.requiredItem.Select(i => i.type)) {
					weaponIngredients.Add(type);
				}

				foreach (int type in recipe.requiredTile.Select(tileType => WEGlobalTile.GetDroppedItem(tileType)).Where(type => type > 0)) {
					weaponIngredients.Add(type);
					craftingStations.Add(type);
				}
			}
			/*
			SortedDictionary<int, int> weaponIngredientCounts = new();
			foreach (Recipe recipe in Main.recipe.Where((r, index) => weaponsList.Contains(r.createItem.type) && (r.createItem.type > ItemID.Count || index <= VANILLA_RECIPE_COUNT))) {
				foreach (int type in recipe.requiredItem.Select(i => i.type)) {
					weaponIngredientCounts.AddOrCombine(type, 1);
				}

				foreach (int type in recipe.requiredTile.Select(tileType => WEGlobalTile.GetDroppedItem(tileType)).Where(type => type > 0)) {
					weaponIngredientCounts.AddOrCombine(type, 1);
					craftingStations.Add(type);
				}
			}

			weaponIngredients = weaponIngredientCounts.Where(pair => pair.Value > 1 || IsWeaponItem(pair.Key.CSI())).Select(pair => pair.Key).ToHashSet();
			*/

			/*
			HashSet<string> requiredTiles = new();
			foreach (Recipe recipe in Main.recipe) {
				foreach (int type in recipe.requiredTile) {
					if (TileID.Search.TryGetName(type, out string tileName)) {
						requiredTiles.Add(tileName);
					}
					//ItemID.Search.TryGetName(sourceItem.Key, out string name)
				}
			}
			
			//$"\n{requiredTiles.Select(tileName => $"\t\tcase TileID.{tileName}:\n\t\t\tdropItem = ItemID.{tileName};\n\t\t\tbreak;").JoinList("\n")}".LogSimple();
			$"\n{Main.recipe
				.Select(recipe => recipe.requiredTile
				.Where(tile => tile >= TileID.Count))
				.SelectMany(t => t)
				.ToHashSet()
				.Select(type => TileLoader.GetTile(type))
				.Where(modTile => modTile != null)
				.Select(modTile => $"\t\tcase {"\""}{modTile.Name}{"\""}, {modTile.FullName}:\n\t\t\tdropItemName = {"\""}{modTile.Name}{"\""};\n\t\t\tbreak;")
				.JoinList("\n")}".LogSimple();
			*/
		}
		private static void GetAllCraftingResources() {
			foreach (int weaponType in weaponsList) {
				if (TryGetAllCraftingIngredientTypes(weaponType, out HashSet<HashSet<int>> ingredients))
					allWeaponRecipies.Add(weaponType, ingredients);
			}

			if (guessingInfusionPowers) {
				$"\n{allWeaponRecipies.Select(weapon => $"{weapon.Key.CSI().S()}:{weapon.Value.Select(ingredient => $" {ingredient.Select(i => i.CSI().Name).JoinList(" or ")}").JoinList(", ")}").JoinList("\n")}".LogSimple();
			}
		}
		//public static List<int> typesQue = new();
		//public static int weaponBeingChecked = -1;
		/*
		public static bool TryGetAllCraftingIngredientTypes(int createItemType, out HashSet<int> ingredients) {
			if (LogMethods.debugging) $"\\/TryGetAllCraftingIngredientTypes({createItemType.CSI().Name}); {typesQue.Select(t => t.CSI().Name).JoinList(", ")}".Log();
			bool isOriginal = false;
			if (weaponBeingChecked == -1) {
				weaponBeingChecked = createItemType;
				isOriginal = true;
			}
			else if (typesQue.Any() && !weaponIngredients.Contains(createItemType) || typesQue.Contains(createItemType)) {
				ingredients = null;
				if (LogMethods.debugging) $"/\\TryGetAllCraftingIngredientTypes({createItemType.CSI().Name}); Removed, {typesQue.Select(t => t.CSI().Name).JoinList(", ")}".Log();
				
				return false;
			}
			
			typesQue.Add(createItemType);
			bool isWeapon = IsWeaponItem(createItemType.CSI());
			ingredients = Main.recipe
				.Where((r, index) => r.createItem.type == createItemType && (r.createItem.type > ItemID.Count || index <= VANILLA_RECIPE_COUNT))
				.Select(r => r.requiredItem.Select(item => 
					!typesQue.Contains(item.type) ? 
						(isWeapon || !IsWeaponItem(item)) && TryGetAllCraftingIngredientTypes(item.type, out HashSet<int> ingredientSet) ?
							ingredientSet 
						: 
							isWeapon ?
								new HashSet<int>() { item.type } 
							: 
								new HashSet<int>() 
					: 
						new HashSet<int>()))
				.SelectMany(hashSetList => hashSetList)
				.SelectMany(hashSet => hashSet)
				.ToHashSet();

			if (LogMethods.debugging) $"/\\TryGetAllCraftingIngredientTypes({createItemType.CSI().Name}); {typesQue.Select(t => t.CSI().Name).JoinList(", ")}; {ingredients.Select(i => i.CSI().Name).JoinList(", ")}".Log();
			if (isOriginal) {
				weaponBeingChecked = -1;
				typesQue.Clear();
			}

			return ingredients.Count > 0;
		}
		*/
		/*
		public static bool TryGetAllCraftingIngredientTypes(int createItemType, out HashSet<int> ingredients) {
			if (LogMethods.debugging) $"\\/TryGetAllCraftingIngredientTypes({createItemType.CSI().Name}); {typesQue.Select(t => t.CSI().Name).JoinList(", ")}".Log();
			bool isOriginal = false;
			if (weaponBeingChecked == -1) {
				weaponBeingChecked = createItemType;
				isOriginal = true;
			}
			else if (typesQue.Any() && !weaponIngredients.Contains(createItemType) || typesQue.Contains(createItemType)) {
				ingredients = null;
				if (LogMethods.debugging) $"/\\TryGetAllCraftingIngredientTypes({createItemType.CSI().Name}); Removed, {typesQue.Select(t => t.CSI().Name).JoinList(", ")}".Log();

				return false;
			}

			typesQue.Add(createItemType);
			bool isWeapon = IsWeaponItem(createItemType.CSI());
			ingredients = Main.recipe
				.Where((r, index) => r.createItem.type == createItemType && (r.createItem.type > ItemID.Count || index <= VANILLA_RECIPE_COUNT))
				.Select(r => 
					r.requiredItem.Concat(
						r.requiredTile
							.Select(tile => WEGlobalTile.GetDroppedItem(tile))
							.Where(type => type > 0)
							.Select(type => type.CSI())
					).ToHashSet()
				.Select(item =>
					!typesQue.Contains(item.type) ?
						(isWeapon || !IsWeaponItem(item)) && TryGetAllCraftingIngredientTypes(item.type, out HashSet<int> ingredientSet) ?
							ingredientSet
						:
							isWeapon ?
								new HashSet<int>() { item.type }
							:
								new HashSet<int>()
					:
						new HashSet<int>()))
				.SelectMany(hashSetList => hashSetList)
				.SelectMany(hashSet => hashSet)
				.ToHashSet();

			if (LogMethods.debugging) $"/\\TryGetAllCraftingIngredientTypes({createItemType.CSI().Name}); {typesQue.Select(t => t.CSI().Name).JoinList(", ")}; {ingredients.Select(i => i.CSI().Name).JoinList(", ")}".Log();
			if (isOriginal) {
				weaponBeingChecked = -1;
				typesQue.Clear();
			}

			return ingredients.Count > 0;
		}
		*/
		/*
		public static bool TryGetAllCraftingIngredientTypes(int createItemType, out HashSet<int> ingredients) {
			//Try splitting this into more simple functions?
			if (LogMethods.debugging) $"\\/TryGetAllCraftingIngredientTypes({createItemType.CSI().Name}); {typesQue.Select(t => t.CSI().Name).JoinList(", ")}".Log();
			bool isOriginal = false;
			string name = createItemType.CSI().Name;//Temp
			if (weaponBeingChecked == -1) {
				weaponBeingChecked = createItemType;
				isOriginal = true;
			}
			else if (typesQue.Any() && !weaponIngredients.Contains(createItemType)) {
				ingredients = null;
				if (LogMethods.debugging) $"/\\TryGetAllCraftingIngredientTypes({createItemType.CSI().Name}); Removed, {typesQue.Select(t => t.CSI().Name).JoinList(", ")}".Log();

				return false;
			}
			else if (reverseCraftableIngredients.Keys.Contains(createItemType)) {
				if (reverseCraftableIngredients[createItemType].Where(t => typesQue.Contains(t)).Any()) {
					ingredients = null;
					if (LogMethods.debugging) $"/\\TryGetAllCraftingIngredientTypes({createItemType.CSI().Name}); Removed, {typesQue.Select(t => t.CSI().Name).JoinList(", ")}".Log();

					return false;
				}
			}

			bool isWeapon = IsWeaponItem(createItemType.CSI());
			bool isCraftingStation = craftingStations.Contains(createItemType);
			bool isUniqueItem = isWeapon || isCraftingStation;
			if (!isWeapon)
				typesQue.Add(createItemType);

			ingredients = Main.recipe
				.Where((r, index) => r.createItem.type == createItemType && (r.createItem.type > ItemID.Count || index <= VANILLA_RECIPE_COUNT))
				.Select(r =>
					r.requiredItem.Concat(
						r.requiredTile
							.Select(tile => WEGlobalTile.GetDroppedItem(tile))
							.Where(type => type > 0)
							.Select(type => type.CSI())
					).ToHashSet()
				.Select(item =>
					!typesQue.Contains(item.type) ?
						(isWeapon || !IsWeaponItem(item)) && TryGetAllCraftingIngredientTypes(item.type, out HashSet<int> ingredientSet) ?
							ingredientSet
						:
							isWeapon ?
								new HashSet<int>() { item.type }
							:
								new HashSet<int>()
					:
						new HashSet<int>()))
				.SelectMany(hashSetList => hashSetList)
				.SelectMany(hashSet => hashSet)
				.ToHashSet();

			if (LogMethods.debugging) $"/\\TryGetAllCraftingIngredientTypes({createItemType.CSI().Name}); {typesQue.Select(t => t.CSI().Name).JoinList(", ")}; {ingredients.Select(i => i.CSI().Name).JoinList(", ")}".Log();
			if (isOriginal) {
				weaponBeingChecked = -1;
				typesQue.Clear();
			}

			return ingredients.Count > 0;
		}
		*/
		private static bool TryGetAllCraftingIngredientTypes(int createItemType, out HashSet<HashSet<int>> ingredients) {
			ingredients = new();
			Item createItem = createItemType.CSI();
			string name = createItemType.CSI().Name;//Temp
			bool isWeapon = IsWeaponItem(createItem);
			//IEnumerable<Recipe> recipies = Main.recipe.Where(r => r.createItem.type == createItemType);//Swap to this after all testing
			IEnumerable<Recipe> recipies = Main.recipe.Where((r, index) => r.createItem.type == createItemType && (r.createItem.type > ItemID.Count || index <= VANILLA_RECIPE_COUNT));
			HashSet<HashSet<HashSet<int>>> requiredItemTypeLists = new();
			foreach (Recipe recipe in recipies) {
				if (!isWeapon && recipe.IsReverseCraftable())
					continue;

				HashSet<HashSet<int>> requiredItemTypes = new();
				foreach (Item ingredientItem in recipe.requiredItem) {
					int ingredientType = ingredientItem.type;
					if (weaponIngredients.Contains(ingredientType)) {
						if (TryGetAllCraftingIngredientTypes(ingredientType, out HashSet<HashSet<int>> ingredientTypes)) {
							string ingredientTypesNames = ingredientTypes.Select(set => set.Select(t => t.CSI().S()).JoinList(" or ")).JoinList(", ");
							requiredItemTypes.CombineHashSet(ingredientTypes);
						}
						else {
							requiredItemTypes.TryAdd(new() { ingredientType });
						}

						if (requiredItemTypes.Count <= 0)
							continue;

						//ingredients.CombineHashSet(requiredItemTypes);

						foreach (Item requiredTyleItem in recipe.requiredTile.Select(tile => WEGlobalTile.GetDroppedItem(tile)).Where(type => type > 0).Select(type => type.CSI())) {
							int requiredTyleItemType = requiredTyleItem.type;
							//if (weaponIngredients.Contains(ingredientType))
							requiredItemTypes.TryAdd(new() { requiredTyleItemType });
						}

						requiredItemTypeLists.Add(requiredItemTypes);
					}
					else {
						ingredients.TryAdd(new() { ingredientType });
					}
				}
			}

			ingredients = ingredients.CombineIngredientLists(requiredItemTypeLists);

			string ingredientTypesNames2 = $"{createItem.S()}: {ingredients.Select(set => set.Select(t => t.CSI().S()).JoinList(" or ")).JoinList(", ")}";
			return ingredients.Count > 0;
			/*
			//Try splitting this into more simple functions?
			if (LogMethods.debugging) $"\\/TryGetAllCraftingIngredientTypes({createItemType.CSI().Name}); {typesQue.Select(t => t.CSI().Name).JoinList(", ")}".Log();
			bool isOriginal = false;
			if (weaponBeingChecked == -1) {
				weaponBeingChecked = createItemType;
				isOriginal = true;
			}
			else if (typesQue.Any() && !weaponIngredients.Contains(createItemType)) {
				ingredients = null;
				if (LogMethods.debugging) $"/\\TryGetAllCraftingIngredientTypes({createItemType.CSI().Name}); Removed, {typesQue.Select(t => t.CSI().Name).JoinList(", ")}".Log();

				return false;
			}
			else if (reverseCraftableIngredients.Keys.Contains(createItemType)) {
				if (reverseCraftableIngredients[createItemType].Where(t => typesQue.Contains(t)).Any()) {
					ingredients = null;
					if (LogMethods.debugging) $"/\\TryGetAllCraftingIngredientTypes({createItemType.CSI().Name}); Removed, {typesQue.Select(t => t.CSI().Name).JoinList(", ")}".Log();

					return false;
				}
			}

			bool isWeapon = IsWeaponItem(createItemType.CSI());
			bool isCraftingStation = craftingStations.Contains(createItemType);
			bool isUniqueItem = isWeapon || isCraftingStation;
			if (!isWeapon)
				typesQue.Add(createItemType);

			ingredients = Main.recipe
				.Where((r, index) => r.createItem.type == createItemType && (r.createItem.type > ItemID.Count || index <= VANILLA_RECIPE_COUNT))
				.Select(r =>
					r.requiredItem.Concat(
						r.requiredTile
							.Select(tile => WEGlobalTile.GetDroppedItem(tile))
							.Where(type => type > 0)
							.Select(type => type.CSI())
					).ToHashSet()
				.Select(item =>
					!typesQue.Contains(item.type) ?
						(isWeapon || !IsWeaponItem(item)) && TryGetAllCraftingIngredientTypes(item.type, out HashSet<int> ingredientSet) ?
							ingredientSet
						:
							isWeapon ?
								new HashSet<int>() { item.type }
							:
								new HashSet<int>()
					:
						new HashSet<int>()))
				.SelectMany(hashSetList => hashSetList)
				.SelectMany(hashSet => hashSet)
				.ToHashSet();

			if (LogMethods.debugging) $"/\\TryGetAllCraftingIngredientTypes({createItemType.CSI().Name}); {typesQue.Select(t => t.CSI().Name).JoinList(", ")}; {ingredients.Select(i => i.CSI().Name).JoinList(", ")}".Log();
			if (isOriginal) {
				weaponBeingChecked = -1;
				typesQue.Clear();
			}

			return ingredients.Count > 0;
			*/
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
			weaponsList.Clear();
			craftingStations.Clear();
			allWeaponRecipies.Clear();
			weaponIngredients.Clear();
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
}
