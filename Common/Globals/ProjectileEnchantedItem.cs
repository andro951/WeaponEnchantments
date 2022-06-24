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
        //public float totalSpeedBonus;
        public Projectile parent = null;
        public bool skipOnHitEffects = false;
        //float speedAdd = 0f;
        //float speedCarryover = 0f;
        float speed;
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
            if(UtilityMethods.debugging)
            {
                for (int i = 0; i < projectile.ai.Length; i++)
                {
                    float aiValue = projectile.ai[i];
                    ($"OnSpawn projectile: {projectile.S()} aiValue: {aiValue} lastAIValue[{i}]: {lastAIValue[i]} ai[{i}]: {projectile.ai[i]}").Log();
                }
            }
            if (!sourceSet)
            {
                if (source is EntitySource_ItemUse uSource)
                {
                    if(uSource.Item != null && WEMod.IsEnchantable(uSource.Item))
                    {
                        sourceItem = uSource.Item;
                        if(sourceItem.DamageType == DamageClass.Melee)
                        {
                            //float speedBonus = sourceItem.GetGlobalItem<EnchantedItem>().totalSpeedBonus;
                            //projectile.velocity /= (1f + speedBonus);
                            projectile.velocity /= (sourceItem.A("velocity", 1f));
                        }
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
                else if(source is EntitySource_Parent parentSource && parentSource.Entity is Projectile parentProjectile)
                {
                    parent = parentProjectile;
                    TryUpdateFromParent();
                }
                else if(source is EntitySource_Misc eSource && eSource.Context != "FallingStar")
                {
                    sourceItem = FindMiscSourceItem(projectile, eSource.Context);
                    sourceSet = sourceItem != null;
                }
                else if(source is EntitySource_Parent projectilePlayerSource && projectilePlayerSource.Entity is Player player)
                {
                    playerSource = player;
                    playerSourceSet = true;
                }
                projectile.GetGlobalProjectile<ProjectileEnchantedItem>().UpdateProjectile(projectile);
                if (UtilityMethods.debugging) ($"OnSpawn(projectile: {projectile.S()}) sourceItem: {sourceItem.S()} playerSource: {playerSource.S()}").Log();
            }
            if(sourceSet)
            {
                if (UtilityMethods.PC("Splitting"))
                {
                    if (!(source is EntitySource_Parent parentSource) || !(parentSource.Entity is Projectile parentProjectile) || parentProjectile.type != projectile.type)
                    {
                        float projectileChance = sourceItem.AEI("Splitting", 0f);
                        int projectiles = (int)projectileChance;
                        projectiles += (Main.rand.NextFloat() >= projectileChance - (float)projectiles ? 1 : 0);
                        if (projectiles > 0)
                        {
                            float spread = (float)Math.PI / 10f;
                            for (int i = 0; i < projectiles; i++)
                            {
                                float rotation = (float)i - ((float)projectiles - 2f) / 2f;
                                Vector2 position = projectile.position.RotatedBy(spread * rotation);
                                Vector2 velocity = projectile.velocity;
                                Projectile.NewProjectile(projectile.GetSource_FromThis(), position, velocity, projectile.type, projectile.damage, projectile.knockBack, projectile.owner);
                            }
                        }
                    }
                }
                if (UtilityMethods.PC("InfinitePenetration"))
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
                            if (difference > 4)
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
                    if(proj.GetGlobalProjectile<ProjectileEnchantedItem>().sourceSet)
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
            return true;
        }
        public override bool ShouldUpdatePosition(Projectile projectile)
        {
            if (sourceItem != null)
            {
                //if(speedAdd != 0)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        float aiValue = projectile.ai[i];
                        //if (finishedObservationPeriod[i])
                        {
                            if (spawnedChild[i])
                            {
                                if (!positiveSet[i])
                                    positive[i] = spawnChildValue[i] > nextValueAfterChild[i];
                                projectile.ai[i] += (positive[i] ? 1 : -1) * (int)((positive[i] ? spawnChildValue[i] : nextValueAfterChild[i]) * speed);
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
                                if(UtilityMethods.debugging) ($"PreDraw projectile: {projectile.S()} aiValue: {aiValue} lastAIValue[{i}]: {lastAIValue[i]} ai[{i}]: {projectile.ai[i]}").Log();
                            }
                        }
                        /*if(aiValue != 0 || lastAIValue[i] != 0)
                            ($"PreDraw projectile: {projectile.S()} aiValue: {aiValue} lastAIValue[{i}]: {lastAIValue[i]} ai[{i}]: {projectile.ai[i]}").Log();*/
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
                if (sourceSet)
                {
                    if (sourceItem.TryGetGlobalItem(out EnchantedItem siGlobal))
                    {
                        /*damageBonus = 1f;

                        if (sourceItem.DamageType == DamageClass.Summon || sourceItem.type == ItemID.LastPrism || sourceItem.type == ItemID.CoinGun)
                        {
                            /*damageBonus += siGlobal.damageBonus;
                            damageBonus *= siGlobal.allForOneBonus;*//*
                            damageBonus = sourceItem.A("Damage", damageBonus);
                        }
                        if(sourceItem.DamageType == DamageClass.Summon)
                        {
                            //projectile.CritChance += siGlobal.critBonus;
                        }*/
                        //projectile.scale += siGlobal.lastGenericScaleBonus; ;//Update item size
                        //projectile.scale = sourceItem.A("scale", projectile.scale);
                        if (projectile.scale >= 1f && projectile.scale < sourceItem.scale || projectile.scale < 1f && projectile.scale > sourceItem.scale)
                            projectile.scale *= sourceItem.scale;
                        /*if (sourceItem.G().eStats.ContainsKey("AllForOne") || sourceItem.G().eStats.ContainsKey("InfinitePenetration"))
                        {
                            if (!projectile.usesIDStaticNPCImmunity && !projectile.usesLocalNPCImmunity)
                            {
                                projectile.usesIDStaticNPCImmunity = true;
                                projectile.idStaticNPCHitCooldown = projectile.aiStyle == 99 ? 10 : 3;
                            }
                            else if (projectile.usesIDStaticNPCImmunity && projectile.idStaticNPCHitCooldown < 1)
                                projectile.idStaticNPCHitCooldown = 3;
                            else if (projectile.usesLocalNPCImmunity && projectile.localNPCHitCooldown < 1)
                                projectile.localNPCHitCooldown = 3;
                        }*/
                        float NPCHitCooldownMultiplier = sourceItem.AEI("NPCHitCooldown", 1f);
                        /*float speed = 1f / NPCHitCooldownMultiplier;
                        speedAdd = speed - 1f;*/
                        if (projectile.minion || projectile.DamageType == DamageClass.Summon)
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
                if(sourceItem != null)
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
                if (sourceSet && sourceItem.G().eStats.ContainsKey("AllForOne") && (sourceItem.DamageType == DamageClass.Summon || sourceItem.DamageType == DamageClass.MagicSummonHybrid))
                {
                    cooldownEnd = Main.GameUpdateCount + (projectile.usesIDStaticNPCImmunity ? (int)((float)projectile.idStaticNPCHitCooldown) : projectile.usesLocalNPCImmunity ? (int)((float)projectile.localNPCHitCooldown) : sourceItem.useTime);
                    if(parent != null)
                        parent.G().cooldownEnd = cooldownEnd;
                }
            }
        }
        public override bool? CanHitNPC(Projectile projectile, NPC target)
        {
            if (sourceSet && Main.GameUpdateCount < cooldownEnd)
                return false;
            return null;
        }
        public override void ModifyDamageHitbox(Projectile projectile, ref Rectangle hitbox)
        {
            if (sourceSet)
            {
                if (projectile.minion && (projectile.scale >= 1f && projectile.scale < sourceItem.scale || projectile.scale < 1f && projectile.scale > sourceItem.scale))
                    projectile.scale *= sourceItem.scale;
                hitbox.Height = (int)Math.Round(hitbox.Height * projectile.scale);
                hitbox.Width = (int)Math.Round(hitbox.Width * projectile.scale);
            }
        }
    }
}
