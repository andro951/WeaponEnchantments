using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Common.Globals
{
	public class WEGlobalItem : GlobalItem
	{
		public override void GrabRange(Item item, Player player, ref int grabRange) {
			if (player?.TryGetModPlayer(out WEPlayer wePlayer) == true) {
				if (wePlayer.CheckEnchantmentStats(EnchantmentStat.PickupRange, out float mult, 1f))
					grabRange = (int)Math.Round((float)grabRange * mult);
			}
		}
	}
}
