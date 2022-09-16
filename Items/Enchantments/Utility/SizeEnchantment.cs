using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments.Utility
{
	public abstract class SizeEnchantment : Enchantment
	{
		public override int StrengthGroup => 10;
		public override void GetMyStats() {
			Effects = new() {
				new Size(EnchantmentStrengthData),
				new WhipRange(EnchantmentStrengthData),
				new YoyoStringLength(EnchantmentStrengthData)
			};
		}

		public override string ShortTooltip => GetShortTooltip(sign: true);
		public override string Artist => "Zorutan";
		public override string ArtModifiedBy => null;
		public override string Designer => "andro951";
	}
	public class SizeEnchantmentBasic : SizeEnchantment
	{
		public override List<WeightedPair> NpcAIDrops => new() {
			new(NPCAIStyleID.DemonEye)
		};
		public override SortedDictionary<ChestID, float> ChestDrops => new() {
			{ ChestID.Chest_Normal, 1f },
			{ ChestID.LivingWood, 1f }
		};
		public override List<WeightedPair> CrateDrops => new() {
			new(CrateID.Wooden, 0.5f),
			new(CrateID.Pearlwood_WoodenHard, 0.5f)
		};
	}
	public class SizeEnchantmentCommon : SizeEnchantment { }
	public class SizeEnchantmentRare : SizeEnchantment { }
	public class SizeEnchantmentEpic : SizeEnchantment { }
	public class SizeEnchantmentLegendary : SizeEnchantment { }

}
