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

namespace WeaponEnchantments.Content.NPCs
{
	[AutoloadHead]
	public class Witch : ModNPC
	{
		public int NumberOfTimesTalkedTo = 0;
		public static bool resetShop = true;
		private Dictionary<int, int> shopEnchantments = new();
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

			//for (int k = 0; k < num; k++) {
			//	Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<Sparkle>());
			//}
		}
		public override bool CanTownNPCSpawn(int numTownNPCs, int money) {
			for (int k = 0; k < 255; k++) {
				Player player = Main.player[k];
				if (!player.active) {
					continue;
				}

				if (player.inventory.Any(item => item.ModItem is Enchantment))
					return true;

				foreach(Item item in player.inventory) {
					if (item.TryGetEnchantedItem(out EnchantedItem enchantedItem)) {
						foreach(Item enchantment in enchantedItem.enchantments) {
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
		public override void SetChatButtons(ref string button, ref string button2) { // What the chat buttons are when you open up the chat UI
			button = Language.GetTextValue("LegacyInterface.28");
			button2 = "Help";
		}

		// Not completely finished, but below is what the NPC will sell

		// public override void SetupShop(Chest shop, ref int nextSlot) {
		// 	shop.item[nextSlot++].SetDefaults(ItemType<ExampleItem>());
		// 	// shop.item[nextSlot].SetDefaults(ItemType<EquipMaterial>());
		// 	// nextSlot++;
		// 	// shop.item[nextSlot].SetDefaults(ItemType<BossItem>());
		// 	// nextSlot++;
		// 	shop.item[nextSlot++].SetDefaults(ItemType<Items.Placeable.Furniture.ExampleWorkbench>());
		// 	shop.item[nextSlot++].SetDefaults(ItemType<Items.Placeable.Furniture.ExampleChair>());
		// 	shop.item[nextSlot++].SetDefaults(ItemType<Items.Placeable.Furniture.ExampleDoor>());
		// 	shop.item[nextSlot++].SetDefaults(ItemType<Items.Placeable.Furniture.ExampleBed>());
		// 	shop.item[nextSlot++].SetDefaults(ItemType<Items.Placeable.Furniture.ExampleChest>());
		// 	shop.item[nextSlot++].SetDefaults(ItemType<ExamplePickaxe>());
		// 	shop.item[nextSlot++].SetDefaults(ItemType<ExampleHamaxe>());
		//
		// 	if (Main.LocalPlayer.HasBuff(BuffID.Lifeforce)) {
		// 		shop.item[nextSlot++].SetDefaults(ItemType<ExampleHealingPotion>());
		// 	}
		//
		// 	// if (Main.LocalPlayer.GetModPlayer<ExamplePlayer>().ZoneExample && !GetInstance<ExampleConfigServer>().DisableExampleWings) {
		// 	// 	shop.item[nextSlot].SetDefaults(ItemType<ExampleWings>());
		// 	// 	nextSlot++;
		// 	// }
		//
		// 	if (Main.moonPhase < 2) {
		// 		shop.item[nextSlot++].SetDefaults(ItemType<ExampleSword>());
		// 	}
		// 	else if (Main.moonPhase < 4) {
		// 		// shop.item[nextSlot++].SetDefaults(ItemType<ExampleGun>());
		// 		shop.item[nextSlot].SetDefaults(ItemType<ExampleBullet>());
		// 	}
		// 	else if (Main.moonPhase < 6) {
		// 		// shop.item[nextSlot++].SetDefaults(ItemType<ExampleStaff>());
		// 	}
		//
		// 	// todo: Here is an example of how your npc can sell items from other mods.
		// 	// var modSummonersAssociation = ModLoader.TryGetMod("SummonersAssociation");
		// 	// if (ModLoader.TryGetMod("SummonersAssociation", out Mod modSummonersAssociation)) {
		// 	// 	shop.item[nextSlot].SetDefaults(modSummonersAssociation.ItemType("BloodTalisman"));
		// 	// 	nextSlot++;
		// 	// }
		//
		// 	// if (!Main.LocalPlayer.GetModPlayer<ExamplePlayer>().WitchGiftReceived && GetInstance<ExampleConfigServer>().WitchFreeGiftList != null) {
		// 	// 	foreach (var item in GetInstance<ExampleConfigServer>().WitchFreeGiftList) {
		// 	// 		if (Item.IsUnloaded) continue;
		// 	// 		shop.item[nextSlot].SetDefaults(Item.Type);
		// 	// 		shop.item[nextSlot].shopCustomPrice = 0;
		// 	// 		shop.item[nextSlot].GetGlobalItem<ExampleInstancedGlobalItem>().WitchFreeGift = true;
		// 	// 		nextSlot++;
		// 	// 		//TODO: Have tModLoader handle index issues.
		// 	// 	}
		// 	// }
		// }

		public override bool CanGoToStatue(bool toKingStatue) => true;
		public override void SetupShop(Chest shop, ref int nextSlot) {
			int num = EnchantingRarity.displayTierNames.Length;
			nextSlot += num;
			int slotNum = nextSlot - 1;
			int essenceType = ModContent.ItemType<EnchantmentEssenceUltraRare>();
			int price = ContentSamples.ItemsByType[essenceType].value * 2;
			for(int i = 0; i < num; i++) {
				shop.item[slotNum].SetDefaults(essenceType - i);
				shop.item[slotNum].value = price;
				price /= 4;
				slotNum--;
			}

			if (resetShop || shopEnchantments.Count == 0) {
				GetEnchantmentsForShop();
				resetShop = false;
			}

			foreach(KeyValuePair<int, int> pair in shopEnchantments) {
				shop.item[nextSlot].SetDefaults(pair.Key);
				shop.item[nextSlot].value *= pair.Value;
				nextSlot++;
			}
		}
		private void GetEnchantmentsForShop() {
			shopEnchantments = new Dictionary<int, int>();
			IEnumerable<Enchantment> enchantments = ModContent.GetContent<ModItem>().OfType<Enchantment>().Where(e => e.EnchantmentTier == 0 || e.EnchantmentValueTierReduction != 0);

			//Always
			AddEnchantmentsToShop(enchantments, SellCondition.Always, 0, 1);

			//Any Time
			AddEnchantmentsToShop(enchantments, SellCondition.AnyTime, 4, 2);

			//Any Time Rare
			AddEnchantmentsToShop(enchantments, SellCondition.AnyTimeRare, 1, 5);

			if (Main.hardMode) {
				//Hard Mode
				AddEnchantmentsToShop(enchantments, SellCondition.HardMode, 2, 20);

				if (NPC.downedPlantBoss) {
					//Post Plantera
					AddEnchantmentsToShop(enchantments, SellCondition.PostPlantera, 1, 50);

					if (NPC.downedAncientCultist) {
						//Post Cultist
						AddEnchantmentsToShop(enchantments, SellCondition.PostCultist, 1, 100);
					}
				}
			}

			if (Main.rand.Next(100) == 0) {
				AddEnchantmentsToShop(enchantments, SellCondition.Luck, 1, 5);
			}
		}
		private void AddEnchantmentsToShop(IEnumerable<Enchantment> enchantments, SellCondition condition, int num, int priceMultiplier) {
			List<int> list = enchantments.Where(e => e.SellCondition == condition).Select(e => e.Type).ToList();

			if (condition == SellCondition.Always)
				num = list.Count;

			for (int i = 0; i < num; i++) {
				int type = list.GetOneFromList();
				shopEnchantments.Add(type, priceMultiplier);
				list.Remove(type);
			}
		}




		//Not finished methods below here:
		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheDungeon,

				// Sets your NPC's flavor text in the bestiary.
				new FlavorTextBestiaryInfoElement("Hailing from a mysterious greyscale cube world, the Example Person is here to help you understand everything about tModLoader."),

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement("Mods.WeaponEnchantments.Bestiary.Witch")
			});
		}
		// todo: implement
		 // public override void TownNPCAttackProj(ref int projType, ref int attackDelay) {
		 // 	projType = ProjectileType<SparklingBall>();
		 // 	attackDelay = 1;
		 // }
		public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset) {
			multiplier = 12f;
			randomOffset = 2f;
		}
		public override void TownNPCAttackStrength(ref int damage, ref float knockback) {
			damage = 20;
			knockback = 4f;
		}
		public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown) {
			cooldown = 30;
			randExtraCooldown = 30;
		}
		public void StatueTeleport() {
			for (int i = 0; i < 30; i++) {
				Vector2 position = Main.rand.NextVector2Square(-20, 21);
				if (Math.Abs(position.X) > Math.Abs(position.Y)) {
					position.X = Math.Sign(position.X) * 20;
				}
				else {
					position.Y = Math.Sign(position.Y) * 20;
				}

				//Dust.NewDustPerfect(NPC.Center + position, ModContent.DustType<Sparkle>(), Vector2.Zero).noGravity = true;
			}
		}

