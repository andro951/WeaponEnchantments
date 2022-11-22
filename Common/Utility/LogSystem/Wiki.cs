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
using WeaponEnchantments.Content.NPCs;

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
        private static bool tier0EnchantmentsOnly = false;
        public static void PrintWiki() {
            if (!LogModSystem.printWiki)
                return;

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

            List<WebPage> webPages = new();

            AddContainments(webPages, containmentItems, enchantments);
            AddEnchantingTables(webPages, enchantingTables);
            AddEssence(webPages, enchantmentEssence);
            AddEnchantments(webPages, enchantments);
            AddPowerBooster(webPages, powerBooster);
            AddUltraPowerBooster(webPages, ultraPowerBooster);
            AddWitch(webPages, modItems.Where(m => m is ISoldByWitch soldByWitch && soldByWitch.SellCondition != SellCondition.Never));

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
            AddLowestCraftableEnchantments(Containments, enchantments);
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
            WebPage EnchantingTable = new("Enchanting Tables");
            foreach (EnchantingTableItem enchantingTable in enchantingTables) {
                int tier = enchantingTable.enchantingTableTier;
                EnchantingTable.AddParagraph($"{enchantingTable.Item.ToItemPNG(link: true)} (Tier {tier})");
                WebPage enchantingTablePage = new(enchantingTable.Item.Name);
                ItemInfo itemInfo = new(enchantingTable);
                enchantingTablePage.AddLink("Enchanting Tables");
                itemInfo.AddStatistics(enchantingTablePage);
                itemInfo.AddDrops(enchantingTablePage);
                itemInfo.AddInfo(enchantingTablePage);
                itemInfo.AddRecipes(enchantingTablePage);
                webPages.Add(enchantingTablePage);
            }

            EnchantingTable.AddParagraph($"All of the essence you pick up are stored in the enchanting table interface. " +
				$"Right clicking the enchanting table opens the interface. In an enchanting table you can {"Crafting and Upgrading Enchantments".ToSectionLink("create and upgrade", "Enchantments")} enchantments via crafting, " +
				$"{"Enchanting items".ToSectionLink("apply and remove")} enchantments, {"Leveling items up".ToSectionLink("convert essence to item experience")}, " +
				$"{"Offer".ToSectionLink("offer")} items, {"Syphon".ToSectionLink("syphon")} items and {"Infusion".ToSectionLink("infuse")} items." +
				$"");
            EnchantingTable.AddSubHeading("Leveling items up");
            EnchantingTable.AddParagraph($"Item experienced can be gained by {"".ToSectionLink("using the item", "")} or consuming essence in the enchanting table interface.\n" +
				$"* Level Up button { "LevelUpButton".ToPNG()} (Will do nothing if you do not have enough essence for 1 level)\n" +
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
                $"It gives half ore and half essence by default (configurable).  It also returns all experience as essence and returns all enchantments and an installed power booster.  \n" + 
                $"# Place the item you want to offer in the item slot." + 
                $"# press the offer button." +
				$"# Press confirm." +
				$"# You receive the offer rewards items.");

            EnchantingTable.AddSubHeading("Syphon " + "SyphonButton".ToPNG());
            EnchantingTable.AddParagraph($"If you have a max level item (level 40), you can convert any excess experience on the item past level 40 " +
				$"(past 100M experience) into essence.\n" +
				$"# Place the max level item into the item slot." +
				$"# Click syphon." +
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

            webPages.Add(EnchantingTable);
        }
        private static void AddEssence(List<WebPage> webPages, IEnumerable<EnchantmentEssence> enchantmentEssence) {
            WebPage Essence = new("Enchantment Essence");
            Essence.AddParagraph("Essence represents solidified experience and are automatically stored in the enchanting table interface when picked up. They can be used to...");
            Essence.AddBulletedList(elements: new string[] {
                "Crafting and Upgrading Enchantments".ToSectionLink("Upgrade enchantments", "Enchantments"),
                "Leveling items up".ToSectionLink("Infuse it's XP value into items", "Enchanting Tables")
            });
            foreach (EnchantmentEssence essence in enchantmentEssence) {
                int tier = essence.EssenceTier;
                Essence.AddParagraph($"{essence.Item.ToItemPNG(link: true)} (Tier {tier})");
                WebPage essencePage = new(essence.Item.Name);
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
            WebPage Enchantments = new("Enchantments");
            WebPage UtilityEnchantments = new("Utility Enchantments");
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
                    $"Topaz can be any Common Gem: {"WeaponEnchantments:CommonGems".ToItemPNGs(link: true)}<br/>\n" +
                    $"Amber can be any Rare Gem: {"WeaponEnchantments:RareGems".ToItemPNGs(link: true)}<br/>\n" +
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
                    
                    //if (first) {
                    //    first = false;
                    //    string enchantmentType = enchantment.EnchantmentTypeName.AddSpaces() + " Enchantment";
                    //    if (!tier0EnchantmentsOnly) {
                    //        enchantmentTypePage = new(enchantmentType);
                    //        enchantmentTypePage.AddLink("Enchantments");
                    //        if (enchantment.WikiDescription != null)
                    //            enchantmentTypePage.AddParagraph(enchantment.WikiDescription);

                    //        string typePNG = enchantment.Item.ToItemPNG(link: true, linkText: enchantmentType);
                    //        Enchantments.AddParagraph(typePNG);
                    //        if (enchantment.Utility)
                    //            UtilityEnchantments.AddParagraph(typePNG);
                    //    }
                        
                    //    typePageLinkString = enchantmentType.ToLink();
                    //}

                    //int tier = enchantment.EnchantmentTier;
                    //if (tier != 0 && tier0EnchantmentsOnly)
                    //    continue;

                    //if (!tier0EnchantmentsOnly)
                    //    enchantmentTypePage.AddParagraph($"{enchantment.Item.ToItemPNG(link: true)} (Tier {tier})");

                    //ItemInfo itemInfo = new(enchantment);
                    //WebPage enchantmentPage = new(itemInfo.Name);
                    //enchantmentPage.AddLink("Enchantments");
                    //enchantmentPage.AddParagraph(typePageLinkString);
                    //itemInfo.AddStatistics(enchantmentPage);
                    //itemInfo.AddDrops(enchantmentPage);
                    //itemInfo.AddInfo(enchantmentPage);
                    //itemInfo.AddRecipes(enchantmentPage);
                    //webPages.Add(enchantmentPage);
                }
                
                WebPage enchantmentPage = new(enchantmentInfoBox.Name);
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

            WebPage AllEnchantmentDrops = new("All Enchantment Drops");
            AllEnchantmentDrops.AddLink("Enchantments");
            ItemInfo.AddAllDrops(AllEnchantmentDrops, typeof(Enchantment));
            webPages.Add(AllEnchantmentDrops);
        }
        private static void AddPowerBooster(List<WebPage> webPages, PowerBooster powerBooster) {
            WebPage PowerBooster = new("Power Booster");
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
            WebPage PowerBooster = new("Ultra Power Booster");
            ItemInfo itemInfo = new(ultraPowerBooster);
            itemInfo.AddStatistics(PowerBooster);
            itemInfo.AddDrops(PowerBooster);
            itemInfo.AddInfo(PowerBooster);
            itemInfo.AddRecipes(PowerBooster);
            PowerBooster.AddParagraph($"<br/>\n" +
                $"A Power Booster is a rare item obtained from defeating post Plantera bosses (inlcudes Plantera).  " +
                $"It adds 20 levels to an item " +
                $"(These levels do not count towards the level 40 cap or give critical strike chance.).  They can only be used once per item.  " +
                $"To apply an ultra power booster to an item, place the item into the enchanting table then click the ultra power booster onto the item in the table as if you were " +
                $"merging two stacks of the same item.  You can also shift left click the power booster from your inventory.  Ultra Power boosters drop from boss bags in " +
                $"Expert/Master mode at the same rates from the table.");

            webPages.Add(PowerBooster);
        }
        private static void AddWitch(List<WebPage> webPages, IEnumerable<ModItem> modItems) {
            WebPage WitchPage = new("Witch");
            NPCInfo npcInfo = new(ModContent.NPCType<Witch>());
            npcInfo.AddStatistics(WitchPage);
            WitchPage.AddParagraph(
                $"The Witch is a town npc that sells a variety of items.  " +
				$"She sells containments and enchantment essence and a selection of enchantments.  The enchantents are " +
				$"chosen randomly from the ones that are allowed to be sold.  This includes all enchantments with a sell condition " +
				$"of \"Always\", 4 enchantments with a sell condtion of \"Any Time\", and 2 from the rare pool.  The rare pool " +
				$"consists of the \"Any Time Rare\" enchantments and all other enchantments if the sell condition for them is met.  " +
				$"Her shop resets as soon as the the game changes from night to day.  This happens at 4:30 am normally, but if using " +
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
                .GroupBy(m => m.TypeAboveModItem().Name)
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
            webPage.AddBulletedList(true, true, enchantments.Where(e => e.LowestCraftableTier == 0 && e.EnchantmentTier == 0).Select(c => c.Name.AddSpaces()).ToArray());
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
        public static string ToSectionLink(this string s, string text = null, string page = null) => $"{(page != null ? page : "")}#{s}".ToLink(text);
        public static string EnchantmentTypeShortLink(this string s) => $"{s.AddSpaces()} Enchantment".ToLink(s.AddSpaces());
        public static string EnchantmentTypeShortLink(this EnchantmentStat enchantmentStat) => enchantmentStat.ToString().EnchantmentTypeShortLink();
        public static string ToExternalLink(this string s, string text = null) => $"[{s}{(text != null ? $" {text}" : "")}]";
        public static string ToVanillaWikiLink(this string s, string text = null) => $"https://terraria.fandom.com/wiki/{s}".ToExternalLink(text != null ? text : s.Replace('_', ' '));
        public static string ToVanillaWikiLink(this InvasionID id, string text = null) => $"{id}".ToVanillaWikiLink(text);
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

                name = npc.netID < 0 ? NPCID.Search.GetName(npc.netID).AddSpaces(true) : npc.FullName;
                if (link)
                    pngLinkString = $"https://terraria.fandom.com/wiki/{npc.FullName.Replace(" ", "_")}".ToExternalLink(name);
            }
            else {
                ModNPC modNPC = npc.ModNPC;
                if (modNPC == null) {
                    name = npc.FullName;
                }
                else {
                    name = modNPC.Name.AddSpaces();
                }

                if (displayPNG)
                    file = name.ToPNG();

                if (link)
                    pngLinkString = name.ToLink();
            }

            return $"{file}{(link ? " " + pngLinkString : displayName ? " " + name : "")}";
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
