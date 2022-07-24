using System.Collections.Generic;

namespace WeaponEnchantments.Items.Enchantments.Utility
{
	public abstract class DangerSenseEnchantment : Enchantment
	{
		public override int EnchantmentValueTierReduction => -2;
		public override int LowestCraftableTier => 5;
		public override Dictionary<string, float> AllowedList => new Dictionary<string, float>() {
			{ "Weapon", 1f },
			{ "Armor", 1f },
			{ "Accessory", 1f }
		};
		public override void GetMyStats() {
			CheckBuffByName();
		}
	}
	public class DangerSenseEnchantmentUltraRare : DangerSenseEnchantment { }
}
