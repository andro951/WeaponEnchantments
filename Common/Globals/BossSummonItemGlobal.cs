using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace WeaponEnchantments.Common.Globals
{
    public class BossSummonItemGlobal : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.useStyle == 4 && entity.consumable && entity.useAnimation == 45 && entity.useTime == 45;
        }
        public override bool? UseItem(Item item, Player player)
        {
            if (player.AEP("spawnRate", 1f) > 1f)
                item.stack++;
            return null;
        }
    }
}
