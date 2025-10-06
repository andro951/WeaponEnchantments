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
using static androLib.Common.EnchantingRarity;
using static WeaponEnchantments.Common.Globals.EnchantedItemStaticMethods;
using static WeaponEnchantments.Items.Enchantment;
using static WeaponEnchantments.WEPlayer;
using androLib.Common.Utility;
using androLib;

namespace WeaponEnchantments.Common.Globals
{
    public class EnchantedWeapon : EnchantedHeldItem, ISortedOnHitEffects {

        #region Stats

        public DamageClassSwap DamageTypeEffect;
        public SortedDictionary<short, BuffStats> OnHitDebuffs { set; get; } = new SortedDictionary<short, BuffStats>();
        public SortedDictionary<short, BuffStats> OnHitBuffs { set; get; } = new SortedDictionary<short, BuffStats>();
        public List<IOnHitEffect> OnHitEffects { set; get; } = new List<IOnHitEffect>();

        public static int AmmoBagStorageID = -1;

        #endregion

        #region Enchantment

        public DamageClass damageType = DamageClass.Default;
        public DamageClass baseDamageType = DamageClass.Default;

		#endregion

		#region Infusion

        public const int DefaultInfusionPower = -1;
        public int GetInfusionPower(ref Item item) {
            if (infusionPower == DefaultInfusionPower)
                infusionPower = item.GetWeaponInfusionPowerSearchIfNeeded(infusedItemName);

            return infusionPower;
        }
        public void SetInfusionPower(int newValue) {
			infusionPower = newValue;
		}
		private int infusionPower = DefaultInfusionPower;
		public float infusionDamageMultiplier = 1f;
        public class GatheringToolStats {
            public const int DefaultStatValue = -1;
            int useTime = DefaultStatValue;
            int useAnimation = DefaultStatValue;
            int pick = DefaultStatValue;
            int axe = DefaultStatValue;
            int hammer = DefaultStatValue;
            public GatheringToolStats(Item infusedItem) {
                if (infusedItem.NullOrAir()) {
                    ResetStats();
                    return;
                }

                useTime = infusedItem.useTime;
                useAnimation = infusedItem.useAnimation;
                pick = infusedItem.pick;
                axe = infusedItem.axe;
                hammer = infusedItem.hammer;
            }

            public void UpdateStats(Item item) {
                Item csi = item.type.CSI();
                if (csi.IsGatheringTool()) {
					if (useTime == DefaultStatValue) {
						item.useTime = csi.useTime;
						item.useAnimation = csi.useAnimation;
					}
					else if (item.useTime > useTime) {
						item.useTime = useTime;
						item.useAnimation = useAnimation;
					}
				}
                
                if (pick == DefaultStatValue) {
                    item.pick = csi.pick;
                }
                else if (item.pick < pick) {
					item.pick = pick;
				}

                if (axe == DefaultStatValue) {
                    item.axe = csi.axe;
                }
                else if (item.axe < axe) {
					item.axe = axe;
				}

                if (hammer == DefaultStatValue) {
                    item.hammer = csi.hammer;
                }
                else if (item.hammer < hammer) {
					item.hammer = hammer;
				}
            }
            public void ResetStats() {
				useTime = DefaultStatValue;
				useAnimation = DefaultStatValue;
				pick = DefaultStatValue;
				axe = DefaultStatValue;
				hammer = DefaultStatValue;
			}
		}
        private GatheringToolStats gatheringToolStats = null;
		public void TrySetGatheringTools(Item item, Item infusedItem) {
            if (!item.type.CSI().IsGatheringTool() && item.useStyle != ItemUseStyleID.Swing)
                return;

            if (infusedItem.type.CSI().IsGatheringTool()) {
				gatheringToolStats = new(infusedItem);
				gatheringToolStats.UpdateStats(item);
			}
            else if (gatheringToolStats != null) {
                gatheringToolStats.ResetStats();
				gatheringToolStats.UpdateStats(item);
			}
		}

		#endregion

		#region Tracking (instance)
		public bool GetStack0(Item item) {
            if (_stack0) {
                if (item.stack > 1) {
					item.stack--;
					_stack0 = false;
				}
            }

            return _stack0;
        }
        public void SetStack0(Item item, bool newValue) {
            if (!_stack0 && newValue && item.stack < 2) {
                item.stack = 2;
            }

            _stack0 = newValue;
        }
        private bool _stack0 = false;

        #endregion

        public EnchantedWeapon() : base() {
            DamageTypeEffect = DamageClassSwap.Default;
        }

        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => lateInstantiation && entity.IsWeaponItem();
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

				clone.infusionPower = GetInfusionPower(ref item);
				clone.infusionDamageMultiplier = infusionDamageMultiplier;

                #endregion

                #region Tracking (instance)

                clone._stack0 = _stack0;

                #endregion
            }

