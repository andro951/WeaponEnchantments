using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;
using Terraria.ModLoader.IO;
using Terraria.UI;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Configs;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using WeaponEnchantments.Items;
using WeaponEnchantments.UI;
using static WeaponEnchantments.Common.Globals.EnchantedItemStaticMethods;
using static WeaponEnchantments.Common.Configs.ConfigValues;
using static WeaponEnchantments.Common.Globals.WEGlobalNPC;
using static WeaponEnchantments.Common.Globals.NPCStaticMethods;
using WeaponEnchantments.Debuffs;
using KokoLib;
using WeaponEnchantments.ModLib.KokoLib;
using Terraria.WorldBuilding;
using rail;
using WeaponEnchantments.Effects.CustomEffects;
using androLib.Common.Utility;
using androLib.Common.Globals;
using androLib.UI;
using androLib;
using WeaponEnchantments.Items.Enchantments;
using VacuumOreBag;
using VacuumOreBag.Items;
using WeaponEnchantments.Content.NPCs;
using androLib.Items;

namespace WeaponEnchantments
{
	public class WEPlayer : ModPlayer, ISortedEnchantmentEffects, ISortedOnHitEffects {

		#region fields/properties

		public static bool WorldOldItemsReplaced = false;
        public static bool WorldEnchantedItemConverted = false;
        public static bool PlayerEnchantedItemConverted = false;
        public static WEPlayer LocalWEPlayer => Main.LocalPlayer.GetWEPlayer();
		public int levelsPerLevelUp;
		internal byte versionUpdate;
        public bool usingEnchantingTable;
        public int enchantingTableTier;
        public int highestTableTierUsed = -1;
        public bool itemInEnchantingTable;
        public Item itemBeingEnchanted = new();
        static float baseOneForAllRange = 240f;
        public float lifeStealRollover = 0f;
        public int allForOneTimer = 0;
        public Item infusionConsumeItem = new();
        public string previousInfusedItemName = "";
        public Item trackedTrashItem = new();
        public bool disableLeftShiftTrashCan = ItemSlot.Options.DisableLeftShiftTrashCan;
        public Point enchantingTableLocation = new Point(-1, -1);
		public int cursedEssenceCount;
        public DuplicateDictionary<int, Item> CalamityRespawnMinionSourceItems = new();
        public Item[] enchantmentStorageItems = Enumerable.Repeat(new Item(), EnchantmentStorageSize).ToArray();
		public const int EnchantmentStorageSize = 300;
        public ref int EnchantmentStorageUILeft {
            get {
                return ref (Witch.DisplayingUI ? ref enchantmentStorageUILeftWitchUI : ref enchantmentStorageUILeft);
            }
		}
        public ref int EnchantmentStorageUITop {
            get {
				return ref (Witch.DisplayingUI ? ref enchantmentStorageUITopWitchUI : ref enchantmentStorageUITop);
			}
		}
        private int enchantmentStorageUILeft;
		private int enchantmentStorageUITop;
        private int enchantmentStorageUILeftWitchUI;
		private int enchantmentStorageUITopWitchUI;
		public bool displayEnchantmentStorage;
        public int enchantingTableUILeft;
		public int enchantingTableUITop;
        public bool vacuumItemsIntoEnchantmentStorage = true;
        public SortedSet<string> trashEnchantmentsFullNames = new();
        public Item enchantingTableItem = new();
        public EnchantmentsArray emptyEnchantments = new EnchantmentsArray(null);
		public EnchantmentsArray enchantingTableEnchantments;
        public Item[] enchantingTableEssence = Enumerable.Repeat(new Item(), EnchantingTableUI.MaxEssenceSlots).ToArray();
		public bool openStorageWhenOpeningTable = true;
        public SortedSet<string> allOfferedItems = new();
        public bool transferedToAndroLib = false;
        public Item[] oreBagItems = new Item[] { new() };
        public int oreBagUILeft;
		public int oreBagUITop;
		public const int OreBagSize = 100;
		public bool vacuumItemsIntoOreBag = true;
		public bool displayOreBagUI = false;
		public Dictionary<string, List<Item[]>> enchantmentLoadouts = new();
        public bool displayEnchantmentLoadoutUI = false;
        public int EnchantmentLoadoutUILeft;
        public int EnchantmentLoadoutUITop;
        public bool openLoadoutsWhenOpeningTable;
		public string displayedLoadout = null;
        public bool autoTrashOfferedItems = true;

		#endregion

		#region Enchantment Effects

		public PlayerEquipment Equipment {
            get {
                if (equipment is null)
                    equipment = new(Player);

                return equipment;
            }
            set => equipment = value;
        }
        private PlayerEquipment equipment = null;
        public SortedDictionary<uint, IUseTimer> EffectTimers = new SortedDictionary<uint, IUseTimer>();
        public SortedDictionary<short, uint> OnTickBuffTimers = new();
	
        public SortedDictionary<EnchantmentStat, EStatModifier> EnchantmentStats { set; get; } = new SortedDictionary<EnchantmentStat, EStatModifier>();
        public SortedDictionary<EnchantmentStat, EStatModifier> VanillaStats { set; get; } = new SortedDictionary<EnchantmentStat, EStatModifier>();
        public SortedList<EnchantmentStat, PlayerSetEffect> PlayerSetEffects { set; get; } = new SortedList<EnchantmentStat, PlayerSetEffect>();
        public SortedDictionary<EnchantmentStat, bool> BoolEffects { set; get; } = new SortedDictionary<EnchantmentStat, bool>();
	    public SortedDictionary<short, BuffStats> OnHitDebuffs { set; get; } = new SortedDictionary<short, BuffStats>();
        public SortedDictionary<short, BuffStats> OnHitBuffs { set; get; } = new SortedDictionary<short, BuffStats>();
        public SortedDictionary<short, BuffStats> OnTickBuffs { set; get; } = new SortedDictionary<short, BuffStats>();
        public List<EnchantmentEffect> EnchantmentEffects { set; get; } = new List<EnchantmentEffect>();
        public List<IPassiveEffect> PassiveEffects { set; get; } = new List<IPassiveEffect>();
        public List<IOnHitEffect> OnHitEffects { set; get; } = new List<IOnHitEffect>();
        public List<IModifyShootStats> ModifyShootStatEffects { set; get; } = new List<IModifyShootStats>();
        public List<StatEffect> StatEffects { set; get; } = new List<StatEffect>();

		#endregion

		#region Combined Enchanmtent Effects

	    public SortedDictionary<EnchantmentStat, EStatModifier> CombinedEnchantmentStats { set; get; } = new SortedDictionary<EnchantmentStat, EStatModifier>();
        public SortedDictionary<EnchantmentStat, EStatModifier> CombinedVanillaStats { set; get; } = new SortedDictionary<EnchantmentStat, EStatModifier>();
        public SortedList<EnchantmentStat, PlayerSetEffect> CombinedPlayerSetEffects { set; get; } = new SortedList<EnchantmentStat, PlayerSetEffect>();
        public SortedDictionary<EnchantmentStat, bool> CombinedBoolEffects { set; get; } = new SortedDictionary<EnchantmentStat, bool>();
        public SortedDictionary<short, BuffStats> CombinedOnHitDebuffs { set; get; } = new SortedDictionary<short, BuffStats>();
        public SortedDictionary<short, BuffStats> CombinedOnHitBuffs { set; get; } = new SortedDictionary<short, BuffStats>();
        public SortedDictionary<short, BuffStats> CombinedOnTickBuffs { set; get; } = new SortedDictionary<short, BuffStats>();
        public List<IOnHitEffect> CombinedOnHitEffects { set; get; } = new List<IOnHitEffect>();
        public List<IModifyShootStats> CombinedModifyShootStatEffects { set; get; } = new List<IModifyShootStats>();
		public List<IPassiveEffect> CombinedPassiveEffects { set; get; } = new List<IPassiveEffect>();

		#endregion

		#region IL

        public static void HookProcessHitAgainstNPC(ILContext il) {
			//Make vanilla crit roll 0
			var c = new ILCursor(il);

			if (!c.TryGotoNext(MoveType.After,
				i => i.MatchLdcI4(101),
				i => i.MatchCallvirt(out _),
				i => i.MatchLdloc(8),
				i => i.MatchBgt(out _),
				i => i.MatchLdcI4(1)
			)) { throw new Exception("Failed to find instructions HookProcessHitAgainstNPC 1/2"); }
			c.Emit(OpCodes.Pop);
			c.Emit(OpCodes.Ldc_I4_0);

            if (!c.TryGotoNext(MoveType.Before,
                i => i.MatchLdarg(3)
			)) { throw new Exception("Failed to find instructions HookProcessHitAgainstNPC 2/2"); }
            c.Emit(OpCodes.Ldloc, 7);
            c.Emit(OpCodes.Ldarga, 0);
            c.Emit(OpCodes.Ldarga, 1);

            c.EmitDelegate((ref NPC.HitModifiers hitModifiers, bool crit, ref Player player, ref Item item) => {
				if (player.TryGetWEPlayer(out WEPlayer wePlayer)) {
					FieldInfo info = typeof(NPC.HitModifiers).GetField("_critOverride", BindingFlags.NonPublic | BindingFlags.Instance);
					bool? critOverride = (bool?)info.GetValue(hitModifiers);
					wePlayer.CalculateCriticalChance(item, ref hitModifiers, crit, critOverride);
				}

				return ref hitModifiers;
            });
		}
        public static void HookFishingCheck_RollDropLevels(ILContext il) {
            var c = new ILCursor(il);

            if (!c.TryGotoNext(MoveType.After,
                i => i.MatchCall(out _),
                i => i.MatchLdcI4(100),
                i => i.MatchCallvirt(out _),
                i => i.MatchLdloc(5)
            )) { throw new Exception("Failed to find instuctions HookFishingCheck_RollDropLevels"); }

            c.EmitDelegate((int crateChance) => {
                if (Main.LocalPlayer?.TryGetModPlayer(out WEPlayer wePlayer) == true) {
                    if (wePlayer.CheckEnchantmentStats(EnchantmentStat.CrateChance, out float mult, 1f))
                        crateChance = (int)Math.Round((float)crateChance * mult);
                }

                return crateChance;
            });
        }
        public static void HookFishingCheck(ILContext il) {
            //var c = new ILCursor(il);

            //if (!c.TryGotoNext(MoveType.Before,
            //    i => i.MatchLdloc(9),
            //    i => i.MatchLdcI4(2)
            //)) { throw new Exception("Failed to find instuctions HookFishingCheck"); }

            //c.Index++;

            //c.Emit(OpCodes.Ldarg_0);
            //c.EmitDelegate((int lavaFishingNum, Projectile projectile) => {
            //    if (projectile.ValidOwner(out Player player)) {
            //        if (player.GetWEPlayer().CheckEnchantmentStats(EnchantmentStat.LavaFishing, out float mult)) {
            //            lavaFishingNum += (int)mult;
            //            mult %= 1f;
            //            if (Main.rand.NextFloat() <= mult)
            //                lavaFishingNum++;
            //        }
            //    }

            //    return lavaFishingNum;
            //});
        }
        private static float ModifyYoyoStringLength(float stringLength, Projectile projectile) {
            if (projectile.ValidOwner(out Player player)) {
                if (player.GetWEPlayer().CheckEnchantmentStats(EnchantmentStat.YoyoStringLength, out float mult, 1f))
                    stringLength *= mult;
            }

            return stringLength;
        }
        public static void HookAI_099_1(ILContext il) {
            var c = new ILCursor(il);
            //ldarg.0 is this projectile

            if (!c.TryGotoNext(MoveType.Before,
                i => i.MatchLdarg(0),
                i => i.MatchLdarg(0),
                i => i.MatchLdfld(out _),
                i => i.MatchLdcR4(0.5f),
                i => i.MatchAdd(),
                i => i.MatchStfld(out _)
                )) { throw new Exception("Failed to find instructions HookAI_099_1"); }

            c.Emit(OpCodes.Ldloc_1);
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate((float stringLength, Projectile projectile) => ModifyYoyoStringLength(stringLength, projectile));
            c.Emit(OpCodes.Stloc_1);
        }
        public static void HookAI_099_2(ILContext il) {
            var c = new ILCursor(il);
            //ldarg.0 is this projectile

            if (!c.TryGotoNext(MoveType.After,
                i => i.MatchLdcR4(200f),
                i => i.MatchStloc(5),
                i => i.MatchLdsfld(out _),
                i => i.MatchLdarg(0),
                i => i.MatchLdfld(out _),
                i => i.MatchLdelemR4()
                )) { throw new Exception("Failed to find instructions HookAI_099_2"); }

            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate((float stringLength, Projectile projectile) => ModifyYoyoStringLength(stringLength, projectile));
        }
        public static void HookTrySwitchingLoadout(ILContext il) {
			var c = new ILCursor(il);

			if (!c.TryGotoNext(MoveType.After,
				i => i.MatchStloc(0),
				i => i.MatchLdarg(0)
			)) { throw new Exception("Failed to find instructions HookTrySwitchingLoadout"); }
			c.Emit(OpCodes.Ldarga, 0);
			c.Emit(OpCodes.Ldarg, 1);

			c.EmitDelegate((ref Player player, int loadoutIndex) => {
				if (player.TryGetWEPlayer(out WEPlayer wePlayer)) {
                    wePlayer.OnSwapEquipmentLoadout(loadoutIndex);
				}
			});
		}

