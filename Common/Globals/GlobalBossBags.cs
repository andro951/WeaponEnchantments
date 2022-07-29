using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Items;
using static WeaponEnchantments.Common.Utility.UtilityMethods;
using static WeaponEnchantments.Common.Globals.WEGlobalNPC;
using System.Collections.Generic;
using WeaponEnchantments.Common.Utility;

namespace WeaponEnchantments.Common.Globals
{
    public class GlobalBossBags : GlobalItem
    {
		public override void ModifyItemLoot(Item item, ItemLoot itemLoot) {
            int type = item.type;
            if (ItemID.Sets.BossBag[type]) {

				#region Debug

				if (LogMethods.debugging && item.ModItem != null) {
                    string bagName = item.ModItem.Name;
                    bagName.Log();
                }

				#endregion

				//IEntitySource src = player.GetSource_OpenItem(type);

                //Check if the bag has an associated npc setup
                NPC npc = GetNPCFromBossBagType(type);

                //If npc is setup, spawn items
                if (npc == null)
                    return;

                GetLoot(itemLoot, npc, true);
            }
        }

        /// <summary>
        /// Finds the boss type based on a boss bag type.<br/>
        /// </summary>
        /// <param name="bossBagType"></param>
        /// <returns>new NPC of the associated boss bag.</returns>
        public static NPC GetNPCFromBossBagType(int bossBagType) {
            int npcID;
            switch (bossBagType) {
                case ItemID.KingSlimeBossBag:
                    npcID = NPCID.KingSlime;
                    break;
                case ItemID.EyeOfCthulhuBossBag:
                    npcID = NPCID.EyeofCthulhu;
                    break;
                case ItemID.EaterOfWorldsBossBag:
                    npcID = NPCID.EaterofWorldsHead;
                    break;
                case ItemID.BrainOfCthulhuBossBag:
                    npcID = NPCID.BrainofCthulhu;
                    break;
                case ItemID.QueenBeeBossBag:
                    npcID = NPCID.QueenBee;
                    break;
                case ItemID.SkeletronBossBag:
                    npcID = NPCID.SkeletronHead;
                    break;
                case ItemID.DeerclopsBossBag:
                    npcID = NPCID.Deerclops;
                    break;
                case ItemID.WallOfFleshBossBag:
                    npcID = NPCID.WallofFlesh;
                    break;
                case ItemID.QueenSlimeBossBag:
                    npcID = NPCID.QueenSlimeBoss;
                    break;
                case ItemID.TwinsBossBag:
                    npcID = NPCID.Retinazer;
                    break;
                case ItemID.DestroyerBossBag:
                    npcID = NPCID.TheDestroyer;
                    break;
                case ItemID.SkeletronPrimeBossBag:
                    npcID = NPCID.SkeletronPrime;
                    break;
                case ItemID.PlanteraBossBag:
                    npcID = NPCID.Plantera;
                    break;
                case ItemID.GolemBossBag:
                    npcID = NPCID.Golem;
                    break;
                case ItemID.FishronBossBag:
                    npcID = NPCID.DukeFishron;
                    break;
                case ItemID.FairyQueenBossBag:
                    npcID = NPCID.HallowBoss;
                    break;
                case ItemID.CultistBossBag:
                    npcID = NPCID.CultistBoss;
                    break;
                case ItemID.MoonLordBossBag:
                    npcID = NPCID.MoonLordCore;
                    break;
                case ItemID.BossBagDarkMage:
                    npcID = NPCID.DD2DarkMageT1;
                    break;
                case ItemID.BossBagOgre:
                    npcID = NPCID.DD2OgreT2;
                    break;
                case ItemID.BossBagBetsy:
                    npcID = NPCID.DD2Betsy;
                    break;
                default:
                    //Modded bag
                    npcID = -1000;
                    string bossName = null;

                    //Check if moddedModded bag has been setup in GetModdedBossNameFromBag.
                    if (ContentSamples.ItemsByType[bossBagType].ModItem != null) {
                        string bagName = ContentSamples.ItemsByType[bossBagType].ModItem.Name;
                        bossName = GetModdedBossNameFromBag(bagName);
                    }

                    if(bossName != null) {
                        //Match the bossName from GetModdedBossNameFromBag to an actual loaded boss.
                        for (int i = 0; i < NPCLoader.NPCCount; i++) {
                            NPC sampleNPC = ContentSamples.NpcsByNetId[i];
                            if (sampleNPC.FullName == bossName) {
                                //Found
                                npcID = sampleNPC.netID;
                                break;
                            }
                        }

                        //Not found
                        if(npcID == -1000)
                            $"Failed to find this boss name: {(bossName != null ? bossName : "Null")} that dropps this boss bag type: {bossBagType}.".LogNT();
                    }
                    break;
            }

            if (npcID != -1000) {
                //Return the npc that drops the bag.
                NPC tempNpc = (NPC)ContentSamples.NpcsByNetId[npcID].Clone();

                return tempNpc;
            }

            return null;
        }

