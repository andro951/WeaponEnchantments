using System;
using System.Collections.Generic;
using System.ComponentModel;
using Terraria.ModLoader.Config;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using WeaponEnchantments.Common.Utility;
using Terraria.ID;
using WeaponEnchantments.Common.Globals;

namespace WeaponEnchantments.Common.Configs
{
	[Label("$Mods.WeaponEnchantments.Config.ServerConfig.Label")]
	public class ServerConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ServerSide;

		//Server Config
		[Header("$Mods.WeaponEnchantments.Config.ServerConfig")]
		[Label("$Mods.WeaponEnchantments.Config.presetData.Label")]
		[ReloadRequired]
		public PresetData presetData;

		[Header("$Mods.WeaponEnchantments.Config.IndividualEnchantmentStrengths")]

		[Label("$Mods.WeaponEnchantments.Config.individualStrengthsEnabled.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.individualStrengthsEnabled.Tooltip")]
		[ReloadRequired]
		[DefaultValue(false)]
		public bool individualStrengthsEnabled;

		[Label("$Mods.WeaponEnchantments.Config.individualStrengths.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.individualStrengths.Tooltip")]
		public List<Pair> individualStrengths = new List<Pair>();

		//Enchantment Settings
		[Header("$Mods.WeaponEnchantments.Config.EnchantmentSettings")]
		[Label("$Mods.WeaponEnchantments.Config.AlwaysOverrideDamageType.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.AlwaysOverrideDamageType.Tooltip")]
		[DefaultValue(true)]
		public bool AlwaysOverrideDamageType;

		[Label("$Mods.WeaponEnchantments.Config.AffectOnVanillaLifeStealLimmit.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.AffectOnVanillaLifeStealLimmit.Tooltip")]
		[DefaultValue(100)]
		[Range(0, 10000)]
		public int AffectOnVanillaLifeStealLimmit;

		[Label("$Mods.WeaponEnchantments.Config.AttackSpeedEnchantmentAutoReuseSetpoint.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.AttackSpeedEnchantmentAutoReuseSetpoint.Tooltip")]
		[Range(0, 10000)]
		[DefaultValue(10)]
		[ReloadRequired]
		public int AttackSpeedEnchantmentAutoReuseSetpoint;

		[Label("$Mods.WeaponEnchantments.Config.AutoReuseDisabledOnMagicMissile.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.AutoReuseDisabledOnMagicMissile.Tooltip")]
		[DefaultValue(true)]
		[ReloadRequired]
		public bool AutoReuseDisabledOnMagicMissile;

		[Label("$Mods.WeaponEnchantments.Config.BuffDuration.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.BuffDuration.Tooltip")]
		[DefaultValue(15)]
		[Range(1, 600)]
		[ReloadRequired]
		public int BuffDuration;

		[Label("$Mods.WeaponEnchantments.Config.AmaterasuSelfGrowthPerTick.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.AmaterasuSelfGrowthPerTick.Tooltip")]
		[DefaultValue(5)]
		[Range(0, 1000000)]
		public int AmaterasuSelfGrowthPerTick;

		[Label("$Mods.WeaponEnchantments.Config.ReduceRecipesToMinimum.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.ReduceRecipesToMinimum.Tooltip")]
		[DefaultValue(false)]
		[ReloadRequired]
		public bool ReduceRecipesToMinimum;

		[Label("$Mods.WeaponEnchantments.Config.ConfigCapacityCostMultiplier.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.ConfigCapacityCostMultiplier.Tooltip")]
		[DefaultValue(100)]
		[Range(0, 1400)]
		[ReloadRequired]
		public int ConfigCapacityCostMultiplier;

		[Label("$Mods.WeaponEnchantments.Config.RemoveEnchantmentRestrictions.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.RemoveEnchantmentRestrictions.Tooltip")]
		[DefaultValue(false)]
		[ReloadRequired]
		public bool RemoveEnchantmentRestrictions;

