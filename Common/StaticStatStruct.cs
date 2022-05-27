using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponEnchantments.Common
{
    public struct StaticStatStruct
    {
        public string Name { get; private set; }
        public float Additive { get; private set; }
        public float Multiplicative { get; private set; }
        public StaticStatStruct(string name, float additive, float multiplicative)
        {
            Name = name;
            Additive = additive;
            Multiplicative = multiplicative;
        }

    }
}
