using androLib.Common.Utility;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Configs;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class CriticalStrikeChanceEnchantment : Enchantment
	{
		protected override string TypeName => "CriticalStrikeChance";
		protected override string NamePrefix => "Enchantments/";
		
		public override void GetMyStats() {
			Effects = new() {
				new CriticalStrikeChance(@base: EnchantmentStrengthData),
			};
		}

		public override string ShortTooltip => GetShortTooltip(sign: true);
		public override string Artist => "Zorutan";
		public override string ArtModifiedBy => null;
		public override string Designer => "andro951";
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<EnchantmentToggle>().CriticalStrikeChance;
		}
	}
	[Autoload(false)]
	public class CriticalStrikeChanceEnchantmentBasic : CriticalStrikeChanceEnchantment
	{
		public override List<DropData> NpcDropTypes => new() {
			new(NPCID.SkeletronHead)
		};
		public override List<DropData> NpcAIDrops => new() {
			new(NPCAIStyleID.ManEater, 4f),
			new(NPCAIStyleID.Jellyfish, 4f),
			new(NPCAIStyleID.Antlion, 10f)
		};
		public override List<DropData> ChestDrops => new() {
			new(ChestID.Chest_Normal),
			new(ChestID.Gold),
			new(ChestID.Gold_DeadMans),
			new(ChestID.RichMahogany)
		};
		public override List<DropData> CrateDrops => new() {
			new(CrateID.Wooden, 0.25f),
			new(CrateID.Pearlwood_WoodenHard, 0.25f),
			new(CrateID.Jungle, 0.5f),
			new(CrateID.Jungle, 0.5f)
		};
	}
	[Autoload(false)]
	public class CriticalStrikeChanceEnchantmentCommon : CriticalStrikeChanceEnchantment { }
	[Autoload(false)]
	public class CriticalStrikeChanceEnchantmentRare : CriticalStrikeChanceEnchantment { }
	[Autoload(false)]
	public class CriticalStrikeChanceEnchantmentEpic : CriticalStrikeChanceEnchantment { }
	[Autoload(false)]
	public class CriticalStrikeChanceEnchantmentLegendary : CriticalStrikeChanceEnchantment { }
	[Autoload(false)]
	public class CriticalStrikeChanceEnchantmentCursed : CriticalStrikeChanceEnchantment { }
}