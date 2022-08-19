using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using static WeaponEnchantments.Common.Globals.EnchantedWeapon;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects {
    public class OnHitSpawnProjectile : EnchantmentEffect, IOnHitEffect {
        public OnHitSpawnProjectile(short id, int damage = 0, float knockback = 0f) {
            _projectileID = id;
            _projectilieDisplayName = GetProjectileName(_projectileID).AddSpaces();
            _damage = damage;
            _knockback = knockback;
        }

        private short _projectileID;
        private string _projectilieDisplayName;
        private int _damage;
        private float _knockback;
        public static string GetProjectileName(short id) {
            if (id < ProjectileID.Count) {
                ProjectileID buffID = new();
                return buffID.GetType().GetFields().Where(field => field.FieldType == typeof(short) && (short)field.GetValue(buffID) == id).First().Name;
            }

            return ModContent.GetModBuff(id).Name;
        }
        public override string Tooltip => $"Spawns a projectile when hitting an enemy: {_projectilieDisplayName}";

		public void OnAfterHit(NPC npc, WEPlayer wePlayer, Item item, int damage, float knockback, bool crit, Projectile projectile = null) {
            if (projectile != null && projectile.GetWEProjectile().skipOnHitEffects)
                return;

            int newProjectileWhoAmI = Projectile.NewProjectile(projectile != null ? projectile.GetSource_FromThis() : item.GetSource_FromThis(), npc.Center, Vector2.Zero, _projectileID, _damage, _knockback, wePlayer.Player.whoAmI);
            Main.projectile[newProjectileWhoAmI].GetWEProjectile().skipOnHitEffects = true;
        }
	}
}
