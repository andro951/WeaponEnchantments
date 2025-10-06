using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using WeaponEnchantments.Items;
using static WeaponEnchantments.Common.Globals.EnchantedWeapon;
using static WeaponEnchantments.WEPlayer;
using static WeaponEnchantments.Common.Configs.ConfigValues;
using androLib.Common.Utility;

namespace WeaponEnchantments.Common {
    public class PlayerEquipment {
        private static int vanillaArmorSlots = 3;       // Head, Chest, Leggings
        private static int vanillaAccesorySlots = 7;    // 5 normal, 1 demon heart, 1 master

        public Item HeldItem => heldItem[0];
        private Item[] heldItem = new Item[1];
        private Item[] Armor = new Item[vanillaArmorSlots];
        public Item InfusedHead;
        public Item InfusedBody;
        public Item InfusedLegs;
        private Item[] Accessories;
        WEPlayer wePlayer;

        public PlayerEquipment(Player player) {
            heldItem[0] = player.HeldItem ?? new();

            wePlayer = player.GetWEPlayer();
            ModAccessorySlotPlayer alp = player.GetModPlayer<ModAccessorySlotPlayer>();
            AccessorySlotLoader loader = LoaderManager.Get<AccessorySlotLoader>();

            List<Item> modAccessories = new();
            if (WEMod.serverConfig.EnchantmentEffectsOnModdedAccessorySlots) {
				for (int i = 0; i < alp.SlotCount; i++) {
					var slot = loader.Get(i, player);
					if (loader.ModdedIsItemSlotUnlockedAndUsable(i, player) /*&& slot.IsEnabled()*/)
						modAccessories.Add(slot.FunctionalItem ?? new());
				}
			}

			Accessories = new Item[vanillaAccesorySlots + modAccessories.Count];

            for(int i = 0; i < vanillaArmorSlots; i++) {
                Armor[i] = player.armor[i] ?? new();

                if (Armor[i].TryGetEnchantedItem(out EnchantedArmor enchantedArmor)) {
                    if (enchantedArmor.infusedItem == null)
                        continue;

					switch (i) {
                        case 0:
                            InfusedHead = enchantedArmor.infusedItem;
                            break;
                        case 1:
                            InfusedBody = enchantedArmor.infusedItem;
                            break;
                        case 2:
                            InfusedLegs = enchantedArmor.infusedItem;
                            break;
					}
				}
            }

            for (int i = 0; i < vanillaAccesorySlots; i++) {
                Item item = player.armor[i + 3];
                Accessories[i] = player.armor[i + 3] ?? new();
            }

            for (int i = 0; i < modAccessories.Count; i++) {
                Item item = modAccessories[i];
                Accessories[i + vanillaAccesorySlots] = modAccessories[i];
			}
        }

        public static bool operator ==(PlayerEquipment pe, PlayerEquipment other) {
            return pe.Equals(other);
        }

        public static bool operator !=(PlayerEquipment pe, PlayerEquipment other) {
            return !pe.Equals(other);
        }

        public static IEnumerable<EnchantedItem> FilterEnchantedItems(IEnumerable<Item> items) {
            IEnumerable<EnchantedItem> enchantedItems = items.Select(i => i.GetEnchantedItem()).OfType<EnchantedItem>();

            return enchantedItems;
        }
        
