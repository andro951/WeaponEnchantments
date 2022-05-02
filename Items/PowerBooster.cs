using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace WeaponEnchantments.Items
{
    internal class PowerBooster : ModItem
    {
        public static int ID;
        public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 4));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            ItemID.Sets.ItemIconPulse[Item.type] = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.value = 500000;
            Item.width = 18;
            Item.height = 18;
            Tooltip.SetDefault("Use this on an item in an Enchantment Table to raise its base level by 10.\nThis item will be consumed and can not be returned.");
            Item.rare = ItemRarityID.Orange;
        }
        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.WhiteSmoke.ToVector3() * 0.55f * Main.essScale); // Makes this item glow when thrown out of inventory.
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
