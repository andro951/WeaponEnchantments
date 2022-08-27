using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponEnchantments.Common.Utility
{
    public struct WeightedPair
    {
        public WeightedPair(int i, float w) {
            Weight = w;
            ID = i;
        }
        public float Weight;
        public int ID;
    }
}
