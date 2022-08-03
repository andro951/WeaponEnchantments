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
using static WeaponEnchantments.Common.Configs.ConfigValues;

namespace WeaponEnchantments.UI
{
    public class ConfirmationUI : UIPanel
    {
        public class ConfirmationButtonID
        {
            public const int Yes = 0;
            public const int No = 1;
            public const int Count = 2;
        }

        public static string[] ConfirmationButtonNames = new string[] { "Yes", "No" };
        public UIText promptText;
        private UIPanel[] confirmationButton = new UIPanel[ConfirmationButtonID.Count];
        private List<UIPanel> confirmationPanels;
        private readonly static Color red = new Color(171, 76, 73);
        private readonly static Color hoverRed = new Color(184, 103, 100);
        private readonly static Color bgColor = new Color(73, 94, 171);
        private readonly static Color hoverColor = new Color(100, 118, 184);

        internal const int width = 680;
        internal const int height = 155;

        internal int RelativeLeft => Main.screenWidth / 2 - width / 2;
        internal int RelativeTop => Main.screenHeight / 2 + 42;
        public override void OnInitialize() {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            Width.Pixels = width;
            Height.Pixels = height;
            Top.Pixels = int.MaxValue / 2;
            Left.Pixels = int.MaxValue / 2 + 100 - 25;
            BackgroundColor = red;

            confirmationPanels = new List<UIPanel>();

            float nextElementY = -PaddingTop / 2;
            //Confirmation label
            promptText = new UIText("") {
                Top = { Pixels = nextElementY + 15 },
                Left = { Pixels = 70 },
                HAlign = 0.5f
            };

            Append(promptText);
            nextElementY += 20;

            nextElementY += 50;
            float ratioFromCenter = 0.22f;

            //Yes Button
            confirmationButton[ConfirmationButtonID.Yes] = new UIPanel() {
                Top = { Pixels = nextElementY },
                Left = { Pixels = -66 - 56 },
                Width = { Pixels = 100f },
                Height = { Pixels = 30f },
                HAlign = 0.5f - ratioFromCenter,
                BackgroundColor = red
            };

            confirmationButton[ConfirmationButtonID.Yes].OnClick += (evt, element) => { ConfirmOffer(); };
            UIText yesButtonText = new UIText("Yes") {
                Top = { Pixels = -4f },
                Left = { Pixels = -6f }
            };

            confirmationButton[ConfirmationButtonID.Yes].Append(yesButtonText);
            Append(confirmationButton[ConfirmationButtonID.Yes]);
            confirmationPanels.Add(confirmationButton[ConfirmationButtonID.Yes]);

            nextElementY += 35;

            //No Button
            confirmationButton[ConfirmationButtonID.No] = new UIPanel() {
                Top = { Pixels = nextElementY },
                Left = { Pixels = -66 - 56 },
                Width = { Pixels = 100f },
                Height = { Pixels = 30f },
                HAlign = 0.5f - ratioFromCenter,
                BackgroundColor = bgColor
            };

            confirmationButton[ConfirmationButtonID.No].OnClick += (evt, element) => { DeclineOffer(); };
            UIText noButtonText = new UIText("No") {
                Top = { Pixels = -4f },
                Left = { Pixels = -6f }
            };

            confirmationButton[ConfirmationButtonID.No].Append(noButtonText);
            Append(confirmationButton[ConfirmationButtonID.No]);
            confirmationPanels.Add(confirmationButton[ConfirmationButtonID.No]);
        }
        private static void DeclineOffer() {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            WEModSystem.promptInterface.SetState(null);
            UIState state = new UIState();
            state.Append(wePlayer.enchantingTableUI);
            WEModSystem.weModSystemUI.SetState(state);
        }
        public static void ConfirmOffer() {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();

            //Offer the item in the table
            int type = OfferItem(ref wePlayer.enchantingTableUI.itemSlotUI[0].Item);
            if (type <= 0)
                return;


            if (!WEMod.clientConfig.OfferAll)
                return;

            Player player = Main.LocalPlayer;

            //Offer every non-Modified item with the same type in the player's inventory.
            for (int i = 0; i < player.inventory.Length; i++) {
                if (!player.inventory[i].TryGetEnchantedItem(out EnchantedItem iGlobal))
                    continue;

                //Offer the inventory item
                if (player.inventory[i].type == type && !iGlobal.Modified)
                    OfferItem(ref player.inventory[i], false, true);
            }
        }
        public static int OfferItem(ref Item item, bool noOre = false, bool nonTableItem = false) {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();

            //Close Offer Prompt interface and open the WeaponEnchantmentUI
            if (item.IsSameEnchantedItem(wePlayer.enchantingTableUI.itemSlotUI[0].Item)) {
                WEModSystem.promptInterface.SetState(null);
                UIState state = new UIState();
                state.Append(wePlayer.enchantingTableUI);
                WEModSystem.weModSystemUI.SetState(state);
            }

            int type = item.type;
            bool stop = false;
            if (!item.TryGetEnchantedItem(out EnchantedItem iGlobal))
                return -1;

            //Enchantments
            for (int i = 0; i < EnchantingTable.maxEnchantments && !stop; i++) {
                if (!nonTableItem && !wePlayer.EnchantmentInUI(i).IsAir) {
                    wePlayer.EnchantmentUISlot(i).Item = wePlayer.Player.GetItem(Main.myPlayer, wePlayer.EnchantmentInUI(i), GetItemSettings.LootAllSettings);
                }
                else if (!iGlobal.enchantments[i].IsAir) {
                    Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), iGlobal.enchantments[i]);
                }

