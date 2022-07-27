using System.Collections.Generic;

namespace WeaponEnchantments.Items.Enchantments.Utility
{
	public abstract class MoveSpeedEnchantment : Enchantment
	{
		public override int StrengthGroup => 11;
		public override Dictionary<string, float> AllowedList => new Dictionary<string, float>() {
			{ "Weapon", 1f },
			{ "Armor", 1f },
			{ "Accessory", 1f }
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