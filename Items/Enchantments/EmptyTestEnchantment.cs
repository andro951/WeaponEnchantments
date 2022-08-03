using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using WeaponEnchantments.EnchantmentEffects;
using static WeaponEnchantments.Common.Utility.UtilityMethods;

namespace WeaponEnchantments.Items.Enchantments {
    public abstract class EmptyTestEnchantment : Enchantment
	{
		public override EnchantmentEffect[] Effects { get => new EnchantmentEffect[] {
			new DebuffEffect(BuffID.OnFire, new Time(5), 0.10f + 0.5f * EnchantmentTier)
		}; }
	}

	public class EmptyTestEnchantmentBasic : EmptyTestEnchantment { }
}
