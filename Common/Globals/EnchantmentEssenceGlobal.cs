using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Items;

namespace WeaponEnchantments.Common.Globals
{
    public class EnchantmentEssenceGlobal : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
            // The item to which this script applies will always be an enchantment essence
            return entity.ModItem != null && entity.ModItem is EnchantmentEssence;
        }

        public override bool OnPickup(Item item, Player player) {
            if (item == null || item.ModItem == null)
                return false;

            WEPlayer wePlayer = player.GetWEPlayer();
            if (WEMod.clientConfig.teleportEssence && !wePlayer.usingEnchantingTable) {
                if (item.ModItem is not EnchantmentEssence essence)
                    return false;

                Item[] essenceSlots = wePlayer.enchantingTableEssence;
                int tier = essence.EssenceTier;
                int tableStack = essenceSlots[tier].stack;
                int toStore = Math.Min(item.maxStack - tableStack, item.stack);

                if (toStore <= 0)
                    return true;

                item.stack -= toStore;
                //Less than max stack when combined

                if (essenceSlots[tier].stack < 1) {
                    //Table is empty
                    essenceSlots[tier] = new Item(item.type, toStore);
                }
				else {
                    //Table not empty
                    essenceSlots[tier].stack += toStore;
                }

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
            WEPlayer wePlayer = player.GetWEPlayer();
            if (WEMod.clientConfig.teleportEssence && !wePlayer.usingEnchantingTable) {
                EnchantmentEssence essence = (EnchantmentEssence)item.ModItem;
                Item[] essenceSlots = wePlayer.enchantingTableEssence;
                int tier = essence.EssenceTier;
                int tableStack = essenceSlots[tier].stack;
                if (tableStack == 0 || tableStack < essenceSlots[tier].maxStack)
                    return true;
            }

            return false;
        }
	}
}
