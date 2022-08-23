using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments {
    public abstract class EmptyTestEnchantment : Enchantment
	{
		public override void GetMyStats() {
			Effects = new() {
				new OnTickPlayerDebuffEffect(BuffID.OnFire, 300, 0.10f + 0.5f * EnchantmentTier),
				new MaxMP(@base: EnchantmentStrengthData)
			};
		}
		public override string Artist => "Kiroto";
		public override string Designer => "Kiroto";
	}

	public class EmptyTestEnchantmentBasic : EmptyTestEnchantment { }
}
