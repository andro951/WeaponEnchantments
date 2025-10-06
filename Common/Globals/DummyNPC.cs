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
using androLib.Common.Utility;
using androLib.Common.Globals;
using androLib;

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
		public override void Load() {
			AndroMod.OnResetGameCounter += () => startingTick = 0;
		}
		public static bool StopDPSCheck = true;
		private Dictionary<string, long> totalItemDamages = new();
		public static SortedDictionary<string, double> allTotalItemDamages = new();
		private bool NotCheckingDPS => !WEMod.clientConfig.LogDummyDPS || !StartDPSCheck;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) {
			return !entity.townNPC && WEMod.clientConfig.LogDummyDPS;
		}
		public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone) {
			if (NotCheckingDPS)
				return;

			OnHitNPCWithAny(npc, item, hit);
		}
		public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone) {
			if (NotCheckingDPS)
				return;

			if (projectile.TryGetGlobalProjectile(out WEProjectile weProj) && weProj?.sourceItem != null) {
				Item item = weProj?.sourceItem;
				OnHitNPCWithAny(npc, item, hit, projectile);
			}
		}
		private void OnHitNPCWithAny(NPC npc, Item item, NPC.HitInfo hit, Projectile projectile = null) {
			totalItemDamages.AddOrCombineAddCheckOverflow(item.Name, (long)hit.Damage);
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
					totalItemDamages.Add("Total", ModMath.SumCheckOverFlow(totalItemDamages.Select(d => d.Value).ToArray()));

				string msg = "\n" + totalItemDamages.Select(pair => $"{npc.ModFullName()} ({npc.whoAmI}), {pair.Key}, {(pair.Value / (ticks / 60d)).ToString("F5")}").JoinList("\n");
				msg.LogSimple();
				Main.NewText(msg);
				string key = totalItemDamages.Count > 1 ? totalItemDamages.Keys.Where(k => k != "Total" && k != "Life Regen").First() : totalItemDamages.Keys.First();
				double damage = totalItemDamages.Values.Last() / (ticks / 60d);
				allTotalItemDamages.AddOrCombineAddCheckOverflow(key, damage);
				totalItemDamages.Clear();
			}
		}
	}
}
