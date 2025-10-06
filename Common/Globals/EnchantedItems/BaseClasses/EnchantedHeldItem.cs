using androLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using WeaponEnchantments.ModIntegration;
using androLib.Common.Utility;
using androLib.Common.Globals;

namespace WeaponEnchantments.Common.Globals
{
	public abstract class EnchantedHeldItem : EnchantedItem, ISortedEnchantmentEffects
	{
		public SortedDictionary<PermenantItemFields, StatModifier> AppliedPermenantStats = new SortedDictionary<PermenantItemFields, StatModifier>();
		public SortedDictionary<PermenantItemFields, StatModifier> PermenantStats = new SortedDictionary<PermenantItemFields, StatModifier>();

		public SortedDictionary<EnchantmentStat, EStatModifier> EnchantmentStats { set; get; } = new SortedDictionary<EnchantmentStat, EStatModifier>();
		public SortedDictionary<EnchantmentStat, EStatModifier> VanillaStats { set; get; } = new SortedDictionary<EnchantmentStat, EStatModifier>();
		public SortedList<EnchantmentStat, PlayerSetEffect> PlayerSetEffects { set; get; } = new SortedList<EnchantmentStat, PlayerSetEffect>();
		public SortedDictionary<EnchantmentStat, bool> BoolEffects { set; get; } = new SortedDictionary<EnchantmentStat, bool>();
		public SortedDictionary<short, BuffStats> OnTickBuffs { set; get; } = new SortedDictionary<short, BuffStats>();

		public List<EnchantmentEffect> EnchantmentEffects { set; get; } = new List<EnchantmentEffect>();
		public List<IPassiveEffect> PassiveEffects { set; get; } = new List<IPassiveEffect>();

		public List<IModifyShootStats> ModifyShootStatEffects { set; get; } = new List<IModifyShootStats>();
		public List<StatEffect> StatEffects { set; get; } = new List<StatEffect>();


		public override void ModifyManaCost(Item item, Player player, ref float reduce, ref float mult) {
			if (CheckGetModifier(EnchantmentStat.ManaUsage, out EStatModifier eStatModifier))
				eStatModifier.ApplyTo(ref reduce, ref mult, item);
		}

