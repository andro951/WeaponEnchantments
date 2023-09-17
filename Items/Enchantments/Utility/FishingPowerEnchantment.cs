using System.Collections.Generic;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments.Utility {
    public abstract class FishingPowerEnchantment : Enchantment {
		public override int StrengthGroup => 4;
		public override void GetMyStats() {
            Effects = new() {
                new FishingPower(@base: EnchantmentStrengthData)
            };

            AllowedList = new Dictionary<EItemType, float>() {
                { EItemType.Armor, 1f },
                { EItemType.Accessories, 1f },
                { EItemType.FishingPoles, 1f }
            };
        }

        public override string ShortTooltip => GetShortTooltip(sign: true, percent: false, multiply100: false);
        public override string Artist => "andro951";
        public override string ArtModifiedBy => null;
        public override string Designer => "andro951";
    }
    [Autoload(false)]
	public class FishingPowerEnchantmentBasic : FishingPowerEnchantment
    {
        public override List<DropData> ChestDrops => new() {
            new(ChestID.Water)
        };
        public override List<DropData> CrateDrops => new() {
            new(CrateID.Wooden),
            new(CrateID.Pearlwood_WoodenHard)
        };
    }
    [Autoload(false)]
	public class FishingPowerEnchantmentCommon : FishingPowerEnchantment { }
    [Autoload(false)]
	public class FishingPowerEnchantmentRare : FishingPowerEnchantment { }
    [Autoload(false)]
	public class FishingPowerEnchantmentEpic : FishingPowerEnchantment { }
    [Autoload(false)]
	public class FishingPowerEnchantmentLegendary : FishingPowerEnchantment { }

}