		//Essence and Experience
		[Header("$Mods.WeaponEnchantments.Config.EssenceandExperience")]
		[Label("$Mods.WeaponEnchantments.Config.BossEssenceMultiplier.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.BossEssenceMultiplier.Tooltip")]
		[Range(0, 10000)]
		[DefaultValue(100)]
		[ReloadRequired]
		public int BossEssenceMultiplier;

		[Label("$Mods.WeaponEnchantments.Config.EssenceMultiplier.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.EssenceMultiplier.Tooltip")]
		[Range(0, 10000)]
		[DefaultValue(100)]
		[ReloadRequired]
		public int EssenceMultiplier;

		[Label("$Mods.WeaponEnchantments.Config.BossExperienceMultiplier.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.BossExperienceMultiplier.Tooltip")]
		[Range(0, 10000)]
		[DefaultValue(100)]
		public int BossExperienceMultiplier;

		[Label("$Mods.WeaponEnchantments.Config.ExperienceMultiplier.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.ExperienceMultiplier.Tooltip")]
		[Range(0, 10000)]
		[DefaultValue(100)]
		public int ExperienceMultiplier;

		[Label("$Mods.WeaponEnchantments.Config.GatheringExperienceMultiplier.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.GatheringExperienceMultiplier.Tooltip")]
		[Range(0, 10000)]
		[DefaultValue(100)]
		public int GatheringExperienceMultiplier;

		[Label("$Mods.WeaponEnchantments.Config.EssenceGrabRange.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.EssenceGrabRange.Tooltip")]
		[DefaultValue(10)]
		[Range(1, 100)]
		public int EssenceGrabRange;

		//Enchantment Drop Rates(%)
		[Header("$Mods.WeaponEnchantments.Config.EnchantmentDropRates(%)")]
		[Label("$Mods.WeaponEnchantments.Config.BossEnchantmentDropChance.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.BossEnchantmentDropChance.Tooltip")]
		[Range(0, 100)]
		[DefaultValue(50)]
		[ReloadRequired]
		public int BossEnchantmentDropChance;

		[Label("$Mods.WeaponEnchantments.Config.EnchantmentDropChance.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.EnchantmentDropChance.Tooltip")]
		[Range(0, 1000)]
		[DefaultValue(100)]
		[ReloadRequired]
		public int EnchantmentDropChance;

		[Label("$Mods.WeaponEnchantments.Config.ChestSpawnChance.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.ChestSpawnChance.Tooltip")]
		[Range(0, 100000)]
		[DefaultValue(50)]
		public int ChestSpawnChance;

		[Label("$Mods.WeaponEnchantments.Config.CrateDropChance.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.CrateDropChance.Tooltip")]
		[Range(0, 10000)]
		[DefaultValue(100)]
		public int CrateDropChance;

		//Other Drop Rates
		[Header("$Mods.WeaponEnchantments.Config.OtherDropRates")]
		[Label("$Mods.WeaponEnchantments.Config.PreventPowerBoosterFromPreHardMode.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.PreventPowerBoosterFromPreHardMode.Tooltip")]
		[DefaultValue(true)]
		[ReloadRequired]
		public bool PreventPowerBoosterFromPreHardMode;

		//Enchanting Table Options
		[Header("$Mods.WeaponEnchantments.Config.EnchantingTableOptions")]
		[Label("$Mods.WeaponEnchantments.Config.AllowHighTierOres.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.AllowHighTierOres.Tooltip")]
		[DefaultValue(true)]
		public bool AllowHighTierOres;

		[Label("$Mods.WeaponEnchantments.Config.EnchantmentSlotsOnWeapons.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.EnchantmentSlotsOnWeapons.Tooltip")]
		[DefaultValue(5)]
		[Range(0, 5)]
		[ReloadRequired]
		public int EnchantmentSlotsOnWeapons;

