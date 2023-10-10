﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Items;
using androLib.Common.Utility;
using androLib.Common.Utility.LogSystem;
using Terraria.ModLoader;

namespace WeaponEnchantments.Common.Utility.LogSystem {
	public class RecipeData_WE : RecipeData {

		public static void RegisterWithRecipeData(Mod mod) {
			RegisterConstructor(mod, (recipe) => new RecipeData_WE(recipe));
		}
		public RecipeData_WE(Recipe recipe) : base(recipe) {

		}
		protected override List<Item> GetOtherCraftedItems(Item item, Item consumedItem) => CraftingEnchantments.GetOtherCraftedItems(item, consumedItem).Select(p => new Item(p.Key, p.Value)).ToList();
		public override bool TryCondenseRecipe(RecipeData other) {
			if (createItem.Count == 1 && requiredItem.Count == 1) {
				if (createItem.All[0].ModItem is EnchantmentEssence && requiredItem.All[0].ModItem is EnchantmentEssence)
					return false;
			}

			return base.TryCondenseRecipe(other);
		}
		public override bool TryAdd(RecipeData other) {
			if (createItem.Count == 1 && requiredItem.Count == 1) {
				if (createItem.All[0].ModItem is EnchantmentEssence && requiredItem.All[0].ModItem is EnchantmentEssence)
					return false;
			}

			return base.TryAdd(other);
		}
	}
}
