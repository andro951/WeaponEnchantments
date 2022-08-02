using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Common;

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class ArmorPenetrationEnchantment : Enchantment
	{
		public override bool? ShowPercentSignInTooltip => false;
		public override bool? MultiplyBy100InTooltip => false;
		public override int StrengthGroup => 4;
		public override Dictionary<string, float> AllowedList => new Dictionary<string, float>() {
			{ "Weapon", 1f }
		};
		public override void GetMyStats() {
			CheckStaticStatByName();
		}

		public override string Artist => "Zorutan";
		public override string Designer => "andro951";
	}
	public class ArmorPenetrationEnchantmentBasic : ArmorPenetrationEnchantment { }
	public class ArmorPenetrationEnchantmentCommon : ArmorPenetrationEnchantment { }
	public class ArmorPenetrationEnchantmentRare : ArmorPenetrationEnchantment { }
	public class ArmorPenetrationEnchantmentSuperRare : ArmorPenetrationEnchantment { }
	public class ArmorPenetrationEnchantmentUltraRare : ArmorPenetrationEnchantment { }
}
