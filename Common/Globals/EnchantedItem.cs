using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using WeaponEnchantments.Items;
using WeaponEnchantments.UI;

namespace WeaponEnchantments.Common.Globals
{
	public class EnchantedItem : GlobalItem
    {
		#region Constants

		public const int MAX_LEVEL = 40;

		#endregion

		#region Reforge (static)

		public static Item reforgeItem = null;
        public static int newPrefix = 0;
        public static bool cloneReforgedItem = false;

        #endregion

        #region Tracking (static)

        public static bool resetGlobals = false;

        #endregion

        #region Properties (static)

        public static float LinearMultiplier => WEMod.serverConfig.presetData.linearStrengthMultiplier / 100f;
        public static float RecomendedMultiplier => WEMod.serverConfig.presetData.recomendedStrengthMultiplier / 100f;

        #endregion


        #region Stats

        public Dictionary<string, StatModifier> statModifiers = new Dictionary<string, StatModifier>();
        public Dictionary<string, StatModifier> appliedStatModifiers = new Dictionary<string, StatModifier>();
        public Dictionary<string, StatModifier> eStats = new Dictionary<string, StatModifier>();
        public Dictionary<string, StatModifier> appliedEStats = new Dictionary<string, StatModifier>();
        public Dictionary<int, int> buffs = new Dictionary<int, int>();
        public Dictionary<int, int> debuffs = new Dictionary<int, int>();
        public Dictionary<int, int> onHitBuffs = new Dictionary<int, int>();

        #endregion

        #region Enchantment

        public Item[] enchantments = new Item[EnchantingTable.maxEnchantments]; 
        public int damageType = -1;
        public int baseDamageType = -1;

        #endregion

        #region Infusion

        public string infusedItemName = "";
        public int infusionPower = 0;
        public float damageMultiplier = 1f;
        public int infusedArmorSlot = -1;
        public int infusionValueAdded = 0;

        #endregion

        #region Experience

        public int experience = 0;
        public int levelBeforeBooster = 0;
        public bool powerBoosterInstalled = false;
        public int level = 0;
        public int lastValueBonus = 0;

        #endregion

        #region Reforge (instance)

        public int prefix = 0;
        public bool normalReforge = false;

        #endregion

        #region Minion weapons

        public Projectile masterProjectile = null;

        #endregion

        #region Tracking (instance)

        public bool inEnchantingTable = false;
        public bool equippedInArmorSlot = false;
        public bool trackedWeapon = false;
        public bool hoverItem = false;
        public bool trashItem = false;
        public bool favorited = false;
        public bool stack0 = false;
        public bool needsUpdateOldItems = false;

        #endregion

        #region Properties (instance)

        public bool Enchanted => experience != 0 || powerBoosterInstalled;
        public bool Modified => experience != 0 || powerBoosterInstalled || infusedItemName != "";

        #endregion

