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
						return baseCraftingInfusionPower + infusionPowerOffset;
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
		public const int VANILLA_RECIPE_COUNT = 2700;
		private static bool finishedSetup = false;
		public static SortedDictionary<int, InfusionPowerSource> WeaponSources { get; private set; } = new();//Not cleared
		public static SortedDictionary<InfusionPowerSource, int> SourceInfusionPowers { get; private set; } = new();//Not cleared
		private static SortedDictionary<int, HashSet<int>> allWeaponRecipies = new();
		private static HashSet<int> weaponsList = new();
		public static HashSet<int> weaponIngredients = new();
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
			{ ItemID.CrystalShard, 8 },
			{ ItemID.SoulofLight, 8 },
			{ ItemID.BonePlatform, 8 },
			{ ItemID.BoneBlockWall, 8 },
			{ ItemID.CopperBar, 10 },
			{ ItemID.RedBrick, 10 },
			{ ItemID.Cactus, 10 },
			{ ItemID.TinBar, 10 },
			{ ItemID.IronBar, 20 },
			{ ItemID.Bomb, 20 },
			{ ItemID.WaterBucket, 20 },
			{ ItemID.IllegalGunParts, 20 },
			{ ItemID.SoulofFright, 20 },
			{ ItemID.LeadBar, 20 },
			{ ItemID.VilePowder, 25 },
			{ ItemID.Silk, 25 },
			{ ItemID.SandstoneBrick, 25 },
			{ ItemID.IceBlock, 25 },
			{ ItemID.ViciousPowder, 25 },
			{ ItemID.Shiverthorn, 30 },
			{ ItemID.MeteoriteBar, 50 },
			{ ItemID.Amethyst, 50 },
			{ ItemID.Grenade, 51 },
			{ ItemID.SilverBar, 55 },
			{ ItemID.Topaz, 55 },
			{ ItemID.AntlionMandible, 55 },
			{ ItemID.TungstenBar, 55 },
			{ ItemID.Sapphire, 60 },
			{ ItemID.JungleRose, 60 },
			{ ItemID.Emerald, 65 },
			{ ItemID.PinkGel, 70 },
			{ ItemID.GoldBar, 75 },
			{ ItemID.Ruby, 75 },
			{ ItemID.PlatinumBar, 75 },
			{ ItemID.Mace, 78 },
			{ ItemID.LifeCrystal, 85 },
			{ ItemID.Diamond, 85 },
			{ ItemID.FlinxFur, 90 },
			{ ItemID.Amber, 100 },
			{ ItemID.FossilOre, 100 },
			{ ItemID.DemoniteBar, 105 },
			{ ItemID.HellstoneBar, 105 },
			{ ItemID.Stinger, 105 },
			{ ItemID.Vine, 105 },
			{ ItemID.SoulofMight, 105 },
			{ ItemID.SoulofSight, 105 },
			{ ItemID.ChlorophyteBar, 105 },
			{ ItemID.HallowedBar, 105 },
			{ ItemID.CrimtaneBar, 105 },
			{ ItemID.CorruptSeeds, 120 },
			{ ItemID.BottledWater, 120 },
			{ ItemID.HallowedSeeds, 120 },
			{ ItemID.EbonsandBlock, 120 },
			{ ItemID.PixieDust, 120 },
			{ ItemID.CrimsandBlock, 120 },
			{ ItemID.CrimsonSeeds, 120 },
			{ ItemID.FallenStar, 157 },
			{ ItemID.TissueSample, 160 },
			{ ItemID.ShadowScale, 162 },
			{ ItemID.Minishark, 222 },
			{ ItemID.BeeWax, 243 },
			{ ItemID.MagicMissile, 251 },
			{ ItemID.Muramasa, 251 },
			{ ItemID.Handgun, 251 },
			{ ItemID.EnchantedSword, 258 },
			{ ItemID.Pearlwood, 400 },
			{ ItemID.CobaltBar, 418 },
			{ ItemID.AdamantiteBar, 420 },
			{ ItemID.SoulofNight, 420 },
			{ ItemID.SpiderFang, 420 },
			{ ItemID.AncientBattleArmorMaterial, 420 },
			{ ItemID.DjinnLamp, 420 },
			{ ItemID.PalladiumBar, 424 },
			{ ItemID.MythrilBar, 427 },
			{ ItemID.OrichalcumBar, 433 },
			{ ItemID.TitaniumBar, 442 },
			{ ItemID.DarkShard, 453 },
			{ ItemID.LightShard, 453 },
			{ ItemID.CursedFlame, 460 },
			{ ItemID.SpellTome, 460 },
			{ ItemID.PoisonStaff, 460 },
			{ ItemID.Ichor, 460 },
			{ ItemID.FrostCore, 460 },
			{ ItemID.Shotgun, 467 },
			{ ItemID.SharkFin, 606 },
			{ ItemID.BlackLens, 628 },
			{ ItemID.Harp, 628 },
			{ ItemID.UnicornHorn, 628 },
			{ ItemID.ShroomiteBar, 678 },
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
			{ "Aerialite Bar", 0 },
			{ "Core of Calamity", 0 },
			{ "Cosmilite Bar", 0 },
			{ "Life Alloy", 0 },
			{ "Astral Bar", 0 },
			{ "Auric Bar", 10 },
			{ "Dark Plasma", 10 },
			{ "Lumenyl", 10 },
			{ "Meld Construct", 10 },
			{ "Uelibloom Bar", 10 },
			{ "Voidstone", 10 },
			{ "Depth Cells", 20 },
			{ "Divine Geode", 20 },
			{ "Essence of Sunlight", 20 },
			{ "Pearl Shard", 20 },
			{ "Plague Cell Canister", 20 },
			{ "Ruinous Soul", 20 },
			{ "Unholy Core", 20 },
			{ "Exodium Cluster", 20 },
			{ "Sea Prism", 20 },
			{ "Sulphurous Sand", 20 },
			{ "Bloodstone Core", 25 },
			{ "Desert Feather", 25 },
			{ "Endothermic Energy", 25 },
			{ "Shadowspec Bar", 25 },
			{ "Core of Sunlight", 30 },
			{ "Cryonic Bar", 30 },
			{ "Essence of Havoc", 30 },
			{ "Wulfrum Metal Scrap", 40 },
			{ "Energy Core", 43 },
			{ "Depth Crusher", 47 },
			{ "Corroded Fossil", 55 },
			{ "Sulphuric Scale", 55 },
			{ "Navystone", 55 },
			{ "Phantoplasm", 60 },
			{ "Unholy Essence", 60 },
			{ "Sea Remains", 87 },
			{ "Scourge of the Desert", 87 },
			{ "Stormlion Mandible", 97 },
			{ "Armored Shell", 100 },
			{ "Core of Havoc", 105 },
			{ "Perennial Bar", 105 },
			{ "Astral Monolith", 120 },
			{ "Astral Sand", 120 },
			{ "Acidwood", 125 },
			{ "Blighted Gel", 160 },
			{ "Blood Sample", 215 },
			{ "Rotten Matter", 215 },
			{ "Dubious Plating", 235 },
			{ "Mysterious Circuitry", 235 },
			{ "Bladecrest Oathsword", 245 },
			{ "Glaive", 245 },
			{ "Staff of Necrosteocytes", 250 },
			{ "Meowthrower", 415 },
			{ "P90", 415 },
			{ "Blast Barrel", 415 },
			{ "Needler", 420 },
			{ "Prototype Plasma Drive Core", 440 },
			{ "Suspicious Scrap", 440 },
			{ "Flak Toxicannon", 600 },
			{ "Hoarfrost Bow", 600 },
			{ "Ashes of Calamity", 700 },
			{ "Crushsaw Crasher", 700 },
			{ "Smooth Voidstone", 710 },
			{ "Core of Eleum", 750 },
			{ "Infected Armor Plating", 860 },
			{ "Scoria Bar", 860 },
			{ "Nullification Pistol", 880 },
			{ "Planty Mush", 885 },
			{ "Wrath of the Ancients", 900 },
			{ "Utensil Poker", 1070 },
			{ "Demonic Bone Ash", 1105 },
			{ "Effulgent Feather", 1150 },
			{ "Golden Eagle", 1150 },
			{ "Purge Guzzler", 1180 },
			{ "Blissful Bombardier", 1180 },
			{ "Telluric Glare", 1180 },
			{ "Molten Amputator", 1180 },
			{ "Aureus Cell", 1210 },
			{ "The Storm", 1270 },
			{ "Twisting Nether", 1300 },
			{ "Cosmic Kunai", 1300 },
			{ "Reaper Tooth", 1330 },
			{ "Ghoulish Gouger", 1330 },
			{ "Calamari's Lament", 1330 },
			{ "Ethereal Subjugator", 1330 },
			{ "Fetid Emesis", 1400 },
			{ "Nightmare Fuel", 1450 },
			{ "Excelsus", 1450 },
			{ "The Obliterator", 1450 },
			{ "Eradicator", 1450 },
			{ "Ascendant Spirit Essence", 1500 },
			{ "Yharon Soul Fragment", 1550 },
			{ "Wrathwing", 1550 },
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
					foreach (int ingredientType in allWeaponRecipies[weaponType]) {
						//Check if weapon's infusion power lowers the infusion power of the ingredient
						Item ingredientSampleItem = ingredientType.CSI();
						//If already an entry in VanillaCraftingItemSourceInfusionPowers or ModdedCraftingItemSourceInfusionPowers, use it instead of guessing
						if (VanillaCraftingItemSourceInfusionPowers.ContainsKey(ingredientType)) {
							if (!guessedSourceInfusionPowers.ContainsKey(ingredientType))
								guessedSourceInfusionPowers.Add(ingredientType, VanillaCraftingItemSourceInfusionPowers[ingredientType]);

							continue;
						}
						else if (ModdedCraftingItemSourceInfusionPowers.ContainsKey(ingredientSampleItem.Name)) {
							if (!guessedSourceInfusionPowers.ContainsKey(ingredientType))
								guessedSourceInfusionPowers.Add(ingredientType, ModdedCraftingItemSourceInfusionPowers[ingredientSampleItem.Name]);

							continue;
						}

						int infusionPower = infusionPowerWeapon;
						if (ingredientSampleItem.TryGetEnchantedItem(out EnchantedWeapon enchantedWeaponIngredient))
							infusionPower = enchantedWeaponIngredient.GetWeaponInfusionPower();

						if (ingredientType < ItemID.Count && weaponType >= ItemID.Count)//Don't allow modded weapon recipies to affect vanilla ingredients
							continue;

						if (guessedSourceInfusionPowers.ContainsKey(ingredientType)) {
							int currentInfusionPower = guessedSourceInfusionPowers[ingredientType];
							if (currentInfusionPower > infusionPower)
								guessedSourceInfusionPowers[ingredientType] = infusionPower;
						}
						else {
							guessedSourceInfusionPowers.Add(ingredientType, infusionPower);
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
					if (!allWeaponRecipies[weaponKey].Contains(key))
						continue;

					foreach (int ingredientType in allWeaponRecipies[weaponKey]) {
						if (guessedSourceInfusionPowers.ContainsKey(ingredientType)) {
							int ingredientInfusionPower = guessedSourceInfusionPowers[ingredientType];
							string ingredientName = ingredientType.CSI().Name;//temp
							if (ingredientName == "Staff of the Mechworm")//temp
								Main.NewText("Staff of the Mechworm");//temp

							if (infusionPower < ingredientInfusionPower) {// || infusionPower == ingredientInfusionPower && IsWeaponItem(key.CSI())) {//Works but not needed?
								isLimmitingItemForThisItem = false;
								break;
							}
						}
					}

					if (isLimmitingItemForThisItem) {
						isLimmitingItem = true;
						break;
					}
				}
				//Not working
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
		private static void GetAllWeaponIngredients() {
			foreach (Recipe recipe in Main.recipe.Where((r, index) => weaponsList.Contains(r.createItem.type) && (r.createItem.type > ItemID.Count || index <= VANILLA_RECIPE_COUNT))) {
				foreach (int type in recipe.requiredItem.Select(i => i.type)) {
					weaponIngredients.Add(type);
				}
			}
		}
		private static void GetAllCraftingResources() {
			foreach (int weaponType in weaponsList) {
				if (TryGetAllCraftingIngredientTypes(weaponType, out HashSet<int> ingredients))
					allWeaponRecipies.Add(weaponType, ingredients);
			}

			if (guessingInfusionPowers) {
				$"\n{
					allWeaponRecipies.Select(weapon => $"{weapon.Key.CSI().S()}:{
						weapon.Value.Select(ingredient => $" {ingredient.CSI().Name}").JoinList(", ")
					}").JoinList("\n")
				}".LogSimple();
			}
		}
		public static List<int> typesQue = new();
		public static int weaponBeingChecked = -1;
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
		private static void PopulateCraftingWeaponSources() {
			foreach (int weaponType in allWeaponRecipies.Keys) {
				InfusionPowerSource highestInfusionPowerSource = new();
				int infusionPower = -1;
				foreach (int ingredientType in allWeaponRecipies[weaponType]) {
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

				if (infusionPower >= 0)
					WeaponSources.Add(weaponType, highestInfusionPowerSource);
			}
		}
		private static void ClearTempDictionaries() {
			weaponsList.Clear();
			allWeaponRecipies.Clear();
			weaponIngredients.Clear();
			finishedSetup = true;
		}

		public static bool TryGetBaseInfusionPower(Item item, out int baseInfusionPower) {
			int weaponType = item.type;
			if (WeaponSources.ContainsKey(weaponType)) {
				baseInfusionPower = WeaponSources[weaponType].InfusionPower;

				return true;
			}

			baseInfusionPower = 0;
			return false;
		}
	}
}
