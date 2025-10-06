using System;
using System.Linq;
using Terraria;
using WeaponEnchantments.Common.Utility;
using static WeaponEnchantments.WEMod;

namespace WeaponEnchantments.Common.Configs
{
	public class ConfigValues {
		public static readonly float[] PresetMultipliers = { 1f, 0.5f, 0.25f, 2.5f };
		public static readonly float[] CursePresetMultipliers = { 1f, 0.5f, 0.25f, 1f };

		private static float _globalStrengthMultiplier = serverConfig.presetData.GlobalEnchantmentStrengthMultiplier / 100f;
		public static float GlobalStrengthMultiplier {
			get {
				if (serverConfig.presetData.AutomaticallyMatchPresetToWorldDifficulty)
					return PresetMultipliers[Main.GameMode];

				return _globalStrengthMultiplier;
			}
		}
		public static Time BuffDurationTicks = new Time((uint)serverConfig.BuffDuration * 60);
		public static float EnchantmentDropChance = serverConfig.EnchantmentDropChance / 100f;
		public static float BossEnchantmentDropChance = serverConfig.BossEnchantmentDropChance / 100f;
		public static float EssenceMultiplier = serverConfig.EssenceMultiplier / 100f;
		public static float BossEssenceMultiplier = serverConfig.BossEssenceMultiplier / 100f;
		public static float GatheringExperienceMultiplier => serverConfig.GatheringExperienceMultiplier / 100f;
		public static bool MultiplicativeCriticalHits => serverConfig.MultiplicativeCriticalHits;
		public static bool AllowCriticalChancePast100 => serverConfig.AllowCriticalChancePast100;
		public static bool AlwaysOverrideDamageType => serverConfig.AlwaysOverrideDamageType;
		public static float InfusionDamageMultiplier = serverConfig.InfusionDamageMultiplier / 1000f;
		public static float BossXPMultiplier => serverConfig.BossExperienceMultiplier / 100f;
		public static float NormalXPMultiplier => serverConfig.ExperienceMultiplier / 100f;
		public static float AffectOnVanillaLifeStealLimit => serverConfig.AffectOnVanillaLifeStealLimmit / 100f;
		public static float AttackSpeedEnchantmentAutoReuseSetpoint = serverConfig.AttackSpeedEnchantmentAutoReuseSetpoint / 100f;
		public static float PercentOfferEssence => serverConfig.PercentOfferEssence / 100f;
		public static float ChestSpawnChance => serverConfig.ChestSpawnChance / 100f;
		public static float CrateDropChance => serverConfig.CrateDropChance / 100f;
		public static int AmaterasuSelfGrowthPerTick => serverConfig.AmaterasuSelfGrowthPerTick;
		public static float[] RarityEnchantmentStrengthMultipliers = {
			(float)serverConfig.presetData.BasicEnchantmentStrengthMultiplier / 100f,
			(float)serverConfig.presetData.CommonEnchantmentStrengthMultiplier / 100f,
			(float)serverConfig.presetData.RareEnchantmentStrengthMultiplier / 100f,
			(float)serverConfig.presetData.EpicEnchantmentStrengthMultiplier / 100f,
			(float)serverConfig.presetData.LegendaryEnchantmentStrengthMultiplier / 100f,
			(float)serverConfig.presetData.CursedEnchantmentStrengthMultiplier / 100f
		};

		public static bool useAllRecipes = !recursiveCraftEnabled && !serverConfig.ReduceRecipesToMinimum;
		public static bool RemoveEnchantmentRestrictions = serverConfig.RemoveEnchantmentRestrictions;
		public static float ConfigCapacityCostMultiplier = serverConfig.ConfigCapacityCostMultiplier / 100f;
		public static float ArmorDamageReductionPerLevel {
			get {
				if (serverConfig.ArmorDamageReductions.Count != ServerConfig.DefaultArmorDamageReductionsCount)
					serverConfig.ArmorDamageReductions = ServerConfig.DefaultArmorDamageReductions;

				return serverConfig.ArmorDamageReductions[Main.GameMode].ArmorDamageReductionPerLevel / 10000000f;
			}
		}
		public static float AccessoryDamageReductionPerLevel => serverConfig.ArmorDamageReductions[Main.GameMode].AccessoryDamageReductionPerLevel / 10000000f;
		public static int[] EnchantmentSlotsOnItems = new int[] {
			serverConfig.EnchantmentSlotsOnWeapons,
			serverConfig.EnchantmentSlotsOnArmor,
			serverConfig.EnchantmentSlotsOnAccessories,
			serverConfig.EnchantmentSlotsOnFishingPoles,
			serverConfig.EnchantmentSlotsOnTools
		};
		public static float MinionLifeStealMultiplier => serverConfig.MinionLifeStealMultiplier / 100f;
		public static float SiphonExperienceCost => serverConfig.SiphonExperiencePercentCost / 100f;
		public static float NegativeDefensePenaltyMultiplier => serverConfig.NegativeDefensePenaltyMultiplier / 1000f;
		public static float CursedEnemyLifeMult => serverConfig.CursedEnemyLifeMultiplier / 100f;
		public static float CursedEnemyDamageMultiplier => serverConfig.CursedEnemyDamageMultiplier / 100f;
		public static float CursedEssenceDropChanceMultiplier => serverConfig.CursedEssenceDropChanceMultiplier / 100f;
		public static float CursedEnemyDebuffAttackRange = serverConfig.CursedEnemyDebuffAttackRange / 100f;
		public static float CursedEnemyDebuffDurationMultiplier = serverConfig.CursedEnemyDebuffDurationMultiplier / 100f;
		public static float CursedEnemyDebuffChanceMultiplier = serverConfig.CursedEnemyDebuffChanceMultiplier / 100f;
		public static float EnchantmentStrengthCurseScaling = serverConfig.EnchantmentStrengthCurseScaling / 100f;
		public static float CurseStrengthMultiplier = serverConfig.CurseStrengthMultiplier / 100f;
		public static float CursedEnemyVisualShaking => clientConfig.CursedEnemyVisualShaking / 100f;
		public static float CursedBuffSpawnRateMultiplier => serverConfig.CursedBuffSpawnRateMultiplier / 100f;
	}
}