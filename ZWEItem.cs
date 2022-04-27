/*
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace WeaponEnchantments
{
    public class WEItem : GlobalItem
	{
		public string weaponName;//replace
		public string weaponOwner;

		public WEItem()
		{
			weaponName = "";//replace
			weaponOwner = "";
		}

		public override bool InstancePerEntity => true;

		public override GlobalItem Clone(Item item, Item itemClone)
		{
			WEItem myClone = (WEItem)base.Clone(item, itemClone);
			//myClone.weaponName = weaponName;
			//myClone.weaponOwner = weaponOwner;
			return myClone;
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			if (weaponName.Length > 0)
			{
				Color color = Color.Lerp(Color.White, Color.Orange, 0.4f);
				tooltips.Add(new TooltipLine(Mod, "PetName", "Pet Name: " + weaponName) //Change
				{
					overrideColor = color
				});
				tooltips.Add(new TooltipLine(Mod, "PetOwner", "Owner: " + weaponOwner) //Change
				{
					overrideColor = color
				});
			}
		}

		public override void LoadData(Item item, TagCompound tag)
		{
			weaponName = tag.GetString("weaponName");
			weaponOwner = tag.GetString("weaponOwner");
		}

		public override void SaveData(Item item, TagCompound tag)
		{
			if (!(weaponName.Length > 0 && WEMod.IsWeaponItem(item)))
			{
				return;
			}

			tag.Add("weaponName", weaponName);
			tag.Add("weaponOwner", weaponOwner);
		}

		public override void NetSend(Item item, BinaryWriter writer)
		{
			writer.Write(weaponName);
			writer.Write(weaponOwner);
		}

		public override void NetReceive(Item item, BinaryReader reader)
		{
			weaponName = reader.ReadString();
			weaponOwner = reader.ReadString();
		}
	}
}
*/