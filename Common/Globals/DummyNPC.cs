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
		private static int ticks = 300;
		private int startingTick;
		private static bool startDPSCheck;
		public static bool StartDPSCheck {
			get => startDPSCheck;
			set {
				startDPSCheck = value;
				if (startDPSCheck)
					startingTick = Main.GameUpdateCount;
			}
		}
		private Dictionary<string, double> totalItemDamages = new();
		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) {
			return entity.IsDummy();
		}

		public override void OnHitByItem(NPC npc, Player player, Item item, int damage, float knockback, bool crit) {
			if (!WEMod.clientConfig.LogDummyDPS || !StartDPSCheck)
				return;
			
			OnHitNPCWithAny(npc, item, damage, knockback, crit);
		}
		public override void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit) {
			if (!WEMod.clientConfig.LogDummyDPS || !StartDPSCheck)
				return;
			
			projectile.TryGetGlobalProjectile(out WEProjectile weProj);
            		Item item = weProj?.sourceItem;
			OnHitNPCWithAny(npc, item, damage, knockback, crit, projectile);
		}
		private void OnHitNPCWith(NPC npc, Item item, float knockback, bool crit, Projectile projectile = null) {
			//TODO: Check if damage includes crit or not for both melee and projectile
			if (!totalItemDamages.ContainsKey(item.Name)) {
				totalItemDamages.Add(item.Name, damage);
			}
			else {
				totalItemDamages[item.Name] += damage;
			}
		}
		private void CheckEndDPSCheck() {
			//TODO: put thisinto an on tick method
			if (Main.GameUpdateCount >= startingTick + ticks) {
				StartDPSCheck = false;
				foreach (KeyValuePair<string, double> pair in totalItemDamages) {
					$"Dummy {NPC.whoAmI}, {pair.Key}, {pair.Value}".LogSimple();
				}
				
				totalItemDamages.Empty();
			}
		}
		//TODO: Add damage from loss of life
	}
}