        public EnchantedItem() {
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++) {
                enchantments[i] = new Item();
            }
        }
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return WEMod.IsEnchantable(entity);
        }
        public override GlobalItem Clone(Item item, Item itemClone) {
            EnchantedItem clone;
            if (cloneReforgedItem || resetGlobals) {
                if(itemClone.TG(out EnchantedItem iGlobal)) {
                    clone = iGlobal;
                }
				else {
                    Main.NewText($"In EnchantedItem, Failed to Clone(item: {item.S()}, itemClone: {itemClone.S()}), cloneReforgedItem: {cloneReforgedItem.S()}, resetGlobals: {resetGlobals.S()}." + UtilityMethods.reporteMessage);
                    return this;
				}

				#region Stats

				clone.statModifiers = new Dictionary<string, StatModifier>(statModifiers);
                clone.appliedStatModifiers = new Dictionary<string, StatModifier>();
                clone.eStats = new Dictionary<string, StatModifier>(eStats);
                clone.appliedEStats = new Dictionary<string, StatModifier>();
                clone.buffs = new Dictionary<int, int>(buffs);
                clone.debuffs = new Dictionary<int, int>(debuffs);
                clone.onHitBuffs = new Dictionary<int, int>(onHitBuffs);

				#endregion

				#region Enchantments

				if (resetGlobals) {
                    for (int i = 0; i < enchantments.Length; i++)
                        clone.enchantments[i] = new Item();
                }
                else if (cloneReforgedItem) {
                    for (int i = 0; i < enchantments.Length; i++)
                        clone.enchantments[i] = enchantments[i];
                }
                
                clone.damageType = damageType;
                clone.baseDamageType = baseDamageType;

                #endregion

                #region Infusion

                clone.infusedItemName = infusedItemName;
                clone.infusionPower = infusionPower;
                clone.damageMultiplier = damageMultiplier;
                clone.infusionValueAdded = infusionValueAdded;

                #endregion

                #region Experience

                clone.experience = experience;
                clone.levelBeforeBooster = levelBeforeBooster;
                clone.powerBoosterInstalled = powerBoosterInstalled;
                clone.level = level;
				if (resetGlobals) {
                    clone.lastValueBonus = lastValueBonus;
                }
                else if (cloneReforgedItem) {
                    clone.lastValueBonus = 0;
                }

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
                //clone.trackedWeapon = trackedWeapon;
                //clone.hoverItem = hoverItem;
                //clone.trashItem = trashItem;
                clone.favorited = favorited;
                clone.stack0 = stack0;

                #endregion

                itemClone.G().UpdateItemValue(itemClone);
            }
            else {
                clone = (EnchantedItem)base.Clone(item, itemClone);
            }

            clone.equippedInArmorSlot = false;

            if(!Main.mouseItem.IsSameEnchantedItem(itemClone))
                clone.trackedWeapon = false;

            return clone;
        }
        public override void LoadData(Item item, TagCompound tag) {

			#region Debug

			if (UtilityMethods.debugging) ($"\\/LoadData(" + item.Name + ")").Log();

			#endregion

			#region Enchantment

			for (int i = 0; i < EnchantingTable.maxEnchantments; i++) {
                if (tag.Get<Item>("enchantments" + i.ToString()) != null) {
                    enchantments[i] = tag.Get<Item>("enchantments" + i.ToString()).Clone();
                    OldItemManager.ReplaceOldItem(ref enchantments[i]);
                }
            }

            #endregion

            #region Experience

            experience = tag.Get<int>("experience");
            powerBoosterInstalled = tag.Get<bool>("powerBooster");

            #endregion

            #region Infusion

            infusedItemName = tag.Get<string>("infusedItemName");
            infusionPower = tag.Get<int>("infusedPower");

            #endregion

            #region Tracking (instance)

            stack0 = tag.Get<bool>("stack0");

            #endregion

            if (experience < 0)
                experience = int.MaxValue;

            #region Debug

            if (UtilityMethods.debugging) ($"/\\LoadData(" + item.Name + ")").Log();

			#endregion
		}
		public override void SaveData(Item item, TagCompound tag) {

			#region Enchantment

			for (int i = 0; i < EnchantingTable.maxEnchantments; i++) {
                if (!enchantments[i].IsAir) {
                    tag["enchantments" + i.ToString()] = enchantments[i].Clone();
                }
            }

			#endregion

			#region Experience

			tag["experience"] = experience;
            tag["powerBooster"] = powerBoosterInstalled;

			#endregion

			#region Infusion

			tag["infusedItemName"] = infusedItemName;
            tag["infusedPower"] = infusionPower;

            #endregion

            #region Tracking (instance)

            tag["stack0"] = stack0;

			#endregion
		}
		public override void NetSend(Item item, BinaryWriter writer) {

            #region Debug

            if (UtilityMethods.debugging) {
                ($"\\/NetSend(" + item.Name + ")").Log();
                ($"eStats.Count: " + eStats.Count + ", statModifiers.Count: " + statModifiers.Count).Log();
            }

            #endregion

            writer.Write(Modified);

			if (!Modified)
                return;

            #region Enchantment

            for (int i = 0; i < EnchantingTable.maxEnchantments; i++) {
                writer.Write((short)enchantments[i].type);

                #region Debug

                if (UtilityMethods.debugging) ($"e " + i + ": " + enchantments[i].Name).Log();

				#endregion
			}

			#endregion

			#region Experience

			writer.Write(experience);
            writer.Write(powerBoosterInstalled);

            #endregion

            #region Infusion

            bool noName = infusedItemName == "";
            writer.Write(noName);
            if (!noName) {
                writer.Write(infusedItemName);
                writer.Write(infusionPower);
            }

            #endregion

            #region Tracking (instance)

            writer.Write(stack0);

			#endregion

            #region Debug

            if (UtilityMethods.debugging) {
                ($"eStats.Count: " + eStats.Count + ", statModifiers.Count: " + statModifiers.Count).Log();
                ($"/\\NetSend(" + item.Name + ")").Log();
            }

			#endregion
		}
		public override void NetReceive(Item item, BinaryReader reader) {

			#region Debug

			if (UtilityMethods.debugging) {
                ($"\\/NetRecieve(" + item.Name + ")").Log();
                ($"eStats.Count: " + eStats.Count + ", statModifiers.Count: " + statModifiers.Count).Log();
            }

            #endregion

            bool dataExistsInReader = reader.ReadBoolean();

            if (!dataExistsInReader)
                return;

            #region Enchantment

            for (int i = 0; i < EnchantingTable.maxEnchantments; i++) {
                enchantments[i] = new Item(reader.ReadUInt16());

				#region Debug

				if (UtilityMethods.debugging) ($"e " + i + ": " + enchantments[i].Name).Log();

				#endregion
			}

			#endregion

			#region Experience

			experience = reader.ReadInt32();
            powerBoosterInstalled = reader.ReadBoolean();

            #endregion

            #region Infusion

            bool noName = reader.ReadBoolean();
            if (!noName) {
                infusedItemName = reader.ReadString();
                infusionPower = reader.ReadInt32();
            }

            #endregion

            #region Tracking (instance)

            stack0 = reader.ReadBoolean();

            #endregion

            item.SetupGlobals();

			#region Debug

			if (UtilityMethods.debugging) {
                ($"eStats.Count: " + eStats.Count + ", statModifiers.Count: " + statModifiers.Count).Log();
                ($"/\\NetRecieve(" + item.Name + ")").Log();
            }

			#endregion
		}
		public override void UpdateInventory(Item item, Player player) {
            if (Modified) {
                //Update Value
                UpdateItemValue(item);

                //Stars Above compatibility fix
                if (baseDamageType != damageType) {
                    if (baseDamageType == -1)
                        baseDamageType = ContentSamples.ItemsByType[item.type].DamageType.Type;

                    if (item.DamageType.Type == baseDamageType)
                        item.UpdateDamageType(damageType);
                }
            }

            equippedInArmorSlot = false;

            //Track favorited
            if (item.favorited) {
                if (!favorited && WEModSystem.AltDown) {
                    favorited = true;
                }
            }
            else {
                if (favorited) {
                    if (!WEModSystem.AltDown) {
                        item.favorited = true;
                    }
                    else {
                        favorited = false;
                    }
                }
            }
        }
        public override void UpdateEquip(Item item, Player player) {
            if (!inEnchantingTable)
                return;

            //Fix for swapping an equipped armor/accessory with one in the enchanting table.
            if (player.G().I().TG()) {
                inEnchantingTable = false;
                if (item.GetInfusionArmorSlot() != infusedArmorSlot) {
                    infusedArmorSlot = -1;
                    item.TryInfuseItem(new Item(), true);
                }
            }
        }
        public override bool OnPickup(Item item, Player player) {
            player.G().UpdateItemStats(ref item);

            return true;
        }
        public void UpdateItemValue(Item item) {
            int enchantmentsValue = 0;
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++) {
                enchantmentsValue += enchantments[i].value;
            }
            int powerBoosterValue = powerBoosterInstalled ? ModContent.GetModItem(ModContent.ItemType<PowerBooster>()).Item.value : 0;
            int valueToAdd = enchantmentsValue + (int)(EnchantmentEssenceBasic.valuePerXP * experience) + powerBoosterValue + infusionValueAdded;
            valueToAdd /= item.stack;
            item.value += valueToAdd - lastValueBonus;
            lastValueBonus = valueToAdd;
        }
        public void UpdateLevel() {
            int l;
            for (l = 0; l < MAX_LEVEL; l++) {
                if (experience < WEModSystem.levelXps[l]) {
                    level = l + 1;
                    break;
                }
            }

            if (l == MAX_LEVEL) {
                levelBeforeBooster = MAX_LEVEL;
            }
            else {
                levelBeforeBooster = l;
            }

            level = levelBeforeBooster;

            if (powerBoosterInstalled)
                level += 10;
        }
        public int GetLevelsAvailable() {
            UpdateLevel();

            int totalEnchantmentLevelCost = 0;
            for (int i = 0; i < enchantments.Length; i++) {
                if (enchantments[i] != null && !enchantments[i].IsAir) {
                    AllForOneEnchantmentBasic enchantment = (AllForOneEnchantmentBasic)enchantments[i].ModItem;
                    totalEnchantmentLevelCost += enchantment.GetLevelCost();
                }
            }

            return level - totalEnchantmentLevelCost;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
            WEPlayer wePlayer = Main.LocalPlayer.G();
            bool enchantmentsToolTipAdded = false;

            UpdateLevel();

            //Common Tooltips
            if (Enchanted || inEnchantingTable) {
                if (stack0) {
                    string tooltip = $"!!!OUT OF AMMO!!!";
                    tooltips.Add(new TooltipLine(Mod, "stack0", tooltip) { OverrideColor = Color.Yellow });
                }

                string pointsName = WEMod.clientConfig.UsePointsAsTooltip ? "Points" : "Enchantment Capacity";

                if (powerBoosterInstalled) {
                    string tooltip = $"Level: {levelBeforeBooster}  {pointsName} available: {GetLevelsAvailable()} (Booster Installed)";
                    tooltips.Add(new TooltipLine(Mod, "level", tooltip) { OverrideColor = Color.LightGreen });
                }
				else {
                    string tooltip = $"Level: {levelBeforeBooster}  {pointsName} available: {GetLevelsAvailable()}";
                    tooltips.Add(new TooltipLine(Mod, "level", tooltip) { OverrideColor = Color.LightGreen });
                }

                string levelString;
                if(levelBeforeBooster < MAX_LEVEL) {
                    levelString = $" ({WEModSystem.levelXps[levelBeforeBooster] - experience} to next level)";
                }
				else {
                    levelString = " (Max Level)";
                }
                string levelTooltip = $"Experience: {experience}{levelString}";
                tooltips.Add(new TooltipLine(Mod, "experience", levelTooltip) { OverrideColor = Color.White });
            }

            //infusionTooltip
            if (infusedItemName != "") {
                string tooltip = "";
                if (WEMod.IsWeaponItem(item)) {
                    tooltip = $"Infusion Power: {infusionPower}   Infused Item: {infusedItemName}";
                } 
                else if (WEMod.IsArmorItem(item)) {
                    tooltip = $"Infused Armor ID: {item.GetInfusionArmorSlot()}   Infused Item: {infusedItemName}";
                }

                if (tooltip != "")
                    tooltips.Add(new TooltipLine(Mod, "infusionTooltip", tooltip) { OverrideColor = Color.DarkRed });
            }
            else if (wePlayer.usingEnchantingTable || WEMod.clientConfig.AlwaysDisplayInfusionPower) {
                string tooltip = "";
                if (WEMod.IsWeaponItem(item)) {
                    tooltip = $"Infusion Power: {item.GetWeaponInfusionPower()}";
                }
                else if (WEMod.IsArmorItem(item)) {
                    tooltip = $"Set Bonus ID: {item.GetInfusionArmorSlot(true)}";
                }

                if (tooltip != "")
                    tooltips.Add(new TooltipLine(Mod, "infusionTooltip", tooltip) { OverrideColor = Color.DarkRed });
            }

            //newInfusionTooltip
            if (inEnchantingTable && wePlayer.infusionConsumeItem != null) {
                if(WEMod.IsWeaponItem(item) && WEMod.IsWeaponItem(wePlayer.infusionConsumeItem)) {
                    string tooltip = $"*New Infusion Power: {wePlayer.infusionConsumeItem.GetWeaponInfusionPower()}   " +
						             $"New Infused Item: {wePlayer.infusionConsumeItem.GetInfusionItemName()}*";

                    tooltips.Add(new TooltipLine(Mod, "newInfusionTooltip", tooltip) { OverrideColor = Color.DarkRed });
                }
                else if (WEMod.IsArmorItem(item) && WEMod.IsArmorItem(wePlayer.infusionConsumeItem)) {
                    string tooltip = $"*New Set Bonus ID: {wePlayer.infusionConsumeItem.GetInfusionArmorSlot()}   " +
						             $"New Infused Item: {wePlayer.infusionConsumeItem.GetInfusionItemName()}*";

                    tooltips.Add(new TooltipLine(Mod, "newInfusionTooltip", tooltip) { OverrideColor = Color.DarkRed });
                }
            }

            //Enchantment Stat tooltips
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++) {
                if (!enchantments[i].IsAir) {
                    AllForOneEnchantmentBasic enchantment = (AllForOneEnchantmentBasic)enchantments[i].ModItem;

                    if (!enchantmentsToolTipAdded) {
                        tooltips.Add(new TooltipLine(Mod, "enchantmentsToolTip", "Enchantments:") { OverrideColor = Color.Violet });
                        enchantmentsToolTipAdded = true;
                    }//Enchantmenst: tooltip

                    string itemType = "";
                    if (WEMod.IsWeaponItem(item)) {
                        itemType = "Weapon";
                    }
                    else if (WEMod.IsArmorItem(item)) {
                        itemType = "Armor";
                    }
                    else if (WEMod.IsAccessoryItem(item)) {
                        itemType = "Accessory";
                    }

                    string tooltip = enchantment.AllowedListTooltips[itemType]; 
                    Color color = AllForOneEnchantmentBasic.rarityColors[enchantment.EnchantmentSize];
                    tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), tooltip) { OverrideColor = color });
                }
            }
        }
		public override void ModifyWeaponCrit(Item item, Player player, ref float crit) {
			if (!WEMod.serverConfig.CritPerLevelDisabled) {
                float multiplier;
                float linearMultiplier = LinearMultiplier;

                if (linearMultiplier != 1f) {
                    multiplier = linearMultiplier;
                }
				else {
                    multiplier = RecomendedMultiplier;
                }
                
                crit += levelBeforeBooster * multiplier;
            }
		}
        public void GainXP(Item item, int xpInt)
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();

            int currentLevel = levelBeforeBooster;

            //xp < 0 return
            if(xpInt < 0) {
                $"Prevented your {item.S()} from loosing experience due to a calculation error.".LogNT();

                return;
            }

            //Add xp
            experience += xpInt;
            
            //Check int overflow
            if(experience < 0)
                experience = int.MaxValue;

            //Already max level return
            if (levelBeforeBooster >= MAX_LEVEL)
                return;

            UpdateLevel();

            //Level up message
            if (levelBeforeBooster > currentLevel && wePlayer.usingEnchantingTable) {
                if(levelBeforeBooster >= MAX_LEVEL) {
                    SoundEngine.PlaySound(SoundID.Unlock);
                    Main.NewText($"Congratulations!  {wePlayer.Player.name}'s {item.Name} reached the maximum level, " +
						$"{levelBeforeBooster} ({WEModSystem.levelXps[levelBeforeBooster - 1]} xp).");
                }
                else {
                    Main.NewText($"{wePlayer.Player.name}'s {item.Name} reached level {levelBeforeBooster} ({WEModSystem.levelXps[levelBeforeBooster - 1]} xp).");
                }
            }
        }
        public override bool CanConsumeAmmo(Item weapon, Item ammo, Player player) {
            float rand = Main.rand.NextFloat();
            float ammoSaveChance = -1f * weapon.AEI("AmmoCost", 0f);

            return rand >= ammoSaveChance;
        }
		public override bool? CanAutoReuseItem(Item item, Player player) {
            if (statModifiers.ContainsKey("P_autoReuse")) {
                return false;
            }
			else {
                return null;
            }
		}
        private void Restock(Item item) {
            Player player = Main.LocalPlayer;

            //Find same item
            for (int i = 0; i < 59; i++) {
                Item inventoryItem = player.inventory[i];
                if (inventoryItem.type != item.type)
                    continue;

                if (!inventoryItem.TG(out EnchantedItem invGlobal))
                    continue;

                if (invGlobal.Modified)
                    continue;

                if (invGlobal.stack0)
                    continue;

                //Restock (found same item)
                item.stack = inventoryItem.stack + 1;
                stack0 = false;
                player.inventory[i] = new Item();
                return;
            }
        }
		public override bool? UseItem(Item item, Player player) {
            WEPlayer wePlayer = player.GetModPlayer<WEPlayer>();

            //CatastrophicRelease
            if (eStats.ContainsKey("CatastrophicRelease")) {
                player.statMana = 0;
            }

            //AllForOne use cooldown
            if (eStats.ContainsKey("AllForOne")) {
                int timer = (int)((float)item.useTime * item.AEI("NPCHitCooldown", 0.5f));
                wePlayer.allForOneTimer = timer;
            }

            //stack0
            if(item.consumable && item.stack < 2 && Modified && item.placeStyle == 0) {
                Restock(item);

                if(item.stack < 2) {
                    item.stack = 2;
                    stack0 = true;
                }
			}

            return null;
        }
        public override bool CanUseItem(Item item, Player player) {
			
            WEPlayer wePlayer = player.GetModPlayer<WEPlayer>();

            //stack0
            if (stack0) {
                Restock(item);

				if (item.stack > 1) {
					item.stack--;
                    stack0 = false;

					return true;
				}
				else {
                    return false;
                }
            }

            //CatastrophicRelease
            if (eStats.ContainsKey("CatastrophicRelease") && player.statManaMax != player.statMana)
                return false;

            //Prevent using items when hoving over enchanting table ui
            if (wePlayer.usingEnchantingTable && WeaponEnchantmentUI.preventItemUse)
                return false;

            //AllForOne
            if (eStats.ContainsKey("AllForOne")) {
                return wePlayer.allForOneTimer <= 0;
            }

            return true;
        }
        public override bool CanRightClick(Item item)
        {
            if (item.stack <= 1)
                return false;


            if (item.stack > 1) {
                if (Modified) {
                    if (Main.mouseItem.IsAir) {
                        return true;
                    }
                }
            }//Prevent splitting stack of enchantable items with maxstack > 1
            return false;
        }
        public override void RightClick(Item item, Player player)
        {
            if (item.stack > 1) {
                if (Modified) {
                    if (item.Name == "Primary Zenith")
                        return;
                    if (Main.mouseItem.IsAir) {
                        RightClickStackableItem(item, player);
                    }
                }
            }//Prevent splitting stack of enchantable items with maxstack > 1
        }
        public void RightClickStackableItem(Item item, Player player) {
            if(item.stack > 1) {
                if(Modified) {
					if (Main.mouseItem.IsAir) {
                        Main.mouseItem = new Item(item.type);
					}
					else if(Main.mouseItem.type == item.type) {
                        Main.mouseItem.stack++;
					}
				}
			}
		}
        public override void OnHitNPC(Item item, Player player, NPC target, int damage, float knockBack, bool crit) {
            item.DamageNPC(player, target, damage, crit, true);
        }
        public override void PostReforge(Item item) {
            if (UtilityMethods.debugging) ($"\\/PostReforge({item.S()})").Log();
            newPrefix = item.prefix;
            if (Main.reforgeItem.IsAir && item != null && !item.IsAir || !Main.reforgeItem.IsAir && Main.reforgeItem.G().normalReforge) {
                ReforgeItem(ref item, Main.LocalPlayer);
            }
            if (UtilityMethods.debugging) ($"/\\PostReforge({item.S()})").Log();
        }
        public override bool PreReforge(Item item) {
            if(UtilityMethods.debugging) ($"\\/PreReforge({item.S()})").Log();
            reforgeItem = item.Clone();
            if (!Main.reforgeItem.IsAir)
                Main.reforgeItem.G().normalReforge = true;
            if (UtilityMethods.debugging) {
                string s = $"reforgeItem: {reforgeItem.S()}, prefix: {reforgeItem.prefix}, Enchantments: ";
                foreach (Item enchantment in reforgeItem.G().enchantments) {
                    s += enchantment.S();
                }
                s.Log();
            }
            if (UtilityMethods.debugging) ($"/\\PreReforge({item.S()})").Log();
            return true;
        }
        public static void ReforgeItem(ref Item item, Player player) {
            WEPlayer wePlayer = player.G();
            cloneReforgedItem = true;
            reforgeItem.G().Clone(reforgeItem, item);
            cloneReforgedItem = false;
            reforgeItem = null;
            newPrefix = 0;
            if (!Main.reforgeItem.IsAir)
                Main.reforgeItem.G().normalReforge = false;
            item.G().normalReforge = false;
            wePlayer.UpdateItemStats(ref item);
        }
		public override bool ReforgePrice(Item item, ref int reforgePrice, ref bool canApplyDiscount) {
            reforgePrice -= lastValueBonus;
            return true;
        }
		public override void OnSpawn(Item item, IEntitySource source) {
			if (WEMod.IsWeaponItem(item) && source is EntitySource_DropAsItem dropSource && dropSource.Context == "Stolen") {
                for (int i = 0; i < Main.item.Length; i++) {
                    if (Main.netMode != NetmodeID.MultiplayerClient && item.type == Main.item[i].type && item.position == Main.item[i].position)
                        WEModSystem.stolenItemToBeCleared = i;
                }
			}
		}
		public override void OnCreate(Item item, ItemCreationContext context) {
			if(context is RecipeCreationContext) {
                item.CombineEnchantedItems(ref WEMod.consumedItems);
            }
		}
		public override bool CanStack(Item item1, Item item2) {
            if (!WEMod.IsEnchantable(item1) || item1.maxStack < 2 || item1.whoAmI > 0 || item2.whoAmI > 0)
                return true;
			if (stack0) {
                stack0 = false;
                item1.stack--;
			}
            else if (item2.G().stack0) {
                item2.G().stack0 = false;
                item2.stack--;
			}
            if (item2.G().inEnchantingTable && !Main.mouseLeft && !Main.mouseRight && !WeaponEnchantmentUI.pressedLootAll)
                return true;
            int maxStack = item1.maxStack;
            //if (item1.stack + item2.stack <= maxStack)
            /*if (item2.G().inEnchantingTable && (item1.stack == maxStack || item2.stack == maxStack))
                return false;*/
            if(item1.stack < maxStack && item2.stack < maxStack) {
                if (item1.G().infusedArmorSlot > -1 && item2.G().infusedArmorSlot < -1)
                    return false;
                if (item1.type == Main.mouseItem.type && item1.stack == Main.mouseItem.stack && Main.mouseRight && item2.stack > 1)
                    return true;
                List<Item> list = new List<Item>();
				/*if (item2.G().inEnchantingTable)
				{
                    list.Add(item1);
                    item1.CombineEnchantedItems(ref list);
                    if (item1.stack + item2.stack > maxStack)
                    {
                        Item tempItem = new Item(item2.type);
                        resetGlobals = true;
                        tempItem.G().Clone(tempItem, item1);
                        resetGlobals = false;
                    }
                }
				else*/
				{
                    list.Add(item2);
                    item1.CombineEnchantedItems(ref list);
                    if (item1.stack + item2.stack > maxStack) {
                        if (item2.G().inEnchantingTable)
                            for (int i = 0; i < enchantments.Length; i++)
                                Main.LocalPlayer.G().enchantingTableUI.enchantmentSlotUI[i].Item = new Item();
                        Item tempItem = new Item(item1.type);
                        resetGlobals = true;
                        tempItem.G().Clone(tempItem, item2);
                        resetGlobals = false;
                    }
                }
            }
            /*for(int i = 0; i < 5 ; i++)
			{
                Item enchantment1 = item1.G().enchantments[i];
                Item enchantment2 = item2.G().enchantments[i];
			}
            int xp1 = item1.G().experience;
            int xp2 = item2.G().experience;*/
            return true;
        }
	}

    public static class EnchantedItemStaticMethods
    {

        #region Properties (static)

        public static float BossXPMultiplier => WEMod.serverConfig.BossExperienceMultiplier / 400f;
        public static float NormalXPMultiplier => WEMod.serverConfig.ExperienceMultiplier / 100f;

        #endregion

        public static void SetupGlobals(this Item item) {
            //Not EnchantedItem return
            if (!item.TG(out EnchantedItem itemEI))
                return;

            //Update Enchantments
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++) {
                Item enchantmentItem = itemEI.enchantments[i];
                AllForOneEnchantmentBasic enchantment = (AllForOneEnchantmentBasic)enchantmentItem.ModItem;
                item.UpdateEnchantment(ref enchantment, i);
            }

            //Get Global Item Stats
            bool obtainedGlobalItemStats = item.TryGetGlotalItemStats();

            //Damage Multiplier (If failed to Get Global Item Stats)
            if(!obtainedGlobalItemStats)
                itemEI.damageMultiplier = item.GetWeaponMultiplier(itemEI.infusionPower);

            //Infusion
            if (itemEI.infusedItemName != "") {
                if (!item.TryInfuseItem(itemEI.infusedItemName) || !item.TryInfuseItem(itemEI.infusedItemName, false, true)) {
                    item.UpdateInfusionDamage(itemEI.damageMultiplier);
                }
            }

            //Update Level
            itemEI.UpdateLevel();
        }
        public static void UpdateEnchantment(this Item item, ref AllForOneEnchantmentBasic enchantment, int slotNum, bool remove = false) {
            //enchantment null return
            if (enchantment == null)
                return;

            //Not EnchantedItem return
            if (!item.TG(out EnchantedItem iGlobal))
                return;

			#region Debug

			if (UtilityMethods.debugging) ($"\\/UpdateEnchantment(" + item.S() + ", " + enchantment.S() + ", slotNum: " + slotNum + ", remove: " + remove).Log();

            #endregion

            //Buffs
            if (enchantment.Buff.Count > 0) {
                foreach (int buff in enchantment.Buff) {
                    if (UtilityMethods.debugging) (iGlobal.buffs.S(buff)).Log();

                    if (iGlobal.buffs.ContainsKey(buff)) {
                        iGlobal.buffs[buff] += (remove ? -1 : 1);

                        if (iGlobal.buffs[buff] < 1)
                            iGlobal.buffs.Remove(buff);
                    }
                    else {
                        iGlobal.buffs.Add(buff, 1);
                    }

                    if (UtilityMethods.debugging) (iGlobal.buffs.S(buff)).Log();
                }
            }

            //Debuffs
            if (enchantment.Debuff.Count > 0) {
                foreach (int debuff in enchantment.Debuff.Keys) {
                    if (UtilityMethods.debugging) (iGlobal.debuffs.S(debuff)).Log();

                    int duration = enchantment.Debuff[debuff];

                    if (iGlobal.debuffs.ContainsKey(debuff)) {
                        iGlobal.debuffs[debuff] += (remove ? -duration : duration);

                        if (iGlobal.debuffs[debuff] < 1)
                            iGlobal.debuffs.Remove(debuff);
                    }
                    else {
                        iGlobal.debuffs.Add(debuff, duration);
                    }

                    if (UtilityMethods.debugging) (iGlobal.debuffs.S(debuff)).Log();
                }
            }

            //OnHitBuffs
            if (enchantment.OnHitBuff.Count > 0) {
                foreach (int onHitBuff in enchantment.OnHitBuff.Keys) {
                    if (UtilityMethods.debugging) (iGlobal.onHitBuffs.S(onHitBuff)).Log();

                    int duration = enchantment.OnHitBuff[onHitBuff];
                    if (iGlobal.onHitBuffs.ContainsKey(onHitBuff)) {
                        iGlobal.onHitBuffs[onHitBuff] += (remove ? -duration : duration);

                        if (iGlobal.onHitBuffs[onHitBuff] < 1)
                            iGlobal.onHitBuffs.Remove(onHitBuff);
                    }
                    else {
                        iGlobal.onHitBuffs.Add(onHitBuff, duration);
                    }

                    if (UtilityMethods.debugging) (iGlobal.onHitBuffs.S(onHitBuff)).Log();
                }
            }

            //Estats
            foreach (EStat eStat in enchantment.EStats) {
                if (UtilityMethods.debugging) ($"eStat: " + eStat.S()).Log();

                float add = eStat.Additive * (remove ? -1f : 1f);
                float mult = remove ? 1 / eStat.Multiplicative : eStat.Multiplicative;
                float flat = eStat.Flat * (remove ? -1f : 1f);
                float @base = eStat.Base * (remove ? -1f : 1f);

                item.ApplyAllowedList(enchantment, ref add, ref mult, ref flat, ref @base);

                StatModifier statModifier = new StatModifier(1f + add, mult, flat, @base);

                if (!iGlobal.eStats.ContainsKey(eStat.StatName)) {
                    iGlobal.eStats.Add(eStat.StatName, statModifier);
                }
                else {
                    iGlobal.eStats[eStat.StatName] = iGlobal.eStats[eStat.StatName].CombineWith(statModifier);
                }
            }

            //StaticStats (StatModifiers)
            foreach (EnchantmentStaticStat staticStat in enchantment.StaticStats) {
                if (UtilityMethods.debugging) ($"staticStat: " + staticStat.S()).Log();

                float add = staticStat.Additive * (remove ? -1f : 1f);
                float mult = remove ? 1 / staticStat.Multiplicative : staticStat.Multiplicative;
                float flat = staticStat.Flat * (remove ? -1f : 1f);
                float @base = staticStat.Base * (remove ? -1f : 1f);

                item.ApplyAllowedList(enchantment, ref add, ref mult, ref flat, ref @base);

                StatModifier statModifier = new StatModifier(1f + add, mult, flat, @base);

                if (!iGlobal.statModifiers.ContainsKey(staticStat.Name)) {
                    item.G().statModifiers.Add(staticStat.Name, statModifier);
                }
                else {
                    iGlobal.statModifiers[staticStat.Name] = iGlobal.statModifiers[staticStat.Name].CombineWith(statModifier);
                }
            }

            //New Damage Type
            if (enchantment.NewDamageType > -1) {
                if (remove) {
                    item.DamageType = ContentSamples.ItemsByType[item.type].DamageType;

                    item.G().damageType = -1;
                }
                else {
                    item.G().damageType = enchantment.NewDamageType;

                    item.UpdateDamageType(enchantment.NewDamageType);
                }
            }

			#region Debug

			if (UtilityMethods.debugging) ($"/\\UpdateEnchantment(" + item.S() + ", " + enchantment.S() + ", slotNum: " + slotNum + ", remove: " + remove).Log();

			#endregion
		}
		public static void DamageNPC(this Item item, Player player, NPC target, int damage, bool crit, bool melee = false) {

            #region Debug

            if (UtilityMethods.debugging) ($"\\/DamageNPC").Log();

            #endregion

            target.GetGlobalNPC<WEGlobalNPC>().xpCalculated = true;
            
            //dummy return
            if (target.type == NPCID.TargetDummy || target.FullName != "Super Dummy")
                return;

            //friendly return
            if (target.friendly || target.townNPC)
                return;

            //value
            float value;
            if (Main.netMode == NetmodeID.MultiplayerClient) {
                value = ContentSamples.NpcsByNetId[target.type].value;
            }
            else {
                value = target.value;
            }

            //value or life max return
            if (value <= 0 && (target.SpawnedFromStatue || target.lifeMax <= 10))
                return;

            //NPC Characteristics Factors
            float noGravityFactor = target.noGravity ? 0.2f : 0f;
            float noTileCollideFactor = target.noTileCollide ? 0.2f : 0f;
            float knockBackResistFactor = 0.2f * (1f - target.knockBackResist);
            float npcCharacteristicsFactor = noGravityFactor + noTileCollideFactor + knockBackResistFactor;

            //Config Multiplier
            float configMultiplier = target.boss ? BossXPMultiplier : NormalXPMultiplier;

            //Experience Multiplier
            float experienceMultiplier = (1f + npcCharacteristicsFactor) * configMultiplier;

            //Armor Penetration
            float armorPenetration = 0;
            if (item != null) {
                armorPenetration = target.checkArmorPenetration(player.GetWeaponArmorPenetration(item));
            }

            //Actual Defense
            float actualDefence = target.defense / 2f - armorPenetration;
            float actualDamage = damage;
            if (!melee) {
                actualDamage -= actualDefence;

                if (crit)
                    actualDamage *= 2f;
            }

            //live vs damage check
            int xpDamage = (int)actualDamage;
            if (target.life < 0)
                xpDamage += target.life;

            //XP Damage <= 0 check
            if (xpDamage <= 0) {
                ($"Prevented an issue that would cause you to loose experience. (xpInt < 0) item: {item.S()}, target: {target.S()}, damage: {damage}, crit: {crit.S()}, " +
                    $"melee: {melee.S()}, Main.GameMode: {Main.GameMode}, xpDamage: {xpDamage}, actualDefence: {actualDefence}, actualDamage: {actualDamage}").LogNT();
                return;
            }

            //Crit Multiplier
            float effectiveDamagePerHit;
            if (item != null) {
                float critMultiplier = 1f + player.GetWeaponCrit(item) / 100f;

                effectiveDamagePerHit = item.damage * critMultiplier;
            }
            else {
                effectiveDamagePerHit = damage;
            }

            //Low Damage help multiplier
            float xp = xpDamage * experienceMultiplier * effectiveDamagePerHit;
            float effDamageDenom = effectiveDamagePerHit - actualDefence;
            if (effDamageDenom > 1)
                xp /= effDamageDenom;

            //Reduction Factor (Life Max)
            float reductionFactor = UtilityMethods.GetReductionFactor(target.lifeMax);
            xp /= reductionFactor;

            //XP <= 0 check
            int xpInt = (int)Math.Round(xp);
            if (xpInt == 0) {
                xpInt = 1;
            }
            else if (xpInt < 0) {
                ($"Prevented an issue that would cause you to loose experience. (xpInt < 0) item: {item.S()}, target: {target.S()}, damage: {damage}, crit: {crit.S()}, " +
                    $"melee: {melee.S()}, Main.GameMode: {Main.GameMode}, xpDamage: {xpDamage}, xpInt: {xpInt}, effectiveDamagePerHit: {effectiveDamagePerHit}, " +
                    $"actualDefence: {actualDefence}, actualDamage: {actualDamage}").LogNT();
                xpInt = 1;
            }

            //Gain XP (Item)
            if (item.TG(out EnchantedItem iGlobal))
                iGlobal.GainXP(item, xpInt);

            //Gain XP (Armor)
            player.AllArmorGainXp(xpInt);

			#region Debug

			if (UtilityMethods.debugging) ($"/\\DamageNPC").Log();

            #endregion
        }
        public static void AllArmorGainXp(this Player player, int xp) {
            int vanillaArmorLength = player.armor.Length / 2;
            var loader = LoaderManager.Get<AccessorySlotLoader>();
            for (int j = 0; j < player.G().equipArmor.Length; j++) {
                Item armor;

                //Check if armorslot is functional
                if(j < vanillaArmorLength) {
                    armor = player.armor[j];
				}
				else {
                    int num = j - vanillaArmorLength;
                    if (loader.ModdedIsAValidEquipmentSlotForIteration(num, player)) {
                        armor = loader.Get(num).FunctionalItem;
                    }
					else {
                        armor = new Item();
                    }
                }

                //Gain xp on each armor
                if (!armor.vanity && armor.TG(out EnchantedItem aGlobal)) {
                    float reductionFactor;
                    if (WEMod.IsArmorItem(armor)) {
                        reductionFactor = 2f;
                    }
                    else {
                        reductionFactor = 4f;
                    }

                    int xpInt = (int)Math.Round(xp / reductionFactor);

                    if (xpInt <= 0)
                        xpInt = 1;

                    aGlobal.GainXP(armor, xpInt);
                }
            }
        }
    }
}
