using System.Linq;
using Terraria;
using static WeaponEnchantments.WEMod;

namespace WeaponEnchantments.Common.Configs
{
	internal class ConfigValues {
		public static readonly float[] PresetMultipliers = { 1f, 0.5f, 0.25f, 2.5f };

		private static float _globalStrengthMultiplier = serverConfig.presetData.GlobalEnchantmentStrengthMultiplier / 100f;
		public static float GlobalStrengthMultiplier {
			get {
				if (serverConfig.presetData.AutomaticallyMatchPreseTtoWorldDifficulty)
					return PresetMultipliers[Main.GameMode];

				return _globalStrengthMultiplier;
			}
		}
		public static float EnchantmentDropChance = serverConfig.EnchantmentDropChance / 100f;
		public static float BossEnchantmentDropChance = serverConfig.BossEnchantmentDropChance / 100f;
		public static float EssenceMultiplier = serverConfig.EssenceMultiplier / 100f;
		public static float BossEssenceMultiplier = serverConfig.BossEssenceMultiplier / 100f;
		public static float GatheringExperienceMultiplier => serverConfig.GatheringExperienceMultiplier / 100f;
		public static bool MultiplicativeCriticalHits => serverConfig.MultiplicativeCriticalHits;
		public static bool AlwaysOverrideDamageType => serverConfig.AlwaysOverrideDamageType;
		public static float InfusionDamageMultiplier = serverConfig.InfusionDamageMultiplier / 1000f;
		public static float BossXPMultiplier => serverConfig.BossExperienceMultiplier / 100f;
		public static float NormalXPMultiplier => serverConfig.ExperienceMultiplier / 100f;
		public static float AffectOnVanillaLifeStealLimit => serverConfig.AffectOnVanillaLifeStealLimmit / 100f;
		public static float AttackSpeedEnchantmentAutoReuseSetpoint = serverConfig.AttackSpeedEnchantmentAutoReuseSetpoint / 100f;
		public static float PercentOfferEssence => clientConfig.PercentOfferEssence / 100f;
		public static float ChestSpawnChance => serverConfig.ChestSpawnChance / 100f;
		public static float CrateDropChance => serverConfig.CrateDropChance / 100f;
		public static int MaxSlotTierAllowed = new int[] { serverConfig.EnchantmentSlotsOnWeapons, serverConfig.EnchantmentSlotsOnArmor, serverConfig.EnchantmentSlotsOnAccessories }.Max() - 1;
	}
}