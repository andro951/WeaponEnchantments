using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using WeaponEnchantments.Common.Utility;

namespace WeaponEnchantments.Common
{
	public struct BuffStats
	{
		public string BuffName { get; private set; }
		public int BuffID { get; private set; }
		public Time Duration { get; private set; }
		public float Chance { get; private set; }

		public BuffStats(string buffName, int buffID, Time duration, float chance) {
			BuffName = buffName;
			BuffID = buffID;
			Duration = duration;
			Chance = chance;
		}

		public void AddBuff(Player player) {
			bool applyBuff = false;
			for (float i = 0; i < Chance; i += 1f) {
				if ()
			}
			if ()
		}
	}
}
