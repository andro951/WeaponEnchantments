using System;
using System.Collections.Generic;
using Terraria.ID;

namespace WeaponEnchantments.Common.Utility
{
    public enum DamageTypeSpecificID {
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
}
