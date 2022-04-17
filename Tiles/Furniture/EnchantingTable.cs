using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using WeaponEnchantments.Common;

namespace WeaponEnchantments.Tiles.Furniture
{
	public class EnchantingTableTile : ModTile
	{
		public int enchantingTableTier = -1;
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

				//DustType = ModContent.DustType<Dusts.Sparkle>();
				AdjTiles = new int[] { TileID.WorkBenches };

				// Placement
				//TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
				//TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
				TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
				TileObjectData.newTile.CoordinateHeights = new[] { 34 };
				TileObjectData.newTile.DrawYOffset = -16;
				TileObjectData.addTile(Type);

				AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);


				// Etc
				ModTranslation name = CreateMapEntryName();
				name.SetDefault(Utility.enchantingTableNames[enchantingTableTier] + " Enchanting Table");
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
				Utility util = new Utility();
				int tableType = -1;
                switch (enchantingTableTier)
                {
					case 0:
						tableType = ModContent.ItemType<Items.EnchantingTable.WoodEnchantingTable>();
						break;
					case 1:
						tableType = ModContent.ItemType<Items.EnchantingTable.DustyEnchantingTable>();
						break;
					case 2:
						tableType = ModContent.ItemType<Items.EnchantingTable.HellishEnchantingTable>();
						break;
					case 3:
						tableType = ModContent.ItemType<Items.EnchantingTable.SoulEnchantingTable>();
						break;
					case 4:
						tableType = ModContent.ItemType<Items.EnchantingTable.UltimateEnchantingTable>();
						break;
				}
				//Mod.Logger.Debug("enchantingTableTier: " + enchantingTableTier.ToString());
				Item.NewItem(new EntitySource_TileBreak(x, y), x * 16, y * 16, 32, 16, tableType);
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
