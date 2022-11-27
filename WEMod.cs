﻿using IL.Terraria.Localization;
using MonoMod.RuntimeDetour.HookGen;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;
using Terraria.ModLoader.IO;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Configs;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Debuffs;
using WeaponEnchantments.Items;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using WeaponEnchantments.UI;
using System.Runtime.CompilerServices;
using WeaponEnchantments.Common.Utility;
using KokoLib;
using OnProjectile = On.Terraria.Projectile;
using OnPlayer = On.Terraria.Player;
using WeaponEnchantments.Content.NPCs;
using System.Linq;

namespace WeaponEnchantments
{
	public class WEMod : Mod {
		internal static ServerConfig serverConfig = ModContent.GetInstance<ServerConfig>();
		internal static ClientConfig clientConfig = ModContent.GetInstance<ClientConfig>();
		public static bool calamityEnabled = false;
		public static bool magicStorageEnabled = false;
		public static bool playerSwapperModEnabled = false;
		public static bool dbtEnabled = false;
		public static List<Item> consumedItems = new List<Item>();

		public override void Load() {
			HookEndpointManager.Add<hook_ItemIOLoad>(ModLoaderIOItemIOLoadMethodInfo, ItemIOLoadDetour);
			HookEndpointManager.Add<hook_CanStack>(ModLoaderCanStackMethodInfo, CanStackDetour);
			HookEndpointManager.Add<hook_ModifyHitNPC>(ModLoaderModifyHitNPCMethodInfo, ModifyHitNPCDetour);
			HookEndpointManager.Add<hook_ModifyHitNPCWithProj>(ModLoaderModifyHitNPCWithProjMethodInfo, ModifyHitNPCWithProjDetour);
			HookEndpointManager.Add<hook_UpdateArmorSet>(ModLoaderUpdateArmorSetMethodInfo, UpdateArmorSetDetour);
			//HookEndpointManager.Add<hook_CaughtFishStack>(ModLoaderCaughtFishStackMethodInfo, CaughtFishStackDetour);
			OnProjectile.AI_061_FishingBobber_GiveItemToPlayer += OnProjectile_AI_061_FishingBobber_GiveItemToPlayer;
			//OnPlayer.ItemCheck_CheckFishingBobber_PullBobber += OnPlayer_ItemCheck_CheckFishingBobber_PullBobber;
			IL.Terraria.Recipe.FindRecipes += HookFindRecipes;
			IL.Terraria.Recipe.Create += HookCreate;
			IL.Terraria.Projectile.FishingCheck += WEPlayer.HookFishingCheck;
			IL.Terraria.Projectile.AI_099_1 += WEPlayer.HookAI_099_1;
			IL.Terraria.Projectile.AI_099_2 += WEPlayer.HookAI_099_2;
		}
		public override void PostSetupContent() {
			if (ModLoader.TryGetMod("Census", out Mod Census)) {
				foreach(ModNPC modNPC in ModContent.GetContent<ModNPC>().Where(m => m is INPCWikiInfo wikiInfo && wikiInfo.TownNPC)) {
					Census.Call("TownNPCCondition", modNPC.NPC.netID, ((INPCWikiInfo)modNPC).SpawnCondition);
				}
				//Census.Call("TownNPCCondition", ModContent.NPCType<Witch>(), "Have an enchantment in your inventory or on your equipment.");
			}
			
			if (ModLoader.TryGetMod("Wikithis", out Mod wikiThis))
				wikiThis.Call("AddModURL", this, "weapon-enchantments-mod-tmodloader.fandom.com");
		}

		private delegate Item orig_ItemIOLoad(TagCompound tag);
		private delegate Item hook_ItemIOLoad(orig_ItemIOLoad orig, TagCompound tag);
		private static readonly MethodInfo ModLoaderIOItemIOLoadMethodInfo = typeof(Main).Assembly.GetType("Terraria.ModLoader.IO.ItemIO")!.GetMethod("Load", BindingFlags.Public | BindingFlags.Static, new System.Type[] { typeof(TagCompound) })!;
		private Item ItemIOLoadDetour(orig_ItemIOLoad orig, TagCompound tag) {
			Item item = orig(tag);
			if (item.ModItem is UnloadedItem)
				OldItemManager.ReplaceOldItem(ref item);

			return item;
		}

