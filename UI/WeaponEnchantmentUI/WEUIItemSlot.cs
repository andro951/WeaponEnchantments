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

namespace WeaponEnchantments.UI.WeaponEnchantmentUI
{
    public class WEUIItemSlot : UIElement
    {
		internal Item Item;
		private readonly int _context;
		private readonly float _scale;
		internal Func<Item, bool> ValidItemFunc;

		internal event Action<int> OnEmptyMouseover;

		private int timer = 0;
		
		internal WEUIItemSlot(int context = ItemSlot.Context.BankItem, float scale = 0.86f)
		{
			_context = context;
			_scale = scale;
			Item = new Item();
			Item.SetDefaults(0);

			//Asset<inventoryBack9> = TextureAssets.InventoryBack9.Value;
			//Asset<Texture2D> inventoryBack9 = TextureAssets.InventoryBack9;
			//Width.Set(inventoryBack9.Width() * scale, 0f);
			//Height.Set(inventoryBack9.Height() * scale, 0f);
			Width.Set(49 * scale, 0f);
			Height.Set(49 * scale, 0f);
		}

		/// <summary>
		/// Returns true if this item can be placed into the slot (either empty or a pet item)
		/// </summary>
		internal bool Valid(Item item)
		{
			return ValidItemFunc(item);
		}

		internal void HandleMouseItem()
		{
			if (ValidItemFunc == null || Valid(Main.mouseItem))
			{
				//Handles all the click and hover actions based on the context
				ItemSlot.Handle(ref Item, _context);
			}
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			float oldScale = Main.inventoryScale;
			Main.inventoryScale = _scale;
			Rectangle rectangle = GetDimensions().ToRectangle();

			bool contains = ContainsPoint(Main.MouseScreen);

			if (contains && !PlayerInput.IgnoreMouseInterface)
			{
				Main.LocalPlayer.mouseInterface = true;
				HandleMouseItem();
			}
			ItemSlot.Draw(spriteBatch, ref Item, _context, rectangle.TopLeft());

			if (contains && Item.IsAir)
			{
				timer++;
				OnEmptyMouseover?.Invoke(timer);
			}
			else if (!contains)
			{
				timer = 0;
			}

			//Main.inventoryScale = oldScale;
		}



		//Based on Vanilla
		/*
        private Item[] _itemArray;
        private int _itemIndex;//Probably not ever used since vanilla will use the UIItemSlot, look in ItemSlot.cs for UIItemSlot
        private int _itemSlotContext;
        private int _itemContext;
        
        internal Item[] item;
        internal event Action<int> OnEmptyMouseover;
        
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
