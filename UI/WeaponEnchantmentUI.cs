using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;
using Terraria.UI;
using Terraria.UI.Chat;
using Terraria.UI.Gamepad;
using WeaponEnchantments;
using WeaponEnchantments.Items;
using WeaponEnchantments.Common.Globals;
using System;

namespace WeaponEnchantments.UI
{
    public class ConfirmationUI : UIPanel
    {
        public class ConfirmationButtonID
        {
            public const int Yes = 0;
            public const int No = 1;
            public const int Count = 2;
        }//LootAll = 0, Offer = 1
        public static string[] ConfirmationButtonNames = new string[] { "Yes", "No" };
        private UIText promptText;
        private UIPanel[] confirmationButton = new UIPanel[ConfirmationButtonID.Count];
        private List<UIPanel> confirmationPanels;//PR
        public static bool offered = false;
        private readonly static Color red = new Color(171, 76, 73);
        private readonly static Color hoverRed = new Color(184, 103, 100);
        private readonly static Color bgColor = new Color(73, 94, 171);//Background UI color
        private readonly static Color hoverColor = new Color(100, 118, 184);//Button hover color
        public static int[] xpTiers = { 100, 400, 1600, 6400, 25600 };

        internal const int width = 680;
        internal const int height = 155;

        internal int RelativeLeft => Main.screenWidth / 2 - width / 2;
        internal int RelativeTop => Main.screenHeight / 2 + 42; //Half the player height on 200% zoom
        public override void OnInitialize()
        {
            if (WeaponEnchantmentUI.PR)
            {
                WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
                Width.Pixels = width;
                Height.Pixels = height;
                Top.Pixels = int.MaxValue / 2;
                Left.Pixels = int.MaxValue / 2 + 100;
                BackgroundColor = red;

                confirmationPanels = new List<UIPanel>();

                float nextElementY = -PaddingTop / 2;
                promptText = new UIText("Are you sure you want to PERMENANTLY DESTROY your\nlevel " + wePlayer.enchantingTableUI.itemSlotUI[0].Item.GetGlobalItem<EnchantedItem>().level.ToString() + " " + wePlayer.enchantingTableUI.itemSlotUI[0].Item.Name + "\nIn exchange for Containment Fragments and Essence?\n(Based on item value/experience.  Enchantments will be returned.)")
                {
                    Top = { Pixels = nextElementY + 15 },
                    Left = { Pixels = 70 },
                    HAlign = 0.5f
                };//Confirmation label
                Append(promptText);
                nextElementY += 20;

                nextElementY += 50;
                float ratioFromCenter = 0.22f;

                //Yes Button
                confirmationButton[ConfirmationButtonID.Yes] = new UIPanel()
                {
                    Top = { Pixels = nextElementY },
                    Left = { Pixels = -66 - 56 },
                    Width = { Pixels = 100f },
                    Height = { Pixels = 30f },
                    HAlign = 0.5f - ratioFromCenter,
                    BackgroundColor = red
                };
                confirmationButton[ConfirmationButtonID.Yes].OnClick += (evt, element) => { ConfirmOffer(); };
                UIText yesButtonText = new UIText("Yes")
                {
                    Top = { Pixels = -4f },
                    Left = { Pixels = -6f }
                };
                confirmationButton[ConfirmationButtonID.Yes].Append(yesButtonText);
                Append(confirmationButton[ConfirmationButtonID.Yes]);
                confirmationPanels.Add(confirmationButton[ConfirmationButtonID.Yes]);

                nextElementY += 35;

                //No Button
                confirmationButton[ConfirmationButtonID.No] = new UIPanel()
                {
                    Top = { Pixels = nextElementY },
                    Left = { Pixels = -66 - 56 },
                    Width = { Pixels = 100f },
                    Height = { Pixels = 30f },
                    HAlign = 0.5f - ratioFromCenter,
                    BackgroundColor = bgColor
                };
                confirmationButton[ConfirmationButtonID.No].OnClick += (evt, element) => { DeclineOffer(); };
                UIText noButtonText = new UIText("No")
                {
                    Top = { Pixels = -4f },
                    Left = { Pixels = -6f }
                };
                confirmationButton[ConfirmationButtonID.No].Append(noButtonText);
                Append(confirmationButton[ConfirmationButtonID.No]);
                confirmationPanels.Add(confirmationButton[ConfirmationButtonID.No]);
            }//PetRenaimer based UI
        }//Set up PR UI
        private static void DeclineOffer()
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            WEModSystem.promptInterface.SetState(null);
            UIState state = new UIState();
            state.Append(wePlayer.enchantingTableUI);
            WEModSystem.weModSystemUI.SetState(state);
        }
        private static void ConfirmOffer()
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            WEModSystem.promptInterface.SetState(null);
            UIState state = new UIState();
            state.Append(wePlayer.enchantingTableUI);
            WEModSystem.weModSystemUI.SetState(state);
            bool stop = false;
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
            {
                if (!wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item.IsAir)
                {
                    wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item.position = wePlayer.Player.Center;
                    wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item = wePlayer.Player.GetItem(Main.myPlayer, wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item, GetItemSettings.LootAllSettings);
                }
                if (!wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item.IsAir) { stop = true; }//Player didn't have enough space in their inventory to take all enchantments
            }//Take all enchantments first
            if (!stop)
            {
                int xp = wePlayer.enchantingTableUI.itemSlotUI[0].Item.GetGlobalItem<EnchantedItem>().experience;
                int value = wePlayer.enchantingTableUI.itemSlotUI[0].Item.value;
                int numberEssenceRecieved;
                int xpCounter = (int)Math.Round(xp * (0.6f + 0.1f * wePlayer.enchantingTableTier));
                for(int tier = EnchantingTable.maxEssenceItems - 1; tier >= 0; tier--)
                {
                    numberEssenceRecieved = xpCounter / xpTiers[tier];
                    xpCounter %= xpTiers[tier];
                    if(xpCounter < 100 && xpCounter > 0 && tier == 0)
                    {
                        xpCounter = 0;
                        numberEssenceRecieved += 1;
                    }
                    if (wePlayer.enchantingTableUI.essenceSlotUI[tier].Item.IsAir)
                    {
                        wePlayer.enchantingTableUI.essenceSlotUI[tier].Item.type = EnchantmentEssence.IDs[tier];
                        wePlayer.enchantingTableUI.essenceSlotUI[tier].Item.stack += numberEssenceRecieved;
                    }
                    else
                    {
                        wePlayer.enchantingTableUI.essenceSlotUI[tier].Item.stack += numberEssenceRecieved;
                    }
                }
                int stack = (int)Math.Round((float)value / ModContent.GetModItem(ModContent.ItemType<ContainmentFragment>()).Item.value);
                if (stack == 0)
                {
                    stack = 1;
                }
                Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), ModContent.ItemType<ContainmentFragment>(), stack);
                wePlayer.enchantingTableUI.itemSlotUI[0].Item = new Item();
                offered = true;
                SoundEngine.PlaySound(SoundID.Grab);
            }
        }//Consume item to upgrade table or get resources
        public override void Update(GameTime gameTime)
        {
            Left.Pixels = RelativeLeft + 100;//PR
            Top.Pixels = RelativeTop;//PR

            foreach (var panel in confirmationPanels)
            {
                if (panel.BackgroundColor == bgColor || panel.BackgroundColor == hoverColor)
                {
                    panel.BackgroundColor = panel.IsMouseHovering ? hoverColor : bgColor;
                }
                else if (panel.BackgroundColor == red || panel.BackgroundColor == hoverRed)
                {
                    panel.BackgroundColor = panel.IsMouseHovering ? hoverRed : red;
                }
            }//Change button color if hovering
        }//PR
    }

    public class WeaponEnchantmentUI : UIPanel
    {
        public class ButtonID
        {
            public const int LootAll = 0;
            public const int Offer = 1;
            public const int xp0 = 0;
            public const int xp1 = 1;
            public const int xp2 = 2;
            public const int xp3 = 3;
            public const int xp4 = 4;
            public const int Count = 7;
            public static int[]xps = new int[] {xp0, xp1, xp2, xp3, xp4};
		}//LootAll = 0, Offer = 1
        public class ItemSlotContext
        {
            public const int Item = 0;
            public const int Enchantment = 1;
            public const int Essence = 2;
        }//Item = 0, Enchantment = 1, Essence = 3

        public const bool PR = true;//Used to toggle between my UI in progress and the UI based on PetRenaimer mod

		public static string[] ButtonNames = new string[] { "Enchant", "Disenchant", "Offer" };
        public const float buttonScaleMinimum = 0.75f;//my UI
        public const float buttonScaleMaximum = 1f;//my UI
        public static float[] ButtonScale = new float[ButtonID.Count];//my UI
        public static bool[] ButtonHovered = new bool[ButtonID.Count];//my UI
        public static bool needToQuickStack;

        private UIText titleText;//PR
        private UIPanel[] button = new UIPanel[ButtonID.Count];//PR
        private List<UIPanel> panels;//PR
        public WEUIItemSlot[] itemSlotUI = new WEUIItemSlot[EnchantingTable.maxItems];//PR
        public WEUIItemSlot[] enchantmentSlotUI = new WEUIItemSlot[EnchantingTable.maxEnchantments];//PR
        public WEUIItemSlot[] essenceSlotUI = new WEUIItemSlot[EnchantingTable.maxEssenceItems];//PR

        private readonly static Color red = new Color(171, 76, 73);
        private readonly static Color hoverRed = new Color(184, 103, 100);
        private readonly static Color bgColor = new Color(73, 94, 171);//Background UI color
        private readonly static Color hoverColor = new Color(100, 118, 184);//Button hover color

        internal const int width = 480;
        internal const int height = 155;

        internal int RelativeLeft => Main.screenWidth / 2 - width / 2;
        internal int RelativeTop => Main.screenHeight / 2 + 42; //Half the player height on 200% zoom

        //internal bool firstDraw = true;//PR but works without it

        public override void OnInitialize()
        {
            if (PR)
            {
                WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
                Width.Pixels = width;
                Height.Pixels = height;
                Top.Pixels = int.MaxValue / 2;
                Left.Pixels = int.MaxValue / 2;

                panels = new List<UIPanel>();

                float nextElementY = -PaddingTop / 2;

                titleText = new UIText("Item           Enchantments      Utility  ")
                {
                    Top = { Pixels = nextElementY },
                    Left = { Pixels = 0 },
                    HAlign = 0.5f
                };//UI slot labels
                Append(titleText);
                nextElementY += 20;

                for (int i = 0; i < EnchantingTable.maxItems; i++)
                {
                    wePlayer.enchantingTableUI.itemSlotUI[i] = new WEUIItemSlot(17, ItemSlotContext.Item)
                    {
                        Left = { Pixels = -145f },
                        Top = { Pixels = nextElementY },
                        HAlign = 0.5f
                    };//ItemSlot(s)
                    wePlayer.enchantingTableUI.itemSlotUI[i].OnMouseover += (timer) =>
                    {
                        Main.hoverItemName = "       Place a weapon, piece of armor or accessory here.       ";
                        if (timer > 60)
                        {
                            Main.hoverItemName =
                            "       Place a weapon, piece of armor or accessory here.       "
                        + "\nUpgrading Enchanting Table Tier unlocks more Enchantment slots."
                        + "\n       Using weapon Enchantments on armor or accessories       " +
                          "\n          provides diminished bonuses and vice versa.          ";
                        }
                    };//ItemSlot(s) mouseover text
                    Append(wePlayer.enchantingTableUI.itemSlotUI[i]);
                }//ItemSlot(s)
                for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
                {
                    if (i < EnchantingTable.maxEnchantments - 1)
                    {
                        wePlayer.enchantingTableUI.enchantmentSlotUI[i] = new WEUIItemSlot(0, ItemSlotContext.Enchantment, i)
                        {
                            Left = { Pixels = -67f + 47.52f * i },
                            Top = { Pixels = nextElementY },
                            HAlign = 0.5f
                        };
                        string extraStr = "";
                        if (i > 0)
                        {
                            extraStr = "\n  Requires " + EnchantingTableItem.enchantingTableNames[i] + " Enchanting Table or Better to use this slot.  ";
                        }
                        wePlayer.enchantingTableUI.enchantmentSlotUI[i].OnMouseover += (timer) =>
                        {
                            Main.hoverItemName = "                   Place Enchantments here.                    "; //change to a titleText = new UIText("Item           Enchantments      Utility  ")
                            if (timer > 60)
                            {
                                Main.hoverItemName =
                            "                   Place Enchantments here.                    "
                        + "\nUpgrading Enchanting Table Tier unlocks more Enchantment slots."
                        + "\n       Using weapon Enchantments on armor or accessories       " +
                            "\n          provides diminished bonuses and vice versa.          " + extraStr;
                            }
                        };
                        wePlayer.enchantingTableUI.enchantmentSlotUI[i].OnItemMouseover += (timer) => { Main.hoverItemName = extraStr; };
                    }//enchantmentSlot 0-3
                    else
                    {
                        wePlayer.enchantingTableUI.enchantmentSlotUI[i] = new WEUIItemSlot(10, ItemSlotContext.Enchantment, i, true)
                        {
                            Left = { Pixels = -67f + 47.52f * i },
                            Top = { Pixels = nextElementY },
                            HAlign = 0.5f
                        };
                        wePlayer.enchantingTableUI.enchantmentSlotUI[i].OnMouseover += (timer) =>
                        {
                            Main.hoverItemName = "            Only utility Enchantments can go here.             "; //change to a titleText = new UIText("Item           Enchantments      Utility  ")
                        if (timer > 60)
                            {
                                Main.hoverItemName =
                            "            Only utility Enchantments can go here.             "
                        + "\nUpgrading Enchanting Table Tier unlocks more Enchantment slots."
                        + "\n       Using weapon Enchantments on armor or accessories       " +
                            "\n          provides diminished bonuses and vice versa.          ";
                            }
                        };
                    }//enchantmentSlot 4 (Utility only Slot)
                    Append(wePlayer.enchantingTableUI.enchantmentSlotUI[i]);
                }//EnchantmentSlots
                for (int i = 0; i < EnchantingTable.maxEssenceItems; i++)
                {
                    wePlayer.enchantingTableUI.essenceSlotUI[i] = new WEUIItemSlot(4, ItemSlotContext.Essence, i)
                    {
                        Left = { Pixels = -67f + 47.52f * i },
                        Top = { Pixels = nextElementY + 50 },
                        HAlign = 0.5f
                    };
                    string type = EnchantmentEssence.rarity[i];
                    wePlayer.enchantingTableUI.essenceSlotUI[i].OnMouseover += (timer) =>
                    {
                        Main.hoverItemName = "                      Place " + type + " Essence here.                "; 
                    if (timer > 60)
                        {
                            Main.hoverItemName =
                        "                      Place " + type + " Essence here.                "
                    + "\nUpgrading Enchanting Table Tier unlocks more Enchantment slots."
                    + "\n       Using weapon Enchantments on armor or accessories       " +
                        "\n          provides diminished bonuses and vice versa.          ";
                        }
                    };
                    Append(wePlayer.enchantingTableUI.essenceSlotUI[i]);

                    //XP+ buttons
                    button[2 + i] = new UIPanel()
                    {
                        Top = { Pixels = nextElementY + 96 },
                        Left = { Pixels = -66f + 47.52f * i },
                        Width = { Pixels = 40f },
                        Height = { Pixels = 30f },
                        HAlign = 0.5f,
                        BackgroundColor = bgColor
                    };

                    switch (i)
                    {
                        case 0:
                            button[2 + i].OnClick += (evt, element) => { ConvertEssenceToXP(0); };
                            break;
                        case 1:
                            button[2 + i].OnClick += (evt, element) => { ConvertEssenceToXP(1); };
                            break;
                        case 2:
                            button[2 + i].OnClick += (evt, element) => { ConvertEssenceToXP(2); };
                            break;
                        case 3:
                            button[2 + i].OnClick += (evt, element) => { ConvertEssenceToXP(3); };
                            break;
                        case 4:
                            button[2 + i].OnClick += (evt, element) => { ConvertEssenceToXP(4); };
                            break;
                    }
                    
                    UIText xpButonText = new UIText("xp")
                    {
                        Top = { Pixels = -8f },
                        Left = { Pixels = 0f }
                    };
                    button[2 + i].Append(xpButonText);
                    Append(button[2 + i]);
                    panels.Add(button[2 + i]);
                }//EssenceSlots

                nextElementY += 50;
                float ratioFromCenter = 0.22f;

                //Loot All Button
                button[ButtonID.LootAll] = new UIPanel()
                {
                    Top = { Pixels = nextElementY },
                    Left = { Pixels = -66 },
                    Width = { Pixels = 100f },
                    Height = { Pixels = 30f },
                    HAlign = 0.5f - ratioFromCenter,
                    BackgroundColor = bgColor
                };
                button[ButtonID.LootAll].OnClick += (evt, element) => { LootAll(); };
                UIText lootAllButonText = new UIText("Loot All")
                {
                    Top = { Pixels = -4f },
                    Left = { Pixels = 5f }
                };
                button[ButtonID.LootAll].Append(lootAllButonText);
                Append(button[ButtonID.LootAll]);
                panels.Add(button[ButtonID.LootAll]);

                nextElementY += 35;

                //Offer Button
                button[ButtonID.Offer] = new UIPanel()
                {
                    Top = { Pixels = nextElementY },
                    Left = { Pixels = -66 },
                    Width = { Pixels = 100f },
                    Height = { Pixels = 30f },
                    HAlign = 0.5f - ratioFromCenter,
                    BackgroundColor = red
                };
                button[ButtonID.Offer].OnClick += (evt, element) => { Offer(); };
                UIText offerButtonText = new UIText("Offer")
                {
                    Top = { Pixels = -4f },
                    Left = { Pixels = -6f }
                };
                button[ButtonID.Offer].Append(offerButtonText);
                Append(button[ButtonID.Offer]);
                panels.Add(button[ButtonID.Offer]);
            }//PetRenaimer based UI
        }//Set up PR UI
        public override void OnActivate()
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            for (int i = 0; i < EnchantingTable.maxItems; i++)
            {
                 wePlayer.enchantingTableUI.itemSlotUI[i].Item = wePlayer.enchantingTable.item[i].Clone();
            }//Get item(s) left in enchanting table
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
            {
                wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item = wePlayer.enchantingTable.enchantmentItem[i].Clone();
            }//Get enchantments left in enchanting table
            for (int i = 0; i < EnchantingTable.maxEssenceItems; i++)
            {
                wePlayer.enchantingTableUI.essenceSlotUI[i].Item = wePlayer.enchantingTable.essenceItem[i].Clone();
            }//Get essence left in enchanting table
        }//Get items left in enchanting table
        public override void OnDeactivate()
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            //base.OnDeactivate();
            if (!Main.gameMenu)
            {
                //SoundEngine.PlaySound(SoundID.MenuClose);
            }
            if(wePlayer.enchantingTableUI?.itemSlotUI?[0]?.Item != null)//If it hasn't been opened yet, it will be null
            {
                for (int i = 0; i < EnchantingTable.maxItems; i++)
                {
                    wePlayer.enchantingTable.item[i] = wePlayer.enchantingTableUI.itemSlotUI[i].Item.Clone();
                }//Store item(s) left in enchanting table to player
                for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
                {
                    wePlayer.enchantingTable.enchantmentItem[i] = wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item.Clone();
                }//Store enchantments left in enchanting table to player
                for (int i = 0; i < EnchantingTable.maxEssenceItems; i++)
                {
                    wePlayer.enchantingTable.essenceItem[i] = wePlayer.enchantingTableUI.essenceSlotUI[i].Item.Clone();
                }//Store essence left in enchanting table to player
            }
        }//Store items left in enchanting table to player

        public override void Update(GameTime gameTime)
        {
            Left.Pixels = RelativeLeft;//PR
            Top.Pixels = RelativeTop;//PR

            foreach (var panel in panels)
            {
                if (panel.BackgroundColor == bgColor || panel.BackgroundColor == hoverColor)
                {
                    panel.BackgroundColor = panel.IsMouseHovering ? hoverColor : bgColor;
                }
                else if (panel.BackgroundColor == red || panel.BackgroundColor == hoverRed)
                {
                    panel.BackgroundColor = panel.IsMouseHovering ? hoverRed : red;
                }
            }//Change button color if hovering
        }//PR
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            //PR version works fine without this
            /*
            if (firstDraw)
            {
                firstDraw = false;
                return;
            }
            if (ContainsPoint(Main.MouseScreen))
            {
                wePlayer.Player.mouseInterface = true;
                wePlayer.Player.cursorItemIconEnabled = false;
                Main.ItemIconCacheUpdate(0);
            }*/
            


            //From vaninna loook at again
            if (!PR)
            {
                if (wePlayer.usingEnchantingTable && !Main.recBigList)
                {
                    Main.inventoryScale = 0.755f;
                    if (Utils.FloatIntersect(Main.mouseX, Main.mouseY, 0f, 0f, 73f, Main.instance.invBottom, 560f * Main.inventoryScale, 224f * Main.inventoryScale))
                        Main.player[Main.myPlayer].mouseInterface = true;
                    DrawButtons(spriteBatch);
                    DrawSlots(spriteBatch);
                }
                else
                {
                    for (int i = 0; i < ButtonID.Count; i++)
                    {
                        ButtonScale[i] = 0.75f;
                        ButtonHovered[i] = false;
                    }
                }
            }
            base.DrawSelf(spriteBatch);
        }//My UI

        public static void UpdateHover(int ID, bool hovering)
        {
            if (hovering)
            {
                if (!ButtonHovered[ID])
                {
                    SoundEngine.PlaySound(SoundID.MenuTick);
                    ButtonHovered[ID] = true;
                    ButtonScale[ID] += 0.05f;
                    if(ButtonScale[ID] > 1f)
                    {
                        ButtonScale[ID] = 1f;
                    }
                }
            }
            else
            {
                ButtonHovered[ID] = false;
                ButtonScale[ID] -= 0.05f;
                if (ButtonScale[ID] < 0.75f)
                    ButtonScale[ID] = 0.75f;
            }
        }//My UI

        //Having Draw exist at all causes the UI to not show up
        /*
        public override void Draw(SpriteBatch spriteBatch)
        {
            /*
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            if (wePlayer.usingEnchantingTable && !Main.recBigList)
            {
                Main.inventoryScale = 0.755f;
                if (Utils.FloatIntersect(Main.mouseX, Main.mouseY, 0f, 0f, 73f, Main.instance.invBottom, 560f * Main.inventoryScale, 224f * Main.inventoryScale))
                    Main.player[Main.myPlayer].mouseInterface = true;
                DrawButtons(spriteBatch);
                DrawSlots(spriteBatch);
            }
            else
            {
                for (int i = 0; i < ButtonID.Count; i++)
                {
                    ButtonScale[i] = 0.75f;
                    ButtonHovered[i] = false;
                }
            }
            //base.DrawSelf(spriteBatch);
        }//My UI
        */

        private static void DrawButtons(SpriteBatch spritebatch)//Not used if Draw is disabled
        {
            for (int i = 0; i < ButtonID.Count; i++)
            {
                DrawButton(spritebatch, i, 506, Main.instance.invBottom + 40);//Change this to be the correct spot
            }
        }//My UI
		private static void DrawButton(SpriteBatch spriteBatch, int ID, int X, int Y)//Not used if Draw is disabled
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
			int num = ID;
			//if (ID == 7)
			//	num = 5;

			Y += num * 26;
			float num2 = ButtonScale[ID];
			string text = Language.GetTextValue("Mods.WeaponEnchantments.Buttons." + ButtonNames[ID]);//Need to set up names in other languages for this to work

			Vector2 value = FontAssets.MouseText.Value.MeasureString(text);
			Color color = new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor) * num2;
			color = Color.White * 0.97f * (1f - (255f - (float)(int)Main.mouseTextColor) / 255f * 0.5f);
			color.A = byte.MaxValue;
			X += (int)(value.X * num2 / 2f);
			bool flag = Utils.FloatIntersect(Main.mouseX, Main.mouseY, 0f, 0f, (float)X - value.X / 2f, Y - 12, value.X, 24f);
			if (ButtonHovered[ID])
				flag = Utils.FloatIntersect(Main.mouseX, Main.mouseY, 0f, 0f, (float)X - value.X / 2f - 10f, Y - 12, value.X + 16f, 24f);

			if (flag)
				color = Main.OurFavoriteColor;

			ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, text, new Vector2(X, Y), color, 0f, value / 2f, new Vector2(num2), -1f, 1.5f);
			value *= num2;
			UILinkPointNavigator.SetPosition(UILinkPointNavigator.Points.Count + 1, new Vector2((float)X - value.X * num2 / 2f * 0.8f, Y));
			
			if (!flag)
			{
				UpdateHover(ID, hovering: false);
				return;
			}

			UpdateHover(ID, hovering: true);
			if (PlayerInput.IgnoreMouseInterface)
				return;
			wePlayer.Player.mouseInterface = true;
			if (Main.mouseLeft && Main.mouseLeftRelease)
			{
				switch (ID)
				{
                    case 1:
                        LootAll();
                        break;
					case 2:
						Offer();
						break;
				}
				Recipe.FindRecipes();
			}
		}//My UI
        private static void ConvertEssenceToXP(int tier)
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            Item essence = wePlayer.enchantingTableUI.essenceSlotUI[tier].Item;
            Item item = wePlayer.enchantingTableUI.itemSlotUI[0].Item;
            if (!essence.IsAir && !item.IsAir)
            {
                essence.stack--;
                Main.NewText(wePlayer.Player.name + " applied " + essence.Name + " to their " + item.Name + " gaining " + ConfirmationUI.xpTiers[tier].ToString() + " xp.");
                item.GetGlobalItem<EnchantedItem>().GainXP(item, ConfirmationUI.xpTiers[tier]);
                SoundEngine.PlaySound(SoundID.MenuTick);
            }
        }
        private static void LootAll()
        {
            if (WEModSystem.promptInterface.CurrentState == null)
            {
                GetItemSettings lootAllSettings = GetItemSettings.LootAllSettings;
                WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
                for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
                {
                    if (!wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item.IsAir)
                    {
                        wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item.position = wePlayer.Player.Center;
                        wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item = wePlayer.Player.GetItem(Main.myPlayer, wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item, lootAllSettings);
                    }
                }//Take all enchantments first
                for (int i = 0; i < EnchantingTable.maxItems; i++)
                {
                    if (!wePlayer.enchantingTableUI.itemSlotUI[i].Item.IsAir)
                    {
                        wePlayer.enchantingTableUI.itemSlotUI[i].Item.position = wePlayer.Player.Center;
                        wePlayer.enchantingTableUI.itemSlotUI[i].Item = wePlayer.Player.GetItem(Main.myPlayer, wePlayer.enchantingTableUI.itemSlotUI[i].Item, lootAllSettings);
                    }
                }//Take item(s)
            }
        }//Loot all item(s) and enchantments from enchantment table (Not Essence)
        private static void Offer()
        {
            if(WEModSystem.promptInterface.CurrentState == null)
            {
                WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
                if (!wePlayer.enchantingTableUI.itemSlotUI[0].Item.IsAir)
                {
                    WEModSystem.weModSystemUI.SetState(null);
                    UIState state = new UIState();
                    state.Append(wePlayer.confirmationUI);
                    WEModSystem.promptInterface.SetState(state);
                }
            }
        }
        
        private static void DrawSlots(SpriteBatch spriteBatch)//Not used if Draw is disabled
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            Item[] item = wePlayer.enchantingTable.item;
            Item[] enchantmentItem = wePlayer.enchantingTable.enchantmentItem;//might need to be for int i < maxEnchantments...or Clone()
            Item[] essenceItem = wePlayer.enchantingTable.essenceItem;//might need to be for int i < maxEssence...or Clone()
            Main.inventoryScale = 0.755f;
            if (Utils.FloatIntersect(Main.mouseX, Main.mouseY, 0f, 0f, 73f, Main.instance.invBottom, 560f * Main.inventoryScale, 224f * Main.inventoryScale) && !PlayerInput.IgnoreMouseInterface)
                wePlayer.Player.mouseInterface = true;
            for (int i = 0; i < EnchantingTable.maxItems; i++)
            {
                int x = (int)(73f + (float)(i * 56) * Main.inventoryScale);//change
                int y = (int)((float)Main.instance.invBottom + (float)( 56) * Main.inventoryScale);//change
                int slot = i  * 10;
                new Color(100, 100, 100, 100);
                if (Utils.FloatIntersect(Main.mouseX, Main.mouseY, 0f, 0f, x, y, (float)TextureAssets.InventoryBack.Width() * Main.inventoryScale, (float)TextureAssets.InventoryBack.Height() * Main.inventoryScale) && !PlayerInput.IgnoreMouseInterface)
                {
                    wePlayer.Player.mouseInterface = true;
                    ItemSlot.Handle(item, 4, slot);
                }
                ItemSlot.Draw(spriteBatch, item, 4, slot, new Vector2(x, y));
            }
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
            {
                int x = (int)(73f + (float)(i * 56) * Main.inventoryScale + 50);//change
                int y = (int)((float)Main.instance.invBottom + (float)( 56) * Main.inventoryScale);//change
                int slot = i  * 10;
                if (i < wePlayer.enchantingTable.availableEnchantmentSlots)
                {
                    new Color(100, 100, 100, 100);
                }
                else
                {
                    new Color(50, 50, 50, 50);
                }
                if (Utils.FloatIntersect(Main.mouseX, Main.mouseY, 0f, 0f, x, y, (float)TextureAssets.InventoryBack.Width() * Main.inventoryScale, (float)TextureAssets.InventoryBack.Height() * Main.inventoryScale) && !PlayerInput.IgnoreMouseInterface)
                {
                    wePlayer.Player.mouseInterface = true;
                    ItemSlot.Handle(enchantmentItem, 4, slot);
                }
                ItemSlot.Draw(spriteBatch, enchantmentItem, 4, slot, new Vector2(x, y));
            }
            for (int i = 0; i < EnchantingTable.maxEssenceItems; i++)
            {
                int x = (int)(73f + (float)(i * 56) * Main.inventoryScale + 100);//change
                int y = (int)((float)Main.instance.invBottom + (float)( 56) * Main.inventoryScale);//change
                int slot = i  * 10;
                new Color(100, 100, 100, 100);
                if (Utils.FloatIntersect(Main.mouseX, Main.mouseY, 0f, 0f, x, y, (float)TextureAssets.InventoryBack.Width() * Main.inventoryScale, (float)TextureAssets.InventoryBack.Height() * Main.inventoryScale) && !PlayerInput.IgnoreMouseInterface)
                {
                    wePlayer.Player.mouseInterface = true;
                    ItemSlot.Handle(essenceItem, 4, slot);
                }
                ItemSlot.Draw(spriteBatch, essenceItem, 4, slot, new Vector2(x, y));
            }
        }//My UI

        public static bool TryPlacingInEnchantingTable(Item I, bool justCheck, int itemSlotContext)//No references to this
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();//sync is always false
            Item[] slotItems;

            switch (itemSlotContext)
            {
                case ItemSlotContext.Item:
                    slotItems = wePlayer.enchantingTable.item;
                    break;
                case ItemSlotContext.Enchantment:
                    slotItems = wePlayer.enchantingTable.enchantmentItem;
                    break;
                case ItemSlotContext.Essence:
                    slotItems = wePlayer.enchantingTable.essenceItem;
                    break;
                default:
                    slotItems = null;
                    break;
            }
            if(!IsBlockedFromTransferIntoEnchantingTable(I, slotItems, itemSlotContext))
            {
                return false;
            }
            bool flag = false;
            switch (itemSlotContext)
            {
                case ItemSlotContext.Item:
                    if (I.stack > 0)
                    {
                        for (int i = 0; i < EnchantingTable.maxItems; i++)
                        {
                            if (slotItems[i].stack != 0)
                                continue;
                            if (justCheck)
                            {
                                flag = true;
                                break;
                            }
                            SoundEngine.PlaySound(SoundID.Grab);
                            slotItems[i] = I.Clone();
                            I.SetDefaults();
                            ItemSlot.AnnounceTransfer(new ItemSlot.ItemTransferInfo(slotItems[i], 0, 3));
                            break;
                        }
                    }
                    return flag;
                case ItemSlotContext.Enchantment:
                    if(I.stack > 0)
                    {
                        for (int i = 0; i < wePlayer.enchantingTable.availableEnchantmentSlots; i++)
                        {
                            if (slotItems[i].stack != 0)
                                continue;
                            if (justCheck)
                            {
                                flag = true;
                                break;
                            }
                            SoundEngine.PlaySound(SoundID.Grab);
                            slotItems[i] = I.Clone();
                            I.SetDefaults();
                            ItemSlot.AnnounceTransfer(new ItemSlot.ItemTransferInfo(slotItems[i], 0, 3));
                            break;
                        }
                    }
                    return flag;
                case ItemSlotContext.Essence:
                    int essenceTier = EnchantmentEssence.GetEssenceTier(I);//Need new function to check essenceTier, GetEssenceTier(I)
                    if (slotItems[essenceTier].stack < I.maxStack)
                    {
                        if(!ItemLoader.CanStack(slotItems[essenceTier], I))
                        {
                            return false;
                        }
                        int stack = I.stack;
                        if (I.stack + slotItems[essenceTier].stack > I.maxStack)
                        {
                            stack = I.maxStack - slotItems[essenceTier].stack;
                        }
                        if (justCheck)
                        {
                            flag = (flag || stack > 0);
                            break;
                        }
                        I.stack -= stack;
                        slotItems[essenceTier].stack += stack;
                        SoundEngine.PlaySound(SoundID.Grab);
                        if(I.stack <= 0)
                        {
                            I.SetDefaults();
                            break;
                        }
                        if (slotItems[essenceTier].type == ItemID.None)
                        {
                            slotItems[essenceTier] = I.Clone();
                            I.SetDefaults();
                        }
                    }
                    return flag;
                default:
                    return false;
            }
            return false;
            //Check against tier for slots above not < available
            //Only let the specific item go in each slot
        }//My UI

        public static bool IsBlockedFromTransferIntoEnchantingTable(Item I, Item[] slotItems, int itemSlotContext)//Look at this again compair to vanilla
        {
            switch (itemSlotContext)
            {
                case ItemSlotContext.Item:
                    if (true)//If I is a weapon or armor or accessory
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                case ItemSlotContext.Enchantment:
                    if (true)//If I is an enchantment
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                case ItemSlotContext.Essence:
                    if (true)//If I is an essence
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                default:
                    return false;
            }
        }//My UI
    }
}
