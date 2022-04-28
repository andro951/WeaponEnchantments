﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.UI;
using Terraria.UI.Gamepad;
using WeaponEnchantments;
using WeaponEnchantments.Common;
using WeaponEnchantments.Items;

namespace WeaponEnchantments.Tiles
{
	public class EnchantingTableTile : ModTile
	{
		public int enchantingTableTier = -1;
		public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
		public override void SetStaticDefaults()
		{
			if (enchantingTableTier > -1)
			{
				// Properties
				Main.tileTable[Type] = true;
				Main.tileSolidTop[Type] = true;
				Main.tileNoAttach[Type] = true;
				Main.tileLavaDeath[Type] = false;
				Main.tileFrameImportant[Type] = true;
				TileID.Sets.DisableSmartCursor[Type] = true;
				TileID.Sets.IgnoredByNpcStepUp[Type] = true; // This line makes NPCs not try to step up this tile during their movement. Only use this for furniture with solid tops.
				TileID.Sets.BasicChest[Type] = true;

				//DustType = ModContent.DustType<Dusts.Sparkle>();
				AdjTiles = new int[] { TileID.WorkBenches };

				// Placement
				//TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
				//TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
				TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
				TileObjectData.newTile.CoordinateHeights = new[] { 34 };
				TileObjectData.newTile.DrawYOffset = -16;
				TileObjectData.newTile.LavaDeath = false;
				TileObjectData.addTile(Type);

				AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);


				// Etc
				ModTranslation name = CreateMapEntryName();
				name.SetDefault(EnchantingTableItem.enchantingTableNames[enchantingTableTier] + " Enchanting Table");
				AddMapEntry(new Color(200, 200, 200), name);
			}
		}

		//public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;  //Causes error

		public override void NumDust(int x, int y, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
		private int[] enchantingTableTypes = new int[5];
		public override void KillMultiTile(int x, int y, int frameX, int frameY)
		{
			if (enchantingTableTier > -1)
			{
				int tableType = -1;
                switch (enchantingTableTier)
                {
					case 0:
						tableType = ModContent.ItemType<Items.EnchantingTableItem.WoodEnchantingTable>();
						break;
					case 1:
						tableType = ModContent.ItemType<Items.EnchantingTableItem.DustyEnchantingTable>();
						break;
					case 2:
						tableType = ModContent.ItemType<Items.EnchantingTableItem.HellishEnchantingTable>();
						break;
					case 3:
						tableType = ModContent.ItemType<Items.EnchantingTableItem.SoulEnchantingTable>();
						break;
					case 4:
						tableType = ModContent.ItemType<Items.EnchantingTableItem.UltimateEnchantingTable>();
						break;
				}
				//Mod.Logger.Debug("enchantingTableTier: " + enchantingTableTier.ToString());
				Item.NewItem(new EntitySource_TileBreak(x, y), x * 16, y * 16, 32, 16, tableType);
				WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
				wePlayer.enchantingTableUI.OnDeactivate();
			}
		}
		public override bool RightClick(int x, int y)
        {
			WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
			wePlayer.Player.CloseSign();
			wePlayer.Player.SetTalkNPC(-1);
			Main.npcChatCornerItem = 0;
			Main.npcChatText = "";
			if (Main.editChest)
			{
				SoundEngine.PlaySound(SoundID.MenuTick);
				Main.editChest = false;
				Main.npcChatText = string.Empty;
			}
			Main.stackSplit = 600;
			if (wePlayer.usingEnchantingTable)
			{
				//wePlayer.usingEnchantingTable = false;
				//SoundEngine.PlaySound(SoundID.MenuClose);
				WEModSystem.CloseWeaponEnchantmentUI();//Move to on tick check
			}
			else
			{
				//wePlayer.usingEnchantingTable = true;
				wePlayer.Player.chest = -1;
				//for each itemslot, i   ItemSlot.SetGlow(i, -1f, chest: true);?
				Main.playerInventory = true;
				UILinkPointNavigator.ForceMovementCooldown(120);
				if (PlayerInput.GrappleAndInteractAreShared)
				{
					PlayerInput.Triggers.JustPressed.Grapple = false;
				}
				Main.recBigList = false;
				wePlayer.Player.chestX = x;
				wePlayer.Player.chestY = y;
				SoundEngine.PlaySound(SoundID.MenuTick);
				WEModSystem.OpenWeaponEnchantmentUI();//Move to on tick check
				Recipe.FindRecipes();
			}

			wePlayer.enchantingTableTier = enchantingTableTier;
			Main.mouseRightRelease = false;
			/*
			Tile tile = Main.tile[x, y];
			int left = x;
			int top = y;
			if(tile.TileFrameX % 36 != 0)//Not sure why this is here
            {
				left--;
            }
			if(tile.TileFrameY != 0)//Not sure why this is here
			{
				top--;
            }
			*/
			//WEModSystem.ToggleWeaponEnchantmentUI();
			Recipe.FindRecipes();
			return true;
		}
        public override void MouseOver(int x, int y)
        {
			WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
			Tile tile = Main.tile[x, y];
			int left = x;
			int top = y;
			if (tile.TileFrameX % 36 != 0)//Dont know what theese are for
			{
				left--;
			}
			if (tile.TileFrameY != 0)//Dont know what theese are for
			{
				top--;
			}
			//wePlayer.Player.cursorItemIconText = EnchantingTableItem.enchantingTableNames[enchantingTableTier] + " Enchanting Table";
			wePlayer.Player.cursorItemIconID = EnchantingTableItem.IDs[enchantingTableTier];
			wePlayer.Player.noThrow = 2;
			wePlayer.Player.cursorItemIconEnabled = true;
		}
        public override void MouseOverFar(int x, int y)
        {
            MouseOver(x, y);
			WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
			if(wePlayer.Player.cursorItemIconText == "")
            {
				wePlayer.Player.cursorItemIconEnabled = false;
				wePlayer.Player.cursorItemIconID = 0;
            }
		}
        public class WoodEnchantingTable : EnchantingTableTile
        {
            WoodEnchantingTable() { enchantingTableTier = 0; }
		}
		public class DustyEnchantingTable : EnchantingTableTile
		{
			DustyEnchantingTable() { enchantingTableTier = 1; }
		}
		public class HellishEnchantingTable : EnchantingTableTile
		{
			HellishEnchantingTable() { enchantingTableTier = 2; }
		}
		public class SoulEnchantingTable : EnchantingTableTile
		{
			SoulEnchantingTable() { enchantingTableTier = 3; }
		}
		public class UltimateEnchantingTable : EnchantingTableTile
		{
			UltimateEnchantingTable() { enchantingTableTier = 4; }
		}
	}
}