using System.Collections.Generic;
using WeaponEnchantments.Common;

namespace WeaponEnchantments.Items.Enchantments.Utility
{
	public abstract class PeaceEnchantment : Enchantment {
		public override int StrengthGroup => 2;
		public override float ScalePercent => -1f;
		public override Dictionary<string, float> AllowedList => new Dictionary<string, float>() {
			{ "Weapon", 1f },
			{ "Armor", 1f },
			{ "Accessory", 1f }
		};
		public override void GetMyStats() {
			AddEStat("spawnRate", 0f, 1f / EnchantmentStrength);
			AddEStat("maxSpawns", 0f, 1f / EnchantmentStrength);
		}

		public override string Artist => "Zorutan";
		public override string Designer => "andro951";
	}
	public class PeaceEnchantmentBasic : PeaceEnchantment { }
	public class PeaceEnchantmentCommon : PeaceEnchantment { }
	public class PeaceEnchantmentRare : PeaceEnchantment { }
	public class PeaceEnchantmentSuperRare : PeaceEnchantment { }
	public class PeaceEnchantmentUltraRare : PeaceEnchantment { }

}
