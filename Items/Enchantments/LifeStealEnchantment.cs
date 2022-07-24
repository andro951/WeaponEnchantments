using System.Collections.Generic;

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class LifeStealEnchantment : Enchantment
	{
		public override string CustomTooltip => "(remainder is saved to prevent always rounding to 0 for low damage weapons)";
		public override int StrengthGroup => 5;
		public override float ScalePercent => 0.8f;
		public override Dictionary<string, float> AllowedList => new Dictionary<string, float>() {
			{ "Weapon", 1f }
		};
	}
	public class LifeStealEnchantmentBasic : LifeStealEnchantment { }
	public class LifeStealEnchantmentCommon : LifeStealEnchantment { }
	public class LifeStealEnchantmentRare : LifeStealEnchantment { }
	public class LifeStealEnchantmentSuperRare : LifeStealEnchantment { }
	public class LifeStealEnchantmentUltraRare : LifeStealEnchantment { }

}
