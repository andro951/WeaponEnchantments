using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Items;
using static WeaponEnchantments.UI.WeaponEnchantmentUI;

namespace WeaponEnchantments.UI
{
    public class WEUIItemSlot : UIElement
    {
		internal Item Item;
		private readonly int _itemContext;
		private readonly int _context;
		private readonly float _scale;
		private readonly bool _utilitySlot;
		private readonly int _slotTier;

		internal event Action<int> OnMouseover;
		internal event Action<int> OnItemMouseover;//Trying to Add this so OnItemMouseover apears when item is in hand

		private int timer = 0;
		
		internal WEUIItemSlot(int context, int itemContext, int slotTier = 0, bool utilitySlot = false, float scale = 0.86f)
		{
			_context = context;//ItemSlot.context.ChestItem or BankItem or InventoryCoin
			_itemContext = itemContext;//0 = itemSlot, 1 = enchantmentSlot, 2 = essenceSlot
			_slotTier = slotTier;//Associated enchantment table tier required for enchantmentSlot to unlock or the rarity of each essenceSlot
			_utilitySlot = utilitySlot;//Only true for the last enchantmentSlot, it can only have utility enchantments
			_scale = scale;
			Item = new Item();
			Item.SetDefaults();
			Width.Set(49 * scale, 0f);
			Height.Set(49 * scale, 0f);
		}//Constructor
		internal bool Valid(Item item)
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
						if (wePlayer.enchantingTableUI.itemSlotUI[0].Item.IsAir)
						{
							for(int i = 0; i < EnchantingTable.maxEnchantments; i++)
                            {
								if (!wePlayer.enchantingTableUI.enchantmentSlotUI[0].Item.IsAir)
                                {
									return false;
                                }
                            }
							return WEMod.IsEnchantable(item);
						}//check item is valid
						else
						{
							return false;
						}
					case ItemSlotContext.Enchantment:
						if (!wePlayer.enchantingTableUI.itemSlotUI[0].Item.IsAir)
						{
							if (_slotTier <= wePlayer.enchantingTableTier)
							{
								if (WEMod.IsEnchantmentItem(item, _utilitySlot))
								{
									return wePlayer.enchantingTableUI.itemSlotUI[0].Item.GetGlobalItem<EnchantedItem>().GetLevelsAvailable() >= ((Enchantments)item.ModItem).GetLevelCost();
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

		internal void HandleMouseItem()
		{
			WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
			if (Valid(Main.mouseItem))
			{
				if (Main.mouseItem.type == PowerBooster.ID && !wePlayer.enchantingTableUI.itemSlotUI[0].Item.IsAir && wePlayer.enchantingTableUI.itemSlotUI[0].Item.GetGlobalItem<EnchantedItem>().powerBoosterInstalled)
				{
					Main.mouseItem = new Item();
					wePlayer.enchantingTableUI.itemSlotUI[0].Item.GetGlobalItem<EnchantedItem>().powerBoosterInstalled = true;
				}//If using a PowerBooster, destroy the booster and update the global item.
				else
				{
					ItemSlot.Handle(ref Item, _context);//Handles all the click and hover actions based on the context
				}
			}
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
			float oldScale = Main.inventoryScale;
			Main.inventoryScale = _scale;
			Rectangle rectangle = GetDimensions().ToRectangle();

			bool contains = ContainsPoint(Main.MouseScreen);

			if (contains && !PlayerInput.IgnoreMouseInterface)
			{
				wePlayer.Player.mouseInterface = true;
				HandleMouseItem();
			}
			ItemSlot.Draw(spriteBatch, ref Item, _context, rectangle.TopLeft());
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



		//Based on Vanilla
		/*
        private Item[] _itemArray;
        private int _itemIndex;//Probably not ever used since vanilla will use the UIItemSlot, look in ItemSlot.cs for UIItemSlot
        private int _itemSlotContext;
        private int _itemContext;
        
        internal Item[] item;
        internal event Action<int> OnMouseover;
        
        public WEUIItemSlot(Item[] itemArray, int itemIndex, int itemSlotContext, int itemContext)
        {
            //ArgumentNullException.ThrowIfNull(itemArray);//Recomended fix by Exterminator
            _itemArray = itemArray;
            _itemIndex = itemIndex;
            _itemSlotContext = itemSlotContext;
            Width = new StyleDimension(48f, 0f);
            Height = new StyleDimension(48f, 0f);
        }
        

        
        private void HandleItemSlotLogic()//Where you need to handle if items are allowed to be placed in.
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            if (IsMouseHovering)
            {
                if (!WeaponEnchantmentUI.IsBlockedFromTransferIntoEnchantingTable(_itemArray[_itemIndex], _itemArray, _itemContext))
                {
                    wePlayer.Player.mouseInterface = true;
                    Item inv = _itemArray[_itemIndex];
                    ItemSlot.OverrideHover(ref inv, _itemSlotContext);
                    ItemSlot.LeftClick(ref inv, _itemSlotContext);
                    ItemSlot.RightClick(ref inv, _itemSlotContext);
                    ItemSlot.MouseHover(ref inv, _itemSlotContext);
                    _itemArray[_itemIndex] = inv;
                }
            }
        }
        

        
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            HandleItemSlotLogic();
            Item inv = _itemArray[_itemIndex];//System.NullReferenceException: 'Object reference not set to an instance of an object. _itemArray was null.
            Vector2 position = GetDimensions().Center() + new Vector2(52f, 52f) * -0.5f * Main.inventoryScale;
            ItemSlot.Draw(spriteBatch, ref inv, _itemSlotContext, position);
        }
        */
	}
}
