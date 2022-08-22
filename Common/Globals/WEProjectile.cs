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
    public class WEProjectile : GlobalProjectile
    {
        //Sources
        public Item sourceItem;
        private bool itemSourceSet;
        private bool ItemSourceSet {
            get => itemSourceSet;
            set {
                if(sourceItem != null)
                    itemSourceSet = value;
			}
        }
        public Player playerSource;
        public Projectile parent = null;

        //Stat changes
        public double hitCooldownEnd = 0;
        float speed;
        bool firstScaleCheck = true;
        bool reApplyScale = false;
        float initialScale = 1f;
        float referenceScale = 1f;
        float lastScaleBonus = 1f;
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
        private bool updated = false;
        public bool skipOnHitEffects = false;
        public int lastInventoryLocation = -1;
        bool weaponProjectile = false;

        public override bool InstancePerEntity => true;
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
            
            //All other sources
            if (source is EntitySource_ItemUse uSource) {
                if (uSource.Item != null && uSource.Item.TryGetEnchantedItem(out EnchantedItem uSourceGlobal)) {
                    //Set Master projectile for VortexBeater, Celeb2, Phantasm fix (Speed Enchantments)
                    if (weaponProjectile)
                        uSourceGlobal.masterProjectile = projectile;

                    sourceItem = uSource.Item;
                    ItemSourceSet = true;
                }
            }
            else if (source is EntitySource_ItemUse_WithAmmo wSource) {
                if (wSource.Item.TryGetEnchantedItem()) {
                    sourceItem = wSource.Item;
                    ItemSourceSet = true;
                }
            }
            else if (source is EntitySource_Parent parentSource && parentSource.Entity is Projectile parentProjectile && projectile.type != ProjectileID.FallingStar) {
                parent = parentProjectile;
                TryUpdateFromParent();
            }
            else if (source is EntitySource_Misc eSource && eSource.Context != "FallingStar") {
                sourceItem = FindMiscSourceItem(projectile, eSource.Context);
                ItemSourceSet = sourceItem.TryGetEnchantedItem();
            }
            else if (source is EntitySource_Parent projectilePlayerSource && projectilePlayerSource.Entity is Player pSource) {
                //Projectiles such as stardust guardian.
                playerSource = pSource;
            }

            //Update Projectile
            projectile.GetGlobalProjectile<WEProjectile>().UpdateProjectile(projectile);

			#region Debug

			if (LogMethods.debugging) ($"OnSpawn(projectile: {projectile.S()}) sourceItem: {sourceItem.S()} playerSource: {playerSource.S()}").Log();

            #endregion

            if (!ItemSourceSet)
                return;

            ActivateMultishot(projectile, source);

            //Player player = Main.player[projectile.owner];
            //Infinite Penetration
            //if (player.ContainsEStat("InfinitePenetration", sourceItem)) {
            //    projectile.penetrate = -1;
            //}
        }
		private void ActivateMultishot(Projectile projectile, IEntitySource source) {
            if (!projectile.TryGetWEPlayer(out WEPlayer wePlayer) || !wePlayer.CheckEnchantmentStats(EnchantmentStat.Multishot, out float multishotChance))
                return;

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

            if (multishotChance != 0f && !weaponProjectile && !multiShotConvertedToDamage) {

                //Multishot
                bool notAMultishotProjectile = !(source is EntitySource_Parent parentSource) || !(parentSource.Entity is Projectile parentProjectile) || parentProjectile.type != projectile.type;
                if (notAMultishotProjectile) {
                    int projectiles = (int)multishotChance;
                    float randFloat = Main.rand.NextFloat();
                    projectiles += randFloat <= multishotChance - projectiles ? 1 : 0;
                    if (projectiles > 0) {
                        float spread = (float)Math.PI / 200f;
                        bool invert = false;
                        int rotationCount = 0;
                        for (int i = 1; i <= projectiles; i++) {
                            if (!invert)
                                rotationCount++;

                            float rotation = (float)rotationCount - ((float)projectiles - 2f) / 2f;
                            if (invert)
                                rotation *= -1f;

                            //Vector2 position = projectile.position.RotatedBy(spread * rotation);
                            Vector2 position = projectile.position;
                            Vector2 velocity = projectile.velocity.RotatedBy(spread * rotation);
                            Projectile.NewProjectile(projectile.GetSource_FromThis(), position, velocity, projectile.type, projectile.damage, projectile.knockBack, projectile.owner);
                            invert = !invert;
                        }
                    }
                }
            }
        }
        public void UpdateProjectile(Projectile projectile) {
            if (updated)
                return;

            if (!ItemSourceSet) {
                if (parent != null)
                    TryUpdateFromParent();

                return;
            }

            if (!sourceItem.TryGetEnchantedItem())
                return;

            //Initial scale
            initialScale = projectile.scale;
            bool projectileScaleNotModified = projectile.scale < sourceItem.scale * ContentSamples.ProjectilesByType[projectile.type].scale;
            if (sourceItem.scale >= 1f && projectileScaleNotModified) {
                projectile.scale *= sourceItem.scale;
                lastScaleBonus = sourceItem.scale;
            }

            //Reference scale (after applying sourceItem.scale if needed)
            referenceScale = projectile.scale;

            //NPC Hit Cooldown
            if (projectile.minion || projectile.DamageType == DamageClass.Summon || weaponProjectile) {
                Item sampleItem = ContentSamples.ItemsByType[sourceItem.type];
                float sampleUseTime = sampleItem.useTime;
                float useTime = sourceItem.useTime;
                float sampleUseAnimation = sampleItem.useAnimation;
                float useAnimation = sourceItem.useAnimation;
                float allForOne = sourceItem.ContainsEStat("AllForOne") ? 4f : 1f;
                float speedMult = (sampleUseTime / useTime + sampleUseAnimation / useAnimation) / (2f * allForOne);
                speed = 1f - 1f / speedMult;
            }

            Main.player[projectile.owner].GetWEPlayer().CheckEnchantmentStats(EnchantmentStat.NPCHitCooldown, out float NPCHitCooldownMultiplier);
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
        }
        private void TryUpdateFromParent() {
            //Player source
            playerSource = parent.GetWEProjectile().playerSource;
            
            if (!parent.TryGetWEProjectile(out WEProjectile pGlobal))
                return;

            //Source Item
            sourceItem = pGlobal.sourceItem;
            ItemSourceSet = true;

            //Hit cooldown end
            hitCooldownEnd = pGlobal.hitCooldownEnd;


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

        /// <summary>
        /// Usually used for projectiles spawned by ai behavior of itself creating a new projectile. (Example Desert Tiger Staff)
        /// </summary>
        /// <param name="projectile"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Item FindMiscSourceItem(Projectile projectile, string context = "") {
            int matchs = 0;
            int bestMatch = -1;
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            List<string> projectileNames = context == "" ? projectile.Name.RemoveProjectileName().SplitString() : context.SplitString();
            for (int i = 0; i < Main.projectile.Length; i++) {
                //Find main projectile i
                Projectile mainProjectile = Main.projectile[i];
                if (mainProjectile.type == ProjectileID.None)
                    continue;

                //Find the best other projectile that matches.
                if (mainProjectile.owner == wePlayer.Player.whoAmI && mainProjectile.type != projectile.type) {
                    if (mainProjectile.GetGlobalProjectile<WEProjectile>().sourceItem.TryGetEnchantedItem()) {
                        List<string> mainProjectileNames = mainProjectile.Name.RemoveProjectileName().SplitString();
                        int checkMatches = projectileNames.CheckMatches(mainProjectileNames);
                        if (checkMatches > matchs) {
                            matchs = checkMatches;
                            bestMatch = i;
                        }
                    }
                }
            }

            return bestMatch >= 0 ? Main.projectile[bestMatch].GetGlobalProjectile<WEProjectile>().sourceItem : null;
        }
        public override bool PreDraw(Projectile projectile, ref Color lightColor) {
            if (!updated)
                UpdateProjectile(projectile);

            if (!sourceItem.TryGetEnchantedItem())
                return true;

            //If source item scale changed, update the projectile scale.
            if (sourceItem.scale == lastScaleBonus)
                return true;

            //Update the prjoectile scale.
            projectile.scale /= lastScaleBonus;
            referenceScale /= lastScaleBonus;
            projectile.scale *= sourceItem.scale;
            referenceScale *= sourceItem.scale;
            lastScaleBonus = sourceItem.scale;

            return true;
        }
        public override bool ShouldUpdatePosition(Projectile projectile) {
            if (!ItemSourceSet)
                return true;

            if (speed <= 0)
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
                if (iGlobal.eStats.ContainsKey("OneForAll")) {
                    //One For All kill its parent
                    if (parent is Projectile)
                        parent.active = false;
                }

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
                                inventory[0] = wePlayer.enchantingTableUI.itemSlotUI[0].Item;
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


                if (summonDamage && iGlobal.eStats.ContainsKey("AllForOne")) {
                    int cooldown;
					if (projectile.usesIDStaticNPCImmunity) {
                        cooldown = projectile.idStaticNPCHitCooldown;
                    }
					else if (projectile.usesLocalNPCImmunity) {
                        cooldown = projectile.localNPCHitCooldown;
                    }
					else {
                        cooldown = sourceItem.useTime;
                    }

                    hitCooldownEnd = Main.GameUpdateCount + cooldown;
                    if (parent != null)
                        parent.GetWEProjectile().hitCooldownEnd = hitCooldownEnd;
                }

                //Gain xp
                sourceItem.DamageNPC(Main.player[projectile.owner], target, damage, crit);
            }
            else if (playerSource != null) {
                //Non item based projectile like the Stardust Guardian
                EnchantedItemStaticMethods.DamageNPC(null, Main.player[projectile.owner], target, damage, crit);
            }
        }
        public override bool? CanHitNPC(Projectile projectile, NPC target) {
            if (sourceItem.TryGetEnchantedItem() && Main.GameUpdateCount < hitCooldownEnd)
                return false;

            return null;
        }
        public override void ModifyDamageHitbox(Projectile projectile, ref Rectangle hitbox) {
            if (!ItemSourceSet)
                return;

            if (firstScaleCheck) {
                firstScaleCheck = false;
                switch (projectile.type) {
                    case ProjectileID.LastPrismLaser:
                    case ProjectileID.StardustDragon1:
                    case ProjectileID.StardustDragon2:
                    case ProjectileID.StardustDragon3:
                    case ProjectileID.StardustDragon4:
                        //Re-apply scale each draw tick
                        reApplyScale = true;
                        break;
                }

                //Update initialScale if source item scale changed.
                if (Math.Abs(projectile.scale - initialScale) < Math.Abs(projectile.scale - referenceScale))
                    initialScale = projectile.scale;
            }

            //Adjust or re-adjust the projectile to the item scale
            if (reApplyScale || sourceItem.scale > 1f && projectile.scale == initialScale) {
                if (projectile.scale / lastScaleBonus >= 1f) {
                    projectile.scale /= lastScaleBonus;
                }

                projectile.scale *= sourceItem.scale;
            }

            //Excluded Projectiles from hitbox change
            switch (projectile.type) {
                case ProjectileID.LastPrismLaser:
                    return;
            }

            //Modify hitbox
            hitbox.Height = (int)Math.Round(hitbox.Height * referenceScale / initialScale);
            hitbox.Width = (int)Math.Round(hitbox.Width * referenceScale / initialScale);
            float scaleShift = (projectile.scale - 1f) / (2f * projectile.scale);
            hitbox.Y -= (int)(scaleShift * hitbox.Height);
            hitbox.X -= (int)(scaleShift * hitbox.Width);
        }
    }
}