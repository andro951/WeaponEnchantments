using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using static WeaponEnchantments.Common.EnchantingRarity;
using static WeaponEnchantments.Common.Globals.EnchantedItemStaticMethods;
using static WeaponEnchantments.Items.Enchantment;

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
            get {
                if(_stack0) {
                    if(Item.stack > 1) {
                        Item.stack--;
                        _stack0 = false;
                        Stack = Item.stack;
                    }
				}

                return _stack0;
            }
			set {
                bool lastValue = _stack0;
                _stack0 = value;

                //If changed, update Value
                if (lastValue != _stack0)
                    UpdateItemValue();
            }
		}
        private bool skipFirstLeftCanStackStack0Check = true;

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
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => IsEnchantable(entity);
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
                    $"In EnchantedItem, Failed to Clone(item: {item.S()}, itemClone: {itemClone.S()}), cloneReforgedItem: {cloneReforgedItem.S()}, resetGlobals: {resetGlobals.S()}.".LogNT(ChatMessagesIDs.CloneFailGetEnchantedItem);
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

            #region Enchantments

            if (resetGlobals) {
                for (int i = 0; i < enchantments.Length; i++)
                    clone.enchantments[i] = new Item();
            }
            else {
                //fixes enchantments being applied to all of an item instead of just the instance
                for (int i = 0; i < enchantments.Length; i++)
                    clone.enchantments[i] = enchantments[i].Clone();
            }

            #endregion


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

			Experience = reader.ReadInt32();
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
        public static EItemType GetEItemType(Item item) {
            EItemType itemType = EItemType.None;
            if (IsWeaponItem(item)) {
                itemType = EItemType.Weapon;
            }
            else if (IsArmorItem(item)) {
                itemType = EItemType.Armor;
            }
            else if (IsAccessoryItem(item)) {
                itemType = EItemType.Accessory;
            }
            return itemType;
        }
        public EItemType GetEItemType() {
            return GetEItemType(Item);
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
            WEPlayer wePlayer = Main.LocalPlayer.GetWEPlayer();

            //Stack0
            if (Modified || inEnchantingTable) {
                if (Stack0) {
                    string tooltip = $"♦ OUT OF AMMO ♦";
                    tooltips.Add(new TooltipLine(Mod, "stack0", tooltip) { OverrideColor = Color.Yellow });
                }
            }

            //Xp and level Tooltips
            if (Enchanted || inEnchantingTable) {

                //~Damage tooltip
                if (WEMod.clientConfig.DisplayApproximateWeaponDamageTooltip) {
                    float damageMultiplier = item.ApplyEStat("Damage", 1f);
                    if (damageMultiplier > 1f) {
                        int damage = (int)Math.Round(wePlayer.Player.GetWeaponDamage(item, true) * damageMultiplier);
                        string tooltip = $"Item Damage ~ {damage} (Against 0 armor enemy)";
                        tooltips.Add(new TooltipLine(Mod, "level", tooltip) { OverrideColor = Color.DarkRed });
                    }
                }


                string pointsName = WEMod.clientConfig.UsePointsAsTooltip ? "Points" : "Enchantment Capacity";

                string levelTooltip = $"Level: {levelBeforeBooster}  {pointsName} available: {GetLevelsAvailable()}";
                if (PowerBoosterInstalled) {
                    levelTooltip += " (Booster Installed)";
                }

                tooltips.Add(new TooltipLine(Mod, "level", levelTooltip) { OverrideColor = Color.LightGreen });

                //Experience tooltip
                string experienceTooltip = $"Experience: {Experience}";
                if (levelBeforeBooster < MAX_LEVEL) {
                    experienceTooltip += $" ({WEModSystem.levelXps[levelBeforeBooster] - Experience} to next level)";
                }
                else {
                    experienceTooltip += " (Max Level)";
                }

                tooltips.Add(new TooltipLine(Mod, "experience", experienceTooltip) { OverrideColor = Color.White });
            }

            //infusionTooltip
            if (infusedItemName != "") {
                string tooltip = "";
                if (IsWeaponItem(item)) {
                    tooltip = $"Infusion Power: {infusionPower}   Infused Item: {infusedItemName}";
                } 
                else if (IsArmorItem(item)) {
                    tooltip = $"Infused Armor ID: {item.GetInfusionArmorSlot()}   Infused Item: {infusedItemName}";
                }

                if (tooltip != "")
                    tooltips.Add(new TooltipLine(Mod, "infusionTooltip", tooltip) { OverrideColor = Color.DarkRed });
            }
            else if (wePlayer.usingEnchantingTable || WEMod.clientConfig.AlwaysDisplayInfusionPower) {
                string tooltip = "";
                if (IsWeaponItem(item)) {
                    tooltip = $"Infusion Power: {item.GetWeaponInfusionPower()}";
                }
                else if (IsArmorItem(item)) {
                    tooltip = $"Set Bonus ID: {item.GetInfusionArmorSlot(true)}";
                }

                if (tooltip != "")
                    tooltips.Add(new TooltipLine(Mod, "infusionTooltip", tooltip) { OverrideColor = Color.DarkRed });
            }

            //newInfusionTooltip
            if (inEnchantingTable && wePlayer.infusionConsumeItem != null) {
                if(IsWeaponItem(item) && IsWeaponItem(wePlayer.infusionConsumeItem)) {
                    string tooltip = $"*New Infusion Power: {wePlayer.infusionConsumeItem.GetWeaponInfusionPower()}   " +
						             $"New Infused Item: {wePlayer.infusionConsumeItem.GetInfusionItemName()}*";

                    tooltips.Add(new TooltipLine(Mod, "newInfusionTooltip", tooltip) { OverrideColor = Color.DarkRed });
                }
                else if (IsArmorItem(item) && IsArmorItem(wePlayer.infusionConsumeItem)) {
                    string tooltip = $"*New Set Bonus ID: {wePlayer.infusionConsumeItem.GetInfusionArmorSlot()}   " +
						             $"New Infused Item: {wePlayer.infusionConsumeItem.GetInfusionItemName()}*";

                    tooltips.Add(new TooltipLine(Mod, "newInfusionTooltip", tooltip) { OverrideColor = Color.DarkRed });
                }
            }

            IEnumerable<Enchantment> enchantmentModItems = enchantments
                .Where(i => !i.IsAir && i.ModItem is Enchantment)
                .Select(i => (Enchantment)i.ModItem);

            EItemType itemType = GetEItemType();

            foreach (Enchantment enchantment in enchantmentModItems) {
                float effectiveness = enchantment.AllowedList[itemType];
                var effectTooltips = enchantment.GetEffectsTooltips();
                tooltips.Add(new TooltipLine(Mod, $"enchantment:{enchantment.Name}", $"{enchantment.EnchantmentTypeName} ({effectiveness.Percent()}%):") { OverrideColor = Color.Violet });
                foreach (var tooltipTuple in effectTooltips) {
                    tooltips.Add(new TooltipLine(Mod, $"effects:{enchantment.Name}", $"• {tooltipTuple.Item1}") { OverrideColor = tooltipTuple.Item2 });
                }
            }
        }
		public override void ModifyWeaponCrit(Item item, Player player, ref float crit) {
			if (!WEMod.serverConfig.CritPerLevelDisabled) {
                float multiplier = GlobalEnchantmentStrengthMultiplier;
                crit += levelBeforeBooster * multiplier;
            }
		}
        public void GainXP(Item item, int xpInt, bool noMessage = false)
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();

            int currentLevel = levelBeforeBooster;

            //xp < 0 return
            if (xpInt < 0) {
                $"Prevented your {item.S()} from loosing experience due to a calculation error.".LogNT(ChatMessagesIDs.GainXPPreventedLoosingExperience);

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

            //True means it will consume ammo
            return rand > ammoSaveChance;
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

            //Consumable weapons  (item.placeStyle fix for a placable enchantable item)
            if(item.consumable && Modified && item.placeStyle == 0) {
                
                //Ammo Cost
                if(_stack < 0 || item.stack == Stack) {
                    float rand = Main.rand.NextFloat();
                    float ammoSaveChance = -1f * item.ApplyEStat("AmmoCost", 0f);

                    if (rand <= ammoSaveChance)
                        item.stack++;
                }

                //Restock and Stack0
                if (item.stack < 2) {
                    Restock(item);

                    if (item.stack < 2) {
                        Stack0 = true;
                        item.stack = 2;
                    }
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
        public override void RightClick(Item item, Player player) {
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
                if(reforgeItem.TryGetEnchantedItem(out EnchantedItem iGlobal)) {
                    foreach (Item enchantment in iGlobal.enchantments) {
                        s += enchantment.S();
                    }
                    s.Log();
                }

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
            if (WEMod.calamityEnabled)
                calamityReforged = true;

			//Calamity deletes global data after reforge, so it is handled on the next tick in WEModSystem.
			if (!WEMod.calamityEnabled) {
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
            item2.TryGetEnchantedItem(out EnchantedItem i2Global);

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

            //Splitting stack with right click
            if (item1.type == Main.mouseItem.type && item1.stack == Main.mouseItem.stack && Main.mouseRight && item2.stack > 1)
                return true;

            //Combine item2 into item1
            List<Item> list = new List<Item>();
            list.Add(item2);
            skipUpdateValue = true;
            item1.CombineEnchantedItems(ref list);
            skipUpdateValue = false;

            //Stack0
            if (Stack0) {
                item1.stack--;
            }
            else if (i2Global.Stack0) {
				if (!WEModSystem.ShiftDown) {
                    item2.stack--;
                }
                else if (Main.mouseLeft) {
                    //Bug fix for both item1 and item2 having Stack0
					if (skipFirstLeftCanStackStack0Check) {
                        skipFirstLeftCanStackStack0Check = false;
                    }
					else {
                        item1.stack--;
                        skipFirstLeftCanStackStack0Check = true;
                    }
				}
            }

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
                if(tempItem.TryGetEnchantedItem(out EnchantedItem tempGlobal))
                    tempGlobal.Clone(tempItem, item2);

                resetGlobals = false;
            }

            return true;
        }
	}

    public static class EnchantedItemStaticMethods {
        public static bool IsEnchantable(Item item) {
            if (IsWeaponItem(item) || IsArmorItem(item) || IsAccessoryItem(item)) {
                return true;
            }
            else {
                return false;
            }
        }
        public static bool IsWeaponItem(Item item) {
            if (item == null || item.IsAir)
                return false;

            bool isWeapon;
            switch (item.type) {
                case ItemID.CoinGun:
                    isWeapon = true;
                    break;
                default:
                    isWeapon = item.damage > 0 && item.ammo == 0;
                    break;
            }

            return isWeapon && !item.accessory;
        }
        public static bool IsArmorItem(Item item) {
            if (item == null || item.IsAir)
                return false;

            return !item.vanity && (item.headSlot > -1 || item.bodySlot > -1 || item.legSlot > -1);
        }
        public static bool IsAccessoryItem(Item item) {
            if (item == null || item.IsAir)
                return false;

            //Check for armor item is a fix for Reforgable armor mod setting armor to accessories
            return item.accessory && !IsArmorItem(item);
        }
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
        public static void ApplyEnchantment(int i) {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            Item item = wePlayer.enchantingTableUI.itemSlotUI[0].Item;
            item.ApplyEnchantment(i);
        }
        public static void ApplyEnchantment(this Item item, int i) {

			#region Debug

			if (LogMethods.debugging) ($"\\/ApplyEnchantment(i: " + i + ")").Log();

			#endregion

			WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            if (item.TryGetEnchantedItem(out EnchantedItem iGlobal)) {
                Enchantment enchantment = (Enchantment)(iGlobal.enchantments[i].ModItem);
                item.UpdateEnchantment(ref enchantment, i);
                wePlayer.UpdateItemStats(ref item);
            }

			#region Debug

			if (LogMethods.debugging) ($"/\\ApplyEnchantment(i: " + i + ")").Log();

			#endregion
		}
		public static void RemoveEnchantment(int i) {

			#region Debug

			if (LogMethods.debugging) ($"\\/RemoveEnchantment(i: " + i + ")").Log();

			#endregion

			WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            Item item = wePlayer.enchantingTableUI.itemSlotUI[0].Item;
            if (item.TryGetEnchantedItem(out EnchantedItem iGlobal)) {
                Enchantment enchantment = (Enchantment)(iGlobal.enchantments[i].ModItem);
                item.UpdateEnchantment(ref enchantment, i, true);
                wePlayer.UpdateItemStats(ref item);
            }

            #region Debug

            if (LogMethods.debugging) ($"/\\RemoveEnchantment(i: " + i + ")").Log();

            #endregion
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
                    iGlobal.statModifiers.Add(staticStat.Name, statModifier);
                }
                else {
                    iGlobal.statModifiers[staticStat.Name] = iGlobal.statModifiers[staticStat.Name].CombineWith(statModifier);
                }
            }

            //New Damage Type
            if (enchantment.NewDamageType > -1) {
                if (remove) {
                    item.DamageType = ContentSamples.ItemsByType[item.type].DamageType;

                    iGlobal.damageType = -1;
                }
                else {
                    iGlobal.damageType = enchantment.NewDamageType;

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
            if (xpDamage <= 0)
                return;

            //Low damage per hit xp boost
            float lowDamagePerHitXPBoost;
            if (item != null && target.defense > 0) {
                //Remove 2x from crit
                float effectiveDamagePerHit = actualDamage;
                if (crit)
                    effectiveDamagePerHit /= 2f;

                //Apply affective crit multiplier
                float critMultiplier = 1f + (player.GetWeaponCrit(item) % 100) / 100f;
                effectiveDamagePerHit *= critMultiplier;

                float effectiveBaseDamagePerHit = effectiveDamagePerHit + actualDefence;

                lowDamagePerHitXPBoost = effectiveBaseDamagePerHit / effectiveDamagePerHit;
            }
            else {
                lowDamagePerHitXPBoost = 1f;
            }

            if(lowDamagePerHitXPBoost < 1f) {
                ($"Prevented an issue that would cause your xp do be reduced.  (xpInt < 0) item: {item.S()}, target: {target.S()}, damage: {damage}, crit: {crit.S()}, " +
                    $"melee: {melee.S()}, Main.GameMode: {Main.GameMode},\n" +
					$"target.defense: {target.defense}, armorPenetration: {armorPenetration} xpDamage: {xpDamage}, lowDamagePerHitXPBoost: {lowDamagePerHitXPBoost}, actualDefence: {actualDefence}, actualDamage: {actualDamage}").LogNT(ChatMessagesIDs.LowDamagePerHitXPBoost);
                lowDamagePerHitXPBoost = 1f;
			}

            //Low Damage help multiplier
            float xp = xpDamage * lowDamagePerHitXPBoost * experienceMultiplier;

            //Reduction Factor (Life Max)
            float reductionFactor = GetReductionFactor(target.lifeMax);
            xp /= reductionFactor;

            //XP <= 0 check
            int xpInt = (int)Math.Round(xp);
            if (xpInt <= 0) {
                xpInt = 1;
            }
            else if (xpInt < 0) {
                ($"Prevented an issue that would cause you to loose experience. (xpInt < 0) item: {item.S()}, target: {target.S()}, damage: {damage}, crit: {crit.S()}, " +
                    $"melee: {melee.S()}, Main.GameMode: {Main.GameMode}, xpDamage: {xpDamage}, xpInt: {xpInt}, lowDamagePerHitXPBoost: {lowDamagePerHitXPBoost}, " +
                    $"actualDefence: {actualDefence}, actualDamage: {actualDamage}").LogNT(ChatMessagesIDs.DamageNPCPreventLoosingXP2);
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
                    if (IsArmorItem(armor)) {
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

                if (!consumedItem.TryGetEnchantedItem(out EnchantedItem cGlobal))
                    continue;

                if (!cGlobal.Modified)
                    continue;

                if (item.TryGetEnchantedItem(out EnchantedItem iGlobal)) {
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
                                    if (enchantment.Utility && iGlobal.enchantments[4].IsAir && (IsWeaponItem(item) || IsArmorItem(item))) {
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
        public static void CheckRemoveEnchantments(this Item item, Player player) {
            if (!item.TryGetEnchantedItem(out EnchantedItem iGlobal))
                return;

            //Check config
            for (int i = EnchantingTable.maxEnchantments - 1; i >= 0 ; i--) {
                Item enchantment = iGlobal.enchantments[i];
                if (enchantment.IsAir)
                    continue;

                bool slotAllowedByConfig = WEUIItemSlot.SlotAllowedByConfig(item, i);
                if (!slotAllowedByConfig) {
                    RemoveEnchantmentNoUpdate(ref iGlobal.enchantments[i], player, $"Slot {i} disabled by config.  Removed {enchantment.Name} from your {item.Name}.");
                }
            }

            //Check enchantment limitations
            List<string> enchantmentTypeNames = new List<string>();
            bool unique = false;
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++) {
                Item enchantmentItem = iGlobal.enchantments[i];

                if (enchantmentItem != null && !enchantmentItem.IsAir && player != null) {
                    Enchantment enchantment = (Enchantment)enchantmentItem.ModItem;
                    if (IsWeaponItem(item) && !enchantment.AllowedList.ContainsKey(EItemType.Weapon)) {
                        RemoveEnchantmentNoUpdate(ref iGlobal.enchantments[i], player, enchantmentItem.Name + " is no longer allowed on weapons and has been removed from your " + item.Name + ".");
                    }
                    else if (IsArmorItem(item) && !enchantment.AllowedList.ContainsKey(EItemType.Armor)) {
                        RemoveEnchantmentNoUpdate(ref iGlobal.enchantments[i], player, enchantmentItem.Name + " is no longer allowed on armor and has been removed from your " + item.Name + ".");
                    }
                    else if (IsAccessoryItem(item) && !enchantment.AllowedList.ContainsKey(EItemType.Accessory)) {
                        RemoveEnchantmentNoUpdate(ref iGlobal.enchantments[i], player, enchantmentItem.Name + " is no longer allowed on acessories and has been removed from your " + item.Name + ".");
                    }

                    if (i == EnchantingTable.maxEnchantments - 1 && !enchantment.Utility)
                        RemoveEnchantmentNoUpdate(ref iGlobal.enchantments[i], player, enchantmentItem.Name + " is no longer a utility enchantment and has been removed from your " + item.Name + ".");

                    if (enchantment.RestrictedClass > -1 && ContentSamples.ItemsByType[item.type].DamageType.Type == enchantment.RestrictedClass)
                        RemoveEnchantmentNoUpdate(ref iGlobal.enchantments[i], player, enchantmentItem.Name + $" is no longer allowed on {item.DamageType.Name} weapons and has removed from your " + item.Name + ".");

                    if (enchantment.Max1 && enchantmentTypeNames.Contains(enchantment.EnchantmentTypeName))
                        RemoveEnchantmentNoUpdate(ref iGlobal.enchantments[i], player, enchantment.EnchantmentTypeName + $" Enchantments are now limmited to 1 per item.  {enchantmentItem.Name} has been removed from your " + item.Name + ".");

                    if (enchantment.Unique) {
                        if (unique) {
                            RemoveEnchantmentNoUpdate(ref iGlobal.enchantments[i], player, enchantment.EnchantmentTypeName + $" Detected multiple uniques on your {item.Name}.  {enchantmentItem.Name} has been removed from your " + item.Name + ".");
                        }
                        else {
                            unique = true;
                        }
                    }

                    enchantmentTypeNames.Add(enchantment.EnchantmentTypeName);
                }
            }

            //Check too many enchantments on item
            if (iGlobal.GetLevelsAvailable() < 0) {
                for (int k = EnchantingTable.maxEnchantments - 1; k >= 0 && iGlobal.GetLevelsAvailable() < 0; k--) {
                    if (!iGlobal.enchantments[k].IsAir) {
                        iGlobal.enchantments[k] = player.GetItem(player.whoAmI, iGlobal.enchantments[k], GetItemSettings.LootAllSettings);
                    }
                    if (!iGlobal.enchantments[k].IsAir) {
                        player.QuickSpawnItem(player.GetSource_Misc("PlayerDropItemCheck"), iGlobal.enchantments[k]);
                        iGlobal.enchantments[k] = new Item();
                    }
                }

                Main.NewText("Your " + item.Name + "' level is too low to use that many enchantments.");
            }

            
            




        }
        private static void RemoveEnchantmentNoUpdate(ref Item enchantmentItem, Player player, string msg) {
            enchantmentItem = player.GetItem(player.whoAmI, enchantmentItem, GetItemSettings.LootAllSettings);
            if (!enchantmentItem.IsAir)
                player.QuickSpawnItem(player.GetSource_Misc("PlayerDropItemCheck"), enchantmentItem);

            enchantmentItem = new Item();
            Main.NewText(msg);
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
            if (IsWeaponItem(item)) {
                if (enchantment.AllowedList.ContainsKey(EItemType.Weapon)) {
                    add *= enchantment.AllowedList[EItemType.Weapon];
                    mult = 1f + (mult - 1f) * enchantment.AllowedList[EItemType.Weapon];
                    flat *= enchantment.AllowedList[EItemType.Weapon];
                    @base *= enchantment.AllowedList[EItemType.Weapon];
                    return;
                }
                else {
                    add = 1f;
                    mult = 1f;
                    flat = 0f;
                    @base = 0f;
                }
            }
            if (IsArmorItem(item)) {
                if (enchantment.AllowedList.ContainsKey(EItemType.Armor)) {
                    add *= enchantment.AllowedList[EItemType.Armor];
                    mult = 1f + (mult - 1f) * enchantment.AllowedList[EItemType.Armor];
                    flat *= enchantment.AllowedList[EItemType.Armor];
                    @base *= enchantment.AllowedList[EItemType.Armor];
                    return;
                }
                else {
                    add = 1f;
                    mult = 1f;
                    flat = 0f;
                    @base = 0f;
                }
            }
            if (IsAccessoryItem(item)) {
                if (enchantment.AllowedList.ContainsKey(EItemType.Accessory)) {
                    add *= enchantment.AllowedList[EItemType.Accessory];
                    mult = 1f + (mult - 1f) * enchantment.AllowedList[EItemType.Accessory];
                    flat *= enchantment.AllowedList[EItemType.Accessory];
                    @base *= enchantment.AllowedList[EItemType.Accessory];
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
