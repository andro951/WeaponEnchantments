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
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.ModItem != null && entity.ModItem is EnchantmentEssence; // The item to which this script applies will always be an enchantment essence
        }

        public override bool OnPickup(Item item, Player player)
        {
            EnchantmentEssence essence = (EnchantmentEssence)item.ModItem;
            WEPlayer wePlayer = player.G();
            if (WEMod.clientConfig.teleportEssence && !wePlayer.usingEnchantingTable)
            {
                List<Item> essenceSlots = wePlayer.enchantingTable.essenceItem;
                int rarity = essence.essenceRarity;
                int tableStack = essenceSlots[rarity].stack;
                int toStore = Math.Min(item.maxStack - tableStack, item.stack);
                item.stack -= toStore;
                //Less than max stack when combined

                if (essenceSlots[rarity].stack < 1) {
                    //Table is empty
                    essenceSlots[rarity] = new Item(item.type, toStore);
                }
				else {
                    //Table not empty
                    essenceSlots[rarity].stack += toStore;
                }

                PopupText.NewText(PopupTextContext.RegularItemPickup, item, toStore);
                if (item.stack < 1)
                {
                    SoundEngine.PlaySound(SoundID.Grab);
                    item.TurnToAir();
                    return false;
                }
            }
            return true;
        }
    }
}
