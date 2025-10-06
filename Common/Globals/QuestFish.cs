using androLib;
using androLib.Common.Utility;
using KokoLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Achievements;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.ModLib.KokoLib;
using static Terraria.Player;

namespace WeaponEnchantments.Common.Globals
{
	public class QuestFish : GlobalItem
	{
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
			return Main.anglerQuestItemNetIDs.ToList().Contains(entity.type);
		}

		public override void CaughtFishStack(int type, ref int stack) {
			int questFish = Main.anglerQuestItemNetIDs[Main.anglerQuest];
			if (questFish == type) {
				Player player = Main.LocalPlayer;
				if (player.GetWEPlayer().CheckEnchantmentStats(EnchantmentStat.QuestFishChance, out float _)) {
					stack = 0;

					NPC angler = new();
					bool foundAngler = false;
					for (int i = 0; i < Main.npc.Length; i++) {
						if (Main.npc[i].netID == NPCID.Angler) {
							angler = Main.npc[i];
							foundAngler = true;
							break;
						}
					}

					SoundEngine.PlaySound(SoundID.Chat);
					Main.LocalPlayer.anglerQuestsFinished++;

					if (foundAngler) {
						Main.LocalPlayer.GetAnglerReward(angler, type);
					}
					else {
						GameMessageTextID.FailedToLocateAngler.ToString().Lang_WE(L_ID1.GameMessages).LogNT(ChatMessagesIDs.AlwaysShowFailedToLocateAngler);// $"Failed to locate the Angler.  You will still receive rewards".LogNT_WE(ChatMessagesIDs.AlwaysShowFailedToLocateAngler);
						GetAnglerLoot();
					}

					Main.anglerQuestFinished = true;
					if (Main.netMode == NetmodeID.MultiplayerClient) {
						NetMessage.SendData(MessageID.AnglerQuestFinished);
					}
					else {
						Main.anglerWhoFinishedToday.Add(player.name);
					}

					AchievementsHelper.HandleAnglerService();

					NetManager.AnglerQuestSwap();
				}
			}
		}

		public static void PrintAnglerQuest() {
			int newQuestFish = Main.anglerQuestItemNetIDs[Main.anglerQuest];
			Main.NewText($"{GameMessageTextID.FishingQuestTurnedIn.ToString().Lang_WE(L_ID1.GameMessages, new object[] { ContentSamples.ItemsByType[newQuestFish].Name, Main.LocalPlayer.anglerQuestsFinished })}\n" +//$"Quest turned in.  Your next quest is {ContentSamples.ItemsByType[newQuestFish].Name}.  Quests finished: {Main.LocalPlayer.anglerQuestsFinished}\n" +
				$"{Lang.AnglerQuestChat(false)}");
		}

		private void GetAnglerLoot() {
			Player player = Main.LocalPlayer;
			int num = player.anglerQuestsFinished;
			float num2 = 1f;
			num2 = ((num <= 50) ? (num2 - (float)num * 0.01f) : ((num <= 100) ? (0.5f - (float)(num - 50) * 0.005f) : ((num > 150) ? 0.15f : (0.25f - (float)(num - 100) * 0.002f))));
			num2 *= 0.9f;
			num2 *= (float)(player.currentShoppingSettings.PriceAdjustment + 1.0) / 2f;

			List<Item> rewardItems = new List<Item>();

			GetAnglerReward_MainReward(player, rewardItems, num, num2);
			GetAnglerReward_Money(rewardItems, num);
			GetAnglerReward_Bait(rewardItems, num, num2);

			PlayerLoader.AnglerQuestReward(player, num2, rewardItems);

			for (int i = 0; i < rewardItems.Count; i++) {
				Item rewardItem = rewardItems[i];
				rewardItem.position = player.Center;

				if (!StorageManager.TryReturnItemToPlayer(ref rewardItem, player, true)) {
					Item getItem = player.GetItem(player.whoAmI, rewardItem, GetItemSettings.NPCEntityToPlayerInventorySettings);

					if (getItem.stack > 0) {
						int number = Item.NewItem(player.GetSource_Loot("Angler Quest Rewards (Weapon Enchantments)"), (int)player.position.X, (int)player.position.Y, player.width, player.height, getItem.type, getItem.stack, noBroadcast: false, 0, noGrabDelay: true);

						if (Main.netMode == 1)
							NetMessage.SendData(21, -1, -1, null, number, 1f);
					}
				}
			}
		}

