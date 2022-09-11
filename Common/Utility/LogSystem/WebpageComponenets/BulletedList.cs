﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponEnchantments.Common.Utility.LogSystem.WebpageComponenets
{
	public class BulletedList
    {
        public object[] Elements { private set; get; }
        private bool png;
        private bool _links;
        public BulletedList(bool png = false, bool links = false, params object[] elements) {
            Elements = elements;
            _links = links;
        }
        public override string ToString() {
            string text = "";
            foreach (object element in Elements) {
                text += "* ";
                string elem = element.ToString();
                if (_links) {
                    if (png) {
                        elem = elem.ToItemPNG(link: true);
                    }
                    else {
                        elem = elem.ToLink();
                    }
                }
                else if (png) {
                    elem = elem.ToItemPNG();
                }

                text += elem;

                text += "\n";
            }

            return text;
        }
    }
}