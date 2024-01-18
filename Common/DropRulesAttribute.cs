using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;

namespace WeaponEnchantments.Common
{
    public class DropRulesAttribute : Attribute {
        /// <summary>
        /// All enchantment drops
        /// </summary>
        private static IEnumerable<Type> _typesThatContainADropRuleAttribute = null;
        public static IEnumerable<Type> typesThatContainADropRuleAttribute {
            get {
                if (_typesThatContainADropRuleAttribute == null) {
                    IEnumerable<Type> types = null;
                    try {
                        types = Assembly.GetExecutingAssembly().GetTypes();
                    } catch(ReflectionTypeLoadException e) {
                        types = e.Types.Where(t => t != null);
                    }
                    _typesThatContainADropRuleAttribute = types.Where(t => t.IsDefined(typeof(DropRulesAttribute)));
                }
                return _typesThatContainADropRuleAttribute;
            } set {
                _typesThatContainADropRuleAttribute = value;
            }
        }

        public static Dictionary<int, ICollection<WeightedPair>> npcTypeDrops = GetNPCTypeDropDict();
        public static Dictionary<int, ICollection<WeightedPair>> npcAiStyleDrops = GetNPCAiStyleDropsDict();

        public (float, int)[] npcTypes;
        public (float, int)[] npcAiStyles;
        public DropRulesAttribute((float, int)[] npcs = null, (float, int)[] AIs = null) {
            npcTypes = npcs;
            npcAiStyles = AIs;
        }

        /// <returns>The drops for any specific mob</returns>
        static Dictionary<int, ICollection<WeightedPair>> GetNPCTypeDropDict() {
            MethodInfo methodInfo = typeof(ModContent).GetMethod("ItemType");
            var dict = new Dictionary<int, ICollection<WeightedPair>>();
            foreach (var typeWithDropRuleAttribute in typesThatContainADropRuleAttribute) {
                var method = methodInfo.MakeGenericMethod(typeWithDropRuleAttribute);

                DropRulesAttribute dropRulesAttribute = (DropRulesAttribute)GetCustomAttribute(typeWithDropRuleAttribute, typeof(DropRulesAttribute));
                (float, int)[] npcTypes = dropRulesAttribute.npcTypes;
                if (npcTypes != null) {
                    foreach ((float, int) pair in npcTypes) {
                        if (!dict.ContainsKey(pair.Item2))
                            dict.Add(pair.Item2, new HashSet<WeightedPair>());

                        int itemID = (int)method.Invoke(null, null);
                        dict[pair.Item2].Add(new WeightedPair(itemID, pair.Item1));
                    }
                }
            }

            return dict;
        }

        /// <returns>The drops for any specific AI</returns>
        static Dictionary<int, ICollection<WeightedPair>> GetNPCAiStyleDropsDict() {
            MethodInfo methodInfo = typeof(ModContent).GetMethod("ItemType");
            var dict = new Dictionary<int, ICollection<WeightedPair>>();
            foreach (var type in typesThatContainADropRuleAttribute) {
                var method = methodInfo.MakeGenericMethod(type);

                (float, int)[] npcAiStyles = ((DropRulesAttribute)GetCustomAttribute(type, typeof(DropRulesAttribute))).npcAiStyles;
                if (npcAiStyles != null) {
                    foreach ((float, int) pair in npcAiStyles) {
                        if (!dict.ContainsKey(pair.Item2))
                            dict.Add(pair.Item2, new HashSet<WeightedPair>());

                        int itemID = (int)method.Invoke(null, null);
                        dict[pair.Item2].Add(new WeightedPair(itemID, pair.Item1));
                    }
                }
            }

            return dict;
        }
    }
}
