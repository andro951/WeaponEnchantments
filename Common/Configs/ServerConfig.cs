using System;
using System.Collections.Generic;
using System.ComponentModel;
using Terraria.ModLoader.Config;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using WeaponEnchantments.Common.Utility;
using Terraria.ID;
using WeaponEnchantments.Common.Globals;
using androLib.Common.Utility;

namespace WeaponEnchantments.Common.Configs
{
	#region  Server configs
	public class ServerConfig : ModConfig
	{
		public const string ServerConfigName = "ServerConfig";
		public override ConfigScope Mode => ConfigScope.ServerSide;

		//Server Config
		[JsonIgnore]
		public const string ServerConfigKey = ServerConfigName;
		[Header($"$Mods.{WEMod.ModName}.{L_ID_Tags.Configs}.{ServerConfigName}.{ServerConfigKey}")]
		[ReloadRequired]
		public PresetData presetData;

		[JsonIgnore]
		public const string IndividualEnchantmentStrengthsKey = "IndividualEnchantmentStrengths";
		[Header($"$Mods.{WEMod.ModName}.{L_ID_Tags.Configs}.{ServerConfigName}.{IndividualEnchantmentStrengthsKey}")]

		[ReloadRequired]
		[DefaultValue(false)]
		public bool individualStrengthsEnabled;

		public List<Pair> individualStrengths = new List<Pair>();

		//Enchantment Settings
		[JsonIgnore]
		public const string EnchantmentSettingsKey = "EnchantmentSettings";
		[Header($"$Mods.{WEMod.ModName}.{L_ID_Tags.Configs}.{ServerConfigName}.{EnchantmentSettingsKey}")]
		[DefaultValue(true)]
		public bool AlwaysOverrideDamageType;

		[DefaultValue(100)]
		[Range(0, 10000)]
		public int AffectOnVanillaLifeStealLimmit;

		[Range(0, 10000)]
		[DefaultValue(10)]
		[ReloadRequired]
		public int AttackSpeedEnchantmentAutoReuseSetpoint;

		[DefaultValue(true)]
		public bool AutoReuseDisabledOnMagicMissile;

		[DefaultValue(15)]
		[Range(1, 600)]
		[ReloadRequired]
		public int BuffDuration;

		[DefaultValue(5)]
		[Range(0, 1000000)]
		public int AmaterasuSelfGrowthPerTick;

		[DefaultValue(false)]
		[ReloadRequired]
		public bool ReduceRecipesToMinimum;

		[DefaultValue(100)]
		[Range(0, 1400)]
		[ReloadRequired]
		public int ConfigCapacityCostMultiplier;

		[DefaultValue(false)]
		[ReloadRequired]
		public bool RemoveEnchantmentRestrictions;

		[DefaultValue(67)]
		[Range(0, 100)]
		[ReloadRequired]
		public int EnchantmentStrengthCurseScaling;

		[DefaultValue(100)]
		[Range(0, 100000)]
		[ReloadRequired]
		public int CurseStrengthMultiplier;

		[DefaultValue(true)]
		public bool EnchantmentEffectsOnModdedAccessorySlots;

		//Essence and Experience
		[JsonIgnore]
		public const string EssenceAndExperienceKey = "EssenceAndExperience";
		[Header($"$Mods.{WEMod.ModName}.{L_ID_Tags.Configs}.{ServerConfigName}.{EssenceAndExperienceKey}")]

		[Range(0, 10000)]
		[DefaultValue(100)]
		[ReloadRequired]
		public int BossEssenceMultiplier;

		[Range(0, 10000)]
		[DefaultValue(100)]
		[ReloadRequired]
		public int EssenceMultiplier;

		[Range(0, 10000)]
		[DefaultValue(100)]
		public int BossExperienceMultiplier;

		[Range(0, 10000)]
		[DefaultValue(100)]
		public int ExperienceMultiplier;

		[Range(0, 10000)]
		[DefaultValue(100)]
		public int GatheringExperienceMultiplier;

		[DefaultValue(10)]
		[Range(1, 100)]
		public int EssenceGrabRange;

		//Enchantment Drop Rates(%)
		[JsonIgnore]
		public const string EnchantmentDropRatesKey = "EnchantmentDropRates(%)";
		[Header($"$Mods.{WEMod.ModName}.{L_ID_Tags.Configs}.{ServerConfigName}.{EnchantmentDropRatesKey}")]

