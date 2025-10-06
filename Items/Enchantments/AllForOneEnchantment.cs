using System;
using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using androLib.Common.Utility;
using androLib.Common.Globals;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Configs;

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class AllForOneEnchantment : Enchantment
	{
		protected override string TypeName => "AllForOne";
		protected override string NamePrefix => "Enchantments/";
		
		public override int StrengthGroup => 6;
		public override float ScalePercent => 0.8f;
		public override bool Max1 => true;
		public override List<int> RestrictedClass => new() { (int)DamageClassID.Summon };
		public override void GetMyStats() {
			Effects = new() {
				new ItemCooldown(EnchantmentStrengthData * 0.4f + 4f),
				new DamageAfterDefenses(multiplicative: EnchantmentStrengthData),
				new NPCHitCooldown(multiplicative: EnchantmentStrengthData * 0.4f + 4f),
				new AttackSpeed(multiplicative: (EnchantmentStrengthData * 0.1f + 1f).Invert()),
				new ManaUsage(@base: EnchantmentStrengthData * 0.15f + 1.5f),
				new AutoReuse(prevent: true),
				new ProjectileVelocity(EnchantmentStrengthData * 0.1f)
			};

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Weapons, 1f }
			};
		}

		public override string ShortTooltip => GetShortTooltip(percent: false, multiply100: false, multiplicative: true);
		public override string Artist => "Zorutan";
		public override string ArtModifiedBy => null;
		public override string Designer => "andro951";
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<EnchantmentToggle>().AllForOne;
		}
	}
	[Autoload(false)]
	public class AllForOneEnchantmentBasic : AllForOneEnchantment
	{
		public override SellCondition SellCondition => SellCondition.PostSkeletron;
		public override List<DropData> NpcDropTypes => new() {
			new(NPCID.Mothron, chance: 0.1f)
		};
		public override List<DropData> ChestDrops => new() {
			new(ChestID.Gold_Locked),
			new(ChestID.Lihzahrd)
		};
		public override List<DropData> CrateDrops => new() {
			new(CrateID.Golden_LockBox, 0.5f)
		};
	}
	[Autoload(false)]
	public class AllForOneEnchantmentCommon : AllForOneEnchantment { }
	[Autoload(false)]
	public class AllForOneEnchantmentRare : AllForOneEnchantment { }
	[Autoload(false)]
	public class AllForOneEnchantmentEpic : AllForOneEnchantment { }
	[Autoload(false)]
	public class AllForOneEnchantmentLegendary : AllForOneEnchantment { }
	[Autoload(false)]
	public class AllForOneEnchantmentCursed : AllForOneEnchantment { }
}
