using System;
using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments.Utility
{
	public abstract class WarEnchantment : Enchantment
	{
		public override int StrengthGroup => 2;
		public override float ScalePercent => -1f;
		public override void GetMyStats() {
			Effects = new() {
				new EnemyMaxSpawns(multiplicative: EnchantmentStrengthData),
				new EnemySpawnRate(multiplicative: EnchantmentStrengthData)
			};

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Weapons, 1f },
				{ EItemType.Armor, 1f },
				{ EItemType.Accessories, 1f },
				{ EItemType.FishingPoles, 1f },
				{ EItemType.Tools, 1f }
			};
		}

		public override string ShortTooltip => $"{Math.Round(EnchantmentStrength * AllowedListMultiplier, 3)}x {EnchantmentTypeName.AddSpaces()}";
		public override string Artist => "Zorutan";
		public override string ArtModifiedBy => null;
		public override string Designer => "andro951";
	}
	public class WarEnchantmentBasic : WarEnchantment
	{
		public override SellCondition SellCondition => SellCondition.AnyTimeRare;
		public override List<WeightedPair> NpcDropTypes => new() {
			new(NPCID.PirateShip),
			new(NPCID.PirateCaptain)
		};
		public override SortedDictionary<ChestID, float> ChestDrops => new() {
			{ ChestID.Shadow, 0.8f },
			{ ChestID.Shadow_Locked, 0.8f }
		};
		public override List<WeightedPair> CrateDrops => new() {
			new(CrateID.Obsidian_LockBox, 0.4f)
		};
	}
	public class WarEnchantmentCommon : WarEnchantment { }
	public class WarEnchantmentRare : WarEnchantment { }
	public class WarEnchantmentEpic : WarEnchantment { }
	public class WarEnchantmentLegendary : WarEnchantment { }
}
