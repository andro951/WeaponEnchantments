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

		//Essence and Experience
		[JsonIgnore]
		public const string EssenceandExperienceKey = "EssenceandExperience";
		[Header($"$Mods.{WEMod.ModName}.{L_ID_Tags.Configs}.{ServerConfigName}.{EssenceandExperienceKey}")]

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
		public bool AutomaticallyMatchPreseTtoWorldDifficulty {
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

		public PresetData() {
			AutomaticallyMatchPreseTtoWorldDifficulty = true;
			Preset = "Normal";
			BasicEnchantmentStrengthMultiplier = -1;
			CommonEnchantmentStrengthMultiplier = -1;
			RareEnchantmentStrengthMultiplier = -1;
			EpicEnchantmentStrengthMultiplier = -1;
			LegendaryEnchantmentStrengthMultiplier = -1;
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
				LegendaryEnchantmentStrengthMultiplier
			}.GetHashCode();
		}
	}
}
