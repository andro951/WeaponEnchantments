using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace WeaponEnchantments.Items
{
    internal class PowerBooster : ModItem
    {
        public static int ID;
        public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
        public override void SetDefaults()
        {
            Item.value = 100000;
            Item.width = 20;
            Item.height = 20;
            Tooltip.SetDefault("Use this on an item in an Enchantment Table to raise its base level by 10.");
        }
        public override void AddRecipes()
        {
            if (Enchantments.cheating)
            {
                Recipe recipie = CreateRecipe();
                recipie.Register();
                ID = Item.type;
            }
        }
    }
}
