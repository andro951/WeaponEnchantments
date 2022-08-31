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
	public class RogueWeapon: EnchantmentEffect, IPermenantStat
	{
		public static DamageClassChange Default => new DamageClassChange(DamageClass.Generic);
		public RogueWeapon() {}
		public override string DisplayName => $"Convert damage type to Throwing. If calamity is installed, it is converted to Rogue.";

		public DamageClass BaseDamageClass = null;

		public void Update(ref Item item, bool reset = false) {
			if (reset) {
				Reset(ref item);
			}
			else {
				ApplyTo(ref item);
			}
		}
		public void ApplyTo(ref Item item) {
			// We check for the Calamity mod first and foremost. 
			// If the mod is not found, the weapon is assigned the Throwing class instead of Rogue.
			if (ModLoader.HasMod("CalamityMod"))
			{
				Mod calamity = ModLoader.GetMod("CalamityMod");

				// Pull "CalamityMod.RogueDamageClass" from the mod.
				DamageClass rogueClass = calamity.Find<DamageClass>("RogueDamageClass");

				if (rogueClass != null && item.TryGetEnchantedWeapon(out EnchantedWeapon enchantedWeapon))
				{
					item.DamageType = rogueClass;
					if (BaseDamageClass == null)
						BaseDamageClass = ContentSamples.ItemsByType[item.type].DamageType;

					enchantedWeapon.damageType = rogueClass;
					enchantedWeapon.baseDamageType = BaseDamageClass;
				}
                else
                {
					Reset(ref item);
				}
			}
            else
            {
				if (item.TryGetEnchantedWeapon(out EnchantedWeapon enchantedWeapon))
				{
					item.DamageType = DamageClass.Throwing;
					if (BaseDamageClass == null)
						BaseDamageClass = ContentSamples.ItemsByType[item.type].DamageType;

					enchantedWeapon.damageType = DamageClass.Throwing;
					enchantedWeapon.baseDamageType = BaseDamageClass;
				}
			}
			
		}
		public void Reset(ref Item item) {
			// We must initialiez BaseDamageClass here too, otherwise we run into softlocks.
			if (BaseDamageClass == null)
				BaseDamageClass = ContentSamples.ItemsByType[item.type].DamageType;

			item.DamageType = BaseDamageClass;
			BaseDamageClass = null;
				
		}
	}
}
