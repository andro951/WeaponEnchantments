using androLib.Common.Utility;
using System.Collections.Generic;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Configs;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments.Utility {
    public abstract class FishingPowerEnchantment : Enchantment {
	    protected override string TypeName => "FishingPower";
	    protected override string NamePrefix => "Enchantments/";
	    
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
        
        public override bool IsLoadingEnabled(Mod mod)
        {
	        return ModContent.GetInstance<EnchantmentToggle>().FishingPower;
        }
    }
    [Autoload(false)]
	public class FishingPowerEnchantmentBasic : FishingPowerEnchantment
    {
        public override List<DropData> ChestDrops => new() {
            new(ChestID.Water)
        };
		public override List<DropData> CrateDrops => new() {
			new(CrateID.Azure_SkyHard, 0.5f),
			new(CrateID.Boreal_FrozenHard, 0.5f),
			new(CrateID.Bramble_JungleHard, 0.5f),
			new(CrateID.Corrupt, 0.5f),
			new(CrateID.Crimson, 0.5f),
			new(CrateID.Defiled_CorruptHard, 0.5f),
			new(CrateID.Divine_HallowedHard, 0.5f),
			new(CrateID.Dungeon, 0.5f),
			new(CrateID.Frozen, 0.5f),
			new(CrateID.Golden_LockBox, 0.25f),
			new(CrateID.Hallowed, 0.5f),
			new(CrateID.Hellstone_ObsidianHard, 0.5f),
			new(CrateID.Hematic_CrimsonHard, 0.5f),
			new(CrateID.Iron, 0.25f),
			new(CrateID.Jungle, 0.5f),
			new(CrateID.Mirage_OasisHard, 0.5f),
			new(CrateID.Mythril_IronHard, 0.25f),
			new(CrateID.Oasis, 0.5f),
			new(CrateID.Obsidian, 0.5f),
			new(CrateID.Obsidian_LockBox, 0.25f),
			new(CrateID.Ocean, 0.5f),
			new(CrateID.Pearlwood_WoodenHard, 0.5f),
			new(CrateID.Seaside_OceanHard, 0.5f),
			new(CrateID.Sky, 0.5f),
			new(CrateID.Stockade_DungeonHard, 0.5f),
			new(CrateID.Wooden, 0.5f),
			new(CrateID.Golden, chance: 0.1f),
			new(CrateID.Titanium_GoldenHard, chance: 0.1f),
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
	[Autoload(false)]
	public class FishingPowerEnchantmentCursed : FishingPowerEnchantment { }

}
