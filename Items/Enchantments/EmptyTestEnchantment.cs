using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Common;
using WeaponEnchantments.Items.Enchantments.EnchantmentEffects;

namespace WeaponEnchantments.Items.Enchantments {
    public abstract class EmptyTestEnchantment : Enchantment
	{
		public override EnchantmentEffect[] Effects { get => new EnchantmentEffect[] { new OverpoweredEffect(1)}; }
	}

	public class EmptyTestEnchantmentBasic : EmptyTestEnchantment { }
}
