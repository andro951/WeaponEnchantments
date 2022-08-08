using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace WeaponEnchantments.Effects {
    internal interface ICanAutoReuseItem {

        /// <summary>
        /// <para>
        /// Returns whether or not the player can autoReuse certain items
        /// </para>
        /// <para>
        /// Stats here are not by reference and modifying them will have no consequence.
        /// </para>
        /// </summary>
        /// <param name="item">The item to be checked</param>
        /// <returns>true if can be autoreused, false if cannot be autoreused, null to default to base</returns>
        public bool? CanAutoReuseItem(Item item);
    }
}
