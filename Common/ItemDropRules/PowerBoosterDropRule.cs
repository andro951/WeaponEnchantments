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
using WeaponEnchantments.Items;
using System.Collections;
using androLib.Common.Utility;

namespace WeaponEnchantments.Common
{
	public class PowerBoosterDropRule : IItemDropRule
	{
		private static bool CanDropItem(int netID, out int boosterType) {
			if (WEMod.serverConfig.PreventPowerBoosterFromPreHardMode && WEGlobalNPC.PreHardModeBossTypes.Contains(netID)) {
				boosterType = ItemID.None;
			}
			else if (WEGlobalNPC.PostPlanteraBossTypes.Contains(netID)) {
				boosterType = ModContent.ItemType<UltraPowerBooster>();
			}
			else {
				boosterType = ModContent.ItemType<PowerBooster>();
			}

			return boosterType > ItemID.None;
		}
		private int netID;
		private float dropChance;
		public List<IItemDropRuleChainAttempt> ChainedRules {
			get;
			private set;
		}

		public PowerBoosterDropRule(int NpcNetID, float DropChance) {
			netID = NpcNetID;
			dropChance = DropChance;

			ChainedRules = new List<IItemDropRuleChainAttempt>();
		}

		public bool CanDrop(DropAttemptInfo info) => CanDropItem(netID, out _);

		public ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info) {
			ItemDropAttemptResult result = default(ItemDropAttemptResult);
			if (!CanDropItem(netID, out int powerBoosterType)) {
				result.State = ItemDropAttemptResultState.DoesntFillConditions;
			}
			else {
				Item item = powerBoosterType.CSI();
				float randFloat = Main.rand.NextFloat();
				if (randFloat <= dropChance) {
					CommonCode.DropItem(info, powerBoosterType, 1);
					result.State = ItemDropAttemptResultState.Success;
				}
				else {
					result.State = ItemDropAttemptResultState.FailedRandomRoll;
				}
			}

			return result;
		}

		public void ReportDroprates(List<DropRateInfo> drops, DropRateInfoChainFeed ratesInfo) {
			float chance = dropChance * ratesInfo.parentDroprateChance;
			if (CanDropItem(netID, out int powerBoosterType))
				drops.Add(new DropRateInfo(powerBoosterType, 1, 1, chance));

			Chains.ReportDroprates(ChainedRules, chance, drops, ratesInfo);
		}
	}
}
