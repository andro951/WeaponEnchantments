using System.Collections.Generic;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments.Unique {
    public abstract class NpcContactAnglerEnchantment : Enchantment {
		public override int StrengthGroup => 5;
		public override void GetMyStats() {
            Effects = new() {
                new QuestFishChance(@base: EnchantmentStrengthData)
            };

            AllowedList = new Dictionary<EItemType, float>() {
                { EItemType.FishingPoles, 1f }
            };
        }

        public override string Artist => "andro951";
        public override string Designer => "andro951";
    }
    public class NpcContactAnglerEnchantmentBasic : NpcContactAnglerEnchantment
    {
        public override List<WeightedPair> CrateDrops => new() {
            new(CrateID.Golden),
            new(CrateID.Titanium_GoldenHard)
        };
    }
    public class NpcContactAnglerEnchantmentCommon : NpcContactAnglerEnchantment { }
    public class NpcContactAnglerEnchantmentRare : NpcContactAnglerEnchantment { }
    public class NpcContactAnglerEnchantmentSuperRare : NpcContactAnglerEnchantment { }
    public class NpcContactAnglerEnchantmentUltraRare : NpcContactAnglerEnchantment { }

}
