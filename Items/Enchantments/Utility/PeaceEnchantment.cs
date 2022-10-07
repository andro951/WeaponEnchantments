using System;
using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments.Utility
{
	public abstract class PeaceEnchantment : Enchantment {
		public override int StrengthGroup => 2;
		public override float ScalePercent => -1f;
		public override void GetMyStats() {
			Effects = new() {
				new EnemyMaxSpawns(multiplicative: EnchantmentStrengthData.Invert()),
				new EnemySpawnRate(multiplicative: EnchantmentStrengthData.Invert())
			};

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Weapons, 1f },
				{ EItemType.Armor, 1f },
				{ EItemType.Accessories, 1f },
				{ EItemType.FishingPoles, 1f },
				{ EItemType.Tools, 1f }
			};
		}

		public override string ShortTooltip => $"{Math.Round(EnchantmentStrength * AllowedListMultiplier, 3)}x {GetLocalizationTypeName()}";
		public override string Artist => "Zorutan";
		public override string ArtModifiedBy => null;
		public override string Designer => "andro951";
	}
	public class PeaceEnchantmentBasic : PeaceEnchantment
	{
		public override List<WeightedPair> NpcDropTypes => new() {
			new(NPCID.Pixie)
		};
		public override SortedDictionary<ChestID, float> ChestDrops => new() {
			{ ChestID.Chest_Normal, 1f }
		};
		public override List<WeightedPair> CrateDrops => new() {
			new(CrateID.Wooden, 0.5f),
			new(CrateID.Pearlwood_WoodenHard, 0.5f)
		};
	}
	public class PeaceEnchantmentCommon : PeaceEnchantment { }
	public class PeaceEnchantmentRare : PeaceEnchantment { }
	public class PeaceEnchantmentEpic : PeaceEnchantment { }
	public class PeaceEnchantmentLegendary : PeaceEnchantment { }

}
