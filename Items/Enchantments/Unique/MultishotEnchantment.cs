using System.Collections.Generic;

namespace WeaponEnchantments.Items.Enchantments.Unique
{
	public abstract class MultishotEnchantment : Enchantment
	{
		public override string CustomTooltip => "(Chance to produce an extra projectile.  Applies to each projectile created.)";
		public override int StrengthGroup => 8;
		public override int DamageClassSpecific => (int)DamageTypeSpecificID.Ranged;
		public override void GetMyStats() {
			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Weapon, 1f }
			};
		}
		public override string Artist => "Zorutan";
		public override string Designer => "andro951";
	}
	public class MultishotEnchantmentBasic : MultishotEnchantment { }
	public class MultishotEnchantmentCommon : MultishotEnchantment { }
	public class MultishotEnchantmentRare : MultishotEnchantment { }
	public class MultishotEnchantmentSuperRare : MultishotEnchantment { }
	public class MultishotEnchantmentUltraRare : MultishotEnchantment { }

}
