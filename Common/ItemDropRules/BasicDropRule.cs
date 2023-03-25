using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using WeaponEnchantments.Common.Utility;

namespace WeaponEnchantments.Common
{
	public class BasicDropRule : IItemDropRule
	{
		public int itemID;
		public float dropChance;

		public List<IItemDropRuleChainAttempt> ChainedRules {
			get;
			private set;
		}

		public BasicDropRule(int id, float chance, float configChance) {
			itemID = id;
			dropChance = chance * configChance;
			dropChance.Clamp();

			ChainedRules = new List<IItemDropRuleChainAttempt>();
		}
		public BasicDropRule(DropData dropData, float configChance) {
			itemID = dropData.ID;
			dropChance = dropData.Chance * configChance;
			dropChance.Clamp();

			ChainedRules = new List<IItemDropRuleChainAttempt>();
		}

		public bool CanDrop(DropAttemptInfo info) => true;

		public ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info) {
			ItemDropAttemptResult result;
			float randFloat = Main.rand.NextFloat();
			if (randFloat <= dropChance) {
				CommonCode.DropItem(info, itemID, 1);
				result = default(ItemDropAttemptResult);
				result.State = ItemDropAttemptResultState.Success;

				return result;
			}

			result = default(ItemDropAttemptResult);
			result.State = ItemDropAttemptResultState.FailedRandomRoll;

			return result;
		}

		public void ReportDroprates(List<DropRateInfo> drops, DropRateInfoChainFeed ratesInfo) {
			float chance = dropChance * ratesInfo.parentDroprateChance;
			drops.Add(new DropRateInfo(itemID, 1, 1, chance));

			Chains.ReportDroprates(ChainedRules, chance, drops, ratesInfo);
		}
	}
}
