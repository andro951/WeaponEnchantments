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
using WeaponEnchantments.Items;
using androLib.Common.Utility;

namespace WeaponEnchantments.Effects
{
	public class DamageClassSwap : EnchantmentEffect, IPermenantStat
	{
		public static DamageClassSwap Default => new DamageClassSwap(DamageClass.Generic);
		public DamageClassSwap(DamageClass damageClass, DamageClass baseDamageClass = null, DamageClassID damageClassNameOveride = DamageClassID.Default) {
			NewDamageClass = damageClass;
			BaseDamageClass = baseDamageClass;
			DamageClassNameOveride = damageClassNameOveride;
		}
		public override EnchantmentEffect Clone() {
			return new DamageClassSwap(NewDamageClass, BaseDamageClass, DamageClassNameOveride);
		}

		public override IEnumerable<object> DisplayNameArgs => new string[] { TooltipValue };
		public override string TooltipValue => $"{damageClassName} {"Damage".Lang_WE(L_ID1.Tooltip, L_ID2.EffectDisplayName)}";
		private string damageClassName => (DamageClassNameOveride != DamageClassID.Default ? DamageClassNameOveride.ToString() : Enchantment.GetDamageClassName(Enchantment.GetDamageClass(NewDamageClass.Type))).Lang_WE(L_ID1.Tooltip, L_ID2.DamageClassNames);
		public override IEnumerable<object> TooltipArgs => null;

		public virtual DamageClass NewDamageClass { get; }
		public DamageClass BaseDamageClass;
		public DamageClassID DamageClassNameOveride;

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
