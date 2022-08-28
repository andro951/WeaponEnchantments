using System.Collections.Generic;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments.Utility {
    public abstract class CrateChanceEnchantment : Enchantment {
		public override int StrengthGroup => 10;
		public override void GetMyStats() {
            Effects = new() {
                new CrateChance(@base: EnchantmentStrengthData)
            };

            AllowedList = new Dictionary<EItemType, float>() {
                { EItemType.Armor, 1f },
                { EItemType.Accessories, 1f },
                { EItemType.FishingPoles, 1f }
            };
        }

        public override string ShortTooltip => GetShortTooltip(sign: true);
        public override string Artist => "andro951";
        public override string Designer => "andro951";
    }
    public class CrateChanceEnchantmentBasic : CrateChanceEnchantment
    {
        public override SortedDictionary<ChestID, float> ChestDrops => new() {
            { ChestID.Water, 1f }
        };
        public override List<WeightedPair> CrateDrops => new() {
            new(CrateID.Iron),
            new(CrateID.Mythril_IronHard)
        };
    }
    public class CrateChanceEnchantmentCommon : CrateChanceEnchantment { }
    public class CrateChanceEnchantmentRare : CrateChanceEnchantment { }
    public class CrateChanceEnchantmentSuperRare : CrateChanceEnchantment { }
    public class CrateChanceEnchantmentUltraRare : CrateChanceEnchantment { }

}
