using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using WeaponEnchantments.Items;

namespace WeaponEnchantments.Common.Utility
{
	public static class StringManipulation
    {
        private static readonly char[] upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        private static readonly char[] lowerCase = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
        private static readonly char[] numbers = "0123456789".ToCharArray();
        private static readonly string[] apla = { "abcdefghijklmnopqrstuvwxyz", "ABCDEFGHIJKLMNOPQRSTUVWKYZ" };

        #region S() Methods

        /// <summary>
        /// Convert to a string
        /// </summary>
        public static string S(this StatModifier statModifier) => "<A: " + statModifier.Additive + ", M: " + statModifier.Multiplicative + ", B: " + statModifier.Base + ", F: " + statModifier.Flat + ">";

        /// <summary>
        /// Convert to a string
        /// </summary>
        public static string S(this EStat eStat) => "<N: " + eStat.StatName + " A: " + eStat.Additive + ", M: " + eStat.Multiplicative + ", B: " + eStat.Base + ", F: " + eStat.Flat + ">";

        /// <summary>
        /// Convert to a string
        /// </summary>
        public static string S(this EnchantmentStaticStat staticStat) => "<N: " + staticStat.Name + " A: " + staticStat.Additive + ", M: " + staticStat.Multiplicative + ", B: " + staticStat.Base + ", F: " + staticStat.Flat + ">";

        /// <summary>
        /// Convert to a string
        /// </summary>
        public static string S(this Item item) => item != null ? !item.IsAir ? item.Name : "<Air>" : "null";

        /// <summary>
        /// Convert to a string
        /// </summary>
        public static string S(this Projectile projectile) => projectile != null ? projectile.Name : "null";

        /// <summary>
        /// Convert to a string
        /// </summary>
        public static string S(this Player player) => player != null ? player.name : "null";

        /// <summary>
        /// Convert to a string
        /// </summary>
        public static string S(this NPC npc, bool stats = false) => npc != null ? $"name: {npc.FullName} whoAmI: {npc.whoAmI}{(stats ? $"defense: {npc.defense}, defDefense: {npc.defDefense}, lifeMax: {npc.lifeMax}, life: {npc.life}" : "")}" : "null";

        /// <summary>
        /// Convert to a string
        /// </summary>
        public static string S(this Enchantment enchantment) => enchantment != null ? enchantment.Name : "null";

        /// <summary>
        /// Convert to a string
        /// </summary>
        public static string S(this Dictionary<int, int> dictionary, int key) => "contains " + key + ": " + dictionary.ContainsKey(key) + " count: " + dictionary.Count + (dictionary.ContainsKey(key) ? " value: " + dictionary[key] : "");

        /// <summary>
        /// Convert to a string
        /// </summary>
        public static string S(this Dictionary<string, StatModifier> dictionary, string key) => "contains " + key + ": " + dictionary.ContainsKey(key) + " count: " + dictionary.Count + (dictionary.ContainsKey(key) ? " value: " + dictionary[key].S() : "");

        /// <summary>
        /// Convert to a string
        /// </summary>
        public static string S(this Dictionary<string, EStat> dictionary, string key) => "contains " + key + ": " + dictionary.ContainsKey(key) + " count: " + dictionary.Count + (dictionary.ContainsKey(key) ? " value: " + dictionary[key].S() : "");

        /// <summary>
        /// Convert to a string
        /// </summary>
        public static string S(this bool b) => b ? "True" : "False";

        #endregion

