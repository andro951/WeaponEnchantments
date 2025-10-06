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
using WeaponEnchantments.Common.Configs;
using System.Diagnostics;
using WeaponEnchantments.ModIntegration;
using androLib.Common.Utility;
using androLib.ModIntegration;
using static androLib.ModIntegration.BossChecklistIntegration;
using androLib;
using androLib.Common.Globals;

namespace WeaponEnchantments.Common.Globals
{
	public class WEGlobalNPC : AndroGlobalNPC {

        #region Static

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
        float baseAmaterasuSpreadRange = 160f;
        public int amaterasuDamage = 0;
        private double lastAmaterasuTime = 0;
        public float amaterasuStrength = 0f;
        private bool amaterasuImmunityUpdated = false;
        public float myWarReduction = 1f;
        public override bool InstancePerEntity => true;
        public override void Load() {
            IL_Projectile.Damage += HookDamage;
            AndroGlobalNPC.GetLootActions += GetWELoot;
			AndroMod.OnResetGameCounter += () => lastAmaterasuTime = 0;
		}
        private static void HookDamage(ILContext il) {
            bool debuggingHookDamage = false;

            var c = new ILCursor(il);

            //Find location of where crit chance is calculated.
            if (!c.TryGotoNext(MoveType.After,
				//i => i.MatchLdloc(8),//i => i.MatchLdloc(36),
				//i => i.MatchBrfalse(out _),
                i => i.MatchLdarg(0),
                i => i.MatchCall(out _),
                i => i.MatchCallvirt(out _),
                i => i.MatchBrfalse(out _),
				i => i.MatchCall(out _),
                i => i.MatchLdcI4(100),
                i => i.MatchCallvirt(out _),
                i => i.MatchLdarg(0),
                i => i.MatchCall(out _)
			)) { throw new Exception("Failed to find instructions HookDamage 1/2"); }

            if (debuggingHookDamage) try { ModContent.GetInstance<WEMod>().Logger.Info("c.Index: " + c.Index.ToString() + " Instruction: " + c.Next.ToString()); } catch (Exception e) { ModContent.GetInstance<WEMod>().Logger.Info("c.Index: " + c.Index.ToString() + " exception: " + e.ToString()); }

            if (debuggingHookDamage) try { ModContent.GetInstance<WEMod>().Logger.Info("c.Index: " + (c.Index - 1).ToString() + " Instruction: " + c.Prev.ToString()); } catch (Exception e) { ModContent.GetInstance<WEMod>().Logger.Info("c.Index: " + (c.Index - 1).ToString() + " exception: " + e.ToString()); }

            //Set crit chance to zero.
            c.Emit(OpCodes.Pop);
            c.Emit(OpCodes.Ldc_I4_0);

			if (!c.TryGotoNext(MoveType.Before,
                i => i.MatchCall(out _),
				i => i.MatchStloc(39)
			//i => i.MatchLdloc(7),
			//i => i.MatchLdarg(3),
			//i => i.MatchLdcI4(1),
			//i => i.MatchLdarg(0),
			//i => i.MatchLdfld<Player>("luck")
			)) { throw new Exception("Failed to find instructions HookDamage 2/2"); }

            c.Emit(OpCodes.Ldloca, 28);
			c.Emit(OpCodes.Ldloc, 30);
			c.Emit(OpCodes.Ldarg, 0);

			c.EmitDelegate((ref NPC.HitModifiers hitModifiers, bool crit, Projectile projectile) => {
                if (projectile.TryGetWEPlayer(out WEPlayer wePlayer) && projectile.TryGetWEProjectile(out WEProjectile weProjectile) && weProjectile.sourceItem is Item item && !item.NullOrAir()) {
					FieldInfo info = typeof(NPC.HitModifiers).GetField("_critOverride", BindingFlags.NonPublic | BindingFlags.Instance);
					bool? critOverride = (bool?)info.GetValue(hitModifiers);
					wePlayer.CalculateCriticalChance(item, ref hitModifiers, crit, critOverride, projectile);
				}
			});
		}
		private void GetWELoot(ILoot loot, NPC npc, float hp, float value, float total, bool boss, bool bossBag = false) {
			if (total <= 0f)
				return;

			bool normalNpcThatDropsBag = normalNpcsThatDropsBags.Contains(npc.netID);

            GetEssenceDropList(npc, normalNpcThatDropsBag, value, hp, total, out float[] essenceValues, out float[] dropRate);

            IItemDropRule dropRule;

            //Essence
            for (int i = 0; i < essenceValues.Length; ++i) {
                float thisDropRate = dropRate[i];

                if (thisDropRate <= 0f)
                    continue;

                //Denom
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

                if (boss) {
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
                int denominator = (int)(10000f / total);
                if (denominator < 1)
                    denominator = 1;

                dropRule = ItemDropRule.Common(ModContent.ItemType<SuperiorContainment>(), denominator, 1, 1);
                AddBossLoot(loot, npc, dropRule, bossBag);

                //Power Booster
                float powerBoosterDropChance = total / 25000f;
                dropRule = new PowerBoosterDropRule(npc.netID, powerBoosterDropChance);
				AddBossLoot(loot, npc, dropRule, bossBag);
            }

            //Calling base would duplicate loot.
        }
		public override bool UseDefaultDropChance(NPC npc) {
			switch (npc.aiStyle) {
				case NPCAIStyleID.Mimic:
				case NPCAIStyleID.BiomeMimic:
					return false;
			}

            return true;
		}
        public static void GetEssenceDropList(NPC npc, bool normalNPCThatDropsBossBag, float value, float hp, float total, out float[] essenceValues, out float[] dropRate) {
            //Prevent low value enemies like critters from dropping essence
            if (value <= 0 && hp <= 10) {
                dropRate = null;
                essenceValues = null;
                return;
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
                    if (target.friendly || target.townNPC || target.dontTakeDamage)
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
                npc.SimpleStrikeNPC(damage, 0, crit: crit);
        }
        public override void OnSpawn(NPC npc, IEntitySource source) {
            if (!war || npc.friendly || npc.townNPC || npc.boss || warReduction <= 1f)
                return;

			CursedModSystem.PreUpdateNPCActions += () => NetManager.ApplyWarPenalties(npc, warReduction);
		}
        public static void ApplyWarPenalties(NPC npc, float warReduction) {
            if (!npc.TryGetWEGlobalNPC(out WEGlobalNPC weGlobalNPC))
                return;

            weGlobalNPC.myWarReduction = warReduction;
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
			if (lastAmaterasuTime + 10 <= Main.GameUpdateCount) {
                if (!npc.IsDummy() && amaterasuDamage > minSpreadDamage) {
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
        public void ResetWarReduction() {
			myWarReduction = 1f;
		}
	}

    public static class NPCStaticMethods
    {
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
	}
}