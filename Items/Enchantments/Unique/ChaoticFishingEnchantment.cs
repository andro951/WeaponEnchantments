using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments.Unique {
    public abstract class ChaoticFishingEnchantment : Enchantment {
		public override int StrengthGroup => 7;
		public override void GetMyStats() {
            Effects = new() {
                new FishingEnemySpawnChance(@base: EnchantmentStrengthData)
            };

            AllowedList = new Dictionary<EItemType, float>() {
                { EItemType.FishingPoles, 1f }
            };
        }

        public override string Artist => "andro951";
        public override string Designer => "andro951";
    }
    public class ChaoticFishingEnchantmentBasic : ChaoticFishingEnchantment {
		public override List<WeightedPair> NpcAIDrops => new List<WeightedPair>() {
            new (NPCID.GoblinShark, 1f),
            new(NPCID.BloodEelHead, 1f),
            new(NPCID.BloodNautilus, 1f),
            new(NPCID.Wraith, 1f)
        };
	}
    public class ChaoticFishingEnchantmentCommon : ChaoticFishingEnchantment { }
    public class ChaoticFishingEnchantmentRare : ChaoticFishingEnchantment { }
    public class ChaoticFishingEnchantmentSuperRare : ChaoticFishingEnchantment { }
    public class ChaoticFishingEnchantmentUltraRare : ChaoticFishingEnchantment { }

}
