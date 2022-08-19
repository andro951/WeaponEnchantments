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

namespace WeaponEnchantments.Common {
    public class PlayerEquipment {
        private static int vanillaArmorSlots = 3;       // Head, Chest, Leggings
        private static int vanillaAccesorySlots = 7;    // 5 normal, 1 demon heart, 1 master

        public Item HeldItem => heldItem[0];
        private Item[] heldItem = new Item[1];
        private Item[] Armor = new Item[vanillaArmorSlots];
        private Item[] Accesories;
        Player owner;
        WEPlayer wePlayer;

        public PlayerEquipment(Player player) {
            if (!Main.HoverItem.NullOrAir()) {
                heldItem[0] = Main.HoverItem;
            }
			else {
                heldItem[0] = player.HeldItem;
            }

            owner = player;
            wePlayer = player.GetWEPlayer();
            ModAccessorySlotPlayer alp = player.GetModPlayer<ModAccessorySlotPlayer>();
            AccessorySlotLoader loader = LoaderManager.Get<AccessorySlotLoader>();

            int moddedSlotCount = alp.SlotCount;

            Accesories = new Item[vanillaAccesorySlots + moddedSlotCount]; 

            for(int i = 0; i < vanillaArmorSlots; i++) {        // Set all (vanilla) armor slots
                Armor[i] = player.armor[i];
            }

            for (int i = 0; i < vanillaAccesorySlots; i++) {    // Set all vanilla accesory slots
                Accesories[i] = player.armor[i + 3];
            }

            for (int i = 0; i < moddedSlotCount; i++) {         // Set all modded accesory slots (cheatsheet does what it wants)
                var slot = loader.Get(i, player);
                if (slot.IsEnabled() && !slot.IsEmpty)
                    Accesories[vanillaAccesorySlots + i] = slot.FunctionalItem;
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

            // Get all non null enchanted items
            foreach (EnchantedEquipItem enchantedItem in enchantedItems) {
                // For each enchanted item, get its enchantments
                GetEnchantmentEffects(enchantedItem, enchantmentEffects);
            }

            wePlayer.EnchantmentEffects = enchantmentEffects;
            SortEnchantmentEffects(wePlayer);
        }
        public void UpdateEnchantedWeaponEffects(EnchantedWeapon enchantedWeapon) {
            List<EnchantmentEffect> enchantmentEffects = new List<EnchantmentEffect>();
            GetEnchantmentEffects(enchantedWeapon, enchantmentEffects);
            enchantedWeapon.EnchantmentEffects = enchantmentEffects;
            SortEnchantmentEffects(enchantedWeapon);
        }

        public void GetEnchantmentEffects(EnchantedItem enchantedItem, List<EnchantmentEffect> effects) {
            IEnumerable<Enchantment> enchantments = enchantedItem.enchantments.Select(e => e.ModItem).OfType<Enchantment>();
            // For each enchantment get its effects
            foreach (Enchantment enchantment in enchantments) {
                foreach (EnchantmentEffect enchantmentEffect in enchantment.Effects) {
                    DamageClass dc = enchantedItem.Item.DamageType;
                    enchantmentEffect.EfficiencyMultiplier = enchantment.AllowedList[enchantedItem.ItemType] * enchantmentEffect.GetClassEfficiency(dc);
                    effects.Add(enchantmentEffect);
                }
            }
        }
        public void SortEnchantmentEffects(ISortedEnchantmentEffects entity) {
            IEnumerable<EnchantmentEffect> enchantmentEffects = entity.EnchantmentEffects;
            entity.PassiveEffects = enchantmentEffects.OfType<IPassiveEffect>().ToList();
            entity.StatEffects = enchantmentEffects.OfType<StatEffect>().ToList();
            entity.OnHitEffects = enchantmentEffects.OfType<IOnHitEffect>().ToList();
            entity.VanillaStats = GetStatEffectDictionary(entity.StatEffects.OfType<IVanillaStat>());
            entity.EnchantmentStats = GetStatEffectDictionary(entity.StatEffects.OfType<INonVanillaStat>());
		    IEnumerable<BuffEffect> buffEffects = enchantmentEffects.OfType<BuffEffect>();
            entity.OnHitDebuffs = GetBuffEffects(buffEffects.OfType<OnHitTargetBuffEffectGeneral>());
            entity.OnHitBuffs = GetBuffEffects(buffEffects.OfType<OnHitPlayerBuffEffectGeneral>());
            entity.OnTickBuffs = GetBuffEffects(buffEffects.OfType<OnTickPlayerBuffEffectGeneral>());
		}
        private SortedDictionary<EnchantmentStat, EStatModifier> GetStatEffectDictionary<T>(IEnumerable<T> statEffects) where T : IApplyStats {
            SortedDictionary<EnchantmentStat, EStatModifier> result = new SortedDictionary<EnchantmentStat, EStatModifier>();
            foreach (T statEffect in statEffects) {
                result.AddOrCombine(statEffect.EStatModifier);
            }

            return result;
        }
	    private SortedDictionary<short, BuffStats> GetBuffEffects<T>(IEnumerable<T> buffEffects) where T : BuffEffect {
		    SortedDictionary<short, BuffStats> result = new SortedDictionary<short, BuffStats>();
		    foreach	(T buffEffect in buffEffects) {
			    result.AddOrCombine(buffEffect);
		    }
		
		    return result;
	    }
        public void CombineDictionaries() {
			if (!HeldItem.TryGetEnchantedItem(out EnchantedWeapon enchantedWeapon))
				enchantedWeapon = new EnchantedWeapon();

			wePlayer.CombinedVanillaStats = CombineStatEffectDictionaries(wePlayer.VanillaStats, enchantedWeapon.VanillaStats, true);
            wePlayer.CombinedOnTickBuffs = CombineBuffEffectDictionaries(wePlayer.OnTickBuffs, enchantedWeapon.OnTickBuffs);
        }
        public void CombineOnHitDictionaries(Item item = null) {
            item ??= heldItem[0];
            if (!item.TryGetEnchantedItem(out EnchantedWeapon enchantedWeapon))
                enchantedWeapon = new EnchantedWeapon();

            wePlayer.CombinedOnHitEffects = wePlayer.OnHitEffects.Concat(enchantedWeapon.OnHitEffects).ToList();
            wePlayer.CombinedEnchantmentStats = CombineStatEffectDictionaries(wePlayer.EnchantmentStats, enchantedWeapon.EnchantmentStats);
            wePlayer.CombinedOnHitDebuffs = CombineBuffEffectDictionaries(wePlayer.OnHitDebuffs, enchantedWeapon.OnHitDebuffs);
            wePlayer.CombinedOnHitBuffs = CombineBuffEffectDictionaries(wePlayer.OnHitBuffs, enchantedWeapon.OnHitBuffs);
        }
        private SortedDictionary<EnchantmentStat, EStatModifier> CombineStatEffectDictionaries(SortedDictionary<EnchantmentStat, EStatModifier> playerDictionary, SortedDictionary<EnchantmentStat, EStatModifier> weaponDictionary, bool vallinllaStatCheck = false) {
            SortedDictionary<EnchantmentStat, EStatModifier> result = new SortedDictionary<EnchantmentStat, EStatModifier>();
            foreach (EnchantmentStat key in weaponDictionary.Keys) {
                if (!vallinllaStatCheck || !WeaponStatDict.Contains(key))
                    result.AddOrCombine(weaponDictionary[key]);
			}

            return result;
		}
        private SortedDictionary<short, BuffStats> CombineBuffEffectDictionaries(SortedDictionary<short, BuffStats> playerDictionary, SortedDictionary<short, BuffStats> weaponDictionary) {
            SortedDictionary<short, BuffStats> result = new SortedDictionary<short, BuffStats>(playerDictionary);
            foreach (short buffID in weaponDictionary.Keys) {
                result.AddOrCombine(weaponDictionary[buffID]);
            }

            return result;
        }
        private IEnumerable<Item> GetAllArmor() {
            Item[] items = new Item[Armor.Length + Accesories.Length];
            Armor.CopyTo(items, 0);
            Accesories.CopyTo(items, Armor.Length);

            return items;
        }

        private IEnumerable<Item> GetAllItems() {
            Item[] items = new Item[1 + Armor.Length + Accesories.Length];
            items[0] = heldItem[0];
            Armor.CopyTo(items, 1);
            Accesories.CopyTo(items, 1 + Armor.Length);

            return items;
        }

        //private IEnumerable<EnchantedItem> GetEnchantedItems() {
        //    return FilterEnchantedItems(GetAllItems());
        //}

        private IEnumerable<EnchantedEquipItem> GetEnchantedEquipItems() {
            return FilterEnchantedItems(GetAllArmor()).OfType<EnchantedEquipItem>();
        }

        private EnchantedWeapon GetEnchantedWeapon() {
            if (HeldItem.TryGetEnchantedItem(out EnchantedWeapon enchantedWeapon))
                return enchantedWeapon;

            return null;
		}

        //public IEnumerable<EnchantmentEffect> GetAllEnchantmentEffects() {
        //    return ExtractEnchantmentEffects(GetEnchantedItems());
        //}

        public void UpdateArmorEnchantmentEffects() {
            UpdateEnchantedEquipItemEffects(GetEnchantedEquipItems());
        }

        public void UpdateWeaponEnchantmentEffects() {
            EnchantedWeapon enchantedWeapon = GetEnchantedWeapon();
            if (enchantedWeapon != null)
                UpdateEnchantedWeaponEffects(enchantedWeapon);
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
}
