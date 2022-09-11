using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility.LogSystem.WebpageComponenets;
using WeaponEnchantments.Items;
using static WeaponEnchantments.Common.Utility.LogSystem.Wiki;
using WeaponEnchantments.Common.Utility;

namespace WeaponEnchantments.Common.Utility.LogSystem
{
    public class ItemInfo
    {
        public ModItem ModItem { private set; get; }
        public Item Item { private set; get; }
        public ItemInfo(ModItem modItem) {
            ModItem = modItem;
            Item = new(modItem.Type);
        }
        public ItemInfo(Item item) {
            Item = item;
        }
        public void AddStatistics(WebPage webpage) {
            string tooltip = "";
            for (int i = 0; i < Item.ToolTip.Lines; i++) {
                tooltip += Item.ToolTip.GetLine(i);
            }
            List<List<string>> info = new() {
                new() { "Type", $"{ModItem.GetType().Name}" },

                new() { "Tooltip", $"'{tooltip}'" },//italics?
                new() { "Rarity", $"{Item.rare}" },//Need to edit to rarity name and color
                new() { "Buy", $"{Item.value.GetCoinsPNG()}" },
                new() { "Sell", $"{(Item.value / 5).GetCoinsPNG()}" },
                new() { "Research", $"{CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Item.type]} required" }
            };

            string label = $"{Item.Name}<br/>{Item.ToItemPNG(displayName: false)}<br/>Statistics";
            webpage.AddTable(info, label);
        }
        public void AddDrops(WebPage webpage) {
            int type = Item.type;
            if (!drops.ContainsKey(type))
                return;

            List<List<string>> allDropInfo = new() { new() { "Entity", "Qty.", "Rate" } };
            foreach (int npcType in drops[type].Keys) {
                int minDropped = drops[type][npcType].stackMin;
                int maxDropped = drops[type][npcType].stackMax;
                float chance = drops[type][npcType].dropRate;
                List<string> dropInfo = new() { $"{npcType.ToNpcPNG()}", (minDropped != maxDropped ? $"{minDropped}-{maxDropped}" : $"{minDropped}"), chance.PercentString() };//Make vanilla link option to vanilla wiki
                allDropInfo.Add(dropInfo);
            }
            /*
            foreach(ChestID chestID in Enum.GetValues(typeof(ChestID)).Cast<ChestID>().ToList().Where(c => c != ChestID.None)) {
                WEModSystem.GetChestLoot(chestID, out List<WeightedPair> pairs, out float baseChance);
                if (pairs == null)
                    continue;

                string name = chestID.ToString() + " Chest";
                float total = 0f;
                foreach(WeightedPair pair in pairs) {
                    total += pair.Weight;
                        }

                foreach(WeightedPair pair in pairs) {
                    Item sampleItem = ContentSamples.ItemsByType[pair.ID];
                    if (sampleItem.ModItem is not Enchantment enchantment)
                    continue;

                    log += $"\n{name},";
                    float chance = baseChance * pair.Weight / total;

                    log += $"{enchantment.EnchantmentTypeName.AddSpaces()},{chance.PercentString()}";
                }
                    }

                foreach(KeyValuePair<int, List<WeightedPair>> crate in GlobalCrates.crateDrops) {
                string name = ((CrateID)crate.Key).ToString() + " Crate";
                float total = 0f;
                foreach(WeightedPair pair in crate.Value) {
                    total += pair.Weight;
                        }

                foreach(WeightedPair pair in crate.Value) {
                    Item sampleItem = ContentSamples.ItemsByType[pair.ID];
                    if (sampleItem.ModItem is not Enchantment enchantment)
                    continue;

                    log += $"\n{name} ({crate.Key}),";
                    float baseChance = GlobalCrates.GetCrateEnchantmentDropChance(crate.Key);
                    float chance = baseChance * pair.Weight / total;

                    log += $"{enchantment.EnchantmentTypeName.AddSpaces()},{chance.PercentString()}";
                }
                    }

                log.Log();
            */
            //foreach chest
            //foreach crate

            string header = $"Obtained From";
            webpage.AddTable(allDropInfo, header, true, true, true);
        }
        public void AddInfo(WebPage webpage) {
            if (ModItem is ISoldByWitch soldByWitch && soldByWitch.SellCondition != SellCondition.Never) {
                int sellPrice = (int)((float)Item.value * soldByWitch.SellPriceModifier);
                string sellPriceString = sellPrice.GetCoinsPNG();
                string sellText = $"Sold by the Witch for {sellPriceString}. Can only appear in the shop if this condition is met: {soldByWitch.SellCondition.ToString().AddSpaces()}";
                webpage.AddParagraph(sellText);
            }
        }
        public IEnumerable<IEnumerable<string>> RecipesCreateItem => GetRecipes(createItem: true);
        public IEnumerable<IEnumerable<string>> RecipesUsedIn => GetRecipes(usedIn: true);
        public IEnumerable<IEnumerable<string>> RecipesReverseRecipes => GetRecipes(reverseRecipes: true);
        public void AddRecipes(WebPage webpage) {
            webpage.AddTable(RecipesCreateItem, "Recipes", true);
            webpage.AddTable(RecipesUsedIn, "Used in", true);
            webpage.AddTable(RecipesReverseRecipes, "Used in (Reverse Crafting Recipes)", true);
        }
        private IEnumerable<IEnumerable<string>> GetRecipes(bool createItem = false, bool usedIn = false, bool reverseRecipes = false) {
            if (!createItem && !usedIn && !reverseRecipes) {
                createItem = true;
                usedIn = true;
                reverseRecipes = true;
            }

            bool recipesAdded = false;

            List<List<string>> myRecipes = new() { new() { "Result", "Ingredients", "Crafting station" } };
            if (createItem && TryAddRecipeData(myRecipes, createItemRecipes))
                recipesAdded = true;

            if (usedIn && TryAddRecipeData(myRecipes, recipesUsedIn))
                recipesAdded = true;

            if (reverseRecipes && TryAddRecipeData(myRecipes, recipesUsedIn, reverseRecipes))
                recipesAdded = true;

            if (!recipesAdded)
                $"No recipe found for {Item.S()}".Log();

            if (myRecipes.Count == 1)
                myRecipes = new();

            return myRecipes;
        }
        private bool TryAddRecipeData(List<List<string>> myRecipes, Dictionary<int, List<RecipeData>> allRecipeData, bool includeReverse = false) {
            if (!allRecipeData.ContainsKey(Item.type))
                return false;

            List<RecipeData> myRecipeData = new();

            if (allRecipeData[Item.type].Count >= 20) {
                foreach (RecipeData recipeData in allRecipeData[Item.type]) {
                    bool added = false;
                    foreach (RecipeData myRecipe in myRecipeData) {
                        if (myRecipe.TryCondenseRecipe(recipeData)) {
                            added = true;
                            break;
                        }
                    }

                    if (!added)
                        myRecipeData.Add(recipeData);
                }
            }
            else {
                myRecipeData = allRecipeData[Item.type];
            }

            List<RecipeData> myRecipeData2 = myRecipeData.Where(rd => rd.requiredTile.ToString().Contains("Enchanting Table")).OrderBy(rd => -rd.requiredTile.ToString().Length).ToList();
            List<RecipeData> myRecipeData3 = myRecipeData.Where(rd => !rd.requiredTile.ToString().Contains("Enchanting Table")).OrderBy(rd => rd.requiredTile.ToString().Length).ToList();

            foreach (RecipeData recipeData in myRecipeData3.Concat(myRecipeData2)) {
                int createItemCount = recipeData.createItem.Count;
                bool reverseRecipe = recipeData.IsReverseRecipe(Item.type);
                if (includeReverse && !reverseRecipe || !includeReverse && reverseRecipe)
                    continue;

                List<string> list = new();

                int createItemNum = recipeData.createItem.CommonList.Count;
                if (createItemNum > 0)
                    createItemNum++;

                int requiredItemNum = recipeData.requiredItem.CommonList.Count;
                if (requiredItemNum > 0)
                    requiredItemNum++;

                bool breaksNeeded = recipeData.createItem.UniqueList != null && recipeData.requiredItem.UniqueList != null && recipeData.createItem.UniqueList.Count > 0 && recipeData.requiredItem.UniqueList.Count > 0;
                int diff = Math.Abs(createItemNum - requiredItemNum);
                string breaks = "<br/>".FillString(diff);

                string createItems = recipeData.createItem.ToString();//Select(i => i.ToItemPNG()).JoinList();
                if (breaksNeeded && createItemNum < requiredItemNum)
                    createItems = breaks + createItems;

                list.Add(createItems);

                string requiredItems = recipeData.requiredItem.ToString();//.Select(i => i.ToItemPNG()).JoinList();
                if (breaksNeeded && requiredItemNum < createItemNum)
                    requiredItems = breaks + requiredItems;

                list.Add(requiredItems);

                if (recipeData.requiredTile.Count > 0) {
                    string requiredTiles = recipeData.requiredTile.ToString();
                    list.Add(requiredTiles);
                }
                else {
                    list.Add($"By Hand\n");
                }

                myRecipes.Add(list);
            }

            return true;
        }
    }
}
