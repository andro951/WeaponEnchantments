using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Items;

namespace WeaponEnchantments.Common
{
    public class OldItemManager
    {
        private enum OldItemContext
        {
            firstWordNames,
            searchWordNames,
            wholeNameReplaceWithItem,
            wholeNameReplaceWithCoins
        }
        private static Dictionary<string, string> firstWordNames = new Dictionary<string, string> { { "Critical", "CriticalStrikeChance" }, { "Size", "Scale" }, { "ManaCost", "Mana" }, { "Defence", "StatDefense" } };
        private static Dictionary<string, int> searchWordNames = new Dictionary<string, int> { { "SuperRare", 3 }, { "UltraRare", 4 }, { "Rare", 2 } };
        private static Dictionary<string, int> wholeNameReplaceWithItem = new Dictionary<string, int> { { "ContainmentFragment", ItemID.GoldBar }, {"Stabilizer", 177}, {"SuperiorStabilizer", 999} };
        private static Dictionary<string, int> wholeNameReplaceWithCoins = new Dictionary<string, int>();// { { "ContainmentFragment", 2000 } };
        public static void ReplaceAllOldItems()
        {
            if(UtilityMethods.debugging) ($"\\/ReplaceAllOldItems()").Log();
            int i = 0;
            foreach (Chest chest in Main.chest)
            {
                if (chest != null)
                {
                    if(UtilityMethods.debugging) ($"chest: {i}").Log();
                    ReplaceOldItems(chest.item);
                }
                i++;
            }
            if(UtilityMethods.debugging) ($"/\\ReplaceAllOldItems()").Log();
        }
        public static void ReplaceAllPlayerOldItems(Player player)
        {
            if(UtilityMethods.debugging) ($"\\/ReplaceAllPlayerOldItems(player: {player.S()})").Log();
            //"armor".Log();
            ReplaceOldItems(player.armor, player, 91);
            //"inventory".Log();
            ReplaceOldItems(player.inventory, player);
            //"bank1".Log();
            ReplaceOldItems(player.bank.item, player, 50, -2);
            //"bank2".Log();
            ReplaceOldItems(player.bank2.item, player, 50, -3);
            //"bank3".Log();
            ReplaceOldItems(player.bank3.item, player, 50, -4);
            //"bank4".Log();
            ReplaceOldItems(player.bank4.item, player, 50, -5);
            if(UtilityMethods.debugging) ($"/\\ReplaceAllPlayerOldItems(player: {player.S()})").Log();
        }
        private static void ReplaceOldItems(Item[] inventory, Player player = null, int itemSlotNumber = 0, int bank = -1)
        {
            if(UtilityMethods.debugging) ($"\\/ReplaceOldItems(inventory, player: {player.S()}, itemSlotNumber: {itemSlotNumber}, bank: {bank})").Log();
            for(int i = 0; i < inventory.Length; i++)
            {
                 ReplaceOldItem(ref inventory[i], player, itemSlotNumber + i, bank);
            }
            if(UtilityMethods.debugging) ($"/\\ReplaceOldItems(inventory, player: {player.S()}, itemSlotNumber: {itemSlotNumber}, bank: {bank})").Log();
        }
        public static void ReplaceOldItem(ref Item item, Player player = null, int itemSlotNumber = 0, int bank = -1)
        {
            if(item != null && !item.IsAir)
            {
                if(UtilityMethods.debugging) ($"\\/ReplaceOldItem(item: {item.S()}, player: {player.S()}, itemSlotNumber: {itemSlotNumber}, bank: {bank})").Log();
                
                if (item.ModItem is UnloadedItem)
                {
                    bool replaced = false;
					if (!replaced) {
                        replaced = TryReplaceItem(ref item, firstWordNames, OldItemContext.firstWordNames);
                    }
                    if (!replaced) {
                        replaced = TryReplaceItem(ref item, searchWordNames, OldItemContext.searchWordNames);
                    }
                    if (!replaced) {
                        replaced = TryReplaceItem(ref item, wholeNameReplaceWithItem, OldItemContext.wholeNameReplaceWithItem);
                    }
                    if (!replaced) {
                        TryReplaceItem(ref item, wholeNameReplaceWithCoins, OldItemContext.wholeNameReplaceWithCoins);
                    }
                }
                if (WEMod.IsEnchantable(item))
                {
                    item.G().needsUpdateOldItems = player == null;
                    if (item.TryGetGlobalItem(out EnchantedItem iGlobal))
                    {
                        for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
                        {
                            Item enchantmentItem = iGlobal.enchantments[i];
                            if (enchantmentItem.ModItem is UnloadedItem)
                                ReplaceOldItem(ref enchantmentItem, player);
                            if (enchantmentItem != null && !enchantmentItem.IsAir && player != null)
                            {
                                Enchantment enchantment = (Enchantment)enchantmentItem.ModItem;
                                if (WEMod.IsWeaponItem(item) && !enchantment.AllowedList.ContainsKey("Weapon"))
                                    RemoveEnchantmentNoUpdate(ref iGlobal.enchantments[i], player, enchantmentItem.Name + " is no longer allowed on weapons and has been removed from your " + item.Name + ".");
                                else if (WEMod.IsArmorItem(item) && !enchantment.AllowedList.ContainsKey("Armor"))
                                    RemoveEnchantmentNoUpdate(ref iGlobal.enchantments[i], player, enchantmentItem.Name + " is no longer allowed on armor and has been removed from your " + item.Name + ".");
                                else if (WEMod.IsAccessoryItem(item) && !enchantment.AllowedList.ContainsKey("Accessory"))
                                    RemoveEnchantmentNoUpdate(ref iGlobal.enchantments[i], player, enchantmentItem.Name + " is no longer allowed on acessories and has been removed from your " + item.Name + ".");
                                if (i == EnchantingTable.maxEnchantments - 1 && !enchantment.Utility)
                                    RemoveEnchantmentNoUpdate(ref iGlobal.enchantments[i], player, enchantmentItem.Name + " is no longer a utility enchantment and has been removed from your " + item.Name + ".");
                                if(enchantment.RestrictedClass > -1 && item.DamageType.Type == enchantment.RestrictedClass)
                                    RemoveEnchantmentNoUpdate(ref iGlobal.enchantments[i], player, enchantmentItem.Name + $" is no longer allowed on {item.DamageType.Name} weapons and has removed from your " + item.Name + ".");
                            }
                        }
                        if (player != null)
                        {
                            item.RemoveUntilPositive(player);

                            item.SetupGlobals();
                        }
                    }
                }

                if(UtilityMethods.debugging) ($"/\\ReplaceOldItem(item: {item.S()}, player: {player.S()}, itemSlotNumber: {itemSlotNumber}, bank: {bank})").Log();
            }
        }

