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

namespace WeaponEnchantments.Common.Globals
{
    public class WEGlobalTile : GlobalTile
    {
		public static int tileType = -1;
		public static Item dropItem = new Item();
		private static SortedDictionary<int, int> tileTypeToItemType = null;
		private static SortedDictionary<int, int> TileTypeToItemType {
			get {
				if (tileTypeToItemType == null)
					SetupTileTypeToItemType();

				return tileTypeToItemType;
			}
		}
		private static bool tileTypeToItemTypesSetup = false;
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
		public override bool CanKillTile(int i, int j, int type, ref bool blockDamaged) {
			Tile tileTarget = Main.tile[i, j];
			if (tileTarget.TileType == MysticSnakeRope)
				return true;

			//Hammer (Don't calculate a dropItem)
			if (Main.tileHammer[tileTarget.TileType]) {
				tileType = tileTarget.TileType;
				dropItem = new Item();
				return true;
			}

			if (!Main.LocalPlayer.TryGetModPlayer(out WEPlayer wePlayer))
				return true;
			
			//Calculate damage done to the tile  (Copied vanilla code)
			int hitBufferIndex = wePlayer.Player.hitTile.HitObject(i, j, 1);
			int damageAmount = 0;
			if (Main.tileAxe[tileTarget.TileType]) {
				//Axe
				if (tileTarget.TileType == Cactus) {
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
				dropItem = new Item(GetDroppedItem(type, tileTarget.TileFrameX, true));
			}

			return true;
        }
        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem) {
			if (!Main.LocalPlayer.TryGetModPlayer(out WEPlayer wePlayer))
				return;

			Item heldItem = wePlayer.Player.HeldItem;

			if (!heldItem.TryGetEnchantedItem(out EnchantedItem hGlobal))
				return;

			if (heldItem.pick <= 0 && heldItem.axe <= 0 && heldItem.hammer <= 0)
				return;

			if (tileType < 0 || tileType == MysticSnakeRope)
				return;

            int xp = 1;
			
			xp += GetTileStrengthXP(tileType);
			if (Main.tileAxe[tileType]) {
				//Axe
				int tiles = 0;
				int y = j;
				while (y > 10 && Main.tile[i, y].HasTile && Sets.IsShakeable[Main.tile[i, y].TileType]) {
					y--;
					tiles++;
				}

				xp = tileType >= TreeTopaz && tileType <= TreeAmber ? tiles  * 50 : tiles * 10;
			}
			else if (Main.tileHammer[tileType]) {
				//Hammer
				xp += 9;
			}
			else {
				//Pickaxe
				if (dropItem.type != ItemID.None) {
					xp += dropItem.value / 100;
				}
			}

			//Config multiplier
			if (GatheringExperienceMultiplier != 1f) {
				xp = (int)Math.Round((float)xp * GatheringExperienceMultiplier);
				if (xp < 1)
					xp = 1;
			}
			
			//Gain xp
			hGlobal.GainXP(wePlayer.Player.HeldItem, xp);
			EnchantedItemStaticMethods.AllArmorGainXp(wePlayer.Player, xp);
			
			//Reset static tile info
			tileType = -1;
			dropItem = new Item();
        }
		public static int GetDroppedItem(int type, int frame = 0, bool forMining = false) {
			int dropItem = 0;
			switch (type) {
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
					if (frame >= 324)
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
					}
					break;
				case TileID.CrystalBall:
					dropItem = ItemID.CrystalBall;
					break;
				case TileID.Loom:
					dropItem = ItemID.Loom;
					break;
				case TileID.Anvils:
					dropItem = ItemID.IronAnvil;
					break;
				case TileID.Furnaces:
					dropItem = ItemID.Furnace;
					break;
				case TileID.Kegs:
					dropItem = ItemID.Keg;
					break;
				case TileID.CookingPots:
					dropItem = ItemID.CookingPot;
					break;
				case TileID.WorkBenches:
					dropItem = ItemID.WorkBench;
					break;
				case TileID.TeaKettle:
					dropItem = ItemID.TeaKettle;
					break;
				case TileID.Bottles:
					dropItem = ItemID.Bottle;
					break;
				case TileID.ImbuingStation:
					dropItem = ItemID.ImbuingStation;
					break;
				case TileID.MythrilAnvil:
					dropItem = ItemID.MythrilAnvil;
					break;
				case TileID.Autohammer:
					dropItem = ItemID.Autohammer;
					break;
				case TileID.LivingLoom:
					dropItem = ItemID.LivingLoom;
					break;
				case TileID.HeavyWorkBench:
					dropItem = ItemID.HeavyWorkBench;
					break;
				case TileID.Blendomatic:
					dropItem = ItemID.BlendOMatic;
					break;
				case TileID.Sawmill:
					dropItem = ItemID.Sawmill;
					break;
				case TileID.LunarCraftingStation:
					dropItem = ItemID.LunarCraftingStation;
					break;
				case TileID.AdamantiteForge:
					dropItem = ItemID.AdamantiteForge;
					break;
				case TileID.Solidifier:
					dropItem = ItemID.Solidifier;
					break;
				case TileID.MeatGrinder:
					dropItem = ItemID.MeatGrinder;
					break;
				case TileID.LesionStation:
					dropItem = ItemID.LesionStation;
					break;
				case TileID.GlassKiln:
					dropItem = ItemID.GlassKiln;
					break;
				case TileID.HoneyDispenser:
					dropItem = ItemID.HoneyDispenser;
					break;
				case TileID.SkyMill:
					dropItem = ItemID.SkyMill;
					break;
				case TileID.LihzahrdFurnace:
					dropItem = ItemID.LihzahrdFurnace;
					break;
				case TileID.IceMachine:
					dropItem = ItemID.IceMachine;
					break;
				case TileID.SteampunkBoiler:
					dropItem = ItemID.SteampunkBoiler;
					break;
				case TileID.FleshCloningVat:
					dropItem = ItemID.FleshCloningVaat;
					break;
				case TileID.BoneWelder:
					dropItem = ItemID.BoneWelder;
					break;
				case TileID.Hellforge:
					dropItem = ItemID.Hellforge;
					break;
				case TileID.DyeVat:
					dropItem = ItemID.DyeVat;
					break;
				case TileID.TinkerersWorkbench:
					dropItem = ItemID.TinkerersWorkshop;
					break;
				case TileID.Tables:
					dropItem = ItemID.WoodenTable;
					break;
				case TileID.Chairs:
					dropItem = ItemID.WoodenChair;
					break;
				case TileID.Bookcases:
					dropItem = ItemID.Bookcase;
					break;
				case TileID.AlchemyTable:
					dropItem = ItemID.AlchemyTable;
					break;
				case TileID.DemonAltar:
					if (WEMod.magicStorageEnabled && TileTypeToItemType.Keys.Contains(type))
						dropItem = TileTypeToItemType[type];

					break;
				default:
					ModTile modTile = TileLoader.GetTile(type);
					//Get item dropped by the tile
					if (modTile != null && (!forMining || TileID.Sets.Ore[tileType])) {
						if (modTile.ItemDrop > 0) {
							dropItem = modTile.ItemDrop;
						}
						else if (TileTypeToItemType.Keys.Contains(type)) {
							dropItem = TileTypeToItemType[type];
						}
						else if (tileTypeToItemTypesSetup) {
							$"Failed to determine the dropItem of tile: {type}, modTile.Name: {modTile.Name}, modTile.ItemDrop: {modTile.ItemDrop}.".LogNT(ChatMessagesIDs.FailedDetermineDropItem);
						}

					}
					else {
						$"Failed to determine the dropItem of tile: {type}.".LogNT(ChatMessagesIDs.FailedDetermineDropItem);
					}

					break;
			}

