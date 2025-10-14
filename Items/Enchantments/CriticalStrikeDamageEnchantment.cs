using WeaponEnchantments.Common;
using Terraria.ModLoader;
using WeaponEnchantments.Effects;
using Terraria.ID;
using WeaponEnchantments.Common.Utility;
using System.Collections.Generic;
using androLib.Items;
using androLib.Common.Utility;
using WeaponEnchantments.Common.Configs;

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class CriticalStrikeDamageEnchantment : Enchantment
	{
		protected override string TypeName => "CriticalStrikeDamage";
		protected override string NamePrefix => "Enchantments/";
		
		public override int StrengthGroup => 14;
		public override float CapacityCostMultiplier => CapacityCostUnique;
		public override void GetMyStats() {
			Effects = new() {
				new CriticalStrikeDamage(additive: EnchantmentStrengthData),
			};
		}

		public override string ShortTooltip => GetShortTooltip(sign: true);
		public override string Artist => "Zorutan";
		public override string ArtModifiedBy => "𝐍𝐢𝐱𝐲♱";
		public override string Designer => "Kokopai";
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<EnchantmentToggle>().CriticalStrikeDamage;
		}
	}

	[Autoload(false)]
	public class CriticalStrikeDamageEnchantmentBasic : CriticalStrikeDamageEnchantment
	{
		public override SellCondition SellCondition => SellCondition.HardMode;
		public override List<DropData> NpcDropTypes => new() {
			new(NPCID.Medusa, chance: 0.2f),
			new(NPCID.GiantFungiBulb, chance: 0.1f),
			new(NPCID.GraniteGolem, chance: 0.15f)
		};
		public override List<DropData> CrateDrops => new() {
			new(CrateID.Wooden, 0.25f),
			new(CrateID.Pearlwood_WoodenHard, 0.25f),
			new(CrateID.Stockade_DungeonHard, 0.5f),
			new(CrateID.Mirage_OasisHard, 0.5f)
		};
	}
	[Autoload(false)]
	public class CriticalStrikeDamageEnchantmentCommon : CriticalStrikeDamageEnchantment { }
	[Autoload(false)]
	public class CriticalStrikeDamageEnchantmentRare : CriticalStrikeDamageEnchantment { }
	[Autoload(false)]
	public class CriticalStrikeDamageEnchantmentEpic : CriticalStrikeDamageEnchantment { }
	[Autoload(false)]
	public class CriticalStrikeDamageEnchantmentLegendary : CriticalStrikeDamageEnchantment { }
	[Autoload(false)]
	public class CriticalStrikeDamageEnchantmentCursed : CriticalStrikeDamageEnchantment { }
}