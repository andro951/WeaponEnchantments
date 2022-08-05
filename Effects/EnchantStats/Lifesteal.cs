using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Configs;

namespace WeaponEnchantments.Effects {
    public class LifeSteal : StatEffect {
        public LifeSteal(StatModifier statModifier) : base(statModifier) { }

        public override bool isVanilla { get; } = false;
        public override string statName { get; } = "lifeSteal";
        public override string DisplayName { get; } = "Life Steal";
    }
}
