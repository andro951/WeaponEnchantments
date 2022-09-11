using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponEnchantments.Common.Utility.LogSystem.WebpageComponenets
{
    public class Link
    {
        public string Text { private set; get; }
        public Link(string s, string text = null, bool png = false) {
            if (png) {
                Text = s.ToItemPNG(link: true);
            }
            else {
                Text = s.ToLink(text);
            }
        }
        public override string ToString() {
            return Text + "<br/>";
        }
    }
}
