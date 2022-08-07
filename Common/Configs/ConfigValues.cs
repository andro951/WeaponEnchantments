using System.Linq;
using static WeaponEnchantments.WEMod;

namespace WeaponEnchantments.Common.Configs
{
	internal class ConfigValues {
		public static float RecomendedStrengthMultiplier = serverConfig.presetData.GlobalEnchantmentStrengthMultiplier / 100f;
		public static float EnchantmentDropChance = serverConfig.EnchantmentDropChance / 100f;
		public static float BossEnchantmentDropChance = serverConfig.BossEnchantmentDropChance / 100f;
		public static float EssenceMultiplier = serverConfig.EssenceMultiplier / 100f;
		public static float BossEssenceMultiplier = serverConfig.BossEssenceMultiplier / 100f;
		public static float GatheringExperienceMultiplier = serverConfig.GatheringExperienceMultiplier / 100f;

		public static bool MultiplicativeCriticalHits = serverConfig.MultiplicativeCriticalHits;
		public static bool AlwaysOverrideDamageType = serverConfig.AlwaysOverrideDamageType;
		public static float InfusionDamageMultiplier = serverConfig.InfusionDamageMultiplier / 1000f;
		public static float BossXPMultiplier = serverConfig.BossExperienceMultiplier / 100f;
		public static float NormalXPMultiplier = serverConfig.ExperienceMultiplier / 100f;
		public static float AffectOnVanillaLifeStealLimit = serverConfig.AffectOnVanillaLifeStealLimmit / 100f;
		public static float SpeedEnchantmentAutoReuseSetpoint = serverConfig.SpeedEnchantmentAutoReuseSetpoint / 100f;
		public static float GlobalEnchantmentStrengthMultiplier = serverConfig.presetData.GlobalEnchantmentStrengthMultiplier / 100f;
		public static float PercentOfferEssence = serverConfig.PercentOfferEssence / 100f;
		public static float ChestSpawnChance = serverConfig.ChestSpawnChance / 100f;
		public static int MaxSlotTierAllowed = new int[] { serverConfig.EnchantmentSlotsOnWeapons, serverConfig.EnchantmentSlotsOnArmor, serverConfig.EnchantmentSlotsOnAccessories }.Max() - 1;
	}
}
