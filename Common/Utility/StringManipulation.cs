using Microsoft.CodeAnalysis.CSharp.Syntax;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Configs;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Items;
using androLib.Common.Globals;
using androLib.Common.Utility;

namespace WeaponEnchantments.Common.Utility
{
	public static class StringManipulation
    {
        #region S() Methods
        
        /// <summary>
        /// Convert to a string
        /// </summary>
        public static string S(this Enchantment enchantment) => enchantment != null ? enchantment.Name : "null";
        
		#endregion
        
	}
}