        public void UpdateEnchantedEquipItemEffects(IEnumerable<EnchantedEquipItem> enchantedItems) {
            List<EnchantmentEffect> enchantmentEffects = new List<EnchantmentEffect>();

            //Damage Reduction Per level
            if (!WEMod.serverConfig.DamageReductionPerLevelDisabled) {
                float armorMultiplier = ArmorDamageReductionPerLevel;
                float accessoryMultiplier = AccessoryDamageReductionPerLevel;
                foreach(EnchantedEquipItem enchantedItem in enchantedItems) {
                    float damageReduction = enchantedItem.GetPerLevelDamageReduction(armorMultiplier, accessoryMultiplier);
					if (damageReduction > 0f)
						enchantmentEffects.Add(new DamageReduction(@base: new DifficultyStrength(damageReduction)));
                }
            }

            // Get all non null enchanted items
            foreach (EnchantedEquipItem enchantedItem in enchantedItems) {
                // For each enchanted item, get its enchantments
                GetEnchantmentEffects(enchantedItem, enchantmentEffects);
            }

            wePlayer.EnchantmentEffects = enchantmentEffects;
            SortEnchantmentEffects(wePlayer);
        }
        public void UpdateEnchantedHeldItemEffects(EnchantedHeldItem enchantedHeldItem) {
            List<EnchantmentEffect> enchantmentEffects = new List<EnchantmentEffect>();
            if (!WEMod.serverConfig.CritPerLevelDisabled && enchantedHeldItem is EnchantedWeapon enchantedWeapon) {
                float bonus = enchantedWeapon.GetPerLevelBonus();
                if (bonus > 0f) {
					if (enchantedHeldItem.Item.IsGatheringTool()) {
						enchantmentEffects.Add(new MiningSpeed(@base: new DifficultyStrength(bonus)));
					}
                    else if (enchantedHeldItem.Item.IsFishingPole()) {
						enchantmentEffects.Add(new FishingPower(@base: new DifficultyStrength(bonus * 100)));
					}
					else if (WEMod.serverConfig.DamagePerLevelInstead) {
                        enchantmentEffects.Add(new DamageAfterDefenses(multiplicative: new DifficultyStrength(1f + bonus)));
				    }
					else {
                        enchantmentEffects.Add(new CriticalStrikeChance(@base: new DifficultyStrength(bonus)));
					}
				}
            }

			GetEnchantmentEffects(enchantedHeldItem, enchantmentEffects);
            List<IAddDynamicEffects> addDynamicEffects = enchantmentEffects.OfType<IAddDynamicEffects>().ToList();
			foreach (IAddDynamicEffects effect in addDynamicEffects) {
                effect.AddDynamicEffects(enchantmentEffects, enchantedHeldItem);
            }

            enchantedHeldItem.EnchantmentEffects = enchantmentEffects;
            SortEnchantmentEffects(enchantedHeldItem);
        }

