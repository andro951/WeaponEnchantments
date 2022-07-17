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
using Terraria.UI;
using WeaponEnchantments.Items;
using WeaponEnchantments.UI;

namespace WeaponEnchantments.Common.Globals
{
    public class EnchantedItem : GlobalItem
    {
        public static Item reforgeItem = null;
        public static int newPrefix = 0;
        public static bool cloneReforgedItem = false;
        public static bool resetGlobals = false;
        //Start Packet fields
        public int experience;//current experience of a weapon/armor/accessory item
        public Item[] enchantments = new Item[EnchantingTable.maxEnchantments];//Track enchantment items on a weapon/armor/accessory item
        //End Packet fields

        public Dictionary<string, StatModifier> statModifiers;
        public Dictionary<string, StatModifier> appliedStatModifiers;
        public Dictionary<string, StatModifier> eStats;
        public Dictionary<string, StatModifier> appliedEStats;
        public Dictionary<int, int> buffs;
        public Dictionary<int, int> debuffs;
        public Dictionary<int, int> onHitBuffs;

        public string infusedItemName = "";
        public int infusedPower = 0;
        public float damageMultiplier = 1f;
        public int infusedArmorSlot = -1;
        public int infusionValueAdded = 0;
        public int damageType = -1;
        public int lastValueBonus;
        public int levelBeforeBooster;
        public int level;
        public bool powerBoosterInstalled;//Tracks if Power Booster is installed on item +10 levels to spend on enchantments (Does not affect experience)
        public bool inEnchantingTable;
        public bool equip = false;
        public bool trackedWeapon = false;
        public bool hoverItem = false;
        public bool trashItem = false;
        public bool favorited = false;
        public const int maxLevel = 40;
        public int prefix;
        public bool normalReforge = false;
        public Projectile masterProjectile = null;
        public bool stolen;
        public bool? needsClear = null;
        public static List<int> needClearWhoAmIs = new();
        public bool stack0 = false;
        public bool needsUpdateOldItems = false;
        public EnchantedItem()
        {
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++) 
            {
                enchantments[i] = new Item();
            }
            statModifiers = new Dictionary<string, StatModifier>();
            appliedStatModifiers = new Dictionary<string, StatModifier>();
            appliedEStats = new Dictionary<string, StatModifier>();
            eStats = new Dictionary<string, StatModifier>();
            buffs = new Dictionary<int, int>();
            debuffs = new Dictionary<int, int>();
            onHitBuffs = new Dictionary<int, int>();
        }//Constructor
        public override bool InstancePerEntity => true;
        /*public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return WEMod.IsEnchantable(entity);
        }*/
        public override GlobalItem Clone(Item item, Item itemClone)
        {
            EnchantedItem clone;
            if (cloneReforgedItem || resetGlobals)
            {
                clone = itemClone.G();
                cloneReforgedItem = false;
                clone.experience = experience;
                clone.infusedItemName = infusedItemName;
                clone.infusedPower = infusedPower;
                clone.damageMultiplier = damageMultiplier;
                clone.infusionValueAdded = infusionValueAdded;
                clone.lastValueBonus = lastValueBonus;
                clone.levelBeforeBooster = levelBeforeBooster;
                clone.level = level;
                clone.powerBoosterInstalled = powerBoosterInstalled;
                //clone.prefix = prefix;
            }
            else
            {
                clone = (EnchantedItem)base.Clone(item, itemClone);
                clone.appliedStatModifiers = new Dictionary<string, StatModifier>(appliedStatModifiers);
                clone.appliedEStats = new Dictionary<string, StatModifier>(appliedEStats);
            }
			if (resetGlobals)
			{
                for (int i = 0; i < enchantments.Length; i++)
                    clone.enchantments[i] = new Item();
			}
			else
			{
                clone.enchantments = (Item[])enchantments.Clone();
                for (int i = 0; i < enchantments.Length; i++)
                {
                    clone.enchantments[i] = enchantments[i].Clone();
                    Item enchantment = clone.enchantments[i];
                }//fixes enchantments being applied to all of an item instead of just the instance
            }
            clone.statModifiers = new Dictionary<string, StatModifier>(statModifiers);
            clone.eStats = new Dictionary<string, StatModifier>(eStats);
            clone.buffs = new Dictionary<int, int>(buffs);
            clone.debuffs = new Dictionary<int, int>(debuffs);
            clone.onHitBuffs = new Dictionary<int, int>(onHitBuffs);
            clone.equip = false;
            clone.prefix = prefix;
            clone.stolen = stolen;
            if(!Main.mouseItem.IsSameEnchantedItem(itemClone))
                clone.trackedWeapon = false;
            return clone;
        }
		public override void UpdateEquip(Item item, Player player)
		{
            if (inEnchantingTable && (Main.LocalPlayer.G().enchantingTableUI?.itemSlotUI?[0]?.Item != null || Main.LocalPlayer.G().enchantingTableUI.itemSlotUI[0].Item.IsAir))
            {
                inEnchantingTable = false;
                if (item.GetInfusionArmorSlot() != infusedArmorSlot)
                {
                    infusedArmorSlot = -1;
                    item.TryInfuseItem(new Item(), true);
                }
            }
        }
		public override bool OnPickup(Item item, Player player)
        {
            //experience = int.MaxValue - 5;
            player.G().UpdateItemStats(ref item);
            return true;
        }
        public override void NetSend(Item item, BinaryWriter writer)
        {
            if (WEMod.IsEnchantable(item))
            {
                if(UtilityMethods.debugging) ($"\\/NetSend(" + item.Name + ")").Log();
                if(UtilityMethods.debugging) ($"eStats.Count: " + eStats.Count + ", statModifiers.Count: " + statModifiers.Count).Log();
                writer.Write(experience);
                writer.Write(powerBoosterInstalled);
                bool noName = infusedItemName == "";
                writer.Write(noName);
                if(!noName)
                    writer.Write(infusedItemName);
                for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
                {
                    writer.Write((short)enchantments[i].type);
                    if(UtilityMethods.debugging) ($"e " + i + ": " + enchantments[i].Name).Log();
                }
                short count = (short)eStats.Count;
                writer.Write(count);
                foreach(string key in eStats.Keys)
                {
                    writer.Write(key);
                    writer.Write(eStats[key].Additive);
                    writer.Write(eStats[key].Multiplicative);
                    writer.Write(eStats[key].Base);
                    writer.Write(eStats[key].Flat);
                }
                count = (short)statModifiers.Count;
                writer.Write(count);
                foreach (string key in statModifiers.Keys)
                {
                    writer.Write(key);
                    writer.Write(statModifiers[key].Additive);
                    writer.Write(statModifiers[key].Multiplicative);
                    writer.Write(statModifiers[key].Flat);
                    writer.Write(statModifiers[key].Base);
                }
                count = (short)buffs.Count;
                writer.Write(count);
                foreach(int key in buffs.Keys)
                {
                    writer.Write(key);
                    writer.Write(buffs[key]);
                }
                count = (short)debuffs.Count;
                writer.Write(count);
                foreach (int key in debuffs.Keys)
                {
                    writer.Write(key);
                    writer.Write(debuffs[key]);
                }
                count = (short)onHitBuffs.Count;
                writer.Write(count);
                foreach (int key in onHitBuffs.Keys)
                {
                    writer.Write(key);
                    writer.Write(onHitBuffs[key]);
                }
                writer.Write((short)damageType);
                if (UtilityMethods.debugging) ($"eStats.Count: " + eStats.Count + ", statModifiers.Count: " + statModifiers.Count).Log();
                if(UtilityMethods.debugging) ($"/\\NetSend(" + item.Name + ")").Log();
            }
        }
        public override void NetReceive(Item item, BinaryReader reader)
        {
            if (WEMod.IsEnchantable(item))
            {
                if(UtilityMethods.debugging) ($"\\/NetRecieve(" + item.Name + ")").Log();
                if(UtilityMethods.debugging) ($"eStats.Count: " + eStats.Count + ", statModifiers.Count: " + statModifiers.Count).Log();
                experience = reader.ReadInt32();
                powerBoosterInstalled = reader.ReadBoolean();
                bool noName = reader.ReadBoolean();
                if (!noName)
                    infusedItemName = reader.ReadString();
                else
                    infusedItemName = "";
                item.TryGetGlotalItemStats(infusedItemName, out infusedPower, out damageMultiplier, out infusedArmorSlot);
                for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
                {
                    enchantments[i] = new Item(reader.ReadUInt16());
                    if(UtilityMethods.debugging) ($"e " + i + ": " + enchantments[i].Name).Log();
                }
                eStats.Clear();
                int count = reader.ReadUInt16();
                for (int i = 0; i < count; i++)
                {
                    string key = reader.ReadString();
                    float additive = reader.ReadSingle();
                    float multiplicative = reader.ReadSingle();
                    float flat = reader.ReadSingle();
                    float @base = reader.ReadSingle();
                    eStats.Add(key, new StatModifier(additive, multiplicative, flat, @base));
                }
                statModifiers.Clear();
                count = reader.ReadUInt16();
                for (int i = 0; i < count; i++)
                {
                    string key = reader.ReadString();
                    float additive = reader.ReadSingle();
                    float multiplicative = reader.ReadSingle();
                    float flat = reader.ReadSingle();
                    float @base = reader.ReadSingle();
                    statModifiers.Add(key, new StatModifier(additive, multiplicative, flat, @base));
                }
                count = reader.ReadUInt16();
                for(int i = 0; i < count; i++)
                {
                    int key = reader.ReadInt32();
                    int value = reader.ReadInt32();
                    buffs.Add(key, value);
                }
                count = reader.ReadUInt16();
                for (int i = 0; i < count; i++)
                {
                    int key = reader.ReadInt32();
                    int value = reader.ReadInt32();
                    debuffs.Add(key, value);
                }
                count = reader.ReadUInt16();
                for (int i = 0; i < count; i++)
                {
                    int key = reader.ReadInt32();
                    int value = reader.ReadInt32();
                    onHitBuffs.Add(key, value);
                }
                damageType = reader.ReadUInt16();
                if(damageType > -1)
                    item.UpdateDamageType(damageType);
                if (UtilityMethods.debugging) ($"eStats.Count: " + eStats.Count + ", statModifiers.Count: " + statModifiers.Count).Log();
                if(UtilityMethods.debugging) ($"/\\NetRecieve(" + item.Name + ")").Log();
            }
        }
        public override void UpdateInventory(Item item, Player player)
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            if(experience > 0 || powerBoosterInstalled || infusedItemName != "")
            {
                int value = 0;
                for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
                {
                    value += enchantments[i].value;
                }
                int powerBoosterValue = powerBoosterInstalled ? ModContent.GetModItem(ModContent.ItemType<PowerBooster>()).Item.value : 0;
                int npcTalking = player.talkNPC != -1 ? Main.npc[player.talkNPC].type : -1;
                int valueToAdd = npcTalking != NPCID.GoblinTinkerer ? value + (int)(EnchantmentEssenceBasic.valuePerXP * experience) + powerBoosterValue + infusionValueAdded : 0;
                valueToAdd /= item.stack;
                item.value += valueToAdd - lastValueBonus;//Update items value based on enchantments installed
                lastValueBonus = valueToAdd;
            }
            equip = false;
            if (wePlayer.stickyFavorited)
            {
                if (item.favorited)
                {
                    if (!favorited && Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftAlt) || Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightAlt))
                    {
                        favorited = true;
                    }
                }//Sticky Favorited
                else
                {
                    if (favorited)
                    {
                        if (!(Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftAlt) || Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightAlt)))
                        {
                            item.favorited = true;
                        }
                        else
                        {
                            favorited = false;
                        }
                    }
                }//Sticky Favorited
            }
        }
        public void UpdateLevel()
        {
            int l;
            for(l = 0; l < maxLevel; l++)
            {
                if(experience < WEModSystem.levelXps[l])
                {
                    level = l + 1;
                    break;
                }
            }
            if (l == maxLevel)
            {
                levelBeforeBooster = maxLevel;
                level = powerBoosterInstalled ? maxLevel + 10 : maxLevel;
            }
            else
            {
                levelBeforeBooster = l;
                level = powerBoosterInstalled ? l + 10 : l;
            }
        }
        public int GetLevelsAvailable()
        {
            UpdateLevel();
            int total = 0;
            for (int i = 0; i < enchantments.Length; i++)
            {
                if (enchantments[i] != null && !enchantments[i].IsAir)
                {
                    //if(UtilityMethods.debugging) ($"enchantments[" + i + "]: name: " + enchantments[i].Name + " type: " + enchantments[i].type).Log();
                    AllForOneEnchantmentBasic enchantment = ((AllForOneEnchantmentBasic)enchantments[i].ModItem);
                    total += enchantment.GetLevelCost();
                }
            }
            return level - total;
        }
        public override void LoadData(Item item, TagCompound tag)
        {
            if (WEMod.IsEnchantable(item))
            {
                if(UtilityMethods.debugging) ($"\\/LoadData(" + item.Name + ")").Log();
                experience = tag.Get<int>("experience");//Load experience tag
                if (experience < 0)
                    experience = int.MaxValue;
                powerBoosterInstalled = tag.Get<bool>("powerBooster");//Load status of powerBoosterInstalled
                infusedItemName = tag.Get<string>("infusedItemName");
                infusedPower = tag.Get<int>("infusedPower");
                damageMultiplier = tag.Get<float>("damageMultiplier");
                stack0 = tag.Get<bool>("stack0");
                if (damageMultiplier == 0f)
                    damageMultiplier = 1f;
                item.UpdateInfusionDamage(damageMultiplier, false);
                UpdateLevel();
                for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
                {
                    if (tag.Get<Item>("enchantments" + i.ToString()) != null)
                    {
                        if (!tag.Get<Item>("enchantments" + i.ToString()).IsAir)
                        {
                            enchantments[i] = tag.Get<Item>("enchantments" + i.ToString()).Clone();
                            OldItemManager.ReplaceOldItem(ref enchantments[i]);
                            if(UtilityMethods.debugging) ($"e " + i + ": " + enchantments[i].Name).Log();
                        }
                        else
                        {
                            enchantments[i] = new Item();
                        }
                    }
                }//Load enchantment item tags
                if(UtilityMethods.debugging) ($"/\\LoadData(" + item.Name + ")").Log();
            }
        }
        public override void SaveData(Item item, TagCompound tag)
        {
            if (enchantments != null)
            {
                for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
                {
                    if (enchantments[i] != null)
                    {
                        if (!enchantments[i].IsAir)
                        {
                            tag["enchantments" + i.ToString()] = enchantments[i].Clone();
                        }
                    }
                }//Save enchantment item tags
            }
            tag["experience"] = experience;//Save experience tag
            tag["powerBooster"] = powerBoosterInstalled;//save status of powerBoosterInstalled
            tag["infusedItemName"] = infusedItemName;
            tag["infusedPower"] = infusedPower;
            tag["damageMultiplier"] = damageMultiplier;
            tag["stack0"] = stack0;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            WEPlayer wePlayer = Main.LocalPlayer.G();
            bool enchantmentsToolTipAdded = false;
            bool enchantemntInstalled = false;
            UpdateLevel();
			for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
			{
				if (!enchantments[i].IsAir)
				{
					enchantemntInstalled = true;
					break;
				}
			}
			if (experience > 0 || powerBoosterInstalled || inEnchantingTable || enchantemntInstalled)
            {
                if (stack0)
                    tooltips.Add(new TooltipLine(Mod, "stack0", $"!!!OUT OF AMMO!!!") { OverrideColor = Color.Yellow });
                string pointsName = WEMod.clientConfig.UsePointsAsTooltip ? "Points" : "Enchantment Capacity";
                if (powerBoosterInstalled)
                    tooltips.Add(new TooltipLine(Mod, "level", $"Level: {levelBeforeBooster}  {pointsName} available: {GetLevelsAvailable()} (Booster Installed)") { OverrideColor = Color.LightGreen });
                else
                    tooltips.Add(new TooltipLine(Mod, "level", $"Level: {levelBeforeBooster}  {pointsName} available: {GetLevelsAvailable()}") { OverrideColor = Color.LightGreen });
                string levelString = levelBeforeBooster < maxLevel ? $" ({WEModSystem.levelXps[levelBeforeBooster] - experience} to next level)" : " (Max Level)";
                tooltips.Add(new TooltipLine(Mod, "experience", $"Experience: {experience}{levelString}") { OverrideColor = Color.White });
            }
            if(infusedItemName != "")
            {
                string tooltip = "";
                if (WEMod.IsWeaponItem(item))
                    tooltip = $"Infusion Power: {infusedPower}   Infused Item: {infusedItemName}";
                else if (WEMod.IsArmorItem(item))
                    tooltip = $"Infused Armor ID: {item.GetInfusionArmorSlot()}   Infused Item: {infusedItemName}";
                tooltips.Add(new TooltipLine(Mod, "infusedItemTooltip", tooltip) { OverrideColor = Color.DarkRed });
            }
            else if (wePlayer.usingEnchantingTable || WEMod.clientConfig.AlwaysDisplayInfusionPower)
            {
                string tooltip = "";
                if (WEMod.IsWeaponItem(item))
                    tooltip = $"Infusion Power: {item.GetWeaponInfusionPower()}";
                else if (WEMod.IsArmorItem(item))
                    tooltip = $"Set Bonus ID: {item.GetInfusionArmorSlot(true)}";
                if (tooltip != "")
                tooltips.Add(new TooltipLine(Mod, "infusionPowerTooltip", tooltip) { OverrideColor = Color.DarkRed });
            }
            if(inEnchantingTable && wePlayer.infusionConsumeItem != null)
            {
                if(WEMod.IsWeaponItem(item) && WEMod.IsWeaponItem(wePlayer.infusionConsumeItem))
                    tooltips.Add(new TooltipLine(Mod, "newInfusedItemTooltip", $"*New Infusion Power: {wePlayer.infusionConsumeItem.GetWeaponInfusionPower()}   New Infused Item: {wePlayer.infusionConsumeItem.GetInfusionItemName()}*") { OverrideColor = Color.DarkRed });
                else if (WEMod.IsArmorItem(item) && WEMod.IsArmorItem(wePlayer.infusionConsumeItem))
                    tooltips.Add(new TooltipLine(Mod, "newInfusedItemTooltip", $"*New Set Bonus ID: {wePlayer.infusionConsumeItem.GetInfusionArmorSlot()}   New Infused Item: {wePlayer.infusionConsumeItem.GetInfusionItemName()}*") { OverrideColor = Color.DarkRed });
            }
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
            {
                if (!enchantments[i].IsAir)
                {
                    AllForOneEnchantmentBasic enchantment = ((AllForOneEnchantmentBasic)enchantments[i].ModItem);
                    if (!enchantmentsToolTipAdded)
                    {
                        tooltips.Add(new TooltipLine(Mod, "enchantmentsToolTip", "Enchantments:") { OverrideColor = Color.Violet});
                        enchantmentsToolTipAdded = true;
                    }//Enchantmenst: tooltip
                    string itemType = "";
                    if (WEMod.IsWeaponItem(item))
                        itemType = "Weapon";
                    else if (WEMod.IsArmorItem(item))
                        itemType = "Armor";
                    else if (WEMod.IsAccessoryItem(item))
                        itemType = "Accessory";
                    tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), enchantment.AllowedListTooltips[itemType])
                    {
                        OverrideColor = AllForOneEnchantmentBasic.rarityColors[enchantment.EnchantmentSize]
                    });
                }
            }//Edit Tooltips
        }
		public override void ModifyWeaponCrit(Item item, Player player, ref float crit)
		{
			if (!WEMod.serverConfig.CritPerLevelDisabled)
			{
                float multiplier;
                float linearMultiplier = WEMod.serverConfig.presetData.linearStrengthMultiplier / 100f;
                if (linearMultiplier != 1f)
                    multiplier = linearMultiplier;
                else
                    multiplier = WEMod.serverConfig.presetData.recomendedStrengthMultiplier / 100f;
                crit += (float)levelBeforeBooster * multiplier;
            }
		}
		public static void DamageNPC(Item item, Player player, NPC target, int damage, bool crit, bool melee = false)
        {
            if(UtilityMethods.debugging) ($"\\/DamageNPC").Log();
            target.GetGlobalNPC<WEGlobalNPC>().xpCalculated = true;
            float value;
            switch (Main.netMode)
            {
                case 1:
                    value = ContentSamples.NpcsByNetId[target.type].value;
                    break;
                default:
                    value = target.value;
                    break;
            }
            if (target.type != NPCID.TargetDummy && target.FullName != "Super Dummy" && !target.friendly && !target.townNPC && (value > 0 || !target.SpawnedFromStatue && target.lifeMax > 10))
            {
                int xpInt;
                int xpDamage;
                float multiplier;
                float effDamage;
                float effDamageDenom;
                float xp;
                multiplier = (1f + ((float)((target.noGravity ? 2f : 0f) + (target.noTileCollide ? 2f : 0f)) + 2f * (1f - target.knockBackResist)) / 10f) * (target.boss ? WEMod.serverConfig.BossExperienceMultiplier/400f : WEMod.serverConfig.ExperienceMultiplier/100f);
                effDamage = item != null ? (float)item.damage * (1f + (float)player.GetWeaponCrit(item) / 100f) : damage;
                float actualDefence = target.defense / 2f - (item != null ? target.checkArmorPenetration(player.GetWeaponArmorPenetration(item)) : 0f);
                float actualDamage = melee ? damage : damage - actualDefence;
                actualDamage = crit && !melee ? actualDamage * 2 : actualDamage;
                xpDamage = target.life < 0 ? (int)actualDamage + target.life : (int)actualDamage;
                if(xpDamage > 0)
                {
                    effDamageDenom = effDamage - actualDefence;
                    if (effDamageDenom > 1)
                        xp = (float)xpDamage * multiplier * effDamage / effDamageDenom;
                    else
                        xp = (float)xpDamage * multiplier * effDamage;
                    xp /= UtilityMethods.GetReductionFactor((int)target.lifeMax);
                    xpInt = (int)Math.Round(xp);
                    xpInt = xpInt > 1 ? xpInt : 1;
                    if (item != null && !item.IsAir && !item.consumable)
                    {
                        //ModContent.GetInstance<WEMod>().Logger.Info(wePlayer.Player.name + " recieved " + xpInt.ToString() + " xp from hitting " + target.FullName + ".");
                        //Main.NewText(wePlayer.Player.name + " recieved " + xpInt.ToString() + " xp from killing " + target.FullName + ".");
                        item.G().GainXP(item, xpInt);
                    }
                    AllArmorGainXp(xpInt);
                }
            }
            if (UtilityMethods.debugging) ($"/\\DamageNPC").Log();
        }
        public static void AllArmorGainXp(int xp)
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            int xpInt;
            int i = 0;
            foreach (Item armor in wePlayer.Player.armor)
            {
                if (i < 10)
                {
                    if (!armor.vanity && !armor.IsAir)
                    {
                        if (armor.GetGlobalItem<EnchantedItem>().levelBeforeBooster < maxLevel)
                        {
                            if (WEMod.IsArmorItem(armor))
                            {
                                xpInt = (int)Math.Round(xp / 2f);
                                xpInt = xpInt > 0 ? xpInt : 1;
                                armor.GetGlobalItem<EnchantedItem>().GainXP(armor, xpInt);
                            }
                            else
                            {
                                xpInt = (int)Math.Round(xp / 4f);
                                xpInt = xpInt > 0 ? xpInt : 1;
                                armor.GetGlobalItem<EnchantedItem>().GainXP(armor, xpInt);
                            }
                            //wePlayer.equiptArmor[i].GetGlobalItem<EnchantedItem>().GainXP(wePlayer.equiptArmor[i], xpInt);
                        }
                    }
                }
                i++;
            }
        }
        public void GainXP(Item item, int xpInt)
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            int currentLevel = levelBeforeBooster;
            if(xpInt < 0)
            {
                xpInt = 0;
                string errorMessage = $"Prevented your {item.S()} from loosing experience due to a calculation error.  Please report this to andro951 allong with a description of what you were doing at the time.";
                errorMessage.Log();
                Main.NewText(errorMessage);
            }
            experience += xpInt;
            if(experience < 0)
                experience = int.MaxValue;
            if (levelBeforeBooster < maxLevel)
            {
                UpdateLevel();
                if (levelBeforeBooster > currentLevel && wePlayer.usingEnchantingTable)
                {
                    if(levelBeforeBooster == 40)
                    {
                        SoundEngine.PlaySound(SoundID.Unlock);
                        //ModContent.GetInstance<WEMod>().Logger.Info("Congratulations!  " + wePlayer.Player.name + "'s " + item.Name + " reached the maximum level, " + levelBeforeBooster.ToString() + " (" + WEModSystem.levelXps[levelBeforeBooster - 1] + " xp).");
                        Main.NewText("Congratulations!  " + wePlayer.Player.name + "'s " + item.Name + " reached the maximum level, " + levelBeforeBooster.ToString() + " (" + WEModSystem.levelXps[levelBeforeBooster - 1] + " xp).");
                    }
                    else
                    {
                        //ModContent.GetInstance<WEMod>().Logger.Info(wePlayer.Player.name + "'s " + item.Name + " reached level " + levelBeforeBooster.ToString() + " (" + WEModSystem.levelXps[levelBeforeBooster - 1] + " xp).");
                        Main.NewText(wePlayer.Player.name + "'s " + item.Name + " reached level " + levelBeforeBooster.ToString() + " (" + WEModSystem.levelXps[levelBeforeBooster - 1] + " xp).");
                    }
                }
            }
        }
        public override bool CanConsumeAmmo(Item weapon, Item ammo, Player player)
        {
            /*float ammoCostBonus = 0f;
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
            {
                AllForOneEnchantmentBasic enchantment = ((AllForOneEnchantmentBasic)enchantments[i].ModItem);
                if (!enchantments[i].IsAir)
                {
                    float str = enchantment.EnchantmentStrength;
                    switch ((EnchantmentTypeID)enchantment.EnchantmentType)
                    {
                        case EnchantmentTypeID.AmmoCost:
                            ammoCostBonus += str;
                            break;
                    }
                }
            }*/
            return Main.rand.NextFloat() >= -1f * weapon.AEI("AmmoCost", 0f); //(eStats.ContainsKey("AmmoCost") ? eStats["AmmoCost"].ApplyTo(0f) : 0f);
        }
		public override bool? CanAutoReuseItem(Item item, Player player)
		{
            if (statModifiers.ContainsKey("P_autoReuse"))
                return false;
            else
                return null;
		}
        private void Restock(Item item)
		{
            Player player = Main.LocalPlayer;
            for (int i = 0; i < 59; i++)
            {
                Item inventoryItem = player.inventory[i];
                if (inventoryItem.type == item.type && inventoryItem.G().experience == 0 && !inventoryItem.G().powerBoosterInstalled && !inventoryItem.G().stack0)
                {
                    item.stack = inventoryItem.stack + 1;
                    stack0 = false;
                    player.inventory[i] = new Item();
                    return;
                }
            }
        }
		public override bool? UseItem(Item item, Player player)
        {
            WEPlayer wePlayer = player.GetModPlayer<WEPlayer>();
            //if (allForOne)
            if (eStats.ContainsKey("CatastrophicRelease"))
            {
                player.statMana = 0;
            }
            if(eStats.ContainsKey("AllForOne"))
            {
                wePlayer.allForOneTimer = (int)((float)item.useTime * item.AEI("NPCHitCooldown", 0.5f));
            }
            if(item.consumable && item.stack < 2 && (experience > 0 || powerBoosterInstalled || infusedItemName != "") && item.placeStyle == -1)
			{
                Restock(item);
                if(item.stack < 2)
                {
                    item.stack = 2;
                    stack0 = true;
                }
			}
            return null;
        }
        public override bool CanUseItem(Item item, Player player)
        {
			if (WEMod.IsEnchantable(item))
            {
                WEPlayer wePlayer = player.GetModPlayer<WEPlayer>();
                if (stack0)
				{
                    Restock(item);
					if (item.stack > 1)
					{
						item.stack--;
						return true;
					}
                    else
                        return false;
                }
                if (eStats.ContainsKey("CatastrophicRelease") && player.statManaMax != player.statMana)
                    return false;
                if (wePlayer.usingEnchantingTable && WeaponEnchantmentUI.preventItemUse)
                    return false;
                if (eStats.ContainsKey("AllForOne"))
                {
                    return wePlayer.allForOneTimer <= 0;
                }
            }
            return true;
        }
        public override bool CanRightClick(Item item)
        {
            if (item.stack > 1)
            {
                if (experience > 0 || powerBoosterInstalled || infusedItemName != "")
                {
                    if (Main.mouseItem.IsAir)
                    {
                        return true;
                    }
                }
            }//Prevent splitting stack of enchantable items with maxstack > 1
            return false;
        }
        public override void RightClick(Item item, Player player)
        {
            if (item.stack > 1)
            {
                if (experience > 0 || powerBoosterInstalled || infusedItemName != "")
                {
                    if (Main.mouseItem.IsAir)
                    {
                        //item.stack++;
                        RightClickStackableItem(item, player);
                    }
                }
            }//Prevent splitting stack of enchantable items with maxstack > 1
        }
        public void RightClickStackableItem(Item item, Player player)
		{
            if(item.stack > 1)
			{
                if(experience > 0 || powerBoosterInstalled || infusedItemName != "")
				{
					if (Main.mouseItem.IsAir)
					{
                        Main.mouseItem = new Item(item.type);
					}
					else if(Main.mouseItem.type == item.type)
					{
                        Main.mouseItem.stack++;
					}
				}
			}
		}
        public override void OnHitNPC(Item item, Player player, NPC target, int damage, float knockBack, bool crit)
        {
            DamageNPC(item, player, target, damage, crit, true);
        }
        public override void PostReforge(Item item)
        {
            if (UtilityMethods.debugging) ($"\\/PostReforge({item.S()})").Log();
            /*if (UtilityMethods.debugging)
            {
                string s = $"reforgeItem: {reforgeItem.S()}, prefix: {reforgeItem.prefix}, Enchantments: ";
                foreach (Item enchantment in reforgeItem.G().enchantments)
                {
                    s += enchantment.S();
                }
                s.Log();
                s = $"item: {item.S()}, prefix: {item.prefix}, Enchantments: ";
                foreach (Item enchantment in reforgeItem.G().enchantments)
                {
                    s += enchantment.S();
                }
                s.Log();
            }*/
            //WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            //int prefix = item.prefix;
            //item = reforgeItem;
            //item.prefix = prefix;
            //wePlayer.UpdateItemStats(ref item);
            newPrefix = item.prefix;
            if (Main.reforgeItem.IsAir && item != null && !item.IsAir || !Main.reforgeItem.IsAir && Main.reforgeItem.G().normalReforge)
            {
                ReforgeItem(ref item, Main.LocalPlayer);
            }
            if (UtilityMethods.debugging) ($"/\\PostReforge({item.S()})").Log();
        }
        public override bool PreReforge(Item item)
        {
            if(UtilityMethods.debugging) ($"\\/PreReforge({item.S()})").Log();
            reforgeItem = item.Clone();
            if (!Main.reforgeItem.IsAir)
                Main.reforgeItem.G().normalReforge = true;
            if (UtilityMethods.debugging)
            {
                string s = $"reforgeItem: {reforgeItem.S()}, prefix: {reforgeItem.prefix}, Enchantments: ";
                foreach (Item enchantment in reforgeItem.G().enchantments)
                {
                    s += enchantment.S();
                }
                s.Log();
            }
            if (UtilityMethods.debugging) ($"/\\PreReforge({item.S()})").Log();
            return true;
        }
        public static void ReforgeItem(ref Item item, Player player)
        {
            WEPlayer wePlayer = player.G();
            cloneReforgedItem = true;
            reforgeItem.G().Clone(reforgeItem, item);
            reforgeItem = null;
            newPrefix = 0;
            if (!Main.reforgeItem.IsAir)
                Main.reforgeItem.G().normalReforge = false;
            item.G().normalReforge = false;
            wePlayer.UpdateItemStats(ref item);
        }
		public override void OnSpawn(Item item, IEntitySource source)
		{
			if (WEMod.IsWeaponItem(item) && source is EntitySource_DropAsItem dropSource && dropSource.Context == "Stolen")
			{
                for (int i = 0; i < Main.item.Length; i++)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient && item.type == Main.item[i].type && item.position == Main.item[i].position)
                        WEModSystem.stolenItemToBeCleared = i;
                }
			}
		}
		public override void OnCreate(Item item, ItemCreationContext context)
		{
			if(context is RecipeCreationContext)
			{
                item.CombineEnchantedItems(ref WEMod.consumedItems);
            }
		}
		public override bool CanStack(Item item1, Item item2)
		{
            if (!WEMod.IsEnchantable(item1) || item1.maxStack < 2 || item1.whoAmI > 0 || item2.whoAmI > 0)
                return true;
			if (stack0)
			{
                stack0 = false;
                item1.stack--;
			}
            else if (item2.G().stack0)
			{
                item2.G().stack0 = false;
                item2.stack--;
			}
            if (item2.G().inEnchantingTable && !Main.mouseLeft && !Main.mouseRight && !WeaponEnchantmentUI.pressedLootAll)
                return true;
            int maxStack = item1.maxStack;
            //if (item1.stack + item2.stack <= maxStack)
            /*if (item2.G().inEnchantingTable && (item1.stack == maxStack || item2.stack == maxStack))
                return false;*/
            if(item1.stack < maxStack && item2.stack < maxStack)
            {
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
                    if (item1.stack + item2.stack > maxStack)
                    {
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
		public override bool CanStackInWorld(Item item1, Item item2)
		{
            //return false;
            return true;
		}
	}
}
