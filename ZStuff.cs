using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.UI;
using WeaponEnchantments.Items;

namespace WeaponEnchantments
{
    internal class ZStuff
    {

		/*
		class AHoverInteractionChecker
        {
			internal HoverStatus AttemptInteraction(WEPlayer player, Rectangle Hitbox)
            {
				bool flag5 = ShouldBlockInteraction(player, Hitbox);
				if (Main.mouseRight && Main.mouseRightRelease && !flag5)
				{
					Main.mouseRightRelease = false;
					player.tileInteractAttempted = true;
					player.tileInteractionHappened = true;
					player.releaseUseTile = false;
					PerformInteraction(player, Hitbox);
				}
			}
        }

		private static void SellOrTrash(Item[] inv, int context, int slot) {
			Player player = Main.player[Main.myPlayer];
			if (inv[slot].type <= 0)
				return;

			if (!inv[slot].favorited) { //Trashes the item
				SoundEngine.PlaySound(7);
				player.trashItem = inv[slot].Clone();
				AnnounceTransfer(new ItemTransferInfo(player.trashItem, context, 6));
				inv[slot].TurnToAir();
				if (context == 3 && Main.netMode == 1)
					NetMessage.SendData(32, -1, -1, null, player.chest, slot);

				Recipe.FindRecipes();
			}
		}
		*/
	}
}
