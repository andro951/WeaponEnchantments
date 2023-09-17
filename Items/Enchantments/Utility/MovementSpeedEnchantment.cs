using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using androLib.Items;
using androLib.Common.Utility;
using Terraria.ModLoader;

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
		public override string ArtModifiedBy => null;
		public override string Designer => "andro951";
	}
	[Autoload(false)]
	public class MovementSpeedEnchantmentBasic : MovementSpeedEnchantment
	{
		public override SellCondition SellCondition => SellCondition.PostEyeOfCthulhu;
		public override List<DropData> NpcDropTypes => new() {
			new(NPCID.EyeofCthulhu),
			new(NPCID.GiantWalkingAntlion),
			new(NPCID.WalkingAntlion)
		};
		public override List<DropData> CrateDrops => new() {
			new(CrateID.Oasis),
			new(CrateID.Mirage_OasisHard)
		};
	}
	[Autoload(false)]
	public class MovementSpeedEnchantmentCommon : MovementSpeedEnchantment { }
	[Autoload(false)]
	public class MovementSpeedEnchantmentRare : MovementSpeedEnchantment { }
	[Autoload(false)]
	public class MovementSpeedEnchantmentEpic : MovementSpeedEnchantment { }
	[Autoload(false)]
	public class MovementSpeedEnchantmentLegendary : MovementSpeedEnchantment { }
}