		[Label("$Mods.WeaponEnchantments.Config.EnchantmentSlotsOnArmor.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.EnchantmentSlotsOnArmor.Tooltip")]
		[DefaultValue(3)]
		[Range(0, 5)]
		[ReloadRequired]
		public int EnchantmentSlotsOnArmor;

		[Label("$Mods.WeaponEnchantments.Config.EnchantmentSlotsOnAccessories.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.EnchantmentSlotsOnAccessories.Tooltip")]
		[DefaultValue(1)]
		[Range(0, 5)]
		[ReloadRequired]
		public int EnchantmentSlotsOnAccessories;

		[Label("$Mods.WeaponEnchantments.Config.EnchantmentSlotsOnFishingPoles.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.EnchantmentSlotsOnFishingPoles.Tooltip")]
		[DefaultValue(5)]
		[Range(0, 5)]
		[ReloadRequired]
		public int EnchantmentSlotsOnFishingPoles;

		[Label("$Mods.WeaponEnchantments.Config.EnchantmentSlotsOnTools.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.EnchantmentSlotsOnTools.Tooltip")]
		[DefaultValue(5)]
		[Range(0, 5)]
		[ReloadRequired]
		public int EnchantmentSlotsOnTools;

		[Label("$Mods.WeaponEnchantments.Config.PercentOfferEssence.Label")]
		[DefaultValue(50)]
		[Range(0, 100)]
		public int PercentOfferEssence;

		[Label("$Mods.WeaponEnchantments.Config.ReduceOfferEfficiencyByTableTier.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.ReduceOfferEfficiencyByTableTier.Tooltip")]
		[DefaultValue(false)]
		public bool ReduceOfferEfficiencyByTableTier;

		[Label("$Mods.WeaponEnchantments.Config.ReduceOfferEfficiencyByBaseInfusionPower.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.ReduceOfferEfficiencyByBaseInfusionPower.Tooltip")]
		[DefaultValue(false)]
		public bool ReduceOfferEfficiencyByBaseInfusionPower;

		//General Game Changes
		[Header("$Mods.WeaponEnchantments.Config.GeneralGameChanges")]
		[Label("$Mods.WeaponEnchantments.Config.ArmorPenetration.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.ArmorPenetration.Tooltip")]
		[DefaultValue(true)]
		public bool ArmorPenetration;

		[Label("$Mods.WeaponEnchantments.Config.DisableMinionCrits.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.DisableMinionCrits.Tooltip")]
		[DefaultValue(false)]
		public bool DisableMinionCrits;

		[Label("$Mods.WeaponEnchantments.Config.CritPerLevelDisabled.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.CritPerLevelDisabled.Tooltip")]
		[DefaultValue(false)]
		[ReloadRequired]
		public bool CritPerLevelDisabled;

		[Label("$Mods.WeaponEnchantments.Config.DamagePerLevelInstead.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.DamagePerLevelInstead.Tooltip")]
		[DefaultValue(false)]
		[ReloadRequired]
		public bool DamagePerLevelInstead;

		[Label("$Mods.WeaponEnchantments.Config.DamageReductionPerLevelDisabled.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.DamageReductionPerLevelDisabled.Tooltip")]
		[DefaultValue(false)]
		public bool DamageReductionPerLevelDisabled;

		[Label("$Mods.WeaponEnchantments.Config.CalculateDamageReductionBeforeDefense.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.CalculateDamageReductionBeforeDefense.Tooltip")]
		[DefaultValue(false)]
		public bool CalculateDamageReductionBeforeDefense;

		[ReloadRequired]
		[Label("$Mods.WeaponEnchantments.Config.ArmorDamageReductions.Label")]
		public List<ArmorDamageReduction> ArmorDamageReductions = new() { new(0), new(1), new(2), new(3) };

		[Label("$Mods.WeaponEnchantments.Config.AllowCriticalChancePast100.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.AllowCriticalChancePast100.Tooltip")]
		[DefaultValue(true)]
		public bool AllowCriticalChancePast100;

