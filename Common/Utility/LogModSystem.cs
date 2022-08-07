using System;
using System.Collections.Generic;
using Terraria.ModLoader;
using WeaponEnchantments.Items;
using static WeaponEnchantments.Common.Utility.LogModSystem.GetItemDictModeID;

namespace WeaponEnchantments.Common.Utility
{
    public class LogModSystem : ModSystem {
        public static bool printListOfContributors = false;
        public static bool printListOfEnchantmentTooltips = false;
        public static bool printLocalization = false;

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


        //Only used to print the full list of enchantment tooltips in WEPlayer OnEnterWorld()  (Normally commented out there)
        public static string listOfAllEnchantmentTooltips = "";

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

        public static void UpdateEnchantmentLocalization(Enchantment enchantment) {
            enchantmentsLocalization.Add(enchantment.EnchantmentTypeName);
		}
        public static void PrintLocalization() {
            List<List<string>> itemsLists = new List<List<string>>() {
                ListAddToEnd(Containment.sizes, "Containment"),
                ListAddToEnd(EnchantingTableItem.enchantingTableNames, "EnchantingTable"),
                //ListAddToEnd(EnchantingRarity.tierNames, "EnchantmentEssence")
            };

            int tabs = 3;
            string localization = "\n" +
                "Mods: {\n" +
                "\tWeaponEnchantments: {\n" +
				"\t\tItemName: {\n";
            foreach(List<string> itemList in itemsLists) {
                localization += GetLocalizationFromList(null, itemList, tabs, true);
            }
            localization += "\t\t}\n" +
                "\t\tEnchantmentEssence: Enchantment Essence\n" +
				"\t\tEnchantment: Enchantment\n";

            localization += GetLocalizationFromList("EnchantmentTypeNames", enchantmentsLocalization, tabs);
            //localization += GetLocalizationFromListAddToEnd("Containments", Containment.sizes, "Containment", tabs);
            localization += GetLocalizationFromList("TierNames", EnchantingRarity.tierNames, tabs);
            localization += GetLocalizationFromList("DisplayTierNames", EnchantingRarity.displayTierNames, tabs);
            //localization += GetLocalizationFromListAddToEnd("EnchantingTables", EnchantingTableItem.enchantingTableNames, "EnchantingTable", tabs);
            //localization += GetLocalizationFromListAddToEnd("EnchantmentEssence", EnchantingRarity.tierNames, "EnchantmentEssence", tabs);
            localization +=
                "\t}\n" +
				"}";

            localization.Log();
        }
        private static string GetLocalizationFromList(string label, IEnumerable<string> list, int tabsNum, bool ignoreLabel = false) {
            
            string localization = ignoreLabel ? "" : Tabs(tabsNum -1) + label + ": {\n";
            string tabs = Tabs(tabsNum);
            foreach (string s in list) {
                localization += tabs + s + $": {s.AddSpaces()}\n";
            }

            localization += ignoreLabel ? "" : Tabs(tabsNum - 1) + "}\n";

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
        public override void OnWorldLoad() {
            if (printListOfContributors) {
                //Enchantment tooltips
                if (printListOfEnchantmentTooltips && listOfAllEnchantmentTooltips != "") {
                    listOfAllEnchantmentTooltips.Log();
                    listOfAllEnchantmentTooltips = "";
                }

                //Contributors  change to give exact file location when added to contributor.
                PrintContributorsList();
            }

            if (printLocalization)
                PrintLocalization();
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

	}
}
