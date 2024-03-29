﻿using System;
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
using WeaponEnchantments.Content.NPCs;
using static Terraria.Localization.GameCulture;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using WeaponEnchantments.Common.Configs;
using System.Collections;
using androLib.Items;
using androLib.Common.Utility;
using VacuumOreBag.Items;

namespace WeaponEnchantments.Common.Utility.LogSystem
{
    public static class Wiki {
        public static Dictionary<int, List<RecipeData>> createItemRecipes;
        public static Dictionary<int, List<RecipeData>> recipesUsedIn;
        public static Dictionary<int, Dictionary<int, DropRateInfo>> enemyDrops;
        public static Dictionary<int, Dictionary<ChestID, float>> chestDrops;
        public static Dictionary<int, Dictionary<CrateID, float>> crateDrops;
        private static int min;
        private static int max;
        private static bool tier0EnchantmentsOnly = false;
        public static DirectoryInfo logsDirectory = new ($"{Path.GetFullPath(Directory.GetCurrentDirectory())}\\tModLoader-Logs");

		public static string nowString = DateTime.Now.ToString().Replace("/", "_").Replace(":", "_");
        public static string wikiPath = $"{logsDirectory.FullName}\\WeaponEnchantmentsWiki_{nowString}";
        public static Folder wikiFolder = null;
        public static string changesSumary = null;
        private static string lastWikiDirectory = "";
		public static string LastWikiDirectory {
            get {
                if (lastWikiDirectory == "") {
					string weWiki = "WeaponEnchantmentsWiki";
					string mostRecent = null;
					int[] mostRecentData = null;
					IEnumerable<string> directoryNames = logsDirectory.GetDirectories().Where(d => d.FullName != wikiPath).Select(d => d.Name);
					foreach (string directoryName in directoryNames) {
						if (directoryName.StartsWith(weWiki)) {
							if (mostRecent == null) {
								mostRecent = directoryName;
								string dataBeforeParse = directoryName.Substring(weWiki.Length + 1, directoryName.Length - 4 - weWiki.Length)
									.Replace(" ", "_");
								mostRecentData = dataBeforeParse.Split("_").Select(s => int.Parse(s)).ToArray();
                                if (directoryName.EndsWith("PM"))
                                    mostRecentData[3] += 12;
							}
							else {
								string dataBeforeParse = directoryName.Substring(weWiki.Length + 1, directoryName.Length - 4 - weWiki.Length)
									.Replace(" ", "_");
								int[] newData = dataBeforeParse.Split("_").Select(s => int.Parse(s)).ToArray();
								if (directoryName.EndsWith("PM"))
									newData[3] += 12;

								for (int i = 0; i < mostRecentData.Length; i++) {
                                    if (newData[i] == mostRecentData[i]) {
                                        continue;
                                    }
                                    else if (newData[i] > mostRecentData[i]) {
										mostRecent = directoryName;
										mostRecentData = newData;
                                    }
                                    else {
                                        break;
                                    }
                                }
							}
						}
					}

					lastWikiDirectory = mostRecent;
				}

                return lastWikiDirectory;
            }        
        }
		public static void PrintWiki() {
            if (!LogModSystem.printWiki)
                return;

			if (Debugger.IsAttached) {
                changesSumary = "";
				Directory.CreateDirectory(wikiPath);

				if (LastWikiDirectory != null) {
					string lastWikiDirectoryPath = $"{logsDirectory.FullName}\\{LastWikiDirectory}";
					wikiFolder = new(lastWikiDirectoryPath);
				}
			}

			IEnumerable<ModItem> modItems = ModContent.GetInstance<WEMod>().GetContent<ModItem>();
            GetMinMax(modItems);
            GetRecpies(modItems);
            GetDrops();
            IEnumerable<ContainmentItem> containmentItems = modItems.OfType<ContainmentItem>().OrderBy(c => c.tier);
            IEnumerable<EnchantingTableItem> enchantingTables = modItems.OfType<EnchantingTableItem>().OrderBy(t => t.enchantingTableTier);
            IEnumerable<EnchantmentEssence> enchantmentEssence = modItems.OfType<EnchantmentEssence>().OrderBy(e => e.EssenceTier);
            IEnumerable<Enchantment> enchantments = modItems.OfType<Enchantment>().OrderBy(e => e.Name);
            PowerBooster powerBooster = modItems.OfType<PowerBooster>().First();
            UltraPowerBooster ultraPowerBooster = modItems.OfType<UltraPowerBooster>().First();
            OreBag oreBag = modItems.OfType<OreBag>().First();

			List<WebPage> webPages = new();

            AddMainPage(webPages);
            AddItemExperience(webPages);
            AddContainments(webPages, containmentItems, enchantments);
            AddEnchantingTables(webPages, enchantingTables);
            AddEssence(webPages, enchantmentEssence);
            AddEnchantments(webPages, enchantments);
            AddPowerBooster(webPages, powerBooster);
            AddUltraPowerBooster(webPages, ultraPowerBooster);
            AddOreBag(webPages, oreBag);
			AddWitch(webPages, modItems.Where(m => m is ISoldByWitch soldByWitch && soldByWitch.SellCondition != SellCondition.Never));

            string wiki = "\n\n";

			foreach (WebPage webPage in webPages) {
				if (Debugger.IsAttached) {
                    webPage.Log();
				}
				else {
					wiki += $"Page: {webPage.HeaderName}\n{webPage}{"\n".FillString(5)}";
				}
			}

            if (Debugger.IsAttached) {
				File.WriteAllText($"{wikiPath}\\Change Sumary.txt", changesSumary);
			}
            else {
				wiki.Log_WE();
			}
        }
        private static void AddMainPage(List<WebPage> webPages) {
            WebPage mainPage = new("Main Page");
            string fullMainPage =
                "<mainpage-leftcolumn-start />\r\n" +
                "<div style=\"text-align: center;>[[File:Icon.png]]</div>\r\n" +
                "<div style=\"text-align: center; font-size: x-large; padding: 1em;\">'''Welcome to the {{SITENAME}}!'''</div>\r\n" +
                "\r\n" +
                "If you are interested in contributing to this wiki, please let me know: https://discord.gg/hEKKVsFBMd - andro951\r\n" +
                "\r\n" +
                "=== Features ===\r\n" +
                "\r\n" +
                "* Item Customization ([[Enchantments]])\r\n" +
                "* Progression System ([[Item Experience]])\r\n" +
                "* Item Upgrading (Infusion)\r\n" +
                "\r\n" +
                "Terraria has you frequently swapping old gear for new. The enchanting system allows you to customize your weapons and armor, and keep your progress as you change or upgrade your gear.\r\n" +
                "\r\n" +
                "===Items===\r\n" +
                $"{typeof(Containment).Name.AddSpaces().ToItemPNG(displayName: false)} {"Containments".ToLink()}\r\n" +
                "\r\n" +
				$"{typeof(WoodEnchantingTable).Name.AddSpaces().ToItemPNG(displayName: false)} {"Enchanting Tables".ToLink()}\r\n" +
                "\r\n" +
				$"{typeof(EnchantmentEssenceBasic).Name.AddSpaces().ToItemPNG(displayName: false)} {"Enchantment Essence".ToLink()}\r\n" +
                "\r\n" +
				$"{"Enchantment Basic".ToPNG()} {"Enchantments".ToLink()}\r\n" +
                "\r\n" +
				$"{typeof(PowerBooster).Name.AddSpaces().ToItemPNG(true)}\r\n" +
                "\r\n" +
				$"{typeof(UltraPowerBooster).Name.AddSpaces().ToItemPNG(true)}\r\n" +
				"\r\n" +
				$"{typeof(OreBag).Name.AddSpaces().ToItemPNG(true)}\r\n" +
				"\r\n" +
                "=== NPCs ===\r\n" +
				$"{typeof(Witch).Name.AddSpaces().ToPNGLink()}\r\n" +
                "\r\n" +
                "=== Config ===\r\n" +
                "Many players will find Enchantments to be too powerful. For players who enjoy a high difficulty experience, it is recommended to change the Enchantment Strength Preset to Expert (50%) or Master (25%). (2nd page of the config)<blockquote>You have an extreme amount of control over the power level of this mod via the config.</blockquote>\r\n" +
                "\r\n" +
                "=== When you start a game... ===\r\n" +
                "\r\n" +
                "* Make an enchanting table right away!\r\n" +
                "** The first enchanting table is created with a workbench and 4 torches.\r\n" +
                "* Gear yourself up (fill in your armor and accessory slots so they start getting XP).\r\n" +
                "* When upgrading, offer your old armor and weapons for essence.\r\n" +
                "* Upgrade your new weapons and armor with the obtained essence.\r\n" +
                "\r\n" +
                "=== New Player Tips and Tricks ===\r\n" +
                "\r\n" +
                "* DONT SELL enchantable items! Offer them instead.\r\n" +
                "** The value from ore received is slightly higher than an item's sell value and you get Essence equivalent to the item's xp.\r\n" +
                "** Offering items returns all Enchantments/Power Booster applied to the consumed item.\r\n" +
                "* Carrying an Enchanting Table with you to convert unwanted items is a good way to save inventory space.(Especially if you set the config to 0% ore, 100% essence)\r\n" +
                "* Make a gem tree farm (especially for diamond/amber). They are used to craft high tier Enchantments.\r\n" +
                "\r\n" +
                "=== Other Mod Integration ===\r\n" +
                "[[Magic Storage Integration]]\r\n" +
                "\r\n" +
                "<mainpage-endcolumn />\r\n" +
                "\r\n" +
                "<mainpage-rightcolumn-start />\r\n" +
                "''Need help building out this community?''\r\n" +
                "\r\n" +
                "*[[Project:Wiki rules|Rules of this wiki]]\r\n" +
                "*[[w:c:community:Help:Getting Started|Getting Started]]\r\n" +
                "*[[w:c:community:Help:Contributing|How to Contribute]]\r\n" +
                "*[[w:c:community:Help:Community Management|Managing your new community]]\r\n" +
                "*[[w:c:community:Help:Contents|Guides]]\r\n" +
                "*[[w:c:community:Help:Index|All Help articles]]\r\n" +
                "\r\n" +
                "You can also be part of the larger Fandom family of communities. Visit [[w:c:community|Fandom's Community Central]]!\r\n" +
                "\r\n" +
                "\r\n" +
                "\r\n''Community Founders'': Write a good and paragraph-length description for your welcome section about your topic. Let your readers know what your topic is about and add some general information about it. Then you should visit [[Special:AdminDashboard|the admin dashboard for more tips]]. \r\n" +
                "<mainpage-endcolumn />\r\n" +
                "[[Category:{{SITENAME}}]]\r\n";
			mainPage.AddParagraph(fullMainPage);
            webPages.Add(mainPage);
        }
        private static void AddItemExperience(List<WebPage> webPages) {
			WebPage itemExperience = new("Item Experience", webPages.Where(wp => wp.HeaderName == "Main Page").First());
            itemExperience.AddParagraph(
                "All weapons, armor and accessories can now gain experience (XP). These are enchantable items.");
            itemExperience.NewLine();
            itemExperience.AddParagraph(
                "Items have a level based on their XP.  Levels increase the enchantment capacity of the item by 1 per level.  \n" +
                "Levels also increases weapon critical strike chance by 1 per level.  Armor and accessories grant you damage reduction per level. " +
                "(Both can be disabled and are affected by the Config Global Enchantment Strength multiplier.)");

            List<List<string>> damageReductionTable = new() { new() { "", "Armor", "Accessory" } };
			short[] gameModes = { 3, 0, 1, 2 };
			foreach (short gameMode in gameModes) {
                float armorDamageReduction = ArmorDamageReduction.DamageReductionPerLevel[gameMode, 0] / 100000f;
                float accessoryDamageReduction = ArmorDamageReduction.DamageReductionPerLevel[gameMode, 1] / 100000f;
				damageReductionTable.Add(new() {
					gameMode.ToGameModeIDName(),
					$"{armorDamageReduction.S(5)}% ({(armorDamageReduction * 40f).S()}% at 40)",
					$"{accessoryDamageReduction.S(5)}% ({(accessoryDamageReduction * 40f).S()}% at 40)"
                });
			}

			itemExperience.AddTable(damageReductionTable, label: "Damage Reduction per level default values", firstRowHeaders: true);
			itemExperience.NewLine();
			itemExperience.AddParagraph(
                "You can obtain XP by damaging enemies, doing skilling activities such as mining and cutting trees, and consuming essence.");
			itemExperience.NewLine();
			itemExperience.AddParagraph(
                "Items with no XP will not display experience or levels.");
			itemExperience.NewLine();
			itemExperience.AddPNG("Musket With Experience", false);
			webPages.Add(itemExperience);
		}
		private static void AddContainments(List<WebPage> webPages, IEnumerable<ContainmentItem> containmentItems, IEnumerable<Enchantment> enchantments) {
            WebPage Containments = new("Containments", webPages.Where(wp => wp.HeaderName == "Main Page").First());
            Containments.AddParagraph("Containments contain the power of an enchantment. More powerful enchantments require larger and stronger containments to hold them.\n" +
            "Containments are crafting materials used to craft enchantments.");
            AddLowestCraftableEnchantments(Containments, enchantments);
            foreach (ContainmentItem containment in containmentItems) {
                int tier = containment.tier;
                string subHeading = $"{containment.Item.ToItemPNG()} (Tier {tier})";
                Containments.AddParagraph($"{containment.Item.ToItemPNG(link: true)} (Tier {tier})");
                WebPage containmentPage = new(containment.Item.Name, Containments);
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
            WebPage EnchantingTable = new("Enchanting Tables", webPages.Where(wp => wp.HeaderName == "Main Page").First());
            foreach (EnchantingTableItem enchantingTable in enchantingTables) {
                int tier = enchantingTable.enchantingTableTier;
                EnchantingTable.AddParagraph($"{enchantingTable.Item.ToItemPNG(link: true)} (Tier {tier})");
                WebPage enchantingTablePage = new(enchantingTable.Item.Name, EnchantingTable);
                ItemInfo itemInfo = new(enchantingTable);
                enchantingTablePage.AddLink("Enchanting Tables");
                itemInfo.AddStatistics(enchantingTablePage);
                itemInfo.AddDrops(enchantingTablePage);
                itemInfo.AddInfo(enchantingTablePage);
                itemInfo.AddRecipes(enchantingTablePage);
                webPages.Add(enchantingTablePage);
            }

            EnchantingTable.AddParagraph($"All of the essence you pick up are stored in the enchanting table interface. " +
				$"Right clicking the enchanting table while it's placed in the world opens the interface. You can also right click the enchanting table item in your " +
                $"inventory, but it won't count for crafting enchantments.<br/>\n" +
                $"In an enchanting table you can {"Crafting and Upgrading Enchantments".ToSectionLink("create and upgrade", "Enchantments")} enchantments via crafting, " +
				$"{"Enchanting items".ToSectionLink("apply and remove")} enchantments, {"Leveling items up".ToSectionLink("convert essence to item experience")}, " +
				$"{"Offer".ToSectionLink("offer")} items, {"Syphon".ToSectionLink("syphon")} items, {"Infusion".ToSectionLink("infuse")} items, " +
                $"{"Enchantment Storage".ToSectionLink("store items")}, and {"Enchantment Loadouts".ToSectionLink("manage/apply enchantment loadouts")}." +
				$"");
            EnchantingTable.AddSubHeading("Leveling items up");
            EnchantingTable.AddParagraph($"Item experienced can be gained by {"".ToSectionLink("using the item", "")} or consuming essence in the enchanting table interface.\n" +
				$"* Level Up button { "LevelUpButton".ToPNG()} (The numbers above the level up button determine how many levels will be added per click of the level up button.  " +
                $"Will do nothing if you do not have enough essence for the selected number of levels.)\n" +
				$"# Place your item in the Item slot\n" +
				$"# Click the Level Up button\n" +
				$"# Enough essence is used to level your item up once.\n" +
				$"Essence can be favorited in the enchanting table interface to prevent it from being consumed by the level up button " +
				$"(ignored if you don't have enough total essence to level up without the favorited essence)<br/>\n" +
				$"* XP buttons { "XPButtons".ToPNG()}\n" +
				$"# Place your item in the Item slot\n" +
				$"# Click the XP button below the essence you want to consume for experience\n" +
				$"# One essence of that type is consumed, adding experience to your item");

            EnchantingTable.AddSubHeading("Enchanting items");
            EnchantingTable.AddPNG("EnchantingTableUI");
            EnchantingTable.AddParagraph($"To enchant an item, it must have enough {"Enchantment Capacity"} to handle the new enchantment's capacity cost.\n" +
				$"# Place the item you wish to enchant into the item slot.  (You can shift left click from your inventory instead of using your mouse)\n" +
				$"# Add (or remove) the enchantment(s) you wish using the enchantment slots.  (Can also be shift clicked from your inventory)\n" +
				$"# Remove the item from the enchanting table (move with mouse or shift click.)  (There is no confirm button for enchantments)<br/>\n" +
				$"The Utility Slot is for {"Utility Enchantments".ToLink()}");
            EnchantingTable.AddPNG("MusketTooltip");

            EnchantingTable.AddSubHeading("Offer " + "OfferButton".ToPNG());
            EnchantingTable.AddParagraph($"If you have an enchantable item you would like to throw away, instead, you can offer it.  \n" +
                $"Offering items gives you ores and essence equal to the item's value (rounds up), so you end up with slightly higher coins if you offer then sell something.  \n" +
                $"It gives half ore and half essence by default (configurable).  It also returns all experience as essence and returns all enchantments and an installed power booster.<br/>\n" +
                $"1. Place the item you want to offer in the item slot.<br/>\n" +
                $"2. press the offer button. " +
                $"(The offer UI will display.  If the Toggle Auto Trash Offered Items option is on, purple, the item you offer will be saved in the list of your offered " +
                $"items.  Any time you pick up an item marked as trash, it will automatically be offered.  You remove items from being marked for automatic offering with " +
                $"{"Manage Offered Items".ToSectionLink("Manage Offered Items")} in the Enchantment Storage.)<br/>\n" +
                $"{"OfferUI".ToPNG()}<br/>\n" +
                $"3. Press confirm.<br/>\n" +
                $"4. You receive the offer rewards items.\n");
            EnchantingTable.AddSubHeading($"Mass offer config option");
            EnchantingTable.AddParagraph($"\"Offer all of the same item.\" under Client Config, Enchanting Table Options.\n" +
                $"This option enables to methods of mass offering items.  (This will not offer any item that has experience/enchantments/power boosters/etc)\n" +
                $"# When offering an item, all of the same item will be offered from your inventory.\n" +
                $"# When offering an item, ALL items of ANY type from chests directly touching the enchanting table will be offered.  Be careful with your storage.");

            EnchantingTable.AddSubHeading("Syphon " + "SyphonButton".ToPNG());
            EnchantingTable.AddParagraph($"If you have a max level item (level 40), you can convert any excess experience on the item past level 40 " +
				$"(past 100M experience) into essence.\n" +
				$"# Place the max level item into the item slot.\n" +
				$"# Click syphon.\n" +
				$"# Essence will be deposited into the enchanting table interface.");

            EnchantingTable.AddSubHeading("Infusion " + "InfusionButton".ToPNG());
            EnchantingTable.AddParagraph($"Allows you to consume a weapon to enhance the power of a lower tier weapon to about the same power as the " +
				$"consumed weapon or transfers the set bonus from a piece of armor to another.  Any experience/enchantments/power booster on a consumed item will be returned just like {"Offer".ToSectionLink("offer")}.");

            EnchantingTable.AddSubHeading("Weapon Infusion", 2);
            EnchantingTable.AddParagraph($"Allows you to consume high rarity items to upgrade the damage of low rarity weapons.  \n" +
				$"Example, if you like Veilthorn more than your newer/stronger weapon, just infuse the new weapon into Veilthorn to upgrade it's damage instead of switching.  \n" +
				$"The damage bonus is based on the difference in rarity and value between the 2 items. Terraria has 10 rarities of vanilla weapons, so I based the system off of those.  \n" +
				$"(modded items can be rarity 11 which will cause their Infusion Power to be the same as the max value rarity 10 items (1100).  \n" +
				$"Infusion Power - A weapon stat that is determined by an item's rarity and value. 100 Infusion Power per rarity (rarity x100).  \n" +
				$"Additionally, the item's rarity will give up to 100 extra infusion power based on the value of the item compared to the average value of items in that rarity.  \n" +
				$"(Example: items of rarity 0 have an average value of about 3000 copper (30 silver). The lowest value item is worth 100 copper.  \n" +
				$"This 100 copper item would have an infusion power of 0. A rarity 0 item worth the average value (~30 silver) would have an infusion power of 50.  \n" +
				$"The max value rarity 0 item would have 100 infusion power. The min, max and average values are calculated based only on vanilla items.  \n" +
				$"Modded items that are above or below the min/max values will be counted as the min/max value for the infusion power calculation.  \n" +
				$"Currently, the highest Infusion Power possible for weapons is from Meowmere (1100) because it is rarity 10 and has the highest item value of rarity 10 weapons.  \n" +
				$"Weapon infusion steps:\n" +
				$"# Place the higher Infusion Power item into the enchanting table (this item will be destroyed)\n" +
				$"# Click Infusion (If you change your mind, you can get the item back by pressing Cancel - Same button as Infusion)\n" +
				$"# Place the lower Infusion Power item into the enchanting table Click Finalize (Same button as Infusion/Cancel)");

            EnchantingTable.AddSubHeading("Armor Infusion", 2);
            EnchantingTable.AddParagraph($"Allows you to consume a piece of armor and replace the set bonus of an item with one from another.  \n" +
				$"The piece of armor will act like the consumed one for the purposes of determining set bonuses. The piece of armor will also look like the consumed one while equipped.  \n" +
				$"Armor infusion steps:\n" +
				$"# Place the armor with the set bonus you want to transfer into the enchanting table (this item will be destroyed)\n" +
				$"# Click Infusion (If you change your mind, you can get the item back by pressing Cancel - Same button as Infusion)\n" +
				$"# Place the armor you want to keep into the enchanting table (It will have it's set bonus replaced with the previous item's) Click Finalize (Same button as Infusion/Cancel)");

			EnchantingTable.AddSubHeading("Enchantment Storage", 2);
            EnchantingTable.AddParagraph($"The Enchantment Storage is similar to a piggy bank.<br/>\n" +
                $"It can store most of the items from Weapon Enchantments.<br/>\n" +
                $"There are a lot of mouse interactions between the Enchantment Storage and Enchanting Table.  " +
                $"Shift left clicking is the easiest way to transfer enchantments between the two.");
            EnchantingTable.AddPNG("EnchantmentStorageUI");
            EnchantingTable.AddParagraph($"The Loot All, Deposit All, Quick Stack and Sort buttons work the same or very similar to the vanilla chest/piggy bank buttons.<br/>\n" +
                $"Toggle Vacuum - Toggles items automatically being sent to the Enchantment Storage instead of your inventory.  Purple means on.<br/>\n" +
                $"Toggle Mark Trash - Replaces your cursor with a trash can, and disables normal mouse actions.  Left click any enchantment in the storage to mark it as " +
                $"trash.  Be very careful when marking enchantments as trash.  Whenever you pick up an enchantment that is marked as trash, it will be uncrafted into essence.<br/>\n" +
                $"Uncraft All Trash - Uncrafts every enchantment marked as trash into essence, containments and gems.  This is the same as crafting them into essence " +
                $"manually.<br/>\n" +
                $"Revert All To Basic - Crafts every enchantment in the storage into basic versions, returning essence, gems and higher tier containments.  " +
                $"(Keep in mind you have to have basic containments to do this.)" +
                $"Manage Trash - Shows you every enchantment, allowing you to mark or unmark enchantments as trash even if you don't have them.<br/>\n" +
                $"Manage Offered Items - Shows you every item that can be offered regardless of you having one or not.  Items with a trash can background will be " +
                $"automatically offered when picked up.");
            EnchantingTable.AddPNG("EnchantmentStorageUIManageOfferedItems");
			EnchantingTable.AddParagraph($"Quick Craft - Allows you to quickly craft any enchantment that you have the ability to craft.  Unlike normal crafting, the item " +
                $"crafted is sent to the enchantment storage instead of being picked up by your mouse.  This is meant to help with changing a lot of enchantments all at once.  " +
                $"Keep in mind, you have to be near an appropriate level enchanting table to craft enchantments, so using the enchanting table from your inventory will not " +
                $"work.");
            EnchantingTable.AddPNG("EnchantmentStorageUIQuickCraft");
            EnchantingTable.AddParagraph(
                $"Items with a gold background can be crafted.If you already have some of an item that you can craft, the stack number will show on the gold " +
				$"background item.  If you don't already have any, the stack will be 0.  It also shows the rest of the enchantments that you can't craft, but that you have in " +
				$"storage at the end of the list with normal blue backgrounds.");
            EnchantingTable.AddParagraph($"Favoriting Enchantments - You can favorite enchantments by pressing the favorite key (alt by default) and clicking on an enchantment.  " +
                $"Favorited enchantments are ignored when uncrafting trash or reverting all to basic.");

			EnchantingTable.AddSubHeading("Enchantment Loadouts", 2);

            EnchantingTable.AddParagraph($"Enchantment loadouts don't store items.");
			EnchantingTable.AddPNG("EnchantmentLoadoutUI");
			EnchantingTable.AddParagraph($"They just save the type and tier of enchantment.  When creating an Enchantment Loadout, you can " +
                $"quickly fill the slots by shift lift clicking enchantments from your storage to fill the next slot (The next slot to fill has a gold background when shift is held).  " +
                $"You can also hold an enchantment in your mouse and click it on a slot instead.  Clicking on a slot with no enchantment in your mouse will clear the slot.<br/>\n" +
                $"Loadout # - Clicking the Loadout button selects that loadout so you can see or edit it.<br/>\n" +
                $"All - All replaces all enchantments on your held item, armor and accessories with the enchantments from the loadout.  If any item isn't high enough level " +
                $"to support the enchantments for it's slot, the loadout will fail to load.<br/>\n" +
                $"Held Item/Armor/Accessories - These buttons to the same thing as the All button, but only load the specific enchantments for the selected category.<br/>\n" +
                $"Add - Adds a new loadout (Max of 15).");

			EnchantingTable.AddSubHeading("Efficiently Upgrading your enchantment loadout", 2);
            EnchantingTable.AddParagraph($"* Load a blank loadout to return all enchantments to the storage.\n" +
                $"* Favorite any enchantments you want to keep.\n" +
                $"* Revert All to Basic.\n" +
                $"* Quick Craft all desired enchantments.\n" +
                $"* Update and load your original loadout.");

			webPages.Add(EnchantingTable);
        }
        private static void AddEssence(List<WebPage> webPages, IEnumerable<EnchantmentEssence> enchantmentEssence) {
            WebPage Essence = new("Enchantment Essence", webPages.Where(wp => wp.HeaderName == "Main Page").First());
            Essence.AddParagraph("Essence represents solidified experience and are automatically stored in the enchanting table interface when picked up. They can be used to...");
            Essence.AddBulletedList(elements: new string[] {
                "Crafting and Upgrading Enchantments".ToSectionLink("Upgrade enchantments", "Enchantments"),
                "Leveling items up".ToSectionLink("Infuse it's XP value into items", "Enchanting Tables")
            });
            foreach (EnchantmentEssence essence in enchantmentEssence) {
                int tier = essence.EssenceTier;
                Essence.AddParagraph($"{essence.Item.ToItemPNG(link: true)} (Tier {tier})");
                WebPage essencePage = new(essence.Item.Name, Essence);
                ItemInfo itemInfo = new(essence);
                essencePage.AddLink("Enchantment Essence");
                itemInfo.AddStatistics(essencePage);
                itemInfo.AddDrops(essencePage);
                itemInfo.AddInfo(essencePage);
                itemInfo.AddRecipes(essencePage);
                webPages.Add(essencePage);
            }

            webPages.Add(Essence);
        }
        private static void AddEnchantments(List<WebPage> webPages, IEnumerable<Enchantment> enchantments) {
            WebPage Enchantments = new("Enchantments", webPages.Where(wp => wp.HeaderName == "Main Page").First());
            WebPage UtilityEnchantments = new("Utility Enchantments", Enchantments);
            if (!tier0EnchantmentsOnly) {
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
                    $"Topaz can be any Common Gem: {"androLib:CommonGems".ToItemPNGs(link: true)}<br/>\n" +
                    $"Amber can be any Rare Gem: {"androLib:RareGems".ToItemPNGs(link: true)}<br/>\n" +
                    $"{"Enchantment Basic".ToLabledPNG()} is used as a generic enchantment when any enchantment could be used.");

                Enchantments.AddSubHeading("Utility Enchantments");
                string shortUtilityExplanation = $"{"Utility Enchantments".ToLink()} are non-damage based enchantments.  " +
                    $"They can be placed into any of the enchantment slots, but only utility enchantments can be placed into the utility slot.";
                Enchantments.AddParagraph(shortUtilityExplanation);
                UtilityEnchantments.AddLink("Enchantments");
                UtilityEnchantments.AddParagraph($"{shortUtilityExplanation}<br/>\n" +
					$"Utility enchantments require half as much essence to craft and cost half the enchantment capacity, 1 to 5 enchantment capacity(based on tier) " +
					$"instead of 2 to 10.");

                Enchantments.AddSubHeading("Obtaining Enchantments");
                AddLowestCraftableEnchantments(Enchantments, enchantments);
                Enchantments.AddParagraph($"All other enchantments can be found from killing enemies or looting chests/fishing crates.  ({"All Enchantment Drops".ToLink("Full drop table")})");

                Enchantments.AddTable(GetGenericEnchantmnetRecipes(), label: "Recipes", firstRowHeaders: true, rowspanColumns: true, collapsible: true);
                Enchantments.AddParagraph($"To view recipes in game, you can use the vanilla guide's crafting interface or the " +
                    $"{"https://steamcommunity.com/sharedfiles/filedetails/?id=2619954303&searchtext=recipe+browser".ToExternalLink("Recipe Browser Mod")}");
                Enchantments.AddSubHeading("All Enchantment types");
            }

            //WebPage enchantmentTypePage = new("");
            string typePageLinkString = "";
            foreach (IEnumerable<Enchantment> list in enchantments.GroupBy(e => e.EnchantmentTypeName).Select(l => l.ToList().OrderBy(e => e.EnchantmentTier))) {
                //bool first = true;
		        EnchantmentInfoBox enchantmentInfoBox = new(FloatID.right);
                foreach (Enchantment enchantment in list.ToList()) {
                    enchantmentInfoBox.Add(enchantment);
                }
                
                WebPage enchantmentPage = new(enchantmentInfoBox.Name, Enchantments);
                enchantmentPage.AddLink("Enchantments");
                enchantmentInfoBox.AddStatistics(enchantmentPage);
                enchantmentInfoBox.AddDrops(enchantmentPage);
                enchantmentInfoBox.TryAddWikiDescription(enchantmentPage);
                enchantmentInfoBox.AddInfo(enchantmentPage);
                enchantmentInfoBox.AddEffects(enchantmentPage);
                enchantmentInfoBox.AddAllowedList(enchantmentPage);
                enchantmentInfoBox.AddRecipes(enchantmentPage);
                enchantmentPage.AddParagraph("{{Enchantments}}");
                enchantmentPage.AddParagraph("[[Category:Enchantments]]");
                webPages.Add(enchantmentPage);
                if (!tier0EnchantmentsOnly) {
                    string enchantmentType = enchantmentInfoBox.Name;
                    Enchantment enchantment = enchantmentInfoBox.enchantments[0];
                    string typePNG = enchantment.Item.ToItemPNG(link: true, linkText: enchantmentType);
                    Enchantments.AddParagraph(typePNG);
                    if (enchantment.Utility)
                        UtilityEnchantments.AddParagraph(typePNG);
                }
                
                //if (!tier0EnchantmentsOnly)
                //    webPages.Add(enchantmentTypePage);
            }

            if (!tier0EnchantmentsOnly) {
                webPages.Add(UtilityEnchantments);
                webPages.Add(Enchantments);
            }

            WebPage AllEnchantmentDrops = new("All Enchantment Drops", Enchantments);
            AllEnchantmentDrops.AddLink("Enchantments");
            ItemInfo.AddAllDrops(AllEnchantmentDrops, typeof(Enchantment));
            webPages.Add(AllEnchantmentDrops);
        }
        private static void AddPowerBooster(List<WebPage> webPages, PowerBooster powerBooster) {
            WebPage PowerBooster = new("Power Booster", webPages.Where(wp => wp.HeaderName == "Main Page").First());
            ItemInfo itemInfo = new(powerBooster);
            itemInfo.AddStatistics(PowerBooster);
            itemInfo.AddDrops(PowerBooster);
            itemInfo.AddInfo(PowerBooster);
            itemInfo.AddRecipes(PowerBooster);
            PowerBooster.AddParagraph($"<br/>\n" +
				$"A Power Booster is a rare item obtained from defeating bosses.  " +
				$"Prehardmode bosses don't drop power boosters unless the config option is enabled.  " +
				$"It adds 10 levels to an item " +
				$"(These levels do not count towards the level 40 cap or give critical strike chance.).  They can only be used once per item.  " +
				$"To apply a power booster to an item, place the item into the enchanting table then click the power booster onto the item in the table as if you were " +
				$"merging two stacks of the same item.  You can also shift left click the power booster from your inventory.  Power boosters drop from boss bags in " +
				$"Expert/Master mode at the same rates from the table.");

            webPages.Add(PowerBooster);
		}
        private static void AddUltraPowerBooster(List<WebPage> webPages, UltraPowerBooster ultraPowerBooster) {
            WebPage PowerBooster = new("Ultra Power Booster", webPages.Where(wp => wp.HeaderName == "Main Page").First());
            ItemInfo itemInfo = new(ultraPowerBooster);
            itemInfo.AddStatistics(PowerBooster);
            itemInfo.AddDrops(PowerBooster);
            itemInfo.AddInfo(PowerBooster);
            itemInfo.AddRecipes(PowerBooster);
            PowerBooster.AddParagraph($"<br/>\n" +
                $"A Power Booster is a rare item obtained from defeating post Plantera bosses (includes Plantera).  " +
                $"It adds 20 levels to an item " +
                $"(These levels do not count towards the level 40 cap or give critical strike chance.).  They can only be used once per item.  " +
                $"To apply an ultra power booster to an item, place the item into the enchanting table then click the ultra power booster onto the item in the table as if you were " +
                $"merging two stacks of the same item.  You can also shift left click the power booster from your inventory.  Ultra Power boosters drop from boss bags in " +
                $"Expert/Master mode at the same rates from the table.");

            webPages.Add(PowerBooster);
        }
        private static void AddOreBag(List<WebPage> webPages, OreBag oreBag) {
            WebPage OreBag = new("Ore Bag", webPages.Where(wp => wp.HeaderName == "Main Page").First());
            ItemInfo itemInfo = new(oreBag);
            itemInfo.AddStatistics(OreBag);
            itemInfo.AddDrops(OreBag);
            itemInfo.AddInfo(OreBag);
            itemInfo.AddRecipes(OreBag);
            OreBag.AddParagraph($"The Ore Bag is a storage item similar to a piggy bank.  It is given to every player when starting the game.<br/>\n" +
                $"By default, the ore bag automatically stores all items you pick up (that are allowed in the Ore Bag) such as ore, bars, gems, sand, and glass.<br/>\n" +
                $"This bag is mainly for making inventory management with Weapon Enchantments easier, and is heavily integrated with Magic Storage so that you " +
                $"don't have to dump the ore bag into magic storage.<br/>\n<br/>\n");
            OreBag.AddParagraph($"To open the bag, right click it from your inventory.  This will display the Ore Bag UI.");
			OreBag.AddPNG("OreBagUI");
            OreBag.AddParagraph($"The Loot All, Deposit All, Quick Stack and Sort buttons work the same or very similar to the vanilla chest/piggy bank buttons.<br/>\n" +
                $"The Toggle Vacuum button toggles items automatically being sent to the Ore Bag instead of your inventory.  Purple means on.<br/>\n" +
                $"The Ore Bag holds up to {WEPlayer.OreBagSize} items.<br/>\n" +
                $"The bag has a search bar and scroll bar to help find your items.<br/>\n" +
                $"The Ore Bag UI can be moved by dragging anywhere on the UI that isn't a button or item slot.<br/>\n" +
                $"The location of the ore bag UI is saved so you don't have to constantly reposition it.");

            webPages.Add(OreBag);
        }
        private static void AddWitch(List<WebPage> webPages, IEnumerable<ModItem> modItems) {
            WebPage WitchPage = new("Witch", webPages.Where(wp => wp.HeaderName == "Main Page").First());
            NPCInfo npcInfo = new(ModContent.NPCType<Witch>());
            npcInfo.AddStatistics(WitchPage);
            WitchPage.AddParagraph(
                $"The Witch is a town npc that sells a variety of items.  " +
				$"She sells containments and enchantment essence and a selection of enchantments.  The enchantments are " +
				$"chosen randomly from the ones that are allowed to be sold.  This includes all enchantments with a sell condition " +
				$"of \"Always\", 4 enchantments with a sell condition of \"Any Time\", and 2 from the rare pool.  The rare pool " +
				$"consists of the \"Any Time Rare\" enchantments and all other enchantments if the sell condition for them is met.  " +
				$"Her shop resets as soon as the game changes from night to day.  This happens at 4:30 am normally, but if using " +
				$"some other effect to set time, it will trigger as well as long as the game registered it being night then day.  " +
				$"The prices of enchantment essence and enchantments (except ones with a sell condition of \"Always\") are higher " +
				$"than normal.  Enchantment essence is 2x higher for Tier 4(Legendary) while the lower tiers are multiplied by an " +
				$"additional 2x each tier.  This is make the cost per experience the exact same at every tier and to prevent an " +
				$"infinite coin/essence exploit from buying Basic essence and crafting it into Legendary, gaining a significant " +
				$"amount of value.  The 2x increase is to add a slight disincentive to buying essence instead of farming for it.");
            npcInfo.AddSpawnCondition(WitchPage);
            List<List<string>> sellItems = new() { new List<string>() { "Item", "Price", "Sell Condition" } };
			foreach (ModItem modItem in 
                modItems
                .OrderBy(m => ((ISoldByWitch)m).SellCondition)
                .GroupBy(m => m.TypeBeforeModItem().Name)
                .Select(g => g.ToList().OrderBy(m => EnchantingRarity.GetTierNumberFromName(m.Name)))
                .SelectMany(i => i)) {

                ItemInfo.TryGetShopPrice(modItem, out int price);
                sellItems.Add(new() { modItem.Item.ToItemPNG(link: true), price.GetCoinsPNG(), ((ISoldByWitch)modItem).SellCondition.ToString().AddSpaces() });
			}

            WitchPage.AddSubHeading("Shop Items");
            WitchPage.AddTable(
                elements: sellItems,
                firstRowHeaders: true
            );
            npcInfo.AddLivingPreferences(WitchPage);

            webPages.Add(WitchPage);
		}
        private static void AddLowestCraftableEnchantments(WebPage webPage, IEnumerable<Enchantment> enchantments) {
            string text = "Only these enchantments can be obtained by crafting.  The others must all be found in other ways.\n";
            webPage.AddParagraph(text);
            webPage.AddBulletedList(false, false, enchantments.Where(e => e.LowestCraftableTier == 0 && e.EnchantmentTier == 0).Select(c => c.Item.ToItemPNG(link: true, linkText: (c.EnchantmentTypeName + "Enchantment").AddSpaces())).ToArray());
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
                WEModSystem.GetChestLoot(chestID, out List<DropData> dropData, out float baseChance);
                if (dropData == null)
                    continue;

                string name = chestID.ToString() + " Chest";
                float total = 0f;
                IEnumerable<DropData> weightedDrops = dropData.Where(d => d.Chance <= 0f);
				IEnumerable<DropData> chanceDrops = dropData.Where(d => d.Chance > 0f);

                foreach (DropData data in chanceDrops) {
                    float randFloat = Main.rand.NextFloat();
                    float chance = ConfigValues.ChestSpawnChance / 0.5f * data.Chance;
					if (chestDrops.ContainsKey(data.ID)) {
						chestDrops[data.ID].Add(chestID, chance);
					}
					else {
						chestDrops.Add(data.ID, new() { { chestID, chance } });
					}
				}

				foreach (DropData data in weightedDrops) {
                    total += data.Weight;
                }

                foreach (DropData data in weightedDrops) {
                    Item sampleItem = ContentSamples.ItemsByType[data.ID];
                    int type = sampleItem.type;
                    if (!(type >= min && type <= max))
                        continue;

                    if (chestDrops.ContainsKey(type)) {
                        chestDrops[type].Add(chestID, baseChance * data.Weight / total);
					}
					else {
                        chestDrops.Add(type, new() { { chestID, baseChance * data.Weight / total } });
                    }
                }
            }

            crateDrops = new();
            foreach (KeyValuePair<int, List<DropData>> crate in GlobalCrates.crateDrops) {
                string name = ((CrateID)crate.Key).ToString() + " Crate";
                float total = 0f;

				IEnumerable<DropData> weightedDrops = crate.Value.Where(d => d.Chance <= 0f);
				IEnumerable<DropData> chanceDrops = crate.Value.Where(d => d.Chance > 0f);

				foreach (DropData data in chanceDrops) {
					float randFloat = Main.rand.NextFloat();
					float chance = ConfigValues.CrateDropChance * data.Chance;
					if (crateDrops.ContainsKey(data.ID)) {
						crateDrops[data.ID].Add((CrateID)crate.Key, chance);
					}
					else {
						crateDrops.Add(data.ID, new() { { (CrateID)crate.Key, chance } });
					}
				}

				foreach (DropData data in crate.Value) {
                    total += data.Weight;
                }

                foreach (DropData data in crate.Value) {
                    Item sampleItem = ContentSamples.ItemsByType[data.ID];
                    int type = sampleItem.type;
                    if (!(type >= min && type <= max))
                        continue;

                    float baseChance = GlobalCrates.GetCrateEnchantmentDropChance(crate.Key);
                    if (crateDrops.ContainsKey(type)) {
                        if (crateDrops[type].ContainsKey((CrateID)crate.Key)) {
                            $"New: item: {sampleItem.S()}, CrateID: {(CrateID)crate.Key}, chance: {baseChance * data.Weight / total}.  Old chance: {crateDrops[type][(CrateID)crate.Key]}".LogSimple_WE();
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

                        crateDrops[type].Add((CrateID)crate.Key, baseChance * data.Weight / total);
                    }
                    else {
                        crateDrops.Add(type, new() { { (CrateID)crate.Key, baseChance * data.Weight / total } });
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
                                    $"itemType: {new Item(itemType).S()}, npcType{ContentSamples.NpcsByNetId[npcNetID]}".LogSimple_WE();
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
        public static string ToSectionLink(this string s, string text = null, string page = null) => $"{(page != null ? page : "")}#{s}".ToLink(text);
        public static string EnchantmentTypeShortLink(this string s) => $"{s.AddSpaces()} Enchantment".ToLink(s.AddSpaces());
        public static string EnchantmentTypeShortLink(this EnchantmentStat enchantmentStat) => enchantmentStat.ToString().EnchantmentTypeShortLink();
        public static string ToExternalLink(this string s, string text = null) => $"[{s}{(text != null ? $" {text}" : "")}]";
        public static string ToVanillaWikiLink(this string s, string text = null) => $"https://terraria.fandom.com/wiki/{s}".ToExternalLink(text != null ? text : s.Replace('_', ' '));
        public static string ToVanillaWikiLink(this InvasionID id, string text = null) => $"{id}".ToVanillaWikiLink(text);
        public static string ToPNG(this string s, bool removeSpaces = true) => $"[[File:{(removeSpaces ? s.RemoveSpaces() : s)}.png]]";
        public static string ToPNGLink(this string s) => s.ToPNG() + s.ToLink();
        public static string ToLabledPNG(this string s) => s.ToPNG() + s;
        public static string ToItemPNG(this Item item, bool link = false, bool displayName = true, bool displayNum = false, string linkText = null) {
            string name;
            string file;
            string linkString = "";
            if (item.type < ItemID.Count) {
				//manually changing the item
				switch (item.type) {
					case ItemID.Fake_GoldChest:
						item = new Item(ItemID.GoldChest);
						name = "Locked Gold Chest";
						break;
                    case ItemID.Fake_ShadowChest:
                        item = new Item(ItemID.ShadowChest);
                        name = "Locked Shadow Chest";
                        break;
					default:
						name = item.Name;
						break;
				}

				file = $"Item_{item.type}";
				if (link)
					linkString = $"https://terraria.fandom.com/wiki/{item.Name.Replace(" ", "_")}".ToExternalLink(name);
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

                if (link) {
                    if (item.ModItem is Enchantment enchantment) {
                        linkString = enchantment.TierName.ToSectionLink(name, $"{enchantment.EnchantmentTypeName.AddSpaces()} Enchantment");
					}
					else {
                        linkString = name.ToLink();
                    }
                }
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
        public static string ToNpcPNG(this int npcNetID, bool link = false, bool displayName = true, bool displayPNG = true) {
            string name;
            string file = "";
            string pngLinkString = "";
            NPC npc = ContentSamples.NpcsByNetId[npcNetID];
            if (npcNetID < NPCID.Count) {
                if (displayPNG) {
                    file = npcNetID.GetNPCPNGLink();
                    if (file == "")
                        file = $"NPC_{npc.netID}".ToPNG();
                }

                name = npc.netID < 0 ? NPCID.Search.GetName(npc.netID).AddSpaces(true) : npc.FullName();
                if (link)
                    pngLinkString = $"https://terraria.fandom.com/wiki/{npc.FullName().Replace(" ", "_")}".ToExternalLink(name);
            }
            else {
                ModNPC modNPC = npc.ModNPC;
                string modNpcFullName = modNPC.FullName;
				name = npc.FullName();

                bool weaponEnchantmentsNpc = modNPC.Mod.Name == "WeaponEnchantments";

                if (displayPNG && weaponEnchantmentsNpc)
                    file = name.ToPNG();

                if (link)
                    pngLinkString = weaponEnchantmentsNpc ? name.ToLink() : modNpcFullName.GetModNpcLink().ToExternalLink(name);
            }

            return $"{file}{(file != null && link ? " " : "")}{(link ? pngLinkString : displayName ? " " + name : "")}";
		}
        public static string ToNpcPNG(this short npcNetID, bool link = false, bool displayName = true, bool displayPNG = true) =>
            ((int)npcNetID).ToNpcPNG(link, displayName, displayPNG);
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
                $"Failed to find an item for tileNum: {tileNum}".Log_WE();
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
        public static string ToWitchSellText(this SellCondition condition, string value) {
            string witch = ModContent.NPCType<Witch>().ToNpcPNG(link: true, displayPNG: false);
            switch (condition) {
                case SellCondition.Never:
                    return $"can never appear in the {witch}'s shop.";
                case SellCondition.Always:
                    return $"will always appear in the {witch}'s shop.";
                case SellCondition.IgnoreCondition:
                case SellCondition.AnyTime:
                    return $"can appear in the {witch}'s shop any time for {value}.";
                case SellCondition.AnyTimeRare:
                    return $"can appear in the {witch}'s shop any time for {value}, but is rare.";
                case SellCondition.PostKingSlime:
                    return $"can appear in the {witch}'s shop after the {NPCID.KingSlime.ToNpcPNG(link: true, displayPNG: false)} has been defeated for {value}.";
                case SellCondition.PostEyeOfCthulhu:
                    return $"can appear in the {witch}'s shop after the {NPCID.EyeofCthulhu.ToNpcPNG(link: true, displayPNG: false)} has been defeated for {value}.";
                case SellCondition.PostEaterOfWorldsOrBrainOfCthulhu:
                    return $"can appear in the {witch}'s shop after the {NPCID.EaterofWorldsHead.ToNpcPNG(link: true, displayPNG: false)} or {NPCID.BrainofCthulhu.ToNpcPNG(link: true, displayPNG: false)} have been defeated for {value}.";
                case SellCondition.PostSkeletron:
                    return $"can appear in the {witch}'s shop after {NPCID.Skeleton.ToNpcPNG(link: true, displayPNG: false)} has been defeated for {value}.";
                case SellCondition.PostQueenBee:
                    return $"can appear in the {witch}'s shop after the {NPCID.QueenBee.ToNpcPNG(link: true, displayPNG: false)} has been defeated for {value}.";
                case SellCondition.PostDeerclops:
                    return $"can appear in the {witch}'s shop after {NPCID.Deerclops.ToNpcPNG(link: true, displayPNG: false)} has been defeated for {value}.";
                case SellCondition.PostGoblinInvasion:
                    return $"can appear in the {witch}'s shop after the {InvasionID.Goblin_Army.ToVanillaWikiLink()} has been defeated for {value}.";
                case SellCondition.Luck:
                    return $"can appear in the {witch}'s shop any time for {value}, but is extremely rare.";
                case SellCondition.HardMode:
                    return $"can appear in the {witch}'s shop during hard mode for {value}.";
                case SellCondition.PostQueenSlime:
                    return $"can appear in the {witch}'s shop after the {NPCID.QueenSlimeBoss.ToNpcPNG(link: true, displayPNG: false)} has been defeated for {value}.";
                case SellCondition.PostPirateInvasion:
                    return $"can appear in the {witch}'s shop after a {InvasionID.Pirate_Invasion.ToVanillaWikiLink()} has been defeated for {value}.";
                case SellCondition.PostTwins:
                    return $"can appear in the {witch}'s shop after {"The_Twins".ToVanillaWikiLink()} have been defeated for {value}.";
                case SellCondition.PostDestroyer:
                    return $"can appear in the {witch}'s shop after {NPCID.TheDestroyer.ToNpcPNG(link: true, displayPNG: false)} has been defeated for {value}.";
                case SellCondition.PostSkeletronPrime:
                    return $"can appear in the {witch}'s shop after {NPCID.SkeletronPrime.ToNpcPNG(link: true, displayPNG: false)} has been defeated for {value}.";
                case SellCondition.PostPlantera:
                    return $"can appear in the {witch}'s shop after {NPCID.Plantera.ToNpcPNG(link: true, displayPNG: false)} has been defeated for {value}.";
                case SellCondition.PostGolem:
                    return $"can appear in the {witch}'s shop after the {NPCID.Golem.ToNpcPNG(link: true, displayPNG: false)} has been defeated for {value}.";
                case SellCondition.PostMartianInvasion:
                    return $"can appear in the {witch}'s shop after {InvasionID.Martian_Madness} has been defeated for {value}.";
                case SellCondition.PostDukeFishron:
                    return $"can appear in the {witch}'s shop after {NPCID.DukeFishron.ToNpcPNG(link: true, displayPNG: false)} has been defeated for {value}.";
                case SellCondition.PostEmpressOfLight:
                    return $"can appear in the {witch}'s shop after the {NPCID.HallowBoss.ToNpcPNG(link: true, displayPNG: false)} has been defeated for {value}.";
                case SellCondition.PostCultist:
                    return $"can appear in the {witch}'s shop after the {NPCID.CultistBoss.ToNpcPNG(link: true, displayPNG: false)} has been defeated for {value}.";
                case SellCondition.PostSolarTower:
                    return $"can appear in the {witch}'s shop after the {NPCID.LunarTowerSolar.ToNpcPNG(link: true, displayPNG: false)} has been defeated for {value}.";
                case SellCondition.PostNebulaTower:
                    return $"can appear in the {witch}'s shop after the {NPCID.LunarTowerNebula.ToNpcPNG(link: true, displayPNG: false)} has been defeated for {value}.";
                case SellCondition.PostStardustTower:
                    return $"can appear in the {witch}'s shop after the {NPCID.LunarTowerStardust.ToNpcPNG(link: true, displayPNG: false)} has been defeated for {value}.";
                case SellCondition.PostVortexTower:
                    return $"can appear in the {witch}'s shop after the {NPCID.LunarTowerVortex.ToNpcPNG(link: true, displayPNG: false)} has been defeated for {value}.";
                case SellCondition.PostMoonLord:
                    return $"can appear in the {witch}'s shop after the {NPCID.MoonLordHead.ToNpcPNG(link: true, displayPNG: false)} has been defeated for {value}.";
                default:
                    return "SellConditionTextNotFound";
            }
        }
    }
}
