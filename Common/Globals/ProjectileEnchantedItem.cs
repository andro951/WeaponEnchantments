using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Items;
using static WeaponEnchantments.Items.Enchantments;

namespace WeaponEnchantments.Common.Globals
{
    internal class ProjectileEnchantedItem : GlobalProjectile
    {
        public Item sourceItem;
        private bool sourceSet;
        public int lastInventoryLocation = -1;
        public override bool InstancePerEntity => true;
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
             if (!sourceSet)
            {
                if (source is EntitySource_ItemUse)
                {
                        sourceItem = ((EntitySource_ItemUse)source).Item;
                        sourceSet = true;
                }
                else if(source is EntitySource_ItemUse_WithAmmo)
                {
                    sourceItem = ((EntitySource_ItemUse_WithAmmo)source).Item;
                    sourceSet = true;
                }
                else if(source is EntitySource_Parent parentSource && parentSource.Entity is Projectile parentProjectile)
                {
                    sourceItem = parentProjectile.GetGlobalProjectile<ProjectileEnchantedItem>().sourceItem;
                    sourceSet = true;
                }
                if (sourceSet)
                {
                    float scale = 0f;
                    for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
                    {
                        if (!sourceItem.GetGlobalItem<EnchantedItem>().enchantments[i].IsAir && ((Enchantments)sourceItem.GetGlobalItem<EnchantedItem>().enchantments[i].ModItem).enchantmentType == EnchantmentTypeIDs.Size)
                        {
                            //projectile.knockBack += (int)(((Enchantments)sourceItem.GetGlobalItem<EnchantedItem>().enchantments[i].ModItem).enchantmentStrength * 100);
                            scale += ((Enchantments)sourceItem.GetGlobalItem<EnchantedItem>().enchantments[i].ModItem).enchantmentStrength / 2;//Only do 50% of enchantmentStrength to size
                        }
                    }
                    scale += sourceItem.GetGlobalItem<EnchantedItem>().lastGenericScaleBonus;
                    projectile.scale += scale;//Update item size
                }
            }
        }
        /*public override void Kill(Projectile projectile, int timeLeft)
        {
            base.Kill(projectile, timeLeft);
            if (sourceItem != null)
            {
                sourceItem.GetGlobalItem<EnchantedItem>().trackedProjectile = null;
                sourceItem.GetGlobalItem<EnchantedItem>().trackingProjectileThroughCloning = false;
                sourceItem = null;
                sourceSet = false;
            }
        }*/
        public override void OnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            if (target.life <= 0)//If NPC died
            {
                if(sourceItem != null)
                {
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
                                if (inventory[inventoryLocation] != null)
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
                    else
                    {
                        //sourceItem.GetGlobalItem<EnchantedItem>().KillNPC(sourceItem, target);//Have item gain xp
                    }//If any other item, 
                }
            }
        }
    }
}
