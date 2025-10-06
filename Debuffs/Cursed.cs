using androLib;
using androLib.Common.Globals;
using androLib.Common.Utility;
using log4net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Configs;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Content.Dusts;
using WeaponEnchantments.Content.Projectiles;
using WeaponEnchantments.Effects;
using WeaponEnchantments.Items;
using WeaponEnchantments.ModLib.KokoLib;

namespace WeaponEnchantments.Debuffs {
	public class Cursed : WEBuff {
		private Asset<Texture2D> animatedTexture;
		private const string AnimationSheetPath = $"{WEMod.ModName}/Debuffs/{nameof(Cursed)}_Animated";
		public override void SetStaticDefaults() {
			Main.debuff[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
			if (Main.netMode != NetmodeID.Server) {
				animatedTexture = ModContent.Request<Texture2D>(AnimationSheetPath);
			}

			AndroMod.RegisterBuffDescription(Type, GetDescription);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, int buffIndex, ref BuffDrawParams drawParams) {
			Texture2D texture = animatedTexture.Value;
			Rectangle source = texture.Frame(verticalFrames: CursedEssence.AnimationFrames, frameY: (int)Main.GameUpdateCount / CursedEssence.TicksPerFrame % CursedEssence.AnimationFrames);
			drawParams.Texture = texture;
			drawParams.SourceRectangle = source;

			return true;
		}
		private static string GetDescription(Player player) {
			int cursedEssenceCount = CurseAttractionNPC.PlayerCursedEssence[Main.myPlayer];
			CurseAttractionNPC.GetSpawnRateAndMaxSpawnsMultipliers(Main.LocalPlayer, out double spawnRateMult, out double maxSpawnsMult);
			double cursedSpawnChance = CurseAttractionNPC.GetCursedSpawnChance(Main.LocalPlayer.position);
			return $"{nameof(Cursed)}.{L_ID2.Description}".Lang_WE(L_ID1.Buffs, new object[] { cursedEssenceCount, ((float)spawnRateMult).S(2), ((float)maxSpawnsMult).S(2), ((float)cursedSpawnChance).PercentString() }); ;
		}
		public override string LocalizationDescription =>
			"Cursed Essence {0}\n" +
			"Spawn Rate x{1}\n" +
			"Max Spawns x{2}\n" +
			"Cursed Chance {3}";
	}

	public class CurseAttractionNPC : GlobalNPC {
		private const double DefaultCursedSpawnChance = 0.001;
		private const double CursedSpawnChanceIncrement = 0.01;
		public static int[] PlayerCursedEssence = new int[Main.player.Length];
		private const double MaxCursedSpawnChance = 0.25;
		private static double CursedEssenceLog2(int cursedEssence) => cursedEssence <= 0 ? 0.0 : Math.Log2((double)cursedEssence / 100.0 + 2.0);
		private static float CursedEssenceSpawnRateRange => 2000f;
		public static double GetCursedSpawnChance(Vector2 spawnPosition) {
			if (!WEMod.serverConfig.AllowCursedEnemies)
				return 0.0;

			int end = Main.netMode == NetmodeID.SinglePlayer ? 1 : Main.player.Length;
			int cursedEssence = 0;
			for (int i = 0; i < end; i++) {
				Player player = Main.player[i];
				if (player.NullOrNotActive() || player.dead)
					continue;

				float distance = player.position.Distance(spawnPosition);
				if (distance <= CursedEssenceSpawnRateRange)
					cursedEssence += PlayerCursedEssence[i];
			}

			double log = CursedEssenceLog2(cursedEssence);
			double cursedSpawnChance = Math.Min(MaxCursedSpawnChance, DefaultCursedSpawnChance + CursedSpawnChanceIncrement * log);

			return cursedSpawnChance;
		}
		public static bool GetSpawnRateAndMaxSpawnsMultipliers(Player player, out double spawnRateMult, out double maxSpawnsMult) {
			int cursedEssence = PlayerCursedEssence[player.whoAmI];
			if (cursedEssence <= 0) {
				spawnRateMult = 1.0;
				maxSpawnsMult = 1.0;

				return false;
			}

			double log = CursedEssenceLog2(cursedEssence);
			spawnRateMult = Math.Pow(1 + ConfigValues.CursedBuffSpawnRateMultiplier * 0.415, log);
			maxSpawnsMult = 1.0 + (spawnRateMult - 1.0) * 0.8;

			return true;
		}
		public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns) {
			if (GetSpawnRateAndMaxSpawnsMultipliers(player, out double spawnRateMult, out double maxSpawnsMult)) {
				spawnRate = (int)Math.Round(spawnRate / spawnRateMult);
				maxSpawns = (int)Math.Round(maxSpawns * maxSpawnsMult);
			}
		}
		public static void CountCursedEssence() {
			AndroUtilityMethods.SearchForItem(ModContent.ItemType<CursedEssence>(), out int cursedEssenceCount, WEPlayer.LocalWEPlayer.enchantmentStorageItems, true, false);
			NetManager.UpdateCursedEssenceCount(cursedEssenceCount);
		}

