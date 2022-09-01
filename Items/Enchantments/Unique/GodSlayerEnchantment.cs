using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments.Unique
{
	public abstract class GodSlayerEnchantment : Enchantment
	{
		public override string CustomTooltip => "(Bonus true damage based on enemy max hp)\n(Bonus damage not affected by LifeSteal)";
		public override int StrengthGroup => 7;
		public override int DamageClassSpecific => (int)DamageClassID.Melee;
		public override SellCondition SellCondition => SellCondition.HardMode;
		public override void GetMyStats() {
			Effects = new() {
				new GodSlayer(@base: EnchantmentStrengthData)
			};

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Weapons, 1f }
			};
		}
		public override string Artist => "Zorutan";
		public override string Designer => "andro951";
	}
	public class GodSlayerEnchantmentBasic : GodSlayerEnchantment
	{
		public override List<WeightedPair> NpcAIDrops => new() {
			new(NPCAIStyleID.BiomeMimic)
		};
	}
	public class GodSlayerEnchantmentCommon : GodSlayerEnchantment { }
	public class GodSlayerEnchantmentRare : GodSlayerEnchantment { }
	public class GodSlayerEnchantmentSuperRare : GodSlayerEnchantment { }
	public class GodSlayerEnchantmentUltraRare : GodSlayerEnchantment { }

}
