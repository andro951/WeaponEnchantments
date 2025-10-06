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
	public abstract class JunglesFuryEnchantment : Enchantment
	{
		protected override string TypeName => "JunglesFury";
		protected override string NamePrefix => "Enchantments/";
		
		public override int StrengthGroup => 9;
		public override float ScalePercent => 0.2f / defaultEnchantmentStrengths[StrengthGroup].enchantmentTierStrength[tierNames.Length - 1];
		public override List<int> RestrictedClass => new() { (int)DamageClassID.Summon };
		public override void GetMyStats() {
			Effects = new() {
				new DamageAfterDefenses(multiplicative: EnchantmentStrengthData),
				new DamageClassSwap(DamageClass.SummonMeleeSpeed),
				new MinionAttackTarget(),
				new BuffEffect(EnchantmentTier >= 2 ? BuffID.Venom : BuffID.Poisoned, BuffStyle.OnHitEnemyDebuff, BuffDuration)
			};

			if (EnchantmentTier >= 3) {
				Effects.Add(new BuffEffect(BuffID.SwordWhipPlayerBuff, BuffStyle.OnHitPlayerBuff, BuffDuration));
				Effects.Add(new BuffEffect(BuffID.SwordWhipNPCDebuff, BuffStyle.OnHitEnemyDebuff, BuffDuration));
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
			return ModContent.GetInstance<EnchantmentToggle>().JunglesFury;
		}
	}
	[Autoload(false)]
	public class JunglesFuryEnchantmentBasic : JunglesFuryEnchantment
	{
		public override SellCondition SellCondition => SellCondition.PostPlantera;
		public override List<DropData> NpcDropTypes => new() {
			new(NPCID.Plantera)
		};
	}
	[Autoload(false)]
	public class JunglesFuryEnchantmentCommon : JunglesFuryEnchantment { }
	[Autoload(false)]
	public class JunglesFuryEnchantmentRare : JunglesFuryEnchantment { }
	[Autoload(false)]
	public class JunglesFuryEnchantmentEpic : JunglesFuryEnchantment { }
	[Autoload(false)]
	public class JunglesFuryEnchantmentLegendary : JunglesFuryEnchantment { }
	[Autoload(false)]
	public class JunglesFuryEnchantmentCursed : JunglesFuryEnchantment { }
}
