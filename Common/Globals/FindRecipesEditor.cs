using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace WeaponEnchantments.Common.Globals
{
	public class FindRecipesEditor : GlobalRecipe
    {
		public override void Load()
		{
			IL.Terraria.Recipe.FindRecipes += HookFindRecipes;
			//This one doesn't break with RecursiveCraft
			//You should also add sortAfter = RecursiveCraft to your build.txt
			//So your edit applies after the recursive craft one
			//So your delegate runs before the recursive craft one
		}
		public static int counter = 0;
		private const bool debugging = false;

		private static void HookFindRecipes(ILContext il)
		{
			var c = new ILCursor(il);
			
            if (debugging)
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
			))
			if(debugging){throw new Exception("Failed to find instructions");}try{ModContent.GetInstance<WEMod>().Logger.Info("c.Index: " + c.Index.ToString() + " Instruction: " + c.Next.ToString());}catch (Exception e){ModContent.GetInstance<WEMod>().Logger.Info("c.Index: " + c.Index.ToString() + " exception: " + e.ToString());}
			
			var incomingLabels = c.IncomingLabels;
			foreach (ILLabel cursorIncomingLabel in incomingLabels)
				c.MarkLabel(cursorIncomingLabel);
			
			if (debugging) { throw new Exception("Failed to find instructions"); }try { ModContent.GetInstance<WEMod>().Logger.Info("c.Index: " + c.Index.ToString() + " Instruction: " + c.Next.ToString()); } catch (Exception e) { ModContent.GetInstance<WEMod>().Logger.Info("c.Index: " + c.Index.ToString() + " exception: " + e.ToString()); }

			c.Emit(OpCodes.Ldloc, 6);
			c.EmitDelegate((Dictionary<int, int> dictionary) =>
			{
				WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
				if (wePlayer.usingEnchantingTable)
				{
                    if (debugging)
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