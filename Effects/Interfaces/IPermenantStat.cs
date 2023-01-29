using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace WeaponEnchantments.Effects {
    public interface IPermenantStat
    {
        public void Update(ref Item item, bool reset = false);
        public void ApplyTo(ref Item item);
        public void Reset(ref Item item);
    }
}
