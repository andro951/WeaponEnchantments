using System.Collections.Generic;
using Terraria.ModLoader;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments {
    public abstract class ControlEnchantEnchantment : Enchantment {
        public override EnchantmentEffect[] Effects => new EnchantmentEffect[] {
            new MaxFallSpeed(0.1f + 0.04f * EnchantmentTier),
            new MoveSlowDown(0.1f + 0.04f * EnchantmentTier),
            new MoveAcceleration(0.1f + 0.04f * EnchantmentTier),
            new JumpSpeed(@base: 5f),
        };

        public override int StrengthGroup => base.StrengthGroup;

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