		// Make something happen when the npc teleports to a statue. Since this method only runs server side, any visual effects like dusts or gores have to be synced across all clients manually.
		public override void OnGoToStatue(bool toKingStatue) {
			if (Main.netMode == NetmodeID.Server) {
				//ModPacket packet = Mod.GetPacket();
				//packet.Write((byte)WeaponEnchantments.MessageType.ExampleTeleportToStatue);
				//packet.Write((byte)NPC.whoAmI);
				//packet.Send();
			}
			else {
				StatueTeleport();
			}
		}
		public override void ModifyNPCLoot(NPCLoot npcLoot) {
			//npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ExampleCostume>()));
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
				Main.npcChatText = Language.GetTextValue("Mods.WeaponEnchantments.Dialogue.Witch.BigAsMine", Main.LocalPlayer.HeldItem.type.Lang(L_ID_V.Item));
			}
		}
		public override string GetChat() {
			WeightedRandom<string> chat = new WeightedRandom<string>();

			int partyGirl = NPC.FindFirstNPC(NPCID.PartyGirl);
			if (partyGirl >= 0 && Main.rand.NextBool(4)) {
				chat.Add(Language.GetTextValue("Mods.WeaponEnchantments.Dialogue.Witch.PartyGirlDialogue", Main.npc[partyGirl].GivenName));
			}
			// These are things that the NPC has a chance of telling you when you talk to it.
			chat.Add(Language.GetTextValue("Mods.WeaponEnchantments.Dialogue.Witch.StandardDialogue1"));
			chat.Add(Language.GetTextValue("Mods.WeaponEnchantments.Dialogue.Witch.StandardDialogue2"));
			chat.Add(Language.GetTextValue("Mods.WeaponEnchantments.Dialogue.Witch.StandardDialogue3"));
			chat.Add(Language.GetTextValue("Mods.WeaponEnchantments.Dialogue.Witch.CommonDialogue"), 5.0);
			chat.Add(Language.GetTextValue("Mods.WeaponEnchantments.Dialogue.Witch.RareDialogue"), 0.1);

			NumberOfTimesTalkedTo++;
			if (NumberOfTimesTalkedTo >= 10) {
				//This counter is linked to a single instance of the NPC, so if Witch is killed, the counter will reset.
				chat.Add(Language.GetTextValue("Mods.WeaponEnchantments.Dialogue.Witch.TalkALot"));
			}

			return chat; // chat is implicitly cast to a string.
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