using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;
using Terraria.UI;
using Terraria.UI.Chat;
using Terraria.UI.Gamepad;
using WeaponEnchantments;
using WeaponEnchantments.Items;
using WeaponEnchantments.Common.GlobalItems;

namespace WeaponEnchantments.UI.WeaponEnchantmentUI
{
    public class WeaponEnchantmentUI : UIPanel
    {
		public class ButtonID//Need button sprites  Do images that show the name when you hover over them
        {
			public const int Enchant = 0;
			public const int Disenchant = 1;
            public const int LootAll = 2;
            public const int Offer = 3;
            public const int Count = 4;
		}
        public class ItemSlotContext
        {
            public const int Item = 0;
            public const int Enchantment = 1;
            public const int Essence = 2;
        }

        public const bool PR = true;

		public static string[] ButtonNames = new string[] { "Enchant", "Disenchant", "Offer" };
        public const float buttonScaleMinimum = 0.75f;
        public const float buttonScaleMaximum = 1f;
        public static float[] ButtonScale = new float[ButtonID.Count];
        public static bool[] ButtonHovered = new bool[ButtonID.Count];

        private UIText titleText;
        private UIPanel[] button = new UIPanel[ButtonID.Count];
        private List<UIPanel> panels;
        public WEUIItemSlot[] itemSlotUI = new WEUIItemSlot[EnchantingTable.maxItems];
        public WEUIItemSlot[] enchantmentSlotUI = new WEUIItemSlot[EnchantingTable.maxEnchantments];
        public WEUIItemSlot[] essenceSlotUI = new WEUIItemSlot[EnchantingTable.maxEssenceItems];

        private readonly static Color bgColor = new Color(73, 94, 171);
        private readonly static Color hoverColor = new Color(100, 118, 184);

        internal const int width = 480;
        internal const int height = 155;

        internal int RelativeLeft => Main.screenWidth / 2 - width / 2;
        internal int RelativeTop => Main.screenHeight / 2 + 42; //Half the player height on 200% zoom

        internal bool firstDraw = true;

        //Used to notify the UI to save the item instead of dropping it
        //internal static bool saveItemInUI = false;

