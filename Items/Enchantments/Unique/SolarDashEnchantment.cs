using System.Collections.Generic;
using WeaponEnchantments.Common.Utility;

namespace WeaponEnchantments.Items.Enchantments.Unique
{
	public abstract class SolarDashEnchantment : Enchantment
	{
		public override int StrengthGroup => 10;
		public override float ScalePercent => 0.6f;
		public override int ArmorSlotSpecific => (int)ArmorSlotSpecificID.Legs;
		public override void GetMyStats() {
			Effects = new() {
				
			}
			//AddStaticStat("dashType", 0f, 1f, 0f, 3f);

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Armor, 1f }
			};
		}

		public override string Artist => "andro951";
		public override string Designer => "andro951";
	}
	public class SolarDashEnchantmentBasic : SolarDashEnchantment { }
	public class SolarDashEnchantmentCommon : SolarDashEnchantment { }
	public class SolarDashEnchantmentRare : SolarDashEnchantment { }
	public class SolarDashEnchantmentSuperRare : SolarDashEnchantment { }
	public class SolarDashEnchantmentUltraRare : SolarDashEnchantment { }
}
