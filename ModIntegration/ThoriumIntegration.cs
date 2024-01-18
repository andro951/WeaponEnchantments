using androLib;
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
		public override void Load() {
			if (AndroMod.thoriumEnabled) {
				bool h = AndroMod.thoriumMod.TryFind("HealerDamage", out ThoriumValues.healerRadiation);
				bool b = AndroMod.thoriumMod.TryFind("BardDamage", out ThoriumValues.bard);
			}
		}
	}
}
