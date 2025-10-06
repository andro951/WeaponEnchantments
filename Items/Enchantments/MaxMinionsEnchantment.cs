using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using androLib.Items;
using androLib.Common.Utility;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Configs;

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class MaxMinionsEnchantment : Enchantment
	{
		protected override string TypeName => "MaxMinions";
		protected override string NamePrefix => "Enchantments/";
		
		public override int StrengthGroup => 10;
		public override float ScalePercent => 0.6f;
		public override void GetMyStats() {
			Effects = new() {
				new MaxMinions(@base: EnchantmentStrengthData)
			};

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Armor, 1f },
				{ EItemType.Accessories, 1f }
			};
		}

		public override string ShortTooltip => GetShortTooltip(sign: true, percent: false, multiply100: false);
		public override string Artist => "𝐍𝐢𝐱𝐲♱";
		public override string ArtModifiedBy => null;
		public override string Designer => "andro951";
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<EnchantmentToggle>().MaxMinions;
		}
	}
	[Autoload(false)]
	public class MaxMinionsEnchantmentBasic : MaxMinionsEnchantment
	{
		public override SellCondition SellCondition => SellCondition.PostQueenBee;
		public override List<DropData> NpcDropTypes => new() {
			new(NPCID.QueenBee)
		};
		public override List<DropData> NpcAIDrops => new() {
			new(NPCAIStyleID.Spider, 4f)
		};
		public override List<DropData> CrateDrops => new() {
			new(CrateID.Oasis, 0.5f),
			new(CrateID.Mirage_OasisHard, 0.5f),
			new(CrateID.Jungle, 0.5f),
			new(CrateID.Bramble_JungleHard, 0.5f)
		};
	}
	[Autoload(false)]
	public class MaxMinionsEnchantmentCommon : MaxMinionsEnchantment { }
	[Autoload(false)]
	public class MaxMinionsEnchantmentRare : MaxMinionsEnchantment { }
	[Autoload(false)]
	public class MaxMinionsEnchantmentEpic : MaxMinionsEnchantment { }
	[Autoload(false)]
	public class MaxMinionsEnchantmentLegendary : MaxMinionsEnchantment { }
	[Autoload(false)]
	public class MaxMinionsEnchantmentCursed : MaxMinionsEnchantment { }

}