		public static void UpdateCursedEssenceCount(int whoAmI, int cursedEssenceCount) {
			PlayerCursedEssence[whoAmI] = cursedEssenceCount;
		}
	}
	public struct Curse {
		public BuffEffect BuffEffect;
		public Color Color;
		public Curse(BuffEffect buffEffect, Color color) {
			BuffEffect = buffEffect;
			Color = color;
		}
	}
	public class CursedNPC : GlobalNPC {
		public override bool InstancePerEntity => true;
		public bool Cursed = false;
		public bool SpawnedByBoss = false;
		public override void DrawEffects(NPC npc, ref Color drawColor) {
			if (GetCurse(out Curse curse)) {
				byte alpha = drawColor.A;
				drawColor = Color.Lerp(drawColor, curse.Color, 0.8f);
				drawColor.A = alpha;
			}
		}

		private static Vector2 position;
		private static float scale;
		float lastScaleRand = 0.5f;
		float lastYRand = 0.5f;
		float lastXRand = 0.5f;
		public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
			if (!Cursed)
				return true;

			float cursedEnemyShaking = ConfigValues.CursedEnemyVisualShaking;
			bool showShaking = cursedEnemyShaking > 0f;
			bool showParticles = WEMod.clientConfig.CursedEnemyParticles;
			if (!showShaking && !showParticles)
				return true;

			float basePositionRange = 6f;
			float positionRange = basePositionRange * cursedEnemyShaking;
			float xRand = Main.rand.NextFloat();
			float yRand = Main.rand.NextFloat();
			float baseScaleRange = 0.2f;
			float scaleRand = Main.rand.NextFloat();

			if (showShaking) {
				position = npc.position;
				scale = npc.scale;
				Vector2 positionChange = new((xRand - 0.5f) * positionRange, (yRand - 0.5f) * positionRange);
				npc.position += positionChange;

				float scaleRange = baseScaleRange * cursedEnemyShaking;
				float scaleChange = (scaleRand - 0.5f) * scaleRange * npc.scale;
				npc.scale += scaleChange;
			}

			if (showParticles) {
				float yRandDiff = yRand - lastYRand;
				float xRandDiff = xRand - lastXRand;
				float scaleRandDiff = scaleRand - lastScaleRand + 0.5f;

				int dustType = ModContent.DustType<CursedNPCEffectDust>();
				int alpha = 100;
				int dustIndex = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, dustType, xRandDiff + npc.velocity.X / 2f, yRandDiff + npc.velocity.Y / 2f, alpha, drawColor, scaleRandDiff);
				Dust dust = Main.dust[dustIndex];

				Vector2 npcCenter = npc.Center;
				float zeroPositionOffset = 0.0001f;
				if (dust.position.X == npcCenter.X) {
					float sign = Main.rand.NextBool().ToSign();
					dust.position.X += sign * zeroPositionOffset;
				}

