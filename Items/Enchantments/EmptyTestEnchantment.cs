using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Common;
using WeaponEnchantments.Items.Enchantments.EnchantmentEffects;

namespace WeaponEnchantments.Items.Enchantments {
    public abstract class EmptyTestEnchantment : Enchantment
	{
		public override EnchantmentEffect[] Effects { get => new EnchantmentEffect[] { new OverpoweredEffect()}; }

		public override bool? ShowPercentSignInTooltip => false;
		public override bool? MultiplyBy100InTooltip => false;
		public override int StrengthGroup => 4;
		public override Dictionary<string, float> AllowedList => new Dictionary<string, float>() {
			{ "Weapon", 1f }
		};
		public override void GetMyStats() {
			CheckStaticStatByName();
		}

		public override string Artist => "Zorutan";
		public override string Designer => "andro951";
	}

	public class EmptyTestEnchantmentBasic : EmptyTestEnchantment { }
}
