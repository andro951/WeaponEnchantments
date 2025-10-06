using androLib.Common.Utility.LogSystem;
using androLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using WeaponEnchantments.Items;

namespace WeaponEnchantments.Common.Utility.LogSystem {
	public class ItemInfo_WE : ItemInfo {
		public ItemInfo_WE(AndroModItem androModItem) : base(androModItem) {

		}
		protected override void PreCondenseModifyRecipeData(RecipeData data) {
			EnchantingTableItem enchantingTableItem = null;
			foreach (Item item in data.requiredTile.All) {
				if (item.ModItem is EnchantingTableItem enchantingTableItem2) {
					enchantingTableItem = enchantingTableItem2;
					break;
				}
			}

			if (enchantingTableItem != null && data.requiredTile.UniqueList == null && data.requiredTile.CommonList.Count == 1) {
				data.requiredTile.AddUnique(data.requiredTile.CommonList[0]);
				data.requiredTile.CommonList.Clear();

				for (int i = enchantingTableItem.enchantingTableTier + 1; i < EnchantingTableItem.enchantingTableNames.Length; i++) {
					int tableType = EnchantingTableItem.IDs[i];
					bool containsTable = false;
					foreach (Item item in data.requiredTile.All) {
						if (item.type == tableType) {
							containsTable = true;
							break;
						}
					}

					if (!containsTable)
						data.requiredTile.UniqueList.Add(new(tableType));
				}
			}
		}
		protected override bool TryAddRecipeData(List<List<string>> myRecipes, List<RecipeData> myRecipeData, bool includeReverse, bool usedIn) {
			List<RecipeData> myRecipeData2 = myRecipeData.Where(rd => rd.requiredTile.ToString().Contains("Enchanting Table"))
				.OrderBy(rd => (rd.requiredTile.ToString().Contains("<br/>or<br/>") ? -rd.requiredTile.ToString().Length : 10 * EnchantingTableItem.GetTableTier(rd.requiredTile.ToString())) - (usedIn ? rd.createItem.All.FirstEnchantmentStrength() : rd.requiredItem.All.FirstEnchantmentStrength()))
				.ToList();

			List<RecipeData> myRecipeData3 = myRecipeData
				.Where(rd => !rd.requiredTile.ToString().Contains("Enchanting Table"))
				.OrderBy(rd => rd.requiredItem.ToString().Length)
				.ToList();


			foreach (RecipeData recipeData in myRecipeData3.Concat(myRecipeData2)) {
				int createItemCount = recipeData.createItem.Count;
				bool requiredContainsItem = recipeData.requiredItem.Contains(Item);
				bool createItemCountLarger = recipeData.requiredItem.Count > 1 && createItemCount > recipeData.requiredItem.Count && requiredContainsItem;
				Enchantment firstRequiredEnchantment = recipeData.requiredItem.All.Select(i => i.ModItem).OfType<Enchantment>().FirstOrDefault(defaultValue: null);
				IEnumerable<ModItem> createModItems = recipeData.createItem.All.Select(i => i.ModItem);
				int createdEssenceTier = createModItems.OfType<EnchantmentEssence>().FirstOrDefault(defaultValue: null)?.EssenceTier ?? -1;
				bool reverseCraftOfCombinationEnchantment = firstRequiredEnchantment?.HasIngredientEnchantments == true && createdEssenceTier == firstRequiredEnchantment.EnchantmentTier;
				bool downGradeEnchanmtent = firstRequiredEnchantment?.EnchantmentTier > createModItems.OfType<Enchantment>().FirstOrDefault(defaultValue: null)?.EnchantmentTier || firstRequiredEnchantment != null && createdEssenceTier == 0;
				bool reverseRecipe = recipeData.IsReverseRecipe(Item.type) || createItemCountLarger || downGradeEnchanmtent || reverseCraftOfCombinationEnchantment;
				if (includeReverse && !reverseRecipe || !includeReverse && reverseRecipe)
					continue;

				ConvertRecipeDataListToStringList(myRecipes, recipeData);
			}

			return true;
		}
	}
	public static class ItemInfoStaticMethods {
		public static int FirstEnchantmentStrength(this IEnumerable<Item> items) {
			Enchantment first = items.Select(i => i.ModItem).OfType<Enchantment>().FirstOrDefault(defaultValue: null);
			//$"Enchantment Name: {first?.Name}, tier: {first?.EnchantmentTier ?? -1}".LogSimple();
			return first?.EnchantmentTier ?? -1;
		}
	}
}
