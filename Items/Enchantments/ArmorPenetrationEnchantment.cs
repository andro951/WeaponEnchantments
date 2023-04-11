using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class ArmorPenetrationEnchantment : Enchantment
	{
		public override int StrengthGroup => 4;
		public override void GetMyStats() {
			Effects = new() {
				new PercentArmorPenetration(@base: EnchantmentStrengthData),
			};

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Weapons, 1f }
			};
		}

		public override string ShortTooltip => GetShortTooltip(sign: true, percent: false, multiply100: false);
		public override string Artist => "Zorutan";
		public override string ArtModifiedBy => null;
		public override string Designer => "andro951";
	}
	public class ArmorPenetrationEnchantmentBasic : ArmorPenetrationEnchantment
	{
		public override SellCondition SellCondition => SellCondition.PostSkeletron;
		public override List<DropData> NpcDropTypes => new() {
			new(NPCID.WallofFlesh)
		};
		public override List<DropData> ChestDrops => new() {
			new(ChestID.Shadow, 0.1f),
			new(ChestID.Shadow_Locked, 0.1f)
		};
		public override List<DropData> CrateDrops => new() {
			new(CrateID.Obsidian_LockBox, 0.05f)
		};
	}
	public class ArmorPenetrationEnchantmentCommon : ArmorPenetrationEnchantment { }
	public class ArmorPenetrationEnchantmentRare : ArmorPenetrationEnchantment { }
	public class ArmorPenetrationEnchantmentEpic : ArmorPenetrationEnchantment { }
	public class ArmorPenetrationEnchantmentLegendary : ArmorPenetrationEnchantment { }
}
