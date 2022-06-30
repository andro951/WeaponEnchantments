using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
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
                LoadIntegration();
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void LoadIntegration()
        {

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
