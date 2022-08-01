using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Items;
using static WeaponEnchantments.UI.WeaponEnchantmentUI;
using Terraria.ID;
using Terraria.GameContent;
using Terraria.UI.Chat;
using Terraria.UI.Gamepad;
using static Terraria.UI.ItemSlot;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;

namespace WeaponEnchantments.UI
{
    public class WEUIItemSlot : UIElement
    {
		internal Item Item;
		private readonly int _itemContext;
		private readonly int _context;
		private readonly float _scale;
		private readonly bool _utilitySlot;
		public readonly int _slotTier;
		public bool contains { get; private set; }

		internal event Action<int> OnMouseover;

		private int timer = 0;
		internal WEUIItemSlot(int context, int itemContext, int slotTier = 4, bool utilitySlot = false, float scale = 0.86f)
		{
			_context = context;
			_itemContext = itemContext;//0 = itemSlot, 1 = enchantmentSlot, 2 = essenceSlot
			_slotTier = slotTier;//Associated enchantment table tier required for enchantmentSlot to unlock or the rarity of each essenceSlot
			_utilitySlot = utilitySlot;//Only true for the last enchantmentSlot, it can only have utility enchantments
			_scale = scale;
			Item = new Item();
			Item.SetDefaults();
			Width.Set(49 * scale, 0f);
			Height.Set(49 * scale, 0f);
		}//Constructor
		public bool Valid(Item item)
		{
			WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
			if (item.IsAir)
			{
				return true;
			}//Hand is empty
			else
			{
				switch (_itemContext)
				{
					case ItemSlotContext.Item:
                        if (!wePlayer.enchantingTableUI.itemSlotUI[0].Item.IsAir)
                        {
							if (item.type == PowerBooster.ID && !wePlayer.enchantingTableUI.itemSlotUI[0].Item.GetEnchantedItem().PowerBoosterInstalled)
								return true;
						}
						return WEMod.IsEnchantable(item);
					case ItemSlotContext.Enchantment:
						if (!wePlayer.enchantingTableUI.itemSlotUI[0].Item.IsAir)
						{
							if (UseEnchantmentSlot())
							{
								if (WEMod.IsEnchantmentItem(item, _utilitySlot))
								{
									bool continueCheck = true;
									int damageClassSpecific = 0;
									Enchantment newEnchantment = ((Enchantment)item.ModItem);
									if (wePlayer.enchantingTableUI.itemSlotUI[0].Item.DamageType != null)//Make this look at content sample damage type ContentSamples.ItemsByType[item.type].DamageType.Type
									{
										damageClassSpecific = Enchantment.GetDamageClass(ContentSamples.ItemsByType[wePlayer.enchantingTableUI.itemSlotUI[0].Item.type].DamageType.Type);
									}
									if(newEnchantment.DamageClassSpecific != 0 && damageClassSpecific != newEnchantment.DamageClassSpecific)
										continueCheck = false;
									if (newEnchantment.RestrictedClass != -1 && damageClassSpecific == newEnchantment.RestrictedClass)
										continueCheck = false;
									if(!CheckAllowedList(newEnchantment))
										continueCheck = false;
									if(newEnchantment.ArmorSlotSpecific > -1)
									{
										int slot = -1;
										switch (newEnchantment.ArmorSlotSpecific)
										{
											case (int)ArmorSlotSpecificID.Head:
												slot = wePlayer.enchantingTableUI.itemSlotUI[0].Item.headSlot;
												break;
											case (int)ArmorSlotSpecificID.Body:
												slot = wePlayer.enchantingTableUI.itemSlotUI[0].Item.bodySlot;
												break;
											case (int)ArmorSlotSpecificID.Legs:
												slot = wePlayer.enchantingTableUI.itemSlotUI[0].Item.legSlot;
												break;
										}
										if (slot == -1)
											continueCheck = false;
									}
									int currentEnchantmentLevelCost = 0;
                                    if (!Item.IsAir)
										currentEnchantmentLevelCost = ((Enchantment)Item.ModItem).GetCapacityCost();

									return continueCheck ? wePlayer.enchantingTableUI.itemSlotUI[0].Item.GetEnchantedItem().GetLevelsAvailable() >= newEnchantment.GetCapacityCost() - currentEnchantmentLevelCost : false;
								}
                                else
                                {
									return false;
                                }
							}
                            else
                            {
								return false;
                            }
						}//check enchantment is valid
                        else
                        {
							return false;
                        }
					case ItemSlotContext.Essence:
                        if (WEMod.IsEssenceItem(item))
                        {
							return ((EnchantmentEssence)item.ModItem).essenceRarity == _slotTier;
						}//check essence is valid
                        else
                        {
							return false;
                        }
					default:
						return false;
				}
			}
		}//Check if Item going into a slot is valid for that slot
		public static bool CheckAllowedList(Enchantment enchantment)
        {
			WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
			return enchantment.AllowedList.ContainsKey("Weapon") && WEMod.IsWeaponItem(wePlayer.ItemInUI())
				|| enchantment.AllowedList.ContainsKey("Armor") && WEMod.IsArmorItem(wePlayer.ItemInUI())
				|| enchantment.AllowedList.ContainsKey("Accessory") && WEMod.IsAccessoryItem(wePlayer.ItemInUI());
		}
		private bool UseEnchantmentSlot()
        {
			WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
			if (_slotTier <= wePlayer.enchantingTableTier || _utilitySlot)
            {
				if (wePlayer.enchantingTableUI?.itemSlotUI[0]?.Item != null)
                {
					Item item = wePlayer.enchantingTableUI?.itemSlotUI[0]?.Item;
					if (!item.IsAir)
                    {
						switch (_slotTier)
						{
							case 0:
								return true;
							case 1:
							case 4:
								if(WEMod.IsWeaponItem(item) || WEMod.IsArmorItem(item))
									return true;
								break;
							default:
								if(WEMod.IsWeaponItem(item))
									return true;
								break;
						}
						return false;
					}
				}
				return true;
			}
			return false;
		}
		internal void HandleMouseItem()
		{
			WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
			if (Valid(Main.mouseItem))
			{
				if (Main.mouseItem.type == PowerBooster.ID)
				{
					if (_itemContext == ItemSlotContext.Item && !wePlayer.enchantingTableUI.itemSlotUI[0].Item.IsAir && !wePlayer.enchantingTableUI.itemSlotUI[0].Item.GetEnchantedItem().PowerBoosterInstalled && Main.mouseLeft && Main.mouseLeftRelease)
					{
						if (Main.mouseItem.stack > 1)
						{
							Main.mouseItem.stack--;
						}
						else
						{
							Main.mouseItem = new Item();
						}
						SoundEngine.PlaySound(SoundID.Grab);
						wePlayer.enchantingTableUI.itemSlotUI[0].Item.GetEnchantedItem().PowerBoosterInstalled = true;
					}
				}
				else if (Main.mouseItem.ModItem is Enchantment enchantment)
				{
                    if (CheckUniqueSlot(enchantment, FindSwapEnchantmentSlot(enchantment, wePlayer.enchantingTableUI.itemSlotUI[0].Item)))
                    {
						if (Main.mouseItem.type != Item.type)
                        {
							if (Main.mouseItem.stack > 1)
							{
								if (Main.mouseLeft && Main.mouseLeftRelease)
								{
									Item = wePlayer.Player.GetItem(Main.myPlayer, Item, GetItemSettings.LootAllSettings);
									if (Item.IsAir)
									{
										Main.mouseItem.stack--;
										Item = Main.mouseItem.Clone();
										Item.stack = 1;
										SoundEngine.PlaySound(SoundID.Grab);
									}
								}
							}
							else
							{
								ItemSlot.Handle(ref Item, ItemSlot.Context.BankItem);//Handles all the click and hover actions based on the context
							}
						}
					}
				}
				else
				{
					ItemSlot.Handle(ref Item, ItemSlot.Context.BankItem);//Handles all the click and hover actions based on the context
				}
			}
		}
		public bool CheckUniqueSlot(Enchantment enchantment, int swapEnchantmentSlot)
        {
			return (!enchantment.Unique && !enchantment.Max1) || swapEnchantmentSlot == -1 || swapEnchantmentSlot == _slotTier;
		}
		public static int FindSwapEnchantmentSlot(Enchantment enchantement, Item item)
        {
			WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
			for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
			{
				if(item.TryGetGlobalItem(out EnchantedItem iGlobal))
                {
					if (!iGlobal.enchantments[i].IsAir)
					{
						Enchantment appliedEnchantment = (Enchantment)iGlobal.enchantments[i].ModItem;
						if (appliedEnchantment != null && (
							(enchantement.Unique) && (appliedEnchantment.Unique) || enchantement.Max1 && enchantement.EnchantmentTypeName == appliedEnchantment.EnchantmentTypeName))
						{
							return i;
						}
						/*if (appliedEnchantment != null && ((enchantement.Unique || enchantement.DamageClassSpecific > 0) && (appliedEnchantment.DamageClassSpecific > 0 || appliedEnchantment.Unique) || enchantement.Max1 && enchantement.EnchantmentType == appliedEnchantment.EnchantmentType))
						{
							return i;
						}*/
					}
				}
			}
			return -1;
			/*bool notFound = true;
			for (int i = 0; i < EnchantingTable.maxEnchantments && notFound; i++)
			{
				Enchantments appliedEnchantment = ((Enchantments)wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item.ModItem);
				if(appliedEnchantment != null && ((enchantement.Unique || enchantement.damageClassSpecific > 0) && (appliedEnchantment.damageClassSpecific > 0 || appliedEnchantment.Unique) || enchantement.Max1 && enchantement.EnchantmentType == appliedEnchantment.EnchantmentType))
                {
					return i;
                }
			}
			return -1;*/
		}
		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
			float oldScale = Main.inventoryScale;
			Main.inventoryScale = _scale;
			Rectangle rectangle = GetDimensions().ToRectangle();

