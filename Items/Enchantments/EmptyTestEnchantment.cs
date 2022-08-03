using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using WeaponEnchantments.EnchantmentEffects;

namespace WeaponEnchantments.Items.Enchantments {
    public abstract class EmptyTestEnchantment : Enchantment
	{
		public override EnchantmentEffect[] Effects { get => new EnchantmentEffect[] { // Right now it's "recalculated" every time it's applied. Might want to make this lazy.
			new DebuffEffect(BuffID.OnFire, new Time(5), 0.10f + 0.5f * EnchantmentTier),
			new BuffEffect(BuffID.OnFire)
		}; }
	}

	public class EmptyTestEnchantmentBasic : EmptyTestEnchantment { }
}
