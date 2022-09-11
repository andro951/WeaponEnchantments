using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments.Utility {
    public abstract class LuckEnchantment : Enchantment {
		public override int StrengthGroup => 16;
        public override void GetMyStats() {
            Effects = new() {
                new Luck(@base: EnchantmentStrengthData),
                new MaxLuck(@base: EnchantmentStrengthData)
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
    }
    public class LuckEnchantmentBasic : LuckEnchantment
    {
        public override SellCondition SellCondition => SellCondition.Luck;
        public override List<WeightedPair> NpcDropTypes => new() {
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
		public override List<WeightedPair> NpcAIDrops => new() {
            new(NPCAIStyleID.Mimic, 0.05f),
            new(NPCAIStyleID.BiomeMimic, 0.1f)
		};
		public override SortedDictionary<ChestID, float> ChestDrops => new() {
            { ChestID.Chest_Normal, 0.05f },
            { ChestID.Gold, 0.05f },
            { ChestID.Gold_Locked, 0.05f },
            { ChestID.Shadow, 0.05f },
            { ChestID.Shadow_Locked, 0.05f },
            { ChestID.RichMahogany, 0.05f },
            { ChestID.Ivy, 0.05f },
            { ChestID.Frozen, 0.05f },
            { ChestID.LivingWood, 0.05f },
            { ChestID.Skyware, 0.05f },
            { ChestID.WebCovered, 0.05f },
            { ChestID.Lihzahrd, 0.05f },
            { ChestID.Water, 0.05f },
            { ChestID.Mushroom, 0.05f },
            { ChestID.Granite, 0.05f },
            { ChestID.Marble, 0.05f },
            { ChestID.Gold_DeadMans, 0.5f },
            { ChestID.SandStone, 0.05f }
        };
		public override List<WeightedPair> CrateDrops => new() {
            new(CrateID.Wooden, 0.05f),
            new(CrateID.Iron, 0.05f),
            new(CrateID.Golden, 0.05f),
            new(CrateID.Jungle, 0.05f),
            new(CrateID.Sky, 0.05f),
            new(CrateID.Corrupt, 0.05f),
            new(CrateID.Crimson, 0.05f),
            new(CrateID.Hallowed, 0.05f),
            new(CrateID.Dungeon, 0.05f),
            new(CrateID.Frozen, 0.05f),
            new(CrateID.Oasis, 0.05f),
            new(CrateID.Ocean, 0.05f),
            new(CrateID.Pearlwood_WoodenHard, 0.05f),
            new(CrateID.Mythril_IronHard, 0.05f),
            new(CrateID.Titanium_GoldenHard, 0.05f),
            new(CrateID.Bramble_JungleHard, 0.05f),
            new(CrateID.Azure_SkyHard, 0.05f),
            new(CrateID.Defiled_CorruptHard, 0.05f),
            new(CrateID.Hematic_CrimsonHard, 0.05f),
            new(CrateID.Divine_HallowedHard, 0.05f),
            new(CrateID.Stockade_DungeonHard, 0.05f),
            new(CrateID.Boreal_FrozenHard, 0.05f),
            new(CrateID.Mirage_OasisHard, 0.05f),
            new(CrateID.Seaside_OceanHard, 0.05f),
            new(CrateID.Golden_LockBox, 0.05f),
            new(CrateID.Obsidian_LockBox, 0.05f)
        };
	}
    public class LuckEnchantmentCommon : LuckEnchantment { }
    public class LuckEnchantmentRare : LuckEnchantment { }
    public class LuckEnchantmentEpic : LuckEnchantment { }
    public class LuckEnchantmentLegendary : LuckEnchantment { }

}
