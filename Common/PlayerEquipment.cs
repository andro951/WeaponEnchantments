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

namespace WeaponEnchantments.Common {
    public class PlayerEquipment {
        private static int vanillaArmorSlots = 3;       // Head, Chest, Leggings
        private static int vanillaAccesorySlots = 7;    // 5 normal, 1 demon heart, 1 master

        public Item HeldItem => owner.HeldItem;
        private Item[] heldItem = new Item[1];
        private Item[] Armor = new Item[vanillaArmorSlots];
        private Item[] Accesories;
        Player owner;
        WEPlayer wePlayer;

        public PlayerEquipment(Player player) {
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
                foreach (EnchantmentEffect enchantmentEffects in enchantment.Effects) {
                    enchantmentEffects.EfficiencyMultiplier = enchantment.AllowedList[enchantedItem.ItemType];
                    effects.Add(enchantmentEffects);
                }
            }
        }
        public void SortEnchantmentEffects(ISortEnchantmentEffects entity) {
            IEnumerable<EnchantmentEffect> enchantmentEffects = entity.EnchantmentEffects;
            entity.PassiveEffects = enchantmentEffects.OfType<IPassiveEffect>();
            entity.StatEffects = enchantmentEffects.OfType<StatEffect>();
            entity.VanillaStats = GetStatEffectDictionary<IVanillaStat>(entity.StatEffects);
            entity.EnchantmentStats = GetStatEffectDictionary<INonVanillaStat>(entity.StatEffects);
            entity.OnHitDebuffs = ;
            entity.OnHitBuffs = ;
            entity.OnTickBuffs = ;
		}
        public SortedDictionary<byte, EStatModifier> GetStatEffectDictionary<T>(IEnumerable<StatEffect> statEffects) where T : IApplyStats {
            SortedDictionary<byte, EStatModifier> result = new SortedDictionary<byte, EStatModifier>();
            foreach (T statEffect in statEffects.OfType<T>()) {
                result.AdOrCombine(statEffect.EStatModifier);
            }
        }

        private IEnumerable<Item> GetAllArmor() {
            Item[] items = new Item[Armor.Length + Accesories.Length];
            Armor.CopyTo(items, 0);
            Accesories.CopyTo(items, Armor.Length);

            return items;
        }

        private IEnumerable<Item> GetAllItems() {
            Item[] items = new Item[1 + Armor.Length + Accesories.Length];
            items[0] = HeldItem;
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

        public void GetArmorEnchantmentEffects() {
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

                if (!ci.IsSameEnchantedItem(ci2))
                    return false;
            }

            return true;
        }
    }
}
