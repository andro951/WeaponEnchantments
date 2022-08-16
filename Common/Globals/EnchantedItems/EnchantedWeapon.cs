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
	public class EnchantedWeapon : EnchantedItem, ISortedEnchantmentEffects
    {
        #region Constants

        public SortedDictionary<byte, WeaponStat> WeaponStatDict = new SortedDictionary<byte, WeaponStat>(Enum.GetValues(typeof(WeaponStat)).Cast<WeaponStat>().ToDictionary(t => (byte)t, t => t));
        public enum WeaponStat : byte {
            AttackSpeed,
            ArmorPenetration,
            AutoReuse,
            CriticalStrikeChance,
            Damage,
            DamageAfterDefenses,
            Knockback,
            LifeSteal,
            ManaCost,
            Size,
        }

        #endregion

        #region Stats

        //New system
        public DamageClassChange DamageTypeEffect;
        public SortedDictionary<byte, EStatModifier> EnchantmentStats { set; get; } = new SortedDictionary<byte, EStatModifier>();
        public SortedDictionary<byte, EStatModifier> VanillaStats { set; get; } = new SortedDictionary<byte, EStatModifier>();
        public SortedDictionary<short, BuffStats> OnHitDebuffs { set; get; } = new SortedDictionary<short, BuffStats>();
        public SortedDictionary<short, BuffStats> OnHitBuffs { set; get; } = new SortedDictionary<short, BuffStats>();
        public SortedDictionary<short, BuffStats> OnTickBuffs { set; get; } = new SortedDictionary<short, BuffStats>();

        public IEnumerable<EnchantmentEffect> EnchantmentEffects { set; get; }
        public IEnumerable<IPassiveEffect> PassiveEffects { set; get; }
        public IEnumerable<StatEffect> StatEffects { set; get; }

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
        public override EItemType ItemType => EItemType.Weapon;
        public override GlobalItem Clone(Item item, Item itemClone) {
            EnchantedWeapon clone = (EnchantedWeapon)base.Clone(item, itemClone);

            if (cloneReforgedItem || resetGlobals) {

                #region Enchantments

                clone.DamageTypeEffect = DamageTypeEffect;
                clone.EnchantmentStats = new SortedDictionary<WeaponStat, EStatModifier>(EnchantmentStats);
                clone.VanillaStats = new SortedDictionary<WeaponStat, EStatModifier>(VanillaStats);
                clone.OnHitDebuffs = new SortedDictionary<short, int>(OnHitDebuffs);
                clone.OnHitBuffs = new SortedDictionary<short, int>(OnHitBuffs);
                clone.OnTickBuffs = new SortedDictionary<short, int>(OnTickBuffs);
                clone.EnchantmentEffects = EnchantmentEffects;
                clone.PassiveEffects = PassiveEffects;
                clone.StatEffects = StatEffects;

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
            /*string name = "EnchantedItem";
            string mod = "WeaponEnchantments";


            bool loadFromCloud = Main.ActiveWorldFileData.IsCloudSave;
            string path = Main.worldPathName;

            path = Path.ChangeExtension(path, ".twld");

            //if (!FileUtilities.Exists(path, isCloudSave))
            //    return;
            bool isCloudSave = loadFromCloud;
            byte[] buf = FileUtilities.ReadAllBytes(path, isCloudSave);

            if (buf[0] != 0x1F || buf[1] != 0x8B) {
                //LoadLegacy(buf);
                return;
            }

            var tag2 = TagIO.FromStream(new MemoryStream(buf));
            var list = tag2.GetList<TagCompound>("globalData");
            foreach (var tag3 in list) {
                if (ModContent.TryFind(mod, name, out GlobalItem globalItemBase) && item.TryGetGlobalItem(globalItemBase, out var globalItem)) {
                    try {
                        globalItem.LoadData(item, tag3.GetCompound("data"));
                    }
                    catch (Exception e) {
                        //throw new CustomModDataException(globalItem.Mod, $"Error in reading custom player data for {globalItem.FullName}", e);
                    }
                }
                else {
                    //Unloaded GlobalItems and GlobalItems that are no longer valid on an item (e.g. through AppliesToEntity)
                    //item.GetGlobalItem<UnloadedGlobalItem>().data.Add(tag3);
                }
            }*/

            //LoadPlayers()
            //ItemIO.LoadGlobals(Item item, IList<TagCompound> list)
            //GlobalItem.LoadData(Item item, TagCompound tag)

            base.LoadData(item, tag);

            #region Tracking (instance)

            Stack0 = tag.Get<bool>("stack0");

            #endregion

            _experience = tag.Get<int>("experience");

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
                    float damageMultiplier = item.ApplyEStat("Damage", 1f);
                    if (damageMultiplier > 1f) {
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
                float multiplier = GlobalEnchantmentStrengthMultiplier;
                crit += levelBeforeBooster * multiplier;
            }
        }
        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage) {
            damage *= infusionDamageMultiplier;
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
