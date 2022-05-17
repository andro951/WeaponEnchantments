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
using static WeaponEnchantments.Items.Stabilizer;

namespace WeaponEnchantments.Items
{
	public class Enchantments : ModItem
	{
		public static readonly bool cheating = true;
		public class EnchantmentTypeIDs
        {
			internal const int Damage = 0;
			internal const int Critical = 1;
			internal const int Size = 2;
			internal const int Speed = 3;
			internal const int Defence = 4;
			internal const int ManaCost = 5;
			internal const int AmmoCost = 6;
			internal const int LifeSteal = 7;
			internal const int ArmorPenetration = 8;
			internal const int AllForOne = 9;
			internal const int OneForAll = 10;
			internal const int Spelunker = 11;
        }

		public int enchantmentSize = -1;
		public float enchantmentStrength;
		public static string[] rarity = new string[5] { "Basic", "Common", "Rare", "SuperRare", "UltraRare" };
		public static Color[] rarityColors = new Color[5] { Color.White, Color.Green, Color.Blue, Color.Purple, Color.Orange};
		public static int[] ID = new int[rarity.Length];
		public static List<int[]> IDs = new List<int[]>();
		public static string[] enchantmentTypeNames = new string[] { "Damage", "Critical" ,"Size", "Speed", "Defence", "ManaCost", "AmmoCost", "LifeSteal", "ArmorPenetration" , "AllForOne", "OneForAll", "Spelunker"};
		public static string[] utilityEnchantmentIDs = new string[] { "Size" , "ManaCost", "AmmoCost", "LifeSteal", "Spelunker" };
		public bool utility;
		public static int shortestEnchantmentTypeName = 4;//DONT FORGET TO UPDATE THIS!!!!
		public string enchantmentTypeName;
		public int enchantmentType = -1;
		public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
		public override void SetDefaults()
		{
			if (enchantmentSize > -1)
			{
				Item.maxStack = 99;
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
				switch (enchantmentType)
				{
					case EnchantmentTypeIDs.AllForOne:
					case EnchantmentTypeIDs.OneForAll:
					case EnchantmentTypeIDs.Spelunker:
						Item.value = (int)(1000 * Math.Pow(8, enchantmentSize - 2));
						break;
					default:
						Item.value = (int)(1000 * Math.Pow(8, enchantmentSize));
						break;
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
					case "Defence":
					case "ArmorPenetration":
						switch (enchantmentSize)
						{
							case 0:
								enchantmentStrength = 1f;
								break;
							case 1:
								enchantmentStrength = 2f;
								break;
							case 2:
								enchantmentStrength = 3f;
								break;
							case 3:
								enchantmentStrength = 5f;
								break;
							case 4:
								enchantmentStrength = 10f;
								break;
						}
						break;
					case "LifeSteal":
						switch (enchantmentSize)
						{
							case 0:
								enchantmentStrength = 0.005f;
								break;
							case 1:
								enchantmentStrength = 0.01f;
								break;
							case 2:
								enchantmentStrength = 0.02f;
								break;
							case 3:
								enchantmentStrength = 0.03f;
								break;
							case 4:
								enchantmentStrength = 0.04f;
								break;
						}
						break;
					case "AllForOne":
						enchantmentStrength = 10f;
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
								enchantmentStrength = 0.16f;
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
                switch (enchantmentType)
                {
					case EnchantmentTypeIDs.Size:
						Tooltip.SetDefault("+" + (enchantmentStrength * 50).ToString() + "% " + enchantmentTypeName + "\n+" + (enchantmentStrength * 100).ToString() + "% Knockback" + "\nLevel cost: " + GetLevelCost().ToString());
						break;
					case EnchantmentTypeIDs.Defence:
						Tooltip.SetDefault("+" + enchantmentStrength.ToString() + " " + enchantmentTypeName + "\nLevel cost: " + GetLevelCost().ToString());
						break;
					case EnchantmentTypeIDs.ArmorPenetration:
						Tooltip.SetDefault(enchantmentStrength.ToString() + " Armor Penetration\nLevel cost: " + GetLevelCost().ToString());
						break;
					case EnchantmentTypeIDs.ManaCost:
						Tooltip.SetDefault("-" + (enchantmentStrength * 100).ToString() + "% Mana Cost\nLevel cost: " + GetLevelCost().ToString());
						break;
					case EnchantmentTypeIDs.AmmoCost:
						Tooltip.SetDefault("-" + (enchantmentStrength * 100).ToString() + "% Chance to consume ammo\nLevel cost: " + GetLevelCost().ToString());
						break;
					case EnchantmentTypeIDs.LifeSteal:
						Tooltip.SetDefault((enchantmentStrength * 100).ToString() + "% Life Steal (remainder is saved to prevent \nalways rounding to 0 for low damage weapons)\nLevel cost: " + GetLevelCost().ToString());
						break;
					case EnchantmentTypeIDs.AllForOne:
						Tooltip.SetDefault("10x Damage, item CD equal to 8x use speed\nLevel cost: " + GetLevelCost().ToString());
						break;
					case EnchantmentTypeIDs.OneForAll:
						Tooltip.SetDefault("Hiting an enemy will damage all nearby enemies, 0.7x attack speed \n(WARNING - DESTROYS PROJECTILES ON HIT)\nLevel cost: " + GetLevelCost().ToString());
						break;
					case EnchantmentTypeIDs.Spelunker:
						Tooltip.SetDefault("Grants the Spelunker buff\nLevel cost: " + GetLevelCost().ToString());
						break;
					default:
						Tooltip.SetDefault("+" + (enchantmentStrength * 100).ToString() + "% " + enchantmentTypeName + "\nLevel cost: " + GetLevelCost().ToString());
						break;
				}
			}
		}
		public override void AddRecipes()
		{
			if (enchantmentSize > -1)
			{
				for(int i = enchantmentSize; i < rarity.Length; i++)
                {
					Recipe recipe;
					int skipIfLessOrEqualToSize;
					switch (enchantmentType)
                    {
						case EnchantmentTypeIDs.AllForOne:
						case EnchantmentTypeIDs.OneForAll:
						case EnchantmentTypeIDs.Spelunker:
							skipIfLessOrEqualToSize = 4;
							break;
						case EnchantmentTypeIDs.LifeSteal:
						case EnchantmentTypeIDs.ArmorPenetration:
						case EnchantmentTypeIDs.AmmoCost:
						case EnchantmentTypeIDs.ManaCost:
						case EnchantmentTypeIDs.Size:
						case EnchantmentTypeIDs.Critical:
						case EnchantmentTypeIDs.Speed:
							skipIfLessOrEqualToSize = 0;
							break;
						default:
							skipIfLessOrEqualToSize = -1;
							break;
					}
					if(enchantmentSize > skipIfLessOrEqualToSize)
                    {
						for (int j = enchantmentSize; j >= skipIfLessOrEqualToSize + 1; j--)
						{
							recipe = CreateRecipe();
							for (int k = enchantmentSize; k >= j; k--)
							{
								recipe.AddIngredient(Mod, "EnchantmentEssence" + EnchantmentEssence.rarity[k], 10);
							}
							if (j > 0)
							{
								recipe.AddIngredient(Mod, enchantmentTypeName + "Enchantment" + rarity[j - 1], 1);
							}
							if (enchantmentSize < 3)
							{
								recipe.AddIngredient(Mod, Containment.sizes[enchantmentSize] + "Containment", 1);
							}
							else if (j < 3)
							{
								recipe.AddIngredient(Mod, Containment.sizes[2] + "Containment", 1);
							}
							if (enchantmentSize == 4)
							{
								recipe.AddIngredient(ModContent.ItemType<SuperiorStabilizer>(), 4);
							}
							recipe.AddTile(Mod, EnchantingTableItem.enchantingTableNames[i] + "EnchantingTable");
							recipe.Register();
						}
					}
				}
				ID[enchantmentSize] = this.Type;
				if(enchantmentSize == rarity.Length - 1)
                {
					IDs.Add(ID);
                }
                if (cheating)
                {
					Mod.CreateRecipe(Type, 1).AddTile(TileID.WoodBlock).Register();
				}
			}
		}
		public int GetLevelCost()
        {
            switch (enchantmentTypeName)
            {
				case "AllForOne":
				case "OneForAll":
					return 15;
				default:
					return utility ? 1 + enchantmentSize : (1 + enchantmentSize) * 2;
			}
        }
        public override void OnCraft(Recipe recipe)
        {
			if (enchantmentSize > 0)
			{
				if (recipe.requiredItem.Count > 0)
                {
					foreach (Item reqiredItem in recipe.requiredItem)
					{
						for(int i = enchantmentSize; i >= 1; i--)
                        {
							if(enchantmentSize - i < 2)
                            {
								if (Item.type - i == reqiredItem.type)
								{
									Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), Containment.IDs[enchantmentSize - i], 1);
								}
							}
                        }
					}
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
			private int[] freeItems = new int[] {437 , 3380, 193, 1225, 520, 521, 2786, 3531, 4365, 4735, 346, 87, 3813, 4076, 514, 561, 4281, 5114, 1309};
			private int[] bossBags = new int[] {ItemID.DeerclopsBossBag, ItemID.BossBagBetsy, ItemID.FairyQueenBossBag, ItemID.QueenSlimeBossBag };
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



						//Recipe recipe = CreateRecipe();
						//recipe.ReplaceResult(freeItems[i]);
						//recipe.AddIngredient(ItemID.Wood, 1);
						//recipe.AddTile(TileID.WorkBenches);
						//recipe.Register();
						//ModItemID.Add(this.Name, this.Type);


						// \/New\/
						Mod.CreateRecipe(freeItems[i], 1).AddTile(TileID.WoodBlock).Register();
						// /\New/\
					}
					for(int i = ItemID.KingSlimeBossBag; i <= ItemID.MoonLordBossBag; i++)
                    {
						Mod.CreateRecipe(i, 1).AddTile(TileID.WoodBlock).Register();
					}
					for (int i = 0; i < bossBags.Length; i++)
					{
						Mod.CreateRecipe(bossBags[i], 1).AddTile(TileID.WoodBlock).Register();
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

		public class DefenceEnchantmentBasic : Enchantments
		{
			DefenceEnchantmentBasic() { enchantmentSize = 0; }
		}
		public class DefenceEnchantmentCommon : Enchantments
		{
			DefenceEnchantmentCommon() { enchantmentSize = 1; }
		}
		public class DefenceEnchantmentRare : Enchantments
		{
			DefenceEnchantmentRare() { enchantmentSize = 2; }
		}
		public class DefenceEnchantmentSuperRare : Enchantments
		{
			DefenceEnchantmentSuperRare() { enchantmentSize = 3; }
		}
		public class DefenceEnchantmentUltraRare : Enchantments
		{
			DefenceEnchantmentUltraRare() { enchantmentSize = 4; }
		}

		public class ManaCostEnchantmentBasic : Enchantments
		{
			ManaCostEnchantmentBasic() { enchantmentSize = 0; }
		}
		public class ManaCostEnchantmentCommon : Enchantments
		{
			ManaCostEnchantmentCommon() { enchantmentSize = 1; }
		}
		public class ManaCostEnchantmentRare : Enchantments
		{
			ManaCostEnchantmentRare() { enchantmentSize = 2; }
		}
		public class ManaCostEnchantmentSuperRare : Enchantments
		{
			ManaCostEnchantmentSuperRare() { enchantmentSize = 3; }
		}
		public class ManaCostEnchantmentUltraRare : Enchantments
		{
			ManaCostEnchantmentUltraRare() { enchantmentSize = 4; }
		}

		public class AmmoCostEnchantmentBasic : Enchantments
		{
			AmmoCostEnchantmentBasic() { enchantmentSize = 0; }
		}
		public class AmmoCostEnchantmentCommon : Enchantments
		{
			AmmoCostEnchantmentCommon() { enchantmentSize = 1; }
		}
		public class AmmoCostEnchantmentRare : Enchantments
		{
			AmmoCostEnchantmentRare() { enchantmentSize = 2; }
		}
		public class AmmoCostEnchantmentSuperRare : Enchantments
		{
			AmmoCostEnchantmentSuperRare() { enchantmentSize = 3; }
		}
		public class AmmoCostEnchantmentUltraRare : Enchantments
		{
			AmmoCostEnchantmentUltraRare() { enchantmentSize = 4; }
		}

		public class LifeStealEnchantmentBasic : Enchantments
		{
			LifeStealEnchantmentBasic() { enchantmentSize = 0; }
		}
		public class LifeStealEnchantmentCommon : Enchantments
		{
			LifeStealEnchantmentCommon() { enchantmentSize = 1; }
		}
		public class LifeStealEnchantmentRare : Enchantments
		{
			LifeStealEnchantmentRare() { enchantmentSize = 2; }
		}
		public class LifeStealEnchantmentSuperRare : Enchantments
		{
			LifeStealEnchantmentSuperRare() { enchantmentSize = 3; }
		}
		public class LifeStealEnchantmentUltraRare : Enchantments
		{
			LifeStealEnchantmentUltraRare() { enchantmentSize = 4; }
		}

		public class ArmorPenetrationEnchantmentBasic : Enchantments
		{
			ArmorPenetrationEnchantmentBasic() { enchantmentSize = 0; }
		}
		public class ArmorPenetrationEnchantmentCommon : Enchantments
		{
			ArmorPenetrationEnchantmentCommon() { enchantmentSize = 1; }
		}
		public class ArmorPenetrationEnchantmentRare : Enchantments
		{
			ArmorPenetrationEnchantmentRare() { enchantmentSize = 2; }
		}
		public class ArmorPenetrationEnchantmentSuperRare : Enchantments
		{
			ArmorPenetrationEnchantmentSuperRare() { enchantmentSize = 3; }
		}
		public class ArmorPenetrationEnchantmentUltraRare : Enchantments
		{
			ArmorPenetrationEnchantmentUltraRare() { enchantmentSize = 4; }
		}

		public class AllForOneEnchantmentUltraRare : Enchantments
		{
			AllForOneEnchantmentUltraRare() { enchantmentSize = 4; }
		}

		public class OneForAllEnchantmentUltraRare : Enchantments
		{
			OneForAllEnchantmentUltraRare() { enchantmentSize = 4; }
		}

		public class SpelunkerEnchantmentUltraRare : Enchantments
		{
			SpelunkerEnchantmentUltraRare() { enchantmentSize = 4; }
		}
	}
}
