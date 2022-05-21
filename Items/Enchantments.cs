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
using static WeaponEnchantments.Items.Containment;

namespace WeaponEnchantments.Items
{
	public class Enchantments : ModItem
	{
		public static bool cheating = false;
		public enum EnchantmentTypeID : int
        {
			Damage,
			Critical,
			Size,
			Speed,
			Defence,
			ManaCost,
			AmmoCost,
			LifeSteal,
			ArmorPenetration,
			AllForOne,
			OneForAll,
			Spelunker,
			DangerSense,
			Hunter,
			War,
			Peace,
			Splitting,
			GodSlayer,//change
			Ranged,//change
			Magic,//change
			Summon,//change
		}
		public enum UtilityEnchantmentNames
		{
			Size, 
			ManaCost, 
			AmmoCost, 
			LifeSteal, 
			Spelunker, 
			DangerSense, 
			Hunter, 
			War, 
			Peace
        }
		public enum DamageTypeSpecificID
		{
			Generic = 1,
			Melee = 2,
			Ranged = 3,
			Magic = 4,
			Summon = 5,
			SummonMeleeSpeed = 6,
			MagicSummonHybrid = 7,
			Throwing = 8
        }
		public static List<int[]> IDs { private set; get; } = new List<int[]>();
		public static readonly string[] rarity = new string[5] { "Basic", "Common", "Rare", "SuperRare", "UltraRare" };
		public static readonly Color[] rarityColors = new Color[5] { Color.White, Color.Green, Color.Blue, Color.Purple, Color.Orange };
		public int EnchantmentSize { private set; get; } = -1;
		public int EnchantmentType { private set; get; } = -1;
		public string EnchantmentTypeName { private set; get; }
		public float EnchantmentStrength { private set; get; }
		public bool Utility { private set; get; }
		public bool Unique { private set; get; }
		public bool Max1 { private set; get; } = false;
		public int damageClassSpecific { private set; get; }
		public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
        public override void SetStaticDefaults()
        {
			int[] arr = new int[rarity.Length];
            for(int i = 0; i < Enum.GetNames(typeof(EnchantmentTypeID)).Length; i++)
            {
				IDs.Add(arr);
            }
        }
        public override void SetDefaults()
		{
			if (EnchantmentSize > -1)
			{
				Item.maxStack = 99;
				EnchantmentTypeName = Name.Substring(0, Name.IndexOf("Enchantment"));
				for (int i = 0; i < Enum.GetNames(typeof(EnchantmentTypeID)).Length; i++)
				{
					if (EnchantmentTypeName == ((EnchantmentTypeID)i).ToString())
					{
						EnchantmentType = i;
						break;
					}
				}//Check EnchantmentType
				for (int i = 0; i < Enum.GetNames(typeof(UtilityEnchantmentNames)).Length; i++)
				{
					if (EnchantmentTypeName == ((UtilityEnchantmentNames)i).ToString())
					{
						Utility = true;
						break;
					}
				}//Check Utility
				for(int i = 0; i < ItemID.Count; i++)
                {
					if(ContentSamples.ItemsByType[i].Name == EnchantmentTypeName)
                    {
						Unique = true;
						break;
                    }
                }//Check Unique (Vanilla Items)
				if (EnchantmentSize < 2)
                {
					Item.width = 10 + 4 * (EnchantmentSize);
					Item.height = 10 + 4 * (EnchantmentSize);
				}//Width/Height
                else 
				{
					Item.width = 40;
					Item.height = 40;
				}//Width/Height
				switch ((EnchantmentTypeID)EnchantmentType)
				{
					case EnchantmentTypeID.Spelunker:
					case EnchantmentTypeID.DangerSense:
					case EnchantmentTypeID.Hunter:
						Item.value = (int)(500 * Math.Pow(8, EnchantmentSize - 2));
						break;
					default:
						Item.value = Utility ? (int)(500 * Math.Pow(8, EnchantmentSize)) : (int)(1000 * Math.Pow(8, EnchantmentSize));
						break;
				}//Base Value
                switch (EnchantmentSize)
                {
					case 3:
						Item.value += Containment.Values[2];
						break;
					case 4:
						Item.value += Containment.Values[2] + Stabilizer.Values[1] * 4;
						break;
					default:
						Item.value += Containment.Values[EnchantmentSize];
						break;
                }//Value - Containment/SuperiorStaibalizers
				switch ((EnchantmentTypeID)EnchantmentType)
                {
					case EnchantmentTypeID.Size:
					case EnchantmentTypeID.War:
					case EnchantmentTypeID.Peace:
					case EnchantmentTypeID.OneForAll:
					case EnchantmentTypeID.Splitting:
						switch (EnchantmentSize)
						{
							case 0:
								EnchantmentStrength = 0.1f;
								break;
							case 1:
								EnchantmentStrength = 0.2f;
								break;
							case 2:
								EnchantmentStrength = 0.50f;
								break;
							case 3:
								EnchantmentStrength = 0.8f;
								break;
							case 4:
								EnchantmentStrength = 1f;
								break;
						}
						break;
					case EnchantmentTypeID.Defence:
						switch (EnchantmentSize)
						{
							case 0:
								EnchantmentStrength = 1f;
								break;
							case 1:
								EnchantmentStrength = 2f;
								break;
							case 2:
								EnchantmentStrength = 3f;
								break;
							case 3:
								EnchantmentStrength = 5f;
								break;
							case 4:
								EnchantmentStrength = 10f;
								break;
						}
						break;
					case EnchantmentTypeID.ArmorPenetration:
						switch (EnchantmentSize)
						{
							case 0:
								EnchantmentStrength = 2f;
								break;
							case 1:
								EnchantmentStrength = 4f;
								break;
							case 2:
								EnchantmentStrength = 8f;
								break;
							case 3:
								EnchantmentStrength = 10f;
								break;
							case 4:
								EnchantmentStrength = 20f;
								break;
						}
						break;
					case EnchantmentTypeID.LifeSteal:
						switch (EnchantmentSize)
						{
							case 0:
								EnchantmentStrength = 0.005f;
								break;
							case 1:
								EnchantmentStrength = 0.01f;
								break;
							case 2:
								EnchantmentStrength = 0.02f;
								break;
							case 3:
								EnchantmentStrength = 0.03f;
								break;
							case 4:
								EnchantmentStrength = 0.04f;
								break;
						}
						break;
					case EnchantmentTypeID.AllForOne:
						switch (EnchantmentSize)
						{
							case 0:
								EnchantmentStrength = 1f;
								break;
							case 1:
								EnchantmentStrength = 2f;
								break;
							case 2:
								EnchantmentStrength = 5f;
								break;
							case 3:
								EnchantmentStrength = 8f;
								break;
							case 4:
								EnchantmentStrength = 10f;
								break;
						}
						break;
					case EnchantmentTypeID.GodSlayer:
						switch (EnchantmentSize)
						{
							case 0:
								EnchantmentStrength = 0.02f;
								break;
							case 1:
								EnchantmentStrength = 0.04f;
								break;
							case 2:
								EnchantmentStrength = 0.06f;
								break;
							case 3:
								EnchantmentStrength = 0.08f;
								break;
							case 4:
								EnchantmentStrength = 0.10f;
								break;
						}
						break;
					case EnchantmentTypeID.Ranged:
						switch (EnchantmentSize)
						{
							case 0:
								EnchantmentStrength = 0.03f;
								break;
							case 1:
								EnchantmentStrength = 0.08f;
								break;
							case 2:
								EnchantmentStrength = 0.16f;
								break;
							case 3:
								EnchantmentStrength = 0.25f;
								break;
							case 4:
								EnchantmentStrength = 0.40f;
								break;
						}
						break;
					case EnchantmentTypeID.Magic:
						switch (EnchantmentSize)
						{
							case 0:
								EnchantmentStrength = 0.03f;
								break;
							case 1:
								EnchantmentStrength = 0.08f;
								break;
							case 2:
								EnchantmentStrength = 0.16f;
								break;
							case 3:
								EnchantmentStrength = 0.25f;
								break;
							case 4:
								EnchantmentStrength = 0.40f;
								break;
						}
						break;
					case EnchantmentTypeID.Summon:
						switch (EnchantmentSize)
						{
							case 0:
								EnchantmentStrength = 0.03f;
								break;
							case 1:
								EnchantmentStrength = 0.08f;
								break;
							case 2:
								EnchantmentStrength = 0.16f;
								break;
							case 3:
								EnchantmentStrength = 0.25f;
								break;
							case 4:
								EnchantmentStrength = 0.40f;
								break;
						}
						break;
					default:
						switch (EnchantmentSize)
						{
							case 0:
								EnchantmentStrength = 0.03f;
								break;
							case 1:
								EnchantmentStrength = 0.08f;
								break;
							case 2:
								EnchantmentStrength = 0.16f;
								break;
							case 3:
								EnchantmentStrength = 0.25f;
								break;
							case 4:
								EnchantmentStrength = 0.40f;
								break;
						}
						break;
                }//EnchantmentStrength
				switch ((EnchantmentTypeID)EnchantmentType)
				{
					case EnchantmentTypeID.GodSlayer:
						damageClassSpecific = (int)DamageTypeSpecificID.Melee;
						break;
					case EnchantmentTypeID.Ranged:
						damageClassSpecific = (int)DamageTypeSpecificID.Ranged;
						break;
					case EnchantmentTypeID.Magic:
						damageClassSpecific = (int)DamageTypeSpecificID.Magic;
						break;
					case EnchantmentTypeID.Summon:
						damageClassSpecific = (int)DamageTypeSpecificID.Summon;
						break;
					case EnchantmentTypeID.AllForOne:
					case EnchantmentTypeID.OneForAll:
						Max1 = true;
						break;
					default:
						damageClassSpecific = 0;
						break;
				}//DamageTypeSpecific
				if (damageClassSpecific > 0 || Unique)
                {
					string limmitationToolTip;
					switch ((EnchantmentTypeID)EnchantmentType)
					{
						case EnchantmentTypeID.GodSlayer:
							limmitationToolTip = "\n   *Melee Only*";
							break;
						case EnchantmentTypeID.Ranged:
							limmitationToolTip = "\n   *Ranged Only*";
							break;
						case EnchantmentTypeID.Magic:
							limmitationToolTip = "\n   *Magic Only*";
							break;
						case EnchantmentTypeID.Summon:
							limmitationToolTip = "\n   *Summon Only*";
							break;
						default:
							limmitationToolTip = "\n   *" + Item.ModItem.DisplayName + " Only*";
							break;
					}//DamageTypeSpecific
					limmitationToolTip += "\n   *Unique*\n(Limmited to 1 Unique Enchantment)";
					switch ((EnchantmentTypeID)EnchantmentType)
                    {
						case EnchantmentTypeID.GodSlayer:
							Tooltip.SetDefault((EnchantmentStrength * 100).ToString() + "% God Slayer Bonus\n(Bonus damage based on enemy max hp)\n(Bonus damage not affected by LifeSteal against bosses)\nLevel cost: " + GetLevelCost().ToString() + limmitationToolTip);
							break;
						default:
							Tooltip.SetDefault("+" + (EnchantmentStrength * 100).ToString() + "% " + EnchantmentTypeName + "\nLevel cost: " + GetLevelCost().ToString() + limmitationToolTip);
							break;
					}//Unique ToolTips
				}
                else
                {
					switch ((EnchantmentTypeID)EnchantmentType)
					{
						case EnchantmentTypeID.Size:
							Tooltip.SetDefault("+" + (EnchantmentStrength * 50).ToString() + "% " + EnchantmentTypeName + "\n+" + (EnchantmentStrength * 100).ToString() + "% Knockback" + "\nLevel cost: " + GetLevelCost().ToString());
							break;
						case EnchantmentTypeID.Defence:
							Tooltip.SetDefault("+" + EnchantmentStrength.ToString() + " " + EnchantmentTypeName + "\nLevel cost: " + GetLevelCost().ToString());
							break;
						case EnchantmentTypeID.ArmorPenetration:
							Tooltip.SetDefault(EnchantmentStrength.ToString() + " Armor Penetration\nLevel cost: " + GetLevelCost().ToString());
							break;
						case EnchantmentTypeID.ManaCost:
							Tooltip.SetDefault("-" + (EnchantmentStrength * 100).ToString() + "% Mana Cost\nLevel cost: " + GetLevelCost().ToString());
							break;
						case EnchantmentTypeID.AmmoCost:
							Tooltip.SetDefault("-" + (EnchantmentStrength * 100).ToString() + "% Chance to consume ammo\nLevel cost: " + GetLevelCost().ToString());
							break;
						case EnchantmentTypeID.LifeSteal:
							Tooltip.SetDefault((EnchantmentStrength * 100).ToString() + "% Life Steal (remainder is saved to prevent \nalways rounding to 0 for low damage weapons)\nLevel cost: " + GetLevelCost().ToString());
							break;
						case EnchantmentTypeID.AllForOne:
							Tooltip.SetDefault(EnchantmentStrength + "x Damage, item CD equal to " + EnchantmentStrength * 0.8f + "x use speed\n" + EnchantmentStrength * 0.4f + "x mana cost\n   *Weapons Only*\n   *Max of 1 per weapon*\nLevel cost: " + GetLevelCost().ToString());
							break;
						case EnchantmentTypeID.OneForAll:
							Tooltip.SetDefault("Hiting an enemy will damage all nearby enemies by " + (EnchantmentStrength * 100).ToString() + "% of damage dealt, " + (30f * EnchantmentStrength).ToString() + "% reduced base attack speed\n   *Weapons Only*\n   *Max of 1 per weapon*\nLevel cost: " + GetLevelCost().ToString());
							break;
						case EnchantmentTypeID.Spelunker:
							Tooltip.SetDefault("Grants the Spelunker buff\nLevel cost: " + GetLevelCost().ToString());
							break;
						case EnchantmentTypeID.DangerSense:
							Tooltip.SetDefault("Grants the Danger Sense buff\nLevel cost: " + GetLevelCost().ToString());
							break;
						case EnchantmentTypeID.Hunter:
							Tooltip.SetDefault("Grants the Hunter buff\nLevel cost: " + GetLevelCost().ToString());
							break;
						case EnchantmentTypeID.War:
							Tooltip.SetDefault((EnchantmentStrength + 1f).ToString() + "x enemy spawn rate and max enemies\nLevel cost: " + GetLevelCost().ToString());
							break;
						case EnchantmentTypeID.Peace:
							Tooltip.SetDefault((1f / (EnchantmentStrength + 1f)).ToString() + "x enemy spawn rate and max enemies\nLevel cost: " + GetLevelCost().ToString());
							break;
						case EnchantmentTypeID.Splitting:
							Tooltip.SetDefault("+" + (EnchantmentStrength * 100).ToString() + "% to produce an extra projectile.\nLevel cost: " + GetLevelCost().ToString());
							break;
						default:
							Tooltip.SetDefault("+" + (EnchantmentStrength * 100).ToString() + "% " + EnchantmentTypeName + "\nLevel cost: " + GetLevelCost().ToString());
							break;
					}//Normal ToolTips
				}
			}
		}
		public override void AddRecipes()
		{
			if (EnchantmentSize > -1)
			{
				for(int i = EnchantmentSize; i < rarity.Length; i++)
                {
					Recipe recipe;
					int skipIfLessOrEqualToSize;
					switch ((EnchantmentTypeID)EnchantmentType)
                    {
						case EnchantmentTypeID.Spelunker:
						case EnchantmentTypeID.DangerSense:
						case EnchantmentTypeID.Hunter:
							skipIfLessOrEqualToSize = 4;
							break;
						case EnchantmentTypeID.Damage:
						case EnchantmentTypeID.Defence:
							skipIfLessOrEqualToSize = -1;
							break;
						default:
							skipIfLessOrEqualToSize = 0;
							break;
					}
					if(EnchantmentSize > skipIfLessOrEqualToSize)
                    {
						for (int j = EnchantmentSize; j >= skipIfLessOrEqualToSize + 1; j--)
						{
							recipe = CreateRecipe();
							for (int k = EnchantmentSize; k >= j; k--)
							{
								int essenceNumber = Utility ? 5 : 10;
								recipe.AddIngredient(Mod, "EnchantmentEssence" + EnchantmentEssence.rarity[k], essenceNumber);
							}
							if (j > 0)
							{
								recipe.AddIngredient(Mod, EnchantmentTypeName + "Enchantment" + rarity[j - 1], 1);
							}
							if (EnchantmentSize < 3)
							{
								recipe.AddIngredient(Mod, Containment.sizes[EnchantmentSize] + "Containment", 1);
							}
							else if (j < 3)
							{
								recipe.AddIngredient(Mod, Containment.sizes[2] + "Containment", 1);
							}
							if (EnchantmentSize == 4)
							{
								recipe.AddIngredient(ModContent.ItemType<SuperiorStabilizer>(), 4);
							}
							recipe.AddTile(Mod, EnchantingTableItem.enchantingTableNames[i] + "EnchantingTable");
							recipe.Register();
						}
					}
				}
				IDs[EnchantmentType][EnchantmentSize] = Type;
                if (cheating)
                {
					Mod.CreateRecipe(Type, 1).AddTile(TileID.WoodBlock).Register();
				}
			}
		}
		public int GetLevelCost()
        {
            switch ((EnchantmentTypeID)EnchantmentType)
            {
				case EnchantmentTypeID.AllForOne:
				case EnchantmentTypeID.OneForAll:
				case EnchantmentTypeID.Splitting:
					return (1 + EnchantmentSize) * 3;
				default:
					return Utility ? 1 + EnchantmentSize : (1 + EnchantmentSize) * 2;
			}
        }
        public class OmniEnchantment : Enchantments
		{
			OmniEnchantment() { EnchantmentSize = -1; }
			
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
			private int[] freeItems = new int[] {
				437, 
				3380, 
				193, 
				1225, 
				520, 
				521, 
				2786, 
				3531, 
				4365, 
				4735, 
				346, 
				87, 
				3813, 
				4076, 
				514, 
				561, 
				4281, 
				5114, 
				1309, 
				ItemID.WoodenBoomerang, 
				ItemID.FallenStar, 
				ItemID.TerraBlade, 
				ItemID.TrueNightsEdge, 
				ItemID.TrueExcalibur, 
				ItemID.BrokenHeroSword, 
				ItemID.MythrilAnvil,
				ItemID.SuspiciousLookingEye
			};
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
			DamageEnchantmentBasic() { EnchantmentSize = 0; }
		}
		public class DamageEnchantmentCommon : Enchantments
		{
			DamageEnchantmentCommon() { EnchantmentSize = 1; }
		}
		public class DamageEnchantmentRare : Enchantments
		{
			DamageEnchantmentRare() { EnchantmentSize = 2; }
		}
		public class DamageEnchantmentSuperRare : Enchantments
		{
			DamageEnchantmentSuperRare() { EnchantmentSize = 3; }
		}
		public class DamageEnchantmentUltraRare : Enchantments
		{
			DamageEnchantmentUltraRare() { EnchantmentSize = 4; }
		}

