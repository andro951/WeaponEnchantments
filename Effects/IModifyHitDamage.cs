using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace WeaponEnchantments.Effects {
    internal interface IModifyHitDamage {
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
        public void ModifyHitDamage(NPC npc, WEPlayer player, Item item, ref int damage, ref float knockback, ref bool crit, int hitDirection, Projectile projectile = null);

    }
}
