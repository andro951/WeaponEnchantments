using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Items;


namespace WeaponEnchantments
{
    public class WEMod : Mod
    {
		List<Item> itemsWaitingForPacket = new List<Item>();
		internal static bool IsEnchantable(Item item)
        {
			if((IsWeaponItem(item) || IsArmorItem(item) || IsAccessoryItem(item)) & !item.consumable)
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
			return (item.damage > 0 && item.ammo == 0 || item.type == ItemID.CoinGun) && !item.accessory;
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
			if(item.ModItem is Enchantments)
            {
                if (utility)
                {
                    if (((Enchantments)item.ModItem).Utility)
                    {
						return true;
                    }
                    else
                    {
						return false;
                    }
                }
                else
                {
					return true;
				}
			}
            else
            {
				return false;
            }
        }
		internal static bool IsEssenceItem(Item item)
        {
			if (item.ModItem is EnchantmentEssence)
			{
				return true;
			}
			else
			{
				return false;
			}
        }
        public override void Unload()
		{
			
		}
    }
}