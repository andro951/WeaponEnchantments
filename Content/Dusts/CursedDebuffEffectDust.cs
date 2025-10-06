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
	public class CursedDebuffEffectDust : ModDust {
		public override string Texture => null;
		public override void OnSpawn(Dust dust) {
			int desiredVanillaDustTexture = DustID.SpectreStaff;
			int frameX = desiredVanillaDustTexture * 10 % 1000;
			int frameY = desiredVanillaDustTexture * 10 / 1000 * 30 + Main.rand.Next(3) * 10;
			dust.frame = new Rectangle(frameX, frameY, 8, 8);
		}
		public override Color? GetAlpha(Dust dust, Color lightColor) {
			return dust.color;
		}
	}
}
