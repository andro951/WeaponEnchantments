using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments.Utility
{
	public abstract class ReducedManaUsageEnchantment : Enchantment
	{
		public override void GetMyStats() {
			Effects = new() {
				new ManaUsage(@base: EnchantmentStrengthData * -1f)
			};
		}

		public override string Artist => "Zorutan";
		public override string Designer => "andro951";
	}
	public class ReducedManaUsageEnchantmentBasic : ReducedManaUsageEnchantment
	{
		public override List<WeightedPair> NpcDropTypes => new() {
			new(NPCID.BrainofCthulhu)
		};
		public override List<WeightedPair> NpcAIDrops => new() {
			new(NPCAIStyleID.Worm),
			new(NPCAIStyleID.Caster),
			new(NPCAIStyleID.CursedSkull)
		};
		public override SortedDictionary<ChestID, float> ChestDrops => new() {
			{ ChestID.Chest_Normal, 1f },
			{ ChestID.Frozen, 1f }
		};
		public override List<WeightedPair> CrateDrops => new() {
			new(CrateID.Wooden, 0.5f),
			new(CrateID.Pearlwood_WoodenHard, 0.5f),
			new(CrateID.Frozen, 0.5f),
			new(CrateID.Boreal_FrozenHard, 0.5f)
		};
	}
	public class ReducedManaUsageEnchantmentCommon : ReducedManaUsageEnchantment { }
	public class ReducedManaUsageEnchantmentRare : ReducedManaUsageEnchantment { }
	public class ReducedManaUsageEnchantmentEpic : ReducedManaUsageEnchantment { }
	public class ReducedManaUsageEnchantmentLegendary : ReducedManaUsageEnchantment { }

}
