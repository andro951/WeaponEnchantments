using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using androLib.Common.Utility;
using androLib.Common.Globals;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Configs;

namespace WeaponEnchantments.Items.Enchantments.Unique
{
	public abstract class MultishotEnchantment : Enchantment
	{
		protected override string TypeName => "Multishot";
		protected override string NamePrefix => "Enchantments/";
		
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
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<EnchantmentToggle>().Multishot;
		}
	}
	[Autoload(false)]
	public class MultishotEnchantmentBasic : MultishotEnchantment
	{
		public override SellCondition SellCondition => SellCondition.PostPlantera;
		public override List<DropData> NpcAIDrops => new() {
			new(NPCAIStyleID.BiomeMimic)
		};
	}
	[Autoload(false)]
	public class MultishotEnchantmentCommon : MultishotEnchantment { }
	[Autoload(false)]
	public class MultishotEnchantmentRare : MultishotEnchantment { }
	[Autoload(false)]
	public class MultishotEnchantmentEpic : MultishotEnchantment { }
	[Autoload(false)]
	public class MultishotEnchantmentLegendary : MultishotEnchantment { }
	[Autoload(false)]
	public class MultishotEnchantmentCursed : MultishotEnchantment { }

}
