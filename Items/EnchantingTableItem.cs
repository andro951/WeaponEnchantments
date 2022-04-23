using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common;

namespace WeaponEnchantments.Items
{
	public class EnchantingTableItem : ModItem
	{
		public int enchantingTableTier = -1;
		public static string[] enchantingTableNames = new string[5] { "Wood", "Dusty", "Hellish", "Soul", "Ultimate" };
		public static int[] IDs = new int[enchantingTableNames.Length];
		public override void SetStaticDefaults()
		{
			if (enchantingTableTier > -1)
			{
				DisplayName.SetDefault(EnchantingTableItem.enchantingTableNames[enchantingTableTier] + " Enchanting Table");
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
					recipe.AddIngredient(Mod, EnchantingTableItem.enchantingTableNames[enchantingTableTier - 1] + "EnchantingTable", 1);
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
				ModItemID.Add(this.Name, this.Type);
				IDs[enchantingTableTier] = this.Type;
			}
		}

		public class WoodEnchantingTable : EnchantingTableItem
		{
			WoodEnchantingTable() { enchantingTableTier = 0; }
		}
		public class DustyEnchantingTable : EnchantingTableItem
		{
			DustyEnchantingTable() { enchantingTableTier = 1; }
		}
		public class HellishEnchantingTable : EnchantingTableItem
		{
			HellishEnchantingTable() { enchantingTableTier = 2; }
		}
		public class SoulEnchantingTable : EnchantingTableItem
		{
			SoulEnchantingTable() { enchantingTableTier = 3; }
		}
		public class UltimateEnchantingTable : EnchantingTableItem
		{
			UltimateEnchantingTable() { enchantingTableTier = 4; }
		}
	}
}
