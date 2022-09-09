using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Items;

namespace WeaponEnchantments.Common
{
	public static class Wiki
	{
        private static Dictionary<int, List<RecipeData>> createItemRecipes;
        private static Dictionary<int, List<RecipeData>> recipesUsedIn;
        private static Dictionary<int, Dictionary<int, DropRateInfo>> drops;
        private static int min;
        private static int max;
        public static void PrintWiki() {
            if (!LogModSystem.printWiki)
                return;

            IEnumerable<ModItem> modItems = ModContent.GetInstance<WEMod>().GetContent<ModItem>();
            GetMinMax(modItems);
            GetRecpies(modItems);
            GetDrops();
            IEnumerable<ContainmentItem> containmentItems = modItems.OfType<ContainmentItem>().OrderBy(c => c.tier);
            IEnumerable<EnchantmentEssence> enchantmentEssence = modItems.OfType<EnchantmentEssence>().OrderBy(e => e.Name);
            IEnumerable<Enchantment> enchantments = modItems.OfType<Enchantment>().OrderBy(e => e.Name);
            List<WebPage> webPages = new();

            AddContainments(webPages, containmentItems, enchantments);


            string wiki = "\n\n";

            foreach (WebPage webPage in webPages) {
                wiki += webPage.ToString();
            }

            wiki.Log();
        }
        private static void AddContainments(List<WebPage> webPages, IEnumerable<ContainmentItem> containmentItems, IEnumerable<Enchantment> enchantments) {
            WebPage Containments = new("Containments");
            Containments.AddParagraph("These contain the power of the enchantments. More powerful enchantments require larger and stronger containments to hold them." +
            "Containments are crafting materials used to craft enchantments.");
            Containments.NewLine();
            foreach (ContainmentItem containment in containmentItems) {
                string name = containment.Name;
                int tier = containment.tier;
                string subHeading = $"{containment.Item.ToItemPNG()} (Tier {tier})";
                Containments.AddParagraph($"{containment.Item.ToItemPNG(link: true)} (Tier {tier})");
                WebPage containmentPage = new(containment.Item.Name);
                ItemInfo itemInfo = new(containment);
                itemInfo.AddStatistics(containmentPage);
                itemInfo.AddDrops(containmentPage);
                itemInfo.AddInfo(containmentPage);

                switch (tier) {
                    case 0://Containment
                        string text = "Only these enchantments can be obtained by crafting.The others must all be found in other ways.";
                        containmentPage.AddParagraph(text);
                        containmentPage.AddBulletedList(true, true, enchantments.Where(e => e.LowestCraftableTier == 0 && e.EnchantmentTier == 0).Select(c => c.Name).ToArray());
                        break;
                    case 1://Medium Containment

                        break;
                    case 2://Superior Containment

                        break;
                }

                itemInfo.AddRecipes(containmentPage);
                webPages.Add(containmentPage);
            }

            webPages.Add(Containments);
        }
        private static void AddEnchantments(List<WebPage> webPages, IEnumerable<Enchantment> enchantments) {
            WebPage Enchantments = new("Enchantments");
            Enchantments.AddParagraph("");//Not finished
            Enchantments.NewLine();
            foreach (IEnumerable<Enchantment> list in enchantments.GroupBy(e => e.EnchantmentTypeName)) {
                bool first = true;
                foreach (Enchantment e in list) {
                    if (first) {
                        first = false;
                        WebPage enchantmentTypePage = new(e.EnchantmentTypeName);
                        enchantmentTypePage.AddParagraph(e.Item.ToItemPNG(link: true));
                        Enchantments.AddParagraph(e.EnchantmentTypeName.ToItemPNG(link: true));
                    }

                    WebPage enchantmentPage = new(e.Item.Name);

                    webPages.Add(enchantmentPage);
                }


            }

        }
        private static void AddEssence(List<WebPage> webPages, IEnumerable<EnchantmentEssence> enchantmentEssence) {
            WebPage Essence = new("Enchantment Essence");
            Essence.AddParagraph("");//Not finished
            Essence.NewLine();


        }
        private static void GetMinMax(IEnumerable<ModItem> modItems) {
            min = int.MaxValue;
            max = int.MinValue;
            foreach (ModItem modItem in modItems) {
                int type = modItem.Type;
                if (type < min) {
                    min = type;
                }
                else if (type > max) {
                    max = type;
                }
            }
        }
        private static void GetRecpies(IEnumerable<ModItem> modItems) {
            createItemRecipes = new();
            recipesUsedIn = new();

            for (int i = 0; i < Recipe.numRecipes; i++) {
                Recipe recipe = Main.recipe[i];
                int type = recipe.createItem.type;
                if (type >= min && type <= max) {
                    RecipeData recipeData = new(recipe);
                    if (createItemRecipes.ContainsKey(type)) {
                        bool recipeAdded = false;

                        foreach (RecipeData createItemRecipe in createItemRecipes[type]) {
                            if (createItemRecipe.TryAdd(recipeData)) {
                                recipeAdded = true;
                                break;
                            }
                        }

                        if (!recipeAdded)
                            createItemRecipes[type].Add(recipeData);
                    }
                    else {
                        createItemRecipes.Add(type, new List<RecipeData> { recipeData });
                    }
                }

                foreach (Item item in recipe.requiredItem) {
                    int requiredItemType = item.type;
                    if (requiredItemType >= min && requiredItemType <= max) {
                        RecipeData recipeData = new(recipe);
                        if (recipesUsedIn.ContainsKey(requiredItemType)) {
                            bool recipeAdded = false;
                            foreach (RecipeData usedInRecipe in recipesUsedIn[requiredItemType]) {
                                if (usedInRecipe.TryAdd(recipeData)) {
                                    recipeAdded = true;
                                    break;
                                }
                            }

                            if (!recipeAdded)
                                recipesUsedIn[requiredItemType].Add(recipeData);
                        }
                        else {
                            recipesUsedIn.Add(requiredItemType, new List<RecipeData> { recipeData });
                        }
                    }
                }
            }
        }
        private static void GetDrops() {
            drops = new();
            //Dictionary<int, Dictionary<int, ItemDropRule>> drops
            for (int npcNetID = NPCID.NegativeIDCount + 1; npcNetID < NPCLoader.NPCCount; npcNetID++) {
                foreach (IItemDropRule dropRule in Main.ItemDropsDB.GetRulesForNPCID(npcNetID)) {
                    List<DropRateInfo> dropRates = new();
                    DropRateInfoChainFeed dropRateInfoChainFeed = new();
                    dropRule.ReportDroprates(dropRates, dropRateInfoChainFeed);
                    foreach (DropRateInfo dropRate in dropRates) {
                        int itemType = dropRate.itemId;
                        if (itemType >= min && itemType <= max) {
                            if (drops.ContainsKey(itemType)) {
                                if (drops[itemType].ContainsKey(npcNetID)) {
                                    $"itemType: {new Item(itemType).S()}, npcType{ContentSamples.NpcsByNetId[npcNetID]}".LogSimple();
                                    //drops[itemType][npcNetID]
                                }
                                else {
                                    drops[itemType].Add(npcNetID, dropRate);
                                }
                            }
                            else {
                                drops.Add(itemType, new() { { npcNetID, dropRate } });
                            }
                        }
                    }
                }
                /*
                foreach (IItemDropRule dropRule in Main.ItemDropsDB. ItemDropDatabase ContentSamples.NpcsByNetId[0] DropLoader.GetDropRules(npcType)) {
					int type = dropRule.dropItem.type;
					if (type >= min && type <= max) {
						if (drops.ContainsKey(type)) {
							drops[type].Add(npcType, dropRule);
						}
						else {
							drops.Add(type, new() { npcType, dropRule });
						}
					}
				}
                */
            }
        }
        private static List<Item> GetAllOtherCraftedItems(Item item, List<Item> consumedItems) => consumedItems.SelectMany(c => GetOtherCraftedItems(item, c)).ToList();
        private static List<Item> GetOtherCraftedItems(Item item, Item consumedItem) => CraftingEnchantments.GetOtherCraftedItems(item, consumedItem).Select(p => new Item(p.Key, p.Value)).ToList();

