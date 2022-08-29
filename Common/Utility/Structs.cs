using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponEnchantments.Common.Utility
{
    public struct WeightedPair
    {
        public WeightedPair(int i, float w = 1f) {
            ID = i;
            Weight = w;
        }
        public WeightedPair(CrateID id, float w = 1f) {
            ID = (int)id;
            Weight = w; 
		}
        public float Weight;
        public int ID;
    }
}
