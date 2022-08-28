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
        public override string Designer => "andro951";
    }
    public class LuckEnchantmentBasic : LuckEnchantment {
		public override List<WeightedPair> NpcDropTypes => new() {
            new(NPCID.Pinky, 0.01f),
            new(NPCID.Nymph, 0.01f),
            new(NPCID.DungeonSlime, 0.01f),
            new(NPCID.RainbowSlime, 0.08f),
            new(NPCID.GoblinScout, 0.01f),
            new(NPCID.BoneLee, 0.02f),
            new(NPCID.Paladin, 0.02f),
            new(NPCID.Moth, 0.01f),
            new(NPCID.RuneWizard, 0.01f),
            new(NPCID.TheGroom, 0.01f),
            new(NPCID.DoctorBones, 0.01f),
            new(NPCID.TheBride, 0.01f),
            new(NPCID.SkeletonSniper, 0.006f),
            new(NPCID.TacticalSkeleton, 0.006f),
            new(NPCID.SkeletonCommando, 0.006f),
            new(NPCID.Tim, 0.2f),
            new(NPCID.UndeadMiner, 0.01f),
            new(NPCID.Clown, 0.003f),
            new(NPCID.CochinealBeetle, 0.005f),
            new(NPCID.CyanBeetle, 0.005f),
            new(NPCID.LacBeetle, 0.005f),
            new(NPCID.Squid, 0.01f),
            new(NPCID.BoneSerpentHead, 0.0005f),
            new(NPCID.DevourerHead, 0.005f),
            new(NPCID.FireImp, 0.01f),
            new(NPCID.Ghost, 0.002f),
            new(NPCID.GiantWormHead, 0.005f),
            new(NPCID.GreekSkeleton, 0.01f),
            new(NPCID.Salamander, 0.01f),
            new(NPCID.Salamander2, 0.01f),
            new(NPCID.Salamander3, 0.01f),
            new(NPCID.Salamander4, 0.01f),
            new(NPCID.Salamander5, 0.01f),
            new(NPCID.Salamander6, 0.01f),
            new(NPCID.Salamander7, 0.01f),
            new(NPCID.Salamander8, 0.01f),
            new(NPCID.Salamander9, 0.01f),
            new(NPCID.VoodooDemon, 0.01f),
            new(NPCID.ChaosElemental, 0.002f),
            new(NPCID.CursedHammer, 0.004f),
            new(NPCID.EnchantedSword, 0.004f),
            new(NPCID.CrimsonAxe, 0.004f),
            new(NPCID.PigronCorruption, 0.002f),
            new(NPCID.PigronCrimson, 0.002f),
            new(NPCID.PigronHallow, 0.002f),
            new(NPCID.DesertLamiaDark, 0.002f),
            new(NPCID.DesertLamiaLight, 0.002f),
            new(NPCID.Medusa, 0.004f),
            new(NPCID.RockGolem, 0.001f),
            new(NPCID.Dandelion, 0.01f),
            new(NPCID.IceGolem, 0.02f),
            new(NPCID.SandElemental, 0.02f),
            new(NPCID.Eyezor, 0.006f),
            new(NPCID.Nailhead, 0.006f),
            new(NPCID.DrManFly, 0.006f),
            new(NPCID.Psycho, 0.006f),
            new(NPCID.Mothron, 0.01f),
            new(NPCID.GoldenSlime, 0.1f)
        };
		public override List<WeightedPair> NpcAIDrops => new() {
            new(NPCAIStyleID.Mimic, 0.002f),
            new(NPCAIStyleID.BiomeMimic, 0.002f)
		};
		public override SortedDictionary<ChestID, float> ChestDrops => new() {
            { ChestID.Chest_Normal, 0.002f },
            { ChestID.Gold, 0.003f },
            { ChestID.Gold_Locked, 0.003f },
            { ChestID.Shadow, 0.001f },
            { ChestID.Shadow_Locked, 0.001f },
            { ChestID.RichMahogany, 0.001f },
            { ChestID.Ivy, 0.0006f },
            { ChestID.Frozen, 0.001f },
            { ChestID.LivingWood, 0.0003f },
            { ChestID.Skyware, 0.004f },
            { ChestID.WebCovered, 0.01f },
            { ChestID.Lihzahrd, 0.006f },
            { ChestID.Water, 0.004f },
            { ChestID.Mushroom, 0.01f },
            { ChestID.Granite, 0.01f },
            { ChestID.Marble, 0.01f },
            { ChestID.Gold_DeadMans, 0.05f },
            { ChestID.SandStone, 0.001f }
        };
		public override List<WeightedPair> CrateDrops => new() {
            new(CrateID.Wooden, 0.01f),
            new(CrateID.Iron, 0.02f),
            new(CrateID.Golden, 0.005f),
            new(CrateID.Jungle, 0.003f),
            new(CrateID.Sky, 0.003f),
            new(CrateID.Corrupt, 0.001f),
            new(CrateID.Crimson, 0.001f),
            new(CrateID.Hallowed, 0.001f),
            new(CrateID.Dungeon, 0.0005f),
            new(CrateID.Frozen, 0.001f),
            new(CrateID.Oasis, 0.001f),
            new(CrateID.Ocean, 0.001f),
            new(CrateID.Pearlwood_WoodenHard, 0.006f),
            new(CrateID.Mythril_IronHard, 0.004f),
            new(CrateID.Titanium_GoldenHard, 0.008f),
            new(CrateID.Bramble_JungleHard, 0.002f),
            new(CrateID.Azure_SkyHard, 0.003f),
            new(CrateID.Defiled_CorruptHard, 0.001f),
            new(CrateID.Hematic_CrimsonHard, 0.001f),
            new(CrateID.Divine_HallowedHard, 0.001f),
            new(CrateID.Stockade_DungeonHard, 0.0002f),
            new(CrateID.Boreal_FrozenHard, 0.001f),
            new(CrateID.Mirage_OasisHard, 0.001f),
            new(CrateID.Seaside_OceanHard, 0.002f),
            new(CrateID.Golden_LockBox, 0.0008f),
            new(CrateID.Obsidian_LockBox, 0.0005f)
        };
	}
    public class LuckEnchantmentCommon : LuckEnchantment { }
    public class LuckEnchantmentRare : LuckEnchantment { }
    public class LuckEnchantmentSuperRare : LuckEnchantment { }
    public class LuckEnchantmentUltraRare : LuckEnchantment { }

}
