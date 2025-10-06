using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using System.Runtime.CompilerServices;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Common.Utility;
using Terraria.ID;
using androLib.Common.Utility;
using androLib.Common.Globals;
using androLib;

namespace WeaponEnchantments.ModIntegration
{
	[JITWhenModsEnabled(CALAMITY_NAME)]
	internal class CalamityIntegration : ModSystem
	{
		public const string CALAMITY_NAME = "CalamityMod";
		private List<Item> mouseItemClones = new List<Item>();
		private Item lastMouseItem = null;
		private double closeInventoryTimerEnd = 0;
		private bool skipOnce = false;

		public override void Load() {
			if (AndroMod.calamityEnabled) {
				AndroMod.calamityMod.TryFind("RogueDamageClass", out CalamityValues.rogue);
				AndroMod.calamityMod.TryFind("TrueMeleeDamageClass", out CalamityValues.trueMelee);
				AndroMod.calamityMod.TryFind("TrueMeleeNoSpeedDamageClass", out CalamityValues.trueMeleeNoSpeed);
			}

			AndroMod.OnResetGameCounter += () => closeInventoryTimerEnd = 0;
		}
		public override void PostDrawInterface(SpriteBatch spriteBatch) {
			if (AndroMod.calamityEnabled) {
				HandleOnTickEvents();
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void HandleOnTickEvents() {
			//Need to add a way to handle item.value (bool pauseAddingItemValue in EnchantedItem?)
			if (Main.LocalPlayer.talkNPC < 0)
				return;

			//Check if talking to NPC
			string npcTalking = Main.LocalPlayer.talkNPC != -1 ? Main.npc[Main.LocalPlayer.talkNPC].ModFullName() : "";
			if (npcTalking != "CalamityMod/WITCH") {
				//Clear cloned item list after timer expires
				if (mouseItemClones.Count > 0) {
					if (closeInventoryTimerEnd == 0) {
						skipOnce = true;

						//Start timer 10 seconds
						closeInventoryTimerEnd = Main.GameUpdateCount + 600;
					}
					else if (Main.GameUpdateCount > closeInventoryTimerEnd) {
						SearchForItem();

						//Clear list
						mouseItemClones.Clear();

						closeInventoryTimerEnd = 0;
					}
					else if (skipOnce) {
						skipOnce = false;

						SearchForItem();
					}
				}
				
				return;
			}

			//Reset timer if speaking to the which
			closeInventoryTimerEnd = 0;

			//Left click
			if (Main.mouseLeft && Main.mouseLeftRelease) {
				if (Main.mouseItem.IsAir) {
					//Put item down
					if (lastMouseItem.TryGetEnchantedItem()) {
						//Check if last mouse item is already in the list
						bool addToList = true;
						foreach (Item clone in mouseItemClones) {
							if (clone.IsSameEnchantedItem(lastMouseItem)) {
								addToList = false;
								break;
							}
						}

						//Add last mouse item to the list
						if (addToList)
							mouseItemClones.Add(lastMouseItem);
					}
				}
				else if (mouseItemClones.Count > 0){
					//Picked item up
					Item mouseItem = Main.mouseItem;
					for(int i = 0; i < mouseItemClones.Count; i++) {
						Item clone = mouseItemClones[i];
						if (mouseItem.IsSameEnchantedItem(clone) && mouseItem.HoverName != clone.HoverName) {
							//Force recalculate UpdateItemStats()
							if(mouseItem.TryGetEnchantedItemSearchAll(out EnchantedItem menchantedItem))
								menchantedItem.prefix = -1;

							//Remove from list
							mouseItemClones.RemoveAt(i);
						}
					}
				}
			}

			lastMouseItem = Main.mouseItem.Clone();
		}
		private void SearchForItem() {
			bool foundItem = false;

			//Look for item in your inventory
			for (int i = 0; i < mouseItemClones.Count; i++) {
				for (int j = 0; j < Main.LocalPlayer.inventory.Length; j++) {
					Item inventoryItem = Main.LocalPlayer.inventory[j];
					Item clone = mouseItemClones[i];
					foundItem = CheckItem(inventoryItem, clone);
					if (foundItem)
						break;
				}

				if (foundItem)
					break;
			}

			//Search Spawned Items
			if (!foundItem) {
				for (int i = 0; i < mouseItemClones.Count; i++) {
					for (int j = 0; j < Main.item.Length; j++) {
						Item worldItem = Main.item[j];
						Item clone = mouseItemClones[i];
						foundItem = CheckItem(worldItem, clone);
						if (foundItem)
							break;
					}

					if (foundItem)
						break;
				}
			}

			if (foundItem)
				mouseItemClones.Clear();
		}
		private bool CheckItem(Item item, Item clone) {
			if (item.IsSameEnchantedItem(clone) && item.HoverName != clone.HoverName) {
				//Force recalculate UpdateItemStats()
				if(item.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem)) {
					enchantedItem.prefix = -1;
					return true;
				}
			}

			return false;
		}
	}
}
