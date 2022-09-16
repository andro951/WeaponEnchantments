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

namespace WeaponEnchantments.Effects
{
	public class ArmorPenetration : ClassedStatEffect, IPermenantStat
	{
		public ArmorPenetration(DifficultyStrength additive = null, DifficultyStrength multiplicative = null, DifficultyStrength flat = null, DifficultyStrength @base = null, DamageClass dc = null) : base(additive, multiplicative, flat, @base, dc) {

		}
		public ArmorPenetration(EStatModifier eStatModifier, DamageClass dc) : base(eStatModifier, dc) { }
		public override EnchantmentEffect Clone() {
			return new ArmorPenetration(EStatModifier.Clone(), damageClass);
		}


		public override EnchantmentStat statName => EnchantmentStat.ArmorPenetration;
		public override string Tooltip => $"{EStatModifier.SignTooltip} {DisplayName}";


		public void Update(ref Item item, bool reset = false) {
			if (reset) {
				Reset(ref item);
			}
			else {
				ApplyTo(ref item);
			}
		}
		public void ApplyTo(ref Item item) {
			item.ArmorPenetration += (int)EStatModifier.Strength;
		}
		public void Reset(ref Item item) {
			item.ArmorPenetration -= (int)EStatModifier.Strength;
		}
	}
}
