using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.UI;

namespace WeaponEnchantments
{
    public class WEModSystem : ModSystem
    {
        internal static UserInterface weModSystemUI;
        internal static UserInterface mouseoverUIInterface;
        private GameTime _lastUpdateUiGameTime;
        public override void OnModLoad()
        {
            if (!Main.dedServ)
            {
                weModSystemUI = new UserInterface();
                mouseoverUIInterface = new UserInterface();
            }
        }//PR
        public override void Unload()
        {
            if (!Main.dedServ)
            {
                weModSystemUI = null;
                mouseoverUIInterface = null;
            }
        }//PR
        public override void PostUpdatePlayers()
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            if (wePlayer.usingEnchantingTable)
            {
                if (wePlayer.enchantingTableUI.itemSlotUI[0].Item.IsAir)
                {
                    if (wePlayer.itemInEnchantingTable)//If item WAS in the itemSlot but it is empty now,
                    {
                        for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
                        {
                            if (wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item != null)//For each enchantment in the enchantmentSlots,
                            {
                                wePlayer.itemBeingEnchanted.GetGlobalItem<EnchantedItem>().enchantments[i] = wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item.Clone();//copy enchantments to the global item
                            }
                            wePlayer.enchantmentInEnchantingTable[i] = false;//The enchantmentSlot's PREVIOUS state is now empty(false)
                            wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item = new Item();//Delete enchantments still in enchantmentSlots(There were transfered to the global item)
                        }
                        wePlayer.itemBeingEnchanted = wePlayer.enchantingTableUI.itemSlotUI[0].Item;//Stop tracking the item that just left the itemSlot
                    }//Transfer items to global item and break the link between the global item and enchanting table itemSlots/enchantmentSlots
                    wePlayer.itemInEnchantingTable = false;//The itemSlot's PREVIOUS state is now empty(false)
                }//Check if the itemSlot is empty because the item was just taken out and transfer the mods to the global item if so
                else if(!wePlayer.itemInEnchantingTable)//If itemSlot WAS empty but now has an item in it
                {
                    wePlayer.itemBeingEnchanted = wePlayer.enchantingTableUI.itemSlotUI[0].Item;// Link the item in the table to the player so it can be updated after being taken out.
                    for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
                    {
                        if (wePlayer.enchantingTableUI.itemSlotUI[0].Item.GetGlobalItem<EnchantedItem>().enchantments[i] != null)//For each enchantment in the global item,
                        {
                            wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item = wePlayer.enchantingTableUI.itemSlotUI[0].Item.GetGlobalItem<EnchantedItem>().enchantments[i].Clone();//copy enchantments to the enchantmentSlots
                        }
                        //else
                        {
                            wePlayer.enchantingTableUI.itemSlotUI[0].Item.GetGlobalItem<EnchantedItem>().enchantments[i] = wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item;//Link global item to the enchantmentSlots
                        }
                    }
                    wePlayer.itemInEnchantingTable = true;//Set PREVIOUS state of itemSlot to having an item in it
                }//Check if itemSlot has item that was just placed there, copy the enchantments to the slots and link the slots to the global item
                for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
                {
                    if (wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item.IsAir)
                    {
                        if (wePlayer.enchantmentInEnchantingTable[i])//if enchantmentSlot HAD an enchantment in it but it was just taken out,
                        {
                            //Force global item to re-link to the enchantmentSlot instead of following the enchantment just taken out
                            wePlayer.enchantingTableUI.itemSlotUI[0].Item.GetGlobalItem<EnchantedItem>().enchantments[i] = wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item;
                        }
                        wePlayer.enchantmentInEnchantingTable[i] = false;//Set PREVIOUS state of enchantmentSlot to empty(false)
                    }
                    else if(wePlayer.enchantingTableUI.itemSlotUI[0].Item.GetGlobalItem<EnchantedItem>().enchantments[i] != wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item)
                    {
                        wePlayer.enchantingTableUI.itemSlotUI[0].Item.GetGlobalItem<EnchantedItem>().enchantments[i] = wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item;
                    }//If player swapped enchantments (without removing the previous one in the enchantmentSlot) Force global item to re-link to the enchantmentSlot instead of following the enchantment just taken out
                    else if (!wePlayer.enchantmentInEnchantingTable[i])
                    {
                        wePlayer.enchantmentInEnchantingTable[i] = true;//Set PREVIOUS state of enchantmentSlot to has an item in it(true)
                        wePlayer.enchantingTableUI.itemSlotUI[0].Item.GetGlobalItem<EnchantedItem>().enchantments[i] = wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item;//Force link to enchantmentSlot just in case
                    }//If it WAS empty but isn't now, re-link global item to enchantmentSlot just in case
                }//Check if enchantments are added/removed from enchantmentSlots and re-link global item to enchantmentSlot
                if (!wePlayer.Player.IsInInteractionRangeToMultiTileHitbox(wePlayer.Player.chestX, wePlayer.Player.chestY) || wePlayer.Player.chest != -1)
                {
                    CloseWeaponEnchantmentUI();
                }//If player is too far away, close the enchantment table

            }//If enchanting table is open, check item(s) and enchantments in it every tick
        }//If enchanting table is open, check item(s) and enchantments in it every tick 
        internal static void CloseWeaponEnchantmentUI()//Check on tick if too far or !wePlayer.Player.chest == -1
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            wePlayer.usingEnchantingTable = false;//Stop checking enchantingTable slots
            SoundEngine.PlaySound(SoundID.MenuClose);
            wePlayer.enchantingTable.Close();
            wePlayer.enchantingTableUI.OnDeactivate();//Store items left in enchanting table to player

