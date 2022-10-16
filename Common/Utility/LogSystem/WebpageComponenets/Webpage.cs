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
            HeaderName = $"Page: {headerName}";
            AddLink("Weapon Enchantments Mod (tModLoader) Wiki", "Main Page");
        }
        public void Add(object obj) => _elements.Add(obj);
        public void AddSubHeading(string text, int num = 1) => Add(new SubHeading(text, num));
        public void AddParagraph(string text) => Add(new Paragraph(text));
        public void AddLink(string s, string text = null, bool png = false) => Add(new Link(s, text, png));
        public void AddPNG(string s) => Add(s.ToPNG() + "<br/>");
        public void AddBulletedList(bool png = false, bool links = false, params object[] elements) => Add(new BulletedList(png, links, elements));
        public void AddTable<T>(IEnumerable<IEnumerable<T>> elements, IEnumerable<string> headers = null, string label = null, bool firstRowHeaders = false, bool sortable = false, bool collapsible = false, bool collapsed = false, bool rowspanColumns = false, bool automaticCollapse = false, int maxWidth = 0, FloatID alignID = FloatID.none) where T : class {
            if (elements.Count() > 0)
                Add(new Table<T>(elements, headers, label, firstRowHeaders, sortable, collapsible, collapsed, rowspanColumns, automaticCollapse, maxWidth, alignID));
        }
        public void AddTable(Table<string> table) {
            if (table.Count > 0)
                Add(table);
		}
        public void NewLine(int num = 1) => _elements.Add('\n'.FillString(num - 1));
        public override string ToString() {
            string text = HeaderName + "\n";
            object last = new();
            Dictionary<FloatID, List<object>> alignedLists = _elements.GroupBy(l => l is WebpageComponent component ? component.AlignID : FloatID.none).ToDictionary(g => g.Key, g => g.ToList());
            for(FloatID i = FloatID.right; i >= FloatID.none; i--) {
                if (!alignedLists.ContainsKey(i))
                    continue;

                List<object> list = alignedLists[i];
                if (i != FloatID.none)
                    text += $"<div style=\"float:{i}\">\n";

                foreach (object element in list) {
                    text += $"{element}\n";
                }

                if (i != FloatID.none)
                    text += "</div>\n";
            }

            return text;
        }
    }
}
