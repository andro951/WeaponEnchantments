using Terraria.ID;
using System.Collections.Generic;
using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Debuffs;
using WeaponEnchantments.Effects;
using WeaponEnchantments.Common.Utility;

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class WorldAblazeEnchantment : Enchantment
	{
		public override string CustomTooltip => EnchantmentTypeName.Lang(L_ID1.Tooltip, L_ID2.EnchantmentCustomTooltips);
		public override int StrengthGroup => 10;
		public override bool Max1 => true;
		public override SellCondition SellCondition => SellCondition.PostTwins;
		public override void GetMyStats() {
			Effects = new() {
				new BuffEffect(BuffID.OnFire, BuffStyle.OnHitEnemyDebuff, BuffDuration, EnchantmentStrengthData),
				new BuffEffect(BuffID.Oiled, BuffStyle.OnHitEnemyDebuff, BuffDuration, EnchantmentStrengthData * 0.8f)
			};

			if (EnchantmentTier >= 1)
				Effects.Add(new BuffEffect(BuffID.CursedInferno, BuffStyle.OnHitEnemyDebuff, BuffDuration, EnchantmentStrengthData * 0.6f));

			if (EnchantmentTier >= 2)
				Effects.Add(new BuffEffect(BuffID.ShadowFlame, BuffStyle.OnHitEnemyDebuff, BuffDuration, EnchantmentStrengthData * 0.4f));

			if (EnchantmentTier >= 3)
				Effects.Add(new BuffEffect(BuffID.OnFire3, BuffStyle.OnHitEnemyDebuff, BuffDuration, EnchantmentStrengthData * 0.2f));

			if (EnchantmentTier == 4)
				Effects.Add(new BuffEffect((short)ModContent.BuffType<AmaterasuDebuff>(), BuffStyle.OnHitEnemyDebuff, 10000, buffStrength: EnchantmentStrengthData));

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Weapons, 1f }
			};
		}

		public override string Artist => "Zorutan";
		public override string Designer => "andro951";
	}
	public class WorldAblazeEnchantmentBasic : WorldAblazeEnchantment
	{
		public override List<WeightedPair> NpcDropTypes => new() {
			new(NPCID.Retinazer),
			new(NPCID.Spazmatism)
		};
	}
	public class WorldAblazeEnchantmentCommon : WorldAblazeEnchantment { }
	public class WorldAblazeEnchantmentRare : WorldAblazeEnchantment { }
	public class WorldAblazeEnchantmentEpic : WorldAblazeEnchantment { }
	public class WorldAblazeEnchantmentLegendary : WorldAblazeEnchantment { }

}
