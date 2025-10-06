using System.Collections.Generic;
using Terraria.ID;
using static androLib.Common.EnchantingRarity;
using WeaponEnchantments.Effects;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;
using androLib.Common.Utility;
using androLib.Common.Globals;
using WeaponEnchantments.Common.Configs;

namespace WeaponEnchantments.Items.Enchantments.Unique
{
	public abstract class ColdSteelEnchantment : Enchantment
	{
		protected override string TypeName => "ColdSteel";
		protected override string NamePrefix => "Enchantments/";
		
		public override int StrengthGroup => 9;
		public override float ScalePercent => 0.2f / defaultEnchantmentStrengths[StrengthGroup].enchantmentTierStrength[tierNames.Length - 1];
		public override List<int> RestrictedClass => new() { (int)DamageClassID.Summon };
		public override void GetMyStats() {
			Effects = new() {
				new DamageAfterDefenses(multiplicative: EnchantmentStrengthData),
				new DamageClassSwap(DamageClass.SummonMeleeSpeed),
				new MinionAttackTarget(),
				new BuffEffect(EnchantmentTier >= 2 ? BuffID.Frostburn2 : BuffID.Frostburn, BuffStyle.OnHitEnemyDebuff, BuffDuration)
			};

			if (EnchantmentTier >= 3) {
				Effects.Add(new BuffEffect(BuffID.CoolWhipPlayerBuff, BuffStyle.OnHitPlayerBuff, BuffDuration));
				Effects.Add(new OnHitSpawnProjectile(ProjectileID.CoolWhipProj, 10));
			}

			if (EnchantmentTier == 4)
				Effects.Add(new BuffEffect(BuffID.RainbowWhipNPCDebuff, BuffStyle.OnHitEnemyDebuff, BuffDuration));

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Weapons, 1f }
			};
		}

		public override string Artist => "Zorutan";
		public override string ArtModifiedBy => null;
		public override string Designer => "andro951";
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<EnchantmentToggle>().ColdSteel;
		}
	}
	[Autoload(false)]
	public class ColdSteelEnchantmentBasic : ColdSteelEnchantment
	{
		public override SellCondition SellCondition => SellCondition.PostSkeletronPrime;
		public override List<DropData> NpcDropTypes => new() {
			new(NPCID.SkeletronPrime)
		};
	}
	[Autoload(false)]
	public class ColdSteelEnchantmentCommon : ColdSteelEnchantment { }
	[Autoload(false)]
	public class ColdSteelEnchantmentRare : ColdSteelEnchantment { }
	[Autoload(false)]
	public class ColdSteelEnchantmentEpic : ColdSteelEnchantment { }
	[Autoload(false)]
	public class ColdSteelEnchantmentLegendary : ColdSteelEnchantment { }
	[Autoload(false)]
	public class ColdSteelEnchantmentCursed : ColdSteelEnchantment { }
}
