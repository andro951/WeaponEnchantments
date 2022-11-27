using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using System.Runtime.CompilerServices;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Common.Utility;
using Terraria.ID;

namespace WeaponEnchantments.ModIntegration
{
	[JITWhenModsEnabled(DBTName)]
	internal class DBZMODPORTIntegration : ModSystem
	{
		public const string DBTName = "DBZMODPORT";
		public static bool Enabled { get; private set; }

		public override void Load()
		{
			Enabled = ModLoader.TryGetMod(DBTName, out Mod DBTMod);
			WEMod.dbtEnabled = Enabled;
		}

	}

}
