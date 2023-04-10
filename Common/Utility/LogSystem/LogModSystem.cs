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
using Terraria.GameContent.Creative;
using Terraria.GameContent.ItemDropRules;
using WeaponEnchantments.Common.Utility.LogSystem;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using static WeaponEnchantments.Common.Globals.EnchantedItemStaticMethods;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text.RegularExpressions;

namespace WeaponEnchantments.Common.Utility
{
    public class LogModSystem : ModSystem {
        public static bool printListOfContributors = false;
        public static bool printListOfEnchantmentTooltips => WEMod.clientConfig.PrintEnchantmentTooltips;
        public static bool printLocalization => WEMod.clientConfig.PrintLocalizationLists && !Debugger.IsAttached;
        public static readonly bool printListForDocumentConversion = false;
        public static readonly bool zzzLocalizationForTesting = false;
        //public static bool printLocalizationKeysAndValues => printLocalizationKeysAndValues && culture == (int)CultureName.English;
        public static bool printLocalizationKeysAndValues => WEMod.clientConfig.PrintLocalizationLists && Debugger.IsAttached;
        private static int localizationValuesCharacterCount = 0;
        public static bool printEnchantmentDrops => WEMod.clientConfig.PrintEnchantmentDrops;
        public static readonly bool printWiki = WEMod.serverConfig.PrintWikiInfo;
        public static readonly bool printNPCIDSwitch = false;

        public static class GetItemDictModeID {
            public static byte Weapon = 0;
            public static byte Armor = 1;
            public static byte Accessory = 2;
        }
        public static Dictionary<int, bool> PrintListOfItems = new Dictionary<int, bool>() {
            { Weapon, WEMod.clientConfig.PrintWeaponInfusionPowers },
            { Armor, false },
            { Accessory, false }
        };

        //Only used to print the full list of contributors.
        public static Dictionary<string, string> contributorLinks = new Dictionary<string, string>() {
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
        public static SortedDictionary<int, List<(float, List<DropData>)>> npcEnchantmentDrops = new();
	    private static string localization = "";
        private static string localizationValues = "";
        private static string localizationKeys = "";
	    private static int tabs = 0;
	    private static List<string> labels;
        private static Dictionary<string, LocalizedText> translations;
        private static int culture;
		private static bool numPad0 = false;
		private static bool numPad1 = false;
        private static bool numPad3 = false;
		private static bool numPad4 = false;
		private static bool numPad6 = false;
		private static bool numPad8 = false;

		//Only used to print the full list of enchantment tooltips in WEPlayer OnEnterWorld()  (Normally commented out there)
		//public static string listOfAllEnchantmentTooltips = "";

		//Requires an input type to have properties: Texture
		public override void OnWorldLoad() {
            PrintListOfEnchantmentTooltips();

            //Contributors  change to give exact file location when added to contributor.
            PrintContributorsList();

            PrintAllLocalization();

            Wiki.PrintWiki();

            PrintNPCIDSwitch();
        }
		public override void PostDrawInterface(SpriteBatch spriteBatch) {
            if (WEMod.clientConfig.EnableSwappingWeapons) {
				bool newNumPad1 = Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.NumPad1);
				bool newNumPad3 = Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.NumPad3);
				bool newNumPad4 = Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.NumPad4);
				bool newNumPad6 = Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.NumPad6);
				bool previousWeapon = newNumPad1 && !numPad1;
				bool nextWeapon = newNumPad3 && !numPad3;
				bool previousModdedWeapon = newNumPad4 && !numPad4;
				bool nextModdedWeapon = newNumPad6 && !numPad6;
				bool tryingToSwapWeapon = previousWeapon || nextWeapon || previousModdedWeapon || nextModdedWeapon;
				if (tryingToSwapWeapon) {
					bool skipVanilla = previousModdedWeapon || nextModdedWeapon;
					bool increasing = nextWeapon || nextModdedWeapon;
					Item lastHeldItem = Main.LocalPlayer.HeldItem;
					//Only allow unmodified weapons to be replaced
					int i;
					if (lastHeldItem.NullOrAir() || lastHeldItem.TryGetEnchantedItem(out EnchantedItem enchantedItem) && !enchantedItem.Modified) {
						i = Main.LocalPlayer.selectedItem;
					}
					else {
						for (i = 0; i < 40; i++) {
							if (Main.LocalPlayer.inventory[i].NullOrAir())
								break;
						}
					}

					if (i < 40) {
						Item newItem = NextWeapon(lastHeldItem.type, increasing, skipVanilla);
						if (!newItem.NullOrAir()) {
							Main.LocalPlayer.inventory[i] = newItem;
							Main.LocalPlayer.selectedItem = i;
							Main.NewText(newItem.S());
						}
						else {
							Main.NewText("newItem was air.");
						}
					}
				}