		[Range(0, 100)]
		[DefaultValue(50)]
		[ReloadRequired]
		public int BossEnchantmentDropChance;

		[Range(0, 1000)]
		[DefaultValue(100)]
		[ReloadRequired]
		public int EnchantmentDropChance;

		[Range(0, 100000)]
		[DefaultValue(50)]
		public int ChestSpawnChance;

		[Range(0, 10000)]
		[DefaultValue(100)]
		[ReloadRequired]
		public int CrateDropChance;

		//Other Drop Rates
		[JsonIgnore]
		public const string OtherDropRatesKey = "OtherDropRates";
		[Header($"$Mods.{WEMod.ModName}.{L_ID_Tags.Configs}.{ServerConfigName}.{OtherDropRatesKey}")]

		[DefaultValue(true)]
		[ReloadRequired]
		public bool PreventPowerBoosterFromPreHardMode;

		//Enchanting Table Options
		[JsonIgnore]
		public const string EnchantingTableOptionsKey = "EnchantingTableOptions";
		[Header($"$Mods.{WEMod.ModName}.{L_ID_Tags.Configs}.{ServerConfigName}.{EnchantingTableOptionsKey}")]

		[DefaultValue(true)]
		public bool AllowHighTierOres;

		[DefaultValue(5)]
		[Range(0, 5)]
		[ReloadRequired]
		public int EnchantmentSlotsOnWeapons;

		[DefaultValue(3)]
		[Range(0, 5)]
		[ReloadRequired]
		public int EnchantmentSlotsOnArmor;

		[DefaultValue(1)]
		[Range(0, 5)]
		[ReloadRequired]
		public int EnchantmentSlotsOnAccessories;

		[DefaultValue(5)]
		[Range(0, 5)]
		[ReloadRequired]
		public int EnchantmentSlotsOnFishingPoles;

		[DefaultValue(5)]
		[Range(0, 5)]
		[ReloadRequired]
		public int EnchantmentSlotsOnTools;

		[DefaultValue(50)]
		[Range(0, 100)]
		public int PercentOfferEssence;

		[DefaultValue(false)]
		public bool ReduceOfferEfficiencyByTableTier;

		[DefaultValue(false)]
		public bool ReduceOfferEfficiencyByBaseInfusionPower;

		[JsonIgnore]
		public const int DefaultSiphonCost = 20;
		[DefaultValue(DefaultSiphonCost)]
		public int SiphonExperiencePercentCost;

		//Curses
		[JsonIgnore]
		public const string CursedEnemiesKey = "CursedEnemies";
		[Header($"$Mods.{WEMod.ModName}.{L_ID_Tags.Configs}.{ServerConfigName}.{CursedEnemiesKey}")]

		[DefaultValue(true)]
		public bool AllowCursedEnemies;

		[DefaultValue(500)]
		[Range(0, 10000000)]
		public int CursedEnemyLifeMultiplier;

		[DefaultValue(0)]
		[Range(0, 10000000)]
		public int CursedEnemyDamageMultiplier;

		[DefaultValue(100)]
		[Range(0, 10000000)]
		public int CursedEssenceDropChanceMultiplier;

		[DefaultValue(40000)]
		[Range(0, 10000000)]
		[ReloadRequired]
		public int CursedEnemyDebuffAttackRange;

		[DefaultValue(100)]
		[Range(0, 10000)]
		[ReloadRequired]
		public int CursedEnemyDebuffDurationMultiplier;

		[DefaultValue(100)]
		[Range(0, 10000)]
		[ReloadRequired]
		public int CursedEnemyDebuffChanceMultiplier;

		[DefaultValue(20)]
		[Range(1, 36000)]
		public int CuredEnemyDebuffTicksPerAttack;

		[DefaultValue(100)]
		[Range(0, 1000)]
		public int CursedBuffSpawnRateMultiplier;

		[DefaultValue(false)]
		[ReloadRequired]
		public bool CursedEnchantmentsAllowedOnSummons;

		//General Game Changes
		[JsonIgnore]
		public const string GeneralGameChangesKey = "GeneralGameChanges";
		[Header($"$Mods.{WEMod.ModName}.{L_ID_Tags.Configs}.{ServerConfigName}.{GeneralGameChangesKey}")]

