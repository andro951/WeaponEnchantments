using System.Collections.Generic;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments.Utility
{
	public abstract class MovementSpeedEnchantment : Enchantment
	{
		public override int StrengthGroup => 11;
		public override void GetMyStats() {
			Effects = new() {
				new MovementSpeed(EnchantmentStrengthData)
			};

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Weapons, 1f },
				{ EItemType.Armor, 1f },
				{ EItemType.Accessories, 1f }
			};
		}

		public override string ShortTooltip => GetShortTooltip(sign: true);
		public override string Artist => "Zorutan";
		public override string Designer => "andro951";
	}
	public class MovementSpeedEnchantmentBasic : MovementSpeedEnchantment { }
	public class MovementSpeedEnchantmentCommon : MovementSpeedEnchantment { }
	public class MovementSpeedEnchantmentRare : MovementSpeedEnchantment { }
	public class MovementSpeedEnchantmentSuperRare : MovementSpeedEnchantment { }
	public class MovementSpeedEnchantmentUltraRare : MovementSpeedEnchantment { }
}