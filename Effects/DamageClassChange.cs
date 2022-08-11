using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WeaponEnchantments.Effects
{
	public class DamageClassChange : EnchantmentEffect
	{
		public DamageClassChange(DamageClass damageClass) {
			NewDamageClass = damageClass;
		}
		public override string DisplayName => $"Convert damage type to {NewDamageClass.DisplayName}";

		public virtual DamageClass NewDamageClass { get; }
		public DamageClass BaseDamageClass = null;

		public void ApplyTo(ref Item item) {
			item.DamageType = NewDamageClass;
			if (BaseDamageClass == null)
				BaseDamageClass = ContentSamples.ItemsByType[item.type].DamageType;
		}

		public void Reset(ref Item item) {
			item.DamageType = BaseDamageClass;
			BaseDamageClass = null;
		}
	}
}
