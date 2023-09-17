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
        public override string ArtModifiedBy => null;
        public override string Designer => "andro951";
    }
    [Autoload(false)]
	public class CrateChanceEnchantmentBasic : CrateChanceEnchantment
    {
        public override List<DropData> ChestDrops => new() {
            new(ChestID.Water)
        };
        public override List<DropData> CrateDrops => new() {
            new(CrateID.Iron),
            new(CrateID.Mythril_IronHard)
        };
    }
    [Autoload(false)]
	public class CrateChanceEnchantmentCommon : CrateChanceEnchantment { }
    [Autoload(false)]
	public class CrateChanceEnchantmentRare : CrateChanceEnchantment { }
    [Autoload(false)]
	public class CrateChanceEnchantmentEpic : CrateChanceEnchantment { }
    [Autoload(false)]
	public class CrateChanceEnchantmentLegendary : CrateChanceEnchantment { }

}