        public static string FillString(int num, char c) => num > 0 ? new string(c, num) : "";
        public static string FillString(int num, string s) {
            string text = "";
            for (int i = 0; i < num; i++) {
                text += s;
            }

            return text;
        }
        private class RecipeData
        {
            public CommonItemList createItem;
            public CommonItemList requiredItem;
            public CommonItemList requiredTile;
            public RecipeData(Recipe recipe) {
                List<Item> tempCreateItem = new() { recipe.createItem.Clone() };
                List<Item> tempRequiredItem = recipe.requiredItem.Select(i => i.Clone()).ToList();
                tempCreateItem.CombineLists(GetAllOtherCraftedItems(tempCreateItem[0], tempRequiredItem));
                createItem = new(tempCreateItem);
                requiredItem = new(tempRequiredItem);
                requiredTile = new(recipe.requiredTile.Select(i => i.GetItemFromTileType()).ToList());
            }
            public bool TryAdd(RecipeData other) {
                if (!createItem.ExactSame(other.createItem))
                    return false;

                if (SameExceptOneTile(other)) {
                    requiredTile.Add(other.requiredTile.CommonList);
                    return true;
                }

                if (SameExceptOneRequiredItem(other)) {
                    requiredItem.Add(other.requiredItem.CommonList);
                    return true;
                }

                return false;
            }
            public bool SameExceptOneTile(RecipeData other) {
                if (!requiredItem.ExactSame(other.requiredItem))
                    return false;

                if (!requiredTile.SameCommonItems(other.requiredTile))
                    return false;

                return true;
            }
            public bool SameExeptOneTileCompareTypesIgnoreStack(RecipeData other) {
                if (!requiredItem.ExactSame(other.requiredItem))
                    return false;

                if (!requiredTile.SameCommonItemsCompareTypesIgnoreStack(other.requiredTile))
                    return false;

                return true;
			}
            public bool SameExceptOneRequiredItem(RecipeData other) {
                if (!requiredTile.ExactSame(other.requiredTile))
                    return false;

                if (!requiredItem.SameCommonItems(other.requiredItem))
                    return false;

                return true;
            }
            public bool SameExceptOneRequiredItemCompareTypesIgnoreStack(RecipeData other) {
                if (!requiredTile.ExactSame(other.requiredTile))
                    return false;

                if (!requiredItem.SameCommonItemsCompareTypesIgnoreStack(other.requiredItem))
                    return false;

                return true;
			}
            public bool TryCondenseRecipe(RecipeData other) {
                //if (createItem.NumberCommonItems(other.createItem) == 0)
                //    return false;

                if (!createItem.SameCommonItemsCompareTypesIgnoreStack(other.createItem))
                    return false;

                bool added = false;
                if (SameExeptOneTileCompareTypesIgnoreStack(other)) {
                    if (requiredTile.TryAdd(other.requiredTile))
                        added = true;
                }

                if (SameExceptOneRequiredItemCompareTypesIgnoreStack(other)) {
                    if (requiredItem.TryAdd(other.requiredItem))
                        added = true;
                }

                if (added)
                    createItem.TryAdd(other.createItem);

                return added;
            }

