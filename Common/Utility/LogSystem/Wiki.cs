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
using WeaponEnchantments.Items.Enchantments;

namespace WeaponEnchantments.Common.Utility.LogSystem
{
	public static class Wiki
	{
        public static Dictionary<int, List<RecipeData>> createItemRecipes;
        public static Dictionary<int, List<RecipeData>> recipesUsedIn;
        public static Dictionary<int, Dictionary<int, DropRateInfo>> enemyDrops;
        public static Dictionary<int, Dictionary<ChestID, float>> chestDrops;
        public static Dictionary<int, Dictionary<CrateID, float>> crateDrops;
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
            IEnumerable<EnchantingTableItem> enchantingTables = modItems.OfType<EnchantingTableItem>().OrderBy(t => t.enchantingTableTier);
            IEnumerable<EnchantmentEssence> enchantmentEssence = modItems.OfType<EnchantmentEssence>().OrderBy(e => e.Name);
            IEnumerable<Enchantment> enchantments = modItems.OfType<Enchantment>().OrderBy(e => e.Name);

            List<WebPage> webPages = new();

            AddContainments(webPages, containmentItems, enchantments);
            AddEnchantingTables(webPages, enchantingTables);
            AddEssence(webPages, enchantmentEssence);
            AddEnchantments(webPages, enchantments);

            string wiki = "\n\n";

            foreach (WebPage webPage in webPages) {
                wiki += webPage.ToString() + "\n".FillString(5);
            }

