using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.UI.Gamepad;
using WeaponEnchantments.Common.Configs;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Items;
using WeaponEnchantments.ModIntegration;

namespace WeaponEnchantments.Tiles
{
	public abstract class EnchantingTableTile : ModTile
	{
		public int enchantingTableTier;
		public static List<int> TableTypes {
			get { 
				if (tableTypes == null) {
					tableTypes = new() {
						ModContent.TileType<WoodEnchantingTable>(),
						ModContent.TileType<DustyEnchantingTable>(),
						ModContent.TileType<HellishEnchantingTable>(),
						ModContent.TileType<SoulEnchantingTable>(),
						ModContent.TileType<UltimateEnchantingTable>()
					};
				}

				return tableTypes;
			} 
		}
		private static List<int> tableTypes;
		public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');

		public virtual string Artist { private set; get; } = "Zorutan";
		public virtual string Designer { private set; get; } = "andro951";

		public override void SetStaticDefaults() {
			GetDefaults();

			//Properties
			Main.tileTable[Type] = true;
			Main.tileSolidTop[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = false;
			Main.tileFrameImportant[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;
			TileID.Sets.IgnoredByNpcStepUp[Type] = true; // This line makes NPCs not try to step up this tile during their movement. Only use this for furniture with solid tops.
			TileID.Sets.BasicChest[Type] = true;

			//Placement
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
			TileObjectData.newTile.CoordinateHeights = new[] { 34 };
			TileObjectData.newTile.DrawYOffset = -16;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.addTile(Type);

			AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);

			//Etc
			LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(200, 200, 200), name);

			List<int> adjTiles = new() { TileID.WorkBenches };
			if (!ConfigValues.useAllRecipes && enchantingTableTier > 0)
				adjTiles.AddRange(TableTypes.GetRange(0, enchantingTableTier));

			AdjTiles = adjTiles.ToArray();
			LogModSystem.UpdateContributorsList(this);
		}
		private void GetDefaults() {
			for (int i = 0; i < Items.EnchantingTableItem.enchantingTableNames.Length; i++) {
				if (Items.EnchantingTableItem.enchantingTableNames[i] == Name.Substring(0, Items.EnchantingTableItem.enchantingTableNames[i].Length)) {
					enchantingTableTier = i;
					break;
				}
			}
		}
		public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;
		public override void NumDust(int x, int y, bool fail, ref int num) {
			num = fail ? 1 : 3;
		}
		public override void KillMultiTile(int x, int y, int frameX, int frameY) {
			if (enchantingTableTier > -1)
				WEModSystem.CloseWeaponEnchantmentUI();
		}
		public override bool RightClick(int x, int y) {
			WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
			wePlayer.Player.CloseSign();
			wePlayer.Player.SetTalkNPC(-1);
			Main.npcChatCornerItem = 0;
			Main.npcChatText = "";
			if (Main.editChest) {
				SoundEngine.PlaySound(SoundID.MenuTick);
				Main.editChest = false;
				Main.npcChatText = string.Empty;
			}

			Main.stackSplit = 600;
			if (wePlayer.usingEnchantingTable) {
				wePlayer.enchantingTableLocation = new(-1, -1);
				WEModSystem.CloseWeaponEnchantmentUI();
			}
			else {
				if (MagicStorageIntegration.MagicStorageIsOpen())
					MagicStorageIntegration.TryClosingMagicStorage();

				wePlayer.enchantingTableLocation = new(x, y);
				wePlayer.enchantingTableTier = enchantingTableTier;
				if (wePlayer.highestTableTierUsed < enchantingTableTier)
					wePlayer.highestTableTierUsed = enchantingTableTier;

				wePlayer.Player.chest = -1;
				Main.playerInventory = true;
				UILinkPointNavigator.ForceMovementCooldown(120);
				if (PlayerInput.GrappleAndInteractAreShared)
					PlayerInput.Triggers.JustPressed.Grapple = false;

				SoundEngine.PlaySound(SoundID.MenuTick);
				WEModSystem.OpenWeaponEnchantmentUI();
				wePlayer.Player.chestX = x;
				wePlayer.Player.chestY = y;
				Recipe.FindRecipes();
			}

			Main.mouseRightRelease = false;

			return true;
		}
        public override void MouseOver(int x, int y) {
			WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
			wePlayer.Player.cursorItemIconText = "";
			wePlayer.Player.cursorItemIconID = Items.EnchantingTableItem.IDs[enchantingTableTier];
			wePlayer.Player.noThrow = 2;
			wePlayer.Player.cursorItemIconEnabled = true;
		}
        public override void MouseOverFar(int x, int y) {
            MouseOver(x, y);
			WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
			if(wePlayer.Player.cursorItemIconText == "") {
				wePlayer.Player.cursorItemIconEnabled = false;
				wePlayer.Player.cursorItemIconID = 0;
            }
		}
	}
	public class WoodEnchantingTable : EnchantingTableTile { }
	public class DustyEnchantingTable : EnchantingTableTile { }
	public class HellishEnchantingTable : EnchantingTableTile { }
	public class SoulEnchantingTable : EnchantingTableTile { }
	public class UltimateEnchantingTable : EnchantingTableTile { }
}
