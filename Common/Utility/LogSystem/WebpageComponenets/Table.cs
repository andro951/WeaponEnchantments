using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponEnchantments.Common.Utility.LogSystem.WebpageComponenets
{
    public class Table<T> where T : class
    {
        public string Header { private set; get; }
        private bool _firstRowIsHeaders;
        private bool _sortable;
        private bool _collapsible;
        private bool _collapsed;
        private bool _rowspanColumns;
        private bool _automaticCollapse;
        public List<List<T>> Elements { private set; get; }
        public Table(IEnumerable<IEnumerable<T>> elements, string header = null, bool firstRowIsHeaders = false, bool sortable = false, bool collapsible = false, bool collapsed = false, bool rowspanColumns = false, bool automaticCollapse = false) {
            Elements = elements.Select(e => e.ToList()).ToList();
            Header = header;
            _firstRowIsHeaders = firstRowIsHeaders;
            _sortable = sortable;
            _collapsible = collapsible;
            _collapsed = collapsed;
            _rowspanColumns = rowspanColumns;
            _automaticCollapse = automaticCollapse;
        }
        public override string ToString() {
            if (_automaticCollapse && Elements.Count >= 10) {
                _sortable = true;
                _collapsible = true;
                _collapsed = true;
            }

            string text = $"{"{"}| class=\"{(_sortable ? "sortable " : "")}{(_collapsible ? "mw-collapsible " : "")}{(_collapsed ? "mw-collapsed " : "")}fandom-table\"\n";
            List<int> rowspan = Enumerable.Repeat(0, Elements[0].Count).ToList();

            if (Header != null)
                text += $"|+{Header}\n";

            bool first = true;
            bool firstRowHeaders;
            int elementsCount = Elements.Count;
            for (int i = 0; i < elementsCount; i++) {
                if (first) {
                    first = false;
                    firstRowHeaders = _firstRowIsHeaders;
                }
                else {
                    firstRowHeaders = false;
                    text += "|-\n";
                }

                for (int j = 0; j < Elements[i].Count; j++) {
                    T item = Elements[i][j];
                    bool isRowspanColumn = false;
                    if (!firstRowHeaders && _rowspanColumns && rowspan[j] == 0) {
                        int k = i;
                        while (k < elementsCount) {
                            if (k + 1 == elementsCount)
                                break;

                            T element = Elements[k + 1][j];
                            if (item.ToString() == element.ToString()) {
                                k++;
                            }
                            else {
                                break;
                            }
                        }

                        if (k > i) {
                            int rowspanNum = k - i;
                            rowspan[j] = rowspanNum;
                            text += $"| rowspan=\"{rowspanNum + 1}\" ";
                            isRowspanColumn = true;
                        }
                    }

                    if (_rowspanColumns && rowspan[j] > 0) {
                        if (!isRowspanColumn) {
                            rowspan[j]--;
                            continue;
                        }
                    }

                    if (firstRowHeaders) {
                        text += "!";
                    }
                    else {
                        text += "|";
                    }

                    text += $"{item}\n";
                }
            }

            text += "|}<br/>\n";

            return text;
        }
    }
}
