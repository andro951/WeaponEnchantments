using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WeaponEnchantments.Items
{
    public class Stabilizer : ModItem
    {
        public static string[] sizes = new string[] { "", "Superior" };
        public static int[,] ingredientTypes = {{177, 178, 179, 180, 181}, {182, 999, -1, -1, -1}}; 
        public int size = 0;
        public static int[] IDs = new int[sizes.Length];
        public static int[] Values = new int[sizes.Length];
        public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
        public override void SetStaticDefaults()
        {
            for(int i = 0; i < sizes.Length; i++)
            {
                Values[i] = (1 + i) * 375;
            }
            if (size == 0)
            {
                Tooltip.SetDefault("Used to create Superior Enchantment Containments");
            }
            else
            {
                Tooltip.SetDefault("Used to create Ultra Rare Enchantments");
            }
        }
        public override void SetDefaults()
        {
            Item.value = Values[size];
            Item.width = 8;
            Item.height = 8;
            Item.maxStack = 1000;
        }
        public override void AddRecipes()
        {
            for (int i = 0; i < ingredientTypes.Length / 2; i++)
            {
                if (ingredientTypes[size, i] != -1)
                {
                    Recipe recipie;
                    if (size == 0)
                    {
                        recipie = CreateRecipe();
                        recipie.AddTile(TileID.Hellforge);
                        recipie.AddIngredient(ingredientTypes[size, i], 1);
                        recipie.Register();
                        recipie = CreateRecipe();
                        recipie.AddTile(TileID.AdamantiteForge);
                    }
                    else
                    {
                        recipie = CreateRecipe(4);
                        recipie.AddTile(TileID.AdamantiteForge);
                    }
                    recipie.AddIngredient(ingredientTypes[size, i], 1);
                    recipie.Register();
                }
            }
            IDs[size] = Item.type;
        }
        public class SuperiorStabilizer : Stabilizer
        {
            public SuperiorStabilizer() { size = 1; }
        }
    }
    public class ContainmentFragment : ModItem
    {
        public static int ID;//Make drop from bosses and ofering
        public static int value = 10000;
        public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Used to create Enchantment Containments");
        }
        public override void SetDefaults()
        {
            Item.width = 10;
            Item.height = 10;
            Item.maxStack = 1000;
            Item.value = value;
        }
        public override void AddRecipes()
        {
            for (int i = 0; i < Containment.sizes.Length; i++)
            {
                Recipe recipie = CreateRecipe(Containment.fragments[i]);
                recipie.AddTile(TileID.WorkBenches);
                recipie.AddIngredient(Mod, Containment.sizes[i] + "Containment", 1);
                recipie.Register();
            }
            ID = Item.type;
        }
    }
    public class Containment : ModItem
    {
        public static string[] sizes = new string[] { "", "Medium", "Superior" };
        public static int[] glass = new int[] { 1, 4, 0};
        public static int[] fragments = new int[] { 4, 8, 16 };
        public static int[] IDs = new int[sizes.Length];
        public static int[] Values = new int[sizes.Length];
        public int size = 0;
        public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
        public override void SetStaticDefaults()
        {
            for(int i = 0; i < sizes.Length; i++)
            {
                Values[i] = fragments[size] * ContainmentFragment.value;
                if (i == 2)
                {
                    Values[i] += Stabilizer.Values[0];
                }
            }
            Tooltip.SetDefault("Used to store " + Enchantments.rarity[size] + " enchantments");
        }
        public override void SetDefaults()
        {
            Item.maxStack = 1000;
            Item.value = fragments[size] * ContainmentFragment.value;
            //Item.value = fragments[size] * ModContent.GetModItem(ModContent.ItemType<ContainmentFragment>()).Item.value;

            if (size < 2)
            {
                Item.width = 10 + 4 * (size);
                Item.height = 10 + 4 * (size);
            }
            else
            {
                Item.value += 4 * ModContent.GetModItem(ModContent.ItemType<Stabilizer>()).Item.value;
                Item.width = 40;
                Item.height = 40;
            }
        }
        public override void AddRecipes()
        {
            Recipe recipie = CreateRecipe();
            recipie.AddTile(TileID.WorkBenches);
            if (size > 0)
            {
                //recipie.AddIngredient(Mod, sizes[size - 1] + "Containment", 1);
            }
            if(size == 2)
            {
                recipie.AddIngredient(ModContent.ItemType<Stabilizer>(), 4);
            }
            else
            {
                recipie.AddIngredient(ItemID.Glass, glass[size]);
            }
            recipie.AddIngredient(ModContent.ItemType<ContainmentFragment>(), fragments[size]);
            //recipie.AddIngredient(ContainmentFragment.ID, fragments[size]);
            recipie.Register();
            IDs[size] = Item.type;
        }
    }
    public class MediumContainment : Containment
    {
        public MediumContainment() { size = 1; }
    }
    public class SuperiorContainment : Containment
    {
        public SuperiorContainment() { size = 2; }
    }
}
