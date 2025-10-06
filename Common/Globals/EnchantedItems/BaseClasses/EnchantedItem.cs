using KokoLib;
using KokoLib.Nets;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;
using Terraria.ModLoader.IO;
using Terraria.Social;
using Terraria.Utilities;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using WeaponEnchantments.Items;
using WeaponEnchantments.Items.Enchantments;
using WeaponEnchantments.ModIntegration;
using WeaponEnchantments.ModLib.KokoLib;
using WeaponEnchantments.UI;
using static WeaponEnchantments.Common.Configs.ConfigValues;
using static androLib.Common.EnchantingRarity;
using static WeaponEnchantments.Common.Globals.EnchantedItemStaticMethods;
using static WeaponEnchantments.Common.Globals.EnchantedWeaponStaticMethods;
using static WeaponEnchantments.Items.Enchantment;
using androLib.Common.Utility;

namespace WeaponEnchantments.Common.Globals
{
	using androLib;
	using androLib.Common.Globals;
	using Terraria.Social.Base;

	public abstract class EnchantedItem : GlobalItem {

        #region Constants

        public const int MAX_Level = 40;

        #endregion

        #region Reforge (static)

        public static Item reforgeItem = null;
        public static Item calamityAndAutoReforgePostReforgeItem = null;
        public static bool calamityReforged = false;
        public static bool cloneReforgedItem = false;

        #endregion

        #region Tracking (static)

        public static bool resetGlobals = false;

        #endregion

        #region Enchantment

        public EnchantmentsArray enchantments;

        #endregion

        #region Infusion

        public string infusedItemName = "";
        protected int _infusionValueAdded = 0;
        public int InfusionValueAdded {
            get { return _infusionValueAdded; }
            set {
                _infusionValueAdded = value;
            }
        }

        #endregion

        #region Experience

        protected int _experience = 0;
        public int Experience {
            get { return _experience; }
            set {
                int lastValue = _experience;
                _experience = value;

                //If changed, update Level/Value
                if (lastValue != _experience)
                    UpdateLevel();
            }
        }
        public int levelBeforeBooster = 0;
        protected bool _powerBoosterInstalled = false;
        public bool PowerBoosterInstalled {
            get { return _powerBoosterInstalled; }
            set {
                bool lastValue = _powerBoosterInstalled;
                _powerBoosterInstalled = value;

                //If changed, update Level/Value
                if (lastValue != _powerBoosterInstalled)
                    UpdateLevel();
            }
        }

        protected bool _ultraPowerBoosterInstalled = false;
        public bool UltraPowerBoosterInstalled {
            get { return _ultraPowerBoosterInstalled; }
            set {
                bool lastValue = _ultraPowerBoosterInstalled;
                _ultraPowerBoosterInstalled = value;

                //If changed, update Level/Value
                if (lastValue != _ultraPowerBoosterInstalled)
                    UpdateLevel();
            }
        }
        public int level = 0;

        #endregion

        #region Reforge (instance)

        public int prefix = 0;
        public bool normalReforge = false;

        #endregion

        #region Minion Items

        public Projectile masterProjectile = null;

        #endregion

        #region Tracking (instance)

        public virtual EItemType ItemType { get; }
        public bool inEnchantingTable = false;
        public bool trashItem = false;
        public bool favorited = false;

        private bool globalsSetup = false;

        #endregion

        #region Properties (instance)

        public Item Item;
        public bool Enchanted => Experience != 0 || PowerBoosterInstalled || UltraPowerBoosterInstalled;
        public bool Modified => Enchanted || infusedItemName != "";

        #endregion

        public EnchantedItem() {
            enchantments = new EnchantmentsArray(this);
        }
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
            return entity.IsEnchantable();
		}
		public override GlobalItem Clone(Item item, Item itemClone) {
			SetupGlobals(item);

			EnchantedItem clone;
            //Set Tracked Item (similar to ModItem.Item)
            if (resetGlobals) {
                Item = itemClone;
            }
            else {
                Item = item;
            }

            if (cloneReforgedItem || resetGlobals) {
                if (itemClone.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem)) {
                    clone = enchantedItem;
                }
                else {
					GameMessageTextID.FailedToCloneItem.ToString().Lang_WE(L_ID1.GameMessages, new object[] { item.S(), itemClone.S(), cloneReforgedItem.S(), resetGlobals.S() }).LogNT(ChatMessagesIDs.CloneFailGetEnchantedItem);
                    return this;
                }

                #region Infusion

                clone.infusedItemName = infusedItemName;
                clone.InfusionValueAdded = InfusionValueAdded;

                #endregion

                #region Experience

                clone.Experience = _experience;
                clone.levelBeforeBooster = levelBeforeBooster;
                clone.PowerBoosterInstalled = PowerBoosterInstalled;
                clone.UltraPowerBoosterInstalled = UltraPowerBoosterInstalled;
                clone.level = level;

                #endregion

                #region Reforge (instance)

                if (resetGlobals) {
                    clone.prefix = prefix;
                }

                #endregion

                #region Minion weapons

                clone.masterProjectile = masterProjectile;

                #endregion

                #region Tracking (instance)

                clone.inEnchantingTable = inEnchantingTable;
                clone.favorited = favorited;
                clone.globalsSetup = globalsSetup;

				#endregion
			}
            else {
                clone = (EnchantedItem)base.Clone(item, itemClone);
            }

            #region Enchantments

            clone.enchantments = enchantments.Clone(clone);

            if (resetGlobals) {
                for (int i = 0; i < enchantments.Length; i++)
                    clone.enchantments[i] = new Item();
            }
            else {
                //fixes enchantments being applied to all of an item instead of just the instance
                //for (int i = 0; i < enchantments.Length; i++)
                //    clone.enchantments[i] = enchantments[i]?.Clone();
            }

            #endregion