            return clone;
        }
        public override void LoadData(Item item, TagCompound tag) {
            base.LoadData(item, tag);

			#region Infusion

			if (infusedItemName != "" && tag.TryGet<int>("infusedPower", out int infusionPower))
				this.infusionPower = infusionPower;

			#endregion

			#region Tracking (instance)

			_stack0 = tag.Get<bool>("stack0");

            #endregion

        }
        public override void SaveData(Item item, TagCompound tag) {
            base.SaveData(item, tag);

			#region Infusion

			if (infusedItemName != "") {
				tag["infusedPower"] = GetInfusionPower(ref item);
			}

			#endregion

			#region Tracking (instance)

			tag["stack0"] = GetStack0(item);

            #endregion

        }
        public override void NetSend(Item item, BinaryWriter writer) {

			#region Infusion

			bool noName = infusedItemName == "";
			writer.Write(noName);
			if (!noName) {
				writer.Write(GetInfusionPower(ref item));
			}

			#endregion

			#region Tracking (instance)

			writer.Write(_stack0);

			#endregion

			//Important for infusionPower to be obtained before base
			base.NetSend(item, writer);
		}
        public override void NetReceive(Item item, BinaryReader reader) {
            Item = item;

			#region Infusion

			bool noName = reader.ReadBoolean();
			if (!noName) {
				infusionPower = reader.ReadInt32();
			}

			#endregion

			#region Tracking (instance)

			_stack0 = reader.ReadBoolean();

			#endregion

            //Important for infusionPower to be obtained before base
			base.NetReceive(item, reader);
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
        protected override void GetTopTooltips(Item item, List<TooltipLine> tooltips) {
            WEPlayer wePlayer = Main.LocalPlayer.GetWEPlayer();

			//~Damage tooltip
			if (WEMod.clientConfig.DisplayDamageTooltipSeperatly) {
				if (GetPlayerModifierStrengthForTooltip(wePlayer.Player, EnchantmentStat.DamageAfterDefenses, out float damageMultiplier) && damageMultiplier != 1f) {
					int damage = (int)Math.Round(wePlayer.Player.GetWeaponDamage(item, true) * damageMultiplier);
                    string tooltip = $"{EnchantmentGeneralTooltipsID.ApproximateItemDamage}".Lang_WE(L_ID1.Tooltip, L_ID2.EnchantmentGeneralTooltips, new object[] { damage });// $"Item Damage ~ {damage} (Against 0 armor enemy)";
					tooltips.Add(new TooltipLine(Mod, "level", tooltip) { OverrideColor = Color.DarkRed });
				}
			}

			//Stack0
			if (Modified || inEnchantingTable) {
                if (GetStack0(item)) {
                    string tooltip = $"♦ {$"{EnchantmentGeneralTooltipsID.OutOfAmmo}".Lang_WE(L_ID1.Tooltip, L_ID2.EnchantmentGeneralTooltips)} ♦";
                    tooltips.Add(new TooltipLine(Mod, "stack0", tooltip) { OverrideColor = Color.Yellow });
                }
            }
        }
        protected override string GetPerLevelBonusTooltip() {
            if (WEMod.serverConfig.CritPerLevelDisabled)
                return "";

            float perLevelBonus = GetPerLevelBonus();
            string tooltip = "";
            if (perLevelBonus > 0f) {
                if (WEMod.serverConfig.DamagePerLevelInstead) {
					tooltip = perLevelBonus > 0f ?
				        $"+{perLevelBonus.PercentString()} {$"{EnchantmentStat.DamageAfterDefenses}".Lang_WE(L_ID1.Tooltip, L_ID2.EffectDisplayName)}" : "";
				}
                else {
					tooltip = perLevelBonus > 0f ?
				        $"+{perLevelBonus.PercentString()} {$"{EnchantmentStat.CriticalStrikeChance}".Lang_WE(L_ID1.Tooltip, L_ID2.EffectDisplayName)}" : "";
				}
            }

            return tooltip;
        }
        protected override string GetInfusedItemTooltip(Item item) => $"{$"{EnchantmentGeneralTooltipsID.InfusionPower}".Lang_WE(L_ID1.Tooltip, L_ID2.EnchantmentGeneralTooltips)} {GetInfusionPower(ref item)}   {$"{EnchantmentGeneralTooltipsID.InfusedItem}".Lang_WE(L_ID1.Tooltip, L_ID2.EnchantmentGeneralTooltips)} {infusedItemName}";
        protected override string GetInfusionTooltip(Item item) => $"{$"{EnchantmentGeneralTooltipsID.InfusionPower}".Lang_WE(L_ID1.Tooltip, L_ID2.EnchantmentGeneralTooltips)} {GetInfusionPower(ref item)}";
        protected override string GetNewInfusedItemTooltip(Item item, WEPlayer wePlayer) {
            if (!wePlayer.infusionConsumeItem.TryGetEnchantedWeapon(out EnchantedWeapon enchantedWeapon))
                return "";

            return
                $"*{$"{EnchantmentGeneralTooltipsID.NewInfusionPower}".Lang_WE(L_ID1.Tooltip, L_ID2.EnchantmentGeneralTooltips)}: {enchantedWeapon.GetInfusionPower(ref wePlayer.infusionConsumeItem)}   " +
                $"{$"{EnchantmentGeneralTooltipsID.NewInfusedItem}".Lang_WE(L_ID1.Tooltip, L_ID2.EnchantmentGeneralTooltips)} {wePlayer.infusionConsumeItem.GetInfusionItemName()}*";
        }
        public override void ModifyWeaponCrit(Item item, Player player, ref float crit) {
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
        private void Restock(Item item) {//TODO: make this look in the ammo bag.
            Player player = Main.LocalPlayer;

            if (TryRestockFromInventory(item, player.inventory))
                return;

            //Find same item
            for (int i = 0; i < 59; i++) {
                Item inventoryItem = player.inventory[i];
                if (inventoryItem.type != item.type)
                    continue;

                if (!inventoryItem.TryGetEnchantedWeapon(out EnchantedWeapon invEnchantedWeapon))
                    continue;

                if (invEnchantedWeapon.Modified)
                    continue;

                if (invEnchantedWeapon.GetStack0(item))
                    continue;

                //Restock (found same item)
                item.stack = inventoryItem.stack;//TODO: Make this use ItemLoader.TransferStack instead of setting stack equal to.
                //SetStack0(item, false);
                player.inventory[i] = new Item();
                return;
            }

            if (AndroMod.vacuumBagsEnabled) {
                IEnumerable<Item> ammoBagItems = StorageManager.GetItems(AmmoBagStorageID);
                if (TryRestockFromInventory(item, ammoBagItems))
					return;

			}
		}
		private bool TryRestockFromInventory(Item item, IEnumerable<Item> inventory) {
            foreach (Item inventoryItem in inventory.Where(i => !i.NullOrAir() && i.type == item.type)) {
				if (!inventoryItem.TryGetEnchantedWeapon(out EnchantedWeapon invEnchantedWeapon))
					continue;

				if (invEnchantedWeapon.Modified)
					continue;

				if (invEnchantedWeapon.GetStack0(item))
					continue;

                if (ItemLoader.TryStackItems(item, inventoryItem, out int _))
                    return true;
			}

            return false;
		}
		public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
            IEnumerable<IUseTimer> useTimers = player.GetWEPlayer().EffectTimers.Where(n => n.Value.TimerStatName == EnchantmentStat.CatastrophicRelease).Select(t => t.Value);
            foreach (IUseTimer useTimer in useTimers) {
                if (!useTimer.TimerOver(player))
                    return false;
            }

            return true;
        }
        public override bool? UseItem(Item item, Player player) {
            WEPlayer wePlayer = player.GetModPlayer<WEPlayer>();

            if (!Modified)
                return null;

            bool? returnValue = base.UseItem(item, player);

            //Consumable weapons  (item.placeStyle fix for a placable enchantable item)
            if (item.consumable && Modified && item.placeStyle == 0) {//Restock and Stack0
                if (item.stack < 2) {
                    Restock(item);
                    SetStack0(item, true);
                }
            }

            return returnValue;
        }
        public override bool CanUseItem(Item item, Player player) {
            //stack0
            if (GetStack0(item)) {
                Restock(item);

                if (GetStack0(item)) {
                    //Restock failed
                    return false;
                }
            }

            foreach (ICanUseItem effect in EnchantmentEffects.OfType<ICanUseItem>()) {
                if (!effect.CanUseItem(item, player))
                    return false;
            }

            return true;
        }
        public override void OnHitNPC(Item item, Player player, NPC target, NPC.HitInfo hit, int damageDone) {
            item.DamageNPC(player, target, hit, true);
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
        public float GetPerLevelBonus() => levelBeforeBooster * GlobalStrengthMultiplier / 100f;
		public override void ResetInfusion(Item item) {
			base.ResetInfusion(item);
            infusionDamageMultiplier = 1f;
            SetInfusionPower(DefaultInfusionPower);
            if (gatheringToolStats != null) {
                gatheringToolStats.ResetStats();
                gatheringToolStats.UpdateStats(item);
            }
		}
	}

    public static class EnchantedWeaponStaticMethods
	{
		public static float GetPrideOfTheWeakMultiplier(this EnchantedWeapon enchantedWeapon) {
            Item item = enchantedWeapon.Item;
            int infusionPower = enchantedWeapon.GetInfusionPower(ref item);
			int infusionPowerFromInfusion = InfusionManager.ReverseEngInfusionPowerFromMultiplierForPrideOfTheWeak(item);
            return 1f - (float)(infusionPower + infusionPowerFromInfusion) / 500f;
		}
	}
}
