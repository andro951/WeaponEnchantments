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
using System.Reflection.Emit;
using androLib.Common.Utility;
using androLib.Localization;

namespace WeaponEnchantments.Common.Utility
{
    public class LogModSystem : ModSystem {
        public static bool printListOfContributors = false;
        public static bool printListOfEnchantmentTooltips => WEMod.clientConfig.PrintEnchantmentTooltips;
        public static readonly bool printListForDocumentConversion = false;
        public static readonly bool zzzLocalizationForTesting = false;
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

        public static SortedDictionary<string, Contributors> contributorsData = new SortedDictionary<string, Contributors>();
        public static List<string> namesAddedToContributorDictionary = new List<string>();
        public static List<string> enchantmentsLocalization = new List<string>();
        public static SortedDictionary<int, List<(float, List<DropData>)>> npcEnchantmentDrops = new();
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
					if (lastHeldItem.NullOrAir() || lastHeldItem.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem) && !enchantedItem.Modified) {
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
                    msg.LogSimple_WE();
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

            tooltipList.Log_WE();
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
            artistsMessage.Log_WE();

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
            text.LogSimple_WE();
		}
    }
}