        private static void RemoveEnchantmentNoUpdate(ref Item enchantmentItem, Player player, string msg)
        {
            enchantmentItem = player.GetItem(player.whoAmI, enchantmentItem, GetItemSettings.LootAllSettings);
            if (!enchantmentItem.IsAir)
            {
                player.QuickSpawnItem(player.GetSource_Misc("PlayerDropItemCheck"), enchantmentItem);
            }
            enchantmentItem = new Item();
            Main.NewText(msg);
        }
        private static bool TryReplaceItem(ref Item item, Dictionary<string, int> dict, OldItemContext context)
        {
            string name = ((UnloadedItem)item.ModItem).ItemName;
            string key = null;
            foreach (string k in dict.Keys)
            {
                switch (context) 
                {
                    case OldItemContext.searchWordNames://Not tested
                        int index = name.IndexOf(k);
                        if (index > -1)
                        {
                            key = k;
                            name = name.Substring(0, index - 1) + Enchantment.rarity[dict[key]] + name.Substring(index);
                        }
                        break;
                    default:
                        if(k == name)
                            key = k;
                        break;
                }
                if (key != null)
                    break;
            }
            if (key != null)
            {
                switch (context)
                {
                    case OldItemContext.searchWordNames://Not tested
                        foreach (ModItem modItem in ModContent.GetInstance<WEMod>().GetContent<ModItem>())
                        {
                            if (modItem is Enchantment enchantment)
                            {
                                if(enchantment.Name == name)
                                {
                                    ReplaceItem(ref item, enchantment.Item.type);
                                    return true;
                                }
                            }
                        }
                        break;
                    case OldItemContext.wholeNameReplaceWithItem:
                        ReplaceItem(ref item, dict[key]);
                        return true;
                    case OldItemContext.wholeNameReplaceWithCoins:
                        ReplaceItem(ref item, dict[key], true);
                        return true;
                }
            }//firstWordNames
            return false;
        }
        private static bool TryReplaceItem(ref Item item, Dictionary<string, string> dict, OldItemContext context) {
            string name = ((UnloadedItem)item.ModItem).ItemName;
            string key = null;
            foreach (string k in dict.Keys) {
                switch (context) {
                    case OldItemContext.firstWordNames:
                        if (name.Length >= k.Length) {
                            string keyCheck = name.Substring(0, k.Length);
                            if (keyCheck == k) {
                                key = k;
                            }
                        }
                        break;
                }
                if (key != null)
                    break;
            }
            if (key != null) {
                switch (context) {
                    case OldItemContext.firstWordNames:
                        foreach (ModItem modItem in ModContent.GetInstance<WEMod>().GetContent<ModItem>()) {
                            if (modItem is Enchantment enchantment) {
                                if (enchantment.EnchantmentTypeName == dict[key]) {
                                    int typeOffset = Enchantment.GetEnchantmentSize(name);
                                    ReplaceItem(ref item, enchantment.Item.type + typeOffset);
                                    return true;
                                }
                            }
                        }
                        break;
                }
            }//firstWordNames
            return false;
        }
        public static void ReplaceItem(ref Item item, int type, bool replaceWithCoins = false)
        {
            int stack = item.stack;
            if(type == 999)
                stack = stack / 4 + (stack % 4 > 0 ? 1 : 0);
            item.TurnToAir();
            if (replaceWithCoins)
            {
                UtilityMethods.SpawnCoins(type * stack, true);//type is coins when replaceWithCoins is true
            }
            else
            {
                item = new Item(type, stack);
            }
        }
    }
}
