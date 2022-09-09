using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using static WeaponEnchantments.Common.Configs.ConfigValues;

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class AttackSpeedEnchantment : Enchantment
	{
		public override SellCondition SellCondition => SellCondition.PostEaterOfWorldsOrBrainOfCthulhu;
		public override void GetMyStats() {
			Effects = new() {
				new AttackSpeed(EnchantmentStrengthData),
				new AutoReuse(AttackSpeedEnchantmentAutoReuseSetpoint, EnchantmentStrengthData)
			};

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Weapons, 1f },
				{ EItemType.Armor, 0.25f },
				{ EItemType.Accessories, 0.25f },
				{ EItemType.FishingPoles, 1f },
				{ EItemType.Tools, 1f }
			};
		}

		public override string ShortTooltip => GetShortTooltip(sign: true);
		public override string Artist => "Zorutan";
		public override string Designer => "andro951";
	}
	public class AttackSpeedEnchantmentBasic : AttackSpeedEnchantment
	{
		public override List<WeightedPair> NpcDropTypes => new() {
			new(NPCID.EaterofWorldsHead)
		};
		public override List<WeightedPair> NpcAIDrops => new() {
			new(NPCAIStyleID.Bat),
			new(NPCAIStyleID.Piranha)
		};
		public override SortedDictionary<ChestID, float> ChestDrops => new() {
			{ ChestID.Chest_Normal, 1f },
			{ ChestID.Gold, 1f },
			{ ChestID.Gold_DeadMans, 1f },
			{ ChestID.Ivy, 1f },
			{ ChestID.Granite, 1f }
		};
		public override List<WeightedPair> CrateDrops => new() {
			new(CrateID.Wooden, 0.5f),
			new(CrateID.Pearlwood_WoodenHard, 0.5f),
			new(CrateID.Iron, 0.5f),
			new(CrateID.Iron, 0.5f),
			new(CrateID.Jungle, 0.5f),
			new(CrateID.Bramble_JungleHard, 0.5f)
		};
	}
	public class AttackSpeedEnchantmentCommon : AttackSpeedEnchantment { }
	public class AttackSpeedEnchantmentRare : AttackSpeedEnchantment { }
	public class AttackSpeedEnchantmentEpic : AttackSpeedEnchantment { }
	public class AttackSpeedEnchantmentLegendary : AttackSpeedEnchantment { }

}
