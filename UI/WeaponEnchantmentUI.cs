using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using WeaponEnchantments.Items;
using WeaponEnchantments.Common.Globals;
using System;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using static WeaponEnchantments.Common.EnchantingRarity;
using static WeaponEnchantments.Common.Configs.ConfigValues;
using System.Reflection;

namespace WeaponEnchantments.UI
{
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
            public const int Infusion = 9;
            public const int Count = 10;
        }

        public class ItemSlotContext
        {
            public const int Item = 0;
            public const int Enchantment = 1;
            public const int Essence = 2;
        }

        public static string[] ButtonNames = new string[] { "Enchant", "Disenchant", "Offer", "Level", "Syphon" };
        public static bool preventItemUse = false;
        public static bool pressedLootAll = true;

        private UIText titleText;
        public UIPanel[] button = new UIPanel[ButtonID.Count];
        public UIText infusionButonText;
        private List<UIPanel> panels;
        public WEUIItemSlot[] itemSlotUI = new WEUIItemSlot[EnchantingTable.maxItems];
        public WEUIItemSlot[] enchantmentSlotUI = new WEUIItemSlot[EnchantingTable.maxEnchantments];
        public WEUIItemSlot[] essenceSlotUI = new WEUIItemSlot[EnchantingTable.maxEssenceItems];

        private readonly static Color red = new Color(171, 76, 73);
        private readonly static Color hoverRed = new Color(184, 103, 100);
        private readonly static Color bgColor = new Color(73, 94, 171);
        private readonly static Color hoverColor = new Color(100, 118, 184);

        internal const int width = 530;
        internal const int height = 155;

        internal int RelativeLeft => Main.screenWidth / 2 - width / 2;
        internal int RelativeTop => Main.screenHeight / 2 + 42;

        public override void OnInitialize() {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            Width.Pixels = width;
            Height.Pixels = height;
            Top.Pixels = int.MaxValue / 2;
            Left.Pixels = int.MaxValue / 2;

            panels = new List<UIPanel>();

            float xOffset = -20;
            float nextElementY = -PaddingTop / 2;

            //UI slot labels
            titleText = new UIText("Item           Enchantments      Utility  ") {
                Top = { Pixels = nextElementY },
                Left = { Pixels = 0 + xOffset },
                HAlign = 0.5f
            };

            Append(titleText);
            nextElementY += 20;

            //ItemSlot(s)
            for (int i = 0; i < EnchantingTable.maxItems; i++) {
                //ItemSlot(s)
                wePlayer.enchantingTableUI.itemSlotUI[i] = new WEUIItemSlot(17, ItemSlotContext.Item) {
                    Left = { Pixels = -145f + xOffset },
                    Top = { Pixels = nextElementY },
                    HAlign = 0.5f
                };

                //ItemSlot(s) mouseover text
                wePlayer.enchantingTableUI.itemSlotUI[i].OnMouseover += (timer) => {
                    Main.hoverItemName = "       Place a weapon, piece of armor or accessory here.       ";
                    if (timer > 60) {
                        Main.hoverItemName =
                            "       Place a weapon, piece of armor or accessory here.       \n" +
                            "Upgrading Enchanting Table Tier unlocks more Enchantment slots.\n" +
                            "       Using weapon Enchantments on armor or accessories       \n" +
                            "          provides diminished bonuses and vice versa.          ";
                    }
                };

                Append(wePlayer.enchantingTableUI.itemSlotUI[i]);
            }

            //EnchantmentSlots
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++) {
                if (i < EnchantingTable.maxEnchantments - 1) {
                    //enchantmentSlot 0-3
                    wePlayer.enchantingTableUI.enchantmentSlotUI[i] = new WEUIItemSlot(0, ItemSlotContext.Enchantment, i) {
                        Left = { Pixels = -67f + 47.52f * i + xOffset },
                        Top = { Pixels = nextElementY },
                        HAlign = 0.5f
                    };

                    string extraStr = "";
                    if (i > 0)
                        extraStr = "\n  Requires " + Items.EnchantingTableItem.enchantingTableNames[i] + " Enchanting Table or Better to use this slot.  ";

                    wePlayer.enchantingTableUI.enchantmentSlotUI[i].OnMouseover += (timer) => {
                        Main.hoverItemName = "                   Place Enchantments here.                    ";
                        if (timer > 60) {
                            Main.hoverItemName =
                                "                   Place Enchantments here.                    \n" +
						        "Upgrading Enchanting Table Tier unlocks more Enchantment slots.\n" +
						        "       Using weapon Enchantments on armor or accessories       \n" +
						        "          provides diminished bonuses and vice versa.          " + extraStr;
                        }
                    };
                }
                else {
                    //enchantmentSlot 4 (Utility only Slot)
                    wePlayer.enchantingTableUI.enchantmentSlotUI[i] = new WEUIItemSlot(10, ItemSlotContext.Enchantment, i, true) {
                        Left = { Pixels = -67f + 47.52f * i + xOffset },
                        Top = { Pixels = nextElementY },
                        HAlign = 0.5f
                    };

                    wePlayer.enchantingTableUI.enchantmentSlotUI[i].OnMouseover += (timer) => {
                        Main.hoverItemName = "            Only utility Enchantments can go here.             ";
                        if (timer > 60) {
                            Main.hoverItemName =
                                "            Only utility Enchantments can go here.             \n" +
                                "Upgrading Enchanting Table Tier unlocks more Enchantment slots.\n" +
                                "       Using weapon Enchantments on armor or accessories       \n" +
						        "          provides diminished bonuses and vice versa.          ";
                        }
                    };
                }

                Append(wePlayer.enchantingTableUI.enchantmentSlotUI[i]);
            }

            //EssenceSlots
            for (int i = 0; i < EnchantingTable.maxEssenceItems; i++) {
                wePlayer.enchantingTableUI.essenceSlotUI[i] = new WEUIItemSlot(4, ItemSlotContext.Essence, i) {
                    Left = { Pixels = -67f + 47.52f * i + xOffset },
                    Top = { Pixels = nextElementY + 50 },
                    HAlign = 0.5f
                };

                string type = tierNames[i];
                wePlayer.enchantingTableUI.essenceSlotUI[i].OnMouseover += (timer) => {
                    Main.hoverItemName = "                      Place " + type + " Essence here.                ";
                    if (timer > 60) {
                        Main.hoverItemName =
                    "                      Place " + type + " Essence here.                \n" +
                    "Upgrading Enchanting Table Tier unlocks more Enchantment slots.\n" +
                    "       Using weapon Enchantments on armor or accessories       \n" +
					"          provides diminished bonuses and vice versa.          ";
                    }
                };

                Append(wePlayer.enchantingTableUI.essenceSlotUI[i]);

                //XP+ buttons
                button[2 + i] = new UIPanel() {
                    Top = { Pixels = nextElementY + 96 },
                    Left = { Pixels = -66f + 47.52f * i + xOffset },
                    Width = { Pixels = 40f },
                    Height = { Pixels = 30f },
                    HAlign = 0.5f,
                    BackgroundColor = bgColor
                };
                
                switch (i) {
                    case 0:
                        button[2 + i].OnClick += (evt, element) => ConvertEssenceToXP(0);
                        break;
                    case 1:
                        button[2 + i].OnClick += (evt, element) => ConvertEssenceToXP(1);
                        break;
                    case 2:
                        button[2 + i].OnClick += (evt, element) => ConvertEssenceToXP(2);
                        break;
                    case 3:
                        button[2 + i].OnClick += (evt, element) => ConvertEssenceToXP(3);
                        break;
                    case 4:
                        button[2 + i].OnClick += (evt, element) => ConvertEssenceToXP(4);
                        break;
                }

                UIText xpButonText = new UIText("xp") {
                    Top = { Pixels = -8f },
                    Left = { Pixels = 0f }
                };

                button[2 + i].Append(xpButonText);
                Append(button[2 + i]);
                panels.Add(button[2 + i]);
            }

            //Level Up button
            button[ButtonID.LevelUp] = new UIPanel() {
                Top = { Pixels = nextElementY + 96 },
                Left = { Pixels = -66f + 47.52f * 5 + 25 + xOffset },
                Width = { Pixels = 90f },
                Height = { Pixels = 30f },
                HAlign = 0.5f,
                BackgroundColor = bgColor
            };

            button[ButtonID.LevelUp].OnClick += (evt, element) => LevelUp();
            UIText levelButonText = new UIText("Level Up") {
                Top = { Pixels = -8f },
                Left = { Pixels = -1f }
            };

            button[ButtonID.LevelUp].Append(levelButonText);
            Append(button[ButtonID.LevelUp]);
            panels.Add(button[ButtonID.LevelUp]);

            //Syphon button
            button[ButtonID.Syphon] = new UIPanel() {
                Top = { Pixels = nextElementY + 0 },
                Left = { Pixels = -66f + 47.52f * 5 + 25 + xOffset },
                Width = { Pixels = 90f },
                Height = { Pixels = 30f },
                HAlign = 0.5f,
                BackgroundColor = bgColor
            };

            button[ButtonID.Syphon].OnClick += (evt, element) => Syphon();
            UIText syphonButonText = new UIText("Syphon") {
                Top = { Pixels = -8f },
                Left = { Pixels = -1f }
            };

            button[ButtonID.Syphon].Append(syphonButonText);
            Append(button[ButtonID.Syphon]);
            panels.Add(button[ButtonID.Syphon]);

            //Infusion button
            button[ButtonID.Infusion] = new UIPanel() {
                Top = { Pixels = nextElementY + 35 },
                Left = { Pixels = -66f + 47.52f * 5 + 25 + xOffset },
                Width = { Pixels = 90f },
                Height = { Pixels = 30f },
                HAlign = 0.5f,
                BackgroundColor = bgColor
            };

            button[ButtonID.Infusion].OnClick += (evt, element) => Infusion();
            string infusionText;
            if (wePlayer.infusionConsumeItem != null) {
                if (wePlayer.enchantingTable.item[0] == null || wePlayer.enchantingTable.item[0].IsAir)
                    infusionText = "Cancel";
                else
                    infusionText = "Finalize";
            }
            else {
                infusionText = "Infusion";
            }

            infusionButonText = new UIText(infusionText) {
                Top = { Pixels = -8f },
                Left = { Pixels = -1f }
            };

            button[ButtonID.Infusion].Append(infusionButonText);
            Append(button[ButtonID.Infusion]);
            panels.Add(button[ButtonID.Infusion]);

            nextElementY += 50;
            float ratioFromCenter = 0.22f;

            //Loot All Button
            button[ButtonID.LootAll] = new UIPanel() {
                Top = { Pixels = nextElementY },
                Left = { Pixels = -66 + xOffset },
                Width = { Pixels = 100f },
                Height = { Pixels = 30f },
                HAlign = 0.5f - ratioFromCenter,
                BackgroundColor = bgColor
            };

            button[ButtonID.LootAll].OnClick += (evt, element) => LootAll();
            UIText lootAllButonText = new UIText("Loot All") {
                Top = { Pixels = -4f },
                Left = { Pixels = 5f }
            };

            button[ButtonID.LootAll].Append(lootAllButonText);
            Append(button[ButtonID.LootAll]);
            panels.Add(button[ButtonID.LootAll]);

            nextElementY += 35;

            //Offer Button
            button[ButtonID.Offer] = new UIPanel() {
                Top = { Pixels = nextElementY },
                Left = { Pixels = -66 + xOffset },
                Width = { Pixels = 100f },
                Height = { Pixels = 30f },
                HAlign = 0.5f - ratioFromCenter,
                BackgroundColor = red
            };

            button[ButtonID.Offer].OnClick += (evt, element) => Offer();
            UIText offerButtonText = new UIText("Offer") {
                Top = { Pixels = -4f },
                Left = { Pixels = -6f }
            };

            button[ButtonID.Offer].Append(offerButtonText);
            Append(button[ButtonID.Offer]);
            panels.Add(button[ButtonID.Offer]);
        }

        private static int GetButtonID(string buttonName) {
            ButtonID buttonID = new ButtonID();
            foreach (PropertyInfo property in buttonID.GetType().GetProperties()) {
                if(property.Name == buttonName)
                    return (int)property.GetValue(property);
			}

            return -1;
		}

        public override void OnActivate() {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            //Get item(s) left in enchanting table
            for (int i = 0; i < EnchantingTable.maxItems; i++) {
                wePlayer.enchantingTableUI.itemSlotUI[i].Item = wePlayer.enchantingTable.item[i].Clone();
            }

            //Get enchantments left in enchanting table
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++) {
                wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item = wePlayer.enchantingTable.enchantmentItem[i].Clone();
            }

            //Get essence left in enchanting table
            for (int i = 0; i < EnchantingTable.maxEssenceItems; i++) {
                if (wePlayer.enchantingTable.essenceItem[i].stack < 1)
                    wePlayer.enchantingTable.essenceItem[i] = new Item();
                wePlayer.enchantingTableUI.essenceSlotUI[i].Item = wePlayer.enchantingTable.essenceItem[i].Clone();
            }
        }
        public override void OnDeactivate() {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            if (wePlayer.enchantingTableUI?.itemSlotUI?[0]?.Item != null) {
                //Store item(s) left in enchanting table to player
                for (int i = 0; i < EnchantingTable.maxItems; i++) {
                    wePlayer.enchantingTable.item[i] = wePlayer.enchantingTableUI.itemSlotUI[i].Item.Clone();
                }

                //Store enchantments left in enchanting table to player
                for (int i = 0; i < EnchantingTable.maxEnchantments; i++) {
                    wePlayer.enchantingTable.enchantmentItem[i] = wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item.Clone();
                }

                //Store essence left in enchanting table to player
                for (int i = 0; i < EnchantingTable.maxEssenceItems; i++) {
                    if (wePlayer.enchantingTableUI.essenceSlotUI[i].Item.stack < 1)
                        wePlayer.enchantingTableUI.essenceSlotUI[i].Item = new Item();

                    wePlayer.enchantingTable.essenceItem[i] = wePlayer.enchantingTableUI.essenceSlotUI[i].Item.Clone();
                }
            }
        }
        public override void Update(GameTime gameTime) {
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            preventItemUse = false;

            //Change button color if hovering
            foreach (var panel in panels) {
                if (panel.BackgroundColor == bgColor || panel.BackgroundColor == hoverColor) {
                    panel.BackgroundColor = panel.IsMouseHovering ? hoverColor : bgColor;
                }
                else if (panel.BackgroundColor == red || panel.BackgroundColor == hoverRed) {
                    panel.BackgroundColor = panel.IsMouseHovering ? hoverRed : red;
                }
            }

            if (IsMouseHovering)
                preventItemUse = true;
        }
        private static void ConvertEssenceToXP(int tier) {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            Item essence = wePlayer.enchantingTableUI.essenceSlotUI[tier].Item;
            Item item = wePlayer.enchantingTableUI.itemSlotUI[0].Item;
            if (!essence.IsAir && !item.IsAir) {
                if(item.GetEnchantedItem().Experience < int.MaxValue) {
                    essence.stack--;
                    //ModContent.GetInstance<WEMod>().Logger.Info(wePlayer.Player.name + " applied " + essence.Name + " to their " + item.Name + " gaining " + ConfirmationUI.xpTiers[tier].ToString() + " xp.");
                    //Main.NewText(wePlayer.Player.name + " applied " + essence.Name + " to their " + item.Name + " gaining " + ConfirmationUI.xpTiers[tier].ToString() + " xp.");
                    item.GetEnchantedItem().GainXP(item, (int)EnchantmentEssence.xpPerEssence[tier]);
                    SoundEngine.PlaySound(SoundID.MenuTick);
                }
                else {
                    Main.NewText($"You cannot gain any more experience on your {item.S()}.");
                }
            }
        }
        public static int ConvertXPToEssence(int xp, bool consumeAll = false) {
            if(xp > 0) {
                WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
                if (wePlayer.usingEnchantingTable) {
                    for (int i = 0; i < EnchantingTable.maxEssenceItems; i++) {
                        wePlayer.enchantingTable.essenceItem[i] = wePlayer.enchantingTableUI.essenceSlotUI[i].Item.Clone();
                    }
                }
                /*bool usingEnchantingTable = wePlayer.usingEnchantingTable;
                if (!usingEnchantingTable)
                    WEModSystem.OpenWeaponEnchantmentUI();*/
                int numberEssenceRecieved;
                int xpCounter = wePlayer.highestTableTierUsed < 4 ? (int)Math.Round(xp * (0.6f + 0.1f * wePlayer.highestTableTierUsed)) : xp;
                int xpInitial = xpCounter;
                int xpNotConsumed = 0;
                for (int tier = EnchantingTable.maxEssenceItems - 1; tier >= 0; tier--) {
                    if (wePlayer.highestTableTierUsed >= tier) {
                        /*if(wePlayer.highestTableTierUsed == EnchantingTable.maxTier)
                        {
                            numberEssenceRecieved = xpCounter / (int)EnchantmentEssence.xpPerEssence[tier];
                            xpCounter %= (int)EnchantmentEssence.xpPerEssence[tier];
                        }
                        else
                        {*/
                        if (tier > 0) {
                            numberEssenceRecieved = xpCounter / (int)EnchantmentEssence.xpPerEssence[tier] * 4 / 5;
                        }
                        else {
                            numberEssenceRecieved = xpCounter / (int)EnchantmentEssence.xpPerEssence[tier];
                        }

                        xpCounter -= (int)EnchantmentEssence.xpPerEssence[tier] * numberEssenceRecieved;
                        //}
                        if (xpCounter < (int)EnchantmentEssence.xpPerEssence[0] && xpCounter > 0 && tier == 0) {
                            xpNotConsumed = xpCounter;
                            xpCounter = 0;
                        }

                        if (wePlayer.enchantingTable.essenceItem[tier].IsAir) {
                            wePlayer.enchantingTable.essenceItem[tier] = new Item(EnchantmentEssence.IDs[tier], numberEssenceRecieved);
                        }
                        else {
                            int maxStack = ModContent.GetModItem(ModContent.ItemType<EnchantmentEssenceBasic>()).Item.maxStack;
                            if (wePlayer.enchantingTable.essenceItem[tier].stack + numberEssenceRecieved > maxStack) {
                                int ammountToTransfer = maxStack - wePlayer.enchantingTable.essenceItem[tier].stack;
                                numberEssenceRecieved -= ammountToTransfer;
                                wePlayer.enchantingTable.essenceItem[tier].stack += ammountToTransfer;
                                while (numberEssenceRecieved > 0) {
                                    int stack = numberEssenceRecieved > maxStack ? maxStack : numberEssenceRecieved;
                                    numberEssenceRecieved -= stack;
                                    Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), ModContent.ItemType<EnchantmentEssenceBasic>() + tier, stack);
                                }
                            }
                            else {
                                wePlayer.enchantingTable.essenceItem[tier].stack += numberEssenceRecieved;
                            }
                        }
                    }
                }
                /*if (!usingEnchantingTable)
                    WEModSystem.CloseWeaponEnchantmentUI();*/
                if (wePlayer.usingEnchantingTable) {
                    for (int i = 0; i < EnchantingTable.maxEssenceItems; i++) {
                        wePlayer.enchantingTableUI.essenceSlotUI[i].Item = wePlayer.enchantingTable.essenceItem[i].Clone();
                    }
                }
                    
                return xpInitial - xpNotConsumed;
            }

            return 0;
        }
        public void Infusion() {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            Item tableItem = wePlayer.enchantingTableUI.itemSlotUI[0].Item;
            if (tableItem != null && !tableItem.IsAir) {
                if(wePlayer.infusionConsumeItem == null) {
                    if (WEMod.IsWeaponItem(tableItem) || WEMod.IsArmorItem(tableItem)) {
                        bool canConsume = false;
                        switch (tableItem.Name) {
                            case "Murasama":
                                Main.NewText("Murasama cannot be consumed for infusion until a check for the Jungle Dragon, Yharon being defeated can be added.");
                                break;
                            default:
                                canConsume = true;
                                break;
						}

						if(canConsume) {
                            if (tableItem.stack > 1) {
                                wePlayer.enchantingTableUI.itemSlotUI[0].Item.stack -= 1;
                                wePlayer.infusionConsumeItem = new Item(tableItem.type);
                                infusionButonText.SetText("Finalize");
                            }
                            else {
                                wePlayer.infusionConsumeItem = tableItem.Clone();
                                wePlayer.enchantingTableUI.itemSlotUI[0].Item = new Item();
                                infusionButonText.SetText("Cancel");
                            }
                        }
                    }
                }
                else {
                    if (WEMod.IsWeaponItem(tableItem) || WEMod.IsArmorItem(tableItem)) {
                        bool canInfuse = false;
						switch (tableItem.Name) {
                            case "Primary Zenith":
                                Main.NewText($"{tableItem.Name} resisted your attempt to empower it.");
                                break;
                            default:
                                canInfuse = true;
                                break;
                        }

                        if (canInfuse && wePlayer.enchantingTableUI.itemSlotUI[0].Item.TryInfuseItem(wePlayer.infusionConsumeItem, false, true)) {
                            ConfirmationUI.OfferItem(ref wePlayer.infusionConsumeItem, true, true);
                            wePlayer.infusionConsumeItem = null;
                            infusionButonText.SetText("Infusion");
                        }
                    }
                }
            }
            else if(wePlayer.infusionConsumeItem != null) {
                wePlayer.enchantingTableUI.itemSlotUI[0].Item = wePlayer.infusionConsumeItem.Clone();
                wePlayer.infusionConsumeItem = null;
                infusionButonText.SetText("Infusion");
            }
        }
        public static void Syphon() {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            if (!wePlayer.enchantingTableUI.itemSlotUI[0].Item.IsAir) {
                EnchantedItem iGlobal = wePlayer.enchantingTableUI.itemSlotUI[0].Item.GetEnchantedItem();
                if(iGlobal.Experience < WEModSystem.levelXps[EnchantedItem.MAX_LEVEL - 1] + EnchantmentEssence.xpPerEssence[0]) {
                    Main.NewText("You can only Syphon an item if it is max level and over " + (WEModSystem.levelXps[EnchantedItem.MAX_LEVEL - 1] + EnchantmentEssence.xpPerEssence[0]).ToString() + " experience.");
                }
                else {
                    int xp = iGlobal.Experience - WEModSystem.levelXps[EnchantedItem.MAX_LEVEL - 1];
                    iGlobal.Experience -= ConvertXPToEssence(xp);
                }
            }
        }
        private static void LootAll() {
            if (WEModSystem.promptInterface.CurrentState == null) {
                pressedLootAll = true;
                WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();

                //Take all enchantments first
                for (int i = 0; i < EnchantingTable.maxEnchantments; i++) {
                    if (!wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item.IsAir) {
                        wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item.position = wePlayer.Player.Center;
                        wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item = wePlayer.Player.GetItem(Main.myPlayer, wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item, GetItemSettings.LootAllSettings);
                        if (wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item.stack < 1) {
                            WEModSystem.RemoveEnchantment(i);
                            //Enchantment enchantment = (Enchantment)wePlayer.enchantingTableUI.itemSlotUI[0].Item.G().enchantments[i].ModItem;
                            //enchantment.statsSet = false;
                            //wePlayer.enchantingTableUI.itemSlotUI[0].Item.UpdateEnchantment(ref enchantment, i, true);
                            wePlayer.enchantingTableUI.itemSlotUI[0].Item.GetEnchantedItem().enchantments[i] = new Item();
                        }
                    }
                }

                //Take item(s)
                for (int i = 0; i < EnchantingTable.maxItems; i++)
                {
                    if (!wePlayer.enchantingTableUI.itemSlotUI[i].Item.IsAir)
                    {
                        wePlayer.enchantingTableUI.itemSlotUI[i].Item.position = wePlayer.Player.Center;
                        wePlayer.enchantingTableUI.itemSlotUI[i].Item = wePlayer.Player.GetItem(Main.myPlayer, wePlayer.enchantingTableUI.itemSlotUI[i].Item, GetItemSettings.LootAllSettings);
                    }
                }

                pressedLootAll = false;
            }
        }
        private static void Offer(Item item = null, bool noOre = false) {
            if (WEModSystem.promptInterface.CurrentState == null) {
                WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
                if (!wePlayer.enchantingTableUI.itemSlotUI[0].Item.IsAir) {
                    WEModSystem.weModSystemUI.SetState(null);
                    UIState state = new UIState();
                    state.Append(wePlayer.confirmationUI);
                    WEModSystem.promptInterface.SetState(state);
                }
            }
        }
        private static void LevelUp() {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            Item tableItem = wePlayer.enchantingTableUI.itemSlotUI[0].Item;
            if (!tableItem.IsAir) {
                int xpAvailable = 0;
		        int nonFavoriteXpAvailable = 0;
                EnchantedItem iGlobal = tableItem.GetEnchantedItem();
                if(iGlobal.levelBeforeBooster != EnchantedItem.MAX_LEVEL) {
                    for (int i = EnchantingTable.maxEnchantments - 1; i >= 0; i--) {
		    	        int xpToAdd = (int)EnchantmentEssence.xpPerEssence[i] * wePlayer.enchantingTableUI.essenceSlotUI[i].Item.stack;
                        xpAvailable += xpToAdd;
			            if(!wePlayer.enchantingTableUI.essenceSlotUI[i].Item.favorited)
				            nonFavoriteXpAvailable += xpToAdd;
                    }

                    int xpNeeded = WEModSystem.levelXps[iGlobal.levelBeforeBooster] - iGlobal.Experience;
		            bool enoughWithoutFavorite = nonFavoriteXpAvailable >= xpNeeded;
                    if (xpAvailable >= xpNeeded) {
                        for (int i = EnchantingTable.maxEnchantments - 1; i >= 0; i--) {
                            Item essenceItem = wePlayer.enchantingTableUI.essenceSlotUI[i].Item;
                            bool allowUsingThisEssence = !wePlayer.enchantingTableUI.essenceSlotUI[i].Item.favorited || !enoughWithoutFavorite;
                            int stack = essenceItem.stack;
                            int numberEssenceNeeded = xpNeeded / (int)EnchantmentEssence.xpPerEssence[i];
                            int numberEssenceTransfered = 0;
			                if(allowUsingThisEssence) {
			    	            if(numberEssenceNeeded > stack) {
                                	numberEssenceTransfered = stack;
                            	}
                            	else {
                                	numberEssenceTransfered = numberEssenceNeeded;
                            	}
			                }

                            int xpAvailableBelowMe = 0;
                            for (int j = i - 1; j >= 0; j--) {
			    	            if(!wePlayer.enchantingTableUI.essenceSlotUI[j].Item.favorited || !enoughWithoutFavorite)
                                	xpAvailableBelowMe += (int)EnchantmentEssence.xpPerEssence[j] * wePlayer.enchantingTableUI.essenceSlotUI[j].Item.stack;
                            }

                            if(allowUsingThisEssence && xpAvailableBelowMe < xpNeeded - (int)EnchantmentEssence.xpPerEssence[i] * numberEssenceTransfered)
                                numberEssenceTransfered++;

                            if (numberEssenceTransfered > 0) {
                                int xpTransfered = (int)EnchantmentEssence.xpPerEssence[i] * numberEssenceTransfered;
                                xpNeeded -= xpTransfered;
                                essenceItem.stack -= numberEssenceTransfered;
                                iGlobal.GainXP(tableItem, xpTransfered);
                            }
                        }
                    }
                    else {
                        Main.NewText("Not Enough Essence. You need " + xpNeeded + " experience for level " + (iGlobal.levelBeforeBooster + 1).ToString() + " you only have " + xpAvailable + " available.");
                    }
                }
                else {
                    Main.NewText("Your " + tableItem.Name + " is already max level.");
                }
            }
        }
    }
}