		[Label("$Mods.WeaponEnchantments.Config.MultiplicativeCriticalHits.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.MultiplicativeCriticalHits.Tooltip")]
		[DefaultValue(false)]
		public bool MultiplicativeCriticalHits;

		[Label("$Mods.WeaponEnchantments.Config.InfusionDamageMultiplier.Label")]
		[DefaultValue(1300)]
		[Range(1000, 2000)]
		[Tooltip("$Mods.WeaponEnchantments.Config.InfusionDamageMultiplier.Tooltip")]
		[ReloadRequired]
		public int InfusionDamageMultiplier;

		[Tooltip("$Mods.WeaponEnchantments.Config.DisableArmorInfusion.Tooltip")]
		[ReloadRequired]
		[DefaultValue(false)]
		public bool DisableArmorInfusion;

		[Label("$Mods.WeaponEnchantments.Config.MinionLifeStealMultiplier.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.MinionLifeStealMultiplier.Tooltip")]
		[DefaultValue(50)]
		[Range(0, 100)]
		public int MinionLifeStealMultiplier;

		//Random Extra Stuff
		[Header("$Mods.WeaponEnchantments.Config.RandomExtraStuff")]
		[Label("$Mods.WeaponEnchantments.Config.DCUStart.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.DCUStart.Tooltip")]
		[DefaultValue(false)]
		public bool DCUStart;

		[Label("$Mods.WeaponEnchantments.Config.DisableResearch.Label")]
		[DefaultValue(false)]
		[ReloadRequired]
		public bool DisableResearch;

		[Label("$Mods.WeaponEnchantments.Config.PrintWikiInfo.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.PrintWikiInfo.Tooltip")]
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

