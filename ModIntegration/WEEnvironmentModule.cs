using MagicStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Items;

namespace WeaponEnchantments.ModIntegration
{
    [ExtendsFromMod(MagicStorageIntegration.magicStorageName)]
    public class WEEnvironmentModule : EnvironmentModule
    {
        public override string Name => "Enchanting Table Essence";
		public override IEnumerable<Item> GetAdditionalItems(EnvironmentSandbox sandbox) {
            return Main.LocalPlayer.GetWEPlayer().enchantingTable.essenceItem;
        }
		public override void ModifyCraftingZones(EnvironmentSandbox sandbox, ref CraftingInformation information) {
            int highestTableTierUsed = Main.LocalPlayer.GetWEPlayer().highestTableTierUsed;
            int baseTableTier = ModContent.TileType<Tiles.WoodEnchantingTable>();
            int tableTier;
            if (highestTableTierUsed == 0) {
                tableTier = baseTableTier;
	        }
            else {
                tableTier = baseTableTier - 5 + highestTableTierUsed;
            }

            if (tableTier > -1)
                information.adjTiles[tableTier] = true;
		}
	}
}
