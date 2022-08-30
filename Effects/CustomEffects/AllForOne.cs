using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects
{
	public class AllForOne : EnchantmentEffect, IUseTimer, ICanUseItem, IUseItem {
		public AllForOne(DifficultyStrength timerStrength) {
			_timerStrength = timerStrength;
		}

		public override string Tooltip => $"(Item CD equal to {EffectStrength * 0.8f}x use speed)";

		private DifficultyStrength _timerStrength;
		public Time TimerDuration => null;

		public EnchantmentStat TimerStatName => EnchantmentStat.AllForOne;

		public bool CanUseItem(Item item, Player player) {
			return (bool)((IUseTimer)this).TimerOver(player);
		}
		public void TimerEnd(WEPlayer wePlyaer) {
			SoundEngine.PlaySound(SoundID.Unlock);
		}
		public bool? UseItem(Item item, Player player) {
			int duration = (int)((float)item.useTime * _timerStrength.Value);
			player.GetWEPlayer().SetEffectTimer(this, duration);

			return null;
		}
	}
}
