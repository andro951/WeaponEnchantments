using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using WeaponEnchantments.Common.Utility;

namespace WeaponEnchantments.Common.Globals
{
    public class BossSummonItemGlobal : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
            if (WEMod.calamityEnabled)
                return false;

            return entity.useStyle == ItemUseStyleID.HoldUp && entity.consumable && entity.useAnimation == 45 && entity.useTime == 45;
        }
        public override bool? UseItem(Item item, Player player) {
            float spawnRateFromEnchantments = player.ApplyEStatFromPlayer(player.GetWEPlayer().trackedWeapon, "spawnRate", 1f);
            if (spawnRateFromEnchantments > 1.6f)
                item.stack++;

            return null;
        }
    }
}
