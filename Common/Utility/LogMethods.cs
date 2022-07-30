using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WeaponEnchantments.Common.Utility
{
	public static class LogMethods
    {
        public readonly static bool debugging = false;
        private static int charNum = 0;
        private static Dictionary<string, double> logsT = new Dictionary<string, double>();
        public static string reportMessage = "\nPlease report this to andro951(Weapon Enchantments) allong with a description of what you were doing at the time.";

        /// <summary>
        /// Prints a message in game and the .log file.<br/>
        /// Adds reportMessage to the end:<br/>
        /// Please report this to andro951(Weapon Enchantments) allong with a description of what you were doing at the time.<br/>
        /// </summary>
        /// <param name="s">Message that will be printed</param>
        public static void LogNT(this string s) {
            s += reportMessage;

            if (Main.netMode < NetmodeID.Server)
                Main.NewText(s);

            s.Log();
        }

        /// <summary>
        /// Prints a message to the .log file.
        /// If you want to use Log() for following execution of methods, put "\\/" as the first characters in a message at the start of a method 
        /// and "/\\" as the first characters in a message at the end of a method.
        /// This will add a "|" to Log messages made within the method to help visually follow the exicution in the .log file.
        /// </summary>
        /// <param name="s">Message that will be printed</param>
        public static void Log(this string s) {
            s.UpdateCharNum();
            ModContent.GetInstance<WEMod>().Logger.Info(s.AddCharToFront());
            s.UpdateCharNum(true);
        }

        /// <summary>
        /// Prints a message to the .log file.
        /// Will not print the exact same string more than once per second. (Good for logging methods that get called every tick)
        /// If you want to use Log() for following execution of methods, put "\\/" as the first characters in a message at the start of a method 
        /// and "/\\" as the first characters in a message at the end of a method.
        /// This will add a "|" to Log messages made within the method to help visually follow the exicution in the .log file.
        /// </summary>
        /// <param name="s">Message that will be printed</param>
        public static void LogT(this string s) {
            //Try to remove any messages that were called 60 ticks or more before now.
            foreach (string key in logsT.Keys) {
                if (logsT[key] + 60 <= Main.GameUpdateCount)
                    logsT.Remove(key);
            }

            if (!logsT.ContainsKey(s)) {
                s.Log();
                logsT.Add(s, Main.GameUpdateCount);
            }
        }
        private static void UpdateCharNum(this string s, bool afterString = false) {
            if (afterString && s.Substring(0, 2) == "\\/") {
                charNum++;
            }
            else if (!afterString && s.Substring(0, 2) == "/\\") {
                charNum--;
            }
        }
        private static string AddCharToFront(this string s, char c = '|') => new string(c, charNum) + s;
    }
}
