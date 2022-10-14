using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponEnchantments.Common.Utility.LogSystem.WebpageComponenets
{
    public class InfoBox
    {
        private string _label;
        private IEnumerable<string> _headers;
        private bool _firstRowHeaders;
        private bool _sortable;
        private bool _collapsible;
        private bool _collapsed;
        private bool _rowspanColumns;
        private bool _automaticCollapse;
        private List<List<T>> _elements;
        private int _maxWidth;
        public Table(IEnumerable<IEnumerable<T>> elements, IEnumerable<string> headers = null, string label = null, bool firstRowHeaders = false, bool sortable = false, bool collapsible = false, bool collapsed = false, bool rowspanColumns = false, bool automaticCollapse = false, int maxWidth = 0, AlignID alignID = AlignID.none) {
            _elements = elements.Select(e => e.ToList()).ToList();
            _headers = headers;
            _label = label;
            _firstRowHeaders = firstRowHeaders;
            _sortable = sortable;
            _collapsible = collapsible;
            _collapsed = collapsed;
            _rowspanColumns = rowspanColumns;
            _automaticCollapse = automaticCollapse;
            _maxWidth = maxWidth;
            AlignID = alignID;
        }
        public override string ToString() {
            if (_automaticCollapse && _elements.Count >= 10) {
                _sortable = true;
                _collapsible = true;
                _collapsed = true;
            }

            string text = $"{"{"}| class=\"{(_sortable ? "sortable " : "")}{(_collapsible ? "mw-collapsible " : "")}{(_collapsed ? "mw-collapsed " : "")}wikitable\"{(_maxWidth != 0 ? $" style=\"max-width:{_maxWidth}px;\"" : "")}\n";
            List<int> rowspan = Enumerable.Repeat(0, _elements[0].Count).ToList();

            if (_label != null)
                text += $"|+{_label}\n";

            int rowCount = _elements[0].Count;
            if (_headers != null) {
                foreach (string s in _headers) {
                    if (s == "")
                        continue;

                    text += $"!colspan=\"{rowCount}\" style=\"padding: 0px\" |{s}\n" +
                             "|-\n";
                }
            }

            bool first = true;
            bool firstRowHeaders;
            int elementsCount = _elements.Count;
            for (int i = 0; i < elementsCount; i++) {
                if (first) {
                    first = false;
                    firstRowHeaders = _firstRowHeaders;
                }
                else {
                    firstRowHeaders = false;
                    text += "|-\n";
                }

                for (int j = 0; j < _elements[i].Count; j++) {
                    T item = _elements[i][j];
                    bool isRowspanColumn = false;
                    if (!firstRowHeaders && _rowspanColumns && rowspan[j] == 0) {
                        int k = i;
                        while (k < elementsCount) {
                            if (k + 1 == elementsCount)
                                break;

                            T element = _elements[k + 1][j];
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

            text += "|}\n";

            return text;
        }
    }
}
