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
    public class ProjectileEnchantedItem : GlobalProjectile
    {
        public Item sourceItem;
        public Player playerSource;
        private bool sourceSet;
        private bool playerSourceSet;
        public int lastInventoryLocation = -1;
        private bool updated = false;
        public float damageBonus = 1f;
        public double cooldownEnd = 0;
        public float[] lastAIValue = new float[] { 0f, 0f };
        public Projectile parent = null;
        public bool skipOnHitEffects = false;
        float speed;
        //float speedAdd;
        float[] speedCarryover = new float[] { 0f, 0f };
        bool firstScaleCheck = true;
        bool secondScaleCheck = true;
        bool reApplyScale = false;
        float firstAIScale = 1f;
        float initialScale = 1f;
        float referenceScale = 1f;
        float lastScaleBonus = 1f;
        public bool[] spawnedChild = { false, false };
        float[] spawnChildValue = { 0f, 0f };
        float[] nextValueAfterChild = { 0f, 0f };
        public bool[] finishedObservationPeriod = { false, false };
        int[] cyclesObserver = { 0, 0 };
        bool[] positive = { true, true };
        bool[] positiveSet = { false, false };
        public override bool InstancePerEntity => true;
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (UtilityMethods.debugging)
            {
                for (int i = 0; i < projectile.ai.Length; i++)
                {
                    float aiValue = projectile.ai[i];
                    ($"OnSpawn projectile: {projectile.S()} aiValue: {aiValue} lastAIValue[{i}]: {lastAIValue[i]} ai[{i}]: {projectile.ai[i]}").Log();
                }
            }
            if (source is EntitySource_ItemUse_WithAmmo vbSource && (vbSource.Item.type == ItemID.VortexBeater && projectile.type != ProjectileID.VortexBeater || vbSource.Item.type == ItemID.Celeb2 && projectile.type != ProjectileID.Celeb2Weapon || vbSource.Item.type == ItemID.Phantasm && projectile.type != ProjectileID.Phantasm))
			{
                if (vbSource.Item.G().masterProjectile != null)
                    source = vbSource.Item.G().masterProjectile.GetSource_FromThis();
            }
            if (source is EntitySource_ItemUse uSource)
            {
                if(uSource.Item != null && WEMod.IsEnchantable(uSource.Item))
                {
                    sourceItem = uSource.Item;
                    if(sourceItem.DamageType == DamageClass.Melee)
                    {
                        projectile.velocity /= (sourceItem.A("velocity", 1f));
                    }
                    if (projectile.type == ProjectileID.VortexBeater || projectile.type == ProjectileID.Celeb2Weapon || projectile.type == ProjectileID.Phantasm)
                        sourceItem.G().masterProjectile = projectile;
                    sourceSet = true;

                }
            }
            else if(source is EntitySource_ItemUse_WithAmmo wSource)
            {
                if (wSource.Item != null && WEMod.IsEnchantable(wSource.Item))
                {
                    sourceItem = wSource.Item;
                    sourceSet = true;
                }
            }
            else if(source is EntitySource_Parent parentSource && parentSource.Entity is Projectile parentProjectile && projectile.type != ProjectileID.FallingStar)
            {
                parent = parentProjectile;
                TryUpdateFromParent();
            }
            else if(source is EntitySource_Misc eSource && eSource.Context != "FallingStar")
            {
                sourceItem = FindMiscSourceItem(projectile, eSource.Context);
                sourceSet = sourceItem.TG();
            }
            else if(source is EntitySource_Parent projectilePlayerSource && projectilePlayerSource.Entity is Player player)
            {
                playerSource = player;
                playerSourceSet = true;
            }
            projectile.GetGlobalProjectile<ProjectileEnchantedItem>().UpdateProjectile(projectile);
            if (UtilityMethods.debugging) ($"OnSpawn(projectile: {projectile.S()}) sourceItem: {sourceItem.S()} playerSource: {playerSource.S()}").Log();
            if(sourceSet)
            {
                Player player = Main.player[projectile.owner];
                if (player.C("Splitting", sourceItem) && projectile.type != ProjectileID.VortexBeater && projectile.type != ProjectileID.Celeb2Weapon && projectile.type != ProjectileID.Phantasm)
                {
                    if (sourceItem.Name == "Shadethrower")
					{
                        projectile.usesLocalNPCImmunity = true;
                        projectile.localNPCHitCooldown = (int)(10f / sourceItem.AEI("Splitting", 1f));
					}
                    if (!(source is EntitySource_Parent parentSource) || !(parentSource.Entity is Projectile parentProjectile) || parentProjectile.type != projectile.type)
                    {
                        float projectileChance = sourceItem.AEI("Splitting", 0f);
                        int projectiles = (int)projectileChance;
                        float chance = Main.rand.NextFloat();
                        projectiles += (chance <= projectileChance - (float)projectiles ? 1 : 0);
                        if (projectiles > 0)
                        {
                            float spread = (float)Math.PI / 200f;
                            bool invert = false;
                            int rotationCount = 0;
                            for (int i = 1; i <= projectiles; i++)
                            {
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
                if (player.C("InfinitePenetration", sourceItem))
                {
                    projectile.penetrate = -1;
                }
            }
        }
        private void TryUpdateFromParent()
        {
            playerSource = parent.G().playerSource;
            if (parent.GetGlobalProjectile<ProjectileEnchantedItem>()?.sourceItem != null)
            {
                sourceItem = parent.GetGlobalProjectile<ProjectileEnchantedItem>().sourceItem;
                sourceSet = true;
                cooldownEnd = parent.G().cooldownEnd;
                //if(parent.G().speedAdd != 0f)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        if (parent.G().positiveSet[i])
                        {
                            parent.G().spawnedChild[i] = true;
                        }
                        else
                        {
                            float lastAIValue = parent.G().lastAIValue[i];
                            float ai = parent.ai[i];
                            double difference = Math.Abs(Math.Abs(ai) - Math.Abs(lastAIValue));
                            if (difference > 3)
                            {
                                parent.G().spawnedChild[i] = true;
                                parent.G().spawnChildValue[i] = lastAIValue;
                                parent.G().nextValueAfterChild[i] = ai;
                            }
                        }
                        if (UtilityMethods.debugging)
                        {
                            string txt = $"parent: {parent.S()} spanedChild at ai values:";
                            txt += $" parent.ai[{i}]: {parent.ai[i]} parent.G().lastAIValue[{i}]: {parent.G().lastAIValue[i]}";
                            txt.Log();
                        }
                    }
                }
            }
        }
        public static Item FindMiscSourceItem(Projectile projectile, string context = "")
        {
            int matchs = 0;
            int bestMatch = -1;
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            //for (int i = 0; i < wePlayer.inventoryItemRecord.Length; i++)
            {
                //Item item = wePlayer.inventoryItemRecord[i];
                //if (!item.IsAir && item.shoot > ProjectileID.None && (item.DamageType == DamageClass.Summon || item.DamageType == DamageClass.MagicSummonHybrid))
                {
                    //string name = ContentSamples.ProjectilesByType[item.shoot].Name;
                    
                }
            }
            List<string> projectileNames;
            List<string> projNames = context == "" ? projectile.Name.RemoveProjectileName().SplitString() : context.SplitString();
            int checkMatches = 0;
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.type == ProjectileID.None)
                    break;
                if (proj.owner == wePlayer.Player.whoAmI && proj.type != projectile.type)
                {
                    if(proj.GetGlobalProjectile<ProjectileEnchantedItem>().sourceItem.TG())
                    {
                        projectileNames = proj.Name.RemoveProjectileName().SplitString();
                        checkMatches = projNames.CheckMatches(projectileNames);
                        if (checkMatches > matchs)
                        {
                            matchs = checkMatches;
                            bestMatch = i;
                        }
                    }
                }
            }
            return bestMatch >= 0 ? Main.projectile[bestMatch].GetGlobalProjectile<ProjectileEnchantedItem>().sourceItem : null;
        }
        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            if (!updated)
                projectile.GetGlobalProjectile<ProjectileEnchantedItem>().UpdateProjectile(projectile);
            if (sourceItem.TG())
            {
                if (sourceItem.scale != lastScaleBonus)
                {
                    projectile.scale /= lastScaleBonus;
                    referenceScale /= lastScaleBonus;
                    projectile.scale *= sourceItem.scale;
                    referenceScale *= sourceItem.scale;
                    lastScaleBonus = sourceItem.scale;
                }
            }
            return true;
        }
        public override bool ShouldUpdatePosition(Projectile projectile)
        {
            if (sourceItem != null)
            {
                if(speed != 0)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        float aiValue = projectile.ai[i];
                        if (spawnedChild[i])
                        {
                            float thisSpawnChildValue = spawnChildValue[i];
                            float thisNextValueAfterChild = nextValueAfterChild[i];
                            if (!positiveSet[i])
                            {
                                positive[i] = thisSpawnChildValue > thisNextValueAfterChild;
                                positiveSet[i] = true;
                            }
                            bool thisPositive = positive[i];
                            float speedAddValue;
                            speedAddValue = (thisPositive ? 1f : -1f) * ((thisPositive ? thisSpawnChildValue : thisNextValueAfterChild) * speed) + speedCarryover[i];
                            int valueToAdd = (int)speedAddValue;
                            speedCarryover[i] = speedAddValue % 1f;
                            projectile.ai[i] += valueToAdd;
                            if (i == 1 && (projectile.type == ProjectileID.VortexBeater || projectile.type == ProjectileID.Celeb2Weapon || projectile.type == ProjectileID.Phantasm))
                            {
                                projectile.ai[0] -= valueToAdd;
                            }
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
                        float newValue = projectile.ai[i];
                        spawnedChild[i] = false;
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
        public void UpdateProjectile(Projectile projectile)
        {
            if (!updated)
            {
                if (sourceItem.TG())
                {
                    for (int i = 0; i < 2; i++)
                        lastAIValue[i] = projectile.ai[i];
                    if (sourceItem.TG())
                    {
                        initialScale = projectile.scale;
                        if (sourceItem.scale >= 1f && projectile.scale < sourceItem.scale * ContentSamples.ProjectilesByType[projectile.type].scale)
                        {
                            projectile.scale *= sourceItem.scale;
                            lastScaleBonus = sourceItem.scale;
                        }
                        referenceScale = projectile.scale;
                        float NPCHitCooldownMultiplier = sourceItem.AEI("NPCHitCooldown", 1f);
                        if (projectile.minion || projectile.DamageType == DamageClass.Summon || projectile.type == ProjectileID.VortexBeater || projectile.type == ProjectileID.Celeb2Weapon || projectile.type == ProjectileID.Phantasm)
                        {
                            float speedMult = ((float)ContentSamples.ItemsByType[sourceItem.type].useTime / (float)sourceItem.useTime + (float)ContentSamples.ItemsByType[sourceItem.type].useAnimation / (float)sourceItem.useAnimation) / (2f * (sourceItem.C("AllForOne") ? 4f : 1f));
                            speed = 1f - 1f / speedMult;
                            //speedAdd = speedMult - 1f;
                        }
                        if (projectile.usesLocalNPCImmunity)
                        {
                            if(NPCHitCooldownMultiplier > 1f)
                            {
                                projectile.usesIDStaticNPCImmunity = true;
                                projectile.usesLocalNPCImmunity = false;
                                projectile.idStaticNPCHitCooldown = projectile.localNPCHitCooldown;
                            }
                            else
                            {
                                if (projectile.localNPCHitCooldown > 0)
                                    projectile.localNPCHitCooldown = (int)((float)projectile.localNPCHitCooldown * NPCHitCooldownMultiplier);
                            }
                        }
                        if (projectile.usesIDStaticNPCImmunity)
                        {
                            if(projectile.idStaticNPCHitCooldown > 0)
                                projectile.idStaticNPCHitCooldown = (int)((float)projectile.idStaticNPCHitCooldown * NPCHitCooldownMultiplier);
                        }
                        updated = true;
                    }
                }
                else if(parent != null)
                {
                    TryUpdateFromParent();
                }
            }
        }
        public override void OnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            projectile.GetGlobalProjectile<ProjectileEnchantedItem>().UpdateProjectile(projectile);
            //if (target.life <= 0)//If NPC died
            {
                if(sourceItem.TG())
                {
                    if (sourceItem.G().eStats.ContainsKey("OneForAll"))
                    {
                        if(parent is Projectile)
                            parent.active = false;
                    }
                    //Since summoner weapons create long lasting projectiles, it can be easy to loose tracking of the item it came from.
                    //If the item is cloned, it will be lost, so we need to verify its location.
                    if (sourceItem.DamageType == DamageClass.Summon || sourceItem.DamageType == DamageClass.MagicSummonHybrid)//If item is a summoner weapon
                    {
                        bool found;
                        WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
                        if (lastInventoryLocation < 0)//lastInventoryLocation default is -1 indicating the location is unknown
                        {
                            found = false;
                        }//If item location is unknown(lastInventoryLocation == -1), found = false
                        else//If there is a previous known location, check that location in the player's inventory or banks
                        {
                            Item[] inventory;
                            int inventoryLocation;
                            switch (lastInventoryLocation)
                            {
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
                            /*if (inventory[inventoryLocation].type != sourceItem.type || wePlayer.Player.inventory[inventoryLocation].value != sourceItem.value || inventory[inventoryLocation].GetGlobalItem<EnchantedItem>().powerBoosterInstalled != sourceItem.GetGlobalItem<EnchantedItem>().powerBoosterInstalled)
                            {
                                found = false;
                            }
                            else
                            {
                                found = true;
                                sourceItem = inventory[inventoryLocation];//If itemSlot item matches sourceItem, Re-set sourceItem to the itemSlot it's in just in case
                            }*/
                        }
                        if (!found)
                        {
                            Item[] inventory = wePlayer.Player.inventory;
                            int inventoryLocation = 0;
                            for (int i = 0; i < 211; i++)
                            {
                                if (inventoryLocation > 39)//Only check inventory if > size of bank inventory
                                {
                                    switch (i)
                                    {
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
                                            if (inventory[inventoryLocation].GetGlobalItem<EnchantedItem>().powerBoosterInstalled == sourceItem.GetGlobalItem<EnchantedItem>().powerBoosterInstalled)
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
                                if (found)
                                {
                                    sourceItem = inventory[inventoryLocation];
                                    lastInventoryLocation = inventoryLocation;
                                    break;
                                }
                                inventoryLocation++;
                            }
                        }//Look through the players inventory and banks for the item
                        if (found)//If found the item
                        {
                            //sourceItem.GetGlobalItem<EnchantedItem>().KillNPC(sourceItem, target);//Have item gain xp
                        }
                        else
                        {
                            lastInventoryLocation = -1;//Item not found
                        }
                    }//If summoner weapon, verify it's location or search for it
                    EnchantedItem.DamageNPC(sourceItem, Main.player[projectile.owner], target, damage, crit);
                }
                else if(playerSource != null)
                {
                    EnchantedItem.DamageNPC(null, Main.player[projectile.owner], target, damage, crit);
                }
                if (sourceItem.TG() && sourceItem.G().eStats.ContainsKey("AllForOne") && (sourceItem.DamageType == DamageClass.Summon || sourceItem.DamageType == DamageClass.MagicSummonHybrid))
                {
                    cooldownEnd = Main.GameUpdateCount + (projectile.usesIDStaticNPCImmunity ? (int)((float)projectile.idStaticNPCHitCooldown) : projectile.usesLocalNPCImmunity ? (int)((float)projectile.localNPCHitCooldown) : sourceItem.useTime);
                    if(parent != null)
                        parent.G().cooldownEnd = cooldownEnd;
                }
            }
        }
        public override bool? CanHitNPC(Projectile projectile, NPC target)
        {
            if (sourceItem.TG() && Main.GameUpdateCount < cooldownEnd)
                return false;
            return null;
        }
        public override void ModifyDamageHitbox(Projectile projectile, ref Rectangle hitbox)
        {
            if (sourceItem.TG())
            {
                if (firstScaleCheck)
                {
                    firstScaleCheck = false;
                    switch (projectile.type)
                    {
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
                if (reApplyScale || sourceItem.scale > 1f && projectile.scale == initialScale)
                {
                    if (projectile.scale / lastScaleBonus >= 1f)
                    {
                        projectile.scale /= lastScaleBonus;
                    }
                    projectile.scale *= sourceItem.scale;
                }

                hitbox.Height = (int)Math.Round(hitbox.Height * referenceScale / initialScale);
                hitbox.Width = (int)Math.Round(hitbox.Width * referenceScale / initialScale);
                float scaleShift = (projectile.scale - 1f)/(2f * projectile.scale);
                hitbox.Y -= (int)(scaleShift * hitbox.Height);
                hitbox.X -= (int)(scaleShift * hitbox.Width);
                //hitbox.Y -= hitbox.Height / 2;
                //hitbox.X -= hitbox.Width / 2;
            }
        }
    }
}