		[DefaultValue(false)]
		public bool DisableMinionCrits;

		[DefaultValue(false)]
		[ReloadRequired]
		public bool CritPerLevelDisabled;

		[DefaultValue(false)]
		[ReloadRequired]
		public bool DamagePerLevelInstead;

		[DefaultValue(false)]
		[ReloadRequired]
		public bool DamageReductionPerLevelDisabled;

		[DefaultValue(false)]
		public bool CalculateDamageReductionBeforeDefense;

		[ReloadRequired]
		public List<ArmorDamageReduction> ArmorDamageReductions = DefaultArmorDamageReductions;
		[JsonIgnore]
		public static List<ArmorDamageReduction> DefaultArmorDamageReductions => new() { new(0), new(1), new(2), new(3) };
		[JsonIgnore]
		public static int DefaultArmorDamageReductionsCount = DefaultArmorDamageReductions.Count;

		[DefaultValue(true)]
		public bool AllowCriticalChancePast100;

		[DefaultValue(false)]
		public bool MultiplicativeCriticalHits;

		[DefaultValue(1300)]
		[Range(1000, 2000)]
		[ReloadRequired]
		public int InfusionDamageMultiplier;

		[ReloadRequired]
		[DefaultValue(false)]
		public bool DisableArmorInfusion;

		[DefaultValue(50)]
		[Range(0, 100)]
		public int MinionLifeStealMultiplier;

		[DefaultValue(1000)]
		[Range(0, 10000)]
		public int NegativeDefensePenaltyMultiplier;

		//Random Extra Stuff
		[JsonIgnore]
		public const string RandomExtraStuffKey = "RandomExtraStuff";
		[Header($"$Mods.{WEMod.ModName}.{L_ID_Tags.Configs}.{ServerConfigName}.{RandomExtraStuffKey}")]

		[DefaultValue(false)]
		public bool DCUStart;

		[DefaultValue(false)]
		[ReloadRequired]
		public bool DisableResearch;

		[DefaultValue(false)]
		[ReloadRequired]
		public bool PrintWikiInfo {
			set {
				if (value) {
					PreventPowerBoosterFromPreHardMode = false;
					presetData.Preset = "Normal";
				}

				printWikiInfo = value;

			}

			get => printWikiInfo;
		}

		private bool printWikiInfo;

		//private void OnPrintWikiInfoChanged(bool newValue) {
		//	PrintWikiInfo = newValue;
		//}

		public ServerConfig() {
			presetData = new PresetData();
			//PrintWikiInfoChanged += OnPrintWikiInfoChanged;
		}

		[OnDeserialized]
		internal void OnDeserializedMethod(StreamingContext context) {
			// If you change ModConfig fields between versions, your users might notice their configuration is lost when they update their mod.
			// We can use [JsonExtensionData] to capture serialized data and manually restore them to new fields.
			// Imagine in a previous version of this mod, we had a field "OldmodifiedEnchantmentStrengths" and we want to preserve that data in "modifiedEnchantmentStrengths".
			// To test this, insert the following into ExampleMod_ModConfigShowcase.json: "OldmodifiedEnchantmentStrengths": [ 99, 999],
			/*if (_additionalData.TryGetValue("OldmodifiedEnchantmentStrengths", out var token))
            {
                var OldmodifiedEnchantmentStrengths = token.ToObject<List<int>>();
                modifiedEnchantmentStrengths.AddRange(OldmodifiedEnchantmentStrengths);
            }
            _additionalData.Clear(); // make sure to clear this or it'll crash.*/
		}
	}

	//[SeparatePage]
	public class EnchantmentToggle : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ServerSide;
		public const string EnchantToggleName = "ServerConfig";
		
		#region General enchantments
		[JsonIgnore]
		public const string GeneralConfigKey = "GeneralEnchantToggleName";
		
		[Header($"$Mods.{WEMod.ModName}.{L_ID_Tags.Configs}.{EnchantToggleName}.{GeneralConfigKey}")]
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool AllForOne;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool AttackSpeed;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool CriticalStrikeChance;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool CriticalStrikeDamage;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool Damage;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool DamageReduction;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool Defense;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool ExtraFishingLine;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool LethalCombination;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool LifeSteal;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool MaxMinions;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool OneForAll;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool PercentArmorPenetration;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool PrideOfTheWeak;
		#endregion
		