		public ServerConfig() {
			presetData = new PresetData();
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

	[Label("$Mods.WeaponEnchantments.Config.ClientConfig.Label")]
	public class ClientConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ClientSide;
		//Enchanting Table Options
		[Header("$Mods.WeaponEnchantments.Config.EnchantingTableOptions")]
		[Label("$Mods.WeaponEnchantments.Config.teleportEssence.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.teleportEssence.Tooltip")]
		[DefaultValue(true)]
		public bool teleportEssence;

		[Label("$Mods.WeaponEnchantments.Config.OfferAll.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.OfferAll.Tooltip")]
		[DefaultValue(false)]
		public bool OfferAll;

		[Label("$Mods.WeaponEnchantments.Config.AllowShiftClickMoveFavoritedItems.Label")]
		[DefaultValue(false)]
		public bool AllowShiftClickMoveFavoritedItems;

		[Label("$Mods.WeaponEnchantments.Config.AlwaysDisplayInfusionPower.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.AlwaysDisplayInfusionPower.Tooltip")]
		[DefaultValue(true)]
		public bool AlwaysDisplayInfusionPower;

		[Label("$Mods.WeaponEnchantments.Config.AllowCraftingIntoLowerTier.Label")]
		[DefaultValue(true)]
		[ReloadRequired]
		public bool AllowCraftingIntoLowerTier;

		[Label("$Mods.WeaponEnchantments.Config.AllowInfusingToLowerPower.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.AllowInfusingToLowerPower.Tooltip")]
		[DefaultValue(false)]
		public bool AllowInfusingToLowerPower;

		//Display Settings
		[Header("$Mods.WeaponEnchantments.Config.DisplaySettings")]
		[Label("$Mods.WeaponEnchantments.Config.UsePointsAsTooltip.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.UsePointsAsTooltip.Tooltip")]
		[DefaultValue(false)]
		public bool UsePointsAsTooltip;

		[Label("$Mods.WeaponEnchantments.Config.UseAlternateEnchantmentEssenceTextures.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.UseAlternateEnchantmentEssenceTextures.Tooltip")]
		[DefaultValue(false)]
		[ReloadRequired]
		public bool UseAlternateEnchantmentEssenceTextures;

		[Label("$Mods.WeaponEnchantments.Config.DisplayApproximateWeaponDamageTooltip.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.DisplayApproximateWeaponDamageTooltip.Tooltip")]
		[DefaultValue(true)]
		public bool DisplayApproximateWeaponDamageTooltip;

		[Label("$Mods.WeaponEnchantments.Config.AlwaysDisplayWeaponLevelUpMessages.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.AlwaysDisplayWeaponLevelUpMessages.Tooltip")]
		[DefaultValue(false)]
		public bool AlwaysDisplayWeaponLevelUpMessages;

		[Label("$Mods.WeaponEnchantments.Config.AlwaysDisplayArmorLevelUpMessages.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.AlwaysDisplayArmorLevelUpMessages.Tooltip")]
		[DefaultValue(false)]
		public bool AlwaysDisplayArmorLevelUpMessages;

		[Label("$Mods.WeaponEnchantments.Config.AlwaysDisplayAccessoryLevelUpMessages.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.AlwaysDisplayAccessoryLevelUpMessages.Tooltip")]
		[DefaultValue(false)]
		public bool AlwaysDisplayAccessoryLevelUpMessages;

		[Label("$Mods.WeaponEnchantments.Config.AlwaysDisplayToolLevelUpMessages.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.AlwaysDisplayToolLevelUpMessages.Tooltip")]
		[DefaultValue(false)]
		public bool AlwaysDisplayToolLevelUpMessages;

		[Label("$Mods.WeaponEnchantments.Config.UITransparency.Label")]
		[DefaultValue(100)]
		[Range(0, (int)byte.MaxValue)]
		public int UITransparency;

		//Error messages
		[Header("$Mods.WeaponEnchantments.Config.ErrorMessages")]
		[Label("$Mods.WeaponEnchantments.Config.DisableAllErrorMessagesInChat.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.DisableAllErrorMessagesInChat.Tooltip")]
		[DefaultValue(false)]
		public bool DisableAllErrorMessagesInChat {
			set {
				if (value) {
					OnlyShowErrorMessagesInChatOnce = false;
				}
				else {
					LogMethods.LoggedChatMessagesIDs.Clear();
				}

				_disableAllErrorMessagesInChat = value;
			}

			get => _disableAllErrorMessagesInChat;
		}

		[JsonIgnore]
		private bool _disableAllErrorMessagesInChat;

		[Label("$Mods.WeaponEnchantments.Config.OnlyShowErrorMessagesInChatOnce.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.OnlyShowErrorMessagesInChatOnce.Tooltip")]
		[DefaultValue(true)]
		public bool OnlyShowErrorMessagesInChatOnce {
			set {
				if (value) {
					DisableAllErrorMessagesInChat = false;
				}
				else {
					LogMethods.LoggedChatMessagesIDs.Clear();
				}

				_onlyShowErrorMessagesInChatOnce = value;
			}

			get => _onlyShowErrorMessagesInChatOnce;
		}

		private bool _onlyShowErrorMessagesInChatOnce;

		//Logging Information
		[Header("$Mods.WeaponEnchantments.Config.LoggingInformation")]
		[Label("$Mods.WeaponEnchantments.Config.PrintEnchantmentTooltips.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.PrintEnchantmentTooltips.Tooltip")]
		[DefaultValue(false)]
		[ReloadRequired]
		public bool PrintEnchantmentTooltips;

		[Label("$Mods.WeaponEnchantments.Config.PrintEnchantmentDrops.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.PrintEnchantmentDrops.Tooltip")]
		[DefaultValue(false)]
		[ReloadRequired]
		public bool PrintEnchantmentDrops;

		[Label("$Mods.WeaponEnchantments.Config.PrintLocalizationLists.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.PrintLocalizationLists.Tooltip")]
		[DefaultValue(false)]
		[ReloadRequired]
		public bool PrintLocalizationLists;

		[Label("$Mods.WeaponEnchantments.Config.PrintWeaponInfusionPowers.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.PrintWeaponInfusionPowers.Tooltip")]
		[DefaultValue(false)]
		[ReloadRequired]
		public bool PrintWeaponInfusionPowers;

		//Mod Testing Tools
		[Header("$Mods.WeaponEnchantments.Config.ModTestingTools")]
		[Label("$Mods.WeaponEnchantments.Config.EnableSwappingWeapons.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.EnableSwappingWeapons.Tooltip")]
		[DefaultValue(false)]
		public bool EnableSwappingWeapons;

		[Label("$Mods.WeaponEnchantments.Config.LogDummyDPS.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.LogDummyDPS.Tooltip")]
		[DefaultValue(false)]
		[ReloadRequired]
		public bool LogDummyDPS;
	}
	public class Pair
	{
		[Tooltip("$Mods.WeaponEnchantments.Config.itemDefinition.Tooltip")]
		[Label("$Mods.WeaponEnchantments.Config.itemDefinition.Label")]
		[ReloadRequired]
		public ItemDefinition itemDefinition;

