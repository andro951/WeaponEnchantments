using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace WeaponEnchantments.Common
{
    public class EnchantmentDropsAttribute : Attribute
    {
        static IEnumerable<Type> types = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsDefined(typeof(EnchantmentDropsAttribute))); // Get all enchantment drops
        
        static Dictionary<int, ICollection<int>> GetMobDropDict() // Returns the drops for any specific mob
        {
            MethodInfo methodInfo = typeof(ModContent).GetMethod("ItemType");
            var dict = new Dictionary<int, ICollection<int>>();
            foreach (var type in types)
            {
                var method = methodInfo.MakeGenericMethod(type);

                int[] validNPCS = ((EnchantmentDropsAttribute)GetCustomAttribute(type, typeof(EnchantmentDropsAttribute))).validNPCs;
                foreach (int validNPC in validNPCS)
                {
                    if (!dict.ContainsKey(validNPC))
                        dict.Add(validNPC, new HashSet<int>());
                    int itemID = (int)method.Invoke(null, null);
                    dict[validNPC].Add(itemID);
                }
            }
            return dict;
        }

        public static Dictionary<int, ICollection<int>> mobDrops = GetMobDropDict();

        public int[] validNPCs;

        public EnchantmentDropsAttribute(int[] npcs)
        {
            this.validNPCs = npcs;
        }
    }
}
