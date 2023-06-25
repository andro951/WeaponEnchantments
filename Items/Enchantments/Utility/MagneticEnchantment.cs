using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments.Utility {
    public abstract class MagneticEnchantment : Enchantment {
		public override int StrengthGroup => 22;
		public override float ScalePercent => 2f/3f;
		public override void GetMyStats() {
            Effects = new() {
                new PickupRange(additive: EnchantmentStrengthData)
            };

            AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Weapons, 1f },
				{ EItemType.Armor, 1f },
                { EItemType.Accessories, 1f },
                { EItemType.FishingPoles, 1f },
				{ EItemType.Tools, 1f }
			};
        }

        public override string ShortTooltip => GetShortTooltip(sign: true);
		public override string Artist => "Sir Bumpleton ✿";
		public override string ArtModifiedBy => null;
        public override string Designer => "andro951";
    }
    public class MagneticEnchantmentBasic : MagneticEnchantment
    {
        public override List<DropData> ChestDrops => new() {
            new(ChestID.Skyware, chance: 0.3f),
			new(ChestID.Granite, chance: 0.5f),
			new(ChestID.Marble, chance: 0.5f)
		};
        public override List<DropData> CrateDrops => new() {
            new(CrateID.Iron, chance: 0.05f),
            new(CrateID.Mythril_IronHard, chance: 0.05f)
        };
		public override List<DropData> NpcDropTypes => new() {
			new(NPCID.GreekSkeleton, chance: 0.05f),
            new(NPCID.GraniteFlyer, chance: 0.05f),
            new(NPCID.GraniteGolem, chance: 0.05f),
            new(NPCID.Medusa, chance: 0.05f),
            new(NPCID.Harpy, chance: 0.05f)
		};
	}
    public class MagneticEnchantmentCommon : MagneticEnchantment { }
    public class MagneticEnchantmentRare : MagneticEnchantment { }
    public class MagneticEnchantmentEpic : MagneticEnchantment { }
    public class MagneticEnchantmentLegendary : MagneticEnchantment { }

}
