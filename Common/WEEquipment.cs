using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.EnchantmentEffects;
using WeaponEnchantments.Items;

namespace WeaponEnchantments.Common {
    public class PlayerEquipment {
        private static int relevantArmorSlots = 10;

        public PlayerEquipment(Player player) {
            HeldItem = player.HeldItem;
            for (int i = 0; i < relevantArmorSlots; i++) {
                Item item = player.armor[i];
                if (i < 3) {
                    Armor[i] = item;
                }
                else {
                    Accesories[i - 3] = item;
                }
            }
        }

        public static IEnumerable<EnchantedItem> FilterEnchantedItems(IEnumerable<Item> items) {
            IEnumerable<EnchantedItem> enchantedItems = items
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
                    effects.AddRange(enchantment.Effects);
                }
            }
            return effects;
        }

        private Item HeldItem;
        private Item[] Armor = new Item[3];
        private Item[] Accesories = new Item[7]; // 5 normal, 1 demon heart, 1 master

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
