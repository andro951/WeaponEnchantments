using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using androLib.Items;
using androLib.Common.Utility;
using Terraria.ModLoader;

namespace WeaponEnchantments.Items.Enchantments.Unique
{
	public abstract class JuicedEnchantment : Enchantment
	{
		public override int StrengthGroup => 10;
		protected override bool UsesTierStrengthData => true;
		public override float ScalePercent => 0f;
		public override bool OnlyApplyScalePercentBelow100 => true;
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

		public override string Artist => "andro951";
		public override string ArtModifiedBy => null;
		public override string Designer => "Mew";
	}
	[Autoload(false)]
	public class JuicedEnchantmentBasic : JuicedEnchantment
	{
		public override SellCondition SellCondition => SellCondition.PostEaterOfWorldsOrBrainOfCthulhu;
		public override List<DropData> NpcDropTypes => new() {
			new(NPCID.EaterofWorldsHead, 1f/9f),
			new(NPCID.BrainofCthulhu, 1f/9f)
		};
	}
	[Autoload(false)]
	public class JuicedEnchantmentCommon : JuicedEnchantment { }
	[Autoload(false)]
	public class JuicedEnchantmentRare : JuicedEnchantment { }
	[Autoload(false)]
	public class JuicedEnchantmentEpic : JuicedEnchantment { }
	[Autoload(false)]
	public class JuicedEnchantmentLegendary : JuicedEnchantment { }
}
