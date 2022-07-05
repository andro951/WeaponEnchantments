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
        public override void Load()
        {
            Enabled = ModLoader.HasMod(magicStorageName);
            if (Enabled)
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
                        Mod.AddContent(combining);
                    }
                }
                /*var weEnvironmentModule = new WEEnvironmentModule();
                Mod.AddContent(weEnvironmentModule);*/
            }
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void LoadIntegration()
        {
            if(ModLoader.TryGetMod(magicStorageName, out Mod magicStorage))
                for (int i = 0; i < ItemLoader.ItemCount; i++)
                {
                    if (WEMod.IsEnchantable(new Item(i)))
                    {
                        var combining = new MagicStorageItemCombining(i);
                        magicStorage.AddContent(combining);
                    }
                }
        }
        public override void Unload()
        {
            if (Enabled)
                UnloadIntegration();
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void UnloadIntegration()
        {

        }

    }
}
