using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class OneForAllEnchantment : Enchantment
	{
		public override int StrengthGroup => 10;
		public override float ScalePercent => 0.8f;
		public override bool Max1 => true;
		public override void GetMyStats() {
			Effects = new() {
				new OneForAll(@base: EnchantmentStrengthData),
				new AttackSpeed(multiplicative: (EnchantmentStrengthData * -0.2f + 1.5f).Invert()),
				new NPCHitCooldown(multiplicative: EnchantmentStrengthData * -0.2f + 1.5f)
			};

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Weapons, 1f }
			};
		}

		public override string Artist => "Zorutan";
		public override string ArtModifiedBy => null;
		public override string Designer => "andro951";
	}
	public class OneForAllEnchantmentBasic : OneForAllEnchantment
	{
		public override SellCondition SellCondition => SellCondition.PostSkeletron;
		public override List<WeightedPair> NpcDropTypes => new() {
			new(NPCID.Mothron)
		};
		public override SortedDictionary<ChestID, float> ChestDrops => new() {
			{ ChestID.Gold_Locked, 1f },
			{ ChestID.Lihzahrd, 1f }
		};
		public override List<WeightedPair> CrateDrops => new() {
			new(CrateID.Golden_LockBox, 0.45f)
		};
	}
	public class OneForAllEnchantmentCommon : OneForAllEnchantment { }
	public class OneForAllEnchantmentRare : OneForAllEnchantment { }
	public class OneForAllEnchantmentEpic : OneForAllEnchantment { }
	public class OneForAllEnchantmentLegendary : OneForAllEnchantment { }

}