            return clone;
        }
        public override void LoadData(Item item, TagCompound tag) {
            Item = item;

            #region Debug

            if (LogMethods.debugging) ($"\\/LoadData(" + item.Name + ")").Log();

            #endregion

            #region Enchantment

            for (int i = 0; i < EnchantingTableUI.MaxEnchantmentSlots; i++) {
                if (tag.Get<Item>("enchantments" + i.ToString()) != null) {
                    Item enchantmentItem = tag.Get<Item>($"enchantments{i}");
					OldItemManager.ReplaceOldItem(ref enchantmentItem);
                    enchantments.AddNoUpdate(enchantmentItem, i);
                }
            }

            #endregion

            #region Experience

            Experience = tag.Get<int>("experience");
            PowerBoosterInstalled = tag.Get<bool>("powerBooster");
            UltraPowerBoosterInstalled = tag.Get<bool>("ultraPowerBooster");

            #endregion

            #region Tracking (instance)

            favorited = item.favorited;

			#endregion

			#region Infusion

			infusedItemName = tag.Get<string>("infusedItemName");

            #endregion

            if (Experience < 0)
                Experience = int.MaxValue;

            #region Debug

            if (LogMethods.debugging) ($"/\\LoadData(" + item.Name + ")").Log();

            #endregion
        }
        public override void SaveData(Item item, TagCompound tag) {

            #region Enchantment

            for (int i = 0; i < EnchantingTableUI.MaxEnchantmentSlots; i++) {
                if (!enchantments[i].IsAir) {
                    tag[$"enchantments{i}"] = enchantments[i];
                }
            }

            #endregion

            #region Experience

            tag["experience"] = Experience;
            tag["powerBooster"] = PowerBoosterInstalled;
            tag["ultraPowerBooster"] = UltraPowerBoosterInstalled;

            #endregion

            #region Infusion

            if (infusedItemName != "") {
				tag["infusedItemName"] = infusedItemName;
			}

            #endregion
        }
        public override void NetSend(Item item, BinaryWriter writer) {

            writer.Write(Modified);

            if (!Modified)
                return;

            #region Debug

            if (LogMethods.debugging) ($"\\/NetSend(" + item.Name + ")").Log();

			#endregion

			#region Enchantment

			for (int i = 0; i < EnchantingTableUI.MaxEnchantmentSlots; i++) {
                writer.Write((short)enchantments[i].type);

                #region Debug

                if (LogMethods.debugging) ($"e " + i + ": " + enchantments[i].Name).Log();

                #endregion
            }

            #endregion

            #region Experience

            writer.Write(Experience);
            writer.Write(PowerBoosterInstalled);
            writer.Write(UltraPowerBoosterInstalled);

            #endregion

            #region Infusion

            bool noName = infusedItemName == "";
            writer.Write(noName);
            if (!noName) {
                writer.Write(infusedItemName);
            }

            #endregion

            #region Debug

            if (LogMethods.debugging) ($"/\\NetSend(" + item.Name + ")").Log();

			#endregion
		}
        public override void NetReceive(Item item, BinaryReader reader) {
            Item = item;

            bool dataExistsInReader = reader.ReadBoolean();

            if (!dataExistsInReader)
                return;

            #region Debug

            if (LogMethods.debugging) ($"\\/NetRecieve(" + item.Name + ")").Log();

			#endregion

			#region Enchantment

			for (int i = 0; i < EnchantingTableUI.MaxEnchantmentSlots; i++) {
                enchantments[i] = new Item(reader.ReadUInt16());

                #region Debug

                if (LogMethods.debugging) ($"e " + i + ": " + enchantments[i].Name).Log();

                #endregion
            }

            #endregion

            #region Experience

            Experience = reader.ReadInt32();
            PowerBoosterInstalled = reader.ReadBoolean();
            UltraPowerBoosterInstalled = reader.ReadBoolean();

            #endregion

            #region Infusion

            bool noName = reader.ReadBoolean();
            if (!noName) {
                infusedItemName = reader.ReadString();
            }

            #endregion

            SetupGlobals(item);

            #region Debug

            if (LogMethods.debugging) ($"/\\NetRecieve(" + item.Name + ")").Log();

			#endregion
		}
        public override void UpdateInventory(Item item, Player player) {
            //Track favorited
            if (item.favorited) {
                if (!favorited && WEModSystem.FavoriteKeyDown) {
                    favorited = true;
                }
            }
            else {
                if (favorited) {
                    if (!WEModSystem.FavoriteKeyDown) {
                        item.favorited = true;
                    }
                    else {
                        favorited = false;
                    }
                }
            }

            SetupGlobals(item);
        }
		public override void UpdateEquip(Item item, Player player) {
			SetupGlobals(item);
		}
		public override void UpdateVanity(Item item, Player player) {
            SetupGlobals(item);
		}
		public void SetupGlobals(Item item) {
			if (globalsSetup)
                return;

			item.SetupGlobals();
			globalsSetup = true;
        }
        public void UpdateLevel() {
            int l;
            for (l = 0; l < MAX_Level; l++) {
                if (_experience < WEModSystem.levelXps[l]) {
                    level = l + 1;
                    break;
                }
            }

            if (l == MAX_Level) {
                levelBeforeBooster = MAX_Level;
            }
            else {
                levelBeforeBooster = l;
            }

            level = levelBeforeBooster;

            if (PowerBoosterInstalled)
                level += 10;

            if (UltraPowerBoosterInstalled)
                level += 20;
        }
        public int GetLevelsAvailable() {
            int totalEnchantmentLevelCost = 0;
            for (int i = 0; i < enchantments.Length; i++) {
                if (enchantments[i] != null && !enchantments[i].IsAir) {
                    Enchantment enchantment = (Enchantment)enchantments[i].ModItem;
                    totalEnchantmentLevelCost += enchantment.GetCapacityCost();
                }
            }

            return level - totalEnchantmentLevelCost;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
            if (Item == null)
                Item = item;

            WEPlayer wePlayer = Main.LocalPlayer.GetWEPlayer();

            GetTopTooltips(item, tooltips);

            GetXPAndLevelTooltips(item, tooltips);

            //infusionTooltip
            if (infusedItemName != "") {
                string tooltip = GetInfusedItemTooltip(item);

                if (tooltip != "")
                    tooltips.Add(new TooltipLine(Mod, "infusionTooltip", tooltip) { OverrideColor = Color.DarkRed });
            }
            else if (wePlayer.usingEnchantingTable || WEMod.clientConfig.AlwaysDisplayInfusionPower) {
                string tooltip = GetInfusionTooltip(item);

                if (tooltip != "")
                    tooltips.Add(new TooltipLine(Mod, "infusionTooltip", tooltip) { OverrideColor = Color.DarkRed });
            }

            //newInfusionTooltip
            if (inEnchantingTable && !wePlayer.infusionConsumeItem.IsAir) {
                if (this.IsSameEnchantedType(wePlayer.infusionConsumeItem)) {
                    string tooltip = GetNewInfusedItemTooltip(item, wePlayer);

                    if (tooltip != "")
                        tooltips.Add(new TooltipLine(Mod, "newInfusionTooltip", tooltip) { OverrideColor = Color.DarkRed });
                }
            }

            GetEnchantementTooltips(tooltips);
        }
        protected virtual void GetTopTooltips(Item item, List<TooltipLine> tooltips) { }
        protected virtual void GetXPAndLevelTooltips(Item item, List<TooltipLine> tooltips) {
            //Xp and level Tooltips
            if (Enchanted || inEnchantingTable) {
                string pointsName = WEMod.clientConfig.UsePointsAsTooltip ? $"{EnchantmentGeneralTooltipsID.Points}".Lang_WE(L_ID1.Tooltip, L_ID2.EnchantmentGeneralTooltips) : $"{EnchantmentGeneralTooltipsID.EnchantmentCapacity}".Lang_WE(L_ID1.Tooltip, L_ID2.EnchantmentGeneralTooltips);// "Points" : "Enchantment Capacity";

                string levelTooltip = $"{EnchantmentGeneralTooltipsID.LevelAvailable}".Lang_WE(L_ID1.Tooltip, L_ID2.EnchantmentGeneralTooltips, new object[] { levelBeforeBooster, pointsName, GetLevelsAvailable() });// $"Level: {levelBeforeBooster}  {pointsName} available: {GetLevelsAvailable()}";
                if (PowerBoosterInstalled || UltraPowerBoosterInstalled) {
                    bool both = PowerBoosterInstalled && UltraPowerBoosterInstalled;
                    levelTooltip += $" ({$"{EnchantmentGeneralTooltipsID.BoosterInstalled}".Lang_WE(L_ID1.Tooltip, L_ID2.EnchantmentGeneralTooltips)} {(PowerBoosterInstalled ? $"{EnchantmentGeneralTooltipsID.NormalBoosterAbreviation}".Lang_WE(L_ID1.Tooltip, L_ID2.EnchantmentGeneralTooltips) : "")}{(both ? " " : "")}{(UltraPowerBoosterInstalled ? $"{EnchantmentGeneralTooltipsID.UltraBoosterAbreviation}".Lang_WE(L_ID1.Tooltip, L_ID2.EnchantmentGeneralTooltips) : "")})";
                }

                tooltips.Add(new TooltipLine(Mod, "level", levelTooltip) { OverrideColor = Color.LightGreen });

                string bonusPerLevelTooltip = GetPerLevelBonusTooltip();
                if (bonusPerLevelTooltip != "")
                    tooltips.Add(new TooltipLine(Mod, "PerLevelBonus", bonusPerLevelTooltip) { OverrideColor = Color.Blue });

                //Experience tooltip
                string experienceTooltip = $"{EnchantmentGeneralTooltipsID.Experience}".Lang_WE(L_ID1.Tooltip, L_ID2.EnchantmentGeneralTooltips, new object[] { Experience });//$"Experience: {Experience}";
				if (levelBeforeBooster < MAX_Level) {
                    experienceTooltip += $"{EnchantmentGeneralTooltipsID.ToNextLevel}".Lang_WE(L_ID1.Tooltip, L_ID2.EnchantmentGeneralTooltips, new object[] { WEModSystem.levelXps[levelBeforeBooster] - Experience });// $" ({WEModSystem.levelXps[levelBeforeBooster] - Experience} to next level)";
				}
                else {
                    experienceTooltip += $" {$"{EnchantmentGeneralTooltipsID.MaxLevel}".Lang_WE(L_ID1.Tooltip, L_ID2.EnchantmentGeneralTooltips)}";// " (Max Level)";
                }

                tooltips.Add(new TooltipLine(Mod, "experience", experienceTooltip) { OverrideColor = Color.White });
            }
        }
        protected virtual string GetPerLevelBonusTooltip() => "";
        protected virtual string GetInfusedItemTooltip(Item item) => "";
        protected virtual string GetInfusionTooltip(Item item) => "";
        protected virtual string GetNewInfusedItemTooltip(Item item, WEPlayer wePlayer) => "";
        protected virtual void GetEnchantementTooltips(List<TooltipLine> tooltips) {
            IEnumerable<Enchantment> enchantmentModItems = enchantments.All.Select(e => e.ModItem).OfType<Enchantment>();
		    int i = 0;
            foreach (Enchantment enchantment in enchantmentModItems) {
		        string tooltip = enchantment.ShortTooltip;
		        tooltips.Add(new TooltipLine(Mod, $"enchantment{i}", tooltip) { OverrideColor = TierColors[enchantment.EnchantmentTier] });
		        i++;
            }
        }
		public void GainXP(Item item, int xpInt) {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();

            int currentLevel = levelBeforeBooster;

            //xp < 0 return
            if (xpInt < 0) {
				GameMessageTextID.PreventedLoosingExperience.ToString().Lang_WE(L_ID1.GameMessages, new object[] { item.S() }).LogNT(ChatMessagesIDs.GainXPPreventedLoosingExperience);//$"Prevented your {item.S()} from loosing experience due to a calculation error.".LogNT_WE(ChatMessagesIDs.GainXPPreventedLoosingExperience);

				return;
            }

            if(Experience + xpInt >= 0) {
                //Add xp
                Experience += xpInt;
            }
			else {
                //Prevent overflow
                Experience = int.MaxValue;
            }

            //Already max level return
            if (currentLevel >= MAX_Level)
                return;

            //Level up message
            if (levelBeforeBooster > currentLevel && (wePlayer.usingEnchantingTable || (
                    item.IsWeaponItem() && WEMod.clientConfig.AlwaysDisplayWeaponLevelUpMessages || 
                    item.IsArmorItem() && WEMod.clientConfig.AlwaysDisplayArmorLevelUpMessages ||
                    item.IsAccessoryItem() && WEMod.clientConfig.AlwaysDisplayAccessoryLevelUpMessages ||
                    (item.IsTool() || item.IsFishingPole()) && WEMod.clientConfig.AlwaysDisplayToolLevelUpMessages))) {
                if (levelBeforeBooster >= MAX_Level) {
                    SoundEngine.PlaySound(SoundID.Unlock);
                    Main.NewText(GameMessageTextID.CongradulationsMaxLevel.ToString().Lang_WE(L_ID1.GameMessages, new object[] { wePlayer.Player.name, item.Name, levelBeforeBooster, WEModSystem.levelXps[levelBeforeBooster - 1] }));// $"Congratulations!  {wePlayer.Player.name}'s {item.Name} reached the maximum level, {levelBeforeBooster} ({WEModSystem.levelXps[levelBeforeBooster - 1]} xp).");
                }
                else {
                    Main.NewText(GameMessageTextID.ItemLevelUp.ToString().Lang_WE(L_ID1.GameMessages, new object[] { wePlayer.Player.name, item.Name, levelBeforeBooster, WEModSystem.levelXps[levelBeforeBooster - 1] }));//$"{wePlayer.Player.name}'s {item.Name} reached level {levelBeforeBooster} ({WEModSystem.levelXps[levelBeforeBooster - 1]} xp).");
				}
            }
        }
        public override bool CanRightClick(Item item) {
            if (item.stack <= 1)
                return false;

            if (!Modified)
                return false;

            //Prevent global item duplication exploit
            if (Main.mouseItem.IsAir) {
                return true;
            }

            return false;
        }
        public override void RightClick(Item item, Player player) {
			if (item.stack <= 1)
                return;

            if (!Modified)
                return;

            //Prevent specific items from running RightClickStackableItem
            if (item.ModFullName().Contains("PrimaryZenith"))
                return;

            //Prevent global item duplication exploit
            if (Main.mouseItem.IsAir) {
                RightClickStackableItem(item);
            }
        }
        public void RightClickStackableItem(Item item) {
            //Dont prevent duplication if stack is 1
            if (item.stack <= 1)
                return;

            if (!Modified)
                return;

            //Prevent global item duplication exploit
            if (Main.mouseItem.IsAir) {
                Main.mouseItem = new Item(item.type);
            }
            else if (Main.mouseItem.type == item.type) {
                Main.mouseItem.stack++;
            }
        }
		public override void PreReforge(Item item) {

			#region Debug

			if (LogMethods.debugging) ($"\\/PreReforge({item.S()})").Log();

            #endregion

            //Calamity
            reforgeItem = item.Clone();

			#region Debug

			if (LogMethods.debugging) {
                string s = $"reforgeItem: {reforgeItem.S()}, prefix: {reforgeItem.prefix}, Enchantments: ";
                if(reforgeItem.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem)) {
                    foreach (Item enchantment in enchantedItem.enchantments.All) {
                        s += enchantment.S();
                    }
                    s.Log();
                }

                ($"/\\PreReforge({item.S()})").Log();
            }

			#endregion
        }
        public override void PostReforge(Item item) {

            #region Debug

            if (LogMethods.debugging) ($"\\/PostReforge({item.S()})").Log();

            #endregion

            if (!Modified)
                return;

			Item = item;

            //Calamity
            if (AndroMod.calamityEnabled)
                calamityReforged = true;

			//Calamity deletes global data after reforge, so it is handled on the next tick in WEModSystem.
			if (!AndroMod.calamityEnabled) {
                //Vanilla and AutoReforge (No Calamity)
                ReforgeItem(ref item, Main.LocalPlayer);
            }
			else {
                //Only used when both Calamity and AutoReforge are enabled.
                calamityAndAutoReforgePostReforgeItem = item;
            }

            #region Debug

            if (LogMethods.debugging) ($"/\\PostReforge({item.S()})").Log();

            #endregion
        }
        public static void ReforgeItem(ref Item item, Player player, bool needCloneGlobals = false) {
            if (item.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem)) {
                //Calamity
                if (needCloneGlobals && reforgeItem.TryGetEnchantedItemSearchAll(out EnchantedItem rGlobal)) {
                    cloneReforgedItem = true;
                    rGlobal.Clone(reforgeItem, item);
                    cloneReforgedItem = false;
                }

                //Vanilla
                enchantedItem.prefix = -1;
			}

            //Calamity
            reforgeItem = null;

            //Vanilla
            item.UpdateItemStats();

			//Calamity
			calamityReforged = false;

            //Calamity and AutoReforge
            calamityAndAutoReforgePostReforgeItem = null;
        }
		public override void OnCreated(Item item, ItemCreationContext context) {
			if(context is RecipeItemCreationContext recipeCreationContext)
				item.CombineEnchantedItems(recipeCreationContext.ConsumedItems, true);
		}
		public override bool CanStackInWorld(Item destination, Item source) {
			if (destination.maxStack < 2 || !destination.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem1) || !source.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem2))
				return true;

			bool modified1 = enchantedItem1.Modified;
			bool modified2 = enchantedItem2.Modified;

			//Prevent stacking in the world if both modified.
			if (modified1 && modified2 && (destination.whoAmI > 0 || source.whoAmI > 0))
				return false;

			//Prevent stackable armor from merging (such as buckets)
			if (enchantedItem1 is EnchantedArmor enchantedArmor1 && enchantedItem2 is EnchantedArmor enchantedArmor2 && enchantedArmor1.infusedArmorSlot != enchantedArmor2.infusedArmorSlot)
				return false;

			return base.CanStackInWorld(destination, source);
		}
		public override void OnStack(Item destination, Item source, int numToTransfer) {
            if (destination.maxStack < 2 || !destination.TryGetEnchantedItemSearchAll(out EnchantedItem destinationEnchantedItem) || !source.TryGetEnchantedItemSearchAll(out EnchantedItem sourceEnchantedItem))
                return;

            //Only combine if the destination item already exists to prevent duplicating enchantments and xp.
            if (destination.stack > 0) {
				List<Item> list = new List<Item>() { source };
				if (sourceEnchantedItem is EnchantedWeapon sourceEnchantedWeapon) {
					if (sourceEnchantedWeapon.GetStack0(source)) {
						destination.stack--;
						sourceEnchantedWeapon.SetStack0(source, false);
					}
				}

				destination.CombineEnchantedItems(list);

                if (destination.stack > 0) {
					if (destinationEnchantedItem is EnchantedWeapon enchantedWeapon && enchantedWeapon.GetStack0(destination)) {
						if (source.stack > 0) {
							destination.stack--;
							enchantedWeapon.SetStack0(destination, false);
						}
					}
				}
			}

			//Clear source if source stack will be > 0 after the transfer
			if (numToTransfer < source.stack) {
				//Clear enchantments in enchanting table if source is in it (Will only have cleared off of the player tracked enchantments from combining)
				if (sourceEnchantedItem.inEnchantingTable) {
					for (int i = 0; i < enchantments.Length; i++) {
						Main.LocalPlayer.GetWEPlayer().enchantingTableEnchantments[i] = new Item();
					}
				}

				//Reset source globals
				Item tempItem = new Item(destination.type);
				resetGlobals = true;
				if (tempItem.TryGetEnchantedItemSearchAll(out EnchantedItem tempGlobal))
					tempGlobal.Clone(tempItem, source);

				resetGlobals = false;
			}
		}
		public override void SplitStack(Item destination, Item source, int numToTransfer) {
            OnStack(destination, source, numToTransfer);
		}
		public void ResetGlobals(Item item) {
			Item tempItem = new Item(item.type);
			resetGlobals = true;
			if (tempItem.TryGetEnchantedItem(out EnchantedWeapon tempEnchantedWeapon)) {
				tempEnchantedWeapon.Clone(tempItem, item);
			}
			else if (tempItem.TryGetEnchantedItemSearchAll(out EnchantedItem tempEnchantedItem)) {
				tempEnchantedItem.Clone(tempItem, item);
			}

			resetGlobals = false;
		}
		public override void OnResearched(Item item, bool fullyResearched) {
			EnchantingTableUI.ReturnAllModifications(ref item);
		}
        public virtual void ResetInfusion(Item item) {
            infusedItemName = "";
			InfusionValueAdded = 0;
		}
	}
	public class EnchantmentsArray
	{
		private Item[] _enchantments;
		private EnchantedItem enchantedItem;
		public int Length => _enchantments.Length;
		public EnchantmentsArray(EnchantedItem enchantedItem) {
			_enchantments = new Item[EnchantingTableUI.MaxEnchantmentSlots];
			for (int i = 0; i < EnchantingTableUI.MaxEnchantmentSlots; i++) {
				_enchantments[i] = new Item();
			}

			this.enchantedItem = enchantedItem;
		}
		public IEnumerable<Item> All => _enchantments;
		public Item this[int index] {
			get => _enchantments[index];
			set {
                if (value == null || value.stack < 0) {
					value = new();
                    value.TurnToAir();
				}

                bool wasAir = _enchantments[index].IsAir;
                bool isAir = value.IsAir;
                if (_enchantments[index]?.ModItem is Enchantment oldEnchantment) {
                    Item oldEnchantmentItem = _enchantments[index];
                    _enchantments[index] = new Item();
					RemoveEnchantment(oldEnchantment);
					_enchantments[index] = oldEnchantmentItem;
				}

				_enchantments[index] = value;
				if (wasAir && isAir)
                    return;

                if (!isAir)
                    ApplyEnchantment(index);

                enchantedItem.Item.UpdateItemStats();
			}
		}
        private void RemoveEnchantment(Enchantment oldEnchantment) {
			enchantedItem?.Item?.UpdateEnchantment(ref oldEnchantment, true);
            if (Main.mouseLeft) {
                //Only remove enchantedItem from the dynamic effect if it's being picked up by the mouse.  Otherwise, hoveritem needs it for the tooltip.
				foreach (IAddDynamicEffects effect in oldEnchantment.Effects.OfType<IAddDynamicEffects>()) {
					effect.EnchantedItem = null;
				}
			}
		}
		private void ApplyEnchantment(int index) {
			Enchantment enchantment = (Enchantment)_enchantments[index].ModItem;
            if (enchantment == null)
				return;

			enchantedItem?.Item?.UpdateEnchantment(ref enchantment);
			foreach (IAddDynamicEffects effect in enchantment.Effects.OfType<IAddDynamicEffects>()) {
				effect.EnchantedItem = enchantedItem;
			}

			if (enchantment is IStoreAppliedItem storeAppliedItem)
				storeAppliedItem.EnchantedItem = enchantedItem;
		}
        public void ApplyAll() {
			for (int i = 0; i < EnchantingTableUI.MaxEnchantmentSlots; i++) {
                if (_enchantments[i].ModItem is Enchantment enchantment) {
                    ApplyEnchantment(i);
                }
			}
		}
        public EnchantmentsArray Clone(EnchantedItem enchantedItem) {
            EnchantmentsArray enchantmentsArray = new(enchantedItem);
            for (int i = 0; i < _enchantments.Length; i++) {
                enchantmentsArray.AddNoUpdate(_enchantments[i].Clone(), i);
            }

            return enchantmentsArray;
		}
		public void AddNoUpdate(Item item, int index) {
            _enchantments[index] = item;
		}
        public void RemoveNoUpdate(int index) {
			_enchantments[index] = new Item();
		}
        public bool TryReturnAllEnchantments(WEPlayer wePlayer, bool allowQuickspawn = false) {
            for (int i = 0; i < EnchantingTableUI.MaxEnchantmentSlots; i++) {
                if (!wePlayer.TryHandleEnchantmentRemoval(i, this, allowQuickspawn))
                    return false; 
            }

            return true;
        }
        public void ApplyLoadout(Item[] enchantments) {
            for (int i = 0; i < EnchantingTableUI.MaxEnchantmentSlots; i++) {
                Item clone = enchantments[i].Clone();
				this[i] = clone;
            }
        }
	}
	public static class EnchantedItemStaticMethods {
        public static bool IsSameEnchantedType(this Item item, Item otherItem) {
            if (item == null || otherItem == null)
                return false;

            if (item.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem) && otherItem.TryGetEnchantedItemSearchAll(out EnchantedItem otherEnchantedItem))
                return IsSameEnchantedType(enchantedItem, otherEnchantedItem);

            return false;
        }
        public static bool IsSameEnchantedType(this Item item, EnchantedItem otherEnchantedItem) {
            if (item == null)
                return false;

            if (item.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem))
                return IsSameEnchantedType(enchantedItem, otherEnchantedItem);

            return false;
        }
        public static bool IsSameEnchantedType(this EnchantedItem enchantedItem, Item otherItem) {
            if (otherItem == null)
                return false;

            if (otherItem.TryGetEnchantedItemSearchAll(out EnchantedItem otherEnchantedItem))
                return IsSameEnchantedType(enchantedItem, otherEnchantedItem);

            return false;
        }
        public static bool IsSameEnchantedType(this EnchantedItem enchantedItem, EnchantedItem otherEnchantedItem) {
            return enchantedItem.ItemType == otherEnchantedItem.ItemType;
		}
        public static void SetupGlobals(this Item item) {
            //Not EnchantedItem return
            if (!item.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem))
                return;

            //Update Enchantments
            enchantedItem.enchantments.ApplyAll();

            //Get Global Item Stats
            enchantedItem.TryGetInfusionStats(ref item);

            //Update Stats
            item.UpdateItemStats();
        }
		public static void UpdateItemStats(this Item item) {
			if (!item.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem))
				return;

			#region Debug

			if (LogMethods.debugging) ($"\\/UpdateItemStats(" + item.S() + ")").Log();

			#endregion

			//Prefix
			int trackedPrefix = enchantedItem.prefix;
			if (trackedPrefix != item.prefix) {
				enchantedItem.prefix = item.prefix;
				if (enchantedItem is EnchantedWeapon enchantedWeapon && enchantedWeapon.damageType != DamageClass.Default)
					item.DamageType = enchantedWeapon.damageType;
			}

			if (enchantedItem is EnchantedArmor enchantedArmor) {
				int infusedArmorSlot = enchantedArmor.infusedArmorSlot;
				int armorSlot = item.GetInfusionArmorSlot(false, true);
				if (infusedArmorSlot != EnchantedArmor.DefaultInfusionArmorSlot && armorSlot != infusedArmorSlot)
					item.UpdateArmorSlot(enchantedArmor.infusedArmorSlot);
			}

			#region Debug

			if (LogMethods.debugging) ($"/\\UpdateItemStats(" + item.S() + ")").Log();

			#endregion
		}
		public static void UpdateEnchantment(this Item item, ref Enchantment enchantment, bool remove = false) {
			if (!item.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem))
				return;

			if (enchantment != null) {
				//New Damage Type
				foreach (IPermenantStat effect in enchantment.Effects.Where(e => e is IPermenantStat).Select(e => (IPermenantStat)e)) {
					effect.Update(ref item, remove);
				}

				enchantment.ItemTypeAppliedOn = remove ? EItemType.None : enchantedItem.ItemType;
			}

            if (enchantedItem is EnchantedHeldItem)
                Main.LocalPlayer.GetWEPlayer().Equipment.UpdateHeldItemEnchantmentEffects(item);
		}
        private const float DefaultXPMultiplier = 0f;
        public static bool GetXPMultiplierFromNPC(this NPC target, out float experienceMultiplier) {
            experienceMultiplier = DefaultXPMultiplier;
			if (target.IsDummy())
                return false;

            if (target.friendly || target.townNPC)
                return false;

			//value
			float value;
			if (Main.netMode == NetmodeID.MultiplayerClient) {
				value = ContentSamples.NpcsByNetId[target.RealNetID()].value;
			}
			else {
				value = target.RealValue();
			}

			int realLifeMax = target.RealLifeMax();
			if (value <= 0 && (target.SpawnedFromStatue || realLifeMax <= 10))
				return false;

			//NPC Characteristics Factors
			float noGravityFactor = target.noGravity ? 0.2f : 0f;
			float noTileCollideFactor = target.noTileCollide ? 0.2f : 0f;
			float knockBackResistFactor = 0.2f * (1f - target.knockBackResist);
			float npcCharacteristicsFactor = noGravityFactor + noTileCollideFactor + knockBackResistFactor;

			//Config Multiplier
			float configMultiplier = target.boss ? BossXPMultiplier : NormalXPMultiplier;

			float balanceMultiplier = target.boss ? 0.25f : 1f;

			//Experience Multiplier
			experienceMultiplier = (1f + npcCharacteristicsFactor) * configMultiplier * balanceMultiplier;

            return true;
		}
		public static void DamageNPC(this Item item, Player player, NPC target, NPC.HitInfo hit, bool melee = false) {

            #region Debug

            if (LogMethods.debugging) ($"\\/DamageNPC").Log();

            #endregion

            if (!GetXPMultiplierFromNPC(target, out float experienceMultiplier))
                goto debugBeforeReturn;

			//life vs damage check
			int xpDamage = hit.Damage;
            int life = target.RealLife();
            if (life < 0)
                xpDamage += life;

            //XP Damage <= 0 check
            if (xpDamage <= 0)
                goto debugBeforeReturn;

            //Low damage per hit xp boost
            float lowDamagePerHitXPBoost;
            int damageBeforeCirt = hit.Damage / (hit.Crit ? 2 : 1);
            if (item != null && damageBeforeCirt < hit.SourceDamage) {
				lowDamagePerHitXPBoost = (float)hit.SourceDamage / (float)damageBeforeCirt;
                if (hit.Crit) {
					//Apply affective crit multiplier
					float critMultiplier = 1f + (player.GetWeaponCrit(item) % 100) / 100f;
					lowDamagePerHitXPBoost *= critMultiplier;
                }
            }
            else {
                lowDamagePerHitXPBoost = 1f;
            }

            if(lowDamagePerHitXPBoost < 1f) {
				GameMessageTextID.PreventedIssueLooseExperience.ToString().Lang_WE(L_ID1.GameMessages, new object[] { item.S(), target.S(), hit, melee.S(), Main.GameMode, target.defense, xpDamage, lowDamagePerHitXPBoost }).LogNT(ChatMessagesIDs.LowDamagePerHitXPBoost);// ($"Prevented an issue that would cause your xp do be reduced.  (xpInt < 0) item: {item.S()}, target: {target.S()}, hit: {hit}, " + $"melee: {melee.S()}, Main.GameMode: {Main.GameMode},\n" + $"target.defense: {target.defense}, xpDamage: {xpDamage}, lowDamagePerHitXPBoost: {lowDamagePerHitXPBoost}").LogNT_WE(ChatMessagesIDs.LowDamagePerHitXPBoost);
                lowDamagePerHitXPBoost = 1f;
			}

            //Low Damage help multiplier
            float xp = xpDamage * lowDamagePerHitXPBoost * experienceMultiplier;

            //Reduction Factor (Life Max)
            int lifeMax = target.RealLifeMax();
            float reductionFactor = androLib.Common.Globals.NPCStaticMethods.GetReductionFactor(lifeMax);
            xp /= reductionFactor;

            //XP <= 0 check
            int xpInt = (int)Math.Round(xp);
            if (xpInt <= 0) {
                xpInt = 1;
            }
            else if (xpInt < 0) {
                GameMessageTextID.PreventedIssueLooseExperienceTwo.ToString().Lang_WE(L_ID1.GameMessages, new object[] { item.S(), target.S(), hit, melee.S(), Main.GameMode, xpDamage, xpInt, lowDamagePerHitXPBoost }).LogNT(ChatMessagesIDs.DamageNPCPreventLoosingXP2);// ($"Prevented an issue that would cause you to loose experience. (xpInt < 0) item: {item.S()}, target: {target.S()}, hit: {hit}, melee: {melee.S()}, Main.GameMode: {Main.GameMode}, xpDamage: {xpDamage}, xpInt: {xpInt}, lowDamagePerHitXPBoost: {lowDamagePerHitXPBoost}, ").LogNT_WE(ChatMessagesIDs.DamageNPCPreventLoosingXP2);
                xpInt = 1;
            }

            //Gain XP (Item)
            if (item.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem))
                enchantedItem.GainXP(item, xpInt);

            //Gain XP (Armor)
            player.AllArmorGainXp(xpInt);

			#region Debug

            debugBeforeReturn:
			if (LogMethods.debugging) ($"/\\DamageNPC").Log();

            #endregion
        }
        public static void CombineEnchantedItems(this Item item, List<Item> consumedItems, bool fromCraft = false) {
            if (consumedItems.Count <= 0)
                return;

			for (int c = 0; c < consumedItems.Count; c++) {
                Item consumedItem = consumedItems[c];
                if (consumedItem.NullOrAir())
                    continue;

				if (!consumedItem.TryGetEnchantedItemSearchAll(out EnchantedItem consumedEnchantedItem))
                    continue;

                bool consumedModified = consumedEnchantedItem.Modified;
                if (!consumedModified && (!fromCraft || Main.mouseItem.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedMouseItem) && !enchantedMouseItem.Modified))
                    continue;

				if (fromCraft && consumedItem.maxStack > 1) {
					if (androLib.ModIntegration.MagicStorageIntegration.MagicStorageEnabledAndOpen) {
						if (MagicStorageIntegration.JustCraftedStackableItem) {
							MagicStorageIntegration.JustCraftedStackableItem = false;
						}
						else {
                            consumedEnchantedItem.ResetGlobals(consumedItem);
						}
					}
                    else {
                        if (!consumedModified) {
							//Fix for Crafting with a modified enchanted weapon already as the mouse item.
							//if (Main.HoverItem.type == Main.recipe[Main.focusRecipe].createItem.type)//If not tested
								consumedItem = Main.mouseItem;
						}
                        else {
							bool found = consumedItem.TryResetSameEnchantedItem(Main.LocalPlayer.inventory, out _);
							if (!found && Main.LocalPlayer.chest > -1) {
								found = consumedItem.TryResetSameEnchantedItem(Main.chest[Main.LocalPlayer.chest].item, out int index);
								if (Main.netMode == NetmodeID.MultiplayerClient && found)
									Net<INetMethods>.Proxy.NetResetEnchantedItemInChest(Main.LocalPlayer.chest, (short)index);
							}
						}
					}
				}

				if (item.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem)) {
                    item.CheckConvertExcessExperience(consumedItem);
                    if (enchantedItem is EnchantedWeapon enchantedWeapon && consumedEnchantedItem is EnchantedWeapon consumedEnchantedWeapon) {
                        int weaponInfusionPower = enchantedWeapon.GetInfusionPower(ref item);
                        int consumedWeaponInfusionPower = consumedEnchantedWeapon.GetInfusionPower(ref consumedItem);
						if (weaponInfusionPower < consumedWeaponInfusionPower) {
							item.TryInfuseItem(consumedItem);
							item.TryInfuseItem(consumedItem, false, true);
						}
					}

                    if (consumedEnchantedItem.PowerBoosterInstalled) {
                        if (!enchantedItem.PowerBoosterInstalled) {
                            enchantedItem.PowerBoosterInstalled = true;
                        }
						else {
                            Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), ModContent.ItemType<PowerBooster>(), 1);
                        }
                    }

                    if (consumedEnchantedItem.UltraPowerBoosterInstalled) {
                        if (!enchantedItem.UltraPowerBoosterInstalled) {
                            enchantedItem.UltraPowerBoosterInstalled = true;
                        }
						else {
                            Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), ModContent.ItemType<UltraPowerBooster>(), 1);
                        }
                    }

                    int j;
                    for (j = 0; j < EnchantingTableUI.MaxEnchantmentSlots; j++) {
                        if (enchantedItem.enchantments[j].IsAir)
                            break;
                    }
                    for (int k = 0; k < EnchantingTableUI.MaxEnchantmentSlots; k++) {
                        if (!consumedEnchantedItem.enchantments[k].IsAir) {
                            Enchantment enchantment = ((Enchantment)consumedEnchantedItem.enchantments[k].ModItem);
                            int uniqueItemSlot = EnchantingTableUI.FindSwapEnchantmentSlot(enchantment, item);
                            bool cantFit = false;
                            if (!EnchantingTableUI.UseEnchantmentSlot(item, j, true))
                                cantFit = true;

                            if (!cantFit && !EnchantingTableUI.EnchantmentAllowedOnItem(item, enchantment))
                                cantFit = true;

                            if (!cantFit && enchantment.GetCapacityCost() <= enchantedItem.GetLevelsAvailable()) {
                                if (uniqueItemSlot == -1) {
                                    if ((RemoveEnchantmentRestrictions || enchantment.Utility) && enchantedItem.enchantments[4].IsAir && EnchantingTableUI.SlotAllowedByConfig(enchantedItem.ItemType, 4)) {
										enchantedItem.enchantments[4] = consumedEnchantedItem.enchantments[k].Clone();
                                    }
                                    else if (j < 4) {
                                        enchantedItem.enchantments[j] = consumedEnchantedItem.enchantments[k].Clone();
                                        j++;
                                    }
                                    else {
                                        cantFit = true;
                                    }
                                }
                                else {
                                    cantFit = true;
                                }
                            }
                            else {
                                cantFit = true;
                            }

                            if (cantFit)
                                WEPlayer.LocalWEPlayer.TryHandleEnchantmentRemoval(k, consumedEnchantedItem.enchantments, true);
                        }

                        consumedEnchantedItem.enchantments[k] = new Item();
                    }
                }
                else {
                    int numberEssenceRecieved;
                    int xpCounter = enchantedItem.Experience;
                    for (int tier = EnchantingTableUI.MaxEssenceSlots - 1; tier >= 0; tier--) {
                        numberEssenceRecieved = xpCounter / (int)EnchantmentEssenceBasic.xpPerEssence[tier] * 4 / 5;
                        xpCounter -= (int)EnchantmentEssenceBasic.xpPerEssence[tier] * numberEssenceRecieved;
                        if (xpCounter < (int)EnchantmentEssenceBasic.xpPerEssence[0] && xpCounter > 0 && tier == 0) {
                            xpCounter = 0;
                            numberEssenceRecieved += 1;
                        }

                        Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), EnchantmentEssenceBasic.IDs[tier], numberEssenceRecieved);
                    }

                    if (consumedEnchantedItem.PowerBoosterInstalled)
                        Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), ModContent.ItemType<PowerBooster>(), 1);

                    if (consumedEnchantedItem.UltraPowerBoosterInstalled)
                        Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), ModContent.ItemType<UltraPowerBooster>(), 1);

                    for (int k = 0; k < EnchantingTableUI.MaxEnchantmentSlots; k++) {
                        if (!consumedEnchantedItem.enchantments[k].IsAir) {
                            Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), consumedEnchantedItem.enchantments[k].type, 1);
                        }
                    }
                }
            }
        }
        public static void CheckRemoveEnchantments(this Item item, Player player) {
            if (!item.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem) || RemoveEnchantmentRestrictions)
                return;

            //Check config
            for (int i = EnchantingTableUI.MaxEnchantmentSlots - 1; i >= 0 ; i--) {
                Item enchantment = enchantedItem.enchantments[i];
                if (enchantment.IsAir)
                    continue;

                bool slotAllowedByConfig = EnchantingTableUI.SlotAllowedByConfig(enchantedItem.ItemType, i);
                if (!slotAllowedByConfig)
                    RemoveEnchantmentNoUpdate(item, enchantedItem, i, player, GameMessageTextID.SlotNumDisabledByConfig.ToString().Lang_WE(L_ID1.GameMessages, new object[] { i, enchantment.Name, item.Name }));// $"Slot {i} disabled by config.  Removed {enchantment.Name} from your {item.Name}.")
            }

            //Check enchantment limitations
            List<string> enchantmentTypeNames = new List<string>();
            bool unique = false;
            for (int i = 0; i < EnchantingTableUI.MaxEnchantmentSlots; i++) {
                Item enchantmentItem = enchantedItem.enchantments[i];

                if (enchantmentItem != null && !enchantmentItem.IsAir && player != null) {
                    ModItem modItem = enchantmentItem.ModItem;
                    if (modItem is UnloadedItem unloadedItem) {
                        RemoveEnchantmentNoUpdate(item, enchantedItem, i, player, GameMessageTextID.RemovedUnloadedEnchantmentFromItem.ToString().Lang_WE(L_ID1.GameMessages, new object[] { unloadedItem.ItemName, item.S() }));// $"Removed Unloaded Item:{unloadedItem.ItemName} from your {item.S()}.  Please inform andro951(WeaponEnchantments).");
                        continue;
                    }

                    if (modItem is not Enchantment enchantment) {
                        RemoveEnchantmentNoUpdate(item, enchantedItem, i, player, GameMessageTextID.DetectedNonEnchantmentInEnchantmentSlot.ToString().Lang_WE(L_ID1.GameMessages, new object[] { enchantmentItem.S(), item.S() }));// $"Detected a non-enchantment item:{enchantmentItem.S()} on your {item.S()}.  It has been returned to your inventory.");
                        continue;
                    }

                    if (item.IsWeaponItem() && !enchantment.AllowedList.ContainsKey(EItemType.Weapons)) {
                        RemoveEnchantmentNoUpdate(item, enchantedItem, i, player, GameMessageTextID.EnchantmentNoLongerAllowed.ToString().Lang_WE(L_ID1.GameMessages, new object[] { enchantmentItem.S(), EItemType.Weapons.ToString().Lang_WE(L_ID1.Tooltip, L_ID2.ItemType), item.S() })); //enchantmentItem.Name + " is no longer allowed on weapons and has been removed from your " + item.Name + ".");
                    }
                    else if (item.IsArmorItem() && !enchantment.AllowedList.ContainsKey(EItemType.Armor)) {
                        RemoveEnchantmentNoUpdate(item, enchantedItem, i, player, GameMessageTextID.EnchantmentNoLongerAllowed.ToString().Lang_WE(L_ID1.GameMessages, new object[] { enchantmentItem.S(), EItemType.Armor.ToString().Lang_WE(L_ID1.Tooltip, L_ID2.ItemType), item.S() })); //enchantmentItem.Name + " is no longer allowed on armor and has been removed from your " + item.Name + ".");
					}
                    else if (item.IsAccessoryItem() && !enchantment.AllowedList.ContainsKey(EItemType.Accessories)) {
                        RemoveEnchantmentNoUpdate(item, enchantedItem, i, player, GameMessageTextID.EnchantmentNoLongerAllowed.ToString().Lang_WE(L_ID1.GameMessages, new object[] { enchantmentItem.S(), EItemType.Accessories.ToString().Lang_WE(L_ID1.Tooltip, L_ID2.ItemType), item.S() })); //enchantmentItem.Name + " is no longer allowed on accessories and has been removed from your " + item.Name + ".");
					}

                    if (i == EnchantingTableUI.MaxEnchantmentSlots - 1 && !enchantment.Utility)
                        RemoveEnchantmentNoUpdate(item, enchantedItem, i, player, GameMessageTextID.NoLongerUtilityEnchantment.ToString().Lang_WE(L_ID1.GameMessages, new object[] { enchantmentItem.S(), item.S() })); //enchantmentItem.Name + " is no longer a utility enchantment and has been removed from your " + item.Name + ".");

					if (enchantment.IsClassRestricted(ContentSamples.ItemsByType[item.type].DamageType.Type))
                        RemoveEnchantmentNoUpdate(item, enchantedItem, i, player, GameMessageTextID.NoLongerAllowedOnDamageType.ToString().Lang_WE(L_ID1.GameMessages, new object[] { enchantmentItem.S(), item.DamageType.Name, item.S() })); //enchantmentItem.Name + $" is no longer allowed on {item.DamageType.Name} weapons and has removed from your " + item.Name + ".");

					if (enchantment.Max1 && enchantmentTypeNames.Contains(enchantment.EnchantmentTypeName))
                        RemoveEnchantmentNoUpdate(item, enchantedItem, i, player, GameMessageTextID.NowLimitedToOne.ToString().Lang_WE(L_ID1.GameMessages, new object[] { enchantment.EnchantmentTypeName, enchantmentItem.S(), item.S() })); //enchantment.EnchantmentTypeName + $" Enchantments are now limmited to 1 per item.  {enchantmentItem.Name} has been removed from your " + item.Name + ".");

					if (enchantment.Unique) {
                        if (unique) {
                            RemoveEnchantmentNoUpdate(item, enchantedItem, i, player, GameMessageTextID.MultipleUniqueEnchantments.ToString().Lang_WE(L_ID1.GameMessages, new object[] { item.S(), enchantmentItem.S(), item.S() })); //enchantment.EnchantmentTypeName + $" Detected multiple uniques on your {item.Name}.  {enchantmentItem.Name} has been removed from your " + item.Name + ".");
						}
                        else {
                            unique = true;
                        }
                    }

                    enchantmentTypeNames.Add(enchantment.EnchantmentTypeName);
                }
            }

            //Check too many enchantments on item
            if (enchantedItem.GetLevelsAvailable() < 0) {
                for (int k = EnchantingTableUI.MaxEnchantmentSlots - 1; k >= 0 && enchantedItem.GetLevelsAvailable() < 0; k--) {
                    WEPlayer.LocalWEPlayer.TryHandleEnchantmentRemoval(k, enchantedItem.enchantments, true);
                }

                Main.NewText(GameMessageTextID.ItemTooLowLevel.ToString().Lang_WE(L_ID1.GameMessages, new object[] { item.Name }));
			}
        }
		private static void RemoveEnchantmentNoUpdate(Item item, EnchantedItem enchantedItem, int i, Player player, string msg) {
			Item enchantmentItem = enchantedItem.enchantments[i];
			enchantmentItem = player.GetItem(player.whoAmI, enchantmentItem, GetItemSettings.LootAllSettings);
			if (!enchantmentItem.IsAir)
				player.QuickSpawnItem(player.GetSource_Misc("PlayerDropItemCheck"), enchantmentItem);

			enchantedItem.enchantments.RemoveNoUpdate(i);
			if (Main.mouseItem?.IsSameEnchantedItem(item) != null && Main.mouseItem.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem1)) {
				enchantedItem1.enchantments.RemoveNoUpdate(i);
			}

			msg.LogSimpleNT();
		}
		public static bool IsSameEnchantedItem(this Item item1, Item item2) {
            bool isEnchantedItem1 = item1.TryGetEnchantedItemSearchAll(out EnchantedItem global1);
            bool isEnchantedItem2 = item2.TryGetEnchantedItemSearchAll(out EnchantedItem global2);
            if (!isEnchantedItem1)
                return !isEnchantedItem2;

            if (item1.type != item2.type || item1.prefix != item2.prefix)
                return false;

            if (Math.Abs(global1.levelBeforeBooster - global2.levelBeforeBooster) > 1)
                return false;

            if (global1.PowerBoosterInstalled != global2.PowerBoosterInstalled || global1.UltraPowerBoosterInstalled != global2.UltraPowerBoosterInstalled || global1.infusedItemName != global2.infusedItemName)
                return false;

            for (int i = 0; i < EnchantingTableUI.MaxEnchantmentSlots; i++) {
                if (global1.enchantments[i].type != global2.enchantments[i].type)
                    return false;
            }

            return true;
        }
        public static bool TryResetSameEnchantedItem(this Item item, IEnumerable<Item> storageItems, out int index) {
            bool found = false;
            index = -1;
            int i = 0;
			foreach (Item storageItem in storageItems) {
				if (storageItem.IsSameEnchantedItem(item) && storageItem.TryGetEnchantedItemSearchAll(out EnchantedItem storageEnchantedItem)) {
					storageEnchantedItem.ResetGlobals(storageItem);
                    found = true;
                    index = i;
				}

                i++;
			}

            return found;
		}
        public static void ResetEnchantedItemInChestFromNet(int chestNum, short index) {
            if (Main.netMode != NetmodeID.Server)
                return;

            if (Main.chest[chestNum].item[index].TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem))
                enchantedItem.ResetGlobals(Main.chest[chestNum].item[index]);
        }
	}
}