                if (!nonTableItem && !wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item.IsAir)
                    stop = true;//Player didn't have enough space in their inventory to take all enchantments
            }

            if (stop)
                return -1;

            //Power Booster
            if (iGlobal.PowerBoosterInstalled) {
                Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), ModContent.ItemType<PowerBooster>());
            }

            int xp = item.GetEnchantedItem().Experience;
            float value = item.value + (item.stack > 1 ? ContentSamples.ItemsByType[item.type].value * (item.stack - 1) : 0f);

            //Xp -> Essence
            WeaponEnchantmentUI.ConvertXPToEssence(xp, true);

            //Item value -> ores/essence
            if (!noOre) {
                int essenceValue = (int)(value * PercentOfferEssence);
                int valueConvertedToOre = (int)Math.Round(value - (float)essenceValue);

                //Ores
                if (valueConvertedToOre > 0) {
                    int[] ores = { ItemID.ChlorophyteOre, WorldDataManager.AdamantiteOre, WorldDataManager.MythrilOre, WorldDataManager.CobaltOre, WorldDataManager.GoldOre, WorldDataManager.SilverOre, WorldDataManager.IronOre };
                    
                    int indexStart;
                    int indexOfGold = ores.Length - 3;
                    bool canRecieveHighTierOres = WEMod.serverConfig.AllowHighTierOres && Main.hardMode;
                    if (!canRecieveHighTierOres) {
                        //Start at Gold
                        indexStart = indexOfGold;
                    }
                    else {
                        bool canGetChlorophyte = NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3;
                        if (!canGetChlorophyte) {
                            //Start at Adamantite
                            indexStart = 1;
                        }
                        else {
                            //Start at Chlorophyte
                            indexStart = 0;
                        }
                    }

                    for (int i = indexStart; i < ores.Length; i++) {
                        int orevalue = ContentSamples.ItemsByType[ores[i]].value;
                        int stack;
                        int oreType = ores[i];
                        if (oreType > ItemID.IronOre) {
                            float oreWeightingMultiplier = i >= indexOfGold ? 0.8f : 0.2f;
                            
                            //Convert a portion of the remaining value into ore
                            stack = (int)Math.Round(valueConvertedToOre * oreWeightingMultiplier / orevalue);
                        }
                        else {
                            //Convert the rest of the remaining value to iron ore
                            stack = (int)((float)valueConvertedToOre / (float)orevalue);
                        }

                        valueConvertedToOre -= stack * orevalue;

                        //Round up for iron
                        if (oreType == ItemID.IronOre)
                            stack++;

                        //Spawn ores
                        if (stack > 0)
                            Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), ores[i], stack);
                    }
                }

                //Essence
                if (essenceValue > 0)
                    WeaponEnchantmentUI.ConvertXPToEssence(essenceValue, true);
            }

            item = new Item();
            SoundEngine.PlaySound(SoundID.Grab);

            return type;
        }
        public override void Update(GameTime gameTime) {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            Left.Pixels = RelativeLeft + 100 - 25;
            Top.Pixels = RelativeTop;
            WeaponEnchantmentUI.preventItemUse = false;

            //Change button color if hovering
            foreach (var panel in confirmationPanels) {
                if (panel.BackgroundColor == bgColor || panel.BackgroundColor == hoverColor) {
                    panel.BackgroundColor = panel.IsMouseHovering ? hoverColor : bgColor;
                }
                else if (panel.BackgroundColor == red || panel.BackgroundColor == hoverRed) {
                    panel.BackgroundColor = panel.IsMouseHovering ? hoverRed : red;
                }
            }
            if (IsMouseHovering) WeaponEnchantmentUI.preventItemUse = true;
            promptText.SetText($"Are you sure you want to PERMENANTLY DESTROY your\n" +
				$"level {wePlayer.enchantingTableUI.itemSlotUI[0].Item.GetEnchantedItem().level} {wePlayer.enchantingTableUI.itemSlotUI[0].Item.Name}\n" +
				$"In exchange for Iron, Silver and Gold ore and Essence?\n" +
				$"(Based on item value/experience.  Enchantments will be returned.)"
            );
        }
    }
}
