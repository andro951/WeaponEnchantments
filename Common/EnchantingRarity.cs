using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace WeaponEnchantments.Common
{
    public abstract class EnchantingRarity : ModRarity {
        public static readonly int enchantingRarityBasic = ModContent.RarityType<EnchantingRarityBasic>();

        public static readonly string[] tierNames = new string[] { "Basic", "Common", "Rare", "SuperRare", "UltraRare" };
        public static readonly string[] displayTierNames = new string[] { "Basic", "Common", "Rare", "Epic", "Legendary" };
        public static Color[] TierColors => WEMod.clientConfig.UseAlternateEnchantmentEssenceTextures ? altTierColors : normalTierColors;
        private static readonly Color[] normalTierColors = new Color[] { 
            new Color(0x2E, 0x7F, 0x4C), 
            new Color(0x1F, 0xD4, 0xDA), 
            new Color(0x67, 0x26, 0xA1), 
            new Color(0xF9, 0x00, 0x23), 
            new Color(0xD7, 0x54, 0x09) 
        };
        private static readonly Color[] altTierColors = new Color[] { 
            new Color(0x3C, 0xA4, 0x62), 
            new Color(0x3A, 0x4C, 0xBF), 
            new Color(0x81, 0x30, 0xC9), 
            new Color(0xCE, 0x2B, 0x42), 
            new Color(0xEF, 0x5D, 0x0A) 
        };

        public override Color RarityColor => TierColors[GetTierNumberFromName(Name)];
        public static int GetRarityFromTier(int tier) => enchantingRarityBasic + tier;

        public static int GetTierNumberFromName(string name) {
            for (int i = tierNames.Length - 1; i >= 0 ; i--) {
                string tierName = tierNames[i];
                int tierNameIndex = name.IndexOf(tierName);
                if (tierNameIndex > -1) {
                    return i;
				}
            }//Get EnchantmentSize
            
            return -1;
        }

        public class EnchantingRarityBasic : EnchantingRarity { }
        public class EnchantingRarityCommon : EnchantingRarity { }
        public class EnchantingRarityRare : EnchantingRarity { }
        public class EnchantingRaritySuperRare : EnchantingRarity { }
        public class EnchantingRarityUltraRare : EnchantingRarity { }
    }
}
