using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using WeaponEnchantments.Items;

namespace WeaponEnchantments
{
    public class EnchantingTable
    {
        public const int maxItems = 1;//Number of itemSlots in enchantingTable
        public const int maxEnchantments = 5;//Number of enchantmentSlots in enchantingTable
        public const int maxEssenceItems = 5;//Number of essenceSlots in enchantingTable
        public const int maxTier = 4;//Number of enchantingTable tiers
        public int tier = 0;//Tier of the current enchantingTable being used by the player
        public bool summonDemon;//Used to determine if the demon shopkeeper NPC should be summoned in the enchanting table (tier = maxTier enchanting table)
        //public static int[] essenceType = new int[maxEssenceItems];
        public List<Item> item;//Stores item(s) when enchanting table UI is closed
        public List<Item> enchantmentItem;//Stores enchantments when enchanting table UI is closed
        public List<Item> essenceItem;//Stores essence when enchanting table UI is closed
        //public Texture[] textures = new Texture[maxTier];
        //private Texture texture;
        public EnchantingTable(int Tier = 0)
        {
            item = new List<Item>(new Item[maxItems]);
            for(int i = 0; i < maxItems; i++)
            {
                item[i] = new Item();
            }//setup items
            enchantmentItem = new List<Item>(new Item[maxEnchantments]);
            for(int i = 0; i < maxEnchantments; i++)
            {
                enchantmentItem[i] = new Item();
            }//setup enchantments
            essenceItem = new List<Item>(new Item[maxEssenceItems]);
            for(int i = 0; i < maxEssenceItems; i++)
            {
                essenceItem[i] = new Item();
            }//setup essence
            tier = Tier;
        }//Constructor
        public void Open()
        {
            if (tier == maxTier)
            {
                summonDemon = true;
            }
        }
        public void Close()
        {
            summonDemon = false;
        }
    }
}
