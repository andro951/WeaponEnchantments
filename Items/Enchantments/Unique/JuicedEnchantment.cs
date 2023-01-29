using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments.Unique
{
	public abstract class JuicedEnchantment : Enchantment
	{
		public override int StrengthGroup => 10;
		protected override bool UsesTierStrengthData => true;
		public override int ArmorSlotSpecific => (int)ArmorSlotSpecificID.Head;
		public override void GetMyStats() {
			Effects = new() {
				new BuffDuration(EnchantmentStrengthData),
				new LifeRegeneration(@base: EnchantmentStrengthData * 5f)
			};

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Armor, 1f }
			};
		}

		//public override string ShortTooltip => GetShortTooltip();
		public override string Artist => "andro951";
		public override string ArtModifiedBy => null;
		public override string Designer => "Mew";
	}
	public class JuicedEnchantmentBasic : JuicedEnchantment
	{
		public override SellCondition SellCondition => SellCondition.PostEaterOfWorldsOrBrainOfCthulhu;
		public override List<DropData> NpcDropTypes => new() {
			new(NPCID.EaterofWorldsHead, 1f/9f),
			new(NPCID.BrainofCthulhu, 1f/9f)
		};
	}
	public class JuicedEnchantmentCommon : JuicedEnchantment { }
	public class JuicedEnchantmentRare : JuicedEnchantment { }
	public class JuicedEnchantmentEpic : JuicedEnchantment { }
	public class JuicedEnchantmentLegendary : JuicedEnchantment { }
}
