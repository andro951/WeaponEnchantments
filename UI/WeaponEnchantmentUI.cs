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
using static WeaponEnchantments.Items.EnchantmentEssence;

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
                Left.Pixels = int.MaxValue / 2 + 100 - 25;
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
                WeaponEnchantmentUI.ConvertXPToEssence(xp, true);
                int stack = (int)Math.Round((float)value / ModContent.GetModItem(ModContent.ItemType<ContainmentFragment>()).Item.value);
                if (stack == 0)
                {
                    stack = 1;
                }
                Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), ModContent.ItemType<ContainmentFragment>(), stack);
                if (wePlayer.enchantingTableUI.itemSlotUI[0].Item.GetGlobalItem<EnchantedItem>().powerBoosterInstalled)
                {
                    Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), ModContent.ItemType<PowerBooster>());
                }
                wePlayer.enchantingTableUI.itemSlotUI[0].Item = new Item();
                offered = true;
                SoundEngine.PlaySound(SoundID.Grab);
            }
        }//Consume item to upgrade table or get resources
        public override void Update(GameTime gameTime)
        {
            Left.Pixels = RelativeLeft + 100 - 25;//PR
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
            public const int xp0 = 0;//2
            public const int xp1 = 1;//3
            public const int xp2 = 2;//4
            public const int xp3 = 3;//5
            public const int xp4 = 4;//6
            public const int LevelUp = 7;
            public const int Syphon = 8;
            public const int Count = 9;
            public static int[] xps = new int[] { xp0, xp1, xp2, xp3, xp4 };
        }//LootAll = 0, Offer = 1
        public class ItemSlotContext
        {
            public const int Item = 0;
            public const int Enchantment = 1;
            public const int Essence = 2;
        }//Item = 0, Enchantment = 1, Essence = 3

        public const bool PR = true;//Used to toggle between my UI in progress and the UI based on PetRenaimer mod

        public static string[] ButtonNames = new string[] { "Enchant", "Disenchant", "Offer", "Level", "Syphon" };
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

        internal const int width = 530;
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

                float xOffset = -20;
                float nextElementY = -PaddingTop / 2;

                titleText = new UIText("Item           Enchantments      Utility  ")
                {
                    Top = { Pixels = nextElementY },
                    Left = { Pixels = 0 + xOffset },
                    HAlign = 0.5f
                };//UI slot labels
                Append(titleText);
                nextElementY += 20;

                for (int i = 0; i < EnchantingTable.maxItems; i++)
                {
                    wePlayer.enchantingTableUI.itemSlotUI[i] = new WEUIItemSlot(17, ItemSlotContext.Item)
                    {
                        Left = { Pixels = -145f + xOffset },
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
                            Left = { Pixels = -67f + 47.52f * i + xOffset },
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
                    }//enchantmentSlot 0-3
                    else
                    {
                        wePlayer.enchantingTableUI.enchantmentSlotUI[i] = new WEUIItemSlot(10, ItemSlotContext.Enchantment, i, true)
                        {
                            Left = { Pixels = -67f + 47.52f * i + xOffset },
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
                        Left = { Pixels = -67f + 47.52f * i + xOffset },
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
                        Left = { Pixels = -66f + 47.52f * i + xOffset },
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

                //Level Up button
                button[ButtonID.LevelUp] = new UIPanel()
                {
                    Top = { Pixels = nextElementY + 96 },
                    Left = { Pixels = -66f + 47.52f * 5 + 25 + xOffset },
                    Width = { Pixels = 90f },
                    Height = { Pixels = 30f },
                    HAlign = 0.5f,
                    BackgroundColor = bgColor
                };
                button[ButtonID.LevelUp].OnClick += (evt, element) => { LevelUp(); };
                UIText levelButonText = new UIText("Level Up")
                {
                    Top = { Pixels = -8f },
                    Left = { Pixels = -1f }
                };
                button[ButtonID.LevelUp].Append(levelButonText);
                Append(button[ButtonID.LevelUp]);
                panels.Add(button[ButtonID.LevelUp]);

                //Syphon button
                button[ButtonID.Syphon] = new UIPanel()
                {
                    Top = { Pixels = nextElementY + 0 },
                    Left = { Pixels = -66f + 47.52f * 5 + 25 + xOffset },
                    Width = { Pixels = 90f },
                    Height = { Pixels = 30f },
                    HAlign = 0.5f,
                    BackgroundColor = bgColor
                };
                button[ButtonID.Syphon].OnClick += (evt, element) => { Syphon(); };
                UIText syphonButonText = new UIText("Sphon")
                {
                    Top = { Pixels = -8f },
                    Left = { Pixels = -1f }
                };
                button[ButtonID.Syphon].Append(syphonButonText);
                Append(button[ButtonID.Syphon]);
                panels.Add(button[ButtonID.Syphon]);

                nextElementY += 50;
                float ratioFromCenter = 0.22f;

                //Loot All Button
                button[ButtonID.LootAll] = new UIPanel()
                {
                    Top = { Pixels = nextElementY },
                    Left = { Pixels = -66 + xOffset },
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
                    Left = { Pixels = -66 + xOffset },
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
            if (wePlayer.enchantingTableUI?.itemSlotUI?[0]?.Item != null)//If it hasn't been opened yet, it will be null
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
        private static void ConvertEssenceToXP(int tier)
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            Item essence = wePlayer.enchantingTableUI.essenceSlotUI[tier].Item;
            Item item = wePlayer.enchantingTableUI.itemSlotUI[0].Item;
            if (!essence.IsAir && !item.IsAir)
            {
                essence.stack--;
                //ModContent.GetInstance<WEMod>().Logger.Info(wePlayer.Player.name + " applied " + essence.Name + " to their " + item.Name + " gaining " + ConfirmationUI.xpTiers[tier].ToString() + " xp.");
                //Main.NewText(wePlayer.Player.name + " applied " + essence.Name + " to their " + item.Name + " gaining " + ConfirmationUI.xpTiers[tier].ToString() + " xp.");
                item.GetGlobalItem<EnchantedItem>().GainXP(item, (int)EnchantmentEssence.xpPerEssence[tier]);
                SoundEngine.PlaySound(SoundID.MenuTick);
            }
        }
        public static int ConvertXPToEssence(int xp, bool consumeAll = false)
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            int numberEssenceRecieved;
            int xpCounter = (int)Math.Round(xp * (0.6f + 0.1f * wePlayer.enchantingTableTier));
            int xpInitial = xpCounter;
            int xpNotConsumed = 0;
            for (int tier = EnchantingTable.maxEssenceItems - 1; tier >= 0; tier--)
            {
                if (wePlayer.enchantingTableTier >= tier)
                {
                    /*if(wePlayer.enchantingTableTier == EnchantingTable.maxTier)
                    {
                        numberEssenceRecieved = xpCounter / (int)EnchantmentEssence.xpPerEssence[tier];
                        xpCounter %= (int)EnchantmentEssence.xpPerEssence[tier];
                    }
                    else
                    {*/
                    if(tier > 0)
                    {
                        numberEssenceRecieved = xpCounter / (int)EnchantmentEssence.xpPerEssence[tier] * 4 / 5;
                    }
                    else
                    {
                        numberEssenceRecieved = xpCounter / (int)EnchantmentEssence.xpPerEssence[tier];
                    }
                    xpCounter -= (int)EnchantmentEssence.xpPerEssence[tier] * numberEssenceRecieved;
                    //}
                    if (xpCounter < (int)EnchantmentEssence.xpPerEssence[0] && xpCounter > 0 && tier == 0)
                    {
                        if (consumeAll)
                        {
                            numberEssenceRecieved += 1;
                        }
                        else
                        {
                            xpNotConsumed = xpCounter;
                        }
                        xpCounter = 0;
                    }
                    if (wePlayer.enchantingTableUI.essenceSlotUI[tier].Item.IsAir)
                    {
                        wePlayer.enchantingTableUI.essenceSlotUI[tier].Item = new Item(EnchantmentEssence.IDs[tier], numberEssenceRecieved);
                    }
                    else
                    {
                        if (wePlayer.enchantingTableUI.essenceSlotUI[tier].Item.stack + numberEssenceRecieved > ModContent.GetModItem(ModContent.ItemType<EnchantmentEssenceBasic>()).Item.maxStack)
                        {
                            int ammountToTransfer = ModContent.GetModItem(ModContent.ItemType<EnchantmentEssenceBasic>()).Item.maxStack - wePlayer.enchantingTableUI.essenceSlotUI[tier].Item.stack;
                            numberEssenceRecieved -= ammountToTransfer;
                            wePlayer.enchantingTableUI.essenceSlotUI[tier].Item.stack += ammountToTransfer;
                            Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), ModContent.ItemType<EnchantmentEssenceBasic>() + tier, numberEssenceRecieved);
                        }
                        else
                        {
                            wePlayer.enchantingTableUI.essenceSlotUI[tier].Item.stack += numberEssenceRecieved;
                        }
                    }
                }
            }
            return xpInitial - xpNotConsumed;
        }
        public static void Syphon()
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            if (!wePlayer.enchantingTableUI.itemSlotUI[0].Item.IsAir)
            {
                EnchantedItem iGlobal = wePlayer.enchantingTableUI.itemSlotUI[0].Item.GetGlobalItem<EnchantedItem>();
                if(iGlobal.experience < WEModSystem.levelXps[EnchantedItem.maxLevel - 1] + EnchantmentEssence.xpPerEssence[0])
                {
                    Main.NewText("You can only Syphon an item if it is max level and over " + (WEModSystem.levelXps[EnchantedItem.maxLevel - 1] + EnchantmentEssence.xpPerEssence[0]).ToString() + " experience.");
                }
                else
                {
                    iGlobal.experience -= ConvertXPToEssence(iGlobal.experience - WEModSystem.levelXps[EnchantedItem.maxLevel - 1]);
                }
            }
        }
        private static void LootAll()
        {
            if (WEModSystem.promptInterface.CurrentState == null)
            {
                WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
                for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
                {
                    if (!wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item.IsAir)
                    {
                        wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item.position = wePlayer.Player.Center;
                        wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item = wePlayer.Player.GetItem(Main.myPlayer, wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item, GetItemSettings.LootAllSettings);
                    }
                }//Take all enchantments first
                for (int i = 0; i < EnchantingTable.maxItems; i++)
                {
                    if (!wePlayer.enchantingTableUI.itemSlotUI[i].Item.IsAir)
                    {
                        wePlayer.enchantingTableUI.itemSlotUI[i].Item.position = wePlayer.Player.Center;
                        wePlayer.enchantingTableUI.itemSlotUI[i].Item = wePlayer.Player.GetItem(Main.myPlayer, wePlayer.enchantingTableUI.itemSlotUI[i].Item, GetItemSettings.LootAllSettings);
                    }
                }//Take item(s)
            }
        }//Loot all item(s) and enchantments from enchantment table (Not Essence)
        private static void Offer()
        {
            if (WEModSystem.promptInterface.CurrentState == null)
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
        private static void LevelUp()
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            Item tableItem = wePlayer.enchantingTableUI.itemSlotUI[0].Item;
            if (!tableItem.IsAir)
            {
                int xpAvailable = 0;
                EnchantedItem iGlobal = tableItem.GetGlobalItem<EnchantedItem>();
                if(iGlobal.levelBeforeBooster != EnchantedItem.maxLevel)
                {
                    for (int i = EnchantingTable.maxEnchantments - 1; i >= 0; i--)
                    {
                        xpAvailable += (int)EnchantmentEssence.xpPerEssence[i] * wePlayer.enchantingTableUI.essenceSlotUI[i].Item.stack;
                    }
                    int xpNeeded = WEModSystem.levelXps[iGlobal.levelBeforeBooster] - iGlobal.experience;
                    if (xpAvailable >= xpNeeded)
                    {
                        for (int i = EnchantingTable.maxEnchantments - 1; i >= 0; i--)
                        {
                            Item essenceItem = wePlayer.enchantingTableUI.essenceSlotUI[i].Item;
                            int stack = essenceItem.stack;
                            int numberEssenceNeeded = xpNeeded / (int)EnchantmentEssence.xpPerEssence[i];
                            int numberEssenceTransfered;
                            if(numberEssenceNeeded > stack)
                            {
                                numberEssenceTransfered = stack;
                            }
                            else
                            {
                                numberEssenceTransfered = numberEssenceNeeded;
                            }
                            int xpAvailableBelowMe = 0;
                            for (int j = i - 1; j >= 0; j--)
                            {
                                xpAvailableBelowMe += (int)EnchantmentEssence.xpPerEssence[j] * wePlayer.enchantingTableUI.essenceSlotUI[j].Item.stack;
                            }
                            if(xpAvailableBelowMe < xpNeeded - (int)EnchantmentEssence.xpPerEssence[i] * numberEssenceTransfered)
                            {
                                numberEssenceTransfered++;
                            }
                            if (numberEssenceTransfered > 0)
                            {
                                int xpTransfered = (int)EnchantmentEssence.xpPerEssence[i] * numberEssenceTransfered;
                                xpNeeded -= xpTransfered;
                                essenceItem.stack -= numberEssenceTransfered;
                                iGlobal.GainXP(tableItem, xpTransfered);
                            }
                        }
                    }
                    else
                    {
                        Main.NewText("Not Enough Essence. You need " + xpNeeded + " experience for level " + (iGlobal.levelBeforeBooster + 1).ToString() + " you only have " + xpAvailable + " available.");
                    }
                }
                else
                {
                    Main.NewText("Your " + tableItem.Name + " is already max level.");
                }
            }
        }
    }
}
