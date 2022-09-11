using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace WeaponEnchantments.Common.Utility.LogSystem.WebpageComponenets
{
    public class WebPage
    {
        public string HeaderName { private set; get; }
        private List<object> _elements = new();
        public WebPage(string headerName) {
            HeaderName = headerName;
            AddLink("Weapon Enchantments Mod (tModLoader) Wiki", "Main Page");
        }
        public void Add(object obj) => _elements.Add(obj);
        public void AddSubHeading(int num, string text) => Add(new SubHeading(num, text));
        public void AddParagraph(string text) => Add(new Paragraph(text));
        public void AddLink(string s, string text = null, bool png = false) => Add(new Link(s, text, png));
        public void AddBulletedList(bool png = false, bool links = false, params object[] elements) => Add(new BulletedList(png, links, elements));
        public void AddTable<T>(IEnumerable<IEnumerable<T>> elements, string header = null, bool firstRowHeaders = false, bool sortable = false, bool collapsible = true, bool collapsed = false, bool rowspanColumns = false, bool automaticCollapse = false) where T : class {
            if (elements.Count() > 0)
                Add(new Table<T>(elements, header, firstRowHeaders, sortable, collapsible, collapsed, rowspanColumns, automaticCollapse));
        }
        public void NewLine(int num = 1) => _elements.Add('\n'.FillString(num - 1));
        public override string ToString() {
            string text = HeaderName + "\n";
            object last = new();
            foreach (object element in _elements) {
                text += element.ToString() + "\n";
            }

            return text;
        }
    }
}
