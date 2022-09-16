using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;

namespace WeaponEnchantments.Effects
{
	public class Channel : BoolEffect, IUseItem, IUseTimer
	{
		public Channel(float minimumStrength = 0f, DifficultyStrength strengthData = null, bool prevent = false) : base(minimumStrength, strengthData, prevent) {

		}
		public override EnchantmentEffect Clone() {
			return new Channel(MinimumStrength, StrengthData.Clone(), !EnableStat);
		}

		public override EnchantmentStat statName => EnchantmentStat.Channel;

		public Time TimerDuration { get; } = new Time(1000);
		private Item Item;

		public EnchantmentStat TimerStatName => statName;

		public bool? UseItem(Item item, Player player) {
			item.channel = true;
			item.useStyle = ItemUseStyleID.Shoot;
			item.reuseDelay = 5;
			item.autoReuse = false;
			Item = item;
			((IUseTimer)this).SetTimer(player.GetWEPlayer());

			return null;
		}

		public void TimerEnd(WEPlayer wePlayer) {
			Item sampleItem = ContentSamples.ItemsByType[Item.type];
			Item.channel = sampleItem.channel;
			Item.useStyle = sampleItem.useStyle;
			Item.reuseDelay = sampleItem.reuseDelay;
		}
	}
}
