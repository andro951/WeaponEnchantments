using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using WeaponEnchantments.Items;
using WeaponEnchantments.UI;
using WeaponEnchantments.Common.Utility;

namespace WeaponEnchantments.Common.Globals
{
	public class VacuumableItemsGlobal : GlobalItem
	{
		public static bool CanVacuum => WEPlayer.LocalWEPlayer.vacuumItemsIntoEnchantmentStorage && (!WEPlayer.LocalWEPlayer.displayEnchantmentStorage || EnchantmentStorage.uncrafting);
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
			return EnchantmentStorage.CanBeStored(entity);
		}

		public override bool OnPickup(Item item, Player player) {
			if (item.NullOrAir() || item.ModItem == null)
				return false;

			WEPlayer wePlayer = player.GetWEPlayer();
			if (CanVacuum) {
				if (!EnchantmentStorage.CanBeStored(item))
					return false;

				Item cloneForInfo = item.Clone();
				if (EnchantmentStorage.DepositAll(ref item)) {
					PopupText.NewText(PopupTextContext.RegularItemPickup, cloneForInfo, cloneForInfo.stack - item.stack);
					SoundEngine.PlaySound(SoundID.Grab);
					if (item.NullOrAir() || item.stack < 1)
						return false;
				}
			}

			return true;
		}
		public override bool ItemSpace(Item item, Player player) {
			WEPlayer wePlayer = player.GetWEPlayer();
			if (CanVacuum) {
				return EnchantmentStorage.ItemSpace(item);
			}

			return false;
		}
	}
}
