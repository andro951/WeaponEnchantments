using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common;

namespace WeaponEnchantments.Common.Globals
{
    public class WEProjectile : GlobalProjectile
    {
        //Sources
        public Item sourceItem;
        private bool itemSourceSet;
        public Player playerSource;
        public Projectile parent = null;

        //private bool playerSourceSet;

        //Stat changes
        public float damageBonus = 1f; //Never actually set.  I think this was from some minions not being affected by damage?  Done delete until tested.
        public double hitCooldownEnd = 0;
        float speed;
        //float speedAdd;
        bool firstScaleCheck = true;
        //bool secondScaleCheck = true;
        bool reApplyScale = false;
        //float firstAIScale = 1f;
        float initialScale = 1f;
        float referenceScale = 1f;
        float lastScaleBonus = 1f;

        //Attack speed tracking
        public float[] lastAIValue = new float[] { 0f, 0f };
        float[] speedCarryover = new float[] { 0f, 0f };
        public bool[] spawnedChild = { false, false };
        float[] spawnChildValue = { 0f, 0f };
        float[] nextValueAfterChild = { 0f, 0f };
        long[] lastChildSpawnTime = { 0, 0 };
        //public bool[] finishedObservationPeriod = { false, false };
        //int[] cyclesObserver = { 0, 0 };
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