		#endregion

		#region General Override Hooks

		public override void Load() {
            IL_Player.ProcessHitAgainstNPC += HookProcessHitAgainstNPC;
            //Terraria.IL_Player.ItemCheck_MeleeHitNPCs += HookItemCheck_MeleeHitNPCs;
            IL_Projectile.FishingCheck_RollDropLevels += HookFishingCheck_RollDropLevels;
            IL_Player.TrySwitchingLoadout += HookTrySwitchingLoadout;
            AndroMod.OnResetGameCounter += () => { CurseHardCrowdControlImmunityReset = 0; EffectTimers = new(); OnTickBuffTimers = new(); };
		}
        public override void OnEnterWorld() {

            #region Debug

            if (LogMethods.debugging) ($"\\/OnEnterWorld({Player.S()})").Log();

			#endregion

			if (!WorldOldItemsReplaced) {
                OldItemManager.ReplaceAllOldItems();
                if (WEModSystem.versionUpdate < 1)
                    WEModSystem.versionUpdate = 1;

                WorldOldItemsReplaced = true;
            }

            OldItemManager.versionUpdate = versionUpdate;
            OldItemManager.ReplaceAllPlayerOldItems(Player);
            if (versionUpdate < 1)
                versionUpdate = 1;

			UpdateEnchantmentEffects();

            //Give all items in old ore bag to player
			for (int i = 0; i < oreBagItems.Length; i++) {
				ref Item item = ref oreBagItems[i];
				if (item.NullOrAir())
					continue;

				StorageManager.TryReturnItemToPlayer(ref item, Player, true);
			}

            //Transfer old ore bag settings to AndroLib
			if (!transferedToAndroLib) {
				if (VacuumOreBag.VacuumOreBag.androLibEnabled) {
					VacuumOreBag.VacuumOreBag.AndroLib.Call("SetUIPosition", OreBag.Instance.BagStorageID, oreBagUILeft, oreBagUITop);
					VacuumOreBag.VacuumOreBag.AndroLib.Call("SetShouldVacuum", OreBag.Instance.BagStorageID, vacuumItemsIntoOreBag);
				}

				transferedToAndroLib = true;
			}

            //Attempt to reduce the size of the player's enchantmentStorage
            if (enchantmentStorageItems.Length > EnchantmentStorageSize) {
                Item[] newEnchantmentStorageItems = new Item[EnchantmentStorageSize];
                int i = 0;
                for (int j = 0; j < enchantmentStorageItems.Length; j++) {
                    Item item = enchantmentStorageItems[j];
					if (item.NullOrAir())
						continue;

					if (i < EnchantmentStorageSize) {
						newEnchantmentStorageItems[i++] = item.Clone();
					}
					else {
						item = Player.GetItem(Player.whoAmI, item, GetItemSettings.InventoryEntityToPlayerInventorySettings);
						if (item.IsAir) {
							Recipe.FindRecipes(true);
                            continue;
						}

						Player.QuickSpawnItem(Player.GetSource_Misc("PlayerDropItemCheck"), item, item.stack);
					}
				}

                for (; i < EnchantmentStorageSize; i++) {
                    newEnchantmentStorageItems[i] = new Item();
				}

                enchantmentStorageItems = newEnchantmentStorageItems;
            }

            #region Debug

            if (LogMethods.debugging) ($"/\\OnEnterWorld({Player.S()})").Log();

            #endregion
        }
		public override void Initialize() {
            enchantingTableEnchantments = emptyEnchantments;
		}
        public override void SaveData(TagCompound tag) {
            tag["enchantingTableItem0"] = enchantingTableItem ?? new();
            for (int i = 0; i < EnchantingTableUI.MaxEssenceSlots; i++) {
                tag[$"enchantingTableEssenceItem{i}"] = enchantingTableEssence[i] ?? new();
            }

            tag["infusionConsumeItem"] = infusionConsumeItem;
            tag["highestTableTierUsed"] = highestTableTierUsed;
            tag["versionUpdate"] = versionUpdate;
			tag["levelsPerLevelUp"] = levelsPerLevelUp;
			tag["enchantmentStorageItems"] = enchantmentStorageItems;
			tag["enchantmentStorageUILocationX"] = enchantmentStorageUILeft;
			tag["enchantmentStorageUILocationY"] = enchantmentStorageUITop;
			tag["enchantmentStorageUILocationLeftWitchUI"] = enchantmentStorageUILeftWitchUI;
			tag["enchantmentStorageUILocationTopWitchUI"] = enchantmentStorageUITopWitchUI;

			tag["enchantingTableUILocationX"] = enchantingTableUILeft;
			tag["enchantingTableUILocationY"] = enchantingTableUITop;
			tag["vacuumItemsIntoEnchantmentStorage"] = vacuumItemsIntoEnchantmentStorage;
			if (trashEnchantmentsFullNames.Count > 0)
				tag["trashEnchantmentsFullNames"] = trashEnchantmentsFullNames.ToList();

			tag["openStorageWhenOpeningTable"] = openStorageWhenOpeningTable;
			string temp = allOfferedItems.StringList((s) => s);
			if (allOfferedItems.Count > 0)
				tag["allOfferedItems"] = allOfferedItems.ToList();

            tag["transferedToAndroLib"] = transferedToAndroLib;
            tag["oreBagItems"] = oreBagItems;

			tag["enchantmentLoadouts"] = enchantmentLoadouts.Select(p => p.Value).ToList();
            tag["loadoutKeys"] = enchantmentLoadouts.Select(p => p.Key).ToList();
			tag["EnchantmentLoadoutUILeft"] = EnchantmentLoadoutUILeft;
            tag["EnchantmentLoadoutUITop"] = EnchantmentLoadoutUITop;
            tag["openLoadoutsWhenOpeningTable"] = openLoadoutsWhenOpeningTable;
            tag["autoTrashOfferedItems"] = autoTrashOfferedItems;
		}
		public override void LoadData(TagCompound tag) {
			enchantingTableItem = tag.Get<Item>("enchantingTableItem0") ?? new();

			for (int i = 0; i < EnchantingTableUI.MaxEssenceSlots; i++) {
                enchantingTableEssence[i] = tag.Get<Item>($"enchantingTableEssenceItem{i}") ?? new();
			}

            infusionConsumeItem = tag.Get<Item>("infusionConsumeItem") ?? new();

            highestTableTierUsed = tag.Get<int>("highestTableTierUsed");
            versionUpdate = tag.Get<byte>("versionUpdate");
			levelsPerLevelUp = tag.Get<int>("levelsPerLevelUp");
			if (levelsPerLevelUp < 1)
				levelsPerLevelUp = 1;

			if (!tag.TryGet("enchantmentStorageItems", out enchantmentStorageItems))
				enchantmentStorageItems = Enumerable.Repeat(new Item(), EnchantmentStorageSize).ToArray();

			if (enchantmentStorageItems.Length < EnchantmentStorageSize)
				enchantmentStorageItems = enchantmentStorageItems.Concat(Enumerable.Repeat(new Item(), EnchantmentStorageSize - enchantmentStorageItems.Length)).ToArray();

            for (int i = 0; i < enchantmentStorageItems.Length; i++) {
                if (enchantmentStorageItems[i] == null || enchantmentStorageItems[i].stack < 1 || enchantmentStorageItems[i].IsAir) {
                    enchantmentStorageItems[i] = new();
                }
            }

			enchantmentStorageUILeft = tag.Get<int>("enchantmentStorageUILocationX");
			enchantmentStorageUITop = tag.Get<int>("enchantmentStorageUILocationY");
            MasterUIManager.CheckOutOfBoundsRestoreDefaultPosition(ref enchantmentStorageUILeft, ref enchantmentStorageUITop, EnchantmentStorage.EnchantmentStorageUIDefaultLeft, EnchantmentStorage.EnchantmentStorageUIDefaultTop);

			enchantmentStorageUILeftWitchUI = tag.Get<int>("enchantmentStorageUILocationLeftWitchUI");
			enchantmentStorageUITopWitchUI = tag.Get<int>("enchantmentStorageUILocationTopWitchUI");
			MasterUIManager.CheckOutOfBoundsRestoreDefaultPosition(ref enchantmentStorageUILeftWitchUI, ref enchantmentStorageUITopWitchUI, EnchantmentStorage.EnchantmentStorageUIDefaultLeftWitchUI, EnchantmentStorage.EnchantmentStorageUIDefaultTopWitchUI);

			enchantingTableUILeft = tag.Get<int>("enchantingTableUILocationX");
            enchantingTableUITop = tag.Get<int>("enchantingTableUILocationY");
			MasterUIManager.CheckOutOfBoundsRestoreDefaultPosition(ref enchantingTableUILeft, ref enchantingTableUITop, EnchantingTableUI.DefaultLeft, EnchantingTableUI.DefaultTop);

            if (tag.TryGet("vacuumItemsIntoEnchantmentStorage", out bool vacuumItemsIntoEnchantmentStorageLoadedValue))
                vacuumItemsIntoEnchantmentStorage = vacuumItemsIntoEnchantmentStorageLoadedValue;

			trashEnchantmentsFullNames = new(tag.Get<List<string>>("trashEnchantmentsFullNames"));
			openStorageWhenOpeningTable = tag.Get<bool>("openStorageWhenOpeningTable");
			allOfferedItems = new(tag.Get<List<string>>("allOfferedItems"));

            transferedToAndroLib = tag.Get<bool>("transferedToAndroLib");

			//Load old ore bag items to give back to player
			if (!tag.TryGet("oreBagItems", out oreBagItems))
                oreBagItems = Enumerable.Repeat(new Item(), OreBagSize).ToArray();

			if (!transferedToAndroLib) {
                //Load old ore bag settings to transfer
				oreBagUILeft = tag.Get<int>("oreBagUILeft");
				oreBagUITop = tag.Get<int>("oreBagUITop");
				MasterUIManager.CheckOutOfBoundsRestoreDefaultPosition(ref oreBagUILeft, ref oreBagUITop, 80, 675);
				if (tag.TryGet("vacuumItemsIntoOreBag", out bool vacuumItemsIntoOreBagLoadedValue))
					vacuumItemsIntoOreBag = vacuumItemsIntoOreBagLoadedValue;
			}

			if (!tag.TryGet("enchantmentLoadouts", out List<List<Item[]>> justLoadouts)) {
				enchantmentLoadouts = new();
			}
            else {
                List<string> loadoutKeys = tag.Get<List<string>>("loadoutKeys") ?? new();
                if (loadoutKeys.Count < justLoadouts.Count) {
                    for (int i = loadoutKeys.Count; i < justLoadouts.Count; i++) {
                        loadoutKeys.Add($"{EnchantmentStorageTextID.Loadout.ToString().Lang_WE(L_ID1.EnchantmentStorageText)} {i + 1}");
                    }
                }

                enchantmentLoadouts = justLoadouts.Select((l, i) => new { Loadout = l, Key = loadoutKeys[i] }).ToDictionary(x => x.Key, x => x.Loadout);
                foreach (string key in enchantmentLoadouts.Keys) {
                    List<Item[]> loadout = enchantmentLoadouts[key];
					for (int i = 0; i < loadout.Count; i++) {
                        Item[] row = loadout[i];
                        for (int k = 0; k < row.Length; k++) {
                            ref Item item = ref row[k];
                            if (!item.IsAir)
                                item.stack = 1;
                        }
                    }
				}
			}

			EnchantmentLoadoutUILeft = tag.Get<int>("EnchantmentLoadoutUILeft");
            EnchantmentLoadoutUITop = tag.Get<int>("EnchantmentLoadoutUITop");
			MasterUIManager.CheckOutOfBoundsRestoreDefaultPosition(ref EnchantmentLoadoutUILeft, ref EnchantmentLoadoutUITop, EnchantmentLoadoutUI.EnchantmentLoadoutUIDefaultLeft, EnchantmentLoadoutUI.EnchantmentLoadoutUIDefaultTop);
            openLoadoutsWhenOpeningTable = tag.Get<bool>("openLoadoutsWhenOpeningTable");
            if (tag.TryGet("autoTrashOfferedItems", out bool val))
                autoTrashOfferedItems = val;
		}
        public override bool ShiftClickSlot(Item[] inventory, int context, int slot) {
			//shop
			if (context == 15 || Main.npcShop != 0)
				return false;

			ref Item item = ref inventory[slot];
			if (usingEnchantingTable) {
				if (!displayEnchantmentLoadoutUI) {
					//Move to Enchanting table
					if (!UIManager.HoveringEnchantingTable) {
						if (CheckShiftClickValid(ref inventory[slot], true))
						    return true;
					}
				}
                else {
                    //Move to Enchantment loadout
                    if (!UIManager.HoveringEnchantmentLoadoutUI) {
						if (item.ModItem is Enchantment) {
							if (EnchantmentLoadoutUI.availableSlotRow > -1 && EnchantmentLoadoutUI.availableSlotIndex > -1) {
								EnchantmentLoadoutUI.UpdateAvailableEnchantmentSlot(this, item);
								SoundEngine.PlaySound(SoundID.MenuTick);

								return true;
							}
						}

						if (!item.IsAir || !Main.mouseItem.IsAir)
							MasterUIManager.SwapMouseItem(ref item);

						return true;
					}
                }
			}

            if (Witch.DisplayingUI) {
                if (!UIManager.HoveringWitchReroll) {
                    //Move to Witch UI
                    if (CheckShiftClickValid_WitchUI(ref inventory[slot], true))
                        return true;
                }
            }

            if (context == 3 && EnchantmentStorage.CanVacuumItem(item, Player)) {
                if (EnchantmentStorage.TryVacuumItem(ref item, Player))
                    return true;
            }

            return false;
		}

