using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class ExtraFishingLineEnchantment : Enchantment
	{
		public override int StrengthGroup => 18;
		public override float ScalePercent => 0.66667f;
		public override void GetMyStats() {
			Effects = new() {
				new Multishot(@base: EnchantmentStrengthData)
			};

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.FishingPoles, 1f }
			};
		}
		public override string Artist => "andro951";
		public override string ArtModifiedBy => null;
		public override string Designer => "andro951";
	}
	public class ExtraFishingLineEnchantmentBasic : ExtraFishingLineEnchantment
	{
		public override List<DropData> NpcDropTypes => new() {
			new(NPCID.FlyingFish)
		};
		public override List<DropData> CrateDrops => new() {
			new(CrateID.Jungle),
			new(CrateID.Bramble_JungleHard),
			new(CrateID.Sky),
			new(CrateID.Azure_SkyHard)
		};
	}
	public class ExtraFishingLineEnchantmentCommon : ExtraFishingLineEnchantment { }
	public class ExtraFishingLineEnchantmentRare : ExtraFishingLineEnchantment { }
	public class ExtraFishingLineEnchantmentEpic : ExtraFishingLineEnchantment { }
	public class ExtraFishingLineEnchantmentLegendary : ExtraFishingLineEnchantment { }

}
