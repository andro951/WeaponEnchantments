using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace WeaponEnchantments.Content.Dusts {
	public class CursedNPCEffectDust : ModDust {
		public override string Texture => null;
		public override void OnSpawn(Dust dust) {
			int desiredVanillaDustTexture = DustID.Pixie;
			int frameX = desiredVanillaDustTexture * 10 % 1000;
			int frameY = desiredVanillaDustTexture * 10 / 1000 * 30 + Main.rand.Next(3) * 10;
			dust.frame = new Rectangle(frameX, frameY, 8, 8);
		}
		public override Color? GetAlpha(Dust dust, Color lightColor) {
			return dust.color;
		}
		public override bool Update(Dust dust) {
			if (dust.scale < 0.05f) {
				dust.active = false;
				return false;
			}

			dust.scale *= 0.99f;

			return true;
		}
	}
}
