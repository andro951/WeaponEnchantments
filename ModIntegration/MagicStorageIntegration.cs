using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using androLib.ModIntegration;
using androLib;
using Terraria.ID;
using Terraria;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Common.Utility;
using System.Runtime.CompilerServices;

namespace WeaponEnchantments.ModIntegration
{
	[JITWhenModsEnabled(AndroMod.magicStorageName)]
	public class MagicStorageIntegration : ModSystem
	{
		private Item lastHoverItem = new Item();
		private Item lastMouseItem = new Item();
		private int lastMouseItemStack = 0;

		public static bool JustCraftedStackableItem = false;
		[MethodImpl(MethodImplOptions.NoInlining)]
		public override void Load() {
			if (AndroMod.magicStorageEnabled)
				RegisterHandleOnTickEvents();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void RegisterHandleOnTickEvents() {
			androLib.ModIntegration.MagicStorageIntegration.HandleMagicStorageOnTickEvents += HandleOnTickEvents;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void HandleOnTickEvents() {
			Item mouseItem = Main.mouseItem;
			Item hoverItem = Main.HoverItem;
			if (Main.mouseRight) {
				if (lastHoverItem.type == mouseItem.type && (lastHoverItem.stack > 1 && lastHoverItem.type == hoverItem.type || !lastMouseItem.IsAir && mouseItem.stack == lastMouseItemStack + 1)) {
					if (lastHoverItem.TryGetEnchantedItemSearchAll(out EnchantedItem henchantedItem) && henchantedItem.Enchanted) {
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
