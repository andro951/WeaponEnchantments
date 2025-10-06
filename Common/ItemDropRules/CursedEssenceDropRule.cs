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
using WeaponEnchantments.Debuffs;
using androLib.Common.Globals;
using static System.Text.StringBuilder;
using Terraria.ModLoader.IO;
using WeaponEnchantments.Common.Configs;

namespace WeaponEnchantments.Common
{
	public class CursedEssenceDropRule : IItemDropRule
	{
		private static bool CanDropItem(NPC npc) => npc.TryGetGlobalNPC(out CursedNPC cursedNPC) && cursedNPC.Cursed || npc.IsBoss();
		public List<IItemDropRuleChainAttempt> ChainedRules {
			get;
			private set;
		}

		public CursedEssenceDropRule() {
			ChainedRules = new List<IItemDropRuleChainAttempt>();
		}

		public bool CanDrop(DropAttemptInfo info) => CanDropItem(info.npc);
		private static int GetRandStack(float chance) {
			int stack = (int)chance;
			chance -= stack;
			if (Main.rand.NextFloat() <= chance)
				stack++;

			return stack;
		}
		private int GetStack(NPC npc) {
			if (npc.realLife != -1)
				return 0;

			if (npc.netID >= NPCID.EaterofWorldsHead && npc.netID <= NPCID.EaterofWorldsTail) {
				if (Main.rand.NextBool(50)) {
					return GetRandStack(ConfigValues.CursedEssenceDropChanceMultiplier);
				}
				else {
					return 0;
				}
			}

			if (npc.IsBoss() || npc.IsMiniBoss()) {
				return GetRandStack(ConfigValues.CursedEssenceDropChanceMultiplier);
			}

			if (!npc.GetXPMultiplierFromNPC(out float xpMultiplier))
				return 0;

			if (npc.RealLifeMax() <= 10)
				return 0;

			float min = ConfigValues.CursedEssenceDropChanceMultiplier < 1f ? 1f : 1f / ConfigValues.CursedEssenceDropChanceMultiplier;
			float cursedEssence = Math.Max(xpMultiplier * npc.netID.CSNPC().RealLifeMax() / 1000f, min);
			int cursedEssenceAmount = GetRandStack(cursedEssence * ConfigValues.CursedEssenceDropChanceMultiplier);
			int cursedEssenceType = ModContent.ItemType<CursedEssence>();
			int stack = Math.Min(cursedEssenceAmount, cursedEssenceType.CSI().maxStack);

			return GetRandStack(ConfigValues.CursedEssenceDropChanceMultiplier * stack);
		} 

		public ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info) {
			ItemDropAttemptResult result = default(ItemDropAttemptResult);
			if (!CanDropItem(info.npc)) {
				result.State = ItemDropAttemptResultState.DoesntFillConditions;
			}
			else {
				int stack = GetStack(info.npc);
				if (stack >= 0) {
					CommonCode.DropItem(info, ModContent.ItemType<CursedEssence>(), stack);
					result.State = ItemDropAttemptResultState.Success;
				}
				else {
					result.State = ItemDropAttemptResultState.DoesntFillConditions;
				}
			}

			return result;
		}

		public void ReportDroprates(List<DropRateInfo> drops, DropRateInfoChainFeed ratesInfo) {
			DropRateInfoChainFeed ratesInfo2 = ratesInfo.With(1f);
			float dropRate = 1f;
			drops.Add(new DropRateInfo(ModContent.ItemType<CursedEssence>(), 1, 1, dropRate, ratesInfo2.conditions));
			Chains.ReportDroprates(ChainedRules, dropRate, drops, ratesInfo2);
		}
	}
}
