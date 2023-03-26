using Terraria.ID;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Common.Utility;

namespace WeaponEnchantments.Common
{
	public class ModdedBagLoot : ILoot
	{
		private List<IItemDropRule> dropRules = new();
		public IItemDropRule Add(IItemDropRule entry) {
			dropRules.Add(entry);
			return entry;
		}

		public List<IItemDropRule> Get(bool includeGlobalDrops = true) {
			return dropRules;
		}

		public IItemDropRule Remove(IItemDropRule entry) {
			dropRules.Remove(entry);
			return entry;
		}

		public void RemoveWhere(Predicate<IItemDropRule> predicate, bool includeGlobalDrops = true) {
			foreach (IItemDropRule dropRule in dropRules) {
				if (predicate(dropRule)) {
					dropRules.Remove(dropRule);
				}
			}
		}
	}
	public class ModdedLootBagDropRule : IItemDropRule
	{
		private static SortedDictionary<int, ModdedBagLoot> allModdedLootBagDropRules = new();
		private static List<IItemDropRule> GetModdedBagLoot(int bagType) {
			if (!allModdedLootBagDropRules.ContainsKey(bagType)) {
				ModdedBagLoot newModdedBagLoot = new();
				allModdedLootBagDropRules.Add(bagType, newModdedBagLoot);
			}

			if (WEGlobalNPC.AllItemDropsFromNpcs != null) {
				int count = allModdedLootBagDropRules[bagType].Get().Count;
				if (count <= 0) {
					ModdedBagLoot newModdedBagLoot = new();
					if (!WEGlobalNPC.AllItemDropsFromNpcs.TryGetValue(bagType, out List<(int, float)> npcs)) {
						if (bagType > ItemID.Count) {
							Item item = bagType.CSI();
							$"Unable to determine the npc that drops this boss bag: {item.Name}, {item.ModFullName()}.".LogNT(ChatMessagesIDs.BossBagDropsFailToFind);
						}
					}
					else if (npcs.Count > 0) {
						NPC npc = npcs.First().Item1.CSNPC();
						WEGlobalNPC.GetLoot(newModdedBagLoot, npc, true);
					}

					allModdedLootBagDropRules[bagType] = newModdedBagLoot;
				}
			}

			return allModdedLootBagDropRules[bagType].Get();
		}
		private int lootBagItemType;
		List<IItemDropRule> myDropRules => GetModdedBagLoot(lootBagItemType);
		public List<IItemDropRuleChainAttempt> ChainedRules {
			get;
			private set;
		}

		public ModdedLootBagDropRule(int LootBagItemType) {
			lootBagItemType = LootBagItemType;

			ChainedRules = new List<IItemDropRuleChainAttempt>();
		}

		public bool CanDrop(DropAttemptInfo info) => true;

		public ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info) {
			ItemDropAttemptResult result = default(ItemDropAttemptResult);
			result.State = ItemDropAttemptResultState.FailedRandomRoll;
			foreach (IItemDropRule dropRule in myDropRules) {
				List<DropRateInfo> dropRateInfos = new();
				DropRateInfoChainFeed ratesInfo = new(1f);
				dropRule.ReportDroprates(dropRateInfos, ratesInfo);
				foreach (DropRateInfo dropRateInfo in dropRateInfos) {
					Item item = dropRateInfo.itemId.CSI();
					float randFloat = Main.rand.NextFloat();
					float dropChance = dropRateInfo.dropRate;
					if (randFloat <= dropChance) {
						int stack = info.rng.Next(dropRateInfo.stackMin, dropRateInfo.stackMax + 1);
						if (stack > 0) {
							CommonCode.DropItem(info, item.type, stack);
							result.State = ItemDropAttemptResultState.Success;
						}
					}
				}
			}
			
			return result;
		}

		public void ReportDroprates(List<DropRateInfo> drops, DropRateInfoChainFeed ratesInfo) {
			foreach (IItemDropRule dropRule in myDropRules) {
				dropRule.ReportDroprates(drops, ratesInfo);
			}

			Chains.ReportDroprates(ChainedRules, ratesInfo.parentDroprateChance, drops, ratesInfo);
		}
	}
}
