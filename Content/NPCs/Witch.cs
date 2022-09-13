using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.GameContent.Personalities;
using Terraria.DataStructures;
using System.Collections.Generic;
using ReLogic.Content;
using Terraria.ModLoader.IO;
using WeaponEnchantments.Items;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Items.Enchantments;
using WeaponEnchantments.Common;
using System.Reflection;

namespace WeaponEnchantments.Content.NPCs
{
	[AutoloadHead]
	public class Witch : ModNPC {
		public int NumberOfTimesTalkedTo = 0;
		public static bool resetShop = true;
		private Dictionary<int, float> shopEnchantments = new();
		public override void SetStaticDefaults() {
			// DisplayName automatically assigned from localization files, but the commented line below is the normal approach.
			DisplayName.SetDefault("Witch");
			Main.npcFrameCount[Type] = 25; // The amount of frames the NPC has

			NPCID.Sets.ExtraFramesCount[Type] = 9; // Generally for Town NPCs, but this is how the NPC does extra things such as sitting in a chair and talking to other NPCs.
			NPCID.Sets.AttackFrameCount[Type] = 4;
			NPCID.Sets.DangerDetectRange[Type] = 700; // The amount of pixels away from the center of the npc that it tries to attack enemies.
			NPCID.Sets.AttackType[Type] = 0;
			NPCID.Sets.AttackTime[Type] = 90; // The amount of time it takes for the NPC's attack animation to be over once it starts.
			NPCID.Sets.AttackAverageChance[Type] = 30;
			NPCID.Sets.HatOffsetY[Type] = 4; // For when a party is active, the party hat spawns at a Y offset.

			// Influences how the NPC looks in the Bestiary
			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0) {
				Velocity = 1f,
				Direction = -1
			};

			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

			// Set Example Person's biome and neighbor preferences with the NPCHappiness hook. You can add happiness text and remarks with localization (See an example in WeaponEnchantments/Localization/en-US.lang).
			// NOTE: The following code uses chaining - a style that works due to the fact that the SetXAffection methods return the same NPCHappiness instance they're called on.
			NPC.Happiness
				.SetBiomeAffection<DungeonBiome>(AffectionLevel.Love)
				.SetBiomeAffection<JungleBiome>(AffectionLevel.Like)
				.SetBiomeAffection<ForestBiome>(AffectionLevel.Dislike)
				.SetBiomeAffection<HallowBiome>(AffectionLevel.Hate)
				.SetNPCAffection(NPCID.WitchDoctor, AffectionLevel.Love)
				.SetNPCAffection(NPCID.Wizard, AffectionLevel.Like)
				.SetNPCAffection(NPCID.Dryad, AffectionLevel.Dislike)
				.SetNPCAffection(NPCID.BestiaryGirl, AffectionLevel.Hate)
			;
		}