			contains = ContainsPoint(Main.MouseScreen);

			if (contains && !PlayerInput.IgnoreMouseInterface)
			{
				wePlayer.Player.mouseInterface = true;
				if (_itemContext == 2 && Main.keyState.IsKeyDown(Main.FavoriteKey) && Main.mouseItem.IsAir)
				{
					Main.cursorOverride = 3;
					if(Main.mouseLeft && Main.mouseLeftRelease)
					{
						Item.favorited = !Item.favorited;
						SoundEngine.PlaySound(SoundID.MenuTick);
					}
				}
				else if (Item.favorited && Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
				{
					Main.cursorOverride = 9;
					if (!Item.IsAir && Main.mouseLeft && Main.mouseLeftRelease)
					{
						Item = wePlayer.Player.GetItem(wePlayer.Player.whoAmI, Item, GetItemSettings.LootAllSettings);
					}
				}
				else if(_itemContext == 0 && Item.maxStack > 1 && Main.mouseRight)
				{
					if (Main.mouseRightRelease)
					{
						if (Item.stack > 1)
						{
							if (Main.mouseItem.IsAir)
							{
								Main.mouseItem = new Item(Item.type);
								Item.stack--;
							}
							else if (Main.mouseItem.type == Item.type)
							{
								Main.mouseItem.stack++;
								Item.stack--;
							}
						}
						else
						{
							if (Main.mouseItem.IsAir)
							{
								Main.mouseItem = Item.Clone();
							}
							else if (Main.mouseItem.GetEnchantedItem().CanStack(Main.mouseItem, Item))
							{
								Main.mouseItem.stack++;
							}
							Item = new Item();
						}
						SoundEngine.PlaySound(SoundID.MenuTick);
					}
				}
				else
					HandleMouseItem();
			}
			Draw(spriteBatch, Item, _context, _slotTier, rectangle.TopLeft(), _slotTier);
			if (contains)
			{
				timer++;
				OnMouseover?.Invoke(timer);
			}
			else if (!contains)
			{
				timer = 0;
			}
			Main.inventoryScale = oldScale;
		}//PR
		public void Draw(SpriteBatch spriteBatch, Item item, int context, int slot, Vector2 position, int slotTier)
		{
			WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
			Player player = Main.player[Main.myPlayer];
			float inventoryScale = Main.inventoryScale;
			Color color = Color.White;
			bool flag = false;
			//int num = 0;
			Item[] inv = new Item[] { item };
			int gamepadPointForSlot = GetGamepadPointForSlot(inv, ItemSlot.Context.BankItem, slot);
			if (PlayerInput.UsingGamepadUI)
			{
				flag = (UILinkPointNavigator.CurrentPoint == gamepadPointForSlot);
				if (PlayerInput.SettingsForUI.PreventHighlightsForGamepad)
					flag = false;
			}

			Texture2D value = TextureAssets.InventoryBack.Value;
			Color color2 = Color.White;
            if (!UseEnchantmentSlot() && (context == 0 || context == 10))
            {
				context = 5;
            }
			switch (context)
			{
				case 2:
					value = TextureAssets.InventoryBack2.Value;
					break;
				case 3:
					value = TextureAssets.InventoryBack3.Value;
					break;
				case 4 when !Item.favorited:
					value = TextureAssets.InventoryBack4.Value;
					break;
				case 4 when Item.favorited:
					value = (Texture2D)ModContent.Request<Texture2D>("WeaponEnchantments/UI/Sprites/Inventory_Back4(Favorited)");
					break;
				case 5:
					value = TextureAssets.InventoryBack5.Value;
					break;
				case 6:
					value = TextureAssets.InventoryBack6.Value;
					break;
				case 7:
					value = TextureAssets.InventoryBack7.Value;
					break;
				case 8:
					value = TextureAssets.InventoryBack8.Value;
					break;
				case 9:
					value = TextureAssets.InventoryBack9.Value;
					break;
				case 10:
					value = TextureAssets.InventoryBack10.Value;
					break;
				case 11:
					value = TextureAssets.InventoryBack11.Value;
					break;
				case 12:
					value = TextureAssets.InventoryBack12.Value;
					break;
				case 13:
					value = TextureAssets.InventoryBack13.Value;
					break;
				case 14:
					value = TextureAssets.InventoryBack14.Value;
					break;
				case 15:
					value = TextureAssets.InventoryBack15.Value;
					break;
				case 16:
					value = TextureAssets.InventoryBack16.Value;
					break;
				case 17:
					value = TextureAssets.InventoryBack17.Value;
					break;
				case 18:
					value = TextureAssets.InventoryBack18.Value;
					break;
				default:
					value = TextureAssets.InventoryBack.Value;
					break;
			}

			spriteBatch.Draw(value, position, null, color2, 0f, default(Vector2), inventoryScale, SpriteEffects.None, 0f);

			Vector2 vector = value.Size() * inventoryScale;
			if (item.type > ItemID.None && item.stack > 0)
			{
				Main.instance.LoadItem(item.type);
				Texture2D value7 = TextureAssets.Item[item.type].Value;
				Rectangle rectangle2 = (Main.itemAnimations[item.type] == null) ? value7.Frame() : Main.itemAnimations[item.type].GetFrame(value7);
				Color currentColor = Color.White;
				float scale3 = 1f;
				GetItemLight(ref currentColor, ref scale3, item);
				float num8 = 1f;
				if (rectangle2.Width > 32 || rectangle2.Height > 32)
					num8 = ((rectangle2.Width <= rectangle2.Height) ? (32f / (float)rectangle2.Height) : (32f / (float)rectangle2.Width));

				num8 *= inventoryScale;
				Vector2 position2 = position + vector / 2f - rectangle2.Size() * num8 / 2f;
				Vector2 origin = rectangle2.Size() * (scale3 / 2f - 0.5f);

				if (!ItemLoader.PreDrawInInventory(item, spriteBatch, position2, rectangle2, item.GetAlpha(currentColor), item.GetColor(color), origin, num8 * scale3))
					goto SkipVanillaItemDraw;

				spriteBatch.Draw(value7, position2, rectangle2, item.GetAlpha(currentColor), 0f, origin, num8 * scale3, SpriteEffects.None, 0f);
				if (item.color != Color.Transparent)
				{
					Color newColor = color;

					// Extra context.

					spriteBatch.Draw(value7, position2, rectangle2, item.GetColor(newColor), 0f, origin, num8 * scale3, SpriteEffects.None, 0f);
				}

			SkipVanillaItemDraw:
				ItemLoader.PostDrawInInventory(item, spriteBatch, position2, rectangle2, item.GetAlpha(currentColor), item.GetColor(color), origin, num8 * scale3);

				if (ItemID.Sets.TrapSigned[item.type])
					spriteBatch.Draw(TextureAssets.Wire.Value, position + new Vector2(40f, 40f) * inventoryScale, new Rectangle(4, 58, 8, 8), color, 0f, new Vector2(4f), 1f, SpriteEffects.None, 0f);

				if (item.stack > 1)
					ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, item.stack.ToString(), position + new Vector2(10f, 26f) * inventoryScale, color, 0f, Vector2.Zero, new Vector2(inventoryScale), -1f, inventoryScale);
			}

