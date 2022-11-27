﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects.CustomEffects
{
    public class KiRegen : StatEffect, IVanillaStat
    {
        public KiRegen(DifficultyStrength additive = null, DifficultyStrength multiplicative = null, DifficultyStrength flat = null, DifficultyStrength @base = null) : base(additive, multiplicative, flat, @base)
        {

        }
        public KiRegen(EStatModifier eStatModifier) : base(eStatModifier) { }
        public override EnchantmentEffect Clone()
        {
            return new KiRegen(EStatModifier.Clone());
        }

        public override EnchantmentStat statName => EnchantmentStat.KiRegen;
        public override string TooltipValue => $"+{Math.Ceiling((EStatModifier.ApplyTo(0) * 60) / 3)}";
    }
}