		#region Status effect enchantments
		[JsonIgnore]
		public const string StatusConfigKey = "StatusEnchantToggleName";
		
		[Header($"$Mods.{WEMod.ModName}.{L_ID_Tags.Configs}.{EnchantToggleName}.{StatusConfigKey}")]
		
		#region Vanilla status effects
		[ReloadRequired]
		[DefaultValue(true)]
		public bool OnFire;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool Poison;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool Frostburn;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool CursedInferno;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool Ichor;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool Venom;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool Shadowflame;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool Daybreak;
		#endregion
		
		#region Custom status effects
		[ReloadRequired]
		[DefaultValue(true)]
		public bool WorldAblaze;
		#endregion
		
		#endregion

		#region Utility enchantments
		[JsonIgnore]
		public const string UtilityConfigKey = "UtilityToggleName";
		
		[Header($"$Mods.{WEMod.ModName}.{L_ID_Tags.Configs}.{EnchantToggleName}.{UtilityConfigKey}")]
		[ReloadRequired]
		[DefaultValue(true)]
		public bool AmmoCost;

		[ReloadRequired]
		[DefaultValue(true)]
		public bool CalmWaters;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool CrateChance;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool FishingPower;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool LavaFishing;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool LifeRegen;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool Luck;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool Magnetic;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool MobilityControl;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool MovementSpeed;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool Peace;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool PennyPinching;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool ProjectileVelocity;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool ReducedManaUsage;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool Size;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool Time;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool War;
		
		#region On tick/potion buffs
		[JsonIgnore]
		public const string OnTickUtilityConfigKey = "OnTickUtilityToggleName";

		[Header($"$Mods.{WEMod.ModName}.{L_ID_Tags.Configs}.{EnchantToggleName}.{OnTickUtilityConfigKey}")]
		[ReloadRequired]
		[DefaultValue(true)]
		public bool OnTick;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool Dangersense;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool Hunter;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool ObsidianSkin;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool Spelunker;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool Fishing;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool Crate;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool Sonar;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool Shine;
		
		#endregion
		#endregion
		
		#region Unique enchanements
		[JsonIgnore]
		public const string UniqueConfigKey = "UniqueEnchantToggleName";
		
		[Header($"$Mods.{WEMod.ModName}.{L_ID_Tags.Configs}.{EnchantToggleName}.{UniqueConfigKey}")]
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool BerserkersRage;
		
		/*[ReloadRequired]
		[DefaultValue(true)]
		public bool CatastrophicRelease;*/

		[ReloadRequired]
		[DefaultValue(true)]
		public bool ChaoticFishing;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool ColdSteel;
		
		/*[ReloadRequired]
		[DefaultValue(true)]
		public bool Eclipse;*/
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool GodSlayer;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool HellsWrath;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool Juiced;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool JunglesFury;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool Moonlight;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool Multishot;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool NpcContactAngler;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool SolarDash;
		
		#endregion
		
		#region Class swap enchantments
		public const string ClassSwapConfigKey = "ClassSwapEnchantToggleName";
		
		[Header($"$Mods.{WEMod.ModName}.{L_ID_Tags.Configs}.{EnchantToggleName}.{ClassSwapConfigKey}")]
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool ClassSwap;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool MeleeSwap;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool RangedSwap;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool MagicSwap;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool WhipSwap;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool ThrowingSwap;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool CalamityRogueSwap;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool DBZKiSwap;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool ThoriumBardSwap;
		
		[ReloadRequired]
		[DefaultValue(true)]
		public bool ThoriumHealerSwap;
		#endregion
		
		#region Mod compat enchantments
		[JsonIgnore]
		public const string ModCompatConfigKey = "EnchantToggleName";
		
		//[Header($"$Mods.{WEMod.ModName}.{L_ID_Tags.Configs}.{EnchantToggleName}.ModCompat{ModCompatConfigKey}")]
		
