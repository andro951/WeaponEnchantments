using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using static androLib.Common.EnchantingRarity;
using androLib.Common.Utility;
using androLib.Common.Globals;
using WeaponEnchantments.Common.Configs;

namespace WeaponEnchantments.Items.Enchantments.Unique
{
	public abstract class HellsWrathEnchantment : Enchantment
	{
		protected override string TypeName => "HellsWrath";
		protected override string NamePrefix => "Enchantments/";
		
		public override int StrengthGroup => 9;
		public override float ScalePercent => 0.2f / defaultEnchantmentStrengths[StrengthGroup].enchantmentTierStrength[tierNames.Length - 1];
		public override List<int> RestrictedClass => new() { (int)DamageClassID.Summon };
		public override void GetMyStats() {
			Effects = new() {
				new DamageAfterDefenses(multiplicative: EnchantmentStrengthData),
				new DamageClassSwap(DamageClass.SummonMeleeSpeed),
				new MinionAttackTarget(),
				new BuffEffect(EnchantmentTier >= 2 ? BuffID.OnFire3 : BuffID.OnFire, BuffStyle.OnHitEnemyDebuff, BuffDuration)
			};

			if (EnchantmentTier >= 3)
				Effects.Add(new BuffEffect(BuffID.FlameWhipEnemyDebuff, BuffStyle.OnHitEnemyDebuff, BuffDuration));

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
			return ModContent.GetInstance<EnchantmentToggle>().HellsWrath;
		}
	}
	[Autoload(false)]
	public class HellsWrathEnchantmentBasic : HellsWrathEnchantment
	{
		public override SellCondition SellCondition => SellCondition.PostQueenSlime;
		public override List<DropData> NpcDropTypes => new() {
			new(NPCID.QueenSlimeBoss)
		};
	}
	[Autoload(false)]
	public class HellsWrathEnchantmentCommon : HellsWrathEnchantment { }
	[Autoload(false)]
	public class HellsWrathEnchantmentRare : HellsWrathEnchantment { }
	[Autoload(false)]
	public class HellsWrathEnchantmentEpic : HellsWrathEnchantment { }
	[Autoload(false)]
	public class HellsWrathEnchantmentLegendary : HellsWrathEnchantment { }
	[Autoload(false)]
	public class HellsWrathEnchantmentCursed : HellsWrathEnchantment { }
}
