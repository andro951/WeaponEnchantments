using androLib.Common.Utility;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Configs;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments.Utility
{
	public abstract class SizeEnchantment : Enchantment
	{
		protected override string TypeName => "Size";
		protected override string NamePrefix => "Enchantments/";
		
		public override int StrengthGroup => 10;
		public override void GetMyStats() {
			Effects = new() {
				new Size(EnchantmentStrengthData),
				new WhipRange(EnchantmentStrengthData),
				new YoyoStringLength(EnchantmentStrengthData)
			};
		}

		public override string ShortTooltip => GetShortTooltip(sign: true);
		public override string Artist => "Zorutan";
		public override string ArtModifiedBy => null;
		public override string Designer => "andro951";
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<EnchantmentToggle>().Size;
		}
	}
	[Autoload(false)]
	public class SizeEnchantmentBasic : SizeEnchantment
	{
		public override List<DropData> NpcAIDrops => new() {
			new(NPCAIStyleID.DemonEye, 4f)
		};
		public override List<DropData> NpcDropTypes => new() {
			new(NPCAIStyleID.EyeOfCthulhu)
		};
		public override List<DropData> ChestDrops => new() {
			new(ChestID.Chest_Normal),
			new(ChestID.LivingWood)
		};
		public override List<DropData> CrateDrops => new() {
			new(CrateID.Wooden, 0.25f),
			new(CrateID.Pearlwood_WoodenHard, 0.25f)
		};
	}
	[Autoload(false)]
	public class SizeEnchantmentCommon : SizeEnchantment { }
	[Autoload(false)]
	public class SizeEnchantmentRare : SizeEnchantment { }
	[Autoload(false)]
	public class SizeEnchantmentEpic : SizeEnchantment { }
	[Autoload(false)]
	public class SizeEnchantmentLegendary : SizeEnchantment { }
	[Autoload(false)]
	public class SizeEnchantmentCursed : SizeEnchantment { }

}
