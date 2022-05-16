using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using WeaponEnchantments.UI;
using WeaponEnchantments.Items;
using WeaponEnchantments.Common.Globals;
using static WeaponEnchantments.Items.Enchantments;

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
        public int lastFocusRecipeNotInTable = Main.availableRecipe[Main.focusRecipe];
        public float itemScale = 0f;
        public float manaCost = 0f;
        public float ammoCost = 0f;
        public float lifeSteal = 0f;
        public float lifeStealRollover = 0f;
        public bool allForOneCooldown = false;
        public int allForOneTimer = 0;
        public Item[] equiptArmor;
        public bool spelunker = false;

        public override void Initialize()
        {
            equiptArmor = new Item[Player.armor.Length];
            for(int i = 0; i < equiptArmor.Length; i++)
            {
                equiptArmor[i] = new Item();
            }
        }
        public override void SaveData(TagCompound tag)
        {
            for (int i = 0; i < EnchantingTable.maxItems; i++)
            {
                //if (!enchantingTable.item[i].IsAir)
                {
                    tag[Player.name + "enchantingTableItem" + i.ToString()] = enchantingTable.item[i];
                }
            }
            /*if (enchantingTableUI?.itemSlotUI[0]?.Item != null)
            {
                //if (enchantingTableUI.itemSlotUI[0].Item.IsAir)
                {
                    for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
                    {
                        if (!enchantingTable.enchantmentItem[i].IsAir)
                        {
                            tag[Player.name + "enchantingTableEnchantmentItem" + i.ToString()] = enchantingTable.enchantmentItem[i];
                        }
                    }
                }//enchantments in the enchantmentSlots are saved by global items.  This is just in case enchantment items are left in after Offering items.
            }*///Not used
            for (int i = 0; i < EnchantingTable.maxEssenceItems; i++)
            {
                tag[Player.name + "enchantingTableEssenceItem" + i.ToString()] = enchantingTable.essenceItem[i];
            }
        }
        public override void LoadData(TagCompound tag)
        {
            for (int i = 0; i < EnchantingTable.maxItems; i++)
            {
                if (tag.Get<Item>(Player.name + "enchantingTableItem" + i.ToString()).IsAir)
                {
                    enchantingTable.item[i] = new Item();
                }
                else
                {
                    enchantingTable.item[i] = tag.Get<Item>(Player.name + "enchantingTableItem" + i.ToString());
                }
            }
            if (enchantingTable.item[0].IsAir)//enchantments in the enchantmentSlots are loaded by global items.  This is just in case enchantment items are left in after Offering items.
            {
                for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
                {
                    if (tag.Get<Item>(Player.name + "enchantingTableEnchantmentItem" + i.ToString()).IsAir)
                    {
                        enchantingTable.enchantmentItem[i] = new Item();
                    }
                    else
                    {
                        enchantingTable.enchantmentItem[i] = tag.Get<Item>(Player.name + "enchantingTableEnchantmentItem" + i.ToString());
                    }
                }
            }
            for (int i = 0; i < EnchantingTable.maxEssenceItems; i++)
            {
                if (tag.Get<Item>(Player.name + "enchantingTableEssenceItem" + i.ToString()).IsAir)
                {
                    enchantingTable.essenceItem[i] = new Item();
                }
                else
                {
                    enchantingTable.essenceItem[i] = tag.Get<Item>(Player.name + "enchantingTableEssenceItem" + i.ToString());
                }
            }
        }
        public override bool ShiftClickSlot(Item[] inventory, int context, int slot)
        {
            if (usingEnchantingTable)
            {
                bool stop = false;
                for (int j = 0; j < EnchantingTable.maxItems; j++)
                {
                    if (enchantingTableUI.itemSlotUI[j].contains)
                    {
                        stop = true;
                    }
                }
                for (int j = 0; j < EnchantingTable.maxEnchantments && !stop; j++)
                {
                    if (enchantingTableUI.enchantmentSlotUI[j].contains)
                    {
                        stop = true;
                    }
                }
                for (int j = 0; j < EnchantingTable.maxEssenceItems && !stop; j++)
                {
                    if (enchantingTableUI.essenceSlotUI[j].contains)
                    {
                        stop = true;
                    }
                }
                if (stop)
                {
                    bool itemWillBeTrashed = true;
                    for(int i = 49; i >= 0 && itemWillBeTrashed; i--)
                    {
                        if (Player.inventory[i].IsAir)
                        {
                            itemWillBeTrashed = false;
                        }
                    }
                    if (itemWillBeTrashed)
                    {
                        return true;
                    }
                }//TODO: Edit this if you ever make ammo bags enchantable
                if (!stop)
                {
                    if (Main.mouseItem.IsAir)
                    {
                        bool valid = false;
                        if (inventory[slot].type == PowerBooster.ID && !enchantingTableUI.itemSlotUI[0].Item.IsAir && !enchantingTableUI.itemSlotUI[0].Item.GetGlobalItem<EnchantedItem>().powerBoosterInstalled)
                        {
                            enchantingTableUI.itemSlotUI[0].Item.GetGlobalItem<EnchantedItem>().powerBoosterInstalled = true;
                            if (inventory[slot].stack > 1)
                            {
                                inventory[slot].stack--;
                            }
                            else
                            {
                                inventory[slot] = new Item();
                            }
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
                                        break;
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
                                            int s = i;
                                            if (((Enchantments)inventory[slot].ModItem).utility && enchantingTableUI.enchantmentSlotUI[4].Item.IsAir)
                                            {
                                                s = 4;
                                            }
                                            enchantingTableUI.enchantmentSlotUI[s].Item = inventory[slot].Clone();
                                            enchantingTableUI.enchantmentSlotUI[s].Item.stack = 1;
                                            if (inventory[slot].stack > 1)
                                            {
                                                inventory[slot].stack--;
                                            }
                                            else
                                            {
                                                inventory[slot] = new Item();
                                            }
                                            SoundEngine.PlaySound(SoundID.Grab);
                                            valid = true;
                                            break;
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
                                                break;
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
            lastFocusRecipe = Main.availableRecipe[Main.focusRecipe];
            /*
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            int availableRecipe;
            float availableRecipeY;
            if (lastFocusRecipe > -1 && lastFocusRecipeListNum > Main.focusRecipe && Main.focusRecipe == Main.numAvailableRecipes - 1)
            {
                availableRecipe = lastFocusRecipe;
            }
            else
            {
                availableRecipe = Main.availableRecipe[Main.focusRecipe];
            }
            availableRecipeY = Main.availableRecipeY[Main.focusRecipe];
            for (int i = 0; i < Recipe.maxRecipes; i++)
            {
                Main.availableRecipe[i] = 0;
            }
            Main.numAvailableRecipes = 0;
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
                        if (dictionary.ContainsKey(item.netID))
                            stack -= dictionary[item.netID];

                        if (stack > 0)
                        {
                            ableToCraft = false;
                            break;
                        }
                    }
                }
                if (ableToCraft && RecipeLoader.RecipeAvailable(Main.recipe[n]))
                {
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
                    lastFocusRecipe = Main.availableRecipe[i];
                    break;
                }
            }
            if(availableRecipe != Main.availableRecipe[Main.focusRecipe])
            {
                if (lastFocusRecipeListNum < Main.numAvailableRecipes)
                {
                    Main.focusRecipe = lastFocusRecipeListNum;
                }
                else
                {
                    Main.focusRecipe = Main.numAvailableRecipes - 1;
                }
            }

            if (Main.focusRecipe < 0)
                Main.focusRecipe = 0;
            lastFocusRecipeListNum = Main.focusRecipe;

            float num8 = Main.availableRecipeY[Main.focusRecipe] - availableRecipeY;
            for (int num9 = 0; num9 < Recipe.maxRecipes; num9++)
            {
                Main.availableRecipeY[num9] -= num8;
            }
            */
        }
        public override void PostUpdate()
        {
            bool check = false;
            bool skipSpelunkerCheck = false;
            spelunker = false;
            int i = 0;
            if(Player.HeldItem.type != ItemID.None)
            {
                if (Player.HeldItem.GetGlobalItem<EnchantedItem>().spelunker)
                {
                    spelunker = true;
                    skipSpelunkerCheck = true;
                }
            }
            if (!skipSpelunkerCheck && spelunker)
            {
                check = true;
            }
            if (!check)
            {
                foreach (Item armor in Player.armor)
                {
                    if (!armor.vanity && !armor.IsAir)
                    {
                        if (equiptArmor[i].IsAir)
                        {
                            check = true;
                            break;
                        }
                        else if (!armor.GetGlobalItem<EnchantedItem>().equip || equiptArmor[i] != armor)
                        {
                            check = true;
                            break;
                        }
                    }
                    else if (armor.IsAir && !equiptArmor[i].IsAir)
                    {
                        check = true;
                        break;
                    }
                    i++;
                }
            }
            if (check)
            {
                itemScale = 0f;
                manaCost = 0f;
                ammoCost = 0f;
                lifeSteal = 0f;
                float itemScaleBonus = 0f;
                float manaCostBonus = 0f;
                float ammoCostBonus = 0f;
                float lifeStealBonus = 0f;
                foreach (Item armor in Player.armor)
                {
                    if (!armor.vanity && !armor.IsAir)
                    {
                        for (i = 0; i < EnchantingTable.maxEnchantments; i++)
                        {
                            if (!armor.GetGlobalItem<EnchantedItem>().enchantments[i].IsAir)
                            {
                                float str = ((Enchantments)armor.GetGlobalItem<EnchantedItem>().enchantments[i].ModItem).enchantmentStrength;
                                switch (((Enchantments)armor.GetGlobalItem<EnchantedItem>().enchantments[i].ModItem).enchantmentType)
                                {
                                    case EnchantmentTypeIDs.Size:
                                        itemScaleBonus += str;
                                        break;
                                    case EnchantmentTypeIDs.ManaCost:
                                        manaCostBonus += str;
                                        break;
                                    case EnchantmentTypeIDs.AmmoCost:
                                        ammoCostBonus += str;
                                        break;
                                    case EnchantmentTypeIDs.LifeSteal:
                                        lifeStealBonus += str;
                                        break;
                                    case EnchantmentTypeIDs.Spelunker:
                                        spelunker = true;
                                        break;
                                }
                            }
                        }
                        if (!equiptArmor[i].IsAir) { equiptArmor[i].GetGlobalItem<EnchantedItem>().equip = false; }
                        armor.GetGlobalItem<EnchantedItem>().equip = true;
                        equiptArmor[i] = armor;
                    }
                }
                itemScale += itemScaleBonus / 4;
                manaCost += manaCostBonus / 4;
                ammoCost += ammoCostBonus / 4;
                lifeSteal += lifeStealBonus / 4;
            }
            if (spelunker) { Player.AddBuff(9, 1); }
            if (allForOneTimer > 0)
            {
                allForOneTimer--;
                if (allForOneTimer == 0)
                {
                    SoundEngine.PlaySound(SoundID.MaxMana);
                }
            }
            lastFocusRecipeNotInTable = Main.availableRecipe[Main.focusRecipe];
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
