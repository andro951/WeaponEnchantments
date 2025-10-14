using androLib.Common.Utility;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Configs;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments.Utility {
    public abstract class MagneticEnchantment : Enchantment {
	    protected override string TypeName => "Magnetic";
	    protected override string NamePrefix => "Enchantments/";
	    
		public override int StrengthGroup => 28;
		public override float ScalePercent => 2f/3f;
		public override void GetMyStats() {
            Effects = new() {
                new PickupRange(@base: EnchantmentStrengthData * 100f),
                new ItemAttractionAndPickupSpeed(additive: EnchantmentStrengthData)
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
        
        public override bool IsLoadingEnabled(Mod mod)
        {
	        return ModContent.GetInstance<EnchantmentToggle>().Magnetic;
        }
    }
    [Autoload(false)]
	public class MagneticEnchantmentBasic : MagneticEnchantment {
		public override List<DropData> NpcDropTypes => new() {
			new(NPCID.KingSlime),
			new(NPCID.GreekSkeleton, chance: 0.05f),
			new(NPCID.GraniteFlyer, chance: 0.05f),
			new(NPCID.GraniteGolem, chance: 0.05f),
			new(NPCID.Medusa, chance: 0.2f),
			new(NPCID.Harpy, chance: 0.01f)
		};
		public override List<DropData> ChestDrops => new() {
			new(ChestID.Chest_Normal),
            new(ChestID.Skyware, chance: 0.6f),
			new(ChestID.Granite, chance: 1f),
			new(ChestID.Marble, chance: 1f)
		};
        public override List<DropData> CrateDrops => new() {
            new(CrateID.Wooden, 0.25f),
            new(CrateID.Pearlwood_WoodenHard, 0.25f)
        };
	}
    [Autoload(false)]
	public class MagneticEnchantmentCommon : MagneticEnchantment { }
    [Autoload(false)]
	public class MagneticEnchantmentRare : MagneticEnchantment { }
    [Autoload(false)]
	public class MagneticEnchantmentEpic : MagneticEnchantment { }
    [Autoload(false)]
	public class MagneticEnchantmentLegendary : MagneticEnchantment { }
	[Autoload(false)]
	public class MagneticEnchantmentCursed : MagneticEnchantment { }

}
