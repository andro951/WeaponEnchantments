using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using WeaponEnchantments.Items;

namespace WeaponEnchantments.Common
{
    public class EnchantingRarityBasic : ModRarity
    {
        public override Color RarityColor => EnchantmentEssenceBasic.EssenceColor;
    }
    public class EnchantingRarityCommon : ModRarity
    {
        public override Color RarityColor => EnchantmentEssenceCommon.EssenceColor;
    }
    public class EnchantingRarityRare : ModRarity
    {
        public override Color RarityColor => EnchantmentEssenceRare.EssenceColor;
    }
    public class EnchantingRaritySuperRare : ModRarity
    {
        public override Color RarityColor => EnchantmentEssenceSuperRare.EssenceColor;
    }
    public class EnchantingRarityUltraRare : ModRarity
    {
        public override Color RarityColor => EnchantmentEssenceUltraRare.EssenceColor;
    }
}
