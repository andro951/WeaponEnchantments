using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace WeaponEnchantments.Items
{
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
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
            //for (int i = 0; i < sizes.Length; i++)
            {
                Values[size] = bars[size] * ContentSamples.ItemsByType[barIDs[0, size]].value;
                if (size == 2)
                {
                    Values[size] += ContentSamples.ItemsByType[180].value * 4;
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
                    if (sizes[i] == Name.Substring(0, Name.IndexOf("Containment")))
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
                Item.value += ContentSamples.ItemsByType[180].value * 4;
            }
        }
        public override void AddRecipes()
        {
            Recipe recipie;
            for (int i = 0; i < 2; i++)
            {
                recipie = CreateRecipe();
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
            Recipe.Create(barIDs[0, size], bars[size]).AddIngredient(Item.type).AddTile(TileID.Furnaces).Register();
        }
    }
    public class MediumContainment : Containment { }
    public class SuperiorContainment : Containment { }
}
