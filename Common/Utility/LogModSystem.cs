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
        public static bool printLocalization = false;
        public static bool printLocalizationMaster = false;
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
	public static string localization = "";
	public static int tabs = 0;

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

        //public static void UpdateEnchantmentLocalization(Enchantment enchantment) {
        //    enchantmentsLocalization.Add(enchantment.EnchantmentTypeName);
        //}
        public static void PrintLocalization() {
            /*List<List<string>> itemsLists = new List<List<string>>() {
                ListAddToEnd(Containment.sizes, "Containment"),
                ListAddToEnd(EnchantingTableItem.enchantingTableNames, "EnchantingTable"),
                ListAddToEnd(EnchantingRarity.tierNames, "EnchantmentEssence")
            };*/

            //int tabs = 3;
            string localization = "\n" +
                "Mods: {\n" +
                "\tWeaponEnchantments: {\n" +
				"\t\tItemName: {";
            IEnumerable<ModItem> modItems = ModContent.GetInstance<WEMod>().GetContent<ModItem>();
            var modItemLists = modItems.GroupBy(mi => mi is Enchantment ? mi.GetType().BaseType.BaseType.Name : mi.GetType().BaseType.Name).Select(mi => new { Key = mi.GetType().BaseType.Name, ModItemList = mi}).OrderBy(group => group.Key);
            //IEnumerable <IEnumerable<ModItem>> modItemLists = modItems.GroupBy(mi => mi.GetType().BaseType.Name, mi => new {Key = mi.GetType().BaseType.Name, ModItem = mi}).OrderBy(obj => obj.Key);
            foreach (var list in modItemLists) {
                localization += GetLocalizationFromList(null, list.ModItemList, tabs);
            }
            /*foreach(List<string> itemList in itemsLists) {
                localization += GetLocalizationFromList(null, itemList, tabs, true);
            }
            localization += "\t\t}\n" +
                "\t\tEnchantmentEssence: Enchantment Essence\n" +
				"\t\tEnchantment: Enchantment\n";*/

            //localization += GetLocalizationFromList("EnchantmentTypeNames", enchantmentsLocalization, tabs);
            //localization += GetLocalizationFromListAddToEnd("Containments", Containment.sizes, "Containment", tabs);
            //localization += GetLocalizationFromList("TierNames", EnchantingRarity.tierNames, tabs);
            //localization += GetLocalizationFromList("DisplayTierNames", EnchantingRarity.displayTierNames, tabs);
            //localization += GetLocalizationFromListAddToEnd("EnchantingTables", EnchantingTableItem.enchantingTableNames, "EnchantingTable", tabs);
            //localization += GetLocalizationFromListAddToEnd("EnchantmentEssence", EnchantingRarity.tierNames, "EnchantmentEssence", tabs);
	    localization += "\n" + 
	    	"\t\t}\n" +
	    	"\t\tTooltips: {\n"
		SortedDictionary<string, string> combined = new();
	    foreach(EnchantmentEffect effect in ) {//Check not abstract
	    	string name = effect.GetType().Name;
		SortedDictionary<string, string> dict = LocalizationData.AllData["Tooltip"].Children["EnchantmentEffects"].Dict;
	    	if(dict.ContainsKey(name)) {
			combined.Add(name, dict[name]);
		}
		else {
			combined.Add(name, name.AddSpaces());
		}
	    }
	    localization += GetLocalizationFromDict("EnchantmentEffects", combined, tabs);
            localization += "\n" +
                "\t\t}\n" +
                "\t}\n" +
				"}";

			if (printLocalizationMaster) {
                localization += "\n\n";
		localization += AddLabel("Mods", ref tabs);
		localization += AddLabel("WeaponEnchantments", ref tabs);
                //localization += "\n" +
                //    "Mods: {\n" +
                //    "\tWeaponEnchantments: {\n" +
                //    "\t\tItemName: {";
                foreach (var list in modItemLists) {
                    localization += GetLocalizationFromList("ItemName", list.ModItemList, tabs, true, true);
                }
		localization += CloseAll();
                //localization += "\n" +
                //    "\t\t}\n" +
                //    "\t}\n" +
                //    "}";
            }

			if (printListForDocumentConversion) {
                localization += "\n\n";
                foreach (var list in modItemLists) {
                    foreach(ModItem modItem in list.ModItemList) {
                        localization += modItem.Name.AddSpaces() + "\n";
					}
                }
            }

            localization.Log();
        }
	private static string AddLabel(string label) {
		localization += Tabs(tabs) + label + ": {\n";
		tabs++;
	}
	private static void Start() {
		AddLabel("Mods");
		AddLabel("WeaponEnchantments");
	}
	private static string Close() {
		tabs--;
		localization += Tabs(tabs) + "}\n";
	}
	private static string CloseAll() {
		while(tabs >= 0) {
			Close();
		}
		
		tabs = 0;
		localization.log();
		localization = "";
	}
        private static string GetLocalizationFromList(string label, IEnumerable<ModType> list, int tabsNum, bool ignoreLabel = false, bool printMaster = false) {
            IEnumerable<string> listNames = list.Select(l => l.Name);
            return GetLocalizationFromList(label, listNames, tabsNum, ignoreLabel, printMaster);
        }
        private static string GetLocalizationFromList(string label, IEnumerable<string> list, int tabsNum, bool ignoreLabel = false, bool printMaster = false) {
            ignoreLabel = ignoreLabel || label == null || label == "";
            string localization = ignoreLabel ? "" : Tabs(tabsNum -1) + label + "\n: {";
            string tabs = Tabs(tabsNum);
            foreach (string s in list) {
                localization += "\n" + tabs + s + ": " + (printMaster ? "" : s.AddSpaces());
            }

            localization += ignoreLabel ? "" : "\n" + Tabs(tabsNum - 1) + "}\n";

            return localization;
        }
	private static string GetLocalizationFromDict(string label, SortedDictionary<string, string> dict, int tabsNum, bool ignoreLabel = false, bool printMaster = false) {
            ignoreLabel = ignoreLabel || label == null || label == "";
            string localization = ignoreLabel ? "" : Tabs(tabsNum -1) + label + "\n: {";
            string tabs = Tabs(tabsNum);
            foreach (KeyValuePair p in dict) {
                localization += "\n" + tabs + p.Key + ": " + (printMaster ? "" : p[p.Key]);
            }

            localization += ignoreLabel ? "" : "\n" + Tabs(tabsNum - 1) + "}\n";

            return localization;
        }
        private static string GetLocalizationFromListAddToEnd(string label, IEnumerable<string> iEnumerable, string addString, int tabsNum) {
            List<string> list = ListAddToEnd(iEnumerable, addString);
            return GetLocalizationFromList(label, list, tabsNum);
		}

        private static List<string> ListAddToEnd(IEnumerable<string> iEnumerable, string addString) {
            List<string> list = new List<string>(iEnumerable);
            for(int i = 0; i < list.Count; i++) {
                list[i] += addString;
			}

            return list;
		}

        private static string Tabs(int num) => new string('\t', num);
        public override void OnWorldLoad() {//Enchantment tooltips
            if (printListOfEnchantmentTooltips) {
                PrintListOfEnchantmentTooltips();
            }

            //Contributors  change to give exact file location when added to contributor.
            if (printListOfContributors)
                PrintContributorsList();

            if (printLocalization)
                PrintLocalization();

            if (printEnchantmentDrops)
                PrintEnchantmentDrops();
        }

        private static void PrintListOfEnchantmentTooltips() {
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