		[Header($"$Mods.{WEMod.ModName}.{L_ID_Tags.Configs}.{EnchantToggleName}.DBZ{ModCompatConfigKey}")]
		[ReloadRequired]
		[DefaultValue(true)]
		public bool DBZKi;
		
		[Header($"$Mods.{WEMod.ModName}.{L_ID_Tags.Configs}.{EnchantToggleName}.Thorium{ModCompatConfigKey}")]
		[ReloadRequired]
		[DefaultValue(true)]
		public bool ThoriumElementalDecay;
		
		[Header($"$Mods.{WEMod.ModName}.{L_ID_Tags.Configs}.{EnchantToggleName}.Depths{ModCompatConfigKey}")]
		[ReloadRequired]
		[DefaultValue(true)]
		public bool DepthsCrystalSkin;
		
		#endregion
	}
	#endregion
	public class ClientConfig : ModConfig
	{
		public const string ClientConfigName = "ClientConfig";
		public override ConfigScope Mode => ConfigScope.ClientSide;
		//Enchanting Table Options
		[Header($"$Mods.{WEMod.ModName}.{L_ID_Tags.Configs}.{ServerConfig.ServerConfigName}.{ServerConfig.EnchantingTableOptionsKey}")]

		[DefaultValue(true)]
		public bool teleportEssence;

		[DefaultValue(false)]
		public bool OfferAll;

		[DefaultValue(false)]
		public bool AllowShiftClickMoveFavoritedItems;

		[DefaultValue(true)]
		public bool AlwaysDisplayInfusionPower;

		[DefaultValue(true)]
		[ReloadRequired]
		public bool AllowCraftingIntoLowerTier;

		[DefaultValue(false)]
		public bool AllowInfusingToLowerPower;

		//Display Settings
		[JsonIgnore]
		public const string DisplaySettingsKey = "DisplaySettings";
		[Header($"$Mods.{WEMod.ModName}.{L_ID_Tags.Configs}.{ClientConfigName}.{DisplaySettingsKey}")]

		[DefaultValue(false)]
		public bool UsePointsAsTooltip;

		[DefaultValue(false)]
		public bool DisplayDamageTooltipSeperatly;

		[DefaultValue(false)]
		public bool AlwaysDisplayWeaponLevelUpMessages;

		[DefaultValue(false)]
		public bool AlwaysDisplayArmorLevelUpMessages;

		[DefaultValue(false)]
		public bool AlwaysDisplayAccessoryLevelUpMessages;

		[DefaultValue(false)]
		public bool AlwaysDisplayToolLevelUpMessages;

		[DefaultValue(100)]
		[Range(0, 500)]
		public int CursedEnemyVisualShaking;

		[DefaultValue(true)]
		public bool CursedEnemyParticles;

		[DefaultValue(true)]
		public bool VisualCursedDebuff;

		//Error messages
		[JsonIgnore]
		public const string ErrorMessagesKey = "ErrorMessages";
		[Header($"$Mods.{WEMod.ModName}.{L_ID_Tags.Configs}.{ClientConfigName}.{ErrorMessagesKey}")]

		//Logging Information
		[JsonIgnore]
		public const string LoggingInformationKey = "LoggingInformation";
		[Header($"$Mods.{WEMod.ModName}.{L_ID_Tags.Configs}.{ClientConfigName}.{LoggingInformationKey}")]

		[DefaultValue(false)]
		[ReloadRequired]
		public bool PrintEnchantmentTooltips;

		[DefaultValue(false)]
		[ReloadRequired]
		public bool PrintWeaponInfusionPowers;

		//Mod Testing Tools
		[JsonIgnore]
		public const string ModTestingToolsKey = "ModTestingTools";
		[Header($"$Mods.{WEMod.ModName}.{L_ID_Tags.Configs}.{ClientConfigName}.{ModTestingToolsKey}")]

		[DefaultValue(false)]
		public bool EnableSwappingWeapons;

		[DefaultValue(false)]
		[ReloadRequired]
		public bool LogDummyDPS;
	}
	
	#region Other classes
	public class Pair
	{
		[ReloadRequired]
		public ItemDefinition itemDefinition;

		[Range(0, 100000)]
		[ReloadRequired]
		public int Strength;

		public override string ToString() {
			return $"{"Enchantment".Lang_WE(L_ID1.Configs)}: {(itemDefinition != null && itemDefinition.Type != 0 ? $"{itemDefinition.Name}: {Strength / 10}%" : "NoneSelected".Lang_WE(L_ID1.Configs))}";
		}

		public override bool Equals(object obj) {
			if (obj is Pair other)
				return itemDefinition == other.itemDefinition && Strength == other.Strength;

			return base.Equals(obj);
		}

		public override int GetHashCode() {
			return new { itemDefinition, Strength }.GetHashCode();
		}
	}
	public class ArmorDamageReduction
	{
		[JsonIgnore]
		public static readonly int[,] DamageReductionPerLevel = {
			{ 25000, 12500 },
			{ 18750, 9375 },
			{ 12500, 6250 },
			{ 62500, 31250 },
		};

		[JsonIgnore]
		short GameModeID;

		[Range(0, 250000)]
		public int ArmorDamageReductionPerLevel;

		[Range(0, 250000)]
		public int AccessoryDamageReductionPerLevel;
		public ArmorDamageReduction(short gameMode) {
			GameModeID = gameMode;
			ArmorDamageReductionPerLevel = DamageReductionPerLevel[gameMode, 0];
			AccessoryDamageReductionPerLevel = DamageReductionPerLevel[gameMode, 1];
		}
		public override bool Equals(object obj) {
			if (obj is ArmorDamageReduction other) {
				if (GameModeID != other.GameModeID)
					return false;

				if (ArmorDamageReductionPerLevel != other.ArmorDamageReductionPerLevel)
					return false;

				if (AccessoryDamageReductionPerLevel != other.AccessoryDamageReductionPerLevel)
					return false;

				return true;
			}

			return base.Equals(obj);
		}
		public override int GetHashCode() {
			return new {
				GameModeID,
				ArmorDamageReductionPerLevel,
				AccessoryDamageReductionPerLevel
			}.GetHashCode();
		}
		public const string ArmorDRValuesKey = "ArmorDRValues";
		public const string AccessoryDRValuesKey = "AccessoryDRValues";
		public override string ToString() {
			return $"{GameModeID.ToGameModeIDName()}" +
				$", {ArmorDRValuesKey.Lang_WE(L_ID1.Configs, new object[] { (ArmorDamageReductionPerLevel / 100000f).S(5), (ArmorDamageReductionPerLevel / 2500f).S(5) })}" +
				$", {AccessoryDRValuesKey.Lang_WE(L_ID1.Configs, new object[] { (AccessoryDamageReductionPerLevel / 100000f).S(5), (AccessoryDamageReductionPerLevel / 2500f).S(5) })}";
		}
	}
	public class PresetData
	{
		[JsonIgnore]
		private static List<int> presetValues = new List<int> { 250, 100, 50, 25 };

		[JsonIgnore]
		private static List<string> presetNames = new List<string>() { "Journey", "Normal", "Expert", "Master" };

		//Automatic Preset based on world difficulty
		[DefaultValue(true)]
		[ReloadRequired]
		public bool AutomaticallyMatchPresetToWorldDifficulty {
			get => _automaticallyMatchPreseTtoWorldDifficulty;
			set {
				_automaticallyMatchPreseTtoWorldDifficulty = value;
				if (value) {
					_preset = "Automatic";
				}
				else {
					GlobalEnchantmentStrengthMultiplier = _globalEnchantmentStrengthMultiplier;
				}
			}
		}

		private bool _automaticallyMatchPreseTtoWorldDifficulty;

		//Presets
		[JsonIgnore]
		public const string PresetsKey = "Presets";
		[Header($"$Mods.{WEMod.ModName}.{L_ID_Tags.Configs}.{ServerConfig.ServerConfigName}.{PresetsKey}")]

		[DrawTicks]
		[OptionStrings(new string[] { "Journey", "Normal", "Expert", "Master", "Automatic", "Custom" })]
		[DefaultValue("$Mods.WeaponEnchantments.Config.Normal")]
		[ReloadRequired]
		public string Preset {
			get => _automaticallyMatchPreseTtoWorldDifficulty ? "Automatic" : _preset;
			set {
				_preset = value;
				if (presetNames.Contains(value))
					_globalEnchantmentStrengthMultiplier = presetValues[presetNames.IndexOf(value)];
			}
		}
		private string _preset;

		//Multipliers
		[JsonIgnore]
		public const string MultipliersKey = "Multipliers";
		[Header($"$Mods.{WEMod.ModName}.{L_ID_Tags.Configs}.{ServerConfig.ServerConfigName}.{MultipliersKey}")]

		[Range(0, 250)]
		[DefaultValue(100)]
		[ReloadRequired]
		public int GlobalEnchantmentStrengthMultiplier {
			get => _globalEnchantmentStrengthMultiplier;
			set {
				_globalEnchantmentStrengthMultiplier = value;
				Preset = presetValues.Contains(_globalEnchantmentStrengthMultiplier) ? presetNames[presetValues.IndexOf(_globalEnchantmentStrengthMultiplier)] : "Custom";
			}
		}
		private int _globalEnchantmentStrengthMultiplier;

		[JsonIgnore]
		public const string RarityEnchantmentStrengthMultipliersKey = "RarityEnchantmentStrengthMultipliers";
		[Header($"$Mods.{WEMod.ModName}.{L_ID_Tags.Configs}.{ServerConfig.ServerConfigName}.{RarityEnchantmentStrengthMultipliersKey}")]

		[Range(-1, 10000)]
		[DefaultValue(-1)]
		[ReloadRequired]
		public int BasicEnchantmentStrengthMultiplier { set; get; }

		[Range(-1, 10000)]
		[DefaultValue(-1)]
		[ReloadRequired]
		public int CommonEnchantmentStrengthMultiplier { set; get; }

		[Range(-1, 10000)]
		[DefaultValue(-1)]
		[ReloadRequired]
		public int RareEnchantmentStrengthMultiplier { set; get; }

		[Range(-1, 10000)]
		[DefaultValue(-1)]
		[ReloadRequired]
		public int EpicEnchantmentStrengthMultiplier { set; get; }

		[Range(-1, 10000)]
		[DefaultValue(-1)]
		[ReloadRequired]
		public int LegendaryEnchantmentStrengthMultiplier { set; get; }

		[Range(-1, 10000)]
		[DefaultValue(-1)]
		[ReloadRequired]
		public int CursedEnchantmentStrengthMultiplier { set; get; }

		public PresetData() {
			AutomaticallyMatchPresetToWorldDifficulty = true;
			Preset = "Normal";
			BasicEnchantmentStrengthMultiplier = -1;
			CommonEnchantmentStrengthMultiplier = -1;
			RareEnchantmentStrengthMultiplier = -1;
			EpicEnchantmentStrengthMultiplier = -1;
			LegendaryEnchantmentStrengthMultiplier = -1;
			CursedEnchantmentStrengthMultiplier = -1;
		}

		public override bool Equals(object obj) {
			if (obj is PresetData other) {
				if (Preset != other.Preset)
					return false;

				if (GlobalEnchantmentStrengthMultiplier != other.GlobalEnchantmentStrengthMultiplier)
					return false;

				if (BasicEnchantmentStrengthMultiplier != other.BasicEnchantmentStrengthMultiplier)
					return false;

				if (CommonEnchantmentStrengthMultiplier != other.CommonEnchantmentStrengthMultiplier)
					return false;

				if (RareEnchantmentStrengthMultiplier != other.RareEnchantmentStrengthMultiplier)
					return false;

				if (EpicEnchantmentStrengthMultiplier != other.EpicEnchantmentStrengthMultiplier)
					return false;

				if (LegendaryEnchantmentStrengthMultiplier != other.LegendaryEnchantmentStrengthMultiplier)
					return false;

				if (CursedEnchantmentStrengthMultiplier != other.CursedEnchantmentStrengthMultiplier)
					return false;

				return true;
			}

			return base.Equals(obj);
		}

		public override int GetHashCode() {
			return new {
				Preset,
				GlobalEnchantmentStrengthMultiplier,
				BasicEnchantmentStrengthMultiplier,
				CommonEnchantmentStrengthMultiplier,
				RareEnchantmentStrengthMultiplier,
				EpicEnchantmentStrengthMultiplier,
				LegendaryEnchantmentStrengthMultiplier,
				CursedEnchantmentStrengthMultiplier
			}.GetHashCode();
		}
	}
	#endregion
}
