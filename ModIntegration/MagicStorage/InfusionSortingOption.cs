using androLib;
using MagicStorage.CrossMod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Common.Utility;

namespace WeaponEnchantments.ModIntegration.MagicStorage {
	[ExtendsFromMod(AndroMod.magicStorageName)]
	public sealed class SortInfusionPower : SortingOption {
		public override IComparer<Item> Sorter => CompareInfusionPower.Instance;
		public override string Texture => "WeaponEnchantments/ModIntegration/MagicStorage/SortInfusionPower";
		public override string Name => "InfusionPower";
		public override Position GetDefaultPosition() => new AfterParent(SortingOptionLoader.Definitions.QuantityRatio);

		private class CompareInfusionPower : IComparer<Item> {
			public static CompareInfusionPower Instance = new CompareInfusionPower();
			public int Compare(Item x, Item y) {
				int xInfusionPower = x.TryGetEnchantedWeapon(out EnchantedWeapon enchantedWeaponX) ? enchantedWeaponX.GetInfusionPower(ref x) : -1;
				int yInfusionPower = y.TryGetEnchantedWeapon(out EnchantedWeapon enchantedWeaponY) ? enchantedWeaponY.GetInfusionPower(ref y) : -1;
				return xInfusionPower - yInfusionPower;
			}
		}
	}

}
