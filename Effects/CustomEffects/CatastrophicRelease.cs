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
	public class CatastrophicRelease : StatEffect, INonVanillaStat, IUseItem, IUseTimer
    {
        public CatastrophicRelease(DifficultyStrength additive = null, DifficultyStrength multiplicative = null, DifficultyStrength flat = null, DifficultyStrength @base = null) : base(additive, multiplicative, flat, @base) {

		}
		public CatastrophicRelease(EStatModifier eStatModifier) : base(eStatModifier) { }
		public override EnchantmentEffect Clone() {
			return new CatastrophicRelease(EStatModifier.Clone());
		}

		public override string TooltipValue => EStatModifier.PercentMult100Tooltip;
		public override EnchantmentStat statName => EnchantmentStat.CatastrophicRelease;

		public Time TimerDuration { get; } = new Time(300);

		public EnchantmentStat TimerStatName => statName;

		public bool? UseItem(Item item, Player player) {
			((IUseTimer)this).SetTimer(player.GetWEPlayer());

			return null;
		}
	}
}
