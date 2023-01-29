using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.ItemDropRules;
using WeaponEnchantments.Common.Utility;

namespace WeaponEnchantments.Common
{
	public class OneFromWeightedOptionsNotScaledWithLuckDropRule : IItemDropRule
	{
		public List<WeightedPair> dropsList;
		public float dropChance;

		public List<IItemDropRuleChainAttempt> ChainedRules {
			get;
			private set;
		}

		public OneFromWeightedOptionsNotScaledWithLuckDropRule(float chance, IEnumerable<DropData> options) {
			dropChance = chance;
			dropsList = new();
			foreach(DropData dropData in options) {
				dropsList.Add(new(dropData));
			}

			ChainedRules = new List<IItemDropRuleChainAttempt>();
		}

		public bool CanDrop(DropAttemptInfo info) => true;

		public ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info) {
			ItemDropAttemptResult result;
			int item = dropsList.GetOneFromWeightedList(dropChance);
			if (item > 0) {
				CommonCode.DropItem(info, item, 1);
				result = default(ItemDropAttemptResult);
				result.State = ItemDropAttemptResultState.Success;

				return result;
			}

			result = default(ItemDropAttemptResult);
			result.State = ItemDropAttemptResultState.FailedRandomRoll;

			return result;
		}

		public void ReportDroprates(List<DropRateInfo> drops, DropRateInfoChainFeed ratesInfo) {
			float parentDropChance = dropChance * ratesInfo.parentDroprateChance;
			float total = 0f;
			foreach(WeightedPair pair in dropsList) {
				total += pair.Weight;
			}

			foreach(WeightedPair pair in dropsList) {
				float chance = parentDropChance * pair.Weight / total;
				chance.Clamp();
				drops.Add(new DropRateInfo(pair.ID, 1, 1, chance, ratesInfo.conditions));
			}

			Chains.ReportDroprates(ChainedRules, dropChance, drops, ratesInfo);
		}
	}
}