			if (gamepadPointForSlot != -1)
				UILinkPointNavigator.SetPosition(gamepadPointForSlot, position + vector * 0.75f);
		}
		private static int GetGamepadPointForSlot(Item[] inv, int context, int slot)
		{
			Player localPlayer = Main.LocalPlayer;
			int result = -1;
			switch (context)
			{
				case 0:
				case 1:
				case 2:
					result = slot;
					break;
				case 8:
				case 9:
				case 10:
				case 11:
					{
						int num = slot;
						if (num % 10 == 9 && !localPlayer.CanDemonHeartAccessoryBeShown())
							num--;

						result = 100 + num;
						break;
					}
				case 12:
					if (inv == localPlayer.dye)
					{
						int num2 = slot;
						if (num2 % 10 == 9 && !localPlayer.CanDemonHeartAccessoryBeShown())
							num2--;

						result = 120 + num2;
					}
					if (inv == localPlayer.miscDyes)
						result = 185 + slot;
					break;
				//TML Context: GamePad number magic aligned to match DemonHeart Accessory.
				//TML Note: There is no Master Mode Accessory slot code here for Gamepads.
				//TML-added [[
				/*TODO: Fix later because gamepads are trashing all
				case -10:
				case -11:
					int num3M = slot;
					if (!LoaderManager.Get<AccessorySlotLoader>().ModdedIsAValidEquipmentSlotForIteration(slot, localPlayer))
						num3M--;

					result = 100 + num3M;
					break;
				case -12:
					int num4M = slot;
					if (!LoaderManager.Get<AccessorySlotLoader>().ModdedIsAValidEquipmentSlotForIteration(slot, localPlayer))
						num4M--;

					result = 120 + num4M;
					break;
				// ]]
				*/
				case 19:
					result = 180;
					break;
				case 20:
					result = 181;
					break;
				case 18:
					result = 182;
					break;
				case 17:
					result = 183;
					break;
				case 16:
					result = 184;
					break;
				case 3:
				case 4:
					result = 400 + slot;
					break;
				case 15:
					result = 2700 + slot;
					break;
				case 6:
					result = 300;
					break;
				case 22:
					if (UILinkPointNavigator.Shortcuts.CRAFT_CurrentRecipeBig != -1)
						result = 700 + UILinkPointNavigator.Shortcuts.CRAFT_CurrentRecipeBig;
					if (UILinkPointNavigator.Shortcuts.CRAFT_CurrentRecipeSmall != -1)
						result = 1500 + UILinkPointNavigator.Shortcuts.CRAFT_CurrentRecipeSmall + 1;
					break;
				case 7:
					result = 1500;
					break;
				case 5:
					result = 303;
					break;
				case 23:
					result = 5100 + slot;
					break;
				case 24:
					result = 5100 + slot;
					break;
				case 25:
					result = 5108 + slot;
					break;
				case 26:
					result = 5000 + slot;
					break;
				case 27:
					result = 5002 + slot;
					break;
				case 29:
					result = 3000 + slot;
					if (UILinkPointNavigator.Shortcuts.CREATIVE_ItemSlotShouldHighlightAsSelected)
						result = UILinkPointNavigator.CurrentPoint;
					break;
				case 30:
					result = 15000 + slot;
					break;
			}

			return result;
		}
	}
}
