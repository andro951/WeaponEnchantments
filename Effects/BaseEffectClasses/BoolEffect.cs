using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects {
    public abstract class BoolEffect : EnchantmentEffect {

        public bool EnableStat = true;
        protected BoolEffect(bool prevent = false) {
            EnableStat = !prevent;
        }

		public abstract EnchantmentStat statName { get; }

        protected virtual string modifierToString() {
            return EnableStat ? "Enabled" : "Prevented";
        }

        public override string Tooltip => $"{DisplayName}: {modifierToString()}";
    }
}
