using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponEnchantments.Effects;
using WeaponEnchantments.Items;
using androLib.Common.Utility;
using androLib.Common.Globals;
using androLib.Common.Utility.LogSystem.WebpageComponenets;
using androLib.Common.Utility.LogSystem;

namespace WeaponEnchantments.Common.Utility.LogSystem.WebpageComponenets
{
    public class EnchantmentInfoBox : WebpageComponent
	{
		public EnchantmentInfoBox(FloatID id = FloatID.none) {
			AlignID = id;
		}
		public string Name {
			get {
				if (name == null)
					return "";
			
				return name;
			}
		}
		private string name;
		public string WikiDescription {
			get {
				if (items.Count == 0)
					return "";
			
				string wikiDescription = enchantments[0].WikiDescription;
			
				return wikiDescription ?? "";
			}
		}
		public List<Enchantment> enchantments = new();
		public List<ItemInfo_WE> items = new();
		public void Add(Enchantment enchantment) {
			if (name == null)
				name = $"{enchantment.EnchantmentTypeName.AddSpaces()} Enchantment";
			
			enchantments.Add(enchantment);
			items.Add(new ItemInfo_WE(enchantment));
		}
		public void AddStatistics(WebPage webPage) => webPage.Add(this);
		public void AddDrops(WebPage webPage) {
			if (items.Count == 0)
				return;
		
			items[0].AddDrops(webPage);
		}
		public void AddInfo(WebPage webPage) {
			if (items.Count == 0)
				return;
		
			items[0].AddInfo(webPage);
		}
		public void AddEffects(WebPage webPage) {
			if (items.Count == 0)
				return;

			webPage.AddSubHeading("Effects");
			List<Dictionary<string, EnchantmentEffect>> effects = new();
			List<string> effectNames = new();
			for(int i = 0; i < items.Count; i++) {
				Dictionary<string, EnchantmentEffect> effectsDict = new();
				foreach(EnchantmentEffect effect in enchantments[i].Effects) {
					string name = effect.DisplayName;
					if (effectsDict.Keys.Contains(name)) {
						$"{name} already exists in the effectsDict.".LogSimple();
					}
					else {
						effectsDict.Add(name, effect);
					}
					
					if (!effectNames.Contains(name))
						effectNames.Add(name);
				}

				effects.Add(effectsDict);
			}
			
			if (effectNames.Count > 0) {
				List<List<string>> lists = new();
				lists.Add(new(effectNames));

				for (int i = 0; i < items.Count; i++) {
					List<string> list = new();
					Dictionary<string, EnchantmentEffect> dict = effects[i];
					foreach (string name in effectNames) {
						string value = "";
						if (dict.ContainsKey(name))
							value = dict[name].TooltipValue;

						list.Add(value);
					}

					lists.Add(list);
				}

				webPage.AddTable(lists, firstRowHeaders: true);
			}
		}
		public void AddAllowedList(WebPage webPage) {
			if (items.Count == 0)
				return;

			webPage.AddSubHeading("Applicability");
			List<List<string>> list = new();
			Enchantment enchantment = enchantments[0];
			foreach(EItemType itemType in Enum.GetValues(typeof(EItemType)).Cast<EItemType>()) {
				string allowedListValue = enchantment.AllowedList.ContainsKey(itemType) ? enchantment.AllowedList[itemType].PercentString() : "Not Allowed" ;
				List<string> entry = new() { itemType.ToString().AddSpaces(), allowedListValue };
			
				list.Add(entry);
			}
			
			webPage.AddTable(list, firstRowHeaders: true);
		}
		public void AddRecipes(WebPage webPage) {
			webPage.AddSubHeading("Crafting");
			Tabber tabber = new();
		
			string[] labels = {"Basic", "Common", "Rare", "Epic", "Legendary"}; 
			for (int i = 0; i < items.Count; i++) {
				ObjectList objectList = new();
				objectList.AddTable(items[i].RecipesCreateItemTable);
				objectList.AddTable(items[i].RecipesUsedInTable);
				objectList.AddTable(items[i].RecipesReverseRecipesTable);
				//string objectlistStirng = objectList.ToString();
				tabber.Add(labels[i], objectList);
			}

			//string tabberString = tabber.ToString();
			webPage.Add(tabber);
		}
		public void TryAddWikiDescription(WebPage webPage) {
			string text = WikiDescription;
			if (text != null)
				webPage.AddParagraph(text);
		}
		public override string ToString() {
			if (items.Count == 0)
				return "";
		
			string text = 
				$"{"{{"}Infobox enchantment\n" + 
				$"| name     = {name}\n\n";

			for (int i = 0; i < items.Count; i++) {
				int num = i + 1;
				ItemInfo_WE itemInfo = items[i];
				text += $"| image{num}   = {itemInfo.Image}\n";

				itemInfo.GetArtists(out string artistString, out string artModifiedBy);

				if (artistString != null || artModifiedBy != null)
					text += $"| artist{num}  = {artistString}{artModifiedBy}\n";

				text +=
					$"| type{num}    = {itemInfo.GetItemTypes()}\n" +
					$"| tooltip{num} = <i>'{itemInfo.Tooltip}'</i>\n" +
					$"| rarity{num}  = {itemInfo.Rarity}\n" +
					(i == 0 && itemInfo.TryGetShopPrice() ? $"| buy      = {itemInfo.ShopPrice.GetCoinsPNG()}\n" : "") +
					$"| sell{num}    = {(itemInfo.Item.value / 5).GetCoinsPNG()}\n\n";
			}
		
			text += 
				$"| research = {items[0].Research}\n" +
				$"{"}}"}\n";
		
            return text;
		}
    }
}
