using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace WeaponEnchantments.ModIntegration
{
	[JITWhenModsEnabled(THORIUM_NAME)]
	internal class ThoriumIntegration : ModSystem
	{
		public const string THORIUM_NAME = "ThoriumMod";
		public static bool Enabled { get; private set; }
		public override void Load() {
			Enabled = ModLoader.TryGetMod(THORIUM_NAME, out Mod thoriumMod);
			WEMod.thoriumEnabled = true;
			if (Enabled) {
				bool h = thoriumMod.TryFind("HealerDamage", out ThoriumValues.healerRadiation);
				bool b = thoriumMod.TryFind("BardDamage", out ThoriumValues.bard);
			}
		}
	}
}
