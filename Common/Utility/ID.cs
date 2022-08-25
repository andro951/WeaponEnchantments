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
        CriticalStrikeChance,
        CriticalStrikeDamage,
        Damage,
        DamageAfterDefenses,
        Defense,
        EnemyMaxSpawns,
        EnemySpawnRate,
        GodSlayer,
        JumpSpeed,
        Knockback,
        LifeRegen,
        LifeSteal,
        ManaUsage,
        ManaRegen,
        MaxFallSpeed,
        MaxHP,
        MaxMinions,
        MaxMP,
        MovementAcceleration,
        MovementSlowdown,
        MovementSpeed,
        Multishot,
        NPCHitCooldown,
        OneForAll,
        ProjectileVelocity,
        Size,
        WingTime,
        WhipRange
    }
    public enum WeaponStat : byte
    {
        AttackSpeed = 3,
        ArmorPenetration,
        AutoReuse,
        CriticalStrikeChance = 7,
        Damage = 9,
        Knockback = 16,
        ManaUsage = 19,
        Size = 32,
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
        Chest,
        GoldChest,
        GoldChestLocked,
        ShadowChest,
        ShadowChestLocked,
        RichMahoganyChest = 8,
        IvyChest = 10,
        FrozenChest,
        LivingWoodChest,
        SkywareChest,
        WebCoveredChest = 15,
        LihzahrdChest,
        WaterChest,
        DungeonJungleChest = 23,
        DungeonCorruptionChest,
        DungeonCrimsonChest,
        DungeonHallowedChest,
        DungeonIceChest,
        MushroomChest = 32,
        GraniteChest = 40,
        MarbleChest,
        GoldDeadMansChest = 104,
        SandStoneChest = 110,
        DungeonDesertChest = 113
    }
    public enum DashID : byte
	{
        NinjaTabiDash = 1,
        EyeOfCthulhuShieldDash,
        SolarDash,
        CrystalNinjaDash = 5
    }
}
