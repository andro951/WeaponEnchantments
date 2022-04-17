using System;
using Terraria;
using Terraria.ModLoader;
using WeaponEnchantments.Common;

namespace WeaponEnchantments.Items
{
	public class EnchantmentEssence : ModItem
	{
		protected int essenceRarity = -1;
		public override void SetDefaults()
		{
			Item.value = (int)(100 * Math.Pow(8, essenceRarity));
			Tooltip.SetDefault("Item value: " + Item.value.ToString());
			Item.maxStack = 100000;
			//DisplayName.SetDefaults("");
			//Tooltip.SetDefault("");
			switch (essenceRarity)
			{
				case 0:
					Item.width = 4;
					Item.height = 4;
					break;
				case 1:
					Item.width = 8;
					Item.height = 8;
					break;
				case 2:
					Item.width = 12;
					Item.height = 12;
					break;
				case 3:
					Item.width = 16;
					Item.height = 16;
					break;
				case 4:
					Item.width = 20;
					Item.height = 20;
					break;
			}
		}
		public override void AddRecipes()
		{
			for (int i = 0; i <= 4; i++)
			{
				if (essenceRarity > -1)
				{
					//Dont sell common/uncommon/rare with NPC!!!
					Recipe recipe = CreateRecipe();
					if (essenceRarity > 0)
					{
						recipe.AddIngredient(Mod, "EnchantmentEssence" + Utility.rarity[essenceRarity - 1], 8 - i);
					}
					recipe.AddTile(Mod, Utility.enchantingTableNames[i] + "EnchantingTable"); //Put this inside if(essenceRarity >0) when not testing
					recipe.Register(); //Put this inside if(essenceRarity >0) when not testing
				}
				if (essenceRarity < 4 && essenceRarity > -1)
				{
					Recipe recipe = CreateRecipe();
					recipe.AddIngredient(Mod, "EnchantmentEssence" + Utility.rarity[essenceRarity + 1], 1);
					recipe.createItem.stack = 2 + i / 2;
					recipe.AddTile(Mod, Utility.enchantingTableNames[i] + "EnchantingTable");
					recipe.Register();
				}
			}
		}
		public class EnchantmentEssenceCommon : EnchantmentEssence
		{
			EnchantmentEssenceCommon() { essenceRarity = 0; }
		}
		public class EnchantmentEssenceUncommon : EnchantmentEssence
		{
			EnchantmentEssenceUncommon() { essenceRarity = 1; }
		}
		public class EnchantmentEssenceRare : EnchantmentEssence
		{
			EnchantmentEssenceRare() { essenceRarity = 2; }
		}
		public class EnchantmentEssenceSuperRare : EnchantmentEssence
		{
			EnchantmentEssenceSuperRare() { essenceRarity = 3; }
		}
		public class EnchantmentEssenceUltraRare : EnchantmentEssence
		{
			EnchantmentEssenceUltraRare() { essenceRarity = 4; }
		}
	}
}
