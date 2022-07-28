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
        public override bool OnPickup(Item item, Player player)
        {
            WEPlayer wePlayer = player.GetWEPlayer();
            EnchantmentEssenceBasic essence = (EnchantmentEssenceBasic)item.ModItem;
            if (WEMod.clientConfig.teleportEssence && !wePlayer.usingEnchantingTable)
            {
                int rarity = essence.essenceRarity;
                int tableStack = wePlayer.enchantingTable.essenceItem[rarity].stack;
                //Less than max stack when combined
                if (item.stack + tableStack <= item.maxStack)
                {
                    if (wePlayer.enchantingTable.essenceItem[rarity].stack < 1) {
                        //Table is empty
                        int basicEssenceType = ModContent.ItemType<EnchantmentEssenceBasic>();
                        wePlayer.enchantingTable.essenceItem[rarity] = new Item(basicEssenceType + rarity, item.stack);
                    }
					else {
                        //Table not empty
                        wePlayer.enchantingTable.essenceItem[rarity].stack += item.stack;
                    }
                    
                    PopupText.NewText(PopupTextContext.RegularItemPickup, item, item.stack);
                    SoundEngine.PlaySound(SoundID.Grab);
                    item.TurnToAir();

                    return false;
                }
            }

            return true;
        }
    }
}
