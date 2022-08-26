using WeaponEnchantments.Common;
using Terraria.ModLoader;
using WeaponEnchantments.Effects;
using Terraria.ID;

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

	[DropRules(
		npcs: new int[] {
			NPCID.Medusa,
			NPCID.GiantFungiBulb
		}
	)]
	public class CriticalStrikeDamageEnchantmentBasic : CriticalStrikeDamageEnchantment { }
	public class CriticalStrikeDamageEnchantmentCommon : CriticalStrikeDamageEnchantment { }
	public class CriticalStrikeDamageEnchantmentRare : CriticalStrikeDamageEnchantment { }
	public class CriticalStrikeDamageEnchantmentSuperRare : CriticalStrikeDamageEnchantment { }
	public class CriticalStrikeDamageEnchantmentUltraRare : CriticalStrikeDamageEnchantment { }
}