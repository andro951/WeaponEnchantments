using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using WeaponEnchantments;
using WeaponEnchantments.Common;

namespace WeaponEnchantments.ModIntegration
{
	[ExtendsFromMod(MagicStorageIntegration.magicStorageName)]
	public class MagicStorageItemCombining : MagicStorage.ItemCombining
	{
		public readonly int itemType;
		public override int TargetItemType => itemType;
		public override string Name => "WeaponEnchantments_" + itemType;
		public MagicStorageItemCombining(int type) => itemType = type;

		public override bool CanCombine(Item item1, Item item2) => !(WEMod.IsEnchantable(item1) && (item1.G().experience > 0 || item1.G().powerBoosterInstalled || item1.G().infusedItemName != "" || item2.G().experience > 0 || item2.G().powerBoosterInstalled || item2.G().infusedItemName != ""));
	}
}
