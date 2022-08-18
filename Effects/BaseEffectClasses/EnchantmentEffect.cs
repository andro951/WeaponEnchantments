using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;

namespace WeaponEnchantments.Effects {
    public abstract class EnchantmentEffect {
        //internal static char s(float f) {
        //    return f > 0 ? '+' : '\0';
        //}

        public EnchantmentEffect(float enchantmentPower = 1f) {
            this.EnchantmentPower = enchantmentPower;
        }

        /// <summary>
        /// <para>
        /// The damage class effectiveness of the enchantment.
        /// </para>
        /// <para>
        /// It maps <see cref="DamageClass">damage classes</see> to efficiency percentages.
        /// </para>
        /// <para>
        /// For example, if DamageClass.Melee is mapped to 0.8f, melee weapon effectiveness will be set to 80%.
        /// </para>
        /// <para>
        /// This can be used on the enchantment implementation, but is not required.
        /// </para>
        /// </summary>
        protected Dictionary<DamageClass, float> EnchantmentDamageEfficiency = new Dictionary<DamageClass, float>();

        /// <summary>
        /// <para>
        /// The power of the effect.
        /// </para>
        /// <para>
        /// It represents how strong the effect should be in the given instance, usually set in the constructor.
        /// </para>
        /// <para>
        /// For example, if LifeSteal is mapped to an EnchantmentPower of 1f, LifeSteal would heal 100% of the damage done (if used with an effective damage class).<br \>
        /// Another example is if Defense is mapped to an enchantment power of 5.5f, the defense would increase by 5 (it is floored for integers)
        /// </para>
        /// <para>
        /// This can be used on the enchantment implementation, but is not required.
        /// </para>
        /// </summary>
        protected float EnchantmentPower { get; protected set; }
        
        /// <summary>
        /// How effective the enchantment is.
        /// Affected by armor efficiency (Armor, Weapon, Accesory)
        /// </summary>
        public virtual float EfficiencyMultiplier { get; protected set; } = 1f;
        
        public abstract string DisplayName { get; protected set; }
        public virtual string Tooltip { get; protected set; } = DisplayName;
        public virtual Color TooltipColor { get; protected set; } = new Color(0xaa, 0xaa, 0xaa);

        public virtual bool showTooltip => true;
        public virtual float SelfStackingPenalty { get; protected set; } = 0f;
        public float GetClassEfficiency(DamageClass dc) {
            if (EnchantmentDamageEfficiency.ContainsKey(dc))
                return EnchantmentDamageEfficiency[dc];

            return 1f;
		}
    }
}