			if (UtilityMethods.debugging)
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
                    if (vbSource.Item.G().masterProjectile != null)
                        source = vbSource.Item.G().masterProjectile.GetSource_FromThis();
                }
            }
            
            //All other sources
            if (source is EntitySource_ItemUse uSource) {
                if (uSource.Item != null && uSource.Item.TG()) {
                    sourceItem = uSource.Item;
                    itemSourceSet = true;
                    //Set Master projectile for VortexBeater, Celeb2, Phantasm fix (Speed Enchantments)
                    if (weaponProjectile)
                        sourceItem.G().masterProjectile = projectile;
                }
            }
            else if (source is EntitySource_ItemUse_WithAmmo wSource) {
                if (wSource.Item != null && wSource.Item.TG()) {
                    sourceItem = wSource.Item;
                    itemSourceSet = true;
                }
            }
            else if (source is EntitySource_Parent parentSource && parentSource.Entity is Projectile parentProjectile && projectile.type != ProjectileID.FallingStar) {
                parent = parentProjectile;
                TryUpdateFromParent();
            }
            else if (source is EntitySource_Misc eSource && eSource.Context != "FallingStar") {
                sourceItem = FindMiscSourceItem(projectile, eSource.Context);
                itemSourceSet = sourceItem.TG();
            }
            else if (source is EntitySource_Parent projectilePlayerSource && projectilePlayerSource.Entity is Player pSource) {
                //Projectiles such as stardust guardian.
                playerSource = pSource;
            }

            //Update Projectile
            projectile.GetGlobalProjectile<WEProjectile>().UpdateProjectile(projectile);

			#region Debug

			if (UtilityMethods.debugging) ($"OnSpawn(projectile: {projectile.S()}) sourceItem: {sourceItem.S()} playerSource: {playerSource.S()}").Log();

            #endregion

            if (!itemSourceSet)
                return;

			//Multishot
            Player player = Main.player[projectile.owner];
            float sultishotChance = sourceItem.AEI("Multishot", 0f);
            if (sultishotChance != 0f && !weaponProjectile) {
                //Shadethrower fix
                if (sourceItem.Name == "Shadethrower") {
                    projectile.usesLocalNPCImmunity = true;
                    projectile.localNPCHitCooldown = (int)Math.Round(10f / (1f + sultishotChance));
                }

                //Multishot
                bool notAMultishotProjectile = !(source is EntitySource_Parent parentSource) || !(parentSource.Entity is Projectile parentProjectile) || parentProjectile.type != projectile.type;
                if (notAMultishotProjectile) {
                    int projectiles = (int)sultishotChance;
                    float randFloat = Main.rand.NextFloat();
                    projectiles += randFloat <= sultishotChance - projectiles ? 1 : 0;
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

            //Infinite Penetration
            if (player.C("InfinitePenetration", sourceItem)) {
                projectile.penetrate = -1;
            }
        }
        public void UpdateProjectile(Projectile projectile) {
            if (updated)
                return;

            if (!itemSourceSet) {
                if (parent != null)
                    TryUpdateFromParent();

                return;
            }

            if (!sourceItem.TG())
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
            float NPCHitCooldownMultiplier = sourceItem.AEI("NPCHitCooldown", 1f);
            if (projectile.minion || projectile.DamageType == DamageClass.Summon || weaponProjectile) {
                Item sampleItem = ContentSamples.ItemsByType[sourceItem.type];
                float sampleUseTime = sampleItem.useTime;
                float useTime = sourceItem.useTime;
                float sampleUseAnimation = sampleItem.useAnimation;
                float useAnimation = sourceItem.useAnimation;
                float allForOne = sourceItem.C("AllForOne") ? 4f : 1f;
                float speedMult = (sampleUseTime / useTime + sampleUseAnimation / useAnimation) / (2f * allForOne);
                speed = 1f - 1f / speedMult;
            }

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
            playerSource = parent.G().playerSource;
            
            if (!parent.TG(out WEProjectile pGlobal))
                return;

            //Source Item
            sourceItem = pGlobal.sourceItem;
            itemSourceSet = true;

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

				if (UtilityMethods.debugging) {
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
            List<string> projectileNames;
            List<string> projNames = context == "" ? projectile.Name.RemoveProjectileName().SplitString() : context.SplitString();
            int checkMatches = 0;
            for (int i = 0; i < Main.projectile.Length; i++) {
                Projectile proj = Main.projectile[i];
                if (proj.type == ProjectileID.None)
                    break;

                if (proj.owner == wePlayer.Player.whoAmI && proj.type != projectile.type) {
                    if (proj.GetGlobalProjectile<WEProjectile>().sourceItem.TG()) {
                        projectileNames = proj.Name.RemoveProjectileName().SplitString();
                        checkMatches = projNames.CheckMatches(projectileNames);
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
                projectile.GetGlobalProjectile<WEProjectile>().UpdateProjectile(projectile);

            if (sourceItem.TG()) {
                if (sourceItem.scale != lastScaleBonus) {
                    projectile.scale /= lastScaleBonus;
                    referenceScale /= lastScaleBonus;
                    projectile.scale *= sourceItem.scale;
                    referenceScale *= sourceItem.scale;
                    lastScaleBonus = sourceItem.scale;
                }
            }

            return true;
        }
        public override bool ShouldUpdatePosition(Projectile projectile) {
            if (sourceItem != null) {
                if (speed != 0) {
                    for (int i = 0; i < 2; i++) {
                        float aiValue = projectile.ai[i];
                        if (spawnedChild[i]) {
                            lastChildSpawnTime[i] = Main.GameUpdateCount;
                            float thisSpawnChildValue = spawnChildValue[i];
                            float thisNextValueAfterChild = nextValueAfterChild[i];
                            if (!completedChildSpawnSpeedSetup[i]) {
                                positive[i] = thisSpawnChildValue > thisNextValueAfterChild;
                                completedChildSpawnSpeedSetup[i] = true;
                            }
                            bool thisPositive = positive[i];
                            float speedAddValue;
                            speedAddValue = (thisPositive ? 1f : -1f) * ((thisPositive ? thisSpawnChildValue : thisNextValueAfterChild) * speed) + speedCarryover[i];
                            int valueToAdd = (int)speedAddValue;
                            speedCarryover[i] = speedAddValue % 1f;
                            projectile.ai[i] += valueToAdd;
                            if (i == 1 && (projectile.type == ProjectileID.VortexBeater || projectile.type == ProjectileID.Celeb2Weapon || projectile.type == ProjectileID.Phantasm)) {
                                projectile.ai[0] -= valueToAdd;
                            }
                            float newValue = projectile.ai[i];
                            spawnedChild[i] = false;
                        }


                        /*if (speed < 1f)
                        {
                            projectile.ai[i] += (positive[i] ? 1 : -1) * (int)(spawnChildValue[i] * (1f - 1f / speed));
                            /*if (positive[i])
                                projectile.ai[i] += ;
                            else
                                projectile.ai[i] -= (int)(nextValueAfterChild[i] * speedAdd);*//*
                        }
                        else
                        {
                            if (positive[i])
                            {

                                projectile.ai[i] += (int)(spawnChildValue[i] * speedAdd);
                            }
                            else
                            {
                                projectile.ai[i] -= (int)(nextValueAfterChild[i] * speedAdd);
                            }
                        }*/

                        /*speedCarryover += speedAdd;
                        //if (aiValue > controlValue)
                        {
                            if (speedCarryover > 1f)
                            {
                                float value = (float)(int)speedCarryover;
                                speedCarryover -= value;
                                projectile.ai[i] += value * (positive[i] ? 1f : -1f);
                            }
                            else if (speedCarryover < -1f)
                            {
                                speedCarryover += 1f;
                                projectile.ai[i] -= 1f * (positive[i] ? 1f : -1f);
                            }

                        }
                        //projectile.ai[0] = 61f;
                        //projectile.ai[1] = 0f;
                        ;*/
                        if (UtilityMethods.debugging) ($"PreDraw projectile: {projectile.S()} aiValue: {aiValue} lastAIValue[{i}]: {lastAIValue[i]} ai[{i}]: {projectile.ai[i]}").Log();
                        /*else if(projectile.type == ProjectileID.VortexBeater)
						{
                            if(i == 0)
                            {
                                speedCarryover += speedAdd;
                                //if (aiValue > controlValue)
                                {
                                    if (speedCarryover > 1f)
                                    {
                                        float value = (float)(int)speedCarryover;
                                        speedCarryover -= value;
                                        projectile.ai[i] += value;
                                    }
                                    else if (speedCarryover < -1f)
                                    {
                                        speedCarryover += 1f;
                                        projectile.ai[i] -= 1f;
                                    }
                                }
                            }
							else
							{

							}
                        }*/
                        /*if(aiValue != 0 || lastAIValue[i] != 0)
                            ($"PreDraw projectile: {projectile.S()} aiValue: {aiValue} lastAIValue[{i}]: {lastAIValue[i]} ai[{i}]: {projectile.ai[i]}").Log();*/
                        /*if (i == 0 && (sourceItem.type == ItemID.VortexBeater || sourceItem.type == ItemID.Celeb2 || sourceItem.type == ItemID.Phantasm))
						{
                            speedCarryover[i] += speed;
                            int speedToAdd = (int)speedCarryover[i];
							speedCarryover[i] -= speedToAdd;
                            projectile.ai[0] += speedToAdd;
                        }*/
                        lastAIValue[i] = aiValue;
                    }
                }
            }

            return true;
        }
        public override void OnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit) {
            projectile.GetGlobalProjectile<WEProjectile>().UpdateProjectile(projectile);
            if (sourceItem.TG()) {
                if (sourceItem.G().eStats.ContainsKey("OneForAll")) {
                    if (parent is Projectile)
                        parent.active = false;
                }
                //Since summoner weapons create long lasting projectiles, it can be easy to loose tracking of the item it came from.
                //If the item is cloned, it will be lost, so we need to verify its location.
                if (sourceItem.DamageType == DamageClass.Summon || sourceItem.DamageType == DamageClass.MagicSummonHybrid) {
                    bool found;
                    WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
                    //lastInventoryLocation default is -1 indicating the location is unknown
                    if (lastInventoryLocation < 0) {
                        found = false;
                    }//If item location is unknown(lastInventoryLocation == -1), found = false
                    else {
                        //If there is a previous known location, check that location in the player's inventory or banks
                        Item[] inventory;
                        int inventoryLocation;
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
                        }//Determine which player inventory to look in
                        found = inventory[inventoryLocation].IsSameEnchantedItem(sourceItem);
                        sourceItem = found ? inventory[inventoryLocation] : sourceItem;
                        /*if (inventory[inventoryLocation].type != sourceItem.type || wePlayer.Player.inventory[inventoryLocation].value != sourceItem.value || inventory[inventoryLocation].G().powerBoosterInstalled != sourceItem.G().powerBoosterInstalled)
                        {
                            found = false;
                        }
                        else
                        {
                            found = true;
                            sourceItem = inventory[inventoryLocation];//If itemSlot item matches sourceItem, Re-set sourceItem to the itemSlot it's in just in case
                        }*/
                    }
                    if (!found) {
                        Item[] inventory = wePlayer.Player.inventory;
                        int inventoryLocation = 0;
                        for (int i = 0; i < 211; i++) {
                            //Only check inventory if > size of bank inventory
                            if (inventoryLocation > 39) {
                                switch (i) {
                                    case < 50://Player inventory
                                        inventory = wePlayer.Player.inventory;
                                        inventoryLocation = i;
                                        break;
                                    case < 90://Bank 1, Piggy bank
                                        inventory = wePlayer.Player.bank.item;
                                        inventoryLocation = i - 50;
                                        break;
                                    case < 130://Bank 2, Vault
                                        inventory = wePlayer.Player.bank2.item;
                                        inventoryLocation = i - 90;
                                        break;
                                    case < 170://Bank 3, Defender's Forge
                                        inventory = wePlayer.Player.bank3.item;
                                        inventoryLocation = i - 130;
                                        break;
                                    case < 210://Bank 4, Void Vault
                                        inventory = wePlayer.Player.bank4.item;
                                        inventoryLocation = i - 170;
                                        break;
                                    default://enchantingTable itemSlot
                                        inventory = new Item[1];
                                        if (wePlayer.enchantingTableUI?.itemSlotUI?[0]?.Item != null)
                                        {
                                            inventory[0] = wePlayer.enchantingTableUI.itemSlotUI[0].Item;
                                        }
                                        else
                                        {
                                            inventory = null;
                                        }
                                        inventoryLocation = 0;
                                        break;
                                }//Determine which player inventory to look in
                            }
                            /*if (inventory?[inventoryLocation] != null)
                            {
                                if (!inventory[inventoryLocation].IsAir)
                                {
                                    if (inventory[inventoryLocation].type == sourceItem.type)
                                    {
                                        if (inventory[inventoryLocation].G().powerBoosterInstalled == sourceItem.G().powerBoosterInstalled)
                                        {
                                            if (inventory[inventoryLocation].value == sourceItem.value)
                                            {
                                                lastInventoryLocation = i;
                                                sourceItem = inventory[inventoryLocation];
                                                found = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }*/
                            found = UtilityMethods.IsSameEnchantedItem(inventory[inventoryLocation], sourceItem);
                            if (found) {
                                sourceItem = inventory[inventoryLocation];
                                lastInventoryLocation = inventoryLocation;
                                break;
                            }
                            inventoryLocation++;
                        }
                    }//Look through the players inventory and banks for the item
                    if (found) {
                        //If found the item
                        //sourceItem.G().KillNPC(sourceItem, target);//Have item gain xp
                    }
                    else {
                        lastInventoryLocation = -1;//Item not found
                    }
                }//If summoner weapon, verify it's location or search for it

                sourceItem.DamageNPC(Main.player[projectile.owner], target, damage, crit);
            }
            else if (playerSource != null) {
                EnchantedItemStaticMethods.DamageNPC(null, Main.player[projectile.owner], target, damage, crit);
            }


            if (sourceItem.TG() && sourceItem.G().eStats.ContainsKey("AllForOne") && (sourceItem.DamageType == DamageClass.Summon || sourceItem.DamageType == DamageClass.MagicSummonHybrid)) {
                hitCooldownEnd = Main.GameUpdateCount + (projectile.usesIDStaticNPCImmunity ? (int)((float)projectile.idStaticNPCHitCooldown) : projectile.usesLocalNPCImmunity ? (int)((float)projectile.localNPCHitCooldown) : sourceItem.useTime);
                if (parent != null)
                    parent.G().hitCooldownEnd = hitCooldownEnd;
            }
        }
        public override bool? CanHitNPC(Projectile projectile, NPC target) {
            if (sourceItem.TG() && Main.GameUpdateCount < hitCooldownEnd)
                return false;

            return null;
        }
        public override void ModifyDamageHitbox(Projectile projectile, ref Rectangle hitbox) {
            if (sourceItem.TG()) {
                if (firstScaleCheck) {
                    firstScaleCheck = false;
                    switch (projectile.type) {
                        case ProjectileID.LastPrismLaser:
                        case ProjectileID.StardustDragon1:
                        case ProjectileID.StardustDragon2:
                        case ProjectileID.StardustDragon3:
                        case ProjectileID.StardustDragon4:
                            reApplyScale = true;
                            break;
                    }
                    if (Math.Abs(projectile.scale - initialScale) < Math.Abs(projectile.scale - referenceScale))
                        initialScale = projectile.scale;
                }
                if (reApplyScale || sourceItem.scale > 1f && projectile.scale == initialScale) {
                    if (projectile.scale / lastScaleBonus >= 1f) {
                        projectile.scale /= lastScaleBonus;
                    }
                    projectile.scale *= sourceItem.scale;
                }

                switch (projectile.type) {
                    case ProjectileID.LastPrismLaser:
                        return;
                }//Excluded Projectiles

                hitbox.Height = (int)Math.Round(hitbox.Height * referenceScale / initialScale);
                hitbox.Width = (int)Math.Round(hitbox.Width * referenceScale / initialScale);
                float scaleShift = (projectile.scale - 1f) / (2f * projectile.scale);
                hitbox.Y -= (int)(scaleShift * hitbox.Height);
                hitbox.X -= (int)(scaleShift * hitbox.Width);
                //hitbox.Y -= hitbox.Height / 2;
                //hitbox.X -= hitbox.Width / 2;
            }
        }
    }
}