using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static WeaponEnchantments.Common.Globals.WEGlobalNPC;
using System.Collections.Generic;
using WeaponEnchantments.Common.Utility;
using System.Diagnostics;
using System.Linq;
using Terraria.GameContent.ItemDropRules;
using androLib.Common.Utility;
using androLib.Common.Globals;
using androLib;

namespace WeaponEnchantments.Common.Globals
{
    public class GlobalBossBags : GlobalItem
    {
        public static bool printNPCNameOnHitForBossBagSupport => false && Debugger.IsAttached;
        private static SortedDictionary<string, (string, float)> manuallySetModBossBags = null;
		public static SortedDictionary<string, (string, float)> ManuallySetModBossBags {
            get {
                if (manuallySetModBossBags == null) {
                    manuallySetModBossBags = new();
                    if (AndroMod.starsAboveEnabled) {
                        manuallySetModBossBags.Add("StarsAbove/VagrantBoss", ("StarsAbove/VagrantBossBag", 1f));
                    }
				}

                return manuallySetModBossBags;
            }
        }
		private static SortedDictionary<string, int> bossTypes = null;
		public static SortedDictionary<string, int> BossTypes {
            get {
                if (bossTypes == null)
                    SetupBossTypes();

                return bossTypes ?? new();
            }
        }
        private static SortedDictionary<int, int> bossBagNPCs = null;
		public static SortedDictionary<int, int> BossBagNPCs {
            get {
                if (bossBagNPCs == null)
                    SetupBossTypes();

                return bossBagNPCs ?? new(); 
            }
        }
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot) {
            int type = item.type;

            if (!BossBagsData.BossBags.Contains(type))
				return;

            ModdedLootBagDropRule moddedLootBagDropRule = new(type);
            itemLoot.Add(moddedLootBagDropRule);
        }
        private static void SetupBossTypes() {
            if (AllItemDropsFromNpcs == null)
                return;

            bossTypes = new();
            bossBagNPCs = new();
			foreach (int bossBagType in BossBagsData.BossBags) {
                if (!AllItemDropsFromNpcs.ContainsKey(bossBagType)) {
                    switch (bossBagType) {
                        case ItemID.CultistBossBag:
                            break;
                        default:
							$"Boss bag not in AllItemDropsFromNpcs: {bossBagType.CSI().S()}".LogSimple_WE();
							break;
                    }
                    
                    continue;
                }

                foreach (int netID in AllItemDropsFromNpcs[bossBagType].Select(p => p.Item1)) {
					string name = netID.CSNPC().ModFullName();
                    bool addedToBossTypes = bossTypes.TryAdd(name, netID);
                    //if (Debugger.IsAttached && !addedToBossTypes) $"{name} already in bossTypes.  new: {netID}, current: {bossTypes[name]}".LogSimple();
                    bool addedToBossBagNPCs = bossBagNPCs.TryAdd(netID, bossBagType);
                    //if (Debugger.IsAttached && !addedToBossBagNPCs) $"{netID.CSNPC().S()} already in bossBagNPCs.  New: {bossBagType}, current: {bossBagNPCs[netID]}".LogSimple();
				}
            }

            if (Debugger.IsAttached) bossBagNPCs.Select(p => $"{p.Key.GetNPCIDOrName()} : {p.Value.GetItemIDOrName()}").S("bossBagNPCs").LogSimple_WE();
        }
	}
}