				numPad1 = newNumPad1;
				numPad3 = newNumPad3;
				numPad4 = newNumPad4;
				numPad6 = newNumPad6;
			}

			if (WEMod.clientConfig.LogDummyDPS) {
                if (DummyNPC.StopDPSCheck)
                    DummyNPC.StartDPSCheck = false;

				bool newNumPad0 = Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.NumPad0);
				bool newNumPad8 = Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.NumPad8);
				if (newNumPad0 && !numPad0) {
                    if (!DummyNPC.StartDPSCheck) {
						string heldItemName = Main.LocalPlayer.HeldItem.Name;
						if (DummyNPC.allTotalItemDamages.ContainsKey(heldItemName))
							DummyNPC.allTotalItemDamages.Remove(heldItemName);

						DummyNPC.StopDPSCheck = false;
						DummyNPC.StartDPSCheck = true;
					}
                    else {
						DummyNPC.StopDPSCheck = true;
					}
				}

                if (newNumPad8 && !numPad8) {
                    string msg = "\n" + DummyNPC.allTotalItemDamages.Select(pair => $"{pair.Key}, {pair.Value.ToString("F5")}").JoinList("\n");
                    msg.LogSimple();
				}

				numPad0 = newNumPad0;
                numPad8= newNumPad8;
			}
		}
        private static Item NextWeapon(int type, bool increasing, bool skipVanilla) {
            int[] ignoreItemTypes = new int[] {
                ItemID.Count
            };

            int[] itemTypes = new int[ItemLoader.ItemCount];
            for (int i = 0; i < itemTypes.Length; i++) {
                itemTypes[i] = i;
            }

            int[] weaponTypes = itemTypes
				.Select(type => ContentSamples.ItemsByType[type])
                .Where(item => IsWeaponItem(item) && !ignoreItemTypes.Contains(item.type))
                .Select(item => item.TryGetEnchantedWeapon(out EnchantedWeapon enchantedWeapon) ? enchantedWeapon : null)
                .Where(enchantedWeapon => enchantedWeapon != null)
                .OrderBy(enchantedWeapon => enchantedWeapon.GetWeaponInfusionPower())
                .Select(enchantedWeapon => enchantedWeapon.Item.type)
                .ToArray();

			int newType = type;
			int index;
			int count = weaponTypes.Length;
			for (index = 0; index < count; index++) {
				int weaponType = weaponTypes[index];
				if (weaponType == type)
					break;
			}

			//Handle i == count

			int startingIndex = index;
			bool gettingType = true;
			Item sampleItem = new();
			while (gettingType) {
				index += increasing ? 1 : -1;
				if (index < 0) {
					index = count - 1;
				}
				else if (index >= count) {
					index = 0;
				}
				else if (index == startingIndex) {
					gettingType = false;
					break;
				}

				newType = weaponTypes[index];

				sampleItem = ContentSamples.ItemsByType[newType];
				if (!skipVanilla || sampleItem.ModItem != null) {
					gettingType = false;
					break;
				}
			}

			return index != startingIndex && newType != type ? new Item(newType, sampleItem.maxStack) : new Item(ItemID.None);
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
            if (!printLocalization && !printLocalizationKeysAndValues)
                return;

            LocalizationData.ChangedData = new();
            LocalizationData.RenamedFullKeys = new();
            Mod mod = ModContent.GetInstance<WEMod>();
            TmodFile file = (TmodFile)typeof(Mod).GetProperty("File", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(mod);
            translations = new();
            Autoload(file);
            
            foreach (int i in Enum.GetValues(typeof(CultureName)).Cast<CultureName>().Where(n => n != CultureName.Unknown).Select(n => (int)n)) {
                PrintLocalization((CultureName)i);
            }
        }
        private static void Autoload(TmodFile file) {
            var modTranslationDictionary = new Dictionary<string, LocalizedText>();

            AutoloadTranslations(file, modTranslationDictionary);

            foreach (LocalizedText value in modTranslationDictionary.Values) {
                AddTranslation(value);
            }
        }
        public static void AddTranslation(LocalizedText translation) => translations.Add(translation.Key, translation);
        private static void AutoloadTranslations(TmodFile file, Dictionary<string, LocalizedText> modTranslationDictionary) {
            
            foreach (var translationFile in file.Where(entry => Path.GetExtension(entry.Name) == ".hjson")) {
                using var stream = file.GetStream(translationFile);
                using var streamReader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);

                string translationFileContents = streamReader.ReadToEnd();

                var culture = GameCulture.FromName(translationFile.Name);

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
                    if (!modTranslationDictionary.TryGetValue(effectiveKey, out LocalizedText mt)) {
                        // removing instances of .$parentVal is an easy way to make this special key assign its value
                        //  to the parent key instead (needed for some cases of .lang -> .hjson auto-conversion)
                        modTranslationDictionary[effectiveKey] = mt = Language.GetOrRegister(effectiveKey);
                    }

                    mt.AddTranslation(culture, value);
                }
            }
        }
        public static void PrintLocalization(CultureName cultureName) {
            Start(cultureName);
	        
	        AddLabel(L_ID1.ItemName.ToString());
            Mod weMod = ModContent.GetInstance<WEMod>();
            IEnumerable<ModItem> modItems = weMod.GetContent<ModItem>();
	        List<string> enchantmentNames = new();
	        foreach (Enchantment enchantment in modItems.OfType<Enchantment>()) {
	    	    enchantmentNames.Add(enchantment.Name);
	        }
            
            enchantmentNames.Sort();
	        GetLocalizationFromList(null, enchantmentNames);

            Dictionary<string, List<ModItem>> modItemLists = modItems
                .Where(mi => mi is not Enchantment)
                .GroupBy(mi => mi.TypeBeforeModItem().Name)
                .ToDictionary(g => g.Key, g => g.ToList());
            
            foreach (KeyValuePair<string, List<ModItem>> pair in modItemLists) {
                GetLocalizationFromList(null, pair.Value);
            }

            Close();

            GetLocalizationFromList(L_ID1.BuffName.ToString(), weMod.GetContent<ModBuff>().Select(b => b.Name));

            FromLocalizationData();
	    
	        End();
        }
	    private static void FromLocalizationData() => GetFromSDataDict(LocalizationData.All);
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
            string tabsString = $"\n{Tabs(tabs)}{label}: {"{"}";
            if (printLocalization)
                localization += tabsString;

            if (printLocalizationKeysAndValues)
                localizationKeys += tabsString;

            tabs++;
		    labels.Add(label);
	    }
	    private static void Start(CultureName cultureName) {
            culture = (int)cultureName;
            if (printLocalization) {
                string label = $"\n\n{cultureName}\n#{LocalizationData.LocalizationComments[cultureName]}";
                localization += label;
            }
            
            if (printLocalizationKeysAndValues) {
                localizationValuesCharacterCount = 0;
                string keyLabel = $"#{LocalizationData.LocalizationComments[cultureName]}";
                localizationKeys += keyLabel;

                string valueLabel = "";
                localizationValues += valueLabel;
            }
            
		    labels = new();
		    AddLabel("Mods");
		    AddLabel("WeaponEnchantments");
	    }
	    private static void Close() {
		    tabs--;
            if (tabs < 0)
                return;

            string tabsString = $"\n{Tabs(tabs)}{"}"}";
            if (printLocalization)
                localization += tabsString;
            
            if (printLocalizationKeysAndValues)
                localizationKeys += tabsString;

            labels.RemoveAt(labels.Count - 1);
	    }
	    private static void End() {
		    while(tabs >= 0) {
			    Close();
		    }
		
		    tabs = 0;
            if (printLocalization) {
                localization.LogSimple();
                localization = "";
            }
		    
            if (printLocalizationKeysAndValues) {
                string cultureName = ((CultureName)culture).ToLanguageName();
                localizationKeys = localizationKeys.ReplaceLineEndings();
                string keyFilePath = @$"C:\Users\Isaac\Desktop\TerrariaDev\Localization Merger\Keys\{cultureName}.txt";
                File.WriteAllText(keyFilePath, localizationKeys);
                localizationKeys = "";

                string valueFilePath = @$"C:\Users\Isaac\Desktop\TerrariaDev\Localization Merger\In\{cultureName}.txt";
                File.WriteAllText(valueFilePath, localizationValues);
                localizationValues = "";
            }
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
                string key = $"{allLabels}.{p.Key}";
                string s = null;
                if (translations.ContainsKey(key)) {
                    s = translations[key].GetTranslation(culture);
					if (culture == (int)CultureName.English) {
                        if (s != p.Value) {
							LocalizationData.ChangedData.Add(key);
						}
                    }
                    
                    if (LocalizationData.ChangedData.Contains(key))
                        s = p.Value;
                }
                else {
                    if (culture == (int)CultureName.English) {
                        if (LocalizationData.RenamedKeys.ContainsKey(p.Key)) {
                            string renamedKey = LocalizationData.RenamedKeys[p.Key];
                            string newKey = $"{allLabels}.{renamedKey}";
                            string newS = translations[newKey].GetTranslation(culture);
                            if (newS != renamedKey.AddSpaces())
                                LocalizationData.RenamedFullKeys.Add(key, newKey);
                        }
                    }

                    if (LocalizationData.RenamedFullKeys.ContainsKey(key)) {
                        string newKey = LocalizationData.RenamedFullKeys[key];
                        string newS = translations[newKey].GetTranslation(culture);
                        if (newS != newKey) {
                            key = newKey;
                            s = translations[key].GetTranslation(culture);
                        }
                    }
                }

                s ??= key;

                //$"{key}: {s}".Log();

                if (s == key)
                    s = p.Value;

                bool noLocalizationFound = s == p.Value && (culture == (int)CultureName.English || !LocalizationData.SameAsEnglish[(CultureName)culture].Contains(s));

				s = s.Replace("\"", "\\\"");
				if ((s.Contains("{") || s.Contains("\"")) && s[0] != '"' && s[0] != '“' && s[0] != '”' && !s.Contains('\n'))
                    s = $"\"{s}\"";

                if (zzzLocalizationForTesting) {
                    if (s[s.Length - 1] == '"') {
                        s = $"{s.Substring(0, s.Length - 1)}zzz\"";
                    }
                    else {
                        s += "zzz";
                    }
                }

                s = CheckTabOutLocalization(s);
                if (printLocalization)
                    localization += $"\n{tabString}{p.Key}: {s}";
                
                if (printLocalizationKeysAndValues) {
                    localizationKeys += $"\n{tabString}{p.Key}: {(!noLocalizationFound ? s : "")}";

                    if (noLocalizationFound) {
                        string valueString = s.Replace("\t", "");
                        int length = valueString.Length;
                        if (localizationValuesCharacterCount + length > 5000) {
                            localizationValues += $"\n{'_'.FillString(4999 - localizationValuesCharacterCount)}";
                            localizationValuesCharacterCount = 0;
                            int newLineIndex = valueString.IndexOf("\n");
                            string checkString = newLineIndex > -1 ? valueString.Substring(0, newLineIndex) : valueString;
                            if (checkString.Contains("'''"))
                                localizationValues += "\n";
						}

                        localizationValuesCharacterCount += length + 1;

                        localizationValues += $"{(localizationValues != "" ? "\n" : "")}{valueString}";
                    }
                }
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
        private static void PrintNPCIDSwitch() {
            if (!printNPCIDSwitch)
                return;

            string text = "";
            text += "\n\nswitch() {\n";
            for(short i = NPCID.NegativeIDCount + 1; i < NPCID.Count; i++) {
                text += $"\tcase NPCID.{NPCID.Search.GetName(i)}://{i} {ContentSamples.NpcsByNetId[i].FullName}\n" +
                        $"\t\treturn \"\";\n";
            }

            text += "\tdefault:\n" +
				"\t\treturn \"\";\n" +
				"}\n";
            text.LogSimple();
		}
    }
}

