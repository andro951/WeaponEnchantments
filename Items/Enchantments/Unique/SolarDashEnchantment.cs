using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using androLib.Items;
using androLib.Common.Utility;
using Terraria.ModLoader;

namespace WeaponEnchantments.Items.Enchantments.Unique
{
	public abstract class SolarDashEnchantment : Enchantment
	{
		public override int StrengthGroup => 1;
		public override int ArmorSlotSpecific => (int)ArmorSlotSpecificID.Legs;
		public override void GetMyStats() {
			DashID dash = EnchantmentTier >= 3 ? DashID.SolarDash : EnchantmentTier > 1 ? DashID.NinjaTabiDash : DashID.EyeOfCthulhuShieldDash;

			Effects = new() {
				new VanillaDash(dash, EnchantmentStrengthData)
			};

			if (EnchantmentTier > 0)
				Effects.Add(new MovementSpeed(additive: EnchantmentStrengthData));

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Armor, 1f }
			};
		}

		public override string ShortTooltip => GetShortTooltip(showValue: false);
		public override string Artist => "Zorutan";
		public override string ArtModifiedBy => "𝐍𝐢𝐱𝐲♱";
		public override string Designer => "andro951";
	}
	[Autoload(false)]
	public class SolarDashEnchantmentBasic : SolarDashEnchantment
	{
		public override SellCondition SellCondition => SellCondition.PostDeerclops;
		public override List<DropData> NpcDropTypes => new() {
			new(NPCID.Deerclops)
		};
	}
	[Autoload(false)]
	public class SolarDashEnchantmentCommon : SolarDashEnchantment { }
	[Autoload(false)]
	public class SolarDashEnchantmentRare : SolarDashEnchantment { }
	[Autoload(false)]
	public class SolarDashEnchantmentEpic : SolarDashEnchantment { }
	[Autoload(false)]
	public class SolarDashEnchantmentLegendary : SolarDashEnchantment { }
}
