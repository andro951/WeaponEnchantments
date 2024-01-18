using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponEnchantments.Common.Utility.LogSystem.WebpageComponenets
{
    public class Paragraph : WebpageComponent
    {
        public string Text { private set; get; }
        public Paragraph(string text) {
            Text = text;
        }
        public override string ToString() {
            return Text + "<br/>";
        }

    }
}
