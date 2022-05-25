﻿using Terraria;
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
		public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
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
						Item.createTile = ModContent.TileType<Tiles.WoodEnchantingTable>();
						break;
					case 1:
						Item.createTile = ModContent.TileType<Tiles.DustyEnchantingTable>();
						break;
					case 2:
						Item.createTile = ModContent.TileType<Tiles.HellishEnchantingTable>();
						break;
					case 3:
						Item.createTile = ModContent.TileType<Tiles.SoulEnchantingTable>();
						break;
					case 4:
						Item.createTile = ModContent.TileType<Tiles.UltimateEnchantingTable>();
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
					//recipe.AddTile(TileID.WorkBenches);
					recipe.AddIngredient(Mod, EnchantingTableItem.enchantingTableNames[enchantingTableTier - 1] + "EnchantingTable", 1);
				}
				switch (enchantingTableTier)
				{
					case 0:
						recipe.AddIngredient(ItemID.WorkBench, 1); //Workbench
						recipe.AddIngredient(ItemID.Torch, 4); //Torches
						break;
					case 1:
						recipe.AddIngredient(ItemID.DesertFossil, 10); //Fossil Helm
						recipe.Register();
						recipe = CreateRecipe();
						recipe.AddIngredient(Mod, EnchantingTableItem.enchantingTableNames[enchantingTableTier - 1] + "EnchantingTable", 1);
						recipe.AddIngredient(ItemID.FossilOre, 1);
						break;
					case 2:
						recipe.AddIngredient(ItemID.ObsidianSkull, 1); //Obsidian Skull
						break;
					case 3:
						recipe.AddIngredient(ItemID.SoulofLight, 2); //Soul of Light
						recipe.Register();
						recipe = CreateRecipe();
						recipe.AddIngredient(Mod, EnchantingTableItem.enchantingTableNames[enchantingTableTier - 1] + "EnchantingTable", 1);
						recipe.AddIngredient(ItemID.SoulofNight, 2); //Soul of Night
						recipe.Register();
						recipe = CreateRecipe();
						recipe.AddIngredient(Mod, EnchantingTableItem.enchantingTableNames[enchantingTableTier - 1] + "EnchantingTable", 1);
						recipe.AddIngredient(ItemID.SoulofNight, 1); //Soul of Night
						recipe.AddIngredient(ItemID.SoulofLight, 1); //Soul of Light
						break;
					case 4:
						recipe.AddIngredient(ItemID.HallowedBar, 2); //Hallowed Bars
						break;
				}
				recipe.Register();
				IDs[enchantingTableTier] = this.Type;
			}
		}
	}
	public class WoodEnchantingTable : EnchantingTableItem
	{
		public WoodEnchantingTable() { enchantingTableTier = 0; }
	}
	public class DustyEnchantingTable : EnchantingTableItem
	{
		public DustyEnchantingTable() { enchantingTableTier = 1; }
	}
	public class HellishEnchantingTable : EnchantingTableItem
	{
		public HellishEnchantingTable() { enchantingTableTier = 2; }
	}
	public class SoulEnchantingTable : EnchantingTableItem
	{
		public SoulEnchantingTable() { enchantingTableTier = 3; }
	}
	public class UltimateEnchantingTable : EnchantingTableItem
	{
		public UltimateEnchantingTable() { enchantingTableTier = 4; }
	}
}
