using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponEnchantments.UI;
using Terraria.ModLoader;

namespace WeaponEnchantments.Commands
{
		public class CoinCommand : ModCommand
		{
			public override CommandType Type
				=> CommandType.Chat;

			public override string Command
				=> "coin";

			public override string Description
				=> "Show the coin rate UI";

			public override void Action(CommandCaller caller, string input, string[] args)
			{
				ExampleUI.Visible = true;
			}
		}
}
