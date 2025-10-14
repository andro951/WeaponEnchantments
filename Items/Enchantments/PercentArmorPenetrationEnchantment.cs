using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using androLib.Items;
using androLib.Common.Utility;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Configs;

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class PercentArmorPenetrationEnchantment : Enchantment
	{
		protected override string TypeName => "PercentArmorPenetration";
		protected override string NamePrefix => "Enchantments/";
		
		public override int StrengthGroup => 27;
		public override void GetMyStats() {
			Effects = new() {
				new PercentArmorPenetration(@base: EnchantmentStrengthData),
			};
		}

		public override string ShortTooltip => GetShortTooltip(sign: true, percent: false, multiply100: false);
		public override string Artist => "Zorutan";
		public override string ArtModifiedBy => null;
		public override string Designer => "andro951";
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<EnchantmentToggle>().PercentArmorPenetration;
		}
	}
	[Autoload(false)]
	public class PercentArmorPenetrationEnchantmentBasic : PercentArmorPenetrationEnchantment
	{
		public override SellCondition SellCondition => SellCondition.PostSkeletron;
		public override List<DropData> NpcDropTypes => new() {
			new(NPCID.WallofFlesh, 2f)
		};
		public override List<DropData> ChestDrops => new() {
			new(ChestID.Shadow, chance: 0.2f),
			new(ChestID.Shadow_Locked, chance: 0.2f)
		};
		public override List<DropData> CrateDrops => new() {
			new(CrateID.Obsidian_LockBox, chance: 0.1f)
		};
	}
	[Autoload(false)]
	public class PercentArmorPenetrationEnchantmentCommon : PercentArmorPenetrationEnchantment { }
	[Autoload(false)]
	public class PercentArmorPenetrationEnchantmentRare : PercentArmorPenetrationEnchantment { }
	[Autoload(false)]
	public class PercentArmorPenetrationEnchantmentEpic : PercentArmorPenetrationEnchantment { }
	[Autoload(false)]
	public class PercentArmorPenetrationEnchantmentLegendary : PercentArmorPenetrationEnchantment { }
	[Autoload(false)]
	public class PercentArmorPenetrationEnchantmentCursed : PercentArmorPenetrationEnchantment { }
}
