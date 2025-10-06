using androLib.Common.Utility;
using System;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Configs;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments.Utility
{
	public abstract class AmmoCostEnchantment : Enchantment
	{
		protected override string TypeName => "AmmoCost";
		protected override string NamePrefix => "Enchantments/";
		
		public override void GetMyStats() {
			
			//AddEStat(EnchantmentTypeName, 0f, 1f, 0f, -EnchantmentStrength);
			Effects = new() {
				new AmmoCost(@base: EnchantmentStrengthData)
			};
		}

		public override string ShortTooltip => GetShortTooltip(text: GetLocalizationTypeName(EnchantmentTypeName + (EnchantmentStrength > 0f ? "1" : "2")));

		public override string Artist => "Zorutan";
		public override string ArtModifiedBy => null;
		public override string Designer => "andro951";
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<EnchantmentToggle>().AmmoCost;
		}
	}
	
	[Autoload(false)]
	public class AmmoCostEnchantmentBasic : AmmoCostEnchantment
	{
		public override List<DropData> NpcAIDrops => new() {
			new(NPCAIStyleID.Flying, 4f)
		};
		public override List<DropData> ChestDrops => new() {
			new(ChestID.Chest_Normal),
			new(ChestID.WebCovered),
			new(ChestID.Mushroom),
			new(ChestID.Marble),
			new(ChestID.SandStone)
		};
		public override List<DropData> CrateDrops => new() {
			new(CrateID.Wooden, 0.25f),
			new(CrateID.Pearlwood_WoodenHard, 0.25f),
		};
	}
	[Autoload(false)]
	public class AmmoCostEnchantmentCommon : AmmoCostEnchantment { }
	[Autoload(false)]
	public class AmmoCostEnchantmentRare : AmmoCostEnchantment { }
	[Autoload(false)]
	public class AmmoCostEnchantmentEpic : AmmoCostEnchantment { }
	[Autoload(false)]
	public class AmmoCostEnchantmentLegendary : AmmoCostEnchantment { }
	[Autoload(false)]
	public class AmmoCostEnchantmentCursed : AmmoCostEnchantment { }
}
