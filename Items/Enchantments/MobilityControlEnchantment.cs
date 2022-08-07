using System.Collections.Generic;
using Terraria.ModLoader;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments {
    public abstract class ControlEnchantEnchantment : Enchantment {
        public override EnchantmentEffect[] Effects => new EnchantmentEffect[] {
            new MaxFallSpeed(EnchantmentStrength * 100),
            new MoveSlowDown(EnchantmentStrength * 100),
            new MoveAcceleration(EnchantmentStrength * 100),
            new JumpSpeed(@base: 5f),
        };

        public override bool Max1 => true;

        public override string Artist => "Sir Bumpleton ✿";
        public override string Designer => "Sir Bumpleton ✿";
    }
    public class ControlEnchantEnchantmentBasic : ControlEnchantEnchantment { }
    public class ControlEnchantEnchantmentCommon : ControlEnchantEnchantment { }
    public class ControlEnchantEnchantmentRare : ControlEnchantEnchantment { }
    public class ControlEnchantEnchantmentSuperRare : ControlEnchantEnchantment { }
    public class ControlEnchantEnchantmentUltraRare : ControlEnchantEnchantment { }

}
