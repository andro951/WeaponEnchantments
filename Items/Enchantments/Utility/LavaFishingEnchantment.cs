using System.Collections.Generic;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments.Utility {
    public abstract class LavaFishingEnchantment : Enchantment {
		public override int StrengthGroup => 8;
		public override bool Max1 => true;
		public override void GetMyStats() {
            Effects = new() {
                new LavaFishing(@base: EnchantmentStrengthData)
            };

            AllowedList = new Dictionary<EItemType, float>() {
                { EItemType.FishingPoles, 1f }
            };
        }

        public override string ShortTooltip => GetShortTooltip(sign: true);
        public override string Artist => "andro951";
        public override string Designer => "Fran";
    }
    public class LavaFishingEnchantmentBasic : LavaFishingEnchantment { }
    public class LavaFishingEnchantmentCommon : LavaFishingEnchantment { }
    public class LavaFishingEnchantmentRare : LavaFishingEnchantment { }
    public class LavaFishingEnchantmentSuperRare : LavaFishingEnchantment { }
    public class LavaFishingEnchantmentUltraRare : LavaFishingEnchantment { }

}
