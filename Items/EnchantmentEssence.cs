using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common;

namespace WeaponEnchantments.Items
{
	public class EnchantmentEssence : ModItem
	{
		public int essenceRarity = -1;
		public static string[] rarity = new string[5] { "Basic", "Common", "Rare", "SuperRare", "UltraRare" };
		public static int[] IDs = new int[rarity.Length];
		public const int maxStack = 100000;
		public static float[] values = new float[rarity.Length];
		public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
        public override void SetStaticDefaults()
        {
            for(int i = 0; i < rarity.Length; i++)
            {
				values[i] = (float)(100 * Math.Pow(8, essenceRarity));
			}
        }
        public override void SetDefaults()
		{
			if(essenceRarity > -1)
            {
				Item.value = (int)values[essenceRarity];
				//Tooltip.SetDefault("Item value: " + Item.value.ToString());
				Tooltip.SetDefault(rarity[essenceRarity] + " material for crafting and upgrading enchantments.");
				Item.maxStack = maxStack;
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
		}
		public override void AddRecipes()
		{
			for (int i = 0; i < rarity.Length; i++)
			{
				if (essenceRarity > -1)
				{
					//Dont sell basic/common/rare with NPC!!!
					Recipe recipe = CreateRecipe();
					if (essenceRarity > 0)
					{
						recipe.AddIngredient(Mod, "EnchantmentEssence" + rarity[essenceRarity - 1], 8 - i);
						recipe.AddTile(Mod, EnchantingTableItem.enchantingTableNames[i] + "EnchantingTable"); //Put this inside if(essenceRarity >0) when not testing
						recipe.Register(); //Put this inside if(essenceRarity >0) when not testing
					}
					

					if (essenceRarity < rarity.Length - 1)
					{
						recipe = CreateRecipe();
						recipe.AddIngredient(Mod, "EnchantmentEssence" + rarity[essenceRarity + 1], 1);
						recipe.createItem.stack = 2 + i / 2;
						recipe.AddTile(Mod, EnchantingTableItem.enchantingTableNames[i] + "EnchantingTable");
						recipe.Register();
					}
					IDs[essenceRarity] = this.Type;
				}
			}
		}

		public static int GetEssenceTier(Item I)
        {
			return I.type - IDs[0];//Cheating instead of doing a for loop
        }
		public class EnchantmentEssenceBasic : EnchantmentEssence
		{
			EnchantmentEssenceBasic() { essenceRarity = 0; }
		}
		public class EnchantmentEssenceCommon : EnchantmentEssence
		{
			EnchantmentEssenceCommon() { essenceRarity = 1; }
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
