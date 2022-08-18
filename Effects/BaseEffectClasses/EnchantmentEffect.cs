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

        public EnchantmentEffect(float EnchantmentStrength = 1f) {
            //this.EnchantmentStrength = EnchantmentStrength;
        }

        /// <summary>
        /// The EnchantmentStrength modifier based on damage type of the weapon.<br/>
        /// It maps <see cref="DamageClass">damage classes</see> to efficiency percentages.<br/>
        /// For example, if DamageClass.Melee is mapped to 0.8f, EnchantmentStrength will be reduced to 80% on melee weapons.<br/>
        /// </summary>
        protected Dictionary<DamageClass, float> EnchantmentDamageEfficiency = new Dictionary<DamageClass, float>();

        /// <summary>
        /// The strength of the effect.<br/>
        /// For example, if LifeSteal is mapped to an EnchantmentStrength of 1f, LifeSteal would heal 100% of the damage done.<br \>
        /// Another example is if Defense is mapped to an enchantment power of 5.5f, the defense would increase by 5.<br/>
        /// Affected by EnchantmentamageEfficiency and EfficiencyMultiplier<br/>
        /// </summary>
        public virtual float EffectStrength { get; protected set; }
        
        /// <summary>
        /// Used to modify the EnchantmentStrength<br/>
        /// Affected by the item type the enchantment is applied on. (Weapon, Armor, Accesory)<br/>
        /// </summary>
        public virtual float EfficiencyMultiplier { get; set; } = 1f;
        
        public virtual string DisplayName => GetType().Name.AddSpaces();
        public virtual string Tooltip => DisplayName;
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