        public void GetEnchantmentEffects(EnchantedItem enchantedItem, List<EnchantmentEffect> effects) {
            IEnumerable<Enchantment> enchantments = enchantedItem.enchantments.All.Select(e => e.ModItem).OfType<Enchantment>();
            // For each enchantment get its effects
            foreach (Enchantment enchantment in enchantments) {
                foreach (EnchantmentEffect enchantmentEffect in enchantment.Effects) {
                    effects.Add(enchantmentEffect);
                }
            }
        }
        public void SortEnchantmentEffects(ISortedEnchantmentEffects entity) {
            IEnumerable<EnchantmentEffect> enchantmentEffects = entity.EnchantmentEffects;
            entity.PassiveEffects = enchantmentEffects.OfType<IPassiveEffect>().ToList();
            entity.StatEffects = enchantmentEffects.OfType<StatEffect>().ToList();
            entity.ModifyShootStatEffects = enchantmentEffects.OfType<IModifyShootStats>().ToList();
            entity.VanillaStats = GetStatEffectDictionary(entity.StatEffects.OfType<IVanillaStat>());
            entity.EnchantmentStats = GetStatEffectDictionary(entity.StatEffects.OfType<INonVanillaStat>());
	        entity.PlayerSetEffects = new SortedList<EnchantmentStat, PlayerSetEffect>(entity.EnchantmentEffects.OfType<PlayerSetEffect>().ToSortedList());
            entity.BoolEffects = GetBoolEffectDictionary(entity.EnchantmentEffects.OfType<BoolEffect>());
		    IEnumerable<BuffEffect> buffEffects = enchantmentEffects.OfType<BuffEffect>();
            entity.OnTickBuffs = GetBuffEffects(buffEffects.OfType<BuffEffect>().Where(e => e.BuffStyle is BuffStyle.OnTickPlayerBuff or BuffStyle.OnTickPlayerDebuff));

            SortOnHitEffects(entity, enchantmentEffects, buffEffects);
        }
        public void SortOnHitEffects(ISortedEnchantmentEffects entity, IEnumerable<EnchantmentEffect> enchantmentEffects, IEnumerable<BuffEffect> buffEffects) {
            if (entity is not ISortedOnHitEffects onHitEffectsEntity)
                return;

            onHitEffectsEntity.OnHitEffects = enchantmentEffects.OfType<IOnHitEffect>().ToList();
            onHitEffectsEntity.OnHitDebuffs = GetBuffEffects(buffEffects.OfType<BuffEffect>().Where(e => e.BuffStyle is BuffStyle.OnHitEnemyBuff or BuffStyle.OnHitEnemyDebuff));
            onHitEffectsEntity.OnHitBuffs = GetBuffEffects(buffEffects.OfType<BuffEffect>().Where(e => e.BuffStyle is BuffStyle.OnHitPlayerBuff or BuffStyle.OnHitPlayerDebuff));
        }
        private SortedDictionary<EnchantmentStat, EStatModifier> GetStatEffectDictionary<T>(IEnumerable<T> statEffects) where T : IApplyStats {
            SortedDictionary<EnchantmentStat, EStatModifier> result = new SortedDictionary<EnchantmentStat, EStatModifier>();
            foreach (T statEffect in statEffects) {
                result.AddOrCombine(statEffect.EStatModifier.Clone());
            }

            return result;
        }
        private SortedDictionary<EnchantmentStat, bool> GetBoolEffectDictionary(IEnumerable<BoolEffect> boolEffects) {
            SortedDictionary<EnchantmentStat, bool> result = new SortedDictionary<EnchantmentStat, bool>();
            foreach(BoolEffect boolEffect in boolEffects) {
                result.AddOrCombine(boolEffect);
			}

            return result;
		}
        private SortedDictionary<short, BuffStats> GetBuffEffects(IEnumerable<BuffEffect> buffEffects) {
		    SortedDictionary<short, BuffStats> result = new SortedDictionary<short, BuffStats>();
		    foreach	(BuffEffect buffEffect in buffEffects) {
			    result.AddOrCombine(buffEffect);
		    }
		    
		    return result;
	    }
        public void CombineDictionaries() {
			if (!HeldItem.TryGetEnchantedHeldItem(out EnchantedHeldItem enchantedHeldItem))
				enchantedHeldItem = new EnchantedWeapon();

			wePlayer.CombinedVanillaStats = CombineStatEffectDictionaries(wePlayer.VanillaStats, enchantedHeldItem.VanillaStats, true);
            wePlayer.CombinedOnTickBuffs = CombineBuffEffectDictionaries(wePlayer.OnTickBuffs, enchantedHeldItem.OnTickBuffs);
            wePlayer.CombinedBoolEffects = CombineBoolEffectDictionaries(wePlayer.CombinedBoolEffects, enchantedHeldItem.BoolEffects);
            wePlayer.CombinedPassiveEffects = CombinePassiveEffectDictionaries(wePlayer.PassiveEffects, enchantedHeldItem.PassiveEffects);
        }
        public void CombineOnHitDictionaries(Item item = null) {
            item ??= heldItem[0];
            if (!item.TryGetEnchantedHeldItem(out EnchantedHeldItem enchantedHeldItem))
                enchantedHeldItem = new EnchantedWeapon();

            wePlayer.CombinedModifyShootStatEffects = wePlayer.ModifyShootStatEffects.Concat(enchantedHeldItem.ModifyShootStatEffects).ToList();
            wePlayer.CombinedPlayerSetEffects = wePlayer.PlayerSetEffects.CombineSortedLists(enchantedHeldItem.PlayerSetEffects);
            wePlayer.CombinedEnchantmentStats = CombineStatEffectDictionaries(wePlayer.EnchantmentStats, enchantedHeldItem.EnchantmentStats);

            if (!item.TryGetEnchantedItem(out EnchantedWeapon enchantedWeapon))
                enchantedWeapon = new EnchantedWeapon();

            wePlayer.CombinedOnHitEffects = wePlayer.OnHitEffects.Concat(enchantedWeapon.OnHitEffects).ToList();
            wePlayer.CombinedOnHitDebuffs = CombineBuffEffectDictionaries(wePlayer.OnHitDebuffs, enchantedWeapon.OnHitDebuffs);
            wePlayer.CombinedOnHitBuffs = CombineBuffEffectDictionaries(wePlayer.OnHitBuffs, enchantedWeapon.OnHitBuffs);
        }
        private SortedDictionary<EnchantmentStat, EStatModifier> CombineStatEffectDictionaries(SortedDictionary<EnchantmentStat, EStatModifier> playerDictionary, SortedDictionary<EnchantmentStat, EStatModifier> heldItemDictionary, bool vallinllaStatCheck = false) {
            SortedDictionary<EnchantmentStat, EStatModifier> result = new SortedDictionary<EnchantmentStat, EStatModifier>(playerDictionary.ToDictionary(k => k.Key, k => k.Value.Clone()));
            foreach (EnchantmentStat key in heldItemDictionary.Keys) {
                if (!vallinllaStatCheck || !ID_Dictionaries.WeaponStatDict.Contains(key))
                    result.AddOrCombine(heldItemDictionary[key].Clone());
			}

            return result;
		}
        private SortedDictionary<short, BuffStats> CombineBuffEffectDictionaries(SortedDictionary<short, BuffStats> playerDictionary, SortedDictionary<short, BuffStats> healdItemDictionary) {
            SortedDictionary<short, BuffStats> result = new SortedDictionary<short, BuffStats>(playerDictionary.ToDictionary(k => k.Key, k => k.Value.Clone()));
            foreach (short buffID in healdItemDictionary.Keys) {
                result.AddOrCombine(healdItemDictionary[buffID].Clone());
            }

            return result;
        }
        private SortedDictionary<EnchantmentStat, bool> CombineBoolEffectDictionaries(SortedDictionary<EnchantmentStat, bool> playerDictionary, SortedDictionary<EnchantmentStat, bool> heldItemDictionary) {
            SortedDictionary<EnchantmentStat, bool> result = new SortedDictionary<EnchantmentStat, bool>(playerDictionary);
            foreach(EnchantmentStat key in heldItemDictionary.Keys) {
                result.AddOrCombine((key, heldItemDictionary[key]));
			}

            return result;
        }
        private List<IPassiveEffect> CombinePassiveEffectDictionaries(List<IPassiveEffect> playerList, List<IPassiveEffect> heldItemList) {
			List<IPassiveEffect> result = new List<IPassiveEffect>(playerList);
            foreach (IPassiveEffect passiveEffect in heldItemList) {
                result.Add(passiveEffect);
            }

            return result;
		}