            public override string ToString() {
                string s = "";
                if (createItem != null) {
                    s += createItem.ToString();
                    s += "     ";
                }

                if (requiredItem != null) {
                    s += requiredItem.ToString();
                    s += "     ";
                }

                if (requiredTile != null)
                    s += requiredTile.ToString();

                return s;
            }
        }

        private class WebPage
        {
            public string HeaderName { private set; get; }
            private List<object> _elements = new();
            public WebPage(string headerName) {
                HeaderName = headerName;
                AddLink("Weapon Enchantments Mod(tModLoader) Wiki", "Main Page");
                NewLine();
            }
            public void Add(object obj) => _elements.Add(obj);
            public void AddSubHeading(int num, string text) => Add(new SubHeading(num, text));
            public void AddParagraph(string text) => Add(new Paragraph(text));
            public void AddLink(string s, string text = null, bool png = false) => Add(new Link(s, text, png));
            public void AddBulletedList(bool png = false, bool links = false, params object[] elements) => Add(new BulletedList(png, links, elements));
            public void AddTable<T>(IEnumerable<IEnumerable<T>> elements, string header = null, bool firstRowHeaders = false, bool sortable = false, bool collapsible = false, bool collapsed = false) where T : class =>
                Add(new Table<T>(elements, header, firstRowHeaders, sortable, collapsible, collapsed));
            public void NewLine(int num = 1) => _elements.Add(FillString(num - 1, '\n'));
            public override string ToString() {
                string text = HeaderName + "\n";
                foreach (object element in _elements) {
                    text += element.ToString() + "\n";
                }

                return text;
            }
        }
        private class SubHeading
        {
            public int HeadingNumber { private set; get; }
            public string Text { private set; get; }
            public SubHeading(int num, string text) {
                HeadingNumber = num + 3;
                Text = text;
            }
            public override string ToString() {
                string markup = FillString(HeadingNumber, '=');
                return $"{markup} {Text} {markup}";
            }
        }
        private class Paragraph
        {
            public string Text { private set; get; }
            public Paragraph(string text) {
                Text = text;
            }
            public override string ToString() {
                return Text + "<br/>";
            }

        }
        private class Link
        {
            public string Text { private set; get; }
            public Link(string s, string text = null, bool png = false) {
                if (png) {
                    Text = s.ToItemPNG(link: true);
                }
                else {
                    Text = s.ToLink(text);
                }
            }
            public override string ToString() {
                return Text + "<br/>";
            }
        }
        private class BulletedList
        {
            public object[] Elements { private set; get; }
            private bool png;
            private bool _links;
            public BulletedList(bool png = false, bool links = false, params object[] elements) {
                Elements = elements;
                _links = links;
            }
            public override string ToString() {
                string text = "";
                foreach (object element in Elements) {
                    text += "* ";
                    string elem = element.ToString();
                    if (_links) {
                        if (png) {
                            elem = elem.ToItemPNG(link: true);
                        }
                        else {
                            elem = elem.ToLink();
                        }
                    }
                    else if (png) {
                        elem = elem.ToItemPNG();
                    }

                    text += elem;

                    text += "\n";
                }

                return text;
            }
        }
        private class NumberedList
        {
            public object[] Elements { private set; get; }
            private bool _links;
            public NumberedList(bool links = false, params object[] elements) {
                Elements = elements;
                _links = links;
            }
            public override string ToString() {
                string text = "";
                foreach (object element in Elements) {
                    text += "# ";
                    string elem = element.ToString();
                    if (_links)
                        elem = elem.ToLink();

                    text += elem;

                    text += "\n";
                }

                return text;
            }
        }
        private class Table<T> where T : class
        {
            public string Header { private set; get; }
            private bool _firstRowIsHeaders;
            private bool _sortable;
            private bool _collapsible;
            private bool _collapsed;
            private bool _rowspanColumns;
            public List<List<T>> Elements { private set; get; }
            public Table(IEnumerable<IEnumerable<T>> elements, string header = null, bool firstRowIsHeaders = false, bool sortable = false, bool collapsible = false, bool collapsed = false, bool rowspanColumns = true) {
                Elements = elements.Select(e => e.ToList()).ToList();
                Header = header;
                _firstRowIsHeaders = firstRowIsHeaders;
                _sortable = sortable;
                _collapsible = collapsible;
                _collapsed = collapsed;
                _rowspanColumns = rowspanColumns;
            }
            public override string ToString() {
                if (Elements.Count >= 10) {
                    _sortable = true;
                    _collapsible = true;
                    _collapsed = true;
                }

                string text = $"{"{"}| class=\"{(_sortable ? "sortable " : "")}{(_collapsible ? "mw-collapsible " : "")}{(_collapsed ? "mw-collapsed " : "")}fandom-table\"\n";
                List<int> rowspan = Enumerable.Repeat(0, Elements[0].Count).ToList();

                if (Header != null)
                    text += $"|+{Header}\n";

                bool first = true;
                bool firstRowHeaders;
                int elementsCount = Elements.Count;
                for (int i = 0; i < elementsCount; i++) {
                    if (first) {
                        first = false;
                        firstRowHeaders = _firstRowIsHeaders;
                    }
                    else {
                        firstRowHeaders = false;
                        text += "|-\n";
                    }

                    for (int j = 0; j < Elements[i].Count; j++) {
                        T item = Elements[i][j];
                        bool isRowspanColumn = false;
                        if (!firstRowHeaders && _rowspanColumns && rowspan[j] == 0) {
                            int k = i;
                            while (k < elementsCount) {
                                if (k + 1 == elementsCount)
                                    break;

                                T element = Elements[k + 1][j];
                                if (item.ToString() == element.ToString()) {
                                    k++;
                                }
                                else {
                                    break;
                                }
                            }

                            if (k > i) {
                                int rowspanNum = k - i;
                                rowspan[j] = rowspanNum;
                                text += $"| rowspan=\"{rowspanNum + 1}\" ";
                                isRowspanColumn = true;
                            }
                        }

                        if (_rowspanColumns && rowspan[j] > 0) {
                            if (!isRowspanColumn) {
                                rowspan[j]--;
                                continue;
                            }
                        }

                        if (firstRowHeaders) {
                            text += "!";
                        }
                        else {
                            text += "|";
                        }

                        text += $"{item}\n";
                    }
                }

                text += "|}<br/>\n";

                return text;
            }
        }
        private class ItemInfo
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
                for(int i = 0; i < Item.ToolTip.Lines; i++) {
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
                    List<string> dropInfo = new() { $"{npcType.ToNpcPNG()}", (minDropped != maxDropped ? $"{minDropped}-{maxDropped}" : $"{minDropped}"), $"{drops[type][npcType].dropRate}%" };//Make vanilla link option to vanilla wiki
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
                    string sellText = $"Sold by the Witch for {sellPriceString}. Can only appear in the shop if this condition is met: {soldByWitch.SellCondition}";
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
                //webpage.NewLine();
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
                    if (includeReverse && createItemCount == 1 || !includeReverse && createItemCount > 1)
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
                    string breaks = FillString(diff, "<br/>");

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
        public class CommonItemList
        {
            public List<Item> CommonList { get; private set; }
            public List<Item> UniqueList { get; private set; }
            public int Count => (CommonList != null ? CommonList.Count : 0) + (UniqueList != null ? 1 : 0);
            public string CommonToAll {
                get {
                    if (commonToAll == null) {
                        commonToAll = UniqueList.Select(i => i.ToItemPNG()).ToList().CommonToAll();
                    }

                    return commonToAll;
                }
            }
            private string commonToAll;
            public CommonItemList(List<Item> list, Item uniqueItem = null) {
                if (uniqueItem != null) {
                    UniqueList = new() { uniqueItem };
                }

                CommonList = new(list);
            }
            public void Add(List<Item> list) {
                List<string> stringList = list.Select(i => i.ToString()).ToList();
                if (UniqueList == null) {
                    UniqueList = new();
                    for (int i = 0; i < CommonList.Count(); i++) {
                        string commonListItemName = CommonList[i].ToString();
                        if (!stringList.Contains(commonListItemName)) {
                            UniqueList.Add(CommonList[i]);
                            commonToAll = null;
                            CommonList.RemoveAt(i);
                            break;
                        }
                    }
                }

                int index = UniqueList.Count;
                List<string> commonStringList = CommonList.Select(i => i.ToString()).ToList();
                List<string> uniqueStringList = UniqueList.Select(i => i.ToString()).ToList();
                for (int i = 0; i < list.Count; i++) {
                    string s = stringList[i];
                    if (!commonStringList.Contains(s)) {
                        if (!uniqueStringList.Contains(s)) {
                            UniqueList.Add(list[i]);
                            uniqueStringList = UniqueList.Select(i => i.ToString()).ToList();
                            commonToAll = null;
                        }

                        break;
                    }
                }
            }
            public bool TryAdd(CommonItemList other) {
                if (SameCommonItemsCompareTypesIgnoreStack(other)) {
                    if (other.UniqueList != null) {
                        UniqueList.CombineLists(other.UniqueList, true);
                    }
                    else {
                        Add(other.CommonList);
                    }

                    return true;
                }

                return false;
            }
            public override string ToString() {
                string text = "";
                if (CommonList.Count > 0) {
                    foreach (string s in CommonList.Select(i => i.ToItemPNG())) {
                        text += $"{s}<br/>";
                    }
                }

                if (UniqueList == null)
                    return text;

                int uniqueCount = UniqueList.Count;
                if (uniqueCount >= 20) {
                    text += CommonToAll;
                }
                else if (uniqueCount > 0) {
                    if (text != "")
                        text += "and<br/>";

                    bool first = true;
                    foreach (string s in UniqueList.Select(i => i.ToItemPNG())) {
                        if (first) {
                            first = false;
                        }
                        else {
                            text += $"<br/>or<br/>";
                        }

                        text += $"{s}";
                    }
                }

                return text;
            }
            public bool SameCommonItems(CommonItemList other) {
                bool nullUniqueList = other.UniqueList == null;
                List<string> myCommonList = CommonList.Select(i => i.ToString()).ToList();
                List<string> otherCommonList = other.CommonList.Select(i => i.ToString()).ToList();
                if (Count != other.Count)
                    return false;

                int failedMatchCount = 0;
                foreach (string s in otherCommonList) {
                    if (!myCommonList.Contains(s)) {
                        failedMatchCount++;
                        if (failedMatchCount > 0 && !nullUniqueList || failedMatchCount > 1 && nullUniqueList)
                            return false;
                    }
                }

                return true;
            }
            public bool SameCommonItemsCompareTypesIgnoreStack(CommonItemList other) {
                bool nullUniqueList = other.UniqueList == null;
                if (Count != other.Count)
                    return false;

                List<int> myCommonList = CommonList.Select(i => i.type).ToList();
                int failedMatchCount = 0;
                foreach (int num in other.CommonList.Select(i => i.type)) {
                    if (!myCommonList.Contains(num)) {
                        failedMatchCount++;
                        if (failedMatchCount >= 0 && !nullUniqueList || failedMatchCount > 1 && nullUniqueList)
                            return false;
					}
				}

                return true;
            }
            public int NumberCommonItems(CommonItemList other) {
                if (CommonList.Count == 0 || other.CommonList.Count == 0)
                    return 0;

                bool nullUniqueList = other.UniqueList == null;
                List<string> myCommonList = CommonList.Select(i => i.ToString()).ToList();
                List<string> otherCommonList = other.CommonList.Select(i => i.ToString()).ToList();

                int failedMatchCount = 0;
                foreach (string s in otherCommonList) {
                    if (!myCommonList.Contains(s)) {
                        failedMatchCount++;
                    }
                }

                return CommonList.Count - failedMatchCount;
            }
            public bool ExactSame(CommonItemList other) => UniqueList.SameAs(other.UniqueList) && CommonList.SameAs(other.CommonList);
        }
    }

