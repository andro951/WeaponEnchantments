using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments.Utility
{
	public abstract class HunterEnchantment : Enchantment
	{
		public override int EnchantmentValueTierReduction => -2;
		public override int LowestCraftableTier => 5;
		public override void GetMyStats() {
			Effects = new() {
				new OnTickPlayerBuffEffect(BuffID.Hunter)
			};

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Weapons, 1f },
				{ EItemType.Armor, 1f },
				{ EItemType.Accessories, 1f }
			};
		}

		public override string Artist => "Zorutan";
		public override string Designer => "andro951";
	}
	public class HunterEnchantmentUltraRare : HunterEnchantment { }
}