		private delegate bool orig_CanStack(Item item1, Item item2);
		private delegate bool hook_CanStack(orig_CanStack orig, Item item1, Item item2);
		private static readonly MethodInfo ModLoaderCanStackMethodInfo = typeof(ItemLoader).GetMethod("CanStack");
		private bool CanStackDetour(orig_CanStack orig, Item item1, Item item2) {
			if (!orig(item1, item2))
				return false;

			if (!item1.TryGetEnchantedItem(out EnchantedItem enchantedItem))
				return true;

			if (magicStorageEnabled) {
				string name = (new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().Name;
				if ("CanCombineItems" == name)
					return true;
			}

			return enchantedItem.OnStack(item1, item2);
		}

		private delegate void orig_UpdateArmorSet(Player player, Item head, Item body, Item legs);
		private delegate void hook_UpdateArmorSet(orig_UpdateArmorSet orig, Player player, Item head, Item body, Item legs);
		private static readonly MethodInfo ModLoaderUpdateArmorSetMethodInfo = typeof(ItemLoader).GetMethod("UpdateArmorSet");
		private void UpdateArmorSetDetour(orig_UpdateArmorSet orig, Player player, Item head, Item body, Item legs) {
			WEPlayer wePlyaer = player.GetWEPlayer(); 
			if (wePlyaer.Equipment.InfusedHead != null)
				head = wePlyaer.Equipment.InfusedHead;

			if (wePlyaer.Equipment.InfusedBody != null)
				body = wePlyaer.Equipment.InfusedBody;

			if (wePlyaer.Equipment.InfusedLegs != null)
				legs = wePlyaer.Equipment.InfusedLegs;

			orig(player, head, body, legs);
		}

		private delegate void orig_ModifyHitNPC(Item item, Player player, NPC target, ref int damage, ref float knockback, ref bool crit);
		private delegate void hook_ModifyHitNPC(orig_ModifyHitNPC orig, Item item, Player player, NPC target, ref int damage, ref float knockback, ref bool crit);
		private static readonly MethodInfo ModLoaderModifyHitNPCMethodInfo = typeof(Terraria.ModLoader.ItemLoader).GetMethod("ModifyHitNPC");
		private void ModifyHitNPCDetour(orig_ModifyHitNPC orig, Item item, Player player, NPC target, ref int damage, ref float knockback, ref bool crit) {
			player.GetWEPlayer().ModifyHitNPCWithAny(item, target, ref damage, ref knockback, ref crit, ref player.direction);
			orig(item, player, target, ref damage, ref knockback, ref crit);
		}
		private delegate void orig_ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection);
		private delegate void hook_ModifyHitNPCWithProj(orig_ModifyHitNPCWithProj orig, Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection);
		private static readonly MethodInfo ModLoaderModifyHitNPCWithProjMethodInfo = typeof(Terraria.ModLoader.ProjectileLoader).GetMethod("ModifyHitNPC");
		private void ModifyHitNPCWithProjDetour(orig_ModifyHitNPCWithProj orig, Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection) {
			Player player = Main.player[proj.owner];
			Item item = null;
			if (proj.TryGetGlobalProjectile(out WEProjectile weProj)) // Try not using a global for this maybe
				item = weProj.sourceItem;

			if (item != null)
				player.GetWEPlayer().ModifyHitNPCWithAny(item, target, ref damage, ref knockback, ref crit, ref hitDirection, proj);

			orig(proj, target, ref damage, ref knockback, ref crit, ref hitDirection);
		}

		//private delegate void orig_CaughtFishStack(Item item);
		//private delegate void hook_CaughtFishStack(orig_CaughtFishStack orig, Item item);
		//private static readonly MethodInfo ModLoaderCaughtFishStackMethodInfo = typeof(ItemLoader).GetMethod("CaughtFishStack");
		//private void CaughtFishStackDetour(orig_CaughtFishStack orig, Item item) { orig(item); }
		private void OnProjectile_AI_061_FishingBobber_GiveItemToPlayer(OnProjectile.orig_AI_061_FishingBobber_GiveItemToPlayer orig, Projectile self, Player thePlayer, int itemType) {
			if (thePlayer.HeldItem.TryGetEnchantedItem(out EnchantedFishingPole enchantedFishingPole)) {
				int value = (int)((float)ContentSamples.ItemsByType[itemType].value / 10f * ConfigValues.GatheringExperienceMultiplier);
				enchantedFishingPole.GainXP(thePlayer.HeldItem, value);
			}

			orig(self, thePlayer, itemType);
		}
		/*private void OnPlayer_ItemCheck_CheckFishingBobber_PullBobber(OnPlayer.orig_ItemCheck_CheckFishingBobber_PullBobber orig, Player self, Projectile bobber, int baitTypeUsed) {
			if (Main.hardMode && self.GetWEPlayer().CheckEnchantmentStats(EnchantmentStat.FishingEnemySpawnChance, out float spawnChance)) {
				spawnChance /= 10f;
				float rand = Main.rand.NextFloat();
				rand = 0f;
				if (rand <= spawnChance) {
					baitTypeUsed = ItemID.TruffleWorm;
				}
			}

			orig(self, bobber, baitTypeUsed);
		}*/

