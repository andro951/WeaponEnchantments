using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using System;
using WeaponEnchantments.Tiles;
using static WeaponEnchantments.Common.Configs.ConfigValues;
using static Terraria.ID.TileID;
using WeaponEnchantments.Common.Utility;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using androLib.Common.Utility;
using androLib.Common.Globals;
using static androLib.Common.Globals.GenericGlobalTile;
using androLib;
using VacuumOreBag.Items;
using WeaponEnchantments.ModLib.KokoLib;

namespace WeaponEnchantments.Common.Globals
{
    public class WEGlobalTile : GlobalTile
    {
		//Can Place Enchanting tables
		public override bool CanPlace(int i, int j, int type) {
			int mainTile = Main.tile[i, j].TileType;
			for (int k = 0; k < Items.EnchantingTableItem.enchantingTableNames.Length; k++) {
				int tableType = ModContent.TileType<WoodEnchantingTable>() - k;
				Item heldItem = Main.LocalPlayer.HeldItem;

				//Prevent block swapping on top of the table (fix that was causing a crash)
				if (mainTile == tableType && !heldItem.IsPickaxe())
					return false;

				//Prevent block swapping the table onto other items except ones that won't crash the game.
				if (type == tableType) {
					switch (mainTile) {
						case 0:
						case 3:
						case 24:
						case 110:
						case 185:
						case 187:
						case 233:
							//Allow placing the table
							break;
						default:
							return false;
					}
				}
			}

			return true;
		}
		public static void KillTile(Tile tile, int dropItem, int dropItemStack, int secondaryItem, int secondaryItemStack) {
			ushort tileType = tile.TileType;

			int xp = 0;
			if (Main.tileAxe[tileType]) {
				//Axe
				int tiles = 0;
				int i = Player.tileTargetX;
				int y = Player.tileTargetY;
				while (y > 10 && Main.tile[i, y].HasTile && Sets.IsShakeable[Main.tile[i, y].TileType]) {
					y--;
					tiles++;
				}

				xp += tileType >= TreeTopaz && tileType <= TreeAmber ? tiles * 50 : tiles * 10;
			}
			else if (Main.tileHammer[tileType]) {
				//Hammer
				xp += 10;
			}
			else {
				xp += GetTileStrengthXP(tileType);
				//Pickaxe
				if (dropItem != ItemID.None) {
					xp += ContentSamples.ItemsByType[dropItem].value / 100 * dropItemStack;
				}

				if (secondaryItem != ItemID.None) {
					xp += ContentSamples.ItemsByType[secondaryItem].value / 100 * secondaryItemStack;
				}
			}

			//Config multiplier
			if (GatheringExperienceMultiplier != 1f) {
				xp = (int)Math.Round((float)xp * GatheringExperienceMultiplier);
				if (xp < 10)
					xp = 10;
			}

			NetManager.GainXPFromBreakTile(xp);
		}
		public static void GainXPFromBreakTile(int xp) {
			//Gain xp
			Item heldItem = WEMod.tileBrokenTool;
			if (!heldItem.NullOrAir()) {
				if (heldItem.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem)) {
					enchantedItem.GainXP(Main.LocalPlayer.HeldItem, xp);
				}
			}

			WEMod.tileBrokenTool = null;

			WEPlayer.LocalWEPlayer.Player.AllArmorGainXp(xp);
		}
		private static int GetTileStrengthXP(int tileType) {
			int xp = 10;
			//Values based on GetPickaxeDamage values from Vanilla source code
			switch (tileType) {
				case Ebonstone:
				case Hellstone:
				case Cobalt:
				case Pearlstone:
				case Crimstone:
				case Palladium:
					xp *= 2;
					break;
				case Tombstones:
				case Mythril:
				case Orichalcum:
					xp *= 3;
					break;
				case Adamantite:
				case Titanium:
				case LihzahrdBrick:
					xp *= 4;
					break;
				case Chlorophyte:
					xp *= 5;
					break;
				case < 0:
					break;
				default:
					if (Main.tileDungeon[tileType]) {
						xp *= 2;
					}
					else {
						//Modded Tile
						ModTile modTile = TileLoader.GetTile(tileType);
						xp = modTile != null ? (int)(xp * modTile.MineResist) : xp;
					}
					break;
			}

			return xp;
		}
	}
}