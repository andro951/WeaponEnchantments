using System.Collections.Generic;
using Terraria.ModLoader;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments {
    public abstract class ControlEnchantEnchantment : Enchantment {
        public override EnchantmentEffect[] Effects => new EnchantmentEffect[] {
            new MaxFallSpeed(EnchantmentStrength),
            new MoveSlowDown(EnchantmentStrength),
            new MoveAcceleration(EnchantmentStrength),
            new JumpSpeed(@base: 5f),
        };

        public override int StrengthGroup => 12;

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
