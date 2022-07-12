using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WeaponEnchantments.ModIntegration
{
    [JITWhenModsEnabled(magicStorageName)]
    public class MagicStorageIntegration : ModSystem
    {
        public const string magicStorageName = "MagicStorage";
        public static bool Enabled { get; private set; }
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

    }
}
