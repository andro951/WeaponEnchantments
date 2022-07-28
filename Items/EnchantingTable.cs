using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;

namespace WeaponEnchantments.Items
{
	public abstract class EnchantingTable : ModItem
	{
		public static string[] enchantingTableNames = new string[5] { "Wood", "Dusty", "Hellish", "Soul", "Ultimate" };
		public static int[] IDs = new int[enchantingTableNames.Length];

		public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');

		public abstract int enchantingTableTier { get; }
		public abstract int Tile {get; }								// The tile type to be placed

		public virtual string Artist { private set; get; } = "Zorutan";
		public virtual string Designer { private set; get; } = "andro951";
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
			Tooltip.SetDefault("Used to apply enchantments to items. (tier " + enchantingTableTier + ")");
			//DisplayName.SetDefault(enchantingTableNames[enchantingTableTier] + " Enchanting Table");

			LogUtilities.UpdateContributorsList(this);
		}

		public override void SetDefaults()
		{
			Item.createTile = Tile;
			Item.maxStack = 99;
			Item.width = 28;
			Item.height = 14;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 10;
			Item.useAnimation = 15;
			Item.consumable = true;
			Item.value = 150;
		}

		private string GetPreviousTierItemName()
		{
			return enchantingTableNames[enchantingTableTier - 1] + "EnchantingTable";
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			if (enchantingTableTier > -1)
			{
				string previousTierName = null; //Will never be used as null. Set if enchanting table tier is > 0
				if (enchantingTableTier > 0)
				{
					//recipe.AddTile(TileID.WorkBenches);
					previousTierName = GetPreviousTierItemName();
					recipe.AddIngredient(Mod, previousTierName, 1);
				}
				switch (enchantingTableTier)
				{
					case 0:
						recipe.AddRecipeGroup("WeaponEnchantments:Workbenches");
						recipe.AddIngredient(ItemID.Torch, 4); //Torches
						break;
					case 1:
						recipe.AddIngredient(ItemID.DesertFossil, 10); //Desert Fossil
						recipe.Register();
						recipe = CreateRecipe();
						recipe.AddIngredient(Mod, previousTierName, 1);
						recipe.AddIngredient(ItemID.FossilOre, 1);
						break;
					case 2:
						recipe.AddIngredient(ItemID.ObsidianSkull, 1); //Obsidian Skull
						break;
					case 3:
						recipe.AddRecipeGroup("WeaponEnchantments:AlignedSoul", 2); // Soul of Light or Night
						break;
					case 4:
						recipe.AddIngredient(ItemID.HallowedBar, 2); //Hallowed Bars
						break;
				}
				recipe.Register();
				IDs[enchantingTableTier] = this.Type;
			}
		}
	}

    public class WoodEnchantingTable : EnchantingTable
    {
        public override int enchantingTableTier => 0;
        public override int Tile => ModContent.TileType<Tiles.WoodEnchantingTable>();
    }

    public class DustyEnchantingTable : EnchantingTable
    {
        public override int enchantingTableTier => 1;
        public override int Tile => ModContent.TileType<Tiles.DustyEnchantingTable>();
    }

    public class HellishEnchantingTable : EnchantingTable
    {
        public override int enchantingTableTier => 2;
        public override int Tile => ModContent.TileType<Tiles.HellishEnchantingTable>();
    }

    public class SoulEnchantingTable : EnchantingTable
    {
        public override int enchantingTableTier => 3;
        public override int Tile => ModContent.TileType<Tiles.SoulEnchantingTable>();
    }

    public class UltimateEnchantingTable : EnchantingTable
    {
        public override int enchantingTableTier => 4;
        public override int Tile => ModContent.TileType<Tiles.UltimateEnchantingTable>();
    }
}
