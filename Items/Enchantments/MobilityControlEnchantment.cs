using System.Collections.Generic;
using Terraria.ModLoader;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments {
    public abstract class MobilityControl : Enchantment {
        public override EnchantmentEffect[] Effects => new EnchantmentEffect[] {
            new MaxFallSpeed(EnchantmentStrength),
            new MoveSlowDown(EnchantmentStrength),
            new MoveAcceleration(EnchantmentStrength),
            new JumpSpeed(EnchantmentStrength),
        };

        public override bool Max1 => true;

        public override string Artist => "Sir Bumpleton ✿";
        public override string Designer => "Sir Bumpleton ✿";
    }
    public class MobilityControlEnchantmentBasic : MobilityControl { }
    public class MobilityControlEnchantmentCommon : MobilityControl { }
    public class MobilityControlEnchantmentRare : MobilityControl { }
    public class MobilityControlEnchantmentSuperRare : MobilityControl { }
    public class MobilityControlEnchantmentUltraRare : MobilityControl { }

}
