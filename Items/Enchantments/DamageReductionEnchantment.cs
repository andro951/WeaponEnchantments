using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments {
    public abstract class DamageReductionEnchantment : Enchantment {
        public override float ScalePercent => 0.6f;
        public override float CapacityCostMultiplier => 3f;
		public override int StrengthGroup => 20;
        public override void GetMyStats() {
            Effects = new() {
                new DamageReduction(@base: EnchantmentStrengthData)
            };

            AllowedList = new Dictionary<EItemType, float>() {
                { EItemType.Weapons, 0.5f },
				{ EItemType.Tools, 0.5f },
				{ EItemType.FishingPoles, 0.5f },
				{ EItemType.Armor, 1f },
                { EItemType.Accessories, 1f }
            };
        }
		public override string Artist => "Zorutan";
        public override string ArtModifiedBy => null;
        public override string Designer => "andro951";
    }
    public class DamageReductionEnchantmentBasic : DamageReductionEnchantment
    {
        public override SellCondition SellCondition => SellCondition.PostSkeletron;
        public override List<WeightedPair> NpcDropTypes => new() {
            new(NPCID.GraniteFlyer),
            new(NPCID.GraniteGolem),
            new(NPCID.GreekSkeleton),
            new(NPCID.PossessedArmor),
            new(NPCID.GiantTortoise),
            new(NPCID.IceTortoise)
        };
        public override List<WeightedPair> CrateDrops => new() {
            new(CrateID.Dungeon, 0.5f),
            new(CrateID.Stockade_DungeonHard, 0.5f)
        };
    }
    public class DamageReductionEnchantmentCommon : DamageReductionEnchantment { }
    public class DamageReductionEnchantmentRare : DamageReductionEnchantment { }
    public class DamageReductionEnchantmentEpic : DamageReductionEnchantment { }
    public class DamageReductionEnchantmentLegendary : DamageReductionEnchantment { }

}
