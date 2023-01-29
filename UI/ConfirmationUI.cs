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
using WeaponEnchantments.Tiles;

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

        //internal const int width = 680;
        internal const int width = 750;
        internal const int height = 155;

        internal int RelativeLeft => Main.screenWidth / 2 - width / 2 + 35;
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
                Left = { Pixels = -66 - 56 - 25 },
                Width = { Pixels = 100f },
                Height = { Pixels = 30f },
                HAlign = 0.5f - ratioFromCenter,
                BackgroundColor = red
            };

            confirmationButton[ConfirmationButtonID.Yes].OnClick += (evt, element) => { ConfirmOffer(); };
            UIText yesButtonText = new UIText(TableTextID.Yes.ToString().Lang(L_ID1.TableText)) {
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
                Left = { Pixels = -66 - 56 - 25 },
                Width = { Pixels = 100f },
                Height = { Pixels = 30f },
                HAlign = 0.5f - ratioFromCenter,
                BackgroundColor = bgColor
            };

            confirmationButton[ConfirmationButtonID.No].OnClick += (evt, element) => { DeclineOffer(); };
            UIText noButtonText = new UIText(TableTextID.No.ToString().Lang(L_ID1.TableText)) {
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
                if (player.inventory[i].favorited)
                    continue;

                if (!player.inventory[i].TryGetEnchantedItem(out EnchantedItem iGlobal))
                    continue;

                //Offer the inventory item
                if (player.inventory[i].type == type && !iGlobal.Modified)
                    OfferItem(ref player.inventory[i], false, true);
            }

            if (FindEnchantingTable(player, out Point tablePoint)) {
                if (FindChestsInRange(tablePoint.X, tablePoint.Y, out List<int> chests, xRangeLeft: 2, xRangeRight: 2, yRangeUp: -1, yRangeDown: 1, exactPoints: true)) {
                    foreach(int chestNum in chests) {
                        int chestLength = Main.chest[chestNum].item.Length;
                        for (int i = 0; i < chestLength; i++) {
                            Item item = Main.chest[chestNum].item[i];
                            if (item.favorited)
                                continue;

                            if (!item.TryGetEnchantedItem(out EnchantedItem enchantedItem))
                                continue;

                             if (enchantedItem.Modified)
                                continue;

                            OfferItem(ref Main.chest[chestNum].item[i], false, true);
                        }
					}
				}
			}
        }
        public static bool FindEnchantingTable(Player player, out Point table) {
            table = new();
            Point clicked = player.GetWEPlayer().enchantingTableLocation;
            if (clicked.X == -1 && clicked.Y == -1)
                return false;

            int tileType = Main.tile[clicked.X - 1, clicked.Y].TileType;
            if (EnchantingTableTile.TableTypes.Contains(tileType)) {
                table = new(clicked.X - 1, clicked.Y);
			}
			else {
                table = new(clicked.X, clicked.Y);
			}

            return true;
		}
        public static bool FindChestsInRange(int xNum, int yNum, out List<int> chests, int xRangeRight = 0, int yRangeUp = 0, int xRangeLeft = int.MinValue, int yRangeDown = 0, bool exactPoints = false) {
            chests = new();
            Point low = new Point(xNum - xRangeLeft, yNum - yRangeDown);
            Point high = new Point(xNum + xRangeRight, yNum + yRangeUp);

            if (exactPoints) {
                CheckAddChest(low.X, low.Y, chests);
                CheckAddChest(high.X, high.Y, chests);
                return chests.Count > 0;
            }

            for(int x = low.X; x <= high.X; x++) {
                for(int y = low.Y; y <= high.Y; y++) {
                    CheckAddChest(x, y, chests);
                }
			}

            return chests.Count > 0;
		}
        private static void CheckAddChest(int x, int y, List<int> chests) {
            if (Main.tileContainer[Main.tile[x, y].TileType]) {
                int chestNum = Chest.FindChest(x, y);
                if (chestNum != -1) {
                    chests.Add(chestNum);
                }
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
            if (iGlobal.PowerBoosterInstalled)
                Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), ModContent.ItemType<PowerBooster>());

            //Ultra Power Booster
            if (iGlobal.UltraPowerBoosterInstalled)
                Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), ModContent.ItemType<UltraPowerBooster>());

            int xp = iGlobal.Experience;
            float value = item.value - iGlobal.lastValueBonus;

            //Xp -> Essence
            if (WEMod.magicStorageEnabled) $"OfferItem(item: {item}, noOre: {noOre.S()}, nonTableItem: {nonTableItem.S()})".Log();
            WeaponEnchantmentUI.ConvertXPToEssence(xp, true, item);

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
                if (essenceValue > 0) {
                    if (WEMod.magicStorageEnabled) $"essenceValue > 0, OfferItem(item: {item}, noOre: {noOre.S()}, nonTableItem: {nonTableItem.S()})".Log();
                    WeaponEnchantmentUI.ConvertXPToEssence(essenceValue, true, item);
                }
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

            if (!wePlayer.ItemInUI().TryGetEnchantedItem(out EnchantedItem iGlobal)) {
                promptText.SetText($"Non-Enchantable item detected in table.\n" +
					$"WARNING, DO NOT PRESS CONFIRM.\n" +
					$"Please report this issue to andro951(Weapon Enchantments)");
                return;
            }

            int oresEnd = !WEMod.serverConfig.AllowHighTierOres || !Main.hardMode ? 3 : 8;
            bool canGetChlorophyte = NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3;
            if (canGetChlorophyte)
                oresEnd++;

            string oreString = $"({WorldDataManager.GetOreNamesList(1, oresEnd)})";
            float percentEss = PercentOfferEssence;
            string oreAndEssencePercent;
            if (percentEss == 1f) {
                //oreAndEssencePercent = $"In exchange for essence?";
                oreAndEssencePercent = TableTextID.ExchangeEssence.ToString().Lang(L_ID1.TableText);
            }
            else if (percentEss == 0f) {
                //oreAndEssencePercent = $"In exchange for ores?";
                oreAndEssencePercent = TableTextID.ExchangeOres.ToString().Lang(L_ID1.TableText);
            }
			else {
                //oreAndEssencePercent = $"In exchange for ores({(1f - percentEss).PercentString()}) and essence({percentEss.PercentString()})?";
                oreAndEssencePercent = TableTextID.ExchangeEssenceAndOres.ToString().Lang(L_ID1.TableText, new object[] { (1f - percentEss).PercentString(), percentEss.PercentString() });
            }
            /*
            promptText.SetText($"Are you sure you want to PERMENANTLY DESTROY your level {iGlobal.level}\n" +
				$"{wePlayer.enchantingTableUI.itemSlotUI[0].Item.Name} {oreAndEssencePercent}\n" +
				(percentEss < 1f ? $"{oreString}\n" : "") +
				$"(Based on item value/experience.  Enchantments will be returned.)"
            );
            */
            object[] args = new object[] { iGlobal.level.ToString(), wePlayer.enchantingTableUI.itemSlotUI[0].Item.Name, oreAndEssencePercent, percentEss < 1f ? $"{oreString}\n" : "" };
            promptText.SetText(TableTextID.AreYouSure.ToString().Lang(L_ID1.TableText, args));
        }
    }
}