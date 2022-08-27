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
using WeaponEnchantments.UI;
using static WeaponEnchantments.Common.Configs.ConfigValues;
using static WeaponEnchantments.Common.EnchantingRarity;
using static WeaponEnchantments.Common.Globals.EnchantedItemStaticMethods;
using static WeaponEnchantments.Items.Enchantment;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Common.Globals
{
    public class EnchantedWeapon : EnchantedHeldItem, ISortedOnHitEffects
    {

        #region Stats

        //New system
        //public SortedDictionary<PermenantItemFields, StatModifier> AppliedPermenantStats = new SortedDictionary<PermenantItemFields, StatModifier>();
        //public SortedDictionary<PermenantItemFields, StatModifier> PermenantStats = new SortedDictionary<PermenantItemFields, StatModifier>();
        public DamageClassChange DamageTypeEffect;

        //public SortedDictionary<EnchantmentStat, EStatModifier> EnchantmentStats { set; get; } = new SortedDictionary<EnchantmentStat, EStatModifier>();
        //public SortedDictionary<EnchantmentStat, EStatModifier> VanillaStats { set; get; } = new SortedDictionary<EnchantmentStat, EStatModifier>();
        //public SortedList<EnchantmentStat, PlayerSetEffect> PlayerSetEffects { set; get; } = new SortedList<EnchantmentStat, PlayerSetEffect>();
        public SortedDictionary<short, BuffStats> OnHitDebuffs { set; get; } = new SortedDictionary<short, BuffStats>();
        public SortedDictionary<short, BuffStats> OnHitBuffs { set; get; } = new SortedDictionary<short, BuffStats>();
        //public SortedDictionary<short, BuffStats> OnTickBuffs { set; get; } = new SortedDictionary<short, BuffStats>();

        //public List<EnchantmentEffect> EnchantmentEffects { set; get; } = new List<EnchantmentEffect>();
        //public List<IPassiveEffect> PassiveEffects { set; get; } = new List<IPassiveEffect>();
        public List<IOnHitEffect> OnHitEffects { set; get; } = new List<IOnHitEffect>();
        //public List<IModifyShootStats> ModifyShootStatEffects { set; get; } = new List<IModifyShootStats>();
        //public List<StatEffect> StatEffects { set; get; } = new List<StatEffect>();

        #endregion

        #region Enchantment

        public DamageClass damageType = DamageClass.Default;
        public DamageClass baseDamageType = DamageClass.Default;

        #endregion

        #region Infusion

        public float infusionDamageMultiplier = 1f;

        #endregion

        #region Tracking (instance)

        public bool trackedWeapon = false;
        public bool hoverItem = false;
        private bool _stack0 = false;
        public bool Stack0 {
            get {
                if (_stack0) {
                    if (Item.stack > 1) {
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

        #endregion

        public EnchantedWeapon() : base() {
            DamageTypeEffect = DamageClassChange.Default;
        }

        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => IsWeaponItem(entity);
        public override EItemType ItemType => EItemType.Weapons;
        public override GlobalItem Clone(Item item, Item itemClone) {
            EnchantedWeapon clone = (EnchantedWeapon)base.Clone(item, itemClone);

            if (cloneReforgedItem || resetGlobals) {

                #region Enchantments

                clone.DamageTypeEffect = DamageTypeEffect;
                clone.EnchantmentStats = new SortedDictionary<EnchantmentStat, EStatModifier>(EnchantmentStats.ToDictionary(k => k.Key, k => k.Value.Clone()));
                clone.VanillaStats = new SortedDictionary<EnchantmentStat, EStatModifier>(VanillaStats.ToDictionary(k => k.Key, k => k.Value.Clone()));
                clone.OnHitDebuffs = new SortedDictionary<short, BuffStats>(OnHitDebuffs.ToDictionary(k => k.Key, k => k.Value.Clone()));
                clone.OnHitBuffs = new SortedDictionary<short, BuffStats>(OnHitBuffs.ToDictionary(k => k.Key, k => k.Value.Clone()));
                clone.OnTickBuffs = new SortedDictionary<short, BuffStats>(OnTickBuffs.ToDictionary(k => k.Key, k => k.Value.Clone()));
                clone.EnchantmentEffects = EnchantmentEffects.GetRange(0, EnchantmentEffects.Count);
                clone.PassiveEffects = PassiveEffects.GetRange(0, PassiveEffects.Count);
                clone.StatEffects = StatEffects.GetRange(0, StatEffects.Count);

                clone.damageType = damageType;
                clone.baseDamageType = baseDamageType;

                #endregion

                #region Infusion

                clone.infusionDamageMultiplier = infusionDamageMultiplier;

                #endregion

                #region Tracking (instance)

                clone.Stack0 = Stack0;

                #endregion
            }

            if (!Main.mouseItem.IsSameEnchantedItem(itemClone))
                clone.trackedWeapon = false;

            return clone;
        }
        public override void LoadData(Item item, TagCompound tag) {
            base.LoadData(item, tag);

            #region Tracking (instance)

            Stack0 = tag.Get<bool>("stack0");

            #endregion

        }
        public override void SaveData(Item item, TagCompound tag) {
            base.SaveData(item, tag);

            #region Tracking (instance)

            tag["stack0"] = Stack0;

            #endregion
        }
        public override void NetSend(Item item, BinaryWriter writer) {
            base.NetSend(item, writer);

            #region Tracking (instance)

            writer.Write(Stack0);

            #endregion
        }
        public override void NetReceive(Item item, BinaryReader reader) {
            base.NetReceive(item, reader);

            #region Tracking (instance)

            Stack0 = reader.ReadBoolean();

            #endregion
        }
        public override void UpdateInventory(Item item, Player player) {
            if (Modified) {
                //Stars Above compatibility fix
                if (baseDamageType != damageType) {
                    if (baseDamageType == DamageClass.Default)
                        baseDamageType = ContentSamples.ItemsByType[item.type].DamageType;

                    if (AlwaysOverrideDamageType || item.DamageType == baseDamageType)
                        item.DamageType = damageType;
                }
            }

            base.UpdateInventory(item, player);
        }
        public override bool OnPickup(Item item, Player player) {
            player.GetWEPlayer().UpdateItemStats(ref item);

            return true;
        }
        protected override void GetTopTooltips(Item item, List<TooltipLine> tooltips) {
            WEPlayer wePlayer = Main.LocalPlayer.GetWEPlayer();

            if (Enchanted) {
                //~Damage tooltip
                if (WEMod.clientConfig.DisplayApproximateWeaponDamageTooltip) {
                    if (GetPlayerModifierStrengthForTooltip(wePlayer.Player, EnchantmentStat.DamageAfterDefenses, out float damageMultiplier) && damageMultiplier != 1f) {
                        int damage = (int)Math.Round(wePlayer.Player.GetWeaponDamage(item, true) * damageMultiplier);
                        string tooltip = $"Item Damage ~ {damage} (Against 0 armor enemy)";
                        tooltips.Add(new TooltipLine(Mod, "level", tooltip) { OverrideColor = Color.DarkRed });
                    }
                }
            }

            //Stack0
            if (Modified || inEnchantingTable) {
                if (Stack0) {
                    string tooltip = $"♦ OUT OF AMMO ♦";
                    tooltips.Add(new TooltipLine(Mod, "stack0", tooltip) { OverrideColor = Color.Yellow });
                }
            }
        }
        protected override string GetInfusedItemTooltip(Item item) => $"Infusion Power: {infusionPower}   Infused Item: {infusedItemName}";
        protected override string GetInfusionTooltip(Item item) => $"Infusion Power: {item.GetWeaponInfusionPower()}";
        protected override string GetNewInfusedItemTooltip(Item item, WEPlayer wePlayer) {
            return
                $"*New Infusion Power: {wePlayer.infusionConsumeItem.GetWeaponInfusionPower()}   " +
                $"New Infused Item: {wePlayer.infusionConsumeItem.GetInfusionItemName()}*";
        }
        public override void ModifyWeaponCrit(Item item, Player player, ref float crit) {
            if (!WEMod.serverConfig.CritPerLevelDisabled) {
                crit += levelBeforeBooster * GlobalStrengthMultiplier;
            }

            CheckEnchantmnetStatsApplyTo(ref crit, EnchantmentStat.CriticalStrikeChance);
        }
        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage) {
            damage *= infusionDamageMultiplier;
            CheckEnchantmentStatsForModifier(ref damage, EnchantmentStat.Damage);
        }
        public override void ModifyWeaponKnockback(Item item, Player player, ref StatModifier knockback) {
            CheckEnchantmentStatsForModifier(ref knockback, EnchantmentStat.Knockback);
        }
        public override bool? CanAutoReuseItem(Item item, Player player) {
            //TODO, seperate bool effects into it's own dictionary of bool?

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
            /*if (statModifiers.ContainsKey("P_autoReuse")) {
                return false;
            }
            else {
                return null;
            }*/
        }
        private void Restock(Item item) {
            Player player = Main.LocalPlayer;

            //Find same item
            for (int i = 0; i < 59; i++) {
                Item inventoryItem = player.inventory[i];
                if (inventoryItem.type != item.type)
                    continue;

                if (!inventoryItem.TryGetEnchantedWeapon(out EnchantedWeapon invEnchantedWeapon))
                    continue;

                if (invEnchantedWeapon.Modified)
                    continue;

                if (invEnchantedWeapon.Stack0)
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

            bool? returnValue = null;
            foreach(IUseItem effect in EnchantmentEffects.OfType<IUseItem>()) {
                bool? useItem = effect.UseItem(item, player);
                if (useItem != null) {
                    if (returnValue == false) {
                        returnValue = useItem;
                    }
                }
            }

            //CatastrophicRelease
            if (eStats.ContainsKey("CatastrophicRelease")) {
                player.statMana = 0;
            }

            //Consumable weapons  (item.placeStyle fix for a placable enchantable item)
            if (item.consumable && Modified && item.placeStyle == 0) {

                //Ammo Cost
                if (_stack < 0 || item.stack == Stack) {
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

            return returnValue;
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

            foreach (ICanUseItem effect in EnchantmentEffects.OfType<ICanUseItem>()) {
                if (!effect.CanUseItem(item, player))
                    return false;
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
        public override void OnHitNPC(Item item, Player player, NPC target, int damage, float knockBack, bool crit) {
            item.DamageNPC(player, target, damage, crit, true);
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
    }

    public static class EnchantedWeaponStaticMethods
	{
        public static float GetReductionFactor(int hp) {
            float factor = hp < 7000 ? hp / 1000f + 1f : 8f;
            return factor;
        }
    }
}