		public class CriticalEnchantmentBasic : Enchantments
		{
			CriticalEnchantmentBasic() { EnchantmentSize = 0; }
		}
		public class CriticalEnchantmentCommon : Enchantments
		{
			CriticalEnchantmentCommon() { EnchantmentSize = 1; }
		}
		public class CriticalEnchantmentRare : Enchantments
		{
			CriticalEnchantmentRare() { EnchantmentSize = 2; }
		}
		public class CriticalEnchantmentSuperRare : Enchantments
		{
			CriticalEnchantmentSuperRare() { EnchantmentSize = 3; }
		}
		public class CriticalEnchantmentUltraRare : Enchantments
		{
			CriticalEnchantmentUltraRare() { EnchantmentSize = 4; }
		}

		public class SizeEnchantmentBasic : Enchantments
		{
			SizeEnchantmentBasic() { EnchantmentSize = 0; }
		}
		public class SizeEnchantmentCommon : Enchantments
		{
			SizeEnchantmentCommon() { EnchantmentSize = 1; }
		}
		public class SizeEnchantmentRare : Enchantments
		{
			SizeEnchantmentRare() { EnchantmentSize = 2; }
		}
		public class SizeEnchantmentSuperRare : Enchantments
		{
			SizeEnchantmentSuperRare() { EnchantmentSize = 3; }
		}
		public class SizeEnchantmentUltraRare : Enchantments
		{
			SizeEnchantmentUltraRare() { EnchantmentSize = 4; }
		}

