using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using androLib.Items;
using androLib.Common.Utility;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Configs;

namespace WeaponEnchantments.Items.Enchantments.Unique
{
	public abstract class JuicedEnchantment : Enchantment
	{
		protected override string TypeName => "Juiced";
		protected override string NamePrefix => "Enchantments/";
		
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
		protected override List<List<EnchantmentEffect>> cursedEffectPossibilities => defaultDefensiveCursedEffectPossibilities;

		public override string Artist => "andro951";
		public override string ArtModifiedBy => null;
		public override string Designer => "Mew";
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<EnchantmentToggle>().Juiced;
		}
	}
	[Autoload(false)]
	public class JuicedEnchantmentBasic : JuicedEnchantment
	{
		public override SellCondition SellCondition => SellCondition.PostEaterOfWorldsOrBrainOfCthulhu;
		public override List<DropData> NpcDropTypes => new() {
			new(NPCID.EaterofWorldsHead, chance: 0.2f),
			new(NPCID.BrainofCthulhu, chance: 0.2f)
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
	[Autoload(false)]
	public class JuicedEnchantmentCursed : JuicedEnchantment { }
}
