using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using Terraria.UI.Chat;
using Terraria.UI.Gamepad;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Content.NPCs;
using WeaponEnchantments.Items;
using WeaponEnchantments.Items.Enchantments;
using WeaponEnchantments.Items.Enchantments.Unique;
using WeaponEnchantments.Items.Enchantments.Utility;
using WeaponEnchantments.Tiles;
using WeaponEnchantments.UI;
using static WeaponEnchantments.Common.Configs.ConfigValues;
using androLib.Common.Utility;

namespace WeaponEnchantments
{
	using androLib.Common.Globals;
	using androLib.UI;
	using VacuumOreBag.Items;

	public class WEModSystem : ModSystem {
        public static bool FavoriteKeyDown => Main.keyState.IsKeyDown(Main.FavoriteKey);
        public static bool ShiftDown => ItemSlot.ShiftInUse;
        internal static byte versionUpdate = 0;
        public static int[] levelXps = new int[EnchantedItem.MAX_Level];
        public static int stolenItemToBeCleared = -1;
        public static List<string> updatedPlayerNames;
        private bool dayTime = Main.dayTime;

		public override void OnModLoad() {
            double previous = 0;
            double current;
            int l;
            for (l = 0; l < EnchantedItem.MAX_Level; l++) {
                current = previous * 1.23356622200537 + (l + 1) * 1000;
                previous = current;
                levelXps[l] = (int)current;
            }

            WEMod.playerSwapperModEnabled = ModLoader.HasMod("PlayerSwapper");
            if (WEMod.playerSwapperModEnabled)
                updatedPlayerNames = new List<string>();
        }
		public override void PostAddRecipes() {
			SetupInfusion();
		}
		public static void SetupInfusion() {
			InfusionManager.SetUpVanillaWeaponInfusionPowers();
			InfusionProgression.PostSetupRecipes();
			InfusionManager.LogAllInfusionPowers();
		}
		public override void PostUpdateEverything() {
			
		}
		public override void PostDrawInterface(SpriteBatch spriteBatch) {
			WEPlayer wePlayer = WEPlayer.LocalWEPlayer;

			//Calamity Reforge
			if (EnchantedItem.calamityReforged) {
                if (Main.reforgeItem.TryGetEnchantedItem()) {
                    //Calamity only
                    EnchantedItem.ReforgeItem(ref Main.reforgeItem, wePlayer.Player, true);
                }
                else {
                    //Calamity and AutoReforge
                    EnchantedItem.ReforgeItem(ref EnchantedItem.calamityAndAutoReforgePostReforgeItem, wePlayer.Player, true);
                }
            }

            //Fargos pirates that steal items
            if (stolenItemToBeCleared != -1 && Main.netMode != NetmodeID.MultiplayerClient) {
                Item itemToClear = Main.item[stolenItemToBeCleared];
                if (itemToClear != null && itemToClear.TryGetEnchantedItemSearchAll(out EnchantedItem iGlobal)) {
                    iGlobal.prefix = -1;
                }

                stolenItemToBeCleared = -1;
            }

            //Player swapper
            if (WEMod.playerSwapperModEnabled && Main.netMode != NetmodeID.Server) {
                string playerName = wePlayer.Player.name;
                if (!updatedPlayerNames.Contains(playerName)) {
                    OldItemManager.ReplaceAllPlayerOldItems(wePlayer.Player);
                    updatedPlayerNames.Add(playerName);
                }
            }
		}
        public static void QuickStackEssence() {
            bool transfered = false;
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            for (int j = 0; j < 50; j++) {
                if (wePlayer.Player.inventory[j].TryGetEnchantmentEssence(out EnchantmentEssence essence)) {
                    int tier = essence.EssenceTier;
                    int ammountToTransfer;
                    int startingStack = wePlayer.Player.inventory[j].stack;
                    if (wePlayer.enchantingTableEssence[tier].IsAir) {
                        wePlayer.enchantingTableEssence[tier] = wePlayer.Player.inventory[j].Clone();
                        wePlayer.Player.inventory[j] = new Item();
                        transfered = true;
                    }
                    else {
                        int maxStack = wePlayer.enchantingTableEssence[tier].maxStack;
                        if (wePlayer.enchantingTableEssence[tier].stack < maxStack) {
                            if (wePlayer.Player.inventory[j].stack + wePlayer.enchantingTableEssence[tier].stack > maxStack) {
                                ammountToTransfer = maxStack - wePlayer.enchantingTableEssence[tier].stack;
                            }
                            else {
                                ammountToTransfer = wePlayer.Player.inventory[j].stack;
                            }

                            wePlayer.enchantingTableEssence[tier].stack += ammountToTransfer;
                            wePlayer.Player.inventory[j].stack -= ammountToTransfer;
                            transfered = true;
                        }
                    }

                    if (wePlayer.Player.inventory[j].stack == startingStack)
                        transfered = false;
                }
            }
            if (transfered)
                SoundEngine.PlaySound(SoundID.Grab);
        }
        public static bool AutoCraftEssence() {
            bool crafted = false;
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            for (int i = EnchantingTableUI.MaxEssenceSlots - 1; i > 0; i--) {
                if (wePlayer.enchantingTableEssence[i].NullOrAir())
                    continue;

                int maxStack = wePlayer.enchantingTableEssence[i].maxStack;
                if (wePlayer.enchantingTableEssence[i].stack < maxStack) {
                    int ammountToTransfer;
                    if (wePlayer.enchantingTableEssence[i].stack == 0 || (maxStack > wePlayer.enchantingTableEssence[i].stack + (wePlayer.enchantingTableEssence[i - 1].stack / 4))) {
                        ammountToTransfer = wePlayer.enchantingTableEssence[i - 1].stack / 4;
                    }
                    else {
                        ammountToTransfer = maxStack - wePlayer.enchantingTableEssence[i].stack;
                    }

                    if (ammountToTransfer > 0) {
                        wePlayer.enchantingTableEssence[i].stack += ammountToTransfer;
                        wePlayer.enchantingTableEssence[i - 1].stack -= ammountToTransfer * 4;
                        crafted = true;
                    }
                }
            }

            for (int i = 1; i < EnchantingTableUI.MaxEssenceSlots; i++) {
                if (wePlayer.enchantingTableEssence[i].NullOrAir())
                    continue;

                int maxStack = wePlayer.enchantingTableEssence[i].maxStack;
                if (wePlayer.enchantingTableEssence[i].stack < maxStack) {
                    int ammountToTransfer;
                    if (wePlayer.enchantingTableEssence[i].stack == 0 || (maxStack > wePlayer.enchantingTableEssence[i].stack + (wePlayer.enchantingTableEssence[i - 1].stack / 4))) {
                        ammountToTransfer = wePlayer.enchantingTableEssence[i - 1].stack / 4;
                    }
                    else {
                        ammountToTransfer = maxStack - wePlayer.enchantingTableEssence[i].stack;
                    }

                    if (ammountToTransfer > 0) {
                        wePlayer.enchantingTableEssence[i].stack += ammountToTransfer;
                        wePlayer.enchantingTableEssence[i - 1].stack -= ammountToTransfer * 4;
                        crafted = true;
                    }
                }
            }

            return crafted;
        }
        public override void PreSaveAndQuit() {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            if (wePlayer.usingEnchantingTable)
                EnchantingTableUI.CloseEnchantingTableUI();
        }
        public override void LoadWorldData(TagCompound tag) {
            versionUpdate = tag.Get<byte>("versionUpdate");
            OldItemManager.versionUpdate = versionUpdate;
        }
        public override void SaveWorldData(TagCompound tag) {
            tag["versionUpdate"] = versionUpdate;
        }
        public override void PostUpdateTime() {
            if (Main.dayTime && !dayTime) {
                Witch.resetShop = true;

                //If player has a fishing pole in inventory with NpcContactAnglerEnchantment, tell them the new fishing quest.
                foreach (Item item in Main.LocalPlayer.inventory.Where(i => i.fishingPole > 0)) {
                    if (item.TryGetEnchantedItem(out EnchantedFishingPole enchantedFishingPole)) {
                        foreach(Enchantment enchantment in enchantedFishingPole.enchantments.All.Select(e => e.ModItem).OfType<Enchantment>()) {
                            if (enchantment is NpcContactAnglerEnchantment anglerEnchantment) {
                                int newQuestFish = Main.anglerQuestItemNetIDs[Main.anglerQuest];
                                Main.NewText($"{GameMessageTextID.DailyFishingQuestReset.ToString().Lang_WE(L_ID1.GameMessages, new object[] { ContentSamples.ItemsByType[newQuestFish].Name })}\n" +//$"The daily fishing quest has reset.  Your next quest is {ContentSamples.ItemsByType[newQuestFish].Name}.\n" +
                                    $"{Lang.AnglerQuestChat(false)}");
                            }
						}
                    }
                }
            }

            dayTime = Main.dayTime;
        }
		public override void ModifyTimeRate(ref double timeRate, ref double tileUpdateRate, ref double eventUpdateRate) {
            /*
            if (lastTimeCheck + 60 <= Main.time) {
                return;
            }
            else {
                lastTimeCheck = Main.time;
				
			}
            */
			if (Main.LocalPlayer.TryGetWEPlayer(out WEPlayer wePlayer))
				wePlayer.ModifyTimeRate(ref timeRate, ref tileUpdateRate, ref eventUpdateRate);
		}
	}
}