		public class SpeedEnchantmentBasic : Enchantments
		{
			SpeedEnchantmentBasic() { EnchantmentSize = 0; }
		}
		public class SpeedEnchantmentCommon : Enchantments
		{
			SpeedEnchantmentCommon() { EnchantmentSize = 1; }
		}
		public class SpeedEnchantmentRare : Enchantments
		{
			SpeedEnchantmentRare() { EnchantmentSize = 2; }
		}
		public class SpeedEnchantmentSuperRare : Enchantments
		{
			SpeedEnchantmentSuperRare() { EnchantmentSize = 3; }
		}
		public class SpeedEnchantmentUltraRare : Enchantments
		{
			SpeedEnchantmentUltraRare() { EnchantmentSize = 4; }
		}

		public class DefenceEnchantmentBasic : Enchantments
		{
			DefenceEnchantmentBasic() { EnchantmentSize = 0; }
		}
		public class DefenceEnchantmentCommon : Enchantments
		{
			DefenceEnchantmentCommon() { EnchantmentSize = 1; }
		}
		public class DefenceEnchantmentRare : Enchantments
		{
			DefenceEnchantmentRare() { EnchantmentSize = 2; }
		}
		public class DefenceEnchantmentSuperRare : Enchantments
		{
			DefenceEnchantmentSuperRare() { EnchantmentSize = 3; }
		}
		public class DefenceEnchantmentUltraRare : Enchantments
		{
			DefenceEnchantmentUltraRare() { EnchantmentSize = 4; }
		}

