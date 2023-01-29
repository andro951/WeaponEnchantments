using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Localization;
using static WeaponEnchantments.Common.EnchantingRarity;

namespace WeaponEnchantments.Items
{
    public abstract class ContainmentItem : WEModItem, ISoldByWitch
    {
        public static string[] sizes = new string[] { "", "Medium", "Superior" };
        public static int[] glass = new int[] { 1, 4, 0};
        public static int[,] barIDs = new int[,] { { ItemID.SilverBar, ItemID.GoldBar, ItemID.DemoniteBar }, { ItemID.TungstenBar, ItemID.PlatinumBar, ItemID.CrimtaneBar } };
        public static int[] IDs = new int[sizes.Length];
        public static int[] Values = new int[sizes.Length];
        
        public int tier = 0;
        private int bars;
        public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
        public virtual SellCondition SellCondition => SellCondition.Always;
        public virtual float SellPriceModifier => 1f;
        public override List<WikiTypeID> WikiItemTypes => new() { WikiTypeID.Containments, WikiTypeID.CraftingMaterial };
        public override int CreativeItemSacrifice => 3;
		public override string LocalizationTooltip => $"Used to store {tierNames[tier]} enchantments";

        public override string Artist => "Zorutan";
        public override string Designer => "andro951";
        public override void SetStaticDefaults() {
            GetDefaults();

            base.SetStaticDefaults();
        }
        private void GetDefaults() {
            for (int i = 0; i < sizes.Length; i++) {
                int indexOfContintment = Name.IndexOf("Containment");
                if (sizes[i] == Name.Substring(0, indexOfContintment)) {
                    tier = i;
                    break;
                }
            }

            GetValues();
        }
        public override void SetDefaults() {
            GetDefaults();
            Item.maxStack = 1000;
            Item.value = Values[tier];
            Item.width = 28 + 4 * tier;
            Item.height = 28 + 4 * tier;
            Item.rare = tier + 1;
        }
        private void GetValues() {
            if (Values[tier] != 0)
                return;

            bars = 4 * (int)Math.Pow(2, tier);
            Values[tier] = bars * ContentSamples.ItemsByType[barIDs[0, tier]].value;
            if (tier == 2)
                Values[tier] += ContentSamples.ItemsByType[ItemID.Topaz].value * 4;
        }
        public override void AddRecipes() {
            Recipe recipie;
            for (int i = 0; i < 2; i++) {
                recipie = CreateRecipe();
                recipie.AddTile(TileID.WorkBenches);
                if (tier == 2) {
                    recipie.AddRecipeGroup("WeaponEnchantments:CommonGems", 4);
                }
                else {
                    recipie.AddIngredient(ItemID.Glass, glass[tier]);
                }
                recipie.AddIngredient(barIDs[i,tier], bars);
                recipie.Register();
            }

            IDs[tier] = Item.type;
            Recipe.Create(barIDs[0, tier], bars).AddIngredient(Item.type).AddTile(TileID.Furnaces).Register();
        }
    }
    public class Containment : ContainmentItem { }
    public class MediumContainment : ContainmentItem { }
    public class SuperiorContainment : ContainmentItem {
        public override SellCondition SellCondition => SellCondition.PostEaterOfWorldsOrBrainOfCthulhu;
    }
}
