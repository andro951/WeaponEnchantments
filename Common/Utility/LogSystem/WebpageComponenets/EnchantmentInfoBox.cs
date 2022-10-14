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
        List<ItemInfo> items = new();
	public void Add(Enchantment enchantment) {
		if (name == null)
			name = $"{enchantment.enchantmentTypeName.AddSpaces()} Enchantment";
		
		items.Add(new ItemInfo(enchantment));
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
