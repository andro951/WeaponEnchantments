using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using WeaponEnchantments;
using WeaponEnchantments.Items;

namespace WeaponEnchantments.Common.Configs
{
    public class EnchantmentConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;
        [Header("$Mods.WeaponEnchantments.Config.Enchantment")]
        [Label("$Mods.WeaponEnchantments.Config.configStrMultiplier.Label")]
        [Tooltip("$Mods.WeaponEnchantments.Config.configStrMultiplier.Tooltip")]
        [DefaultValue(1f)]
        [ReloadRequired]
        public float configStrMultiplier;
    }
}
