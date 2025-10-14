using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using androLib.Items;
using androLib.Common.Utility;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Configs;

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class PrideOfTheWeakEnchantment : Enchantment, IStoreAppliedItem
	{
		protected override string TypeName => "PrideOfTheWeak";
		protected override string NamePrefix => "Enchantments/";
		
		public override int StrengthGroup => 23;
		public override int LowestCraftableTier => 0;
		public override float CapacityCostMultiplier => CapacityCostNone;
		public override float ScalePercent => -1f;
		public override bool Max1 => true;
		public override void GetMyStats() {
			Effects = new() {
				new PrideOfTheWeak(multiplicative: EnchantmentStrengthData)
			};

			AllowedList = new() {
				{ EItemType.Weapons, 1f }
			};
		}

		public override string ShortTooltip => GetShortTooltip(percent: false, multiply100: false, multiplicative: true);
		public EnchantedWeapon EnchantedWeapon { get; set; }
		public EnchantedItem EnchantedItem {
			get => EnchantedWeapon;
			set {
				if (value is EnchantedWeapon enchantedWeapon)
					EnchantedWeapon = enchantedWeapon;
			}
		}
		public override float EnchantmentStrength => EnchantedWeapon != null ? 1f + (EnchantmentStrengthData.Value - 1f) * EnchantedWeapon.GetPrideOfTheWeakMultiplier() : EnchantmentStrengthData.Value;
		public override string Artist => "Zorutan";
		public override string ArtModifiedBy => "andro951";
		public override string Designer => "andro951";
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<EnchantmentToggle>().PrideOfTheWeak;
		}
	}
	[Autoload(false)]
	public class PrideOfTheWeakEnchantmentBasic : PrideOfTheWeakEnchantment
	{
		public override SellCondition SellCondition => SellCondition.Always;
		public override List<DropData> NpcAIDrops => new() {
			new(NPCAIStyleID.Slime, chance: 0.05f)
		};
	}
	[Autoload(false)]
	public class PrideOfTheWeakEnchantmentCommon : PrideOfTheWeakEnchantment { }
	[Autoload(false)]
	public class PrideOfTheWeakEnchantmentRare : PrideOfTheWeakEnchantment { }
	[Autoload(false)]
	public class PrideOfTheWeakEnchantmentEpic : PrideOfTheWeakEnchantment { }
	[Autoload(false)]
	public class PrideOfTheWeakEnchantmentLegendary : PrideOfTheWeakEnchantment { }

}
