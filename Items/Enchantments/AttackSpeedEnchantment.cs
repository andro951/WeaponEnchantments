using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using static WeaponEnchantments.Common.Configs.ConfigValues;
using androLib.Items;
using androLib.Common.Utility;

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class AttackSpeedEnchantment : Enchantment
	{
		public override void GetMyStats() {
			Effects = new() {
				new AttackSpeed(EnchantmentStrengthData),
				new MiningSpeed(EnchantmentStrengthData * 1.5f),
				new NPCHitCooldown(EnchantmentStrengthData * -1)
			};

			if (EnchantmentStrength >= AttackSpeedEnchantmentAutoReuseSetpoint)
				Effects.Add(new AutoReuse());

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Weapons, 1f },
				{ EItemType.Armor, 0.25f },
				{ EItemType.Accessories, 0.25f },
				{ EItemType.FishingPoles, 1f },
				{ EItemType.Tools, 1f }
			};
		}

		public override string ShortTooltip => GetShortTooltip(sign: true);
		public override string Artist => "Zorutan";
		public override string ArtModifiedBy => null;
		public override string Designer => "andro951";
		public override string WikiDescription =>
			$"The '''Attack Speed Enchantments''' are [[enchantments]] that decrease an item's [https://terraria.wiki.gg/wiki/Use_time use time], " +
			$"resulting in more uses per second. Additionally, it increases mining speed on tools, the fire rate of minion's that shoot projectiles " +
			$"and reduces how long it takes a fish to bite when used on fishing poles.  If the enchantment gives 10%(configurable) or more attack speed, " +
			$"it will also enable [https://terraria.wiki.gg/wiki/Autoswing autoswing].";
	}
	public class AttackSpeedEnchantmentBasic : AttackSpeedEnchantment
	{
		public override SellCondition SellCondition => SellCondition.PostEaterOfWorldsOrBrainOfCthulhu;
		public override List<DropData> NpcDropTypes => new() {
			new(NPCID.EaterofWorldsHead)
		};
		public override List<DropData> NpcAIDrops => new() {
			new(NPCAIStyleID.Bat),
			new(NPCAIStyleID.Piranha)
		};
		public override List<DropData> ChestDrops => new() {
			new(ChestID.Chest_Normal),
			new(ChestID.Gold),
			new(ChestID.Gold_DeadMans),
			new(ChestID.Ivy),
			new(ChestID.Granite)
		};
		public override List<DropData> CrateDrops => new() {
			new(CrateID.Wooden, 0.5f),
			new(CrateID.Pearlwood_WoodenHard, 0.5f),
			new(CrateID.Iron, 0.5f),
			new(CrateID.Iron, 0.5f),
			new(CrateID.Jungle, 0.5f),
			new(CrateID.Bramble_JungleHard, 0.5f)
		};
	}
	public class AttackSpeedEnchantmentCommon : AttackSpeedEnchantment { }
	public class AttackSpeedEnchantmentRare : AttackSpeedEnchantment { }
	public class AttackSpeedEnchantmentEpic : AttackSpeedEnchantment { }
	public class AttackSpeedEnchantmentLegendary : AttackSpeedEnchantment { }

}
