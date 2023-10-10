using Terraria;
using System;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using System.Linq;
using androLib.Common.Utility;

namespace WeaponEnchantments.Items.Enchantments.Utility
{
	public abstract class TimeEnchantment : Enchantment, IRerollableEnchantment
	{
		public override string CustomTooltip => EnchantmentTypeName.Lang_WE(L_ID1.Tooltip, L_ID2.EnchantmentCustomTooltips);
		public override int StrengthGroup => 2;
		public override float ScalePercent => -1f;
		public override void GetMyStats() {
			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Weapons, 1f },
				{ EItemType.Armor, 1f },
				{ EItemType.Accessories, 1f },
				{ EItemType.FishingPoles, 1f },
				{ EItemType.Tools, 1f }
			};
		}
		public override void SetDefaults() {
			base.SetDefaults();
			Reroll();
		}
		public override void LoadData(TagCompound tag) {
			base.LoadData(tag);
			List<string> effectNames = tag.Get<List<string>>("effects");
			List<bool> effectsInverted = tag.Get<List<bool>>("effectsInverted");
			SetMyEffects(effectNames, effectsInverted);
		}
		private void SetMyEffects(List<string> effectNames, List<bool> inverted) {
			Effects = new();
			for (int i = 0; i < effectNames.Count; i++) {
				if (Enum.TryParse(effectNames[i], out EnchantmentStat enchantmentStat)) {
					AddEffect(enchantmentStat, inverted[i]);
				}
			}
		}
		private void SetMyEffects(List<EnchantmentStat> enchantmentStats, List<bool> inverted) {
			Effects = new();
			for (int i = 0; i < enchantmentStats.Count; i++) {
				AddEffect(enchantmentStats[i], inverted[i]);
			}
		}
		public void AddEffect(EnchantmentStat enchantmentStat, bool inverted) {
			DifficultyStrength strengthData = inverted ? EnchantmentStrengthData.Invert() : EnchantmentStrengthData;
			switch (enchantmentStat) {
				case EnchantmentStat.DayTimeRate:
					Effects.Add(new DayTimeRate(multiplicative: strengthData));
					break;
				case EnchantmentStat.DayTileUpdateRate:
					Effects.Add(new DayTileUpdateRate(multiplicative: strengthData));
					break;
				case EnchantmentStat.DayEventUpdateRate:
					Effects.Add(new DayEventUpdateRate(multiplicative: strengthData));
					break;
				case EnchantmentStat.NightTimeRate:
					Effects.Add(new NightTimeRate(multiplicative: strengthData));
					break;
				case EnchantmentStat.NightTileUpdateRate:
					Effects.Add(new NightTileUpdateRate(multiplicative: strengthData));
					break;
				case EnchantmentStat.NightEventUpdateRate:
					Effects.Add(new NightEventUpdateRate(multiplicative: strengthData));
					break;
			}
		}
		public override void SaveData(TagCompound tag) {
			base.SaveData(tag);
			List<string> effectNames = GetListsByNames(out List<bool> invertedList);
			tag["effects"] = effectNames;
			tag["effectsInverted"] = invertedList;
		}
		public List<EnchantmentStat> GetListsByEnchantmentStat(out List<bool> inverted) {
			inverted = Effects.Select(e => e.EffectStrength < 1f).ToList();
			return Effects.OfType<StatEffect>().Select(e => e.statName).ToList();
		}
		private List<string> GetListsByNames(out List<bool> inverted) {
			inverted = Effects.Select(e => e.EffectStrength < 1f).ToList();
			return Effects.OfType<StatEffect>().Select(e => e.statName.ToString()).ToList();
		}
		public override void OnCreated(ItemCreationContext context) {
			if (context is RecipeItemCreationContext recipeCreationContext) {
				IEnumerable<TimeEnchantment> consumedTimeEnchantments = recipeCreationContext.ConsumedItems.Select(i => i.ModItem).OfType<TimeEnchantment>();
				if (consumedTimeEnchantments.Any()) {
					TimeEnchantment consumedTimeEnchantment = consumedTimeEnchantments.First();
					List<EnchantmentStat> consumedEnchantmentStats = consumedTimeEnchantment.GetListsByEnchantmentStat(out List<bool> inverted);
					SetMyEffects(consumedEnchantmentStats, inverted);
				}
			}
		}

		public void Reroll() {
			List<List<EnchantmentEffect>> possibleEffects = new() {
				new() { new DayTimeRate(multiplicative: EnchantmentStrengthData), new DayTimeRate(multiplicative: EnchantmentStrengthData.Invert()) },
				new() { new DayTileUpdateRate(multiplicative: EnchantmentStrengthData), new DayTileUpdateRate(multiplicative: EnchantmentStrengthData.Invert()) },
				new() { new DayEventUpdateRate(multiplicative: EnchantmentStrengthData), new DayEventUpdateRate(multiplicative: EnchantmentStrengthData.Invert()) },
				new() { new NightTimeRate(multiplicative: EnchantmentStrengthData), new NightTimeRate(multiplicative: EnchantmentStrengthData.Invert()) },
				new() { new NightTileUpdateRate(multiplicative: EnchantmentStrengthData), new NightTileUpdateRate(multiplicative: EnchantmentStrengthData.Invert()) },
				new() { new NightEventUpdateRate(multiplicative: EnchantmentStrengthData), new NightEventUpdateRate(multiplicative: EnchantmentStrengthData.Invert()) }
			};

			int minEffects = Main.gameMenu ? possibleEffects.Count : 1;
			Effects = RandomEffectHandler.GetRandomEffects(possibleEffects, chance: 0.5f, minEffects: minEffects);
		}

		public override string ShortTooltip => GetShortTooltip(percent: false, multiply100: false, multiplicative: true);
		public override string Artist => "andro951";
		public override string ArtModifiedBy => null;
		public override string Designer => "andro951";
	}
	[Autoload(false)]
	public class TimeEnchantmentBasic : TimeEnchantment
	{
		public override SellCondition SellCondition => SellCondition.Never;
		public override List<DropData> NpcDropTypes => new() {
			new(NPCID.Pixie, chance: 0.02f),
			new(NPCID.Wraith, chance: 0.05f),
			new(NPCID.Mummy, chance: 0.15f),
			new(NPCID.BloodMummy, chance: 0.15f),
			new(NPCID.DarkMummy, chance: 0.15f),
			new(NPCID.LightMummy, chance: 0.15f),
		};
		public override List<DropData> ChestDrops => new() {
			new(ChestID.Skyware, chance: 1f)
		};
		public override List<DropData> CrateDrops => new() {
			new(CrateID.Sky, 0.5f),
			new(CrateID.Azure_SkyHard, 0.5f)
		};
	}
	[Autoload(false)]
	public class TimeEnchantmentCommon : TimeEnchantment { }
	[Autoload(false)]
	public class TimeEnchantmentRare : TimeEnchantment { }
	[Autoload(false)]
	public class TimeEnchantmentEpic : TimeEnchantment { }
	[Autoload(false)]
	public class TimeEnchantmentLegendary : TimeEnchantment { }
}
