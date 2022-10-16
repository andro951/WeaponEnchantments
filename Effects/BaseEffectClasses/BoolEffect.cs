using System.Collections.Generic;
using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects {
    public abstract class BoolEffect : EnchantmentEffect, IEnchantmentStat {

        public bool EnableStat { get; protected set; }
        protected BoolEffect(float minimumStrength, DifficultyStrength strengthData, bool prevent) {
            MinimumStrength = minimumStrength;
            EnableStat = !prevent;
            StrengthData = strengthData;
        }

        public DifficultyStrength StrengthData;
        public float MinimumStrength;
        public abstract EnchantmentStat statName { get; }
        public override IEnumerable<object> TooltipArgs => new string[] { DisplayName };
        public override string TooltipKey => EnableStat ? "Enabled" : "Prevented";
		public override string TooltipValue => TooltipKey;
		public override string TooltipName {
            get {
                if (tooltipName == null)
                    tooltipName = typeof(BoolEffect).Name;

                return tooltipName;
            }
        }
        private string tooltipName;
        /*
        public static override Dictionary<string, string> LocalizationTooltips => new() {
            { "Enabled", "\"{0} Enabled\"" },
            { "Disabled", "\"{0} Disabled\"" }
        };
        */
    }
}