		//Calamity Rogue weapon Attackspeed fix
		private bool findingUseSpeed;
		public override float UseSpeedMultiplier(Item item, Player player) {
			float attackSpeed = GetVanillaModifierStrength(EnchantmentStat.AttackSpeed);

			//Calamity Rogue weapon Attackspeed fix
			if (AndroMod.calamityEnabled && attackSpeed != 1f && !findingUseSpeed) {
				if (ContentSamples.ItemsByType[item.type].DamageType == CalamityValues.rogue) {
					string itemFullName = item.ModFullName();
					if (itemFullName == "CalamityMod/ExecutionersBlade" || itemFullName == "CalamityHunt/CometKunai") {
						findingUseSpeed = true;
						float multiplier = CombinedHooks.TotalUseTimeMultiplier(player, item);
						Item sampleItem = ContentSamples.ItemsByType[item.type];
						item.useTime = Math.Max(1, (int)Math.Round(sampleItem.useTime * multiplier));
						item.useAnimation = Math.Max(1, (int)Math.Round(sampleItem.useAnimation * multiplier));
						findingUseSpeed = false;
					}
					else {
						player.GetAttackSpeed(CalamityValues.rogue) *= attackSpeed;
					}

					return 1f;
				}
			}

			if (GetPlayerModifierStrength(player, EnchantmentStat.CatastrophicRelease, out float catastrophicReleaseMultiplier))
				attackSpeed *= 0.1f;

			return attackSpeed;
		}
		public override bool CanUseItem(Item item, Player player) {
			return base.CanUseItem(item, player);
		}
		public override bool? UseItem(Item item, Player player) {
			bool? returnValue = null;
			foreach (IUseItem effect in EnchantmentEffects.OfType<IUseItem>()) {
				bool? useItem = effect.UseItem(item, player);
				if (useItem != null) {
					if (returnValue is false or null) {
						returnValue = useItem;
					}
				}
			}

			return returnValue;
		}
		public override bool CanConsumeAmmo(Item weapon, Item ammo, Player player) {
			/*float rand = Main.rand.NextFloat();
            float ammoSaveChance = -1f * weapon.ApplyEStat("AmmoCost", 0f);

            //True means it will consume ammo
            return rand > ammoSaveChance;*/
			return CheckConsumeAmmoEffect(weapon, ammo, player);
		}
		public override bool ConsumeItem(Item item, Player player) {
			return CheckConsumeAmmoEffect(item, item, player);
		}
		private bool CheckConsumeAmmoEffect(Item weapon, Item ammo, Player player) {
			if (GetPlayerModifierStrength(player, EnchantmentStat.AmmoCost, out float strength)) {
				float weaponChance;
				switch (weapon.type) {
					case ItemID.VortexBeater:
					case ItemID.Phantasm:
					case ItemID.SDMG:
						weaponChance = 1f / 3f;
						break;
					case ItemID.Celeb2:
					case ItemID.Gatligator:
					case ItemID.Megashark:
					case ItemID.ChainGun:
						weaponChance = 0.5f;
						break;
					case ItemID.CandyCornRifle:
					case ItemID.Minishark:
						weaponChance = 2f / 3f;
						break;
					default:
						weaponChance = 1f;
						break;
				}

				if (player.magicQuiver && AmmoID.Sets.IsArrow[weapon.useAmmo])
					weaponChance *= 0.8f;

				if (player.ammoBox)
					weaponChance *= 0.8f;

				if (player.ammoPotion)
					weaponChance *= 0.8f;

				if (player.huntressAmmoCost90)
					weaponChance *= 0.9f;

				if (player.chloroAmmoCost80)
					weaponChance *= 0.8f;

				if (player.ammoCost80)
					weaponChance *= 0.8f;

				if (player.ammoCost75)
					weaponChance *= 0.75f;

				if (weapon.CountsAsClass(DamageClass.Throwing)) {
					if (player.ThrownCost50)
						weaponChance *= 0.5f;

					if (player.ThrownCost33)
						weaponChance *= 1f / 3f;
				}

				float combinedStrength = strength / weaponChance;
				float rand = Main.rand.NextFloat();
				return rand > combinedStrength;
			}

			return true;
		}
		protected void CheckEnchantmentStatsForModifier(ref StatModifier statModifier, EnchantmentStat enchantmentStat) {
			if (VanillaStats.ContainsKey(enchantmentStat))
				statModifier = statModifier.CombineWith(VanillaStats[enchantmentStat].StatModifier);
		}
		protected void CheckEnchantmnetStatsApplyTo(ref float value, EnchantmentStat enchantmentStat) {
			if (VanillaStats.ContainsKey(enchantmentStat))
				VanillaStats[enchantmentStat].ApplyTo(ref value);
		}
		protected bool CheckGetModifier(EnchantmentStat enchantmentStat, out EStatModifier m) {
			if (!VanillaStats.ContainsKey(enchantmentStat)) {
				m = null;
				return false;
			}

			m = VanillaStats[enchantmentStat];
			return true;
		}
		public float GetVanillaModifierStrength(EnchantmentStat enchantmentStat) {
			if (VanillaStats.ContainsKey(enchantmentStat))
				return VanillaStats[enchantmentStat].Strength;

			return 1f;
		}
		protected bool GetPlayerModifierStrength(Player player, EnchantmentStat enchantmentStat, out float strength) {
			WEPlayer wePlayer = player.GetWEPlayer();
			strength = 0f;
			if (wePlayer.CombinedEnchantmentStats.ContainsKey(enchantmentStat)) {
				strength = wePlayer.CombinedEnchantmentStats[enchantmentStat].Strength;
				return true;
			}

			return false;
		}
		public bool GetPlayerModifierStrengthForTooltip(Player player, EnchantmentStat enchantmentStat, out float strength) {
			WEPlayer wePlayer = player.GetWEPlayer();
			strength = 1f;
			if (wePlayer.EnchantmentStats.ContainsKey(enchantmentStat))
				wePlayer.EnchantmentStats[enchantmentStat].ApplyTo(ref strength);

			if (EnchantmentStats.ContainsKey(enchantmentStat))
				EnchantmentStats[enchantmentStat].ApplyTo(ref strength);

			return strength != 1f;
		}
	}
}