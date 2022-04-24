using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using WeaponEnchantments.Common;
using log4net;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace WeaponEnchantments.Items
{
	public class Enchantments : ModItem
	{
		public int enchantmentSize = -1;
		public float enchantmentStrength;
		public static string[] rarity = new string[5] { "Common", "Uncommon", "Rare", "SuperRare", "UltraRare" };
		public static Color[] rarityColors = new Color[5] { Color.White, Color.Green, Color.Blue, Color.Purple, Color.Orange};
		public static int[] ID = new int[rarity.Length];
		public static List<int[]> IDs = new List<int[]>();
		public static string[] enchantmentTypeNames = new string[2] { "Damage", "Critical" };
		public static int shortestEnchantmentTypeName = 6;//DONT FORGET TO UPDATE THIS!!!!
		public string enchantmentTypeName;
		public int enchantmentType = -1;
		//public override string Texture => "WeaponEnchantments/Items/DamageEnchantmentCommon";
		public override void SetDefaults()
		{
			if (enchantmentSize > -1)
			{
				Item.value = (int)(100 * Math.Pow(20, enchantmentSize));
				Tooltip.SetDefault("Item value: " + Item.value.ToString());
				//DisplayName.SetDefaults("");
				//Tooltip.SetDefault("");
				switch (enchantmentSize)
				{
					case 0:
						Item.width = 4;
						Item.height = 4;
						enchantmentStrength = 0.03f;
						break;
					case 1:
						Item.width = 8;
						Item.height = 8;
						enchantmentStrength = 0.08f;
						break;
					case 2:
						Item.width = 12;
						Item.height = 12;
						enchantmentStrength = 0.15f;
						break;
					case 3:
						Item.width = 16;
						Item.height = 16;
						enchantmentStrength = 0.25f;
						break;
					case 4:
						Item.width = 20;
						Item.height = 20;
						enchantmentStrength = 0.40f;
						break;
				}
				/*
				for (int i = rarity[enchantmentSize].Length + shortestEnchantmentTypeName; i < Name.Length; i++)
				{
					if (Name[i] == ' ')
					{
						enchantmentTypeName = Name.Substring(rarity[enchantmentSize].Length + shortestEnchantmentTypeName, i - (rarity[enchantmentSize].Length + shortestEnchantmentTypeName) + 1);
						break;
					}
				}
				for (int i = 0; i < enchantmentTypeNames.Length; i++)
				{
					if (enchantmentTypeNames[i] == enchantmentTypeName)
					{
						enchantmentType = i;
					}
				}
				*/
				for(int i = 0; i < enchantmentTypeNames.Length; i++)
                {
					if(enchantmentTypeNames[i] == Name.Substring(0, enchantmentTypeNames[i].Length))
                    {
						enchantmentTypeName = enchantmentTypeNames[i];
						enchantmentType = i;
                    }
                }
			}
		}
		public override void UpdateInventory(Player player)
		{
			for (int i = 16; i < 20; i++)
			{
				if (player.inventory[i] == this.Item)
				{
					UpdateStats();
				}
			};
		}
		public virtual void UpdateStats()
		{
			;
		}
		public override void AddRecipes()
		{
			if (enchantmentSize > -1)
			{
				Recipe recipe = CreateRecipe();
				//recipe.AddIngredient(ItemID.DirtBlock, 10);
				//recipe.AddTile(TileID.WorkBenches);
				recipe.Register();
				ModItemID.Add(this.Name, this.Type);
				ID[enchantmentSize] = this.Type;
				if(enchantmentSize == rarity.Length - 1)
                {
					IDs.Add(ID);
                }
			}
		}

		public class OmniEnchantment : Enchantments
		{
			OmniEnchantment() { enchantmentSize = 4; }
			
			public override void UpdateInventory(Player player)
			{
				for (int i = 16; i < 20; i++)
				{
					if (player.inventory[i] == this.Item)
					{
						player.GetDamage(DamageClass.Generic) += 0.2f;
						player.GetDamage(DamageClass.Ranged) += 0.2f;
						player.GetDamage(DamageClass.Melee) += 0.2f;
						player.GetDamage(DamageClass.Magic) += 0.2f;
						player.GetDamage(DamageClass.Summon) += 0.2f;
						player.GetCritChance(DamageClass.Ranged) += 3;
						player.GetCritChance(DamageClass.Generic) += 3;
						player.GetCritChance(DamageClass.Melee) += 3;
						player.GetCritChance(DamageClass.Magic) += 3;
						player.maxMinions++;
						player.statManaMax += 500;
						player.statManaMax2 += 500;
						player.armorPenetration += 5;
						player.meleeSpeed += 1f;
						player.moveSpeed += 0.5f;
						player.manaCost -= 0.5f;
						player.extraAccessorySlots = 3;
						player.extraAccessory = true;
						player.lifeSteal = 0.1f;
						player.ichor = true;
						//player.statDefense += 100;
						//player.statLifeMax += 50; 
						//player.statLifeMax2 += 100;
						player.statLife += 1;
						player.statMana += 1;
						player.lifeRegen = 1;
						player.lifeRegenCount = 5;
						player.lifeRegenTime = 5;
						player.manaRegen = 1;
						player.manaRegenCount = 5;
						player.manaRegenDelay = 0;
						player.manaRegenBuff = true;
						player.noKnockback = true;
						player.spaceGun = true;
					}
				};
			}
			private int[] freeItems = new int[14] {437 , 3374, 193, 1225, 520, 521, 2786, 3531, 4365, 4735, 346, 87, 3813, 4076};
			public override void AddRecipes()
			{

				//Creates new recipe for Vanilla item
				
				for (int i = 0; i < freeItems.Length; i++){
					/*
					// \/Old\/
					if (Recipe.numRecipes<Recipe.maxRecipes)
					{
						//Main.recipe[Recipe.numRecipes].createItem.SetDefaults(freeItems[i]);

						Main.recipe[Recipe.numRecipes].createItem.stack = 1;
						//Main.recipe[Recipe.numRecipes].requiredItem[0] = new Item();
						Main.recipe[Recipe.numRecipes].requiredItem[0].type = ItemID.Wood;
						Main.recipe[Recipe.numRecipes].requiredItem[0].stack = 1;
						Main.recipe[Recipe.numRecipes].AddTile(TileID.WorkBenches);
						//Main.recipe[Recipe.numRecipes].requiredTile[0] = TileID.WorkBenches; //Doesn't work
						Recipe.numRecipes++;
					}
					// /\Old/\
					*/


					//I think this tracks all NPCIDs use to set all npcs to drop essence
					//int num = NPCID.FromNetId(id);



					//Recipe recipe = CreateRecipe();
					//recipe.ReplaceResult(freeItems[i]);
					//recipe.AddIngredient(ItemID.Wood, 1);
					//recipe.AddTile(TileID.WorkBenches);
					//recipe.Register();
					//ModItemID.Add(this.Name, this.Type);


					// \/New\/
					Mod.CreateRecipe(freeItems[i], 1).AddIngredient(ItemID.Wood, 1).AddTile(TileID.WorkBenches).Register();
					// /\New/\
				}
			}

		}

		public class DamageEnchantmentCommon : Enchantments
		{
			DamageEnchantmentCommon() { enchantmentSize = 0; }
		}
		public class DamageEnchantmentUncommon : Enchantments
		{
			DamageEnchantmentUncommon() { enchantmentSize = 1; }
		}
		public class DamageEnchantmentRare : Enchantments
		{
			DamageEnchantmentRare() { enchantmentSize = 2; }
		}
		public class DamageEnchantmentSuperRare : Enchantments
		{
			DamageEnchantmentSuperRare() { enchantmentSize = 3; }
		}
		public class DamageEnchantmentUltraRare : Enchantments
		{
			DamageEnchantmentUltraRare() { enchantmentSize = 4; }
		}

		public class CriticalEnchantmentCommon : Enchantments
		{
			CriticalEnchantmentCommon() { enchantmentSize = 0; }
		}
		public class CriticalEnchantmentUncommon : Enchantments
		{
			CriticalEnchantmentUncommon() { enchantmentSize = 1; }
		}
		public class CriticalEnchantmentRare : Enchantments
		{
			CriticalEnchantmentRare() { enchantmentSize = 2; }
		}
		public class CriticalEnchantmentSuperRare : Enchantments
		{
			CriticalEnchantmentSuperRare() { enchantmentSize = 3; }
		}
		public class CriticalEnchantmentUltraRare : Enchantments
		{
			CriticalEnchantmentUltraRare() { enchantmentSize = 4; }
		}
	}
}
