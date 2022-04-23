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
    internal class ModItemID
    {
        public static List<(string, int)> IDs = new List<(string, int)>();
        public static void Add(string name, int ID)
        {
            (string, int) T = (name, ID);
            IDs.Add(T);
        }
        public static bool GetName(ref string name, int ID)
        {
            for(int i = 0; i < IDs.Count; i++)
            {
                if(IDs[i].Item2 == ID)
                {
                    name = IDs[i].Item1;
                    return true;
                }
            }
            return false;
        }
        public static bool GetID(string name, ref int ID)
        {
            for (int i = 0; i < IDs.Count; i++)
            {
                if (IDs[i].Item1 == name)
                {
                    ID = IDs[i].Item2;
                    return true;
                }
            }
            return false;
        }
    }
}   
