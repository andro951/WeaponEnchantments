using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponEnchantments.Common.Utility.LogSystem.WebpageComponenets
{
    public class NumberedList
    {
        public object[] Elements { private set; get; }
        private bool _links;
        public NumberedList(bool links = false, params object[] elements) {
            Elements = elements;
            _links = links;
        }
        public override string ToString() {
            string text = "";
            foreach (object element in Elements) {
                text += "# ";
                string elem = element.ToString();
                if (_links)
                    elem = elem.ToLink();

                text += elem;

                text += "\n";
            }

            return text;
        }
    }
}
