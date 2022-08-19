using Terraria;
using Terraria.ModLoader;
using WeaponEnchantments.Common;
using static WeaponEnchantments.Common.Globals.EnchantedWeapon;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects {
    public class MinionAttackTarget : EnchantmentEffect, IOnHitEffect {
        public MinionAttackTarget() { }

		public override string Tooltip => $"Enemies hit become the minion attack target.  Same effect as whips.";

		public void OnAfterHit(NPC npc, WEPlayer wePlayer, Item item, int damage, float knockback, bool crit, Projectile projectile = null) {
            wePlayer.Player.MinionAttackTargetNPC = npc.whoAmI;
        }
	}
}
