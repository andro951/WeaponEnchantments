using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;

namespace WeaponEnchantments.Common.Globals
{
    public class WEProjectile : ProjectileWithSourceItem
    {
        //Stat changes
        public bool multiShotConvertedToDamage = false;

        //Attack speed tracking
        public float[] lastAIValue = new float[] { 0f, 0f };
        float[] speedCarryover = new float[] { 0f, 0f };
        public bool[] spawnedChild = { false, false };
        float[] spawnChildValue = { 0f, 0f };
        float[] nextValueAfterChild = { 0f, 0f };
        long[] lastChildSpawnTime = { 0, 0 };
        bool[] positive = { true, true };
        bool[] completedChildSpawnSpeedSetup = { false, false };

        //Tracking
        public bool skipOnHitEffects = false;
        public int lastInventoryLocation = -1;
        bool weaponProjectile = false;
        public bool activatedOneForAll = false;

        public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) {
            if (!base.AppliesToEntity(entity, lateInstantiation))
                return false;

            return entity.aiStyle != ProjAIStyleID.Bobber;
		}
		public override void OnSpawn(Projectile projectile, IEntitySource source) {

			#region Debug

			if (LogMethods.debugging)
            {
                for (int i = 0; i < projectile.ai.Length; i++)
                {
                    float aiValue = projectile.ai[i];
                    ($"OnSpawn projectile: {projectile.S()} aiValue: {aiValue} lastAIValue[{i}]: {lastAIValue[i]} ai[{i}]: {projectile.ai[i]}").Log();
                }
            }

            #endregion

            //VortexBeater, Celeb2, Phantasm fix (Speed Enchantments)
            weaponProjectile = projectile.type == ProjectileID.VortexBeater || projectile.type == ProjectileID.Celeb2Weapon || projectile.type == ProjectileID.Phantasm;

            if (source is EntitySource_ItemUse_WithAmmo vbSource) {
                //These weapons shoot the weapon sprite instead of shooting bullest/arrows etc.  This causes many challenges with changing attackspeed.
                bool projectileFromVortexBeater = vbSource.Item.type == ItemID.VortexBeater;
                bool projectileFromCeleb2 = vbSource.Item.type == ItemID.Celeb2;
                bool prjectileFromPhantasm = vbSource.Item.type == ItemID.Phantasm;
                if (!weaponProjectile && ( projectileFromVortexBeater || projectileFromCeleb2 || prjectileFromPhantasm)) {
                    //Try get source projectile from the weapon.
                    if(vbSource.Item.TryGetEnchantedItem(out EnchantedItem vbSourceGlobal)) {
                        if (vbSourceGlobal.masterProjectile != null)
                            source = vbSourceGlobal.masterProjectile.GetSource_FromThis();
                    }
                }
            }

            base.OnSpawn(projectile, source);

            if (source is EntitySource_ItemUse uSource) {
                if (uSource.Item != null && uSource.Item.TryGetEnchantedItem(out EnchantedItem uSourceGlobal)) {
                    //Set Master projectile for VortexBeater, Celeb2, Phantasm fix (Speed Enchantments)
                    if (weaponProjectile)
                        uSourceGlobal.masterProjectile = projectile;
                }
            }

            //Update Projectile
            projectile.GetGlobalProjectile<WEProjectile>().UpdateProjectile(projectile);

			#region Debug

			if (LogMethods.debugging) ($"OnSpawn(projectile: {projectile.S()}) sourceItem: {sourceItem.S()} playerSource: {playerSource.S()}").Log();

            #endregion
        }
        public override bool UpdateProjectile(Projectile projectile) {
            if (updated)
                return false;

            if (!base.UpdateProjectile(projectile))
                return false;

            //NPC Hit Cooldown
            if (projectile.minion || projectile.DamageType == DamageClass.Summon || weaponProjectile) {
                GetSharedVanillaModifierStrength(projectile.owner, EnchantmentStat.AttackSpeed, out float attackSpeedMultiplier);

                if(GetEnchantmentModifierStrength(EnchantmentStat.AllForOne, out float allForOne))
                    allForOne /= 2.5f;

                float speedMultiplier = attackSpeedMultiplier / allForOne;
                speed = 1f - 1f / speedMultiplier;
            }

            Main.player[projectile.owner].GetWEPlayer().CheckEnchantmentStats(EnchantmentStat.NPCHitCooldown, out float NPCHitCooldownMultiplier, 1f);

            //Immunities
            if (projectile.usesLocalNPCImmunity) {
                if (NPCHitCooldownMultiplier > 1f) {
                    projectile.usesIDStaticNPCImmunity = true;
                    projectile.usesLocalNPCImmunity = false;
                    projectile.idStaticNPCHitCooldown = projectile.localNPCHitCooldown;
                }
                else if (projectile.localNPCHitCooldown > 0) {
                    projectile.localNPCHitCooldown = (int)Math.Round((float)projectile.localNPCHitCooldown * NPCHitCooldownMultiplier);
                }
            }

            if (projectile.usesIDStaticNPCImmunity) {
                if (projectile.idStaticNPCHitCooldown > 0)
                    projectile.idStaticNPCHitCooldown = (int)Math.Round((float)projectile.idStaticNPCHitCooldown * NPCHitCooldownMultiplier);
            }

            updated = true;

            return true;//Return value not used in overriden methods
        }
        protected override void ActivateMultishot(Projectile projectile, IEntitySource source) {
            //Convert multishot to damage multiplier instead (Happens in WEGlobalNPC)
            switch (sourceItem.Name) {
                //Fix issues with weapons and multishot
                case "Titanium Railgun":
                    multiShotConvertedToDamage = true;
                    break;
            }

            //Flamethrowers fix
            if (!multiShotConvertedToDamage) {
                multiShotConvertedToDamage = sourceItem.useAmmo == ItemID.Gel;
            }

            if (!weaponProjectile && !multiShotConvertedToDamage)
                base.ActivateMultishot(projectile, source);
        }
        protected virtual void TryUpdateFromParent() {
            base.TryUpdateFromParent(out WEProjectile pGlobal);

            for (int i = 0; i < 2; i++) {
                if (pGlobal.completedChildSpawnSpeedSetup[i]) {
                    //Parent has spawned a child before
                    float ai = parent.ai[i];
                    double lastspawntime = pGlobal.lastChildSpawnTime[i];
                    float nextAfterChild = pGlobal.nextValueAfterChild[i];
                    if (Main.GameUpdateCount - 1 > lastspawntime) {
                        pGlobal.spawnedChild[i] = true;
                        if (Math.Abs(ai) < Math.Abs(nextAfterChild))
                            pGlobal.nextValueAfterChild[i] = ai;
                    }
                    else {
                        //Force recalculate nextValueAfterChild if triggering every tick. (Fixes an infinite loop of shooting every tick)
                        pGlobal.completedChildSpawnSpeedSetup[i] = false;
                    }
                }
                else {
                    //Parent has not spwned a child before
                    float lastAIValue = pGlobal.lastAIValue[i];
                    float ai = parent.ai[i];
                    double difference = Math.Abs(Math.Abs(ai) - Math.Abs(lastAIValue));
                    if (difference > 3) {
                        pGlobal.spawnedChild[i] = true;
                        pGlobal.spawnChildValue[i] = lastAIValue;
                        pGlobal.nextValueAfterChild[i] = ai;
                    }
                }

                #region Debug

                if (LogMethods.debugging) {
                    string txt = $"parent: {parent.S()} spanedChild at ai values:";
                    txt += $" parent.ai[{i}]: {parent.ai[i]} pGlobal.lastAIValue[{i}]: {pGlobal.lastAIValue[i]}";
                    txt.Log();
                }

                #endregion
            }
        }
        public override bool ShouldUpdatePosition(Projectile projectile) {
            //$"{projectile.S()}, update: {Main.GameUpdateCount}, localAI[0]: {projectile.localAI[0]}, localAI[1]: {projectile.localAI[1]}, ai[0]: {projectile.ai[0]}, ai[1]: {projectile.ai[1]}".Log();
            //Item item = sourceItem;
            //Player player = Main.player[projectile.owner];
            //$"{projectile.S()}, itemAnimation: {player.itemAnimation}, itemAnimationMax: {player.itemAnimationMax}".LogSimple();
            //$"{projectile.S()}, reuseDelay: {item.reuseDelay}".LogSimple();
            //$"{projectile.S()}, attackCD: {player.attackCD}".LogSimple();
            //$"({item.shoot > 0 && player.itemAnimation > 0 && player.ItemTimeIsZero && (item.useLimitPerAnimation != null && player.ItemUsesThisAnimation >= item.useLimitPerAnimation.Value)}) updateCount: {Main.GameUpdateCount} {item.S()}: itemAnimation: {player.itemAnimation}, itemTime: {player.itemTime}, ItemTimeIsZero: {player.ItemTimeIsZero.S()}, useLimitPerAnimation: {item.useLimitPerAnimation}, ItemUsesThisAnimation: {player.ItemUsesThisAnimation}, useLimitPerAnimation.Value: {(item.useLimitPerAnimation != null ? item.useLimitPerAnimation.Value : "null")}".LogSimple();
            if (!base.ShouldUpdatePosition(projectile))
                return true;

            //If the parent projectile spawned this child and conditions are met, apply the parent's speed multiplier to the parent's ai values.
            for (int i = 0; i < 2; i++) {
                float aiValue = projectile.ai[i];
                if (spawnedChild[i]) {
                    lastChildSpawnTime[i] = Main.GameUpdateCount;
                    float thisSpawnChildValue = spawnChildValue[i];
                    float thisNextValueAfterChild = nextValueAfterChild[i];

                    //Setup
                    if (!completedChildSpawnSpeedSetup[i]) {
                        //Set positive (If ai value counts up or down to spawn a projectile)
                        positive[i] = thisSpawnChildValue > thisNextValueAfterChild;
                        completedChildSpawnSpeedSetup[i] = true;
                    }

                    bool thisPositive = positive[i];
                    float inverting = thisPositive ? 1f : -1f;
                    float sizeOfAiRange = thisPositive ? thisSpawnChildValue : thisNextValueAfterChild;
                    float carryOver = speedCarryover[i];
                    float speedAddValue = inverting * (sizeOfAiRange * speed) + carryOver;
                    int valueToAdd = (int)speedAddValue;
                    speedCarryover[i] = speedAddValue % 1f;
                    projectile.ai[i] += valueToAdd;

                    //The 3 weaponProjectile weapons from vanilla spawn projectiles basid on the ai[0], but don't reset it ever.
                    //The ai[1] does reset when they are spawned, so we want to affect both.
                    if (i == 1 && weaponProjectile) {
                        projectile.ai[0] -= valueToAdd;
                    }

                    spawnedChild[i] = false;
                }

				#region Debug

				if (LogMethods.debugging) ($"PreDraw projectile: {projectile.S()} aiValue: {aiValue} lastAIValue[{i}]: {lastAIValue[i]} ai[{i}]: {projectile.ai[i]}").Log();

				#endregion

				lastAIValue[i] = aiValue;
            }

            return true;
        }
        public override void OnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit) {
            projectile.GetGlobalProjectile<WEProjectile>().UpdateProjectile(projectile);
            if (sourceItem.TryGetEnchantedItem(out EnchantedItem iGlobal)) {
                bool summonDamage = sourceItem.DamageType == DamageClass.Summon || sourceItem.DamageType == DamageClass.MagicSummonHybrid;

                //Since summoner weapons create long lasting projectiles, it can be easy to loose tracking of the item it came from.
                //If the item is cloned, it will be lost, so we need to find its location.
                if (summonDamage) {
                    bool found;
                    WEPlayer wePlayer = Main.player[projectile.owner].GetWEPlayer();
                    //lastInventoryLocation default is -1 indicating the location is unknown
                    if (lastInventoryLocation < 0) {
                        //If item location is unknown(lastInventoryLocation == -1), found = false
                        found = false;
                    }
                    else {
                        //If there is a previous known location, check that location in the player's inventory or banks
                        Item[] inventory;
                        int inventoryLocation;
                        //Determine which player inventory to look in
                        switch (lastInventoryLocation) {
                            case < 50://Player Inventory
                                inventory = wePlayer.Player.inventory;
                                inventoryLocation = lastInventoryLocation;
                                break;
                            case < 90://Bank 1, Piggy bank
                                inventory = wePlayer.Player.bank.item;
                                inventoryLocation = lastInventoryLocation - 50;
                                break;
                            case < 130://Bank 2, Vault
                                inventory = wePlayer.Player.bank2.item;
                                inventoryLocation = lastInventoryLocation - 90;
                                break;
                            case < 170://Bank 3, Defender's Forge
                                inventory = wePlayer.Player.bank3.item;
                                inventoryLocation = lastInventoryLocation - 130;
                                break;
                            case < 210://Bank 4, Void Vault
                                inventory = wePlayer.Player.bank4.item;
                                inventoryLocation = lastInventoryLocation - 170;
                                break;
                            default://enchantingTable itemSlot
                                inventory = new Item[1];
                                inventory[0] = wePlayer.enchantingTableUI?.itemSlotUI[0]?.Item;
                                inventoryLocation = 0;
                                break;
                        }

                        found = inventory[inventoryLocation].IsSameEnchantedItem(sourceItem);
                        if(found)
                            sourceItem = inventory[inventoryLocation];
                    }

                    //Look through the players inventory and banks for the item
                    if (!found) {
                        Item[] inventory = wePlayer.Player.inventory;
                        int inventoryLocation = 0;
                        for (int i = 0; i < 211; i++) {
                            //Whole inventory searched
                            if (inventoryLocation >= 40 && i >= 50) {
                                //Select the next inventory.
                                switch (i) {
                                    case < 50://Player inventory
                                        inventory = wePlayer.Player.inventory;
                                        inventoryLocation = i;
                                        break;
                                    case 50://Bank 1, Piggy bank
                                        inventory = wePlayer.Player.bank.item;
                                        inventoryLocation = i - 50;
                                        break;
                                    case 90://Bank 2, Vault
                                        inventory = wePlayer.Player.bank2.item;
                                        inventoryLocation = i - 90;
                                        break;
                                    case 130://Bank 3, Defender's Forge
                                        inventory = wePlayer.Player.bank3.item;
                                        inventoryLocation = i - 130;
                                        break;
                                    case 170://Bank 4, Void Vault
                                        inventory = wePlayer.Player.bank4.item;
                                        inventoryLocation = i - 170;
                                        break;
                                    case 210:
                                        if (wePlayer.enchantingTableUI?.itemSlotUI[0]?.Item != null) {
                                            inventory = new Item[] { wePlayer.enchantingTableUI.itemSlotUI[0].Item };
                                        }
                                        else {
                                            inventory = null;
                                        }
                                        inventoryLocation = 0;
                                        break;
                                    default://enchantingTable itemSlot
                                        inventory = null;
                                        break;
                                }
                            }

                            found = inventory != null ? EnchantedItemStaticMethods.IsSameEnchantedItem(inventory[inventoryLocation], sourceItem) : false;
                            if (found) {
                                sourceItem = inventory[inventoryLocation];
                                lastInventoryLocation = inventoryLocation;
                                break;
                            }

                            inventoryLocation++;
                        }
                    }

                    if (!found) {
                        //Item not found
                        lastInventoryLocation = -1;
                    }
                }

                //Gain xp
                sourceItem.DamageNPC(Main.player[projectile.owner], target, damage, crit);
            }
            else if (playerSource != null) {
                //Non item based projectile like the Stardust Guardian
                EnchantedItemStaticMethods.DamageNPC(null, Main.player[projectile.owner], target, damage, crit);
            }
        }
    }
}