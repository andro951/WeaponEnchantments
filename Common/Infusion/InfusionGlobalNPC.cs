using Terraria.ID;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Common.Utility;

namespace WeaponEnchantments.Common.Infusion
{
	public class InfusionGlobalNPC : GlobalNPC
	{
		public static bool StoreNPCSpawnInfo => true;
		private static NPCSpawnInfo lastSpawnInfo;
		private static bool started = false;
		public static SortedDictionary<int, (NPCSpawnInfo, NPCSpawnInfo)> StoredNPCSpawnInfo { private set; get; }
		private static SortedSet<int> npcsThatAreSetup = new();//TODO: needs to be populated by SpawnGroups

		public override void Load() {
			if (StoreNPCSpawnInfo)
				StoredNPCSpawnInfo = new();
		}
		public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo) {
			lastSpawnInfo = spawnInfo;
			started = true;
		}
		public override void OnSpawn(NPC npc, IEntitySource source) {
			if (!started || npc.IsBoss())
				return;

			if (!InfusionProgression.NPCsThatDropWeaponsOrIngredients.Contains(npc.netID)) {
				$"{npc.S()} has no weapon or weapon ingredient drops".LogSimple();//temp
				return;
			}

			int netID = npc.netID;
			if (StoreNPCSpawnInfo) {
				if (!StoredNPCSpawnInfo.ContainsKey(netID)) {
					StoredNPCSpawnInfo.Add(netID, (lastSpawnInfo, lastSpawnInfo));
				}
				else {
					CompairUpdateSpawnInfo(StoredNPCSpawnInfo, netID, lastSpawnInfo);
				}
			}
			else if (!npcsThatAreSetup.Contains(netID)) {
				$"{netID.GetNPCNameString()}, {lastSpawnInfo.S()}, source: {source.Context}, WorldSize: {""}".LogNT(ChatMessagesIDs.NPCSpawnSourceNotSetup);
				npcsThatAreSetup.Add(netID);
			}
		}
		public static void SaveWorldData() {
			if (StoreNPCSpawnInfo)
				StoredNPCSpawnInfo.NPCDictionaryStrings("NPCSpawnRules", ((NPCSpawnInfo, NPCSpawnInfo) info) => $"({info.Item1.S()}, {info.Item2.S()})", "((int[], bool[]), (int[], bool[]))").LogSimple();
		}
		private static void CompairUpdateSpawnInfo(SortedDictionary<int, (NPCSpawnInfo, NPCSpawnInfo)> dict, int netID, NPCSpawnInfo newInfo) {
			(NPCSpawnInfo, NPCSpawnInfo) info = dict[netID];
			(int[], bool[]) newData = newInfo.EncodeNPCSpawnInfo();
			(int[], bool[]) minData = info.Item1.EncodeNPCSpawnInfo();
			(int[], bool[]) maxData = info.Item2.EncodeNPCSpawnInfo();

			bool minChanged = false;
			bool maxChanged = false;
			//0 is the player id, skip it.
			for (int i = 1; i < newData.Item1.Length; i++) {
				int newValue = newData.Item1[i];
				int minValue = minData.Item1[i];
				int maxValue = maxData.Item1[i];
				if (newValue < minValue) {
					minData.Item1[i] = newValue;
					minChanged = true;
				}
				else if (newValue > maxValue) {
					maxData.Item1[i] = newValue;
					maxChanged = true;
				}
			}

			for (int i = 0; i < newData.Item2.Length; i++) {
				if (!newData.Item2[i]) {
					if (minData.Item2[i]) {
						minData.Item2[i] = false;
						minChanged = true;
					}
					if (maxData.Item2[i]) {
						maxData.Item2[i] = false;
						maxChanged = true;
					}
				}
			}

			if (minChanged && maxChanged) {
				dict[netID] = (minData.DecodeNPCSpawnInfo(), maxData.DecodeNPCSpawnInfo());
			}
			else if (minChanged) {
				dict[netID] = (minData.DecodeNPCSpawnInfo(), info.Item2);
			}
			else if (maxChanged) {
				dict[netID] = (info.Item1, maxData.DecodeNPCSpawnInfo());
			}
		}
	}
}
