using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects {
    public abstract class StatEffect : EnchantmentEffect {
        protected StatEffect(StatModifier sm) {
            statModifier = sm;
        }

        protected StatEffect(float additive = 0f, float multiplicative = 1f, float flat = 0f, float @base = 0f) {
            statModifier = new StatModifier(1f + additive, multiplicative, flat, @base);
		}

        protected StatModifier statModifier;
        public StatModifier StatModifier { 
            get {
                float additive = 1f + (statModifier.Additive - 1f) * EfficiencyMultiplier;
                float multiplicative = 1f + (statModifier.Multiplicative - 1f) * EfficiencyMultiplier;
                float flat = statModifier.Flat * EfficiencyMultiplier;
                float @base = statModifier.Base * EfficiencyMultiplier;

                return new StatModifier(additive, multiplicative, flat, @base);
            }
        }
        public abstract EditableStat statName { get; }

        protected virtual string modifierToString() {
            string final = "";
            float mult = StatModifier.Multiplicative + StatModifier.Additive - 2;
            float flats = StatModifier.Base * mult + StatModifier.Flat;

            if (flats > 0f) {
                final += $"{s(flats)}{flats}";
            }

            if (mult > 0f) {
                if (final != "") final += ' ';
                final += $"{s(mult)}{mult.Percent()}%";
            }

            return final;
        }

        public override string Tooltip => $"{DisplayName}: {modifierToString()}";
    }
}
