using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using WeaponEnchantments.Common;
using log4net;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

namespace WeaponEnchantments.Items
{
	public class Enchantments : ModItem
	{
		public static bool cheating = false;
		public class EnchantmentTypeIDs
        {
			internal static readonly int Damage = 0;
			internal static readonly int Critical = 1;
			internal static readonly int Size = 2;
			internal static readonly int Speed = 3;
        }

		public int enchantmentSize = -1;
		public float enchantmentStrength;
		public static string[] rarity = new string[5] { "Basic", "Common", "Rare", "SuperRare", "UltraRare" };
		public static Color[] rarityColors = new Color[5] { Color.White, Color.Green, Color.Blue, Color.Purple, Color.Orange};
		public static int[] ID = new int[rarity.Length];
		public static List<int[]> IDs = new List<int[]>();
		public static string[] enchantmentTypeNames = new string[] { "Damage", "Critical" ,"Size", "Speed"};
		public static string[] utilityEnchantmentIDs = new string[1] { "Size" };
		public bool utility;
		public static int shortestEnchantmentTypeName = 4;//DONT FORGET TO UPDATE THIS!!!!
		public string enchantmentTypeName;
		public int enchantmentType = -1;
		public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
		public override void SetDefaults()
		{
			if (enchantmentSize > -1)
			{
				Item.value = (int)(1000 * Math.Pow(8, enchantmentSize));

				for (int i = 0; i < enchantmentTypeNames.Length; i++)
				{
					if (enchantmentTypeNames[i] == Name.Substring(0, enchantmentTypeNames[i].Length))
					{
						enchantmentTypeName = enchantmentTypeNames[i];
						enchantmentType = i;
					}
				}
				for (int i = 0; i < utilityEnchantmentIDs.Length; i++)
				{
					if (enchantmentTypeNames[enchantmentType] == utilityEnchantmentIDs[i])
					{
						utility = true;
					}
				}
				if (enchantmentSize < 2)
                {
					Item.width = 10 + 4 * (enchantmentSize);
					Item.height = 10 + 4 * (enchantmentSize);
				}
                else
				{
					Item.width = 40;
					Item.height = 40;
				}
                switch (enchantmentTypeName)
                {
					case "Size":
						switch (enchantmentSize)
						{
							case 0:
								enchantmentStrength = 0.1f;
								break;
							case 1:
								enchantmentStrength = 0.2f;
								break;
							case 2:
								enchantmentStrength = 0.50f;
								break;
							case 3:
								enchantmentStrength = 0.8f;
								break;
							case 4:
								enchantmentStrength = 1f;
								break;
						}
						break;
					default:
						switch (enchantmentSize)
						{
							case 0:
								enchantmentStrength = 0.03f;
								break;
							case 1:
								enchantmentStrength = 0.08f;
								break;
							case 2:
								enchantmentStrength = 0.15f;
								break;
							case 3:
								enchantmentStrength = 0.25f;
								break;
							case 4:
								enchantmentStrength = 0.40f;
								break;
						}
						break;
                }
				if(enchantmentType == EnchantmentTypeIDs.Size)
                {
					Tooltip.SetDefault("+" + ((int)(enchantmentStrength * 50)).ToString() + "% " + enchantmentTypeName + "\n+" + ((int)(enchantmentStrength * 100)).ToString() + "% Knockback" + "\nLevel cost: " + GetLevelCost().ToString());
				}
                else
                {
					Tooltip.SetDefault("+" + ((int)(enchantmentStrength * 100)).ToString() + "% " + enchantmentTypeName + "\nLevel cost: " + GetLevelCost().ToString());
				}
			}
		}
		public override void AddRecipes()
		{
			if (enchantmentSize > -1)
			{
				for(int i = enchantmentSize; i < rarity.Length; i++)
                {
					Recipe recipe = CreateRecipe();
					recipe.AddIngredient(Mod, "EnchantmentEssence" + EnchantmentEssence.rarity[enchantmentSize], 10);
					if(enchantmentSize > 0)
                    {
						recipe.AddIngredient(Mod, enchantmentTypeName + "Enchantment" + rarity[enchantmentSize - 1], 1);
					}
					if(enchantmentSize < 3)
                    {
						recipe.AddIngredient(Mod, Containment.sizes[enchantmentSize] + "Containment", 1);
                    }
					else if(enchantmentSize == 4)
					{
						recipe.AddIngredient(Stabilizer.IDs[1], 4);
                    }
					recipe.AddTile(Mod, EnchantingTableItem.enchantingTableNames[i] + "EnchantingTable");
					recipe.Register();
				}
				ID[enchantmentSize] = this.Type;
				if(enchantmentSize == rarity.Length - 1)
                {
					IDs.Add(ID);
                }
			}
		}
		public int GetLevelCost()
        {
			return utility ? enchantmentSize / 2 : enchantmentSize;
        }
		public override void OnCreate(ItemCreationContext context) 
		{
			if (0 < enchantmentSize && enchantmentSize < 3)
			{
                if (Containment.IDs[enchantmentSize - 1] > ItemID.None)
                {
					Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), Containment.IDs[enchantmentSize - 1], 1);
				}
			}
		}

		public class OmniEnchantment : Enchantments
		{
			OmniEnchantment() { enchantmentSize = -1; }
			
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
			private int[] freeItems = new int[15] {437 , 3374, 193, 1225, 520, 521, 2786, 3531, 4365, 4735, 346, 87, 3813, 4076, 514};
			public override void AddRecipes()
			{

                //Creates new recipe for Vanilla item
                if (cheating)
                {
					for (int i = 0; i < freeItems.Length; i++)
					{
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



						Recipe recipe = CreateRecipe();
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

		}

		public class DamageEnchantmentBasic : Enchantments
		{
			DamageEnchantmentBasic() { enchantmentSize = 0; }
		}
		public class DamageEnchantmentCommon : Enchantments
		{
			DamageEnchantmentCommon() { enchantmentSize = 1; }
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

		public class CriticalEnchantmentBasic : Enchantments
		{
			CriticalEnchantmentBasic() { enchantmentSize = 0; }
		}
		public class CriticalEnchantmentCommon : Enchantments
		{
			CriticalEnchantmentCommon() { enchantmentSize = 1; }
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

		public class SizeEnchantmentBasic : Enchantments
		{
			SizeEnchantmentBasic() { enchantmentSize = 0; }
		}
		public class SizeEnchantmentCommon : Enchantments
		{
			SizeEnchantmentCommon() { enchantmentSize = 1; }
		}
		public class SizeEnchantmentRare : Enchantments
		{
			SizeEnchantmentRare() { enchantmentSize = 2; }
		}
		public class SizeEnchantmentSuperRare : Enchantments
		{
			SizeEnchantmentSuperRare() { enchantmentSize = 3; }
		}
		public class SizeEnchantmentUltraRare : Enchantments
		{
			SizeEnchantmentUltraRare() { enchantmentSize = 4; }
		}

		public class SpeedEnchantmentBasic : Enchantments
		{
			SpeedEnchantmentBasic() { enchantmentSize = 0; }
		}
		public class SpeedEnchantmentCommon : Enchantments
		{
			SpeedEnchantmentCommon() { enchantmentSize = 1; }
		}
		public class SpeedEnchantmentRare : Enchantments
		{
			SpeedEnchantmentRare() { enchantmentSize = 2; }
		}
		public class SpeedEnchantmentSuperRare : Enchantments
		{
			SpeedEnchantmentSuperRare() { enchantmentSize = 3; }
		}
		public class SpeedEnchantmentUltraRare : Enchantments
		{
			SpeedEnchantmentUltraRare() { enchantmentSize = 4; }
		}
	}
}
