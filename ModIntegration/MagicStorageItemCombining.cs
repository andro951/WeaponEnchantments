using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using WeaponEnchantments;
using MagicStorage;

namespace WeaponEnchantments.ModIntegration
{
	public class MagicStorageItemCombining : MagicStorage.ItemCombining
	{
			public readonly int itemType;

			public override int TargetItemType => itemType;

			public MagicStorageItemCombining(int type) => itemType = type;

			public override bool CanCombine(Item item1, Item Item2) => item1.G().experience > 0 || item1.G().powerBoosterInstalled || item2.G().experience > 0 || item2.G().powerBoosterInstalled;
	}
}