		public class ManaCostEnchantmentBasic : Enchantments
		{
			ManaCostEnchantmentBasic() { EnchantmentSize = 0; }
		}
		public class ManaCostEnchantmentCommon : Enchantments
		{
			ManaCostEnchantmentCommon() { EnchantmentSize = 1; }
		}
		public class ManaCostEnchantmentRare : Enchantments
		{
			ManaCostEnchantmentRare() { EnchantmentSize = 2; }
		}
		public class ManaCostEnchantmentSuperRare : Enchantments
		{
			ManaCostEnchantmentSuperRare() { EnchantmentSize = 3; }
		}
		public class ManaCostEnchantmentUltraRare : Enchantments
		{
			ManaCostEnchantmentUltraRare() { EnchantmentSize = 4; }
		}

		public class AmmoCostEnchantmentBasic : Enchantments
		{
			AmmoCostEnchantmentBasic() { EnchantmentSize = 0; }
		}
		public class AmmoCostEnchantmentCommon : Enchantments
		{
			AmmoCostEnchantmentCommon() { EnchantmentSize = 1; }
		}
		public class AmmoCostEnchantmentRare : Enchantments
		{
			AmmoCostEnchantmentRare() { EnchantmentSize = 2; }
		}
		public class AmmoCostEnchantmentSuperRare : Enchantments
		{
			AmmoCostEnchantmentSuperRare() { EnchantmentSize = 3; }
		}
		public class AmmoCostEnchantmentUltraRare : Enchantments
		{
			AmmoCostEnchantmentUltraRare() { EnchantmentSize = 4; }
		}