		public static int counter = 0;
		private const bool debuggingHookFindRecipes = false;
		private const bool debuggingHookCreate = false;
		private static void HookCreate(ILContext il) {
			counter = 0;
			var c = new ILCursor(il);

			if (debuggingHookCreate) {
				while (c.Next != null) {
					bool catchingExceptions = true;
					ModContent.GetInstance<WEMod>().Logger.Info("c.Index: " + c.Index.ToString() + " Instruction: " + c.Next.ToString());
					while (catchingExceptions) {
						c.Index++;
						try {
							if (c.Next != null) {
								string tempString = c.Next.ToString();
							}
							catchingExceptions = false;
						}
						catch (Exception e) {
							ModContent.GetInstance<WEMod>().Logger.Info("c.Index: " + c.Index.ToString() + " exception: " + e.ToString().Substring(0, 20));
						}
					}
				}
				c.Index = 0;



				bool searching = true;
				int line = 0;
				int j = 0;
				int jNext = c.Context.ToString().Substring(0).IndexOf("IL_");
				while (searching) {
					j = jNext;
					//Debug.WriteLine("length: " + c.Context.ToString().Length.ToString() + " jNext + 1: " + (jNext + 1).ToString());
					jNext = c.Context.ToString().Substring(jNext + 1).IndexOf("IL_") + j + 1;
					//Debug.WriteLine("substring: " + c.Context.ToString().Substring(j, jNext - j - 2) + ", length: " + c.Context.ToString().Substring(j, jNext - j - 2).Length.ToString());
					if (jNext == j) {
						ModContent.GetInstance<WEMod>().Logger.Info(line + " " + c.Context.ToString().Substring(j));
						//Debug.WriteLine(line + " " + c.Context.ToString().Substring(j));
						searching = false;
					}
					else {
						ModContent.GetInstance<WEMod>().Logger.Info(line + " " + c.Context.ToString().Substring(j, jNext - j - 2));
						//Debug.WriteLine(line + " " + c.Context.ToString().Substring(j, jNext - j - 2));
					}
					line++;
				}
			}

			if (!c.TryGotoNext(MoveType.Before,
				i => i.MatchLdsfld(out _),
				i => i.MatchLdsfld(out _),
				i => i.MatchLdelemRef(),
				i => i.MatchLdfld(out _),
				i => i.MatchLdcI4(-1),
				i => i.MatchBeq(out _)

			)) { 
				throw new Exception("Failed to find instructions HookCreate 3"); 
			}

			var incomingLabels = c.IncomingLabels;
			foreach (ILLabel cursorIncomingLabel in incomingLabels)
				c.MarkLabel(cursorIncomingLabel);

			c.Emit(OpCodes.Ldloc, 3);
			c.Emit(OpCodes.Ldloc, 4);

			//Consume essence from enchanting table when crafting.
			c.EmitDelegate((Item item, int num) => {
				WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
				if (wePlayer.usingEnchantingTable) {
					for (int i = 0; i < EnchantingTable.maxEssenceItems; i++) {
						Item slotItem = wePlayer.enchantingTableUI.essenceSlotUI[i].Item;
						if (item.type == slotItem.type) {
							slotItem.stack -= num;
						}
					}
				}
			});
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
			)) { 
				throw new Exception("Failed to find instructions HookFindRecipes"); 
			}

			if (debuggingHookFindRecipes) try { ModContent.GetInstance<WEMod>().Logger.Info("c.Index: " + c.Index.ToString() + " Instruction: " + c.Next.ToString()); } catch (Exception e) { ModContent.GetInstance<WEMod>().Logger.Info("c.Index: " + c.Index.ToString() + " exception: " + e.ToString()); }

			var incomingLabels = c.IncomingLabels;
			foreach (ILLabel cursorIncomingLabel in incomingLabels) {
				c.MarkLabel(cursorIncomingLabel);
			}

			if (debuggingHookFindRecipes) try { ModContent.GetInstance<WEMod>().Logger.Info("c.Index: " + c.Index.ToString() + " Instruction: " + c.Next.ToString()); } catch (Exception e) { ModContent.GetInstance<WEMod>().Logger.Info("c.Index: " + c.Index.ToString() + " exception: " + e.ToString()); }

			c.Emit(OpCodes.Ldloc, 6);
			c.EmitDelegate((Dictionary<int, int> dictionary) => {
				//Add essence in enchanting table to recipe ingredients dictionary
				WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
				if (wePlayer.usingEnchantingTable) {
					if (debuggingHookFindRecipes) {
						counter++;
						ModContent.GetInstance<WEMod>().Logger.Info("counter: " + counter.ToString());
					}
					for (int i = 0; i < EnchantingTable.maxEssenceItems; i++) {
						Item item = wePlayer.enchantingTableUI.essenceSlotUI[i].Item;
						if (item != null && item.stack > 0) {
							if (dictionary.ContainsKey(item.netID)) {
								dictionary[item.netID] += item.stack;
							}
							else {
								dictionary[item.netID] = item.stack;
							}
						}
					}
				}
			});
		}
	}
}
