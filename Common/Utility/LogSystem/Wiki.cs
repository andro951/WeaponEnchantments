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
using WeaponEnchantments.Common.Utility.LogSystem;
using WeaponEnchantments.Common.Utility.LogSystem.WebpageComponenets;

namespace WeaponEnchantments.Common.Utility.LogSystem
{
	public static class Wiki
	{
        public static Dictionary<int, List<RecipeData>> createItemRecipes;
        public static Dictionary<int, List<RecipeData>> recipesUsedIn;
        public static Dictionary<int, Dictionary<int, DropRateInfo>> drops;
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
            Containments.AddParagraph("Containments contain the power of an enchantment. More powerful enchantments require larger and stronger containments to hold them.\n" +
            "Containments are crafting materials used to craft enchantments.");
            Containments.NewLine();
            foreach (ContainmentItem containment in containmentItems) {
                
                string name = containment.Name;
                int tier = containment.tier;
                string subHeading = $"{containment.Item.ToItemPNG()} (Tier {tier})";
                Containments.AddParagraph($"{containment.Item.ToItemPNG(link: true)} (Tier {tier})");
                WebPage containmentPage = new(containment.Item.Name);
                ItemInfo itemInfo = new(containment);
                containmentPage.AddLink("Containments");
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
            }
        }
    }

    public static class WikiStaticMethods
    {
        private static Dictionary<int, int> itemsFromTiles;
        public static string ToLink(this string s, string text = null) => $"[[{s}{(text != null ? $"|{text}" : "")}]]";
        public static string ToFile(this string s) => $"[[File:{s.RemoveSpaces()}.png]]";
        public static string ToItemPNG(this Item item, bool link = false, bool displayName = true, bool displayNum = false) {
            int type = item.type;
            string name;
            string file;
            if (type < ItemID.Count) {
                file = $"Item_{type}";
                name = item.Name;
            }
            else {
                ModItem modItem = item.ModItem;
                if (modItem == null) {
                    file = item.Name;
                    name = item.Name;
                }
                else {
                    file = modItem.Name;
                    name = modItem.Name.AddSpaces();
                }
            }

            int stack = item.stack;
            return $"{(!displayName && displayNum ? $"{stack}" : "")}{file.ToFile()}{(link ? " " + name.ToLink() : displayName ? " " + name : "")}{(displayName && stack > 1 ? $" ({stack})" : "")}";
        }
        public static string ToItemPNG(this int type, int num = 1, bool link = false, bool displayName = true, bool dislpayNum = false) {
            return new Item(type, num).ToItemPNG(link, displayName, dislpayNum);
        }
        public static string ToItemPNG(this string s, bool link = false, bool displayName = true) {
            return $"{s.ToFile()} {(link ? s.ToLink() : displayName ? s : "")}";
        }
        public static string ToNpcPNG(this int npcNetID, bool link = false, bool displayName = true) {
            string name;
            string file;
            NPC npc = ContentSamples.NpcsByNetId[npcNetID];
            if (npcNetID < NPCID.Count) {
                file = $"NPC_{npcNetID}";
                name = npc.FullName;
            }
            else {
                ModNPC modNPC = npc.ModNPC;
                if (modNPC == null) {
                    name = npc.FullName;
                    file = name;
                }
                else {
                    name = modNPC.Name.AddSpaces();
                    file = name;
                }
            }

            return $"{file.ToFile()}{(link ? " " + name.ToLink() : displayName ? " " + name : "")}";
        }
        public static string GetCoinsPNG(this int sellPrice) {
            int coinType = ItemID.PlatinumCoin;
            int coinValue = 1000000;
            string text = "";
            bool first = true;
            while (sellPrice > 0) {
                int numCoinsToSpawn = sellPrice / coinValue;
                if (numCoinsToSpawn > 0) {
                    if (first) {
                        first = false;
					}
                    else {
                        text += " ";
					}

                    text += coinType.ToItemPNG(numCoinsToSpawn, displayName: false, dislpayNum: true);
                }

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