		public bool CheckShiftClickValid_WitchUI(ref Item item, bool moveItem = false) {
			bool valid = false;
			if (!item.NullOrAir()) {
				//Trash Item
				if (!Player.trashItem.IsAir) {
					if (Player.trashItem.TryGetEnchantedItemSearchAll(out EnchantedItem tGlobal) && !tGlobal.trashItem) {
						if (trackedTrashItem.TryGetEnchantedItemSearchAll(out EnchantedItem trackedTrashGlobal))
							trackedTrashGlobal.trashItem = false;

						tGlobal.trashItem = true;
					}
				}
				else if (trackedTrashItem.TryGetEnchantedItemSearchAll(out EnchantedItem trackedTrashGlobal)) {
					trackedTrashGlobal.trashItem = false;
				}

				bool hoveringOverTrash = false;
				if (!item.IsAir) {
					if (item.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem) && enchantedItem.trashItem)
						hoveringOverTrash = true;
				}

				bool allowShiftClick = WEMod.clientConfig.AllowShiftClickMoveFavoritedItems;
				bool canMoveItem = !item.favorited || allowShiftClick;

				if (!hoveringOverTrash && canMoveItem) {
                    if (Witch.witchUI_Item.NullOrAir() && WitchRerollUI.WitchUICanAcceptItem(item)) {
                        valid = true;
                        if (moveItem) {
							SoundEngine.PlaySound(SoundID.Grab);
                            Witch.witchUI_Item = item.Clone();
                            item.TurnToAir();
						}
                    }
				}

				if (!valid && moveItem) {
					//Pick up item
					MasterUIManager.SwapMouseItem(ref item);
				}
				else if (valid && !moveItem && !hoveringOverTrash) {
					Main.cursorOverride = CursorOverrideID.InventoryToChest;
				}
			}
			else if (moveItem) {
				//Put item down
				MasterUIManager.SwapMouseItem(ref item);

				return true;//Return true to prevent trashing the item after it's put down
			}

            return valid;
		}

