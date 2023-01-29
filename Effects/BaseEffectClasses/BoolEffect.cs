using System.Collections.Generic;
using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects {
    public abstract class BoolEffect : EnchantmentEffect, IEnchantmentStat {

        public bool EnableStat { get; protected set; }
        protected BoolEffect(bool prevent) {
            EnableStat = !prevent;
        }

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
    }
}
