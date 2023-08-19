using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Common.Utility;

namespace WeaponEnchantments.ModIntegration
{
	public static class BossChecklistIntegration
	{
		public static bool ShouldSetupBossPowerBoosterDrops => !UsedBossChecklistForBossPowerBoosterDrops && BossInfos != null;
		public static bool UsedBossChecklistForBossPowerBoosterDrops = false;  
		private static readonly Version BossChecklistAPIVersion = new Version(1, 1); // Do not change this yourself.
		private static SortedDictionary<string, BossChecklistBossInfo> bossInfos = null;
		public static SortedDictionary<string, BossChecklistBossInfo> BossInfos {
			get {
				if (bossInfos == null)
					DoBossChecklistIntegration(ModContent.GetInstance<WEMod>());

				return bossInfos;
			}
		}
		public static SortedDictionary<int, string> BossInfoNetIDKeys = new();
		public static bool DoBossChecklistIntegration(Mod mod) {
			if (!WEModSystem.StartedPostAddRecipes)
				return false;

			bossInfos = null;
			Mod BossChecklist = ModLoader.GetMod("BossChecklist");
			if (BossChecklist != null && BossChecklist.Version >= BossChecklistAPIVersion) {
				object currentBossInfoResponse = BossChecklist.Call("GetBossInfoDictionary", mod, BossChecklistAPIVersion.ToString());
				if (currentBossInfoResponse is Dictionary<string, Dictionary<string, object>> bossInfoList) {
					bossInfos = new(bossInfoList.ToDictionary(boss => boss.Key, boss => new BossChecklistBossInfo() {
						key = boss.Value.ContainsKey("key") ? boss.Value["key"] as string : "",
						modSource = boss.Value.ContainsKey("modSource") ? boss.Value["modSource"] as string : "",
						internalName = boss.Value.ContainsKey("internalName") ? boss.Value["internalName"] as string : "",
						displayName = boss.Value.ContainsKey("displayName") ? boss.Value["displayName"] as string : "",
						progression = boss.Value.ContainsKey("progression") ? Convert.ToSingle(boss.Value["progression"]) : 0f,
						downed = boss.Value.ContainsKey("downed") ? boss.Value["downed"] as Func<bool> : () => false,
						isBoss = boss.Value.ContainsKey("isBoss") ? Convert.ToBoolean(boss.Value["isBoss"]) : false,
						isMiniboss = boss.Value.ContainsKey("isMiniboss") ? Convert.ToBoolean(boss.Value["isMiniboss"]) : false,
						isEvent = boss.Value.ContainsKey("isEvent") ? Convert.ToBoolean(boss.Value["isEvent"]) : false,
						npcIDs = boss.Value.ContainsKey("npcIDs") ? boss.Value["npcIDs"] as List<int> : new List<int>(),
						spawnItem = boss.Value.ContainsKey("spawnItem") ? boss.Value["spawnItem"] as List<int> : new List<int>(),
						loot = boss.Value.ContainsKey("loot") ? boss.Value["loot"] as List<int> : new List<int>(),
						collection = boss.Value.ContainsKey("collection") ? boss.Value["collection"] as List<int> : new List<int>(),
					}));

					BossInfoNetIDKeys = new();
					foreach (var bossInfo in bossInfos.Where(boss => boss.Value.isBoss || boss.Value.isMiniboss)) {
						List<int> npcIDs = bossInfo.Value.npcIDs;
						if (npcIDs.Count < 1) {
							$"Skipping bossInfo, npcIDs.Count < 1: {bossInfo.Key}, {bossInfo.Value.internalName}, {bossInfo.Value.progression}".LogSimple_WE();
							continue;
						}

						int netID = npcIDs.First();
						bool added = BossInfoNetIDKeys.TryAdd(netID, bossInfo.Key);
						if (!added) {
							string currentKey = BossInfoNetIDKeys[netID];
							BossChecklistBossInfo currentInfo = bossInfos[currentKey];
							$"bossInfo netID already exists new: {bossInfo.Key}, {bossInfo.Value.internalName}, {bossInfo.Value.progression}\ncurrent: {currentKey}, {currentInfo.internalName}, {currentInfo.progression}".LogSimple_WE();
						}
					}

					return true;
				}
			}
			return false;
		}
		public static void UnloadBossChecklistIntegration() {
			bossInfos = null;
		}
	}
}
