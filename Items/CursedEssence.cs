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
	public abstract class CursedEssence : ModItem, IItemWikiInfo {
		private int entitySize = 20;
		public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
		public Color glowColor = TierColors[3];
		public virtual List<WikiItemTypeID> WikiItemTypes => new() { WikiItemTypeID.CursedEssence, WikiItemTypeID.CraftingMaterial };

		public virtual string Artist { private set; get; } = "Kiroto";
		public virtual string Designer { private set; get; } = "andro951";

		public override void SetStaticDefaults() {
            if (!WEMod.serverConfig.DisableResearch)
                CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;

			//Log contributors
			if (LogModSystem.printListOfContributors)
				LogModSystem.UpdateContributorsList(this);
		}
		public override void ModifyTooltips(List<TooltipLine> tooltips) {
            int cursedEssenceCount = Main.LocalPlayer.GetWEPlayer().cursedEssenceCount;
            tooltips.Add(new(Mod, "cursedEssenceTooltip", $"Energy of a curse so powerful it's taken form.  Other curses will be drawn to its power." +
				$"\tIncreases enemy spawn rate.  (Current bonus {((float)Main.LocalPlayer.GetWEPlayer().cursedEssenceCount * 0.05f).PercentString()})" +
				$"\tIncreased chance of cursed enemies spawning."));
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
                if (WEMod.serverConfig.ReduceRecipesToMinimum && i != tierNames.Length - 1)
                    continue;

                Recipe recipe = CreateRecipe();
                recipe.createItem = new(ModContent.ItemType<EnchantmentEssenceEpic>());
                recipe.AddIngredient(Type);
                int num = WEMod.serverConfig.ReduceRecipesToMinimum ? 20 / (tierNames.Length - 1) : 20 / (i + 1);
                recipe.AddIngredient(ItemID.PurificationPowder, num);
                recipe.AddTile(Mod, EnchantingTableItem.enchantingTableNames[i] + "EnchantingTable");
                recipe.Register();
			}
		}
		public override void UpdateInventory(Player player) {
            player.GetWEPlayer().cursedEssenceCount += Item.stack;
            player.GetWEPlayer().cursedEssenceCount.Clamp(0, 100);
        }
	}
    
    public class CursedEnemy : GlobalNPC
    {
        private static bool nextRareEnemyCursed = false;
        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns) {
            spawnRate = (int)Math.Round((float)spawnRate * (1f + (float)player.GetWEPlayer().cursedEssenceCount * 0.05f));
        }
        public static float GetCursedEnemySpawnChance(WEPlayer wePlayer) {
            float chance = 0f;
            
            if (!Main.dayTime || Main.eclipse || wePlayer.Player.ZoneNormalUnderground)
                chance += 0.5f;
            
            if (Main.invasionType > 0 || Main.pumpkinMoon || Main.snowMoon)
                chance += 0.25f;
            
            if (Main.bloodMoon)
                chance += 0.25f;
            
            if (wePlayer.Player.ZoneCorrupt || wePlayer.Player.ZoneCrimson)
                chance += 0.5f;
            
            if (wePlayer.Player.ZoneUnderworldHeight)
                chance += 0.5f;
            
            if (chance > 0f && wePlayer.cursedEssenceCount > 0)
                chance += (float)wePlayer.cursedEssenceCount / 40f + 0.5f;

            return chance;
        }
    }
}
