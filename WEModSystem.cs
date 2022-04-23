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
using WeaponEnchantments.UI;
using WeaponEnchantments.UI.WeaponEnchantmentUI;

namespace WeaponEnchantments
{
    public class WEModSystem : ModSystem
    {
        internal static UserInterface weModSystemUI;
        internal static UserInterface mouseoverUIInterface;
        internal static MouseoverUI mouseoverUI;
        private GameTime _lastUpdateUiGameTime;
        //public static SpriteBatch spriteBatch;
        public override void OnModLoad()//*
        {
            if (!Main.dedServ)
            {
                weModSystemUI = new UserInterface();
                mouseoverUI = new MouseoverUI();
                mouseoverUI.Activate();
                mouseoverUIInterface = new UserInterface();
                mouseoverUIInterface.SetState(mouseoverUI);
            }
        }
        public override void Unload()//*
        {
            if (!Main.dedServ)
            {
                weModSystemUI = null;

                mouseoverUIInterface = null;
                mouseoverUI = null;
            }
        }
        internal static void ToggleWeaponEnchantmentUI()//*
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            //if (wePlayer.usingEnchantingTable)
            if (weModSystemUI.CurrentState != null)
            {
                wePlayer.usingEnchantingTable = false;
                SoundEngine.PlaySound(SoundID.MenuClose);
                wePlayer.enchantingTable.Close();//move here if still needed
                wePlayer.enchantingTableUI.OnDeactivate();//move here if still needed
                weModSystemUI.SetState(null);
            }
            else
            {
                wePlayer.enchantingTableUI = new WeaponEnchantmentUI();//Defined in player instead
                wePlayer.usingEnchantingTable = true;
                SoundEngine.PlaySound(SoundID.MenuOpen);
                UIState state = new UIState();
                state.Append(wePlayer.enchantingTableUI);
                weModSystemUI.SetState(state);
                wePlayer.enchantingTable.Open();
                wePlayer.enchantingTableUI.OnActivate();
            }
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
            mouseoverUI.Update(gameTime);
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
