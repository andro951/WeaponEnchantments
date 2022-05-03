using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI.Chat;
using WeaponEnchantments.Common;
using WeaponEnchantments.UI;
using WeaponEnchantments.Items;
using WeaponEnchantments.Common.Globals;

namespace WeaponEnchantments
{ 
    public class WEPlayer : ModPlayer
    {
        public bool usingEnchantingTable;
        public int enchantingTableTier;
        public bool itemInEnchantingTable;
        public bool[] enchantmentInEnchantingTable = new bool[EnchantingTable.maxEnchantments];
        public Item itemBeingEnchanted;
        public EnchantingTable enchantingTable = new EnchantingTable();
        public WeaponEnchantmentUI enchantingTableUI = new WeaponEnchantmentUI();
        public ConfirmationUI confirmationUI = new ConfirmationUI();
        public Item[] inventoryItemRecord = new Item[102];
        public int lastFocusRecipeListNum = -1;
        public int lastFocusRecipe = -1;

        /*
        public void RefreshModItems()
        {
            RefreshItemArray(enchantingTable.item);
            RefreshItemArray(enchantingTable.enchantmentItem);
            RefreshItemArray(enchantingTable.essenceItem);
        }
        */

        public override void Initialize()
        {

        }
        public override void SaveData(TagCompound tag)
        {
            for (int i = 0; i < EnchantingTable.maxItems; i++)
            {
                if (!enchantingTable.item[i].IsAir)
                {
                    tag["enchantingTableItem" + i.ToString()] = enchantingTable.item[i];
                }
            }
            if (enchantingTableUI?.itemSlotUI[0]?.Item != null)
            {
                if (enchantingTableUI.itemSlotUI[0].Item.IsAir)
                {
                    for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
                    {
                        if (!enchantingTable.enchantmentItem[i].IsAir)
                        {
                            tag["enchantingTableEnchantmentItem" + i.ToString()] = enchantingTable.enchantmentItem[i];
                        }
                    }
                }//enchantments in the enchantmentSlots are saved by global items.  This is just in case enchantment items are left in after Offering items.
            }
            for (int i = 0; i < EnchantingTable.maxEssenceItems; i++)
            {
                if (!enchantingTable.essenceItem[i].IsAir)
                {
                    tag["enchantingTableEssenceItem" + i.ToString()] = enchantingTable.essenceItem[i];
                }
            }
        }
        public override void LoadData(TagCompound tag)
        {
            for (int i = 0; i < EnchantingTable.maxItems; i++)
            {
                if (tag.Get<Item>("enchantingTableItem" + i.ToString()).IsAir)
                {
                    enchantingTable.item[i] = new Item();
                }
                else
                {
                    enchantingTable.item[i] = tag.Get<Item>("enchantingTableItem" + i.ToString());
                }
            }
            if (enchantingTable.item[0].IsAir)//enchantments in the enchantmentSlots are loaded by global items.  This is just in case enchantment items are left in after Offering items.
            {
                for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
                {
                    if (tag.Get<Item>("enchantingTableEnchantmentItem" + i.ToString()).IsAir)
                    {
                        enchantingTable.enchantmentItem[i] = new Item();
                    }
                    else
                    {
                        enchantingTable.enchantmentItem[i] = tag.Get<Item>("enchantingTableEnchantmentItem" + i.ToString());
                    }
                }
            }
            for (int i = 0; i < EnchantingTable.maxEssenceItems; i++)
            {
                if (tag.Get<Item>("enchantingTableEssenceItem" + i.ToString()).IsAir)
                {
                    enchantingTable.essenceItem[i] = new Item();
                }
                else
                {
                    enchantingTable.essenceItem[i] = tag.Get<Item>("enchantingTableEssenceItem" + i.ToString());
                }
            }
        }
        public override bool ShiftClickSlot(Item[] inventory, int context, int slot)
        {
            if (usingEnchantingTable)
            {
                if(inventory == Player.inventory)
                {
                    if (Main.mouseItem.IsAir)
                    {
                        bool valid = false;
                        if (inventory[slot].type == PowerBooster.ID && !enchantingTableUI.itemSlotUI[0].Item.IsAir && !enchantingTableUI.itemSlotUI[0].Item.GetGlobalItem<EnchantedItem>().powerBoosterInstalled)
                        {
                            inventory[slot] = new Item();
                            enchantingTableUI.itemSlotUI[0].Item.GetGlobalItem<EnchantedItem>().powerBoosterInstalled = true;
                            SoundEngine.PlaySound(SoundID.Grab);
                            valid = true;
                        }//If using a PowerBooster, destroy the booster and update the global item.
                        else
                        {
                            for (int i = 0; i < EnchantingTable.maxItems; i++)
                            {
                                if (enchantingTableUI.itemSlotUI[i].Valid(inventory[slot]))
                                {
                                    if (!inventory[slot].IsAir)
                                    {
                                        enchantingTableUI.itemSlotUI[i].Item = inventory[slot].Clone();
                                        inventory[slot] = new Item();
                                        SoundEngine.PlaySound(SoundID.Grab);
                                        valid = true;
                                    }
                                }
                            }
                            if (!valid)
                            {
                                for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
                                {
                                    if (enchantingTableUI.enchantmentSlotUI[i].Valid(inventory[slot]))
                                    {
                                        if (!inventory[slot].IsAir && enchantingTableUI.enchantmentSlotUI[i].Item.IsAir)
                                        {
                                            if (((Enchantments)inventory[slot].ModItem).utility && enchantingTableUI.enchantmentSlotUI[4].Item.IsAir)
                                            {
                                                enchantingTableUI.enchantmentSlotUI[4].Item = inventory[slot].Clone();
                                                inventory[slot] = new Item();
                                            }
                                            else
                                            {
                                                enchantingTableUI.enchantmentSlotUI[i].Item = inventory[slot].Clone();
                                                inventory[slot] = new Item();
                                            }
                                            SoundEngine.PlaySound(SoundID.Grab);
                                            valid = true;
                                        }
                                    }
                                }
                            }
                            if (!valid)
                            {
                                for (int i = 0; i < EnchantingTable.maxEssenceItems; i++)
                                {
                                    if (enchantingTableUI.essenceSlotUI[i].Valid(inventory[slot]))
                                    {
                                        if (!inventory[slot].IsAir)
                                        {
                                            bool transfered = false;
                                            if (enchantingTableUI.essenceSlotUI[i].Item.IsAir)
                                            {
                                                enchantingTableUI.essenceSlotUI[i].Item = inventory[slot].Clone();
                                                inventory[slot] = new Item();
                                                transfered = true;
                                            }
                                            else
                                            {
                                                if (enchantingTableUI.essenceSlotUI[i].Item.stack < EnchantmentEssence.maxStack)
                                                {
                                                    int ammountToTransfer;
                                                    if (inventory[slot].stack + enchantingTableUI.essenceSlotUI[i].Item.stack > EnchantmentEssence.maxStack)
                                                    {
                                                        ammountToTransfer = EnchantmentEssence.maxStack - enchantingTableUI.essenceSlotUI[i].Item.stack;
                                                        inventory[slot].stack -= ammountToTransfer;
                                                    }
                                                    else
                                                    {
                                                        ammountToTransfer = inventory[slot].stack;
                                                        inventory[slot].stack = 0;
                                                    }
                                                    enchantingTableUI.essenceSlotUI[i].Item.stack += ammountToTransfer;
                                                    transfered = true;
                                                }
                                            }
                                            if (transfered)
                                            {
                                                SoundEngine.PlaySound(SoundID.Grab);
                                                valid = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (!valid)
                        {
                            Main.mouseItem = inventory[slot].Clone();
                            inventory[slot] = new Item();
                        }
                    }
                    return true;
                }
            }
            return false;
        }
        public void CustomFindRecipeis()
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            int availableRecipe;
            float availableRecipeY;
            if (lastFocusRecipe > -1 && lastFocusRecipeListNum > Main.focusRecipe && Main.focusRecipe == Main.numAvailableRecipes - 1)
            {
                availableRecipe = lastFocusRecipe;
                //availableRecipeY = lastFocusRecipeY;
            }
            else
            {
                availableRecipe = Main.availableRecipe[Main.focusRecipe];
                //availableRecipeY = Main.availableRecipeY[Main.focusRecipe];
            }
            availableRecipeY = Main.availableRecipeY[Main.focusRecipe];
            //for (int i = 0; i < Recipe.maxRecipes; i++)
            //{
            //    Main.availableRecipe[i] = 0;
            //}

            //Main.numAvailableRecipes = 0;
            /*if (Main.guideItem.type > 0 && Main.guideItem.stack > 0 && Main.guideItem.Name != "")
            {
                for (int j = 0; j < Recipe.maxRecipes && Main.recipe[j].createItem.type != 0; j++)
                {
                    for (int k = 0; k < Main.recipe[j].requiredItem.Count; k++)
                    {
                        if (Main.guideItem.IsTheSameAs(Main.recipe[j].requiredItem[k]) || Main.recipe[j].useWood(Main.guideItem.type, Main.recipe[j].requiredItem[k].type) || Main.recipe[j].useSand(Main.guideItem.type, Main.recipe[j].requiredItem[k].type) || Main.recipe[j].useIronBar(Main.guideItem.type, Main.recipe[j].requiredItem[k].type) || Main.recipe[j].useFragment(Main.guideItem.type, Main.recipe[j].requiredItem[k].type) || Main.recipe[j].AcceptedByItemGroups(Main.guideItem.type, Main.recipe[j].requiredItem[k].type) || Main.recipe[j].usePressurePlate(Main.guideItem.type, Main.recipe[j].requiredItem[k].type))
                        {
                            Main.availableRecipe[Main.numAvailableRecipes] = j;
                            Main.numAvailableRecipes++;
                            break;
                        }
                    }
                }
            }*/
            //else
            Dictionary<int, int> dictionary = new Dictionary<int, int>();
            Item item;
            Item[] inventory = Main.player[Main.myPlayer].inventory;
            for (int i = 0; i < 58; i++)
            {
                item = inventory[i];
                if (item.stack > 0)
                {
                    if (dictionary.ContainsKey(item.netID))
                        dictionary[item.netID] += item.stack;
                    else
                        dictionary[item.netID] = item.stack;
                }
            }

            if (Main.player[Main.myPlayer].chest != -1)
            {
                if (Main.player[Main.myPlayer].chest > -1)
                    inventory = Main.chest[Main.player[Main.myPlayer].chest].item;
                else if (Main.player[Main.myPlayer].chest == -2)
                    inventory = Main.player[Main.myPlayer].bank.item;
                else if (Main.player[Main.myPlayer].chest == -3)
                    inventory = Main.player[Main.myPlayer].bank2.item;
                else if (Main.player[Main.myPlayer].chest == -4)
                    inventory = Main.player[Main.myPlayer].bank3.item;
                else if (Main.player[Main.myPlayer].chest == -5)
                    inventory = Main.player[Main.myPlayer].bank4.item;

                for (int i = 0; i < 40; i++)
                {
                    item = inventory[i];
                    if (item != null && item.stack > 0)
                    {
                        if (dictionary.ContainsKey(item.netID))
                            dictionary[item.netID] += item.stack;
                        else
                            dictionary[item.netID] = item.stack;
                    }
                }
            }
            if (wePlayer.usingEnchantingTable)
            {
                if(wePlayer.enchantingTableUI?.essenceSlotUI != null)
                {
                    for (int i = 0; i < EnchantmentEssence.rarity.Length; i++)
                    {
                        item = wePlayer.enchantingTableUI.essenceSlotUI[i].Item;
                        if (item != null && item.stack > 0)
                        {
                            if (dictionary.ContainsKey(item.netID))
                                dictionary[item.netID] += item.stack;
                            else
                                dictionary[item.netID] = item.stack;
                        }
                    }
                }
            }

            for (int n = 0; n < Recipe.maxRecipes && Main.recipe[n].createItem.type != ItemID.None; n++)
            {
                bool ableToCraft = true;
                for (int i = 0; i < Main.recipe[n].requiredTile.Count && Main.recipe[n].requiredTile[i] != -1; i++)
                {
                    if (!wePlayer.Player.adjTile[Main.recipe[n].requiredTile[i]])
                    {
                        ableToCraft = false;
                        break;
                    }
                }

                if (ableToCraft)
                {
                    for (int i = 0; i < Main.recipe[n].requiredItem.Count; i++)
                    {
                        item = Main.recipe[n].requiredItem[i];

                        int stack = item.stack;
                        /*bool flag2 = false;
                        foreach (int key in dictionary.Keys)
                        {
                            if (Main.recipe[n].useWood(key, item.type) || Main.recipe[n].useSand(key, item.type) || Main.recipe[n].useIronBar(key, item.type) || Main.recipe[n].useFragment(key, item.type) || Main.recipe[n].AcceptedByItemGroups(key, item.type) || Main.recipe[n].usePressurePlate(key, item.type))
                            {
                                stack -= dictionary[key];
                                flag2 = true;
                            }
                        }*/
                        //if (!flag2 && dictionary.ContainsKey(item.netID))
                        if (dictionary.ContainsKey(item.netID))
                            stack -= dictionary[item.netID];

                        if (stack > 0)
                        {
                            ableToCraft = false;
                            break;
                        }
                    }
                }
                /*
                if (ableToCraft)
                {
                    bool num6 = !Main.recipe[n].needWater || Main.player[Main.myPlayer].adjWater;
                    bool flag3 = !Main.recipe[n].needHoney || Main.recipe[n].needHoney == Main.player[Main.myPlayer].adjHoney;
                    bool flag4 = !Main.recipe[n].needLava || Main.recipe[n].needLava == Main.player[Main.myPlayer].adjLava;
                    bool flag5 = !Main.recipe[n].needSnowBiome || Main.player[Main.myPlayer].ZoneSnow;
                    bool flag6 = !Main.recipe[n].needGraveyardBiome || Main.player[Main.myPlayer].ZoneGraveyard;
                    if (!(num6 && flag3 && flag4 && flag5 && flag6))
                        ableToCraft = false;
                }*/

                if (ableToCraft && RecipeLoader.RecipeAvailable(Main.recipe[n]))
                {
                    bool stillNeeded = true;
                    foreach(int r in Main.availableRecipe)
                    {
                        if(r == n)
                        {
                            stillNeeded = false;
                        }
                    }
                    if (stillNeeded)
                    {
                        Main.availableRecipe[Main.numAvailableRecipes] = n;
                        Main.numAvailableRecipes++;
                    }
                }
            }

            for (int i = 0; i < Main.numAvailableRecipes; i++)
            {
                if (availableRecipe == Main.availableRecipe[i])
                {
                    Main.focusRecipe = i;
                    //lastFocusRecipe = i;
                    lastFocusRecipe = Main.availableRecipe[i];
                    break;
                }
            }

            //if (Main.focusRecipe >= Main.numAvailableRecipes)
            if (lastFocusRecipeListNum >= Main.numAvailableRecipes)
                Main.focusRecipe = Main.numAvailableRecipes - 1;

            if (Main.focusRecipe < 0)
                Main.focusRecipe = 0;
            lastFocusRecipeListNum = Main.focusRecipe;

            float num8 = Main.availableRecipeY[Main.focusRecipe] - availableRecipeY;
            for (int num9 = 0; num9 < Recipe.maxRecipes; num9++)
            {
                Main.availableRecipeY[num9] -= num8;
            }
        }
        private void RefreshItemArray(Item[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if(!array[i].IsAir)
                    array[i].Refresh();//Find what this does
            }
        }//My UI
    }
}
