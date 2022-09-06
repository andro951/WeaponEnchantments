using MagicStorage;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Common.Utility;

namespace WeaponEnchantments.ModIntegration
{
    [JITWhenModsEnabled(magicStorageName)]
    public class MagicStorageIntegration : ModSystem
    {
        public const string magicStorageName = "MagicStorage";
        public static bool Enabled { get; private set; }
        private Item lastHoverItem = new Item();
        private Item lastMouseItem = new Item();
        private int lastMouseItemStack = 0;
        public override void Load() {
            Enabled = ModLoader.HasMod(magicStorageName);

            WEMod.magicStorageEnabled = Enabled;
        }
        public override void PostDrawInterface(SpriteBatch spriteBatch) {
			if (Enabled)
                HandleOnTickEvents();
        }
        public static bool MagicStorageIsOpen() {
            if (!Enabled)
                return false;

            return IsOpen();
        }
        public static void TryClosingMagicStorage() {
            if (MagicStorageIsOpen())
                CloseMagicStorage();
        }


        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void CloseMagicStorage() {
            StoragePlayer.LocalPlayer.CloseStorage();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static bool IsOpen() {
            return Main.playerInventory && MagicStorage.StoragePlayer.LocalPlayer.ViewingStorage().X >= 0;
        }
	    
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void HandleOnTickEvents() {
            if (IsOpen()) {
                if (Main.netMode < NetmodeID.Server) {
                    WEPlayer wePlayer = Main.LocalPlayer.GetWEPlayer();
                    if (wePlayer.usingEnchantingTable)
                        WEModSystem.CloseWeaponEnchantmentUI(true);
                }

                Item mouseItem = Main.mouseItem;
                Item hoverItem = Main.HoverItem;
                if (Main.mouseRight) {
                    if (lastHoverItem.type == mouseItem.type && (lastHoverItem.stack > 1 && lastHoverItem.type == hoverItem.type || !lastMouseItem.IsAir && mouseItem.stack == lastMouseItemStack + 1)) {
                        if (lastHoverItem.TryGetEnchantedItem(out EnchantedItem hiGlobal) && (hiGlobal.Experience > 0 || hiGlobal.PowerBoosterInstalled || hiGlobal.infusedItemName != "")) {
                            if (mouseItem.stack == 1) {
                                Main.mouseItem = new Item(mouseItem.type);
			                }
                            else if (lastHoverItem.stack == 1 && hoverItem.stack == 0) {
                                Main.mouseItem = lastHoverItem.Clone();
                                Main.mouseItem.stack = mouseItem.stack;
                            }
                        }
                    }
		    
                    lastMouseItemStack = mouseItem.stack;
                }
		
                lastHoverItem = hoverItem;
                lastMouseItem = mouseItem;
            }
        }
	}
}
