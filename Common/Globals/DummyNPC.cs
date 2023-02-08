using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace WeaponEnchantments.Common.Globals
{
	public class DummyNPC : GlobalNPC
	{
		private double dps => damage / (ticks / 60d);
		private double damage;
		private static int ticks = 300;
		private int startingTick;
		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) {
			return entity.IsDummy();
		}

		public override void OnHitByItem(NPC npc, Player player, Item item, int damage, float knockback, bool crit) {
			
		}
		public override void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit) {
			
		}

	}
}
