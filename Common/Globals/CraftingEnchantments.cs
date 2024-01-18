using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Items;
using WeaponEnchantments.UI;
using androLib.Common.Utility;
using androLib.Common;

namespace WeaponEnchantments.Common.Globals
{
	public class CraftingEnchantments : GlobalItem
	{
		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
            //Bars created from uncrafting Containments
			switch (entity.type) {
                case ItemID.SilverBar:
                case ItemID.TungstenBar:
                case ItemID.GoldBar:
                case ItemID.PlatinumBar:
                case ItemID.DemoniteBar:
                case ItemID.CrimtaneBar:
                    return true;
			}

			if(entity.ModItem == null)
                return false;

            ModItem modItem = entity.ModItem;
            return modItem is Enchantment or EnchantmentEssence;
		}

		public override void OnCreated(Item item, ItemCreationContext context) {
            if (context is RecipeItemCreationContext recipeCreationContext) {
                if (recipeCreationContext.ConsumedItems == null || recipeCreationContext.ConsumedItems.Count < 1)
                    return;
                 
                SortedDictionary<int, int> otherCraftedItems = EnchantmentStorage.crafting ? EnchantmentStorage.uncraftedExtraItems : new();
                
                GetOtherCraftedItems(otherCraftedItems, item, recipeCreationContext.ConsumedItems);
                
				if (!EnchantmentStorage.crafting) {
					foreach (KeyValuePair<int, int> pair in otherCraftedItems) {
						Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("Crafting"), pair.Key, pair.Value);
					}
				}
            }
        }

        public static void GetOtherCraftedItems(SortedDictionary<int, int> dict, Item item, IEnumerable<Item> consumedItems) {
			bool hasIngredientEnchantments = false;
			bool skippedFirst = false;
			int tier = -1;
			if (item?.ModItem is Enchantment enchantent) {
				hasIngredientEnchantments = enchantent.HasIngredientEnchantments;
				tier = enchantent.EnchantmentTier;
			}

			foreach (Item consumedItem in consumedItems) {
				//Ingredient Enchantments
				if (hasIngredientEnchantments && !skippedFirst) {
					if (consumedItem.ModItem is Enchantment consumedEnchantment) {
						if (tier == consumedEnchantment.EnchantmentTier) {
							skippedFirst = true;
							continue;
						}
						else {
							//Shouldn't be hit.  Indicates an issue
							hasIngredientEnchantments = false;
						}
					}
				}

				//Get other crafted items
				dict.AddOrCombine(GetOtherCraftedItems(item, consumedItem));
			}
		}
        private static SortedDictionary<int, int> GetOtherCraftedItems(Item item,  Item consumedItem) {
            SortedDictionary<int, int> dict = new();
            if (consumedItem?.ModItem is Enchantment consumedEnchantment) {
				int craftedSize;
                bool creatingEnchantmentWithIngredientEnchantments = false;
                if (item?.ModItem is Enchantment enchantment) {
                    craftedSize = enchantment.EnchantmentTier;
                    creatingEnchantmentWithIngredientEnchantments = enchantment.HasIngredientEnchantments;
                }
                else {
                    craftedSize = 0;
                }

                int consumedSize = consumedEnchantment.EnchantmentTier;
                if (craftedSize > consumedSize) {
                    //Upgraded Enchantment
                    if (consumedSize < 2) {
                        dict.AddOrCombine((ContainmentItem.IDs[consumedSize], 1));
                    }
                    else if (consumedSize == 3) {
                        dict.AddOrCombine((ItemID.Topaz, 2));
                    }
                }
                else {
					//Downgraded Enchantment (or same tier)
                    if (consumedEnchantment.HasIngredientEnchantments && !creatingEnchantmentWithIngredientEnchantments) {
                        //Ingredient Enchantments
                        foreach (int ingredientBaseEnchantmentType in consumedEnchantment.IngredientEnchantments) {
                            Item ingredientItem = ingredientBaseEnchantmentType.CSI();
                            if (ingredientItem.ModItem is Enchantment ingredientEnchantment) {
                                string typeName = ingredientEnchantment.EnchantmentTypeName;
                                string ingredientName = typeName + "Enchantment" + EnchantingRarity.tierNames[consumedEnchantment.EnchantmentTier];
                                if (ModContent.TryFind(WEMod.ModName, ingredientName, out ModItem ingredientModItem))
                                    dict.AddOrCombine((ingredientModItem.Type, 1));
                            }
                        }
					}
                    else {
						//Gems
						if (consumedSize == 4) {
							dict.AddOrCombine((ItemID.Amber, 1));
						}
						else if (consumedSize == 3) {
							dict.AddOrCombine((ItemID.Topaz, 2));
						}

						//Containment
						if (consumedSize >= 2) {
							if (craftedSize < 2 || creatingEnchantmentWithIngredientEnchantments && craftedSize == consumedSize)
								dict.AddOrCombine((ContainmentItem.IDs[2], 1));
						}
						else {
							dict.AddOrCombine((ContainmentItem.IDs[consumedSize], 1));
						}

						//Essence
						int essenceNumber = consumedEnchantment.EssenceQuantityWithIngredientEnchantments;
						for (int k = craftedSize + 1; k <= consumedSize; k++) {
							dict.AddOrCombine((EnchantmentEssence.IDs[k], essenceNumber));
						}
					}
				}
            }
            else if (consumedItem?.ModItem is ContainmentItem containment) {
                if (containment.tier == 2 && item.type == ContainmentItem.barIDs[0, 2])
                    dict.AddOrCombine((ItemID.Topaz, 4));
            }

            return dict;
        }
	}
}