				if (dust.position.Y == npcCenter.Y) {
					float sign = Main.rand.NextBool().ToSign();
					dust.position.Y += sign * zeroPositionOffset;
				}

				float xDiff = dust.position.X - npcCenter.X;
				float yDiff = dust.position.Y - npcCenter.Y;
				float xSign = (xDiff >= 0f).ToSign();
				xDiff *= xSign;
				float ySign = (yDiff >= 0f).ToSign();
				yDiff *= ySign;
				bool? axis = null;
				bool useXAxis = true;
				bool useYAxis = false;
				if (xDiff < 1f || yDiff < 1f) {
					if (xDiff < yDiff) {
						if (xDiff < 0.01f)
							axis = useXAxis;
					}
					else {
						if (yDiff < 0.01f)
							axis = useYAxis;
					}
				}

				float xRatio = xDiff / (float)npc.width;
				float yRatio = yDiff / (float)npc.height;

				if (axis == null) {
					//Use lesser ratio because that part of the NPC's outer hitbox will be reached first by extending along the angle between the npc.Center and dust.position
					axis = xRatio <= yRatio;
				}

				if (axis.Value == useXAxis) {
					dust.position.X += xSign * (float)npc.width / 2f;
					dust.position.Y += ySign * yDiff * xRatio;
				}
				else {
					dust.position.X += xSign * xDiff * yRatio;
					dust.position.Y += ySign * (float)npc.height / 2f;
				}

				lastYRand = yRand;
				lastScaleRand = scaleRand;
				lastXRand = xRand;
			}

