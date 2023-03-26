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
using static WeaponEnchantments.Common.Globals.NPCStaticMethods;
using WeaponEnchantments.Items.Utility;
using WeaponEnchantments.Common.Configs;
using System.Diagnostics;
using WeaponEnchantments.ModIntegration;
using static WeaponEnchantments.ModIntegration.BossChecklistIntegration;

namespace WeaponEnchantments.Common.Globals
{
    public class WEGlobalNPC : GlobalNPC {
        #region Static

        private static SortedSet<int> preHardModeBossTypes = null;
		public static SortedSet<int> PreHardModeBossTypes {
            get {
                TryBossChecklistSetup();
				if (preHardModeBossTypes == null)
                    DefaultBossSetup();

                return preHardModeBossTypes;
			}
        }
		private static SortedSet<int> postPlanteraBossTypes = null;
		public static SortedSet<int> PostPlanteraBossTypes {
			get {
				TryBossChecklistSetup();
				if (postPlanteraBossTypes == null)
					DefaultBossSetup();

				return postPlanteraBossTypes;
			}
		}
		private static void DefaultBossSetup() {
            if (preHardModeBossTypes == null || postPlanteraBossTypes == null) {
				preHardModeBossTypes = new() {
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

				postPlanteraBossTypes = new() {
					NPCID.HallowBoss,
					NPCID.CultistBoss,
					NPCID.MoonLordCore,
					NPCID.MoonLordHead,
					NPCID.Plantera,
					NPCID.Golem,
					NPCID.DukeFishron,
					NPCID.MartianSaucer
				};
			}
        }
        private static void TryBossChecklistSetup() {
            if (!WEMod.bossChecklistEnabled) {
                if (preHardModeBossTypes == null || postPlanteraBossTypes == null)
                    $"BossChecklist mod is not enabled.  Weapon Enchantments uses BossChecklist to determin which bosses determin Power Booster drops from Modded bosses.  Since BossChecklist is not enabled, all Modded bosses will drop the regular Power Booster.".LogSimple();

				return;
			}

            if (!ShouldSetupBossPowerBoosterDrops)
                return;

			preHardModeBossTypes = new();
			postPlanteraBossTypes = new();
			if (!BossInfoNetIDKeys.TryGetValue(NPCID.WallofFlesh, out string wallKey) || !BossInfos.TryGetValue(wallKey, out BossChecklistBossInfo wallInfo) || !BossInfoNetIDKeys.TryGetValue(NPCID.Plantera, out string planteraKey) || !BossInfos.TryGetValue(planteraKey, out BossChecklistBossInfo planteraInfo)) {
                "Failed to determine the progression of Wall of Flesh and Plantera from BossChecklistData".LogSimple();
                return;
            }

            float wallOfFleshProgression = wallInfo.progression;
            float planteraProgression = planteraInfo.progression;
			foreach (KeyValuePair<string, BossChecklistBossInfo> bossInfoPair in BossInfos.Where(p => p.Value.isBoss || p.Value.isMiniboss)) {
                float progression = bossInfoPair.Value.progression;
                List<int> netIDs = bossInfoPair.Value.npcIDs;
                if (netIDs.Count < 1)
                    continue;

                NPC npc = netIDs.First().CSNPC();
                if (progression < wallOfFleshProgression) {
                    preHardModeBossTypes.UnionWith(netIDs);
				}
                else if (progression >= planteraProgression) {
                    postPlanteraBossTypes.UnionWith(netIDs);
                }
            }

            if (Debugger.IsAttached) $"{BossInfos.OrderBy(i => i.Value.progression).Select(i => $"Key: {i.Key}, internalName: {i.Value.internalName}, progression: {i.Value.progression}, netIDs: {i.Value.npcIDs.StringList(netID => netID.GetNPCIDOrName())}").S("BossChecklist BossInfos")}".LogSimple();

            UsedBossChecklistForBossPowerBoosterDrops = true;
		}
        public static SortedDictionary<int, float> multipleSegmentBossTypes;
        public static List<int> normalNpcsThatDropsBags;

        static bool war = false;
        static float warReduction = 1f;
        public static SortedDictionary<int, List<DropData>> npcDropTypes = new();
        public static SortedDictionary<int, List<DropData>> npcAIDrops = new();

		private static SortedDictionary<int, List<(int, float)>> allItemDropsFromNpcs = null;
		public static SortedDictionary<int, List<(int, float)>> AllItemDropsFromNpcs {
			get {
				if (allItemDropsFromNpcs == null)
					GetAllNpcDrops();

				return allItemDropsFromNpcs;
			}
		}
        public static bool UsedBossChecklist = false;

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

            multipleSegmentBossTypes = new SortedDictionary<int, float>() {
                { NPCID.EaterofWorldsHead, 100f },
                { NPCID.EaterofWorldsBody, 100f },
                { NPCID.EaterofWorldsTail, 100f },
                { NPCID.TheDestroyer, 1f },
                { NPCID.TheDestroyerBody, 1f },
                { NPCID.TheDestroyerTail, 1f },
            };

            normalNpcsThatDropsBags = new List<int>() {
                NPCID.DD2Betsy
            };

            if (WEMod.thoriumEnabled) {
                normalNpcsThatDropsBags.Add(NPCID.DD2DarkMageT1);
				normalNpcsThatDropsBags.Add(NPCID.DD2DarkMageT3);
				normalNpcsThatDropsBags.Add(NPCID.DD2OgreT2);
				normalNpcsThatDropsBags.Add(NPCID.DD2OgreT3);
				normalNpcsThatDropsBags.Add(NPCID.PirateShip);
			}
        }
        private static void HookDamage(ILContext il) {
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
                    //itemTypes.Add(ModContent.ItemType<DamageEnchantmentBasic>());
                    break;
                case NPCID.EyeofCthulhu when !bossBag:
                case ItemID.EyeOfCthulhuBossBag when bossBag:
                    //itemTypes.Add(ModContent.ItemType<MovementSpeedEnchantmentBasic>());
                    break;
                case NPCID.EaterofWorldsHead when !bossBag:
                case NPCID.EaterofWorldsBody when !bossBag:
                case NPCID.EaterofWorldsTail when !bossBag:
                case ItemID.EaterOfWorldsBossBag when bossBag:
                    //itemTypes.Add(ModContent.ItemType<AttackSpeedEnchantmentBasic>());
                    break;
                case NPCID.BrainofCthulhu when !bossBag:
                case ItemID.BrainOfCthulhuBossBag when bossBag:
                    //itemTypes.Add(ModContent.ItemType<ReducedManaUsageEnchantmentBasic>());
                    break;
                case NPCID.QueenBee when !bossBag:
                case ItemID.QueenBeeBossBag when bossBag:
                    //itemTypes.Add(ModContent.ItemType<MaxMinionsEnchantmentBasic>());
                    break;
                case NPCID.SkeletronHead when !bossBag:
                case ItemID.SkeletronBossBag when bossBag:
                    //itemTypes.Add(ModContent.ItemType<CriticalStrikeChanceEnchantmentBasic>());
                    break;
                case NPCID.Deerclops when !bossBag:
                case ItemID.DeerclopsBossBag when bossBag:
                    //itemTypes.Add(ModContent.ItemType<SolarDashEnchantmentBasic>());
                    break;
                case NPCID.WallofFlesh when !bossBag:
                case ItemID.WallOfFleshBossBag when bossBag:
                    //itemTypes.Add(ModContent.ItemType<LifeStealEnchantmentBasic>());
                    //itemTypes.Add(ModContent.ItemType<ArmorPenetrationEnchantmentBasic>());
                    break;
                case NPCID.QueenSlimeBoss when !bossBag:
                case ItemID.QueenSlimeBossBag when bossBag:
                    //itemTypes.Add(ModContent.ItemType<HellsWrathEnchantmentBasic>());
                    break;
                case NPCID.Retinazer when !bossBag:
                case ItemID.TwinsBossBag when bossBag:
                case NPCID.Spazmatism when !bossBag:
                    //itemTypes.Add(ModContent.ItemType<WorldAblazeEnchantmentBasic>());
                    break;
                case NPCID.TheDestroyer when !bossBag:
                case ItemID.DestroyerBossBag when bossBag:

                    break;
                case NPCID.SkeletronPrime when !bossBag:
                case ItemID.SkeletronPrimeBossBag when bossBag:
                    //itemTypes.Add(ModContent.ItemType<ColdSteelEnchantmentBasic>());
                    break;
                case NPCID.Plantera when !bossBag:
                case ItemID.PlanteraBossBag when bossBag:
                    //itemTypes.Add(ModContent.ItemType<JunglesFuryEnchantmentBasic>());
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
                    //itemTypes.Add(ModContent.ItemType<MoonlightEnchantmentBasic>());
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
                case NPCID.WallofFlesh:
                case NPCID.MoonLordCore:
                case NPCID.TorchGod:
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

			bool normalNpcThatDropsBag = normalNpcsThatDropsBags.Contains(npc.netID);

			GetEssenceDropList(npc, normalNpcThatDropsBag, out float[] essenceValues, out float[] dropRate, out float hp, out float total);

            if (total <= 0f)
                return;

            IItemDropRule dropRule;

            bool multipleSegmentBoss = multipleSegmentBossTypes.ContainsKey(npc.netID);
            float multipleSegmentBossMultiplier = GetMultiSegmentBossMultiplier(npc.netID);
            bool boss = npc.boss || multipleSegmentBoss || normalNpcThatDropsBag;

            if (multipleSegmentBoss && bossBag)
                total *= multipleSegmentBossMultiplier;

            //Essence
            for (int i = 0; i < essenceValues.Length; ++i) {
                float thisDropRate = dropRate[i];

                if (thisDropRate <= 0f)
                    continue;

                //Multi-segment boss bag
                if (multipleSegmentBoss && bossBag)
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

                dropRule = ItemDropRule.Common(EnchantmentEssence.IDs[i], denominator, minDropped, maxDropped);

                if (npc.boss || multipleSegmentBoss || normalNpcThatDropsBag) {
                    //Boss or multisegmented boss that doesn't technically count as a boss.
                    AddBossLoot(loot, npc, dropRule, bossBag);
                }
                else {
                    loot.Add(dropRule);
                }
            }

            //Enchantments and other boss drops
            if (boss) {
                //Boss Drops

                //Superior Containment
                int denominator = (int)(50000f / total);
                if (denominator < 1)
                    denominator = 1;

                dropRule = ItemDropRule.Common(ModContent.ItemType<SuperiorContainment>(), denominator, 1, 1);
                AddBossLoot(loot, npc, dropRule, bossBag);

                //Power Booster
                bool preHardModeBoss = PreHardModeBossTypes.Contains(npc.netID);
                bool postPlanteraBoss = PostPlanteraBossTypes.Contains(npc.netID);
                float powerBoosterDropChance = total / 100000f;
                dropRule = new PowerBoosterDropRule(npc.netID, powerBoosterDropChance);
				AddBossLoot(loot, npc, dropRule, bossBag);

                //Enchantments
                if (npcDropTypes.ContainsKey(npc.netID)) {
                    //Enchantment drop chance
                    float chance = GetEnchantmentDropChance(npc.netID, bossBag);
                    if (npcDropTypes[npc.netID].Count == 1)
                        chance *= npcDropTypes[npc.netID][0].Weight;

                    List<IItemDropRule> dropRules = GetDropRules(chance, npcDropTypes[npc.netID], WEMod.serverConfig.BossEnchantmentDropChance);
                    foreach(IItemDropRule rule in dropRules) {
						AddBossLoot(loot, npc, rule, bossBag);
					}

					if (LogModSystem.printEnchantmentDrops && (bossBag || AllItemDropsFromNpcs != null && !AllItemDropsFromNpcs.Values.SelectMany(l => l).Select(p => p.Item1).Contains(npc.netID)))
                        LogModSystem.npcEnchantmentDrops.AddOrCombine(npc.netID, (chance, npcDropTypes[npc.netID]));
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

                bool useDefaultChance = true;
                switch (npc.aiStyle) {
                    case NPCAIStyleID.Mimic:
                    case NPCAIStyleID.BiomeMimic:
                        useDefaultChance = false;
                        break;
                }

                float chance;
                if (useDefaultChance) {
                    chance = (total * mult) / (5000f + hp * 0.5f);
				}
				else {
                    chance = 1f;
				}

                if (npcDropTypes.ContainsKey(npc.netID)) {
                    if (npcDropTypes[npc.netID].Where(d => d.Chance <= 0f).Count() == 1)
                        chance *= npcDropTypes[npc.netID][0].Weight;

					List<IItemDropRule> dropRules = GetDropRules(chance, npcDropTypes[npc.netID], EnchantmentDropChance);
					foreach (IItemDropRule rule in dropRules) {
						loot.Add(rule);
					}

                    if (LogModSystem.printEnchantmentDrops)
                        LogModSystem.npcEnchantmentDrops.AddOrCombine(npc.netID, (chance, npcDropTypes[npc.netID]));
                }

                if (npcAIDrops.ContainsKey(npc.aiStyle)) {
                    if (npcAIDrops[npc.aiStyle].Count == 1)
                        chance *= npcAIDrops[npc.aiStyle][0].Weight;

					List<IItemDropRule> dropRules = GetDropRules(chance, npcAIDrops[npc.aiStyle], EnchantmentDropChance);
					foreach (IItemDropRule rule in dropRules) {
						loot.Add(rule);
					}

					if (LogModSystem.printEnchantmentDrops)
                        LogModSystem.npcEnchantmentDrops.AddOrCombine(npc.netID, (chance, npcAIDrops[npc.aiStyle]));
                }
            }
        }
        public static List<IItemDropRule> GetDropRules(float chance, IEnumerable<DropData> dropData, float configChance) {
            List<IItemDropRule> itemDropRules = new();
            IEnumerable<DropData> weightedDrops = dropData.Where(d => d.Chance <= 0f);
			IEnumerable<DropData> basicDrops = dropData.Where(d => d.Chance > 0f);
            if (weightedDrops.Count() > 0)
                itemDropRules.Add(new OneFromWeightedOptionsNotScaledWithLuckDropRule(chance, weightedDrops));

            foreach(DropData data in basicDrops) {
                itemDropRules.Add(new BasicDropRule(data, configChance));
            }

            return itemDropRules;
		}
		private static void AddBossLoot(ILoot loot, NPC npc, IItemDropRule dropRule, bool bossBag) {
            bool bossCantDropBossBags = false;

            switch (npc.netID) {
                //UnobtainableBossBags
                case NPCID.CultistBoss:
                case NPCID.DD2DarkMageT1:
                case NPCID.DD2OgreT2:
                    bossCantDropBossBags = !WEMod.thoriumEnabled;
                    break;
            }

            if (bossBag || bossCantDropBossBags) {
                loot.Add(dropRule);
            }
            else {
                loot.Add(new DropBasedOnExpertMode(dropRule, ItemDropRule.DropNothing()));
            }
        }
        public static void GetEssenceDropList(NPC npc, bool normalNPCThatDropsBossBag, out float[] essenceValues, out float[] dropRate, out float hp, out float total) {
            //Defense
            float defenseMultiplier = 1f + (float)npc.defDefense / 40f;

            //HP
            int lifeMax = npc.RealLifeMax();
            hp = (float)lifeMax * defenseMultiplier;

            //Value
            float value = npc.RealValue();

            //Prevent low value enemies like critters from dropping essence
            if (value <= 0 && hp <= 10) {
                total = 0;
                dropRate = null;
                essenceValues = null;
                return;
            }

            //Total
            if (value > 0) {
                total = hp + 0.2f * value;
            }
            else {
                total = hp * 2.6f;
                //Thorium bags for Dark Mage and Ogre only drop at a 25% rate.
                if (WEMod.thoriumEnabled && (npc.type == NPCID.DD2OgreT2 || npc.type == NPCID.DD2OgreT3 || npc.type == NPCID.DD2DarkMageT1 || npc.type == NPCID.DD2DarkMageT3))
                    total *= 4f;
            }

            //Hp reduction factor
            float hpReductionFactor = EnchantedWeaponStaticMethods.GetReductionFactor((int)hp);
            total /= hpReductionFactor;

            //NPC Characteristics Factors
            float noGravityFactor = npc.noGravity ? 0.2f : 0f;
            float noTileCollideFactor = npc.noTileCollide ? 0.2f : 0f;
            float knockBackResistFactor = 0.2f * npc.knockBackResist;
            float npcCharacteristicsFactor = 1f + noGravityFactor + noTileCollideFactor + knockBackResistFactor;
            total *= npcCharacteristicsFactor;

            //Balance Multiplier (Extra multiplier for us to control the values manually)
            float balanceMultiplier = 0.2f;
            total *= balanceMultiplier;

            //Modify total for specific enemies.
            switch (npc.netID) {
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
            bool multiSegmentBoss = multipleSegmentBossTypes.ContainsKey(npc.netID);

            //Config Multiplier
            if (npc.boss || normalNPCThatDropsBossBag || multiSegmentBoss) {
                essenceTotal *= BossEssenceMultiplier;
            }
            else {
                essenceTotal *= EssenceMultiplier;
            }

            essenceValues = EnchantmentEssence.values;
            dropRate = new float[essenceValues.Length];
            int essenceTier = 0;

            //Calculate the main essence tier that will be dropped.
            if (npc.boss || normalNPCThatDropsBossBag) {
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
        public static Dictionary<int, float> SortNPCsByRange(NPC npc, float range) {
            Dictionary<int, float> npcs = new Dictionary<int, float>();
            foreach (NPC target in Main.npc) {
                if (!target.active)
                    continue;

                if (target.whoAmI != npc.whoAmI) {
                    if (target.friendly || target.townNPC || target.netID == NPCID.DD2LanePortal)
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
            if (npc.active && npc.RealLife() > 0)
                npc.StrikeNPC(damage, 0, 0, crit, false, true);
        }
        public override void OnSpawn(NPC npc, IEntitySource source) {
            if (!war || npc.friendly || npc.townNPC || npc.boss)
                return;

            //Apply war downsides
            myWarReduction = warReduction;
            npc.lavaImmune = true;
            npc.trapImmune = true;
        }
        public override void UpdateLifeRegen(NPC npc, ref int damage) {
            if (!npc.HasBuff<Amaterasu>())
                return;

            #region Debug

            if (LogMethods.debugging) ($"\\/UpdateLifeRegen(npc: {npc.S()}, damage: {damage} amaterasuDamage: {amaterasuDamage} amaterasuStrength: {amaterasuStrength} npc.life: {npc.RealLife()} npc.liferegen: {npc.lifeRegen}").Log();

            #endregion

            bool isWorm = npc.IsWorm();
            int minSpreadDamage = isWorm ? 13 : 100;

            //Controls how fast the damage tick rate is.
            damage += (int)((float)amaterasuDamage / 240f * amaterasuStrength);

            //Set damage over time (amaterasuStrength is the EnchantmentStrength affected by config values.)
            int lifeRegen = (int)((float)amaterasuDamage / 30f * amaterasuStrength);
            npc.lifeRegen -= lifeRegen;

            int life = npc.RealLife();

			
			if (life - damage < 1) {
                if (life > 0)
				    damage = life;

				//Fix for bosses not dying from Amaterasu
				if (npc.realLife != -1 && life <= 1)
                    StrikeNPC(npc, 1, false);
			}

			//Spread to other enemies ever 10 ticks
			if (lastAmaterasuTime + 10 <= Main.GameUpdateCount && npc.netID != NPCID.TargetDummy) {
                if (amaterasuDamage > minSpreadDamage) {
                    Dictionary<int, float> npcs = SortNPCsByRange(npc, baseAmaterasuSpreadRange);
                    foreach (int whoAmI in npcs.Keys) {
                        NPC mainNPC = Main.npc[whoAmI];
                        WEGlobalNPC wEGlobalNPC = Main.npc[whoAmI].GetWEGlobalNPC();
                        if (mainNPC.IsWorm()) {
                            if (wEGlobalNPC.amaterasuDamage <= 0)
                                wEGlobalNPC.amaterasuDamage += AmaterasuSelfGrowthPerTick/5;
                        }
                        else {
                            wEGlobalNPC.amaterasuDamage += AmaterasuSelfGrowthPerTick;
                        }

                        if (!wEGlobalNPC.amaterasuImmunityUpdated) {
                            Amaterasu.RemoveImmunities(mainNPC);
                            wEGlobalNPC.amaterasuStrength = amaterasuStrength;
                            mainNPC.AddBuff(ModContent.BuffType<Amaterasu>(), 10000);
                            wEGlobalNPC.amaterasuImmunityUpdated = true;
                        }
                    }
                }

                if (isWorm) {
                    amaterasuDamage += AmaterasuSelfGrowthPerTick/5;
                }
				else {
                    amaterasuDamage += AmaterasuSelfGrowthPerTick;
                }
                
                npc.AddBuff(ModContent.BuffType<Amaterasu>(), 10000, true);
                lastAmaterasuTime = Main.GameUpdateCount;
            }
            else {
                //Amaeterasu damage goes up over time on its own.
                if (!isWorm)
                    amaterasuDamage++;
            }

            #region Debug

            if (LogMethods.debugging) ($"/\\UpdateLifeRegen(npc: {npc.S()}, damage: {damage} amaterasuDamage: {amaterasuDamage} amaterasuStrength: {amaterasuStrength} npc.life: {npc.RealLife()} npc.liferegen: {npc.lifeRegen }").Log();

            #endregion
        }
        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns) {

            #region Debug

            if (LogMethods.debugging) ($"\\/EditSpawnRate(" + player.name + ", spawnRate: " + spawnRate + ", maxSpawns: " + maxSpawns + ")").LogT();

            #endregion

            //Spawn Rate
            if (player.GetWEPlayer().CheckEnchantmentStats(EnchantmentStat.EnemySpawnRate, out float enemySpawnRateBonus, 1f)) {
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
            if (player.GetWEPlayer().CheckEnchantmentStats(EnchantmentStat.EnemyMaxSpawns, out float enemyMaxSpawnBonus, 1f)) {
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
            if (npc.HasBuff<Amaterasu>()) {
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
        public void ResetWarReduction() => myWarReduction = 1f;
		private static void GetAllNpcDrops() {
            if (!WEModSystem.StartedPostAddRecipes)
                return;

			allItemDropsFromNpcs = new();
            SortedDictionary<string, (int, float)> manuallySetModBossBags = new();
			foreach (KeyValuePair<int, NPC> npcPair in ContentSamples.NpcsByNetId) {
				int netID = npcPair.Key;
				NPC npc = npcPair.Value;
				IEnumerable<IItemDropRule> dropRules = Main.ItemDropsDB.GetRulesForNPCID(netID, false);
				foreach (IItemDropRule dropRule in dropRules) {
					List<DropRateInfo> dropRates = new();
					DropRateInfoChainFeed dropRateInfoChainFeed = new(1f);
					dropRule.ReportDroprates(dropRates, dropRateInfoChainFeed);
					foreach (DropRateInfo dropRate in dropRates) {
						int itemType = dropRate.itemId;
                        float chance = dropRate.dropRate;
						//Item item = itemType.CSI();
						allItemDropsFromNpcs.AddOrCombine(itemType, (netID, chance));
					}
				}

                if (GlobalBossBags.ManuallySetModBossBags.TryGetValue(npc.ModFullName(), out (string bagModName, float chance) value))
                    manuallySetModBossBags.Add(value.bagModName, (netID, value.chance));
			}

			for (int i = 0; i < ItemLoader.ItemCount; i++) {
				if (manuallySetModBossBags.Count <= 0)
					break;

				string modFullName = i.CSI().ModFullName();
				if (manuallySetModBossBags.TryGetValue(modFullName, out (int netID, float chance) value)) {
                    allItemDropsFromNpcs.AddOrCombine(i, (value.netID, value.chance));
					manuallySetModBossBags.Remove(modFullName);
				}
			}

			//if (Debugger.IsAttached) allItemDropsFromNpcs.Select(p => p.Value.StringList(n => $"{n.Item1.CSNPC().S()} : {n.Item2.PercentString()}" , p.Key.CSI().S())).S("allItemDropsFromNpcs").LogSimple();
		}
	}

    public static class NPCStaticMethods
    {
        public static bool IsWorm(this NPC npc) {
            return npc.aiStyle == NPCAIStyleID.Worm || npc.aiStyle == NPCAIStyleID.TheDestroyer;
        }
        public static bool IsDummy(this NPC npc) => npc.netID < NPCID.Count ? npc.netID == NPCID.TargetDummy : npc.ModFullName() is string modFullName && (WEMod.calamityEnabled && modFullName == "CalamityMod/SuperDummyNPC" || WEMod.fargosEnabled && modFullName == "Fargowiltas/SuperDummy");
        public static bool IsBoss(this NPC npc) => npc.boss || WEGlobalNPC.multipleSegmentBossTypes.ContainsKey(npc.netID) || WEGlobalNPC.normalNpcsThatDropsBags.Contains(npc.netID);
		public static void HandleOnHitNPCBuffs(this NPC target, int damage, float amaterasuStrength, Dictionary<short, int> debuffs, HashSet<short> dontDissableImmunitiy) {
			target.RemoveNPCBuffImunities(debuffs, dontDissableImmunitiy);

			if (target.TryGetWEGlobalNPC(out WEGlobalNPC wEGlobalNPC)) {
				wEGlobalNPC.amaterasuDamage += damage;
				wEGlobalNPC.amaterasuStrength = amaterasuStrength;
			}

			target.ApplyBuffs(debuffs);
		}
        public static void RemoveNPCBuffImunities(this NPC target, Dictionary<short, int> debuffs, HashSet<short> dontDissableImmunitiy) {
            HashSet<short> debuffIDs = new HashSet<short>(debuffs.Keys);
            if (dontDissableImmunitiy.Count > 0) {
                foreach (short id in dontDissableImmunitiy) {
                    debuffIDs.Remove(id);
                }
            }

            foreach (short id in debuffIDs) {
                target.buffImmune[id] = false;
            }
        }
        public static void ApplyBuffs(this NPC target, Dictionary<short, int> buffs) {
            foreach (KeyValuePair<short, int> buff in buffs) {
                target.AddBuff(buff.Key, buff.Value, true);
            }
        }
        public static void ApplyBuffs(this NPC target, SortedDictionary<short, BuffStats> buffs) {
            foreach (KeyValuePair<short, BuffStats> buff in buffs) {
                float chance = buff.Value.Chance;
                if (chance >= 1f || chance >= Main.rand.NextFloat()) {
                    target.AddBuff(buff.Key, buff.Value.Duration.Ticks);
                }
            }
        }
        public static int RealNetID(this NPC npc) => npc.realLife == -1 ? npc.netID : Main.npc[npc.realLife].netID;
        public static int RealLife(this NPC npc) => npc.realLife == -1 ? npc.life : Main.npc[npc.realLife].life;
		public static int RealLifeMax(this NPC npc) => npc.realLife == -1 ? npc.lifeMax : Main.npc[npc.realLife].lifeMax;
        public static int RealLifeRegen(this NPC npc) => npc.realLife == -1 ? npc.lifeRegen : Main.npc[npc.realLife].lifeRegen;
        public static float RealValue(this NPC npc) => npc.realLife == -1 ? npc.value : Main.npc[npc.realLife].value;
        public static void AddValue(this NPC npc, float value) {
            if (npc.realLife == -1) {
                npc.value += value;
			}
            else {
				Main.npc[npc.realLife].value += value;
			}
        }

		//Fix for Draedon boss having a null reference error in Calamity code when sampleNPC.FullName is called.  (Error in NPC.ToString())
		public static string FullName(this NPC npc) {
			if (WEMod.calamityEnabled) {
				string sampleModNPCFullName = npc.ModNPC?.FullName;
				switch (sampleModNPCFullName) {
					case "CalamityMod/ThanatosBody1":
					case "CalamityMod/ThanatosBody2":
						return "Draedon";
					case "CalamityMod/ThanatosTail":
						return "XM-05 Thanatos";
					default:
						return npc.FullName;
				}
			}
			else {
				return npc.FullName;
			}
		}
        public static string ModFullName(this NPC npc) => npc.ModNPC?.FullName ?? npc.type.GetNPCIDOrName();
	}
}