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
        JumpSpeedBoost,
        Knockback,
        LifeRegen,
        LifeSteal,
        ManaCost,
        ManaRegen,
        MaxFallSpeed,
        MaxHP,
        MaxMinions,
        MaxMP,
        MoveAcceleration,
        MoveSlowdown,
        MoveSpeed,
        Size,
        WingTime,
    }
}
