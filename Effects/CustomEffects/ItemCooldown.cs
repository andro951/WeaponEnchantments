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
	public class ItemCooldown : EnchantmentEffect, IUseTimer, ICanUseItem, IUseItem {
		public ItemCooldown(DifficultyStrength timerStrength) {
			_timerStrength = timerStrength;
		}
		public override string Tooltip => StandardTooltip;
		public override IEnumerable<object> TooltipArgs => new object[] { TooltipValue };
		public override string TooltipValue => $"{EffectStrength.S()}x";
		public override float EffectStrength => _timerStrength.Value * 0.8f;

		private DifficultyStrength _timerStrength;
		public Time TimerDuration => null;

		public EnchantmentStat TimerStatName => EnchantmentStat.AllForOne;

		public bool CanUseItem(Item item, Player player) {
			return ((IUseTimer)this).TimerOver(player);
		}
		public void TimerEnd(WEPlayer wePlyaer) {
			SoundEngine.PlaySound(SoundID.Unlock);
		}
		public bool? UseItem(Item item, Player player) {
			int duration = (int)((float)item.useTime * EffectStrength);
			if (Main.netMode < NetmodeID.Server && player?.whoAmI == Main.myPlayer)
				player.GetWEPlayer().SetEffectTimer(this, duration);

			return null;
		}
		public override EnchantmentEffect Clone() {
			return new ItemCooldown(_timerStrength.Clone());
		}
	}
}
