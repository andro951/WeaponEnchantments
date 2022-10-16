using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponEnchantments.Common.Utility.LogSystem.WebpageComponenets
{
	public class Tabber : WebpageComponent
	{
		List<string> labels = new();
		List<object> objects = new();

		public void Add(string label, object obj) {
			labels.Add(label);
			objects.Add(obj);
		}

		public override string ToString() {
			string tabber = "<div class=\"tabber-borderless\"><tabber>\n";

			for (int i = 0; i < objects.Count; i++) {
				tabber += 
					$"{(i > 0 ? "|-|" : "")}{labels[i]}=\n" +
					$"{objects[i]}";
			}

			tabber += "</tabber></div>";

			return tabber;
		}
	}
}
