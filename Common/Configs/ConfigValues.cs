using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponEnchantments.Common.Configs
{
	internal class ConfigValues
	{
		public static float LinearStrengthMultiplier => WEMod.serverConfig.presetData.linearStrengthMultiplier / 100f;
		public static float RecomendedStrengthMultiplier => WEMod.serverConfig.presetData.recomendedStrengthMultiplier / 100f;

	}
}
