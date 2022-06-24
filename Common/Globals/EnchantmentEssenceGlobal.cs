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
            if (WEMod.serverConfig.clientConfig.teleportEssence && !wePlayer.usingEnchantingTable)
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
        public override bool OnPickup(Item item, Player player)
        {
            WEPlayer wePlayer = player.G();
            EnchantmentEssenceBasic modItem = (EnchantmentEssenceBasic)item.ModItem;
            if (WEMod.clientConfig.teleportEssence && !wePlayer.usingEnchantingTable)
            {
                if (item.stack + wePlayer.enchantingTable.essenceItem[modItem.essenceRarity].stack <= item.maxStack)
                {
                    if (wePlayer.enchantingTable.essenceItem[modItem.essenceRarity].stack < 1)
                        wePlayer.enchantingTable.essenceItem[modItem.essenceRarity] = new Item(ModContent.ItemType<EnchantmentEssenceBasic>() + modItem.essenceRarity, item.stack);
                    else
                        wePlayer.enchantingTable.essenceItem[modItem.essenceRarity].stack += item.stack;
                    PopupText.NewText(PopupTextContext.RegularItemPickup, item, item.stack);
                    SoundEngine.PlaySound(SoundID.Grab);
                    item.TurnToAir();
                    return false;
                }
            }
            return true;
        }
        /*public override bool CanPickup(Item item, Player player)
        {
            if(Main.netMode == NetmodeID.SinglePlayer)
            {
                //($"\\/CanPickup(item: {item.S()}, player: {player.S()})").Log();
                WEPlayer wePlayer = player.G();
                EnchantmentEssenceBasic modItem = (EnchantmentEssenceBasic)item.ModItem;
                if (WEMod.clientConfig.teleportEssence && !wePlayer.usingEnchantingTable)
                {
                    if (item.stack + wePlayer.enchantingTable.essenceItem[modItem.essenceRarity].stack <= item.maxStack)
                    {
                        //wePlayer.PickUpEssence(modItem.essenceRarity, item.stack);
                        wePlayer.PickUpEssence(item.whoAmI);
                        //PopupText.NewText(PopupTextContext.RegularItemPickup, item, item.stack);
                        //bool popupTextActive = Main.popupText[0].active;
                        //PopupText.NewText(PopupTextContext.RegularItemPickup, new Item(item.type, item.stack), item.stack);
                        item.TurnToAir();
                        //($"/\\CanPickup(item: {item.S()}, player: {player.S()}) return false").Log();
                        return false;
                    }
                }
                //($"/\\CanPickup(item: {item.S()}, player: {player.S()}) return true").Log();
                return true;
            }
            else
            {
                if (WEMod.playerTeleportItemSetting.ContainsKey(player.name))
                {
                    if (WEMod.playerTeleportItemSetting[player.name])
                    {
                        ModPacket packet = ModContent.GetInstance<WEMod>().GetPacket();
                        EnchantmentEssenceBasic modItem = (EnchantmentEssenceBasic)item.ModItem;
                        packet.Write(WEMod.PacketIDs.PickUpEssence);
                        packet.Write((byte)modItem.essenceRarity);
                        packet.Write(item.stack);
                        packet.Send(player.whoAmI);
                        item.TurnToAir();
                        return true;
                    }
                }
                return true;
            }
        }*/
    }
}
