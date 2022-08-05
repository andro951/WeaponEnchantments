using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponEnchantments.Effects {
    internal interface IPassiveEffect {
        /// <summary>
        /// <para>
        /// The part of the enchantment that runs every frame after all misc effects have triggered.
        /// </para>
        /// <para>
        /// This happens before health regeneration and gravity and such.
        /// </para>
        /// </summary>
        /// <param name="player">The player for which this enchantment applies</param>
        public void PostUpdateMiscEffects(WEPlayer player);
    }
}
