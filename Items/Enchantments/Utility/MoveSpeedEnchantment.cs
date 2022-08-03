using System.Collections.Generic;

namespace WeaponEnchantments.Items.Enchantments.Utility
{
	public abstract class MoveSpeedEnchantment : Enchantment
	{
		public override int StrengthGroup => 11;
		public override Dictionary<EItemType, float> AllowedList => new Dictionary<EItemType, float>() {
			{ EItemType.Weapon, 1f },
			{ EItemType.Armor, 1f },
			{ EItemType.Accesory, 1f }
		};
		public override void GetMyStats() {
			CheckStaticStatByName();
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