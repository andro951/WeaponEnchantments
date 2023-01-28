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
        public WeightedPair(DropData dropData) {
            ID = dropData.ID;
            Weight = dropData.Weight;
        }
        public float Weight;
        public int ID;
	}
	public struct DropData {
        public int ID;
        public float Weight;
        public float Chance;
        public DropData(int id, float weight = 1f, float chance = -1f) {
            ID = id;
            Weight = weight;
            Chance = chance;
        }
		public DropData(ChestID id, float weight = 1f, float chance = -1f) {
			ID = (int)id;
			Weight = weight;
			Chance = chance;
		}
		public DropData(CrateID id, float weight = 1f, float chance = -1f) {
			ID = (int)id;
			Weight = weight;
			Chance = chance;
		}
	}
}
