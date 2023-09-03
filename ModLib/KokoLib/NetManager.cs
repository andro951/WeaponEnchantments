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
using Terraria.ModLoader;
using WeaponEnchantments.UI;
using androLib.Common.Utility;
using androLib.Common.Globals;

namespace WeaponEnchantments.ModLib.KokoLib
{
	public interface INetMethods
	{
		public void NetStrikeNPC(NPC npc, int damage, bool crit);
		public void NetDebuffs(NPC npc, int damage, float amaterasuStrength, Dictionary<short, int> debuffs, HashSet<short> dontDissableImmunitiy);
		public void NetActivateOneForAll(Dictionary<NPC, (int, bool)> oneForAllNPCDictionary);
		public void NetAddNPCValue(NPC npc, int value);
		public void NetResetWarReduction(NPC npc);
		public void NetOfferChestItems(SortedDictionary<int, SortedSet<short>> chestItems);
		public void NetResetEnchantedItemInChest(int chestNum, short index);
		public void NetAnglerQuestSwap();
	}
	public class NetManager : ModHandler<INetMethods>, INetMethods
	{
		public override INetMethods Handler => this;
		public void NetStrikeNPC(NPC npc, int damage, bool crit) {
			WEGlobalNPC.StrikeNPC(npc, damage, crit);
			if (Main.netMode == NetmodeID.Server) {
				Net.IgnoreClient = WhoAmI;
				Net<INetMethods>.Proxy.NetStrikeNPC(npc, damage, crit);
			}
		}
		public void NetDebuffs(NPC target, int damage, float amaterasuStrength, Dictionary<short, int> debuffs, HashSet<short> dontDissableImmunitiy) {
			target.HandleOnHitNPCBuffs(damage, amaterasuStrength, debuffs, dontDissableImmunitiy);

			if (Main.netMode == NetmodeID.Server) {
				Net.IgnoreClient = WhoAmI;
				Net<INetMethods>.Proxy.NetDebuffs(target, damage, amaterasuStrength, debuffs, dontDissableImmunitiy);
			}
		}
		public void NetActivateOneForAll(Dictionary<NPC, (int, bool)> oneForAllNPCDictionary) {
			foreach (NPC npc in oneForAllNPCDictionary.Keys) {
				WEGlobalNPC.StrikeNPC(npc, oneForAllNPCDictionary[npc].Item1, oneForAllNPCDictionary[npc].Item2);
			}

			if (Main.netMode == NetmodeID.Server) {
				Net.IgnoreClient = WhoAmI;
				Net<INetMethods>.Proxy.NetActivateOneForAll(oneForAllNPCDictionary);
			}
		}
		public void NetAddNPCValue(NPC npc, int value) {
			npc.AddValue(value);

			if (Main.netMode == NetmodeID.Server) {
				Net.IgnoreClient = WhoAmI;
				Net<INetMethods>.Proxy.NetAddNPCValue(npc, value);
			}
		}

		public void NetResetWarReduction(NPC npc) {
			if (!npc.TryGetWEGlobalNPC(out WEGlobalNPC weGlobalNPC))
				weGlobalNPC.ResetWarReduction();

			if (Main.netMode == NetmodeID.Server) {
				Net.IgnoreClient = WhoAmI;
				Net<INetMethods>.Proxy.NetResetWarReduction(npc);
			}
		}

		public void NetOfferChestItems(SortedDictionary<int, SortedSet<short>> chestItems) {
			if (Main.netMode == NetmodeID.Server)
				EnchantingTableUI.OfferChestItems(chestItems);
		}

		public void NetResetEnchantedItemInChest(int chestNum, short index) {
			if (Main.netMode == NetmodeID.Server)
				EnchantedItemStaticMethods.ResetEnchantedItemInChestFromNet(chestNum, index);
		}

		public void NetAnglerQuestSwap() {
			if (Main.netMode == NetmodeID.Server) {
				Main.AnglerQuestSwap();
				Net<INetMethods>.Proxy.NetAnglerQuestSwap();
			}
			else {
				QuestFish.PrintAnglerQuest();
			}
		}
	}
}