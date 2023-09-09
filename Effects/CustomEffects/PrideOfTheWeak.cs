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
		public PrideOfTheWeak(EStatModifier eStatModifier, DamageClass dc, EnchantedWeapon enchantedWeapon) : base(eStatModifier, dc) {
			EnchantedWeapon = enchantedWeapon;
		}
		public override EnchantmentEffect Clone() {
			return new PrideOfTheWeak(EStatModifier.Clone(), damageClass, EnchantedWeapon);
		}
		public override float EffectStrength => EnchantedWeapon == null ? EStatModifier.Strength : 1f + (EStatModifier.Strength - 1f) * EnchantedWeapon.GetPrideOfTheWeakMultiplier();
		public override IEnumerable<object> TooltipArgs => new object[] { base.Tooltip };
		public override string TooltipValue => EStatModifier.GetTootlip(true, false, false, multiplier: EnchantedWeapon?.GetPrideOfTheWeakMultiplier() ?? 1f);
		public override string Tooltip => StandardTooltip;
		public EnchantedWeapon EnchantedWeapon { get; set; } = null;
		public EnchantedItem EnchantedItem {
			get => EnchantedWeapon;
			set {
				if (value is EnchantedWeapon enchantedWeapon) {
					EnchantedWeapon = enchantedWeapon;
				}
				else if (value is null) {
					EnchantedWeapon = null;
				}
			}
		}
		public void AddDynamicEffects(List<EnchantmentEffect> effects, EnchantedItem enchantedItem) {
			if (enchantedItem is EnchantedWeapon enchantedWeapon)
				effects.Add(new DamageAfterDefenses(multiplicative: new DifficultyStrength(1f + (EStatModifier.Strength - 1f) * enchantedWeapon.GetPrideOfTheWeakMultiplier())));
		}
		public override EnchantmentStat statName => EnchantmentStat.PrideOfTheWeak;
	}
}
