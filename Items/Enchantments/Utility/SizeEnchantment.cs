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
		public override List<DropData> NpcAIDrops => new() {
			new(NPCAIStyleID.DemonEye)
		};
		public override List<DropData> ChestDrops => new() {
			new(ChestID.Chest_Normal),
			new(ChestID.LivingWood)
		};
		public override List<DropData> CrateDrops => new() {
			new(CrateID.Wooden, 0.5f),
			new(CrateID.Pearlwood_WoodenHard, 0.5f)
		};
	}
	public class SizeEnchantmentCommon : SizeEnchantment { }
	public class SizeEnchantmentRare : SizeEnchantment { }
	public class SizeEnchantmentEpic : SizeEnchantment { }
	public class SizeEnchantmentLegendary : SizeEnchantment { }

}
