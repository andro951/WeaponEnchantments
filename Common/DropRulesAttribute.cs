using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria.ModLoader;

namespace WeaponEnchantments.Common
{
    public class DropRulesAttribute : Attribute
    {
        /// <summary>
        /// All enchantment drops
        /// </summary>
        static IEnumerable<Type> typesThatContainADropRullAttribute = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsDefined(typeof(DropRulesAttribute)));

        public static Dictionary<int, ICollection<int>> npcTypeDrops = GetNPCTypeDropDict();
        public static Dictionary<int, ICollection<int>> npcAiStyleDrops = GetNPCAiStyleDropsDict();

        public int[] npcTypes;
        public int[] npcAiStyles;
        public DropRulesAttribute(int[] npcs = null, int[] AIs = null) {
            this.npcTypes = npcs;
            this.npcAiStyles = AIs;
        }

        /// <returns>The drops for any specific mob</returns>
        static Dictionary<int, ICollection<int>> GetNPCTypeDropDict() {
            MethodInfo methodInfo = typeof(ModContent).GetMethod("ItemType");
            var dict = new Dictionary<int, ICollection<int>>();
            foreach (var typeWithDropRuleAttribute in typesThatContainADropRullAttribute) {
                var method = methodInfo.MakeGenericMethod(typeWithDropRuleAttribute);

                DropRulesAttribute dropRulesAttribute = (DropRulesAttribute)GetCustomAttribute(typeWithDropRuleAttribute, typeof(DropRulesAttribute));
                int[] npcTypes = dropRulesAttribute.npcTypes;
                if (npcTypes != null) {
                    foreach (int npcType in npcTypes) {
                        if (!dict.ContainsKey(npcType))
                            dict.Add(npcType, new HashSet<int>());

                        int itemID = (int)method.Invoke(null, null);
                        dict[npcType].Add(itemID);
                    }
                }
            }

            return dict;
        }

        /// <returns>The drops for any specific AI</returns>
        static Dictionary<int, ICollection<int>> GetNPCAiStyleDropsDict() {
            MethodInfo methodInfo = typeof(ModContent).GetMethod("ItemType");
            var dict = new Dictionary<int, ICollection<int>>();
            foreach (var type in typesThatContainADropRullAttribute) {
                var method = methodInfo.MakeGenericMethod(type);

                int[] npcAiStyles = ((DropRulesAttribute)GetCustomAttribute(type, typeof(DropRulesAttribute))).npcAiStyles;
                if (npcAiStyles != null) {
                    foreach (int npcAiStyle in npcAiStyles) {
                        if (!dict.ContainsKey(npcAiStyle))
                            dict.Add(npcAiStyle, new HashSet<int>());

                        int itemID = (int)method.Invoke(null, null);
                        dict[npcAiStyle].Add(itemID);
                    }
                }
            }

            return dict;
        }
    }
}
