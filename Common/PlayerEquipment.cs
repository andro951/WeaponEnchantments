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

        public PlayerEquipment(Player player) {
            ModAccessorySlotPlayer alp = player.GetModPlayer<ModAccessorySlotPlayer>();
            AccessorySlotLoader loader = LoaderManager.Get<AccessorySlotLoader>();

            int moddedSlotCount = alp.SlotCount;

            HeldItem = player.HeldItem;
            Accesories = new Item[vanillaAccesorySlots + moddedSlotCount]; 

            for(int i = 0; i < vanillaArmorSlots; i++) {        // Set all (vanilla) armor slots
                Armor[i] = player.armor[i];
            }

            for (int i = 0; i < vanillaAccesorySlots; i++) {    // Set all vanilla accesory slots
                Accesories[i] = player.armor[i + 3];
            }

            for (int i = 0; i < moddedSlotCount; i++) {         // Set all modded accesory slots (cheatsheet does what it wants)
                var slot = loader.Get(i, player);
                if (slot.IsEnabled() && !slot.IsEmpty) {
                    Accesories[vanillaAccesorySlots + i] = slot.FunctionalItem;
                }
            }
        }

        public static IEnumerable<EnchantedItem> FilterEnchantedItems(IEnumerable<Item> items) {
            IEnumerable<EnchantedItem> enchantedItems = items
                .Where(i => i != null)
                .Select(i => i.GetEnchantedItem())
                .Where(i => i != null);
            return enchantedItems;
        }

        public static IEnumerable<EnchantmentEffect> ExtractEnchantmentEffects(EnchantedItem enchantedItem) {
            return ExtractEnchantmentEffects(new EnchantedItem[] { enchantedItem });
        }
        
        public static IEnumerable<EnchantmentEffect> ExtractEnchantmentEffects(IEnumerable<EnchantedItem> enchantedItems) {
            List<EnchantmentEffect> effects = new List<EnchantmentEffect>();

            // Get all non null enchanted items
            foreach (EnchantedItem enchantedItem in enchantedItems) {
                // For each enchanted item, get its enchantments
                IEnumerable<Enchantment> enchantments = enchantedItem.slottedItems
                    .Where(i => i != null && i.ModItem is Enchantment)
                    .Select(i => (Enchantment)i.ModItem);
                // For each enchantment get its effects
                foreach (Enchantment enchantment in enchantments) {
                    foreach (EnchantmentEffect effect in enchantment.Effects) {
                        effect.EquipmentEfficiency = enchantment.AllowedList[enchantedItem.GetEItemType()];
                        effects.Add(effect);
                    }
                }
            }
            return effects;
        }

        private Item HeldItem;
        private Item[] Armor = new Item[vanillaArmorSlots];
        private Item[] Accesories; 

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

        private IEnumerable<EnchantedItem> GetEnchantedItems() {
            return FilterEnchantedItems(GetAllItems());
        }

        private IEnumerable<EnchantedItem> GetEnchantedArmor() {
            return FilterEnchantedItems(GetAllArmor());
        }

        public IEnumerable<EnchantmentEffect> GetAllEnchantmentEffects() {
            return ExtractEnchantmentEffects(GetEnchantedItems());
        }

        public IEnumerable<EnchantmentEffect> GetArmorEnchantmentEffects() {
            return ExtractEnchantmentEffects(GetEnchantedArmor());
        }
    }
}
