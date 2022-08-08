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
			Effects = new EnchantmentEffect[] {
				new DebuffEffect(BuffID.OnFire, new Time(5), 0.10f + 0.5f * EnchantmentTier),
				new MaxMP(1,1, 100)
			};
		}
		public override string Artist => "Kiroto";
		public override string Designer => "Kiroto";
	}

	public class EmptyTestEnchantmentBasic : EmptyTestEnchantment { }
}
