using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments.Utility {
    public abstract class LavaFishingEnchantment : Enchantment {
		public override int StrengthGroup => 8;
		public override bool Max1 => true;
		public override float CapacityCostMultiplier => 1;
		public override float ScalePercent => 0f;
		public override void GetMyStats() {
            Effects = new() {
                new LavaFishing(@base: EnchantmentStrengthData)
            };

            AllowedList = new Dictionary<EItemType, float>() {
                { EItemType.FishingPoles, 1f }
            };
        }

        public override string ShortTooltip => GetShortTooltip(sign: true);
        public override string Artist => "andro951";
        public override string ArtModifiedBy => null;
        public override string Designer => "Fran";
    }
    public class LavaFishingEnchantmentBasic : LavaFishingEnchantment
    {
        public override SellCondition SellCondition => SellCondition.AnyTimeRare;
        public override List<WeightedPair> NpcDropTypes => new() {
            new(NPCID.HellButterfly),
            new(NPCID.Lavafly),
            new(NPCID.MagmaSnail),
            new(NPCID.Hellbat)
		};
		public override List<WeightedPair> CrateDrops => new() {
            new(CrateID.Iron),
            new(CrateID.Mythril_IronHard)
		};
	}
    public class LavaFishingEnchantmentCommon : LavaFishingEnchantment { }
    public class LavaFishingEnchantmentRare : LavaFishingEnchantment { }
    public class LavaFishingEnchantmentEpic : LavaFishingEnchantment { }
    public class LavaFishingEnchantmentLegendary : LavaFishingEnchantment { }

}
