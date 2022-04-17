using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common;

namespace WeaponEnchantments.Items
{
	public class EnchantingTable : ModItem
	{
		public int enchantingTableTier = -1;

		public override void SetStaticDefaults()
		{
			if (enchantingTableTier > -1)
			{
				DisplayName.SetDefault(Utility.enchantingTableNames[enchantingTableTier] + " Enchanting Table");
			}
		}
		public override void SetDefaults()
		{
			if (enchantingTableTier > -1)
			{
				switch (enchantingTableTier)
				{
					case 0:
						Item.createTile = ModContent.TileType<Tiles.Furniture.EnchantingTableTile.WoodEnchantingTable>();
						break;
					case 1:
						Item.createTile = ModContent.TileType<Tiles.Furniture.EnchantingTableTile.DustyEnchantingTable>();
						break;
					case 2:
						Item.createTile = ModContent.TileType<Tiles.Furniture.EnchantingTableTile.HellishEnchantingTable>();
						break;
					case 3:
						Item.createTile = ModContent.TileType<Tiles.Furniture.EnchantingTableTile.SoulEnchantingTable>();
						break;
					case 4:
						Item.createTile = ModContent.TileType<Tiles.Furniture.EnchantingTableTile.UltimateEnchantingTable>();
						break;
				}
			}
			Item.width = 28;
			Item.height = 14;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 10;
			Item.useAnimation = 15;
			Item.consumable = true;
			Item.value = 150;
		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			if (enchantingTableTier > -1)
			{
				if (enchantingTableTier > 0)
				{
					recipe.AddTile(TileID.WorkBenches);
					recipe.AddIngredient(Mod, Utility.enchantingTableNames[enchantingTableTier - 1] + "EnchantingTable", 1);
				}
				switch (enchantingTableTier)
				{
					case 0:
						recipe.AddIngredient(36, 1); //Workbench
						recipe.AddIngredient(8, 4); //Torches
						break;
					case 1:
						recipe.AddIngredient(3374, 1); //Fossil Helm
						break;
					case 2:
						recipe.AddIngredient(193, 1); //Obsidian Skull
						break;
					case 3:
						recipe.AddIngredient(520, 1); //Soul of Light
						recipe.AddIngredient(521, 1); //Soul of Night
						break;
					case 4:
						recipe.AddIngredient(1225, 2); //Hallowed Bars
						break;
				}
				recipe.Register();
			}
		}

		public class WoodEnchantingTable : EnchantingTable
		{
			WoodEnchantingTable() { enchantingTableTier = 0; }
		}
		public class DustyEnchantingTable : EnchantingTable
		{
			DustyEnchantingTable() { enchantingTableTier = 1; }
		}
		public class HellishEnchantingTable : EnchantingTable
		{
			HellishEnchantingTable() { enchantingTableTier = 2; }
		}
		public class SoulEnchantingTable : EnchantingTable
		{
			SoulEnchantingTable() { enchantingTableTier = 3; }
		}
		public class UltimateEnchantingTable : EnchantingTable
		{
			UltimateEnchantingTable() { enchantingTableTier = 4; }
		}
	}
}
