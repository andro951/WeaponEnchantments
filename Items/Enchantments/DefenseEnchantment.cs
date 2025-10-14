using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using androLib.Items;
using androLib.Common.Utility;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Configs;

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class DefenseEnchantment : Enchantment
	{
		protected override string TypeName => "Defense";
		protected override string NamePrefix => "Enchantments/";
		
		public override int StrengthGroup => 3;
		public override int LowestCraftableTier => 0;
		public override void GetMyStats() {
			Effects = new() {
				new Defense(@base: EnchantmentStrengthData),
			};

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Weapons, 0.5f },
				{ EItemType.Tools, 0.5f },
				{ EItemType.FishingPoles, 0.5f },
				{ EItemType.Armor, 1f },
				{ EItemType.Accessories, 1f }
			};
		}
		protected override List<List<EnchantmentEffect>> cursedEffectPossibilities => defaultDefensiveCursedEffectPossibilities;

		public override string ShortTooltip => GetShortTooltip(sign: true, percent: false, multiply100: false);
		public override string Artist => "Zorutan";
		public override string ArtModifiedBy => null;
		public override string Designer => "andro951";
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<EnchantmentToggle>().Defense;
		}
	}
	[Autoload(false)]
	public class DefenseEnchantmentBasic : DefenseEnchantment
	{
		public override SellCondition SellCondition => SellCondition.Always;
		public override List<DropData> NpcAIDrops => new() {
			new(NPCAIStyleID.Fighter, 0.95f)
		};
	}
	[Autoload(false)]
	public class DefenseEnchantmentCommon : DefenseEnchantment { }
	[Autoload(false)]
	public class DefenseEnchantmentRare : DefenseEnchantment { }
	[Autoload(false)]
	public class DefenseEnchantmentEpic : DefenseEnchantment { }
	[Autoload(false)]
	public class DefenseEnchantmentLegendary : DefenseEnchantment { }
	[Autoload(false)]
	public class DefenseEnchantmentCursed : DefenseEnchantment { }

}