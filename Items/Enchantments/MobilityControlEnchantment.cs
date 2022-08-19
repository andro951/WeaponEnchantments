using System.Collections.Generic;
using Terraria.ModLoader;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments {
    public abstract class ControlEnchantEnchantment : Enchantment {
        public override int StrengthGroup => 12;
        public override bool Max1 => true;
		public override void GetMyStats() {
            Effects = new() {
                new MaxFallSpeed(EnchantmentStrengthData),
                new MoveSlowDown(EnchantmentStrengthData),
                new MoveAcceleration(EnchantmentStrengthData),
                new JumpSpeed(@base: EnchantmentStrengthData * (50f / 3f)),
            };
		}

		public override string Artist => "Sir Bumpleton ✿";
        public override string Designer => "Sir Bumpleton ✿";
    }
    public class ControlEnchantEnchantmentBasic : ControlEnchantEnchantment { }
    public class ControlEnchantEnchantmentCommon : ControlEnchantEnchantment { }
    public class ControlEnchantEnchantmentRare : ControlEnchantEnchantment { }
    public class ControlEnchantEnchantmentSuperRare : ControlEnchantEnchantment { }
    public class ControlEnchantEnchantmentUltraRare : ControlEnchantEnchantment { }

}
