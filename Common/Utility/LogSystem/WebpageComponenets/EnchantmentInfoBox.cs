using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponEnchantments.Common.Utility.LogSystem.WebpageComponenets
{
    public class EnchantmentInfoBox
    {
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
			
			string wikiDescription = ((Enchantment)items[0].WEModItem).WikiDescription;
			
			return wikiDescription ?? "";
		}
	}
	List<Enchantment> enchantments = new();
        List<ItemInfo> items = new();
	public void Add(Enchantment enchantment) {
		if (name == null)
			name = $"{enchantment.enchantmentTypeName.AddSpaces()} Enchantment";
		
		enchantments.Add(enchantment);
		items.Add(new ItemInfo(enchantment));
	}
	public void AddStatistics(WebPage webPage) {
		webPage.AddParagraph(this.ToString());
	}
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
		
		List<Dictionary<string, EnchantmentEffect>> effects = new();
		List<string> effectNames = new();
		for(int i = 0; i < items.Count; i++) {
			foreach(EnchantmentEffect effect in enchantments[i].Effects) {
				string name = effect.Name;
				effects[i].Add(name, effect);
				if (!effectNames.Contains(name))
					effectNames.Add(name);
			}
		}
		
		List<List<string>> list = new();
		list.Add(effectNames);
		
		for(int i = 0; i < items.Count; i++) {
			Dictionary<string, EnchantmentEffect> dict = effects[i];
			foreach(string name in effectNames) {
				string value = "";
				if (dict.ContainsKey(name))
					value = dict[name].;//Need to make add a tooltip value to all Enchantment Effects for this
				
				list[i].Add(value);
			}
		}
		
		webPage.AddTable(effects);
	}
	public void AddAllowedList(WebPage webPage) {
		if (items.Count == 0)
			return;
		
		List<List<string>> list = new();
		Enchantment enchantment = enchantments[0];
		foreach(EItemType itemType in Enum.GetValues...) {
		string allowedListValue = enchantment.AllowedList.Containts(itemType) ? enchantment.AllowedList[itemType].PercentString() : "Not Allowed" ;
			List<string> entry = new() { itemType.ToString().AddSpaces(), allowedListValue };
			
			list.Add(entry);
		}
		
		webPage.AddTable(list...);
	}
	public void AddRecipes(WebPage webPage) {
		
		webPage.AddParagraph("==Crafting==\n" +
			"<div class="tabber-borderless"><tabber>");
		
		string[] labels = {"Basic", "Common", "Rare", "Epic", "Legendary"}; 
		for (int i = 0; i < items.Count; i++) {
			webPage.AddParagraph($"{i > 0 ? "|-|" : ""}{labels[i]}=");
			items[i].AddRecipes(webPage);
		}
		
		webPage.AddParagraph("</tabber></div>");
	}
        public override string ToString() {
		if (items.Count == 0)
			return "";
		
		string text = $"{"{{"}Infobox enchantment" + 
		$"| name     = {name}\n\n";
		for (int i = 0; i < items.Count; i++) {
			int num = i + 1;
			ItemInfo itemInfo = items[i];
			text += 
			$"| image{num}   = {itemInfo.Image}\n" + 
			$"| tooltip{num} = <i>'{itemInfo.Tooltip}'</i>\n" +
			$"{(i == 0 && itemInfo.TryGetShopPrice() ? $"| buy      = {itemInfo.ShopPrice.GetCoinsPNG()}" : "")}\n" +
			$"| sell{num}    = {(itemInfo.Item.value / 5).GetCoinsPNG()}\n\n";
		}
		
		text += $"| research = {items[0].Research}";
		text += "}}\n";
		
            	return text;
        }
    }
}
