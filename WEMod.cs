using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using WeaponEnchantments.Items;

namespace WeaponEnchantments
{
    internal class WEMod : Mod
    {
		internal static ModKeybind WeaponEnchantmentUIHotkey;
		internal static bool IsEnchantable(Item item)
        {
			if(IsWeaponItem(item) || IsArmorItem(item) || IsAccessoryItem(item))
            {
				return true;
			}
            else
            {
				return false;
            }
        }
		internal static bool IsWeaponItem(Item item)
		{
			return item.damage > 0;
		}
		internal static bool IsArmorItem(Item item)
		{
			return !item.vanity && (item.headSlot > -1 || item.bodySlot > -1 || item.legSlot > -1);
		}
		internal static bool IsAccessoryItem(Item item)
		{
			return item.accessory == true;
		}
		internal static bool IsEnchantmentItem(Item item, bool utility)
        {
			for (int i = 0; i < Enchantments.IDs.Count; i++)
            {
				for(int j = 0; j < Enchantments.rarity.Length; j++)
                {
					if(Enchantments.IDs[i][j] == item.type)
                    {
						return true;
                    }
                }
            }
			return false;
            /*try
            {
				if(((Enchantments)item.ModItem) != null)
                {
					return true;
				}
                else
                {
					return false;
                }			
			}
            catch (InvalidCastException)
            {
				return false;
			}*/
        }
		internal static bool IsEssenceItem(Item item)
        {
			return true;
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