		private void GetAnglerReward_MainReward(Player player, List<Item> rewardItems, int questsDone, float rarityReduction) {
			Item item = new Item();
			item.type = 0;
			switch (questsDone) {
				case 5:
					item.SetDefaults(ItemID.FuzzyCarrot);
					break;
				case 10:
					item.SetDefaults(ItemID.AnglerHat);
					break;
				case 15:
					item.SetDefaults(ItemID.AnglerVest);
					break;
				case 20:
					item.SetDefaults(ItemID.AnglerPants);
					break;
				case 30:
					item.SetDefaults(ItemID.GoldenFishingRod);
					break;
				default: {
					List<int> itemIdsOfAccsWeWant = new List<int> {
						2373,
						2374,
						2375
					};

					List<int> itemIdsOfAccsWeWant2 = new List<int> {
						3120,
						3037,
						3096
					};

					if (questsDone > 75 && Main.rand.Next((int)(250f * rarityReduction)) == 0) {
						item.SetDefaults(2294);
						break;
					}

					if (Main.hardMode && questsDone > 25 && Main.rand.Next((int)(100f * rarityReduction)) == 0) {
						item.SetDefaults(2422);
						break;
					}

					if (Main.hardMode && questsDone > 10 && Main.rand.Next((int)(70f * rarityReduction)) == 0) {
						item.SetDefaults(2494);
						break;
					}

					if (Main.hardMode && questsDone > 10 && Main.rand.Next((int)(70f * rarityReduction)) == 0) {
						item.SetDefaults(3031);
						break;
					}

					if (Main.hardMode && questsDone > 10 && Main.rand.Next((int)(70f * rarityReduction)) == 0) {
						item.SetDefaults(3032);
						break;
					}

					if (Main.rand.Next((int)(80f * rarityReduction)) == 0) {
						item.SetDefaults(3183);
						break;
					}

					if (Main.rand.Next((int)(60f * rarityReduction)) == 0) {
						item.SetDefaults(2360);
						break;
					}

					if (Main.rand.Next((int)(60f * rarityReduction)) == 0) {
						item.SetDefaults(4067);
						break;
					}

					if (DropAnglerAccByMissing(player, itemIdsOfAccsWeWant, (int)(40f * rarityReduction), out bool botheredRollingForADrop, out int itemIdToDrop)) {
						item.SetDefaults(itemIdToDrop);
						break;
					}

					if (!botheredRollingForADrop && Main.rand.Next((int)(40f * rarityReduction)) == 0) {
						item.SetDefaults(2373);
						break;
					}

					if (!botheredRollingForADrop && Main.rand.Next((int)(40f * rarityReduction)) == 0) {
						item.SetDefaults(2374);
						break;
					}

					if (!botheredRollingForADrop && Main.rand.Next((int)(40f * rarityReduction)) == 0) {
						item.SetDefaults(2375);
						break;
					}

					if (DropAnglerAccByMissing(player, itemIdsOfAccsWeWant2, (int)(30f * rarityReduction), out bool botheredRollingForADrop2, out int itemIdToDrop2)) {
						item.SetDefaults(itemIdToDrop2);
						break;
					}

					if (!botheredRollingForADrop2 && Main.rand.Next((int)(30f * rarityReduction)) == 0) {
						item.SetDefaults(3120);
						break;
					}

					if (!botheredRollingForADrop2 && Main.rand.Next((int)(30f * rarityReduction)) == 0) {
						item.SetDefaults(3037);
						break;
					}

					if (!botheredRollingForADrop2 && Main.rand.Next((int)(30f * rarityReduction)) == 0) {
						item.SetDefaults(3096);
						break;
					}

					if (Main.rand.Next((int)(40f * rarityReduction)) == 0) {
						item.SetDefaults(2417);
						break;
					}

					if (Main.rand.Next((int)(40f * rarityReduction)) == 0) {
						item.SetDefaults(2498);
						break;
					}

					switch (Main.rand.Next(70)) {
						case 0:
							item.SetDefaults(2442);
							break;
						case 1:
							item.SetDefaults(2443);
							break;
						case 2:
							item.SetDefaults(2444);
							break;
						case 3:
							item.SetDefaults(2445);
							break;
						case 4:
							item.SetDefaults(2497);
							break;
						case 5:
							item.SetDefaults(2495);
							break;
						case 6:
							item.SetDefaults(2446);
							break;
						case 7:
							item.SetDefaults(2447);
							break;
						case 8:
							item.SetDefaults(2448);
							break;
						case 9:
							item.SetDefaults(2449);
							break;
						case 10:
							item.SetDefaults(2490);
							break;
						case 12:
							item.SetDefaults(2496);
							break;
						default:
							switch (Main.rand.Next(3)) {
								case 0:
									item.SetDefaults(2354);
									item.stack = Main.rand.Next(2, 6);
									break;
								case 1:
									item.SetDefaults(2355);
									item.stack = Main.rand.Next(2, 6);
									break;
								default:
									item.SetDefaults(2356);
									item.stack = Main.rand.Next(2, 6);
									break;
							}
							break;
					}

					break;
				}
			}

			rewardItems.Add(item);

			if (item.type == 2417) {
				Item item3 = new Item();
				Item item4 = new Item();
				item3.SetDefaults(2418);
				rewardItems.Add(item3);

				item4.SetDefaults(2419);
				rewardItems.Add(item4);
			}
			else {
				if (item.type != 2498)
					return;

				Item item5 = new Item();
				Item item6 = new Item();
				item5.SetDefaults(2499);
				rewardItems.Add(item5);

				item6.SetDefaults(2500);
				rewardItems.Add(item6);
			}
		}
		public bool DropAnglerAccByMissing(Player player, List<int> itemIdsOfAccsWeWant, int randomChanceForASingleAcc, out bool botheredRollingForADrop, out int itemIdToDrop) {
			botheredRollingForADrop = false;
			itemIdToDrop = 0;
			Item[] array = player.inventory;
			for (int i = 0; i < array.Length; i++) {
				RemoveAnglerAccOptionsFromRewardPool(itemIdsOfAccsWeWant, array[i]);
			}

			array = player.armor;
			for (int j = 0; j < array.Length; j++) {
				RemoveAnglerAccOptionsFromRewardPool(itemIdsOfAccsWeWant, array[j]);
			}

			array = player.bank.item;
			for (int k = 0; k < array.Length; k++) {
				RemoveAnglerAccOptionsFromRewardPool(itemIdsOfAccsWeWant, array[k]);
			}

			array = player.bank2.item;
			for (int l = 0; l < array.Length; l++) {
				RemoveAnglerAccOptionsFromRewardPool(itemIdsOfAccsWeWant, array[l]);
			}

			array = player.bank3.item;
			for (int m = 0; m < array.Length; m++) {
				RemoveAnglerAccOptionsFromRewardPool(itemIdsOfAccsWeWant, array[m]);
			}

			array = player.bank4.item;
			for (int n = 0; n < array.Length; n++) {
				RemoveAnglerAccOptionsFromRewardPool(itemIdsOfAccsWeWant, array[n]);
			}

			if (itemIdsOfAccsWeWant.Count == 0)
				return false;

			bool flag = false;
			for (int num = 0; num < itemIdsOfAccsWeWant.Count; num++) {
				flag |= (Main.rand.Next(randomChanceForASingleAcc) == 0);
			}

			botheredRollingForADrop = true;
			if (flag) {
				itemIdToDrop = Main.rand.NextFromList(itemIdsOfAccsWeWant.ToArray());
				return true;
			}

			return false;
		}
		private void RemoveAnglerAccOptionsFromRewardPool(List<int> itemIdsOfAccsWeWant, Item itemToTestAgainst) {
			if (!itemToTestAgainst.IsAir) {
				switch (itemToTestAgainst.type) {
					default:
						itemIdsOfAccsWeWant.Remove(itemToTestAgainst.type);
						break;
					case 3721:
					case 5064:
						itemIdsOfAccsWeWant.Remove(2373);
						itemIdsOfAccsWeWant.Remove(2375);
						itemIdsOfAccsWeWant.Remove(2374);
						break;
					case 3036:
					case 3123:
					case 3124:
						itemIdsOfAccsWeWant.Remove(3120);
						itemIdsOfAccsWeWant.Remove(3037);
						itemIdsOfAccsWeWant.Remove(3096);
						break;
				}
			}
		}
		private void GetAnglerReward_Bait(List<Item> rewardItems, int questsDone, float rarityReduction) {
			if (Main.rand.Next((int)(100f * rarityReduction)) > 50)
				return;

			Item item = new Item();
			if (Main.rand.Next((int)(15f * rarityReduction)) == 0)
				item.SetDefaults(2676);
			else if (Main.rand.Next((int)(5f * rarityReduction)) == 0)
				item.SetDefaults(2675);
			else
				item.SetDefaults(2674);

			if (Main.rand.Next(25) <= questsDone)
				item.stack++;

			if (Main.rand.Next(50) <= questsDone)
				item.stack++;

			if (Main.rand.Next(100) <= questsDone)
				item.stack++;

			if (Main.rand.Next(150) <= questsDone)
				item.stack++;

			if (Main.rand.Next(200) <= questsDone)
				item.stack++;

			if (Main.rand.Next(250) <= questsDone)
				item.stack++;

			rewardItems.Add(item);
			/*
			item.position = base.Center;
			Item item2 = GetItem(whoAmI, item, GetItemSettings.NPCEntityToPlayerInventorySettings);
			if (item2.stack > 0) {
				int number = Item.NewItem(source, (int)position.X, (int)position.Y, width, height, item2.type, item2.stack, noBroadcast: false, 0, noGrabDelay: true);
				if (Main.netMode == 1)
					NetMessage.SendData(21, -1, -1, null, number, 1f);
			}
			*/
		}

		private void GetAnglerReward_Money(List<Item> rewardItems, int questsDone) {
			Item item = new Item();
			int num = (questsDone + 50) / 2;
			num = (int)((float)(num * Main.rand.Next(50, 201)) * 0.015f);
			num = (int)((double)num * 1.5);
			if (Main.expertMode)
				num *= 2;

			if (num > 100) {
				num /= 100;
				if (num > 10)
					num = 10;

				if (num < 1)
					num = 1;

				item.SetDefaults(73);
				item.stack = num;
			}
			else {
				if (num > 99)
					num = 99;

				if (num < 1)
					num = 1;

				item.SetDefaults(72);
				item.stack = num;
			}

			rewardItems.Add(item);
			/*
			item.position = base.Center;
			Item item2 = GetItem(whoAmI, item, anglerRewardSettings);
			if (item2.stack > 0) {
				int number = Item.NewItem(source, (int)position.X, (int)position.Y, width, height, item2.type, item2.stack, noBroadcast: false, 0, noGrabDelay: true);
				if (Main.netMode == 1)
					NetMessage.SendData(21, -1, -1, null, number, 1f);
			}
			*/
		}
	}
}
