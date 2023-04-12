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
using WeaponEnchantments.Items.Enchantments.Utility;

namespace WeaponEnchantments.Content.NPCs
{
	[AutoloadHead]
	public class Witch : ModNPC, INPCWikiInfo {
		public int NumberOfTimesTalkedTo = 0;
		public static bool resetShop = true;
		private Dictionary<int, float> shopEnchantments = new();
		public static bool rerollUI = false;
		public static Item rerollItem = new();
		public static bool mouseRerollEnchantment = false;
		public static float rerollScale = 1f;
		public static string EnchantmentShopName = "EnchantmentsShop";
		private int firstNullOrAirItemIndex = 0;

		public List<WikiTypeID> WikiNPCTypes => new() { WikiTypeID.NPC };

		public string Artist => "Sir Bumpleton ?";

		public Dictionary<IShoppingBiome, AffectionLevel> BiomeAffections => new() {
			{ ModContent.GetInstance<DungeonBiome>(), AffectionLevel.Love },
			{ ModContent.GetInstance<JungleBiome>(), AffectionLevel.Like },
			{ ModContent.GetInstance<ForestBiome>(), AffectionLevel.Dislike },
			{ ModContent.GetInstance<HallowBiome>(), AffectionLevel.Hate },
		};
		public Dictionary<int, AffectionLevel> NPCAffections => new() {
			{ NPCID.WitchDoctor, AffectionLevel.Love },
			{ NPCID.Wizard, AffectionLevel.Like },
			{ NPCID.Dryad, AffectionLevel.Dislike },
			{ NPCID.BestiaryGirl, AffectionLevel.Hate }
		};

		public bool TownNPC => true;

		public string SpawnCondition => "Have an enchantment in your inventory or on your equipment.";

		public override void SetStaticDefaults() {
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
			foreach (KeyValuePair<IShoppingBiome, AffectionLevel> pair in BiomeAffections) {
				NPC.Happiness.SetBiomeAffection(pair.Key, pair.Value);
			}

			foreach (KeyValuePair<int, AffectionLevel> pair in NPCAffections) {
				NPC.Happiness.SetNPCAffection(pair.Key, pair.Value);
			}
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
		public override void HitEffect(NPC.HitInfo hit) {
			int num = NPC.RealLife() > 0 ? 1 : 5;

			for (int k = 0; k < num; k++) {
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood);
			}
		}
		public override bool CanTownNPCSpawn(int numTownNPCs) {
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
			if (rerollUI) {
				button = "Back";
			}
			else {
				button = Language.GetTextValue("LegacyInterface.28");
				button2 = "Re-roll Enchantment";
			}
		}
		public override void OnChatButtonClicked(bool firstButton, ref string shopName) {
			if (firstButton) {
				if (rerollUI) {
					rerollUI = false;
					Main.npcChatText = "What more do you want?  I'm busy.";
				}
				else {
					shopName = EnchantmentShopName;
				}
			}
			else {
				if (rerollUI) {
					if (rerollItem?.ModItem is IRerollableEnchantment rerollableEnchantment) {
						if (true) {//Change to enough money for reroll
							SoundEngine.PlaySound(SoundID.Tink);
							rerollableEnchantment.Reroll();
						}
					}
				}
				else {
					rerollUI = true;
					Main.playerInventory = true;
					Main.npcChatText = "I guess I could try to improve your enchantments, but no refunds or complaints.";
					SoundEngine.PlaySound(SoundID.MenuOpen);
				}
			}
		}
		public override bool CanGoToStatue(bool toKingStatue) => true;
		public override void AddShops() {
			NPCShop witchShop = new NPCShop(Type, EnchantmentShopName);
			witchShop.Register();
		}
		public override void ModifyActiveShop(string shopName, Item[] items) {
			if (shopName == EnchantmentShopName) {
				if (resetShop || firstNullOrAirItemIndex == 0) {
					GetItemsForShop(ref items);
					resetShop = false;
				}
			}
		}
		private void GetItemsForShop(ref Item[] items) {
			firstNullOrAirItemIndex = 0;
			List<ISoldByWitch> allItems = ModContent.GetContent<ModItem>().OfType<ISoldByWitch>().Where(i => i.SellCondition.CanSell()).ToList();
			List<ISoldByWitch> enchanmtnets = allItems.OfType<Enchantment>().Select(e => (ISoldByWitch)e).Where(e => e.SellCondition > SellCondition.Always).ToList();
			List<ISoldByWitch> otherItems = allItems
				.Where(i => i is not Enchantment || i.SellCondition <= SellCondition.Always)
				.GroupBy(i => ((ModItem)i).TypeBeforeModItem().Name)
				.Select(g => g.ToList().OrderBy(i => EnchantingRarity.GetTierNumberFromName(((ModItem)i).Name)))
				.SelectMany(i => i)
				.ToList();

			//Always
			AddItemsToShop(otherItems, ref items);

			//Any Time
			AddEnchantmentsToShop(enchanmtnets, ref items, SellCondition.AnyTime, 4);

			AddEnchantmentsToShop(enchanmtnets.Where(e => e.SellCondition != SellCondition.AnyTime).ToList(), ref items, SellCondition.IgnoreCondition, 2);

			if (Main.rand.Next(100) == 0)
				AddEnchantmentsToShop(enchanmtnets, ref items, SellCondition.Luck, 1);
		}
		private void AddEnchantmentsToShop(List<ISoldByWitch> soldByWitch, ref Item[] items, SellCondition condition = SellCondition.IgnoreCondition, int num = 0) {
			List<int> list;
			List<ISoldByWitch> filteredList;
			if (condition == SellCondition.IgnoreCondition) {
				filteredList = soldByWitch;
				list = soldByWitch.Select(e => ((ModItem)e).Type).ToList();
			}
			else {
				filteredList = soldByWitch.Where(e => e.SellCondition == condition).ToList();
				list = filteredList.Select(e => ((ModItem)e).Type).ToList();
			}

			if (condition == SellCondition.Always)
				num = list.Count;

			for (int i = 0; i < num; i++) {
				int type = list.GetOneFromList();
				float sellPriceModifier = filteredList[list.IndexOf(type)].SellPriceModifier;
				for (int j = 0; j < firstNullOrAirItemIndex; j++) {
					if (type == items[j].type) {
						$"Prevented an issue that would add a duplicate item to the Wich's shop item: {ContentSamples.ItemsByType[type].S()}".LogNT(ChatMessagesIDs.AlwaysShowDuplicateItemInWitchsShop);
						i--;
						list.Remove(type);
						continue;
					}
				}

				Item item = new(type);
				item.value = (int)Math.Round(item.value * sellPriceModifier);
				items[firstNullOrAirItemIndex] = item;
				firstNullOrAirItemIndex++;
				list.Remove(type);
			}
		}
		private void AddItemsToShop(List<ISoldByWitch> modItems, ref Item[] items) {
			foreach(ISoldByWitch soldByWitch in modItems) {
				ModItem modItem = (ModItem)soldByWitch;
				float sellPriceModifier = soldByWitch.SellPriceModifier;
				Item item = new(modItem.Type);
				item.value = (int)Math.Round(item.value * sellPriceModifier);
				items[firstNullOrAirItemIndex] = item;
				firstNullOrAirItemIndex++;
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
					string otherNPCString = ((TownNPCTypeID)i).ToString();
					string otherNPCName = Main.npc[npcWhoAmI].GivenName;
					string[] args = { otherNPCName };
					if (otherNPCString.Lang(out string c, L_ID1.Dialogue, npcID, args))
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
				case <= SellCondition.PostMoonLord:
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