		public class LifeStealEnchantmentBasic : Enchantments
		{
			LifeStealEnchantmentBasic() { EnchantmentSize = 0; }
		}
		public class LifeStealEnchantmentCommon : Enchantments
		{
			LifeStealEnchantmentCommon() { EnchantmentSize = 1; }
		}
		public class LifeStealEnchantmentRare : Enchantments
		{
			LifeStealEnchantmentRare() { EnchantmentSize = 2; }
		}
		public class LifeStealEnchantmentSuperRare : Enchantments
		{
			LifeStealEnchantmentSuperRare() { EnchantmentSize = 3; }
		}
		public class LifeStealEnchantmentUltraRare : Enchantments
		{
			LifeStealEnchantmentUltraRare() { EnchantmentSize = 4; }
		}

		public class ArmorPenetrationEnchantmentBasic : Enchantments
		{
			ArmorPenetrationEnchantmentBasic() { EnchantmentSize = 0; }
		}
		public class ArmorPenetrationEnchantmentCommon : Enchantments
		{
			ArmorPenetrationEnchantmentCommon() { EnchantmentSize = 1; }
		}
		public class ArmorPenetrationEnchantmentRare : Enchantments
		{
			ArmorPenetrationEnchantmentRare() { EnchantmentSize = 2; }
		}
		public class ArmorPenetrationEnchantmentSuperRare : Enchantments
		{
			ArmorPenetrationEnchantmentSuperRare() { EnchantmentSize = 3; }
		}
		public class ArmorPenetrationEnchantmentUltraRare : Enchantments
		{
			ArmorPenetrationEnchantmentUltraRare() { EnchantmentSize = 4; }
		}

