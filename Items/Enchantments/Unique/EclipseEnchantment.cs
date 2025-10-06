using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using static androLib.Common.EnchantingRarity;

namespace WeaponEnchantments.Items.Enchantments.Unique
{
	/*
	public abstract class EclipseEnchantment : Enchantment
	{
		public override int StrengthGroup => 9;
		public override float ScalePercent => 0.2f / defaultEnchantmentStrengths[StrengthGroup].enchantmentTierStrength[tierNames.Length - 1];
		public override int RestrictedClass => (int)DamageTypeID.Magic;
		public override void GetMyStats() {
			Effects = new() {
				new DamageAfterDefenses(multiplicative: EnchantmentStrengthData),
				new DamageClassSwap(DamageClass.Magic)
			};

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Weapons, 1f }
			};
		}

		public override string Artist => "Vyklade";
		public override string Designer => "Vyklade";
	}
	
	public class EclipseEnchantmentBasic : EclipseEnchantment
	{
		public override List<DropData> NpcDropTypes => new() {
			new(NPCID.Mothron)
		};
	}
	public class EclipseEnchantmentCommon : EclipseEnchantment { }
	public class EclipseEnchantmentRare : EclipseEnchantment { }
	public class EclipseEnchantmentEpic : EclipseEnchantment { }
	public class EclipseEnchantmentLegendary : EclipseEnchantment { }
	public class EclipseEnchantmentCursed : EclipseEnchantment { }
	*/
}
