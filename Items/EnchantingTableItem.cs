﻿using Terraria.UI;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Configs;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Localization;
using WeaponEnchantments.Tiles;
using androLib.Common.Utility;
using androLib;
using System.Reflection;
using System;
using System.Linq;

namespace WeaponEnchantments.Items
{
	public abstract class EnchantingTableItem : WEModItem
	{
		public int enchantingTableTier = -1;
		public static string[] enchantingTableNames = new string[5] { "Wood", "Dusty", "Hellish", "Soul", "Ultimate" };
		public static int[] IDs = new int[enchantingTableNames.Length];
		public static int[] Values = new int[enchantingTableNames.Length];
		private static List<Dictionary<int, int>> ingredients = new() {
			new() { { ItemID.WorkBench, 1 }, { ItemID.Torch, 4 } },
			new() { { ItemID.FossilOre, 1 } },
			new() { { ItemID.ObsidianSkull, 1 }, { ItemID.Hellstone, 2 } },
			new() { { ItemID.SoulofLight, 2 } },
			new() { { ItemID.HallowedBar, 2 } }
		};
		public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
		public override List<WikiTypeID> WikiItemTypes {
			get {
				List<WikiTypeID> types = new() { WikiTypeID.EnchantingTables, WikiTypeID.Storage, WikiTypeID.CraftingStation };
				if (enchantingTableTier < EnchantingRarity.tierNames.Length - 1)
					types.Add(WikiTypeID.CraftingMaterial);

				return types;
			}
		}
		public override int CreativeItemSacrifice => 1;
		public override string LocalizationTooltip =>
			$"Used to apply enchantments to items. (tier {enchantingTableTier})\n" +
			$"Can be used from your inventory by right clicking." +
			$"(Shift right click allows you to split the stack.)";

		public override string Artist => "Zorutan";
		public override string Designer => "andro951";
		public override void SetStaticDefaults() {
			GetDefaults();

			IDs[enchantingTableTier] = Type;

			for(int i = 0; i <= enchantingTableTier; i++) {
				foreach (KeyValuePair<int, int> pair in ingredients[i]) {
					Values[enchantingTableTier] += ContentSamples.ItemsByType[pair.Key].value * pair.Value;
				}
			}

			base.SetStaticDefaults();
		}
		private void GetDefaults() {
			for (int i = 0; i < enchantingTableNames.Length; i++) {
				if (enchantingTableNames[i] == Name.Substring(0, enchantingTableNames[i].Length)) {
					enchantingTableTier = i;

					break;
				}
			}
		}
		public override void SetDefaults()
		{
			GetDefaults();

			Item.createTile = EnchantingTableTile.GetTableTypeByTier(enchantingTableTier);

			Item.maxStack = 99;
			Item.width = 28;
			Item.height = 14;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 10;
			Item.useAnimation = 15;
			Item.consumable = true;
			Item.rare = EnchantingRarity.GetRarityFromTier(enchantingTableTier);
			Item.value = Values[enchantingTableTier];
		}
		private string GetTableName(int tier) {
			return enchantingTableNames[tier] + "EnchantingTable";
		}
		public override void AddRecipes() {
			for (int i = -1; i < enchantingTableTier; i++) {
				if (!ConfigValues.useAllRecipes && i < enchantingTableTier - 1)
					continue;

				for (int d = 0; d <= (i <= 0 && enchantingTableTier >= 1 ? 1 : 0); d++) {
					Recipe recipe = CreateRecipe();

					if (i == -1) {
						recipe.AddRecipeGroup($"{AndroMod.ModName}:{AndroModSystem.Workbenches}"); //Workbench
					}
					else {
						recipe.AddIngredient(Mod, GetTableName(i), 1); //Enchanting Table
					}

					if (i < 0 && enchantingTableTier >= 0) {
						recipe.AddIngredient(ItemID.Torch, 4); //Torches
					}
					
					if (i < 1 && enchantingTableTier >= 1) {
						if (d == 0) {
							recipe.AddIngredient(ItemID.DesertFossil, 10); //Desert Fossil
						}
						else {
							recipe.AddIngredient(ItemID.FossilOre, 1);
						}
					}

					if (i < 2 && enchantingTableTier >= 2) {
						recipe.AddIngredient(ItemID.ObsidianSkull, 1); //Obsidian Skull
						recipe.AddIngredient(ItemID.Hellstone, 2); //Hellstone ore
					}

					if (i < 3 && enchantingTableTier >= 3)
						recipe.AddRecipeGroup($"{AndroMod.ModName}:{AndroModSystem.AnyAlignedSoul}", 2); //Soul of Light or Night

					if (i < 4 && enchantingTableTier >= 4)
						recipe.AddIngredient(ItemID.HallowedBar, 2); //Hallowed Bars

					//if (enchantingTableTier == 0)
					//	recipe.SortBeforeFirstRecipesOf(ModContent.ItemType<DustyEnchantingTable>());

					recipe.Register();
				}
			}
		}
		public static int GetTableTier(string s) {
			for(int i = 0; i < enchantingTableNames.Length; i++) {
				if (s.Contains(enchantingTableNames[i]))
					return i;
			}

			return enchantingTableNames.Length;
		}
		public override bool CanRightClick() => !ItemSlot.ShiftInUse;
		public override void RightClick(Player player) {
			Item.stack++;
			WEPlayer wePlayer = player.GetWEPlayer();
			int x;
			int y;
			if (wePlayer.usingEnchantingTable) {
				x = player.chestX;
				y = player.chestY;
			}
			else {
				x = (int)(player.position.X / 16f);
				y = (int)(player.position.Y / 16f);
			}

			EnchantingTableTile.RightClickEnchantingTable(x, y, enchantingTableTier);
		}
	}

	[Autoload(false)]
	public class WoodEnchantingTable : EnchantingTableItem { }
	[Autoload(false)]
	public class DustyEnchantingTable : EnchantingTableItem { }
	[Autoload(false)]
	public class HellishEnchantingTable : EnchantingTableItem { }
	[Autoload(false)]
	public class SoulEnchantingTable : EnchantingTableItem { }
	[Autoload(false)]
	public class UltimateEnchantingTable : EnchantingTableItem { }
}