		public class AllForOneEnchantmentBasic : Enchantments
		{
			AllForOneEnchantmentBasic() { EnchantmentSize = 0; }
		}
		public class AllForOneEnchantmentCommon : Enchantments
		{
			AllForOneEnchantmentCommon() { EnchantmentSize = 1; }
		}
		public class AllForOneEnchantmentRare : Enchantments
		{
			AllForOneEnchantmentRare() { EnchantmentSize = 2; }
		}
		public class AllForOneEnchantmentSuperRare : Enchantments
		{
			AllForOneEnchantmentSuperRare() { EnchantmentSize = 3; }
		}
		public class AllForOneEnchantmentUltraRare : Enchantments
		{
			AllForOneEnchantmentUltraRare() { EnchantmentSize = 4; }
		}

		public class OneForAllEnchantmentBasic : Enchantments
		{
			OneForAllEnchantmentBasic() { EnchantmentSize = 0; }
		}
		public class OneForAllEnchantmentCommon : Enchantments
		{
			OneForAllEnchantmentCommon() { EnchantmentSize = 1; }
		}
		public class OneForAllEnchantmentRare : Enchantments
		{
			OneForAllEnchantmentRare() { EnchantmentSize = 2; }
		}
		public class OneForAllEnchantmentSuperRare : Enchantments
		{
			OneForAllEnchantmentSuperRare() { EnchantmentSize = 3; }
		}
		public class OneForAllEnchantmentUltraRare : Enchantments
		{
			OneForAllEnchantmentUltraRare() { EnchantmentSize = 4; }
		}

