﻿using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using androLib.Items;
using androLib.Common.Utility;
using Terraria.ModLoader;

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
	[Autoload(false)]
	public class PennyPinchingEnchantmentBasic : PennyPinchingEnchantment
	{
		public override SellCondition SellCondition => SellCondition.PostEyeOfCthulhu;
		public override List<DropData> NpcDropTypes => new() {
			new(NPCID.EyeofCthulhu)
		};
	}
	[Autoload(false)]
	public class PennyPinchingEnchantmentCommon : PennyPinchingEnchantment { }
	[Autoload(false)]
	public class PennyPinchingEnchantmentRare : PennyPinchingEnchantment { }
	[Autoload(false)]
	public class PennyPinchingEnchantmentEpic : PennyPinchingEnchantment { }
	[Autoload(false)]
	public class PennyPinchingEnchantmentLegendary : PennyPinchingEnchantment { }

}