		public override void SetDefaults() {
			NPC.townNPC = true;
			NPC.friendly = true;
			NPC.width = 18;
			NPC.height = 40;
			NPC.aiStyle = 7;
			NPC.damage = 10;
			NPC.defense = 15;
			NPC.lifeMax = 250;
			NPC.HitSound = SoundID.NPCHit40;
			NPC.DeathSound = SoundID.NPCDeath42;
			NPC.knockBackResist = 0.5f;

			AnimationType = NPCID.Guide;
		}
		public override bool UsesPartyHat() => false;
		public override void HitEffect(int hitDirection, double damage) {
			int num = NPC.life > 0 ? 1 : 5;

			for (int k = 0; k < num; k++) {
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood);
			}
		}
		public override bool CanTownNPCSpawn(int numTownNPCs, int money) {
			for (int k = 0; k < 255; k++) {
				Player player = Main.player[k];
				if (!player.active) {
					continue;
				}

				if (player.inventory.Any(item => item.ModItem is Enchantment))
					return true;

				foreach (Item item in player.inventory) {
					if (item.TryGetEnchantedItem(out EnchantedItem enchantedItem)) {
						foreach (Item enchantment in enchantedItem.enchantments) {
							if (!enchantment.NullOrAir())
								return true;
						}
					}
				}

				foreach (Item item in player.GetWEPlayer().Equipment.GetAllArmor()) {
					if (item.TryGetEnchantedItem(out EnchantedItem enchantedItem)) {
						foreach (Item enchantment in enchantedItem.enchantments) {
							if (!enchantment.NullOrAir())
								return true;
						}
					}
				}
			}

			return false;
		}
		public override ITownNPCProfile TownNPCProfile() {
			return new WitchProfile();
		}
		public override List<string> SetNPCNameList() {
			string ext = "NPCNames.Witch";
			return new List<string>() {
				"Gruntilda".Lang(ext),	//Banjo Kazooie
				"Brentilda".Lang(ext),	//Banjo Kazooie
				"Blobbelda".Lang(ext),	//Banjo Kazooie
				"Mingella".Lang(ext),		//Banjo Kazooie
				"MissGulch".Lang(ext),	//The Wizard of Oz
				"Sabrina".Lang(ext),		//Sabrina the teenage witch
				"Winifred".Lang(ext),		//Hocus Pocus
				"Sarah".Lang(ext),		//Hocus Pocus
				"Mary".Lang(ext),			//Hocus Pocus
				"Maleficient".Lang(ext),	//Name is self-explanatory
				"Salem".Lang(ext),		//American history
				"Binx".Lang(ext),			//Hocus Pocus
				"Medusa".Lang(ext),		//Greek mythos
				"Melusine".Lang(ext),		//Name is self-explanatory
				"Ursula".Lang(ext),		//The Little Mermaid
				"Jasminka".Lang(ext),		//Little Witch Academia
				"Agatha".Lang(ext),		//Marvel
				"Freyja".Lang(ext),		//Norse mythos
				"Hazel".Lang(ext),		//Looney Tunes
				"Akko".Lang(ext),			//Little Witch Academia
				"Kyubey".Lang(ext),		//Puella Magi Madoka Magica
				"Morgana".Lang(ext)		//The legend of king Arthur
			};
		}
		public override void SetChatButtons(ref string button, ref string button2) {
			button = Language.GetTextValue("LegacyInterface.28");
			//button2 = "Help";
		}
		public override bool CanGoToStatue(bool toKingStatue) => true;
		public override void SetupShop(Chest shop, ref int nextSlot) {
			if (resetShop || shopEnchantments.Count == 0) {
				GetItemsForShop();
				resetShop = false;
			}

			foreach (KeyValuePair<int, float> pair in shopEnchantments) {
				shop.item[nextSlot].SetDefaults(pair.Key);
				shop.item[nextSlot].value = (int)((float)shop.item[nextSlot].value * pair.Value);
				nextSlot++;
			}
		}
		private void GetItemsForShop() {
			shopEnchantments = new();
			List<ISoldByWitch> allItems = ModContent.GetContent<ModItem>().OfType<ISoldByWitch>().Where(i => i.SellCondition.CanSell()).ToList();
			List<ISoldByWitch> enchanmtnets = allItems.OfType<Enchantment>().Select(e => (ISoldByWitch)e).ToList();
			List<ISoldByWitch> otherItems = allItems
				.Where(i => i is not Enchantment || i.SellCondition <= SellCondition.Always)
				.GroupBy(i => ((ModItem)i).TypeAboveModItem().Name)
				.Select(g => g.ToList().OrderBy(i => EnchantingRarity.GetTierNumberFromName(((ModItem)i).Name)))
				.SelectMany(i => i)
				.ToList();

			//Always
			AddItemsToShop(otherItems);

			//Any Time
			AddEnchantmentsToShop(enchanmtnets, SellCondition.AnyTime, 4);

			AddEnchantmentsToShop(enchanmtnets.Where(e => e.SellCondition != SellCondition.AnyTime).ToList(), SellCondition.IgnoreCondition, 2);

			if (Main.rand.Next(100) == 0)
				AddEnchantmentsToShop(enchanmtnets, SellCondition.Luck, 1);
		}
		private void AddEnchantmentsToShop(List<ISoldByWitch> soldByWitch, SellCondition condition = SellCondition.IgnoreCondition, int num = 0) {
			List<int> list;
			if (condition == SellCondition.IgnoreCondition) {
				list = soldByWitch.Select(e => ((ModItem)e).Type).ToList();
			}
			else {
				list = soldByWitch.Where(e => e.SellCondition == condition).Select(e => ((ModItem)e).Type).ToList();
			}

			if (condition == SellCondition.Always)
				num = list.Count;

			for (int i = 0; i < num; i++) {
				int type = list.GetOneFromList();
				ModItem modItem = (ModItem)soldByWitch[list.IndexOf(type)];
				float sellPriceModifier = soldByWitch[list.IndexOf(type)].SellPriceModifier;
				shopEnchantments.Add(type, sellPriceModifier);
				list.Remove(type);
			}
		}
		private void AddItemsToShop(List<ISoldByWitch> modItems) {
			foreach(ISoldByWitch soldByWitch in modItems) {
				ModItem modItem = (ModItem)soldByWitch;
				float sellPriceModifier = soldByWitch.SellPriceModifier;
				shopEnchantments.Add(modItem.Type, sellPriceModifier);
			}
		}
		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheDungeon,
				new FlavorTextBestiaryInfoElement("Mods.WeaponEnchantments.Bestiary.Witch")
			});
		}
		public override void TownNPCAttackProj(ref int projType, ref int attackDelay) {
			projType = ProjectileID.ToxicFlask;
			attackDelay = 1;
		}
		public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset) {
			multiplier = 12f;
			randomOffset = 2f;
		}
		public override void TownNPCAttackStrength(ref int damage, ref float knockback) {
			damage = 5;
			knockback = 4f;
		}
		public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown) {
			cooldown = 30;
			randExtraCooldown = 30;
		}
		public override void ModifyNPCLoot(NPCLoot npcLoot) {
			int[] loot = {
				ItemID.WitchHat,
				ItemID.WitchDress,
				ItemID.WitchBoots
			};

			foreach (int id in loot) {
				npcLoot.Add(ItemDropRule.Common(id));
			}
		}
		public override void OnChatButtonClicked(bool firstButton, ref bool shop) {
			if (firstButton) {

				// We want 3 different functionalities for chat buttons, so we use HasItem to change button 1 between a shop and upgrade action.
				/*
				if (Main.LocalPlayer.HasItem(ItemID.HiveBackpack)) {
					SoundEngine.PlaySound(SoundID.Item37); // Reforge/Anvil sound

					Main.npcChatText = $"I upgraded your {Lang.GetItemNameValue(ItemID.HiveBackpack)} to a {Lang.GetItemNameValue(ModContent.ItemType<WaspNest>())}";

					int hiveBackpackItemIndex = Main.LocalPlayer.FindItem(ItemID.HiveBackpack);
					var entitySource = NPC.GetSource_GiftOrReward();

					Main.LocalPlayer.inventory[hiveBackpackItemIndex].TurnToAir();
					Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<WaspNest>());

					return;
				}
				*/
				shop = true;
			}

			if (!firstButton) {
				Main.npcChatText = "Help not yet implemented";//Language.GetTextValue("Mods.WeaponEnchantments.Dialogue.Witch.BigAsMine", Main.LocalPlayer.HeldItem.type.Lang(L_ID_V.Item));
			}
		}
		public override string GetChat() {
			WeightedRandom<string> chat = new WeightedRandom<string>();

			AddStandardDialogue(chat, L_ID2.Witch);

			AddOtherNPCDialouges(chat, L_ID2.Witch);

			AddBiomeDialogues(chat, L_ID2.Witch);

			if (Main.bloodMoon)
				chat.Add($"{DialogueID.BloodMoon}".Lang(L_ID1.Dialogue, L_ID2.Witch), 4);

			if (Terraria.GameContent.Events.BirthdayParty.ManualParty || Terraria.GameContent.Events.BirthdayParty.GenuineParty)
				chat.Add($"{DialogueID.BirthdayParty}".Lang(L_ID1.Dialogue, L_ID2.Witch), 10);

			if (Main.IsItStorming)
				chat.Add($"{DialogueID.Storm}".Lang(L_ID1.Dialogue, L_ID2.Witch), 4);

			if (!NPC.downedQueenBee)
				chat.Add($"{DialogueID.QueenBee}".Lang(L_ID1.Dialogue, L_ID2.Witch));

			return chat;
		}
		public void AddStandardDialogue(WeightedRandom<string> chat, L_ID2 npcID) {
			for (int i = 0;; i++) {
				if (!$"{DialogueID.StandardDialogue}{i}".Lang(out string result, L_ID1.Dialogue, npcID))
					break;

				chat.Add(result);
			}
		}
		public void AddOtherNPCDialouges(WeightedRandom<string> chat, L_ID2 npcID) {
			foreach(int i in NPCID.Sets.TownNPCBestiaryPriority) {
				int npcWhoAmI = NPC.FindFirstNPC(i);
				if (npcWhoAmI >= 0) {
					string[] args = { npcID.ToString() };
					if (((TownNPCTypeID)i).ToString().Lang(out string c, L_ID1.Dialogue, npcID, args))
						chat.Add(c, 0.5);
				}
			}
		}
		public void AddBiomeDialogues(WeightedRandom<string> chat, L_ID2 npcID, bool shareCorrupted = true) {
			Player player = Main.LocalPlayer;
			foreach(BiomeID biomeName in Enum.GetValues(typeof(BiomeID))) {
				bool zone = (bool)typeof(Player).GetProperty($"Zone{biomeName}").GetValue(player);
				if (zone) {
					if (biomeName.ToString().Lang(out string c, L_ID1.Dialogue, npcID)) {
						chat.Add(c);
					}
					else if (shareCorrupted) {
						if (biomeName == BiomeID.Corrupt && BiomeID.Crimson.ToString().Lang(out c, L_ID1.Dialogue, npcID) || biomeName == BiomeID.Crimson && BiomeID.Corrupt.ToString().Lang(out c, L_ID1.Dialogue, npcID))
							chat.Add(c);
					}
				}
			}
		}
		public static float DistanceToHome(NPC npc) {
			return (new Vector2(npc.homeTileX, npc.homeTileY) - npc.Center).Length();
		}
		public static Dictionary<int, float> GetTownNPCsInRange(NPC npc, float range) {
			Dictionary<int, float> npcs = new Dictionary<int, float>();
			foreach (NPC target in Main.npc) {
				if (!target.active)
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
		public static float GetSellPriceModifier(SellCondition c) {
			switch(c) {
				case <= SellCondition.Always:
					return 1f;
				case SellCondition.AnyTime:
					return 2f;
				case < SellCondition.HardMode:
					return 5f + (float)c;
				case <= SellCondition.PostPlantera:
					return 20f + 10f * (c - SellCondition.HardMode);
				case <= SellCondition.PostCultist:
					return 100f + 50f * (c - SellCondition.PostGolem);
				case <= SellCondition.PostVortexTower:
					return 500f;
				case SellCondition.PostMoonLord:
					return 1000f;
				default:
					return 1f;
			}
		}
	}

	public class WitchProfile : ITownNPCProfile
	{
		public int RollVariation() => 0;
		public string GetNameForVariant(NPC npc) => npc.getNewNPCName();
		public Asset<Texture2D> GetTextureNPCShouldUse(NPC npc) {
			if (npc.IsABestiaryIconDummy && !npc.ForcePartyHatOn)
				return ModContent.Request<Texture2D>("WeaponEnchantments/Content/NPCs/Witch");

			if (npc.altTexture == 1)
				return ModContent.Request<Texture2D>("WeaponEnchantments/Content/NPCs/Witch_Party");

			return ModContent.Request<Texture2D>("WeaponEnchantments/Content/NPCs/Witch");
		}
		public int GetHeadTextureIndex(NPC npc) => ModContent.GetModHeadSlot("WeaponEnchantments/Content/NPCs/Witch_Head");
	}
}