		public bool CheckShiftClickValid(ref Item item, bool moveItem = false) {
            if (EnchantingTableUI.DisplayOfferUI)
                return false;

            bool valid = false;
            if (!item.IsAir) {
                //Trash Item
                if (!Player.trashItem.IsAir) {
                    if (Player.trashItem.TryGetEnchantedItemSearchAll(out EnchantedItem tGlobal) && !tGlobal.trashItem) {
                        if (trackedTrashItem.TryGetEnchantedItemSearchAll(out EnchantedItem trackedTrashGlobal))
                            trackedTrashGlobal.trashItem = false;

                        tGlobal.trashItem = true;
                    }
                }
                else if (trackedTrashItem.TryGetEnchantedItemSearchAll(out EnchantedItem trackedTrashGlobal)) {
                    trackedTrashGlobal.trashItem = false;
                }

                bool hoveringOverTrash = false;
                if (!item.IsAir) {
                    if(item.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem) && enchantedItem.trashItem)
                        hoveringOverTrash = true;
                }

                bool allowShiftClick = WEMod.clientConfig.AllowShiftClickMoveFavoritedItems;
                bool canMoveItem = !item.favorited || allowShiftClick;

                if (!hoveringOverTrash && canMoveItem) {
                    Item tableItem = enchantingTableItem;
                    bool tableItemIsEnchaned = enchantingTableItem.TryGetEnchantedItemSearchAll(out EnchantedItem tableItemGlobal);
					if (item.type == PowerBooster.ID && tableItemIsEnchaned && !tableItemGlobal.PowerBoosterInstalled) {
                        //Power Booster
                        if (moveItem) {
                            tableItemGlobal.PowerBoosterInstalled = true;
                            if (item.stack > 1) {
                                item.stack--;
                            }
							else {
                                item = new Item();
                            }

                            SoundEngine.PlaySound(SoundID.Grab);
                        }

                        valid = true;
                    }
                    else if (item.type == UltraPowerBooster.ID && tableItemIsEnchaned && !tableItemGlobal.UltraPowerBoosterInstalled) {
                        //Ultra Power Booster
                        if (moveItem) {
                            tableItemGlobal.UltraPowerBoosterInstalled = true;
                            if (item.stack > 1) {
                                item.stack--;
                            }
                            else {
                                item = new Item();
                            }

                            SoundEngine.PlaySound(SoundID.Grab);
                        }

                        valid = true;
                    }
                    else {
						//Check/Move item
						if (EnchantingTableUI.ValidItemForEnchantingSlot(item)) {
							if (!item.IsAir) {
								if (CanSwapArmor(tableItem, item)) {
									if (moveItem) {
                                        Item swapItem = item.Clone();
                                        item = enchantingTableItem.IsAir ? new() : enchantingTableItem.Clone();
                                        enchantingTableItem = swapItem;
										SoundEngine.PlaySound(SoundID.Grab);
									}

									valid = true;
								}
							}
						}

						if (!valid) {
                            //Check/Move Enchantment
                            if (item.ModItem is Enchantment enchantment) {
                                int uniqueItemSlot = EnchantingTableUI.FindSwapEnchantmentSlot(enchantment, enchantingTableItem);
                                bool uniqueSlotNotFound = uniqueItemSlot == -1;
								int utilitySlotIndex = EnchantingTableUI.MaxEnchantmentSlots - 1;
                                for (int i = 0; i < EnchantingTableUI.MaxEnchantmentSlots; i++) {
                                    if (!EnchantingTableUI.ValidItemForEnchantingTableEnchantmentSlot(item, i, i == utilitySlotIndex))
                                        continue;

                                    if (item.IsAir)
                                        continue;

                                    if (enchantingTableEnchantments[i].IsAir && uniqueSlotNotFound) {
                                        //Empty slot or not a unique enchantment
                                        if (moveItem) {
                                            int slotIndex = i;
                                            //Utility
                                            if ((enchantment.Utility || RemoveEnchantmentRestrictions) && enchantingTableEnchantments[utilitySlotIndex].IsAir) {
                                                bool utilitySlotAllowedOnItem = tableItemIsEnchaned && EnchantingTableUI.SlotAllowedByConfig(tableItemGlobal.ItemType, 1);
                                                if (utilitySlotAllowedOnItem)
                                                    slotIndex = utilitySlotIndex;
                                            }

											enchantingTableEnchantments[slotIndex] = item.Clone();
											enchantingTableEnchantments[slotIndex].stack = 1;
                                            if (item.stack > 1) {
                                                item.stack--;
                                            }
                                            else {
                                                item = new Item();
                                            }

                                            SoundEngine.PlaySound(SoundID.Grab);
                                        }

                                        valid = true;

                                        break;
                                    }
                                    else {
                                        bool uniqueEnchantmentOnItem = EnchantingTableUI.CheckUniqueSlot(enchantment, uniqueItemSlot, i);
                                        if (!uniqueSlotNotFound && uniqueEnchantmentOnItem && item.type != enchantingTableEnchantments[i].type) {
                                            //Check unique can swap
                                            if (moveItem) {
												TryReturnEnchantmentFromTableToPlayer(i, true);
                                                Item newEnchantmentItem = item.Clone();
                                                newEnchantmentItem.stack = 1;
												if (item.stack > 1) {
													item.stack--;
												}
                                                else {
													item = new Item();
												}
												
                                                enchantingTableEnchantments[i] = newEnchantmentItem;

                                                SoundEngine.PlaySound(SoundID.Grab);
                                            }

                                            valid = true;

                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        //Check/Move Essence
                        if (!valid) {
                            for (int i = 0; i < EnchantingTableUI.MaxEssenceSlots; i++) {
                                if (EnchantingTableUI.ValidItemForEssenceSlot(item, i)) {
                                    if (!item.IsAir) {
                                        bool canTransfer = false;
                                        if (enchantingTableEssence[i].IsAir) {
                                            //essence slot empty
                                            if (moveItem) {
												enchantingTableEssence[i] = item.Clone();
                                                item = new Item();
                                            }

                                            canTransfer = true;
                                        }
                                        else {
                                            //Essence slot not empty
                                            int maxStack = enchantingTableEssence[i].maxStack;
                                            if (enchantingTableEssence[i].stack < maxStack) {
                                                if (moveItem) {
                                                    int ammountToTransfer;
                                                    if (item.stack + enchantingTableEssence[i].stack > maxStack) {
                                                        ammountToTransfer = maxStack - enchantingTableEssence[i].stack;
                                                        item.stack -= ammountToTransfer;
                                                    }
                                                    else {
                                                        ammountToTransfer = item.stack;
                                                        item.stack = 0;
                                                    }

													enchantingTableEssence[i].stack += ammountToTransfer;
                                                }

                                                canTransfer = true;
                                            }
                                        }

                                        //Common to all essence transfer
                                        if (canTransfer) {
                                            if (moveItem)
                                                SoundEngine.PlaySound(SoundID.Grab);

                                            valid = true;
                                            
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (!valid && moveItem) {
					//Pick up item
					MasterUIManager.SwapMouseItem(ref item);
				}
                else if (valid && !moveItem && !hoveringOverTrash) {
                    Main.cursorOverride = CursorOverrideID.InventoryToChest;
                }
            }
            else if (moveItem) {
				//Put item down
				MasterUIManager.SwapMouseItem(ref item);

                return true;//Return true to prevent trashing the item after it's put down
			}

            return valid;
		}
        public bool TryReturnEnchantmentFromTableToPlayer(int enchantmentIndex, bool allowQuickSpawn = false) {
            return TryHandleEnchantmentRemoval(enchantmentIndex, enchantingTableEnchantments, allowQuickSpawn);
		}
        /// <summary>
        /// Default is to return to the player.  Set returnFunc to change this to other things like swapping with mouse.
        /// </summary>
		public bool TryHandleEnchantmentRemoval(int enchantmentIndex, EnchantmentsArray enchantmentsArray, bool allowQuickSpawn = false, Func<Item, Player, bool, ItemAndBool> returnFunc = null) {
			if (returnFunc == null)
				returnFunc = (Item itemToReturn, Player player, bool quickSpawnAllowed) => {
                    bool returnResult = StorageManager.TryReturnItemToPlayer(ref itemToReturn, player, quickSpawnAllowed);
                    return new ItemAndBool(ref itemToReturn, returnResult);
				};

			Item item = enchantmentsArray[enchantmentIndex];
			enchantmentsArray[enchantmentIndex] = new Item();
			ItemAndBool result = returnFunc(item, Player, allowQuickSpawn);
            enchantmentsArray[enchantmentIndex] = result.Item;

			return result.Result;
		}
        public void GiveNewItemToPlayer(int itemType) {
            Item item = new Item(itemType);
			StorageManager.TryReturnItemToPlayer(ref item, Player, true);
		}
		public bool CanSwapArmor(Item newItem, Item currentItem) {
            if (newItem.NullOrAir())
                return true;

            bool tryingToSwapArmor = currentItem.IsAccessoryItem() && !currentItem.IsArmorItem() && (newItem.IsAccessoryItem() || newItem.IsArmorItem());
            bool armorTypeDoesntMatch = currentItem.headSlot > -1 && newItem.headSlot == -1 || currentItem.bodySlot > -1 && newItem.bodySlot == -1 || currentItem.legSlot > -1 && newItem.legSlot == -1;
            if (tryingToSwapArmor || armorTypeDoesntMatch)
                return false;//Fix for Armor Modifiers & Reforging setting item.accessory to true to allow reforging armor

            if (!(newItem.accessory == currentItem.accessory))
                return false;

            return true;
        }
        public bool InEnchantingTableInteractionRange() {
            int chestPointX = Player.chestX;
            int chestPointY = Player.chestY;
			int num = (int)(((double)Player.position.X + (double)Player.width * 0.5) / 16.0);
			int num2 = (int)(((double)Player.position.Y + (double)Player.height * 0.5) / 16.0);
            Rectangle r = new Rectangle(chestPointX * 16, chestPointY * 16, 32, 32);
			r.Inflate(-1, -1);
			Point point = r.ClosestPointInRect(Player.Center).ToTileCoordinates();
			chestPointX = point.X;
			chestPointY = point.Y;

			return num >= chestPointX - Player.tileRangeX && num <= chestPointX + Player.tileRangeX + 1 && num2 >= chestPointY - Player.tileRangeY && num2 <= chestPointY + Player.tileRangeY + 1;
        }
		public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource) {
            if (AndroMod.calamityEnabled) {
                CalamityRespawnMinionSourceItems.Clear();
                for (int i = 0; i < 200; i++) {
                    Projectile projectile = Main.projectile[i];
                    if (projectile.minion && projectile.TryGetWEProjectile(out WEProjectile weProjectile) && !weProjectile.sourceItem.NullOrAir())
                        CalamityRespawnMinionSourceItems.Add(projectile.type, weProjectile.sourceItem);
                }
			}
        }
        public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath) {
            List<Item> items = new();
            if (WEMod.serverConfig.DCUStart) {
                Item item = new Item(ItemID.DrillContainmentUnit);
                items.Add(item);
            }

            return items;
        }
		public override IEnumerable<Item> AddMaterialsForCrafting(out ItemConsumedCallback itemConsumedCallback) {
			itemConsumedCallback = null;
			List<Item> items = new();
			if (usingEnchantingTable) {
				for (int i = 0; i < EnchantingTableUI.MaxEssenceSlots; i++) {
					ref Item item = ref enchantingTableEssence[i];
					if (!item.NullOrAir() && item.stack > 0)
						items.Add(item);
				}
			}

            if (EnchantmentStorage.DisplayStorage || EnchantmentStorage.crafting) {
                for (int i = 0; i < enchantmentStorageItems.Length; i++) {
                    ref Item item = ref enchantmentStorageItems[i];
                    if (!item.NullOrAir() && item.stack > 0 && !item.favorited)
                        items.Add(item);
                }
            }

            return items.Count > 0 ? items : null;
		}
		public override void PostUpdateMiscEffects() {
			ApplyPostMiscEnchants();
		}
		public void OnSwapEquipmentLoadout(int loadoutIndex) {
			//Main.NewText($"new Loadout Number: {loadoutIndex}, old: {Player.CurrentLoadoutIndex}");
		}
		public override void PostSellItem(NPC vendor, Item[] shopInventory, Item item) {
			if (item.TryGetEnchantedItemSearchAll(out _))
				EnchantingTableUI.Siphon(ref item, true);

            Witch.OnSellItem(item);
		}
		public override void PostBuyItem(NPC vendor, Item[] shopInventory, Item item) {
			if (vendor.ModNPC is Witch)
                Witch.OnPurchaseItem(item, shopInventory);
		}

        private uint CurseHardCrowdControlImmunityReset = Main.GameUpdateCount;
        private const uint CurseHardCrowdControlImmunityDuration = 60;
        public bool CanBeCurseHardCrowdControlled => Main.GameUpdateCount >= CurseHardCrowdControlImmunityReset;
		public override void PostUpdate() {
            if (Player.webbed || Player.stoned || Player.frozen)
                CurseHardCrowdControlImmunityReset = Main.GameUpdateCount + CurseHardCrowdControlImmunityDuration;
		}

		#endregion

		#region Modify Hit

		public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers) {
            ModifyHitNPCWithAny(item, target, ref modifiers);
		}
		public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers) {
			Player player = Main.player[proj.owner];
			Item item = null;
			if (proj.TryGetGlobalProjectile(out WEProjectile weProj))
				item = weProj.sourceItem;

            if (item.NullOrAir())
                return;

			ModifyHitNPCWithAny(item, target, ref modifiers, proj);
		}
		public void ModifyHitNPCWithAny(Item item, NPC target, ref NPC.HitModifiers modifiers, Projectile projectile = null) {
            //Called from WEMod detours
            #region Debug

            if (LogMethods.debugging) ($"\\/HitNPC(target: {target.ModFullName()}, Player: {Player.S()}, item: {item.S()}, modifiers: {modifiers}, projectile: {projectile.S()})").Log();

            #endregion

            Equipment.CombineOnHitDictionaries(item);

            ApplyWarDamageRediction(projectile, target, ref modifiers, out bool multiShotConvertedToDamage);

            ApplyMinionDamageModifications(item, ref modifiers, projectile);

            if (GetPlayerModifierStrength(EnchantmentStat.PercentArmorPenetration, out float percentArmorPenetration))
                modifiers.ScalingArmorPenetration += percentArmorPenetration;

            if (GetPlayerModifierStrength(EnchantmentStat.DamageAfterDefenses, out float damageMultiplier, 1f))
                modifiers.FinalDamage *= damageMultiplier;

            if (multiShotConvertedToDamage && GetPlayerModifierStrength(EnchantmentStat.Multishot, out float multishotDamageMultiplier, 1f))
				modifiers.FinalDamage *= multishotDamageMultiplier;

            ApplyModifyHitEnchants(item, target, ref modifiers, projectile);

            #region Debug

            debugBeforeReturn:
            if (LogMethods.debugging) ($"/\\HitNPC(target: {target.ModFullName()}, Player: {Player.S()}, item: {item.S()}, modifiers: {modifiers}, projectile: {projectile.S()})").Log();

            #endregion
        }
        private void ApplyWarDamageRediction(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers, out bool multiShotConvertedToDamage) {
            multiShotConvertedToDamage = false;

            //Minion damage reduction from war enchantment
            if (projectile != null) {
                WEGlobalNPC weGlobalNPC = target.GetWEGlobalNPC();
                WEProjectile wEProjectile = (WEProjectile)projectile.GetMyGlobalProjectile();
                bool minionOrMinionChild = projectile.minion || projectile.type == ProjectileID.StardustGuardian || wEProjectile.parent != null && projectile.GetMyGlobalProjectile().parent.minion;
                if (weGlobalNPC.myWarReduction > 1f && projectile != null && target.whoAmI != Player.MinionAttackTargetNPC && minionOrMinionChild) {
                    modifiers.FinalDamage /= weGlobalNPC.myWarReduction;
                }

                multiShotConvertedToDamage = wEProjectile.multiShotConvertedToDamage;
            }
        }
        private void ApplyMinionDamageModifications(Item item, ref NPC.HitModifiers modifiers, Projectile projectile) {
            //Stardust dragon scale damage multiplier correction//Stardust Dragon
            if (projectile != null) {
                //Some weapons aren't affected by changing their damage stat.
                bool notAffectedByDamageStat = false;
                switch (item.type) {
                    case ItemID.LastPrism:
                    //case ItemID.AbigailsFlower:
                    //case ItemID.StormTigerStaff:
                        notAffectedByDamageStat = true;
						break;
                    default:
                        notAffectedByDamageStat = !WEMod.minionDmgPatchEnabled && (projectile.minion || projectile.DamageType == DamageClass.Summon);
                        break;
				}

				//Minion, item damage doesn't apply to minions
				if (item.TryGetEnchantedItem(out EnchantedWeapon enchantedWeapon) && notAffectedByDamageStat)
                    modifiers.SourceDamage *= enchantedWeapon.infusionDamageMultiplier;

                if (ProjectileID.Sets.StardustDragon[projectile.type]) {
                    float enchantmentScaleMultiplier = GetVanillaModifierStrength(EnchantmentStat.Size);
                    if (enchantedWeapon != null)
                        enchantmentScaleMultiplier *= enchantedWeapon.GetVanillaModifierStrength(EnchantmentStat.Size);

                    if (enchantmentScaleMultiplier > 1f && projectile.scale / enchantmentScaleMultiplier < 1.5f) {
                        float scaleBeforeEnchantments = projectile.scale / enchantmentScaleMultiplier;
                        float correctedMultiplier = 1f + Utils.Clamp((scaleBeforeEnchantments - 1f) * 100f, 0f, 50f) * 0.23f;
                        float vanillaMultiplier = 1f + (Utils.Clamp((projectile.scale - 1f) * 100f, 0f, 50f)) * 0.23f;
                        float combinedMultiplier = correctedMultiplier / vanillaMultiplier;
                        modifiers.SourceDamage *= combinedMultiplier;
                    }
                }
            }
        }
        public void CalculateCriticalChance(Item item, ref NPC.HitModifiers hitModifiers, bool crit, bool? critOverride, Projectile projectile = null) {
            if (critOverride == false)
                return;

            //Critical strike
            bool anyBesidesSummon = item.DamageType != DamageClass.Summon && item.DamageType != DamageClass.MagicSummonHybrid;
			if (anyBesidesSummon || !WEMod.serverConfig.DisableMinionCrits) {
                bool shouldAddWeaponCritChance = true;
                int critChance = 0;
                if (projectile != null) {
                    critChance += projectile.CritChance;
                    shouldAddWeaponCritChance = !anyBesidesSummon || projectile.ContinuouslyUpdateDamageStats;
				}

                if (shouldAddWeaponCritChance)
					critChance += Player.GetWeaponCrit(item);

                critChance += (crit ? 100 : 0) + (critOverride == true ? 100 : 0);
                int critLevel = critChance / 100;
                critChance %= 100;
                if (Main.rand.Next(0, 100) < critChance)
                    critLevel++;

                if (critLevel > 0) {
                    CheckEnchantmentStats(EnchantmentStat.CriticalStrikeDamage, out float critDamageMultiplier, 1f);
                    hitModifiers.SetCrit();
                    critLevel--;

                    if (AllowCriticalChancePast100 && critLevel > 0) {
                        if (MultiplicativeCriticalHits) {
                            //Multiplicative
                            float multiplicativeCritMultiplier = (float)Math.Pow(2 * critDamageMultiplier, critLevel);
                            critDamageMultiplier *= multiplicativeCritMultiplier;
                        }
                        else {
                            //Additive
                            float additiveCritMultiplier = critLevel * (critDamageMultiplier - 0.5f);
                            critDamageMultiplier += additiveCritMultiplier;
                        }
                    }

                    if (critDamageMultiplier != 1f)
                        hitModifiers.CritDamage *= critDamageMultiplier;
                }//MultipleCritlevels
            }
        }

        #endregion

        #region On Hit

        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone) {
            OnHitNPCWithAny(item, target, hit);
        }
        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone) {
            proj.TryGetGlobalProjectile(out WEProjectile weProj);
            Item item = weProj.sourceItem;

            OnHitNPCWithAny(item, target, hit, proj);
        }
        private void OnHitNPCWithAny(Item item, NPC target, NPC.HitInfo hit, Projectile projectile = null) {
            if (!item.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem))
                return;

            if (!target.TryGetWEGlobalNPC(out WEGlobalNPC weGlobalNPC))
                return;

            int damage = hit.Damage;
            float knockback = hit.Knockback;
            bool crit = hit.Crit;
            //Remove target.myWarReduction if hit by non-minion
            TryResetWarReduction(target, projectile);

			//Used to help identify the ModNPC name of modded bosses for setting up mod boss bag support.
			if (GlobalBossBags.printNPCNameOnHitForBossBagSupport)
                $"NPC hit by item: {item.Name}, target.Name: {target.ModFullName()}, target.ModNPC?.Name: {target.ModNPC?.Name}, target.boss: {target.boss}, target.netID: {target.netID}".LogSimple();

            bool fromProjectile = projectile != null;
            bool skipOnHitEffects = fromProjectile ? ((WEProjectile)projectile.GetMyGlobalProjectile()).skipOnHitEffects : false;
            bool dummyTarget = target.IsDummy();

            //Enemy debuffs
            if (!skipOnHitEffects) {
                //Debuffs
                int amaterasuDamageAdded = 0;
                short amaterasuID = (short)ModContent.BuffType<Amaterasu>();
                if (CombinedOnHitDebuffs.ContainsKey(amaterasuID)) {
                    if (weGlobalNPC.amaterasuStrength == 0)
                        weGlobalNPC.amaterasuStrength = CombinedOnHitDebuffs[amaterasuID].BuffStrength;

                    amaterasuDamageAdded = damage * (crit ? 2 : 1);
                }

                if (Main.netMode != NetmodeID.Server) {
                    HashSet<short> dontDissableImmunitiy = new HashSet<short>();
                    Dictionary<short, int> debuffs = new Dictionary<short, int>();
                    foreach (var pair in CombinedOnHitDebuffs) {
                        float chance = pair.Value.Chance;
                        if (chance >= 1f || chance >= Main.rand.NextFloat()) {
                            debuffs.Add(pair.Key, pair.Value.Duration.Ticks);
                            if (!pair.Value.DisableImmunity)
                                dontDissableImmunitiy.Add(pair.Value.BuffID);
                        }
                    }

                    if (target.IsWorm() || AndroGlobalNPC.multipleSegmentBossTypes.ContainsKey(target.netID)) {
                        foreach (short key in debuffs.Keys) {
                            debuffs[key] = (int)Math.Round((float)debuffs[key] / 5f);
                        }
                    }

                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        Net<INetMethods>.Proxy.NetDebuffs(target, amaterasuDamageAdded, weGlobalNPC.amaterasuStrength, debuffs, dontDissableImmunitiy);

                    target.HandleOnHitNPCBuffs(amaterasuDamageAdded, weGlobalNPC.amaterasuStrength, debuffs, dontDissableImmunitiy);
				}
                else {
                    $"NetDebuffs called from server.".Log();
                }
            }

			//One For All
			int oneForAllDamageDealt = ActivateOneForAll(target, item, hit, projectile, dummyTarget);

			if (!dummyTarget) {
                //Player buffs
                if (!skipOnHitEffects) {
                    //On Hit Player buffs
                    Player.ApplyBuffs(CombinedOnHitBuffs);
                }

                ApplyLifeSteal(item, target, hit, oneForAllDamageDealt);
            }

            //GodSlayer
            ActivateGodSlayer(target, hit, fromProjectile);

            UpdateNPCImmunity(target);

            if (!skipOnHitEffects)
                ApplyOnHitEnchants(item, target, damage, knockback, crit, projectile);
		}
		private void TryResetWarReduction(NPC target, Projectile projectile) {
			bool reset = false;
			if (projectile == null) {
				reset = true;
			}
			else {
				if (projectile?.minion == false && projectile.TryGetProjectileWithSourceItem(out ProjectileWithSourceItem projectileWithSourceItem)) {
					DamageClass projDamageType = projectile.DamageType;
					if (projDamageType != DamageClass.Summon && projDamageType != DamageClass.MagicSummonHybrid) {
						if (projectileWithSourceItem.parent == null || !projectileWithSourceItem.parent.minion) {
							reset = true;
						}
					}
				}
			}

			if (reset) {
				NetManager.ResetWarReduction(target);
			}
		}
		private int ActivateOneForAll(NPC target, Item item, NPC.HitInfo hit, Projectile projectile, bool dummyOnly) {
            WEProjectile weProjectile = null;
            if (projectile?.TryGetWEProjectile(out weProjectile) == true && weProjectile.activatedOneForAll)
                return 0;

            if (!CheckEnchantmentStats(EnchantmentStat.OneForAll, out float allForOneMultiplier))
                return 0;

            int total = 0;
            int wormCounter = 0;
            Dictionary<NPC, (int, bool)> oneForAllNPCDictionary = new Dictionary<NPC, (int, bool)>();

            if (!item.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem))
                return 0;

			//Range
            float oneForAllScale = item.scale;
            if (oneForAllScale < 1f)
				oneForAllScale = 1f;

			if (GetSharedVanillaModifierStrength(item, EnchantmentStat.Size, out float strength))
                oneForAllScale *= strength;

			float oneForAllRange = baseOneForAllRange * oneForAllScale;

			//Sorted List by range
            Dictionary<int, float> npcs = SortNPCsByRange(target, oneForAllRange);

			foreach (KeyValuePair<int, float> npcDataPair in npcs.OrderBy(key => key.Value)) {
				int whoAmI = npcDataPair.Key;
				NPC ofaTarget = Main.npc[whoAmI];
				if (!ofaTarget.active)
					continue;

				float distanceFromOrigin = npcDataPair.Value;

				if (dummyOnly && !ofaTarget.IsDummy())
					continue;

				bool isWorm = ofaTarget.IsWorm();

				//Worms
				if (isWorm)
					wormCounter++;

                float allForOneFinalMultiplier = (oneForAllRange - distanceFromOrigin) / oneForAllRange * allForOneMultiplier;

				//Worm damage reduction
				if (isWorm) {
					float wormReductionFactor = 1f;
					if (wormCounter > 10) {
						if (wormCounter <= 20) {
							wormReductionFactor = 1f - (float)(wormCounter - 10f) / 10f;
						}
						else {
							wormReductionFactor = 0f;
						}
					}

                    allForOneFinalMultiplier *= wormReductionFactor;
				}

                NPC.HitInfo ofaHit = ofaTarget.CalculateHitInfo(hit.SourceDamage, 0, hit.Crit);
                ofaHit.Damage = (int)MathF.Round(ofaHit.Damage * allForOneFinalMultiplier);

                total += StrikeNPCDirect(ofaTarget, ofaHit);
			}

            if (weProjectile != null)
                weProjectile.activatedOneForAll = true;

            return total;
		}
        public int StrikeNPCDirect(NPC npc, NPC.HitInfo hit) {
			Player.OnHit(npc.Center.X, npc.Center.Y, npc);
			NPCKillAttempt attempt = new NPCKillAttempt(npc);
			int dmg = npc.StrikeNPC(hit);
			PlayerLoader.OnHitNPC(Player, npc, hit, dmg);

			if (Player.accDreamCatcher && !npc.HideStrikeDamage)
				Player.addDPS(dmg);

			if (Main.netMode != NetmodeID.SinglePlayer)
				NetMessage.SendStrikeNPC(npc, hit);

			int num2 = Item.NPCtoBanner(npc.BannerID());
			if (num2 >= 0)
				Player.lastCreatureHit = num2;

			if (attempt.DidNPCDie())
				Player.OnKillNPC(ref attempt, null);

            return dmg;
		}
		public void ApplyLifeSteal(Item item, NPC npc, NPC.HitInfo hit, int oneForAllDamage) {
            if (!CheckEnchantmentStats(EnchantmentStat.LifeSteal, out float lifeSteal))
                return;

            Player player = Player;

            float healTotal = (hit.Damage + oneForAllDamage) * lifeSteal * (player.moonLeech ? 0.5f : 1f);

            //Temporary until system for damage type checking is implemented
            bool summonDamage = item.DamageType == DamageClass.Summon || item.DamageType == DamageClass.MagicSummonHybrid;
            if (summonDamage)
                healTotal *= MinionLifeStealMultiplier;

            healTotal += lifeStealRollover;

            int heal = (int)healTotal;

			if (player.statLife < player.statLifeMax2) {
				//Player hp less than max
				if (heal > 0 && player.lifeSteal > 0f) {
					int excess = player.statLife + heal - player.statLifeMax2;
					if (excess > 0)
						heal -= excess;

					//Vanilla lifesteal mitigation
					float maxLifeStealMultiplier = ConfigValues.AffectOnVanillaLifeStealLimit;//1.5f;
					CheckEnchantmentStats(EnchantmentStat.MaxLifeSteal, out float maxLifeStealEnchantmentMultiplier);
					maxLifeStealMultiplier /= maxLifeStealEnchantmentMultiplier;//0.05

					float vanillaLifeStealValue = heal * maxLifeStealMultiplier;
					if (player.lifeSteal < vanillaLifeStealValue) {
						heal = (int)(player.lifeSteal / maxLifeStealMultiplier) + 1;
						vanillaLifeStealValue = heal * maxLifeStealMultiplier;
					}

					player.lifeSteal -= vanillaLifeStealValue;

					Projectile.NewProjectile(player.GetSource_ItemUse(item), npc.Center, new Vector2(0, 0), ProjectileID.VampireHeal, 0, 0f, player.whoAmI, player.whoAmI, heal);
				}

				//Life Steal Rollover
				lifeStealRollover = (healTotal - (float)heal) % 1f;
			}
			else {
				//Player hp is max
				lifeStealRollover = 0f;
			}
        }
        public int ActivateGodSlayer(NPC target, NPC.HitInfo hit, bool projectile) {
            if (!CheckEnchantmentStats(EnchantmentStat.GodSlayer, out float godSlayerBonus))
                return 0;

            if (target.friendly || target.townNPC || !target.active || target.netID == NPCID.DD2LanePortal)
                return 0;

            int lifeMax = target.RealLifeMax();
            float actualDamageDealt = hit.Damage;
            float godSlayerDamage = actualDamageDealt * godSlayerBonus * lifeMax / 100f;

            //Projectile damage reduction
            float projectileMultiplier = projectile == true ? 0.5f : 1f;
            godSlayerDamage *= projectileMultiplier;

            //Max life reduction factor
            float denominator = 1f + lifeMax * 49f / 150000f;
            godSlayerDamage /= denominator;

            int godSlayerDamageInt = (int)Math.Round(godSlayerDamage);

			//Hit npc
			NPC.HitInfo gsHit = target.CalculateHitInfo(hit.SourceDamage, 0, hit.Crit);

			//Bypass armor
            if (godSlayerDamage > gsHit.Damage)
			    gsHit.Damage = godSlayerDamageInt;

			return StrikeNPCDirect(target, gsHit);
		}
        private void UpdateNPCImmunity(NPC target) {
            //If projectile/npc doesn't use npc.immune, return
            if (target.immune[Player.whoAmI] <= 0)
                return;

            if (Player.GetWEPlayer().CheckEnchantmentStats(EnchantmentStat.NPCHitCooldown, out float NPCHitCooldownMultiplier, 1f)) {
                //npc.immune
                float oldImmune = (float)target.immune[Player.whoAmI];
                int newImmune = (int)(Math.Round(oldImmune * NPCHitCooldownMultiplier));
                if (newImmune < 1)
                    newImmune = 1;

                target.immune[Player.whoAmI] = newImmune;
            }
        }

		#endregion

		#region Enchantment Effect Hooks

		public override void ModifyShootStats(Item item, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
			foreach(var e in CombinedModifyShootStatEffects) {
                e.ModifyShootStats(item, ref position, ref velocity, ref type, ref damage, ref knockback);
			}
		}
		protected bool GetPlayerModifierStrength(EnchantmentStat enchantmentStat, out float strength, float baseValue = 0f) {
            strength = baseValue;
            if (CombinedEnchantmentStats.ContainsKey(enchantmentStat)) {
                strength = CombinedEnchantmentStats[enchantmentStat].ApplyTo(baseValue);
                return true;
            }

            return false;
        }
        public override bool? CanAutoReuseItem(Item item) {
            //Magic missile and similar weapon prevent auto reuse
            if (WEMod.serverConfig.AutoReuseDisabledOnMagicMissile) {
                Item sampleItem = ContentSamples.ItemsByType[item.type];
                if (sampleItem.mana > 0 && sampleItem.useStyle == ItemUseStyleID.Swing && sampleItem.channel)
                    return null;
            }

            if (item.TryGetEnchantedItem(out EnchantedFishingPole _))
                return null;

            return ApplyAutoReuseEnchants();
        }
        public bool? ApplyAutoReuseEnchants() {
            // Divide effects based on what is needed.
            bool? enableAutoReuse = null;
            foreach (AutoReuse effect in EnchantmentEffects.OfType<AutoReuse>()) {
                if (effect.EnableStat) {
                    enableAutoReuse = true;
                }
                else if (!effect.EnableStat) {
                    return false;
                }
            }

            return enableAutoReuse;
        }
        public override void ModifyItemScale(Item item, ref float scale) {
			if (GetSharedVanillaModifierStrength(item, EnchantmentStat.Size, out float strength))
                scale *= strength;
		}
		public bool GetSharedVanillaModifierStrength(Item item, EnchantmentStat enchantmentStat, out float strength, float baseStrength = 1f) {
			strength = baseStrength;

			if (VanillaStats.ContainsKey(enchantmentStat))
				VanillaStats[enchantmentStat].ApplyTo(ref strength);

			if (item.TryGetEnchantedHeldItem(out EnchantedHeldItem enchantedHeldItem)) {
				if (enchantedHeldItem.VanillaStats.ContainsKey(enchantmentStat))
					enchantedHeldItem.VanillaStats[enchantmentStat].ApplyTo(ref strength);
			}

			return strength != 1f;
		}
		public override void ModifyManaCost(Item item, ref float reduce, ref float mult) {
            if (CheckGetVanillaModifier(EnchantmentStat.ManaUsage, out EStatModifier eStatModifier))
                eStatModifier.ApplyTo(ref reduce, ref mult, item);
        }
        public override void ModifyWeaponCrit(Item item, ref float crit) {
            if (CheckGetVanillaModifier(EnchantmentStat.CriticalStrikeChance, out EStatModifier eStatModifier))
                eStatModifier.ApplyTo(ref crit);
        }
        protected bool CheckGetVanillaModifier(EnchantmentStat enchantmentStat, out EStatModifier m) {
            if (!VanillaStats.ContainsKey(enchantmentStat)) {
                m = null;
                return false;
            }

            m = VanillaStats[enchantmentStat];
            return true;
        }
        public float GetVanillaModifierStrength(EnchantmentStat enchantmentStat) {
            if (VanillaStats.ContainsKey(enchantmentStat))
                return VanillaStats[enchantmentStat].Strength;

            return 1f;
        }
        public bool CheckEnchantmentStats(EnchantmentStat playerStat, out float value, float baseValue = 0f) {
            value = baseValue;
            if (CombinedEnchantmentStats.ContainsKey(playerStat)) {
                CombinedEnchantmentStats[playerStat].ApplyTo(ref value);
                return true;
            }

            return false;
        }
        public bool CheckEnchantmentStatsCheckSourceIsHeldItem(EnchantmentStat stat, IEntitySource source, out float value, float baseValue = 0f) {
            value = baseValue;
            if (EnchantmentStats.ContainsKey(stat)) {
                EStatModifier playerStatModifier = EnchantmentStats[stat];
                Item heldItem = Player.HeldItem;
                bool validSource = false;
                if (source is EntitySource_ItemUse itemSource) {
                    if (itemSource.Item.type == heldItem.type)
                        validSource = true;
                }
                else if (source is EntitySource_Parent parentSource) {
					if (parentSource.Entity is Projectile parentProjectile) {
						if (parentProjectile.TryGetGlobalProjectile(out WEProjectile weProjectile)) {
							if (!weProjectile.sourceItem.NullOrAir()) {
								if (weProjectile.sourceItem.type == heldItem.type)
									validSource = true;
							}
						}
					}
				}

				if (!validSource)
                    return true;

				if (!heldItem.NullOrAir()) {
                    if (heldItem.TryGetGlobalItem(out EnchantedHeldItem enchantedItem)) {
						if (enchantedItem.EnchantmentStats.TryGetValue(stat, out var heldItemStatModifier)) {
							playerStatModifier.CombineWith(heldItemStatModifier);
						}
					}
                }

                playerStatModifier.ApplyTo(ref value);
                return true;
            }
            else {
				Item heldItem = Player.HeldItem;
				bool validSource = false;
				if (source is EntitySource_ItemUse itemSource) {
					if (itemSource.Item.type == heldItem.type)
						validSource = true;
				}
				else if (source is EntitySource_Parent parentSource) {
					if (parentSource.Entity is Projectile parentProjectile) {
						if (parentProjectile.TryGetGlobalProjectile(out WEProjectile weProjectile)) {
							if (!weProjectile.sourceItem.NullOrAir()) {
								if (weProjectile.sourceItem.type == heldItem.type)
									validSource = true;
							}
						}
					}
				}

				if (!validSource)
					return false;

				if (!heldItem.NullOrAir()) {
                    if (heldItem.TryGetEnchantedHeldItem(out EnchantedHeldItem enchantedItem)) {
						if (enchantedItem.EnchantmentStats.TryGetValue(stat, out var heldItemStatModifier)) {
							heldItemStatModifier.ApplyTo(ref value);
							return true;
						}
					}
				}
			}

            return false;
        }
        public bool CheckVanillaEnchantmentStats(EnchantmentStat playerStat, out float value, float baseValue = 0f) {
            value = baseValue;
            if (CombinedVanillaStats.TryGetValue(playerStat, out EStatModifier eStatModifier)) {
                eStatModifier.ApplyTo(ref value);
                return true;
            }

            return false;
        }
        private void CheckClearTimers() {
            uint updateCount = Main.GameUpdateCount;
            List<uint> toRemove = new List<uint>();
            foreach (KeyValuePair<uint, IUseTimer> timer in EffectTimers) {
                if (updateCount >= timer.Key) {
                    toRemove.Add(timer.Key);
                    timer.Value.TimerEnd(this);
                }
                else {
                    break;
                }
            }

            foreach (uint timer in toRemove) {
                EffectTimers.Remove(timer);
            }
        }
        public bool TimerOver(IUseTimer effect) {
            EnchantmentStat statName = effect.TimerStatName;
			
		    return TimerOver(statName);
	    }
        public bool TimerOver(EnchantmentStat statName) {
            foreach (var e in EffectTimers) {
                if (e.Value.TimerStatName == statName)
                    return false;
            }

            return true;
        }
	    public void SetEffectTimer(IUseTimer effect, int duration = -1) {
            if (!effect.MultipleAllowed && EffectTimers.Select(e => e.Value.TimerStatName).Contains(effect.TimerStatName))
                return;

            if (duration == -1)
                duration = effect.TimerDuration.Ticks;

            uint endTime = Main.GameUpdateCount + (uint)duration;
            EffectTimers.Add(endTime, effect);
        }
        private void CheckClearOnTickBuffTimers() {
            uint updateCount = Main.GameUpdateCount;
            foreach (short timer in OnTickBuffTimers.Where(t => t.Value <= updateCount).Select(t => t.Key).ToList()) {
                OnTickBuffTimers.Remove(timer);
            }
        }
        public bool OnTickBuffTimerOver(short id) {
            if (OnTickBuffTimers.Keys.Contains(id))
                return false;

            return true;
        }
        private void SetOnTickBuffTimer(short id, int duration) {
            uint endTime = Main.GameUpdateCount + (uint)duration;
            OnTickBuffTimers.Add(id, endTime);
        }
		public override void ModifyHurt(ref Player.HurtModifiers modifiers) {
            //Increase damage by flat 1 and 1% for every 1 defense less than zero. (Disincentive for having less than 0 defense from curses)
            float configStrength = ConfigValues.NegativeDefensePenaltyMultiplier;
            if (configStrength > 0f) {
				float realDefense = Player.GetRealDefense();
				if (realDefense < 0) {
                    realDefense *= -configStrength;
                    modifiers.FinalDamage.Base += realDefense;
                    modifiers.FinalDamage *= (1f + (float)realDefense / 100f);
				}
			}

            if (CheckEnchantmentStats(EnchantmentStat.DamageReduction, out float mult)) {
				float damageMultiplier = 1f - mult;
				if (WEMod.serverConfig.CalculateDamageReductionBeforeDefense) {
                    modifiers.IncomingDamageMultiplier *= damageMultiplier;
				}
                else {
					modifiers.FinalDamage *= damageMultiplier;
				}
			}
		}
        public void ModifyTimeRate(ref double timeRate, ref double tileUpdateRate, ref double eventUpdateRate) {
            if (Main.dayTime) {
				if (CheckEnchantmentStats(EnchantmentStat.DayTimeRate, out float timeRateMultiplier, 1f))
					timeRate *= timeRateMultiplier;

				if (CheckEnchantmentStats(EnchantmentStat.DayTileUpdateRate, out float tileUpdateRateMulitplier, 1f))
					tileUpdateRate *= tileUpdateRateMulitplier;

				if (CheckEnchantmentStats(EnchantmentStat.DayEventUpdateRate, out float eventUpdateRateMultiplier, 1f))
					eventUpdateRate *= eventUpdateRateMultiplier;
			}
            else {
				if (CheckEnchantmentStats(EnchantmentStat.NightTimeRate, out float timeRateMultiplier, 1f))
					timeRate *= timeRateMultiplier;

				if (CheckEnchantmentStats(EnchantmentStat.NightTileUpdateRate, out float tileUpdateRateMulitplier, 1f))
					tileUpdateRate *= tileUpdateRateMulitplier;

				if (CheckEnchantmentStats(EnchantmentStat.NightEventUpdateRate, out float eventUpdateRateMultiplier, 1f))
					eventUpdateRate *= eventUpdateRateMultiplier;
			}
		}

		/// <summary>
		/// Player.accRunSpeed = speed, Player.runAcceleration = acceleration
		/// </summary>
		public void ModifyHorizontalWingSpeeds() {
			CheckVanillaEnchantmentStats(EnchantmentStat.FlightSpeed, out Player.accRunSpeed, Player.accRunSpeed);
            CheckVanillaEnchantmentStats(EnchantmentStat.FlightAcceleration, out Player.runAcceleration, Player.runAcceleration);
		}

		#endregion

		#region Enchantment Effect Management

        public void ApplyPostMiscEnchants() {
	        CheckClearTimers();
            CheckClearOnTickBuffTimers();

            PlayerEquipment newEquipment = new PlayerEquipment(Player);
            bool needsUpdateEquipment = newEquipment != Equipment;
			Equipment = newEquipment;
			if (needsUpdateEquipment)
                UpdateEnchantmentEffects();

            Equipment.CombineDictionaries();
            Equipment.CombineOnHitDictionaries();

            foreach (IPassiveEffect effect in CombinedPassiveEffects) {
                effect.PostUpdateMiscEffects(this);
            }
            
            ApplyStatEffects();

            ApplyPlayerSetEffects();

            ApplyOnTickBuffs(CombinedOnTickBuffs);
        }
        public void ApplyOnTickBuffs(SortedDictionary<short, BuffStats> buffs) {
            foreach (KeyValuePair<short, BuffStats> buff in buffs) {
                if (OnTickBuffTimerOver(buff.Key)) {
                    buff.Value.TryApplyToPlayer(Player, true);
                    SetOnTickBuffTimer(buff.Key, BuffDurationTicks);
                }
            }
        }
        private void ApplyPlayerSetEffects() {
            foreach(KeyValuePair<EnchantmentStat, PlayerSetEffect> effect in CombinedPlayerSetEffects) {
                effect.Value.SetEffect(Player);
            }
		}
        public void UpdateEnchantmentEffects() {
            Equipment.CheckRemoveEnchantments();
            Equipment.UpdateArmorEnchantmentEffects();
            Equipment.UpdateHeldItemEnchantmentEffects();
        }
        private void ApplyStatEffects() {
            foreach (EnchantmentStat key in CombinedVanillaStats.Keys) {
                ModifyStat(CombinedVanillaStats[key]);
			}

			if (WEMod.dbtEnabled) {
                if (CheckEnchantmentStats(EnchantmentStat.MaxKi, out float maxKeyBonus)) {
					var dbzmod = ModLoader.GetMod("DBZMODPORT");
					var DbtPlayerClass = dbzmod.Code.DefinedTypes.First(a => a.Name.Equals("MyPlayer"));
					var DbtPlayer = DbtPlayerClass.GetMethod("ModPlayer").Invoke(null, new object[] { Player });
					var MaxKi = (int)DbtPlayerClass.GetField("kiMax2").GetValue(DbtPlayer);
					DbtPlayerClass.GetField("kiMax2").SetValue(DbtPlayer,  (int)(MaxKi + maxKeyBonus));
				}
				
                if (CheckEnchantmentStats(EnchantmentStat.KiRegen, out float kiRegenBonus)) {
					var dbzmod = ModLoader.GetMod("DBZMODPORT");
					var DbtPlayerClass = dbzmod.Code.DefinedTypes.First(a => a.Name.Equals("MyPlayer"));
					var DbtPlayer = DbtPlayerClass.GetMethod("ModPlayer").Invoke(null, new object[] { Player });
					var KiRegen = (int)DbtPlayerClass.GetField("kiRegen").GetValue(DbtPlayer);
					DbtPlayerClass.GetField("kiRegen").SetValue(DbtPlayer, (int)(KiRegen + kiRegenBonus));
				}
			}
		}
        private void ModifyStat(EStatModifier sm) {
            EnchantmentStat es = sm.StatType;
            switch (es) {
                case EnchantmentStat.BonusManaRegen:
                    Player.manaRegenBonus = (int)sm.ApplyTo(Player.manaRegenBonus);
                    break;
                case EnchantmentStat.Defense:
                    Player.statDefense += (int)sm.ApplyTo(Player.statDefense.AdditiveBonus.Value);
                    break;
                case EnchantmentStat.FishingPower:
                    Player.fishingSkill = (int)sm.ApplyTo(Player.fishingSkill);
                    break;
                case EnchantmentStat.JumpSpeed:
                    Player.jumpSpeedBoost += sm.ApplyTo(Player.jumpSpeed) - Player.jumpSpeed;
                    break;
                case EnchantmentStat.LifeRegen:
                    Player.lifeRegen = (int)sm.ApplyTo(Player.lifeRegen);
                    break;
                case EnchantmentStat.Luck:
                    Player.luck = sm.ApplyTo(Player.luck);
                    break;
                case EnchantmentStat.ManaRegen:
                    Player.manaRegen = (int)sm.ApplyTo(Player.manaRegen);
                    break;
                case EnchantmentStat.MaxLife:
                    Player.statLifeMax2 = (int)sm.ApplyTo(Player.statLifeMax2);
                    break;
                case EnchantmentStat.MaxMinions:
                    Player.maxMinions = (int)sm.ApplyTo(Player.maxMinions);
                    break;
                case EnchantmentStat.MaxMP:
                    Player.statManaMax2 = (int)sm.ApplyTo(Player.statManaMax2);
                    break;
                case EnchantmentStat.MaxFallSpeed:
                    Player.maxFallSpeed = sm.ApplyTo(Player.maxFallSpeed);
                    break;
                case EnchantmentStat.MiningSpeed:
                    Player.pickSpeed = sm.InvertApplyTo(Player.pickSpeed);
                    break;
                case EnchantmentStat.MovementAcceleration:
                    Player.runAcceleration = sm.ApplyTo(Player.runAcceleration);
                    break;
                case EnchantmentStat.MovementSlowdown:
                    Player.runSlowdown = sm.ApplyTo(Player.runSlowdown);
                    break;
                case EnchantmentStat.MovementSpeed:
                    Player.moveSpeed = sm.ApplyTo(Player.moveSpeed);
                    break;
                case EnchantmentStat.WingTime:
                    Player.wingTimeMax = (int)sm.ApplyTo(Player.wingTimeMax);
                    break;
                case EnchantmentStat.WhipRange:
                    Player.whipRangeMultiplier = sm.ApplyTo(Player.whipRangeMultiplier);
                    break;
            }
        }
        public void ApplyModifyHitEnchants(Item item, NPC target, ref NPC.HitModifiers modifiers, Projectile proj = null) {
            // Not using hitDirection yet.

            foreach (IModifyHitEffect effect in EnchantmentEffects.OfType<IModifyHitEffect>()) {
                effect.OnModifyHit(target, this, item, ref modifiers, proj);
            }
        }
        public void ApplyOnHitEnchants(Item item, NPC target, int damage, float knockback, bool crit, Projectile proj = null) {
            foreach (IOnHitEffect effect in CombinedOnHitEffects) {
                effect.OnHitNPC(target, this, item, damage, knockback, crit, proj);
            }

            ActivateBonusCoins(target, this, item, damage, knockback, crit, proj);
		}
        private void ActivateBonusCoins(NPC npc, WEPlayer wePlayer, Item item, int damage, float knockback, bool crit, Projectile projectile = null) {
            if (!CheckEnchantmentStats(EnchantmentStat.BonusCoins, out float bonusCoinsMultiplier))
                return;

			if (npc.friendly || npc.townNPC || npc.SpawnedFromStatue || npc.type == NPCID.TargetDummy)
				return;

			NPC realNPC = npc.RealNPC();
			int damageInt = damage;
			if (crit)
				damageInt *= 2;

			int life = realNPC.life;
			if (life < 0)
				damageInt += life;

			int lifeMax = realNPC.lifeMax;
			if (damageInt > lifeMax)
				damageInt = lifeMax;

			if (damageInt <= 0)
				return;

			float npcValue = realNPC.value;
			float value = (float)damageInt / (float)lifeMax * npcValue;
			if (value < damageInt)
				value = (float)damageInt;

			value *= 1f + Math.Min(wePlayer.Player.luck, 2f);

			int coins = (int)Math.Round(bonusCoinsMultiplier * value);
			if (coins <= 0)
				coins = 1;

            NetManager.AddNPCValue(realNPC, coins);
		}

		#endregion

		#region Fishing

		public override void AnglerQuestReward(float rareMultiplier, List<Item> rewardItems) {
			
		}
		public override bool? CanConsumeBait(Item bait) {
            if (GetPlayerModifierStrength(EnchantmentStat.AmmoCost, out float strength)) {
                float baitMultiplier = 1f + bait.bait / 50f;
                if (Player.accTackleBox)
                    baitMultiplier += 0.5f;
                
                float combinedStrength = strength * baitMultiplier;
                float rand = Main.rand.NextFloat();
                return rand > combinedStrength ? null : false;
            }

            return null;
        }
		public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition) {
			if (attempt.questFish != -1) {
                if (CheckEnchantmentStats(EnchantmentStat.QuestFishChance, out float questFishChance)) {
                    float weightedChance = .1f;
                    int questFish = attempt.questFish;
                    bool correctZone = false;
                    bool correctHeight = false;

                    //Zones
                    switch (questFish) {
                        case ItemID.BumblebeeTuna:
                            if (attempt.inHoney)
                                correctZone = true;
                            break;
                        case ItemID.Cursedfish:
                        case ItemID.InfectedScabbardfish:
                        case ItemID.EaterofPlankton:
                            if (Player.ZoneCorrupt)
                                correctZone = true;
                            break;
                        case ItemID.BloodyManowar:
                        case ItemID.Ichorfish:
                            if (Player.ZoneCrimson)
                                correctZone = true;
                            break;
                        case ItemID.MirageFish:
                        case ItemID.Pixiefish:
                        case ItemID.UnicornFish:
                            if (Player.ZoneHallow)
                                correctZone = true;
                            break;
                        case ItemID.Pengfish:
                        case ItemID.TundraTrout:
                        case ItemID.Fishron:
                        case ItemID.MutantFlinxfin:
                            if (Player.ZoneSnow)
                                correctZone = true;
                            break;
                        case ItemID.Catfish:
                        case ItemID.Derpfish:
                        case ItemID.TropicalBarracuda:
                        case ItemID.Mudfish:
                            if (Player.ZoneJungle)
                                correctZone = true;
                            break;
                        case ItemID.AmanitaFungifin:
                            if (Player.ZoneGlowshroom)
                                correctZone = true;
                            break;
                        case ItemID.CapnTunabeard:
                        case ItemID.Clownfish:
                            bool ocean = (attempt.X < 380 || attempt.X > Main.maxTilesX - 380) && attempt.waterTilesCount > 1000;
                            if (ocean)
                                correctZone = true;
                            break;
                        case ItemID.ScarabFish:
                        case ItemID.ScorpioFish:
                            if (Player.ZoneDesert)
                                correctZone = true;
                            break;
                        default:
                            correctZone = true;
                            break;
                    }

                    //heightLevels (Higher number is lower level in the game)
                    switch (questFish) {
                        case ItemID.Hungerfish:
                        case ItemID.DemonicHellfish:
                        case ItemID.GuideVoodooFish:
                        case ItemID.Fishotron:
                            if (attempt.heightLevel > 2)
                                correctHeight = true;
                            break;
                        case ItemID.Fishron:
                            if (attempt.heightLevel >= 2)
                                correctHeight = true;
                            break;
                        case ItemID.MirageFish:
                        case ItemID.MutantFlinxfin:
                        case ItemID.Bonefish:
                        case ItemID.Batfish:
                        case ItemID.Jewelfish:
                        case ItemID.Spiderfish:
                            if (attempt.heightLevel > 1)
                                correctHeight = true;
                            break;
                        case ItemID.TundraTrout:
                        case ItemID.Catfish:
                        case ItemID.Derpfish:
                        case ItemID.TropicalBarracuda:
                        case ItemID.Bunnyfish:
                        case ItemID.DynamiteFish:
                        case ItemID.ZombieFish:
                            if (attempt.heightLevel == 1)
                                correctHeight = true;
                            break;
                        case ItemID.Pixiefish:
                        case ItemID.Pengfish:
                        case ItemID.Harpyfish:
                        case ItemID.FallenStarfish:
                        case ItemID.TheFishofCthulu:
                            if (attempt.heightLevel < 2)
                                correctHeight = true;
                            break;
                        case ItemID.Mudfish:
                            if (attempt.heightLevel > 0)
                                correctHeight = true;
                            break;
                        case ItemID.AmanitaFungifin:
                        case ItemID.Cloudfish:
                        case ItemID.Wyverntail:
                        case ItemID.Angelfish:
                            if (attempt.heightLevel == 0)
                                correctHeight = true;
                            break;
                        case ItemID.Dirtfish:
                            if (attempt.heightLevel > 0 && attempt.heightLevel < 3)
                                correctHeight = true;
                            break;
                        default:
                            correctHeight = true;
                            break;
                    }

                    if (correctZone)
                        weightedChance *= 3.16228f;

                    if (correctHeight)
                        weightedChance *= 3.16228f;

                    questFishChance *= weightedChance;
                    float fishingPowerModifier = 1.0f + ((float)attempt.fishingLevel / 100f);
					questFishChance *= fishingPowerModifier;

					float rand = Main.rand.NextFloat();
                    if (rand <= questFishChance) {
                        itemDrop = attempt.questFish;
                        npcSpawn = NPCID.NegativeIDCount;
						if (LogMethods.debugging) $"success, questFishChance: {questFishChance}, rand: {rand}".Log();
                        return;
                    }
					else {
						if (LogMethods.debugging) $"failed, questFishChance: {questFishChance}, rand: {rand}".Log();
                    }
                }
			}

            if (!attempt.inHoney && !attempt.inLava && CheckEnchantmentStats(EnchantmentStat.FishingEnemySpawnChance, out float spawnChance)) {
                if (attempt.bobberType == ProjectileID.BobberBloody)
                    spawnChance *= 2f;

                if (Main.dayTime)
                    spawnChance *= 0.2f;

                float rand = Main.rand.NextFloat();
                if (rand <= spawnChance) {//TODO: convert to WeightedPair
                    List<(float, int)> npcs = new List<(float, int)>() {
                        { (40f, NPCID.Shark) }
                    };
                    if (!Main.dayTime) {
                        npcs.Add((70f, NPCID.ZombieMerman));
                        npcs.Add((70f, NPCID.EyeballFlyingFish));
                    }

                    if (Main.hardMode) {
						if (!Main.dayTime) {
                            npcs.Add((50f, NPCID.GoblinShark));
                            npcs.Add((50f, NPCID.BloodEelHead));
                            npcs.Add((30f, NPCID.BloodNautilus));
                        }

                        npcs.Add((20f, NPCID.DukeFishron));
                        npcs.Add((30f, NPCID.Medusa));
                        npcs.Add((40f, NPCID.WyvernHead));
                    }

                    npcSpawn = npcs.GetOneFromWeightedList(1f);
                }
            }
        }
		private void AI_061_FishingBobber_GiveItemToPlayer(Player thePlayer, int itemType) {
			Item item = new Item();
			item.SetDefaults(itemType);
			if (itemType == 3196) {
				int finalFishingLevel = thePlayer.GetFishingConditions().FinalFishingLevel;
				int minValue = (finalFishingLevel / 20 + 3) / 2;
				int num = (finalFishingLevel / 10 + 6) / 2;
				if (Main.rand.Next(50) < finalFishingLevel)
					num++;

				if (Main.rand.Next(100) < finalFishingLevel)
					num++;

				if (Main.rand.Next(150) < finalFishingLevel)
					num++;

				if (Main.rand.Next(200) < finalFishingLevel)
					num++;

				item.stack = Main.rand.Next(minValue, num + 1);
			}

			if (itemType == 3197) {
				int finalFishingLevel2 = thePlayer.GetFishingConditions().FinalFishingLevel;
				int minValue2 = (finalFishingLevel2 / 4 + 15) / 2;
				int num3 = (finalFishingLevel2 / 2 + 30) / 2;
				if (Main.rand.Next(50) < finalFishingLevel2)
					num3 += 4;

				if (Main.rand.Next(100) < finalFishingLevel2)
					num3 += 4;

				if (Main.rand.Next(150) < finalFishingLevel2)
					num3 += 4;

				if (Main.rand.Next(200) < finalFishingLevel2)
					num3 += 4;

				item.stack = Main.rand.Next(minValue2, num3 + 1);
			}

			PlayerLoader.ModifyCaughtFish(thePlayer, item);
            ItemLoader.CaughtFishStack(item);
            item.newAndShiny = true;
			Item item2 = thePlayer.GetItem(Player.whoAmI, item, default(GetItemSettings));
            if (item2.stack > 0) {
				int number = Item.NewItem(new EntitySource_Loot(Player.HeldItem), (int)Player.position.X, (int)Player.position.Y, Player.width, Player.height, itemType, item2.stack, noBroadcast: false, 0, noGrabDelay: true);
				if (Main.netMode == 1)
					NetMessage.SendData(21, -1, -1, null, number, 1f);
			}
			else {
				item.position.X = Player.Center.X - (float)(item.width / 2);
				item.position.Y = Player.Center.Y - (float)(item.height / 2);
				item.active = true;
				PopupText.NewText(PopupTextContext.RegularItemPickup, item, 0);
			}
		}
		public override void GetFishingLevel(Item fishingRod, Item bait, ref float fishingLevel) {
            if (fishingRod.TryGetEnchantedItem(out EnchantedFishingPole pole))
                fishingLevel += pole.levelBeforeBooster / 100f * GlobalStrengthMultiplier;
		}
		public override void ModifyCaughtFish(Item fish) {

		}
		public override void ModifyFishingAttempt(ref FishingAttempt attempt) {
            if (!attempt.CanFishInLava && CheckEnchantmentStats(EnchantmentStat.LavaFishing, out float mult))
                attempt.CanFishInLava = true;
        }

		#endregion
	}
	public static class PlayerFunctions {
        public static void ApplyBuffs(this Player player, SortedDictionary<short, BuffStats> buffs, bool addToExisting = false) {
            foreach (KeyValuePair<short, BuffStats> buff in buffs) {
                buff.Value.TryApplyToPlayer(player, addToExisting);
            }
        }
	}
}