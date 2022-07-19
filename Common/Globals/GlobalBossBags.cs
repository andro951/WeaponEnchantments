using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using WeaponEnchantments.Items;
using WeaponEnchantments.UI;

namespace WeaponEnchantments.Common.Globals
{
    public class GlobalBossBags : GlobalItem
    {
        public override void OpenVanillaBag(string context, Player player, int arg)
        {
            if (context == "bossBag")
            {
                if(ContentSamples.ItemsByType[arg].ModItem != null)
				{
                    string bagName = ContentSamples.ItemsByType[arg].ModItem.Name;
                    bagName.Log();
                }
                IEntitySource src = player.GetSource_OpenItem(arg);
                NPC npc = GetNPCFromBossBagType(arg);
                if (npc != null)
                {
                    WEGlobalNPC.GetEssenceDropList(npc, out float[] essenceValues, out float[] dropRate, out int baseID, out float hp, out float total);
                    for (int i = 0; i < essenceValues.Length; ++i)
                    {
                        if (dropRate[i] > 0)
                        {
                            switch (npc.type)
                            {
                                case NPCID.EaterofWorldsHead:
                                    dropRate[i] *= 100f;
                                    break;
                                default:

                                    break;
                            }
                            int stack = Main.rand.NextBool() ? (int)Math.Round(dropRate[i]) : (int)Math.Round(dropRate[i] + 1f);
                            player.QuickSpawnItem(src, baseID + i, stack);
                        }
                    }
                    float value = npc.value;
                    switch (npc.type)
                    {
                        case NPCID.EaterofWorldsHead:
                            value *= 100f;
                            break;
                        default:

                            break;
                    }
                    bool canDropPowerBooster = false;
                    int bossType = GetBossTypeFromBag(arg);
                    if (!WEMod.serverConfig.PreventPowerBoosterFromPreHardMode)
                        canDropPowerBooster = true;
					else if (bossType > int.MinValue && !WEGlobalNPC.preHardModeBossTypes.Contains(bossType))
                        canDropPowerBooster = true;
                    else if(npc.ModNPC != null)
                    {
                        string bossName = GetModdedBossNameFromBag(ContentSamples.ItemsByType[arg].ModItem.Name);
                        if (bossName != "" && !WEGlobalNPC.preHardModeModBossNames.Contains(bossName))
                            canDropPowerBooster = true;
                    }
                    if (canDropPowerBooster && Main.rand.NextFloat() < value / 1000000f)
                    {
                        player.QuickSpawnItem(src, ModContent.ItemType<PowerBooster>());
                    }
                    if (Main.rand.NextFloat() < value / 500000f)
                    {
                        player.QuickSpawnItem(src, ModContent.ItemType<SuperiorContainment>());
                    }
                    float chance = WEGlobalNPC.GetDropChance(arg);
                    List<int> itemTypes = WEGlobalNPC.GetDropItems(arg, true);
                    if (itemTypes.Count > 1)
                    {
                        float randFloat = Main.rand.NextFloat();
                        for (int i = 0; i < itemTypes.Count; i++)
                        {
                            if (randFloat >= (float)i / (float)itemTypes.Count * chance && randFloat < ((float)i + 1f) / (float)itemTypes.Count * chance)
                            {
                                player.QuickSpawnItem(src, itemTypes[i]);
                                break;
                            }
                        }
                    }
                    else if (itemTypes.Count > 0)
                    {
                        if (Main.rand.NextFloat() < chance)
                        {
                            player.QuickSpawnItem(src, itemTypes[0]);
                        }
                    }
                }
            }
        }
        public static NPC GetNPCFromBossBagType(int bossBagType)
        {
            int npcID;
            switch (bossBagType)
            {
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
                    npcID = -1000;
                    string bossName = null;
                    if(ContentSamples.ItemsByType[bossBagType].ModItem != null)
					{
                        bossName = GetModdedBossNameFromBag(ContentSamples.ItemsByType[bossBagType].ModItem.Name);
                    }
                    if(bossName != null)
					{
                        for (int i = 0; i < NPCLoader.NPCCount; i++)
                        {
                            NPC sampleNPC = ContentSamples.NpcsByNetId[i];
                            if (sampleNPC.FullName == bossName)
                            {
                                npcID = sampleNPC.netID;
                                break;
                            }
                        }
                        if(npcID == -1000)
						{
                            string error = $"Failed to find this boss name: {(bossName != null ? bossName : "Null")} that dropps this boss bag type: {bossBagType}.\nPlease inform andro951(Weapon Enchantments) including what boss bag you tried to open and what mod it is from.";
                            Main.NewText(error);
                            error.Log();
                        }
                    }
                    break;
            }
            if (npcID != -1000)
            {
                NPC tempNpc = (NPC)ContentSamples.NpcsByNetId[npcID].Clone();
                return tempNpc;
            }
            return null;
        }
        public static int GetBossTypeFromBag(int bagID)
        {
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
        public static string GetModdedBossNameFromBag(string bagName)
        {
            switch (bagName)
            {
                // \/Calamity contributed by SnarkyEspresso
                case "Treasure Bag (Desert Scourge)":
                    return "Desert Scourge";
                case "Treasure Bag (Crabulon)":
                    return "Crabulon";
                case "Treasure Bag (The Hive Mind)":
                    return "The Hive Mind";
                case "Treasure Bag (The Perforators)":
                    return "The Perforator Hive";
                case "Treasure Bag (The Slime God)":
                    return "The Slime God";
                case "Treasure Bag (Cryogen)":
                    return "Cryogen";
                case "Treasure Bag (Aquatic Scourge)":
                    return "Aquatic Scourge";
                case "Treasure Bag (Brimstone Elemental)":
                    return "Brimstone Elemental";
                case "Treasure Bag (Calamitas)":
                    return "Calamitas";
                case "Treasure Bag (Leviathan and Anahita)":
                    return "The Leviathan";
                case "Treasure Bag (Astrum Aureus)":
                    return "Astrum Aureus";
                case "Treasure Bag (The Plaguebringer Goliath)":
                    return "The Plaguebringer Goliath";
                case "Treasure Bag (Ravager)":
                    return "Ravager";
                case "Treasure Bag (Astrum Deus)":
                    return "Astrum Deus";
                case "Treasure Bag (The Dragonfolly)":
                    return "The Dragonfolly";
                case "Treasure Bag (Providence, the Profaned Goddess)":
                    return "Providence, the Profaned Goddess";
                case "Treasure Bag (Ceaseless Void)":
                    return "Ceaseless Void";
                case "Treasure Bag (Storm Weaver)":
                    return "Storm Weaver";
                case "Treasure Bag (Signus, Envoy of the Devourer)":
                    return "Signus, Envoy of the Devourer";
                case "Treasure Bag (Polterghast)":
                    return "Polterghast";
                case "Treasure Bag (The Old Duke)":
                    return "The Old Duke";
                case "Treasure Bag (The Devourer of Gods)":
                    return "The Devourer of Gods";
                case "Treasure Bag (Jungle Dragon, Yharon)":
                    return "Jungle Dragon, Yharon";
                case "Treasure Box (Exo Mechs)":
                    return "XF-09 Ares";
                case "Treasure Coffer (Supreme Calamitas)":
                    return "Supreme Calamitas";
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
                //case "":
                //    return "Tsukiyomi, the First Starfarer";//No drops?
                // /\Stars Above

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
                    string message = $"Support for this boss bag: {bagName} has not yet been added.\nPlease inform andro951(Weapon Enchantments) and include the name of the boss that drops it and which mod it is from.";
                    Main.NewText(message);
                    message.Log();
                    return null;
            }
        }
    }
}
