using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Localization;

namespace WeaponEnchantments.Items
{
    public class SuperSoap : WEModItem, ISoldByWitch
    {
        public static int ID;
        public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
        public override DropRestrictionsID DropRestrictionsID => DropRestrictionsID.None;
        public override List<WikiTypeID> WikiItemTypes => new() { WikiTypeID.SuperSoap };
        public override bool ConfigOnlyDrop => false;
        public override int CreativeItemSacrifice => 1;
		public override string LocalizationTooltip => 
            "Use this while the item you want to clean is in an Enchantment Table to reset its skill points.\n" +
			"(Shift left click from your inventory or left click on item in the table with this on your cursor.)";

        public override string Artist => "SirBumpleton";
        public override string Designer => "SirBumpleton";
        public override void SetStaticDefaults() {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 1));
            ItemID.Sets.AnimatesAsSoul[Item.type] = false;
            ItemID.Sets.ItemIconPulse[Item.type] = false;
            ItemID.Sets.ItemNoGravity[Item.type] = false;

            base.SetStaticDefaults();
        }
        public override void SetDefaults() {
            Item.value = 50000;
            Item.width = 15;
            Item.height = 13;
            Item.maxStack = 99;
            Item.rare = ItemRarityID.Cyan;
        }
        public override void PostUpdate() {
            Lighting.AddLight(Item.Center, Color.LightCyan.ToVector3() * 0.55f * Main.essScale); // Makes this item glow when thrown out of inventory.
        }
        public override void AddRecipes() {
            ID = Item.type;
        }
    }
}
