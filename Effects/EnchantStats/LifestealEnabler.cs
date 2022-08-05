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
    public class LifeStealEnabler : StatEffect {
        public LifeStealEnabler() : base(new StatModifier()) { }

        public override bool isVanilla { get; } = false;
        public override string statName { get; } = "canLifeSteal";
        public override string DisplayName { get; } = "EnableLifesteal";
        public override bool showTooltip { get; } = false;
    }
}
