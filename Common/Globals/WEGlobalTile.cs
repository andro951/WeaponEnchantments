using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;

namespace WeaponEnchantments.Common.Globals
{
    public class WEGlobalTile : GlobalTile
    {
        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            if (wePlayer.Player.HeldItem.pick > 0 || wePlayer.Player.HeldItem.axe > 0 || wePlayer.Player.HeldItem.hammer > 0)
            {
                int xp = 1;
                /*if (Main.tileSolid[type] && (TileID.Sets.Ore[type] || Main.tileMergeDirt[type]))
                {
                    ModTile modTile = TileLoader.GetTile(type);
                    if(modTile != null)
                    {
                        Item dropItem = ItemLoader.GetItem(modTile.ItemDrop).Item;
                        if (dropItem.value > 0)
                        {
                            xp = dropItem.value;
                        }
                    }
                }
                Main.NewText(wePlayer.Player.name + " recieved " + xp.ToString() + " xp from mining.");*/
                wePlayer.Player.HeldItem.GetGlobalItem<EnchantedItem>().GainXP(wePlayer.Player.HeldItem, xp);
            }
        }
    }
}
