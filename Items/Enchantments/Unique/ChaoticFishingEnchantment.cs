using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using androLib.Items;
using androLib.Common.Utility;

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
        public override string ArtModifiedBy => null;
        public override string Designer => "andro951";
    }
    public class ChaoticFishingEnchantmentBasic : ChaoticFishingEnchantment
    {
        public override SellCondition SellCondition => SellCondition.HardMode;
        public override List<DropData> NpcDropTypes => new() {
            new(NPCID.GoblinShark),
            new(NPCID.BloodEelHead),
            new(NPCID.BloodNautilus),
            new(NPCID.Shark)
        };

        public override List<DropData> CrateDrops => new() {
            new(CrateID.Golden),
            new(CrateID.Titanium_GoldenHard)
		};
	}
    public class ChaoticFishingEnchantmentCommon : ChaoticFishingEnchantment { }
    public class ChaoticFishingEnchantmentRare : ChaoticFishingEnchantment { }
    public class ChaoticFishingEnchantmentEpic : ChaoticFishingEnchantment { }
    public class ChaoticFishingEnchantmentLegendary : ChaoticFishingEnchantment { }

}
