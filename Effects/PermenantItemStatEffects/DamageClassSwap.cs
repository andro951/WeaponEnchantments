using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Common.Utility;

namespace WeaponEnchantments.Effects
{
	public class DamageClassSwap : EnchantmentEffect, IPermenantStat
	{
		public static DamageClassSwap Default => new DamageClassSwap(DamageClass.Generic);
		public DamageClassSwap(DamageClass damageClass, DamageClass baseDamageClass = null) {
			NewDamageClass = damageClass;
			BaseDamageClass = baseDamageClass;
		}
		public override EnchantmentEffect Clone() {
			return new DamageClassSwap(NewDamageClass, BaseDamageClass);
		}
		//public override string DisplayName => $"Convert damage type to {NewDamageClass.DisplayName}";
		//public override string Tooltip => DisplayName;
		public override IEnumerable<object> DisplayNameArgs => new string[] { NewDamageClass.DisplayName };
		public override IEnumerable<object> TooltipArgs => null;

		public virtual DamageClass NewDamageClass { get; }
		public DamageClass BaseDamageClass;

		public void Update(ref Item item, bool reset = false) {
			if (reset) {
				Reset(ref item);
			}
			else {
				ApplyTo(ref item);
			}
		}
		public void ApplyTo(ref Item item) {
			if (item.TryGetEnchantedWeapon(out EnchantedWeapon enchantedWeapon)) {
				item.DamageType = NewDamageClass;
				if (BaseDamageClass == null)
					BaseDamageClass = ContentSamples.ItemsByType[item.type].DamageType;

				enchantedWeapon.damageType = NewDamageClass;
				if (item.TryGetEnchantedItem(out EnchantedWeapon weapon))
					weapon.damageType = NewDamageClass;

				enchantedWeapon.baseDamageType = BaseDamageClass;
			}
		}
		public void Reset(ref Item item) {
			item.DamageType = BaseDamageClass;
			if (item.TryGetEnchantedItem(out EnchantedWeapon weapon))
				weapon.damageType = BaseDamageClass;

			BaseDamageClass = null;
		}
	}
}
