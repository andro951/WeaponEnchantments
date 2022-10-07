using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Common.Utility;
using static WeaponEnchantments.Common.Globals.EnchantedWeapon;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects {
    public class OnHitSpawnProjectile : EnchantmentEffect, IOnHitEffect {
        public OnHitSpawnProjectile(short id, int damage = 0, float knockback = 0f, bool unique = true) {
            _projectileID = id;
            _projectilieDisplayName = GetProjectileName(_projectileID).AddSpaces();
            _damage = damage;
            _knockback = knockback;
            _unique = unique;
        }
        public override EnchantmentEffect Clone() {
            return new OnHitSpawnProjectile(_projectileID, _damage, _knockback, _unique);
        }

        private short _projectileID;
        private string _projectilieDisplayName;
        private int _damage;
        private float _knockback;
        private bool _unique;
        public static string GetProjectileName(short id) {
            if (id < ProjectileID.Count) {
                ProjectileID buffID = new();
                return buffID.GetType().GetFields().Where(field => field.FieldType == typeof(short) && (short)field.GetValue(buffID) == id).First().Name;
            }

            return ModContent.GetModBuff(id).Name;
        }
		//public override string Tooltip => $"Spawns a projectile when hitting an enemy: {_projectilieDisplayName}";
		public override IEnumerable<object> TooltipArgs => new object[] { _projectilieDisplayName };

		public void OnHitNPC(NPC target, WEPlayer wePlayer, Item item, int damage, float knockback, bool crit, Projectile projectile = null) {
            if (projectile != null && ((WEProjectile)projectile.GetMyGlobalProjectile()).skipOnHitEffects || target.netID == NPCID.TargetDummy)
                return;

            if (_unique) {
                foreach (Projectile mainProjectile in Main.projectile) {
                    if (mainProjectile.type == _projectileID)
                        return;
                }
			}

            int newProjectileWhoAmI = Projectile.NewProjectile(projectile != null ? projectile.GetSource_FromThis() : item.GetSource_FromThis(), target.Center, Vector2.Zero, _projectileID, _damage, _knockback, wePlayer.Player.whoAmI);
            ((WEProjectile)Main.projectile[newProjectileWhoAmI].GetMyGlobalProjectile()).skipOnHitEffects = true;
        }
	}
}
