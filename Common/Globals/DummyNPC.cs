using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Common.Globals
{
	public class DummyNPC : GlobalNPC
	{
		public static uint ticks => Main.GameUpdateCount - startingTick;
		private static uint startingTick;
		private static bool startDPSCheck;
		public static bool StartDPSCheck {
			get => startDPSCheck;
			set {
				startDPSCheck = value;
				if (value)
					startingTick = Main.GameUpdateCount;
			}
		}
		public static bool StopDPSCheck = true;
		private Dictionary<string, long> totalItemDamages = new();
		public static SortedDictionary<string, double> allTotalItemDamages = new();
		private bool NotCheckingDPS => !WEMod.clientConfig.LogDummyDPS || !StartDPSCheck;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) {
			return !entity.townNPC && WEMod.clientConfig.LogDummyDPS;
		}
		public override void OnHitByItem(NPC npc, Player player, Item item, int damage, float knockback, bool crit) {
			if (NotCheckingDPS)
				return;

			int actualDamage = (int)Main.CalculateDamageNPCsTake((int)damage, npc.defense);
			OnHitNPCWithAny(npc, item, actualDamage);
		}
		public override void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit) {
			if (NotCheckingDPS)
				return;

			projectile.TryGetGlobalProjectile(out WEProjectile weProj);
            Item item = weProj?.sourceItem;
			int actualDamage = (int)Main.CalculateDamageNPCsTake(damage, npc.defense) * (crit ? 2 : 1);
			OnHitNPCWithAny(npc, item, actualDamage, projectile);
		}
		private void OnHitNPCWithAny(NPC npc, Item item, int damage, Projectile projectile = null) {
			//TODO: Check if damage includes crit or not for both melee and projectile
			totalItemDamages.AddOrCombineAddCheckOverflow(item.Name, (long)damage);
			/*if (!totalItemDamages.ContainsKey(item.Name)) {
				totalItemDamages.Add(item.Name, damage);
			}
			else {
				totalItemDamages[item.Name].AddCheckOverflow(damage);
			}*/
		}
		public override void UpdateLifeRegen(NPC npc, ref int damage) {
			if (NotCheckingDPS)
				return;

			if (npc.lifeRegen < 0)
				totalItemDamages.SetValue("Life Regen", (long)-npc.lifeRegen);

			CheckEndDPSCheck(npc);
		}
		private void CheckEndDPSCheck(NPC npc) {
			if (totalItemDamages.Count < 1)
				return;

			if (StopDPSCheck) {
				if (totalItemDamages.Count > 1)
					totalItemDamages.Add("Total", WEMath.SumCheckOverFlow(totalItemDamages.Select(d => d.Value).ToArray()));

				string msg = "\n" + totalItemDamages.Select(pair => $"{npc.FullName} ({npc.whoAmI}), {pair.Key}, {(pair.Value / (ticks / 60d)).ToString("F5")}").JoinList("\n");
				msg.LogSimple();
				Main.NewText(msg);
				string key = totalItemDamages.Count > 1 ? totalItemDamages.Keys.Where(k => k != "Total" && k != "Life Regen").First() : totalItemDamages.Keys.First();
				double damage = totalItemDamages.Values.Last() / (ticks / 60d);
				allTotalItemDamages.AddOrCombineAddCheckOverflow(key, damage);
				totalItemDamages.Clear();
			}
		}
		//TODO: Add damage from loss of life
	}
}
