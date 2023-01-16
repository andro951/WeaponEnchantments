using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class LifeRegenEnchantment : Enchantment
	{
		public override int StrengthGroup => 22;
		public override int LowestCraftableTier => 0;
		public override void GetMyStats() {
			Effects = new() {
                new LifeRegeneration(@base: EnchantmentStrengthData * 0.5f),
			};

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Armor, 1f },
				{ EItemType.Accessories, 1f }
			};
		}

		public override string ShortTooltip => GetShortTooltip(sign: true, percent: false, multiply100: false);
		public override string Artist => "Auseawesome";
		public override string ArtModifiedBy => null;
		public override string Designer => "Auseawesome";
	}
	public class LifeRegenEnchantmentBasic : LifeRegenEnchantment
    {
		public override SellCondition SellCondition => SellCondition.Always;
		public override List<WeightedPair> NpcDropTypes => new() {
			new(NPCID.Zombie)
		};
		public override SortedDictionary<ChestID, float> ChestDrops => new() {
			{ ChestID.Chest_Normal, 0.5f }
		};
		public override List<WeightedPair> CrateDrops => new() {
			new(CrateID.Wooden, 0.5f),
            new(CrateID.Iron, 0.5f)
        };
	}
	public class LifeRegenEnchantmentCommon : LifeRegenEnchantment { }
	public class LifeRegenEnchantmentRare : LifeRegenEnchantment { }
	public class LifeRegenEnchantmentEpic : LifeRegenEnchantment { }
	public class LifeRegenEnchantmentLegendary : LifeRegenEnchantment { }

}