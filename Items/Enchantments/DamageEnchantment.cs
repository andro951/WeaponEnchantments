using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class DamageEnchantment : Enchantment
	{
		public override int LowestCraftableTier => 0;
		public override SellCondition SellCondition => SellCondition.Always;
		public override void GetMyStats() {
			Effects = new() {
				new DamageAfterDefenses(EnchantmentStrengthData)
			};
		}

		public override string ShortTooltip => GetShortTooltip(sign: true);
		public override string Artist => "Zorutan";
		public override string Designer => "andro951";
	}
	public class DamageEnchantmentBasic : DamageEnchantment
	{
		public override List<WeightedPair> NpcDropTypes => new() {
			new(NPCID.KingSlime)
		};
		public override List<WeightedPair> NpcAIDrops => new() {
			new(NPCAIStyleID.Slime)
		};
		public override SortedDictionary<ChestID, float> ChestDrops => new() {
			{ ChestID.Chest_Normal, 1f }
		};
		public override List<WeightedPair> CrateDrops => new() {
			new(CrateID.Wooden, 0.5f),
			new(CrateID.Pearlwood_WoodenHard, 0.5f)
		};
	}
	public class DamageEnchantmentCommon : DamageEnchantment { }
	public class DamageEnchantmentRare : DamageEnchantment { }
	public class DamageEnchantmentSuperRare : DamageEnchantment { }
	public class DamageEnchantmentUltraRare : DamageEnchantment { }

}
