using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace WeaponEnchantments.Common.Globals
{
    public class BossSummonItemGlobal : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
            if (WEMod.calamity)
                return false;

            return entity.useStyle == ItemUseStyleID.HoldUp && entity.consumable && entity.useAnimation == 45 && entity.useTime == 45;
        }
        public override bool? UseItem(Item item, Player player) {
            float spawnRateFromEnchantments = player.AEP(player.G().trackedWeapon, "spawnRate", 1f);
            if (spawnRateFromEnchantments > 1.6f)
                item.stack++;

            return null;
        }
    }
}
