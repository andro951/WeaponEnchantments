using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Common;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class ArmorPenetrationEnchantment : Enchantment
	{
		public override int StrengthGroup => 4;
		public override void GetMyStats() {
			Effects = new EnchantmentEffect[] {
				new ArmorPenetration(@base: EnchantmentStrength),
			};
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
