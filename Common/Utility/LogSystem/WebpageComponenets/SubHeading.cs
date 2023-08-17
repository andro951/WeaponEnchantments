using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using androLib.Common.Utility;
using androLib.Common.Globals;

namespace WeaponEnchantments.Common.Utility.LogSystem.WebpageComponenets
{
    public class SubHeading : WebpageComponent
    {
        public int HeadingNumber { private set; get; }
        public string Text { private set; get; }
        public SubHeading(string text, int num = 1) {
            HeadingNumber = num + 2;
            Text = text;
        }
        public override string ToString() {
            string markup = '='.FillString(HeadingNumber);
            return $"{markup} {Text} {markup}";
        }
    }
}