            wiki.Log();
        }
        private static void AddContainments(List<WebPage> webPages, IEnumerable<ContainmentItem> containmentItems, IEnumerable<Enchantment> enchantments) {
            WebPage Containments = new("Containments");
            Containments.AddParagraph("Containments contain the power of an enchantment. More powerful enchantments require larger and stronger containments to hold them.\n" +
            "Containments are crafting materials used to craft enchantments.");
            string text = "Only these enchantments can be obtained by crafting.The others must all be found in other ways.\n";
            Containments.AddParagraph(text);
            Containments.AddBulletedList(true, true, enchantments.Where(e => e.LowestCraftableTier == 0 && e.EnchantmentTier == 0).Select(c => c.Name.AddSpaces()).ToArray());
            foreach (ContainmentItem containment in containmentItems) {
                int tier = containment.tier;
                string subHeading = $"{containment.Item.ToItemPNG()} (Tier {tier})";
                Containments.AddParagraph($"{containment.Item.ToItemPNG(link: true)} (Tier {tier})");
                WebPage containmentPage = new(containment.Item.Name);
                ItemInfo itemInfo = new(containment);
                containmentPage.AddLink("Containments");
                itemInfo.AddStatistics(containmentPage);
                itemInfo.AddDrops(containmentPage);
                itemInfo.AddInfo(containmentPage);
                itemInfo.AddRecipes(containmentPage);
                webPages.Add(containmentPage);
            }

            webPages.Add(Containments);
        }
        private static void AddEnchantingTables(List<WebPage> webPages, IEnumerable<EnchantingTableItem> enchantingTables) {
            WebPage EnchantingTable = new("Enchantment enchantingTable");
            EnchantingTable.AddParagraph("");
            EnchantingTable.AddBulletedList(elements: new string[] {
                
            });
            foreach (EnchantingTableItem enchantingTable in enchantingTables) {
                int tier = enchantingTable.enchantingTableTier;
                EnchantingTable.AddParagraph($"{enchantingTable.Item.ToItemPNG(link: true)} (Tier {tier})");
                WebPage enchantingTablePage = new(enchantingTable.Item.Name);
                ItemInfo itemInfo = new(enchantingTable);
                enchantingTablePage.AddLink("Enchantment enchantingTable");
                itemInfo.AddStatistics(enchantingTablePage);
                itemInfo.AddDrops(enchantingTablePage);
                itemInfo.AddDrops(enchantingTablePage);
                itemInfo.AddRecipes(enchantingTablePage);
                webPages.Add(enchantingTablePage);

                EnchantingTable.AddParagraph($"{enchantingTable.Item.ToItemPNG(link: true)} (Tier {tier})");
            }

            webPages.Add(EnchantingTable);
        }
        private static void AddEssence(List<WebPage> webPages, IEnumerable<EnchantmentEssence> enchantmentEssence) {
            WebPage Essence = new("Enchantment Essence");
            Essence.AddParagraph("Essence represents solidified experience and are automatically stored in the enchanting table inventory (like a piggy bank). They can be used to...");
            Essence.AddBulletedList(elements: new string[] {
                "Upgrade enchantments",
                "Infuse it's XP value into items"
            });
            foreach (EnchantmentEssence essence in enchantmentEssence) {
                int tier = essence.EssenceTier;
                Essence.AddParagraph($"{essence.Item.ToItemPNG(link: true)} (Tier {tier})");
                WebPage essencePage = new(essence.Item.Name);
                ItemInfo itemInfo = new(essence);
                essencePage.AddLink("Enchantment Essence");
                itemInfo.AddStatistics(essencePage);
                itemInfo.AddDrops(essencePage);
                itemInfo.AddDrops(essencePage);
                itemInfo.AddRecipes(essencePage);
                webPages.Add(essencePage);
            }

            webPages.Add(Essence);
        }
        private static void AddEnchantments(List<WebPage> webPages, IEnumerable<Enchantment> enchantments) {
            WebPage Enchantments = new("Enchantments");
            Enchantments.AddSubHeading("Enchantment Effects");
            Enchantments.AddParagraph($"Enchantments allow customization of your items with various effects.  Some are very basic stat upgrades " +
                $"such as {EnchantmentStat.Damage.EnchantmentTypeShortLink()}, {EnchantmentStat.CriticalStrikeChance.EnchantmentTypeShortLink()}, " +
                $"{EnchantmentStat.Defense.EnchantmentTypeShortLink()}, {EnchantmentStat.LifeSteal.EnchantmentTypeShortLink()}.  Other are more unique such as: " +
                $"Hitting all enemies in an area ({EnchantmentStat.OneForAll.EnchantmentTypeShortLink()}), " +
                $"Dealing massive damage but having a long recovery ({EnchantmentStat.AllForOne.EnchantmentTypeShortLink()}), " +
                $"Flames that spread between enemies ({"WorldAblaze".EnchantmentTypeShortLink()}), " +
                $"Max health true damage ({EnchantmentStat.GodSlayer.EnchantmentTypeShortLink()}).");
            Enchantments.AddSubHeading("Capacity Cost");
            Enchantments.AddParagraph("Each enchantment has a capacity cost. This cost is subtracted from the item enchantment capacity.");
            Enchantments.AddSubHeading("Crafting and Upgrading Enchantments");
            Enchantments.AddParagraph($"Essence in the enchanting table is available for crafting. There is no need to take them out of the crafting table. " +
                $"({"https://steamcommunity.com/sharedfiles/filedetails/?id=2563309347&searchtext=magic+storage".ToExternalLink("Magic Storage")} can access the essence via the Environment Simulator.  See {"Magic Storage Integration".ToLink()})<br/>\n" +
                $"Each enchantment page has the specific crafting recipes for the enchantment.  These are the general recipes:<br/>\n" +
                $"Topaz can be any Common Gem: {"WeaponEnchantments:CommonGems".ToItemPNGs(link: true)}<br/>\n" +
                $"Amber can be any Rare Gem: {"WeaponEnchantments:RareGems".ToItemPNGs(link: true)}<br/>\n" +
				$"{"Enchantment Basic".ToLabledPNG()} is used as a generic enchantment when any enchantment could be used.");
            Enchantments.AddTable(GetGenericEnchantmnetRecipes(), label: "Recipes", firstRowHeaders: true, rowspanColumns: true, collapsible: true);
            Enchantments.AddParagraph($"To view recipes in game, you can use the vanilla guide's crafting interface or the " +
				$"{"https://steamcommunity.com/sharedfiles/filedetails/?id=2619954303&searchtext=recipe+browser".ToExternalLink("Recipe Browser Mod")}");
            Enchantments.AddSubHeading("All Enchantment types");

            WebPage enchantmentTypePage = new("");
            string typePageLinkString = "";
            foreach (IEnumerable<Enchantment> list in enchantments.GroupBy(e => e.EnchantmentTypeName).Select(l => l.ToList().OrderBy(e => e.EnchantmentTier))) {
                bool first = true;
                foreach (Enchantment enchantment in list.ToList()) {
                    if (first) {
                        first = false;
                        string enchantmentType = enchantment.EnchantmentTypeName.AddSpaces() + " Enchantment";
                        enchantmentTypePage = new(enchantmentType);
                        enchantmentTypePage.AddLink("Enchantments");
                        Enchantments.AddParagraph(enchantment.Item.ToItemPNG(link: true, linkText: enchantmentType));
                        typePageLinkString = enchantmentType.ToLink();
                    }

                    int tier = enchantment.EnchantmentTier;
                    enchantmentTypePage.AddParagraph($"{enchantment.Item.ToItemPNG(link: true)} (Tier {tier})");
                    WebPage enchantmentPage = new(enchantment.Item.Name);
                    enchantmentPage.AddLink("Enchantments");
                    enchantmentPage.AddParagraph(typePageLinkString);
                    ItemInfo itemInfo = new(enchantment);
                    itemInfo.AddStatistics(enchantmentPage);
                    itemInfo.AddDrops(enchantmentPage);
                    itemInfo.AddRecipes(enchantmentPage);
                    webPages.Add(enchantmentPage);
                }

                webPages.Add(enchantmentTypePage);
            }

            webPages.Add(Enchantments);
        }
        private static List<List<string>> GetGenericEnchantmnetRecipes() {
            List<RecipeData> genericEnchantmentRecipes = new();
            List<int> enchantmentIDs = new() {
                ModContent.ItemType<DamageEnchantmentBasic>(),
                ModContent.ItemType<DamageEnchantmentCommon>(),
                ModContent.ItemType<DamageEnchantmentRare>(),
                ModContent.ItemType<DamageEnchantmentEpic>(),
                ModContent.ItemType<DamageEnchantmentLegendary>()
            };

            for (int i = 0; i < enchantmentIDs.Count; i++) {
                foreach (RecipeData data in createItemRecipes[enchantmentIDs[i]]) {
                    if (data.requiredItem.CommonList.Where(m => m.ModItem is EnchantmentEssence).Count() == 1) {
                        genericEnchantmentRecipes.Add(data);
                        continue;
                    }
                }
            }

            List<List<string>> recipes = new() { new() { "Result", "Ingredients", "Crafting station" } };
            foreach (RecipeData data in genericEnchantmentRecipes.OrderBy(rd => -rd.requiredTile.ToString().Length)) {
                ItemInfo.ConvertRecipeDataListToStringList(recipes, data);
            }

            for(int i = 0; i < recipes.Count; i++) {
                List<string> recipe = recipes[i];
                for(int k = 0; k < recipe.Count; k++) {
                    string newString = recipes[i][k].Replace("Damage ", "").Replace("Damage", "");
                    if (newString != recipes[i][k]) 
                        recipes[i][k] = newString.Replace("[[Enchantment Basic]]", "Enchantment Basic").Replace("[[Enchantment Common]]", "Enchantment Common").Replace("[[Enchantment Rare]]", "Enchantment Rare").Replace("[[Enchantment Epic]]", "Enchantment Epic").Replace("[[Enchantment Legendary]]", "Enchantment Legendary");
				}
			}

            return recipes;
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

            chestDrops = new();
            foreach (ChestID chestID in Enum.GetValues(typeof(ChestID)).Cast<ChestID>().ToList().Where(c => c != ChestID.None)) {
                WEModSystem.GetChestLoot(chestID, out List<WeightedPair> pairs, out float baseChance);
                if (pairs == null)
                    continue;

                string name = chestID.ToString() + " Chest";
                float total = 0f;
                foreach (WeightedPair pair in pairs) {
                    total += pair.Weight;
                }

                foreach (WeightedPair pair in pairs) {
                    Item sampleItem = ContentSamples.ItemsByType[pair.ID];
                    int type = sampleItem.type;
                    if (!(type >= min && type <= max))
                        continue;

                    if (chestDrops.ContainsKey(type)) {
                        chestDrops[type].Add(chestID, baseChance * pair.Weight / total);
					}
					else {
                        chestDrops.Add(type, new() { { chestID, baseChance * pair.Weight / total } });
                    }
                }
            }

            crateDrops = new();
            foreach (KeyValuePair<int, List<WeightedPair>> crate in GlobalCrates.crateDrops) {
                string name = ((CrateID)crate.Key).ToString() + " Crate";
                float total = 0f;
                foreach (WeightedPair pair in crate.Value) {
                    total += pair.Weight;
                }

                foreach (WeightedPair pair in crate.Value) {
                    Item sampleItem = ContentSamples.ItemsByType[pair.ID];
                    int type = sampleItem.type;
                    if (!(type >= min && type <= max))
                        continue;

                    float baseChance = GlobalCrates.GetCrateEnchantmentDropChance(crate.Key);
                    if (crateDrops.ContainsKey(type)) {
                        if (crateDrops[type].ContainsKey((CrateID)crate.Key)) {
                            $"New: item: {sampleItem.S()}, CrateID: {(CrateID)crate.Key}, chance: {baseChance * pair.Weight / total}.  Old chance: {crateDrops[type][(CrateID)crate.Key]}".LogSimple();
                            continue;
                            /*
[23:42:04.661] [.NET ThreadPool Worker/INFO] [WeaponEnchantments]: New: item: Attack Speed Enchantment Basic, CrateID: Iron, chance: 0.008305647.  Old chance: 0.008305647
[23:42:04.663] [.NET ThreadPool Worker/INFO] [WeaponEnchantments]: New: item: Critical Strike Chance Enchantment Basic, CrateID: Iron, chance: 0.008305647.  Old chance: 0.008305647
[23:42:04.664] [.NET ThreadPool Worker/INFO] [WeaponEnchantments]: New: item: Ammo Cost Enchantment Basic, CrateID: Iron, chance: 0.008305647.  Old chance: 0.008305647
[23:42:04.666] [.NET ThreadPool Worker/INFO] [WeaponEnchantments]: New: item: Crate Enchantment Basic, CrateID: Iron, chance: 0.016611295.  Old chance: 0.016611295
[23:42:04.668] [.NET ThreadPool Worker/INFO] [WeaponEnchantments]: New: item: Danger Sense Enchantment Basic, CrateID: Iron, chance: 0.008305647.  Old chance: 0.008305647
[23:42:04.670] [.NET ThreadPool Worker/INFO] [WeaponEnchantments]: New: item: Fishing Enchantment Basic, CrateID: Iron, chance: 0.016611295.  Old chance: 0.016611295
[23:42:04.671] [.NET ThreadPool Worker/INFO] [WeaponEnchantments]: New: item: Hunter Enchantment Basic, CrateID: Iron, chance: 0.008305647.  Old chance: 0.008305647
[23:42:04.672] [.NET ThreadPool Worker/INFO] [WeaponEnchantments]: New: item: Obsidian Skin Enchantment Basic, CrateID: Iron, chance: 0.008305647.  Old chance: 0.008305647
[23:42:04.673] [.NET ThreadPool Worker/INFO] [WeaponEnchantments]: New: item: Sonar Enchantment Basic, CrateID: Iron, chance: 0.016611295.  Old chance: 0.016611295
[23:42:04.675] [.NET ThreadPool Worker/INFO] [WeaponEnchantments]: New: item: Spelunker Enchantment Basic, CrateID: Iron, chance: 0.008305647.  Old chance: 0.008305647
[23:42:04.677] [.NET ThreadPool Worker/INFO] [WeaponEnchantments]: New: item: Critical Strike Chance Enchantment Basic, CrateID: Jungle, chance: 0.040983606.  Old chance: 0.040983606
[23:42:04.679] [.NET ThreadPool Worker/INFO] [WeaponEnchantments]: New: item: Luck Enchantment Basic, CrateID: Seaside_OceanHard, chance: 0.008333334.  Old chance: 0.008333334
                            */
                        }

                        crateDrops[type].Add((CrateID)crate.Key, baseChance * pair.Weight / total);
                    }
                    else {
                        crateDrops.Add(type, new() { { (CrateID)crate.Key, baseChance * pair.Weight / total } });
                    }
                }
            }
        }
        private static void GetDrops() {
            enemyDrops = new();
            for (int npcNetID = NPCID.NegativeIDCount + 1; npcNetID < NPCLoader.NPCCount; npcNetID++) {
                foreach (IItemDropRule dropRule in Main.ItemDropsDB.GetRulesForNPCID(npcNetID)) {
                    List<DropRateInfo> dropRates = new();
                    DropRateInfoChainFeed dropRateInfoChainFeed = new(1f);
                    dropRule.ReportDroprates(dropRates, dropRateInfoChainFeed);
                    foreach (DropRateInfo dropRate in dropRates) {
                        int itemType = dropRate.itemId;
                        if (itemType >= min && itemType <= max) {
                            if (enemyDrops.ContainsKey(itemType)) {
                                if (enemyDrops[itemType].ContainsKey(npcNetID)) {
                                    $"itemType: {new Item(itemType).S()}, npcType{ContentSamples.NpcsByNetId[npcNetID]}".LogSimple();
                                }
                                else {
                                    enemyDrops[itemType].Add(npcNetID, dropRate);
                                }
                            }
                            else {
                                enemyDrops.Add(itemType, new() { { npcNetID, dropRate } });
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
        public static string EnchantmentTypeShortLink(this string s) => $"{s.AddSpaces()} Enchantment".ToLink(s.AddSpaces());
        public static string EnchantmentTypeShortLink(this EnchantmentStat enchantmentStat) => enchantmentStat.ToString().EnchantmentTypeShortLink();
        public static string ToExternalLink(this string s, string text = null) => $"[{s} {text}]";
        public static string ToPNG(this string s) => $"[[File:{s.RemoveSpaces()}.png]]";
        public static string ToPNGLink(this string s) => s.ToPNG() + s.ToLink();
        public static string ToLabledPNG(this string s) => s.ToPNG() + s;
        public static string ToItemPNG(this Item item, bool link = false, bool displayName = true, bool displayNum = false, string linkText = null) {
            int type = item.type;
            string name;
            string file;
            string linkString = "";
            if (type < ItemID.Count) {
                file = $"Item_{type}";
                name = item.Name;
                if (link)
                    linkString = $"https://terraria.fandom.com/wiki/{name.Replace(" ", "_")}".ToExternalLink(name);
            }
            else {
                ModItem modItem = item.ModItem;
                if (modItem == null) {
                    file = item.Name;
                    name = linkText ?? item.Name;
                }
                else {
                    file = modItem.Name;
                    name = linkText ?? modItem.Name.AddSpaces();
                }

                if (link)
                    linkString = name.ToLink();
            }

            int stack = item.stack;
            return $"{(!displayName && displayNum ? $"{stack}" : "")}{file.ToPNG()}{(link ? " " + linkString : displayName ? " " + name : "")}{(displayName && stack > 1 ? $" ({stack})" : "")}";
        }
        public static string ToItemPNG(this int type, int num = 1, bool link = false, bool displayName = true, bool dislpayNum = false, string label = null) {
            return new Item(type, num).ToItemPNG(link, displayName, dislpayNum, label);
        }
        public static string ToItemPNG(this short type, int num = 1, bool link = false, bool displayName = true, bool dislpayNum = false, string label = null) =>
            ToItemPNG((int)type, num, link, displayName, dislpayNum, label);
        public static string ToItemPNG(this string s, bool link = false, bool displayName = true, string linkText = null) {
            return $"{s.ToPNG()} {(link ? s.ToLink(linkText) : displayName ? s : "")}";
        }
        public static string ToItemPNGs(this string recipeGroupKey, bool link = false, bool displayName = true) =>
            RecipeGroup.recipeGroups[RecipeGroup.recipeGroupIDs[recipeGroupKey]].ValidItems.Select(i => i.ToItemPNG(link: link, displayName: displayName)).JoinList(", ");
        public static string ToNpcPNG(this int npcNetID, bool link = false, bool displayName = true) {
            string name;
            string file;
            string linkString = "";
            NPC npc = ContentSamples.NpcsByNetId[npcNetID];
            if (npcNetID < NPCID.Count) {
                file = npcNetID.GetNPCPNGLink();
                name = npc.FullName;
                if (link)
                    linkString = $"https://terraria.fandom.com/wiki/{name.Replace(" ", "_")}".ToExternalLink(name);
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

                if (link)
                    linkString = name.ToLink();
            }

            return $"{file.ToPNG()}{(link ? " " + linkString : displayName ? " " + name : "")}";
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
