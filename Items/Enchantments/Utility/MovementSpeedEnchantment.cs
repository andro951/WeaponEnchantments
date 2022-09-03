using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments.Utility
{
	public abstract class MovementSpeedEnchantment : Enchantment
	{
		public override int StrengthGroup => 11;
		public override void GetMyStats() {
			Effects = new() {
				new MovementSpeed(EnchantmentStrengthData)
			};

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Weapons, 1f },
				{ EItemType.Armor, 1f },
				{ EItemType.Accessories, 1f },
				{ EItemType.FishingPoles, 1f },
				{ EItemType.Tools, 1f }
			};
		}

		public override string ShortTooltip => GetShortTooltip(sign: true);
		public override string Artist => "Zorutan";
		public override string Designer => "andro951";
	}
	public class MovementSpeedEnchantmentBasic : MovementSpeedEnchantment
	{
		public override List<WeightedPair> NpcDropTypes => new() {
			new(NPCID.EyeofCthulhu),
			new(NPCID.GiantWalkingAntlion),
			new(NPCID.WalkingAntlion)
		};
		public override List<WeightedPair> CrateDrops => new() {
			new(CrateID.Oasis),
			new(CrateID.Mirage_OasisHard)
		};
	}
	public class MovementSpeedEnchantmentCommon : MovementSpeedEnchantment { }
	public class MovementSpeedEnchantmentRare : MovementSpeedEnchantment { }
	public class MovementSpeedEnchantmentEpic : MovementSpeedEnchantment { }
	public class MovementSpeedEnchantmentLegendary : MovementSpeedEnchantment { }
}