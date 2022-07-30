using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WeaponEnchantments.Common.Utility
{
	public static class LogMethods
    {
        public readonly static bool debugging = false;
        private static int spaces = 0;
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
        /// Adds 
        /// </summary>
        /// <param name="s">Message that will be printed</param>
        public static void Log(this string s) {
            s.AddCharToFront();
            ModContent.GetInstance<WEMod>().Logger.Info(s.AddWS());
            s.AddCharToFront(true);
        }

        /// <summary>
        /// Prints a message to the .log file.
        /// </summary>
        /// <param name="s">Message that will be printed</param>
        public static void LogT(this string s) {
            AddCharToFront(s);
            foreach (string key in logsT.Keys) {
                if (logsT[key] + 59 < Main.GameUpdateCount)
                    logsT.Remove(key);
            }
            if (!logsT.ContainsKey(s)) {
                ModContent.GetInstance<WEMod>().Logger.Info(s.AddWS());
                logsT.Add(s, Main.GameUpdateCount);
            }
            AddCharToFront(s, true);
        }
        public static void AddCharToFront(this string s, bool afterString = false) {
            if (afterString && s.Substring(0, 2) == "\\/")
                spaces++;
            else if (!afterString && s.Substring(0, 2) == "/\\")
                spaces--;
        }
        public static string AddWS(this string s) => new string('|', spaces) + s;
    }
}
