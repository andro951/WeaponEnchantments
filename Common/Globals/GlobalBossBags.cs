﻿using Microsoft.Xna.Framework;
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
                    if (Main.rand.NextFloat() < value / 500000f)
                    {
                        player.QuickSpawnItem(src, ModContent.ItemType<SuperiorContainment>());
                    }
                    if (Main.rand.NextFloat() < value / 1000000f)
                    {
                        player.QuickSpawnItem(src, ModContent.ItemType<PowerBooster>());
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
                    break;
            }
            if (npcID != -1000)
            {
                NPC tempNpc = (NPC)ContentSamples.NpcsByNetId[npcID].Clone();
                return tempNpc;
            }
            return null;
        }
    }
}