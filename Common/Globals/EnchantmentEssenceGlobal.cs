using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Items;
using WeaponEnchantments.ModLib.KokoLib;
using WeaponEnchantments.UI;
using androLib.Common.Utility;

namespace WeaponEnchantments.Common.Globals
{
    public class EnchantmentEssenceGlobal : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
            // The item to which this script applies will always be an enchantment essence
            return entity.ModItem != null && entity.ModItem is EnchantmentEssence;
        }

        public override bool OnPickup(Item item, Player player) {
			if (player.whoAmI != Main.myPlayer)
				return true;

			if (item.NullOrAir() || item.ModItem == null)
                return true;

            WEPlayer wePlayer = player.GetWEPlayer();
            if (WEMod.clientConfig.teleportEssence) {
                if (item.ModItem is not EnchantmentEssence essence)
                    return true;

                int toStore = item.stack - EnchantingTableUI.GetEssence(essence.EssenceTier, item.stack, false, wePlayer);

                if (toStore <= 0)
                    return true;

                item.stack -= toStore;

                PopupText.NewText(PopupTextContext.RegularItemPickup, item, toStore);
                SoundEngine.PlaySound(SoundID.Grab);
                if (item.stack < 1) {
                    item.TurnToAir();

                    return false;
                }
            }

            return true;
        }
		public override void GrabRange(Item item, Player player, ref int grabRange) {
            grabRange *= WEMod.serverConfig.EssenceGrabRange;
		}

		public override bool ItemSpace(Item item, Player player) {
            if (Main.netMode == NetmodeID.Server)
                return true;

			WEPlayer wePlayer = player.GetWEPlayer();
			if (WEMod.clientConfig.teleportEssence) {
				EnchantmentEssence essence = (EnchantmentEssence)item.ModItem;
				Item[] essenceSlots = wePlayer.enchantingTableEssence;
				if (essenceSlots == null)
					return false;

				int tier = essence.EssenceTier;
				if (essenceSlots[tier] == null)
					return false;

				int tableStack = essenceSlots[tier].stack;
				if (tableStack == 0 || tableStack < essenceSlots[tier].maxStack)
					return true;
			}

			return false;
		}
	}
}
