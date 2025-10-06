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
using System.IO;

namespace WeaponEnchantments.ModLib.KokoLib
{
	public interface INetMethods {
		public void NetDebuffs(NPC npc, int damage, float amaterasuStrength, Dictionary<short, int> debuffs, HashSet<short> dontDissableImmunitiy);
		public void NetAddNPCValue(NPC npc, int value);
		public void NetApplyWarPenalties(NPC npc, float warReduction);
		public void NetResetWarReduction(NPC npc);
		public void NetOfferChestItems(SortedDictionary<int, SortedSet<short>> chestItems);
		public void NetResetEnchantedItemInChest(int chestNum, short index);
		public void NetAnglerQuestSwap();
		public void SyncCursedNPCData(NPCNetInfoCursedNPC npc);
		public void NetUpdateCursedEssenceCount(int clientWhoAmI, int cursedEssenceCount);
		public void NetBreakTileTarget(int x, int y);
		public void NetGainXPFromBreakTile(int xp);
	}
	public class NetManager : ModHandler<INetMethods>, INetMethods {
		public override INetMethods Handler => this;
		public void NetDebuffs(NPC target, int damage, float amaterasuStrength, Dictionary<short, int> debuffs, HashSet<short> dontDissableImmunitiy) {
			target.HandleOnHitNPCBuffs(damage, amaterasuStrength, debuffs, dontDissableImmunitiy);

			if (Main.netMode == NetmodeID.Server) {
				Net.IgnoreClient = WhoAmI;
				Net<INetMethods>.Proxy.NetDebuffs(target, damage, amaterasuStrength, debuffs, dontDissableImmunitiy);
			}
		}

		public static void AddNPCValue(NPC npc, int value) {
			switch (Main.netMode) {
				case NetmodeID.SinglePlayer:
					npc.AddValue(value);
					break;
				case NetmodeID.MultiplayerClient:
					Net<INetMethods>.Proxy.NetAddNPCValue(npc, value);
					npc.AddValue(value);
					break;
				case NetmodeID.Server:
					throw new Exception("AddNPCValue called by server.");
			}
		}
		public void NetAddNPCValue(NPC npc, int value) {
			switch (Main.netMode) {
				case NetmodeID.Server:
					Net.IgnoreClient = WhoAmI;
					Net<INetMethods>.Proxy.NetAddNPCValue(npc, value);
					npc.AddValue(value);
					break;
				case NetmodeID.MultiplayerClient:
					npc.AddValue(value);
					break;
				case NetmodeID.SinglePlayer:
					throw new Exception("NetAddNPCValue recieved in single player.");
			}
		}

		public static void ApplyWarPenalties(NPC npc, float warReduction) {
			switch (Main.netMode) {
				case NetmodeID.SinglePlayer:
					WEGlobalNPC.ApplyWarPenalties(npc, warReduction);
					break;
				case NetmodeID.MultiplayerClient:
					throw new Exception("ApplyWarPenalties called by multiplayer client.");
				case NetmodeID.Server:
					Net<INetMethods>.Proxy.NetApplyWarPenalties(npc, warReduction);
					break;
			}
		}
		public void NetApplyWarPenalties(NPC npc, float warReduction) {
			switch (Main.netMode) {
				case NetmodeID.Server:
					throw new Exception("NetApplyWarPenalties recieved in single player.");
				case NetmodeID.MultiplayerClient:
					WEGlobalNPC.ApplyWarPenalties(npc, warReduction);
					break;
				case NetmodeID.SinglePlayer:
					throw new Exception("NetApplyWarPenalties recieved in single player.");
			}
		}

