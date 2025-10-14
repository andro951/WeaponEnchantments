using androLib.Common.Utility;
using System.Collections.Generic;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Configs;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments.Utility {
    public abstract class CrateChanceEnchantment : Enchantment {
	    protected override string TypeName => "CrateChance";
	    protected override string NamePrefix => "Enchantments/";
	    
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
        
        public override bool IsLoadingEnabled(Mod mod)
        {
	        return ModContent.GetInstance<EnchantmentToggle>().CrateChance;
        }
    }
    [Autoload(false)]
	public class CrateChanceEnchantmentBasic : CrateChanceEnchantment
    {
        public override List<DropData> ChestDrops => new() {
            new(ChestID.Water)
        };
        public override List<DropData> CrateDrops => new() {
            new(CrateID.Azure_SkyHard),
			new(CrateID.Boreal_FrozenHard),
			new(CrateID.Bramble_JungleHard),
			new(CrateID.Corrupt),
			new(CrateID.Crimson),
			new(CrateID.Defiled_CorruptHard),
			new(CrateID.Divine_HallowedHard),
			new(CrateID.Dungeon),
			new(CrateID.Frozen),
			new(CrateID.Golden_LockBox, 0.5f),
			new(CrateID.Hallowed),
			new(CrateID.Hellstone_ObsidianHard),
			new(CrateID.Hematic_CrimsonHard),
			new(CrateID.Iron),
			new(CrateID.Jungle),
			new(CrateID.Mirage_OasisHard),
			new(CrateID.Mythril_IronHard),
			new(CrateID.Oasis),
			new(CrateID.Obsidian),
			new(CrateID.Obsidian_LockBox, 0.5f),
			new(CrateID.Ocean),
			new(CrateID.Seaside_OceanHard),
			new(CrateID.Sky),
			new(CrateID.Stockade_DungeonHard),
			new(CrateID.Golden, chance: 0.05f),
			new(CrateID.Titanium_GoldenHard, chance: 0.05f),
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
	[Autoload(false)]
	public class CrateChanceEnchantmentCursed : CrateChanceEnchantment { }

}
