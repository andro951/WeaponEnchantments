using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Common.Utility;
using androLib.ModIntegration;
using static androLib.ModIntegration.BossChecklistIntegration;

namespace WeaponEnchantments.ModIntegration
{
	public static class BossChecklistIntegration
	{
		public static bool ShouldSetupBossPowerBoosterDrops => !UsedBossChecklistForBossPowerBoosterDrops && BossInfos != null;
		public static bool UsedBossChecklistForBossPowerBoosterDrops = false;
	}
}
