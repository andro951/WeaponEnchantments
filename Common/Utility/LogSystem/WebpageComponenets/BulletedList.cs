using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponEnchantments.Common.Utility.LogSystem.WebpageComponenets
{
	public class BulletedList : WebpageComponent
    {
        public object[] Elements { private set; get; }
        private bool _png;
        private bool _links;
        public BulletedList(bool png = false, bool links = false, params object[] elements) {
            _png = png;
            _links = links;
            Elements = elements;
        }
        public override string ToString() {
            string text = "";
            foreach (object element in Elements) {
                text += "* ";
                string elem = element.ToString();
                if (_links) {
                    if (_png) {
                        elem = elem.ToItemPNG(link: true);
                    }
                    else {
                        elem = elem.ToLink();
                    }
                }
                else if (_png) {
                    elem = elem.ToItemPNG();
                }

                text += elem;

                text += "\n";
            }

            return text;
        }
    }
}
