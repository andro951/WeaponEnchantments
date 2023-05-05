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
	public class VacuumToStorageItems : GlobalItem
	{
		public bool favorited;
		public static bool CanVacuum(Player player) => player.GetWEPlayer().vacuumItemsIntoEnchantmentStorage;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
			return EnchantmentStorage.CanBeStored(entity);
		}
		public override void LoadData(Item item, TagCompound tag) {
			favorited = item.favorited;
		}
		public override void SaveData(Item item, TagCompound tag) {
			
		}
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

			if (item.NullOrAir() || item.ModItem == null)
				return true;

			WEPlayer wePlayer = player.GetWEPlayer();
			if (CanVacuum(player)) {
				if (!EnchantmentStorage.CanBeStored(item))
					return true;

				Item cloneForInfo = item.Clone();
				if (EnchantmentStorage.DepositAll(ref item)) {
					PopupText.NewText(PopupTextContext.RegularItemPickup, cloneForInfo, cloneForInfo.stack - item.stack);
					SoundEngine.PlaySound(SoundID.Grab);
					if (item.NullOrAir() || item.stack < 1) {
						if (wePlayer.trashEnchantmentsFullNames.Contains(cloneForInfo.type.GetItemIDOrName())) {
							EnchantmentStorage.UncraftTrash(cloneForInfo);
						}

						return false;
					}
				}
			}

			return true;
		}
		public override bool ItemSpace(Item item, Player player) {
			bool result = CanVacuum(player) && EnchantmentStorage.RoomInStorage(item, player);
			return result;
		}
	}
}