		public IEnumerable<Item> GetAllArmor() => Armor.Concat(Accessories);

        private IEnumerable<Item> GetAllItems() {
            Item[] items = new Item[1 + Armor.Length + Accessories.Length];
            items[0] = heldItem[0];
            Armor.CopyTo(items, 1);
            Accessories.CopyTo(items, 1 + Armor.Length);

            return items;
        }

        //private IEnumerable<EnchantedItem> GetEnchantedItems() {
        //    return FilterEnchantedItems(GetAllItems());
        //}

        private IEnumerable<EnchantedEquipItem> GetEnchantedEquipItems() {
            return FilterEnchantedItems(GetAllArmor()).OfType<EnchantedEquipItem>();
        }

        private EnchantedHeldItem GetEnchantedHeldItem(Item item) {
            if (item != null && item.TryGetEnchantedHeldItem(out EnchantedHeldItem enchantedHeldItem))
                return enchantedHeldItem;

            if (Main.mouseItem.TryGetEnchantedHeldItem(out enchantedHeldItem))
                return enchantedHeldItem;

			if (HeldItem.TryGetEnchantedHeldItem(out enchantedHeldItem))
                return enchantedHeldItem;

            return null;
		}

        //public IEnumerable<EnchantmentEffect> GetAllEnchantmentEffects() {
        //    return ExtractEnchantmentEffects(GetEnchantedItems());
        //}
        public void CheckRemoveEnchantments() {
            Player player = wePlayer.Player;
			if (!HeldItem.NullOrAir())
				HeldItem.CheckRemoveEnchantments(player);

			for (int i = 0; i < Armor.Length; i++) {
				ref Item item = ref Armor[i];
				if (!item.NullOrAir())
					item.CheckRemoveEnchantments(player);
			}

			for (int i = 0; i < Accessories.Length; i++) {
				ref Item item = ref Accessories[i];
				if (!item.NullOrAir())
					item.CheckRemoveEnchantments(player);
			}
		}
        public void UpdateArmorEnchantmentEffects() {
            UpdateEnchantedEquipItemEffects(GetEnchantedEquipItems());
        }

        public void UpdateHeldItemEnchantmentEffects(Item item = null) {
            EnchantedHeldItem enchantedHeldItem = GetEnchantedHeldItem(item);
            if (enchantedHeldItem != null)
                UpdateEnchantedHeldItemEffects(enchantedHeldItem);
		}

        public override bool Equals(object obj) {
            if (ReferenceEquals(this, obj))
                return true;

            if (ReferenceEquals(obj, null))
                return false;

            if (obj is not PlayerEquipment other)
                return false;

            IEnumerable<Item> myItems = GetAllItems();
            IEnumerable<Item> otherItems = other.GetAllItems();
            int count = myItems.Count();
            if (count != otherItems.Count())
                return false;

            for (int i = 0; i < count; i++) {
                Item ci = myItems.ElementAt(i);
                Item ci2 = otherItems.ElementAt(i);
                if (ci.NullOrAir() && ci2.NullOrAir())
                    continue;

                if (!ci.IsSameEnchantedItem(ci2))
                    return false;
            }

            return true;
        }
    }

    public static class PlayerEquipmentStaticMethods {
		public static void AllArmorGainXp(this Player player, int xp) {
			IEnumerable<Item> allArmor = player.GetWEPlayer().Equipment.GetAllArmor();
			foreach (Item armor in allArmor) {
				//Gain xp on each armor
				if (!armor.vanity && armor.TryGetEnchantedItemSearchAll(out EnchantedItem aGlobal)) {
					float reductionFactor;
					if (armor.IsArmorItem()) {
						reductionFactor = 2f;
					}
					else {
						reductionFactor = 4f;
					}

					int xpInt = (int)Math.Round(xp / reductionFactor);

					if (xpInt <= 0)
						xpInt = 1;

					aGlobal.GainXP(armor, xpInt);
				}
			}
		}
	}
}
