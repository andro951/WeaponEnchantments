using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class CriticalStrikeChanceEnchantment : Enchantment
	{
		public override void GetMyStats() {
			Effects = new() {
				new CriticalStrikeChance(@base: EnchantmentStrengthData),
			};
		}

		public override string ShortTooltip => GetShortTooltip(sign: true);
		public override string Artist => "Zorutan";
		public override string Designer => "andro951";
	}
	public class CriticalStrikeChanceEnchantmentBasic : CriticalStrikeChanceEnchantment
	{
		public override List<WeightedPair> NpcDropTypes => new() {
			new(NPCID.SkeletronHead)
		};
		public override List<WeightedPair> NpcAIDrops => new() {
			new(NPCAIStyleID.ManEater),
			new(NPCAIStyleID.Jellyfish),
			new(NPCAIStyleID.Antlion)
		};
		public override SortedDictionary<ChestID, float> ChestDrops => new() {
			{ ChestID.Chest_Normal, 1f },
			{ ChestID.Gold, 1f },
			{ ChestID.Gold_DeadMans, 1f },
			{ ChestID.RichMahogany, 1f }
		};
		public override List<WeightedPair> CrateDrops => new() {
			new(CrateID.Wooden, 0.5f),
			new(CrateID.Pearlwood_WoodenHard, 0.5f),
			new(CrateID.Iron, 0.5f),
			new(CrateID.Iron, 0.5f),
			new(CrateID.Jungle, 0.5f),
			new(CrateID.Jungle, 0.5f)
		};
	}
	public class CriticalStrikeChanceEnchantmentCommon : CriticalStrikeChanceEnchantment { }
	public class CriticalStrikeChanceEnchantmentRare : CriticalStrikeChanceEnchantment { }
	public class CriticalStrikeChanceEnchantmentEpic : CriticalStrikeChanceEnchantment { }
	public class CriticalStrikeChanceEnchantmentLegendary : CriticalStrikeChanceEnchantment { }
}