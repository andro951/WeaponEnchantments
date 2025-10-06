using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using androLib.Items;
using androLib.Common.Utility;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Configs;

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class OneForAllEnchantment : Enchantment
	{
		protected override string TypeName => "OneForAll";
		protected override string NamePrefix => "Enchantments/";
		
		public override int StrengthGroup => 10;
		public override float ScalePercent => 0.8f;
		public override bool Max1 => true;
		public override void GetMyStats() {
			Effects = new() {
				new OneForAll(@base: EnchantmentStrengthData),
				new AttackSpeed(multiplicative: (EnchantmentStrengthData * -0.05f + 1.125f).Invert()),
				new NPCHitCooldown(multiplicative: EnchantmentStrengthData * -0.05f + 1.125f)
			};

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Weapons, 1f }
			};
		}

		public override string Artist => "Zorutan";
		public override string ArtModifiedBy => null;
		public override string Designer => "andro951";
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<EnchantmentToggle>().OneForAll;
		}
	}
	[Autoload(false)]
	public class OneForAllEnchantmentBasic : OneForAllEnchantment
	{
		public override SellCondition SellCondition => SellCondition.PostSkeletron;
		public override List<DropData> NpcDropTypes => new() {
			new(NPCID.Mothron, chance: 0.1f)
		};
		public override List<DropData> ChestDrops => new() {
			new(ChestID.Gold_Locked),
			new(ChestID.Lihzahrd, 1f)
		};
		public override List<DropData> CrateDrops => new() {
			new(CrateID.Golden_LockBox, 0.5f)
		};
	}
	[Autoload(false)]
	public class OneForAllEnchantmentCommon : OneForAllEnchantment { }
	[Autoload(false)]
	public class OneForAllEnchantmentRare : OneForAllEnchantment { }
	[Autoload(false)]
	public class OneForAllEnchantmentEpic : OneForAllEnchantment { }
	[Autoload(false)]
	public class OneForAllEnchantmentLegendary : OneForAllEnchantment { }
	[Autoload(false)]
	public class OneForAllEnchantmentCursed : OneForAllEnchantment { }

}
