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
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Items;
using WeaponEnchantments.UI;
using static WeaponEnchantments.Common.Configs.ConfigValues;

namespace WeaponEnchantments.Common.Globals
{
	public class EnchantedItem : GlobalItem
    {
		#region Constants

		public const int MAX_LEVEL = 40;

		#endregion

		#region Reforge (static)

		public static Item reforgeItem = null;
        public static Item calamityAndAutoReforgePostReforgeItem = null;
        public static bool calamityReforged = false;
        public static bool cloneReforgedItem = false;
        public static bool resetLastValueBonus = false;

        #endregion

        #region Tracking (static)

        public static bool resetGlobals = false;
        public static bool skipUpdateValue = false;

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
        private int _infusionValueAdded = 0;
        public int InfusionValueAdded {
            get { return _infusionValueAdded; }
			set {
                int lastValue = _infusionValueAdded;
                _infusionValueAdded = value;

                //If value changed, upted Item Value
                if(lastValue != _infusionValueAdded) {
                    UpdateItemValue();
				}
			}
		}

        #endregion

        #region Experience

        private int _experience = 0;
        public int Experience {
            get { return _experience; }
            set {
                int lastValue = _experience;
                _experience = value;
                
                //If changed, update Level/Value
                if(lastValue != _experience)
                    UpdateLevelAndValue();
            }
        }
        public int levelBeforeBooster = 0;
        private bool _powerBoosterInstalled = false;
        public bool PowerBoosterInstalled { 
            get { return _powerBoosterInstalled; }
			set {
                bool lastValue = _powerBoosterInstalled;
                _powerBoosterInstalled = value;

                //If changed, update Level/Value
                if (lastValue != _powerBoosterInstalled)
                    UpdateLevelAndValue();
            }
        }
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
        private int _stack = 0;
        public int Stack {
            get {
                if (_stack <= 0)
                    _stack = Item.stack;
                return _stack;
            }
			set {
                int lastValue = _stack;
                
                //If changed, update Value
                if (lastValue != value && (lastValue > 1 || value > 1))
                    UpdateItemValue();

                _stack = value;
            }
		}
        private bool _stack0 = false;
        public bool Stack0 {
            get { return _stack0; }
			set {
                bool lastValue = _stack0;
                _stack0 = value;

                //If changed, update Value
                if (lastValue != _stack0)
                    UpdateItemValue();
            }
		}
        public bool needsUpdateOldItems = false;

        #endregion

        #region Properties (instance)

        public Item Item;
        public bool Enchanted => Experience != 0 || PowerBoosterInstalled;
        public bool Modified => Experience != 0 || PowerBoosterInstalled || infusedItemName != "";

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

            //Set Tracked Item (similar to ModItem.Item)
            if (resetGlobals) {
                Item = itemClone;
            }
            else {
                Item = item;
            }

