using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KokoLib.Emitters;
using KokoLib;
using KokoLib.Nets;
using Terraria;
using Terraria.ID;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Debuffs;

namespace WeaponEnchantments.ModLib.KokoLib
{
	public interface INetOnHitEffects
	{
		public void NetStrikeNPC(NPC npc, int damage, bool crit);
		public void NetDebuffs(NPC npc, int damage, float amaterasuStrength, Dictionary<short, int> debuffs, HashSet<short> dontDissableImmunitiy);
		public void NetActivateOneForAll(Dictionary<NPC, (int, bool)> oneForAllNPCDictionary);
		public void NetAddNPCValue(NPC npc, float value);
	}
	public class NetManager : ModHandler<INetOnHitEffects>, INetOnHitEffects
	{
		public override INetOnHitEffects Handler => this;
		public void NetStrikeNPC(NPC npc, int damage, bool crit) {
			WEGlobalNPC.StrikeNPC(npc, damage, crit);
			if (Main.netMode == NetmodeID.Server)
				Net<INetOnHitEffects>.Proxy.NetStrikeNPC(npc, damage, crit);
		}
		public void NetDebuffs(NPC target, int damage, float amaterasuStrength, Dictionary<short, int> debuffs, HashSet<short> dontDissableImmunitiy) {
			//AmaterasuDebuff.ForceUpdate(npc);
			target.RemoveNPCBuffImunities(debuffs, dontDissableImmunitiy);

			if (target.TryGetWEGlobalNPC(out WEGlobalNPC wEGlobalNPC)) {
				wEGlobalNPC.amaterasuDamage += damage;
				wEGlobalNPC.amaterasuStrength = amaterasuStrength;
			}

			target.ApplyBuffs(debuffs);

			if (Main.netMode == NetmodeID.Server)
				Net<INetOnHitEffects>.Proxy.NetDebuffs(target, damage, amaterasuStrength, debuffs, dontDissableImmunitiy);
		}
		public void NetActivateOneForAll(Dictionary<NPC, (int, bool)> oneForAllNPCDictionary) {
			foreach (NPC npc in oneForAllNPCDictionary.Keys) {
				WEGlobalNPC.StrikeNPC(npc, oneForAllNPCDictionary[npc].Item1, oneForAllNPCDictionary[npc].Item2);
			}

			if (Main.netMode == NetmodeID.Server) {
				Net.IgnoreClient = WhoAmI;
				Net<INetOnHitEffects>.Proxy.NetActivateOneForAll(oneForAllNPCDictionary);
			}
		}
		public void NetAddNPCValue(NPC npc, float value) {
			npc.value += value;
		}
	}
}