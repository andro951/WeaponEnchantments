using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Items;
using static WeaponEnchantments.Items.AllForOneEnchantmentBasic;

namespace WeaponEnchantments.Common.Globals
{
    internal class ProjectileEnchantedItem : GlobalProjectile
    {
        public Item sourceItem;
        public Player playerSource;
        private bool sourceSet;
        private bool playerSourceSet;
        public int lastInventoryLocation = -1;
        private bool updated = false;
        public float damageBonus = 1f;
        public float totalSpeedBonus;
        private Projectile parent = null;
        public override bool InstancePerEntity => true;
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (!sourceSet)
            {
                if (source is EntitySource_ItemUse uSource)
                {
                    if(uSource.Item != null)
                    {
                        sourceItem = uSource.Item;
                        if(sourceItem.DamageType == DamageClass.Melee)
                        {
                            float speedBonus = sourceItem.GetGlobalItem<EnchantedItem>().totalSpeedBonus;
                            projectile.velocity /= (1f + speedBonus);
                        }
                        sourceSet = true;
                    }
                }
                else if(source is EntitySource_ItemUse_WithAmmo wSource)
                {
                    if (wSource.Item != null)
                    {
                        sourceItem = wSource.Item;
                        sourceSet = true;
                    }
                }
                else if(source is EntitySource_Parent parentSource && parentSource.Entity is Projectile parentProjectile)
                {
                    if (parentProjectile.GetGlobalProjectile<ProjectileEnchantedItem>()?.sourceItem != null)
                    {
                        sourceItem = parentProjectile.GetGlobalProjectile<ProjectileEnchantedItem>().sourceItem;
                        sourceSet = true;
                        parent = parentProjectile;
                    }
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
            projectile.GetGlobalProjectile<ProjectileEnchantedItem>().UpdateProjectile(projectile);
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
                        damageBonus = 1f;

                        if (sourceItem.DamageType == DamageClass.Summon || sourceItem.type == ItemID.LastPrism || sourceItem.type == ItemID.CoinGun)
                        {
                            damageBonus += siGlobal.damageBonus;
                            damageBonus *= siGlobal.allForOneBonus;
                        }
                        if(sourceItem.DamageType == DamageClass.Summon)
                        {
                            //projectile.CritChance += siGlobal.critBonus;
                        }
                        projectile.scale += siGlobal.lastGenericScaleBonus; ;//Update item size
                        /*for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
                    {
                        if (!siGlobal.enchantments[i].IsAir)
                        {
                            Enchantments enchantment = ((Enchantments)siGlobal.enchantments[i].ModItem);
                            switch ((EnchantmentTypeID)enchantment.EnchantmentType)
                            {
                                case EnchantmentTypeID.Size:
                                    scale += enchantment.EnchantmentStrength / 2;//Only do 50% of enchantmentStrength to size
                                    break;
                                case EnchantmentTypeID.AllForOne:
                                    if (sourceItem.DamageType == DamageClass.Summon || sourceItem.type == ItemID.LastPrism || sourceItem.type == ItemID.CoinGun)
                                    {
                                        allForOneMultiplier *= enchantment.EnchantmentStrength;
                                    }
                                    break;
                                case EnchantmentTypeID.Damage:
                                    if (sourceItem.DamageType == DamageClass.Summon || sourceItem.type == ItemID.LastPrism || sourceItem.type == ItemID.CoinGun)
                                    {
                                        damageBonus += enchantment.EnchantmentStrength;
                                    }
                                    break;
                            }
                        }
                    }
                    damageBonus = damageBonus * allForOneMultiplier;*/
                    
                        if (projectile.usesIDStaticNPCImmunity)
                        {
                            projectile.idStaticNPCHitCooldown = (int)((float)projectile.idStaticNPCHitCooldown * (1 + siGlobal.immunityBonus));
                        }
                        if (projectile.usesLocalNPCImmunity)
                        {
                            projectile.localNPCHitCooldown = (int)((float)projectile.localNPCHitCooldown * (1 + siGlobal.immunityBonus));
                        }
                        updated = true;
                    }
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
                    if (sourceItem.GetGlobalItem<EnchantedItem>().oneForAll)
                    {
                        parent.Kill();
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
                            if (inventory[inventoryLocation].type != sourceItem.type || wePlayer.Player.inventory[inventoryLocation].value != sourceItem.value || inventory[inventoryLocation].GetGlobalItem<EnchantedItem>().powerBoosterInstalled != sourceItem.GetGlobalItem<EnchantedItem>().powerBoosterInstalled)
                            {
                                found = false;
                            }
                            else
                            {
                                found = true;
                                sourceItem = inventory[inventoryLocation];//If itemSlot item matches sourceItem, Re-set sourceItem to the itemSlot it's in just in case
                            }
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
                                if (inventory?[inventoryLocation] != null)
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
                    Player player = Main.player[projectile.owner];
                    sourceItem.GetGlobalItem<EnchantedItem>().DamageNPC(sourceItem, player, target, damage, crit);
                }
            }
        }
    }
}
