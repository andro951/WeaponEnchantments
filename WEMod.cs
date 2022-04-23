using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace WeaponEnchantments
{
    internal class WEMod : Mod
    {
		internal static ModKeybind WeaponEnchantmentUIHotkey;
		internal static bool IsWeaponItem(Item item)
		{
			//return item.type > ItemID.None && item.shoot > ProjectileID.None && //Change
			//item.buffType > 0 && item.buffType < Main.vanityPet.Length && //Change
			//(Main.vanityPet[item.buffType] || Main.lightPet[item.buffType]); //Change
			return true; //Temporary
		}
		internal static bool IsArmorItem(Item item)
		{
			return true;//Temportary
		}
		internal static bool IsAccessoryItem(Item item)
		{
			return true;//Temporary
		}
		public override void Load()
		{
			WeaponEnchantmentUIHotkey = KeybindLoader.RegisterKeybind(this, "Enchant Weapon", "P");//Temporary
		}
		public override void Unload()
		{
			WeaponEnchantmentUIHotkey = null; //Why?
		}
		public override void PostSetupContent()
		{
			List<int> tempList = new List<int>();
			//for (int i = Main.maxProjectileTypes; i < ProjectileLoader.ProjectileCount; i++)  //Change me
			{

			}
		}
    }
}
