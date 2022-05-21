using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
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
        private bool oneForAllOrigin = true;
        public bool immuneToAllForOne = false;
        public double[] timeHitByAllForOne = new double[256];
        public override bool InstancePerEntity => true;
        public static List<int> GetDropItems(int arg, bool bossBag = false)
        {
            List<int> itemTypes = new List<int>(); 
            switch (arg)
            {
                case NPCID.KingSlime when !bossBag:
                case ItemID.KingSlimeBossBag when bossBag:
                    itemTypes.Add(ModContent.ItemType<DamageEnchantmentBasic>());
                    break;
                case NPCID.EyeofCthulhu when !bossBag:
                case ItemID.EyeOfCthulhuBossBag when bossBag:
                    itemTypes.Add(ModContent.ItemType<SizeEnchantmentBasic>());
                    break;
                case NPCID.EaterofWorldsHead when !bossBag:
                case NPCID.EaterofWorldsBody when !bossBag:
                case NPCID.EaterofWorldsTail when !bossBag:
                case ItemID.EaterOfWorldsBossBag when bossBag:
                    itemTypes.Add(ModContent.ItemType<SpeedEnchantmentBasic>());
                    break;
                case NPCID.BrainofCthulhu when !bossBag:
                case ItemID.BrainOfCthulhuBossBag when bossBag:
                    itemTypes.Add(ModContent.ItemType<ManaCostEnchantmentBasic>());
                    break;
                case NPCID.QueenBee when !bossBag:
                case ItemID.QueenBeeBossBag when bossBag:
                    itemTypes.Add(ModContent.ItemType<AmmoCostEnchantmentBasic>());
                    break;
                case NPCID.SkeletronHead when !bossBag:
                case ItemID.SkeletronBossBag when bossBag:
                    itemTypes.Add(ModContent.ItemType<CriticalEnchantmentBasic>());
                    break;
                case NPCID.Deerclops when !bossBag:
                case ItemID.DeerclopsBossBag when bossBag:
                    itemTypes.Add(ModContent.ItemType<DefenceEnchantmentBasic>());
                    break;
                case NPCID.WallofFlesh when !bossBag:
                case ItemID.WallOfFleshBossBag when bossBag:
                    itemTypes.Add(ModContent.ItemType<LifeStealEnchantmentBasic>());
                    itemTypes.Add(ModContent.ItemType<ArmorPenetrationEnchantmentBasic>());
                    break;
                case NPCID.QueenSlimeBoss when !bossBag:
                case ItemID.QueenSlimeBossBag when bossBag:

                    break;
                case NPCID.Retinazer when !bossBag:
                case ItemID.TwinsBossBag when bossBag:
                case NPCID.Spazmatism when !bossBag:

                    break;
                case NPCID.TheDestroyer when !bossBag:
                case ItemID.DestroyerBossBag when bossBag:

                    break;
                case NPCID.SkeletronPrime when !bossBag:
                case ItemID.SkeletronPrimeBossBag when bossBag:

                    break;
                case NPCID.Plantera when !bossBag:
                case ItemID.PlanteraBossBag when bossBag:

                    break;
                case NPCID.Golem when !bossBag:
                case ItemID.GolemBossBag when bossBag:

                    break;
                case NPCID.DukeFishron when !bossBag:
                case ItemID.FishronBossBag when bossBag:

                    break;
                case NPCID.HallowBoss when !bossBag:
                case ItemID.FairyQueenBossBag when bossBag:

                    break;
                case NPCID.CultistBoss when !bossBag:
                case ItemID.CultistBossBag when bossBag:

                    break;
                case NPCID.MoonLordCore when !bossBag:
                case ItemID.MoonLordBossBag when bossBag:

                    break;
                case NPCID.DD2DarkMageT1 when !bossBag:
                case ItemID.BossBagDarkMage when bossBag://Unobtainable

                    break;
                case NPCID.DD2DarkMageT3 when !bossBag:

                    break;
                case NPCID.DD2OgreT2 when !bossBag:
                case ItemID.BossBagOgre when bossBag://Unobtainable

                    break;
                case NPCID.DD2OgreT3 when !bossBag:

                    break;
                case NPCID.DD2Betsy when !bossBag:
                case ItemID.BossBagBetsy when bossBag:

                    break;
                case NPCID.PirateShip when !bossBag:

                    break;
                case NPCID.MourningWood when !bossBag:

                    break;
                case NPCID.Pumpking when !bossBag:

                    break;
                case NPCID.Everscream when !bossBag:

                    break;
                case NPCID.SantaNK1 when !bossBag:

                    break;
                case NPCID.IceQueen when !bossBag:

                    break;
                case NPCID.MartianSaucer when !bossBag:

                    break;
                case NPCID.LunarTowerSolar when !bossBag:

                    break;
                case NPCID.LunarTowerNebula when !bossBag:

                    break;
                case NPCID.LunarTowerVortex when !bossBag:

                    break;
                case NPCID.LunarTowerStardust when !bossBag:

                    break;
                default:
                    itemTypes.Add(0);
                    break;
            }
            return itemTypes;
        }
        public static float GedDropChance(int arg, bool bossBag = false)
        {
            float chance;
            switch (arg)
            {
                case NPCID.WallofFlesh when !bossBag:
                    chance = 1f;
                    break;
                default:
                    chance = 0.5f;
                    break;
            }
            return chance;
        }
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if(!npc.friendly && !npc.townNPC && !npc.SpawnedFromStatue)
            {
                GetEssenceDropList(npc, out float[] essenceValues, out float[] dropRate, out int baseID, out float hp, out float total);
                if(total > 0f)
                {
                    IItemDropRule dropRule;
                    for (int i = 0; i < essenceValues.Length; ++i)
                    {
                        if (dropRate[i] > 0)
                        {
                            if (npc.boss && (npc.type < NPCID.EaterofWorldsHead || npc.type > NPCID.EaterofWorldsTail))
                            {
                                dropRule = new DropBasedOnExpertMode(ItemDropRule.NotScalingWithLuck(baseID + i, 1, (int)Math.Round(dropRate[i]), (int)Math.Round(dropRate[i] + 1f)), ItemDropRule.DropNothing());
                                npcLoot.Add(dropRule);
                            }
                            else
                            {
                                int denominator = (int)Math.Round(1f / dropRate[i]);
                                dropRule = new DropBasedOnExpertMode(ItemDropRule.Common(baseID + i, denominator, 1, 1), ItemDropRule.DropNothing());
                                npcLoot.Add(dropRule);
                            }
                        }
                    }
                    if (npc.boss || (npc.type >= NPCID.EaterofWorldsHead && npc.type <= NPCID.EaterofWorldsTail))
                    {
                        if (npc.type >= NPCID.EaterofWorldsHead && npc.type <= NPCID.EaterofWorldsTail)
                        {
                            dropRule = new DropBasedOnExpertMode(ItemDropRule.Common(ModContent.ItemType<ContainmentFragment>(), (int)(20000 / 3 / npc.value), 1, 1), ItemDropRule.DropNothing());
                            npcLoot.Add(dropRule);
                        }
                        else
                        {
                            dropRule = new DropBasedOnExpertMode(ItemDropRule.Common(ModContent.ItemType<ContainmentFragment>(), 1, (int)(npc.value / 10000), (int)(npc.value / 5000)), ItemDropRule.DropNothing());
                            npcLoot.Add(dropRule);
                        }
                        dropRule = new DropBasedOnExpertMode(ItemDropRule.Common(ModContent.ItemType<SuperiorContainment>(), (int)(500000 / npc.value), 1, 1), ItemDropRule.DropNothing());
                        npcLoot.Add(dropRule);
                        dropRule = new DropBasedOnExpertMode(ItemDropRule.Common(ModContent.ItemType<PowerBooster>(), (int)(1000000 / npc.value), 1, 1), ItemDropRule.DropNothing());
                        npcLoot.Add(dropRule);
                        float chance = WEGlobalNPC.GedDropChance(npc.type);
                        List<int> itemTypes = WEGlobalNPC.GetDropItems(npc.type);
                        if (itemTypes.Count > 1)
                        {
                            dropRule = new DropBasedOnExpertMode(ItemDropRule.OneFromOptions((int)Math.Round(1f / chance), itemTypes.ToArray()), ItemDropRule.DropNothing());
                            npcLoot.Add(dropRule);
                        }
                        else if (itemTypes.Count > 0)
                        {
                            dropRule = new DropBasedOnExpertMode(ItemDropRule.NotScalingWithLuck(itemTypes[0], (int)Math.Round(1f / chance)), ItemDropRule.DropNothing());
                            npcLoot.Add(dropRule);
                        }
                    }
                    else
                    {
                        switch (npc.aiStyle)
                        {
                            case 1://Slime
                                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DamageEnchantmentBasic>(), (int)(500 * hp / (total * 1)), 1, 1));
                                break;
                            case 2://Demon Eye
                                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SizeEnchantmentBasic>(), (int)(500 * hp / (total * 1)), 1, 1));
                                break;
                            case 3://Fighter
                                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DefenceEnchantmentBasic>(), (int)(500 * hp / (total * 1)), 1, 1));
                                break;
                            case 5://Flying
                                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AmmoCostEnchantmentBasic>(), (int)(500 * hp / (total * 1)), 1, 1));
                                break;
                            case 6://Worm
                                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ManaCostEnchantmentBasic>(), (int)(500 * hp / (total * 1)), 1, 1));
                                break;
                            case 8://Caster
                                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ManaCostEnchantmentBasic>(), (int)(500 * hp / (total * 1)), 1, 1));
                                break;
                            case 10://Cursed Skull
                                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ManaCostEnchantmentBasic>(), (int)(500 * hp / (total * 1)), 1, 1));
                                break;
                            case 13://Plant
                                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CriticalEnchantmentBasic>(), (int)(500 * hp / (total * 1)), 1, 1));
                                break;
                            case 14://Bat
                                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SpeedEnchantmentBasic>(), (int)(500 * hp / (total * 1)), 1, 1));
                                break;
                            case 16://Swimming
                                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SpeedEnchantmentBasic>(), (int)(500 * hp / (total * 1)), 1, 1));
                                break;
                            case 17://Vulture
                                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<LifeStealEnchantmentBasic>(), (int)(500 * hp / (total * 1)), 1, 1));
                                break;
                            case 18://Jellyfish
                                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CriticalEnchantmentBasic>(), (int)(500 * hp / (total * 1)), 1, 1));
                                break;
                            case 19://Antlion
                                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CriticalEnchantmentBasic>(), (int)(500 * hp / (total * 1)), 1, 1));
                                break;
                            case 22://Hovering

                                break;
                            case 23://Flying Weapon

                                break;
                            case 25://Mimic
                                int[] options = new int[] 
                                { 
                                    ModContent.ItemType<SpelunkerEnchantmentUltraRare>(),
                                    ModContent.ItemType<DangerSenseEnchantmentUltraRare>(),
                                    ModContent.ItemType<HunterEnchantmentUltraRare>()
                                };
                                npcLoot.Add(ItemDropRule.OneFromOptions(1, options));
                                break;
                            case 26://Unicorn

                                break;
                            case 29://The Hungry - Wall of flesh plant minions
                                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<LifeStealEnchantmentBasic>(), 100, 1, 1));
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
                                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<LifeStealEnchantmentBasic>(), 100, 1, 1));
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
                }
            }
        }
        public static void GetEssenceDropList(NPC npc, out float[] essenceValues, out float[] dropRate, out int baseID, out float hp, out float total)
        {
            float multiplier = (1f + ((float)((npc.noGravity ? 1f : 0f) + (npc.noTileCollide ? 1f : 0f)) - npc.knockBackResist) / 10f) * (npc.boss ? 1f : 2f);
            hp = (float)npc.lifeMax * (1f + (float)npc.defDefense + (float)npc.defDamage / 2f) / 40f;
            float value = (float)npc.value;
            float neg = Math.Abs(value - hp) * 0.8f;
            total = value > 0 ? (hp + value - neg) * multiplier : 0f;
            essenceValues = new float[] { 100f, 800f, 6400f, 51200f, 409600f };
            dropRate = new float[essenceValues.Length];
            baseID = ModContent.ItemType<EnchantmentEssenceBasic>();

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
        }
        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            HitNPC(npc, player, item, ref damage, ref knockback, ref crit, player.direction);
        }
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            Item item = projectile.GetGlobalProjectile<ProjectileEnchantedItem>()?.sourceItem == null ? null : projectile.GetGlobalProjectile<ProjectileEnchantedItem>().sourceItem;
            damage = (int)Math.Round((float)damage * projectile.GetGlobalProjectile<ProjectileEnchantedItem>().minionDamageMultiplier);
            HitNPC(npc, Main.player[projectile.owner], item, ref damage, ref knockback, ref crit, hitDirection, projectile);
        }
        private void HitNPC(NPC npc, Player player, Item item, ref int damage, ref float knockback, ref bool crit, int hitDirection, Projectile projectile = null)
        {
            if (item?.GetGlobalItem<EnchantedItem>() != null)
            {
                sourceItem = item;
                WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
                int total = 0;
                if (sourceItem.GetGlobalItem<EnchantedItem>().oneForAll && oneForAllOrigin)
                {
                    total = ActivateOneForAll(npc, player, item, ref damage, ref knockback, ref crit, hitDirection, projectile);
                }
                if (sourceItem.GetGlobalItem<EnchantedItem>().lifeSteal > 0f || wePlayer.lifeSteal > 0f)
                {
                    float lifeSteal = sourceItem.GetGlobalItem<EnchantedItem>().lifeSteal + wePlayer.lifeSteal;
                    Vector2 speed = new Vector2(0, 0);
                    float healTotal = (damage + total) * lifeSteal + wePlayer.lifeStealRollover;
                    int heal = (int)healTotal;
                    if (player.statLife < player.statLifeMax2)
                    {
                        if (heal > 0)
                        {
                            Projectile.NewProjectile(sourceItem.GetSource_ItemUse(sourceItem), npc.Center, speed, ProjectileID.VampireHeal, 0, 0f, player.whoAmI, player.whoAmI, heal);
                        }
                        wePlayer.lifeStealRollover = healTotal - heal;
                    }
                    else
                    {
                        wePlayer.lifeStealRollover = 0f;
                    }
                }
                if(sourceItem.GetGlobalItem<EnchantedItem>().godSlayerBonus > 0f)
                {
                    ActivateGodSlayer(npc, player, item, ref damage, ref knockback, ref crit, hitDirection, projectile);
                }
                if (sourceItem.GetGlobalItem<EnchantedItem>().oneForAll && oneForAllOrigin && projectile != null)
                {
                    projectile.Kill();
                }
            }
            /*if (projectile.GetGlobalProjectile<ProjectileEnchantedItem>()?.sourceItem != null)
            {
                sourceItem = projectile.GetGlobalProjectile<ProjectileEnchantedItem>().sourceItem;
            }
            if (sourceItem != null)
            {
                if (sourceItem.GetGlobalItem<EnchantedItem>().allForOne)
                {
                    immuneToAllForOne = true;
                    timeHitByAllForOne[projectile.owner] = Main.GameUpdateCount;
                }
                damage = (int)Math.Round((float)damage * projectile.GetGlobalProjectile<ProjectileEnchantedItem>().minionDamageMultiplier);
                int total = 0;
                if (sourceItem.GetGlobalItem<EnchantedItem>().oneForAll && oneForAllOrigin)
                {
                    total = ActivateOneForAllProjectile(npc, projectile, ref damage, ref knockback, ref crit, ref hitDirection, sourceItem);
                }
                WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
                if (sourceItem.GetGlobalItem<EnchantedItem>().lifeSteal > 0f || wePlayer.lifeSteal > 0f)
                {
                    float lifeSteal = sourceItem.GetGlobalItem<EnchantedItem>().lifeSteal + wePlayer.lifeSteal;
                    Vector2 speed = new Vector2(0, 0);
                    float healTotal = (damage + total) * lifeSteal + wePlayer.lifeStealRollover;
                    int heal = (int)healTotal;
                    if (wePlayer.Player.statLife < wePlayer.Player.statLifeMax2)
                    {
                        if (heal > 0)
                        {
                            Projectile.NewProjectile(sourceItem.GetSource_ItemUse(sourceItem), npc.Center, speed, ProjectileID.VampireHeal, 0, 0f, projectile.owner, projectile.owner, heal);
                        }
                        wePlayer.lifeStealRollover = healTotal - heal;
                    }
                    else
                    {
                        wePlayer.lifeStealRollover = 0f;
                    }
                }
                if (sourceItem.GetGlobalItem<EnchantedItem>().oneForAll && oneForAllOrigin)
                {
                    //ActivateOneForAllProjectile(npc, projectile, ref damage, ref knockback, ref crit, ref hitDirection, sourceItem);
                    projectile.Kill();
                }
            }*/
        }
        public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile)
        {
            if(!npc.townNPC && !npc.friendly)
            {
                if (projectile.GetGlobalProjectile<ProjectileEnchantedItem>()?.sourceItem != null)
                {
                    sourceItem = projectile.GetGlobalProjectile<ProjectileEnchantedItem>().sourceItem;
                }
                if (sourceItem != null)
                {
                    if (sourceItem.GetGlobalItem<EnchantedItem>().allForOne)
                    {
                        if (npc.GetGlobalNPC<WEGlobalNPC>().immuneToAllForOne)
                        {
                            if (timeHitByAllForOne[projectile.owner] + 80 > Main.GameUpdateCount)
                            {
                                return false;
                            }
                            else
                            {
                                timeHitByAllForOne[projectile.owner] = 0;
                                bool noPlayersLeft = true;
                                for (int j = 0; j < timeHitByAllForOne.Length; j++)
                                {
                                    if (timeHitByAllForOne[j] > 0)
                                    {
                                        noPlayersLeft = false;
                                        break;
                                    }
                                }
                                if (noPlayersLeft)
                                {
                                    immuneToAllForOne = false;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }
        private int ActivateOneForAll(NPC npc, Player player, Item item, ref int damage, ref float knockback, ref bool crit, int direction, Projectile projectile = null)
        {
            int total = 0;
            foreach (NPC target in Main.npc)
            {
                if (target.whoAmI != npc.whoAmI)
                {
                    if (!target.friendly && !target.townNPC)
                    {
                        Vector2 vector2 = target.Center - npc.Center;
                        if (vector2.Length() <= 192f * item.scale)
                        {
                            target.GetGlobalNPC<WEGlobalNPC>().oneForAllOrigin = false;
                            target.GetGlobalNPC<WEGlobalNPC>().sourceItem = sourceItem;
                            int allForOneDamage = (int)((float)damage * item.GetGlobalItem<EnchantedItem>().oneForAllBonus);
                            total += (int)target.StrikeNPC(allForOneDamage, knockback, direction);
                            /*if(projectile != null)
                            {
                                target.GetGlobalNPC<WEGlobalNPC>().ModifyHitByProjectile(target, projectile, ref damage, ref knockback, ref crit, ref hitDirection);
                            }
                            else
                            {
                                target.GetGlobalNPC<WEGlobalNPC>().ModifyHitByItem(target, player, item, ref damage, ref knockback, ref crit);
                            }*/
                            target.GetGlobalNPC<WEGlobalNPC>().oneForAllOrigin = true;
                        }
                    }
                }
            }
            return total;
        }
        private void ActivateGodSlayer(NPC npc, Player player, Item item, ref int damage, ref float knockback, ref bool crit, int direction, Projectile projectile = null)
        {
            if (!npc.friendly && !npc.townNPC)
            {
                float godSlayerBonus = npc.boss ? item.GetGlobalItem<EnchantedItem>().godSlayerBonus / 10f : item.GetGlobalItem<EnchantedItem>().godSlayerBonus;
                int godSlayerDamage = (int)((float)(damage + (npc.defDefense - player.GetWeaponArmorPenetration(item) / 2)) / 100f * (godSlayerBonus * npc.lifeMax));
                npc.StrikeNPC(godSlayerDamage, knockback, direction);
            }
        }
        public override void OnKill(NPC npc)
        {
            if(!xpCalculated && sourceItem != null)
            {
                sourceItem.GetGlobalItem<EnchantedItem>().KillNPC(sourceItem, npc);
            }
        }
        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            WEPlayer wePlayer = player.GetModPlayer<WEPlayer>();
            float enemySpawnBonus = wePlayer.enemySpawnBonus;
            int rate = (int)(spawnRate / enemySpawnBonus);
            spawnRate = rate < 1 ? 1 : rate;
            int spawns = (int)Math.Round(maxSpawns * enemySpawnBonus);
            maxSpawns = spawns >= 0 ? spawns : 0;
        }
    }
}
