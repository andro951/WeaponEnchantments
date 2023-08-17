using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using androLib.Common.Utility;
using androLib.Common.Globals;

namespace WeaponEnchantments.Items.Enchantments.Unique
{
	public abstract class MultishotEnchantment : Enchantment
	{
		public override int StrengthGroup => 8;
		public override List<int> RestrictedClass => new() { (int)DamageClassID.Summon };
		public override void GetMyStats() {
			Effects = new() {
				new Multishot(@base: EnchantmentStrengthData)
			};

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Weapons, 1f }
			};
		}
		public override string Artist => "Zorutan";
		public override string ArtModifiedBy => null;
		public override string Designer => "andro951";
	}
	public class MultishotEnchantmentBasic : MultishotEnchantment
	{
		public override SellCondition SellCondition => SellCondition.PostPlantera;
		public override List<DropData> NpcAIDrops => new() {
			new(NPCAIStyleID.BiomeMimic)
		};
	}
	public class MultishotEnchantmentCommon : MultishotEnchantment { }
	public class MultishotEnchantmentRare : MultishotEnchantment { }
	public class MultishotEnchantmentEpic : MultishotEnchantment { }
	public class MultishotEnchantmentLegendary : MultishotEnchantment { }

}