		[Label("$Mods.WeaponEnchantments.Config.Strength.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.Strength.Tooltip")]
		[Range(0, 100000)]
		[ReloadRequired]
		public int Strength;

		public override string ToString() {
			return $"{"Enchantment".Lang(L_ID1.Config)}: {(itemDefinition != null && itemDefinition.Type != 0 ? $"{itemDefinition.Name}: {Strength / 10}%" : "NoneSelected".Lang(L_ID1.Config))}";
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

		[Label("$Mods.WeaponEnchantments.Config.ArmorDamageReductionPerLevel.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.ArmorDamageReductionPerLevel.Tooltip")]
		[Range(0, 250000)]
		public int ArmorDamageReductionPerLevel;

		[Label("$Mods.WeaponEnchantments.Config.AccessoryDamageReductionPerLevel.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.AccessoryDamageReductionPerLevel.Tooltip")]
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
		public override string ToString() {
			return $"{GameModeID.ToGameModeIDName()}" +
				$", {"ArmorDRValues".Lang(L_ID1.Config, new object[] { (ArmorDamageReductionPerLevel / 100000f).S(5), (ArmorDamageReductionPerLevel / 2500f).S(5) })}" +
				$", {"AccessoryDRValues".Lang(L_ID1.Config, new object[] { (AccessoryDamageReductionPerLevel / 100000f).S(5), (AccessoryDamageReductionPerLevel / 2500f).S(5) })}";
		}
	}
	public class PresetData
	{
		[JsonIgnore]
		private static List<int> presetValues = new List<int> { 250, 100, 50, 25 };

		[JsonIgnore]
		private static List<string> presetNames = new List<string>() { "Journey", "Normal", "Expert", "Master" };

		//Automatic Preset based on world difficulty
		[Label("$Mods.WeaponEnchantments.Config.AutomaticallyMatchPreseTtoWorldDifficulty.Label")]
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
		[Header("$Mods.WeaponEnchantments.Config.Presets")]
		[DrawTicks]
		[OptionStrings(new string[] { "Journey", "Normal", "Expert", "Master", "Automatic", "Custom" })]
		[DefaultValue("$Mods.WeaponEnchantments.Config.Normal")]
		[Tooltip("$Mods.WeaponEnchantments.Config.Preset.Tooltip")]
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
		[Header("$Mods.WeaponEnchantments.Config.Multipliers")]
		[Label("$Mods.WeaponEnchantments.Config.GlobalEnchantmentStrengthMultiplier.Label")]
		[Range(0, 250)]
		[DefaultValue(100)]
		[Tooltip("$Mods.WeaponEnchantments.Config.GlobalEnchantmentStrengthMultiplier.Tooltip")]
		[ReloadRequired]
		public int GlobalEnchantmentStrengthMultiplier {
			get => _globalEnchantmentStrengthMultiplier;
			set {
				_globalEnchantmentStrengthMultiplier = value;
				Preset = presetValues.Contains(_globalEnchantmentStrengthMultiplier) ? presetNames[presetValues.IndexOf(_globalEnchantmentStrengthMultiplier)] : "Custom";
			}
		}
		private int _globalEnchantmentStrengthMultiplier;

