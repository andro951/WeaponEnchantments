using System;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using WeaponEnchantments.Common;

namespace WeaponEnchantments.Items
{
	public class EnchantmentEssenceBasic : ModItem
	{
		public int essenceRarity = -1;
		public static string[] rarity = new string[5] { "Basic", "Common", "Rare", "SuperRare", "UltraRare" };
		public static int[] IDs = new int[rarity.Length];
		public const int maxStack = 9999;
		public static float[] values = new float[rarity.Length];
		public static float[] xpPerEssence = new float[rarity.Length];
		public static float valuePerXP;
		public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
        public override void SetStaticDefaults()
        {
            for(int i = 0; i < rarity.Length; i++)
            {
				values[i] = (float)(25 * Math.Pow(8, i));
				xpPerEssence[i] = (float)(400 * Math.Pow(4, i));
			}
			valuePerXP = (values[rarity.Length - 1] / xpPerEssence[rarity.Length - 1]);
			GetDefaults();
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
			Tooltip.SetDefault(AllForOneEnchantmentBasic.displayRarity[essenceRarity].AddSpaces() + " material for crafting and upgrading enchantments.\nCan be converted to " + xpPerEssence[essenceRarity] + " experience in an enchanting table.");
			if (!WEMod.clientConfig.UseOldRarityNames)
				DisplayName.SetDefault(UtilityMethods.AddSpaces(Name.Substring(0, Name.IndexOf(rarity[essenceRarity])) + AllForOneEnchantmentBasic.displayRarity[essenceRarity]));
		}
        private void GetDefaults()
        {
			for (int i = 0; i < rarity.Length; i++)
			{
				if (rarity[i] == Name.Substring(Name.IndexOf("Essence") + 7))
				{
					essenceRarity = i;
					break;
				}
			}
		}
		public override void SetDefaults()
		{
			GetDefaults();
			Item.value = (int)values[essenceRarity];
			Item.maxStack = maxStack;
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
			for (int i = 0; i < rarity.Length; i++)
			{
				if (essenceRarity > -1)
				{
					//Dont sell basic/common/rare with NPC!!!
					Recipe recipe = CreateRecipe();
					if (essenceRarity > 0)
					{
						recipe.AddIngredient(Mod, "EnchantmentEssence" + rarity[essenceRarity - 1], 8 - i);
						recipe.AddTile(Mod, WoodEnchantingTable.enchantingTableNames[i] + "EnchantingTable"); //Put this inside if(essenceRarity >0) when not testing
						recipe.Register(); //Put this inside if(essenceRarity >0) when not testing
					}
					

					if (essenceRarity < rarity.Length - 1)
					{
						recipe = CreateRecipe();
						recipe.AddIngredient(Mod, "EnchantmentEssence" + rarity[essenceRarity + 1], 1);
						recipe.createItem.stack = 2 + i / 2;
						recipe.AddTile(Mod, WoodEnchantingTable.enchantingTableNames[i] + "EnchantingTable");
						recipe.Register();
					}
					IDs[essenceRarity] = this.Type;
				}
			}
		}
    }
	public class EnchantmentEssenceCommon : EnchantmentEssenceBasic { }
	public class EnchantmentEssenceRare : EnchantmentEssenceBasic { }
	public class EnchantmentEssenceSuperRare : EnchantmentEssenceBasic { }
	public class EnchantmentEssenceUltraRare : EnchantmentEssenceBasic { }
}
