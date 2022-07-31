using Microsoft.Xna.Framework;
using System;
using Terraria.ModLoader;
using WeaponEnchantments.Items;

namespace WeaponEnchantments.Common
{
    public class EnchantingRarityBasic : ModRarity
    {
        public override Color RarityColor => ((EssenceColorAttribute)Attribute.GetCustomAttribute(typeof(EnchantmentEssenceBasic), typeof(EssenceColorAttribute))).color;
    }
    public class EnchantingRarityCommon : ModRarity
    {
        public override Color RarityColor => ((EssenceColorAttribute)Attribute.GetCustomAttribute(typeof(EnchantmentEssenceCommon), typeof(EssenceColorAttribute))).color;
    }
    public class EnchantingRarityRare : ModRarity
    {
        public override Color RarityColor => ((EssenceColorAttribute)Attribute.GetCustomAttribute(typeof(EnchantmentEssenceRare), typeof(EssenceColorAttribute))).color;
    }
    public class EnchantingRaritySuperRare : ModRarity
    {
        public override Color RarityColor => ((EssenceColorAttribute)Attribute.GetCustomAttribute(typeof(EnchantmentEssenceSuperRare), typeof(EssenceColorAttribute))).color;
    }
    public class EnchantingRarityUltraRare : ModRarity
    {
        public override Color RarityColor => ((EssenceColorAttribute)Attribute.GetCustomAttribute(typeof(EnchantmentEssenceUltraRare), typeof(EssenceColorAttribute))).color;
    }
}
