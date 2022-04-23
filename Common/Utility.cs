using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.UI;
using WeaponEnchantments.Common;
using log4net;

namespace WeaponEnchantments.Common
{
	public class Utility
	{
		//Main.NewText("Test"); //Prints message in game.
		//Mod.Logger.Debug("Tpye: " + Type.ToString() + " enchantingTableTier: " + enchantingTableTier.ToString());

		//ModContent.GetInstance<WeaponEnchantments>();
        //Mod.Logger.Debug("WEPlayers.cs Initialize()");
		
		/*
		public ModItem GetItem(Mod mod, string itemName)
		{
			mod ??= this.Mod;

			if (!ModContent.TryFind(mod.Name, itemName, out ModItem item))
				throw new Exception($"The item {itemName} does not exist in the mod {mod.Name}.\r\nIf you are trying to use a vanilla item, try removing the first argument.");

			return item;
		}
		*/
		/*
		public ModTile GetTile(Mod mod, string tileName)
		{
			mod ??= this.Mod;

			if (!ModContent.TryFind(mod.Name, tileName, out ModItem item))
				throw new Exception($"The item {tileName} does not exist in the mod {mod.Name}.\r\nIf you are trying to use a vanilla item, try removing the first argument.");

			return ;
		}
		*/
	}
}
