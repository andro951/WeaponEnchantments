using androLib.Common.Utility;
using androLib.Common.Utility.LogSystem;
using androLib.Common.Utility.LogSystem.WebpageComponenets;
using androLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VacuumOreBag.Items;
using WeaponEnchantments.Common.Configs;
using WeaponEnchantments.Common.Utility.LogSystem.WebpageComponenets;
using WeaponEnchantments.Content.NPCs;
using WeaponEnchantments.Items;
using WeaponEnchantments.Items.Enchantments;
using androLib.Common;
using androLib;

namespace WeaponEnchantments.Common.Utility.LogSystem {
	public class WeaponEnchantmentsWiki : Wiki {
		public override bool ShouldPrintWiki => LogModSystem.printWiki;
		public override Func<Mod> GetMod => ModContent.GetInstance<WEMod>;
		private static bool tier0EnchantmentsOnly = false;
		public override string ModName => WEMod.ModName;
		protected override void AddWikiPages(List<WebPage> webPages, IEnumerable<ModItem> modItems) {
			IEnumerable<ContainmentItem> containmentItems = modItems.OfType<ContainmentItem>().OrderBy(c => c.tier);
			IEnumerable<EnchantingTableItem> enchantingTables = modItems.OfType<EnchantingTableItem>().OrderBy(t => t.enchantingTableTier);
			IEnumerable<EnchantmentEssence> enchantmentEssence = modItems.OfType<EnchantmentEssence>().OrderBy(e => e.EssenceTier);
			IEnumerable<Enchantment> enchantments = modItems.OfType<Enchantment>().OrderBy(e => e.Name);
			PowerBooster powerBooster = modItems.OfType<PowerBooster>().First();
			UltraPowerBooster ultraPowerBooster = modItems.OfType<UltraPowerBooster>().First();
			OreBag oreBag = ModContent.GetContent<OreBag>().First();

			AddMainPage(webPages);
			AddMagicStorageIntegration(webPages);
			AddItemExperience(webPages);
			AddContainments(webPages, containmentItems, enchantments);
			AddEnchantingTables(webPages, enchantingTables);
			AddEssence(webPages, enchantmentEssence);
			AddEnchantments(webPages, enchantments);
			AddPowerBooster(webPages, powerBooster);
			AddUltraPowerBooster(webPages, ultraPowerBooster);
			AddOreBag(webPages, oreBag);
			AddWitch(webPages, modItems.Where(m => m is ISoldByNPC soldByWitch && soldByWitch.SellCondition != SellCondition.Never));
		}
		private static void AddMainPage(List<WebPage> webPages) {
			WebPage mainPage = new(WebPage.MainPageName, mainWikiPage: "Weapon Enchantments Mod (tModLoader) Wiki");
			string fullMainPage =
				"<mainpage-leftcolumn-start />\r\n" +
				"<div style=\"text-align: center;>[[File:Icon.png]]</div>\r\n" +
				"<div style=\"text-align: center; font-size: x-large; padding: 1em;\">'''Welcome to the {{SITENAME}}!'''</div>\r\n" +
				"\r\n" +
				"If you are interested in contributing to this wiki, please let me know: https://discord.gg/hEKKVsFBMd - andro951\r\n" +
				"\r\n" +
				"=== Features ===\r\n" +
				"\r\n" +
				$"* Item Customization ({"Enchantments".ToLink()})\r\n" +
				$"* Progression System ({"Item Experience".ToLink()})\r\n" +
				$"* Item Upgrading ({"Infusion".ToSectionLink(page: "Enchanting Tables")})\r\n" +
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
				$"{"Magic Storage Integration".ToLink()}\r\n" +
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
		private static void AddMagicStorageIntegration(List<WebPage> webPages) {
			WebPage EnchantinTable = new("Magic Storage Integration", webPages.Where(wp => wp.HeaderName == WebPage.MainPageName).First());
			EnchantinTable.AddParagraph("Weapon Enchantments is fully integrated to Magic Storage as of Magic Storage version 0.5.7.7");
			EnchantinTable.AddSubHeading("Don'ts:");
			EnchantinTable.AddParagraph("Don't put an enchanting table into the Crafting Interface crafting station slot.  This is accomplished with the Storage Configuration Interface.");
			EnchantinTable.AddPNG("Crafting_Stations");
			EnchantinTable.AddParagraph("Don't put essence into the Magic Storage system.  Leave it in the Enchanting table.");
			EnchantinTable.AddPNG("Essence in Magic Storage", false);
			EnchantinTable.AddSubHeading("Storage Configuration Interface:");
			EnchantinTable.AddParagraph("The Storage Configuration Interface from magic storage is used by Weapon Enchantments for 2 things.");
			EnchantinTable.AddParagraph("Allow crafting in with the Crafting Interface to use Essence from in the Enchanting table as materials for crafting.");
			EnchantinTable.AddParagraph("Acts as the highest tier of enchanting table you have used for the purpose of crafting.");
			EnchantinTable.AddPNG("Environment Module", false);
			EnchantinTable.AddSubHeading("Toggle On/Off");
			EnchantinTable.AddParagraph("You can toggle these features to hide all of the enchanting related recipes if desired.  (I recommend leaving them on)");
			EnchantinTable.AddPNG("Environment Module Toggle", false);
			webPages.Add(EnchantinTable);
		}
		private static void AddItemExperience(List<WebPage> webPages) {
			WebPage itemExperience = new("Item Experience", webPages.Where(wp => wp.HeaderName == WebPage.MainPageName).First());
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
			WebPage Containments = new("Containments", webPages.Where(wp => wp.HeaderName == WebPage.MainPageName).First());
			Containments.AddParagraph("Containments contain the power of an enchantment. More powerful enchantments require larger and stronger containments to hold them.\n" +
			"Containments are crafting materials used to craft enchantments.");
			AddLowestCraftableEnchantments(Containments, enchantments);
			foreach (ContainmentItem containment in containmentItems) {
				int tier = containment.tier;
				string subHeading = $"{containment.Item.ToItemPNG()} (Tier {tier})";
				Containments.AddParagraph($"{containment.Item.ToItemPNG(link: true)} (Tier {tier})");
				WebPage containmentPage = new(containment.Item.Name, Containments);
				ItemInfo_WE itemInfo = new(containment);
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
			WebPage EnchantingTable = new("Enchanting Tables", webPages.Where(wp => wp.HeaderName == WebPage.MainPageName).First());
			foreach (EnchantingTableItem enchantingTable in enchantingTables) {
				int tier = enchantingTable.enchantingTableTier;
				EnchantingTable.AddParagraph($"{enchantingTable.Item.ToItemPNG(link: true)} (Tier {tier})");
				WebPage enchantingTablePage = new(enchantingTable.Item.Name, EnchantingTable);
				ItemInfo_WE itemInfo = new(enchantingTable);
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
				$"{"Offer".ToSectionLink("offer")} items, {"Siphon".ToSectionLink("siphon")} items, {"Infusion".ToSectionLink("infuse")} items, " +
				$"{"Enchantment Storage".ToSectionLink("store items")}, and {"Enchantment Loadouts".ToSectionLink("manage/apply enchantment loadouts")}." +
				$"");
			EnchantingTable.AddSubHeading("Leveling items up");
			EnchantingTable.AddParagraph($"Item experienced can be gained by {"".ToSectionLink("using the item", "")} or consuming essence in the enchanting table interface.\n" +
				$"* Level Up button {"LevelUpButton".ToPNG()} (The numbers above the level up button determine how many levels will be added per click of the level up button.  " +
				$"Will do nothing if you do not have enough essence for the selected number of levels.)\n" +
				$"# Place your item in the Item slot\n" +
				$"# Click the Level Up button\n" +
				$"# Enough essence is used to level your item up once.\n" +
				$"Essence can be favorited in the enchanting table interface to prevent it from being consumed by the level up button " +
				$"(ignored if you don't have enough total essence to level up without the favorited essence)<br/>\n" +
				$"* XP buttons {"XPButtons".ToPNG()}\n" +
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

			EnchantingTable.AddSubHeading("Siphon " + "SiphonButton".ToPNG());
			EnchantingTable.AddParagraph($"Siphoning an item removes all modifications to that item.\n" +
				$"Siphoning is different from Offering because Siphoning prevents the item from being destroyed.\n" +
				$"However, as a cost for saving the item, {ServerConfig.DefaultSiphonCost}% (configurable) of the experience is lost.  " +
				$"(This is reduced to 4x the value of the item as essence value if the number is lower to prevent spending massive amounts of essence to save a low value item.)\n" +
				$"# Place the item into the item slot.\n" +
				$"# Click siphon.\n" +
				$"# Essence, Enchantments and Power Boosters will be deposited into the enchanting table interface, and the infused item will be returned to you.");

			EnchantingTable.AddSubHeading("Infusion " + "InfusionButton".ToPNG());
			EnchantingTable.AddParagraph($"Allows you to consume a weapon to enhance the power of a lower tier weapon to about the same power as the " +
				$"consumed weapon or transfers the set bonus from a piece of armor to another.  Any experience/enchantments/power booster on a consumed item will be returned just like {"Offer".ToSectionLink("offer")}.");

			EnchantingTable.AddSubHeading("Weapon Infusion", 2);
			EnchantingTable.AddParagraph($"Allows you to consume a more powerful weapon to upgrade the damage of a lower power weapon.  \n" +
				$"Example, if you like Veilthorn more than your newer/stronger weapon, just infuse the new weapon into Veilthorn to upgrade it's damage instead of switching.  \n" +
				$"The damage bonus is based on the difference in infusion powers of the two weapons.\n" +
				$"Infusion Power - A weapon stat that is determined by looking at how the weapon is obtained such as crafting or enemy/chest drops.\n" +
				$"I manually set the infusion power of every gathered crafting material, every boss/enemy that drops a weapon/crafting material, and manually set any remaining ones." +
				$"The basis I use for determining the infusion power is when a weapon is available during progression.\n" +
				$"For instance, all weapons that are either dropped by Skeletron will have the same infusion power, and all weapon from the dungeon will be slightly higher than those dropped by Skeletron.\n" +
				$"This is not a perfect way to determine the power of a weapon.  If a weapon is relatively powerful compared to when you can get it, the weapon will be more powerful than others when infused." +
				$"Currently, the highest Infusion Power possible for vanilla weapons is from Zenith (1105).  Modded weapons can be up to a max of 1350 infusion power.\n" +
				$"\n" +
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

			EnchantingTable.AddPNG("EnchantmentLoadoutUI");
			EnchantingTable.AddParagraph($"Enchantment loadouts don't store items.<br/>\n" +
				$"They just save the type and tier of enchantment.  When creating an Enchantment Loadout, you can " +
				$"quickly fill the slots by shift left clicking enchantments from your storage to fill the next slot (The next slot to fill has a gold background when shift is held).  " +
				$"You can also hold an enchantment in your mouse and click it on a slot instead.  Clicking on a slot with no enchantment in your mouse will clear the slot.<br/>\n" +
				$"Add - Adds a new loadout (Max of 15).<br/>\n" +
				$"Add From Equipped Enchantments - Create a loadout by copying your equipped enchantments<br/>\n" +
				$"Loadout # - Clicking the Loadout button selects that loadout so you can see or edit it.<br/>\n" +
				$"All - All replaces all enchantments on your held item, armor and accessories with the enchantments from the loadout.  If any item isn't high enough level " +
				$"to support the enchantments for it's slot, the loadout will fail to load.<br/>\n" +
				$"Held Item/Armor/Accessories - These buttons to the same thing as the All button, but only load the specific enchantments for the selected category.<br/>");

			EnchantingTable.AddSubHeading("Efficiently Upgrading your enchantment loadout", 2);
			EnchantingTable.AddParagraph($"* Load a blank loadout to return all enchantments to the storage.\n" +
				$"* Favorite any enchantments you want to keep.\n" +
				$"* Revert All to Basic.\n" +
				$"* Quick Craft all desired enchantments.\n" +
				$"* Update and load your original loadout.");

			webPages.Add(EnchantingTable);
		}
		private static void AddEssence(List<WebPage> webPages, IEnumerable<EnchantmentEssence> enchantmentEssence) {
			WebPage Essence = new("Enchantment Essence", webPages.Where(wp => wp.HeaderName == WebPage.MainPageName).First());
			Essence.AddParagraph("Essence represents solidified experience and are automatically stored in the enchanting table interface when picked up. They can be used to...");
			Essence.AddBulletedList(elements: new string[] {
				"Crafting and Upgrading Enchantments".ToSectionLink("Upgrade enchantments", "Enchantments"),
				"Leveling items up".ToSectionLink("Infuse it's XP value into items", "Enchanting Tables")
			});
			foreach (EnchantmentEssence essence in enchantmentEssence) {
				int tier = essence.EssenceTier;
				Essence.AddParagraph($"{essence.Item.ToItemPNG(link: true)} (Tier {tier})");
				WebPage essencePage = new(essence.Item.Name, Essence);
				ItemInfo_WE itemInfo = new(essence);
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
			WebPage Enchantments = new("Enchantments", webPages.Where(wp => wp.HeaderName == WebPage.MainPageName).First());
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
					$"({"https://steamcommunity.com/sharedfiles/filedetails/?id=2563309347&searchtext=magic+storage".ToExternalLink("Magic Storage")} can access the essence via the Storage Configuration Interface.  See {"Magic Storage Integration".ToLink()})<br/>\n" +
					$"Each enchantment page has the specific crafting recipes for the enchantment.  These are the general recipes:<br/>\n" +
					$"Topaz can be any Common Gem: {$"{AndroMod.ModName}:{AndroModSystem.AnyCommonGem}".ToItemPNGs(link: true)}<br/>\n" +
					$"Amber can be any Rare Gem: {$"{AndroMod.ModName}:{AndroModSystem.AnyRareGem}".ToItemPNGs(link: true)}<br/>\n" +
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

			foreach (IEnumerable<Enchantment> list in enchantments.GroupBy(e => e.EnchantmentTypeName).Select(l => l.ToList().OrderBy(e => e.EnchantmentTier))) {
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
			WebPage PowerBooster = new("Power Booster", webPages.Where(wp => wp.HeaderName == WebPage.MainPageName).First());
			ItemInfo_WE itemInfo = new(powerBooster);
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
			WebPage PowerBooster = new("Ultra Power Booster", webPages.Where(wp => wp.HeaderName == WebPage.MainPageName).First());
			ItemInfo_WE itemInfo = new(ultraPowerBooster);
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
			WebPage OreBag = new("Ore Bag", webPages.Where(wp => wp.HeaderName == WebPage.MainPageName).First());
			ItemInfo_WE itemInfo = new(oreBag);
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
			WebPage WitchPage = new("Witch", webPages.Where(wp => wp.HeaderName == WebPage.MainPageName).First());
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
				.OrderBy(m => ((ISoldByNPC)m).SellCondition)
				.GroupBy(m => m.GetModItemCompairisonType().Name)
				.Select(g => g.ToList().OrderBy(m => EnchantingRarity.GetTierNumberFromName(m.Name)))
				.SelectMany(i => i)) {

				ItemInfo.TryGetShopPrice(modItem, out int price);
				sellItems.Add(new() { modItem.Item.ToItemPNG(link: true), price.GetCoinsPNG(), ((ISoldByNPC)modItem).SellCondition.ToString().AddSpaces() });
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
			List<RecipeData_WE> genericEnchantmentRecipes = new();
			List<int> enchantmentIDs = new() {
				ModContent.ItemType<DamageEnchantmentBasic>(),
				ModContent.ItemType<DamageEnchantmentCommon>(),
				ModContent.ItemType<DamageEnchantmentRare>(),
				ModContent.ItemType<DamageEnchantmentEpic>(),
				ModContent.ItemType<DamageEnchantmentLegendary>()
			};

			for (int i = 0; i < enchantmentIDs.Count; i++) {
				foreach (RecipeData_WE data in createItemRecipes[enchantmentIDs[i]].OfType<RecipeData_WE>()) {
					if (data.requiredItem.CommonList.Where(m => m.ModItem is EnchantmentEssence).Count() == 1) {
						genericEnchantmentRecipes.Add(data);
						continue;
					}
				}
			}

			List<List<string>> recipes = new() { new() { "Result", "Ingredients", "Crafting station" } };
			foreach (RecipeData_WE data in genericEnchantmentRecipes.OrderBy(rd => -rd.requiredTile.ToString().Length)) {
				ItemInfo.ConvertRecipeDataListToStringList(recipes, data);
			}

			for (int i = 0; i < recipes.Count; i++) {
				List<string> recipe = recipes[i];
				for (int k = 0; k < recipe.Count; k++) {
					string newString = recipes[i][k].Replace("Damage ", "").Replace("Damage", "");
					if (newString != recipes[i][k])
						recipes[i][k] = newString.Replace("[[Enchantment Basic]]", "Enchantment Basic").Replace("[[Enchantment Common]]", "Enchantment Common").Replace("[[Enchantment Rare]]", "Enchantment Rare").Replace("[[Enchantment Epic]]", "Enchantment Epic").Replace("[[Enchantment Legendary]]", "Enchantment Legendary");
				}
			}

			return recipes;
		}
	}

	public static class WeaponEnchantmentsWikiStaticMethods {
		public static string EnchantmentTypeShortLink(this string s) => $"{s.AddSpaces()} Enchantment".ToLink(s.AddSpaces());
		public static string EnchantmentTypeShortLink(this EnchantmentStat enchantmentStat) => enchantmentStat.ToString().EnchantmentTypeShortLink();
	}
}