		public static void ResetWarReduction(NPC npc) {
			switch (Main.netMode) {
				case NetmodeID.SinglePlayer:
					if (npc.TryGetWEGlobalNPC(out WEGlobalNPC weGlobalNPC))
						weGlobalNPC.ResetWarReduction();
					break;
				case NetmodeID.MultiplayerClient:
					Net<INetMethods>.Proxy.NetResetWarReduction(npc);
					if (npc.TryGetWEGlobalNPC(out WEGlobalNPC weGlobalNPC2))
						weGlobalNPC2.ResetWarReduction();
					break;
				case NetmodeID.Server:
					throw new Exception("ResetWarReduction called by server.");
			}
		}
		public void NetResetWarReduction(NPC npc) {
			switch (Main.netMode) {
				case NetmodeID.Server:
					Net.IgnoreClient = WhoAmI;
					Net<INetMethods>.Proxy.NetResetWarReduction(npc);
					if (npc.TryGetWEGlobalNPC(out WEGlobalNPC weGlobalNPC))
						weGlobalNPC.ResetWarReduction();
					break;
				case NetmodeID.MultiplayerClient:
					if (npc.TryGetWEGlobalNPC(out WEGlobalNPC weGlobalNPC2))
						weGlobalNPC2.ResetWarReduction();
					break;
				case NetmodeID.SinglePlayer:
					throw new Exception("NetResetWarReduction recieved in single player.");
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

		public static void AnglerQuestSwap() {
			switch (Main.netMode) {
				case NetmodeID.SinglePlayer:
					Main.AnglerQuestSwap();
					QuestFish.PrintAnglerQuest();
					break;
				case NetmodeID.MultiplayerClient:
					Net<INetMethods>.Proxy.NetAnglerQuestSwap();
					break;
				case NetmodeID.Server:
					throw new Exception($"AnglerQuestSwap called by the server.");
			}
		}
		public void NetAnglerQuestSwap() {
			switch (Main.netMode) {
				case NetmodeID.Server:
					Main.AnglerQuestSwap();
					Net<INetMethods>.Proxy.NetAnglerQuestSwap();
					break;
				case NetmodeID.MultiplayerClient:
					QuestFish.PrintAnglerQuest();
					break;
				case NetmodeID.SinglePlayer:
					throw new Exception($"NetAnglerQuestSwap recieved in single player.");
			}
		}

		public static void SendCursedNPCDataToClients(int npcWhoAmI) {
			switch (Main.netMode) {
				case NetmodeID.SinglePlayer:
					throw new Exception($"SyncCursedNPCData called in single player.");
				case NetmodeID.MultiplayerClient:
					throw new Exception($"SyncCursedNPCData called by a multiplayer client. Main.myPlayer: {Main.myPlayer}");
				case NetmodeID.Server:
					NPC npc = Main.npc[npcWhoAmI];
					if (npc.TryGetGlobalNPC(out CursedNPC cursedNPC) && (cursedNPC.Cursed || cursedNPC.SpawnedByBoss)) {
						Net<INetMethods>.Proxy.SyncCursedNPCData(new NPCNetInfoCursedNPC(Main.npc[npcWhoAmI]));
					}
					break;
			}
		}
		public void SyncCursedNPCData(NPCNetInfoCursedNPC npc) {
			switch (Main.netMode) {
				case NetmodeID.Server:
					throw new Exception($"SyncCursedNPCData recieved by the server.");
				case NetmodeID.MultiplayerClient:
					npc.SetStatsFromInfoCursedNPC();
					break;
				case NetmodeID.SinglePlayer:
					throw new Exception($"SyncCursedNPCData recieved in single player.");
			}
		}

		public static void UpdateCursedEssenceCount(int cursedEssenceCount) {
			switch (Main.netMode) {
				case NetmodeID.SinglePlayer:
					CurseAttractionNPC.UpdateCursedEssenceCount(Main.myPlayer, cursedEssenceCount);
					break;
				case NetmodeID.MultiplayerClient:
					if (cursedEssenceCount != CurseAttractionNPC.PlayerCursedEssence[Main.myPlayer]) {
						Net.IgnoreClient = Main.myPlayer;
						Net<INetMethods>.Proxy.NetUpdateCursedEssenceCount(Main.myPlayer, cursedEssenceCount);
						CurseAttractionNPC.UpdateCursedEssenceCount(Main.myPlayer, cursedEssenceCount);
					}

					break;
				case NetmodeID.Server:
					throw new Exception($"SynchCursedEssenceCount called by the server");
			}
		}
		public void NetUpdateCursedEssenceCount(int clientWhoAmI, int cursedEssenceCount) {
			switch (Main.netMode) {
				case NetmodeID.Server:
					Net.IgnoreClient = WhoAmI;
					Net<INetMethods>.Proxy.NetUpdateCursedEssenceCount(clientWhoAmI, cursedEssenceCount);
					CurseAttractionNPC.UpdateCursedEssenceCount(clientWhoAmI, cursedEssenceCount);
					break;
				case NetmodeID.MultiplayerClient:
					CurseAttractionNPC.UpdateCursedEssenceCount(clientWhoAmI, cursedEssenceCount);
					break;
				case NetmodeID.SinglePlayer:
					throw new Exception($"SyncCursedEssenceCount recieved in single player.");
			}
		}

		public static void BreakTileTarget(int x, int y) {
			switch (Main.netMode) {
				case NetmodeID.SinglePlayer:
					WEMod.UpdateBrokenTileTarget(x, y, Main.myPlayer);
					break;
				case NetmodeID.MultiplayerClient:
					Net<INetMethods>.Proxy.NetBreakTileTarget(x, y);
					break;
				case NetmodeID.Server:
					throw new Exception($"BreakTileTarget called by the server");
			}
		}
		public void NetBreakTileTarget(int x, int y) {
			switch (Main.netMode) {
				case NetmodeID.Server:
					WEMod.UpdateBrokenTileTarget(x, y, WhoAmI);
					break;
				case NetmodeID.MultiplayerClient:
					Net<INetMethods>.Proxy.NetBreakTileTarget(x, y);
					break;
				case NetmodeID.SinglePlayer:
					throw new Exception($"NetBreakTileTarget recieved in single player.");
			}
		}

		public static void GainXPFromBreakTile(int xp) {
			switch (Main.netMode) {
				case NetmodeID.SinglePlayer:
					WEGlobalTile.GainXPFromBreakTile(xp);
					break;
				case NetmodeID.MultiplayerClient:
					throw new Exception($"GainXPFromBreakTile called by a multiplayer client.  Main.myPlayer: {Main.myPlayer}");
				case NetmodeID.Server:
					Net.ToClient = WEMod.playerBrokenTargetWhoAmI;
					Net<INetMethods>.Proxy.NetGainXPFromBreakTile(xp);
					break;
			}
		}
		public void NetGainXPFromBreakTile(int xp) {
			switch (Main.netMode) {
				case NetmodeID.SinglePlayer:
					throw new Exception($"NetGainXPFromBreakTile recieved in single player.");
				case NetmodeID.MultiplayerClient:
					WEGlobalTile.GainXPFromBreakTile(xp);
					break;
				case NetmodeID.Server:
					throw new Exception($"NetGainXPFromBreakTile recieved by a server.");
			}
		}
	}
	public struct NPCNetInfoCursedNPC {
		public bool cursed;
		public bool spawnedByBoss;
		public int curseIndex = -1;
		public int whoAmI;
		public int lifeMax;
		public float npcSlots;
		public int damage;
		public int defDamage;
		public NPCNetInfoCursedNPC(NPC npc) {
			if (npc.TryGetGlobalNPC(out CursedNPC cursedNPC)) {
				cursed = cursedNPC.Cursed;
				spawnedByBoss = cursedNPC.SpawnedByBoss;

				if (!cursed)
					return;

				curseIndex = cursedNPC.curseEffectIndex;
			}

			whoAmI = npc.whoAmI;
			lifeMax = npc.lifeMax;
			npcSlots = npc.npcSlots;
			damage = npc.damage;
			defDamage = npc.defDamage;
		}
		public NPCNetInfoCursedNPC(BinaryReader reader) {
			cursed = reader.ReadBoolean();
			spawnedByBoss = reader.ReadBoolean();
			if (!cursed)
				return;

			curseIndex = reader.ReadInt32();
			whoAmI = reader.ReadInt32();
			lifeMax = reader.ReadInt32();
			npcSlots = reader.ReadInt32();
			damage = reader.ReadInt32();
			defDamage = reader.ReadInt32();
		}
		public void Write(BinaryWriter writer) {
			writer.Write(cursed);
			writer.Write(spawnedByBoss);
			if (!cursed)
				return;

			writer.Write(curseIndex);
			writer.Write(whoAmI);
			writer.Write(lifeMax);
			writer.Write(npcSlots);
			writer.Write(damage);
			writer.Write(defDamage);
		}
		public void SetStatsFromInfoCursedNPC() {
			NPC npc = Main.npc[whoAmI];
			if (npc.TryGetGlobalNPC(out CursedNPC cursedNPC)) {
				cursedNPC.Cursed = cursed;
				cursedNPC.SpawnedByBoss = spawnedByBoss;
				if (!cursed)
					 return;

				cursedNPC.curseEffectIndex = curseIndex;
			}

			npc.lifeMax = lifeMax;
			npc.npcSlots = npcSlots;
			npc.damage = damage;
			npc.defDamage = defDamage;
		}
	}
}