            if (cloneReforgedItem || resetGlobals) {
                if(itemClone.TryGetEnchantedItem(out EnchantedItem iGlobal)) {
                    clone = iGlobal;
                }
				else {
                    $"In EnchantedItem, Failed to Clone(item: {item.S()}, itemClone: {itemClone.S()}), cloneReforgedItem: {cloneReforgedItem.S()}, resetGlobals: {resetGlobals.S()}.".LogNT();
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
                clone.InfusionValueAdded = InfusionValueAdded;

                #endregion

                #region Experience

                clone.Experience = _experience;
                clone.levelBeforeBooster = levelBeforeBooster;
                clone.PowerBoosterInstalled = PowerBoosterInstalled;
                clone.level = level;
				if (resetGlobals) {
                    clone.lastValueBonus = lastValueBonus;
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
                clone.favorited = favorited;
                clone.Stack0 = Stack0;

                #endregion
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
            Item = item;

			#region Debug

			if (LogMethods.debugging) ($"\\/LoadData(" + item.Name + ")").Log();

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

            Experience = tag.Get<int>("experience");
            PowerBoosterInstalled = tag.Get<bool>("powerBooster");

            #endregion

            #region Infusion

            infusedItemName = tag.Get<string>("infusedItemName");
            infusionPower = tag.Get<int>("infusedPower");

            #endregion

            #region Tracking (instance)

            Stack0 = tag.Get<bool>("stack0");

            #endregion

            if (Experience < 0)
                Experience = int.MaxValue;

            #region Debug

            if (LogMethods.debugging) ($"/\\LoadData(" + item.Name + ")").Log();

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

			tag["experience"] = Experience;
            tag["powerBooster"] = PowerBoosterInstalled;

			#endregion

			#region Infusion

			tag["infusedItemName"] = infusedItemName;
            tag["infusedPower"] = infusionPower;

            #endregion

            #region Tracking (instance)

            tag["stack0"] = Stack0;

			#endregion
		}
		public override void NetSend(Item item, BinaryWriter writer) {

            #region Debug

            if (LogMethods.debugging) {
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

                if (LogMethods.debugging) ($"e " + i + ": " + enchantments[i].Name).Log();

				#endregion
			}

			#endregion

			#region Experience

			writer.Write(Experience);
            writer.Write(PowerBoosterInstalled);

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

            writer.Write(Stack0);

			#endregion

            #region Debug

            if (LogMethods.debugging) {
                ($"eStats.Count: " + eStats.Count + ", statModifiers.Count: " + statModifiers.Count).Log();
                ($"/\\NetSend(" + item.Name + ")").Log();
            }

			#endregion
		}
		public override void NetReceive(Item item, BinaryReader reader) {
            Item = item;

			#region Debug

			if (LogMethods.debugging) {
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

				if (LogMethods.debugging) ($"e " + i + ": " + enchantments[i].Name).Log();

				#endregion
			}

			#endregion

			#region Experience

			_experience = reader.ReadInt32();
            PowerBoosterInstalled = reader.ReadBoolean();

            #endregion

            #region Infusion

            bool noName = reader.ReadBoolean();
            if (!noName) {
                infusedItemName = reader.ReadString();
                infusionPower = reader.ReadInt32();
            }

            #endregion

            #region Tracking (instance)

            Stack0 = reader.ReadBoolean();

            #endregion

            item.SetupGlobals();

			#region Debug

			if (LogMethods.debugging) {
                ($"eStats.Count: " + eStats.Count + ", statModifiers.Count: " + statModifiers.Count).Log();
                ($"/\\NetRecieve(" + item.Name + ")").Log();
            }

			#endregion
		}
		public override void UpdateInventory(Item item, Player player) {
            if (Modified) {
                //Stars Above compatibility fix
                if (baseDamageType != damageType) {
                    if (baseDamageType == -1)
                        baseDamageType = ContentSamples.ItemsByType[item.type].DamageType.Type;

                    if (AlwaysOverrideDamageType || item.DamageType.Type == baseDamageType)
                        item.UpdateDamageType(damageType);
                }

                //Update Item Value if stack changed.
                Stack = item.stack;
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
            if (player.GetWEPlayer().ItemInUI().TryGetEnchantedItem()) {
                inEnchantingTable = false;
                if (item.GetInfusionArmorSlot() != infusedArmorSlot) {
                    infusedArmorSlot = -1;
                    item.TryInfuseItem(new Item(), true);
                }
            }
        }
        public override bool OnPickup(Item item, Player player) {
            player.GetWEPlayer().UpdateItemStats(ref item);

            return true;
        }
        public void UpdateItemValue() {
            //Fix for stack sizes not being updated until after CanStack
            if (skipUpdateValue)
                return;

            int enchantmentsValue = 0;
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++) {
                enchantmentsValue += enchantments[i].value;
            }
            int powerBoosterValue = PowerBoosterInstalled ? ModContent.GetModItem(ModContent.ItemType<PowerBooster>()).Item.value : 0;
            int valueToAdd = enchantmentsValue + (int)(EnchantmentEssence.valuePerXP * Experience) + powerBoosterValue + InfusionValueAdded;
            valueToAdd /= Item.stack;

            if (Stack0)
                valueToAdd -= ContentSamples.ItemsByType[Item.type].value;

			//Item.value can be reset by reforging
			if (resetLastValueBonus) {
                lastValueBonus = 0;
                resetLastValueBonus = false;
			}

            Item.value += valueToAdd - lastValueBonus / Stack;
            lastValueBonus = valueToAdd * Item.stack;
        }
        public void UpdateLevelAndValue() {
            int l;
            for (l = 0; l < MAX_LEVEL; l++) {
                if (_experience < WEModSystem.levelXps[l]) {
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

            if (PowerBoosterInstalled)
                level += 10;

            UpdateItemValue();
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
            WEPlayer wePlayer = Main.LocalPlayer.GetWEPlayer();
            bool enchantmentsToolTipAdded = false;

            //Stack0
            if(Modified || inEnchantingTable) {
                if (Stack0) {
                    string tooltip = $"!!!OUT OF AMMO!!!";
                    tooltips.Add(new TooltipLine(Mod, "stack0", tooltip) { OverrideColor = Color.Yellow });
                }
            }

            //Xp and level Tooltips
            if (Enchanted || inEnchantingTable) {

                string pointsName = WEMod.clientConfig.UsePointsAsTooltip ? "Points" : "Enchantment Capacity";

                if (PowerBoosterInstalled) {
                    string tooltip = $"Level: {levelBeforeBooster}  {pointsName} available: {GetLevelsAvailable()} (Booster Installed)";
                    tooltips.Add(new TooltipLine(Mod, "level", tooltip) { OverrideColor = Color.LightGreen });
                }
				else {
                    string tooltip = $"Level: {levelBeforeBooster}  {pointsName} available: {GetLevelsAvailable()}";
                    tooltips.Add(new TooltipLine(Mod, "level", tooltip) { OverrideColor = Color.LightGreen });
                }

                string levelString;
                if(levelBeforeBooster < MAX_LEVEL) {
                    levelString = $" ({WEModSystem.levelXps[levelBeforeBooster] - Experience} to next level)";
                }
				else {
                    levelString = " (Max Level)";
                }
                string levelTooltip = $"Experience: {Experience}{levelString}";
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
                    Enchantment enchantment = (Enchantment)enchantments[i].ModItem;

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
                    Color color = Enchantment.rarityColors[enchantment.EnchantmentTier];
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
        public void GainXP(Item item, int xpInt, bool noMessage = false)
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();

            int currentLevel = levelBeforeBooster;

            //xp < 0 return
            if (xpInt < 0) {
                $"Prevented your {item.S()} from loosing experience due to a calculation error.".LogNT();

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

            //UpdateLevelAndValue();

            if (noMessage)
                return;

            //Already max level return
            if (currentLevel >= MAX_LEVEL)
                return;

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
            float ammoSaveChance = -1f * weapon.ApplyEStat("AmmoCost", 0f);

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

                if (!inventoryItem.TryGetEnchantedItem(out EnchantedItem invGlobal))
                    continue;

                if (invGlobal.Modified)
                    continue;

                if (invGlobal.Stack0)
                    continue;

                //Restock (found same item)
                item.stack = inventoryItem.stack;
                Stack0 = false;
                player.inventory[i] = new Item();
                return;
            }
        }
		public override bool? UseItem(Item item, Player player) {
            WEPlayer wePlayer = player.GetModPlayer<WEPlayer>();

            if (!Modified)
                return null;

            //CatastrophicRelease
            if (eStats.ContainsKey("CatastrophicRelease")) {
                player.statMana = 0;
            }

            //AllForOne use cooldown
            if (eStats.ContainsKey("AllForOne")) {
                int timer = (int)((float)item.useTime * item.ApplyEStat("NPCHitCooldown", 0.5f));
                wePlayer.allForOneTimer = timer;
            }

            //stack0  (item.placeStyle fix for a placable enchantable item)
            if(item.consumable && item.stack < 2 && Modified && item.placeStyle == 0) {
                Restock(item);

                if(item.stack < 2) {
                    item.stack = 2;
                    Stack0 = true;
                }
			}

            return null;
        }
        public override bool CanUseItem(Item item, Player player) {
			
            WEPlayer wePlayer = player.GetModPlayer<WEPlayer>();

            //stack0
            if (Stack0) {
                Restock(item);

				if (Stack0) {
                    //Restock failed
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

            if (!Modified)
                return false;

            //Prevent global item duplication exploit
            if (Main.mouseItem.IsAir) {
                return true;
            }

            return false;
        }
        public override void RightClick(Item item, Player player)
        {
            if (item.stack <= 1)
                return;

            if (!Modified)
                return;

            //Prevent specific items from running RightClickStackableItem
            switch (item.Name) {
                case "Primary Zenith":
                    return;
            }

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

            if(Main.mouseItem.TryGetEnchantedItem(out EnchantedItem mGlobal))
                mGlobal.Stack = Main.mouseItem.stack;

            //Update Item Value if stack changed.
            Stack = item.stack;
        }
        public override void OnHitNPC(Item item, Player player, NPC target, int damage, float knockBack, bool crit) {
            item.DamageNPC(player, target, damage, crit, true);
        }
		public override bool PreReforge(Item item) {

			#region Debug

			if (LogMethods.debugging) ($"\\/PreReforge({item.S()})").Log();

            #endregion

            //Calamity
            reforgeItem = item.Clone();

			#region Debug

			if (LogMethods.debugging) {
                string s = $"reforgeItem: {reforgeItem.S()}, prefix: {reforgeItem.prefix}, Enchantments: ";
                foreach (Item enchantment in reforgeItem.GetEnchantedItem().enchantments) {
                    s += enchantment.S();
                }
                s.Log();

                ($"/\\PreReforge({item.S()})").Log();
            }

			#endregion

			return true;
        }
        public override void PostReforge(Item item) {

            #region Debug

            if (LogMethods.debugging) ($"\\/PostReforge({item.S()})").Log();

            #endregion

            if (!Modified)
                return;

            Item = item;

            //Vanilla
            resetLastValueBonus = true;

            //Calamity
            if (WEMod.calamity)
                calamityReforged = true;

			//Calamity deletes global data after reforge, so it is handled on the next tick in WEModSystem.
			if (!WEMod.calamity) {
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
            if (item.TryGetEnchantedItem(out EnchantedItem iGlobal)) {
                //Calamity
                if (needCloneGlobals && reforgeItem.TryGetEnchantedItem(out EnchantedItem rGlobal)) {
                    cloneReforgedItem = true;
                    rGlobal.Clone(reforgeItem, item);
                    cloneReforgedItem = false;
                }

                //Vanilla
                iGlobal.UpdateItemValue();
                iGlobal.prefix = -1;
            }

            //Calamity
            reforgeItem = null;

            //Vanilla
            player.GetWEPlayer().UpdateItemStats(ref item);

            //Calamity
            calamityReforged = false;

            //Calamity and AutoReforge
            calamityAndAutoReforgePostReforgeItem = null;
        }
		public override bool ReforgePrice(Item item, ref int reforgePrice, ref bool canApplyDiscount) {
            float priceMultiplier = ((float)item.value - (float)lastValueBonus) / (float)item.value;
            float reforgePriceFloat = reforgePrice * priceMultiplier;
            reforgePrice = (int)reforgePriceFloat;

            return true;
        }
		public override void OnSpawn(Item item, IEntitySource source) {
            //Fargo's Mod fix for pirates that steal items
			if (source is EntitySource_DropAsItem dropSource && dropSource.Context == "Stolen") {
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    return;

                for (int i = 0; i < Main.item.Length; i++) {
                    if (item.type == Main.item[i].type && item.position == Main.item[i].position)
                        WEModSystem.stolenItemToBeCleared = i;
                }
			}
		}
		public override void OnCreate(Item item, ItemCreationContext context) {
			if(context is RecipeCreationContext recipeCreationContext) {
                if(recipeCreationContext.ConsumedItems?.Count > 0) {
                    item.CombineEnchantedItems(ref recipeCreationContext.ConsumedItems);
                }
				else {
                    //Temporary for Magic storage
                    item.CombineEnchantedItems(ref WEMod.consumedItems);
				}
                //Temporary for Magic storage
                WEMod.consumedItems.Clear();
            }
		}
		public override bool CanStack(Item item1, Item item2) {
            //Check max stack and always allow combining in the 
            if (!item1.TryGetEnchantedItem(out EnchantedItem i1Global) || item1.maxStack < 2)
                return true;

            //item1 already tested for try.
            EnchantedItem i2Global = item2.GetEnchantedItem();

            bool modified1 = i1Global.Modified;
            bool modified2 = i2Global.Modified;

            if (!modified1 && !modified2) {
                //Not Modified
                return true;
            }
            else if (modified1 && modified2 && (item1.whoAmI > 0 || item2.whoAmI > 0)) {
                //Prevent stacking in the world if both modified.
                return false;
			}

            //Stack0
            if (Stack0) {
                item1.stack--;
                Stack0 = false;
			}
            else if (i2Global.Stack0) {
                item2.stack--;
                i2Global.Stack0 = false;
			}

            //Enchanting table loot all
            if (i2Global.inEnchantingTable && !Main.mouseLeft && !Main.mouseRight && !WeaponEnchantmentUI.pressedLootAll)
                return true;


            int maxStack = item1.maxStack;

            //Both at max stack
            if (item1.stack >= maxStack && item2.stack >= maxStack)
                return false;

            //Prevent stackable armor from merging (such as buckets)
            if (i1Global.infusedArmorSlot > -1 && i2Global.infusedArmorSlot < -1)
                return false;

            //Multishot stack with right click
            if (item1.type == Main.mouseItem.type && item1.stack == Main.mouseItem.stack && Main.mouseRight && item2.stack > 1)
                return true;

            //Combine item2 into item1
            List<Item> list = new List<Item>();
            list.Add(item2);
            skipUpdateValue = true;
            item1.CombineEnchantedItems(ref list);
            skipUpdateValue = false;

            //Clear item2 if stackTotal > max stack
            int stackTotal = item1.stack + item2.stack;
            if (stackTotal > maxStack) {
                //Clear enchantments in enchanting table if item2 is in it (Will only have cleared off of the player tracked enchantments from combining)
                if (i2Global.inEnchantingTable) {
                    for (int i = 0; i < enchantments.Length; i++) {
                        Main.LocalPlayer.GetWEPlayer().enchantingTableUI.enchantmentSlotUI[i].Item = new Item();
                    }
                        
                }
                
                //Reset item2 globals
                Item tempItem = new Item(item1.type);
                resetGlobals = true;
                tempItem.GetEnchantedItem().Clone(tempItem, item2);
                resetGlobals = false;
            }
            
            return true;
        }
	}

    public static class EnchantedItemStaticMethods
    {
        public static void SetupGlobals(this Item item) {
            //Not EnchantedItem return
            if (!item.TryGetEnchantedItem(out EnchantedItem iGlobal))
                return;

            //Update Enchantments
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++) {
                Item enchantmentItem = iGlobal.enchantments[i];
                Enchantment enchantment = (Enchantment)enchantmentItem.ModItem;
                item.UpdateEnchantment(ref enchantment, i);
            }

            //Get Global Item Stats
            bool obtainedGlobalItemStats = item.TryGetInfusionStats();

            //Damage Multiplier (If failed to Get Global Item Stats)
            if(!obtainedGlobalItemStats)
                iGlobal.damageMultiplier = item.GetWeaponMultiplier(iGlobal.infusionPower);
        }
        public static void UpdateEnchantment(this Item item, ref Enchantment enchantment, int slotNum, bool remove = false) {
            //enchantment null return
            if (enchantment == null)
                return;

            //Not EnchantedItem return
            if (!item.TryGetEnchantedItem(out EnchantedItem iGlobal))
                return;

			#region Debug

			if (LogMethods.debugging) ($"\\/UpdateEnchantment(" + item.S() + ", " + enchantment.S() + ", slotNum: " + slotNum + ", remove: " + remove).Log();

            #endregion

            //Buffs
            if (enchantment.Buff.Count > 0) {
                foreach (int buff in enchantment.Buff) {
                    if (LogMethods.debugging) (iGlobal.buffs.S(buff)).Log();

                    if (iGlobal.buffs.ContainsKey(buff)) {
                        iGlobal.buffs[buff] += (remove ? -1 : 1);

                        if (iGlobal.buffs[buff] < 1)
                            iGlobal.buffs.Remove(buff);
                    }
                    else {
                        iGlobal.buffs.Add(buff, 1);
                    }

                    if (LogMethods.debugging) (iGlobal.buffs.S(buff)).Log();
                }
            }

            //Debuffs
            if (enchantment.Debuff.Count > 0) {
                foreach (int debuff in enchantment.Debuff.Keys) {
                    if (LogMethods.debugging) (iGlobal.debuffs.S(debuff)).Log();

                    int duration = enchantment.Debuff[debuff];

                    if (iGlobal.debuffs.ContainsKey(debuff)) {
                        iGlobal.debuffs[debuff] += (remove ? -duration : duration);

                        if (iGlobal.debuffs[debuff] < 1)
                            iGlobal.debuffs.Remove(debuff);
                    }
                    else {
                        iGlobal.debuffs.Add(debuff, duration);
                    }

                    if (LogMethods.debugging) (iGlobal.debuffs.S(debuff)).Log();
                }
            }

            //OnHitBuffs
            if (enchantment.OnHitBuff.Count > 0) {
                foreach (int onHitBuff in enchantment.OnHitBuff.Keys) {
                    if (LogMethods.debugging) (iGlobal.onHitBuffs.S(onHitBuff)).Log();

                    int duration = enchantment.OnHitBuff[onHitBuff];
                    if (iGlobal.onHitBuffs.ContainsKey(onHitBuff)) {
                        iGlobal.onHitBuffs[onHitBuff] += (remove ? -duration : duration);

                        if (iGlobal.onHitBuffs[onHitBuff] < 1)
                            iGlobal.onHitBuffs.Remove(onHitBuff);
                    }
                    else {
                        iGlobal.onHitBuffs.Add(onHitBuff, duration);
                    }

                    if (LogMethods.debugging) (iGlobal.onHitBuffs.S(onHitBuff)).Log();
                }
            }

            //Estats
            foreach (EStat eStat in enchantment.EStats) {
                if (LogMethods.debugging) ($"eStat: " + eStat.S()).Log();

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
                if (LogMethods.debugging) ($"staticStat: " + staticStat.S()).Log();

                float add = staticStat.Additive * (remove ? -1f : 1f);
                float mult = remove ? 1 / staticStat.Multiplicative : staticStat.Multiplicative;
                float flat = staticStat.Flat * (remove ? -1f : 1f);
                float @base = staticStat.Base * (remove ? -1f : 1f);

                item.ApplyAllowedList(enchantment, ref add, ref mult, ref flat, ref @base);

                StatModifier statModifier = new StatModifier(1f + add, mult, flat, @base);

                if (!iGlobal.statModifiers.ContainsKey(staticStat.Name)) {
                    item.GetEnchantedItem().statModifiers.Add(staticStat.Name, statModifier);
                }
                else {
                    iGlobal.statModifiers[staticStat.Name] = iGlobal.statModifiers[staticStat.Name].CombineWith(statModifier);
                }
            }

            //New Damage Type
            if (enchantment.NewDamageType > -1) {
                if (remove) {
                    item.DamageType = ContentSamples.ItemsByType[item.type].DamageType;

                    item.GetEnchantedItem().damageType = -1;
                }
                else {
                    item.GetEnchantedItem().damageType = enchantment.NewDamageType;

                    item.UpdateDamageType(enchantment.NewDamageType);
                }
            }

            //Update item Value
            iGlobal.UpdateItemValue();

            #region Debug

            if (LogMethods.debugging) ($"/\\UpdateEnchantment(" + item.S() + ", " + enchantment.S() + ", slotNum: " + slotNum + ", remove: " + remove).Log();

			#endregion
		}
		public static void DamageNPC(this Item item, Player player, NPC target, int damage, bool crit, bool melee = false) {

            #region Debug

            if (LogMethods.debugging) ($"\\/DamageNPC").Log();

            #endregion
            
            //dummy return
            if (target.type == NPCID.TargetDummy || target.FullName == "Super Dummy")
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

            float balanceMultiplier = target.boss ? 0.25f : 1f;

            //Experience Multiplier
            float experienceMultiplier = (1f + npcCharacteristicsFactor) * configMultiplier * balanceMultiplier;

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
            float reductionFactor = GetReductionFactor(target.lifeMax);
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
            if (item.TryGetEnchantedItem(out EnchantedItem iGlobal))
                iGlobal.GainXP(item, xpInt);

            //Gain XP (Armor)
            player.AllArmorGainXp(xpInt);

			#region Debug

			if (LogMethods.debugging) ($"/\\DamageNPC").Log();

            #endregion
        }
        public static float GetReductionFactor(int hp) {
            float factor = hp < 7000 ? hp / 1000f + 1f : 8f;
            return factor;
        }
        public static void AllArmorGainXp(this Player player, int xp) {
            int vanillaArmorLength = player.armor.Length / 2;
            var loader = LoaderManager.Get<AccessorySlotLoader>();
            for (int j = 0; j < player.GetWEPlayer().equipArmor.Length; j++) {
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
                if (!armor.vanity && armor.TryGetEnchantedItem(out EnchantedItem aGlobal)) {
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
        public static void CombineEnchantedItems(this Item item, ref List<Item> consumedItems) {
            if (consumedItems.Count <= 0)
                return;

            for (int c = 0; c < consumedItems.Count; c++) {
                Item consumedItem = consumedItems[c];
                if (consumedItem.IsAir)
                    continue;

                if (!consumedItem.TryGetGlobalItem(out EnchantedItem cGlobal))
                    continue;

                if (!cGlobal.Modified)
                    continue;

                if (item.TryGetGlobalItem(out EnchantedItem iGlobal)) {
                    item.CheckConvertExcessExperience(consumedItem);
                    if (iGlobal.infusionPower < cGlobal.infusionPower && item.GetWeaponInfusionPower() < cGlobal.infusionPower) {
                        item.TryInfuseItem(consumedItem);
                        item.TryInfuseItem(consumedItem, false, true);
                    }

                    if (cGlobal.PowerBoosterInstalled) {
                        if (!iGlobal.PowerBoosterInstalled)
                            iGlobal.PowerBoosterInstalled = true;
                        else
                            Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), ModContent.ItemType<PowerBooster>(), 1);
                    }
                                    
                    int j;
                    for (j = 0; j <= EnchantingTable.maxEnchantments; j++) {
                        if (j > 4)
                            break;
                        if (iGlobal.enchantments[j].IsAir)
                            break;
                    }
                    for (int k = 0; k < EnchantingTable.maxEnchantments; k++) {
                        if (!cGlobal.enchantments[k].IsAir) {
                            Enchantment enchantment = ((Enchantment)cGlobal.enchantments[k].ModItem);
                            int uniqueItemSlot = WEUIItemSlot.FindSwapEnchantmentSlot(enchantment, item);
                            bool cantFit = false;
                            if (enchantment.GetCapacityCost() <= iGlobal.GetLevelsAvailable()) {
                                if (uniqueItemSlot == -1) {
                                    if (enchantment.Utility && iGlobal.enchantments[4].IsAir && (WEMod.IsWeaponItem(item) || WEMod.IsArmorItem(item))) {
                                        iGlobal.enchantments[4] = cGlobal.enchantments[k].Clone();
                                        item.ApplyEnchantment(j);
                                    }
                                    else if (j < 4) {
                                        iGlobal.enchantments[j] = cGlobal.enchantments[k].Clone();
                                        item.ApplyEnchantment(j);
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
                            if (cantFit) {
                                Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), cGlobal.enchantments[k].type, 1);
                            }
                        }
                        cGlobal.enchantments[k] = new Item();
                    }
                }
                else {
                    item.CheckConvertExcessExperience(consumedItem);
                    int numberEssenceRecieved;
                    int xpCounter = iGlobal.Experience;
                    for (int tier = EnchantingTable.maxEssenceItems - 1; tier >= 0; tier--) {
                        numberEssenceRecieved = xpCounter / (int)EnchantmentEssenceBasic.xpPerEssence[tier] * 4 / 5;
                        xpCounter -= (int)EnchantmentEssenceBasic.xpPerEssence[tier] * numberEssenceRecieved;
                        if (xpCounter < (int)EnchantmentEssenceBasic.xpPerEssence[0] && xpCounter > 0 && tier == 0) {
                            xpCounter = 0;
                            numberEssenceRecieved += 1;
                        }
                        Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), EnchantmentEssenceBasic.IDs[tier], 1);
                    }
                    if (cGlobal.PowerBoosterInstalled) {
                        Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), ModContent.ItemType<PowerBooster>(), 1);
                    }
                    for (int k = 0; k < EnchantingTable.maxEnchantments; k++) {
                        if (!cGlobal.enchantments[k].IsAir) {
                            Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), cGlobal.enchantments[k].type, 1);
                        }
                    }
                }
            }

            consumedItems.Clear();
        }
        public static void RemoveUntilPositive(this Item item, Player player) {
            int netMode = Main.netMode;
            int gameMode = Main.GameMode;
            if (!item.IsAir) {
                if (WEMod.IsEnchantable(item)) {
                    if (item.TryGetGlobalItem(out EnchantedItem iGlobal)) {
                        if (iGlobal.GetLevelsAvailable() < 0) {
                            for (int k = EnchantingTable.maxEnchantments - 1; k >= 0 && iGlobal.GetLevelsAvailable() < 0; k--) {
                                if (!iGlobal.enchantments[k].IsAir) {
                                    item.GetEnchantedItem().enchantments[k] = player.GetItem(player.whoAmI, iGlobal.enchantments[k], GetItemSettings.LootAllSettings);
                                }
                                if (!iGlobal.enchantments[k].IsAir) {
                                    player.QuickSpawnItem(player.GetSource_Misc("PlayerDropItemCheck"), iGlobal.enchantments[k]);
                                    iGlobal.enchantments[k] = new Item();
                                }
                            }
                            Main.NewText("Your " + item.Name + "' level is too low to use that many enchantments.");
                        }//Check too many enchantments on item
                    }
                }
            }
        }
        public static bool IsSameEnchantedItem(this Item item1, Item item2) {
            if (!item1.TryGetEnchantedItem(out EnchantedItem global1) || !item2.TryGetEnchantedItem(out EnchantedItem global2))
                return false;

            if (item1.type != item2.type || item1.prefix != item2.prefix)
                return false;

            if (global1.PowerBoosterInstalled != global2.PowerBoosterInstalled || global1.infusedItemName != global2.infusedItemName)
                return false;

            for (int i = 0; i < EnchantingTable.maxEnchantments; i++) {
                if (global1.enchantments[i].type != global2.enchantments[i].type)
                    return false;
            }

            return true;
        }
        public static void RoundCheck(this StatModifier statModifier, ref float value, int baseValue, StatModifier appliedStatModifier, int contentSample) {
            if (LogMethods.debugging) ($"\\/RoundCheck").Log();
            if (value > baseValue) {
                float checkValue = (float)((int)value + 1) * 1f / statModifier.ApplyTo(1f);
                if ((int)Math.Round(checkValue) == baseValue) {
                    float sampleValue = WEPlayer.CombineStatModifier(appliedStatModifier, statModifier, true).ApplyTo(contentSample);
                    if ((int)Math.Round(sampleValue) == baseValue)
                        value += 0.5f;
                }
            }
            if (LogMethods.debugging) ($"/\\RoundCheck").Log();
        }
        public static int DamageBeforeArmor(this Item item, bool crit) {
            return (int)Math.Round(item.ApplyEStat("Damage", (float)item.damage * (crit ? 2f : 1f)));
        }
        public static void ApplyEnchantment(this Item item, int i) {
            if (LogMethods.debugging) ($"\\/ApplyEnchantment(i: " + i + ")").Log();
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            if (!item.IsAir) {
                EnchantedItem iGlobal = item.GetEnchantedItem();
                Enchantment enchantment = (Enchantment)(iGlobal.enchantments[i].ModItem);
                item.UpdateEnchantment(ref enchantment, i);
                wePlayer.UpdateItemStats(ref item);
            }
            if (LogMethods.debugging) ($"/\\ApplyEnchantment(i: " + i + ")").Log();
        }
        public static void UpdateDamageType(this Item item, int type) {
            switch ((DamageTypeSpecificID)type) {
                case DamageTypeSpecificID.Default:
                    item.DamageType = DamageClass.Default;
                    break;
                case DamageTypeSpecificID.Generic:
                    item.DamageType = DamageClass.Generic;
                    break;
                case DamageTypeSpecificID.Melee:
                    item.DamageType = DamageClass.Melee;
                    break;
                case DamageTypeSpecificID.MeleeNoSpeed:
                    item.DamageType = DamageClass.MeleeNoSpeed;
                    break;
                case DamageTypeSpecificID.Ranged:
                    item.DamageType = DamageClass.Ranged;
                    break;
                case DamageTypeSpecificID.Magic:
                    item.DamageType = DamageClass.Magic;
                    break;
                case DamageTypeSpecificID.Summon:
                    item.DamageType = DamageClass.Summon;
                    break;
                case DamageTypeSpecificID.SummonMeleeSpeed:
                    item.DamageType = DamageClass.SummonMeleeSpeed;
                    break;
                case DamageTypeSpecificID.MagicSummonHybrid:
                    item.DamageType = DamageClass.MagicSummonHybrid;
                    break;
                case DamageTypeSpecificID.Throwing:
                    item.DamageType = DamageClass.Throwing;
                    break;
            }
        }
        public static void ApplyAllowedList(this Item item, Enchantment enchantment, ref float add, ref float mult, ref float flat, ref float @base) {
            if (WEMod.IsWeaponItem(item)) {
                if (enchantment.AllowedList.ContainsKey("Weapon")) {
                    add *= enchantment.AllowedList["Weapon"];
                    mult = 1f + (mult - 1f) * enchantment.AllowedList["Weapon"];
                    flat *= enchantment.AllowedList["Weapon"];
                    @base *= enchantment.AllowedList["Weapon"];
                    return;
                }
                else {
                    add = 1f;
                    mult = 1f;
                    flat = 0f;
                    @base = 0f;
                }
            }
            if (WEMod.IsArmorItem(item)) {
                if (enchantment.AllowedList.ContainsKey("Armor")) {
                    add *= enchantment.AllowedList["Armor"];
                    mult = 1f + (mult - 1f) * enchantment.AllowedList["Armor"];
                    flat *= enchantment.AllowedList["Armor"];
                    @base *= enchantment.AllowedList["Armor"];
                    return;
                }
                else {
                    add = 1f;
                    mult = 1f;
                    flat = 0f;
                    @base = 0f;
                }
            }
            if (WEMod.IsAccessoryItem(item)) {
                if (enchantment.AllowedList.ContainsKey("Accessory")) {
                    add *= enchantment.AllowedList["Accessory"];
                    mult = 1f + (mult - 1f) * enchantment.AllowedList["Accessory"];
                    flat *= enchantment.AllowedList["Accessory"];
                    @base *= enchantment.AllowedList["Accessory"];
                    return;
                }
                else {
                    add = 1f;
                    mult = 1f;
                    flat = 0f;
                    @base = 0f;
                }
            }
        }
    }
}
