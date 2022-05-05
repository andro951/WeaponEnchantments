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
using static WeaponEnchantments.Items.Enchantments;

namespace WeaponEnchantments.Common.Globals
{
    public class WEGlobalNPC : GlobalNPC
    {
        public Item sourceItem;
        public bool xpCalculated = false;
        public override bool InstancePerEntity => true;
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if(!npc.friendly && !npc.townNPC && !npc.SpawnedFromStatue)
            {
                float multiplier = (1f + ((float)((npc.noGravity ? 1f : 0f) + (npc.noTileCollide ? 1f : 0f)) - npc.knockBackResist) / 10f) * (npc.boss ? 1f : 2f);
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
                //total *= 2;
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
                    dropRate[rarity + 1] = 0.06125f * total / essenceValues[rarity];
                }
                for (int i = 0; i < essenceValues.Length; ++i)
                {
                    if (dropRate[i] > 0)
                    {
                        if (npc.boss && (npc.type < NPCID.EaterofWorldsHead || npc.type > NPCID.EaterofWorldsTail))
                        {
                            npcLoot.Add(ItemDropRule.Common(baseID + i, 1, (int)Math.Round(dropRate[i]), (int)Math.Round(dropRate[i] + 1f)));
                        }
                        else
                        {
                            int denominator = (int)Math.Round(1f / dropRate[i]);
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
                else
                {
                    switch (npc.aiStyle)
                    {
                        case 1://Slime
                            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DamageEnchantmentBasic>(), (int)(1000 * hp /(total * 1)), 1, 1));
                            break;
                        case 2://Demon Eye
                            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SizeEnchantmentBasic>(), (int)(1000 * hp / (total * 1)), 1, 1));
                            break;
                        case 3://Fighter
                            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DefenceEnchantmentBasic>(), (int)(1000 * hp / (total * 1)), 1, 1));
                            break;
                        case 5://Flying
                            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AmmoCostEnchantmentBasic>(), (int)(1000 * hp / (total * 1)), 1, 1));
                            break;
                        case 6://Worm

                            break;
                        case 8://Caster
                            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ManaCostEnchantmentBasic>(), (int)(1000 * hp / (total * 1)), 1, 1));
                            break;
                        case 10://Cursed Skull
                            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ManaCostEnchantmentBasic>(), (int)(1000 * hp / (total * 1)), 1, 1));
                            break;
                        case 13://Plant
                            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CriticalEnchantmentBasic>(), (int)(1000 * hp / (total * 1)), 1, 1));
                            break;
                        case 14://Bat
                            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SpeedEnchantmentBasic>(), (int)(1000 * hp / (total * 1)), 1, 1));
                            break;
                        case 16://Swimming

                            break;
                        case 17://Vulture

                            break;
                        case 18://Jellyfish

                            break;
                        case 19://Antlion
                            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CriticalEnchantmentBasic>(), (int)(1000 * hp / (total * 1)), 1, 1));
                            break;
                        case 22://Hovering

                            break;
                        case 23://Flying Weapon

                            break;
                        case 25://Mimic
                            //100%
                            break;
                        case 26://Unicorn

                            break;
                        case 29://The Hungry - Wall of flesh plant minions

                            break;
                        case 38://Snowman

                            break;
                        case 39://Tortoise

                            break;
                        case 40://Spider

                            break;
                        case 41://Derpling - Blue jungle bug

                            break;
                        case 44://Flying Fish

                            break;
                        case 49://Angry Nimbus

                            break;
                        case 55://Creeper Brain of Cthulhu minions

                            break;
                        case 56://Dungeon Spirit - 1 hit kill dungion skulls

                            break;
                        case 62://Elf Copter

                            break;
                        case 63://Flocko

                            break;
                        case 71://Sharkron - Duke Fishron minions

                            break;
                        case 73://Tesla Turret

                            break;
                        case 74://Corite/Martian Drone

                            break;
                        case 75://Rider

                            break;
                        case 80://Martian Probe - Spawns martian invasion
                            //100% dropchance
                            break;
                        case 82://Moon Leach Clot - Moon lord minion

                            break;
                        case 83://Lunatic Devote - Spawn Lunatic Cultist

                            break;
                        case 85://Star Cell/Brain Sucker

                            break;
                        case 87://Biome Mimic - Big Mimics
                            //100%
                            break;
                        case 88://Mothron - Solar Eclipse

                            break;
                        case 91://Granite Elemental

                            break;
                        case 94://Celestial Pillar
                            //High chance unique based on type
                            break;
                        case 95://Small Star Cell

                            break;
                        case 96://Flow Invader

                            break;
                        case 97://Nebula Floater

                            break;
                        case 98://Unknown? If the player is far, rolls quickly towards it, else approaches slowly before shooting Solar Flares.

                            break;
                        case 102://Sand Elemental
                            //High
                            break;
                        case 103://Sand Shark

                            break;
                        case 107://Attacker - Eternia Crystal event

                            break;
                        case 108://Flying Attacker

                            break;
                        case 109://Dark Mage

                            break;
                        case 111://Etherian Lightning Bug

                            break;
                        case 119://Angry Dandelion

                            break;
                        case 120://Pirate's Curse

                            break;
                        default:

                            break;
                    }
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

        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            if(item.GetGlobalItem<EnchantedItem>() != null)
            {
                sourceItem = item;
            }
        }
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (projectile.GetGlobalProjectile<ProjectileEnchantedItem>()?.sourceItem != null)
            {
                sourceItem = projectile.GetGlobalProjectile<ProjectileEnchantedItem>().sourceItem;
            }
        }

        public override void OnKill(NPC npc)
        {
            if(!xpCalculated && sourceItem != null)
            {
                sourceItem.GetGlobalItem<EnchantedItem>().KillNPC(sourceItem, npc);
            }
        }
    }
}
