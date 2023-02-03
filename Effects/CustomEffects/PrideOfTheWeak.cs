using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Common.Utility;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects
{
	public class PrideOfTheWeak : ClassedStatEffect, INonVanillaStat, IAddDynamicEffects
    {
        public PrideOfTheWeak(DifficultyStrength additive = null, DifficultyStrength multiplicative = null, DifficultyStrength flat = null, DifficultyStrength @base = null, DamageClass dc = null) : base(additive, multiplicative, flat, @base, dc) {

		}
		public PrideOfTheWeak(EStatModifier eStatModifier, DamageClass dc, EnchantedItem enchantedItem) : base(eStatModifier, dc) {
			EnchantedItem = enchantedItem;
		}
		public override EnchantmentEffect Clone() {
			return new PrideOfTheWeak(EStatModifier.Clone(), damageClass, EnchantedItem);
		}
		public override float EffectStrength => EnchantedItem == null ? EStatModifier.Strength : 1f + (EStatModifier.Strength - 1f) * EnchantedItem.GetPrideOfTheWeakMultiplier();
		public override IEnumerable<object> TooltipArgs => new object[] { base.Tooltip };
		public override string TooltipValue => EStatModifier.GetTootlip(true, false, false, multiplier: EnchantedItem?.GetPrideOfTheWeakMultiplier());
		public override string Tooltip => StandardTooltip;
		public EnchantedItem EnchantedItem { get; set; } = null;
		public void AddDynamicEffects(List<EnchantmentEffect> effects, EnchantedItem enchantedItem) {
			effects.Add(new DamageAfterDefenses(multiplicative: new DifficultyStrength(1f + (EStatModifier.Strength - 1f) * enchantedItem.GetPrideOfTheWeakMultiplier())));
		}
		public override EnchantmentStat statName => EnchantmentStat.PrideOfTheWeak;
	}
}
