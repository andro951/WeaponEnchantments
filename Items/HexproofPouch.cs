using androLib;
using androLib.Common.Utility;
using androLib.Items;
using androLib.UI;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Content.NPCs;
using static androLib.UI.BagUI;
using static Terraria.ID.ContentSamples.CreativeHelper;

namespace WeaponEnchantments.Items {
	[Autoload(false)]
	internal class HexproofPouch : WEModItem, IBagModItem, INeedsSetUpAllowedList, ISoldByNPC {
		public static IBagModItem Instance {
			get {
				if (instance == null)
					instance = new HexproofPouch();

				return instance;
			}
		}
		private static IBagModItem instance;
		public override void SetDefaults() {
			base.SetDefaults();
			Item.width = 12;
			Item.height = 12;
			Item.value = 2000;
		}
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient(ItemID.PurificationPowder, 10)
				.AddIngredient(ItemID.Silk, 1)
				.AddIngredient(ItemID.WhiteString)
				.AddTile(TileID.Loom)
				.Register();
		}
		public override bool CanBeStoredInEnchantmentStorage => true;
		public override int CreativeItemSacrifice => 1;
		public override List<WikiTypeID> WikiItemTypes => new() { WikiTypeID.Storage };
		public override string Artist => "andro951";
		public override string Designer => "andro951";
		public Func<int> SoldByNPCNetID => ModContent.NPCType<Witch>;
		public SellCondition SellCondition => SellCondition.Always;

		public int BagStorageID { get; set; }
		public Color PanelColor => new(18, 14, 37);
		public Color ScrollBarColor => new(4, 2, 18);
		public Color ButtonHoverColor => new(31, 25, 55);
		public int GetBagType() => ModContent.ItemType<HexproofPouch>();
		public int DefaultBagSize => 4;
		public Func<Player, IList<Item>> ExtraStorageLocation => (player) => player.TryGetModPlayer(out WEPlayer wePlayer) ? wePlayer.enchantmentStorageItems : null;
		public void RegisterWithAndroLib(Mod mod) {
			((IBagModItem)this).RegisterWithAndroLibIBagModItem(mod);

			StorageManager.AddBagUIEdit(BagStorageID, (BagUI bagUI) => {
				bagUI.GetPlayersInventory = () => Main.LocalPlayer.inventory.TakePlayerInventory40().Concat(WEPlayer.LocalWEPlayer.enchantmentStorageItems);
				bagUI.GetLootAllTargetInventory = () => WEPlayer.LocalWEPlayer.enchantmentStorageItems;
			});

			INeedsSetUpAllowedList.RegisterAllowedItemsManager(((IBagModItem)this).BagStorageID, CreateAllowedItemsManager);
		}
		public override string LocalizationTooltip => 
			"A small pouch meant to protect the holder from the negative effects of Cursed Essence.\n" +
			"(Placing Cursed Essence in this bag will prevent it from contributing to the player's Cursed Debuff.)";
		public virtual bool ItemAllowedToBeStored(Item item) => AllowedItems.Contains(item.type);
		public virtual void UpdateAllowedList(int item, bool add) {
			if (add) {
				AllowedItems.Add(item);
			}
			else {
				AllowedItems.Remove(item);
			}
		}
		public SortedSet<int> AllowedItems => GetAllowedItemsManager.AllowedItems;
		public virtual AllowedItemsManager GetAllowedItemsManager => INeedsSetUpAllowedList.AllowedItemsManagers[((IBagModItem)this).BagStorageID];
		public virtual Func<AllowedItemsManager> CreateAllowedItemsManager => () => new(GetBagType, () => ((IBagModItem)this).BagStorageID, DevCheck, DevWhiteList, DevModWhiteList, DevBlackList, DevModBlackList, ItemGroups, EndWords, SearchWords, PostSetup);

		public virtual void PostSetup() { }
		public virtual bool? DevCheck(ItemSetInfo info, SortedSet<ItemGroup> itemGroups, SortedSet<string> endWords, SortedSet<string> searchWords) {
			return null;
		}
		public virtual SortedSet<int> DevWhiteList() {
			SortedSet<int> devWhiteList = new() {
				ModContent.ItemType<CursedEssence>()
			};

			return devWhiteList;
		}
		public virtual SortedSet<string> DevModWhiteList() {
			SortedSet<string> devModWhiteList = new() {

			};

			return devModWhiteList;
		}
		public virtual SortedSet<int> DevBlackList() {
			SortedSet<int> devBlackList = new() {

			};

			return devBlackList;
		}
		public virtual SortedSet<string> DevModBlackList() {
			SortedSet<string> devModBlackList = new() {

			};

			return devModBlackList;
		}
		public virtual SortedSet<ItemGroup> ItemGroups() {
			SortedSet<ItemGroup> itemGroups = new() {

			};

			return itemGroups;
		}
		public virtual SortedSet<string> EndWords() {
			SortedSet<string> endWords = new() {

			};

			return endWords;
		}
		public virtual SortedSet<string> SearchWords() {
			SortedSet<string> searchWords = new() {

			};

			return searchWords;
		}
	}
}
