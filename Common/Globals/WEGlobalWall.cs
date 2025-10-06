using androLib.Common.Utility;
using System;
using Terraria;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;
using static WeaponEnchantments.Common.Configs.ConfigValues;

namespace WeaponEnchantments.Common.Globals
{
	public class WEGlobalWall : GlobalWall
	{
		public override void KillWall(int i, int j, int type, ref bool fail) {
			WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
			Item heldItem = wePlayer.Player.HeldItem;

			if (!heldItem.IsHammer())
				return;

			if (!heldItem.TryGetEnchantedItemSearchAll(out EnchantedItem hGlobal))
				return;

			int xp = 10;
			//Config multiplier
			if (GatheringExperienceMultiplier != 1f) {
				xp = (int)Math.Round((float)xp * GatheringExperienceMultiplier);
				if (xp < 1)
					xp = 1;
			}

			//Gain xp
			hGlobal.GainXP(wePlayer.Player.HeldItem, xp);
			wePlayer.Player.AllArmorGainXp(xp);
		}
	}
}
