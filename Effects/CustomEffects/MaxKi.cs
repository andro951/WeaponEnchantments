﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects.CustomEffects
{
    public class MaxKi : StatEffect, IVanillaStat
    {
        public MaxKi(DifficultyStrength additive = null, DifficultyStrength multiplicative = null, DifficultyStrength flat = null, DifficultyStrength @base = null) : base(additive, multiplicative, flat, @base)
        {

        }
        public MaxKi(EStatModifier eStatModifier) : base(eStatModifier) { }
        public override EnchantmentEffect Clone()
        {
            return new MaxKi(EStatModifier.Clone());
        }

        public override EnchantmentStat statName => EnchantmentStat.MaxKi;
        public override string TooltipValue => $"+{EStatModifier.ApplyTo(0)}";
    }
}
