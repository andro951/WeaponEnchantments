using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using androLib.Items;
using androLib.Common.Utility;
using WeaponEnchantments.Common.Configs;

namespace WeaponEnchantments.Items.Enchantments.Utility {
    public abstract class LavaFishingEnchantment : Enchantment {
	    protected override string TypeName => "LavaFishing";
	    protected override string NamePrefix => "Enchantments/";
	    
		public override int StrengthGroup => 8;
		public override bool Max1 => true;
		public override float CapacityCostMultiplier => CapacityCostUtility;
		public override float ScalePercent => 0f;
		public override void GetMyStats() {
            Effects = new() {
                new LavaFishing(@base: EnchantmentStrengthData)
            };

            AllowedList = new Dictionary<EItemType, float>() {
                { EItemType.FishingPoles, 1f }
            };
        }

        public override string ShortTooltip => GetShortTooltip(sign: true);
        public override string Artist => "andro951";
        public override string ArtModifiedBy => null;
        public override string Designer => "Fran";
        
        public override bool IsLoadingEnabled(Mod mod)
        {
	        return ModContent.GetInstance<EnchantmentToggle>().LavaFishing;
        }
    }
    [Autoload(false)]
	public class LavaFishingEnchantmentBasic : LavaFishingEnchantment
    {
        public override SellCondition SellCondition => SellCondition.AnyTimeRare;
        public override List<DropData> NpcDropTypes => new() {
            new(NPCID.Hellbat, chance: 0.05f)
		};
		public override List<DropData> CrateDrops => new() {
			new(CrateID.Azure_SkyHard, 0.25f),
			new(CrateID.Boreal_FrozenHard, 0.25f),
			new(CrateID.Bramble_JungleHard, 0.25f),
			new(CrateID.Corrupt, 0.25f),
			new(CrateID.Crimson, 0.25f),
			new(CrateID.Defiled_CorruptHard, 0.25f),
			new(CrateID.Divine_HallowedHard, 0.25f),
			new(CrateID.Dungeon, 0.25f),
			new(CrateID.Frozen, 0.25f),
			new(CrateID.Golden_LockBox, 0.25f),
			new(CrateID.Hallowed, 0.25f),
			new(CrateID.Hellstone_ObsidianHard, 0.25f),
			new(CrateID.Hematic_CrimsonHard, 0.25f),
			new(CrateID.Iron, 0.25f),
			new(CrateID.Jungle, 0.25f),
			new(CrateID.Mirage_OasisHard, 0.25f),
			new(CrateID.Mythril_IronHard, 0.25f),
			new(CrateID.Oasis, 0.25f),
			new(CrateID.Obsidian, 0.25f),
			new(CrateID.Obsidian_LockBox, 0.25f),
			new(CrateID.Ocean, 0.25f),
			new(CrateID.Seaside_OceanHard, 0.25f),
			new(CrateID.Sky, 0.25f),
			new(CrateID.Stockade_DungeonHard, 0.25f),
		};
	}
    [Autoload(false)]
	public class LavaFishingEnchantmentCommon : LavaFishingEnchantment { }
    [Autoload(false)]
	public class LavaFishingEnchantmentRare : LavaFishingEnchantment { }
    [Autoload(false)]
	public class LavaFishingEnchantmentEpic : LavaFishingEnchantment { }
    [Autoload(false)]
	public class LavaFishingEnchantmentLegendary : LavaFishingEnchantment { }
	[Autoload(false)]
	public class LavaFishingEnchantmentCursed : LavaFishingEnchantment { }

}