            if (WeaponEnchantmentUI.PR)
            {
                weModSystemUI.SetState(null);
            }//PR
            Recipe.FindRecipes();
        }
        internal static void OpenWeaponEnchantmentUI()
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            //wePlayer.enchantingTableUI = new WeaponEnchantmentUI();//Defined in player instead
            wePlayer.usingEnchantingTable = true;
            SoundEngine.PlaySound(SoundID.MenuOpen);

            if (WeaponEnchantmentUI.PR)
            {
                UIState state = new UIState();
                state.Append(wePlayer.enchantingTableUI);
                weModSystemUI.SetState(state);
            }

            wePlayer.enchantingTable.Open();
            wePlayer.enchantingTableUI.OnActivate();
        }
        
        public override void PreSaveAndQuit()//*
        {
            weModSystemUI.SetState(null);
        }
        public override void UpdateUI(GameTime gameTime)//*
        {
            _lastUpdateUiGameTime = gameTime;
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            //if (wePlayer.usingEnchantingTable)
            if(weModSystemUI?.CurrentState != null)
            {
                weModSystemUI.Update(gameTime);
                //wePlayer.enchantingTableUI.DrawSelf(Main.spriteBatch);
            }
            //mouseoverUI.Update(gameTime);
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)//*
        {
            int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Over"));
            if (index != -1)
            {
                layers.Insert
                (
                    ++index, 
                    new LegacyGameInterfaceLayer
                    (
                        "WeaponEnchantments: Mouse Over", 
                        delegate 
                        { 
                            if (_lastUpdateUiGameTime != null && mouseoverUIInterface?.CurrentState != null) 
                            { 
                                mouseoverUIInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime); 
                            } return true; 
                        }, 
                        InterfaceScaleType.UI
                     )
                );
            }
            index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));//++
            if (index != -1)//++
            {
                layers.Insert(index, new LegacyGameInterfaceLayer(//++
                    "WeaponEnchantments: ",//++
                    delegate//++
                    {
                        if (_lastUpdateUiGameTime != null && weModSystemUI?.CurrentState != null)//++
                        {
                            weModSystemUI.Draw(Main.spriteBatch, _lastUpdateUiGameTime);//++
                        }
                        return true;//++
                    },
                    InterfaceScaleType.UI)//++
                );
            }
        }
    }
}
