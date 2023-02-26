﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static WeaponEnchantments.Common.Globals.WEGlobalNPC;
using System.Collections.Generic;
using WeaponEnchantments.Common.Utility;
using System.Diagnostics;

namespace WeaponEnchantments.Common.Globals
{
    public class GlobalBossBags : GlobalItem
    {
        public static bool modBossBagIntegrationSetup = false;
        public static bool printNPCNameOnHitForBossBagSupport => false && Debugger.IsAttached;
        public static SortedDictionary<int, List<int>> bossBagNPCIDs = new SortedDictionary<int, List<int>>() {
            { ItemID.KingSlimeBossBag, new() { NPCID.KingSlime } },
            { ItemID.EyeOfCthulhuBossBag, new() { NPCID.EyeofCthulhu } },
            { ItemID.EaterOfWorldsBossBag, new() { NPCID.EaterofWorldsHead } },
            { ItemID.BrainOfCthulhuBossBag, new() { NPCID.BrainofCthulhu } },
            { ItemID.QueenBeeBossBag, new() { NPCID.QueenBee } },
            { ItemID.SkeletronBossBag, new() { NPCID.SkeletronHead } },
            { ItemID.DeerclopsBossBag, new() { NPCID.Deerclops } },
            { ItemID.WallOfFleshBossBag, new() { NPCID.WallofFlesh } },
            { ItemID.QueenSlimeBossBag, new() { NPCID.QueenSlimeBoss } },
            { ItemID.TwinsBossBag, new() { NPCID.Retinazer } },
            { ItemID.DestroyerBossBag, new() { NPCID.TheDestroyer } },
            { ItemID.SkeletronPrimeBossBag, new() { NPCID.SkeletronPrime } },
            { ItemID.PlanteraBossBag, new() { NPCID.Plantera } },
            { ItemID.GolemBossBag, new() { NPCID.Golem } },
            { ItemID.FishronBossBag, new() { NPCID.DukeFishron } },
            { ItemID.FairyQueenBossBag, new() { NPCID.HallowBoss } },
            { ItemID.CultistBossBag/*Unobtainable*/, new() { NPCID.CultistBoss } },
            { ItemID.MoonLordBossBag, new() { NPCID.MoonLordCore } },
            { ItemID.BossBagDarkMage/*Unobtainable*/, new() { NPCID.DD2DarkMageT1 } },
            { ItemID.BossBagOgre/*Unobtainable*/, new() { NPCID.DD2OgreT2 } },
            { ItemID.BossBagBetsy, new() { NPCID.DD2Betsy } }
		};
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot) {

            int type = item.type;

            if (!bossBagNPCIDs.ContainsKey(type))
				return;

            List<int> npcTypes = bossBagNPCIDs[type];
            foreach(int npcType in npcTypes) {
				NPC npc = ContentSamples.NpcsByNetId[npcType];

				#region Debug

				if (LogMethods.debugging && item.ModItem != null) {
					string bagName = item.ModItem.Name;
					bagName.Log();
				}

				#endregion

				GetLoot(itemLoot, npc, true);
			}
        }
        public static void SetupModBossBagIntegration() {
            SortedDictionary<string, int> supportedNPCsThatDropBags = new SortedDictionary<string, int>();
            //Check if each modded item has a boss bag set up in GetModdedBossNameFromBag()
            for(int i = ItemID.Count; i < ItemLoader.ItemCount; i++) {
                Item sampleItem = ContentSamples.ItemsByType[i];
                bool itemIsBossBag = ItemID.Sets.BossBag[i];

                /* Normal code for after Querty's is fixed
                if (!itemIsBossBag)
                    continue;

                string bossName = GetModdedBossNameFromBag(item.ModItem.Name);
                if(bossName == null) {
                    $"Support for this boss bag: {item.S()} has not yet been added.".Log();
                    continue;
                }
                */

                //Fix for a mod by bsicaly im a cat (aka FoxXD_) causing old boss bags to turn into other items -> no mod item
                if (sampleItem.ModItem == null)
                    continue;

                // \/ Fix for Querty's bags not included in BossBag item sets
                string bossName = GetModdedBossNameFromBag(sampleItem.ModItem.Name);

                if (itemIsBossBag) {
                    if(bossName == null) {
                        $"Support for this boss bag: {sampleItem.S()}, ModItem: {sampleItem.ModItem.Name} has not yet been added Mod: {sampleItem.ModItem.Mod.Name}.".LogNT(ChatMessagesIDs.BossBagNameNull);
                        continue;
                    }
                }
                else {
                    if (bossName == null)
                        continue;
				}
                // /\ Fix for Querty's bags not included in BossBag item sets

                supportedNPCsThatDropBags.Add(bossName, sampleItem.type);
            }

            //Find the boss that drops the modded boss bag
            for (int i = 0; i < NPCLoader.NPCCount; i++) {
                NPC sampleNPC = ContentSamples.NpcsByNetId[i];
                string sampleNPCName = sampleNPC.FullName();
				if (supportedNPCsThatDropBags.ContainsKey(sampleNPCName)) {
                    int bagType = supportedNPCsThatDropBags[sampleNPCName];
                    if (!bossBagNPCIDs.ContainsKey(bagType)) {
						bossBagNPCIDs.Add(bagType, new() { sampleNPC.netID });
					}
                    else {
                        bossBagNPCIDs[bagType].Add(sampleNPC.netID);
                    }
                }
            }
        }

