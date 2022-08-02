using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Common;
using WeaponEnchantments.EnchantmentEffects;

namespace WeaponEnchantments.Items.Enchantments {
    public abstract class EmptyTestEnchantment : Enchantment
	{
		public override EnchantmentEffect[] Effects { get => new EnchantmentEffect[] { new OverpoweredEffect(1), new LifeSteal(0.1f)}; }
	}

	public class EmptyTestEnchantmentBasic : EmptyTestEnchantment { }
}
