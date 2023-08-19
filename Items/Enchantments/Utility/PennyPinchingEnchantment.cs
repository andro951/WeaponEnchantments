using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using androLib.Items;
using androLib.Common.Utility;

namespace WeaponEnchantments.Items.Enchantments.Utility
{
	public abstract class PennyPinchingEnchantment : Enchantment
	{
		public override int StrengthGroup => 10;
		public override float ScalePercent => 2f/3f;
		public override void GetMyStats() {
			Effects = new() {
				new BonusCoins(@base: EnchantmentStrengthData)
			};

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Weapons, 1f },
				{ EItemType.Armor, 0.5f },
				{ EItemType.Accessories, 0.5f }
			};
		}

		public override string Artist => "andro951";
		public override string ArtModifiedBy => null;
		public override string Designer => "andro951";
	}
	public class PennyPinchingEnchantmentBasic : PennyPinchingEnchantment
	{
		public override SellCondition SellCondition => SellCondition.PostEyeOfCthulhu;
		public override List<DropData> NpcDropTypes => new() {
			new(NPCID.EyeofCthulhu)
		};
	}
	public class PennyPinchingEnchantmentCommon : PennyPinchingEnchantment { }
	public class PennyPinchingEnchantmentRare : PennyPinchingEnchantment { }
	public class PennyPinchingEnchantmentEpic : PennyPinchingEnchantment { }
	public class PennyPinchingEnchantmentLegendary : PennyPinchingEnchantment { }

}