			return true;
		}
		public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
			if (!Cursed)
				return;

			npc.position = position;
			npc.scale = scale;
		}
		public static List<Curse> curses;
		public static List<Curse> cursesHard;
		private const int maxNPCsPerCurse = 2;
		private static List<List<int>> cursedNPCs;
		private const int NoNPC = -1;
		public int curseEffectIndex = -1;
		public bool GetCurse(out Curse curse) {
			if (!Cursed) {
				curse = default(Curse);
				return false;
			}

			return GetCurse(curseEffectIndex, out curse);
		}
		public static bool GetCurse(int index, out Curse curse) {
			if (index >= 0) {
				if (index < curses.Count) {
					curse = curses[index];
				}
				else {
					curse = cursesHard[index - curses.Count];
				}

				return true;
			}

			curse = default(Curse);
			return false;
		}
		private static Color Black => new(10, 10, 10, 100);
		public override void Load() {
			uint baseDuration = (uint)Math.Max(1, (int)Math.Round(60 * ConfigValues.CursedEnemyDebuffDurationMultiplier));
			float baseChance = Math.Min(1f, ConfigValues.CursedEnemyDebuffChanceMultiplier);
			curses = new() {
				new(new(BuffID.OnFire, BuffStyle.OnTickPlayerDebuff, baseDuration, baseChance, range: ConfigValues.CursedEnemyDebuffAttackRange), Color.Red),
				new(new(BuffID.Slow, BuffStyle.OnTickPlayerDebuff, baseDuration, baseChance, range: ConfigValues.CursedEnemyDebuffAttackRange), Color.Brown),
				new(new(BuffID.Confused, BuffStyle.OnTickPlayerDebuff, baseDuration * 4, Math.Min(1f, 0.1f * ConfigValues.CursedEnemyDebuffChanceMultiplier), range: ConfigValues.CursedEnemyDebuffAttackRange), Color.LightBlue),
				new(new(BuffID.BrokenArmor, BuffStyle.OnTickPlayerDebuff, baseDuration, baseChance, range: ConfigValues.CursedEnemyDebuffAttackRange), Color.Silver),
				new(new(BuffID.Blackout, BuffStyle.OnTickPlayerDebuff, baseDuration, baseChance, range: ConfigValues.CursedEnemyDebuffAttackRange), Black),
			};

			cursesHard = new() {
				new(new(BuffID.WitheredArmor, BuffStyle.OnTickPlayerDebuff, baseDuration, baseChance, range: ConfigValues.CursedEnemyDebuffAttackRange), Color.Silver),
				new(new(BuffID.Obstructed, BuffStyle.OnTickPlayerDebuff, baseDuration, baseChance, range: ConfigValues.CursedEnemyDebuffAttackRange), Black),
				new(new(BuffID.Webbed, BuffStyle.OnTickPlayerDebuff, baseDuration * 2, Math.Min(1f, 0.05f * ConfigValues.CursedEnemyDebuffChanceMultiplier), range: ConfigValues.CursedEnemyDebuffAttackRange), Color.White),
				new(new(BuffID.Obstructed, BuffStyle.OnTickPlayerDebuff, baseDuration, baseChance, range: ConfigValues.CursedEnemyDebuffAttackRange), Color.DarkViolet),
				new(new(BuffID.WitheredWeapon, BuffStyle.OnTickPlayerDebuff, baseDuration, baseChance, range: ConfigValues.CursedEnemyDebuffAttackRange), Color.Orange),
				new(new(BuffID.OgreSpit, BuffStyle.OnTickPlayerDebuff, baseDuration, baseChance, range: ConfigValues.CursedEnemyDebuffAttackRange), Color.YellowGreen),
				new(new(BuffID.Burning, BuffStyle.OnTickPlayerDebuff, baseDuration, baseChance, range: ConfigValues.CursedEnemyDebuffAttackRange), Color.Red),
				new(new(BuffID.Suffocation, BuffStyle.OnTickPlayerDebuff, baseDuration, baseChance, range: ConfigValues.CursedEnemyDebuffAttackRange), Color.Tan),
				new(new(BuffID.ManaSickness, BuffStyle.OnTickPlayerDebuff, baseDuration, baseChance, range: ConfigValues.CursedEnemyDebuffAttackRange), Color.Blue),
				new(new(BuffID.PotionSickness, BuffStyle.OnTickPlayerDebuff, baseDuration, baseChance, range: ConfigValues.CursedEnemyDebuffAttackRange), Color.Magenta),
				new(new(BuffID.Frozen, BuffStyle.OnTickPlayerDebuff, baseDuration, Math.Min(1f, 0.05f * ConfigValues.CursedEnemyDebuffChanceMultiplier), range: ConfigValues.CursedEnemyDebuffAttackRange), Color.LightBlue),
				new(new(BuffID.Stoned, BuffStyle.OnTickPlayerDebuff, baseDuration * 2, Math.Min(1f, 0.025f * ConfigValues.CursedEnemyDebuffChanceMultiplier), range: ConfigValues.CursedEnemyDebuffAttackRange), Color.OliveDrab),
			};

			cursedNPCs = new() {};
			for (int i = 0; i < curses.Count; i++) {
				cursedNPCs.Add(Enumerable.Repeat(NoNPC, maxNPCsPerCurse).ToList());
			}
		}
		public static bool CheckSpawnedByBossAndCursed(IEntitySource source, out bool cursed) {
			cursed = false;
			if (source is EntitySource_Parent parent) {
				if (parent.Entity is NPC parentNPC) {
					if (parentNPC.TryGetGlobalNPC(out CursedNPC cursedNPC)) {
						if (cursedNPC.SpawnedByBoss || parentNPC.IsBoss() || AndroGlobalNPC.bossPartsNotMarkedAsBoss.Contains(parentNPC.netID)) {
							cursed = false;
							return true;
						}

						if (cursedNPC.Cursed)
							cursed = true;
					}
				}
				else if (parent.Entity is Projectile parentProjectile) {
					if (parentProjectile.TryGetGlobalProjectile(out CursedProjectile cursedProjectile)) {
						if (cursedProjectile.SpawnedByBoss) {
							cursed = false;
							return true;
						}
					}
				}
			}

			return false;
		}
		public override void OnSpawn(NPC npc, IEntitySource source) {
			if (!WEMod.serverConfig.AllowCursedEnemies)
				return;

			SpawnedByBoss = CheckSpawnedByBossAndCursed(source, out Cursed);
			if (SpawnedByBoss) {
				bool BossMinionsCanBeCursed = false;
				bool BossMinionsCursedHardModeOnly = true;
				if (!BossMinionsCanBeCursed || (BossMinionsCursedHardModeOnly && !Main.hardMode))
					return;
			}

			if (!Cursed) {
				if (!npc.IsDummy() && !npc.friendly && npc.realLife == -1 && (npc.damage > 0 || npc.RealLifeMax() > 10) && !npc.IsBoss() && !npc.IsMiniBoss() && !npc.ModBossPart() && !npc.IsWorm() && !npc.SpawnedFromStatue && !npc.isLikeATownNPC && !npc.townNPC && !NPCID.Sets.ShouldBeCountedAsBoss[npc.type] && !AndroGlobalNPC.IsIregularNPC(npc)) {
					double cursedSpawnChance = CurseAttractionNPC.GetCursedSpawnChance(npc.position);
					if (Main.rand.NextDouble() <= cursedSpawnChance) {
						string npcFullName = npc.ModFullName();
						Cursed = true;
					}
				}
			}
			
			if (Cursed) {
				//Buff NPC stats
				float cursedEnemyLifeMult = ConfigValues.CursedEnemyLifeMult;
				float cursedEnemyDamageMult = ConfigValues.CursedEnemyDamageMultiplier;
				if (npc.damage > 0) {
					npc.damage = (int)Math.Round((float)npc.damage * cursedEnemyDamageMult);
					npc.defDamage = (int)Math.Round((float)npc.defDamage * cursedEnemyDamageMult);
				}

				npc.lifeMax = (int)Math.Round((float)npc.lifeMax * cursedEnemyLifeMult);
				npc.life = (int)Math.Round((float)npc.life * cursedEnemyLifeMult);
				npc.npcSlots = 0f;

				//Set cursed effect
				int randMax = Main.hardMode ? curses.Count + cursesHard.Count : curses.Count;
				curseEffectIndex = Main.rand.Next(randMax);
			}

			//Sync stats
			if (Main.netMode == NetmodeID.Server && (Cursed || SpawnedByBoss)) {
				int whoAmI = npc.whoAmI;
				CursedModSystem.PreUpdateNPCActions += () => NetManager.SendCursedNPCDataToClients(whoAmI);
			}
		}
		private uint[] nextProjectileTime = new uint[Main.player.Length];
		public override void AI(NPC npc) {
			if (GetCurse(out Curse curse)) {
				if (Main.netMode != NetmodeID.MultiplayerClient) {
					BuffEffect curseEffect = curse.BuffEffect;
					List<int> affectedPlayers = new();
					switch (Main.netMode) {
						case NetmodeID.SinglePlayer:
						case NetmodeID.MultiplayerClient:
							float distance = npc.position.Distance(Main.LocalPlayer.position);
							if (distance <= curseEffect.Range) {
								affectedPlayers.Add(Main.myPlayer);
							}

							break;
						case NetmodeID.Server:
							for (int i = 0; i < Main.player.Length; i++) {
								Player player = Main.player[i];
								if (player.NullOrNotActive() || player.dead)
									continue;

								if (npc.position.Distance(player.position) <= curseEffect.Range) {
									affectedPlayers.Add(i);
								}
							}

							break;
					}

					foreach (int affectedPlayer in affectedPlayers) {
						if (nextProjectileTime[affectedPlayer] <= Main.GameUpdateCount) {
							uint ProjectileTimerDelay = (uint)WEMod.serverConfig.CuredEnemyDebuffTicksPerAttack;
							uint MinProjectileTimerDelay = ProjectileTimerDelay / 2;
							nextProjectileTime[affectedPlayer] = Math.Max(Main.GameUpdateCount + MinProjectileTimerDelay, nextProjectileTime[affectedPlayer] + ProjectileTimerDelay);
							Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, new Vector2(0, 0), ModContent.ProjectileType<CursedEffectProjectile>(), 0, 0f, /*affectedPlayer*/-1, (float)affectedPlayer, (float)curseEffectIndex);
						}
					}
				}
			}
		}
		public override void OnHitNPC(NPC npc, NPC target, NPC.HitInfo hit) {
			base.OnHitNPC(npc, target, hit);
		}
		public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo) {
			base.OnHitPlayer(npc, target, hurtInfo);
		}
		public override void ModifyGlobalLoot(GlobalLoot globalLoot) {
			globalLoot.Add(new CursedEssenceDropRule());
		}
		public static void SetNPCStatsFromNetData(NPCNetInfoCursedNPC npcNetInfo) {
			NPC npc = Main.npc[npcNetInfo.whoAmI];
			npc.lifeMax = npcNetInfo.lifeMax;
		}

		public static void CursedEffectProjectileHitPlayer(Projectile projectile) {
			Player player = Main.player[(int)projectile.ai[CursedEffectProjectile.aiPlayerIndex]];
			int curseIndex = (int)projectile.ai[CursedEffectProjectile.aiCurseIndex];
			if (GetCurse(curseIndex, out Curse curse)) {
				switch (curse.BuffEffect.BuffStats.BuffID) {
					case BuffID.Frozen:
					case BuffID.Stoned:
					case BuffID.Webbed:
						if (player.TryGetModPlayer(out WEPlayer wePlayer)) {
							if (!wePlayer.CanBeCurseHardCrowdControlled)
								return;
						}

						break;
				}
				
				curse.BuffEffect.BuffStats.TryApplyToPlayer(player);
			}
		}
	}
	public class CursedProjectile : GlobalProjectile {
		public override bool InstancePerEntity => true;
		public bool SpawnedByBoss = false;
		public override void OnSpawn(Projectile projectile, IEntitySource source) {
			SpawnedByBoss = CursedNPC.CheckSpawnedByBossAndCursed(source, out bool _);
		}
	}
	public class CursedModSystem : ModSystem {
		public static Action PreUpdateNPCActions = null;
		public override void PreUpdateNPCs() {
			PreUpdateNPCActions?.Invoke();
			PreUpdateNPCActions = null;
		}

		private static uint nextCount = Main.GameUpdateCount;
		private const uint timerIncrement = 60;
		private const uint minWaitTime = 50;

		private static uint nextBuff = Main.GameUpdateCount;
		private static int buffIncrement => 55;
		private static uint buffWaitTime => 30;
		private static uint minBuffWaitTime => 20;
		public override void PostUpdateEverything() {
			if (Main.netMode < NetmodeID.Server) {
				if (Main.GameUpdateCount >= nextCount) {
					nextCount = Math.Max(nextCount + timerIncrement, Main.GameUpdateCount + minWaitTime);
					CurseAttractionNPC.CountCursedEssence();
				}
				
				if (Main.GameUpdateCount >= nextBuff) {
					nextBuff = Math.Max(nextBuff + buffWaitTime, Main.GameUpdateCount + minBuffWaitTime);
					int cursedEssenceCount = CurseAttractionNPC.PlayerCursedEssence[Main.myPlayer];
					if (cursedEssenceCount > 0 && WEMod.clientConfig.VisualCursedDebuff) {
						Main.LocalPlayer.AddBuff(ModContent.BuffType<Cursed>(), buffIncrement);
					}
				}
			}
		}
		public override void Load() {
			AndroMod.OnResetGameCounter += () => {
				nextCount = Main.GameUpdateCount;
				nextBuff = Main.GameUpdateCount;
			};
		}
	}
}
