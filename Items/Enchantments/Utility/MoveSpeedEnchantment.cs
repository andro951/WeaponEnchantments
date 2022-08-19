using System.Collections.Generic;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments.Utility
{
	public abstract class MoveSpeedEnchantment : Enchantment
	{
		public override int StrengthGroup => 11;
		public override void GetMyStats() {
			Effects = new() {
				new MoveSpeed(EnchantmentStrengthData)
			};

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Weapon, 1f },
				{ EItemType.Armor, 1f },
				{ EItemType.Accessory, 1f }
			};
		}

		public override string Artist => "Zorutan";
		public override string Designer => "andro951";
	}
	public class MoveSpeedEnchantmentBasic : MoveSpeedEnchantment { }
	public class MoveSpeedEnchantmentCommon : MoveSpeedEnchantment { }
	public class MoveSpeedEnchantmentRare : MoveSpeedEnchantment { }
	public class MoveSpeedEnchantmentSuperRare : MoveSpeedEnchantment { }
	public class MoveSpeedEnchantmentUltraRare : MoveSpeedEnchantment { }
}