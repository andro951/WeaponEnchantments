using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using androLib.Items;
using androLib.Common.Utility;

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class DefenseEnchantment : Enchantment
	{
		public override int StrengthGroup => 3;
		public override int LowestCraftableTier => 0;
		public override void GetMyStats() {
			Effects = new() {
				new Defense(@base: EnchantmentStrengthData),
			};

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Weapons, 0.5f },
				{ EItemType.Tools, 0.5f },
				{ EItemType.FishingPoles, 0.5f },
				{ EItemType.Armor, 1f },
				{ EItemType.Accessories, 1f }
			};
		}

		public override string ShortTooltip => GetShortTooltip(sign: true, percent: false, multiply100: false);
		public override string Artist => "Zorutan";
		public override string ArtModifiedBy => null;
		public override string Designer => "andro951";
	}
	public class DefenseEnchantmentBasic : DefenseEnchantment
	{
		public override SellCondition SellCondition => SellCondition.Always;
		public override List<DropData> NpcAIDrops => new() {
			new(NPCAIStyleID.Fighter)
		};
		public override List<DropData> ChestDrops => new() {
			new(ChestID.Chest_Normal)
		};
		public override List<DropData> CrateDrops => new() {
			new(CrateID.Wooden, 0.5f),
			new(CrateID.Pearlwood_WoodenHard, 0.5f)
		};
	}
	public class DefenseEnchantmentCommon : DefenseEnchantment { }
	public class DefenseEnchantmentRare : DefenseEnchantment { }
	public class DefenseEnchantmentEpic : DefenseEnchantment { }
	public class DefenseEnchantmentLegendary : DefenseEnchantment { }

}