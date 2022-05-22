using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponEnchantments.Common
{
    public static class UtilityMethods
    {
        private static readonly char[] upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        public static bool IsUpper(this char c)
        {
            foreach (char upper in upperCase)
            {
                if (upper == c)
                    return true;
            }
            return false;
        }
        public static List<string> SplitString(this string s)
        {
            List<string> list = new List<string>();
            int start = 0;
            int end = 0;
            for (int i = 1; i < s.Length; i++)
            {
                if (s[i].IsUpper())
                {
                    end = i - 1;
                    list.Add(s.Substring(start, end - start + 1));
                    start = end + 1;
                }
                else if (i == s.Length - 1)
                {
                    end = i;
                    list.Add(s.Substring(start, end - start + 1));
                }
            }
            return list;
        }
        public static string RemoveProjectileName(this string s)
        {
            int i = s.IndexOf("ProjectileName.");
            return i == 0 ? s.Substring(15) : s;
        }
        public static int CheckMatches(this List<string> l1, List<string> l2)
        {
            int matches = 0;
            foreach(string s in l1)
            {
                foreach(string s2 in l2)
                {
                    if(s2.IndexOf(s) > -1)
                    {
                        matches++;
                    }
                }
            }
            return matches;
        }
    }
}
