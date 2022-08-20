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
        ManaCost,
        ManaRegen,
        MaxFallSpeed,
        MaxHP,
        MaxMinions,
        MaxMP,
        MovementAcceleration,
        MovementSlowdown,
        MovementSpeed,
        ProjectileVelocity,
        Size,
        WingTime,
    }
    public enum WeaponStat : byte
    {
        AttackSpeed = 2,
        ArmorPenetration,//Not implemented, no hook
        AutoReuse,
        CriticalStrikeChance = 6,
        Damage,
        Knockback = 11,
        ManaCost = 14,
        Size = 24,
    }
    public enum PermenantItemFields : short
	{
        
    }
    public enum PermenantItemProperties : short
	{
        ArmorPenetration
    }
}
