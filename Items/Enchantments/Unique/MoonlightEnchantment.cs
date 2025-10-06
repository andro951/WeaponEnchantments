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
	public abstract class MoonlightEnchantment : Enchantment
	{
		protected override string TypeName => "Moonlight";
		protected override string NamePrefix => "Enchantments/";
		
		public override int StrengthGroup => 9;
		public override float ScalePercent => 0.2f / defaultEnchantmentStrengths[StrengthGroup].enchantmentTierStrength[tierNames.Length - 1];
		public override List<int> RestrictedClass => new() { (int)DamageClassID.Summon };
		public override void GetMyStats() {
			Effects = new() {
				new DamageAfterDefenses(multiplicative: EnchantmentStrengthData),
				new DamageClassSwap(DamageClass.SummonMeleeSpeed),
				new MinionAttackTarget()
			};

			if (EnchantmentTier >= 2)
				Effects.Add(new BuffEffect(BuffID.ScytheWhipPlayerBuff, BuffStyle.OnHitPlayerBuff, BuffDuration));

			if (EnchantmentTier >= 3)
				Effects.Add(new BuffEffect(BuffID.ScytheWhipEnemyDebuff, BuffStyle.OnHitEnemyDebuff, BuffDuration));

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
			return ModContent.GetInstance<EnchantmentToggle>().Moonlight;
		}
	}
	[Autoload(false)]
	public class MoonlightEnchantmentBasic : MoonlightEnchantment
	{
		public override SellCondition SellCondition => SellCondition.PostCultist;
		public override List<DropData> NpcDropTypes => new() {
			new(NPCID.CultistBoss)
		};
	}
	[Autoload(false)]
	public class MoonlightEnchantmentCommon : MoonlightEnchantment { }
	[Autoload(false)]
	public class MoonlightEnchantmentRare : MoonlightEnchantment { }
	[Autoload(false)]
	public class MoonlightEnchantmentEpic : MoonlightEnchantment { }
	[Autoload(false)]
	public class MoonlightEnchantmentLegendary : MoonlightEnchantment { }
	[Autoload(false)]
	public class MoonlightEnchantmentCursed : MoonlightEnchantment { }
}
