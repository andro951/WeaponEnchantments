using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects
{
	public class QuestFishChance : StatEffect, INonVanillaStat
    {
        public QuestFishChance(DifficultyStrength additive = null, DifficultyStrength multiplicative = null, DifficultyStrength flat = null, DifficultyStrength @base = null) : base(additive, multiplicative, flat, @base) {

        }
        public QuestFishChance(EStatModifier eStatModifier) : base(eStatModifier) { }
        public override EnchantmentEffect Clone() {
            return new QuestFishChance(EStatModifier.Clone());
        }

        public override string Tooltip => $"{EStatModifier.SignPercentMult100Tooltip} {DisplayName} (Quest fish caught will be automatically turned in and start a new quest, bypassing the 1 per day limmit.)";
		public override EnchantmentStat statName => EnchantmentStat.QuestFishChance;
	}
}
