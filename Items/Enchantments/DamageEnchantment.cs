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
	public abstract class DamageEnchantment : Enchantment
	{
		protected override string TypeName => "Damage";
		protected override string NamePrefix => "Enchantments/";

		public override int LowestCraftableTier => 0;
		public override void GetMyStats() {
			Effects = new() {
				new DamageAfterDefenses(EnchantmentStrengthData)
			};
		}

		public override string ShortTooltip => GetShortTooltip(sign: true);
		public override string Artist => "Zorutan";
		public override string ArtModifiedBy => null;
		public override string Designer => "andro951";
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<EnchantmentToggle>().Damage;
		}
	}
	[Autoload(false)]
	public class DamageEnchantmentBasic : DamageEnchantment
	{
		public override SellCondition SellCondition => SellCondition.Always;
		public override List<DropData> NpcAIDrops => new() {
			new(NPCAIStyleID.Slime)
		};
	}
	[Autoload(false)]
	public class DamageEnchantmentCommon : DamageEnchantment { }
	[Autoload(false)]
	public class DamageEnchantmentRare : DamageEnchantment { }
	[Autoload(false)]
	public class DamageEnchantmentEpic : DamageEnchantment { }
	[Autoload(false)]
	public class DamageEnchantmentLegendary : DamageEnchantment { }

	[Autoload(false)]
	public class DamageEnchantmentCursed : DamageEnchantment {}
}
