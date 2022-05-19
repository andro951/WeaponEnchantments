using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WeaponEnchantments.Common.Globals
{
    public class VanillaItem : GlobalItem//Not Used
    {
        public int Type { private set; get; }
        public override bool InstancePerEntity => true;
        public override void SetDefaults(Item item)
        {
            switch (item.useStyle)
            {
                case ItemUseStyleID.None when item.useAnimation == 0://Hooks

                    break;
                case ItemUseStyleID.Swing when item.useAnimation == 1 || item.useAnimation == 25://Drill/Chainsaw/Jackhammer

                    break;
                case ItemUseStyleID.EatFood:

                    break;
                case ItemUseStyleID.Thrust:

                    break;
                case ItemUseStyleID.HoldUp:

                    break;
                case ItemUseStyleID.Shoot when item.useAnimation == 4://Chain Gun
                case ItemUseStyleID.Shoot when item.useAnimation == 5://S.D.M.G.
                case ItemUseStyleID.Shoot when item.useAnimation == 6://Celebration Mk2

                    break;
                case ItemUseStyleID.DrinkLong:

                    break;
                case ItemUseStyleID.DrinkOld:

                    break;
                case ItemUseStyleID.GolfPlay:

                    break;
                case ItemUseStyleID.DrinkLiquid:

                    break;
                case ItemUseStyleID.HiddenAnimation:

                    break;
                case ItemUseStyleID.MowTheLawn:

                    break;
                case ItemUseStyleID.Guitar:

                    break;
                case ItemUseStyleID.Rapier:

                    break;
                case ItemUseStyleID.RaiseLamp:

                    break;
            }
        }
        public override GlobalItem Clone(Item item, Item itemClone)
        {
            return base.Clone(item, itemClone);
        }
    }//Not Used
}
