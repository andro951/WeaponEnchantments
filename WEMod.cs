using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Items;


namespace WeaponEnchantments
{
    public class WEMod : Mod
    {
		internal static bool IsEnchantable(Item item)
        {
			if((IsWeaponItem(item) || IsArmorItem(item) || IsAccessoryItem(item)) && !item.consumable)
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
			return item.accessory && !IsArmorItem(item);
		}
		internal static bool IsEnchantmentItem(Item item, bool utility)
        {
			if(item.ModItem is AllForOneEnchantmentBasic)
            {
                if (utility)
                {
                    if (((AllForOneEnchantmentBasic)item.ModItem).Utility)
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
			if (item.ModItem is EnchantmentEssenceBasic)
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