using WeaponEnchantments.Common;
using Terraria.ModLoader;
using WeaponEnchantments.Effects;
using Terraria.ID;
using WeaponEnchantments.Common.Utility;
using System.Collections.Generic;
using androLib.Items;
using androLib.Common.Utility;

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class CriticalStrikeDamageEnchantment : Enchantment
	{
		public override int StrengthGroup => 14;
		public override float CapacityCostMultiplier => 3;
		public override void GetMyStats() {
			Effects = new() {
				new CriticalStrikeDamage(additive: EnchantmentStrengthData),
			};
		}

		public override string ShortTooltip => GetShortTooltip(sign: true);
		public override string Artist => "Zorutan";
		public override string ArtModifiedBy => "𝐍𝐢𝐱𝐲♱";
		public override string Designer => "Kokopai";
	}

	public class CriticalStrikeDamageEnchantmentBasic : CriticalStrikeDamageEnchantment
	{
		public override SellCondition SellCondition => SellCondition.HardMode;
		public override List<DropData> NpcDropTypes => new() {
			new(NPCID.Medusa, 1f),
			new(NPCID.GiantFungiBulb, 1f)
		};
		public override List<DropData> CrateDrops => new() {
			new(CrateID.Stockade_DungeonHard, 0.5f),
			new(CrateID.Mirage_OasisHard, 0.5f)
		};
	}
	public class CriticalStrikeDamageEnchantmentCommon : CriticalStrikeDamageEnchantment { }
	public class CriticalStrikeDamageEnchantmentRare : CriticalStrikeDamageEnchantment { }
	public class CriticalStrikeDamageEnchantmentEpic : CriticalStrikeDamageEnchantment { }
	public class CriticalStrikeDamageEnchantmentLegendary : CriticalStrikeDamageEnchantment { }
}