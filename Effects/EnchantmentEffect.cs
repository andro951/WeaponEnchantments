using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace WeaponEnchantments.Effects {
    public abstract class EnchantmentEffect {
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
        protected virtual Dictionary<DamageClass, float> EnchantmentDamageEfficiency => new Dictionary<DamageClass, float>();

        /// <summary>
        /// <para>
        /// The power of the effect.
        /// </para>
        /// <para>
        /// It represents how strong the effect should be in the given instance, usually set in the constructor.
        /// </para>
        /// <para>
        /// For example, if LifeSteal is mapped to an EnchantmentPower of 1f, LifeSteal would heal 100% of the damage done (if used with an effective damage class).
        /// </para>
        /// <para>
        /// This can be used on the enchantment implementation, but is not required.
        /// </para>
        /// </summary>
        protected float EnchantmentPower { get; set; }
        
        /// <summary>
        /// How effective the enchantment is on the current gear (Armor, Weapon, Accesory)
        /// </summary>
        public float EquipmentEfficiency { get; set; } = 0f;
        
        public virtual string DisplayName { get; set; } = "Default";
        public virtual string Tooltip => $"{DisplayName}: {Math.Round(EnchantmentPower * 100, 1)}%";
        public virtual Color TooltipColor { get; set; } = new Color(0xaa, 0xaa, 0xaa);

        /// <summary>
        /// <para>
        /// The part of the enchantment that runs every frame after all misc effects have triggered.
        /// </para>
        /// <para>
        /// This happens before health regeneration and gravity and such.
        /// </para>
        /// </summary>
        /// <param name="player">The player for which this enchantment applies</param>
        public virtual void PostUpdateMiscEffects(WEPlayer player) { }

        /// <summary>
        /// <para>
        /// The part of the enchantment that runs after hitting an NPC, before the damage has been applied.
        /// </para>
        /// <para>
        /// Stats here are by reference, and as such can be modified as convenient.
        /// </para>
        /// </summary>
        /// <param name="npc">The npc that was just hit</param>
        /// <param name="player">The player for which this enchantment applies</param>
        /// <param name="item">The item that applied the hit</param>
        /// <param name="damage">The damage about to be dealt to the npc</param>
        /// <param name="knockback">The amount of knockback about to be sustained by the npc</param>
        /// <param name="crit">Whether or not the damage is from a critical strike</param>
        /// <param name="hitDirection">The direction from which the attack was done (left or right)</param>
        /// <param name="projectile">If it was issued by a projectile, the projectile instance.</param>
        public virtual void OnModifyHit(NPC npc, WEPlayer player, Item item, ref int damage, ref float knockback, ref bool crit, int hitDirection, Projectile projectile = null) { }

        /// <summary>
        /// <para>
        /// The part of the enchantment that runs after hitting an NPC, after the damage and knockback has been applied.
        /// </para>
        /// <para>
        /// Stats here are not by reference and modifying them will have no consequence.
        /// </para>
        /// </summary>
        /// <param name="npc">The npc that was just hit</param>
        /// <param name="player">The player for which this enchantment applies</param>
        /// <param name="item">The item that applied the hit</param>
        /// <param name="damage">The damage about to be dealt to the npc</param>
        /// <param name="knockback">The amount of knockback about to be sustained by the npc</param>
        /// <param name="crit">Whether or not the damage is from a critical strike</param>
        /// <param name="projectile">If it was issued by a projectile, the projectile instance.</param>
        public virtual void OnAfterHit(NPC npc, WEPlayer player, Item item, int damage, float knockback, bool crit, Projectile projectile = null) { }    
    }
}
