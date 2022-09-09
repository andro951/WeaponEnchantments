using Hjson;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Items;
using WeaponEnchantments.Localization;
using static Terraria.Localization.GameCulture;
using static WeaponEnchantments.Common.Utility.LogModSystem.GetItemDictModeID;
using static WeaponEnchantments.Common.Utility.LogModSystem;

namespace WeaponEnchantments.Common.Utility
{
    public class LogModSystem : ModSystem {
        public static bool printListOfContributors = false;
        public static bool printListOfEnchantmentTooltips => WEMod.clientConfig.PrintEnchantmentTooltips;
        public static bool printLocalization = WEMod.clientConfig.PrintLocalizationLists;
        public static bool printListForDocumentConversion = false;
        public static bool printEnchantmentDrops => WEMod.clientConfig.PrintEnchantmentDrops;
        public static bool printWiki = WEMod.clientConfig.PrintWikiInfo;

        public static class GetItemDictModeID {
            public static byte Weapon = 0;
            public static byte Armor = 1;
            public static byte Accessory = 2;
        }
        public static Dictionary<int, bool> PrintListOfItems = new Dictionary<int, bool>() {
            { Weapon, false },
            { Armor, false },
            { Accessory, false }
        };


        //Only used to print the full list of contributors.
        private static Dictionary<string, string> contributorLinks = new Dictionary<string, string>() {
            { "Zorutan", "https://twitter.com/ZorutanMesuta" }
		};

        public struct Contributors
        {
            public Contributors(string artist, string designer) {
                Artist = artist;
                Designer = designer;
            }
            public string Artist;
            public string Designer;
        }
        public static SortedDictionary<string, Contributors> contributorsData = new SortedDictionary<string, Contributors>();
        public static List<string> namesAddedToContributorDictionary = new List<string>();
        public static List<string> enchantmentsLocalization = new List<string>();
        public static SortedDictionary<int, List<(float, List<WeightedPair>)>> npcEnchantmentDrops = new();
	    private static string localization = "";
	    private static int tabs = 0;
	    private static List<string> labels;
        private static Dictionary<string, ModTranslation> translations;
        private static int culture;

        //Only used to print the full list of enchantment tooltips in WEPlayer OnEnterWorld()  (Normally commented out there)
        //public static string listOfAllEnchantmentTooltips = "";