        /// <summary>
        /// Finds the boss type based on a boss bag type.<br/>
        /// </summary>
        /// <param name="bossBagType"></param>
        /// <returns>new NPC of the associated boss bag.</returns>
        public static NPC GetVanillaNPCFromBossBagType(int bossBagType) {
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
                    return null;
            }

            return (NPC)ContentSamples.NpcsByNetId[npcID].Clone();
        }

        /// <summary>
        /// Finds the boss NPCID based on a boss bag type.  Only set up for vanilla bosses.<br/>
        /// </summary>
        /// <param name="bagID"></param>
        /// <returns>Boss NPCID of the associated boss bag.</returns>
        public static int GetBossTypeFromBag(int bagID) {
            switch (bagID) {
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
        public static string GetModdedBossNameFromBag(string bagName) {
            switch (bagName) {
                // \/Calamity contributed by SnarkyEspresso
                case "AquaticScourgeBag":
                    return "Aquatic Scourge";
                case "AstrumAureusBag":
                    return "Astrum Aureus";
                case "AstrumDeusBag":
                    return "Astrum Deus";
                case "BrimstoneWaifuBag":
                    return "Brimstone Elemental";
                case "CalamitasCloneBag":
                    return "Calamitas Clone";
				case "CalamitasCoffer":
					return "Supreme Witch, Calamitas";
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
				case "DraedonBag":
					return "XF-09 Ares";
				case "DragonfollyBag":
                    return "The Dragonfolly";
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

                // \/Vitality
                case "AnarchulesBeetleBossBag":
                    return "Anarchules Beetle";
                case "ChaosbringerBossBag":
                    return "Chaosbringer";
                case "DreadnaughtBossBag":
                    return "Dreadnaught";
                case "GemstoneElementalBossBag":
                    return "Gemstone Elemental";
                case "GrandAntlionBossBag":
                    return "The Grand Antlion";
                case "MoonlightDragonflyBossBag":
                    return "Moonlight Dragonfly";
                case "PaladinSpiritBossBag":
                    return "Paladin Spirit";
                case "StormCloudBossBag":
                    return "The Storm Cloud";
                // /\Vitality

                // \/ Querty's Bosses and Items 2
                case "AncientMachineBag":
                    return "Ancient Machine";
                case "B4Bag":
                    return "Oversized Laser-emitting Obliteration Radiation-emitting Destroyer";
                case "BladeBossBag":
                    return "Imperious";
                case "FortressBossBag":
                    return "The Divine Light";
                case "HydraBag":
                    return "Hydra Head";
                case "NoehtnapBag":
                    return "Noehtnap";
                case "RuneGhostBag":
                    return "Rune Ghost";
                case "TundraBossBag":
                    return "Polar Exterminator";
                // /\ Querty's Bosses and Items 2


                // \/ StormDiversMod
                case "StormBossBag":
                    return "Overloaded Scandrone";
				// \/ StormDiversMod


				// \/ ThoriumMod
				case "AbyssionBag":
					return "Forgotten One";
				case "CultistBag":
					return "Lunatic Cultist";
				case "BoreanBag":
					return "Borean Strider";
				case "DarkMageBag":
					return "Dark Mage";
				case "OgreBag":
					return "Ogre";
				case "RagBag":
					return "Slag Fury, the First Flame";
				case "GraniteBag":
					return "Granite Energy Storm";
				case "BeholderBag":
					return "Fallen Beholder";
				case "HeroBag":
					return "Buried Champion";
				case "LichBag":
					return "Lich";
				case "FlyingDutchmanBag":
					return "Flying Dutchman";
				case "MartianSaucerBag":
					return "Martian Saucer";
				case "JellyFishBag":
					return "Queen Jellyfish";
				case "ScouterBag":
					return "Star Scouter";
				case "ThunderBirdBag":
					return "The Grand Thunder Bird";
				case "CountBag":
					return "Viscount";
				// \/ ThoriumMod



				//Extras for later
				/*case "":
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
                    return null;
            }
        }
    }
}
