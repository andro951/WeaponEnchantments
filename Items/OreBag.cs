using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Localization;
using WeaponEnchantments.UI;
using static WeaponEnchantments.Common.EnchantingRarity;

namespace WeaponEnchantments.Items
{
    public  class OreBag : WEModItem, ISoldByWitch
    {
        public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
        public virtual SellCondition SellCondition => SellCondition.Always;
        public virtual float SellPriceModifier => 1f;
        public override List<WikiTypeID> WikiItemTypes => new() { WikiTypeID.Storage };
        public override int CreativeItemSacrifice => 1;
		public override string LocalizationTooltip => 
            $"Automatically stores ores, bars, gems, and glass.\n" +
            $"When in your inventory, the contents of the bag are available for crafting.\n" +
            $"Right click to open the bag.";

		public override string Artist => "andro951";
        public override string Designer => "andro951";
        public override void SetStaticDefaults() {
            GetDefaults();

            base.SetStaticDefaults();
        }
        private void GetDefaults() {
            GetValues();
        }
        public override void SetDefaults() {
            GetDefaults();
            Item.maxStack = 1;
            Item.value = 100000;
            Item.width = 24;
            Item.height = 24;
            Item.rare = 1;
        }
        private void GetValues() {
            
        }
        public override void AddRecipes() {
            Recipe recipie = CreateRecipe();
			recipie.AddTile(TileID.WorkBenches);
			recipie.AddIngredient(ItemID.TatteredCloth);
			recipie.AddIngredient(ItemID.WhiteString);
			recipie.Register();
        }
		public override bool CanRightClick() => true;
        public override void RightClick(Player player) {
            Item.stack = 2;
			UseBag(player);
		}

		public override bool? UseItem(Player player) {
            if (Main.myPlayer == player.whoAmI && Main.netMode != NetmodeID.Server)
                UseBag(player);

            return null;
		}
        private void UseBag(Player player) {
            if (player.GetWEPlayer().displayOreBagUI && Main.LocalPlayer.chest == -1) {
                OreBagUI.CloseOreBag(true);
			}
            else {
				OreBagUI.OpenOreBag();
			}
		}
	}
}
