using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Items;

namespace WeaponEnchantments.Common.Globals
{
    public class WEGlobalNPC : GlobalNPC
    {
        public Item sourceItem;
        public bool xpCalculated = false;
        private bool oneForAllOrigin = true;
        public bool immuneToAllForOne = false;
        float baseOneForAllRange = 240f;
        static bool war = false;
        static int warReduction = 1;
        int myWarReduction = 1;
        //public double[] timeHitByAllForOne = new double[256];
        public override bool InstancePerEntity => true;
        public override void Load()
        {
            IL.Terraria.Projectile.Damage += HookDamage;
        }
        private static bool debuggingHookDamage = false;
        private static void HookDamage(ILContext il)
        {
            var c = new ILCursor(il);

            if (!c.TryGotoNext(MoveType.After,
                i => i.MatchLdloc(36),
                i => i.MatchBrfalse(out _),
                i => i.MatchLdarg(0),
                i => i.MatchCall(out _),
                i => i.MatchCallvirt(out _)
            )) { throw new Exception("Failed to find instructions HookDamage"); }

            if (debuggingHookDamage) try { ModContent.GetInstance<WEMod>().Logger.Info("c.Index: " + c.Index.ToString() + " Instruction: " + c.Next.ToString()); } catch (Exception e) { ModContent.GetInstance<WEMod>().Logger.Info("c.Index: " + c.Index.ToString() + " exception: " + e.ToString()); }

            if (debuggingHookDamage) try { ModContent.GetInstance<WEMod>().Logger.Info("c.Index: " + (c.Index - 1).ToString() + " Instruction: " + c.Prev.ToString()); } catch (Exception e) { ModContent.GetInstance<WEMod>().Logger.Info("c.Index: " + (c.Index - 1).ToString() + " exception: " + e.ToString()); }
            c.Emit(OpCodes.Pop);
            c.Emit(OpCodes.Ldc_I4_0);
        }
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
                    itemTypes.Add(ModContent.ItemType<ScaleEnchantmentBasic>());
                    break;
                case NPCID.EaterofWorldsHead when !bossBag:
                case NPCID.EaterofWorldsBody when !bossBag:
                case NPCID.EaterofWorldsTail when !bossBag:
                case ItemID.EaterOfWorldsBossBag when bossBag:
                    itemTypes.Add(ModContent.ItemType<SpeedEnchantmentBasic>());
                    break;
                case NPCID.BrainofCthulhu when !bossBag:
                case ItemID.BrainOfCthulhuBossBag when bossBag:
                    itemTypes.Add(ModContent.ItemType<ManaEnchantmentBasic>());
                    break;
                case NPCID.QueenBee when !bossBag:
                case ItemID.QueenBeeBossBag when bossBag:
                    itemTypes.Add(ModContent.ItemType<AmmoCostEnchantmentBasic>());
                    break;
                case NPCID.SkeletronHead when !bossBag:
                case ItemID.SkeletronBossBag when bossBag:
                    itemTypes.Add(ModContent.ItemType<CriticalStrikeChanceEnchantmentBasic>());
                    break;
                case NPCID.Deerclops when !bossBag:
                case ItemID.DeerclopsBossBag when bossBag:
                    itemTypes.Add(ModContent.ItemType<StatDefenseEnchantmentBasic>());
                    break;
                case NPCID.WallofFlesh when !bossBag:
                case ItemID.WallOfFleshBossBag when bossBag:
                    itemTypes.Add(ModContent.ItemType<LifeStealEnchantmentBasic>());
                    itemTypes.Add(ModContent.ItemType<ArmorPenetrationEnchantmentBasic>());
                    break;
                case NPCID.QueenSlimeBoss when !bossBag:
                case ItemID.QueenSlimeBossBag when bossBag:
                    itemTypes.Add(ModContent.ItemType<HellsWrathEnchantmentBasic>());
                    break;
                case NPCID.Retinazer when !bossBag:
                case ItemID.TwinsBossBag when bossBag:
                case NPCID.Spazmatism when !bossBag:
                    itemTypes.Add(ModContent.ItemType<ColdSteelEnchantmentBasic>());
                    break;
                case NPCID.TheDestroyer when !bossBag:
                case ItemID.DestroyerBossBag when bossBag:

                    break;
                case NPCID.SkeletronPrime when !bossBag:
                case ItemID.SkeletronPrimeBossBag when bossBag:

