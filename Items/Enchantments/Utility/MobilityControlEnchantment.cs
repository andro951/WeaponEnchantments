using System.Collections.Generic;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments.Utility {
    public abstract class MobilityControlEnchantment : Enchantment {
        public override int StrengthGroup => 12;
		public override void GetMyStats() {
            Effects = new() {
                new MaxFallSpeed(EnchantmentStrengthData),
                new MovementSlowdown(EnchantmentStrengthData),
                new MovementAcceleration(EnchantmentStrengthData),
                new JumpSpeed(@base: EnchantmentStrengthData * 2.5f),
            };
            
            AllowedList = new Dictionary<EItemType, float>() {
                { EItemType.Weapons, 1f },
                { EItemType.Armor, 1f },
                { EItemType.Accessories, 1f },
                { EItemType.FishingPoles, 1f },
                { EItemType.Tools, 1f }
            };
        }

		public override string Artist => "Sir Bumpleton ✿";
        public override string Designer => "Sir Bumpleton ✿";
    }
    public class MobilityControlEnchantmentBasic : MobilityControlEnchantment { }
    public class MobilityControlEnchantmentCommon : MobilityControlEnchantment { }
    public class MobilityControlEnchantmentRare : MobilityControlEnchantment { }
    public class MobilityControlEnchantmentSuperRare : MobilityControlEnchantment { }
    public class MobilityControlEnchantmentUltraRare : MobilityControlEnchantment { }

}
