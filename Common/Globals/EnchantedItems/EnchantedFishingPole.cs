using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace WeaponEnchantments.Common.Globals.EnchantedItems
{
	public class EnchantedFishingPole : EnchantedItem
	{
		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
			return entity.fishingPole > 0;
		}

		public override void CaughtFishStack(int type, ref int stack) {
			base.CaughtFishStack(type, ref stack);
		}
	}
}
