using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponEnchantments.Common.Utility.LogSystem.WebpageComponenets
{
    public class SubHeading
    {
        public int HeadingNumber { private set; get; }
        public string Text { private set; get; }
        public SubHeading(int num, string text) {
            HeadingNumber = num + 3;
            Text = text;
        }
        public override string ToString() {
            string markup = '='.FillString(HeadingNumber);
            return $"{markup} {Text} {markup}";
        }
    }
}
