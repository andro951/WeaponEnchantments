using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using WeaponEnchantments.Items;
using WeaponEnchantments.UI;

namespace WeaponEnchantments.Common.Globals
{
	public class RecipeEditor : GlobalRecipe
    {
		public override void Load()
		{
			IL.Terraria.Recipe.FindRecipes += HookFindRecipes;
			IL.Terraria.Recipe.Create += HookCreate;
		}
		public static int counter = 0;
		private const bool debuggingHookCreate = false;
		private const bool debuggingHookFindRecipes = false;
		private static Item OnUseItemAsIngredient(Item arrItem)
		{
			if (!arrItem.IsAir)
			{
				EnchantedItem miGlobal = Main.mouseItem.GetGlobalItem<EnchantedItem>();
				EnchantedItem i2Global = arrItem.GetGlobalItem<EnchantedItem>();
				if (i2Global.experience > 0 || i2Global.powerBoosterInstalled)
				{
					if (WEMod.IsEnchantable(Main.mouseItem))
					{
						miGlobal.experience += i2Global.experience;
						if (i2Global.powerBoosterInstalled)
						{
							if (!miGlobal.powerBoosterInstalled)
							{
								miGlobal.powerBoosterInstalled = true;
							}
							else
							{
								Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), ModContent.ItemType<PowerBooster>(), 1);
							}
						}
						miGlobal.UpdateLevel();
						int j;
						for (j = 0; j <= EnchantingTable.maxEnchantments; j++)
						{
							if (j > 4)
								break;
							if (miGlobal.enchantments[j].IsAir)
								break;
						}
						for (int k = 0; k < EnchantingTable.maxEnchantments; k++)
						{
							if (!i2Global.enchantments[k].IsAir)
							{
								AllForOneEnchantmentBasic enchantment = ((AllForOneEnchantmentBasic)i2Global.enchantments[k].ModItem);
								int uniqueItemSlot = WEUIItemSlot.FindSwapEnchantmentSlot(enchantment, Main.mouseItem);
								bool cantFit = false;
								if (enchantment.GetLevelCost() < miGlobal.GetLevelsAvailable())
								{
									if(uniqueItemSlot == -1)
                                    {
										if (enchantment.Utility && miGlobal.enchantments[4].IsAir)
										{
											miGlobal.enchantments[4] = i2Global.enchantments[k].Clone();
										}
										else if (j < 4)
										{
											miGlobal.enchantments[j] = i2Global.enchantments[k].Clone();
											j++;
										}
										else
										{
											cantFit = true;
										}
									}
                                    else
                                    {
										cantFit = true;
                                    }
								}
								else
								{
									cantFit = true;
								}
								if (cantFit)
								{
									Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), i2Global.enchantments[k].type, 1);
								}
							}
						}
					}
					else
					{
						miGlobal.experience += i2Global.experience;
						int numberEssenceRecieved;
						int xpCounter = miGlobal.experience;
						for (int tier = EnchantingTable.maxEssenceItems - 1; tier >= 0; tier--)
						{
							numberEssenceRecieved = xpCounter / (int)EnchantmentEssenceBasic.xpPerEssence[tier] * 4 / 5;
							xpCounter -= (int)EnchantmentEssenceBasic.xpPerEssence[tier] * numberEssenceRecieved;
							if (xpCounter < (int)EnchantmentEssenceBasic.xpPerEssence[0] && xpCounter > 0 && tier == 0)
							{
								xpCounter = 0;
								numberEssenceRecieved += 1;
							}
							Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), EnchantmentEssenceBasic.IDs[tier], 1);
						}
						if (i2Global.powerBoosterInstalled)
						{
							Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), ModContent.ItemType<PowerBooster>(), 1);
						}
						for (int k = 0; k < EnchantingTable.maxEnchantments; k++)
						{
							if (!i2Global.enchantments[k].IsAir)
							{
								Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), i2Global.enchantments[k].type, 1);
							}
						}
					}
				}
                else
                {
					if(arrItem.ModItem is AllForOneEnchantmentBasic)
                    {
						int size = ((AllForOneEnchantmentBasic)arrItem.ModItem).EnchantmentSize;
						if (size < 3)
                        {
							Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), Containment.IDs[size], 1);
						}

					}
					else if(arrItem.ModItem is Containment containment)
					{
						if(containment.size == 2 && Main.mouseItem.type == Containment.barIDs[0, 2])
							Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), 180, 4);
					}
                }
			}
			if (debuggingHookCreate)
			{
				counter++;
				Main.NewText("counter: " + counter.ToString() + " item2.name: " + arrItem.Name);
				ModContent.GetInstance<WEMod>().Logger.Info("counter: " + counter.ToString() + " item2.name: " + arrItem.Name);
			}
			return arrItem;
		}
		private static void HookCreate(ILContext il)
		{
			counter = 0;
			var c = new ILCursor(il);

			if (debuggingHookCreate)
			{
				while (c.Next != null)
				{
					bool catchingExceptions = true;
					ModContent.GetInstance<WEMod>().Logger.Info("c.Index: " + c.Index.ToString() + " Instruction: " + c.Next.ToString());
					while (catchingExceptions)
					{
						c.Index++;
						try
						{
							if (c.Next != null)
							{
								string tempString = c.Next.ToString();
							}
							catchingExceptions = false;
						}
						catch (Exception e)
						{
							ModContent.GetInstance<WEMod>().Logger.Info("c.Index: " + c.Index.ToString() + " exception: " + e.ToString().Substring(0, 20));
						}
					}
				}
				c.Index = 0;



				bool searching = true;
				int line = 0;
				int j = 0;
				int jNext = c.Context.ToString().Substring(0).IndexOf("IL_");
				while (searching)
				{
					j = jNext;
					//Debug.WriteLine("length: " + c.Context.ToString().Length.ToString() + " jNext + 1: " + (jNext + 1).ToString());
					jNext = c.Context.ToString().Substring(jNext + 1).IndexOf("IL_") + j + 1;
					//Debug.WriteLine("substring: " + c.Context.ToString().Substring(j, jNext - j - 2) + ", length: " + c.Context.ToString().Substring(j, jNext - j - 2).Length.ToString());
					if (jNext == j)
					{
						ModContent.GetInstance<WEMod>().Logger.Info(line + " " + c.Context.ToString().Substring(j));
						//Debug.WriteLine(line + " " + c.Context.ToString().Substring(j));
						searching = false;
					}
					else
					{
						ModContent.GetInstance<WEMod>().Logger.Info(line + " " + c.Context.ToString().Substring(j, jNext - j - 2));
						//Debug.WriteLine(line + " " + c.Context.ToString().Substring(j, jNext - j - 2));
					}
					line++;
				}
			}

			if (!c.TryGotoNext(MoveType.After,
				i => i.MatchLdarg(0),
				i => i.MatchLdloc(1),
				i => i.MatchLdfld(out _),
				i => i.MatchLdloc(3),
				i => i.MatchLdfld(out _),
				i => i.MatchCall(out _),
				i => i.MatchBrfalse(out _),

				i => i.MatchLdloc(1)
			)) { throw new Exception("Failed to find instructions HookCreate 1"); }

			if (debuggingHookCreate) try { ModContent.GetInstance<WEMod>().Logger.Info("c.Index: " + c.Index.ToString() + " Instruction: " + c.Next.ToString()); } catch (Exception e) { ModContent.GetInstance<WEMod>().Logger.Info("c.Index: " + c.Index.ToString() + " exception: " + e.ToString()); }

			if (debuggingHookCreate) try { ModContent.GetInstance<WEMod>().Logger.Info("c.Index: " + c.Index.ToString() + " Instruction: " + c.Next.ToString()); } catch (Exception e) { ModContent.GetInstance<WEMod>().Logger.Info("c.Index: " + c.Index.ToString() + " exception: " + e.ToString()); }

			c.EmitDelegate((Item arrItem) => OnUseItemAsIngredient(arrItem));

			if (!c.TryGotoNext(MoveType.Before,
				i => i.MatchLdsfld(out _),
				i => i.MatchLdsfld(out _),
				i => i.MatchLdelemRef(),
				i => i.MatchLdfld(out _),
				i => i.MatchLdcI4(-1),
				i => i.MatchBeq(out _)

			)) { throw new Exception("Failed to find instructions HookCreate 3"); }

			var incomingLabels = c.IncomingLabels;
			foreach (ILLabel cursorIncomingLabel in incomingLabels)
				c.MarkLabel(cursorIncomingLabel);

			c.Emit(OpCodes.Ldloc, 3);
			c.Emit(OpCodes.Ldloc, 4);

			c.EmitDelegate((Item item, int num) =>
			{
				WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
                if (wePlayer.usingEnchantingTable)
                {
					for (int i = 0; i < EnchantingTable.maxEssenceItems; i++)
					{
						Item slotItem = wePlayer.enchantingTableUI.essenceSlotUI[i].Item;
						if (item.type == slotItem.type)
						{
							slotItem.stack -= num;
						}
					}
				}
			});

			if (!c.TryGotoNext(MoveType.After,
				i => i.MatchLdarg(0),
				i => i.MatchLdloc(1),
				i => i.MatchLdfld(out _),
				i => i.MatchLdloc(3),
				i => i.MatchLdfld(out _),
				i => i.MatchCall(out _),
				i => i.MatchBrfalse(out _),

				i => i.MatchLdloc(1)
			)) { throw new Exception("Failed to find instructions HookCreate 2"); }

			if (debuggingHookCreate) try { ModContent.GetInstance<WEMod>().Logger.Info("c.Index: " + c.Index.ToString() + " Instruction: " + c.Next.ToString()); } catch (Exception e) { ModContent.GetInstance<WEMod>().Logger.Info("c.Index: " + c.Index.ToString() + " exception: " + e.ToString()); }

			if (debuggingHookCreate) try { ModContent.GetInstance<WEMod>().Logger.Info("c.Index: " + c.Index.ToString() + " Instruction: " + c.Next.ToString()); } catch (Exception e) { ModContent.GetInstance<WEMod>().Logger.Info("c.Index: " + c.Index.ToString() + " exception: " + e.ToString()); }
			c.EmitDelegate((Item arrItem) => OnUseItemAsIngredient(arrItem));
		}
		private static void HookFindRecipes(ILContext il)
		{
			var c = new ILCursor(il);
			
            if (debuggingHookFindRecipes)
            {
				while (c.Next != null)
				{
					bool catchingExceptions = true;
					ModContent.GetInstance<WEMod>().Logger.Info("c.Index: " + c.Index.ToString() + " Instruction: " + c.Next.ToString());
					while (catchingExceptions)
					{
						c.Index++;
						try
						{
							if (c.Next != null)
							{
								string tempString = c.Next.ToString();
							}
							catchingExceptions = false;
						}
						catch (Exception e)
						{
							ModContent.GetInstance<WEMod>().Logger.Info("c.Index: " + c.Index.ToString() + " exception: " + e.ToString().Substring(0, 20));
						}
					}
				}
				c.Index = 0;



				bool searching = true;
				int line = 0;
				int j = 0;
				int jNext = c.Context.ToString().Substring(0).IndexOf("IL_");
				while (searching)
				{
					j = jNext;
					//Debug.WriteLine("length: " + c.Context.ToString().Length.ToString() + " jNext + 1: " + (jNext + 1).ToString());
					jNext = c.Context.ToString().Substring(jNext + 1).IndexOf("IL_") + j + 1;
					//Debug.WriteLine("substring: " + c.Context.ToString().Substring(j, jNext - j - 2) + ", length: " + c.Context.ToString().Substring(j, jNext - j - 2).Length.ToString());
					if (jNext == j)
					{
						ModContent.GetInstance<WEMod>().Logger.Info(line + " " + c.Context.ToString().Substring(j));
						//Debug.WriteLine(line + " " + c.Context.ToString().Substring(j));
						searching = false;
					}
					else
					{
						ModContent.GetInstance<WEMod>().Logger.Info(line + " " + c.Context.ToString().Substring(j, jNext - j - 2));
						//Debug.WriteLine(line + " " + c.Context.ToString().Substring(j, jNext - j - 2));
					}
					line++;
				}
			}
			
			if (!c.TryGotoNext(MoveType.After,
				i => i.MatchLdloc(12),
				i => i.MatchLdcI4(1),
				i => i.MatchAdd(),
				i => i.MatchStloc(12),
				i => i.MatchLdloc(12),
				i => i.MatchLdcI4(40),
				i => i.MatchBlt(out _)
			)) { throw new Exception("Failed to find instructions HookFindRecipes"); }

			if (debuggingHookFindRecipes)try{ModContent.GetInstance<WEMod>().Logger.Info("c.Index: " + c.Index.ToString() + " Instruction: " + c.Next.ToString());}catch (Exception e){ModContent.GetInstance<WEMod>().Logger.Info("c.Index: " + c.Index.ToString() + " exception: " + e.ToString());}
			
			var incomingLabels = c.IncomingLabels;
			foreach (ILLabel cursorIncomingLabel in incomingLabels)
				c.MarkLabel(cursorIncomingLabel);
			
			if (debuggingHookFindRecipes)try{ModContent.GetInstance<WEMod>().Logger.Info("c.Index: " + c.Index.ToString() + " Instruction: " + c.Next.ToString()); } catch (Exception e) { ModContent.GetInstance<WEMod>().Logger.Info("c.Index: " + c.Index.ToString() + " exception: " + e.ToString()); }

			c.Emit(OpCodes.Ldloc, 6);
			c.EmitDelegate((Dictionary<int, int> dictionary) =>
			{
				WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
				if (wePlayer.usingEnchantingTable)
				{
                    if (debuggingHookFindRecipes)
                    {
						counter++;
						Main.NewText("counter: " + counter.ToString());
						ModContent.GetInstance<WEMod>().Logger.Info("counter: " + counter.ToString());
					}
					for(int i = 0; i < EnchantingTable.maxEssenceItems; i++)
					{
						Item item = wePlayer.enchantingTableUI.essenceSlotUI[i].Item;
						if (item != null && item.stack > 0)
						{
							if (dictionary.ContainsKey(item.netID))
								dictionary[item.netID] += item.stack;
							else
								dictionary[item.netID] = item.stack;
						}
					}
				}
			});
		}
	}
}
