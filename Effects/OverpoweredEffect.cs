using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace WeaponEnchantments.Items.Enchantments.EnchantmentEffects {
    public class OverpoweredEffect : EnchantmentEffect {

        public override void OnAfterHit(NPC npc, WEPlayer player, Item item, ref int damage, ref float knockback, ref bool crit, Projectile projectile = null) {
            throw new NotImplementedException();
        }

        public override void OnModifyHit(NPC npc, WEPlayer player, Item item, ref int damage, ref float knockback, ref bool crit, int hitDirection, Projectile projectile = null) {
            throw new NotImplementedException();
        }

        public override void PostUpdateMiscEffects(WEPlayer player) {
            player.Player.GetDamage(DamageClass.Generic) += enchantmentPower;
        }
    }
}
