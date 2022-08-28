using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments {
    public abstract class LifeStealEnchantment : Enchantment {
        public override string CustomTooltip => $"(remainder is saved to prevent always rounding to 0 for low damage weapons)";
        public override float ScalePercent => 0.8f;
        public override bool Max1 => true;
        public override float CapacityCostMultiplier => 2f;
		public override int StrengthGroup => 5;
		public override void GetMyStats() {
            Effects = new() {
                new LifeSteal(@base: EnchantmentStrengthData)
            };

            AllowedList = new Dictionary<EItemType, float>() {
                { EItemType.Weapons, 1f }
            };
        }
		public override string Artist => "Zorutan";
        public override string Designer => "andro951";
    }
    public class LifeStealEnchantmentBasic : LifeStealEnchantment
    {
        public override List<WeightedPair> NpcDropTypes => new() {
            new(NPCID.WallofFlesh)
        };
        public override List<WeightedPair> NpcAIDrops => new() {
            new(NPCAIStyleID.Vulture),
            new(NPCAIStyleID.TheHungry),
            new(NPCAIStyleID.Creeper)
        };
        public override SortedDictionary<ChestID, float> ChestDrops => new() {
            { ChestID.Shadow, 0.1f },
            { ChestID.Shadow_Locked, 0.1f }
        };
        public override List<WeightedPair> CrateDrops => new() {
            new(CrateID.Obsidian_LockBox, 0.05f),
            new(CrateID.Crimson, 0.5f),
            new(CrateID.Hematic_CrimsonHard, 0.5f)
        };
    }
    public class LifeStealEnchantmentCommon : LifeStealEnchantment { }
    public class LifeStealEnchantmentRare : LifeStealEnchantment { }
    public class LifeStealEnchantmentSuperRare : LifeStealEnchantment { }
    public class LifeStealEnchantmentUltraRare : LifeStealEnchantment { }

}
