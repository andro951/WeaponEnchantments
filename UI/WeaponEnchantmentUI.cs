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

        //public static string[] ButtonNames = new string[] { "Enchant", "Disenchant", "Offer", "Level", "Syphon" };
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
            string item = TableTextID.Item.ToString().Lang(L_ID1.TableText);
            string enchantments = TableTextID.Enchantments.ToString().Lang(L_ID1.TableText);
            string utility = EnchantmentGeneralTooltipsID.Utility.ToString().Lang(L_ID1.Tooltip, L_ID2.EnchantmentGeneralTooltips);
            string labels = $"{item}{' '.FillString(15 - item.Length)}{enchantments}{' '.FillString(18 - enchantments.Length)}{utility}{' '.FillString(9 - utility.Length)}";
            titleText = new UIText(labels) {
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
                    List<string> texts = new();
                    texts.Add(TableTextID.weapon0.ToString().Lang(L_ID1.TableText));
                    for (int j = 1; j <= 3; j++) {
                        texts.Add($"general{j}".Lang(L_ID1.TableText));
                    }

                    texts.PadStrings();

                    if (timer < 60) {
                        Main.hoverItemName = texts[0];
					}
					else {
                        Main.hoverItemName = texts.JoinList("\n");
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

                    string enchantment4String = TableTextID.enchantment4.ToString().Lang(L_ID1.TableText, new object[] { ContentSamples.ItemsByType[EnchantingTableItem.IDs[i]].Name });
                    wePlayer.enchantingTableUI.enchantmentSlotUI[i].OnMouseover += (timer) => {
                        List<string> texts = new();
                        texts.Add(TableTextID.enchantment0.ToString().Lang(L_ID1.TableText));
                        for (int j = 1; j <= 3; j++) {
                            texts.Add($"general{j}".Lang(L_ID1.TableText));
                        }

                        texts.Add(enchantment4String);

                        texts.PadStrings();

                        if (timer < 60) {
                            Main.hoverItemName = texts[0];
                        }
                        else {
                            Main.hoverItemName = texts.JoinList("\n");
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
                        List<string> texts = new();
                        texts.Add(TableTextID.utility0.ToString().Lang(L_ID1.TableText));
                        for (int j = 1; j <= 3; j++) {
                            texts.Add($"general{j}".Lang(L_ID1.TableText));
                        }

                        texts.PadStrings();

                        if (timer < 60) {
                            Main.hoverItemName = texts[0];
                        }
                        else {
                            Main.hoverItemName = texts.JoinList("\n");
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

                string essence0String = TableTextID.essence0.ToString().Lang(L_ID1.TableText, new object[] { ContentSamples.ItemsByType[EnchantmentEssence.IDs[i]].Name });
                wePlayer.enchantingTableUI.essenceSlotUI[i].OnMouseover += (timer) => {
                    List<string> texts = new();
                    texts.Add(essence0String);
                    for (int j = 1; j <= 3; j++) {
                        texts.Add($"general{j}".Lang(L_ID1.TableText));
                    }

                    texts.PadStrings();

                    if (timer < 60) {
                        Main.hoverItemName = texts[0];
                    }
                    else {
                        Main.hoverItemName = texts.JoinList("\n");
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

                UIText xpButonText = new UIText(TableTextID.xp.ToString().Lang(L_ID1.TableText)) {
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
            UIText levelButonText = new UIText(TableTextID.LevelUp.ToString().Lang(L_ID1.TableText)) {
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
            UIText syphonButonText = new UIText(TableTextID.Syphon.ToString().Lang(L_ID1.TableText)) {
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
                    infusionText = TableTextID.Cancel.ToString().Lang(L_ID1.TableText);
                else
                    infusionText = TableTextID.Finalize.ToString().Lang(L_ID1.TableText);
            }
            else {
                infusionText = TableTextID.Infusion.ToString().Lang(L_ID1.TableText);
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
            UIText lootAllButonText = new UIText(TableTextID.LootAll.ToString().Lang(L_ID1.TableText)) {
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
            UIText offerButtonText = new UIText(TableTextID.Offer.ToString().Lang(L_ID1.TableText)) {
                Top = { Pixels = -4f },
                Left = { Pixels = -6f }
            };

            button[ButtonID.Offer].Append(offerButtonText);
            Append(button[ButtonID.Offer]);
            panels.Add(button[ButtonID.Offer]);
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
            if (essence.IsAir || item.IsAir)
                return;

            if (!item.TryGetEnchantedItem(out EnchantedItem iGlobal))
                return;

            if(iGlobal.Experience < int.MaxValue) {
                essence.stack--;
                int xp = (int)EnchantmentEssence.xpPerEssence[tier];
                iGlobal.GainXP(item, xp);
                SoundEngine.PlaySound(SoundID.MenuTick);
            }
            else {
                Main.NewText($"You cannot gain any more experience on your {item.S()}.");
            }
        }
        public static int ConvertXPToEssence(int xp, bool consumeAll = false, Item item = null) {
            if(xp <= 0)
                return 0;

            if (WEMod.magicStorageEnabled) $"Converted xp to essence. xp: {xp}, consumeAll: {consumeAll.S()}".Log();
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();

            //Force player essence data to sync with the ui
            if (wePlayer.usingEnchantingTable) {
                for (int i = 0; i < EnchantingTable.maxEssenceItems; i++) {
                    wePlayer.enchantingTable.essenceItem[i] = wePlayer.enchantingTableUI.essenceSlotUI[i].Item.Clone();
                }
            }

            //Apply table tier reduction
            int xpCounter;
            if (WEMod.serverConfig.ReduceOfferEfficiencyByTableTier && wePlayer.highestTableTierUsed < 4) {
                //Tier 3 or lower table
                float essenceReduction = 0.6f + 0.1f * wePlayer.highestTableTierUsed;
                xpCounter = (int)Math.Round(xp * essenceReduction);
            }
			else {
                //Tier 4 table
                xpCounter = xp;
            }

            if (item?.TryGetEnchantedItem(out EnchantedWeapon enchantedWeapon) == true && WEMod.serverConfig.ReduceOfferEfficiencyByBaseInfusionPower) {
                float infusionPower = Math.Min((float)enchantedWeapon.infusionPower, 1100f);
                xpCounter = (int)Math.Round((float)xpCounter * (1f - 0.2f * (infusionPower / 1100f)));
            }

            int xpInitial = xpCounter;
            int xpNotConsumed = 0;
            int numberEssenceRecieved;
            for (int tier = EnchantingTable.maxEssenceItems - 1; tier >= 0; tier--) {
                if (wePlayer.highestTableTierUsed < tier)
                    continue;

                int xpPerEssence = (int)EnchantmentEssence.xpPerEssence[tier];
                if (tier > 0) {
                    numberEssenceRecieved = xpCounter / xpPerEssence * 4 / 5;
                }
                else {
                    numberEssenceRecieved = xpCounter / xpPerEssence;
                }


                xpCounter -= (int)EnchantmentEssence.xpPerEssence[tier] * numberEssenceRecieved;
                if (tier == 0 && xpCounter > 0) {
					if (consumeAll) {
                        numberEssenceRecieved++;
					}
					else {
                        xpNotConsumed = xpCounter;
                    }
                    xpCounter = 0;
                }

                //Get or spawn essence
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
                            Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), EnchantmentEssence.IDs[tier], stack);
                        }
                    }
                    else {
                        wePlayer.enchantingTable.essenceItem[tier].stack += numberEssenceRecieved;
                    }
                }
            }

            //Force player essence data to sync with the ui
            if (wePlayer.usingEnchantingTable) {
                for (int i = 0; i < EnchantingTable.maxEssenceItems; i++) {
                    wePlayer.enchantingTableUI.essenceSlotUI[i].Item = wePlayer.enchantingTable.essenceItem[i].Clone();
                }
            }
                    
            return xpInitial - xpNotConsumed;
        }
        public void Infusion() {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            Item tableItem = wePlayer.enchantingTableUI.itemSlotUI[0].Item;
            if (tableItem.TryGetEnchantedItem()) {
                if (!EnchantedItemStaticMethods.IsWeaponItem(tableItem) && !EnchantedItemStaticMethods.IsArmorItem(tableItem))
                    return;

                if (wePlayer.infusionConsumeItem == null) {

                    bool canConsume = false;

                    //Prevent specific items from being consumed for infusion.
                    switch (tableItem.Name) {
                        case "Murasama":
                            Main.NewText("Murasama cannot be consumed for infusion until a check for the Jungle Dragon, Yharon being defeated can be added.");
                            break;
                        default:
                            canConsume = true;
                            break;
					}

                    if (!canConsume)
                        return;

                    //Store item for infusion
                    if (tableItem.stack > 1) {
                        wePlayer.enchantingTableUI.itemSlotUI[0].Item.stack -= 1;
                        wePlayer.infusionConsumeItem = new Item(tableItem.type);
                        infusionButonText.SetText(TableTextID.Finalize.ToString().Lang(L_ID1.TableText));
                    }
                    else {
                        if (wePlayer.ItemInUI().TryGetEnchantedItem(out EnchantedItem enchantedItem) && enchantedItem.favorited) {
                            Main.NewText("Favorited items cannot be consumed for infusion.");
                            return;
                        }

                        wePlayer.infusionConsumeItem = tableItem.Clone();
                        wePlayer.enchantingTableUI.itemSlotUI[0].Item = new Item();
                        infusionButonText.SetText(TableTextID.Cancel.ToString().Lang(L_ID1.TableText));
                    }
                }
                else {
                    bool canInfuse = false;

                    //Prevent specific items from being upgraded with infusion.
					switch (tableItem.Name) {
                        case "Primary Zenith":
                            Main.NewText($"{tableItem.Name} resisted your attempt to empower it.");
                            break;
                        default:
                            canInfuse = true;
                            break;
                    }

                    if (!canInfuse)
                        return;

                    //Infuse (Finalize)
                    if (wePlayer.enchantingTableUI.itemSlotUI[0].Item.TryInfuseItem(wePlayer.infusionConsumeItem, false, true)) {
                        ConfirmationUI.OfferItem(ref wePlayer.infusionConsumeItem, true, true);
                        wePlayer.infusionConsumeItem = null;
                        infusionButonText.SetText(TableTextID.Infusion.ToString().Lang(L_ID1.TableText));
                    }
                }
            }
            else if(wePlayer.infusionConsumeItem != null) {
                //Return infusion item to table
                wePlayer.enchantingTableUI.itemSlotUI[0].Item = wePlayer.infusionConsumeItem.Clone();
                wePlayer.infusionConsumeItem = null;
                infusionButonText.SetText(TableTextID.Infusion.ToString().Lang(L_ID1.TableText));
            }
        }
        public static void Syphon() {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            Item itemInUI = wePlayer.ItemInUI();

            if (itemInUI.IsAir)
                return;

            if (!itemInUI.TryGetEnchantedItem(out EnchantedItem iGlobal))
                return;

            int maxLevelXP = WEModSystem.levelXps[EnchantedItem.MAX_Level - 1];
            int smallestXpPerEssence = (int)EnchantmentEssence.xpPerEssence[0];
            int minimumXPToSyphon = maxLevelXP + smallestXpPerEssence;
            if (iGlobal.Experience < minimumXPToSyphon) {
                Main.NewText($"You can only Syphon an item if it is max level and over {minimumXPToSyphon} experience.");
            }
            else {
                int xp = iGlobal.Experience - maxLevelXP;
                if (WEMod.magicStorageEnabled) $"Syphon(), itemInUI: {itemInUI.S()}".Log();
                iGlobal.Experience -= ConvertXPToEssence(xp, item: itemInUI);
            }
        }
        private static void LootAll() {
            if (WEModSystem.promptInterface.CurrentState != null)
                return;

            pressedLootAll = true;

            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            wePlayer.ItemInUI().TryGetEnchantedItem(out EnchantedItem iGlobal);

            //Take all enchantments first
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++) {
                if (!wePlayer.EnchantmentInUI(i).IsAir) {
                    wePlayer.EnchantmentInUI(i).position = wePlayer.Player.Center;
                    wePlayer.EnchantmentUISlot(i).Item = wePlayer.Player.GetItem(Main.myPlayer, wePlayer.EnchantmentInUI(i), GetItemSettings.LootAllSettings);
                    if (wePlayer.EnchantmentInUI(i).stack < 1) {
                        EnchantedItemStaticMethods.RemoveEnchantment(i);
                    }
                }
            }

            //Take item(s)
            for (int i = 0; i < EnchantingTable.maxItems; i++) {
                if (!wePlayer.ItemInUI().IsAir) {
                    wePlayer.ItemInUI().position = wePlayer.Player.Center;
                    wePlayer.ItemUISlot().Item = wePlayer.Player.GetItem(Main.myPlayer, wePlayer.ItemInUI(), GetItemSettings.LootAllSettings);
                }
            }

            pressedLootAll = false;
        }
        private static void Offer() {
            if (WEModSystem.promptInterface.CurrentState != null)
                return;

            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            if (wePlayer.ItemInUI().IsAir)
                return;

            if (wePlayer.ItemInUI().TryGetEnchantedItem(out EnchantedItem enchantedItem) && enchantedItem.favorited) {
                Main.NewText("Favorited items cannot be offerd.");
                return;
			}
            
            WEModSystem.weModSystemUI.SetState(null);
            UIState state = new UIState();
            state.Append(wePlayer.confirmationUI);
            WEModSystem.promptInterface.SetState(state);
        }
        private static void LevelUp() {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            Item tableItem = wePlayer.enchantingTableUI.itemSlotUI[0].Item;
            if (!wePlayer.ItemInUI().TryGetEnchantedItem(out EnchantedItem iGlobal))
                return;

            int xpAvailable = 0;
		    int nonFavoriteXpAvailable = 0;
            if(iGlobal.levelBeforeBooster == EnchantedItem.MAX_Level) {
                Main.NewText("Your " + tableItem.Name + " is already max level.");
                return;
            }

            //xpAvailable
            for (int i = EnchantingTable.maxEnchantments - 1; i >= 0; i--) {
		    	int xpToAdd = WEMath.MultiplyCheckOverflow((int)EnchantmentEssence.xpPerEssence[i], wePlayer.enchantingTableUI.essenceSlotUI[i].Item.stack);
                xpAvailable.AddCheckOverflow(xpToAdd);
			    if(!wePlayer.enchantingTableUI.essenceSlotUI[i].Item.favorited)
				    nonFavoriteXpAvailable.AddCheckOverflow(xpToAdd);
            }

            //xpNeeded
            int xpNeeded = WEModSystem.levelXps[iGlobal.levelBeforeBooster] - iGlobal.Experience;
		    bool enoughWithoutFavorite = nonFavoriteXpAvailable >= xpNeeded;
            if (xpAvailable < xpNeeded) {
                Main.NewText("Not Enough Essence. You need " + xpNeeded + " experience for level " + (iGlobal.levelBeforeBooster + 1).ToString() + " you only have " + xpAvailable + " available.");
                return;
            }

            //Consume xp and convert to essence
            for (int i = EnchantingTable.maxEnchantments - 1; i >= 0; i--) {
                Item essenceItem = wePlayer.EssenceInTable(i);
                bool allowUsingThisEssence = !essenceItem.favorited || !enoughWithoutFavorite;
                int stack = essenceItem.stack;
                int xpPerEssence = (int)EnchantmentEssence.xpPerEssence[i];
                int numberEssenceNeeded = xpNeeded / xpPerEssence;
                int numberEssenceTransfered = 0;
			    if(allowUsingThisEssence) {
			    	if(numberEssenceNeeded > stack) {
                        numberEssenceTransfered = stack;
                    }
                    else {
                        numberEssenceTransfered = numberEssenceNeeded;
                    }
			    }

                //Check essence available below me
                int xpAvailableBelowThis = 0;
                for (int j = i - 1; j >= 0; j--) {
			    	if(!wePlayer.EssenceInTable(j).favorited || !enoughWithoutFavorite) {
                        int xpPerEssenceLowerTier = (int)EnchantmentEssence.xpPerEssence[j];
                        xpAvailableBelowThis += xpPerEssenceLowerTier * wePlayer.EssenceInTable(j).stack;
                    }
                }

                if(allowUsingThisEssence && xpAvailableBelowThis < xpNeeded - xpPerEssence * numberEssenceTransfered)
                    numberEssenceTransfered++;

                if (numberEssenceTransfered > 0) {
                    int xpTransfered = xpPerEssence * numberEssenceTransfered;
                    xpNeeded -= xpTransfered;
                    essenceItem.stack -= numberEssenceTransfered;
                    iGlobal.GainXP(tableItem, xpTransfered);
                }
            }
        }

        //TODO
        //Add max level up button.                
    }
}
