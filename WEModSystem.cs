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
using WeaponEnchantments.Items.Utility;
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
        public static SortedDictionary<ChestID, List<DropData>> chestDrops = new();
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
			MasterUIManager.PostUpdateEverything();
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
        public override void PostWorldGen() {
            for (int chestIndex = 0; chestIndex < 1000; chestIndex++) {
                Chest chest = Main.chest[chestIndex];
                if (chest == null)
                    continue;

                int itemsPlaced = 0;

                ChestID chestID = GetChestIDFromChest(chest);
                GetChestLoot(chestID, out List<DropData> options, out float chance);

                if (chance <= 0f)
                    continue;

                if (options == null)
                    continue;

                IEnumerable<DropData> weightedDropData = options.Where(d => d.Chance <= 0f);
				IEnumerable<DropData> chanceDropData = options.Where(d => d.Chance > 0f);
                foreach(DropData dropData in chanceDropData) {
					float randFloat = Main.rand.NextFloat();
                    float dropChance = dropData.Chance * ChestSpawnChance / 0.5f;
                    if (randFloat > dropChance)
                        continue;

					for (int j = 0; j < 40; j++) {
						if (chest.item[j].type != ItemID.None)
							continue;

                        int type = dropData.ID;
						for (int k = j; k >= 0; k--) {
							if (chest.item[k].type == type && chest.item[k].stack < chest.item[k].maxStack) {
								chest.item[k].stack++;
                                break;
							}
						}

						chest.item[j] = new Item(type);
                        break;
					}
				}

				for (int j = 0; j < 40 && itemsPlaced < chance; j++) {
                    if (chest.item[j].type != ItemID.None)
                        continue;

                    int type = weightedDropData.GetOneFromWeightedList(chance);

                    if (type > 0) {
                        bool found = false;
                        for (int k = j; k >= 0; k--) {
                            if (chest.item[k].type == type && chest.item[k].stack < chest.item[k].maxStack) {
                                chest.item[k].stack++;
                                found = true;
                                j--;
                                break;
                            }
                        }

                        if (!found)
                            chest.item[j] = new Item(type);
                    }

                    itemsPlaced++;
                }
            }
		}
		public static ChestID GetChestIDFromChest(Chest chest) {
            Tile tile = Main.tile[chest.x, chest.y];
            ushort tileType = tile.TileType;
            short tileFrameX = tile.TileFrameX;
            // If you look at the sprite for Chests by extracting Tiles_21.xnb, you'll see that the 12th chest is the Ice Chest.
            // Since we are counting from 0, this is where 11 comes from. 36 comes from the width of each tile including padding.
            switch (tileType) {
                case TileID.Containers:
                case TileID.FakeContainers:
                    return (ChestID)(tileFrameX / 36);
                case TileID.Containers2:
                case TileID.FakeContainers2:
                    return (ChestID)(tileFrameX / 36 + 100);
                default:
                    return ChestID.None;
            }
        }
        public static void GetChestLoot(ChestID chestID, out List<DropData> itemTypes, out float chance) {
            chance = 0f;
            itemTypes = chestDrops.ContainsKey(chestID) ? chestDrops[chestID] : null;
            if (itemTypes == null)
                return;

            chance = ChestSpawnChance;
            if (itemTypes.Where(d => d.Chance <= 0f).Count() == 1)
                chance *= itemTypes[0].Weight;

            switch (chestID) {
                case ChestID.Chest_Normal:
                    chance *= 0.7f;
                    //itemTypes.Add(ModContent.ItemType<DefenseEnchantmentBasic>());
                    //itemTypes.Add(ModContent.ItemType<DamageEnchantmentBasic>());
                    //itemTypes.Add(ModContent.ItemType<CriticalStrikeChanceEnchantmentBasic>());
                    //itemTypes.Add(ModContent.ItemType<ReducedManaUsageEnchantmentBasic>());
                    //itemTypes.Add(ModContent.ItemType<SizeEnchantmentBasic>());
                    //itemTypes.Add(ModContent.ItemType<AmmoCostEnchantmentBasic>());
                    //itemTypes.Add(ModContent.ItemType<AttackSpeedEnchantmentBasic>());
                    //itemTypes.Add(ModContent.ItemType<PeaceEnchantmentBasic>());
                    break;
                case ChestID.Gold:
                    //itemTypes.Add(ModContent.ItemType<CriticalStrikeChanceEnchantmentBasic>());
                    //itemTypes.Add(ModContent.ItemType<SpelunkerEnchantmentLegendary>());
                    //itemTypes.Add(ModContent.ItemType<DangerSenseEnchantmentLegendary>());
                    //itemTypes.Add(ModContent.ItemType<HunterEnchantmentLegendary>());
                    //itemTypes.Add(ModContent.ItemType<ObsidianSkinEnchantmentLegendary>());
                    //itemTypes.Add(ModContent.ItemType<AttackSpeedEnchantmentBasic>());
                    break;
                case ChestID.Gold_Locked:
                    //itemTypes.Add(ModContent.ItemType<AllForOneEnchantmentBasic>());
                    //itemTypes.Add(ModContent.ItemType<OneForAllEnchantmentBasic>());
                    break;
                case ChestID.Shadow:
                case ChestID.Shadow_Locked:
                    chance *= 2f;
                    //itemTypes.Add(ModContent.ItemType<ArmorPenetrationEnchantmentBasic>());
                    //itemTypes.Add(ModContent.ItemType<LifeStealEnchantmentBasic>());
                    //itemTypes.Add(ModContent.ItemType<WarEnchantmentBasic>());
                    break;
                case ChestID.RichMahogany:
                    //itemTypes.Add(ModContent.ItemType<CriticalStrikeChanceEnchantmentBasic>());
                    break;
                case ChestID.Ivy:
                    //itemTypes.Add(ModContent.ItemType<CriticalStrikeChanceEnchantmentBasic>());
                    break;
                case ChestID.Frozen:
                    //itemTypes.Add(ModContent.ItemType<ReducedManaUsageEnchantmentBasic>());
                    break;
                case ChestID.LivingWood:
                    //itemTypes.Add(ModContent.ItemType<SizeEnchantmentBasic>());
                    break;
                case ChestID.Skyware:
                    //itemTypes.Add(ModContent.ItemType<AttackSpeedEnchantmentBasic>());
                    break;
                case ChestID.WebCovered:
                    //itemTypes.Add(ModContent.ItemType<AmmoCostEnchantmentBasic>());
                    break;
                case ChestID.Lihzahrd:
                    chance *= 2f;
                    //itemTypes.Add(ModContent.ItemType<ArmorPenetrationEnchantmentBasic>());
                    //itemTypes.Add(ModContent.ItemType<LifeStealEnchantmentBasic>());
                    //itemTypes.Add(ModContent.ItemType<AllForOneEnchantmentBasic>());
                    //itemTypes.Add(ModContent.ItemType<OneForAllEnchantmentBasic>());
                    break;
                case ChestID.Water:
                    //itemTypes.Add(ModContent.ItemType<ReducedManaUsageEnchantmentBasic>());
                    break;
                case ChestID.Jungle_Dungeon:
                    chance = 1f;
                    //itemTypes.Add(ModContent.ItemType<Enchantment>());
                    break;
                case ChestID.Corruption_Dungeon:
                    chance = 1f;
                    //itemTypes.Add(ModContent.ItemType<Enchantment>());
                    break;
                case ChestID.Crimson_Dungeon:
                    chance = 1f;
                    //itemTypes.Add(ModContent.ItemType<Enchantment>());
                    break;
                case ChestID.Hallowed_Dungeon:
                    chance = 1f;
                    //itemTypes.Add(ModContent.ItemType<Enchantment>());
                    break;
                case ChestID.Ice_Dungeon:
                    chance = 1f;
                    //itemTypes.Add(ModContent.ItemType<Enchantment>());
                    break;
                case ChestID.Mushroom:
                    //itemTypes.Add(ModContent.ItemType<AmmoCostEnchantmentBasic>());
                    break;
                case ChestID.Granite:
                    //itemTypes.Add(ModContent.ItemType<AttackSpeedEnchantmentBasic>());
                    break;
                case ChestID.Marble:
                    //itemTypes.Add(ModContent.ItemType<AmmoCostEnchantmentBasic>());
                    break;
                case ChestID.Gold_DeadMans:
                    //itemTypes.Add(ModContent.ItemType<CriticalStrikeChanceEnchantmentBasic>());
                    //itemTypes.Add(ModContent.ItemType<SpelunkerEnchantmentLegendary>());
                    //itemTypes.Add(ModContent.ItemType<DangerSenseEnchantmentLegendary>());
                    //itemTypes.Add(ModContent.ItemType<HunterEnchantmentLegendary>());
                    //itemTypes.Add(ModContent.ItemType<ObsidianSkinEnchantmentLegendary>());
                    //itemTypes.Add(ModContent.ItemType<AttackSpeedEnchantmentBasic>());
                    break;
                case ChestID.SandStone:
                    //itemTypes.Add(ModContent.ItemType<AmmoCostEnchantmentBasic>());
                    break;
                case ChestID.Desert_Dungeon:
                    chance = 1f;
                    //itemTypes.Add(ModContent.ItemType<Enchantment>());
                    break;
            }
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