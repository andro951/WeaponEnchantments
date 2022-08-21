using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects
{
	public class DamageAfterDefenses : ClassedStatEffect, INonVanillaStat
    {
        public DamageAfterDefenses(DifficultyStrength additive = null, DifficultyStrength multiplicative = null, DifficultyStrength flat = null, DifficultyStrength @base = null, DamageClass dc = null) : base(additive, multiplicative, flat, @base, dc) {

        }

        public override string DisplayName { get; } = $"Damage (Applied after defenses. Not visible in weapon tooltip)";
		public override EnchantmentStat statType => EnchantmentStat.DamageAfterDefenses;
	}
}
