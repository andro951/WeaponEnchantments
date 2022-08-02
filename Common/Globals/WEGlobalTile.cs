using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using System;
using WeaponEnchantments.Tiles;
using static WeaponEnchantments.Common.Configs.ConfigValues;
using static Terraria.ID.TileID;
using WeaponEnchantments.Common.Utility;

namespace WeaponEnchantments.Common.Globals
{
    public class WEGlobalTile : GlobalTile
    {
		public static int tileType = -1;
		public static Item dropItem = new Item();
		public override bool CanPlace(int i, int j, int type) {
			int mainTile = Main.tile[i, j].TileType;
			for (int k = 0; k < Items.EnchantingTableItem.enchantingTableNames.Length; k++) {
				int tableType = ModContent.TileType<WoodEnchantingTable>() - k;
				Item heldItem = Main.LocalPlayer.HeldItem;

				//Prevent block swapping on top of the table (fix that was causing a crash)
				if (mainTile == tableType && heldItem.pick == 0)
					return false;

				//Prevent block swapping the table onto other items except ones that won't crash the game.
				if(type == tableType) {
					switch (mainTile) {
						case 0:
						case 3:
						case 24:
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
		public override bool CanKillTile(int i, int j, int type, ref bool blockDamaged) {
			Tile tileTarget = Main.tile[i, j];
			if (tileTarget.TileType == TileID.MysticSnakeRope)
				return true;

			//Hammer (Don't calculate a dropItem)
			if (Main.tileHammer[tileTarget.TileType]) {
				dropItem = new Item();
				return true;
			}

			WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
			
			//Calculate damage done to the tile  (Copied vanilla code)
			int hitBufferIndex = wePlayer.Player.hitTile.HitObject(i, j, 1);
			int damageAmount = 0;
			if (Main.tileAxe[tileTarget.TileType]) {
				//Axe
				if (tileTarget.TileType == TileID.Cactus) {
					//Cactus
					damageAmount += (int)(wePlayer.Player.HeldItem.axe * 3 * 1.2f);
				}
				else {
					//Wood and other axable things
					TileLoader.MineDamage(wePlayer.Player.HeldItem.axe, ref damageAmount);
				}
			}
            else {
				//Pickaxe
				damageAmount = GetPickaxeDamage(i, j, wePlayer.Player.inventory[wePlayer.Player.selectedItem].pick, hitBufferIndex, tileTarget);
			}

			//Get actual damage dealt
			int damage = wePlayer.Player.hitTile.AddDamage(hitBufferIndex, damageAmount, false);
			if (damage >= 100) {
				tileType = tileTarget.TileType;
				ModTile modTile = TileLoader.GetTile(type);
				//Get item dropped by the tile
				if (modTile != null && TileID.Sets.Ore[tileType]) {
					//Modded ore
					dropItem = ItemLoader.GetItem(modTile.ItemDrop).Item;
				}
				else {
					//Vanilla tile
					dropItem = new Item(GetDroppedItems(tileTarget));
				}
			}

			return true;
        }
        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem) {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
			Item heldItem = wePlayer.Player.HeldItem;

			if (heldItem.pick <= 0 && heldItem.axe <= 0 && heldItem.hammer <= 0)
				return;

			if (tileType < 0 || tileType == TileID.MysticSnakeRope)
				return;

                int xp = 1;
			
			xp += GetTileStrengthXP(tileType);
			if (Main.tileAxe[tileType]) {
				//Axe
				int tiles = 0;
				int y = j;
				while (y > 10 && Main.tile[i, y].HasTile && TileID.Sets.IsShakeable[Main.tile[i, y].TileType]) {
					y--;
					tiles++;
				}

				xp = tileType >= TileID.TreeTopaz && tileType <= TileID.TreeAmber ? tiles  * 50 : tiles * 10;
			}
			else if (Main.tileHammer[tileType]) {
				//Hammer
				xp += 4;
			}
            else {
				//Pickaxe
				if (dropItem.type != ItemID.None) {
					xp += dropItem.value / 100;
				}
			}

			//Config multiplier
			if (GatheringExperienceMultiplier > 1f) {
				xp = (int)Math.Round((float)xp * GatheringExperienceMultiplier);
				if (xp < 1)
					xp = 1;
			}
			
			//Gain xp
			wePlayer.Player.HeldItem.GetEnchantedItem().GainXP(wePlayer.Player.HeldItem, xp);
			EnchantedItemStaticMethods.AllArmorGainXp(wePlayer.Player, xp);
			
			//Reset static tile info
			tileType = -1;
			dropItem = new Item();
        }
		public static int GetDroppedItems(Tile tileCache) {
			int dropItem = 0;
			switch (tileCache.TileType) {
				//Coin Piles
				case TileID.CopperCoinPile:
					dropItem = ItemID.CopperCoin;
					break;
				case TileID.SilverCoinPile:
					dropItem = ItemID.SilverCoin;
					break;
				case TileID.GoldCoinPile:
					dropItem = ItemID.GoldCoin;
					break;
				case TileID.PlatinumCoinPile:
					dropItem = ItemID.PlatinumCoin;
					break;

				//Ores
				case TileID.Iron:
					dropItem = ItemID.IronOre;
					break;
				case TileID.Copper:
					dropItem = ItemID.CopperOre;
					break;
				case TileID.Gold:
					dropItem = ItemID.GoldOre;
					break;
				case TileID.Silver:
					dropItem = ItemID.SilverOre;
					break;
				case TileID.Palladium:
					dropItem = ItemID.PalladiumOre;
					break;
				case TileID.Orichalcum:
					dropItem = ItemID.OrichalcumOre;
					break;
				case TileID.Titanium:
					dropItem = ItemID.TitaniumOre;
					break;
				case TileID.Demonite:
					dropItem = ItemID.DemoniteOre;
					break;
				case TileID.Meteorite:
					dropItem = ItemID.Meteorite;
					break;
				case TileID.Hellstone:
					dropItem = ItemID.Hellstone;
					break;
				case TileID.Tin:
					dropItem = ItemID.TinOre;
					break;
				case TileID.Lead:
					dropItem = ItemID.LeadOre;
					break;
				case TileID.Tungsten:
					dropItem = ItemID.TungstenOre;
					break;
				case TileID.Platinum:
					dropItem = ItemID.PlatinumOre;
					break;
				case TileID.Crimtane:
					dropItem = ItemID.CrimtaneOre;
					break;
				case TileID.Cobalt:
					dropItem = ItemID.CobaltOre;
					break;
				case TileID.Mythril:
					dropItem = ItemID.MythrilOre;
					break;
				case TileID.Adamantite:
					dropItem = ItemID.AdamantiteOre;
					break;
				case TileID.Chlorophyte:
					dropItem = ItemID.ChlorophyteOre;
					break;
				case TileID.LunarOre:
					dropItem = ItemID.LunarOre;
					break;
				case TileID.DesertFossil:
					dropItem = ItemID.DesertFossil;
					break;
				case TileID.FossilOre:
					dropItem = ItemID.FossilOre;
					break;

				//Gems and crystals
				case TileID.Crystals:
					if (tileCache.TileFrameX >= 324)
						dropItem = ItemID.QueenSlimeCrystal;
					else
						dropItem = ItemID.CrystalShard;
					break;
				case TileID.Sapphire:
				case TileID.Ruby:
				case TileID.Emerald:
				case TileID.Topaz:
				case TileID.Amethyst:
				case TileID.Diamond:
					dropItem = tileType - TileID.Sapphire + ItemID.Sapphire;
					break;
				case TileID.AmberStoneBlock:
					dropItem = ItemID.Amber;
					break;
				case TileID.TreeTopaz:
					dropItem = ItemID.Topaz;
					break;
				case TileID.TreeAmethyst:
					dropItem = ItemID.Amethyst;
					break;
				case TileID.TreeSapphire:
					dropItem = ItemID.Sapphire;
					break;
				case TileID.TreeEmerald:
					dropItem = ItemID.Emerald;
					break;
				case TileID.TreeRuby:
					dropItem = ItemID.Ruby;
					break;
				case TileID.TreeDiamond:
					dropItem = ItemID.Diamond;
					break;
				case TileID.TreeAmber:
					dropItem = ItemID.Amber;
					break;
				case TileID.ExposedGems:
					int frame = tileCache.TileFrameX / 18;
					switch (frame) {
						case 0:
							dropItem = ItemID.Amethyst;
							break;
						case 1:
							dropItem = ItemID.Topaz;
							break;
						case 2:
							dropItem = ItemID.Sapphire;
							break;
						case 3:
							dropItem = ItemID.Emerald;
							break;
						case 4:
							dropItem = ItemID.Ruby;
							break;
						case 5:
							dropItem = ItemID.Diamond;
							break;
						case 6:
							dropItem = ItemID.Amber;
							break;
						default:
							$"Failed to determine the dropItem of tile: tileCache.LiquidType: {tileCache.LiquidType}, tileCache.TileFrameX: {tileCache.TileFrameX}, tileCache.TileFrameY: {tileCache.TileFrameY}.".LogNT();
							break;
					}
					break;
			}

			return dropItem;
		}
		private int GetPickaxeDamage(int x, int y, int pickPower, int hitBufferIndex, Tile tileTarget) {

			//All copied from Vanilla souce code.
			Player player = Main.LocalPlayer;
			int num = 0;
			if (Main.tileNoFail[tileTarget.TileType])
				num = 100;

			if (Main.tileDungeon[tileTarget.TileType] || tileTarget.TileType == Ebonstone || tileTarget.TileType == Hellstone || tileTarget.TileType == Pearlstone || tileTarget.TileType == Crimstone)
				num += pickPower / 2;
			else if (tileTarget.TileType == Tombstones)
				num += pickPower / 3;
			else if (tileTarget.TileType == Spikes || tileTarget.TileType == WoodenSpikes)
				num += pickPower * 2;
			else if (tileTarget.TileType == LihzahrdBrick)
				num += pickPower / 4;
			else if (tileTarget.TileType == Cobalt || tileTarget.TileType == Palladium)
				num += pickPower / 2;
			else if (tileTarget.TileType == Mythril || tileTarget.TileType == Orichalcum)
				num += pickPower / 3;
			else if (tileTarget.TileType == Adamantite || tileTarget.TileType == Titanium)
				num += pickPower / 4;
			else if (tileTarget.TileType == Chlorophyte)
				num += pickPower / 5;
			else
				TileLoader.MineDamage(pickPower, ref num);
			if (tileTarget.TileType == Chlorophyte && pickPower < 200)
				num = 0;
			
			if ((tileTarget.TileType == Ebonstone || tileTarget.TileType == Crimstone) && pickPower < 65) {
				num = 0;
			}
			else if (tileTarget.TileType == Pearlstone && pickPower < 65) {
				num = 0;
			}
			else if (tileTarget.TileType == Meteorite && pickPower < 50) {
				num = 0;
			}
			else if ((tileTarget.TileType == Demonite || tileTarget.TileType == Crimtane) && (double)y > Main.worldSurface && pickPower < 55) {
				num = 0;
			}
			else if (tileTarget.TileType == Obsidian && pickPower < 55) {
				num = 0;
			}
			else if (tileTarget.TileType == Hellforge && pickPower < 65 && y >= Main.UnderworldLayer) {
				num = 0;
			}
			else if (tileTarget.TileType == Hellstone && pickPower < 65) {
				num = 0;
			}
			else if ((tileTarget.TileType == LihzahrdBrick || tileTarget.TileType == LihzahrdAltar) && pickPower < 210) {
				num = 0;
			}
			else if (tileTarget.TileType == Traps && pickPower < 210) {
				int num2 = tileTarget.TileFrameY / 18;
				if ((uint)(num2 - 1) <= 3u)
					num = 0;
			}
			else if (Main.tileDungeon[tileTarget.TileType] && pickPower < 100 && (double)y > Main.worldSurface) {
				if ((double)x < (double)Main.maxTilesX * 0.35 || (double)x > (double)Main.maxTilesX * 0.65)
					num = 0;
			}
			else if (tileTarget.TileType == Cobalt && pickPower < 100) {
				num = 0;
			}
			else if (tileTarget.TileType == Mythril && pickPower < 110) {
				num = 0;
			}
			else if (tileTarget.TileType == Adamantite && pickPower < 150) {
				num = 0;
			}
			else if (tileTarget.TileType == Palladium && pickPower < 100) {
				num = 0;
			}
			else if (tileTarget.TileType == Orichalcum && pickPower < 110) {
				num = 0;
			}
			else if (tileTarget.TileType == Titanium && pickPower < 150) {
				num = 0;
			}
			else {
				TileLoader.PickPowerCheck(tileTarget, pickPower, ref num);
			}

			if (tileTarget.TileType == SnowBlock || tileTarget.TileType == Dirt || tileTarget.TileType == ClayBlock || tileTarget.TileType == Sand || tileTarget.TileType == Ash || tileTarget.TileType == Mud || tileTarget.TileType == Silt || tileTarget.TileType == Slush || tileTarget.TileType == HardenedSand)
				num += pickPower;

			if (tileTarget.TileType == DesertFossil)
				num += 5;

			if (tileTarget.TileType == Stalactite || Main.tileRope[tileTarget.TileType] || tileTarget.TileType == CrimsonGrass)
				num = 100;

			if (tileTarget.TileType == Mannequin || tileTarget.TileType == Womannequin) {
				if (tileTarget.TileFrameX == 18 || tileTarget.TileFrameX == 54) {
					x--;
					tileTarget = Main.tile[x, y];
					player.hitTile.UpdatePosition(hitBufferIndex, x, y);
				}

				if (tileTarget.TileFrameX >= 100) {
					num = 0;
					Main.blockMouse = true;
				}
			}

			if (tileTarget.TileType == WeaponsRack) {
				if (tileTarget.TileFrameY == 0) {
					y++;
					tileTarget = Main.tile[x, y];
					player.hitTile.UpdatePosition(hitBufferIndex, x, y);
				}

				if (tileTarget.TileFrameY == 36) {
					y--;
					tileTarget = Main.tile[x, y];
					player.hitTile.UpdatePosition(hitBufferIndex, x, y);
				}

				int frameX = tileTarget.TileFrameX;
				bool flag = frameX >= 5000;
				bool flag2 = false;
				if (!flag) {
					int num3 = frameX / 18;
					num3 %= 3;
					x -= num3;
					tileTarget = Main.tile[x, y];
					if (tileTarget.TileFrameX >= 5000)
						flag = true;
				}

				if (flag) {
					frameX = tileTarget.TileFrameX;
					int num4 = 0;
					while (frameX >= 5000)
					{
						frameX -= 5000;
						num4++;
					}

					if (num4 != 0)
						flag2 = true;
				}

				if (flag2) {
					num = 0;
					//Main.blockMouse = true;
				}
			}
			
			return num;
		}
		private int GetTileStrengthXP(int tileType) {
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
