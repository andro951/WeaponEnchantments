using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.UI;
using Terraria;
using WeaponEnchantments.Items;
using WeaponEnchantments.Common.Utility;
using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using static System.Net.Mime.MediaTypeNames;
using Terraria.UI.Chat;
using Microsoft.CodeAnalysis;
using Terraria.ID;
using Terraria.Audio;
using WeaponEnchantments.Common;
using System.Reflection;
using WeaponEnchantments.Effects;
using WeaponEnchantments.Common.Configs;

namespace WeaponEnchantments.UI
{
	public static class EnchantmentStorage
	{
		public class EnchantmentStorageButtonID
		{
			public const int LootAll = 0;
			public const int DepositAll = 1;
			public const int QuickStack = 2;
			public const int Sort = 3;
			public const int ToggleVacuum = 4;
			public const int ToggleMarkTrash = 5;
			public const int UncraftAllTrash = 6;
			public const int Count = 7;
		}
		public static int ID => UI_ID.EnchantmentStorage;
		public static int enchantmentStorageUIDefaultX => Main.screenWidth / 2;
		public static int enchantmentStorageUIDefaultY => 300;
		private static int spacing => 4;
		private static int panelBorder => 10;
		public const float buttonScaleMinimum = 0.75f;
		public const float buttonScaleMaximum = 1f;
		public static float[] ButtonScale = Enumerable.Repeat(buttonScaleMinimum, EnchantmentStorageButtonID.Count).ToArray();
		private static bool markingTrash = false;
		private static int glowTime = 0;
		private static float glowHue = 0f;
		public static bool uncratingTrash = false;
		public static void PostDrawInterface(SpriteBatch spriteBatch) {
			//WEPlayer.LocalWEPlayer.displayEnchantmentStorage = true;
			WEPlayer wePlayer = WEPlayer.LocalWEPlayer;
			if (wePlayer.displayEnchantmentStorage || true) {
				if (glowTime > 0) {
					glowTime--;
					if (glowTime <= 0)
						glowHue = 0f;
				}

				Color mouseColor = Color.White * (1f - (255f - (float)(int)Main.mouseTextColor) / 255f * 0.5f);
				mouseColor.A = byte.MaxValue;
				//ItemSlots Data 1/2
				int itemSlotColumns = 10;
				int itemSlotRows = 4;//wePlayer.enchantmentStorageItems.Length / columns
				int itemSlotSpaceWidth = UIManager.ItemSlotSize + spacing;
				int itemSlotSpaceHeight = UIManager.ItemSlotSize + spacing;
				int itemSlotsWidth = (itemSlotColumns - 1) * itemSlotSpaceWidth + UIManager.ItemSlotSize;
				int itemSlotsHeight = (itemSlotRows - 1) * itemSlotSpaceHeight + UIManager.ItemSlotSize;
				int itemSlotsLeft = wePlayer.enchantmentStorageUILeft + panelBorder;

				//Name Data
				int nameLeft = itemSlotsLeft + itemSlotsWidth / 2;//itemSlotsLeft + (itemSlotsWidth - nameWidth) / 2;
				int nameTop = wePlayer.enchantmentStorageUITop + panelBorder;
				string name = EnchantmentStorageTextID.EnchantmentStorage.ToString().Lang(L_ID1.EnchantmentStorageText);
				name = "Enchantment Storage Name Test";
				UITextData nameData = new(UI_ID.None, nameLeft, nameTop, name, 1f, mouseColor, true);

				//Panel Data 1/2
				int panelBorderTop = nameData.Height + spacing + 2;

				//ItemSlots Data 2/2
				int itemSlotsTop = wePlayer.enchantmentStorageUITop + panelBorderTop;

				//Text buttons Data
				int buttonsLeft = itemSlotsLeft + itemSlotsWidth + spacing;
				int currentButtonTop = itemSlotsTop;
				UITextData[] textButtons = new UITextData[EnchantmentStorageButtonID.Count];
				int longestButtonNameWidth = 0;
				for (int buttonIndex = 0; buttonIndex < EnchantmentStorageButtonID.Count; buttonIndex++) {
					string text = ((EnchantmentStorageTextID)buttonIndex).ToString().Lang(L_ID1.EnchantmentStorageText);
					text = $"{(EnchantmentStorageTextID)buttonIndex} test";
					float scale = ButtonScale[buttonIndex];
					Color color;
					if (buttonIndex == EnchantmentStorageButtonID.ToggleVacuum && wePlayer.vacuumItemsIntoEnchantmentStorage) {
						color = new(162, 22, 255);
					}
					else if (buttonIndex == EnchantmentStorageButtonID.ToggleMarkTrash && markingTrash) {
						color = new(100, 100, 100);
					}
					else {
						color = mouseColor;
					}

					UITextData thisButton = new(UI_ID.EnchantmentStorageLootAll + buttonIndex, buttonsLeft, currentButtonTop, text, scale, color, ancorBotomLeft: true);
					textButtons[buttonIndex] = thisButton;
					longestButtonNameWidth = Math.Max(longestButtonNameWidth, thisButton.Width);
					currentButtonTop += thisButton.BaseHeight;
				}

				//Panel Data 2/2
				int panelBorderRight = spacing + longestButtonNameWidth + panelBorder;
				int panelWidth = itemSlotsWidth + panelBorder + panelBorderRight;
				int panelHeight = itemSlotsHeight + panelBorder + panelBorderTop;
				UIPanelData panel = new(ID, wePlayer.enchantmentStorageUILeft, wePlayer.enchantmentStorageUITop, panelWidth, panelHeight);

				//Panel Draw
				UIManager.DrawUIPanel(spriteBatch, panel, new Color(26, 2, 56, 100));

				//ItemSlots Draw
				int itemSlotY = itemSlotsTop;
				int itemSlotIndex = 0;
				int startRow = 0;
				int endRow = startRow + 4;
				for (int row = startRow; row < endRow; row++) {
					int itemSlotX = itemSlotsLeft;
					for (int column = 0; column < itemSlotColumns; column++) {
						ref Item item = ref wePlayer.enchantmentStorageItems[itemSlotIndex];
						if (UIManager.MouseHoveringItemSlot(itemSlotX, itemSlotY, UI_ID.EnchantmentStorageItemSlot1 + itemSlotIndex)) {
							if (Main.mouseItem.NullOrAir() || CanBeStored(Main.mouseItem)) {
								if (markingTrash && Main.mouseItem.NullOrAir()) {
									if (UIManager.LeftMouseClicked && item.ModItem is Enchantment) {
										if (wePlayer.trashEnchantments.Contains(item.type)) {
											wePlayer.trashEnchantments.Remove(item.type);
										}
										else {
											wePlayer.trashEnchantments.Add(item.type);
										}
									}
								}
								else {
									UIManager.ItemSlotClickInteractions(ref item);
								}
							}
						}

						if (wePlayer.trashEnchantments.Contains(item.type) && !item.favorited) {
							UIManager.DrawItemSlot(spriteBatch, ref item, itemSlotX, itemSlotY, 6, glowHue, glowTime);
						}
						else {
							UIManager.DrawItemSlot(spriteBatch, ref item, itemSlotX, itemSlotY, hue: glowHue, glowTime: glowTime);
						}

						itemSlotX += itemSlotSpaceWidth;
						itemSlotIndex++;
					}

					itemSlotY += itemSlotSpaceHeight;
				}

				//Name Draw
				nameData.Draw(spriteBatch);

				//Text Buttons Draw
				for (int buttonIndex = 0; buttonIndex < EnchantmentStorageButtonID.Count; buttonIndex++) {
					UITextData textButton = textButtons[buttonIndex];
					textButton.Draw(spriteBatch);
					if (UIManager.MouseHovering(textButton, true)) {
						ButtonScale[buttonIndex] += 0.05f;

						if (ButtonScale[buttonIndex] > buttonScaleMaximum)
							ButtonScale[buttonIndex] = buttonScaleMaximum;

						if (UIManager.LeftMouseClicked) {
							switch(buttonIndex) {
								case EnchantmentStorageButtonID.LootAll:
									LootAll();
									break;
								case EnchantmentStorageButtonID.DepositAll:
									DepositAll(Main.LocalPlayer.inventory);
									break;
								case EnchantmentStorageButtonID.QuickStack:
									QuickStack(Main.LocalPlayer.inventory);
									break;
								case EnchantmentStorageButtonID.Sort:
									Sort();
									break;
								case EnchantmentStorageButtonID.ToggleVacuum:
									ToggleVacuum();
									break;
								case EnchantmentStorageButtonID.ToggleMarkTrash:
									ToggleMarkTrash();
									break;
								case EnchantmentStorageButtonID.UncraftAllTrash:
									UncraftAllTrash();
									break;
							}
						}
					}
					else {
						ButtonScale[buttonIndex] -= 0.05f;

						if (ButtonScale[buttonIndex] < buttonScaleMinimum)
							ButtonScale[buttonIndex] = buttonScaleMinimum;
					}
				}

				//Panel Hover and Drag
				if (UIManager.MouseHovering(panel)) {
					UIManager.TryStartDraggingUI(panel);
				}

				if (UIManager.ShouldDragUI(panel))
					UIManager.DragUI(out wePlayer.enchantmentStorageUILeft, out wePlayer.enchantmentStorageUITop);
			}
		}
		public static bool CanBeStored(Item item) {
			if (item?.ModItem is WEModItem weModItem)
				return weModItem.CanBeStoredInEnchantmentStroage;

			return false;
		}
		private static void LootAll() {
			for (int i = 0; i < WEPlayer.LocalWEPlayer.enchantmentStorageItems.Length; i++) {
				if (WEPlayer.LocalWEPlayer.enchantmentStorageItems[i].type > ItemID.None) {
					WEPlayer.LocalWEPlayer.enchantmentStorageItems[i] = Main.LocalPlayer.GetItem(Main.myPlayer, WEPlayer.LocalWEPlayer.enchantmentStorageItems[i], GetItemSettings.LootAllSettings);
				}
			}
		}
		public static bool DepositAll(ref Item item) => DepositAll(new Item[] { item });
		public static bool DepositAll(Item[] inv) {
			bool transferedAnyItem = QuickStack(inv, false);
			int storageIndex = 0;
			for (int i = 0; i < inv.Length; i++) {
				ref Item item = ref inv[i];
				if (!item.favorited && CanBeStored(item)) {
					while (storageIndex < WEPlayer.LocalWEPlayer.enchantmentStorageItems.Length && WEPlayer.LocalWEPlayer.enchantmentStorageItems[storageIndex].type > ItemID.None) {
						storageIndex++;
					}

					if (storageIndex < WEPlayer.LocalWEPlayer.enchantmentStorageItems.Length) {
						WEPlayer.LocalWEPlayer.enchantmentStorageItems[storageIndex] = item.Clone();
						item.TurnToAir();
						transferedAnyItem = true;
					}
					else {
						break;
					}
				}
			}

			if (transferedAnyItem)
				SoundEngine.PlaySound(SoundID.Grab);

			return transferedAnyItem;
		}
		public static bool QuickStack(ref Item item) => QuickStack(new Item[] { item });
		public static bool QuickStack(Item[] inv, bool playSound = true) {
			bool transferedAnyItem = false;
			SortedDictionary<int, List<int>> nonAirItemsInStorage = new();
			for (int i = 0; i < WEPlayer.LocalWEPlayer.enchantmentStorageItems.Length; i++) {
				int type = WEPlayer.LocalWEPlayer.enchantmentStorageItems[i].type;
				if (type > ItemID.None)
					nonAirItemsInStorage.AddOrCombine(type, i);
			}

			for (int i = 0; i < inv.Length; i++) {
				ref Item item = ref inv[i];
				if (!item.favorited && CanBeStored(item) && nonAirItemsInStorage.TryGetValue(item.type, out List<int> storageIndexes)) {
					foreach (int storageIndex in storageIndexes) {
						ref Item storageItem = ref WEPlayer.LocalWEPlayer.enchantmentStorageItems[storageIndex];
						if (storageItem.stack < item.maxStack) {
							if (ItemLoader.TryStackItems(storageItem, item, out int transfered)) {
								transferedAnyItem = true;
								if (item.stack < 1) {
									item.TurnToAir();
									break;
								}
							}
						}
					}
				}
			}

			if (playSound && transferedAnyItem)
				SoundEngine.PlaySound(SoundID.Grab);

			return transferedAnyItem;
		}
		private static void Sort() {
			MethodInfo sort = typeof(ItemSorting).GetMethod("Sort", BindingFlags.NonPublic | BindingFlags.Static);
			sort.Invoke(null, new object[] { WEPlayer.LocalWEPlayer.enchantmentStorageItems, new int[0] });
			Type itemSlotType = typeof(ItemSlot);
			int[] inventoryGlowTime = (int[])itemSlotType.GetField("inventoryGlowTime", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
			for (int i = 0; i < inventoryGlowTime.Length; i++) {
				inventoryGlowTime[i] = 0;
			}

			if (Main.LocalPlayer.chest != -1) {
				int[] inventoryGlowTimeChest = (int[])itemSlotType.GetField("inventoryGlowTimeChest", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
				for (int i = 0; i < inventoryGlowTimeChest.Length; i++) {
					inventoryGlowTimeChest[i] = 0;
				}
			}

			IEnumerable<Item> containments = WEPlayer.LocalWEPlayer.enchantmentStorageItems.Where(i => i.ModItem is ContainmentItem).OrderBy(i => i.type);
			IEnumerable<Item> powerBoosters = WEPlayer.LocalWEPlayer.enchantmentStorageItems.Where(i => i.ModItem is PowerBooster or UltraPowerBooster).OrderBy(i => i.type);
			IEnumerable<Item> enchantments = WEPlayer.LocalWEPlayer.enchantmentStorageItems.Where(i => i.ModItem is Enchantment)
				.GroupBy(i => ((Enchantment)i.ModItem).EnchantmentTypeName).Select(l => l.ToList().OrderBy(i => ((Enchantment)i.ModItem).EnchantmentTier)).SelectMany(l => l);
			IEnumerable<Item> otherItems = WEPlayer.LocalWEPlayer.enchantmentStorageItems.Where(i => i.ModItem == null || i.ModItem is not (Enchantment or ContainmentItem or PowerBooster or UltraPowerBooster));
			WEPlayer.LocalWEPlayer.enchantmentStorageItems = containments.Concat(powerBoosters).Concat(enchantments).Concat(otherItems).ToArray();
			glowTime = 300;
			glowHue = 0.5f;
		}
		private static void ToggleVacuum() {
			WEPlayer.LocalWEPlayer.vacuumItemsIntoEnchantmentStorage = !WEPlayer.LocalWEPlayer.vacuumItemsIntoEnchantmentStorage;
		}
		private static void ToggleMarkTrash() {
			markingTrash = !markingTrash;
		}
		private static void UncraftAllTrash() {
			uncratingTrash = true;
			Dictionary<int, HashSet<int>> storageIndexes = new();
			for (int i = 0; i < WEPlayer.LocalWEPlayer.enchantmentStorageItems.Length; i++) {
				ref Item item = ref WEPlayer.LocalWEPlayer.enchantmentStorageItems[i];
				if (WEPlayer.LocalWEPlayer.trashEnchantments.Contains(item.type))
					storageIndexes.AddOrCombine(item.type, i);
			}

			int enchantmentEssence = ModContent.ItemType<EnchantmentEssenceBasic>();
			Dictionary<int, int> recipeNumbers = new();
			for (int i = 0; i < Main.recipe.Length; i++) {
				Recipe r = Main.recipe[i];
				if (r.createItem.type == enchantmentEssence && r.requiredItem.Count > 0 && r.requiredItem.First().type is int type && storageIndexes.ContainsKey(type) && !recipeNumbers.ContainsKey(type))
					recipeNumbers.Add(type, i);
			}

			bool anyUncrafted = false;
			foreach (KeyValuePair<int, HashSet<int>> itemType in storageIndexes) {
				int type = itemType.Key;
				if (recipeNumbers.TryGetValue(type, out int recipeNum)) {
					Recipe r = Main.recipe[recipeNum];
					foreach (int slot in itemType.Value) {
						int stack = WEPlayer.LocalWEPlayer.enchantmentStorageItems[slot].stack;
						for (int i = 0; i < stack; i++) {
							Item crafted = r.createItem.Clone();
							crafted.Prefix(-1);
							r.Create();
							RecipeLoader.OnCraft(crafted, r, new Item());
							Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("Crafting"), crafted, crafted.stack);
							PopupText.NewText(PopupTextContext.ItemCraft, crafted, crafted.stack);
							anyUncrafted = true;
						}
					}
				}
			}

			if (anyUncrafted)
				SoundEngine.PlaySound(SoundID.Grab);

			uncratingTrash = false;
			Recipe.FindRecipes();
		}
		public static bool ItemSpace(Item item) {
			WEPlayer wePlayer = WEPlayer.LocalWEPlayer;
			for (int i = 0; i < wePlayer.enchantmentStorageItems.Length; i++) {
				Item storageItem = wePlayer.enchantmentStorageItems[i];
				if (item.IsAir || item.type == storageItem.type && storageItem.stack < item.maxStack)
					return true;
			}

			return false;
		}
	}
}
