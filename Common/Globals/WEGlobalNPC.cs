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
using System.Reflection;
using WeaponEnchantments.ModLib.KokoLib;
using KokoLib;

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
        public bool oneForAllOrigin = true;
        float baseAmaterasuSpreadRange = 160f;
        public int amaterasuDamage = 0;
        private double lastAmaterasuTime = 0;
        public float amaterasuStrength = 0f;
        private bool amaterasuImmunityUpdated = false;
        public float myWarReduction = 1f;
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
                { NPCID.EaterofWorldsTail, 100f },
                { NPCID.TheDestroyer, 1f },
                { NPCID.TheDestroyerBody, 1f },
                { NPCID.TheDestroyerTail, 1f },
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
                    AddBossLoot(loot, npc, dropRule, bossBag);
                }
                else {
                    loot.Add(dropRule);
                }
            }

            //Enchantments and other boss drops
            if (npc.boss || multipleSegmentBoss) {
                //Boss Drops

                //Superior Containment
                int denominator = (int)(500000 / total);
                if (denominator < 1)
                    denominator = 1;

                dropRule = ItemDropRule.Common(ModContent.ItemType<SuperiorContainment>(), denominator, 1, 1);
                AddBossLoot(loot, npc, dropRule, bossBag);

                //Power Booster
                bool preHardModeBoss = preHardModeBossTypes.Contains(npc.type) || preHardModeModBossNames.Contains(npc.FullName);
                if (!WEMod.serverConfig.PreventPowerBoosterFromPreHardMode || !preHardModeBoss) {
                    denominator = (int)(1000000 / total);
                    if (denominator < 1)
                        denominator = 1;

                    dropRule = ItemDropRule.Common(ModContent.ItemType<PowerBooster>(), denominator, 1, 1);
                    AddBossLoot(loot, npc, dropRule, bossBag);
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
                    dropRule = ItemDropRule.OneFromOptions(denominator, itemTypes.ToArray());

                    AddBossLoot(loot, npc, dropRule, bossBag);
                }
                else if (itemTypes.Count > 0) {
                    //One drop option
                    dropRule = ItemDropRule.Common(itemTypes[0], denominator);

                    AddBossLoot(loot, npc, dropRule, bossBag);
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

                /*//Ai style drops from attibutes
                DropRulesAttribute.npcAiStyleDrops.TryGetValue(npc.aiStyle, out ICollection<int> aiBasedDrops);
                if (aiBasedDrops != null) {
                    foreach (int dropID in aiBasedDrops) {
                        loot.Add(ItemDropRule.Common(dropID, defaultDenom, 1, 1));
                    }
                }

                //npc type drops from attributes
                DropRulesAttribute.npcTypeDrops.TryGetValue(npc.type, out ICollection<int> mobBasedDrops);
                if (mobBasedDrops != null) {
                    foreach (int dropID in mobBasedDrops) {
                        loot.Add(ItemDropRule.Common(dropID, defaultDenom, 1, 1));
                    }
                }*/

                //Ai Style
                switch (npc.aiStyle) {
                    case 1://Slime
                        loot.Add(ItemDropRule.Common(ModContent.ItemType<DamageEnchantmentBasic>(), defaultDenom, 1, 1));
                        break;
                    case 2://Demon Eye
                        loot.Add(ItemDropRule.Common(ModContent.ItemType<ScaleEnchantmentBasic>(), defaultDenom, 1, 1));
                        break;
                    case 3://Fighter
                        loot.Add(ItemDropRule.Common(ModContent.ItemType<DefenseEnchantmentBasic>(), defaultDenom, 1, 1));
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
        private static void AddBossLoot(ILoot loot, NPC npc, IItemDropRule dropRule, bool bossBag) {
            //Setup mod boss bag support (Relies on NPC loot being set up before boss bag loot)
            if (!GlobalBossBags.modBossBagIntegrationSetup) {
                GlobalBossBags.SetupModBossBagIntegration();
                GlobalBossBags.modBossBagIntegrationSetup = true;
            }

            bool npcCantDropBossBags;

            switch (npc.type) {
                //UnobtainableBossBags
                case NPCID.CultistBoss:
                case NPCID.DD2DarkMageT1:
                case NPCID.DD2OgreT2:
                    npcCantDropBossBags = true;
                    break;
                default:
                    npcCantDropBossBags = !GlobalBossBags.bossBagNPCIDs.Values.Contains(npc.type);
                    break;
            }

            if (bossBag || npcCantDropBossBags) {
                loot.Add(dropRule);
			}
			else {
                loot.Add(new DropBasedOnExpertMode(dropRule, ItemDropRule.DropNothing()));
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
            float hpReductionFactor = EnchantedWeaponStaticMethods.GetReductionFactor((int)hp);
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

            float NPCHitCooldownMultiplier = SourceItem.ApplyEStat("NPCHitCooldown", 1f);

            //npc.immune
            int newImmune = (int)((float)npc.immune[player.whoAmI] * NPCHitCooldownMultiplier);
            if (newImmune < 1)
                newImmune = 1;

            npc.immune[player.whoAmI] = newImmune;
        }
        public static Dictionary<int, float> SortNPCsByRange(NPC npc, float range) {
            Dictionary<int, float> npcs = new Dictionary<int, float>();
            foreach (NPC target in Main.npc) {
                if (!npc.active)
                    continue;

                if (target.whoAmI != npc.whoAmI) {
                    if (target.friendly || target.townNPC || target.type == NPCID.DD2LanePortal)
                        continue;

                    Vector2 vector2 = target.Center - npc.Center;
                    float distanceFromOrigin = vector2.Length();
                    if (distanceFromOrigin <= range) {
                        npcs.Add(target.whoAmI, distanceFromOrigin);
                    }
                }
            }

            return npcs;
        }
        public static void StrikeNPC(NPC npc, int damage, bool crit) {
            if(npc.active && npc.life > 0)
                npc.StrikeNPC(damage, 0, 0, crit, false, true);
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
            if (!npc.HasBuff<AmaterasuDebuff>())
                return;

			#region Debug
            
			if (LogMethods.debugging) ($"\\/UpdateLifeRegen(npc: {npc.S()}, damage: {damage} amaterasuDamage: {amaterasuDamage} amaterasuStrength: {amaterasuStrength} npc.life: {npc.life} npc.liferegen: {npc.lifeRegen}").Log();

            #endregion
            
            bool isWorm = IsWorm(npc);
            int minSpreadDamage = isWorm ? 13 : 100;

            //Controls how fast the damage tick rate is.
            damage += amaterasuDamage / 240;

            //Set damage over time (amaterasuStrength is the EnchantmentStrength affected by config values.)
            int lifeRegen = (int)(((float)amaterasuDamage / 30f) * amaterasuStrength);
            npc.lifeRegen -= lifeRegen;

            //Fix for bosses not dying from Amaterasu
            if (npc.boss || multipleSegmentBossTypes.ContainsKey(npc.type)) {
                if (npc.life + npc.lifeRegen < 1 && npc.lifeRegen < 0)
                    npc.lifeRegen = 0;
            }

            //Spread to other enemies ever 10 ticks
            if (lastAmaterasuTime + 10 <= Main.GameUpdateCount && npc.type != NPCID.TargetDummy) {
                if(amaterasuDamage > minSpreadDamage) {
                    Dictionary<int, float> npcs = SortNPCsByRange(npc, baseAmaterasuSpreadRange);
                    foreach (int whoAmI in npcs.Keys) {
                        NPC mainNPC = Main.npc[whoAmI];
                        WEGlobalNPC wEGlobalNPC = Main.npc[whoAmI].GetWEGlobalNPC();
                        if (IsWorm(mainNPC)) {
                            if (wEGlobalNPC.amaterasuDamage <= 0)
                                wEGlobalNPC.amaterasuDamage++;
                        }
						else {
                            wEGlobalNPC.amaterasuDamage += 5;
                        }

                        if (!wEGlobalNPC.amaterasuImmunityUpdated) {
                            AmaterasuDebuff.ForceUpdate(mainNPC);
                            wEGlobalNPC.amaterasuStrength = amaterasuStrength;
                            mainNPC.AddBuff(ModContent.BuffType<AmaterasuDebuff>(), int.MaxValue);
                            wEGlobalNPC.amaterasuImmunityUpdated = true;
                        }
                    }
                }

                amaterasuDamage++;
                npc.AddBuff(ModContent.BuffType<AmaterasuDebuff>(), int.MaxValue, true);
                lastAmaterasuTime = Main.GameUpdateCount;
            }
			else {
                //Amaeterasu damage goes up over time on its own.
                if (!isWorm)
                    amaterasuDamage++;
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
            if(npc.HasBuff<AmaterasuDebuff>()) {
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
        public static bool IsWorm(NPC npc) {
            return npc.aiStyle == NPCAIStyleID.Worm || npc.aiStyle == NPCAIStyleID.TheDestroyer;
        }
    }
}