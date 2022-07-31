using System;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;

namespace WeaponEnchantments.Items
{
    public abstract class ContainmentItem : ModItem
    {
        public static string[] sizes = new string[] { "", "Medium", "Superior" };
        public static int[] glass = new int[] { 1, 4, 0};
        public static int[,] barIDs = new int[,] { { ItemID.SilverBar, ItemID.GoldBar, ItemID.DemoniteBar }, { ItemID.TungstenBar, ItemID.PlatinumBar, ItemID.CrimtaneBar } };
        public static int[] IDs = new int[sizes.Length];
        public static int[] Values = new int[sizes.Length];
        
        public int size = 0;
        private int bars;
        public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');

        public virtual string Artist { private set; get; } = "Zorutan";
        public virtual string Designer { private set; get; } = "andro951";
        public override void SetStaticDefaults() {
            GetDefaults();
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
            bars = 4 * (int)Math.Pow(2, size);
            Values[size] = bars * ContentSamples.ItemsByType[barIDs[0, size]].value;
            if (size == 2)
                Values[size] += ContentSamples.ItemsByType[180].value * 4;

            Tooltip.SetDefault("Used to store " + Enchantment.rarity[size] + " enchantments");
            
            LogModSystem.UpdateContributorsList(this);
        }
        private void GetDefaults() {
            for (int i = 0; i < sizes.Length; i++) {
                int indexOfContintment = Name.IndexOf("Containment");
                if (sizes[i] == Name.Substring(0, indexOfContintment)) {
                    size = i;
                    break;
                }
            }
        }
        public override void SetDefaults() {
            GetDefaults();
            Item.maxStack = 1000;
            Item.value = Values[size];
            Item.width = 28 + 4 * size;
            Item.height = 28 + 4 * size;
        }
        public override void AddRecipes() {
            Recipe recipie;
            for (int i = 0; i < 2; i++) {
                recipie = CreateRecipe();
                recipie.AddTile(TileID.WorkBenches);
                if (size == 2) {
                    recipie.AddRecipeGroup("WeaponEnchantments:CommonGems", 4);
                }
                else {
                    recipie.AddIngredient(ItemID.Glass, glass[size]);
                }
                recipie.AddIngredient(barIDs[i,size], bars);
                recipie.Register();
            }

            IDs[size] = Item.type;
            Recipe.Create(barIDs[0, size], bars).AddIngredient(Item.type).AddTile(TileID.Furnaces).Register();
        }
    }
    public class Containment : ContainmentItem { }
    public class MediumContainment : ContainmentItem { }
    public class SuperiorContainment : ContainmentItem { }
}
