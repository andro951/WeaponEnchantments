using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Localization;
using androLib.Common.Utility;

namespace WeaponEnchantments.Items
{
	[Autoload(false)]
	public class UltraPowerBooster : WEModItem
    {
        public static int ID;
        public override DropRestrictionsID DropRestrictionsID => DropRestrictionsID.PostPlanteraBosses;
        public override List<WikiTypeID> WikiItemTypes => new() { WikiTypeID.PowerBooster };
        public override int CreativeItemSacrifice => 1;
		public override bool CanBeStoredInEnchantmentStorage => true;
		public override string LocalizationTooltip => 
            "Use this while the item you want to boost is in an Enchantment Table to raise its base level by 20.\n" +
			"(Shift left click from your inventory or left click on item in the table with this on your cursor.)\n" +
			"This item will be returned if the boosted item is offered.";

        public override string Artist => "andro951";
        public override string Designer => "andro951";
        public override void SetStaticDefaults() {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 4));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            ItemID.Sets.ItemIconPulse[Item.type] = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;

            base.SetStaticDefaults();
        }
        public override void SetDefaults() {
            Item.value = 1000000;
            Item.width = 18;
            Item.height = 18;
            Item.maxStack = 99;
            Item.rare = ItemRarityID.LightRed;
        }
        public override void PostUpdate() {
            Lighting.AddLight(Item.Center, Color.WhiteSmoke.ToVector3() * 0.55f * Main.essScale);
        }
        public override void AddRecipes() {
            ID = Item.type;
        }
    }
}