        //Requires an input type to have properties: Texture
        public override void OnWorldLoad() {
            PrintListOfEnchantmentTooltips();

            //Contributors  change to give exact file location when added to contributor.
            PrintContributorsList();

            PrintAllLocalization();

            PrintEnchantmentDrops();

            PrintWiki();
        }
        public static void UpdateContributorsList<T>(T modTypeWithTexture, string sharedName = null) {
            if (!printListOfContributors)
                return;

            //Already added
            if (sharedName != null && namesAddedToContributorDictionary.Contains(sharedName))
                return;

            Type thisObjectsType = modTypeWithTexture.GetType();
            string texture = (string)thisObjectsType.GetProperty("Texture").GetValue(modTypeWithTexture);
            string artist = (string)thisObjectsType.GetProperty("Artist").GetValue(modTypeWithTexture);
            string designer = (string)thisObjectsType.GetProperty("Designer").GetValue(modTypeWithTexture);

            if (!contributorsData.ContainsKey(texture))
                contributorsData.Add(texture, new Contributors(artist, designer));

            if (sharedName != null)
                namesAddedToContributorDictionary.Add(sharedName);
        }
        private static void PrintAllLocalization() {
            if (!printLocalization)
                return;

            Mod mod = ModContent.GetInstance<WEMod>();
            TmodFile file = (TmodFile)typeof(Mod).GetProperty("File", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(mod);
            translations = new();
            Autoload(file);
            /*foreach (int i in Enum.GetValues(typeof(CultureName)).Cast<CultureName>().Where(n => n != CultureName.Unknown).Select(n => (int)n)) {
                foreach(string key in translations.Keys) {
                    $"{key}: {translations[key].GetTranslation(i)}".Log();
				}
            }*/
            foreach (int i in Enum.GetValues(typeof(CultureName)).Cast<CultureName>().Where(n => n != CultureName.Unknown).Select(n => (int)n)) {
                PrintLocalization((CultureName)i);
            }
        }
        private static void Autoload(TmodFile file) {
            var modTranslationDictionary = new Dictionary<string, ModTranslation>();

            AutoloadTranslations(file, modTranslationDictionary);

            foreach (var value in modTranslationDictionary.Values) {
                AddTranslation(value);
            }
        }
        public static void AddTranslation(ModTranslation translation) {
            translations[translation.Key] = translation;
        }
        private static void AutoloadTranslations(TmodFile file, Dictionary<string, ModTranslation> modTranslationDictionary) {
            
            foreach (var translationFile in file.Where(entry => Path.GetExtension(entry.Name) == ".hjson")) {
                using var stream = file.GetStream(translationFile);
                using var streamReader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);

                string translationFileContents = streamReader.ReadToEnd();

                var culture = GameCulture.FromPath(translationFile.Name);

                // Parse HJSON and convert to standard JSON
                string jsonString = HjsonValue.Parse(translationFileContents).ToString();

                // Parse JSON
                var jsonObject = JObject.Parse(jsonString);
                // Flatten JSON into dot seperated key and value
                var flattened = new Dictionary<string, string>();

                foreach (JToken t in jsonObject.SelectTokens("$..*")) {
                    if (t.HasValues) {
                        continue;
                    }

                    // Custom implementation of Path to allow "x.y" keys
                    string path = "";
                    JToken current = t;

                    for (JToken parent = t.Parent; parent != null; parent = parent.Parent) {
                        path = parent switch {
                            JProperty property => property.Name + (path == string.Empty ? string.Empty : "." + path),
                            JArray array => array.IndexOf(current) + (path == string.Empty ? string.Empty : "." + path),
                            _ => path
                        };
                        current = parent;
                    }

                    flattened.Add(path, t.ToString());
                }

                foreach (var (key, value) in flattened) {
                    string effectiveKey = key.Replace(".$parentVal", "");
                    if (!modTranslationDictionary.TryGetValue(effectiveKey, out ModTranslation mt)) {
                        // removing instances of .$parentVal is an easy way to make this special key assign its value
                        //  to the parent key instead (needed for some cases of .lang -> .hjson auto-conversion)
                        modTranslationDictionary[effectiveKey] = mt = LocalizationLoader.CreateTranslation(effectiveKey);
                    }

                    mt.AddTranslation(culture, value);
                }
            }
        }
        public static void PrintLocalization(CultureName cultureName) {
	        Start(cultureName);
	    
	        AddLabel("ItemName");
            IEnumerable<ModItem> modItems = ModContent.GetInstance<WEMod>().GetContent<ModItem>();
	        List<string> enchantmentNames = new();
	        foreach (Enchantment enchantment in modItems.OfType<Enchantment>()) {
	    	    enchantmentNames.Add(enchantment.Name);
	        }
	    
	        enchantmentNames.Sort();
	        GetLocalizationFromList(null, enchantmentNames);
	    
            var modItemLists = modItems
	    	    .Where(mi => mi is not Enchantment)
	    	    .GroupBy(mi => mi is Enchantment ? mi.GetType().BaseType.BaseType.Name : mi.GetType().BaseType.Name)
		        .Select(mi => new { Key = mi.GetType().BaseType.Name, ModItemList = mi})
		        .OrderBy(group => group.Key);
            
            foreach (var list in modItemLists) {
                GetLocalizationFromList(null, list.ModItemList);
            }

	        Close();
	    
	        FromLocalizationData();
	    
	        End();
        }
	    private static void FromLocalizationData() {
		    SortedDictionary<string, SData> all = LocalizationData.All;
		    GetFromSDataDict(all);
	    }
	    private static void GetFromSDataDict(SortedDictionary<string, SData> dict) {
            foreach (KeyValuePair<string, SData> pair in dict) {
                AddLabel(pair.Key);
                if (LocalizationData.autoFill.Contains(pair.Key))
                    AutoFill(pair);

                GetFromSData(pair.Value);
                Close();
            }
	    }
	    private static void GetFromSData(SData d) {
		    if (d.Values != null)
			    GetLocalizationFromList(null, d.Values);
		
		    if (d.Dict != null)
			    GetLocalizationFromDict(null, d.Dict);
		
		    if (d.Children != null)
			    GetFromSDataDict(d.Children);
	    }
	    private static void AutoFill(KeyValuePair<string, SData> pair) {
            IEnumerable<Type> types = AssemblyManager.GetLoadableTypes(Assembly.GetExecutingAssembly());

            List<string> list = types.Where(t => t.GetType() == Type.GetType(pair.Key))
			    .Where(t => !t.IsAbstract)
			    .Select(t => t.Name)
			    .ToList();
            SortedDictionary<string, string> dict = pair.Value.Dict;
		    foreach(string s in list) {
			    if(!dict.ContainsKey(s))
				    dict.Add(s, s.AddSpaces());
		    }
	    }
	    private static void AddLabel(string label) {
		    localization += Tabs(tabs) + label + ": {\n";
		    tabs++;
		    labels.Add(label);
	    }
	    private static void Start(CultureName cultureName) {
            culture = (int)cultureName;
            localization += "\n\n" + cultureName.ToString() + "\n";
		    labels = new();
		    AddLabel("Mods");
		    AddLabel("WeaponEnchantments");
	    }
	    private static void Close() {
		    tabs--;
            if (tabs < 0)
                return;

		    localization += Tabs(tabs) + "}\n";
			
		    labels.RemoveAt(labels.Count - 1);
	    }
	    private static void End() {
		    while(tabs >= 0) {
			    Close();
		    }
		
		    tabs = 0;
		    localization.Log();
		    localization = "";
	    }
        private static void GetLocalizationFromList(string label, IEnumerable<ModType> list, bool ignoreLabel = false, bool printMaster = false) {
            IEnumerable<string> listNames = list.Select(l => l.Name);
            GetLocalizationFromList(label, listNames, ignoreLabel, printMaster);
        }
        private static void GetLocalizationFromList(string label, IEnumerable<string> list, bool ignoreLabel = false, bool printMaster = false) {
            SortedDictionary<string, string> dict = new();
            foreach (string s in list) {
                //$"{s}: {s.AddSpaces()}".Log();
				dict.Add($"{s}", $"{s.AddSpaces()}");
			}

            GetLocalizationFromDict(label, dict, ignoreLabel, printMaster);
		}
	    private static void GetLocalizationFromDict(string label, SortedDictionary<string, string> dict, bool ignoreLabel = false, bool printMaster = false) {
            ignoreLabel = ignoreLabel || label == null || label == "";
	        if (!ignoreLabel)
	    	    AddLabel(label);

            string tabString = Tabs(tabs);
		    string allLabels = string.Join(".", labels.ToArray());
            foreach (KeyValuePair<string, string> p in dict) {
                string key = allLabels + "." + p.Key;
                string s = translations.ContainsKey(key) ? translations[key].GetTranslation(culture) : key;
                //$"{key}: {s}".Log();
                if (s == key) {
                    s = p.Value;
                }

                s = CheckTabOutLocalization(s);

                localization += tabString + p.Key + ": " + (printMaster ? "" : s) + "\n";
            }

            if (!ignoreLabel)
	    	Close();
        }
        private static void GetLocalizationFromListAddToEnd(string label, IEnumerable<string> list, string addString, int tabsNum) {
            List<string> newList = ListAddToEnd(list, addString);
            GetLocalizationFromList(label, newList);
		}
        private static List<string> ListAddToEnd(IEnumerable<string> iEnumerable, string addString) {
            List<string> list = iEnumerable.ToList();
            for(int i = 0; i < list.Count; i++) {
                list[i] += addString;
			}

            return list;
		}
        private static string Tabs(int num) => num > 0 ? new string('\t', num) : "";
        public static string FillString(int num, char c) => num > 0 ? new string(c, num) : "";
        public static string FillString(int num, string s) {
            string text = "";
            for (int i = 0; i < num; i++) {
                text += s;
			}

            return text;
		}
        private static string CheckTabOutLocalization(string s) {
            if (s.Contains("'''"))
                return s;

            if (!s.Contains('\n'))
                return s;

            tabs++;
            string newString = $"\n{Tabs(tabs)}'''\n";
            int start = 0;
            int i = 0;
            for (; i < s.Length; i++) {
                if (s[i] == '\n') {
                    newString += Tabs(tabs) + s.Substring(start, i - start + 1);
                    start = i + 1;
                }
            }

            if (s[s.Length - 1] != '\n') {
                newString += Tabs(tabs) + s.Substring(start, i - start);
            }

            newString += $"\n{Tabs(tabs)}'''";
            tabs--;

            return newString;
		}
        private static void PrintListOfEnchantmentTooltips() {
		if (!printListOfEnchantmentTooltips)
			return;
	
            string tooltipList = "";
            foreach(Enchantment e in ModContent.GetContent<ModItem>().OfType<Enchantment>()) {
                tooltipList += $"\n\n{e.Name}";
                IEnumerable<string> tooltips = e.GenerateFullTooltip().Select(t => t.Item1);
                foreach(string tooltip in tooltips) {
                    tooltipList += $"\n{tooltip}";
                }
			}

            tooltipList.Log();
        }
        private static void PrintContributorsList() {
            if (!printListOfContributors)
                return;

            if (contributorsData.Count <= 0)
                return;
            
            //New dictionary with artist names as the key
            SortedDictionary<string, List<string>> artistCredits = new SortedDictionary<string, List<string>>();
            foreach (string key in contributorsData.Keys) {
                string artistName = contributorsData[key].Artist;
                if (artistName != null) {
                    if (artistCredits.ContainsKey(artistName)) {
                        artistCredits[artistName].Add(key);
                    }
                    else {
                        artistCredits.Add(artistName, new List<string>() { key });
                    }
                }
            }

            //Create and print the GitHub Artist credits.
            string artistsMessage = "";
            foreach (string artistName in artistCredits.Keys) {
                artistsMessage += $"\n{artistName}: ";
                if (contributorLinks.ContainsKey(artistName))
                    artistsMessage += contributorLinks[artistName];

                artistsMessage += "\n\n";
                foreach (string texture in artistCredits[artistName]) {
                    artistsMessage += $"![{texture.GetFileName('/')}]({texture.RemoveFirstFolder('/', false)}.png)\n";
                }
            }
            artistsMessage.Log();

            namesAddedToContributorDictionary.Clear();
            contributorsData.Clear();
        }
        private static void PrintEnchantmentDrops() {
		if (!printEnchantmentDrops)
			return;
            string log = "\n";
            foreach(KeyValuePair<int, List<(float, List<WeightedPair>)>> npc in npcEnchantmentDrops) {
                string name = ContentSamples.NpcsByNetId[npc.Key].TypeName;
                foreach((float, List<WeightedPair>) enchantmentGroup in npc.Value) {
                    float total = 0f;
                    foreach(WeightedPair pair in enchantmentGroup.Item2) {
                        total += pair.Weight;
                    }

                    foreach(WeightedPair pair in enchantmentGroup.Item2) {
                        Item sampleItem = ContentSamples.ItemsByType[pair.ID];
                        if (sampleItem.ModItem is not Enchantment enchantment)
                            continue;

                        log += $"\n{name} ({npc.Key}),";
                        float chance = enchantmentGroup.Item1 * pair.Weight / total;

                        log += $"{enchantment.EnchantmentTypeName.AddSpaces()},{chance.PercentString()}";
                    }
                }
			}

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
		}
        private static Dictionary<int, List<RecipeData>> createItemRecipes;
        private static Dictionary<int, List<RecipeData>> recipesUsedIn;
		private static Dictionary<int, Dictionary<int, ItemDropRule>> drops;
		private static int min;
		private static int max;
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
		//private static Dictionary<int, List<RecipeData>> recipesReverseRecipes;
        private static void PrintWiki() {
            if (!printWiki)
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

            foreach(WebPage webPage in webPages) {
                wiki += webPage.ToString();
			}

            wiki.Log();
        }
		private void AddContainments(List<WebPage> webPages, IEnumerable<ContainmentItem> containmentItems, IEnumerable<Enchantment> enchantments) {
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
		
                Containments.AddSubHeading(1, subHeading);
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
				containmentPages.Add(containmentPage);
				webPages.Add(containmentPage);
            }

