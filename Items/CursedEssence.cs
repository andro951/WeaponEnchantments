using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;
using static WeaponEnchantments.Common.EnchantingRarity;

namespace WeaponEnchantments.Items
{
	public class CursedEssence : ModItem, IItemWikiInfo {
		private int entitySize = 20;
		public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
		public Color glowColor = TierColors[3];
		public virtual List<WikiItemTypeID> WikiItemTypes => new() { WikiItemTypeID.CursedEssence, WikiItemTypeID.CraftingMaterial };

		public virtual string Artist { private set; get; } = "Kiroto";
		public virtual string Designer { private set; get; } = "andro951";

		public override void SetStaticDefaults() {
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;

			//Log contributors
			if (LogModSystem.printListOfContributors)
				LogModSystem.UpdateContributorsList(this);
		}
        public override voied ModifyTooltips(Tooltip tooltip) {
            int cursedEssenceCount = player.GetWEPlayer().cursedEssenceCount;
            tooltip.Add($"Energy of a curse so powerful it's taken form.  Other curses will be drawn to its power.");
            tooltip.Add($"\tIncreases enemy spawn rate.  (Current bonus {(Main.LocalPlayer.GetWEPlayer().cursedEssenceCount * 5).PercentString()})");
            tooltip.Add($"\tIncreased chance of cursed enemies spawning.")
        }
		public override void PostUpdate() {
			float intensity = 0.5f;
			Lighting.AddLight(Item.Center, glowColor.ToVector3() * intensity * Main.essScale);
		}
		public override void SetDefaults() {
			Item.value = (int)EnchantmentEssence.values[3];
			Item.maxStack = 100;
			Item.width = entitySize;
			Item.height = entitySize;
			Item.rare = GetRarityFromTier(3);
		}

		public override void AddRecipes() {
			for (int i = 0; i < tierNames.Length; i++) {
                Recipe recipe = CreateRecipe();
                recipe.createItem = new(ModContent.ItemType<EnchantmentEssenceEpic>());
                recipe.AddIngredient(Type);
                recipe.AddIngredient(ItemId.PurificationPowder, 20 / (i + 1));
                recipe.AddTile(Mod, EnchantingTableItem.enchantingTableNames[i] + "EnchantingTable");
                recipe.Register();
			}
		}
        public override void UpdateInventory(Player player, Item item) {
            player.GetWEPlayer().cursedEssenceCount += item.stack;
            player.GetWEPlayer().cursedEssenceCount.Clamp(0, 100);
        }
	}
    
    public static class CursedEnemy : GlobalNPC
    {
        private static bool nextRareEnemyCursed = false;
        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns) {
            spawnRate = (int)Math.Round((float)spawnRate * (1f + (float)player.GetWEPlayer().cursedEssenceCount * 0.05f));
        }
        public static float GetCursedEnemySpawnChance(WEPlayer wePlayer) {
            float chance = 0f;
            
            if (!Main.dayTime || Main.solarEclipse || player.ZoneUnderground)
                chance += 0.5f;
            
            if (Main.pumpkinMoon || Main.goblinArmy || Main.pirateInvasion || Main.martianInvasion || Main.frostMoon)
                chance += 0.25f;
            
            if (Main.bloodMoon)
                chance += 0.25f;
            
            if (player.zoneCorruption || player.zoneCrimson)
                chance += 0.5f;
            
            if (zoneUnderworld)
                chance += 0.5f;
            
            if (chance > 0f && wePlayer.cursedEssenceCount > 0)
                chance += (float)wePlayer.cursedEssenceCount / 40f + 0.5f;
        }
    }
}