			return dropItem;
		}
		public static int GetRequiredPickaxePower(int type, bool forInfusionPower = false) {
			ModTile modTile = TileLoader.GetTile(type);
			if (modTile != null)
				return modTile.MinPick;

			switch (type) {
				case Meteorite:
					if (forInfusionPower)
						return 60;//Not correct for vanilla.  Needed it to be changed.

					return 50;
				case Demonite:
				case Crimtane:
				case Obsidian:
					return 55;
				case Ebonstone:
				case Crimstone:
				case Pearlstone:
				case Hellstone:
				case Hellforge:
					return 65;
				case Cobalt:
				case Palladium:
					return 100;
				case Mythril:
				case Orichalcum:
					return 110;
				case Adamantite:
				case Titanium:
					return 150;
				case Chlorophyte:
					return 200;
				case LihzahrdBrick:
				case LihzahrdAltar:
					return 210;
				default:
					if (Main.tileDungeon[type])
						return 100;

					return 0;
			}
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
		/*public override void PlaceInWorld(int i, int j, int type, Item item) {
			ITileData tileData;
			Main.tile[i, j].
			David 👻 [Crowd Control Mod] — Today at 5:34 PM
			I'm not too too familiar. You create a struct that implements ITileData. Then with a tile instance you can do ref tile.Get<YourStruct>().
		}*/
		private static void SetupTileTypeToItemType() {
			tileTypeToItemType = new();
			foreach (int tileType in Main.recipe.Select(recipe => recipe.requiredTile).SelectMany(tiles => tiles).ToHashSet()) {
				int itemType = GetDroppedItem(tileType);
				if (itemType <= 0) {
					if (tileType > TileID.Count || WEMod.magicStorageEnabled && tileType == TileID.DemonAltar) {
						if (TryGetModTileName(tileType, out string modTileName) && TryGetModTileItemType(modTileName, out int modTileItemType)) {
							itemType = modTileItemType;
						}
						else {
							$"Failed to find find modded tile name for tile: {tileType}, modTileName: {modTileName}".LogSimple();
						}
					}
					else {
						$"Failed to find find vanilla tile type: {tileType}".LogSimple();
					}
				}

				if (itemType > 0)
					tileTypeToItemType.Add(tileType, itemType);
			}

			tileTypeToItemTypesSetup = true;
			//$"\n{tileTypeToItemType.Select(t => $"{t.Key}: {t.Value.CSI().S()}").JoinList("\n")}".LogSimple();
		}
		private static bool TryGetModTileName(int tileType, out string modTileName) {
			modTileName = "";
			if (WEMod.magicStorageEnabled && tileType == TileID.DemonAltar) {
				modTileName = "DemonAltar";
				return true;
			}

			if (tileType < TileID.Count)
				return false;

			ModTile modTile = TileLoader.GetTile(tileType);
			if (modTile == null)
				return false;

			modTileName = modTile.Name;
			return true;
		}
		private static bool TryGetModTileItemType(string modTileName, out int modTileItemType) {//TODO: change to fullname instead of name
			modTileItemType = 0;
			for (int type = ItemID.Count; type < ItemLoader.ItemCount; type++) {
				ModItem modItem = ContentSamples.ItemsByType[type].ModItem;
				if (modItem == null)
					continue;

				if (modTileName == modItem.Name) {
					modTileItemType = modItem.Type;
					return true;
				}
			}

			return false;
		}
	}
}
