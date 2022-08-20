using System.Collections.Generic;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class DefenseEnchantment : Enchantment
	{
		public override int StrengthGroup => 3;
		public override string MyDisplayName => "Defence";
		public override int LowestCraftableTier => 0;
		public override void GetMyStats() {
			Effects = new() {
				new DefenseEffect(@base: EnchantmentStrengthData),
			};

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Weapons, 0.5f },
				{ EItemType.Armor, 1f },
				{ EItemType.Accessories, 1f }
			};
		}

		public override string ShortTooltip => GetShortTooltip(sign: true, percent: false, multiply100: false);
		public override string Artist => "Zorutan";
		public override string Designer => "andro951";
	}
	public class DefenseEnchantmentBasic : DefenseEnchantment { }
	public class DefenseEnchantmentCommon : DefenseEnchantment { }
	public class DefenseEnchantmentRare : DefenseEnchantment { }
	public class DefenseEnchantmentSuperRare : DefenseEnchantment { }
	public class DefenseEnchantmentUltraRare : DefenseEnchantment { }

}