        public override void OnInitialize()//*
        {
            if (PR)
            {
                WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
                Width.Pixels = width;
                Height.Pixels = height;
                Top.Pixels = int.MaxValue / 2;
                Left.Pixels = int.MaxValue / 2;

                panels = new List<UIPanel>();

                float nextElementY = -PaddingTop / 2;

                titleText = new UIText("Item           Enchantments      Utility  ")
                {
                    Top = { Pixels = nextElementY },
                    Left = { Pixels = 0 },
                    HAlign = 0.5f
                };
                Append(titleText);

                nextElementY += 20;
                //Edit sprite size and change constructor to have color or png name argument.
                for (int i = 0; i < EnchantingTable.maxItems; i++)
                {
                    //wePlayer.enchantingTableUI.itemSlotUI = new UIItemSlot(wePlayer.enchantingTable.item, wePlayer.enchantingTable.item.Length, ItemSlotContext.Item)
                    wePlayer.enchantingTableUI.itemSlotUI[i] = new WEUIItemSlot(ItemSlot.Context.ChestItem)//Vanilla Main.CreativeMenuc.ProvideItemSlotElement(0)    Only ever calls ItemIndex 0, not sure what to do?
                    {
                        Left = { Pixels = -145f },
                        Top = { Pixels = nextElementY },
                        HAlign = 0.5f,
                        ValidItemFunc = item => item.IsAir || WEMod.IsEnchantable(item)
                    };
                    wePlayer.enchantingTableUI.itemSlotUI[i].OnEmptyMouseover += (timer) =>
                    {
                        Main.hoverItemName = "       Place a weapon, piece of armor or accessory here.       ";
                        if (timer > 60)
                        {
                            Main.hoverItemName =
                            "       Place a weapon, piece of armor or accessory here.       "
                        + "\nUpgrading Enchanting Table Tier unlocks more Enchantment slots."
                        + "\n       Using weapon Enchantments on armor or accessories       " +
                            "\n          provides diminished bonuses and vice versa.          ";
                        }
                    };
                    //wePlayer.enchantingTableUI.itemSlotUI[i].Item = wePlayer.enchantingTable.item[i].Clone();
                    //wePlayer.enchantingTable.item[i].stack = 0;
                    Append(wePlayer.enchantingTableUI.itemSlotUI[i]);
                }
                for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
                {
                    if (i < EnchantingTable.maxEnchantments - 1)
                    {
                        wePlayer.enchantingTableUI.enchantmentSlotUI[i] = new WEUIItemSlot(ItemSlot.Context.BankItem)
                        {
                            Left = { Pixels = -67f + 47.52f * i },
                            Top = { Pixels = nextElementY },
                            HAlign = 0.5f,
                            ValidItemFunc = item => item.IsAir || WEMod.IsEnchantmentItem(item, false)
                        };
                        wePlayer.enchantingTableUI.enchantmentSlotUI[i].OnEmptyMouseover += (timer) =>
                        {
                            Main.hoverItemName = "                   Place Enchantments here.                    "; //change to a titleText = new UIText("Item           Enchantments      Utility  ")
                        if (timer > 60)
                            {
                                Main.hoverItemName =
                            "                   Place Enchantments here.                    "
                        + "\nUpgrading Enchanting Table Tier unlocks more Enchantment slots."
                        + "\n       Using weapon Enchantments on armor or accessories       " +
                            "\n          provides diminished bonuses and vice versa.          ";
                            }
                        };
                    }
                    else
                    {
                        wePlayer.enchantingTableUI.enchantmentSlotUI[i] = new WEUIItemSlot(ItemSlot.Context.BankItem)
                        {
                            Left = { Pixels = -67f + 47.52f * i },
                            Top = { Pixels = nextElementY },
                            HAlign = 0.5f,
                            ValidItemFunc = item => item.IsAir || WEMod.IsEnchantmentItem(item, true)
                        };
                        wePlayer.enchantingTableUI.enchantmentSlotUI[i].OnEmptyMouseover += (timer) =>
                        {
                            Main.hoverItemName = "            Only utility Enchantments can go here.             "; //change to a titleText = new UIText("Item           Enchantments      Utility  ")
                        if (timer > 60)
                            {
                                Main.hoverItemName =
                            "            Only utility Enchantments can go here.             "
                        + "\nUpgrading Enchanting Table Tier unlocks more Enchantment slots."
                        + "\n       Using weapon Enchantments on armor or accessories       " +
                            "\n          provides diminished bonuses and vice versa.          ";
                            }
                        };
                    }
                    Append(wePlayer.enchantingTableUI.enchantmentSlotUI[i]);
                }
                for (int i = 0; i < EnchantingTable.maxEssenceItems; i++)
                {
                    wePlayer.enchantingTableUI.essenceSlotUI[i] = new WEUIItemSlot(ItemSlot.Context.InventoryCoin)//Copy to be same as code above
                    {
                        Left = { Pixels = -67f + 47.52f * i },
                        Top = { Pixels = nextElementY + 60 },
                        HAlign = 0.5f,
                        ValidItemFunc = item => item.IsAir || WEMod.IsEssenceItem(item)
                    };
                    wePlayer.enchantingTableUI.essenceSlotUI[i].OnEmptyMouseover += (timer) =>
                    {
                        Main.hoverItemName = "                   Place Enchantments here.                    "; //change to a titleText = new UIText("Item           Enchantments      Utility  ")
                    if (timer > 60)
                        {
                            Main.hoverItemName =
                        "                   Place Enchantments here.                    "
                    + "\nUpgrading Enchanting Table Tier unlocks more Enchantment slots."
                    + "\n       Using weapon Enchantments on armor or accessories       " +
                        "\n          provides diminished bonuses and vice versa.          ";
                        }
                    };
                    Append(wePlayer.enchantingTableUI.essenceSlotUI[i]);
                }

                nextElementY += 50;

                float ratioFromCenter = 0.22f;

                button[ButtonID.Enchant] = new UIPanel()
                {
                    Top = { Pixels = nextElementY },
                    Left = { Pixels = -66 },
                    Width = { Pixels = 100f },
                    Height = { Pixels = 30f },
                    HAlign = 0.5f - ratioFromCenter,
                    BackgroundColor = bgColor
                };
                button[ButtonID.Enchant].OnClick += (evt, element) => { Enchant(); };

                UIText enchantButonText = new UIText("Enchant")
                {
                    Top = { Pixels = -4f },
                    Left = { Pixels = 5f }
                };
                button[ButtonID.Enchant].Append(enchantButonText);
                Append(button[ButtonID.Enchant]);
                panels.Add(button[ButtonID.Enchant]);

                nextElementY += 35;

                button[ButtonID.Disenchant] = new UIPanel()
                {
                    Top = { Pixels = nextElementY },
                    Left = { Pixels = -66 },
                    Width = { Pixels = 100f },
                    Height = { Pixels = 30f },
                    HAlign = 0.5f - ratioFromCenter,
                    BackgroundColor = bgColor
                };
                button[ButtonID.Disenchant].OnClick += (evt, element) => { Disenchant(); }; //Change this  Not origionally in code but I'll need it.

                UIText disenchantButtonText = new UIText("Disenchant")
                {
                    Top = { Pixels = -4f },
                    Left = { Pixels = -6f }
                };
                button[ButtonID.Disenchant].Append(disenchantButtonText);
                Append(button[ButtonID.Disenchant]);
                panels.Add(button[ButtonID.Disenchant]);
            }
        }
        public override void OnActivate()//*
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            base.OnActivate();//Try to get rid of
            SoundEngine.PlaySound(SoundID.MenuOpen);
            for (int i = 0; i < EnchantingTable.maxItems; i++)
            {
                 wePlayer.enchantingTableUI.itemSlotUI[i].Item = wePlayer.enchantingTable.item[i].Clone();
            }
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
            {
                wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item = wePlayer.enchantingTable.enchantmentItem[i].Clone();
            }
            for (int i = 0; i < EnchantingTable.maxEssenceItems; i++)
            {
                wePlayer.enchantingTableUI.essenceSlotUI[i].Item = wePlayer.enchantingTable.essenceItem[i].Clone();
            }
        }
        public override void OnDeactivate()//*
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            base.OnDeactivate();
            if (!Main.gameMenu)
            {
                SoundEngine.PlaySound(SoundID.MenuClose);
            }
            for (int i = 0; i < EnchantingTable.maxItems; i++)
            {
                wePlayer.enchantingTable.item[i] = wePlayer.enchantingTableUI.itemSlotUI[i].Item.Clone();
            }
            for(int i = 0; i < EnchantingTable.maxEnchantments; i++)
            {
                wePlayer.enchantingTable.enchantmentItem[i] = wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item.Clone();
            }
            for(int i = 0; i < EnchantingTable.maxEssenceItems; i++)
            {
                wePlayer.enchantingTable.essenceItem[i] = wePlayer.enchantingTableUI.essenceSlotUI[i].Item.Clone();
            }
        }

        public override void Update(GameTime gameTime)//*
        {
            base.Update(gameTime);//Try getting rid of
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;

            foreach (var panel in panels)
            {
                panel.BackgroundColor = panel.IsMouseHovering ? hoverColor : bgColor;
            }
        }
        protected override void DrawSelf(SpriteBatch spriteBatch)//*
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            /* PR version works fine without this
            if (firstDraw)
            {
                firstDraw = false;
                return;
            }
            if (ContainsPoint(Main.MouseScreen))
            {
                wePlayer.Player.mouseInterface = true;
                wePlayer.Player.cursorItemIconEnabled = false;
                Main.ItemIconCacheUpdate(0);
            }
            */


            //From vaninna loook at again
            if (!PR)
            {
                if (wePlayer.usingEnchantingTable && !Main.recBigList)
                {
                    Main.inventoryScale = 0.755f;
                    if (Utils.FloatIntersect(Main.mouseX, Main.mouseY, 0f, 0f, 73f, Main.instance.invBottom, 560f * Main.inventoryScale, 224f * Main.inventoryScale))
                        Main.player[Main.myPlayer].mouseInterface = true;
                    DrawButtons(spriteBatch);
                    DrawSlots(spriteBatch);
                }
                else
                {
                    for (int i = 0; i < ButtonID.Count; i++)
                    {
                        ButtonScale[i] = 0.75f;
                        ButtonHovered[i] = false;
                    }
                }
            }
            base.DrawSelf(spriteBatch);
        }



        
        public static void UpdateHover(int ID, bool hovering)
        {
            if (hovering)
            {
                if (!ButtonHovered[ID])
                {
                    SoundEngine.PlaySound(12);
                    ButtonHovered[ID] = true;
                    ButtonScale[ID] += 0.05f;
                    if(ButtonScale[ID] > 1f)
                    {
                        ButtonScale[ID] = 1f;
                    }
                }
            }
            else
            {
                ButtonHovered[ID] = false;
                ButtonScale[ID] -= 0.05f;
                if (ButtonScale[ID] < 0.75f)
                    ButtonScale[ID] = 0.75f;
            }
        }

        //Having Draw exist at all causes the UI to not show up
        /*
        public override void Draw(SpriteBatch spriteBatch)
        {
            /*
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            if (wePlayer.usingEnchantingTable && !Main.recBigList)
            {
                Main.inventoryScale = 0.755f;
                if (Utils.FloatIntersect(Main.mouseX, Main.mouseY, 0f, 0f, 73f, Main.instance.invBottom, 560f * Main.inventoryScale, 224f * Main.inventoryScale))
                    Main.player[Main.myPlayer].mouseInterface = true;
                DrawButtons(spriteBatch);
                DrawSlots(spriteBatch);
            }
            else
            {
                for (int i = 0; i < ButtonID.Count; i++)
                {
                    ButtonScale[i] = 0.75f;
                    ButtonHovered[i] = false;
                }
            }
            //base.DrawSelf(spriteBatch);
        }
        */

        private static void DrawButtons(SpriteBatch spritebatch)//Not used if Draw is disabled
        {
            for (int i = 0; i < ButtonID.Count; i++)
            {
                DrawButton(spritebatch, i, 506, Main.instance.invBottom + 40);//Change this to be the correct spot
            }
        }
		private static void DrawButton(SpriteBatch spriteBatch, int ID, int X, int Y)//Not used if Draw is disabled
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
			int num = ID;
			//if (ID == 7)
			//	num = 5;

			Y += num * 26;
			float num2 = ButtonScale[ID];
			string text = Language.GetTextValue("Mods.WeaponEnchantments.Buttons." + ButtonNames[ID]);//Need to set up names in other languages for this to work

			Vector2 value = FontAssets.MouseText.Value.MeasureString(text);
			Color color = new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor) * num2;
			color = Color.White * 0.97f * (1f - (255f - (float)(int)Main.mouseTextColor) / 255f * 0.5f);
			color.A = byte.MaxValue;
			X += (int)(value.X * num2 / 2f);
			bool flag = Utils.FloatIntersect(Main.mouseX, Main.mouseY, 0f, 0f, (float)X - value.X / 2f, Y - 12, value.X, 24f);
			if (ButtonHovered[ID])
				flag = Utils.FloatIntersect(Main.mouseX, Main.mouseY, 0f, 0f, (float)X - value.X / 2f - 10f, Y - 12, value.X + 16f, 24f);

			if (flag)
				color = Main.OurFavoriteColor;

			ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, text, new Vector2(X, Y), color, 0f, value / 2f, new Vector2(num2), -1f, 1.5f);
			value *= num2;
			UILinkPointNavigator.SetPosition(UILinkPointNavigator.Points.Count + 1, new Vector2((float)X - value.X * num2 / 2f * 0.8f, Y));
			
			if (!flag)
			{
				UpdateHover(ID, hovering: false);
				return;
			}

			UpdateHover(ID, hovering: true);
			if (PlayerInput.IgnoreMouseInterface)
				return;
			wePlayer.Player.mouseInterface = true;
			if (Main.mouseLeft && Main.mouseLeftRelease)
			{
				switch (ID)
				{
					case 0:
						Enchant();
						break;
					case 1:
						Disenchant();
						break;
                    case 2:
                        LootAll();
                        break;
					case 3:
						Offer();
						break;
				}
				Recipe.FindRecipes();
			}
		}
        
        private static void Enchant()
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            if (!wePlayer.enchantingTableUI.itemSlotUI[0].Item.IsAir)
            {
                SoundEngine.PlaySound(SoundID.MaxMana);
                //wePlayer.enchantingTableUI.itemSlotUI[0].Item.damage *= 2;
                //wePlayer.enchantingTableUI.itemSlotUI[0].Item.GetGlobalItem<EnchantedItem>(wePlayer.enchantingTableUI.enchantmentSlotUI);
                EnchantedItem enchantedItem = wePlayer.enchantingTableUI.itemSlotUI[0].Item.GetGlobalItem<EnchantedItem>();
                for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
                {
                    if (!wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item.IsAir)
                    {
                        enchantedItem.enchantments[i] = wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item.Clone();
                        //wePlayer.enchantingTableUI.itemSlotUI[0].Item.damage = (int)(1.1f * wePlayer.enchantingTableUI.itemSlotUI[0].Item.damage);
                    }
                }
                //wePlayer.enchantingTableUI.itemSlotUI[0].Item.type++;
            }
            else
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
            }
        }
        private static void Disenchant()
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            if (!wePlayer.enchantingTableUI.itemSlotUI[0].Item.IsAir)
            {
                SoundEngine.PlaySound(SoundID.MaxMana);
                wePlayer.enchantingTableUI.itemSlotUI[0].Item.damage = ContentSamples.ItemsByType[wePlayer.enchantingTableUI.itemSlotUI[0].Item.type].damage;
            }
            else
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
            }
            //Dont forget to check if created essence + essence in table > maxStack and spawn diff
        }
        private static void LootAll()
        {
            GetItemSettings lootAllSettings = GetItemSettings.LootAllSettings;
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
            {
                if (wePlayer.enchantingTable.enchantmentItem[i].type > 0)
                {
                    wePlayer.enchantingTable.enchantmentItem[i].position = wePlayer.Player.Center;
                    wePlayer.enchantingTable.enchantmentItem[i] = wePlayer.Player.GetItem(Main.myPlayer, wePlayer.enchantingTable.enchantmentItem[i], lootAllSettings);
                }
            }
        }
        private static void Offer()
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            //If offer item is upgrade item
            wePlayer.enchantingTable.tier++;
            wePlayer.enchantingTable.Update();
        }

        
        private static void DrawSlots(SpriteBatch spriteBatch)//Not used if Draw is disabled
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            Item[] item = wePlayer.enchantingTable.item;
            Item[] enchantmentItem = wePlayer.enchantingTable.enchantmentItem;//might need to be for int i < maxEnchantments...or Clone()
            Item[] essenceItem = wePlayer.enchantingTable.essenceItem;//might need to be for int i < maxEssence...or Clone()
            Main.inventoryScale = 0.755f;
            if (Utils.FloatIntersect(Main.mouseX, Main.mouseY, 0f, 0f, 73f, Main.instance.invBottom, 560f * Main.inventoryScale, 224f * Main.inventoryScale) && !PlayerInput.IgnoreMouseInterface)
                wePlayer.Player.mouseInterface = true;
            for (int i = 0; i < EnchantingTable.maxItems; i++)
            {
                int x = (int)(73f + (float)(i * 56) * Main.inventoryScale);//change
                int y = (int)((float)Main.instance.invBottom + (float)( 56) * Main.inventoryScale);//change
                int slot = i  * 10;
                new Color(100, 100, 100, 100);
                if (Utils.FloatIntersect(Main.mouseX, Main.mouseY, 0f, 0f, x, y, (float)TextureAssets.InventoryBack.Width() * Main.inventoryScale, (float)TextureAssets.InventoryBack.Height() * Main.inventoryScale) && !PlayerInput.IgnoreMouseInterface)
                {
                    wePlayer.Player.mouseInterface = true;
                    ItemSlot.Handle(item, 4, slot);
                }
                ItemSlot.Draw(spriteBatch, item, 4, slot, new Vector2(x, y));
            }
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
            {
                int x = (int)(73f + (float)(i * 56) * Main.inventoryScale + 50);//change
                int y = (int)((float)Main.instance.invBottom + (float)( 56) * Main.inventoryScale);//change
                int slot = i  * 10;
                if (i < wePlayer.enchantingTable.availableEnchantmentSlots)
                {
                    new Color(100, 100, 100, 100);
                }
                else
                {
                    new Color(50, 50, 50, 50);
                }
                if (Utils.FloatIntersect(Main.mouseX, Main.mouseY, 0f, 0f, x, y, (float)TextureAssets.InventoryBack.Width() * Main.inventoryScale, (float)TextureAssets.InventoryBack.Height() * Main.inventoryScale) && !PlayerInput.IgnoreMouseInterface)
                {
                    wePlayer.Player.mouseInterface = true;
                    ItemSlot.Handle(enchantmentItem, 4, slot);
                }
                ItemSlot.Draw(spriteBatch, enchantmentItem, 4, slot, new Vector2(x, y));
            }
            for (int i = 0; i < EnchantingTable.maxEssenceItems; i++)
            {
                int x = (int)(73f + (float)(i * 56) * Main.inventoryScale + 100);//change
                int y = (int)((float)Main.instance.invBottom + (float)( 56) * Main.inventoryScale);//change
                int slot = i  * 10;
                new Color(100, 100, 100, 100);
                if (Utils.FloatIntersect(Main.mouseX, Main.mouseY, 0f, 0f, x, y, (float)TextureAssets.InventoryBack.Width() * Main.inventoryScale, (float)TextureAssets.InventoryBack.Height() * Main.inventoryScale) && !PlayerInput.IgnoreMouseInterface)
                {
                    wePlayer.Player.mouseInterface = true;
                    ItemSlot.Handle(essenceItem, 4, slot);
                }
                ItemSlot.Draw(spriteBatch, essenceItem, 4, slot, new Vector2(x, y));
            }
        }

        /*
        public static bool TryPlacingInEnchantingTable(Item I, bool justCheck, int itemSlotContext)//No references to this
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();//sync is always false
            Item[] slotItems;

            switch (itemSlotContext)
            {
                case ItemSlotContext.Item:
                    slotItems = wePlayer.enchantingTable.item;
                    break;
                case ItemSlotContext.Enchantment:
                    slotItems = wePlayer.enchantingTable.enchantmentItem;
                    break;
                case ItemSlotContext.Essence:
                    slotItems = wePlayer.enchantingTable.essenceItem;
                    break;
                default:
                    slotItems = null;
                    break;
            }
            if(!IsBlockedFromTransferIntoEnchantingTable(I, slotItems, itemSlotContext))
            {
                return false;
            }
            bool flag = false;
            switch (itemSlotContext)
            {
                case ItemSlotContext.Item:
                    if (I.stack > 0)
                    {
                        for (int i = 0; i < EnchantingTable.maxItems; i++)
                        {
                            if (slotItems[i].stack != 0)
                                continue;
                            if (justCheck)
                            {
                                flag = true;
                                break;
                            }
                            SoundEngine.PlaySound(SoundID.Grab);
                            slotItems[i] = I.Clone();
                            I.SetDefaults();
                            ItemSlot.AnnounceTransfer(new ItemSlot.ItemTransferInfo(slotItems[i], 0, 3));
                            break;
                        }
                    }
                    return flag;
                case ItemSlotContext.Enchantment:
                    if(I.stack > 0)
                    {
                        for (int i = 0; i < wePlayer.enchantingTable.availableEnchantmentSlots; i++)
                        {
                            if (slotItems[i].stack != 0)
                                continue;
                            if (justCheck)
                            {
                                flag = true;
                                break;
                            }
                            SoundEngine.PlaySound(SoundID.Grab);
                            slotItems[i] = I.Clone();
                            I.SetDefaults();
                            ItemSlot.AnnounceTransfer(new ItemSlot.ItemTransferInfo(slotItems[i], 0, 3));
                            break;
                        }
                    }
                    return flag;
                case ItemSlotContext.Essence:
                    int essenceTier = EnchantmentEssence.GetEssenceTier(I);//Need new function to check essenceTier, GetEssenceTier(I)
                    if (slotItems[essenceTier].stack < I.maxStack)
                    {
                        if(!ItemLoader.CanStack(slotItems[essenceTier], I))
                        {
                            return false;
                        }
                        int stack = I.stack;
                        if (I.stack + slotItems[essenceTier].stack > I.maxStack)
                        {
                            stack = I.maxStack - slotItems[essenceTier].stack;
                        }
                        if (justCheck)
                        {
                            flag = (flag || stack > 0);
                            break;
                        }
                        I.stack -= stack;
                        slotItems[essenceTier].stack += stack;
                        SoundEngine.PlaySound(SoundID.Grab);
                        if(I.stack <= 0)
                        {
                            I.SetDefaults();
                            break;
                        }
                        if (slotItems[essenceTier].type == 0)
                        {
                            slotItems[essenceTier] = I.Clone();
                            I.SetDefaults();
                        }
                    }
                    return flag;
                default:
                    return false;
            }
            return false;
            //Check against tier for slots above not < available
            //Only let the specific item go in each slot
        }*/
        /*public static bool TryPlacingInEnchantment(Item I, bool justCheck, int itemSlotContext)
        {

            //Check against tier for slots above not < available
        }*/
        /*public static bool TryPlacingInEssence(Item I, bool justCheck, int itemSlotContext)
        {
            //Only let the specific item go in each slot
        }*/

        public static bool IsBlockedFromTransferIntoEnchantingTable(Item I, Item[] slotItems, int itemSlotContext)//Look at this again compair to vanilla
        {
            switch (itemSlotContext)
            {
                case ItemSlotContext.Item:
                    if (true)//If I is a weapon or armor or accessory
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                case ItemSlotContext.Enchantment:
                    if (true)//If I is an enchantment
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                case ItemSlotContext.Essence:
                    if (true)//If I is an essence
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                default:
                    return false;
            }
        }
    }
}
