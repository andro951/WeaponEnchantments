using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Common.Utility;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects.CustomEffects
{
    //public class KiDamage : StatEffect, IVanillaStat
    //{
    //    public KiDamage(DifficultyStrength additive = null, DifficultyStrength multiplicative = null, DifficultyStrength flat = null, DifficultyStrength @base = null) : base(additive, multiplicative, flat, @base)
    //    {

    //    }
    //    public KiDamage(EStatModifier eStatModifier) : base(eStatModifier) { }
    //    public override EnchantmentEffect Clone()
    //    {
    //        return new KiDamage(EStatModifier.Clone());
    //    }

    //    public override EnchantmentStat statName => EnchantmentStat.KiDamage;
    //    public override string Tooltip => String.Empty;
    //}

    public class KiDamageSwap : StatEffect, IPermenantStat, IVanillaStat
    {

        public KiDamageSwap(DifficultyStrength additive = null, DifficultyStrength multiplicative = null, DifficultyStrength flat = null, DifficultyStrength @base = null) : base(additive, multiplicative, flat, @base)
        {

        }
        public KiDamageSwap(EStatModifier eStatModifier) : base(eStatModifier) 
        {
            EStatModifier = eStatModifier;
        }
        public override EnchantmentEffect Clone()
        {
            return new KiDamageSwap(EStatModifier.Clone());
        }
        public override string TooltipValue => "Ki";
        public override IEnumerable<object> TooltipArgs => new string[] { TooltipValue };

        public override EnchantmentStat statName => EnchantmentStat.KiDamage;

        public DamageClass BaseDamageClass;

        public void ApplyTo(ref Item item)
        {
            if (item.TryGetEnchantedWeapon(out EnchantedWeapon enchantedWeapon))
            {
                item.DamageType = DamageClass.Default;
                if (BaseDamageClass == null)
                    BaseDamageClass = ContentSamples.ItemsByType[item.type].DamageType;

                enchantedWeapon.damageType = DamageClass.Default;
                if (item.TryGetEnchantedItem(out EnchantedWeapon weapon))
                    weapon.damageType = DamageClass.Default;
                
                enchantedWeapon.baseDamageType = BaseDamageClass;
            }
            
        }
        public void Reset(ref Item item)
        {
            item.DamageType = BaseDamageClass;
            if (item.TryGetEnchantedItem(out EnchantedWeapon weapon))
                weapon.damageType = BaseDamageClass;

            BaseDamageClass = null;
        }

        public void Update(ref Item item, bool reset = false)
        {
            if (reset)
            {
                Reset(ref item);
            }
            else
            {
                ApplyTo(ref item);
            }
        }
    }
}
