using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace WeaponEnchantments.Effects {
    internal interface IOnHitEffect {

        /// <summary>
        /// <para>
        /// The part of the enchantment that runs after hitting an NPC, after the damage and knockback has been applied.
        /// </para>
        /// <para>
        /// Stats here are not by reference and modifying them will have no consequence.
        /// </para>
        /// </summary>
        /// <param name="npc">The npc that was just hit</param>
        /// <param name="wePlayer">The player for which this enchantment applies</param>
        /// <param name="item">The item that applied the hit</param>
        /// <param name="damage">The damage about to be dealt to the npc</param>
        /// <param name="knockback">The amount of knockback about to be sustained by the npc</param>
        /// <param name="crit">Whether or not the damage is from a critical strike</param>
        /// <param name="projectile">If it was issued by a projectile, the projectile instance.</param>
        public void OnAfterHit(NPC npc, WEPlayer wePlayer, Item item, int damage, float knockback, bool crit, Projectile projectile = null);
    }
}
