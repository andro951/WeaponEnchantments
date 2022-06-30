using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using System;

namespace WeaponEnchantments.Common.Globals
{
    public class WEGlobalTile : GlobalTile
    {
		public static int tileType = -1;
		public static Item dropItem = new Item();
        public override bool Drop(int i, int j, int type)
        {

			return true;
        }
        public override bool CanKillTile(int i, int j, int type, ref bool blockDamaged)
        {
			Tile tileTarget = Main.tile[i, j];
			if (tileTarget.TileType != 504)
			{
				WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
				int hitBufferIndex = wePlayer.Player.hitTile.HitObject(i, j, 1);
				int damageAmount = 0;
				if (Main.tileAxe[tileTarget.TileType])
                {
					if (tileTarget.TileType == 80)
						damageAmount += (int)(wePlayer.Player.HeldItem.axe * 3 * 1.2f);
					else
						TileLoader.MineDamage(wePlayer.Player.HeldItem.axe, ref damageAmount);
				}
                else
                {
					damageAmount = GetPickaxeDamage(i, j, wePlayer.Player.inventory[wePlayer.Player.selectedItem].pick, hitBufferIndex, tileTarget);
				}
				int damage = wePlayer.Player.hitTile.AddDamage(hitBufferIndex, damageAmount, false);
				if (damage >= 100)
				{
					tileType = tileTarget.TileType;
					ModTile modTile = TileLoader.GetTile(type);
					if (modTile != null && TileID.Sets.Ore[tileType])
					{
						dropItem = ItemLoader.GetItem(modTile.ItemDrop).Item;
					}
					else
					{
						dropItem = new Item(GetDroppedItems(tileTarget));
					}
				}
			}
			return true;
        }
        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            if (wePlayer.Player.HeldItem.pick > 0 || wePlayer.Player.HeldItem.axe > 0 || wePlayer.Player.HeldItem.hammer > 0)
            {
                int xp = 1;
				if(tileType != 504)
                {
					if(tileType >= 0)
                    {
						xp += GetTileStrengthXP(tileType);
						if (Main.tileAxe[tileType])
						{
							int tiles = 0;
							int y = j;
							while (y > 10 && Main.tile[i, y].HasTile && TileID.Sets.IsShakeable[Main.tile[i, y].TileType])
							{
								y--;
								tiles++;
							}
							xp = tileType >= TileID.TreeTopaz && tileType <= TileID.TreeAmber ? tiles  * 50 : tiles * 10;
							//Main.NewText(wePlayer.Player.name + " recieved " + xp.ToString() + " xp from cutting down a tree.");
							//ModContent.GetInstance<WEMod>().Logger.Info(wePlayer.Player.name + " recieved " + xp.ToString() + " xp from cutting down a tree.");
						}
                        else
                        {
							if (dropItem.type != ItemID.None)
							{
								xp += dropItem.value / 100;
							}
						}
						//Main.NewText(wePlayer.Player.name + " recieved " + xp.ToString() + " xp from mining" + dropItem.Name + ".");
						float configMultiplier = (float)WEMod.serverConfig.GatheringExperienceMultiplier / 100f;
						if (configMultiplier > 1f)
						{
							xp = (int)Math.Round((float)xp * configMultiplier);
							if (xp < 1)
								xp = 1;
						}
						wePlayer.Player.HeldItem.GetGlobalItem<EnchantedItem>().GainXP(wePlayer.Player.HeldItem, xp);
						EnchantedItem.AllArmorGainXp(xp);
						tileType = -1;
						dropItem = new Item();
					}
				}
            }
        }
		public static int GetDroppedItems(Tile tileCache)
		{
			int dropItem = 0;
			switch (tileCache.TileType)
			{
				case 330:
					dropItem = 71;
					break;
				case 331:
					dropItem = 72;
					break;
				case 332:
					dropItem = 73;
					break;
				case 333:
					dropItem = 74;
					break;
				case 408://Luminite
					dropItem = 3460;
					break;
				case 404:
					dropItem = 3347;
					break;
				case 407:
					dropItem = 3380;
					break;
				case 211:
					dropItem = 947;
					break;
				case 6:
					dropItem = 11;
					break;
				case 7:
					dropItem = 12;
					break;
				case 8:
					dropItem = 13;
					break;
				case 9:
					dropItem = 14;
					break;
				case 221:
					dropItem = 1104;
					break;
				case 222:
					dropItem = 1105;
					break;
				case 223:
					dropItem = 1106;
					break;
				case 204:
					dropItem = 880;
					break;
				case 166:
					dropItem = 699;
					break;
				case 167:
					dropItem = 700;
					break;
				case 168:
					dropItem = 701;
					break;
				case 169:
					dropItem = 702;
					break;
				case 22:
					dropItem = 56;
					break;
				case 37:
					dropItem = 116;
					break;
				case 58:
					dropItem = 174;
					break;
				case 107:
					dropItem = 364;
					break;
				case 108:
					dropItem = 365;
					break;
				case 111:
					dropItem = 366;
					break;
				case 63:
				case 64:
				case 65:
				case 66:
				case 67:
				case 68:
					dropItem = tileType - 63 + 177;
					break;
				case 566:
					dropItem = 999;
					break;
				case 129:
					if (tileCache.TileFrameX >= 324)
						dropItem = 4988;
					else
						dropItem = 502;
					break;
				
				case 589:
					dropItem = 999;
					break;
				case 584:
					dropItem = 181;
					break;
				case 583:
					dropItem = 180;
					break;
				case 586:
					dropItem = 179;
					break;
				case 585:
					dropItem = 177;
					break;
				case 587:
					dropItem = 178;
					break;
				case 588:
					dropItem = 182;
					break;
			}
			return dropItem;
		}
		private int GetPickaxeDamage(int x, int y, int pickPower, int hitBufferIndex, Tile tileTarget)
		{
			Player player = Main.LocalPlayer;
			int num = 0;
			if (Main.tileNoFail[tileTarget.TileType])
				num = 100;

			if (Main.tileDungeon[tileTarget.TileType] || tileTarget.TileType == 25 || tileTarget.TileType == 58 || tileTarget.TileType == 117 || tileTarget.TileType == 203)
				num += pickPower / 2;
			else if (tileTarget.TileType == 85)
				num += pickPower / 3;
			else if (tileTarget.TileType == 48 || tileTarget.TileType == 232)
				num += pickPower * 2;
			else if (tileTarget.TileType == 226)
				num += pickPower / 4;
			else if (tileTarget.TileType == 107 || tileTarget.TileType == 221)
				num += pickPower / 2;
			else if (tileTarget.TileType == 108 || tileTarget.TileType == 222)
				num += pickPower / 3;
			else if (tileTarget.TileType == 111 || tileTarget.TileType == 223)
				num += pickPower / 4;
			else if (tileTarget.TileType == 211)
				num += pickPower / 5;
			else
				TileLoader.MineDamage(pickPower, ref num);
			if (tileTarget.TileType == 211 && pickPower < 200)
				num = 0;

			if ((tileTarget.TileType == 25 || tileTarget.TileType == 203) && pickPower < 65)
			{
				num = 0;
			}
			else if (tileTarget.TileType == 117 && pickPower < 65)
			{
				num = 0;
			}
			else if (tileTarget.TileType == 37 && pickPower < 50)
			{
				num = 0;
			}
			else if ((tileTarget.TileType == 22 || tileTarget.TileType == 204) && (double)y > Main.worldSurface && pickPower < 55)
			{
				num = 0;
			}
			else if (tileTarget.TileType == 56 && pickPower < 55)
			{
				num = 0;
			}
			else if (tileTarget.TileType == 77 && pickPower < 65 && y >= Main.UnderworldLayer)
			{
				num = 0;
			}
			else if (tileTarget.TileType == 58 && pickPower < 65)
			{
				num = 0;
			}
			else if ((tileTarget.TileType == 226 || tileTarget.TileType == 237) && pickPower < 210)
			{
				num = 0;
			}
			else if (tileTarget.TileType == 137 && pickPower < 210)
			{
				int num2 = tileTarget.TileFrameY / 18;
				if ((uint)(num2 - 1) <= 3u)
					num = 0;
			}
			else if (Main.tileDungeon[tileTarget.TileType] && pickPower < 100 && (double)y > Main.worldSurface)
			{
				if ((double)x < (double)Main.maxTilesX * 0.35 || (double)x > (double)Main.maxTilesX * 0.65)
					num = 0;
			}
			else if (tileTarget.TileType == 107 && pickPower < 100)
			{
				num = 0;
			}
			else if (tileTarget.TileType == 108 && pickPower < 110)
			{
				num = 0;
			}
			else if (tileTarget.TileType == 111 && pickPower < 150)
			{
				num = 0;
			}
			else if (tileTarget.TileType == 221 && pickPower < 100)
			{
				num = 0;
			}
			else if (tileTarget.TileType == 222 && pickPower < 110)
			{
				num = 0;
			}
			else if (tileTarget.TileType == 223 && pickPower < 150)
			{
				num = 0;
			}
			else
			{
				TileLoader.PickPowerCheck(tileTarget, pickPower, ref num);
			}

			if (tileTarget.TileType == 147 || tileTarget.TileType == 0 || tileTarget.TileType == 40 || tileTarget.TileType == 53 || tileTarget.TileType == 57 || tileTarget.TileType == 59 || tileTarget.TileType == 123 || tileTarget.TileType == 224 || tileTarget.TileType == 397)
				num += pickPower;

			if (tileTarget.TileType == 404)
				num += 5;

			if (tileTarget.TileType == 165 || Main.tileRope[tileTarget.TileType] || tileTarget.TileType == 199)
				num = 100;

			if (tileTarget.TileType == 128 || tileTarget.TileType == 269)
			{
				if (tileTarget.TileFrameX == 18 || tileTarget.TileFrameX == 54)
				{
					x--;
					tileTarget = Main.tile[x, y];
					player.hitTile.UpdatePosition(hitBufferIndex, x, y);
				}

				if (tileTarget.TileFrameX >= 100)
				{
					num = 0;
					Main.blockMouse = true;
				}
			}

			if (tileTarget.TileType == 334)
			{
				if (tileTarget.TileFrameY == 0)
				{
					y++;
					tileTarget = Main.tile[x, y];
					player.hitTile.UpdatePosition(hitBufferIndex, x, y);
				}

				if (tileTarget.TileFrameY == 36)
				{
					y--;
					tileTarget = Main.tile[x, y];
					player.hitTile.UpdatePosition(hitBufferIndex, x, y);
				}

				int frameX = tileTarget.TileFrameX;
				bool flag = frameX >= 5000;
				bool flag2 = false;
				if (!flag)
				{
					int num3 = frameX / 18;
					num3 %= 3;
					x -= num3;
					tileTarget = Main.tile[x, y];
					if (tileTarget.TileFrameX >= 5000)
						flag = true;
				}

				if (flag)
				{
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

				if (flag2)
				{
					num = 0;
					Main.blockMouse = true;
				}
			}

			return num;
		}
		private int GetTileStrengthXP(int tileType)
        {
			int xp = 10;
            switch (tileType)
            {
				case 25:
				case 58:
				case 107:
				case 117:
				case 203:
				case 221:
					xp *= 2;
					break;
				case 85:
				case 108:
				case 222:
					xp *= 3;
					break;
				case 111:
				case 223:
				case 226:
					xp *= 4;
					break;
				case 211:
					xp *= 5;
					break;
				default:
					if (Main.tileDungeon[tileType])
                    {
						xp *= 2;
                    }
                    else
                    {
						ModTile modTile = TileLoader.GetTile(tileType);
						xp = modTile != null ? (int)(xp * modTile.MineResist) : xp;
                    }
					break;
            }
			return xp;
        }
	}
}
