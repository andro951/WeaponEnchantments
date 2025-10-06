using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Localization;
using Terraria.ModLoader;

namespace WeaponEnchantments.Debuffs {
	public abstract class WEBuff : ModBuff {
		public abstract string LocalizationDescription { get; }
		public override LocalizedText Description => LocalizedText.Empty;
	}
}
