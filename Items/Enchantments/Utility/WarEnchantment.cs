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
		public override string CustomTooltip =>
			"(Minion Damage is reduced by your spawn rate multiplier, from enchantments, unless they are your minion attack target)\n" +
			"(minion attack target set from hitting enemies with whips or a weapon that is converted to summon damage from an enchantment)\n" +
			"(Prevents consuming boss summoning items if spawn rate multiplier, from enchantments, is > 1.6)\n" +
			"(Enemies spawned will be immune to lava/traps)";
		public override int StrengthGroup => 2;
		public override float ScalePercent => -1f;
		public override SellCondition SellCondition => SellCondition.AnyTimeRare;
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
		public override string Designer => "andro951";
	}
	public class WarEnchantmentBasic : WarEnchantment
	{
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
	public class WarEnchantmentSuperRare : WarEnchantment { }
	public class WarEnchantmentUltraRare : WarEnchantment { }
}
