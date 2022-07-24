using static WeaponEnchantments.WEMod;

namespace WeaponEnchantments.Common.Configs
{
	internal class ConfigValues
	{
		public static float LinearStrengthMultiplier => serverConfig.presetData.linearStrengthMultiplier / 100f;
		public static float RecomendedStrengthMultiplier => serverConfig.presetData.recomendedStrengthMultiplier / 100f;
		public static float EnchantmentDropChance => serverConfig.EnchantmentDropChance / 100f;
		public static float BossEnchantmentDropChance => serverConfig.BossEnchantmentDropChance / 100f;
		public static float EssenceMultiplier => serverConfig.EssenceMultiplier / 100f;
		public static float BossEssenceMultiplier => serverConfig.BossEssenceMultiplier / 100f;
	}
}
