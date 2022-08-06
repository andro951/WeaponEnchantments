using System.Collections.Generic;

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class StatDefenseEnchantment : Enchantment
	{
		public override bool? ShowPercentSignInTooltip => false;
		public override bool? MultiplyBy100InTooltip => false;
		public override int StrengthGroup => 3;
		public override string MyDisplayName => "Defence";
		public override int LowestCraftableTier => 0;
		public override Dictionary<EItemType, float> AllowedList => new Dictionary<EItemType, float>() {
			{ EItemType.Weapon, 0.5f },
			{ EItemType.Armor, 1f },
			{ EItemType.Accessory, 1f }
		};
		public override void GetMyStats() {
			CheckStaticStatByName();
		}

		public override string Artist => "Zorutan";
		public override string Designer => "andro951";
	}
	public class StatDefenseEnchantmentBasic : StatDefenseEnchantment { }
	public class StatDefenseEnchantmentCommon : StatDefenseEnchantment { }
	public class StatDefenseEnchantmentRare : StatDefenseEnchantment { }
	public class StatDefenseEnchantmentSuperRare : StatDefenseEnchantment { }
	public class StatDefenseEnchantmentUltraRare : StatDefenseEnchantment { }

}