            webPages.Add(Containments);
		}
		private void AddEnchantments(List<WebPage> webPages, IEnumerable<Enchantment> enchantments) {
			WebPage Enchantments = new("Enchantments");
			Enchantments.AddParagraph();
			Enchantments.NewLine();
			
			
		}
		private void AddEssence(List<WebPage> webPages, IEnumerable<EnchantmentEssence> enchantmentEssence) {
			WebPage Essence = new("Enchantment Essence");
			Essence.AddParagraph();
			Essence.NewLine();
			
			
		}
        private static void GetRecpies(IEnumerable<ModItem> modItems) {
            createItemRecipes = new();
            recipesUsedIn = new();

            for(int i = 0; i < Recipe.numRecipes; i++) {
                Recipe recipe = Main.recipe[i];
                int type = recipe.createItem.type;
                if (type >= min && type <= max) {
                    RecipeData recipeData = new(recipe);
                    if (createItemRecipes.ContainsKey(type)) {
                        bool recipeAdded = false;
                        
                        foreach(RecipeData createItemRecipe in createItemRecipes[type]) {
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

                foreach(Item item in recipe.requiredItem) {
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
		private void GetDrops() {
			drops = new();
			//Dictionary<int, Dictionary<int, ItemDropRule>> drops
			for(int npcType = 1; npcType < NPCLoader.Count; npcType++) {
				foreach(ItemDropRule dropRule in DropLoader.GetDropRules(npcType)) {
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
			}
		}
        private static List<Item> GetAllOtherCraftedItems(Item item, List<Item> consumedItems) => consumedItems.SelectMany(c => GetOtherCraftedItems(item, c)).ToList();
        private static List<Item> GetOtherCraftedItems(Item item, Item consumedItem) => CraftingEnchantments.GetOtherCraftedItems(item, consumedItem).Select(p => new Item(p.Key, p.Value)).ToList();
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
            public bool SameExceptOneRequiredItem(RecipeData other) {
                if (!requiredTile.ExactSame(other.requiredTile))
                    return false;

                if (!requiredItem.SameCommonItems(other.requiredItem))
                    return false;

                return true;
            }
            public bool TryCondenseRecipe(RecipeData other) {
                //if (createItem.NumberCommonItems(other.createItem) == 0)
                //    return false;

                if (!createItem.SameCommonItems(other.createItem))
                    return false;

                bool added = false;
                if (SameExceptOneTile(other)) {
                    if (requiredTile.TryAdd(other.requiredTile))
                        added = true;
				}

                if (SameExceptOneRequiredItem(other)) {
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
            public void AddLink(string s, string text = null) => Add(new Link(s, text));
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
                return Text;
			}

        }
        private class Link {
            public string Text { private set; get; }
            public Link(string s, string text = null) {
                Text = s.ToLink(text);
            }
            public override string ToString() {
                return Text;
            }
        }
        private class BulletedList
		{
            public object[] Elements { private set; get; }
            private bool png;
            private bool _links;
            public BulletedList(bool png = false, bool links = false, params object[] elements) {
                Elements = elements;
                _links = links;			}
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
                            while(k < elementsCount) {
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
        private class ItemInfo {
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
				List<List<string>> info = new() {
					{ "Type", $"{modItem.GetType().Name}" }
					{ "Tooltip", $"'{Item.Tooltip}'" },//italics?
					{ "Rarity", $"{item.rare}" },//Need to edit to rarity name and color
					{ "Buy", $"{item.value.GetCoinsPNG()}" },
					{ "Sell", $"{(item.value / 5).GetCoinsPNG()}" },
					{ "Research", $"{item.research} required" }
				}
				
				string label = $"{item.Name}<br/>{item.ToItemPNG(displayName: false)}<br/>Statistics";
				webpage.AddTable(label, info);
			}
			public void AddDrops(WebPage webpage) {
				int type = Item.type;
				if (!drops.ContainsKey(type))
					return;
				
				List<List<string>> allDropInfo = new() { "Entity", "Qty.", "Rate"};
				foreach(int npcType in drops[type]) {
					int minDropped = drops[type][npcType].min;
					int maxDropped = drops[type][npcType].max;
					List<string> dropInfo = new() { $"{npcType.ToNpcPNG()}", (minDropped != maxDropped ? $"{minDropped}-{maxDropped}" : $"{minDropped}"), $"{drops[type][npcType].dropRate}%" };//Make vanilla link option to vanilla wiki
					allDropInfo.Add(dropInfo);
				}
				
				//foreach chest
				//foreach crate
				
				string label = $"Obtained From";
				webpage.AddTable(label, allDropInfo, true);
			}
            public void AddInfo(WebPage webpage) {
				if (modItem is ISoldByWitch soldByWitch && soldByWitch.SellCondition != SellCopndition.Never) {
					int sellPrice = (int)((float)item.value * soldByWitch.SellPriceModifier);
					string sellPriceString = sellPrice.GetCoinsPNG();
					string sellText = $"Sold by the Witch for {sellPriceString}. Can only appear in the shop if this condition is met: {soldByWitch.SellCondition}";
					webPage.AddParagraph(sellText);
				}
            }
            public IEnumerable<IEnumerable<string>> RecipesCreateItem => GetRecipes(createItem: true);
            public IEnumerable<IEnumerable<string>> RecipesUsedIn => GetRecipes(usedIn: true);
	    	public IEnumerable<IEnumerable<string>> RecipesReverseRecipes => GetRecipes(reverseRecipes: true);
	    	public void AddRecipes(WebPage webpage) {
	    		webpage.AddTable(itemInfo.RecipesCreateItem, "Recipes", true);
                webpage.AddTable(itemInfo.RecipesUsedIn, "Used in", true);
				webpage.AddTable(itemInfo.RecipesReverseRecipes, "Used in (Reverse Crafting Recipes)", true);
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
		    
				if (reverseRecipes && TryAddRecipeData(myRecipes, recipesReverseRecipes, reverseRecipes))
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
                        foreach(RecipeData myRecipe in myRecipeData) {
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
                    for(int i = 0; i < CommonList.Count(); i++) {
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
                if (SameCommonItems(other)) {
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

    public static class LogModSystemStaticMethods
    {
        private static Dictionary<int, int> itemsFromTiles;
        public static string ToLink(this string s, string text = null) => $"[[{s.AddSpaces()}{(text != null ? $"|{text}" : "")}]]";
        public static string ToFile(this string s) => $"[[File:{s.RemoveSpaces()}.png]]";
        public static string ToItemPNG(this Item item, int num = 1, bool link = false, bool displayName = true) {
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
            return $"{(!displayName && stack > 1 ? $"{stack}" : "")}{name}{(link ? " " + item.Name.ToLink() : displayName ? " " + item.Name : "")}{(displayName && stack > 1 ? $" ({stack})" : "")}";
        }
        public static string ToItemPNG(this int type, int num = 1, bool link = false, bool displayName = true) {
            return new Item(type).ToItemPNG(num, link, displayName);
        }
        public static string ToItemPNG(this string s, int num = 1, bool link = false, bool displayName = true) {
            return $"{s.ToFile()} {(link ? s.ToLink() : displayName ? s : "")}";
		}
		public static string ToNPCPNG(this int npcType, bool link = false, bool displayName = true) {
			string name;
			if (npcType < NPCID.Count) {
				name = $"NPC_{npcType}";
			}
			else {
				NPC npc = new(npcType);
				ModNPC modNPC = ContentSamples.NPCByType[npcType];
				if (modNPC == null) {
					name = npc.Name;
				}
				else {
					name = modNPC.Name;
				}
				
				return $"{name.ToFile()}{(link ? " " + name.ToLink() : displayName ? " " + name : "")}";
			}
		}
		public static string GetCoinsPNG(this int sellPrice) {
			int coinType = ItemID.PlatinumCoin;
            int coinValue = 1000000;
			string text = "";
            while (coins > 0) {
                int numCoinsToSpawn = coins / coinValue;
                if (numCoinsToSpawn > 0)
					text += coinType.ToItemPNG(numCoinsToSpawn) + " ";

                coins %= coinValue;
                coinType--;
                coinValue /= 100;
            }
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

            for(int i = 0; i < ItemLoader.ItemCount; i++) {
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
            foreach(IEnumerable<string> list in lists) {
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

            foreach(int type in list1.Select(i => i.type)) {
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

