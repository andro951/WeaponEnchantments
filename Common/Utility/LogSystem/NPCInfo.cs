using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility.LogSystem.WebpageComponenets;
using WeaponEnchantments.Items;
using static WeaponEnchantments.Common.Utility.LogSystem.Wiki;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Common.Globals;
using Terraria.GameContent.UI;
using Microsoft.Xna.Framework;
using Terraria.ID;
using static WeaponEnchantments.Common.Utility.LogModSystem;
namespace WeaponEnchantments.Common.Utility.LogSystem
{
	public class NPCInfo
	{
		public ModNPC ModNPC { private set; get; }
		public NPC NPC { private set; get; }
		public NPCInfo(int modNPCType) {
			NPC = ContentSamples.NpcsByNetId[modNPCType];
			ModNPC = NPC.ModNPC;
		}
		public void AddStatistics(WebPage webpage, bool name = true, bool image = true, ) {

		}
	}
}
