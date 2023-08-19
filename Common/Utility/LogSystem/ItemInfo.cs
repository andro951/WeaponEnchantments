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
using WeaponEnchantments.Common.Globals;
using Terraria.GameContent.UI;
using Microsoft.Xna.Framework;
using Terraria.ID;
using static WeaponEnchantments.Common.Utility.LogModSystem;
using androLib.Items;
using androLib.Common.Utility;

namespace WeaponEnchantments.Common.Utility.LogSystem
{
    public class ItemInfo {
        public AndroModItem androModItem { private set; get; }
        public Item Item { private set; get; }
        public ItemInfo(AndroModItem weModItem) {
            androModItem = weModItem;
            Item = new(weModItem.Type);
        }
        public ItemInfo(Item item) {
            Item = item;
        }
        public int ShopPrice;
        public void AddStatistics(WebPage webpage, bool name = true, bool image = true, bool artists = true, bool types = true, bool tooltip = true, bool rarity = true, bool buy = true, bool sell = true, bool research = true) {
            List<List<string>> info = new();
            List<string> labels = new();

            if (name)
                labels.Add(Name);

            if (image)
                labels.Add(Image);

            if (artists) {
                GetArtists(out string artistString, out string artModifiedBy);

                if (artistString != null || artModifiedBy != null)
                    labels.Add($"{artistString}{artModifiedBy}");
            }

            labels.Add("Statistics");

            //Type
            if (types)
                info.Add(new() { "Type", GetItemTypes() });

            //Tooltip
            if (tooltip)
                info.Add(new() { "Tooltip", $"<i>'{Tooltip}'</i>" });

            //Rarity
            if (rarity) {
                string rareString = GetRarity(out Color color);
                info.Add(new() { "https://terraria.fandom.com/wiki/Rarity".ToExternalLink("Rarity"), $"<b><span style=\"color:rgb({color.R}, {color.G}, {color.B});\">{rareString}</span></b>" });
            }

            //Buy
            if (buy && TryGetShopPrice())
                info.Add(new() { "Buy", $"{ShopPrice.GetCoinsPNG()}{(androModItem is ISoldByWitch soldByWitch ? $" ({soldByWitch.SellCondition.ToString().AddSpaces()})" : "")}" });

            //Sell
            if (sell)
                info.Add(new() { "Sell", $"{(Item.value / 5).GetCoinsPNG()}" });

            //Research
            if (research)
                info.Add(new() { "https://terraria.fandom.com/wiki/Journey_Mode".ToExternalLink("Research"), Research });

            webpage.AddTable(info, headers: labels, maxWidth: 400, alignID: FloatID.right, collapsible: true);
        }
        public string Name => androModItem.Name.AddSpaces();
        public string Image => $"{Item.ToItemPNG(displayName: false)}";
        public void GetArtists(out string artistString, out string artModifiedBy) {
            artistString = androModItem.Artist;
            artModifiedBy = androModItem.ArtModifiedBy;
            if (artistString == "andro951")
                artistString = null;

            if (artistString != null) {
                if (contributorLinks.ContainsKey(artistString))
                    artistString = contributorLinks[artistString].ToExternalLink(artistString);

                //artistString = $"  (art by {artistString}{(artModifiedBy == null ? ")" : "")}";
            }

            if (artModifiedBy != null) {
                if (contributorLinks.ContainsKey(artModifiedBy))
                    artModifiedBy = contributorLinks[artModifiedBy].ToExternalLink(artModifiedBy);

                //artModifiedBy = $"{(artistString == null ? "  (" : " ")}modified by {artModifiedBy})";
                artModifiedBy = $"{(artistString == null ? "" : " ")}modified by {artModifiedBy}";
            }
        }
        public string GetItemTypes() {
            string typeText = "";
            bool first = true;
            foreach (WikiTypeID id in androModItem.WikiItemTypes) {
                if (first) {
                    first = false;
                }
                else {
                    typeText += ", ";
                }

                string linkText = id.GetLinkText(out bool external);
                typeText += external ? linkText.ToExternalLink(id.ToString().AddSpaces()) : linkText.ToLink(id.ToString().AddSpaces());
            }

            return typeText;
        }
        public string Tooltip => GetTooltip(androModItem);
        public string Rarity {
            get {
                string rareString = GetRarity(out Color color);
                return $"<b><span style=\"color:rgb({color.R}, {color.G}, {color.B});\">{rareString}</span></b>";
            }
        }
        public static string GetTooltip(ModItem modItem) {
            Item item = new Item(modItem.Type);
            string tooltip = "";
            List<string> tooltipStrings = new();
            for (int i = 0; i < item.ToolTip.Lines; i++) {
                tooltipStrings.Add(item.ToolTip.GetLine(i));
            }

            tooltip += tooltipStrings.JoinList();

            if (tooltip == "") {
                List<TooltipLine> tooltipLines = new();
                //Item item = new(Item.type);
                //if (item.ModItem is Enchantment enchantment) {
                if (modItem is Enchantment enchantment) {
                    //float temp = enchantment.EnchantmentStrength;
                    enchantment.ModifyTooltips(tooltipLines);
                    tooltip += tooltipLines.Select(t => t.Text).JoinList();
                }
            }

            return tooltip;
        }
        public string GetRarity(out Color color) {

            int rare = Item.rare;
            color = ItemRarity.GetColor(rare);
            string rareString;
            if (EnchantingRarity.TierColors.Contains(color)) {
                rareString = EnchantingRarity.GetTierNameFromColor(color);
            }
            else {
                if (!ItemRarityID.Search.TryGetName(Item.rare, out rareString))
                    rareString = "Failed to find rarity name";
            }

            return rareString;
        }
        public string Research => $"{CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Item.type]} required";
        public void AddDrops(WebPage webpage) {
            int type = Item.type;
            List<(string, List<List<string>>)> dropTables = new();

            if (enemyDrops.ContainsKey(type)) {
                List<List<string>> allDropInfo = new() { new() { "Entity", "Qty.", "Rate" } };
                foreach (int npcNetID in enemyDrops[type].Keys) {
                    int minDropped = enemyDrops[type][npcNetID].stackMin;
                    int maxDropped = enemyDrops[type][npcNetID].stackMax;
                    float chance = enemyDrops[type][npcNetID].dropRate;
                    bool configDrop = androModItem is WEModItem weModItem && weModItem.ConfigOnlyDrop && WEGlobalNPC.PreHardModeBossTypes.Contains(npcNetID);
                    string chanceString = $"{(configDrop ? "(" : "")}{chance.PercentString()}{(configDrop ? ")<br/>if config enabled" : "")}";
                    List<string> dropInfo = new() { $"{npcNetID.ToNpcPNG(link: true)}", (minDropped != maxDropped ? $"{minDropped}-{maxDropped}" : $"{minDropped}"), chanceString };//Make vanilla link option to vanilla wiki
                    allDropInfo.Add(dropInfo);
                }

                dropTables.Add(($"{"Bestiary_Boss_Enemy".ToPNG()} Obtained From", allDropInfo));
            }

            if (chestDrops.ContainsKey(type)) {
                List<List<string>> allChestDropInfo = new() { new() { "Entity", "Rate" } };
                foreach (KeyValuePair<ChestID, float> pair in chestDrops[type]) {
                    List<string> dropInfo = new() { $"{pair.Key.GetItemType().ToItemPNG(link: true)}", pair.Value.PercentString() };
                    allChestDropInfo.Add(dropInfo);
                }

                dropTables.Add(($"{ItemID.Chest.ToItemPNG(displayName: false)} Obtained From", allChestDropInfo));
            }

            if (crateDrops.ContainsKey(type)) {
                List<List<string>> allCrateDropInfo = new() { new() { "Entity", "Rate" } };
                foreach (KeyValuePair<CrateID, float> pair in crateDrops[type]) {
                    List<string> dropInfo = new() { $"{((int)pair.Key).ToItemPNG(link: true)}", pair.Value.PercentString() };
                    allCrateDropInfo.Add(dropInfo);
                }

                dropTables.Add(($"{ItemID.WoodenCrate.ToItemPNG(displayName: false)} Obtained From", allCrateDropInfo));
            }

            foreach ((string, List<List<string>>) dropTable in dropTables) {
                webpage.AddTable(dropTable.Item2, firstRowHeaders: true, collapsible: true, maxWidth: 400, alignID: FloatID.right, label: dropTable.Item1);
            } 
        }
        public static void AddAllDrops(WebPage webpage, Type type = null) {
            List<(string, List<List<string>>)> dropTables = new();
            List<List<string>> allDropInfo = new();
            foreach (int itemType in enemyDrops.Keys) {
                ModItem modItem = ContentSamples.ItemsByType[itemType].ModItem;
                if (type != null && !modItem.TypeBeforeModItem().Equals(type))
                    continue;

                string itemPNG = itemType.ToItemPNG(link: true);

                foreach (int npcNetID in enemyDrops[itemType].Keys) {
                    int minDropped = enemyDrops[itemType][npcNetID].stackMin;
                    int maxDropped = enemyDrops[itemType][npcNetID].stackMax;
                    float chance = enemyDrops[itemType][npcNetID].dropRate;
                    bool configDrop = modItem is WEModItem weModItem && weModItem.ConfigOnlyDrop && WEGlobalNPC.PreHardModeBossTypes.Contains(npcNetID);
                    string chanceString = $"{(configDrop ? "(" : "")}{chance.PercentString()}{(configDrop ? ")<br/>if config enabled" : "")}";
                    List<string> dropInfo = new() { itemPNG, $"{npcNetID.ToNpcPNG(link: true)}", (minDropped != maxDropped ? $"{minDropped}-{maxDropped}" : $"{minDropped}"), chanceString };//Make vanilla link option to vanilla wiki
                    allDropInfo.Add(dropInfo);
                }
            }

            if (allDropInfo.Count > 0)
                dropTables.Add(($"{"Bestiary_Boss_Enemy".ToPNG()} Obtained From", new List<List<string>>() { new() { "Item", "Entity", "Qty.", "Rate" } }.Concat(allDropInfo.OrderBy(l => l[0])).ToList()));

            List<List<string>> allChestDropInfo = new();

            foreach(int itemType in chestDrops.Keys) {
                ModItem modItem = ContentSamples.ItemsByType[itemType].ModItem;
                if (type != null && !modItem.TypeBeforeModItem().Equals(type))
                    continue;

                string itemPNG = itemType.ToItemPNG(link: true);

                foreach (KeyValuePair<ChestID, float> pair in chestDrops[itemType]) {
                    List<string> dropInfo = new() { itemPNG, $"{pair.Key.GetItemType().ToItemPNG(link: true)}", pair.Value.PercentString() };
                    allChestDropInfo.Add(dropInfo);
                }
            }

            if (allChestDropInfo.Count > 0)
                dropTables.Add(($"{ItemID.Chest.ToItemPNG(displayName: false)} Obtained From", new List<List<string>>() { new() { "Item", "Entity", "Rate" } }.Concat(allChestDropInfo.OrderBy(l => l[0])).ToList()));

            List<List<string>> allCrateDropInfo = new();
            foreach (int itemType in crateDrops.Keys) {
                ModItem modItem = ContentSamples.ItemsByType[itemType].ModItem;
                if (type != null && !modItem.TypeBeforeModItem().Equals(type))
                    continue;

                string itemPNG = itemType.ToItemPNG(link: true);

                foreach (KeyValuePair<CrateID, float> pair in crateDrops[itemType]) {
                    List<string> dropInfo = new() { itemPNG, $"{((int)pair.Key).ToItemPNG(link: true)}", pair.Value.PercentString() };
                    allCrateDropInfo.Add(dropInfo);
                }
            }

            if (allCrateDropInfo.Count > 0)
                dropTables.Add(($"{ItemID.WoodenCrate.ToItemPNG(displayName: false)} Obtained From", new List<List<string>>() { new() { "Item", "Entity", "Rate" } }.Concat(allCrateDropInfo.OrderBy(l => l[0])).ToList()));

            foreach ((string, List<List<string>>) dropTable in dropTables) {
                webpage.AddTable(dropTable.Item2, firstRowHeaders: true, collapsible: true, label: dropTable.Item1, sortable: true);
            }
        }
        public void AddInfo(WebPage webpage) {
            //webpage.AddParagraph($"Tooltip:<br/>{Tooltip}");
            if (TryGetShopPrice() && androModItem is ISoldByWitch soldByWitch) {
                webpage.NewLine();
                string sellPriceString = ShopPrice.GetCoinsPNG();
                //string sellText = $"Can appear in the Witch's shop for {sellPriceString}. ({soldByWitch.SellCondition.ToString().AddSpaces()})";
                string sellText = $"The {Item.Name} {soldByWitch.SellCondition.ToWitchSellText(sellPriceString)}";
                webpage.AddParagraph(sellText);
            }
        }
        public bool TryGetShopPrice() {
            if (ShopPrice != 0)
                return true;

            if (androModItem is ISoldByWitch soldByWitch && soldByWitch.SellCondition != SellCondition.Never) {
                ShopPrice = (int)((float)Item.value * soldByWitch.SellPriceModifier);
                return true;
            }

            return false;
        }
        public static bool TryGetShopPrice(ModItem modItem, out int price) {
            if (modItem is ISoldByWitch soldByWitch && soldByWitch.SellCondition != SellCondition.Never) {
                price = (int)((float)modItem.Item.value * soldByWitch.SellPriceModifier);
                return true;
            }

            price = 0;
            return false;
		}
        public IEnumerable<IEnumerable<string>> RecipesCreateItem => GetRecipes(createItem: true);
        public IEnumerable<IEnumerable<string>> RecipesUsedIn => GetRecipes(usedIn: true);
        public IEnumerable<IEnumerable<string>> RecipesReverseRecipes => GetRecipes(createItem: true, usedIn: true, reverseRecipes: true);
        public void AddRecipes(WebPage webpage) {
            webpage.AddTable(RecipesCreateItemTable);
            webpage.AddTable(RecipesUsedInTable);
            webpage.AddTable(RecipesReverseRecipesTable);
        }
        public Table<string> RecipesCreateItemTable => new Table<string>(RecipesCreateItem, label: "Recipes", firstRowHeaders: true, rowspanColumns: true, collapsible: true);
        public Table<string> RecipesUsedInTable => new Table<string>(RecipesUsedIn, label: "Used in", firstRowHeaders: true, rowspanColumns: true, collapsible: true);
        public Table<string> RecipesReverseRecipesTable => new Table<string>(RecipesReverseRecipes, label: "Used in (Reverse Crafting Recipes)", firstRowHeaders: true, rowspanColumns: true, collapsible: true);
        private IEnumerable<IEnumerable<string>> GetRecipes(bool createItem = false, bool usedIn = false, bool reverseRecipes = false) {
            bool recipesAdded = false;

            List<List<string>> myRecipes = new() { new() { "Result", "Ingredients", "Crafting station" } };
            if (createItem && TryAddRecipeData(myRecipes, createItemRecipes, reverseRecipes))
                recipesAdded = true;

            if (usedIn && TryAddRecipeData(myRecipes, recipesUsedIn, reverseRecipes, true))
                recipesAdded = true;

            if (!recipesAdded)
                $"No recipe found for {Item.S()}".Log_WE();

            if (myRecipes.Count == 1)
                myRecipes = new();

            return myRecipes;
        }
        private bool TryAddRecipeData(List<List<string>> myRecipes, Dictionary<int, List<RecipeData>> allRecipeData, bool includeReverse = false, bool usedIn = false) {
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

            List<RecipeData> myRecipeData2 = myRecipeData.Where(rd => rd.requiredTile.ToString().Contains("Enchanting Table"))
                .OrderBy(rd => (rd.requiredTile.ToString().Contains("<br/>or<br/>") ? -rd.requiredTile.ToString().Length : 10 * EnchantingTableItem.GetTableTier(rd.requiredTile.ToString())) - (usedIn ? rd.createItem.All.FirstEnchantmentStrength() : rd.requiredItem.All.FirstEnchantmentStrength()))
                .ToList();
            
            List<RecipeData> myRecipeData3 = myRecipeData
                .Where(rd => !rd.requiredTile.ToString().Contains("Enchanting Table"))
                .OrderBy(rd => rd.requiredItem.ToString().Length)
                .ToList();
            

            foreach (RecipeData recipeData in myRecipeData3.Concat(myRecipeData2)) {
                int createItemCount = recipeData.createItem.Count;
                bool requiredContainsItem = recipeData.requiredItem.Contains(Item);
                bool createItemCountLarger = recipeData.requiredItem.Count > 1 && createItemCount > recipeData.requiredItem.Count && requiredContainsItem;
                Enchantment firstRequiredEnchantment = recipeData.requiredItem.All.Select(i => i.ModItem).OfType<Enchantment>().FirstOrDefault(defaultValue: null);
                IEnumerable<ModItem> createModItems = recipeData.createItem.All.Select(i => i.ModItem);
                bool downGradeEnchanmtent = firstRequiredEnchantment?.EnchantmentTier > createModItems.OfType<Enchantment>().FirstOrDefault(defaultValue: null)?.EnchantmentTier || firstRequiredEnchantment != null && createModItems.OfType<EnchantmentEssence>().FirstOrDefault(defaultValue: null)?.EssenceTier == 0;
                bool reverseRecipe = recipeData.IsReverseRecipe(Item.type) || createItemCountLarger || downGradeEnchanmtent;
                if (includeReverse && !reverseRecipe || !includeReverse && reverseRecipe)
                    continue;

                ConvertRecipeDataListToStringList(myRecipes, recipeData);
            }

            return true;
        }
        public static void ConvertRecipeDataListToStringList(List<List<string>> recipes, RecipeData recipeData) {
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

            string createItems = recipeData.createItem.ToString();
            if (breaksNeeded && createItemNum < requiredItemNum)
                createItems = breaks + createItems;

            list.Add(createItems);

            string requiredItems = recipeData.requiredItem.ToString();
            if (breaksNeeded && requiredItemNum < createItemNum)
                requiredItems = breaks + requiredItems;

            list.Add(requiredItems);

            if (recipeData.requiredTile.Count > 0) {
                string requiredTiles = recipeData.requiredTile.ToString();
                list.Add(requiredTiles);
            }
            else {
                list.Add($"By Hand");
            }

            recipes.Add(list);
        }
    }
    public static class ItemInfoStaticMethods
	{
        public static int FirstEnchantmentStrength(this IEnumerable<Item> items) {
            Enchantment first = items.Select(i => i.ModItem).OfType<Enchantment>().FirstOrDefault(defaultValue: null);
            //$"Enchantment Name: {first?.Name}, tier: {first?.EnchantmentTier ?? -1}".LogSimple();
            return first?.EnchantmentTier ?? -1;
        }
	}
}
