using Terraria;
using Terraria.ModLoader;
using WeaponEnchantments.Common;
using static WeaponEnchantments.Common.Globals.EnchantedWeapon;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects {
    public class MinionAttackTarget : EnchantmentEffect, IOnHitEffect {
        public MinionAttackTarget() { }
        public override EnchantmentEffect Clone() {
            return new MinionAttackTarget();
        }

		public override string TooltipValue => "Enabled";
		public void OnHitNPC(NPC npc, WEPlayer wePlayer, Item item, int damage, float knockback, bool crit, Projectile projectile = null) {
            wePlayer.Player.MinionAttackTargetNPC = npc.whoAmI;
        }
	}
}