		public class SpelunkerEnchantmentUltraRare : Enchantments
		{
			SpelunkerEnchantmentUltraRare() { EnchantmentSize = 4; }
		}

		public class DangerSenseEnchantmentUltraRare : Enchantments
		{
			DangerSenseEnchantmentUltraRare() { EnchantmentSize = 4; }
		}

		public class HunterEnchantmentUltraRare : Enchantments
		{
			HunterEnchantmentUltraRare() { EnchantmentSize = 4; }
		}

		public class WarEnchantmentBasic : Enchantments
		{
			WarEnchantmentBasic() { EnchantmentSize = 0; }
		}
		public class WarEnchantmentCommon : Enchantments
		{
			WarEnchantmentCommon() { EnchantmentSize = 1; }
		}
		public class WarEnchantmentRare : Enchantments
		{
			WarEnchantmentRare() { EnchantmentSize = 2; }
		}
		public class WarEnchantmentSuperRare : Enchantments
		{
			WarEnchantmentSuperRare() { EnchantmentSize = 3; }
		}
		public class WarEnchantmentUltraRare : Enchantments
		{
			WarEnchantmentUltraRare() { EnchantmentSize = 4; }
		}

		public class PeaceEnchantmentBasic : Enchantments
		{
			PeaceEnchantmentBasic() { EnchantmentSize = 0; }
		}
		public class PeaceEnchantmentCommon : Enchantments
		{
			PeaceEnchantmentCommon() { EnchantmentSize = 1; }
		}
		public class PeaceEnchantmentRare : Enchantments
		{
			PeaceEnchantmentRare() { EnchantmentSize = 2; }
		}
		public class PeaceEnchantmentSuperRare : Enchantments
		{
			PeaceEnchantmentSuperRare() { EnchantmentSize = 3; }
		}
		public class PeaceEnchantmentUltraRare : Enchantments
		{
			PeaceEnchantmentUltraRare() { EnchantmentSize = 4; }
		}

