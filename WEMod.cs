using MonoMod.RuntimeDetour.HookGen;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;
using Terraria.ModLoader.IO;
using WeaponEnchantments.Common;
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
			return item != null && (item.damage > 0 && item.ammo == 0 || item.type == ItemID.CoinGun) && !item.accessory && !item.IsAir;
		}
		internal static bool IsArmorItem(Item item)
		{
			return item != null && !item.vanity && (item.headSlot > -1 || item.bodySlot > -1 || item.legSlot > -1) && !item.IsAir;
		}
		internal static bool IsAccessoryItem(Item item)
		{
			return item != null && item.accessory && !IsArmorItem(item) && !item.IsAir;
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

		private delegate Item orig_ItemIOLoad(TagCompound tag);
		private delegate Item hook_ItemIOLoad(orig_ItemIOLoad orig, TagCompound tag);
		private static readonly MethodInfo ModLoaderIOItemIOLoadMethodInfo = 
			typeof(Main).Assembly.GetType("Terraria.ModLoader.IO.ItemIO")!.GetMethod("Load", BindingFlags.Public | BindingFlags.Static, new System.Type[] { typeof(TagCompound) })!;

		public override void Load()
        {
			HookEndpointManager.Add<hook_ItemIOLoad>(ModLoaderIOItemIOLoadMethodInfo, ItemIOLoadDetour);
        }
		public override void Unload()
		{
			return;
		}
		private Item ItemIOLoadDetour(orig_ItemIOLoad orig, TagCompound tag)
        {
			Item item = orig(tag);
			if(item.ModItem is UnloadedItem)
            {
				OldItemManager.ReplaceOldItem(ref item);
				//orig(item, tag);
			}
			return item;
        }


		/*private delegate void orig_ItemIOLoad(Item item, TagCompound tag);
		private delegate void hook_ItemIOLoad(orig_ItemIOLoad orig, Item item, TagCompound tag);
		private static readonly MethodInfo ModLoaderIOItemIOLoadMethodInfo = 
			typeof(Main).Assembly.GetType("Terraria.ModLoader.IO.ItemIO")!.GetMethod("Load", BindingFlags.Public | BindingFlags.Static, new System.Type[] { typeof(Item), typeof(TagCompound) })!;

		public override void Load()
        {
			HookEndpointManager.Add<hook_ItemIOLoad>(ModLoaderIOItemIOLoadMethodInfo, ItemIOLoadDetour);
        }
		public override void Unload()
		{
			return;
		}
		private void ItemIOLoadDetour(orig_ItemIOLoad orig, Item item, TagCompound tag)
        {
			orig(item, tag);
			if(item.ModItem is UnloadedItem)
            {
				OldItemManager.ReplaceOldItem(ref item);
				//orig(item, tag);
			 }
        }*/

		/*private delegate void orig_ItemIOLoad(Item item, TagCompound tag);
		private delegate void hook_ItemIOLoad(orig_ItemIOLoad orig, Item item, TagCompound tag);
		private static readonly MethodInfo ModLoaderIOItemIOLoadMethodInfo = 
			typeof(Main).Assembly.GetType("Terraria.ModLoader.Default.UnloadedGlobalItem")!.GetMethod("LoadData", BindingFlags.Public, new System.Type[] { typeof(Item), typeof(TagCompound) })!;
		public override void Load()
        {
			HookEndpointManager.Add<hook_ItemIOLoad>(ModLoaderIOItemIOLoadMethodInfo, ItemIOLoadDetour);
        }
		public override void Unload()
		{
			return;
		}
		private void ItemIOLoadDetour(orig_ItemIOLoad orig, Item item, TagCompound tag)
        {
			orig(item, tag);
			if(item.ModItem is UnloadedItem)
            {
				OldItemManager.ReplaceOldItem(ref item);
				//orig(item, tag);
			}
        }*/



		/*OldItemManager.ReplaceOldItem(ref item);
            if(item.Name == "Unloaded Item")
                base.LoadData(item, tag);*/
	}
}