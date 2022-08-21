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
        Knockback = 11,
        ManaUsage = 14,
        Size = 26,
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