        /// <summary>
        /// Finds the boss NPCID based on a boss bag type.  Only set up for vanilla bosses.<br/>
        /// </summary>
        /// <param name="bagID"></param>
        /// <returns>Boss NPCID of the associated boss bag.</returns>
        public static int GetBossTypeFromBag(int bagID) {
            switch (bagID)
            {
                case ItemID.KingSlimeBossBag:
                    return NPCID.KingSlime;
                case ItemID.EyeOfCthulhuBossBag:
                    return NPCID.EyeofCthulhu;
                case ItemID.EaterOfWorldsBossBag:
                    return NPCID.EaterofWorldsHead;
                case ItemID.BrainOfCthulhuBossBag:
                    return NPCID.BrainofCthulhu;
                case ItemID.QueenBeeBossBag:
                    return NPCID.QueenBee;
                case ItemID.SkeletronBossBag:
                    return NPCID.SkeletronHead;
                case ItemID.DeerclopsBossBag:
                    return NPCID.Deerclops;
                case ItemID.WallOfFleshBossBag:
                    return NPCID.WallofFlesh;
                case ItemID.QueenSlimeBossBag:
                    return NPCID.QueenSlimeBoss;
                case ItemID.TwinsBossBag:
                    return NPCID.Retinazer;
                case ItemID.DestroyerBossBag:
                    return NPCID.TheDestroyer;
                case ItemID.SkeletronPrimeBossBag:
                    return NPCID.SkeletronPrime;
                case ItemID.PlanteraBossBag:
                    return NPCID.Plantera;
                case ItemID.GolemBossBag:
                    return NPCID.Golem;
                case ItemID.FishronBossBag:
                    return NPCID.DukeFishron;
                case ItemID.FairyQueenBossBag:
                    return NPCID.HallowBoss;
                case ItemID.CultistBossBag://Unobtainable
                    return NPCID.CultistBoss;
                case ItemID.MoonLordBossBag:
                    return NPCID.MoonLordCore;
                case ItemID.BossBagDarkMage://Unobtainable
                    return NPCID.DD2DarkMageT1;
                case ItemID.BossBagOgre://Unobtainable
                    return NPCID.DD2OgreT2;
                case ItemID.BossBagBetsy:
                    return NPCID.DD2Betsy;
                default:
                    return int.MinValue;
            }
        }

        /// <summary>
        /// Only set up for modded bosses/bossbags.
        /// </summary>
        /// <param name="bagName"></param>
        /// <returns>Name of boss npc that drops the boss bag.</returns>
        public static string GetModdedBossNameFromBag(string bagName)
        {
            switch (bagName)
            {
                // \/Calamity contributed by SnarkyEspresso
                case "AquaticScourgeBag":
                    return "Aquatic Scourge";
                case "AstrumAureusBag":
                    return "Astrum Aureus";
                case "AstrumDeusBag":
                    return "Astrum Deus";
                case "BrimstoneWaifuBag":
                    return "Brimstone Elemental";
                case "CalamitasBag":
                    return "Calamitas";
                case "CeaselessVoidBag":
                    return "Ceaseless Void";
                case "CrabulonBag":
                    return "Crabulon";
                case "CryogenBag":
                    return "Cryogen";
                case "DesertScourgeBag":
                    return "Desert Scourge";
                case "DevourerofGodsBag":
                    return "The Devourer of Gods";
                case "DragonfollyBag":
                    return "The Dragonfolly";
                case "DraedonBag":
                    return "XF-09 Ares";
                case "HiveMindBag":
                    return "The Hive Mind";
                case "LeviathanBag":
                    return "The Leviathan";
                case "OldDukeBag":
                    return "The Old Duke";
                case "PerforatorBag":
                    return "The Perforator Hive";
                case "PlaguebringerGoliathBag":
                    return "The Plaguebringer Goliath";
                case "PolterghastBag":
                    return "Polterghast";
                case "ProvidenceBag":
                    return "Providence, the Profaned Goddess";
                case "RavagerBag":
                    return "Ravager";
                case "SignusBag":
                    return "Signus, Envoy of the Devourer";
                case "SlimeGodBag":
                    return "The Slime God";
                case "StormWeaverBag":
                    return "Storm Weaver";
                case "SupremeCalamitasCoffer":
                    return "Supreme Calamitas";
                case "YharonBag":
                    return "Jungle Dragon, Yharon";
                // /\Calamity contributed by SnarkyEspresso

                // \/Fargo's Souls contributed by SnarkyEspresso
                case "TrojanSquirrelBag":
                    return "Trojan Squirrel";
                case "DeviBag":
                    return "Deviantt";
                case "CosmosBag":
                    return "Eridanus, Champion of Cosmos";
                case "AbomBag":
                    return "Abominationn";
                case "MutantBag":
                    return "Mutant";
                // /\Fargo's Souls contributed by SnarkyEspresso

                // \/Stars Above
                case "VagrantBossBag":
                    return "The Vagrant of Space and Time";
                case "NalhaunBossBag":
                    return "Nalhaun, The Burnished King";
                case "PenthBossBag":
                    return "Penthesilea, the Witch of Ink";
                case "ArbitrationBossBag":
                    return "Arbitration";
                case "WarriorBossBag":
                    return "The Warrior Of Light";
                case "":
                    return "Tsukiyomi, the First Starfarer";//No drops?
                // /\Stars Above

                /* Extras for later
                case "":
                    return "";
                case "":
                    return "";
                case "":
                    return "";
                case "":
                    return "";
                case "":
                    return "";
                case "":
                    return "";
                case "":
                    return "";
                case "":
                    return "";
                case "":
                    return "";
                case "":
                    return "";
                case "":
                    return "";
                case "":
                    return "";*/
                default:
                    $"Support for this boss bag: {bagName} has not yet been added.".LogNT();

                    return null;
            }
        }
    }
}
