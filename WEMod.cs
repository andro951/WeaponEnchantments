﻿using MonoMod.RuntimeDetour.HookGen;
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
using WeaponEnchantments.Content.NPCs;
using System.Linq;
using WeaponEnchantments.ModIntegration;
using MonoMod.RuntimeDetour;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.GameInput;
using Microsoft.Xna.Framework.Input;
using androLib.UI;
using androLib;
using androLib.Localization;
using WeaponEnchantments.Localization;
using Terraria.Map;

namespace WeaponEnchantments
{
	public class WEMod : Mod {
		public static string ModName = ModContent.GetInstance<WEMod>().Name;
		public static ServerConfig serverConfig = ModContent.GetInstance<ServerConfig>();
		public static ClientConfig clientConfig = ModContent.GetInstance<ClientConfig>();
		public static bool playerSwapperModEnabled = false;
		public static bool dbtEnabled = false;
		public static bool recursiveCraftEnabled = ModLoader.TryGetMod("RecursiveCraft", out Mod _);
		public static bool imkSushisModEnabled = ModLoader.TryGetMod("imkSushisMod", out Mod _);
		public static bool avaliRaceEnabled = ModLoader.TryGetMod("AvaliRace", out Mod _);
		public static bool bossChecklistEnabled = ModLoader.TryGetMod("BossChecklist", out Mod _);
		public static bool bountifulGoodieBagsEnabled = ModLoader.TryGetMod("BountifulGoodieBags", out Mod _);
		public static bool amuletOfManyMinionsEnabled = ModLoader.TryGetMod("AmuletOfManyMinions", out Mod _);
		public static bool redCloudEnabled = ModLoader.TryGetMod("tsorcRevamp", out Mod _);
		public static bool aequusEnabled = ModLoader.TryGetMod("aequus", out Mod _);
		public static bool clickerClassEnabled = ModLoader.TryGetMod("ClickerClass", out Mod _);
		public static bool secretsOfTheShadowsEnabled = ModLoader.TryGetMod("SOTS", out Mod _);
		public static Mod wikiThis;
		public static bool wikiThisEnabled = ModLoader.TryGetMod("Wikithis", out wikiThis);

		public const string WIKI_URL = "https://weapon-enchantments-mod-tmodloader.fandom.com/wiki/";

		List<Hook> hooks = new();
		public override void Load() {
			AddAllContent(this);

			InfusionProgression.vanillaRecipeCount = Recipe.numRecipes;
			hooks.Add(new(ModLoaderIOItemIOLoadMethodInfo, ItemIOLoadDetour));
			//hooks.Add(new(ModLoaderModifyHitNPCMethodInfo, ModifyHitNPCDetour));
			//hooks.Add(new(ModLoaderModifyHitNPCWithProjMethodInfo, ModifyHitNPCWithProjDetour));
			hooks.Add(new(ModLoaderUpdateArmorSetMethodInfo, UpdateArmorSetDetour));
			//hooks.Add(new(ModLoaderToHitInfoMethodInfo, ToHitInfoDetour));
			//hooks.Add(new(ModLoaderCaughtFishStackMethodInfo, CaughtFishStackDetour));
			foreach (Hook hook in hooks) {
				hook.Apply();
			}

			On_Projectile.AI_061_FishingBobber_GiveItemToPlayer += OnProjectile_AI_061_FishingBobber_GiveItemToPlayer;
			On_Item.GetShimmered += On_Item_GetShimmered;
			On_WorldGen.KillTile_GetItemDrops += On_WorldGen_KillTile_GetItemDrops;
			On_HitTile.AddDamage += On_HitTile_AddDamage;
			On_WorldGen.KillTile_DropItems += On_WorldGen_KillTile_DropItems;
			On_Recipe.FindRecipes += EnchantmentStorage.FindRecipes;
			
			//On_Player.ItemCheck_CheckFishingBobber_PullBobber += OnPlayer_ItemCheck_CheckFishingBobber_PullBobber;
			IL_Projectile.FishingCheck += WEPlayer.HookFishingCheck;
			IL_Projectile.AI_099_1 += WEPlayer.HookAI_099_1;
			IL_Projectile.AI_099_2 += WEPlayer.HookAI_099_2;
			IL_Main.MouseText_DrawItemTooltip_GetLinesInfo += OnMouseText_DrawItemTooltip_GetLinesInfo;

			UIManager.RegisterWithMaster();

			LocalizationData.RegisterSDataPackage();
		}
		private void AddAllContent(WEMod weMod) {
			IEnumerable<Type> types = null;
			try {
				types = Assembly.GetExecutingAssembly().GetTypes();
			}
			catch (ReflectionTypeLoadException e) {
				types = e.Types.Where(t => t != null);
			}

			types = types.Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(WEModItem)));

