using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using androLib.Items;
using androLib.Common.Utility;
using WeaponEnchantments.Common.Configs;

namespace WeaponEnchantments.Items.Enchantments.Utility {
    public abstract class LuckEnchantment : Enchantment {
	    protected override string TypeName => "Luck";
	    protected override string NamePrefix => "Enchantments/";
	    
		public override int StrengthGroup => 16;
		public override float ScalePercent => 0.6f;
		public override void GetMyStats() {
            Effects = new() {
                new Luck(@base: EnchantmentStrengthData)
            };

            AllowedList = new Dictionary<EItemType, float>() {
                { EItemType.Weapons, 1f },
                { EItemType.Armor, 1f },
                { EItemType.Accessories, 1f },
                { EItemType.FishingPoles, 1f },
                { EItemType.Tools, 1f }
            };
        }

        public override string ShortTooltip => GetShortTooltip(sign: true, percent: false, multiply100: false);
        public override string Artist => "andro951";
        public override string ArtModifiedBy => null;
        public override string Designer => "andro951";
        
        public override bool IsLoadingEnabled(Mod mod)
        {
	        return ModContent.GetInstance<EnchantmentToggle>().Luck;
        }
    }
    [Autoload(false)]
	public class LuckEnchantmentBasic : LuckEnchantment
    {
        public override SellCondition SellCondition => SellCondition.Luck;
        public override List<DropData> NpcDropTypes => new() {
            new(NPCID.Pinky, 1f),
            new(NPCID.Nymph, 1f),
            new(NPCID.DungeonSlime, 0.2f),
            new(NPCID.RainbowSlime, 1f),
            new(NPCID.GoblinScout, 0.2f),
            new(NPCID.BoneLee, 0.1f),
            new(NPCID.Paladin, 0.1f),
            new(NPCID.Moth, 0.1f),
            new(NPCID.RuneWizard, 0.1f),
            new(NPCID.TheGroom, 0.2f),
            new(NPCID.DoctorBones, 0.2f),
            new(NPCID.TheBride, 0.2f),
            new(NPCID.SkeletonSniper, 0.1f),
            new(NPCID.TacticalSkeleton, 0.1f),
            new(NPCID.SkeletonCommando, 0.1f),
            new(NPCID.Tim, 1f),
            new(NPCID.UndeadMiner, 0.05f),
            new(NPCID.Clown, 0.1f),
            new(NPCID.CochinealBeetle, 0.1f),
            new(NPCID.CyanBeetle, 0.1f),
            new(NPCID.LacBeetle, 0.1f),
            new(NPCID.Squid, 0.1f),
            new(NPCID.BoneSerpentHead, 0.1f),
            new(NPCID.DevourerHead, 0.1f),
            new(NPCID.FireImp, 0.1f),
            new(NPCID.Ghost, 0.1f),
            new(NPCID.GiantWormHead, 0.1f),
            new(NPCID.GreekSkeleton, 0.1f),
            new(NPCID.Salamander, 0.1f),
            new(NPCID.Salamander2, 0.1f),
            new(NPCID.Salamander3, 0.1f),
            new(NPCID.Salamander4, 0.1f),
            new(NPCID.Salamander5, 0.1f),
            new(NPCID.Salamander6, 0.1f),
            new(NPCID.Salamander7, 0.1f),
            new(NPCID.Salamander8, 0.1f),
            new(NPCID.Salamander9, 0.1f),
            new(NPCID.VoodooDemon, 0.1f),
            new(NPCID.ChaosElemental, 0.1f),
            new(NPCID.CursedHammer, 0.1f),
            new(NPCID.EnchantedSword, 0.1f),
            new(NPCID.CrimsonAxe, 0.1f),
            new(NPCID.PigronCorruption, 0.1f),
            new(NPCID.PigronCrimson, 0.1f),
            new(NPCID.PigronHallow, 0.1f),
            new(NPCID.DesertLamiaDark, 0.1f),
            new(NPCID.DesertLamiaLight, 0.1f),
            new(NPCID.Medusa, 0.1f),
            new(NPCID.RockGolem, 0.1f),
            new(NPCID.Dandelion, 0.1f),
            new(NPCID.IceGolem, 0.2f),
            new(NPCID.SandElemental, 0.2f),
            new(NPCID.Eyezor, 0.05f),
            new(NPCID.Nailhead, 0.05f),
            new(NPCID.DrManFly, 0.05f),
            new(NPCID.Psycho, 0.05f),
            new(NPCID.Mothron, 0.01f),
            new(NPCID.GoldenSlime, 1f)
        };
		public override List<DropData> NpcAIDrops => new() {
            new(NPCAIStyleID.Mimic, chance: 0.02f),
            new(NPCAIStyleID.BiomeMimic, chance: 0.05f)
		};
		public override List<DropData> ChestDrops => new() {
            new(ChestID.Chest_Normal, chance: 0.01f),
            new(ChestID.Gold, chance: 0.01f),
            new(ChestID.Gold_Locked, chance: 0.01f),
            new(ChestID.Shadow, chance: 0.01f),
            new(ChestID.Shadow_Locked, chance: 0.01f),
            new(ChestID.RichMahogany, chance: 0.01f),
            new(ChestID.Ivy, chance: 0.01f),
            new(ChestID.Frozen, chance: 0.01f),
            new(ChestID.LivingWood, chance: 0.01f),
            new(ChestID.Skyware, chance: 0.01f),
            new(ChestID.WebCovered, chance: 0.01f),
            new(ChestID.Lihzahrd, chance: 0.01f),
            new(ChestID.Water, chance: 0.01f),
            new(ChestID.Mushroom, chance: 0.01f),
            new(ChestID.Granite, chance: 0.01f),
            new(ChestID.Marble, chance: 0.01f),
            new(ChestID.Gold_DeadMans, chance: 0.1f),
            new(ChestID.SandStone, chance: 0.01f)
        };
		public override List<DropData> CrateDrops => new() {
            new(CrateID.Wooden, chance: 0.001f),
            new(CrateID.Iron, chance: 0.002f),
            new(CrateID.Golden, chance: 0.005f),
            new(CrateID.Jungle, chance: 0.01f),
            new(CrateID.Sky, chance: 0.01f),
            new(CrateID.Corrupt, chance: 0.01f),
            new(CrateID.Crimson, chance: 0.01f),
            new(CrateID.Hallowed, chance: 0.01f),
            new(CrateID.Dungeon, chance: 0.01f),
            new(CrateID.Frozen, chance: 0.01f),
            new(CrateID.Oasis, chance: 0.01f),
            new(CrateID.Ocean, chance: 0.01f),
            new(CrateID.Pearlwood_WoodenHard, chance: 0.002f),
            new(CrateID.Mythril_IronHard, chance: 0.004f),
            new(CrateID.Titanium_GoldenHard, chance: 0.01f),
            new(CrateID.Bramble_JungleHard, chance: 0.01f),
            new(CrateID.Azure_SkyHard, chance: 0.01f),
            new(CrateID.Defiled_CorruptHard, chance: 0.01f),
            new(CrateID.Hematic_CrimsonHard, chance: 0.01f),
            new(CrateID.Divine_HallowedHard, chance: 0.01f),
            new(CrateID.Stockade_DungeonHard, chance: 0.01f),
            new(CrateID.Boreal_FrozenHard, chance: 0.01f),
            new(CrateID.Mirage_OasisHard, chance: 0.01f),
            new(CrateID.Seaside_OceanHard, chance: 0.01f),
            new(CrateID.Golden_LockBox, chance: 0.02f),
            new(CrateID.Obsidian_LockBox, chance: 0.02f)
        };
	}
    [Autoload(false)]
	public class LuckEnchantmentCommon : LuckEnchantment { }
    [Autoload(false)]
	public class LuckEnchantmentRare : LuckEnchantment { }
    [Autoload(false)]
	public class LuckEnchantmentEpic : LuckEnchantment { }
    [Autoload(false)]
	public class LuckEnchantmentLegendary : LuckEnchantment { }
	[Autoload(false)]
	public class LuckEnchantmentCursed : LuckEnchantment { }

}