                    break;
                case NPCID.Plantera when !bossBag:
                case ItemID.PlanteraBossBag when bossBag:
                    itemTypes.Add(ModContent.ItemType<JunglesFuryEnchantmentBasic>());
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
                    itemTypes.Add(ModContent.ItemType<MoonlightEnchantmentBasic>());
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
        public static float GetDropChance(int arg, bool bossBag = false)
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
                            else if (npc.boss)
                            {
                                int denominator = (int)Math.Round(1f / dropRate[i]);
                                dropRule = new DropBasedOnExpertMode(ItemDropRule.Common(baseID + i, denominator, 1, 1), ItemDropRule.DropNothing());
                                npcLoot.Add(dropRule);
                            }
                            else
                            {
                                int denominator = (int)Math.Round(1f / dropRate[i]);
                                dropRule = ItemDropRule.Common(baseID + i, denominator, 1, 1);
                                npcLoot.Add(dropRule);
                            }
                        }
                    }
                    if (npc.boss || (npc.type >= NPCID.EaterofWorldsHead && npc.type <= NPCID.EaterofWorldsTail))
                    {
                        dropRule = new DropBasedOnExpertMode(ItemDropRule.Common(ModContent.ItemType<SuperiorContainment>(), (int)(500000 / npc.value), 1, 1), ItemDropRule.DropNothing());
                        npcLoot.Add(dropRule);
                        dropRule = new DropBasedOnExpertMode(ItemDropRule.Common(ModContent.ItemType<PowerBooster>(), (int)(1000000 / npc.value), 1, 1), ItemDropRule.DropNothing());
                        npcLoot.Add(dropRule);
                        float chance = GetDropChance(npc.type);
                        if (npc.type >= NPCID.EaterofWorldsHead && npc.type <= NPCID.EaterofWorldsTail)
                            chance /= 100f;
                        List<int> itemTypes = GetDropItems(npc.type);
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
                                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ScaleEnchantmentBasic>(), (int)(500 * hp / (total * 1)), 1, 1));
                                break;
                            case 3://Fighter
                                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<StatDefenseEnchantmentBasic>(), (int)(500 * hp / (total * 1)), 1, 1));
                                break;
                            case 5://Flying
                                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AmmoCostEnchantmentBasic>(), (int)(500 * hp / (total * 1)), 1, 1));
                                break;
                            case 6://Worm
                                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ManaEnchantmentBasic>(), (int)(500 * hp / (total * 1)), 1, 1));
                                break;
                            case 8://Caster
                                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ManaEnchantmentBasic>(), (int)(500 * hp / (total * 1)), 1, 1));
                                break;
                            case 10://Cursed Skull
                                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ManaEnchantmentBasic>(), (int)(500 * hp / (total * 1)), 1, 1));
                                break;
                            case 13://Plant
                                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CriticalStrikeChanceEnchantmentBasic>(), (int)(500 * hp / (total * 1)), 1, 1));
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
                                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CriticalStrikeChanceEnchantmentBasic>(), (int)(500 * hp / (total * 1)), 1, 1));
                                break;
                            case 19://Antlion
                                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CriticalStrikeChanceEnchantmentBasic>(), (int)(500 * hp / (total * 1)), 1, 1));
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
                                    ModContent.ItemType<HunterEnchantmentUltraRare>(),
                                    ModContent.ItemType<ObsidianSkinEnchantmentUltraRare>()
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
                                int[] optionsBiomeMimic = new int[]
                                {
                                    ModContent.ItemType<GodSlayerEnchantmentBasic>(),
                                    ModContent.ItemType<SplittingEnchantmentBasic>()
                                };
                                npcLoot.Add(ItemDropRule.OneFromOptions(1, optionsBiomeMimic));
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
                        switch (npc.type)
                        {
                            case NPCID.Pixie://75
                                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PeaceEnchantmentBasic>(), (int)(500 * hp / (total * 1)), 1, 1));
                                break;
                            case NPCID.Mothron://477
                                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<OneForAllEnchantmentBasic>(), (int)(500 * hp / (total * 1)), 1, 1));
                                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AllForOneEnchantmentBasic>(), (int)(500 * hp / (total * 1)), 1, 1));
                                break;
                            case NPCID.PirateShip://491
                                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<WarEnchantmentBasic>(), (int)(500 * hp / (total * 1)), 1, 1));
                                break;
                        }
                    }
                }
            }
        }
        public static void GetEssenceDropList(NPC npc, out float[] essenceValues, out float[] dropRate, out int baseID, out float hp, out float total)
        {
            float multiplier = (1f + ((float)((npc.noGravity ? 1f : 0f) + (npc.noTileCollide ? 1f : 0f)) - npc.knockBackResist) / 10f) * (npc.boss ? 1f : 4f);
            hp = (float)npc.lifeMax * (1f + (float)npc.defDefense + (float)npc.defDamage / 2f) / 40f;
            float value = (float)npc.value;
            total = value > 0 ? (hp + 0.2f * value) * multiplier : hp * 2.6f;
            total /= UtilityMethods.GetReductionFactor((int)hp);
            essenceValues = EnchantmentEssenceBasic.values;
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
            //($"\\/ModifyHitByItem(npc: {npc.FullName}, player: {player.S()}, item: {item.S()}, damage: {damage}, knockback: {knockback}, crit: {crit})").Log();
            HitNPC(npc, player, item, ref damage, ref knockback, ref crit, player.direction);
            //($"/\\ModifyHitByItem(npc: {npc.FullName}, player: {player.S()}, item: {item.S()}, damage: {damage}, knockback: {knockback}, crit: {crit})").Log();
        }
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            //($"\\/ModifyHitByProjectile(npc: {npc.FullName}, projectile: {projectile.S()}, damage: {damage}, knockback: {knockback}, crit: {crit})").Log();
            Item item = projectile.GetGlobalProjectile<ProjectileEnchantedItem>()?.sourceItem == null ? null : projectile.GetGlobalProjectile<ProjectileEnchantedItem>().sourceItem;
            damage = (int)Math.Round((float)damage * projectile.GetGlobalProjectile<ProjectileEnchantedItem>().damageBonus);
            HitNPC(npc, Main.player[projectile.owner], item, ref damage, ref knockback, ref crit, hitDirection, projectile);
            //($"/\\ModifyHitByProjectile(npc: {npc.FullName}, projectile: {projectile.S()}, damage: {damage}, knockback: {knockback}, crit: {crit})").Log();
        }
        private void HitNPC(NPC npc, Player player, Item item, ref int damage, ref float knockback, ref bool crit, int hitDirection, Projectile projectile = null)
        {
            //($"\\/HitNPC(npc: {npc.FullName}, player: {player.S()}, item: {item.S()}, damage: {damage}, knockback: {knockback}, crit: {crit}, hitDirection: {hitDirection}, projectile: {projectile.S()})").Log();
            if (item?.GetGlobalItem<EnchantedItem>() != null)
            {
                sourceItem = item;
                if (sourceItem.IsAir)
                    sourceItem = null;
                if(sourceItem != null)
                {
                    int baseDamage = ContentSamples.ItemsByType[item.type].damage;
                    int damageReduction = npc.defense / 2 - npc.checkArmorPenetration(player.GetWeaponArmorPenetration(item));
                    if (damageReduction >= damage)
                        damageReduction = damage - 1;
                    damage -= damageReduction;
                    float temp2 = player.AEP("Damage", 1f);
                    damage = (int)Math.Round(item.AEP("Damage", (float)damage));
                    int critChance = player.GetWeaponCrit(item) + (crit ? 100 : 0);
                    int critLevel = 0;
                    crit = false;
                    while(critChance > 100)
                    {
                        critLevel++;
                        critChance -= 100;
                    }//FirstCritLevel
                    if (Main.rand.Next(0, 100) < critChance)
                        critLevel++;
                    if(critLevel > 0)
                    {
                        crit = true;
                        critLevel--;
                        damage *= (int)Math.Pow(2, critLevel);
                    }//MultipleCritlevels
                    damage += damageReduction;
                    WEPlayer wePlayer = player.GetModPlayer<WEPlayer>();
                    int total = 0;
                    foreach(int debuff in item.G().debuffs.Keys)
                    {
                        npc.AddBuff(debuff, 300);
                    }//AddDebuffs
                    //($"sourceItem: {sourceItem.S()} {sourceItem.G().eStats.S("OneForAll")}").Log();
                    if (sourceItem.G().eStats.ContainsKey("OneForAll") && oneForAllOrigin)
                    {
                        total = ActivateOneForAll(npc, player, item, ref damage, ref knockback, ref crit, hitDirection, projectile);
                    }//OneForAll
                    if(item.G().eStats.ContainsKey("LifeSteal"))
                    {
                        //float lifeSteal = sourceItem.GetGlobalItem<EnchantedItem>().lifeSteal + wePlayer.lifeSteal;
                        float lifeSteal = item.G().eStats["LifeSteal"].ApplyTo(0f);
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
                    }//LifeSteal
                    if(sourceItem.G().eStats.ContainsKey("GodSlayer"))
                    {
                        ActivateGodSlayer(npc, player, item, ref damage, ref knockback, ref crit, hitDirection, projectile);
                    }//GodSlayer
                    if (sourceItem.G().eStats.ContainsKey("OneForAll") && oneForAllOrigin && projectile != null)
                    {
                        projectile.Kill();
                    }
                }
            }
            if (myWarReduction > 1f && projectile != null && npc.FindBuffIndex(BuffID.RainbowWhipNPCDebuff) == -1 && (projectile.minion || projectile.type == ProjectileID.StardustGuardian || projectile.G().parent != null && projectile.G().parent.minion))
                damage /= myWarReduction;
            //($"/\\HitNPC(npc: {npc.FullName}, player: {player.S()}, item: {item.S()}, damage: {damage}, knockback: {knockback}, crit: {crit}, hitDirection: {hitDirection}, projectile: {projectile.S()})").Log();
        }
        public override void OnHitByItem(NPC npc, Player player, Item item, int damage, float knockback, bool crit)
        {
            //($"\\/OnHitByItem(npc: {npc.FullName}, player: {player.S()}, item: {item.S()}, damage: {damage}, knockback: {knockback}, crit: {crit})").Log();
            OnHitNPC(npc, player, item, ref damage, ref knockback, ref crit);
            //($"/\\OnHitByItem(npc: {npc.FullName}, player: {player.S()}, item: {item.S()}, damage: {damage}, knockback: {knockback}, crit: {crit})").Log();
        }
        public override void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit)
        {
            //($"\\/OnHitByProjectile(npc: {npc.FullName}, projectile: {projectile.S()}, damage: {damage}, knockback: {knockback}, crit: {crit})").Log();
            Item item = projectile.GetGlobalProjectile<ProjectileEnchantedItem>()?.sourceItem == null ? null : projectile.GetGlobalProjectile<ProjectileEnchantedItem>().sourceItem;
            damage = (int)Math.Round((float)damage * projectile.GetGlobalProjectile<ProjectileEnchantedItem>().damageBonus);
            OnHitNPC(npc, Main.player[projectile.owner], item, ref damage, ref knockback, ref crit, projectile);
            //($"/\\OnHitByProjectile(npc: {npc.FullName}, projectile: {projectile.S()}, damage: {damage}, knockback: {knockback}, crit: {crit})").Log();
        }
        private void OnHitNPC(NPC npc, Player player, Item item, ref int damage, ref float knockback, ref bool crit, Projectile projectile = null)
        {
            if(sourceItem != null)
            {
                if (sourceItem.TryGetGlobalItem(out EnchantedItem iGlobal))
                {
                    //int newImmune = (int)((float)npc.immune[player.whoAmI] * (1 + iGlobal.immunityBonus));
                    int newImmune = (int)((float)npc.immune[player.whoAmI] * sourceItem.A("NPCHitCooldown", 1f));
                    npc.immune[player.whoAmI] = newImmune < 1 ? 1 : newImmune;
                }
            }
        }
        private int ActivateOneForAll(NPC npc, Player player, Item item, ref int damage, ref float knockback, ref bool crit, int direction, Projectile projectile = null)
        {
            //($"\\/ActivateOneForAll(npc: {npc.FullName}, player: {player.S()}, item: {item.S()}, damage: {damage}, knockback: {knockback}, crit: {crit}, direction: {direction}, projectile: {projectile.S()})").Log();
            int total = 0;
            int wormCounter = 0;
            float oneForAllRange = baseOneForAllRange * item.scale;
            Dictionary<int, float> npcs = new Dictionary<int, float>();
            foreach (NPC target in Main.npc)
            {
                if (target.whoAmI != npc.whoAmI)
                {
                    if (!target.friendly && !target.townNPC && target.type != NPCID.DD2LanePortal)
                    {
                        Vector2 vector2 = target.Center - npc.Center;
                        float distanceFromOrigin = vector2.Length();
                        if (distanceFromOrigin <= oneForAllRange)
                        {
                            npcs.Add(target.whoAmI, distanceFromOrigin);
                        }
                    }
                }
            }
            //Dictionary<int, float> sortedNpcs = new Dictionary<int, float>();
            // = from entry in npcs.Values ascending select entry;
            foreach(KeyValuePair<int, float> pair in npcs.OrderBy(key => key.Value))
            {
                float distanceFromOrigin = pair.Value;
                int whoAmI = pair.Key;
                NPC target = Main.npc[whoAmI];
                if (npc.aiStyle == NPCAIStyleID.Worm || npc.aiStyle == NPCAIStyleID.TheDestroyer)
                    wormCounter++;
                target.GetGlobalNPC<WEGlobalNPC>().oneForAllOrigin = false;
                target.GetGlobalNPC<WEGlobalNPC>().sourceItem = sourceItem;
                //int allForOneDamage = (int)((float)damage * item.GetGlobalItem<EnchantedItem>().oneForAllBonus);
                float baseAllForOneDamage = ((float)damage * item.G().eStats["OneForAll"].ApplyTo(0f));
                int allForOneDamage = (int)((wormCounter > 10 ? wormCounter < 21 ? 1f - (float)(wormCounter - 10) / 10f : 0f : 1f) * (baseAllForOneDamage * (oneForAllRange - distanceFromOrigin) / oneForAllRange));
                if(Main.netMode == NetmodeID.SinglePlayer)
                    total += (int)target.StrikeNPC(allForOneDamage, knockback, direction);
                else
                    total += (int)target.StrikeNPC(allForOneDamage, knockback, direction, false, false, true);
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
            //($"/\\ActivateOneForAll(npc: {npc.FullName}, player: {player.S()}, item: {item.S()}, damage: {damage}, knockback: {knockback}, crit: {crit}, direction: {direction}, projectile: {projectile.S()}) total: {total}").Log();
            return total;
        }
        private void ActivateGodSlayer(NPC npc, Player player, Item item, ref int damage, ref float knockback, ref bool crit, int direction, Projectile projectile = null)
        {
            if (!npc.friendly && !npc.townNPC)
            {
                //($"\\/ActivateGodSlayer").Log();
                //float godSlayerBonus = npc.boss ? item.GetGlobalItem<EnchantedItem>().godSlayerBonus / 10f : item.GetGlobalItem<EnchantedItem>().godSlayerBonus;
                float godSlayerBonusDefault = item.G().eStats["GodSlayer"].ApplyTo(0f);
                float godSlayerBonus = npc.boss ? godSlayerBonusDefault / (10 * UtilityMethods.GetGodSlayerReductionFactor(npc.lifeMax)) : godSlayerBonusDefault;
                int godSlayerDamage;
                godSlayerDamage = (int)Math.Round(((float)damage / 100f * (godSlayerBonus * npc.lifeMax)) + (npc.defDefense - player.GetWeaponArmorPenetration(item)) / 2);
                if (Main.netMode == NetmodeID.SinglePlayer)
                    npc.StrikeNPC(godSlayerDamage, knockback, direction, crit);
                else
                    npc.StrikeNPC(godSlayerDamage, knockback, direction, crit, false, true);
            }
        }
        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (war && !npc.friendly && !npc.townNPC && !npc.boss)
            {
                myWarReduction = warReduction;
                npc.lavaImmune = true;
                npc.trapImmune = true;
            }
        }
        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            WEPlayer wePlayer = player.GetModPlayer<WEPlayer>();
            if (wePlayer.eStats.ContainsKey("spawnRate"))
            {
                //("\\/EditSpawnRate(" + player.name + ", spawnRate: " + spawnRate + ", maxSpawns: " + maxSpawns + ")").LogT();
                float enemySpawnRateBonus = wePlayer.eStats["spawnRate"].ApplyTo(1f);
                int rate = (int)(spawnRate / enemySpawnRateBonus);
                if (enemySpawnRateBonus > 1f)
                {
                    warReduction = (int)enemySpawnRateBonus;
                    war = true;
                }
                spawnRate = rate < 1 ? 1 : rate;
            }
            else
            {
                warReduction = 1;
                war = false;
            }
            if (wePlayer.eStats.ContainsKey("maxSpawns"))
            {
                float enemyMaxSpawnBonus = wePlayer.eStats["maxSpawns"].ApplyTo(1f);
                int spawns = (int)Math.Round(maxSpawns * enemyMaxSpawnBonus);
                maxSpawns = spawns >= 0 ? spawns : 0;
                //("/\\EditSpawnRate(" + player.name + ", spawnRate: " + spawnRate + ", maxSpawns: " + maxSpawns + ")").LogT();
            }
        }
    }
}