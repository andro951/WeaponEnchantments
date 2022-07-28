using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public static string reporteMessage = "\nPlease report this to andro951(Weapon Enchantments) allong with a description of what you were doing at the time.";
        public static void LogNT(this string s) {
            s += reporteMessage;

            if (Main.netMode < NetmodeID.Server)
                Main.NewText(s);

            s.Log();
        }
        public static void Log(this string s) {
            UpdateSpaces(s);
            ModContent.GetInstance<WEMod>().Logger.Info(s.AddWS());
            UpdateSpaces(s, true);
        }
        public static void LogT(this string s) {
            UpdateSpaces(s);
            foreach (string key in logsT.Keys) {
                if (logsT[key] + 59 < Main.GameUpdateCount)
                    logsT.Remove(key);
            }
            if (!logsT.ContainsKey(s)) {
                ModContent.GetInstance<WEMod>().Logger.Info(s.AddWS());
                logsT.Add(s, Main.GameUpdateCount);
            }
            UpdateSpaces(s, true);
        }
        public static void UpdateSpaces(string s, bool atEnd = false) {
            if (atEnd && s.Substring(0, 2) == "\\/")
                spaces++;
            else if (!atEnd && s.Substring(0, 2) == "/\\")
                spaces--;
        }
        public static string AddWS(this string s) => new string('|', spaces) + s;
    }
}
