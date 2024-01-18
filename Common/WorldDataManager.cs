using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Items;
using androLib.Common.Utility;

namespace WeaponEnchantments.Common
{
	public class WorldDataManager : ModSystem
	{
		//World ores
		public static int CopperOre;
		public static int IronOre;
		public static int SilverOre;
		public static int GoldOre;
		public static int DemoniteOre;
		public static int CobaltOre;
		public static int MythrilOre;
		public static int AdamantiteOre;


		//World bars (from ores)
		public static int CopperBar;
		public static int IronBar;
		public static int SilverBar;
		public static int GoldBar;
		public static int DemoniteBar;
		public static int CobaltBar;
		public static int MythrilBar;
		public static int AdamantiteBar;

		public static string GetOreNamesList(int start, int end = 9, bool andAtEnd = false, bool skipDemonite = true) {
			string list = "";
			bool first = true;
			for(int i = start; i < end; i++) {
				if (i == 4 && skipDemonite)
					continue;

				if (first) {
					first = false;
				}
				else {
					if (!andAtEnd && i == end - 1) {
						list += $" {EnchantmentGeneralTooltipsID.And.ToString().Lang_WE(L_ID1.Tooltip, L_ID2.EnchantmentGeneralTooltips)} ";
					}
					else {
						list += ", ";
					}
				}

				list += $"{GetOreName(ores[i])}";

				if (andAtEnd && i == end - 1)
					list += $" {EnchantmentGeneralTooltipsID.And.ToString().Lang_WE(L_ID1.Tooltip, L_ID2.EnchantmentGeneralTooltips)} ";
			}

			return list;
		}

		private static List<int> ores;

		private static string GetOreName(int id) {
			string name;
			switch (id) {
				case ItemID.CopperOre:
					name = "copper";
					break;
				case ItemID.TinOre:
					name = "tin";
					break;
				case ItemID.IronOre:
					name = "iron";
					break;
				case ItemID.LeadOre:
					name = "lead";
					break;
				case ItemID.SilverOre:
					name = "silver";
					break;
				case ItemID.TungstenOre:
					name = "tungsten";
					break;
				case ItemID.GoldOre:
					name = "gold";
					break;
				case ItemID.PlatinumOre:
					name = "platinum";
					break;
				case ItemID.DemoniteOre:
					name = "demonite";
					break;
				case ItemID.CrimtaneOre:
					name = "crimtane";
					break;
				case ItemID.CobaltOre:
					name = "cobalt";
					break;
				case ItemID.PalladiumOre:
					name = "palladium";
					break;
				case ItemID.MythrilOre:
					name = "mythril";
					break;
				case ItemID.OrichalcumOre:
					name = "orichalcum";
					break;
				case ItemID.AdamantiteOre:
					name = "adamantite";
					break;
				case ItemID.TitaniumOre:
					name = "titanium";
					break;
				case ItemID.ChlorophyteOre:
					name = "chlorophyte";
					break;
				default:
					return "";
			}

			return name.Lang_WE(L_ID1.Ores);
		}

		public override void OnWorldLoad() { 
			//World ores
			CopperOre = WorldGen.SavedOreTiers.Copper == TileID.Copper ? ItemID.CopperOre : ItemID.TinOre;
			IronOre = WorldGen.SavedOreTiers.Iron == TileID.Iron ? ItemID.IronOre : ItemID.LeadOre;
			SilverOre = WorldGen.SavedOreTiers.Silver == TileID.Silver ? ItemID.SilverOre : ItemID.TungstenOre;
			GoldOre = WorldGen.SavedOreTiers.Gold == TileID.Gold ? ItemID.GoldOre : ItemID.PlatinumOre;
			DemoniteOre = !WorldGen.crimson ? ItemID.DemoniteOre : ItemID.CrimtaneOre;
			CobaltOre = WorldGen.SavedOreTiers.Cobalt == TileID.Cobalt ? ItemID.CobaltOre : ItemID.PalladiumOre;
			MythrilOre = WorldGen.SavedOreTiers.Mythril == TileID.Mythril ? ItemID.MythrilOre : ItemID.OrichalcumOre;
			AdamantiteOre = WorldGen.SavedOreTiers.Adamantite == TileID.Adamantite ? ItemID.AdamantiteOre : ItemID.TitaniumOre;

			//World bars (from ores)
			CopperBar = WorldGen.SavedOreTiers.Copper == TileID.Copper ? ItemID.CopperBar : ItemID.TinBar;
			IronBar = WorldGen.SavedOreTiers.Iron == TileID.Iron ? ItemID.IronBar : ItemID.LeadBar;
			SilverBar = WorldGen.SavedOreTiers.Silver == TileID.Silver ? ItemID.SilverBar : ItemID.TungstenBar;
			GoldBar = WorldGen.SavedOreTiers.Gold == TileID.Gold ? ItemID.GoldBar : ItemID.PlatinumBar;
			DemoniteBar = !WorldGen.crimson ? ItemID.DemoniteBar : ItemID.CrimtaneBar;
			CobaltBar = WorldGen.SavedOreTiers.Cobalt == TileID.Cobalt ? ItemID.CobaltBar : ItemID.PalladiumBar;
			MythrilBar = WorldGen.SavedOreTiers.Mythril == TileID.Mythril ? ItemID.MythrilBar : ItemID.OrichalcumBar;
			AdamantiteBar = WorldGen.SavedOreTiers.Adamantite == TileID.Adamantite ? ItemID.AdamantiteBar : ItemID.TitaniumBar;

			//Edit recipies for crafting containments back into bars based on the world gen ores.
			for(int i = 0; i < Main.recipe.Length; i++) {
				Recipe recipe = Main.recipe[i];

				//Check if bar needs to be swapped from the default.
				int newItemType;
				switch (recipe.createItem.type) {
					case ItemID.CopperBar:
						if (ItemID.CopperBar == CopperBar) {
							continue;
						}
						else {
							newItemType = CopperBar;
						}
						break;
					case ItemID.SilverBar:
						if (ItemID.SilverBar == SilverBar) {
							continue;
						}
						else {
							newItemType = SilverBar;
						}
						break;
					case ItemID.GoldBar:
						if (ItemID.GoldBar == GoldBar) {
							continue;
						}
						else {
							newItemType = GoldBar;
						}
						break;
					case ItemID.DemoniteBar:
						if (ItemID.DemoniteBar == DemoniteBar) {
							continue;
						}
						else {
							newItemType = DemoniteBar;
						}
						break;
					case ItemID.CobaltBar:
						if (ItemID.CobaltBar == CobaltBar) {
							continue;
						}
						else {
							newItemType = CobaltBar;
						}
						break;
					case ItemID.MythrilBar:
						if (ItemID.MythrilBar == MythrilBar) {
							continue;
						}
						else {
							newItemType = MythrilBar;
						}
						break;
					case ItemID.AdamantiteBar:
						if (ItemID.AdamantiteBar == AdamantiteBar) {
							continue;
						}
						else {
							newItemType = AdamantiteBar;
						}
						break;
					default:
						continue;
				}

				foreach(Item ingredient in recipe.requiredItem) {
					ModItem modItem = ingredient.ModItem;
					if (modItem == null)
						continue;

					if (modItem is ContainmentItem) {
						int stack = recipe.createItem.stack;
						Main.recipe[i].createItem = new Item(newItemType, stack);
					}
				}
			}

			ores = new() {
				CopperOre,
				IronOre,
				SilverOre,
				GoldOre,
				DemoniteOre,
				CobaltOre,
				MythrilOre,
				AdamantiteOre,
				ItemID.ChlorophyteOre
			};
		}
	}
}
