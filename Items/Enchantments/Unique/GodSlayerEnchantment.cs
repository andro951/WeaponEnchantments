using System.Collections.Generic;

namespace WeaponEnchantments.Items.Enchantments.Unique
{
	public abstract class GodSlayerEnchantment : Enchantment
	{
		public override string CustomTooltip => "(Bonus true damage based on enemy max hp)\n(Bonus damage not affected by LifeSteal)";
		public override int StrengthGroup => 7;
		public override int DamageClassSpecific => (int)DamageTypeSpecificID.Melee;
		public override void GetMyStats() {
			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Weapon, 1f }
			};
		}
		public override string Artist => "Zorutan";
		public override string Designer => "andro951";
	}
	public class GodSlayerEnchantmentBasic : GodSlayerEnchantment { }
	public class GodSlayerEnchantmentCommon : GodSlayerEnchantment { }
	public class GodSlayerEnchantmentRare : GodSlayerEnchantment { }
	public class GodSlayerEnchantmentSuperRare : GodSlayerEnchantment { }
	public class GodSlayerEnchantmentUltraRare : GodSlayerEnchantment { }

}
