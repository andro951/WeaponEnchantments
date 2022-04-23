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
    public class EnchantingTable : Chest
    {
        new public const int maxItems = 1;
        public const int maxEnchantments = 5;
        public const int maxEssenceItems = 5;
        public const int maxTier = 4;
        public int tier = 0;
        public bool summonDemon;
        public int availableEnchantmentSlots;
        public bool[] canUseEnchantmentSlot = new bool[maxEnchantments];
        //public static int[] essenceType = new int[maxEssenceItems];
        new public Item[] item;
        public Item[] enchantmentItem;
        public Item[] essenceItem;
        //public Texture[] textures = new Texture[maxTier];
        //private Texture texture;
        public EnchantingTable(int Tier = 0)
        {
            item = new Item[maxItems];
            for(int i = 0; i < maxItems; i++)
            {
                item[i] = new Item();
            }
            enchantmentItem = new Item[maxEnchantments];
            for(int i = 0; i < maxEnchantments; i++)
            {
                enchantmentItem[i] = new Item();
            }
            essenceItem = new Item[maxEssenceItems];
            for(int i = 0; i < maxEssenceItems; i++)
            {
                essenceItem[i] = new Item();
            }
            tier = Tier;
            availableEnchantmentSlots = maxEnchantments - tier;
            for(int i = 0; i < availableEnchantmentSlots; i++)
            {
                canUseEnchantmentSlot[i] = true;
            }
            //texture = textures[Tier];
            if (tier == maxTier)
            {
                summonDemon = true;
            }
        }

        new public static void Initialize()
        {
            
        }
        public void Update()
        {
            //needs the 
            
        }
        public void Open()
        {
            availableEnchantmentSlots = maxEnchantments - tier;
            for (int i = 0; i < maxEnchantments; i++)
            {
                if (i < availableEnchantmentSlots)
                {
                    canUseEnchantmentSlot[i] = true;
                }
                else
                {
                    canUseEnchantmentSlot[i] = false;
                }
            }
            //texture = textures[Tier];//Should go in WeaponEnchantmentsUI not here
            if (tier == maxTier)
            {
                summonDemon = true;
            }


            /* Move to WeaponEnchantmentUI OnActivate(), replaced by TryPlacingInEnchantingTable
            Player player = Main.player[Main.myPlayer];
            autoCraftEssence();
            foreach(Item item in player.inventory) //Check how autostack is done in Player.cs
            {
                for (int i = 0; i <= maxEssenceItems; i++)
                {
                    if (item.type == EnchantmentEssence.IDs[i])
                    {
                        int maxTakeAmmount = EnchantmentEssence.maxStack - essenceItem[i].stack;
                        if (maxTakeAmmount < item.stack)
                        {
                            essenceItem[i].stack += item.stack;
                            item.TurnToAir();
                            //PlayStackSound
                        }
                    }
                }
            }
            autoCraftEssence();
            */
        }
        public void Close()
        {
            summonDemon = false;
        }
        private void autoCraftEssence()//Move to WeaponEnchantmentUI TryPlacingInEnchantingTable
        {
            if (tier == maxTier)
            {
                for (int i = 0; i < maxEssenceItems - 1; i++)
                {
                    while (essenceItem[i].stack > 4 && (essenceItem[i + 1].stack > essenceItem[i + 1].maxStack))
                    {
                        //calculate and set new numbers
                        //Play craft sound
                    }
                }
            }
        }
    }
}
