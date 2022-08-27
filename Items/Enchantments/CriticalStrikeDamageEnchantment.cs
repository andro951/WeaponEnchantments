using WeaponEnchantments.Common;
using Terraria.ModLoader;
using WeaponEnchantments.Effects;
using Terraria.ID;
using WeaponEnchantments.Common.Utility;
using System.Collections.Generic;

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
		public override string Artist => "𝐍𝐢𝐱𝐲♱";
		public override string Designer => "Kokopai";
	}

	/*[DropRules(
		npcs: new (float, int)[] {
			(1f, NPCID.Medusa),
			(1f, NPCID.GiantFungiBulb)
		}
	)]*/
	public class CriticalStrikeDamageEnchantmentBasic : CriticalStrikeDamageEnchantment {
		public override List<WeightedPair> NpcDropTypes => new List<WeightedPair>() {
			new(NPCID.Medusa, 1f),
			new(NPCID.GiantFungiBulb, 1f)
		};
	}
	public class CriticalStrikeDamageEnchantmentCommon : CriticalStrikeDamageEnchantment { }
	public class CriticalStrikeDamageEnchantmentRare : CriticalStrikeDamageEnchantment { }
	public class CriticalStrikeDamageEnchantmentSuperRare : CriticalStrikeDamageEnchantment { }
	public class CriticalStrikeDamageEnchantmentUltraRare : CriticalStrikeDamageEnchantment { }
}