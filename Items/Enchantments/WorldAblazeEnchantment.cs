using Terraria.ID;
using System.Collections.Generic;
using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Debuffs;
using WeaponEnchantments.Effects;
using WeaponEnchantments.Common.Utility;
using androLib.Common.Utility;
using WeaponEnchantments.Common.Configs;

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class WorldAblazeEnchantment : Enchantment
	{
		protected override string TypeName => "WorldAblaze";
		protected override string NamePrefix => "Enchantments/";
        
		public override string CustomTooltip => EnchantmentTypeName.Lang_WE(L_ID1.Tooltip, L_ID2.EnchantmentCustomTooltips);
		public override int StrengthGroup => 10;
		public override bool Max1 => true;
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

			if (EnchantmentTier >= 4)
				Effects.Add(new BuffEffect((short)ModContent.BuffType<Amaterasu>(), BuffStyle.OnHitEnemyDebuff, 10000, buffStrength: EnchantmentStrengthData));

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Weapons, 1f }
			};
		}

		public override string Artist => "Zorutan";
		public override string ArtModifiedBy => "andro951";
		public override string Designer => "andro951";
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<EnchantmentToggle>().WorldAblaze;
		}
	}
	[Autoload(false)]
	public class WorldAblazeEnchantmentBasic : WorldAblazeEnchantment
	{
		public override SellCondition SellCondition => SellCondition.PostTwins;
		public override List<DropData> NpcDropTypes => new() {
			new(NPCID.Retinazer),
			new(NPCID.Spazmatism)
		};
	}
	[Autoload(false)]
	public class WorldAblazeEnchantmentCommon : WorldAblazeEnchantment { }
	[Autoload(false)]
	public class WorldAblazeEnchantmentRare : WorldAblazeEnchantment { }
	[Autoload(false)]
	public class WorldAblazeEnchantmentEpic : WorldAblazeEnchantment { }
	[Autoload(false)]
	public class WorldAblazeEnchantmentLegendary : WorldAblazeEnchantment { }
	[Autoload(false)]
	public class WorldAblazeEnchantmentCursed : WorldAblazeEnchantment { }

}
