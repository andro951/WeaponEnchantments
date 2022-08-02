using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace WeaponEnchantments.EnchantmentEffects {
    public abstract class EnchantmentEffect {


        public EnchantmentEffect(float enchantmentPower = 1f) {
            this.enchantmentPower = enchantmentPower;
        }

        public virtual Dictionary<DamageClass, float> EnchantmentDamageEfficiency => new Dictionary<DamageClass, float>();

        public float enchantmentPower { get; set; }

        public virtual void PostUpdateMiscEffects(WEPlayer player) { }

        // Happens before the damage has been applied. You can modify stuff here.
        public virtual void OnModifyHit(NPC npc, WEPlayer player, Item item, ref int damage, ref float knockback, ref bool crit, int hitDirection, Projectile projectile = null) { }

        // Happens after the damage has been applied. The NPC might as well be dead here.
        public virtual void OnAfterHit(NPC npc, WEPlayer player, Item item, ref int damage, ref float knockback, ref bool crit, Projectile projectile = null) { }
    }
}