		[Header("$Mods.WeaponEnchantments.Config.RarityEnchantmentStrengthMultipliers")]
		[Label("$Mods.WeaponEnchantments.Config.BasicEnchantmentStrengthMultiplier.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.BasicEnchantmentStrengthMultiplier.Tooltip")]
		[Range(-1, 10000)]
		[DefaultValue(-1)]
		[ReloadRequired]
		public int BasicEnchantmentStrengthMultiplier { set; get; }

		[Label("$Mods.WeaponEnchantments.Config.CommonEnchantmentStrengthMultiplier.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.CommonEnchantmentStrengthMultiplier.Tooltip")]
		[Range(-1, 10000)]
		[DefaultValue(-1)]
		[ReloadRequired]
		public int CommonEnchantmentStrengthMultiplier { set; get; }

		[Label("$Mods.WeaponEnchantments.Config.RareEnchantmentStrengthMultiplier.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.RareEnchantmentStrengthMultiplier.Tooltip")]
		[Range(-1, 10000)]
		[DefaultValue(-1)]
		[ReloadRequired]
		public int RareEnchantmentStrengthMultiplier { set; get; }

		[Label("$Mods.WeaponEnchantments.Config.EpicEnchantmentStrengthMultiplier.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.EpicEnchantmentStrengthMultiplier.Tooltip")]
		[Range(-1, 10000)]
		[DefaultValue(-1)]
		[ReloadRequired]
		public int EpicEnchantmentStrengthMultiplier { set; get; }

		[Label("$Mods.WeaponEnchantments.Config.LegendaryEnchantmentStrengthMultiplier.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.LegendaryEnchantmentStrengthMultiplier.Tooltip")]
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