        public static bool IsUpper(this char c) {
            foreach (char upper in upperCase) {
                if (upper == c)
                    return true;
            }

            return false;
        }
        public static bool IsLower(this char c) {
            foreach (char lower in lowerCase) {
                if (lower == c)
                    return true;
            }

            return false;
        }
        public static bool IsNumber(this char c) {
            foreach (char number in numbers) {
                if (number == c)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Create a list of words from a string, splitting them when encountering capital letters<br/>
        /// </summary>
        /// <param name="s"></param>
        /// <returns>list of words</returns>
        public static List<string> SplitString(this string s) {
            List<string> list = new List<string>();
            int start = 0;
            int end;
            for (int i = 1; i < s.Length; i++) {
                if (s[i].IsUpper()) {
                    end = i - 1;
                    list.Add(s.Substring(start, end - start + 1));
                    start = end + 1;
                }
                else if (i == s.Length - 1) {
                    end = i;
                    list.Add(s.Substring(start, end - start + 1));
                }
            }

            return list;
        }

        /// <summary>
        /// Gets a folder name from a string, s.
        /// </summary>
        /// <param name="s">File path</param>
        /// <param name="numberOfFolders">How many times to find the searchChar and remove everything before it.  1 will find the folder of the file.</param>
        /// <param name="searchChar">The character that seperates the folders and files in the path.  Default '.'</param>
        /// <returns>New file path after removing the file name (and folders if numberOfFolders > 1)</returns>
        public static string GetFolderName(this string s, int numberOfFolders = 1, char searchChar = '.') {
            int i = s.Length - 1;
            for (int j = 0; j < numberOfFolders; j++) {
                i = s.FindChar(searchChar, false);

                //Not last time loop will run
                if (j != numberOfFolders - 1) {
                    //Remove last folder from the string and continue the loop
                    s = s.Substring(0, i);
                }
            }

            return s.Substring(i + 1);
        }
        public static string GetFileName(this string s, char searchChar = '.') {
            int i = s.FindChar(searchChar, false);

            return s.Substring(i + 1);
        }
        public static int FindChar(this string s, char searchChar, bool startLeft = true) {
            int length = s.Length;
            int i = startLeft ? 0 : length - 1;
            if (startLeft) {
                for (; i < length; i++) {
                    char c = s[i];
                    if (c == searchChar) {
                        return i;
                    }
                }
            }
            else {
                for (; i >= 0; i--) {
                    char c = s[i];
                    if (c == searchChar) {
                        return i;
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// Used only to first part of a file path.<br/>
        /// Example: WeaponEnchantments.Common.Utility -> Common.Utility
        /// </summary>
        /// <param name="s">File path or namespace</param>
        /// <param name="searchChar">Charachter that seperates the folders and file.  Default '.'</param>
        /// <param name="removeSearchChar">Wether to remove the sperating char.  .Common.Utility vs Common.Utility</param>
        /// <returns>New file path after removing the first part of the file path.</returns>
        public static string RemoveFirstFolder(this string s, char searchChar = '.', bool removeSearchChar = true) {
            int i = s.FindChar(searchChar);

            if (removeSearchChar)
                i++;

            return s.Substring(i);
        }

        /// <summary>
        /// Add spaces before capitals and numbers.<br/>
        /// (multiple capatials or numbers in a row will split only the last one.  It assumes there is an abriviation.)<br/>
        /// </summary>
        /// <param name="s"></param>
        /// <returns>String with spaces added.</returns>
        public static string AddSpaces(this string s) {
            int start = 0;
            int end;
            string finalString = "";
            for (int i = 1; i < s.Length; i++) {
                char c = s[i];
                char cm1 = s[i - 1];
                if (c.IsUpper() || c.IsNumber()) {
                    if (cm1.IsUpper()) {
                        int j = 0;
                        while (i + j < s.Length - 1 && s[i + j].IsUpper()) {
                            j++;
                        }
                        i += j - 1;
                    }
                    else if (cm1.IsNumber()) {
                        int j = 0;
                        while (i + j < s.Length - 1 && s[i + j].IsNumber()) {
                            j++;
                        }
                        i += j - 1;
                    }

                    end = i - 1;
                    finalString += s.Substring(start, end - start + 1) + " ";
                    start = end + 1;
                }
                else if (i == s.Length - 1) {
                    end = i;
                    finalString += s.Substring(start, end - start + 1);
                    start = -1;
                }
            }
            if (start != -1)
                finalString += s.Substring(start);

            return finalString;
        }

        /// <summary>
        /// Remove all spaces from a string.
        /// </summary>
        public static string RemoveSpaces(this string s) {
            bool started = false;
            int start = 0;
            int end;
            string finalString = "";
            for (int i = 0; i < s.Length; i++) {
                char c = s[i];
                if (started) {
                    if (c == ' ') {
                        started = false;
                        end = i;
                        finalString += s.Substring(start, end - start);
                    }
                }
                else {
                    if (c != ' ') {
                        started = true;
                        start = i;
                    }
                }
            }
            if (started)
                finalString += s.Substring(start, s.Length - start);

            return finalString;
        }

        /// <summary>
        /// Capitalize the first character in a string.
        /// </summary>
        public static string CapitalizeFirst(this string s) {
            if (s.Length > 0) {
                if (s[0].IsLower())
                    for (int i = 0; i < apla[0].Length; i++) {
                        if (s[0] == apla[0][i]) {
                            char c = apla[1][i];
                            return c + s.Substring(1);
                        }
                    }
            }
            return s;
        }

        /// <summary>
        /// Set the first character in the string to lowercase.
        /// </summary>
        public static string ToFieldName(this string s) {
            if (s.Length > 0) {
                if (s[0].IsUpper())
                    for (int i = 0; i < apla[0].Length; i++) {
                        if (s[0] == apla[1][i]) {
                            char c = apla[0][i];
                            return c + s.Substring(1);
                        }
                    }
            }

            return s;
        }

        /// <summary>
        /// Removes "ProjectileName." from the start of the string.
        /// </summary>
        public static string RemoveProjectileName(this string s) {
            int i = s.IndexOf("ProjectileName.");
            return i == 0 ? s.Substring(15) : s;
        }

        /// <summary>
        /// Counts the number of list L2 strings that are contained in L1 strings.
        /// </summary>
        /// <returns>Number of matches.</returns>
        public static int CheckMatches(this List<string> l1, List<string> l2) {
            int matches = 0;
            foreach (string s in l1) {
                foreach (string s2 in l2) {
                    if (s2.IndexOf(s) > -1) {
                        matches++;
                    }
                }
            }

            return matches;
        }
    }
}
