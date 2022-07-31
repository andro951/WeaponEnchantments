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
using WeaponEnchantments.Debuffs;
using WeaponEnchantments.Items.Enchantments.Utility;
using WeaponEnchantments.Items.Enchantments.Unique;
using WeaponEnchantments.Items.Enchantments;
using static WeaponEnchantments.Common.Configs.ConfigValues;
using WeaponEnchantments.Common.Utility;

namespace WeaponEnchantments.Common.Globals
{
    public static class OnHitEffectID
    {
        public const int GodSlayer = 0;
        public const int OneForAll = 1;
        public const int Amaterasu = 2;

        public const int Count = 3;
    }
    public class WEGlobalNPC : GlobalNPC
    {
        #region Static

        public static List<int> preHardModeBossTypes;
        public static List<string> preHardModeModBossNames;
        public static Dictionary<int, float> multipleSegmentBossTypes;
        static bool war = false;
        static float warReduction = 1f;


        #endregion

        private Item _sourceItem;
        public Item SourceItem {
            get => _sourceItem;
			set {
                if (value.IsAir) {
                    _sourceItem = null;
                }
				else {
                    _sourceItem = value;
				}
			}
        }
        private bool oneForAllOrigin = true;
        float baseOneForAllRange = 240f;
        float baseAmaterasuSpreadRange = 60f;
        public int amaterasuDamage = 0;
        private double lastAmaterasuTime = 0;
        public float amaterasuStrength = 0f;
        float myWarReduction = 1f;
        public override bool InstancePerEntity => true;
        public override void Load() {
            IL.Terraria.Projectile.Damage += HookDamage;

            preHardModeBossTypes = new List<int>() {
                NPCID.EyeofCthulhu,
                NPCID.EaterofWorldsBody,
                NPCID.EaterofWorldsHead,
                NPCID.EaterofWorldsTail,
                NPCID.BrainofCthulhu,
                NPCID.KingSlime,
                NPCID.Deerclops,
                NPCID.QueenBee,
                NPCID.SkeletronHead
            };

            preHardModeModBossNames = new List<string>() {
                "Desert Scourge",//Calamity
                "Crabulon",//Calamity
                "The Hive Mind",//Calamity
                "The Perforator Hive",//Calamity
                "The Slime God",//Calamity
                "Trojan Squirrel",//Fargo's
                "Deviantt",//Fargo's
                "The Storm Cloud",//Vitality
                "Gemstone Elemental",//Vitality
                "The Grand Antlion",//Vitality
                "Moonlight Dragonfly",//Vitality
                "Polar Exterminator",//Querty's Bosses and Items 2
                "The Divine Light",//Querty's Bosses and Items 2
                "Ancient Machine",//Querty's Bosses and Items 2
                "Noehtnap",//Querty's Bosses and Items 2
            };

            multipleSegmentBossTypes = new Dictionary<int, float>() {
                { NPCID.EaterofWorldsHead, 100f },
                { NPCID.EaterofWorldsBody, 100f },
                { NPCID.EaterofWorldsTail, 100f }
            };
        }
        private static void HookDamage(ILContext il)
        {
            bool debuggingHookDamage = false;

            var c = new ILCursor(il);

            //Find location of where crit chance is calculated.
            if (!c.TryGotoNext(MoveType.After,
                i => i.MatchLdloc(36),
                i => i.MatchBrfalse(out _),
                i => i.MatchLdarg(0),
                i => i.MatchCall(out _),
                i => i.MatchCallvirt(out _)
            )) { throw new Exception("Failed to find instructions HookDamage"); }

            if (debuggingHookDamage) try { ModContent.GetInstance<WEMod>().Logger.Info("c.Index: " + c.Index.ToString() + " Instruction: " + c.Next.ToString()); } catch (Exception e) { ModContent.GetInstance<WEMod>().Logger.Info("c.Index: " + c.Index.ToString() + " exception: " + e.ToString()); }

            if (debuggingHookDamage) try { ModContent.GetInstance<WEMod>().Logger.Info("c.Index: " + (c.Index - 1).ToString() + " Instruction: " + c.Prev.ToString()); } catch (Exception e) { ModContent.GetInstance<WEMod>().Logger.Info("c.Index: " + (c.Index - 1).ToString() + " exception: " + e.ToString()); }
            
            //Set crit roll to zero.
            c.Emit(OpCodes.Pop);
            c.Emit(OpCodes.Ldc_I4_0);
        }
        public static float GetMultiSegmentBossMultiplier(int npcType) {
            if (multipleSegmentBossTypes.ContainsKey(npcType))
                return multipleSegmentBossTypes[npcType];

            return 1f;
        }
        public static List<int> GetEnchantmentDropList(int arg, bool bossBag = false) {
            List<int> itemTypes = new List<int>();
            switch (arg) {
                case NPCID.KingSlime when !bossBag:
                case ItemID.KingSlimeBossBag when bossBag:
                    itemTypes.Add(ModContent.ItemType<DamageEnchantmentBasic>());
                    break;
                case NPCID.EyeofCthulhu when !bossBag:
                case ItemID.EyeOfCthulhuBossBag when bossBag:
                    itemTypes.Add(ModContent.ItemType<MoveSpeedEnchantmentBasic>());
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
                    itemTypes.Add(ModContent.ItemType<MaxMinionsEnchantmentBasic>());
                    break;
                case NPCID.SkeletronHead when !bossBag:
                case ItemID.SkeletronBossBag when bossBag:
                    itemTypes.Add(ModContent.ItemType<CriticalStrikeChanceEnchantmentBasic>());
                    break;
                case NPCID.Deerclops when !bossBag:
                case ItemID.DeerclopsBossBag when bossBag:
                    //itemTypes.Add(ModContent.ItemType<PhaseJumpEnchantmentBasic>());
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
                    itemTypes.Add(ModContent.ItemType<WorldAblazeEnchantmentBasic>());
                    break;
                case NPCID.TheDestroyer when !bossBag:
                case ItemID.DestroyerBossBag when bossBag:

                    break;
                case NPCID.SkeletronPrime when !bossBag:
                case ItemID.SkeletronPrimeBossBag when bossBag:
                    itemTypes.Add(ModContent.ItemType<ColdSteelEnchantmentBasic>());
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
                case ItemID.CultistBossBag when bossBag://Unobtainable
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
            }

            return itemTypes;
        }
        public static float GetEnchantmentDropChance(int arg, bool bossBag = false) {
            float chance = BossEnchantmentDropChance;
            
            //Apply boss specific multiplier.
            switch (arg) {
                case NPCID.WallofFlesh when !bossBag:
                case ItemID.WallOfFleshBossBag when bossBag:
                case NPCID.MoonLordCore when !bossBag:
                case ItemID.MoonLordBossBag when bossBag:
                    chance *= 2f;
                    break;
            }

			//Multi segment bosses
			if (!bossBag) {
                float multiSegmentBossMultiplier = GetMultiSegmentBossMultiplier(arg);
                chance /= multiSegmentBossMultiplier;
            }

            //Limmit to 1f
            if (chance > 1f)
                chance = 1f;

            return chance;
        }
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot) {
            GetLoot(npcLoot, npc);
        }
        public static void GetLoot(ILoot loot, NPC npc, bool bossBag = false) {
            if (npc.friendly || npc.townNPC || npc.SpawnedFromStatue)
                return;

            GetEssenceDropList(npc, out float[] essenceValues, out float[] dropRate, out int baseID, out float hp, out float total);

            if (total <= 0f)
                return;

            IItemDropRule dropRule;

            bool multipleSegmentBoss = multipleSegmentBossTypes.ContainsKey(npc.type);
            float multipleSegmentBossMultiplier = GetMultiSegmentBossMultiplier(npc.type);
            if (multipleSegmentBoss && bossBag)
                total *= multipleSegmentBossMultiplier;

            //Essence
            for (int i = 0; i < essenceValues.Length; ++i) {
                float thisDropRate = dropRate[i];

                if (thisDropRate <= 0f)
                    continue;

                //Multi-segment boss bag
                if(multipleSegmentBoss && bossBag)
                    thisDropRate *= multipleSegmentBossMultiplier;

                //Denom
                //float denom = multipleSegmentBossMultiplier / thisDropRate;
                float denom = 1f / thisDropRate;

                int denominator;
                int minDropped;
                int maxDropped;
                if (denom < 1f) {
                    //100% chance or higher to drop essence
                    denominator = 1;
                    minDropped = (int)Math.Round(thisDropRate);
                    maxDropped = minDropped + 1;
                }
                else {
                    //Less than 100% chance to drop essence
                    denominator = (int)Math.Round(denom);
                    minDropped = 1;
                    maxDropped = 1;
                }

                dropRule = ItemDropRule.Common(baseID + i, denominator, minDropped, maxDropped);

                if (npc.boss || multipleSegmentBoss) {
                    //Boss or multisegmented boss that doesn't technically count as a boss.
                    loot.Add(new DropBasedOnExpertMode(dropRule, ItemDropRule.DropNothing()));
                }
                else {
                    loot.Add(dropRule);
                }
            }

            if (npc.boss || multipleSegmentBoss) {
                //Boss Drops

                //Superior Containment
                int denominator = (int)(500000 / total);
                if (denominator < 1)
                    denominator = 1;

                dropRule = ItemDropRule.Common(ModContent.ItemType<SuperiorContainment>(), denominator, 1, 1);
                loot.Add(new DropBasedOnExpertMode(dropRule, ItemDropRule.DropNothing()));

                //Power Booster
                bool preHardModeBoss = preHardModeBossTypes.Contains(npc.type) || preHardModeModBossNames.Contains(npc.FullName);
                if (!WEMod.serverConfig.PreventPowerBoosterFromPreHardMode || !preHardModeBoss) {
                    denominator = (int)(1000000 / total);
                    if (denominator < 1)
                        denominator = 1;

                    dropRule = ItemDropRule.Common(ModContent.ItemType<PowerBooster>(), denominator, 1, 1);
                    loot.Add(new DropBasedOnExpertMode(dropRule, ItemDropRule.DropNothing()));
                }

                //Enchantment drop chance
                float chance = GetEnchantmentDropChance(npc.type, bossBag);

                //Enchantment Drop List
                List<int> itemTypes = GetEnchantmentDropList(npc.type);

                denominator = (int)Math.Round(1f / chance);
                if (denominator < 1)
                    denominator = 1;

                //Enchantments
                if (itemTypes.Count > 1) {
                    //More than 1 drop option

                    switch (npc.type) {
                        case NPCID.CultistBoss:
                            dropRule = ItemDropRule.OneFromOptions(denominator, itemTypes.ToArray());
                            break;
                        default:
                            IItemDropRule expertDropRule = ItemDropRule.OneFromOptions(denominator, itemTypes.ToArray());
                            dropRule = new DropBasedOnExpertMode(expertDropRule, ItemDropRule.DropNothing());
                            break;
                    }

                    loot.Add(dropRule);
                }
                else if (itemTypes.Count > 0) {
                    //One drop option
                    switch (npc.type) {
                        case NPCID.CultistBoss:
                            dropRule = ItemDropRule.Common(itemTypes[0], denominator);
                            break;
                        default:
                            IItemDropRule expertDropRule = ItemDropRule.Common(itemTypes[0], denominator);
                            dropRule = new DropBasedOnExpertMode(expertDropRule, ItemDropRule.DropNothing());
                            break;
                    }

                    loot.Add(dropRule);
                }
            }
            else {
                //Non-boss drops

                //mult is the config multiplier
                float mult = EnchantmentDropChance;

                //defaultDenom is the denominator of the drop rate.  The numerator is always 1 (This is part of how Terraria calculates drop rates, I can't change it)
                //  hp is the npc's max hp
                //  total is calculated based on npc max hp.  Use total = hp + 0.2 * value
                //Example: defaultDenom = 5, numerator is 1.  Drop rate = 1/5 = 20%
                //Aproximate drop rate = (hp + 0.2 * value)/(5000 + hp * 5) * config multiplier
                if (EnchantmentDropChance <= 0f)
                    return;

                int defaultDenom = (int)((5000f + hp * 5f) / (total * mult));
                if (defaultDenom < 1)
                    defaultDenom = 1;

                int denom100 = (int)Math.Round(1f / mult);

                //Ai Style
                switch (npc.aiStyle) {
                    case 1://Slime
                        loot.Add(ItemDropRule.Common(ModContent.ItemType<DamageEnchantmentBasic>(), defaultDenom, 1, 1));
                        break;
                    case 2://Demon Eye
                        loot.Add(ItemDropRule.Common(ModContent.ItemType<ScaleEnchantmentBasic>(), defaultDenom, 1, 1));
                        break;
                    case 3://Fighter
                        loot.Add(ItemDropRule.Common(ModContent.ItemType<StatDefenseEnchantmentBasic>(), defaultDenom, 1, 1));
                        break;
                    case 5://Flying
                        loot.Add(ItemDropRule.Common(ModContent.ItemType<AmmoCostEnchantmentBasic>(), defaultDenom, 1, 1));
                        break;
                    case 6://Worm
                        loot.Add(ItemDropRule.Common(ModContent.ItemType<ManaEnchantmentBasic>(), defaultDenom, 1, 1));
                        break;
                    case 8://Caster
                        loot.Add(ItemDropRule.Common(ModContent.ItemType<ManaEnchantmentBasic>(), defaultDenom, 1, 1));
                        break;
                    case 10://Cursed Skull
                        loot.Add(ItemDropRule.Common(ModContent.ItemType<ManaEnchantmentBasic>(), defaultDenom, 1, 1));
                        break;
                    case 13://Plant
                        loot.Add(ItemDropRule.Common(ModContent.ItemType<CriticalStrikeChanceEnchantmentBasic>(), defaultDenom, 1, 1));
                        break;
                    case 14://Bat
                        loot.Add(ItemDropRule.Common(ModContent.ItemType<SpeedEnchantmentBasic>(), defaultDenom, 1, 1));
                        break;
                    case 16://Swimming
                        loot.Add(ItemDropRule.Common(ModContent.ItemType<SpeedEnchantmentBasic>(), defaultDenom, 1, 1));
                        break;
                    case 17://Vulture
                        loot.Add(ItemDropRule.Common(ModContent.ItemType<LifeStealEnchantmentBasic>(), defaultDenom, 1, 1));
                        break;
                    case 18://Jellyfish
                        loot.Add(ItemDropRule.Common(ModContent.ItemType<CriticalStrikeChanceEnchantmentBasic>(), defaultDenom, 1, 1));
                        break;
                    case 19://Antlion
                        loot.Add(ItemDropRule.Common(ModContent.ItemType<CriticalStrikeChanceEnchantmentBasic>(), defaultDenom, 1, 1));
                        break;
                    case 22://Hovering

                        break;
                    case 23://Flying Weapon

                        break;
                    case 25://Mimic
                        int[] options = new int[] {
                            ModContent.ItemType<SpelunkerEnchantmentUltraRare>(),
                            ModContent.ItemType<DangerSenseEnchantmentUltraRare>(),
                            ModContent.ItemType<HunterEnchantmentUltraRare>(),
                            ModContent.ItemType<ObsidianSkinEnchantmentUltraRare>()
                        };
                        loot.Add(ItemDropRule.OneFromOptions(denom100, options));
                        break;
                    case 26://Unicorn

                        break;
                    case 29://The Hungry - Wall of flesh plant minions
                        loot.Add(ItemDropRule.Common(ModContent.ItemType<LifeStealEnchantmentBasic>(), defaultDenom, 1, 1));
                        break;
                    case 38://Snowman

                        break;
                    case 39://Tortoise

                        break;
                    case 40://Spider
                        loot.Add(ItemDropRule.Common(ModContent.ItemType<MaxMinionsEnchantmentBasic>(), defaultDenom, 1, 1));
                        break;
                    case 41://Derpling - Blue jungle bug

                        break;
                    case 44://Flying Fish

                        break;
                    case 49://Angry Nimbus

                        break;
                    case 55://Creeper Brain of Cthulhu minions
                        loot.Add(ItemDropRule.Common(ModContent.ItemType<LifeStealEnchantmentBasic>(), defaultDenom, 1, 1));
                        break;
                    case 56://Dungeon Guardians

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
                        int[] optionsBiomeMimic = new int[] {
                            ModContent.ItemType<GodSlayerEnchantmentBasic>(),
                            ModContent.ItemType<MultishotEnchantmentBasic>()
                        };
                        loot.Add(ItemDropRule.OneFromOptions(denom100, optionsBiomeMimic));
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
                }

                //Npc type
                switch (npc.type) {
                    case NPCID.Harpy://48
                    case NPCID.SnowmanGangsta://143
                    case NPCID.SnowBalla://145
                        loot.Add(ItemDropRule.Common(ModContent.ItemType<ShootSpeedEnchantmentBasic>(), defaultDenom, 1, 1));
                        break;
                    case NPCID.Pixie://75
                        loot.Add(ItemDropRule.Common(ModContent.ItemType<PeaceEnchantmentBasic>(), defaultDenom, 1, 1));
                        break;
                    case NPCID.Mothron://477
                        loot.Add(ItemDropRule.Common(ModContent.ItemType<OneForAllEnchantmentBasic>(), defaultDenom, 1, 1));
                        loot.Add(ItemDropRule.Common(ModContent.ItemType<AllForOneEnchantmentBasic>(), defaultDenom, 1, 1));
                        break;
                    case NPCID.PirateShip://491
                        loot.Add(ItemDropRule.Common(ModContent.ItemType<WarEnchantmentBasic>(), defaultDenom, 1, 1));
                        break;
                    case NPCID.GiantWalkingAntlion://508
                        loot.Add(ItemDropRule.Common(ModContent.ItemType<MoveSpeedEnchantmentBasic>(), defaultDenom, 1, 1));
                        break;
                    case NPCID.WalkingAntlion://580
                        loot.Add(ItemDropRule.Common(ModContent.ItemType<MoveSpeedEnchantmentBasic>(), defaultDenom, 1, 1));
                        break;
                }
            }
        }
        public static void GetEssenceDropList(NPC npc, out float[] essenceValues, out float[] dropRate, out int baseID, out float hp, out float total) {
            //Defense
            float defenseMultiplier = 1f + (float)npc.defDefense / 40f;

            //HP
            hp = (float)npc.lifeMax * defenseMultiplier;
            
            //Value
            float value = npc.value;

            //Prevent low value enemies like critters from dropping essence
            if(value <= 0 && hp <= 10) {
                total = 0;
                dropRate = null;
                baseID = 0;
                essenceValues = null;
                return;
			}

            //Total
            if(value > 0) {
                total = hp + 0.2f * value;
			}
			else {
                total = hp * 2.6f;
            }

            //Hp reduction factor
            float hpReductionFactor = EnchantedItemStaticMethods.GetReductionFactor((int)hp);
            total /= hpReductionFactor;

            //NPC Characteristics Factors
            float noGravityFactor = npc.noGravity ? 0.4f : 0f;
            float noTileCollideFactor = npc.noTileCollide ? 0.4f : 0f;
            float knockBackResistFactor = 0.4f * (1f - npc.knockBackResist);
            float npcCharacteristicsFactor = noGravityFactor + noTileCollideFactor + knockBackResistFactor;
            total *= npcCharacteristicsFactor;

            //Balance Multiplier (Extra multiplier for us to control the values manually)
            float balanceMultiplier = 2f;
            total *= balanceMultiplier;

            //Modify total for specific enemies.
            switch (npc.type) {
                case NPCID.DungeonGuardian:
                    total /= 50f;
                    break;
                case NPCID.EaterofWorldsHead:
                case NPCID.EaterofWorldsBody:
                case NPCID.EaterofWorldsTail:
                    total /= 8f;
                    break;
            }

            float essenceTotal = total;
            bool multiSegmentBoss = multipleSegmentBossTypes.ContainsKey(npc.type);

            //Config Multiplier
            if (npc.boss || multiSegmentBoss) {
                essenceTotal *= BossEssenceMultiplier;
            }
            else {
                essenceTotal *= EssenceMultiplier;
            }

            essenceValues = EnchantmentEssence.values;
            dropRate = new float[essenceValues.Length];
            baseID = ModContent.ItemType<EnchantmentEssenceBasic>();
            int essenceTier = 0;

            //Calculate the main essence tier that will be dropped.
            if (npc.boss) {
                //Bosses
                for (int i = 0; i < essenceValues.Length; ++i) {
                    float essenceValue = essenceValues[i];
                    if (essenceTotal / essenceValue > 1) {
                        essenceTier = i;
                    }
                    else {
                        break;
                    }
                }
            }
            else {
                //Non-bosses

                //tierCuttoff is 1/8th of the max drop rate you want to see.
                float tierCutoff = multiSegmentBoss ? 0.1f : 0.025f;
                for (int i = 0; i < essenceValues.Length; ++i) {
                    float essenceValue = essenceValues[i];
                    if (essenceTotal / essenceValue < tierCutoff) {
                        break;
                    }
                    else {
                        essenceTier = i;
                    }
                }
            }

            float thisDropRate = essenceTotal / essenceValues[essenceTier];

            //Main tier and below
            if (essenceTier == 0) {
                //Main essence is Tier 0, can't go below it, so drop 25% more of this tier.
                dropRate[essenceTier] = 1.25f * thisDropRate;
            }
            else {
                //100% towards Main essence tier. (25% boost if tier 4 because no tier 5 exists.)
                dropRate[essenceTier] = essenceTier == 4 ? thisDropRate * 1.25f : thisDropRate;

                //50% towards the tier below.
                dropRate[essenceTier - 1] = 0.5f * thisDropRate;
            }

            //Tier above
            if (essenceTier < 4) {
                dropRate[essenceTier + 1] = 0.06125f * thisDropRate;
            }
        }
        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref int damage, ref float knockback, ref bool crit) {

			#region Debug

			if (LogMethods.debugging) ($"\\/ModifyHitByItem(npc: {npc.FullName}, player: {player.S()}, item: {item.S()}, damage: {damage}, knockback: {knockback}, crit: {crit})").Log();

			#endregion

			HitNPC(npc, player, item, ref damage, ref knockback, ref crit, player.direction);

            #region Debug

            if (LogMethods.debugging) ($"/\\ModifyHitByItem(npc: {npc.FullName}, player: {player.S()}, item: {item.S()}, damage: {damage}, knockback: {knockback}, crit: {crit})").Log();
            
            #endregion
        }
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection) {

            #region Debug

            if (LogMethods.debugging) ($"\\/ModifyHitByProjectile(npc: {npc.FullName}, projectile: {projectile.S()}, damage: {damage}, knockback: {knockback}, crit: {crit})").Log();

            #endregion

            bool weProjectileIsNull = projectile.GetGlobalProjectile<WEProjectile>()?.sourceItem == null;

            //Projectile SourceItem
            Item item = weProjectileIsNull ? null : projectile.GetGlobalProjectile<WEProjectile>().sourceItem;

            HitNPC(npc, Main.player[projectile.owner], item, ref damage, ref knockback, ref crit, hitDirection, projectile);

            #region Debug

            if (LogMethods.debugging) ($"/\\ModifyHitByProjectile(npc: {npc.FullName}, projectile: {projectile.S()}, damage: {damage}, knockback: {knockback}, crit: {crit})").Log();

            #endregion
        }
        private void HitNPC(NPC npc, Player player, Item item, ref int damage, ref float knockback, ref bool crit, int hitDirection, Projectile projectile = null) {

            #region Debug

            if (LogMethods.debugging) ($"\\/HitNPC(npc: {npc.FullName}, player: {player.S()}, item: {item.S()}, damage: {damage}, knockback: {knockback}, crit: {crit}, hitDirection: {hitDirection}, projectile: {projectile.S()})").Log();

            #endregion

            //Minion damage reduction from war enchantment
            if(projectile != null) {
                bool minionOrMinionChild = projectile.minion || projectile.type == ProjectileID.StardustGuardian || projectile.GetWEProjectile().parent != null && projectile.GetWEProjectile().parent.minion;
                if (myWarReduction > 1f && projectile != null && npc.whoAmI != player.MinionAttackTargetNPC && minionOrMinionChild) {
                    damage = (int)Math.Round(damage / myWarReduction);
                }
            }

            if (!item.TryGetEnchantedItem(out EnchantedItem iGlobal))
                return;

            SourceItem = item;

            //Stardust dragon scale damage multiplier correction//Stardust Dragon
            if (projectile != null && ProjectileID.Sets.StardustDragon[projectile.type]) {
                float enchantmentScaleMultiplier = SourceItem.ApplyStatModifier("scale", 1f);
                if(enchantmentScaleMultiplier > 1f && projectile.scale / enchantmentScaleMultiplier < 1.5f) {
                    float scaleBeforeEnchantments = projectile.scale / enchantmentScaleMultiplier;
                    float correctedMultiplier = 1f + Utils.Clamp((scaleBeforeEnchantments - 1f) * 100f, 0f, 50f) * 0.23f;
                    float vanillaMultiplier = 1f + (Utils.Clamp((projectile.scale - 1f) * 100f, 0f, 50f)) * 0.23f;
                    float combinedMultiplier = correctedMultiplier / vanillaMultiplier;
                    damage = (int)Math.Round((float)damage * combinedMultiplier);
                }
            }

            //Defense (damage reduction)
            int armorPenetration = player.GetWeaponArmorPenetration(item);
            int damageReduction = npc.defense / 2 - npc.checkArmorPenetration(armorPenetration);

            //Prevent damage from being less than 1
            if (damageReduction >= damage)
                damageReduction = damage - 1;

            damage -= damageReduction;

            //Armor penetration bonus damage
            if (WEMod.serverConfig.ArmorPenetration && armorPenetration > npc.defDamage) {
                int armorPenetrationBonusDamage = (int)Math.Round((float)(armorPenetration - npc.defDamage) / 2f);
                if (armorPenetrationBonusDamage > 50) {
                    int maxArmorPenetration = 50 + (int)item.ApplyStatModifier("ArmorPenetration", 0f) / 2;
                    if (armorPenetrationBonusDamage > maxArmorPenetration)
                        armorPenetrationBonusDamage = maxArmorPenetration;
                }

                damage += armorPenetrationBonusDamage;
            }

            //Damage Enchantment
            damage = (int)Math.Round(item.ApplyEStat("Damage", (float)damage));

            //Critical strike
            if(item.DamageType != DamageClass.Summon || !WEMod.serverConfig.DisableMinionCrits) {
                int critChance = player.GetWeaponCrit(item) + (crit ? 100 : 0);
                crit = false;
                int critLevel = critChance / 100;
                critChance %= 100;
                if (Main.rand.Next(0, 100) < critChance)
                    critLevel++;

                if (critLevel > 0) {
                    crit = true;
                    critLevel--;

                    if (MultiplicativeCriticalHits) {
                        //Multiplicative
                        damage *= (int)Math.Pow(2, critLevel);
                    }
				    else {
                        //Additive
                        float additiveCritMultiplier = 1f + 0.5f * critLevel;
                        damage = (int)(damage * additiveCritMultiplier);
				    }
                }//MultipleCritlevels
            }

            damage += damageReduction;

            bool makingPacket = false;

            //Setup packet
            ModPacket onHitEffectsPacket = null;
            bool[] onHitEffects = new bool[OnHitEffectID.Count];
            if (Main.netMode == NetmodeID.MultiplayerClient) {
                onHitEffectsPacket = ModContent.GetInstance<WEMod>().GetPacket();
                makingPacket = true;
                onHitEffectsPacket.Write(WEMod.PacketIDs.OnHitEffects);
                onHitEffectsPacket.Write(npc.whoAmI);
                onHitEffectsPacket.Write(damage);
                onHitEffectsPacket.Write(crit);
            }

            bool skipOnHitEffects = projectile != null ? projectile.GetWEProjectile().skipOnHitEffects : false;

            WEPlayer wePlayer = player.GetModPlayer<WEPlayer>();

            Dictionary<string, StatModifier> ItemEStats = item.GetEnchantedItem().eStats;

            //Buffs and debuffs
            if (!skipOnHitEffects) {
                //Debuffs
                foreach (int debuff in item.GetEnchantedItem().debuffs.Keys) {
                    //Amaterasu
                    if(debuff == ModContent.BuffType<AmaterasuDebuff>()) {
                        onHitEffects[OnHitEffectID.Amaterasu] = true;
                        if (amaterasuStrength == 0)
                            amaterasuStrength = item.ApplyEStat("Amaterasu", 0f);

                        amaterasuDamage += damage * (crit ? 2 : 1);
                    }

                    npc.AddBuff(debuff, item.GetEnchantedItem().debuffs[debuff]);
                }

                //Sets Minion Attack target
                if (ItemEStats.ContainsKey("ColdSteel") || ItemEStats.ContainsKey("HellsWrath") || ItemEStats.ContainsKey("JunglesFury") || ItemEStats.ContainsKey("Moonlight"))
                    player.MinionAttackTargetNPC = npc.whoAmI;
            }

            List<int> oneForAllWhoAmIs = new List<int>();
            List<int> oneForAllDamages = new List<int>();
            if (npc.type != NPCID.TargetDummy) {
                int oneForAllDamageDealt = 0;
                //Buffs and Debuffs
                if (!skipOnHitEffects) {
                    //On Hit Player buffs
                    foreach (int onHitBuff in item.GetEnchantedItem().onHitBuffs.Keys) {
                        switch (onHitBuff) {
                            case BuffID.CoolWhipPlayerBuff:
                                //CoolWhip Snowflake
                                if (player.FindBuffIndex(onHitBuff) == -1) {
                                    int newProjectileWhoAmI = Projectile.NewProjectile(projectile != null ? projectile.GetSource_FromThis() : item.GetSource_FromThis(), npc.Center, Vector2.Zero, ProjectileID.CoolWhipProj, 10, 0f, player.whoAmI);
                                    Main.projectile[newProjectileWhoAmI].GetWEProjectile().skipOnHitEffects = true;
                                }
                                break;
                        }

                        player.AddBuff(onHitBuff, item.GetEnchantedItem().onHitBuffs[onHitBuff]);
                    }
                }

				#region Debug

				if (LogMethods.debugging) ($"sourceItem: {SourceItem.S()} {ItemEStats.S("OneForAll")}").Log();

				#endregion

                //One For All
				if (ItemEStats.ContainsKey("OneForAll") && oneForAllOrigin) {
                    oneForAllDamageDealt = ActivateOneForAll(npc, player, item, ref damage, ref knockback, ref crit, hitDirection, out oneForAllWhoAmIs, out oneForAllDamages, projectile);

                    if (makingPacket && oneForAllWhoAmIs.Count > 0)
                        onHitEffects[OnHitEffectID.OneForAll] = true;
                }

                //LifeSteal
                if (ItemEStats.ContainsKey("LifeSteal")) {
                    float lifeSteal = ItemEStats["LifeSteal"].ApplyTo(0f);
                    float healTotal = (damage + oneForAllDamageDealt) * lifeSteal * (player.moonLeech ? 0.5f : 1f) + wePlayer.lifeStealRollover;

                    //Summon damage reduction
                    bool summonDamage = SourceItem.DamageType == DamageClass.Summon || SourceItem.DamageType == DamageClass.MagicSummonHybrid;
                    if (summonDamage)
                        healTotal *= 0.5f;

                    int heal = (int)healTotal;

                    if (player.statLife < player.statLifeMax2) {
                        //Player hp less than max
                        if (heal > 0 && player.lifeSteal > 0f) {
                            //Vanilla lifesteal mitigation
                            int vanillaLifeStealValue = (int)Math.Round(heal * AffectOnVanillaLifeStealLimmit);
                            player.lifeSteal -= vanillaLifeStealValue;

                            Vector2 speed = new Vector2(0, 0);
                            Projectile.NewProjectile(SourceItem.GetSource_ItemUse(SourceItem), npc.Center, speed, ProjectileID.VampireHeal, 0, 0f, player.whoAmI, player.whoAmI, heal);
                        }

                        //Life Steal Rollover
                        wePlayer.lifeStealRollover = healTotal - heal;
                    }
                    else {
                        //Player hp is max
                        wePlayer.lifeStealRollover = 0f;
                    }
                }
            }

            //GodSlayer
            int godSlayerDamage = 0;
            if (ItemEStats.ContainsKey("GodSlayer")) {
                godSlayerDamage = ActivateGodSlayer(npc, player, item, ref damage, damageReduction, ref knockback, ref crit, hitDirection, projectile);

                if (makingPacket)
                    onHitEffects[OnHitEffectID.GodSlayer] = true;
            }

            //One for all kill projectile on hit.
            if (ItemEStats.ContainsKey("OneForAll") && oneForAllOrigin && projectile != null) {
                if(projectile.penetrate != 1)
                    projectile.active = false;
            }

            //Finish and send packet
            if (makingPacket) {
                for(int i = 0; i < onHitEffects.Length; i++) {
                    onHitEffectsPacket.Write(onHitEffects[i]);
                    if (onHitEffects[i]) {
                        switch (i) {
                            case OnHitEffectID.GodSlayer:
                                onHitEffectsPacket.Write(godSlayerDamage);
                                break;
                            case OnHitEffectID.OneForAll:
                                onHitEffectsPacket.Write(oneForAllWhoAmIs.Count);
                                for(int j = 0; j < oneForAllWhoAmIs.Count; j++) {
                                    onHitEffectsPacket.Write(oneForAllWhoAmIs[j]);
                                    onHitEffectsPacket.Write(oneForAllDamages[j]);
                                }
                                break;
                            case OnHitEffectID.Amaterasu:
                                onHitEffectsPacket.Write(item.ApplyEStat("Amaterasu", 0f));
                                break;
                        }
                    }
                }

                onHitEffectsPacket.Send();
            }

			#region Debug

			if (LogMethods.debugging) ($"/\\HitNPC(npc: {npc.FullName}, player: {player.S()}, item: {item.S()}, damage: {damage}, knockback: {knockback}, crit: {crit}, hitDirection: {hitDirection}, projectile: {projectile.S()})").Log();

			#endregion
		}
		public override void OnHitByItem(NPC npc, Player player, Item item, int damage, float knockback, bool crit) {

            #region Debug

            if (LogMethods.debugging) ($"\\/OnHitByItem(npc: {npc.FullName}, player: {player.S()}, item: {item.S()}, damage: {damage}, knockback: {knockback}, crit: {crit})").Log();

            #endregion

            OnHitNPC(npc, player, item, ref damage, ref knockback, ref crit);

            #region Debug

            if (LogMethods.debugging) ($"/\\OnHitByItem(npc: {npc.FullName}, player: {player.S()}, item: {item.S()}, damage: {damage}, knockback: {knockback}, crit: {crit})").Log();

            #endregion
        }
        public override void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit) {

            #region Debug

            if (LogMethods.debugging) ($"\\/OnHitByProjectile(npc: {npc.FullName}, projectile: {projectile.S()}, damage: {damage}, knockback: {knockback}, crit: {crit})").Log();

            #endregion

            bool weProjectileIsNull = projectile.GetGlobalProjectile<WEProjectile>()?.sourceItem == null;

            //Projectile SourceItem
            Item item = weProjectileIsNull ? null : projectile.GetGlobalProjectile<WEProjectile>().sourceItem;

            OnHitNPC(npc, Main.player[projectile.owner], item, ref damage, ref knockback, ref crit, projectile);

            #region Debug

            if (LogMethods.debugging) ($"/\\OnHitByProjectile(npc: {npc.FullName}, projectile: {projectile.S()}, damage: {damage}, knockback: {knockback}, crit: {crit})").Log();

            #endregion
        }
        private void OnHitNPC(NPC npc, Player player, Item item, ref int damage, ref float knockback, ref bool crit, Projectile projectile = null) {
            if (!SourceItem.TryGetEnchantedItem(out EnchantedItem iGlobal))
                return;

            //If projectile/npc doesn't use npc.immune, return
            if (npc.immune[player.whoAmI] <= 0)
                return;

            //Fix for Multishot not improving damage on flamethrowers
            float NPCHitCooldownMultiplier = SourceItem.ApplyEStat("NPCHitCooldown", 1f);
            if(SourceItem.useAmmo == ItemID.Gel && SourceItem.Name != "Shadethrower")
                NPCHitCooldownMultiplier *= 1f / (SourceItem.ApplyEStat("Multishot", 1f));

            //npc.immune
            int newImmune = (int)((float)npc.immune[player.whoAmI] * NPCHitCooldownMultiplier);
            if (newImmune < 1)
                newImmune = 1;

            npc.immune[player.whoAmI] = newImmune;
        }
        private int ActivateOneForAll(NPC npc, Player player, Item item, ref int damage, ref float knockback, ref bool crit, int direction, out List<int> whoAmIs, out List<int> damages, Projectile projectile = null) {

            #region Debug

            if (LogMethods.debugging) ($"\\/ActivateOneForAll(npc: {npc.FullName}, player: {player.S()}, item: {item.S()}, damage: {damage}, knockback: {knockback}, crit: {crit}, direction: {direction}, projectile: {projectile.S()})").Log();

            #endregion

            int total = 0;
            int wormCounter = 0;
            whoAmIs = new List<int>();
            damages = new List<int>();

            //Range
            float oneForAllRange = baseOneForAllRange * item.scale;

            //Sorted List by range
            Dictionary<int, float> npcs = SortNPCsByRange(npc, oneForAllRange);

            foreach(KeyValuePair<int, float> npcDataPair in npcs.OrderBy(key => key.Value)) {
                if (!npc.active)
                    continue;

                whoAmIs.Add(npcDataPair.Key);
                float distanceFromOrigin = npcDataPair.Value;
                int whoAmI = npcDataPair.Key;
                NPC target = Main.npc[whoAmI];

                //Worms
                bool isWorm = npc.aiStyle == NPCAIStyleID.Worm || npc.aiStyle == NPCAIStyleID.TheDestroyer;
                if (isWorm)
                    wormCounter++;

                target.GetGlobalNPC<WEGlobalNPC>().oneForAllOrigin = false;
                target.GetGlobalNPC<WEGlobalNPC>().SourceItem = SourceItem;
                float allForOneMultiplier = item.GetEnchantedItem().eStats["OneForAll"].ApplyTo(0f);
                float baseAllForOneDamage = damage * allForOneMultiplier;

                float allForOneDamage = baseAllForOneDamage * (oneForAllRange - distanceFromOrigin) / oneForAllRange;

                //Worm damage reduction
				if (isWorm) {
                    float wormReductionFactor = 1f;
                    if (wormCounter > 10) {
                        if (wormCounter <= 20) {
                            wormReductionFactor = 1f - (float)(wormCounter - 10f) / 10f;
                        }
                        else {
                            wormReductionFactor = 0f;
                        }
                    }
                    allForOneDamage *= wormReductionFactor;
                }

                int allForOneDamageInt = (int)Math.Round(allForOneDamage);

                if(allForOneDamageInt > 0) {
                    //Hit target
                    total += (int)target.StrikeNPC(allForOneDamageInt, knockback, direction);
                }
                damages.Add(allForOneDamageInt);
                target.GetGlobalNPC<WEGlobalNPC>().oneForAllOrigin = true;
            }

			#region Debug

			if (LogMethods.debugging) ($"/\\ActivateOneForAll(npc: {npc.FullName}, player: {player.S()}, item: {item.S()}, damage: {damage}, knockback: {knockback}, crit: {crit}, direction: {direction}, projectile: {projectile.S()}) total: {total}").Log();

			#endregion

			return total;
        }
        private Dictionary<int, float> SortNPCsByRange(NPC npc, float range) {
            Dictionary<int, float> npcs = new Dictionary<int, float>();
            foreach (NPC target in Main.npc) {
                if (target.whoAmI != npc.whoAmI) {
                    if (target.friendly || target.townNPC || target.type == NPCID.DD2LanePortal)
                        continue;

                    Vector2 vector2 = target.Center - npc.Center;
                    float distanceFromOrigin = vector2.Length();
                    if (distanceFromOrigin <= range)
                        npcs.Add(target.whoAmI, distanceFromOrigin);
                }
            }

            return npcs;
        }
        public int ActivateGodSlayer(NPC npc, Player player, Item item, ref int damage, int damageReduction, ref float knockback, ref bool crit, int direction, Projectile projectile = null) {
            if(npc.friendly || npc.townNPC || !npc.active || npc.type == NPCID.DD2LanePortal)
                return 0;

			#region Debug

			if (LogMethods.debugging) ($"\\/ActivateGodSlayer").Log();

			#endregion
			
			float godSlayerBonus = item.GetEnchantedItem().eStats["GodSlayer"].ApplyTo(0f);
            
            float actualDamageDealt = damage - damageReduction;
            float godSlayerDamage = actualDamageDealt * godSlayerBonus * npc.lifeMax / 100f;

            //Projectile damage reduction
            float projectileMultiplier = projectile != null ? 0.5f : 1f;
            godSlayerDamage *= projectileMultiplier;

            //Max life reduction factor
            float denominator = 1f + npc.lifeMax * 49f / 150000f;
            godSlayerDamage /= denominator;

            //Bypass armor
            godSlayerDamage += damageReduction;

            int godSlayerDamageInt = (int)Math.Round(godSlayerDamage);

            //Hit npc
            npc.StrikeNPC(godSlayerDamageInt, knockback, direction, crit);

			#region Debug

			if (LogMethods.debugging) ($"/\\ActivateGodSlayer").Log();

			#endregion

			return godSlayerDamageInt;
        }
        public static void StrikeNPC(int npcWhoAmI, int damage, bool crit) {
            if(Main.npc[npcWhoAmI].active)
                Main.npc[npcWhoAmI].StrikeNPC(damage, 0, 0, crit, false, true);
        }
        public override void OnSpawn(NPC npc, IEntitySource source) {
            if (npc.ModNPC != null) {
                string n = npc.FullName;
                string n2 = npc.ModNPC.Name;
                string n3 = npc.GivenName;
                float v = npc.value;
                int l = npc.lifeMax;
            }
            if (!war || npc.friendly || npc.townNPC || npc.boss)
                return;

            //Apply war downsides
            myWarReduction = warReduction;
            npc.lavaImmune = true;
            npc.trapImmune = true;
    }
        public override void UpdateLifeRegen(NPC npc, ref int damage) {
            if (amaterasuDamage <= 0)
                return;

			#region Debug
            
			if (LogMethods.debugging) ($"\\/UpdateLifeRegen(npc: {npc.S()}, damage: {damage} amaterasuDamage: {amaterasuDamage} amaterasuStrength: {amaterasuStrength} npc.life: {npc.life} npc.liferegen: {npc.lifeRegen}").Log();

			#endregion

            //Amaeterasu damage goes up over time on its own.
			amaterasuDamage++;

            //Controls how fast the damage tick rate is.
            damage += amaterasuDamage / 240;

            //Set damage over time (amaterasuStrength is the EnchantmentStrength affected by config values.)
            npc.lifeRegen -=  (int)(((float)amaterasuDamage / 30f) * amaterasuStrength);

            //Spread to other enemies ever 10 ticks
            if(npc.type != NPCID.TargetDummy && lastAmaterasuTime + 10 <= Main.GameUpdateCount) {
                Dictionary<int, float> npcs = SortNPCsByRange(npc, baseAmaterasuSpreadRange);
                foreach (int whoAmI in npcs.Keys) {
                    Main.npc[whoAmI].GetWEGlobalNPC().amaterasuDamage += 10;
                    Main.npc[whoAmI].GetWEGlobalNPC().amaterasuStrength = amaterasuStrength;
                    Main.npc[whoAmI].AddBuff(ModContent.BuffType<AmaterasuDebuff>(), -1);
                }

                lastAmaterasuTime = Main.GameUpdateCount;
            }

			#region Debug

			if (LogMethods.debugging) ($"/\\UpdateLifeRegen(npc: {npc.S()}, damage: {damage} amaterasuDamage: {amaterasuDamage} amaterasuStrength: {amaterasuStrength} npc.life: {npc.life} npc.liferegen: {npc.lifeRegen}").Log();

			#endregion
		}
		public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns) {

            #region Debug

            if (LogMethods.debugging) {
                ($"\\/EditSpawnRate(" + player.name + ", spawnRate: " + spawnRate + ", maxSpawns: " + maxSpawns + ")").LogT();
                (player.GetWEPlayer().eStats.S("spawnRate")).LogT();
            }

			#endregion

            //Spawn Rate
			if (player.ContainsEStatOnPlayer("spawnRate")) {
                float enemySpawnRateBonus = player.ApplyEStatFromPlayer("spawnRate", 1f);
                int rate = (int)(spawnRate / enemySpawnRateBonus);
                if (enemySpawnRateBonus > 1f) {
                    warReduction = enemySpawnRateBonus;
                    war = true;
                }

                if (rate < 1)
                    rate = 1;

                spawnRate = rate;
            }
            else {
                warReduction = 1f;
                war = false;
            }

            //Max Spawns
            if (player.ContainsEStatOnPlayer("maxSpawns")) {
                float enemyMaxSpawnBonus = player.ApplyEStatFromPlayer("maxSpawns", 1f);
                int spawns = (int)Math.Round(maxSpawns * enemyMaxSpawnBonus);

                if (spawns < 0)
                    spawns = 0;

                maxSpawns = spawns;
            }

			#region Debug

			if (LogMethods.debugging) ($"/\\EditSpawnRate(" + player.name + ", spawnRate: " + spawnRate + ", maxSpawns: " + maxSpawns + ")").LogT();

			#endregion
		}
		public override void DrawEffects(NPC npc, ref Color drawColor) {
            if(amaterasuDamage > 0) {
                //Black On fire dust
                if (Main.rand.Next(4) < 3) {
                    Dust dust4 = Dust.NewDustDirect(new Vector2(npc.position.X - 2f, npc.position.Y - 2f), npc.width + 4, npc.height + 4, DustID.WhiteTorch, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, Color.Black, 3.5f);
                    dust4.noGravity = true;
                    dust4.velocity *= 1.8f;
                    dust4.velocity.Y -= 0.5f;
                    if (Main.rand.Next(4) == 0) {
                        dust4.noGravity = false;
                        dust4.scale *= 0.5f;
                    }
                }

                Lighting.AddLight((int)(npc.position.X / 16f), (int)(npc.position.Y / 16f + 1f), 1f, 0.3f, 0.1f);
            }
        }
    }
}