		public class GodSlayerEnchantmentBasic : Enchantments
		{
			GodSlayerEnchantmentBasic() { EnchantmentSize = 0; }
		}
		public class GodSlayerEnchantmentCommon : Enchantments
		{
			GodSlayerEnchantmentCommon() { EnchantmentSize = 1; }
		}
		public class GodSlayerEnchantmentRare : Enchantments
		{
			GodSlayerEnchantmentRare() { EnchantmentSize = 2; }
		}
		public class GodSlayerEnchantmentSuperRare : Enchantments
		{
			GodSlayerEnchantmentSuperRare() { EnchantmentSize = 3; }
		}
		public class GodSlayerEnchantmentUltraRare : Enchantments
		{
			GodSlayerEnchantmentUltraRare() { EnchantmentSize = 4; }
		}

		public class SplittingEnchantmentBasic : Enchantments
		{
			SplittingEnchantmentBasic() { EnchantmentSize = 0; }
		}
		public class SplittingEnchantmentCommon : Enchantments
		{
			SplittingEnchantmentCommon() { EnchantmentSize = 1; }
		}
		public class SplittingEnchantmentRare : Enchantments
		{
			SplittingEnchantmentRare() { EnchantmentSize = 2; }
		}
		public class SplittingEnchantmentSuperRare : Enchantments
		{
			SplittingEnchantmentSuperRare() { EnchantmentSize = 3; }
		}
		public class SplittingEnchantmentUltraRare : Enchantments
		{
			SplittingEnchantmentUltraRare() { EnchantmentSize = 4; }
		}
	}
}
