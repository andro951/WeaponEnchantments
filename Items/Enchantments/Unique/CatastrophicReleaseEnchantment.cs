using System.Collections.Generic;
using WeaponEnchantments.Common;

namespace WeaponEnchantments.Items.Enchantments.Unique
{
	public abstract class CatastrophicReleaseEnchantment : Enchantment
	{
		public override string CustomTooltip => "(Does not work, Not finished)";
		public override int StrengthGroup => 8;
		public override int DamageClassSpecific => (int)DamageTypeSpecificID.Magic;
		public override Dictionary<string, float> AllowedList => new Dictionary<string, float>() {
			{ "Weapon", 1f }
		};
		public override void GetMyStats() {
			AddEStat(EnchantmentTypeName, 0f, 1f, EnchantmentStrength);
			AddEStat("InfinitePenetration", 0f, 1f, 13.13f);
			AddStaticStat("scale", 0f, EnchantmentStrength * 10f);
			AddStaticStat("shootSpeed", 0f, 1f - 0.8f * EnchantmentStrength);
			//AddStaticStat("useTime", 0f, 1000f);
			AddStaticStat("P_autoReuse", EnchantmentStrength);
		}

		public override string Artist => "andro951";
		public override string Designer => "andro951";
	}
	/*public class CatastrophicReleaseEnchantmentBasic : CatastrophicReleaseEnchantment { }
	public class CatastrophicReleaseEnchantmentCommon : CatastrophicReleaseEnchantment { }
	public class CatastrophicReleaseEnchantmentRare : CatastrophicReleaseEnchantment { }
	public class CatastrophicReleaseEnchantmentSuperRare : CatastrophicReleaseEnchantment { }
	public class CatastrophicReleaseEnchantmentUltraRare : CatastrophicReleaseEnchantment { }*/
}
