using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Items;
using static WeaponEnchantments.Common.Utility.LogModSystem.GetItemDictModeID;

namespace WeaponEnchantments.Common.Utility
{
    public class LogModSystem : ModSystem {
        public static bool printListOfContributors = false;
        public static bool printListOfEnchantmentTooltips => WEMod.clientConfig.PrintEnchantmentTooltips;
        public static bool printLocalization = true;
        public static bool printListForDocumentConversion = false;
        public static bool printEnchantmentDrops => WEMod.clientConfig.PrintEnchantmentDrops;

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

        //Only used to print the full list of enchantment tooltips in WEPlayer OnEnterWorld()  (Normally commented out there)
        //public static string listOfAllEnchantmentTooltips = "";

        //Requires an input type to have properties: Texture
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
        public static void PrintLocalization() {
	    Start();
	    
            IEnumerable<ModItem> modItems = ModContent.GetInstance<WEMod>().GetContent<ModItem>();
            var modItemLists = modItems
	    	.GroupBy(mi => mi is Enchantment ? mi.GetType().BaseType.BaseType.Name : mi.GetType().BaseType.Name)
		.Select(mi => new { Key = mi.GetType().BaseType.Name, ModItemList = mi})
		.OrderBy(group => group.Key);
            
            foreach (var list in modItemLists) {
                GetLocalizationFromList("ItemName",, list.ModItemList, tabs);
            }
	    
	    FromLocalizationData();
	    
	    End();
        }
	private static void FromLocalizationData() {
		SortedDictionary<string, SData> all = LocalizationData.All;
		GetFromSDataDict(all);
	}
	private static void GetFromSDataDict(SortedDictionary<string, SData> dict) {
		foreach(KeyValuePair<string, SData> pair in dict) P
			AddLabel(pair.Key);
			if (LocalizationData.autoFill.ContainsKey(pair.Key))
				AutoFill(pair);
			
			GetFromSData(pair.Value);
			Close();
		{
	}
	private static void GetFromSData(SData d) {
		if (d.Values != null)
			GetLocalizationFromList(d.Values);
		
		if (d.Dict != null)
			GetLocalizationFromDict(d.Dict);
		
		if (d.Children != null)
			GetFromSDataDict(d.Children);
	}
	private static void AutoFill(KeyValuePair<string, SData> pair) {
		List<string> list = Assembly.GetExecutingAssembly().GetTypes()
			.OfType(pair.Key)
			.Where(t => !t.isAbstract)
			.Select(t => t.Name)
			.ToList();
		SortedDictionary<string, string> dict = pair.Value.Dict.
		foreach(string s in list) {
			if(!dict.ContainsKey(s))
				dict.Add(s, s.AddSpaces());
		}
	}
	private static void PrintAllLocalization() {
		if (!printLocalization)
			return;
		
		foreach(int i in Enum.GetValues(CultureName).Cast<CultureName>().Where(n => n != CultureName.Unknown).Select(n => (int)n)) {
			LanguageManager.Instance.SetLanguage(i);
			PrintLocalization();
		}
		
		LanguageManager.Instance.SetLanguage((int)CultureName.English);
	}
	private static string AddLabel(string label) {
		localization += Tabs(tabs) + label + ": {\n";
		tabs++;
		labels.Add(label);
	}
	private static void Start() {
		labels = new();
		AddLabel("Mods");
		AddLabel("WeaponEnchantments");
	}
	private static string Close(bool newLine = true) {
		tabs--;
		localization += Tabs(tabs) + "}\n";
		if (newLine)
			localization += "\n";
			
		labels.RemoveAt(labels.Count - 1);
	}
	private static string End() {
		while(tabs >= 0) {
			Close(false);
		}
		
		tabs = 0;
		localization.log();
		localization = "";
	}
        private static void GetLocalizationFromList(string label, IEnumerable<ModType> list, bool ignoreLabel = false, bool printMaster = false) {
            IEnumerable<string> listNames = list.Select(l => l.Name);
            GetLocalizationFromList(label, listNames, tabs, ignoreLabel, printMaster);
        }
        private static void GetLocalizationFromList(string label, IEnumerable<string> list, bool ignoreLabel = false, bool printMaster = false) {
	    SortedDictionary<string, string> dict = new();
	    foreach (string s in list) {
	    	dict.Add(s, s.AddSpaces());
	    }
	    
	    GetLocalizationFromDict(label, dict, ignoreLabel, printMaster);
        }
	private static void GetLocalizationFromDict(string label, SortedDictionary<string, string> dict, bool ignoreLabel = false, bool printMaster = false) {
            ignoreLabel = ignoreLabel || label == null || label == "";
	    if (!ignoreLabel)
	    	AddLabel(label);
            string tabString = Tabs(tabs);
		string key = String.Join(".", labels) += "." + p.Key;
		string s = Language.GetTextValue(key);
		if (s == key)
			s = p[p.Key];
		
            foreach (KeyValuePair p in dict) {
                localization += "\n" + tabString + p.Key + ": " + (printMaster ? "" : s);
            }

            if (!ignoreLabel)
	    	Close();
        }
        private static string GetLocalizationFromListAddToEnd(string label, IEnumerable<string> list, string addString, int tabsNum) {
            List<string> newList = ListAddToEnd(list, addString);
            GetLocalizationFromList(label, newList, tabsNum);
		}

        private static List<string> ListAddToEnd(IEnumerable<string> iEnumerable, string addString) {
            List<string> list = iEnumerable.ToList();
            for(int i = 0; i < list.Count; i++) {
                list[i] += addString;
			}

            return list;
		}

        private static string Tabs(int num) => new string('\t', num);
        public override void OnWorldLoad() {
            PrintListOfEnchantmentTooltips();

            //Contributors  change to give exact file location when added to contributor.
            PrintContributorsList();

            PrintAllLocalization();

	    PrintEnchantmentDrops();
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
    }
}