	/*
	public class PresetData
	{
		[JsonIgnore]
		private static List<int> presetValues = new List<int> { 250, 100, 50, 25 };

		[JsonIgnore]
		private static SortedDictionary<string, string> translatedPresetNames = new() {
			{ "Journey", "Journey".Lang(L_ID1.Config) },
			{ "Normal", "Normal".Lang(L_ID1.Config) },
			{ "Expert", "Expert".Lang(L_ID1.Config) },
			{ "Master", "Master".Lang(L_ID1.Config) },
			{ "Automatic", "Automatic".Lang(L_ID1.Config) },
			{ "Custom", "Custom".Lang(L_ID1.Config) }
		};

		[JsonIgnore]
		private static List<string> presetNames = new List<string>() { translatedPresetNames["Journey"], translatedPresetNames["Normal"], translatedPresetNames["Expert"], translatedPresetNames["Master"] };

		//Automatic Preset based on world difficulty
		[Label("$Mods.WeaponEnchantments.Config.AutomaticallyMatchPreseTtoWorldDifficulty.Label")]
		[DefaultValue(true)]
		[ReloadRequired]
		public bool AutomaticallyMatchPreseTtoWorldDifficulty {
			get => _automaticallyMatchPreseTtoWorldDifficulty;
			set {
				_automaticallyMatchPreseTtoWorldDifficulty = value;
				if (value) {
					_preset = translatedPresetNames["Automatic"];
				}
				else {
					GlobalEnchantmentStrengthMultiplier = _globalEnchantmentStrengthMultiplier;
				}
			}
		}

		private bool _automaticallyMatchPreseTtoWorldDifficulty;

		//Presets
		[Header("$Mods.WeaponEnchantments.Config.Presets")]
		[DrawTicks]
		[OptionStrings(new string[] { "$Mods.WeaponEnchantments.Config.Journey", "$Mods.WeaponEnchantments.Config.Normal", "$Mods.WeaponEnchantments.Config.Expert", "$Mods.WeaponEnchantments.Config.Master",
			"$Mods.WeaponEnchantments.Config.Automatic", "$Mods.WeaponEnchantments.Config.Custom" })]
		[DefaultValue("$Mods.WeaponEnchantments.Config.Normal")]
		[Tooltip("$Mods.WeaponEnchantments.Config.Preset.Tooltip")]
		[ReloadRequired]
		public string Preset {
			get => _automaticallyMatchPreseTtoWorldDifficulty ? translatedPresetNames["Automatic"] : _preset;
			set {
				_preset = value;
				if (presetNames.Contains(value))
					_globalEnchantmentStrengthMultiplier = presetValues[presetNames.IndexOf(value)];
			}
		}
		private string _preset;

		//Multipliers
		[Header("$Mods.WeaponEnchantments.Config.Multipliers")]
		[Label("$Mods.WeaponEnchantments.Config.GlobalEnchantmentStrengthMultiplier.Label")]
		[Range(0, 250)]
		[DefaultValue(100)]
		[Tooltip("$Mods.WeaponEnchantments.Config.GlobalEnchantmentStrengthMultiplier.Tooltip")]
		[ReloadRequired]
		public int GlobalEnchantmentStrengthMultiplier {
			get => _globalEnchantmentStrengthMultiplier;
			set {
				_globalEnchantmentStrengthMultiplier = value;
				Preset = presetValues.Contains(_globalEnchantmentStrengthMultiplier) ? presetNames[presetValues.IndexOf(_globalEnchantmentStrengthMultiplier)] : translatedPresetNames["Custom"];
			}
		}
		private int _globalEnchantmentStrengthMultiplier;

		[Header("$Mods.WeaponEnchantments.Config.RarityEnchantmentStrengthMultipliers")]
		[Label("$Mods.WeaponEnchantments.Config.BasicEnchantmentStrengthMultiplier.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.BasicEnchantmentStrengthMultiplier.Tooltip")]
		[Range(-1, 10000)]
		[DefaultValue(-1)]
		[ReloadRequired]
		public int BasicEnchantmentStrengthMultiplier { set; get; }

		[Label("$Mods.WeaponEnchantments.Config.CommonEnchantmentStrengthMultiplier.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.CommonEnchantmentStrengthMultiplier.Tooltip")]
		[Range(-1, 10000)]
		[DefaultValue(-1)]
		[ReloadRequired]
		public int CommonEnchantmentStrengthMultiplier { set; get; }

		[Label("$Mods.WeaponEnchantments.Config.RareEnchantmentStrengthMultiplier.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.RareEnchantmentStrengthMultiplier.Tooltip")]
		[Range(-1, 10000)]
		[DefaultValue(-1)]
		[ReloadRequired]
		public int RareEnchantmentStrengthMultiplier { set; get; }

		[Label("$Mods.WeaponEnchantments.Config.EpicEnchantmentStrengthMultiplier.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.EpicEnchantmentStrengthMultiplier.Tooltip")]
		[Range(-1, 10000)]
		[DefaultValue(-1)]
		[ReloadRequired]
		public int EpicEnchantmentStrengthMultiplier { set; get; }

		[Label("$Mods.WeaponEnchantments.Config.LegendaryEnchantmentStrengthMultiplier.Label")]
		[Tooltip("$Mods.WeaponEnchantments.Config.LegendaryEnchantmentStrengthMultiplier.Tooltip")]
		[Range(-1, 10000)]
		[DefaultValue(-1)]
		[ReloadRequired]
		public int LegendaryEnchantmentStrengthMultiplier { set; get; }

		public PresetData() {
			AutomaticallyMatchPreseTtoWorldDifficulty = true;
			Preset = translatedPresetNames["Normal"];
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
	*/
}
