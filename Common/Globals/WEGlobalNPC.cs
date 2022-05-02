using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Items;
using static WeaponEnchantments.Items.Containment;
using static WeaponEnchantments.Items.EnchantmentEssence;

namespace WeaponEnchantments.Common.Globals
{
    public class WEGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if(!npc.friendly && !npc.townNPC && !npc.SpawnedFromStatue)
            {
                float multiplier = (1f + ((float)((npc.noGravity ? 1f : 0f) + (npc.noTileCollide ? 1f : 0f)) - npc.knockBackResist) / 10f) / (npc.boss ? 2f : 1f); //* (npc.boss ? 1f : 2f);
                float hp = (float)npc.lifeMax * (1f + (float)npc.defDefense + (float)npc.defDamage / 2f) / 40f;
                float value = (float)npc.value;
                float neg = Math.Abs(value - hp) * 0.8f;
                float total = (hp + value - neg) * multiplier;
                float[] essenceValues = new float[] { 100f, 800f, 6400f, 51200f, 409600f };
                float[] dropRate = new float[essenceValues.Length];
                int baseID = ModContent.ItemType<EnchantmentEssenceBasic>();

                int rarity = 0;
                if (npc.boss && (npc.type < NPCID.EaterofWorldsHead || npc.type > NPCID.EaterofWorldsTail))
                {
                    for (int i = 0; i < essenceValues.Length; ++i)
                    {
                        if (total / essenceValues[i] > 1)
                        {
                            rarity = i;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < essenceValues.Length; ++i)
                    {
                        if (total / essenceValues[i] < 0.05)
                        {
                            break;
                        }
                        else
                        {
                            rarity = i;
                        }
                    }
                }
                total *= 2;
                if (rarity == 0)
                {
                    dropRate[rarity] = 1.25f * total / essenceValues[rarity];
                }
                else
                {
                    dropRate[rarity] = total / essenceValues[rarity];
                    dropRate[rarity - 1] = 0.5f * total / essenceValues[rarity];
                }
                if (rarity < 4)
                {
                    dropRate[rarity + 1] = 0.125f * total / essenceValues[rarity];
                }
                for (int i = 0; i < essenceValues.Length; ++i)
                {
                    if (dropRate[i] > 0)
                    {
                        if (npc.boss && (npc.type < NPCID.EaterofWorldsHead || npc.type > NPCID.EaterofWorldsTail))
                        {
                            npcLoot.Add(ItemDropRule.Common(baseID + i, 1, (int)dropRate[i], (int)(dropRate[i] + 1)));
                        }
                        else
                        {
                            int denominator = (int)(1 / dropRate[i]);
                            npcLoot.Add(ItemDropRule.Common(baseID + i, denominator, 1, 1));
                        }
                    }
                }
                if (npc.boss || (npc.type >= NPCID.EaterofWorldsHead && npc.type <= NPCID.EaterofWorldsTail))
                {
                    if(npc.type >= NPCID.EaterofWorldsHead && npc.type <= NPCID.EaterofWorldsTail)
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ContainmentFragment>(), (int)(20000 / 3 / npc.value), 1, 1));
                    }
                    else
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ContainmentFragment>(), 1, (int)(npc.value / 10000), (int)(npc.value / 5000)));
                    }
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SuperiorContainment>(), (int)(500000 / npc.value), 1, 1));
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PowerBooster>(), (int)(1000000 / npc.value), 1, 1));
                }
                /*
                npc.boss;
                npc.buffImmune;
                npc.lifeMax;
                npc.defDamage;
                npc.defDefense;
                    npc.friendly;
                npc.knockBackResist;
                npc.lavaImmune;
                npc.noGravity;
                npc.noTileCollide;
                npc.reflectsProjectiles;
                    npc.SpawnedFromStatue;
                npc.statsAreScaledForThisManyPlayers;
                npc.stepSpeed;
                npc.strengthMultiplier;
                    npc.townNPC;
                npc.trapImmune;
                npc.value;
                npc.width;
                npc.height;
                */
            }
        }
    }
}
