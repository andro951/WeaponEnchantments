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
		public override Dictionary<string, float> AllowedList => new Dictionary<string, float>() {
			{ "Weapon", 0.5f },
			{ "Armor", 1f },
			{ "Accessory", 1f }
		};
		public override void GetMyStats() {
			CheckStaticStatByName();
		}
	}
	public class StatDefenseEnchantmentBasic : StatDefenseEnchantment { }
	public class StatDefenseEnchantmentCommon : StatDefenseEnchantment { }
	public class StatDefenseEnchantmentRare : StatDefenseEnchantment { }
	public class StatDefenseEnchantmentSuperRare : StatDefenseEnchantment { }
	public class StatDefenseEnchantmentUltraRare : StatDefenseEnchantment { }

}
