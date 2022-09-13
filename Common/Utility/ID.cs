using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace WeaponEnchantments.Common.Utility
{
    public enum DamageClassID {
		Default,
		Generic,
		Melee,
		MeleeNoSpeed,
		Ranged,
		Magic,
		Summon,
		SummonMeleeSpeed,
		MagicSummonHybrid,
		Throwing
	} //Located in DamageClassLoader.cs
    public enum EItemType
    {
        None,
        Weapons,
        Armor,
        Accessories,
        FishingPoles,
        Tools
    }
    public enum ArmorSlotSpecificID {
		Head,
		Body,
		Legs
	}

    public enum EnchantmentStat : byte
    {
        None,
        AllForOne,
        AmmoCost,
        AttackSpeed,
        ArmorPenetration,
        AutoReuse,
        BonusManaRegen,
        CatastrophicRelease,
        Channel,
        CrateChance,
        CriticalStrikeChance,
        CriticalStrikeDamage,
        Damage,
        DamageAfterDefenses,
        Defense,
        EnemyMaxSpawns,
        EnemySpawnRate,
        FishingEnemySpawnChance,
        FishingPower,
        GodSlayer,
        InfinitePenetration,
        JumpSpeed,
        Knockback,
        LavaFishing,
        LifeRegen,
        LifeSteal,
        Luck,
        ManaUsage,
        ManaRegen,
        MaxFallSpeed,
        MaxLife,
        MaxLuck,
        MaxMinions,
        MaxMP,
        MovementAcceleration,
        MovementSlowdown,
        MovementSpeed,
        Multishot,
        NPCHitCooldown,
        OneForAll,
        ProjectileVelocity,
        QuestFishChance,
        Size,
        WingTime,
        WhipRange
    }
    public static class ID_Dictionaries
    {
        public static List<EnchantmentStat> WeaponStatDict = new List<EnchantmentStat>() {
            EnchantmentStat.AttackSpeed,
            EnchantmentStat.ArmorPenetration,
            EnchantmentStat.AutoReuse,
            EnchantmentStat.CriticalStrikeChance,
            EnchantmentStat.Damage,
            EnchantmentStat.Knockback,
            EnchantmentStat.ManaUsage,
            EnchantmentStat.Size,
        };
    }
    public enum WeaponStat : byte
    {
        AttackSpeed = 3,
        ArmorPenetration,
        AutoReuse,
        CriticalStrikeChance = 7,
        Damage = 9,
        Knockback = 17,
        ManaUsage = 20,
        Size = 33,
    }
    public enum BuffStyle
	{
        OnTickPlayerBuff,
        OnTickPlayerDebuff,
        OnTickAreaTeamBuff,
        OnTickAreaTeamDebuff,
        OnTickEnemyBuff,
        OnTickEnemyDebuff,
        OnTickAreaEnemyBuff,
        OnTickAreaEnemyDebuff,
        OnHitPlayerBuff,
        OnHitPlayerDebuff,
        OnHitEnemyBuff,
        OnHitEnemyDebuff,
        OnHitAreaTeamBuff,
        OnHitAreaTeamDebuff,
        OnHitAreaEnemyBuff,
        OnHitAreaEnemyDebuff
    }
    public enum PermenantItemFields : short
	{
        
    }
    public enum PermenantItemProperties : short
	{
        ArmorPenetration,
        DamageType
    }

    public enum ChestID
    {
        None = -1,
        Chest_Normal,
        Gold,
        Gold_Locked,
        Shadow,
        Shadow_Locked,
        RichMahogany = 8,
        Ivy = 10,
        Frozen,
        LivingWood,
        Skyware,
        WebCovered = 15,
        Lihzahrd,
        Water,
        Jungle_Dungeon = 23,
        Corruption_Dungeon,
        Crimson_Dungeon,
        Hallowed_Dungeon,
        Ice_Dungeon,
        Mushroom = 32,
        Granite = 40,
        Marble,
        Gold_DeadMans = 104,
        SandStone = 110,
        Desert_Dungeon = 113
    }
    public static class ChestIDMethods
	{
        public static int GetItemType(this ChestID id) {
			switch (id) {
                case ChestID.Chest_Normal:
                    return ItemID.Chest;
                case ChestID.Gold:
                case ChestID.Gold_Locked:
                    return ItemID.GoldChest;
                case ChestID.Shadow:
                case ChestID.Shadow_Locked:
                    return ItemID.ShadowChest;
                case ChestID.RichMahogany:
                    return ItemID.RichMahoganyChest;
                case ChestID.Ivy:
                    return ItemID.IvyChest;
                case ChestID.Frozen:
                    return ItemID.FrozenChest;
                case ChestID.LivingWood:
                    return ItemID.LivingWoodChest;
                case ChestID.Skyware:
                    return ItemID.SkywareChest;
                case ChestID.WebCovered:
                    return ItemID.WebCoveredChest;
                case ChestID.Lihzahrd:
                    return ItemID.LihzahrdChest;
                case ChestID.Water:
                    return ItemID.WaterChest;
                case ChestID.Jungle_Dungeon:
                    return ItemID.JungleChest;
                case ChestID.Corruption_Dungeon:
                    return ItemID.CorruptionChest;
                case ChestID.Crimson_Dungeon:
                    return ItemID.CrimsonChest;
                case ChestID.Hallowed_Dungeon:
                    return ItemID.HallowedChest;
                case ChestID.Ice_Dungeon:
                    return ItemID.IceChest;
                case ChestID.Mushroom:
                    return ItemID.MushroomChest;
                case ChestID.Granite:
                    return ItemID.GraniteChest;
                case ChestID.Marble:
                    return ItemID.MarbleChest;
                case ChestID.Gold_DeadMans:
                    return ItemID.Fake_GoldChest;
                case ChestID.SandStone:
                    return ItemID.DesertChest;
                case ChestID.Desert_Dungeon:
                    return ItemID.DungeonDesertChest;
                default:
                    return -1;
            }
		}
	}
    public enum CrateID
	{
        None = -1,
        Wooden = ItemID.WoodenCrate,
        Iron = ItemID.IronCrate,
        Golden = ItemID.GoldenCrate,
        Jungle = ItemID.JungleFishingCrate,
        Sky = ItemID.FloatingIslandFishingCrate,
        Corrupt = ItemID.CorruptFishingCrate,
        Crimson = ItemID.CrimsonFishingCrate,
        Hallowed = ItemID.HallowedFishingCrate,
        Dungeon = ItemID.DungeonFishingCrate,
        Frozen = ItemID.FrozenCrate,
        Oasis = ItemID.OasisCrate,
        Obsidian = ItemID.LavaCrate,
        Ocean = ItemID.OceanCrate,
        Pearlwood_WoodenHard = ItemID.WoodenCrateHard,
        Mythril_IronHard = ItemID.IronCrateHard,
        Titanium_GoldenHard = ItemID.GoldenCrateHard,
        Bramble_JungleHard = ItemID.JungleFishingCrateHard,
        Azure_SkyHard = ItemID.FloatingIslandFishingCrateHard,
        Defiled_CorruptHard = ItemID.CorruptFishingCrateHard,
        Hematic_CrimsonHard = ItemID.CrimsonFishingCrateHard,
        Divine_HallowedHard = ItemID.HallowedFishingCrateHard,
        Stockade_DungeonHard = ItemID.DungeonFishingCrateHard,
        Boreal_FrozenHard = ItemID.FrozenCrateHard,
        Mirage_OasisHard = ItemID.OasisCrateHard,
        Hellstone_ObsidianHard = ItemID.LavaCrateHard,
        Seaside_OceanHard = ItemID.OasisCrateHard,

        Golden_LockBox = ItemID.LockBox,
        Obsidian_LockBox = ItemID.ObsidianLockbox
	}
    public enum DashID : byte
	{
        NinjaTabiDash = 1,
        EyeOfCthulhuShieldDash,
        SolarDash,
        CrystalNinjaDash = 5
    }
    public enum SellCondition
	{
        IgnoreCondition,
        Never,
        Always,
        AnyTime,
        AnyTimeRare,
        PostKingSlime,
        PostEyeOfCthulhu,
        PostEaterOfWorldsOrBrainOfCthulhu,
        PostSkeletron,
        PostQueenBee,
        PostDeerclops,
        PostGoblinInvasion,
        Luck,
        HardMode,
        PostQueenSlime,
        PostPirateInvasion,
        PostTwins,
        PostDestroyer,
        PostSkeletronPrime,
        PostPlantera,
        PostGolem,
        PostMartianInvasion,
        PostDukeFishron,
        PostEmpressOfLight,
        PostCultist,
        PostSolarTower,
        PostNebulaTower,
        PostStardustTower,
        PostVortexTower,
        PostMoonLord,
	}
    public static class SellConditionChecks
	{
        public static bool CanSell(this SellCondition condition) {
			switch (condition) {
                case SellCondition.Always:
                case SellCondition.AnyTime:
                case SellCondition.AnyTimeRare:
                    return true;
                case SellCondition.PostKingSlime:
                    return NPC.downedSlimeKing;
                case SellCondition.PostEyeOfCthulhu:
                    return NPC.downedBoss1;
                case SellCondition.PostEaterOfWorldsOrBrainOfCthulhu:
                    return NPC.downedBoss2;
                case SellCondition.PostSkeletron:
                    return NPC.downedBoss3;
                case SellCondition.PostQueenBee:
                    return NPC.downedQueenBee;
                case SellCondition.PostQueenSlime:
                    return NPC.downedQueenSlime;
                case SellCondition.PostNebulaTower:
                    return NPC.downedTowerNebula;
                case SellCondition.PostSolarTower:
                    return NPC.downedTowerSolar;
                case SellCondition.PostStardustTower:
                    return NPC.downedTowerStardust;
                case SellCondition.PostVortexTower:
                    return NPC.downedTowerVortex;
                case SellCondition.PostDeerclops:
                    return NPC.downedDeerclops;
                case SellCondition.PostGoblinInvasion:
                    return NPC.downedGoblins;
                case SellCondition.HardMode:
                    return Main.hardMode;
                case SellCondition.PostGolem:
                    return NPC.downedGolemBoss;
                case SellCondition.PostTwins:
                    return NPC.downedMechBoss1;
                case SellCondition.PostDestroyer:
                    return NPC.downedMechBoss2;
                case SellCondition.PostSkeletronPrime:
                    return NPC.downedMechBoss3;
                case SellCondition.PostPirateInvasion:
                    return NPC.downedPirates;
                case SellCondition.PostPlantera:
                    return NPC.downedPlantBoss;
                case SellCondition.PostEmpressOfLight:
                    return NPC.downedEmpressOfLight;
                case SellCondition.PostDukeFishron:
                    return NPC.downedFishron;
                case SellCondition.PostCultist:
                    return NPC.downedAncientCultist;
                case SellCondition.PostMoonLord:
                    return NPC.downedMoonlord;
                case SellCondition.Never:
                default:
                    return false;
            }
		}
	}
    public enum TownNPCTypeID
	{
        Guide = 22,
        Merchant = 17,
        Nurse = 18,
        Demolitionist = 38,
        Angler = 369,
        Dryad = 20,
        ArmsDealer = 19,
        DyeTrader = 207,
        Painter = 227,
        Stylist = 353,
        Zoologist = 633,
        Tavernkeep = 550,
        Golfer = 588,
        GoblinTinkerer = 107,
        WitchDoctor = 228,
        Mechanic = 124,
        Clothier = 54,
        Wizard = 108,
        Steampunker = 178,
        Pirate = 229,
        Truffle = 160,
        TaxCollector = 441,
        Cyborg = 209,
        PartyGirl = 208,
        Princess = 663,
        SantaClaus = 142,
        Cat = 637,
        Dog = 638,
        Bunny = 656,
        TravelingMerchant = 368,
        SkeletonMerchant = 453,
        OldMan = 37
    }
    public enum L_ID1
	{
        Tooltip,
        Dialogue,
        NPCNames,
        Bestiary,
        TownNPCMood
    }
    public enum L_ID2
	{
        None,
        Witch,
        EffectDisplayName,
        EnchantmentEffects,
        EnchantmentCustomTooltips
    }
    public enum L_ID_V
	{
        Item,
        Projectile,
        NPC,
        Buff,
		BuffDescription
	}
	public enum DialogueID {
        StandardDialogue,
        BloodMoon,
        BirthdayParty,
        Storm,
        QueenBee,


        Content,
        NoHome,
        LoveSpace,
        FarFromHome,
        DislikeCrowded,
        HateCrouded,
        LikeBiome,
        LoveBiome,
        DislikeBiome,
        HateBiome,
        LikeNPC,
        LoveNPC,
        DislikeNPC,
        HateNPC
    }
    public enum BiomeID
	{
        Beach,
        Corrupt,
        Crimson,
        Desert,
        DirtLayerHeight,
        Dungeon,
        Forest,
        GemCave,
        Glowshroom,
        Granite,
        Graveyard,
        Hallow,
        Hive,
        Jungle,
        LihzhardTemple,
        Marble,
        Meteor,
        OldOneArmy,
        OverworldHeight,
        PeaceCandle,
        Rain,
        RockLayerHeight,
        Sandstorm,
        SkyHeight,
        Snow,
        TowerNebula,
        TowerStardust,
        TowerSolar,
        TowerVortex,
        UndergroundDesert,
        UnderworldHeight,
        WaterCandle
    }
	public enum DropRestrictionsID
	{
        None,
        HardModeBosses
	}
    public enum WikiItemTypeID
	{
        CraftingMaterial,
        Containments,
        EnchantingTable,
        Enchantments,
        EnchantmentEssence,
        Furniture,
        CraftingStation,
        Storage,
        Armor,
        Set,
        Weapon,
        Tool,
        Mechanism,
        LightSource,
        PowerBooster
	}
    public static class WikiItemTypeMethods
	{
        public static string GetLinkText(this WikiItemTypeID id, out bool external) {
            external = true;
			switch (id) {
                case WikiItemTypeID.CraftingMaterial:
                    return "https://terraria.fandom.com/wiki/Category:Crafting_material_items";
                case WikiItemTypeID.Furniture:
                    return "";
                case WikiItemTypeID.CraftingStation:
                    return "";
                case WikiItemTypeID.Storage:
                    return "";
                case WikiItemTypeID.Armor:
                    return "";
                case WikiItemTypeID.Set:
                    return "";
                case WikiItemTypeID.Weapon:
                    return "";
                case WikiItemTypeID.Tool:
                    return "";
                case WikiItemTypeID.Mechanism:
                    return "";
                case WikiItemTypeID.LightSource:
                    return "";
                default:
                    external = false;
                    return id.ToString().AddSpaces();
			}
		}
	}
    public enum AlignID
	{
        none,
        left,
        middle,
        right,
	}
}
