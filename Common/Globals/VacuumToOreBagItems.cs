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
using Terraria.ModLoader.IO;
using Steamworks;

namespace WeaponEnchantments.Common.Globals
{
	public class VacuumToOreBagItems : GlobalItem
	{
		public bool favorited;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
			return true;
		}
		public override void LoadData(Item item, TagCompound tag) {
			favorited = item.favorited;
		}
		public override void SaveData(Item item, TagCompound tag) {

		}
		public static bool CanVacuum(Player player) => player.GetWEPlayer().vacuumItemsIntoOreBag && player.HasItem(ModContent.ItemType<OreBag>());
		public override void UpdateInventory(Item item, Player player) {
			//Track favorited
			if (item.favorited) {
				if (!favorited && WEModSystem.FavoriteKeyDown) {
					favorited = true;
				}
			}
			else {
				if (favorited) {
					if (!WEModSystem.FavoriteKeyDown) {
						item.favorited = true;
					}
					else {
						favorited = false;
					}
				}
			}
		}
		public override bool OnPickup(Item item, Player player) {
			if (player.whoAmI != Main.myPlayer)
				return true;

			if (item.NullOrAir())
				return true;

			if (CanVacuum(player)) {
				if (!OreBagUI.CanBeStored(item))
					return true;

				Item cloneForInfo = item.Clone();
				if (OreBagUI.DepositAll(ref item)) {
					PopupText.NewText(PopupTextContext.RegularItemPickup, cloneForInfo, cloneForInfo.stack - item.stack);
					SoundEngine.PlaySound(SoundID.Grab);
					if (item.NullOrAir() || item.stack < 1)
						return false;
				}
			}

			return true;
		}
		public override bool ItemSpace(Item item, Player player) {
			return CanVacuum(player) && OreBagUI.RoomInStorage(item, player);
		}
	}
}
