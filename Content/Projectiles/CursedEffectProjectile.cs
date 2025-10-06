using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using static Humanizer.In;
using WeaponEnchantments.Content.NPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.ID;
using WeaponEnchantments.Debuffs;
using WeaponEnchantments.Content.Dusts;
using Terraria.DataStructures;

namespace WeaponEnchantments.Content.Projectiles {
	public class CursedEffectProjectile : ModProjectile {
		public override string Texture => base.Texture;
		public const int aiPlayerIndex = 0;
		public const int aiCurseIndex = 1;
		public const int aiTimerIndex = 2;
		public override void AutoStaticDefaults() {
			TextureAssets.Projectile[Projectile.type] = TextureAssets.Projectile[ProjectileID.VampireHeal];
			Main.projFrames[Projectile.type] = 1;
		}
		public override void SetDefaults() {
			Projectile.width = 6;
			Projectile.height = 6;
			Projectile.alpha = 255;
			Projectile.tileCollide = false;
			Projectile.extraUpdates = 2;
		}
		public override void PostDraw(Color lightColor) {
			base.PostDraw(lightColor);
		}
		public override bool PreDraw(ref Color lightColor) {
			return true;
		}
		public override void OnSpawn(IEntitySource source) {
			Projectile.ai[aiTimerIndex] = Main.GameUpdateCount;
		}
		public override void AI() {
			int playerNum = (int)Projectile.ai[aiPlayerIndex];
			float num416 = 4f;
			Vector2 vector36 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
			float num417 = Main.player[playerNum].Center.X - vector36.X;
			float num418 = Main.player[playerNum].Center.Y - vector36.Y;
			float num419 = (float)Math.Sqrt(num417 * num417 + num418 * num418);
			Player player = Main.player[playerNum];
			if (Main.GameUpdateCount >= Projectile.ai[aiTimerIndex] + 120) {
				Projectile.Kill();
			}
			else if (num419 < 50f && Projectile.position.X < player.position.X + (float)player.width && Projectile.position.X + (float)Projectile.width > player.position.X && Projectile.position.Y < player.position.Y + (float)player.height && Projectile.position.Y + (float)Projectile.height > player.position.Y) {
				if (playerNum == Main.myPlayer) {
					CursedNPC.CursedEffectProjectileHitPlayer(Projectile);
				}

				Projectile.Kill();
			}

			num419 = num416 / num419;
			num417 *= num419;
			num418 *= num419;
			Projectile.velocity.X = (Projectile.velocity.X * 15f + num417) / 16f;
			Projectile.velocity.Y = (Projectile.velocity.Y * 15f + num418) / 16f;
			int curseIndex = (int)Projectile.ai[aiCurseIndex];
			if (CursedNPC.GetCurse(curseIndex, out Curse curse)) {
				Color color = curse.Color;
				int dustType = ModContent.DustType<CursedDebuffEffectDust>();
				int alpha = 100;
				float scale = 1.1f;
				for (int dustNum = 0; dustNum < 5; dustNum++) {
					float num427 = Projectile.velocity.X * 0.2f * (float)dustNum;
					float num428 = (0f - Projectile.velocity.Y * 0.2f) * (float)dustNum;
					int dustIndex = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustType, 0f, 0f, alpha, color, scale);
					Main.dust[dustIndex].noGravity = true;
					Dust dust = Main.dust[dustIndex];
					dust.velocity *= 0f;
					Main.dust[dustIndex].position.X -= num427;
					Main.dust[dustIndex].position.Y -= num428;
				}
			}
			else {
				//Should never be hit
				Projectile.Kill();
			}
		}
	}
}
