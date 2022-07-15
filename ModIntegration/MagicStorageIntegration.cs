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
		/*public override void Load()
        {
            Enabled = ModLoader.HasMod(magicStorageName);
            if (Enabled)
			{
                LoadIntegration();
            }
        }*/
		/*[MethodImpl(MethodImplOptions.NoInlining)]
        private static void LoadIntegration()
        {
            for (int i = 0; i < ItemLoader.ItemCount; i++)
            {
                Item item;
                bool add;
                if (i < ItemID.Count)
                {
                    item = new Item(i);
                    add = WEMod.IsEnchantable(item);
                }
                else
                {
                    add = true;
                    //item = ItemLoader.GetItem(i).Item;
                }
                if (add)
                {
                    var combining = new MagicStorageItemCombining(i);
                    ModContent.GetInstance<WEMod>().AddContent(combining);
                }
            }
        }*/
		/*public override void Unload()
        {
            if (Enabled)
                UnloadIntegration();
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void UnloadIntegration()
        {

        }*/
		public override void PostDrawInterface(SpriteBatch spriteBatch)
		{
            if (Main.playerInventory && MagicStorage.StoragePlayer.LocalPlayer.ViewingStorage().X >= 0)
			{
                Item mouseItem = Main.mouseItem;
                Item hoverItem = Main.HoverItem;
                if(Main.mouseRight)
                {
                    if(lastHoverItem.type == mouseItem.type && (lastHoverItem.stack > 1 && lastHoverItem.type == hoverItem.type || !lastMouseItem.IsAir && mouseItem.stack == lastMouseItemStack + 1))
					{
                        if (lastHoverItem.TryGetGlobalItem(out EnchantedItem hiGlobal) && (hiGlobal.experience > 0 || hiGlobal.powerBoosterInstalled || hiGlobal.infusedItemName != ""))
                        {
                            if (mouseItem.stack == 1)
                                Main.mouseItem = new Item(mouseItem.type);
                            else if (lastHoverItem.stack == 1 && hoverItem.stack == 0)
                            {
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
