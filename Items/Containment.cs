using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WeaponEnchantments.Items
{
    /*public class Stabilizer : ModItem
    {
        public static string[] sizes = new string[] { "", "Superior" };
        public static int[,] ingredientTypes = { { 177, 178, 179, 180, 181 }, { 182, 999, -1, -1, -1 } };
        public int size = 0;
        public static int[] IDs = new int[sizes.Length];
        public static int[] Values = new int[sizes.Length];
        public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
        public override void SetStaticDefaults()
        {
            GetDefaults();
            for (int i = 0; i < sizes.Length; i++)
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
        private void GetDefaults()
        {
            for (int i = 0; i < sizes.Length; i++)
            {
                if(Name.IndexOf("Stabilizer") == 0)
                {
                    size = 0;
                }
                else
                {
                    if (sizes[i] == Name.Substring(Name.IndexOf("Stabilizer") + 10))
                    {
                        size = i;
                        break;
                    }
                }
            }
        }
        public override void SetDefaults()
        {
            GetDefaults();
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
    }
    public class SuperiorStabilizer : Stabilizer { }*/
    public class Containment : ModItem
    {
        public static string[] sizes = new string[] { "", "Medium", "Superior" };
        public static int[] glass = new int[] { 1, 4, 0};
        public static int[] bars = new int[] { 4, 8, 16 };
        public static int[,] barIDs = new int[,] { { ItemID.SilverBar, ItemID.GoldBar, ItemID.DemoniteBar }, { ItemID.TungstenBar, ItemID.PlatinumBar, ItemID.CrimtaneBar } };
        public static int[] IDs = new int[sizes.Length];
        public static int[] Values = new int[sizes.Length];
        
        public int size = 0;
        public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
        public override void SetStaticDefaults()
        {
            GetDefaults();
            //for (int i = 0; i < sizes.Length; i++)
            {
                Values[size] = bars[size] * ContentSamples.ItemsByType[barIDs[0, size]].value;
                if (size == 2)
                {
                    Values[size] += ContentSamples.ItemsByType[177].value * 4;
                }
            }
            Tooltip.SetDefault("Used to store " + AllForOneEnchantmentBasic.rarity[size] + " enchantments");
        }
        private void GetDefaults()
        {
            for (int i = 0; i < sizes.Length; i++)
            {
                if (Name.IndexOf("Containment") == 0)
                {
                    size = 0;
                }
                else
                {
                    if (sizes[i] == Name.Substring(Name.IndexOf("Containment") + 11))
                    {
                        size = i;
                        break;
                    }
                }
            }
        }
        public override void SetDefaults()
        {
            GetDefaults();
            Item.maxStack = 1000;
            Item.value = bars[size] * ContentSamples.ItemsByType[barIDs[0, size]].value;
            //Item.value = fragments[size] * ModContent.GetModItem(ModContent.ItemType<ContainmentFragment>()).Item.value;
            Item.width = 28 + 4 * (size);
            Item.height = 28 + 4 * (size);
            if(size == 2)
            {
                Item.value += ContentSamples.ItemsByType[177].value * 4;
            }
        }
        public override void AddRecipes()
        {
            for(int i = 0; i < 2; i++)
            {
                Recipe recipie = CreateRecipe();
                recipie.AddTile(TileID.WorkBenches);
                if (size == 2)
                {
                    recipie.AddRecipeGroup("WeaponEnchantments:CommonGems", 4);
                }
                else
                {
                    recipie.AddIngredient(ItemID.Glass, glass[size]);
                }
                recipie.AddIngredient(barIDs[i,size], bars[size]);
                recipie.Register();
            }
            IDs[size] = Item.type;
        }
    }
    public class MediumContainment : Containment { }
    public class SuperiorContainment : Containment { }
}
