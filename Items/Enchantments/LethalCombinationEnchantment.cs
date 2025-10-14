using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using androLib.Items;
using androLib.Common.Utility;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Configs;
using static WeaponEnchantments.Common.Configs.ConfigValues;

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class LethalCombinationEnchantment : Enchantment
	{
		protected override string TypeName => "LethalCombination";
		protected override string NamePrefix => "Enchantments/";
		
		public override int StrengthGroup => 26;
		public override float CapacityCostMultiplier => CapacityCostUnique;
		public override List<int> IngredientEnchantments => new() {
			ModContent.ItemType<AttackSpeedEnchantmentBasic>(),
			ModContent.ItemType<CriticalStrikeChanceEnchantmentBasic>(),
			ModContent.ItemType<CriticalStrikeDamageEnchantmentBasic>(),
			ModContent.ItemType<DamageEnchantmentBasic>(),
			ModContent.ItemType<PercentArmorPenetrationEnchantmentBasic>(),
		};
		public override void GetMyStats() {
			Effects = new() {
				new AttackSpeed(EnchantmentStrengthData),
				new MiningSpeed(EnchantmentStrengthData * 3f),
				new NPCHitCooldown(EnchantmentStrengthData * -1),
				new CriticalStrikeChance(@base: EnchantmentStrengthData),
				new CriticalStrikeDamage(additive: EnchantmentStrengthData * 1.25f),
				new DamageAfterDefenses(EnchantmentStrengthData),
				new PercentArmorPenetration(@base: EnchantmentStrengthData * 100f),
			};

			if (AttackSpeedEnchantmentAutoReuseSetpoint > 0f)
				Effects.Add(new AutoReuse());
		}

		public override string ShortTooltip => GetShortTooltip(sign: true);
		public override string Artist => "@level12lobster";
		public override string ArtModifiedBy => null;
		public override string Designer => "@level12lobster";
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<EnchantmentToggle>().LethalCombination;
		}
	}
	[Autoload(false)]
	public class LethalCombinationEnchantmentBasic : LethalCombinationEnchantment
	{
		public override SellCondition SellCondition => SellCondition.Never;
	}
	[Autoload(false)]
	public class LethalCombinationEnchantmentCommon : LethalCombinationEnchantment { }
	[Autoload(false)]
	public class LethalCombinationEnchantmentRare : LethalCombinationEnchantment { }
	[Autoload(false)]
	public class LethalCombinationEnchantmentEpic : LethalCombinationEnchantment { }
	[Autoload(false)]
	public class LethalCombinationEnchantmentLegendary : LethalCombinationEnchantment { }
	[Autoload(false)]
	public class LethalCombinationEnchantmentCursed : LethalCombinationEnchantment { }
}
