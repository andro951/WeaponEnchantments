using Terraria.ID;
using System.Collections.Generic;
using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Debuffs;

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class WorldAblazeEnchantment : Enchantment
	{
		public override string CustomTooltip =>
			$"(Amaterasu debuff and below notes about it only apply at Enchantment tier 4.)" +
			$"(None shall survive the unstopable flames of Amaterasu)\n" +
			$"(Inflict a unique fire debuff to enemies that never stops)\n" +
			$"(The damage from the debuff grows over time and from dealing more damage to the target)\n" +
			$"(Spreads to nearby enemies and prevents enemies from being immune from other WorldAblaze debuffs.)";
		public override int StrengthGroup => 10;
		public override bool Max1 => true;
		public override Dictionary<string, float> AllowedList => new Dictionary<string, float>() {
			{ "Weapon", 1f }
		};
		public override void GetMyStats() {
			if (EnchantmentTier == 4) {
				AddEStat("Amaterasu", 0f, 1f, 0f, EnchantmentStrength);
				Debuff.Add(ModContent.BuffType<AmaterasuDebuff>(), -1);
			}
			Debuff.Add(BuffID.OnFire, (int)((float)BuffDuration * EnchantmentStrength));
			Debuff.Add(BuffID.Oiled, (int)((float)BuffDuration * 0.8f * EnchantmentStrength));
			
			float tier0DefaultStrength = defaultEnchantmentStrengths[StrengthGroup].enchantmentTierStrength[0];
			if (EnchantmentStrength > tier0DefaultStrength)
				Debuff.Add(BuffID.CursedInferno, (int)((float)BuffDuration * 0.6f * EnchantmentStrength));

			float tier1DefaultStrength = defaultEnchantmentStrengths[StrengthGroup].enchantmentTierStrength[1];
			if (EnchantmentStrength > tier1DefaultStrength)
				Debuff.Add(BuffID.ShadowFlame, (int)((float)BuffDuration * 0.4f * EnchantmentStrength));

			float tier2DefaultStrength = defaultEnchantmentStrengths[StrengthGroup].enchantmentTierStrength[2];
			if (EnchantmentStrength > tier2DefaultStrength)
				Debuff.Add(BuffID.OnFire3, (int)((float)BuffDuration * 0.2f * EnchantmentStrength));
		}

		public override string Artist => "Zorutan";
		public override string Designer => "andro951";
	}
	public class WorldAblazeEnchantmentBasic : WorldAblazeEnchantment { }
	public class WorldAblazeEnchantmentCommon : WorldAblazeEnchantment { }
	public class WorldAblazeEnchantmentRare : WorldAblazeEnchantment { }
	public class WorldAblazeEnchantmentSuperRare : WorldAblazeEnchantment { }
	public class WorldAblazeEnchantmentUltraRare : WorldAblazeEnchantment { }

}