			IEnumerable<ModItem> allItems = types.Select(t => Activator.CreateInstance(t)).Where(i => i != null).OfType<ModItem>();
			IEnumerable<ModItem> enchantingTables = allItems.OfType<EnchantingTableItem>();
			IEnumerable<ModItem> containments = allItems.OfType<ContainmentItem>();
			IEnumerable<ModItem> powerBoosters = allItems.Where(i => i is PowerBooster or UltraPowerBooster).OrderBy(i => i.Name);
			IEnumerable<ModItem> enchantmentEssences = allItems.OfType<EnchantmentEssence>().OrderBy(i => i.EssenceTier);
			IEnumerable<ModItem> enchantments =
				allItems
				.OfType<Enchantment>()
				.GroupBy(i => i.EnchantmentTier)
				.Select(g => g.ToList().OrderBy(i => i.EnchantmentTypeName))
				.SelectMany(i => i);

			foreach (ModItem modItem in enchantingTables.Concat(containments).Concat(powerBoosters).Concat(enchantmentEssences).Concat(enchantments)) {
				weMod.AddContent(modItem);
			}
		}

		#region Kill tile

		public static Item bestPickaxe = new();
		private static bool justBrokeBlock = false;
		private void On_WorldGen_KillTile_DropItems(On_WorldGen.orig_KillTile_DropItems orig, int x, int y, Tile tileCache, bool includeLargeObjectDrops, bool includeAllModdedLargeObjectDrops) {
			justBrokeBlock = true;
			orig(x, y, tileCache, includeLargeObjectDrops, includeAllModdedLargeObjectDrops);
		}
		private int On_HitTile_AddDamage(On_HitTile.orig_AddDamage orig, HitTile self, int tileId, int damageAmount, bool updateAmount) {
			bestPickaxe = Main.LocalPlayer.GetBestPickaxe();

			return orig(self, tileId, damageAmount, updateAmount);
		}
		private void On_WorldGen_KillTile_GetItemDrops(On_WorldGen.orig_KillTile_GetItemDrops orig, int x, int y, Tile tileCache, out int dropItem, out int dropItemStack, out int secondaryItem, out int secondaryItemStack, bool includeLargeObjectDrops) {
			orig(x, y, tileCache, out dropItem, out dropItemStack, out secondaryItem, out secondaryItemStack, includeLargeObjectDrops);

			if (!justBrokeBlock)
				return;

			justBrokeBlock = false;
			
			if (bestPickaxe != null)
				WEGlobalTile.KillTile(bestPickaxe, tileCache.TileType, dropItem, dropItemStack, secondaryItem, secondaryItemStack);
		}

		#endregion

		public override void Unload() {
			foreach (Hook hook in hooks) {
				hook?.Undo();
			}

			hooks.Clear();
			hooks = null;
		}
		public override void PostSetupContent() {
			if (wikiThis != null)
				wikiThis.Call("url", this, WIKI_URL + "{}");
			
			if (AndroMod.vacuumBagsEnabled)
				EnchantedWeapon.AmmoBagStorageID = StorageManager.GetStorageID(AndroMod.vacuumBagsName, "AmmoBag");
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

		/*
		private delegate NPC.HitInfo orig_ToHitInfo(float baseDamage, bool crit, float baseKnockback, bool damageVariation = false, float luck = 0f);
		private delegate NPC.HitInfo hook_ToHitInfo(orig_ToHitInfo orig, float baseDamage, bool crit, float baseKnockback, bool damageVariation = false, float luck = 0f);
		private static readonly MethodInfo ModLoaderToHitInfoMethodInfo = typeof(Terraria.NPC.HitModifiers).GetMethod("ToHitInfo");
		private void ToHitInfoDetour(NPC.HitModifiers hitModifiers, orig_ToHitInfo orig, float baseDamage, bool crit, float baseKnockback, bool damageVariation = false, float luck = 0f) {
			bool critOverride = (bool)typeof(NPC.HitModifiers).GetProperty("_critOverride").GetValue(hitModifiers);
			if (critOverride && crit) {
				hitModifiers.CritDamage *= 1.5f;
			}
		}
		*/
		/*
		private delegate void orig_ModifyHitNPC(Item item, Player player, NPC target, ref NPC.HitModifiers modifiers);
		private delegate void hook_ModifyHitNPC(orig_ModifyHitNPC orig, Item item, Player player, NPC target, ref NPC.HitModifiers modifiers);
		private static readonly MethodInfo ModLoaderModifyHitNPCMethodInfo = typeof(Terraria.ModLoader.ItemLoader).GetMethod("ModifyHitNPC");
		private void ModifyHitNPCDetour(orig_ModifyHitNPC orig, Item item, Player player, NPC target, ref NPC.HitModifiers modifiers) {
			player.GetWEPlayer().ModifyHitNPCWithAny(item, target, ref modifiers);
			orig(item, player, target, ref modifiers);
		}
		private delegate void orig_ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers);
		private delegate void hook_ModifyHitNPCWithProj(orig_ModifyHitNPCWithProj orig, Projectile proj, NPC target, ref NPC.HitModifiers modifiers);
		private static readonly MethodInfo ModLoaderModifyHitNPCWithProjMethodInfo = typeof(Terraria.ModLoader.ProjectileLoader).GetMethod("ModifyHitNPC");
		private void ModifyHitNPCWithProjDetour(orig_ModifyHitNPCWithProj orig, Projectile proj, NPC target, ref NPC.HitModifiers modifiers) {
			Player player = Main.player[proj.owner];
			Item item = null;
			if (proj.TryGetGlobalProjectile(out WEProjectile weProj)) // Try not using a global for this maybe
				item = weProj.sourceItem;

			if (item != null)
				player.GetWEPlayer().ModifyHitNPCWithAny(item, target, ref modifiers, proj);

			orig(proj, target, ref modifiers);
		}
		*/

		//private delegate void orig_CaughtFishStack(Item item);
		//private delegate void hook_CaughtFishStack(orig_CaughtFishStack orig, Item item);
		//private static readonly MethodInfo ModLoaderCaughtFishStackMethodInfo = typeof(ItemLoader).GetMethod("CaughtFishStack");
		//private void CaughtFishStackDetour(orig_CaughtFishStack orig, Item item) { orig(item); }
		private void OnProjectile_AI_061_FishingBobber_GiveItemToPlayer(On_Projectile.orig_AI_061_FishingBobber_GiveItemToPlayer orig, Projectile self, Player thePlayer, int itemType) {
			if (thePlayer.HeldItem.TryGetEnchantedItem(out EnchantedFishingPole enchantedFishingPole)) {
				int value = (int)((float)ContentSamples.ItemsByType[itemType].value / 10f * ConfigValues.GatheringExperienceMultiplier);
				enchantedFishingPole.GainXP(thePlayer.HeldItem, value);
			}

			orig(self, thePlayer, itemType);
		}
		private void On_Item_GetShimmered(On_Item.orig_GetShimmered orig, Item self) {
			EnchantingTableUI.ReturnAllModifications(ref self);

			orig(self);
		}

		private static bool shouldShowCritChance = false;
		private void OnMouseText_DrawItemTooltip_GetLinesInfo(ILContext il) {

			var c = new ILCursor(il);
			//IL_01e4: ldarg.0
			//IL_01e5: ldc.i4.1
			//IL_01e6: callvirt instance int32 Terraria.Player::GetWeaponDamage(class Terraria.Item, bool)
			if (!c.TryGotoNext(MoveType.After,
				i => i.MatchLdarg(0),
				i => i.MatchLdcI4(1),
				i => i.MatchCallvirt<Player>("GetWeaponDamage")
			)) { throw new Exception("Failed to find instructions OnMouseText_DrawItemTooltip_GetLinesInfo 1/3"); }

			c.Emit(OpCodes.Ldarg_0);
			c.EmitDelegate((int damage, Item item) => {
				if (!WEMod.clientConfig.DisplayDamageTooltipSeperatly && item.TryGetEnchantedWeapon(out EnchantedWeapon enchantedWeapon)) {
					if (enchantedWeapon.GetPlayerModifierStrengthForTooltip(Main.LocalPlayer, EnchantmentStat.DamageAfterDefenses, out float damageMultiplier))
						damage = (int)Math.Round((float)damage * damageMultiplier);
				}

				return damage;
			});

			//IL_0219: ldarg.0
			//IL_021a: callvirt instance class Terraria.ModLoader.DamageClass Terraria.Item::get_DamageType()
			//IL_021f: callvirt instance bool Terraria.ModLoader.DamageClass::get_UseStandardCritCalcs()
			if (!c.TryGotoNext(MoveType.After,
				i => i.MatchLdarg(0),
				i => i.MatchCallvirt<Item>("get_DamageType"),
				i => i.MatchCallvirt<DamageClass>("get_UseStandardCritCalcs")
			)) { throw new Exception("Failed to find instructions OnMouseText_DrawItemTooltip_GetLinesInfo 2/3"); }

			c.Emit(OpCodes.Ldarg_0);
			c.EmitDelegate((bool useStandardCritCalcs, Item item) => {
				shouldShowCritChance = false;
				if (useStandardCritCalcs)
					return useStandardCritCalcs;

				if (!WEMod.serverConfig.DisableMinionCrits && (item.DamageType == DamageClass.MagicSummonHybrid || item.DamageType == DamageClass.MagicSummonHybrid || item.DamageType == DamageClass.SummonMeleeSpeed)) {
					shouldShowCritChance = true;
				}

				return shouldShowCritChance;
			});

			//IL_0226: ldarg.0
			//IL_0227: callvirt instance class Terraria.ModLoader.DamageClass Terraria.Item::get_DamageType()
			//IL_022c: ldsfld class Terraria.Player[] Terraria.Main::player
			//IL_0231: ldsfld int32 Terraria.Main::myPlayer
			//IL_0236: ldelem.ref
			//IL_0237: ldstr "CritChance"
			//IL_023c: callvirt instance bool Terraria.ModLoader.DamageClass::ShowStatTooltipLine(class Terraria.Player, string)

			if (!c.TryGotoNext(MoveType.After,
				//i => i.MatchLdarg(0),
				//i => i.MatchCallvirt<Item>("get_DamageType"),
				//i => i.MatchLdsfld<Main>("player"),
				//i => i.MatchLdsfld<int>("myPlayer"),
				//i => i.MatchLdelemRef(),
				i => i.MatchLdstr("CritChance"),
				i => i.MatchCallvirt<DamageClass>("ShowStatTooltipLine")
			)) { throw new Exception("Failed to find instructions OnMouseText_DrawItemTooltip_GetLinesInfo 3/3"); }

			c.EmitDelegate((bool useStandardCritCalcs) => {
				if (useStandardCritCalcs)
					return useStandardCritCalcs;

				return shouldShowCritChance;
			});
		}
		/*
		private void OnPlayer_ItemCheck_CheckFishingBobber_PullBobber(OnPlayer.orig_ItemCheck_CheckFishingBobber_PullBobber orig, Player self, Projectile bobber, int baitTypeUsed) {
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
		/*
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
			c.Emit(OpCodes.Ldarg_0);

			//Consume essence from enchanting table when crafting.
			c.EmitDelegate((Item item, int num, Recipe recipe) => {
				if (num < 1)
					return;

				WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
				if (item.ModItem is EnchantmentEssence) {
					ConsumeFromCraft(item, num, wePlayer.enchantingTableEssence, recipe);
				}
				else if (EnchantmentStorage.CanBeStored(item)) {
					ConsumeFromCraft(item, num, wePlayer.enchantmentStorageItems, recipe, true);
				}
				else if (OreBagUI.CanBeStored(item)) {
					ConsumeFromCraft(item, num, wePlayer.oreBagItems, recipe);
				}
			});
		}
		public static void ConsumeFromCraft(Item item, int stack, Item[] inventory, Recipe recipe, bool ignoreFavorited = false) {
			if (stack < 1)
				return;

			for (int i = 0; i < inventory.Length; i++) {
				ref Item storageItem = ref inventory[i];
				if ((!storageItem.favorited || !ignoreFavorited) && item.type == storageItem.type || recipe.AcceptedByItemGroups(item.type, storageItem.type)) {
					int ammountToTransfer = Math.Min(stack, storageItem.stack);
					Item consumedItem = storageItem.Clone();
					consumedItem.stack = ammountToTransfer;
					List<Item> ConsumedItems = (List<Item>)typeof(RecipeLoader).GetField("ConsumedItems", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
					ConsumedItems.Add(consumedItem);
					storageItem.stack -= ammountToTransfer;
					if (storageItem.stack < 1) {
						storageItem.TurnToAir();
					}

					stack -= ammountToTransfer;
					if (stack < 1)
						break;
				}
			}
		}
		private static bool IsSameCraftingItem() {
			return false;
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
				WEPlayer wePlayer = WEPlayer.LocalWEPlayer;
				if (debuggingHookFindRecipes) {
					counter++;
					ModContent.GetInstance<WEMod>().Logger.Info("counter: " + counter.ToString());
				}

				List<Item> items = new();
				if (wePlayer.usingEnchantingTable) {
					for (int i = 0; i < EnchantingTableUI.MaxEssenceSlots; i++) {
						ref Item item = ref wePlayer.enchantingTableEssence[i];
						if (!item.NullOrAir() && item.stack > 0)
							items.Add(item);
					}
				}

				if (wePlayer.displayEnchantmentStorage || EnchantmentStorage.crafting) {
					for (int i = 0; i < wePlayer.enchantmentStorageItems.Length; i++) {
						ref Item item = ref wePlayer.enchantmentStorageItems[i];
						if (!item.NullOrAir() && item.stack > 0 && !item.favorited)
							items.Add(item);
					}
				}

				if (wePlayer.Player.HasItem(ModContent.ItemType<OreBag>())) {
					for (int i = 0; i < wePlayer.oreBagItems.Length; i++) {
						ref Item item = ref wePlayer.oreBagItems[i];
						if (!item.NullOrAir() && item.stack > 0)
							items.Add(item);
					}
				}

				for (int i = 0; i < items.Count; i++) {
					Item item = items[i];
					if (item != null && item.stack > 0) {
						if (dictionary.ContainsKey(item.netID)) {
							dictionary[item.netID] += item.stack;
						}
						else {
							dictionary[item.netID] = item.stack;
						}
					}
				}
			});
		}
		*/
	}
}
