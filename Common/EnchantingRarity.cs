using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace WeaponEnchantments.Common
{
    public abstract class EnchantingRarity : ModRarity {
        //public static readonly string[] tierNames = new string[] { "Basic", "Common", "Rare", "Epic", "Legendary" };
        public static readonly string[] tierNames = new string[] { "Basic", "Common", "Rare", "Epic", "Legendary", "Cursed" };
        public static Color[] TierColors => WEMod.clientConfig.UseAlternateEnchantmentEssenceTextures ? altTierColors : normalTierColors;
        private static readonly Color[] normalTierColors = new Color[] { 
            new Color(0x2E, 0x7F, 0x4C), 
            new Color(0x1F, 0xD4, 0xDA), 
            new Color(0x67, 0x26, 0xA1), 
            new Color(0xF9, 0x00, 0x23), 
            new Color(0xD7, 0x54, 0x09),
            new Color(10, 10, 10)
        };
        private static readonly Color[] altTierColors = new Color[] { 
            new Color(0x3C, 0xA4, 0x62),
            new Color(0x3A, 0x4C, 0xBF), 
            new Color(0x81, 0x30, 0xC9), 
            new Color(0xCE, 0x2B, 0x42),
            new Color(0xEF, 0x5D, 0x0A),
            new Color(10, 10, 10)
        };

        public override Color RarityColor => TierColors[GetTierNumberFromName(Name)];
        public static int GetRarityFromTier(int tier) {
			switch (tier) {
                case 1:
                    return ModContent.RarityType<EnchantingRarityCommon>();
                case 2:
                    return ModContent.RarityType<EnchantingRarityRare>();
                case 3:
                    return ModContent.RarityType<EnchantingRarityEpic>();
                case 4:
                    return ModContent.RarityType<EnchantingRarityLegendary>();
                case 5:
                    return ModContent.RarityType<EnchantingRarityCursed>();
                default:
                    return ModContent.RarityType<EnchantingRarityBasic>();
            }
		}

		public static int GetTierNumberFromName(string name) {
            for (int i = tierNames.Length - 1; i >= 0; i--) {
                string tierName = tierNames[i];
                int tierNameIndex = name.IndexOf(tierName);
                if (tierNameIndex > -1)
                    return i;
            }

            return -1;
        }
        public static string GetTierNameFromColor(Color color) {
            Color[] colors = TierColors;
            for (int i = 0; i < tierNames.Length; i++) {
                if (color == colors[i])
                    return tierNames[i];
			}

            return "Failed to find color name";
        }
        public class EnchantingRarityBasic : EnchantingRarity { }
        public class EnchantingRarityCommon : EnchantingRarity { }
        public class EnchantingRarityRare : EnchantingRarity { }
        public class EnchantingRarityEpic : EnchantingRarity { }
        public class EnchantingRarityLegendary : EnchantingRarity { }
        public class EnchantingRarityCursed : EnchantingRarity { }
    }
}
