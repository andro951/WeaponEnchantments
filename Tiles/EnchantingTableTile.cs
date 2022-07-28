using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.UI.Gamepad;
using WeaponEnchantments.Common.Utility;

namespace WeaponEnchantments.Tiles
{
	public abstract class EnchantingTableTile : ModTile
	{
		protected abstract int DroppedItem { get; }
		protected abstract int EnchantingTier { get; }

		public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');

		public virtual string Artist { private set; get; } = "Zorutan";
		public virtual string Designer { private set; get; } = "andro951";

		public override void SetStaticDefaults()
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
			name.SetDefault(Items.EnchantingTable.enchantingTableNames[EnchantingTier] + " Enchanting Table");
			AddMapEntry(new Color(200, 200, 200), name);

			LogUtilities.UpdateContributorsList(this);
		}
		public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;
		public override void NumDust(int x, int y, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
		public override void KillMultiTile(int x, int y, int frameX, int frameY)
		{
			Item.NewItem(new EntitySource_TileBreak(x, y), x * 16, y * 16, 32, 16, DroppedItem);
			WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
			WEModSystem.CloseWeaponEnchantmentUI();
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
				WEModSystem.CloseWeaponEnchantmentUI();//Move to on tick check
				Recipe.FindRecipes();
			}
			else
			{
				wePlayer.enchantingTableTier = EnchantingTier;
				if (wePlayer.highestTableTierUsed < EnchantingTier)
					wePlayer.highestTableTierUsed = EnchantingTier;
				wePlayer.Player.chest = -1;
				Main.playerInventory = true;
				UILinkPointNavigator.ForceMovementCooldown(120);
				if (PlayerInput.GrappleAndInteractAreShared)
				{
					PlayerInput.Triggers.JustPressed.Grapple = false;
				}
				SoundEngine.PlaySound(SoundID.MenuTick);
				WEModSystem.OpenWeaponEnchantmentUI(false);
				wePlayer.Player.chestX = x;
				wePlayer.Player.chestY = y;
				Recipe.FindRecipes();
				wePlayer.StoreLastFocusRecipe();
			}
			Main.mouseRightRelease = false;
			return true;
		}
        public override void MouseOver(int x, int y)
        {
			WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
			Tile tile = Main.tile[x, y];
			int left = x;
			int top = y;
			if (tile.TileFrameX % 36 != 0)
			{
				left--;
			}
			if (tile.TileFrameY != 0)
			{
				top--;
			}
			wePlayer.Player.cursorItemIconText = "";
			wePlayer.Player.cursorItemIconID = DroppedItem;
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
	}

    public class WoodEnchantingTable : EnchantingTableTile
    {
		protected override int DroppedItem => ModContent.ItemType<Items.WoodEnchantingTable>();
        protected override int EnchantingTier => 0;
    }

    public class DustyEnchantingTable : EnchantingTableTile {
		protected override int DroppedItem => ModContent.ItemType<Items.DustyEnchantingTable>();
		protected override int EnchantingTier => 1;
	}

	public class HellishEnchantingTable : EnchantingTableTile {
		protected override int DroppedItem => ModContent.ItemType<Items.HellishEnchantingTable>();
		protected override int EnchantingTier => 2;
	}

	public class SoulEnchantingTable : EnchantingTableTile {
		protected override int DroppedItem => ModContent.ItemType<Items.SoulEnchantingTable>();
		protected override int EnchantingTier => 3;
	}

	public class UltimateEnchantingTable : EnchantingTableTile {
		protected override int DroppedItem => ModContent.ItemType<Items.UltimateEnchantingTable>();
		protected override int EnchantingTier => 4;
	}
}