    public static class WikiStaticMethods
    {
        private static Dictionary<int, int> itemsFromTiles;
        public static string ToLink(this string s, string text = null) => $"[[{s.AddSpaces()}{(text != null ? $"|{text}" : "")}]]";
        public static string ToFile(this string s) => $"[[File:{s.RemoveSpaces()}.png]]";
        public static string ToItemPNG(this Item item, bool link = false, bool displayName = true) {
            int type = item.type;
            string name;
            if (type < ItemID.Count) {
                name = $"Item_{type}".ToFile();
            }
            else {
                ModItem modItem = item.ModItem;
                if (modItem == null) {
                    name = item.Name;
                }
                else {
                    name = modItem.Name;
                }

                name = name.ToFile();
            }

            int stack = item.stack;
            return $"{(!displayName ? $"{stack}" : "")}{name}{(link ? " " + item.Name.ToLink() : displayName ? " " + item.Name : "")}{(displayName && stack > 1 ? $" ({stack})" : "")}";
        }
        public static string ToItemPNG(this int type, int num = 1, bool link = false, bool displayName = true) {
            return new Item(type, num).ToItemPNG(link, displayName);
        }
        public static string ToItemPNG(this string s, bool link = false, bool displayName = true) {
            return $"{s.ToFile()} {(link ? s.ToLink() : displayName ? s : "")}";
        }
        public static string ToNpcPNG(this int npcNetID, bool link = false, bool displayName = true) {
            string name;
            NPC npc = ContentSamples.NpcsByNetId[npcNetID];
            if (npcNetID < NPCID.Count) {
                name = $"NPC_{npcNetID}";
            }
            else {
                ModNPC modNPC = npc.ModNPC;
                if (modNPC == null) {
                    name = npc.FullName;
                }
                else {
                    name = modNPC.Name;
                }
            }

            return $"{name.ToFile()}{(link ? " " + name.ToLink() : displayName ? " " + npc.FullName : "")}";
        }
        public static string GetCoinsPNG(this int sellPrice) {
            int coinType = ItemID.PlatinumCoin;
            int coinValue = 1000000;
            string text = "";
            while (sellPrice > 0) {
                int numCoinsToSpawn = sellPrice / coinValue;
                if (numCoinsToSpawn > 0)
                    text += coinType.ToItemPNG(numCoinsToSpawn, displayName: false) + " ";

                sellPrice %= coinValue;
                coinType--;
                coinValue /= 100;
            }

            return text;
        }
        public static string ToItemFromTilePNG(this int tileNum) {
            if (tileNum <= 0) {
                return "NoTileRequired.png";
            }

            int itemType = GetItemTypeFromTileType(tileNum);
            if (itemType <= 0) {
                $"Failed to find an item for tileNum: {tileNum}".Log();
                return "NoTileRequired.png";
            }

            Item item = new(itemType);

            return item.ToItemPNG();
        }
        public static int GetItemTypeFromTileType(this int tileType) {
            if (tileType <= 0)
                return -1;

            if (itemsFromTiles == null) {
                itemsFromTiles = new();
            }
            else {
                if (itemsFromTiles.ContainsKey(tileType))
                    return itemsFromTiles[tileType];
            }

            for (int i = 0; i < ItemLoader.ItemCount; i++) {
                Item sampleItem = ContentSamples.ItemsByType[i];
                if (sampleItem == null)
                    continue;

                if (sampleItem.createTile == tileType) {
                    itemsFromTiles.Add(tileType, i);
                    return i;
                }
            }

            return -1;
        }
        public static Item GetItemFromTileType(this int tileType) {
            return new(GetItemTypeFromTileType(tileType));
        }
        public static string JoinLists(this IEnumerable<IEnumerable<string>> lists, string joinString = "or") {
            string text = "";
            bool first = true;
            foreach (IEnumerable<string> list in lists) {
                if (first) {
                    first = false;
                }
                else {
                    text += $"<br/>{joinString}<br/>";
                }

                text += list.JoinList();
            }

            return text;
        }
        public static string JoinList(this IEnumerable<string> list, string joinString = "<br/>") {
            string text = "";
            bool firstString = true;
            foreach (string s in list) {
                if (firstString) {
                    firstString = false;
                }
                else {
                    text += joinString;
                }

                text += s;
            }

            return text;
        }
        public static bool SameAs(this IEnumerable<Item> list1, IEnumerable<Item> list2) {
            if (list1 == null && list2 == null)
                return true;

            if (list1 == null || list2 == null)
                return false;

            if (list1.Count() != list2.Count())
                return false;

            IEnumerable<int> list2Types = list2.Select(i => i.type);

            foreach (int type in list1.Select(i => i.type)) {
                if (!list2Types.Contains(type))
                    return false;
            }

            return true;
        }
        //public static bool SameExceptOne(this IEnumerable<Item> list1, IEnumerable<Item> list2) {
        //    if (Math.Abs(list1.Count() - list2.Count()) > 1)
        //}
    }
}
