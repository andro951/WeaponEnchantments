using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Items;

namespace WeaponEnchantments.Common.Globals
{
    public class EnchantmentEssenceGlobal : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            if(entity.ModItem != null)
            {
                if (entity.ModItem is EnchantmentEssenceBasic)
                    return true;
            }
            return false;
        }
        /*public override void UpdateInventory(Item item, Player player)
        {
            WEPlayer wePlayer = player.G();
            EnchantmentEssenceBasic modItem = (EnchantmentEssenceBasic)item.ModItem;
            if (WEMod.config.clientConfig.teleportEssence && !wePlayer.usingEnchantingTable)
            {
                if (item.stack + wePlayer.enchantingTable.essenceItem[modItem.essenceRarity].stack < item.maxStack)
                {
                    if (wePlayer.enchantingTable.essenceItem[modItem.essenceRarity].stack < 1)
                        wePlayer.enchantingTable.essenceItem[modItem.essenceRarity] = new Item(ModContent.ItemType<EnchantmentEssenceBasic>() + modItem.essenceRarity, item.stack);
                    else
                        wePlayer.enchantingTable.essenceItem[modItem.essenceRarity].stack += item.stack;
                    item.stack = 0;
                    
                }
            }
        }*/
        public override bool CanPickup(Item item, Player player)
        {
            WEPlayer wePlayer = player.G();
            EnchantmentEssenceBasic modItem = (EnchantmentEssenceBasic)item.ModItem;
            if (WEMod.config.clientConfig.teleportEssence && !wePlayer.usingEnchantingTable)
            {
                if (item.stack + wePlayer.enchantingTable.essenceItem[modItem.essenceRarity].stack <= item.maxStack)
                {
                    if (wePlayer.enchantingTable.essenceItem[modItem.essenceRarity].stack < 1)
                        wePlayer.enchantingTable.essenceItem[modItem.essenceRarity] = new Item(ModContent.ItemType<EnchantmentEssenceBasic>() + modItem.essenceRarity, item.stack);
                    else
                        wePlayer.enchantingTable.essenceItem[modItem.essenceRarity].stack += item.stack;
                    item.stack = 0;
                    PopupText.NewText(PopupTextContext.RegularItemPickup, item, item.stack);
                    item.TurnToAir();
                    SoundEngine.PlaySound(SoundID.Grab);

                    return false;
                }
            }
            return true;
        }
    }
}
