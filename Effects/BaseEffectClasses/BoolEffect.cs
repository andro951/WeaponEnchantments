using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects {
    public abstract class BoolEffect : EnchantmentEffect {

        public bool EnableStat { get; protected set; }
        protected BoolEffect(float minimumStrength, DifficultyStrength strengthData, bool prevent) {
            MinimumStrength = minimumStrength;
            EnableStat = !prevent;
            StrengthData = strengthData;
        }

        public DifficultyStrength StrengthData;
        public float MinimumStrength;
		public abstract EnchantmentStat statName { get; }

        public override string Tooltip {
            get {
                if (StrengthData != null && MinimumStrength > StrengthData.Value)
                    return "";

                return $"{DisplayName}{": " + (EnableStat ? "Enabled" : "Prevented")}";
            }
		}
    }
}
