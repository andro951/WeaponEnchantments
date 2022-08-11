using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects {
    public abstract class BoolEffect : EnchantmentEffect {
        protected BoolEffect(EStatModifier sm) {
            EStatModifier = sm;
        }

        protected BoolEffect(float additive = 0f, float multiplicative = 1f, float flat = 0f, float @base = 0f) {
            EStatModifier = new EStatModifier(additive, multiplicative, flat, @base);
		}

        public EStatModifier EStatModifier = EStatModifier.Default;
        public StatModifier StatModifier => EStatModifier.StatModifier;
        public float EffectStrength => EStatModifier.Strength;
		public override float EfficiencyMultiplier { 
            get => EStatModifier.EfficiencyMultiplier; 
            set => EStatModifier.EfficiencyMultiplier = value; 
        }

		public abstract EditableStat statName { get; }

        protected virtual string modifierToString() {
            string final = "";
            float mult = EStatModifier.Multiplicative + EStatModifier.Additive - 2;
            float flats = EStatModifier.Base * mult + EStatModifier.Flat;

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
