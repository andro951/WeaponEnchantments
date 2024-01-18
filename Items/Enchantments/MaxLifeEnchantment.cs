using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class MaxLifeEnchantment : Enchantment
	{
		public override int StrengthGroup => 3;
		public override void GetMyStats() {
			Effects = new() {
                new MaxLife(@base: EnchantmentStrengthData),
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
		public override string Artist => "Auseawesome";
		public override string ArtModifiedBy => null;
		public override string Designer => "Auseawesome";
	}
	public class MaxLifeEnchantmentBasic : MaxLifeEnchantment
    {
		public override SellCondition SellCondition => SellCondition.AnyTime;
		public override List<WeightedPair> NpcDropTypes => new() {
			new(NPCID.CaveBat, 0.1f),
			new(NPCID.GiantBat, 0.1f),
            new(NPCID.IceBat, 0.1f),
            new(NPCID.IlluminantBat, 0.1f),
            new(NPCID.JungleBat, 0.1f),
            new(NPCID.SporeBat, 0.1f),
            new(NPCID.VampireBat, 0.1f),
            new(NPCID.Hellbat, 0.1f),
            new(NPCID.Lavabat, 0.1f)
        };
		public override SortedDictionary<ChestID, float> ChestDrops => new() {
			{ ChestID.Chest_Normal, 0.5f }
		};
		public override List<WeightedPair> CrateDrops => new() {
			new(CrateID.Wooden, 0.5f),
            new(CrateID.Iron, 0.5f)
        };
	}
	public class MaxLifeEnchantmentCommon : MaxLifeEnchantment { }
	public class MaxLifeEnchantmentRare : MaxLifeEnchantment { }
	public class MaxLifeEnchantmentEpic : MaxLifeEnchantment { }
	public class MaxLifeEnchantmentLegendary : MaxLifeEnchantment { }

}