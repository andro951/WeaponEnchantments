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
        AmmoCost,
        AttackSpeed,
        ArmorPenetration,
        AutoReuse,
        BonusManaRegen,
        CriticalStrikeChance,
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
        AttackSpeed = 2,
        ArmorPenetration,
        AutoReuse,
        CriticalStrikeChance = 6,
        Damage,
        Knockback = 14,
        ManaUsage = 17,
        Size = 30,
    }
    public enum PermenantItemFields : short
	{
        
    }
    public enum PermenantItemProperties : short
	{
        ArmorPenetration,
        DamageType
    }
}
